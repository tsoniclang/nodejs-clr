import * as ts from "typescript";
import * as fs from "node:fs";
import * as path from "node:path";

type EntrypointName = "index" | "nodejs.Http";

interface NodeExportInfo {
  name: string;
  kind: "function" | "class" | "variable" | "namespace" | "enum" | "unknown";
}

interface NodeModuleApi {
  name: string;
  valueExports: Map<string, NodeExportInfo>;
}

interface TsonicEntrypointApi {
  entrypoint: EntrypointName;
  facadePath: string;
  internalPath: string;
  facadeValueExports: Set<string>;
  facadeTypeExports: Set<string>;
  moduleObjectExportMap: Map<string, string>; // exportedName -> internalSymbolName (e.g. fs -> fs$instance)
  moduleObjects: Map<string, TsonicModuleObjectApi>; // exportedName -> api
}

interface TsonicModuleObjectApi {
  exportedName: string;
  internalSymbolName: string;
  members: Set<string>; // static members on the $instance class
  referencedTypeNames: Set<string>; // identifiers referenced in member signatures
}

interface ModuleMapping {
  nodeModule: string;
  tsonicEntrypoint?: EntrypointName;
  tsonicModuleObjectExport?: string; // e.g. "fs", "net", "performance"
}

interface ModuleDiff {
  nodeModule: string;
  nodeExportCount: number;
  coveredCount: number;
  missing: NodeExportInfo[];
  covered: NodeExportInfo[];
  // For module-object-backed modules only:
  tsonicModuleObjectMembers?: string[];
  tsonicExtraMembers?: string[];
  notes: string[];
}

const DEFAULT_NODE_TYPES_INDEX = path.resolve(__dirname, "../../../node_modules/@types/node/index.d.ts");
const DEFAULT_NODE_TYPES_DIR = path.resolve(__dirname, "../../../node_modules/@types/node");
const DEFAULT_TSONIC_NODEJS_VERSIONS10 = path.resolve(__dirname, "../../../../nodejs/versions/10");
const DEFAULT_NODEJS_CLR_SRC_DIR = path.resolve(__dirname, "../../../src/nodejs");

const IGNORE_NODE_TYPES_FILES = new Set([
  "index",
  "globals",
  "globals.typedarray",
  "buffer.buffer",
  "inspector.generated",
]);

const IGNORE_NODE_TYPES_DIRS = new Set([
  "compatibility",
  "ts5.6",
  "ts5.7",
  "web-globals",
]);

const NODE_TO_TSONIC_MODULE_OBJECT_OVERRIDES: Record<string, { entrypoint: EntrypointName; moduleObjectExport: string }> = {
  // Node's perf_hooks exports a "performance" object; Tsonic exposes it directly.
  perf_hooks: { entrypoint: "index", moduleObjectExport: "performance" },
  // Node's http module lives in nodejs.Http namespace.
  http: { entrypoint: "nodejs.Http", moduleObjectExport: "http" },
};

const NODE_EXPORT_ALIASES: Record<string, Record<string, string>> = {
  // Node: dgram.Socket, Tsonic: DgramSocket
  dgram: { Socket: "DgramSocket" },
  // Node: tls.Server, Tsonic: TLSServer
  tls: { Server: "TLSServer" },
};

function shouldIgnoreNodeMemberName(name: string): boolean {
  // Unique-symbol property keys show up as synthetic names like "__@foo@123".
  if (name.startsWith("__@")) return true;
  if (name.includes("@")) return true;
  // Constructor functions expose prototype; not actionable for API parity.
  if (name === "prototype") return true;
  return false;
}

function readJson<T>(filePath: string): T {
  return JSON.parse(fs.readFileSync(filePath, "utf8")) as T;
}

function unquoteModuleName(name: string): string {
  if (name.startsWith('"') && name.endsWith('"')) return name.slice(1, -1);
  return name;
}

function nodeExportKindFromFlags(flags: ts.SymbolFlags): NodeExportInfo["kind"] {
  if (flags & (ts.SymbolFlags.Function | ts.SymbolFlags.Method)) return "function";
  if (flags & ts.SymbolFlags.Class) return "class";
  if (flags & ts.SymbolFlags.Enum) return "enum";
  if (flags & (ts.SymbolFlags.Variable | ts.SymbolFlags.Property)) return "variable";
  if (flags & ts.SymbolFlags.NamespaceModule) return "namespace";
  return "unknown";
}

function isValueExportSymbol(sym: ts.Symbol, checker: ts.TypeChecker): { isValue: boolean; flags: ts.SymbolFlags } {
  let resolved = sym;
  if (resolved.flags & ts.SymbolFlags.Alias) {
    resolved = checker.getAliasedSymbol(resolved);
  }
  return { isValue: (resolved.flags & ts.SymbolFlags.Value) !== 0, flags: resolved.flags };
}

function tryGetExportEqualsType(moduleSymbol: ts.Symbol, checker: ts.TypeChecker): ts.Type | undefined {
  for (const decl of moduleSymbol.declarations ?? []) {
    if (!ts.isModuleDeclaration(decl)) continue;
    if (!decl.body || !ts.isModuleBlock(decl.body)) continue;

    for (const stmt of decl.body.statements) {
      if (!ts.isExportAssignment(stmt) || !stmt.isExportEquals) continue;
      const targetSymbol = checker.getSymbolAtLocation(stmt.expression);
      if (!targetSymbol) return undefined;

      let resolvedTarget = targetSymbol;
      if (resolvedTarget.flags & ts.SymbolFlags.Alias) resolvedTarget = checker.getAliasedSymbol(resolvedTarget);

      return checker.getTypeOfSymbolAtLocation(resolvedTarget, stmt.expression);
    }
  }

  return undefined;
}

function collectNodeBuiltinModuleNames(nodeTypesDir: string): Set<string> {
  const result = new Set<string>();

  for (const entry of fs.readdirSync(nodeTypesDir, { withFileTypes: true })) {
    if (entry.isFile() && entry.name.endsWith(".d.ts")) {
      const base = entry.name.slice(0, -".d.ts".length);
      if (!IGNORE_NODE_TYPES_FILES.has(base)) result.add(base);
      continue;
    }

    if (entry.isDirectory() && !IGNORE_NODE_TYPES_DIRS.has(entry.name)) {
      const subDir = path.join(nodeTypesDir, entry.name);
      const subFiles = fs.readdirSync(subDir, { withFileTypes: true });
      for (const sub of subFiles) {
        if (!sub.isFile() || !sub.name.endsWith(".d.ts")) continue;
        const base = sub.name.slice(0, -".d.ts".length);
        result.add(`${entry.name}/${base}`);
      }
    }
  }

  return result;
}

function loadNodeApi(nodeTypesIndexPath: string, builtinModules: Set<string>): Map<string, NodeModuleApi> {
  const program = ts.createProgram([nodeTypesIndexPath], {
    target: ts.ScriptTarget.ES2022,
    module: ts.ModuleKind.ESNext,
    skipLibCheck: true,
  });
  const checker = program.getTypeChecker();

  const api = new Map<string, NodeModuleApi>();

  for (const moduleSymbol of checker.getAmbientModules()) {
    const rawName = unquoteModuleName(moduleSymbol.name);
    const baseName = rawName.startsWith("node:") ? rawName.slice("node:".length) : rawName;
    if (!builtinModules.has(baseName)) continue;

    let mod = api.get(baseName);
    if (!mod) {
      mod = { name: baseName, valueExports: new Map() };
      api.set(baseName, mod);
    }

    // Named exports (ESM-style)
    for (const exp of checker.getExportsOfModule(moduleSymbol)) {
      const { isValue, flags } = isValueExportSymbol(exp, checker);
      if (!isValue) continue;
      if (shouldIgnoreNodeMemberName(exp.name)) continue;
      if (!mod.valueExports.has(exp.name)) mod.valueExports.set(exp.name, { name: exp.name, kind: nodeExportKindFromFlags(flags) });
    }

    // export = <value> style (CommonJS): treat members of the exported value as module surface
    const exportEqualsType = tryGetExportEqualsType(moduleSymbol, checker);
    if (exportEqualsType) {
      for (const prop of checker.getPropertiesOfType(exportEqualsType)) {
        const { isValue, flags } = isValueExportSymbol(prop, checker);
        if (!isValue) continue;
        if (shouldIgnoreNodeMemberName(prop.name)) continue;
        if (!mod.valueExports.has(prop.name)) mod.valueExports.set(prop.name, { name: prop.name, kind: nodeExportKindFromFlags(flags) });
      }
    }
  }

  return api;
}

function readSourceFile(filePath: string): ts.SourceFile {
  const text = fs.readFileSync(filePath, "utf8");
  return ts.createSourceFile(filePath, text, ts.ScriptTarget.ES2022, true, ts.ScriptKind.TS);
}

function hasModifier(node: ts.Node, kind: ts.SyntaxKind): boolean {
  const modifiers = (node as { modifiers?: ts.NodeArray<ts.Modifier> }).modifiers;
  return !!modifiers?.some((m) => m.kind === kind);
}

function getPropertyNameText(name: ts.PropertyName | undefined): string | undefined {
  if (!name) return undefined;
  if (ts.isIdentifier(name)) return name.text;
  if (ts.isStringLiteral(name)) return name.text;
  if (ts.isNumericLiteral(name)) return name.text;
  return undefined;
}

function collectEntityNameParts(entity: ts.EntityName, out: Set<string>): void {
  if (ts.isIdentifier(entity)) {
    out.add(entity.text);
    return;
  }
  collectEntityNameParts(entity.left, out);
  out.add(entity.right.text);
}

function collectTypeIdentifiers(typeNode: ts.TypeNode | undefined, out: Set<string>): void {
  if (!typeNode) return;

  if (ts.isTypeReferenceNode(typeNode)) {
    collectEntityNameParts(typeNode.typeName, out);
    typeNode.typeArguments?.forEach((arg) => collectTypeIdentifiers(arg, out));
    return;
  }

  if (ts.isArrayTypeNode(typeNode)) {
    collectTypeIdentifiers(typeNode.elementType, out);
    return;
  }

  if (ts.isUnionTypeNode(typeNode) || ts.isIntersectionTypeNode(typeNode)) {
    typeNode.types.forEach((t) => collectTypeIdentifiers(t, out));
    return;
  }

  if (ts.isTupleTypeNode(typeNode)) {
    typeNode.elements.forEach((t) => collectTypeIdentifiers(t, out));
    return;
  }

  if (ts.isParenthesizedTypeNode(typeNode)) {
    collectTypeIdentifiers(typeNode.type, out);
    return;
  }

  if (ts.isTypeOperatorNode(typeNode)) {
    collectTypeIdentifiers(typeNode.type, out);
    return;
  }

  if (ts.isIndexedAccessTypeNode(typeNode)) {
    collectTypeIdentifiers(typeNode.objectType, out);
    collectTypeIdentifiers(typeNode.indexType, out);
    return;
  }

  if (ts.isConditionalTypeNode(typeNode)) {
    collectTypeIdentifiers(typeNode.checkType, out);
    collectTypeIdentifiers(typeNode.extendsType, out);
    collectTypeIdentifiers(typeNode.trueType, out);
    collectTypeIdentifiers(typeNode.falseType, out);
    return;
  }

  if (ts.isFunctionTypeNode(typeNode)) {
    typeNode.parameters.forEach((p) => collectTypeIdentifiers(p.type, out));
    collectTypeIdentifiers(typeNode.type, out);
    return;
  }

  if (ts.isTypeLiteralNode(typeNode) || ts.isMappedTypeNode(typeNode) || ts.isImportTypeNode(typeNode) || ts.isTypeQueryNode(typeNode)) {
    return;
  }

  // Keyword / literal / template literal / etc: ignore.
}

function parseTsonicEntrypoint(entrypoint: EntrypointName, facadePath: string, internalPath: string): TsonicEntrypointApi {
  const facade = readSourceFile(facadePath);

  const facadeValueExports = new Set<string>();
  const facadeTypeExports = new Set<string>();
  const moduleObjectExportMap = new Map<string, string>();

  for (const stmt of facade.statements) {
    if (ts.isExportDeclaration(stmt) && stmt.exportClause && ts.isNamedExports(stmt.exportClause)) {
      for (const spec of stmt.exportClause.elements) {
        const exportedName = spec.name.text;
        const localName = spec.propertyName?.text ?? spec.name.text;
        facadeValueExports.add(exportedName);
        if (localName.endsWith("$instance")) {
          moduleObjectExportMap.set(exportedName, localName);
        }
      }
      continue;
    }

    if (ts.isTypeAliasDeclaration(stmt) && hasModifier(stmt, ts.SyntaxKind.ExportKeyword)) {
      facadeTypeExports.add(stmt.name.text);
      continue;
    }
  }

  const internal = readSourceFile(internalPath);
  const classesByName = new Map<string, ts.ClassDeclaration>();
  for (const stmt of internal.statements) {
    if (ts.isClassDeclaration(stmt) && stmt.name) {
      classesByName.set(stmt.name.text, stmt);
    }
  }

  const moduleObjects = new Map<string, TsonicModuleObjectApi>();
  for (const [exportedName, internalSymbolName] of moduleObjectExportMap) {
    const classDecl = classesByName.get(internalSymbolName);
    if (!classDecl) continue;

    const members = new Set<string>();
    const referencedTypeNames = new Set<string>();

    for (const member of classDecl.members) {
      if (!hasModifier(member, ts.SyntaxKind.StaticKeyword)) continue;

      if (
        ts.isMethodDeclaration(member) ||
        ts.isPropertyDeclaration(member) ||
        ts.isGetAccessorDeclaration(member) ||
        ts.isSetAccessorDeclaration(member)
      ) {
        const nameText = getPropertyNameText(member.name);
        if (nameText) members.add(nameText);

        if (ts.isMethodDeclaration(member) || ts.isGetAccessorDeclaration(member) || ts.isSetAccessorDeclaration(member)) {
          member.parameters.forEach((p) => collectTypeIdentifiers(p.type, referencedTypeNames));
          collectTypeIdentifiers(member.type, referencedTypeNames);
        }

        if (ts.isPropertyDeclaration(member)) {
          collectTypeIdentifiers(member.type, referencedTypeNames);
        }
      }
    }

    moduleObjects.set(exportedName, { exportedName, internalSymbolName, members, referencedTypeNames });
  }

  return {
    entrypoint,
    facadePath,
    internalPath,
    facadeValueExports,
    facadeTypeExports,
    moduleObjectExportMap,
    moduleObjects,
  };
}

function scanPublicCSharpTypes(directoryPath: string): Set<string> {
  const results = new Set<string>();

  const entries = fs.readdirSync(directoryPath, { withFileTypes: true });
  for (const entry of entries) {
    if (!entry.isFile() || !entry.name.endsWith(".cs")) continue;
    const filePath = path.join(directoryPath, entry.name);
    const text = fs.readFileSync(filePath, "utf8");

    const typeRe =
      /\bpublic\s+(?:static\s+)?(?:abstract\s+)?(?:sealed\s+)?(?:partial\s+)?(?:class|interface|struct|record)\s+([A-Za-z_][A-Za-z0-9_]*)\b/g;
    const delegateRe = /\bpublic\s+delegate\s+[^\s]+\s+([A-Za-z_][A-Za-z0-9_]*)\b/g;

    for (const re of [typeRe, delegateRe]) {
      let match: RegExpExecArray | null;
      while ((match = re.exec(text))) {
        results.add(match[1]);
      }
    }
  }

  return results;
}

function scanSourceTypesByModule(sourceModulesDir: string): Map<string, Set<string>> {
  const modules = new Map<string, Set<string>>();
  if (!fs.existsSync(sourceModulesDir)) return modules;

  for (const entry of fs.readdirSync(sourceModulesDir, { withFileTypes: true })) {
    if (!entry.isDirectory()) continue;
    const dirPath = path.join(sourceModulesDir, entry.name);
    modules.set(entry.name, scanPublicCSharpTypes(dirPath));
  }

  return modules;
}

function buildModuleMappings(nodeModules: string[], tsonicIndex: TsonicEntrypointApi, tsonicHttp: TsonicEntrypointApi): ModuleMapping[] {
  const mappings: ModuleMapping[] = [];

  for (const nodeModule of nodeModules) {
    const override = NODE_TO_TSONIC_MODULE_OBJECT_OVERRIDES[nodeModule];
    if (override) {
      mappings.push({ nodeModule, tsonicEntrypoint: override.entrypoint, tsonicModuleObjectExport: override.moduleObjectExport });
      continue;
    }

    if (tsonicIndex.moduleObjects.has(nodeModule)) {
      mappings.push({ nodeModule, tsonicEntrypoint: "index", tsonicModuleObjectExport: nodeModule });
      continue;
    }

    if (tsonicHttp.moduleObjects.has(nodeModule)) {
      mappings.push({ nodeModule, tsonicEntrypoint: "nodejs.Http", tsonicModuleObjectExport: nodeModule });
      continue;
    }

    // No module object. Still allow "leaf" compatibility via exported types/classes.
    if (nodeModule === "buffer" || nodeModule === "events" || nodeModule === "string_decoder" || nodeModule === "url") {
      mappings.push({ nodeModule, tsonicEntrypoint: "index" });
      continue;
    }

    mappings.push({ nodeModule });
  }

  return mappings;
}

function computeModuleDiff(
  nodeApi: NodeModuleApi | undefined,
  mapping: ModuleMapping,
  tsonicIndex: TsonicEntrypointApi,
  tsonicHttp: TsonicEntrypointApi,
  sourceTypesByModule: Map<string, Set<string>>,
): ModuleDiff {
  const notes: string[] = [];
  const nodeExports = nodeApi ? Array.from(nodeApi.valueExports.values()) : [];
  nodeExports.sort((a, b) => a.name.localeCompare(b.name));

  const nodeExportCount = nodeExports.length;

  const hasTsonicMapping = !!mapping.tsonicEntrypoint;
  const entry = mapping.tsonicEntrypoint === "nodejs.Http" ? tsonicHttp : tsonicIndex;

  let moduleObject: TsonicModuleObjectApi | undefined;
  if (hasTsonicMapping && mapping.tsonicModuleObjectExport) {
    moduleObject = entry.moduleObjects.get(mapping.tsonicModuleObjectExport);
    if (!moduleObject) {
      notes.push(`Tsonic module object '${mapping.tsonicModuleObjectExport}' not found in ${entry.entrypoint}.`);
    }
  }

  const relatedPublicTypes = new Set<string>();
  if (hasTsonicMapping) {
    const moduleDirName = mapping.nodeModule.includes("/") ? undefined : mapping.nodeModule;
    const sourceTypes = moduleDirName ? sourceTypesByModule.get(moduleDirName) : undefined;

    for (const typeName of sourceTypes ?? []) {
      if (entry.facadeValueExports.has(typeName)) relatedPublicTypes.add(typeName);
    }

    if (moduleObject) {
      for (const typeName of moduleObject.referencedTypeNames) {
        if (entry.facadeValueExports.has(typeName)) relatedPublicTypes.add(typeName);
      }
    }
  }

  const aliases = NODE_EXPORT_ALIASES[mapping.nodeModule] ?? {};

  const covered: NodeExportInfo[] = [];
  const missing: NodeExportInfo[] = [];

  const moduleMemberNames = moduleObject ? moduleObject.members : new Set<string>();

  for (const exp of nodeExports) {
    const aliasTarget = aliases[exp.name];
    const isCovered =
      (aliasTarget && relatedPublicTypes.has(aliasTarget)) ||
      (moduleObject && mapping.tsonicModuleObjectExport && exp.name === mapping.tsonicModuleObjectExport) ||
      moduleMemberNames.has(exp.name) ||
      relatedPublicTypes.has(exp.name) ||
      (!moduleObject && hasTsonicMapping ? relatedPublicTypes.has(exp.name) : false);

    if (isCovered) covered.push(exp);
    else missing.push(exp);
  }

  const coveredCount = covered.length;

  const diff: ModuleDiff = {
    nodeModule: mapping.nodeModule,
    nodeExportCount,
    coveredCount,
    missing,
    covered,
    notes,
  };

  if (moduleObject) {
    diff.tsonicModuleObjectMembers = Array.from(moduleObject.members).sort();

    const nodeExportNames = new Set(nodeExports.map((e) => e.name));
    diff.tsonicExtraMembers = diff.tsonicModuleObjectMembers.filter((m) => !nodeExportNames.has(m));
  }

  if (!nodeApi) {
    diff.notes.push("No Node.js type definition module found.");
  }

  return diff;
}

function formatPercent(n: number): string {
  return (n * 100).toFixed(n >= 0.1 ? 1 : 2) + "%";
}

function writeReportFiles(
  outDir: string,
  nodeTypesVersion: string,
  tsonicNodejsVersion: string,
  nodeModules: string[],
  diffs: ModuleDiff[],
): void {
  const generatedAt = new Date().toISOString();

  const missingModules = diffs.filter((d) => d.coveredCount === 0).map((d) => d.nodeModule).sort();
  const partialOrImplemented = diffs.filter((d) => d.coveredCount > 0).slice();
  partialOrImplemented.sort((a, b) => (a.coveredCount / Math.max(a.nodeExportCount, 1)) - (b.coveredCount / Math.max(b.nodeExportCount, 1)));

  const summaryLines: string[] = [];
  summaryLines.push("# Node.js API Gap Summary (Node @types vs @tsonic/nodejs)");
  summaryLines.push("");
  summaryLines.push(`Generated: ${generatedAt}`);
  summaryLines.push(`Node types: @types/node@${nodeTypesVersion}`);
  summaryLines.push(`Tsonic types: @tsonic/nodejs@${tsonicNodejsVersion}`);
  summaryLines.push("");
  summaryLines.push("## Coverage Overview");
  summaryLines.push("");
  summaryLines.push(`- Node core modules considered: **${nodeModules.length}**`);
  summaryLines.push(`- Modules with any coverage: **${partialOrImplemented.length}**`);
  summaryLines.push(`- Modules with zero coverage: **${missingModules.length}**`);
  summaryLines.push("");

  if (missingModules.length > 0) {
    summaryLines.push("## Missing Modules (zero coverage)");
    summaryLines.push("");
    summaryLines.push(missingModules.map((m) => `- \`${m}\``).join("\n"));
    summaryLines.push("");
  }

  summaryLines.push("## Lowest Coverage Modules");
  summaryLines.push("");
  summaryLines.push("| Module | Covered / Node exports | Coverage |");
  summaryLines.push("|---|---:|---:|");
  for (const d of partialOrImplemented.slice(0, 15)) {
    const denom = Math.max(d.nodeExportCount, 1);
    const pct = d.coveredCount / denom;
    summaryLines.push(`| \`${d.nodeModule}\` | ${d.coveredCount} / ${d.nodeExportCount} | ${formatPercent(pct)} |`);
  }
  summaryLines.push("");
  summaryLines.push("Full per-module details: `tools/verification-report.md`");
  summaryLines.push("");

  const fullLines: string[] = [];
  fullLines.push("# Node.js API Gap Report (Node @types vs @tsonic/nodejs)");
  fullLines.push("");
  fullLines.push(`Generated: ${generatedAt}`);
  fullLines.push(`Node types: @types/node@${nodeTypesVersion}`);
  fullLines.push(`Tsonic types: @tsonic/nodejs@${tsonicNodejsVersion}`);
  fullLines.push("");
  fullLines.push("## Methodology / Limitations");
  fullLines.push("");
  fullLines.push("- Compares **names only** (value exports), not signatures/overloads.");
  fullLines.push("- For Tsonic modules implemented as `$instance` static classes, counts:");
  fullLines.push("  - static members on the module object (e.g. `fs.readFile`), and");
  fullLines.push("  - publicly exported types referenced by those signatures (to avoid cross-module name collisions like `Socket`).");
  fullLines.push("- Tsonic does not support Node builtin specifiers (`\"fs\"`, `\"node:fs\"`, ...); this is a compatibility analysis of the **API surface**.");
  fullLines.push("");

  fullLines.push("## Module Coverage Table");
  fullLines.push("");
  fullLines.push("| Module | Covered / Node exports | Coverage |");
  fullLines.push("|---|---:|---:|");
  const sortedByName = diffs.slice().sort((a, b) => a.nodeModule.localeCompare(b.nodeModule));
  for (const d of sortedByName) {
    const denom = Math.max(d.nodeExportCount, 1);
    const pct = d.coveredCount / denom;
    fullLines.push(`| \`${d.nodeModule}\` | ${d.coveredCount} / ${d.nodeExportCount} | ${formatPercent(pct)} |`);
  }
  fullLines.push("");

  fullLines.push("## Per-module Details");
  fullLines.push("");

  for (const d of partialOrImplemented) {
    const denom = Math.max(d.nodeExportCount, 1);
    const pct = d.coveredCount / denom;

    fullLines.push(`### ${d.nodeModule}`);
    fullLines.push("");
    fullLines.push(`- Coverage: **${d.coveredCount} / ${d.nodeExportCount}** (${formatPercent(pct)})`);

    if (d.tsonicExtraMembers && d.tsonicExtraMembers.length > 0) {
      fullLines.push(`- Tsonic-only module members: **${d.tsonicExtraMembers.length}**`);
    }

    if (d.notes.length > 0) {
      for (const note of d.notes) fullLines.push(`- Note: ${note}`);
    }

    if (Object.keys(NODE_EXPORT_ALIASES[d.nodeModule] ?? {}).length > 0) {
      const aliases = NODE_EXPORT_ALIASES[d.nodeModule];
      fullLines.push(`- Alias handling: ${Object.entries(aliases).map(([from, to]) => `\`${from}\`→\`${to}\``).join(", ")}`);
    }

    if (d.tsonicExtraMembers && d.tsonicExtraMembers.length > 0) {
      const shown = d.tsonicExtraMembers.slice(0, 80);
      fullLines.push("");
      fullLines.push(`#### Tsonic-only module members (${d.tsonicExtraMembers.length})`);
      fullLines.push("");
      fullLines.push(
        `- ${shown.map((name) => `\`${name}\``).join(", ")}${d.tsonicExtraMembers.length > shown.length ? `, … (+${d.tsonicExtraMembers.length - shown.length} more)` : ""}`,
      );
    }

    if (d.missing.length > 0) {
      const missingByKind = new Map<NodeExportInfo["kind"], string[]>();
      for (const exp of d.missing) {
        const list = missingByKind.get(exp.kind) ?? [];
        list.push(exp.name);
        missingByKind.set(exp.kind, list);
      }

      fullLines.push("");
      fullLines.push(`#### Missing exports (${d.missing.length})`);
      fullLines.push("");
      for (const kind of ["function", "class", "variable", "namespace", "enum", "unknown"] as const) {
        const names = missingByKind.get(kind);
        if (!names || names.length === 0) continue;
        names.sort();
        const shown = names.slice(0, 80);
        fullLines.push(`- ${kind}: ${shown.map((n) => `\`${n}\``).join(", ")}${names.length > shown.length ? `, … (+${names.length - shown.length} more)` : ""}`);
      }
    }

    fullLines.push("");
  }

  const summaryPath = path.join(outDir, "VERIFICATION-SUMMARY.md");
  const fullPath = path.join(outDir, "verification-report.md");
  fs.writeFileSync(summaryPath, summaryLines.join("\n"), "utf8");
  fs.writeFileSync(fullPath, fullLines.join("\n"), "utf8");
}

function main() {
  const nodeTypesIndex = process.argv.includes("--node-types")
    ? process.argv[process.argv.indexOf("--node-types") + 1]
    : DEFAULT_NODE_TYPES_INDEX;

  const tsonicNodejsDir = process.argv.includes("--tsonic-nodejs")
    ? process.argv[process.argv.indexOf("--tsonic-nodejs") + 1]
    : DEFAULT_TSONIC_NODEJS_VERSIONS10;

  const nodeTypesDir = path.dirname(nodeTypesIndex);

  if (!fs.existsSync(nodeTypesIndex)) {
    throw new Error(`Node types index not found: ${nodeTypesIndex}`);
  }

  const tsonicIndexFacade = path.join(tsonicNodejsDir, "index.d.ts");
  const tsonicIndexInternal = path.join(tsonicNodejsDir, "index", "internal", "index.d.ts");
  const tsonicHttpFacade = path.join(tsonicNodejsDir, "nodejs.Http.d.ts");
  const tsonicHttpInternal = path.join(tsonicNodejsDir, "nodejs.Http", "internal", "index.d.ts");

  for (const p of [tsonicIndexFacade, tsonicIndexInternal, tsonicHttpFacade, tsonicHttpInternal]) {
    if (!fs.existsSync(p)) {
      throw new Error(`Tsonic @tsonic/nodejs declarations not found (expected): ${p}`);
    }
  }

  const nodeTypesPkg = readJson<{ version: string }>(path.join(nodeTypesDir, "package.json"));
  const tsonicPkg = readJson<{ version: string }>(path.join(tsonicNodejsDir, "package.json"));

  const builtinModules = collectNodeBuiltinModuleNames(nodeTypesDir);
  const nodeApi = loadNodeApi(nodeTypesIndex, builtinModules);
  const nodeModules = Array.from(builtinModules).sort();

  const tsonicIndex = parseTsonicEntrypoint("index", tsonicIndexFacade, tsonicIndexInternal);
  const tsonicHttp = parseTsonicEntrypoint("nodejs.Http", tsonicHttpFacade, tsonicHttpInternal);

  const sourceTypesByModule = scanSourceTypesByModule(DEFAULT_NODEJS_CLR_SRC_DIR);

  const mappings = buildModuleMappings(nodeModules, tsonicIndex, tsonicHttp);

  const diffs: ModuleDiff[] = [];
  for (const m of mappings) {
    diffs.push(computeModuleDiff(nodeApi.get(m.nodeModule), m, tsonicIndex, tsonicHttp, sourceTypesByModule));
  }

  const outDir = path.resolve(__dirname, "../.."); // tools/
  writeReportFiles(outDir, nodeTypesPkg.version, tsonicPkg.version, nodeModules, diffs);

  console.log(`Wrote reports:\n- ${path.join(outDir, "VERIFICATION-SUMMARY.md")}\n- ${path.join(outDir, "verification-report.md")}`);
}

main();
