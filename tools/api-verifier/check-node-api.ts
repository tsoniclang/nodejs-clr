import * as fs from "node:fs";
import * as path from "node:path";
import { execFileSync } from "node:child_process";
import * as ts from "typescript";

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
  moduleObjectExportMap: Map<string, string>;
  moduleObjects: Map<string, TsonicModuleObjectApi>;
}

interface TsonicModuleObjectApi {
  exportedName: string;
  internalSymbolName: string;
  members: Set<string>;
  referencedTypeNames: Set<string>;
}

interface ModuleMapping {
  nodeModule: string;
  tsonicEntrypoint?: EntrypointName;
  tsonicModuleObjectExport?: string;
}

interface ModuleDiff {
  nodeModule: string;
  nodeExportCount: number;
  coveredCount: number;
  missing: NodeExportInfo[];
  covered: NodeExportInfo[];
  tsonicModuleObjectMembers?: string[];
  tsonicExtraMembers?: string[];
  notes: string[];
}

interface AliasModuleApi {
  moduleName: string;
  valueExports: Set<string>;
  typeExports: Set<string>;
}

interface ModuleBindingInfo {
  moduleName: string;
  assembly: string;
  type: string;
}

interface ConsistencyIssue {
  severity: "error" | "warning";
  scope: string;
  message: string;
}

interface VerificationOutput {
  generatedAt: string;
  nodeTypesVersion: string;
  tsonicNodejsVersion: string;
  moduleCount: number;
  modulesWithCoverage: number;
  modulesWithZeroCoverage: number;
  consistencyErrors: ConsistencyIssue[];
  consistencyWarnings: ConsistencyIssue[];
  diffs: Array<{
    nodeModule: string;
    nodeExportCount: number;
    coveredCount: number;
    missing: NodeExportInfo[];
    covered: NodeExportInfo[];
    notes: string[];
    tsonicExtraMembers: string[];
  }>;
}

const DEFAULT_NODE_TYPES_INDEX = path.resolve(__dirname, "../../node_modules/@types/node/index.d.ts");
const DEFAULT_NODE_TYPES_DIR = path.resolve(__dirname, "../../node_modules/@types/node");
const DEFAULT_TSONIC_NODEJS_VERSIONS10 = path.resolve(__dirname, "../../../nodejs/versions/10");
const DEFAULT_NODEJS_CLR_SRC_DIR = path.resolve(__dirname, "../../src/nodejs");

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
  perf_hooks: { entrypoint: "index", moduleObjectExport: "performance" },
  http: { entrypoint: "nodejs.Http", moduleObjectExport: "http" },
};

const NODE_EXPORT_ALIASES: Record<string, Record<string, string>> = {
  dgram: { Socket: "DgramSocket" },
  tls: { Server: "TLSServer" },
};

function readJson<T>(filePath: string): T {
  return JSON.parse(fs.readFileSync(filePath, "utf8")) as T;
}

function unquoteModuleName(name: string): string {
  if (name.startsWith('"') && name.endsWith('"')) return name.slice(1, -1);
  return name;
}

function shouldIgnoreNodeMemberName(name: string): boolean {
  if (name.startsWith("__@")) return true;
  if (name.includes("@")) return true;
  if (name === "prototype") return true;
  return false;
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
  if (resolved.flags & ts.SymbolFlags.Alias) resolved = checker.getAliasedSymbol(resolved);
  return { isValue: (resolved.flags & ts.SymbolFlags.Value) !== 0, flags: resolved.flags };
}

function tryGetExportEqualsType(moduleSymbol: ts.Symbol, checker: ts.TypeChecker): ts.Type | undefined {
  for (const decl of moduleSymbol.declarations ?? []) {
    if (!ts.isModuleDeclaration(decl) || !decl.body || !ts.isModuleBlock(decl.body)) continue;
    for (const stmt of decl.body.statements) {
      if (!ts.isExportAssignment(stmt) || !stmt.isExportEquals) continue;
      const targetSymbol = checker.getSymbolAtLocation(stmt.expression);
      if (!targetSymbol) return undefined;
      const resolved = targetSymbol.flags & ts.SymbolFlags.Alias ? checker.getAliasedSymbol(targetSymbol) : targetSymbol;
      return checker.getTypeOfSymbolAtLocation(resolved, stmt.expression);
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

    if (!entry.isDirectory() || IGNORE_NODE_TYPES_DIRS.has(entry.name)) continue;

    const subDir = path.join(nodeTypesDir, entry.name);
    for (const sub of fs.readdirSync(subDir, { withFileTypes: true })) {
      if (!sub.isFile() || !sub.name.endsWith(".d.ts")) continue;
      const base = sub.name.slice(0, -".d.ts".length);
      result.add(`${entry.name}/${base}`);
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

    for (const exp of checker.getExportsOfModule(moduleSymbol)) {
      const { isValue, flags } = isValueExportSymbol(exp, checker);
      if (!isValue || shouldIgnoreNodeMemberName(exp.name)) continue;
      if (!mod.valueExports.has(exp.name)) mod.valueExports.set(exp.name, { name: exp.name, kind: nodeExportKindFromFlags(flags) });
    }

    const exportEqualsType = tryGetExportEqualsType(moduleSymbol, checker);
    if (!exportEqualsType) continue;

    for (const prop of checker.getPropertiesOfType(exportEqualsType)) {
      const { isValue, flags } = isValueExportSymbol(prop, checker);
      if (!isValue || shouldIgnoreNodeMemberName(prop.name)) continue;
      if (!mod.valueExports.has(prop.name)) mod.valueExports.set(prop.name, { name: prop.name, kind: nodeExportKindFromFlags(flags) });
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
  return !!modifiers?.some((modifier) => modifier.kind === kind);
}

function getPropertyNameText(name: ts.PropertyName | undefined): string | undefined {
  if (!name) return undefined;
  if (ts.isIdentifier(name) || ts.isStringLiteral(name) || ts.isNumericLiteral(name)) return name.text;
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
    typeNode.types.forEach((node) => collectTypeIdentifiers(node, out));
    return;
  }

  if (ts.isTupleTypeNode(typeNode)) {
    typeNode.elements.forEach((node) => collectTypeIdentifiers(node, out));
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
    typeNode.parameters.forEach((parameter) => collectTypeIdentifiers(parameter.type, out));
    collectTypeIdentifiers(typeNode.type, out);
  }
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
        if (stmt.isTypeOnly) facadeTypeExports.add(exportedName);
        else facadeValueExports.add(exportedName);
        if (!stmt.isTypeOnly && localName.endsWith("$instance")) moduleObjectExportMap.set(exportedName, localName);
      }
      continue;
    }

    if (ts.isTypeAliasDeclaration(stmt) && hasModifier(stmt, ts.SyntaxKind.ExportKeyword)) {
      facadeTypeExports.add(stmt.name.text);
      continue;
    }

    if (
      (ts.isClassDeclaration(stmt) ||
        ts.isInterfaceDeclaration(stmt) ||
        ts.isEnumDeclaration(stmt) ||
        ts.isFunctionDeclaration(stmt)) &&
      hasModifier(stmt, ts.SyntaxKind.ExportKeyword) &&
      stmt.name
    ) {
      if (ts.isInterfaceDeclaration(stmt)) facadeTypeExports.add(stmt.name.text);
      else facadeValueExports.add(stmt.name.text);
    }
  }

  const internal = readSourceFile(internalPath);
  const classesByName = new Map<string, ts.ClassDeclaration>();
  for (const stmt of internal.statements) {
    if (ts.isClassDeclaration(stmt) && stmt.name) classesByName.set(stmt.name.text, stmt);
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
          member.parameters.forEach((parameter) => collectTypeIdentifiers(parameter.type, referencedTypeNames));
          collectTypeIdentifiers(member.type, referencedTypeNames);
        }

        if (ts.isPropertyDeclaration(member)) collectTypeIdentifiers(member.type, referencedTypeNames);
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
  for (const entry of fs.readdirSync(directoryPath, { withFileTypes: true })) {
    if (!entry.isFile() || !entry.name.endsWith(".cs")) continue;

    const filePath = path.join(directoryPath, entry.name);
    const text = fs.readFileSync(filePath, "utf8");

    const typeRe =
      /\bpublic\s+(?:static\s+)?(?:abstract\s+)?(?:sealed\s+)?(?:partial\s+)?(?:class|interface|struct|record)\s+([A-Za-z_][A-Za-z0-9_]*)\b/g;
    const delegateRe = /\bpublic\s+delegate\s+[^\s]+\s+([A-Za-z_][A-Za-z0-9_]*)\b/g;

    for (const re of [typeRe, delegateRe]) {
      let match: RegExpExecArray | null;
      while ((match = re.exec(text))) results.add(match[1]);
    }
  }
  return results;
}

function scanSourceTypesByModule(sourceModulesDir: string): Map<string, Set<string>> {
  const modules = new Map<string, Set<string>>();
  if (!fs.existsSync(sourceModulesDir)) return modules;

  for (const entry of fs.readdirSync(sourceModulesDir, { withFileTypes: true })) {
    if (!entry.isDirectory()) continue;
    modules.set(entry.name, scanPublicCSharpTypes(path.join(sourceModulesDir, entry.name)));
  }

  return modules;
}

function parseAliasModules(aliasFilePath: string): Map<string, AliasModuleApi> {
  const source = readSourceFile(aliasFilePath);
  const modules = new Map<string, AliasModuleApi>();

  for (const stmt of source.statements) {
    if (!ts.isModuleDeclaration(stmt) || !ts.isStringLiteral(stmt.name) || !stmt.body || !ts.isModuleBlock(stmt.body)) continue;

    const api: AliasModuleApi = {
      moduleName: stmt.name.text,
      valueExports: new Set<string>(),
      typeExports: new Set<string>(),
    };

    for (const moduleStmt of stmt.body.statements) {
      if (ts.isExportDeclaration(moduleStmt) && moduleStmt.exportClause && ts.isNamedExports(moduleStmt.exportClause)) {
        for (const spec of moduleStmt.exportClause.elements) {
          if (moduleStmt.isTypeOnly) api.typeExports.add(spec.name.text);
          else api.valueExports.add(spec.name.text);
        }
        continue;
      }

      if (ts.isVariableStatement(moduleStmt) && hasModifier(moduleStmt, ts.SyntaxKind.ExportKeyword)) {
        for (const decl of moduleStmt.declarationList.declarations) {
          if (ts.isIdentifier(decl.name)) api.valueExports.add(decl.name.text);
        }
        continue;
      }

      if (ts.isFunctionDeclaration(moduleStmt) && hasModifier(moduleStmt, ts.SyntaxKind.ExportKeyword) && moduleStmt.name) {
        api.valueExports.add(moduleStmt.name.text);
        continue;
      }

      if (ts.isClassDeclaration(moduleStmt) && hasModifier(moduleStmt, ts.SyntaxKind.ExportKeyword) && moduleStmt.name) {
        api.valueExports.add(moduleStmt.name.text);
        continue;
      }

      if (
        (ts.isInterfaceDeclaration(moduleStmt) || ts.isTypeAliasDeclaration(moduleStmt)) &&
        hasModifier(moduleStmt, ts.SyntaxKind.ExportKeyword) &&
        moduleStmt.name
      ) {
        api.typeExports.add(moduleStmt.name.text);
      }
    }

    modules.set(api.moduleName, api);
  }

  return modules;
}

function loadModuleBindings(bindingsPath: string): Map<string, ModuleBindingInfo> {
  const bindingsJson = readJson<{ bindings: Record<string, { kind?: string; assembly: string; type: string }> }>(bindingsPath);
  const modules = new Map<string, ModuleBindingInfo>();

  for (const [moduleName, binding] of Object.entries(bindingsJson.bindings ?? {})) {
    modules.set(moduleName, { moduleName, assembly: binding.assembly, type: binding.type });
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
  const nodeExports = nodeApi ? Array.from(nodeApi.valueExports.values()).sort((a, b) => a.name.localeCompare(b.name)) : [];
  const nodeExportCount = nodeExports.length;

  const hasTsonicMapping = !!mapping.tsonicEntrypoint;
  const entry = mapping.tsonicEntrypoint === "nodejs.Http" ? tsonicHttp : tsonicIndex;

  let moduleObject: TsonicModuleObjectApi | undefined;
  if (hasTsonicMapping && mapping.tsonicModuleObjectExport) {
    moduleObject = entry.moduleObjects.get(mapping.tsonicModuleObjectExport);
    if (!moduleObject) notes.push(`Tsonic module object '${mapping.tsonicModuleObjectExport}' not found in ${entry.entrypoint}.`);
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

  const diff: ModuleDiff = {
    nodeModule: mapping.nodeModule,
    nodeExportCount,
    coveredCount: covered.length,
    missing,
    covered,
    notes,
  };

  if (moduleObject) {
    diff.tsonicModuleObjectMembers = Array.from(moduleObject.members).sort();
    const nodeExportNames = new Set(nodeExports.map((exp) => exp.name));
    diff.tsonicExtraMembers = diff.tsonicModuleObjectMembers.filter((member) => !nodeExportNames.has(member));
  }

  if (!nodeApi) diff.notes.push("No Node.js type definition module found.");
  return diff;
}

function compareSets(left: Set<string>, right: Set<string>): { onlyLeft: string[]; onlyRight: string[] } {
  const onlyLeft = Array.from(left).filter((item) => !right.has(item)).sort();
  const onlyRight = Array.from(right).filter((item) => !left.has(item)).sort();
  return { onlyLeft, onlyRight };
}

function buildConsistencyIssues(
  nodeApi: Map<string, NodeModuleApi>,
  aliasModules: Map<string, AliasModuleApi>,
  bindings: Map<string, ModuleBindingInfo>,
): ConsistencyIssue[] {
  const issues: ConsistencyIssue[] = [];
  const baseModules = new Set<string>();

  for (const moduleName of aliasModules.keys()) {
    baseModules.add(moduleName.startsWith("node:") ? moduleName.slice("node:".length) : moduleName);
  }

  for (const baseModule of Array.from(baseModules).sort()) {
    const nodeSpecifier = `node:${baseModule}`;
    const nodeAlias = aliasModules.get(nodeSpecifier);
    const bareAlias = aliasModules.get(baseModule);
    const nodeBinding = bindings.get(nodeSpecifier);
    const bareBinding = bindings.get(baseModule);

    if (!nodeAlias) issues.push({ severity: "error", scope: nodeSpecifier, message: "Missing module declaration in node-aliases.d.ts." });
    if (!bareAlias) issues.push({ severity: "error", scope: baseModule, message: "Missing bare alias module declaration in node-aliases.d.ts." });
    if (!nodeBinding) issues.push({ severity: "error", scope: nodeSpecifier, message: "Missing module binding in bindings.json." });
    if (!bareBinding) issues.push({ severity: "error", scope: baseModule, message: "Missing bare alias module binding in bindings.json." });

    if (nodeAlias && bareAlias) {
      const valueDiff = compareSets(nodeAlias.valueExports, bareAlias.valueExports);
      const typeDiff = compareSets(nodeAlias.typeExports, bareAlias.typeExports);
      if (valueDiff.onlyLeft.length > 0 || valueDiff.onlyRight.length > 0) {
        issues.push({
          severity: "error",
          scope: baseModule,
          message: `Bare/module-specifier value exports diverge. only node:${valueDiff.onlyLeft.join(", ") || "-"}; only bare:${valueDiff.onlyRight.join(", ") || "-"}.`,
        });
      }
      if (typeDiff.onlyLeft.length > 0 || typeDiff.onlyRight.length > 0) {
        issues.push({
          severity: "error",
          scope: baseModule,
          message: `Bare/module-specifier type exports diverge. only node:${typeDiff.onlyLeft.join(", ") || "-"}; only bare:${typeDiff.onlyRight.join(", ") || "-"}.`,
        });
      }
    }

    if (nodeBinding && bareBinding && (nodeBinding.assembly !== bareBinding.assembly || nodeBinding.type !== bareBinding.type)) {
      issues.push({
        severity: "error",
        scope: baseModule,
        message: `Bare/module-specifier bindings diverge: node:${nodeBinding.assembly}/${nodeBinding.type} vs bare:${bareBinding.assembly}/${bareBinding.type}.`,
      });
    }

    const nodeModule = nodeApi.get(baseModule);
    if (!nodeModule) {
      issues.push({ severity: "warning", scope: baseModule, message: "No builtin Node module definition found in @types/node for this alias." });
      continue;
    }

    if (nodeAlias) {
      const allowedExtraExports = new Set<string>([baseModule]);
      const unexpectedAliasOnlyExports = compareSets(nodeAlias.valueExports, new Set(nodeModule.valueExports.keys())).onlyLeft.filter(
        (name) => !allowedExtraExports.has(name),
      );
      if (unexpectedAliasOnlyExports.length > 0) {
        issues.push({
          severity: "warning",
          scope: nodeSpecifier,
          message: `Alias exports names not present in @types/node: ${unexpectedAliasOnlyExports.join(", ")}.`,
        });
      }
    }
  }

  for (const binding of bindings.values()) {
    if (!binding.moduleName.includes(":") && !aliasModules.has(binding.moduleName)) {
      issues.push({
        severity: "warning",
        scope: binding.moduleName,
        message: "bindings.json contains a module entry with no matching declaration in node-aliases.d.ts.",
      });
    }
  }

  return issues;
}

function formatPercent(value: number): string {
  return `${(value * 100).toFixed(value >= 0.1 ? 1 : 2)}%`;
}

function writeReportFiles(
  outDir: string,
  nodeTypesVersion: string,
  tsonicNodejsVersion: string,
  nodeModules: string[],
  diffs: ModuleDiff[],
  issues: ConsistencyIssue[],
): void {
  const generatedAt = new Date().toISOString();
  const errors = issues.filter((issue) => issue.severity === "error");
  const warnings = issues.filter((issue) => issue.severity === "warning");
  const missingModules = diffs.filter((diff) => diff.coveredCount === 0).map((diff) => diff.nodeModule).sort();
  const partialOrImplemented = diffs
    .filter((diff) => diff.coveredCount > 0)
    .sort((left, right) => left.coveredCount / Math.max(left.nodeExportCount, 1) - right.coveredCount / Math.max(right.nodeExportCount, 1));

  const summaryLines: string[] = [
    "# Node.js API Verification Summary",
    "",
    `Generated: ${generatedAt}`,
    `Node types: @types/node@${nodeTypesVersion}`,
    `Tsonic types: @tsonic/nodejs@${tsonicNodejsVersion}`,
    "",
    "## Internal Consistency",
    "",
    `- Errors: **${errors.length}**`,
    `- Warnings: **${warnings.length}**`,
    "",
  ];

  if (errors.length > 0) {
    summaryLines.push("### Errors", "");
    for (const issue of errors) summaryLines.push(`- [${issue.scope}] ${issue.message}`);
    summaryLines.push("");
  }

  if (warnings.length > 0) {
    summaryLines.push("### Warnings", "");
    for (const issue of warnings.slice(0, 20)) summaryLines.push(`- [${issue.scope}] ${issue.message}`);
    if (warnings.length > 20) summaryLines.push(`- ... ${warnings.length - 20} more warnings`);
    summaryLines.push("");
  }

  summaryLines.push("## Coverage Overview", "");
  summaryLines.push(`- Node core modules considered: **${nodeModules.length}**`);
  summaryLines.push(`- Modules with any coverage: **${partialOrImplemented.length}**`);
  summaryLines.push(`- Modules with zero coverage: **${missingModules.length}**`);
  summaryLines.push("");

  if (missingModules.length > 0) {
    summaryLines.push("## Missing Modules (zero coverage)", "");
    summaryLines.push(missingModules.map((moduleName) => `- \`${moduleName}\``).join("\n"));
    summaryLines.push("");
  }

  summaryLines.push("## Lowest Coverage Modules", "");
  summaryLines.push("| Module | Covered / Node exports | Coverage |");
  summaryLines.push("|---|---:|---:|");
  for (const diff of partialOrImplemented.slice(0, 15)) {
    const percent = diff.coveredCount / Math.max(diff.nodeExportCount, 1);
    summaryLines.push(`| \`${diff.nodeModule}\` | ${diff.coveredCount} / ${diff.nodeExportCount} | ${formatPercent(percent)} |`);
  }
  summaryLines.push("");
  summaryLines.push("Full details: `tools/verification-report.md`");
  summaryLines.push("");

  const fullLines: string[] = [
    "# Node.js API Verification Report",
    "",
    `Generated: ${generatedAt}`,
    `Node types: @types/node@${nodeTypesVersion}`,
    `Tsonic types: @tsonic/nodejs@${tsonicNodejsVersion}`,
    "",
    "## What This Checks",
    "",
    "- Internal consistency between generated module declarations and generated bindings.",
    "- Bare specifier parity (`\"fs\"`) with `node:` specifier parity (`\"node:fs\"`).",
    "- Name-level API coverage of `@tsonic/nodejs` against `@types/node`.",
    "- Public C# source types referenced by generated module-object APIs.",
    "",
    "## Limits",
    "",
    "- Coverage is name-based, not overload/signature-semantic parity.",
    "- Non-public C# implementation details are intentionally ignored.",
    "- Missing Node coverage is reported, but does not fail this script unless it creates an internal contract break.",
    "",
    "## Internal Consistency Findings",
    "",
  ];

  if (issues.length === 0) {
    fullLines.push("- No consistency errors or warnings.");
  } else {
    for (const issue of issues) fullLines.push(`- ${issue.severity.toUpperCase()} [${issue.scope}] ${issue.message}`);
  }
  fullLines.push("");

  fullLines.push("## Module Coverage Table", "");
  fullLines.push("| Module | Covered / Node exports | Coverage |");
  fullLines.push("|---|---:|---:|");
  for (const diff of diffs.slice().sort((left, right) => left.nodeModule.localeCompare(right.nodeModule))) {
    const percent = diff.coveredCount / Math.max(diff.nodeExportCount, 1);
    fullLines.push(`| \`${diff.nodeModule}\` | ${diff.coveredCount} / ${diff.nodeExportCount} | ${formatPercent(percent)} |`);
  }
  fullLines.push("", "## Per-module Details", "");

  for (const diff of partialOrImplemented) {
    const percent = diff.coveredCount / Math.max(diff.nodeExportCount, 1);
    fullLines.push(`### ${diff.nodeModule}`, "");
    fullLines.push(`- Coverage: **${diff.coveredCount} / ${diff.nodeExportCount}** (${formatPercent(percent)})`);

    if (diff.notes.length > 0) for (const note of diff.notes) fullLines.push(`- Note: ${note}`);

    const aliasMap = NODE_EXPORT_ALIASES[diff.nodeModule];
    if (aliasMap) fullLines.push(`- Alias handling: ${Object.entries(aliasMap).map(([from, to]) => `\`${from}\`→\`${to}\``).join(", ")}`);

    if (diff.tsonicExtraMembers && diff.tsonicExtraMembers.length > 0) {
      fullLines.push("", `#### Tsonic-only module members (${diff.tsonicExtraMembers.length})`, "");
      fullLines.push(`- ${diff.tsonicExtraMembers.slice(0, 80).map((name) => `\`${name}\``).join(", ")}${diff.tsonicExtraMembers.length > 80 ? `, … (+${diff.tsonicExtraMembers.length - 80} more)` : ""}`);
    }

    if (diff.missing.length > 0) {
      const missingByKind = new Map<NodeExportInfo["kind"], string[]>();
      for (const exp of diff.missing) {
        const names = missingByKind.get(exp.kind) ?? [];
        names.push(exp.name);
        missingByKind.set(exp.kind, names);
      }

      fullLines.push("", `#### Missing exports (${diff.missing.length})`, "");
      for (const kind of ["function", "class", "variable", "namespace", "enum", "unknown"] as const) {
        const names = missingByKind.get(kind);
        if (!names || names.length === 0) continue;
        names.sort();
        fullLines.push(`- ${kind}: ${names.slice(0, 80).map((name) => `\`${name}\``).join(", ")}${names.length > 80 ? `, … (+${names.length - 80} more)` : ""}`);
      }
    }

    fullLines.push("");
  }

  const json: VerificationOutput = {
    generatedAt,
    nodeTypesVersion,
    tsonicNodejsVersion,
    moduleCount: nodeModules.length,
    modulesWithCoverage: partialOrImplemented.length,
    modulesWithZeroCoverage: missingModules.length,
    consistencyErrors: errors,
    consistencyWarnings: warnings,
    diffs: diffs.map((diff) => ({
      nodeModule: diff.nodeModule,
      nodeExportCount: diff.nodeExportCount,
      coveredCount: diff.coveredCount,
      missing: diff.missing,
      covered: diff.covered,
      notes: diff.notes,
      tsonicExtraMembers: diff.tsonicExtraMembers ?? [],
    })),
  };

  fs.writeFileSync(path.join(outDir, "VERIFICATION-SUMMARY.md"), `${summaryLines.join("\n")}\n`, "utf8");
  fs.writeFileSync(path.join(outDir, "verification-report.md"), `${fullLines.join("\n")}\n`, "utf8");
  fs.writeFileSync(path.join(outDir, "verification-report.json"), `${JSON.stringify(json, null, 2)}\n`, "utf8");
}

function getArgValue(flag: string): string | undefined {
  const index = process.argv.indexOf(flag);
  return index >= 0 ? process.argv[index + 1] : undefined;
}

function ensureGeneratedPackage(requiredFiles: string[], tsonicNodejsDir: string): void {
  if (requiredFiles.every((filePath) => fs.existsSync(filePath))) return;

  const dotnetMajor = path.basename(tsonicNodejsDir);
  const nodejsRepoDir = path.resolve(tsonicNodejsDir, "..", "..");
  const packageJsonPath = path.join(nodejsRepoDir, "package.json");

  if (!fs.existsSync(packageJsonPath)) {
    throw new Error(`Required generated files are missing and nodejs repo root could not be inferred: ${nodejsRepoDir}`);
  }

  execFileSync("npm", ["run", `generate:${dotnetMajor}`], {
    cwd: nodejsRepoDir,
    stdio: "inherit",
  });

  for (const filePath of requiredFiles) {
    if (!fs.existsSync(filePath)) throw new Error(`Required generated file not found after regeneration: ${filePath}`);
  }
}

function main(): void {
  const nodeTypesIndex = getArgValue("--node-types") ?? DEFAULT_NODE_TYPES_INDEX;
  const tsonicNodejsDir = getArgValue("--tsonic-nodejs") ?? DEFAULT_TSONIC_NODEJS_VERSIONS10;
  const nodejsClrSrcDir = getArgValue("--nodejs-clr-src") ?? DEFAULT_NODEJS_CLR_SRC_DIR;
  const outDir = getArgValue("--out-dir") ?? path.resolve(__dirname, "..");

  if (!fs.existsSync(nodeTypesIndex)) throw new Error(`Node types index not found: ${nodeTypesIndex}`);

  const requiredFiles = [
    path.join(tsonicNodejsDir, "index.d.ts"),
    path.join(tsonicNodejsDir, "index", "internal", "index.d.ts"),
    path.join(tsonicNodejsDir, "nodejs.Http.d.ts"),
    path.join(tsonicNodejsDir, "nodejs.Http", "internal", "index.d.ts"),
    path.join(tsonicNodejsDir, "node-aliases.d.ts"),
    path.join(tsonicNodejsDir, "bindings.json"),
  ];
  ensureGeneratedPackage(requiredFiles, tsonicNodejsDir);

  const nodeTypesDir = path.dirname(nodeTypesIndex);
  const nodeTypesPkg = readJson<{ version: string }>(path.join(nodeTypesDir, "package.json"));
  const tsonicPkg = readJson<{ version: string }>(path.join(tsonicNodejsDir, "package.json"));

  const builtinModules = collectNodeBuiltinModuleNames(nodeTypesDir);
  const nodeApi = loadNodeApi(nodeTypesIndex, builtinModules);
  const nodeModules = Array.from(builtinModules).sort();

  const tsonicIndex = parseTsonicEntrypoint(
    "index",
    path.join(tsonicNodejsDir, "index.d.ts"),
    path.join(tsonicNodejsDir, "index", "internal", "index.d.ts"),
  );
  const tsonicHttp = parseTsonicEntrypoint(
    "nodejs.Http",
    path.join(tsonicNodejsDir, "nodejs.Http.d.ts"),
    path.join(tsonicNodejsDir, "nodejs.Http", "internal", "index.d.ts"),
  );

  const aliasModules = parseAliasModules(path.join(tsonicNodejsDir, "node-aliases.d.ts"));
  const bindings = loadModuleBindings(path.join(tsonicNodejsDir, "bindings.json"));
  const sourceTypesByModule = scanSourceTypesByModule(nodejsClrSrcDir);
  const mappings = buildModuleMappings(nodeModules, tsonicIndex, tsonicHttp);
  const diffs = mappings.map((mapping) => computeModuleDiff(nodeApi.get(mapping.nodeModule), mapping, tsonicIndex, tsonicHttp, sourceTypesByModule));
  const issues = buildConsistencyIssues(nodeApi, aliasModules, bindings);

  writeReportFiles(outDir, nodeTypesPkg.version, tsonicPkg.version, nodeModules, diffs, issues);

  const errors = issues.filter((issue) => issue.severity === "error");
  const warnings = issues.filter((issue) => issue.severity === "warning");
  console.log(`Node API verification complete.`);
  console.log(`- Coverage report: ${path.join(outDir, "verification-report.md")}`);
  console.log(`- Summary: ${path.join(outDir, "VERIFICATION-SUMMARY.md")}`);
  console.log(`- JSON: ${path.join(outDir, "verification-report.json")}`);
  console.log(`- Consistency errors: ${errors.length}`);
  console.log(`- Consistency warnings: ${warnings.length}`);

  if (errors.length > 0) {
    for (const issue of errors) console.error(`ERROR [${issue.scope}] ${issue.message}`);
    process.exit(1);
  }
}

main();
