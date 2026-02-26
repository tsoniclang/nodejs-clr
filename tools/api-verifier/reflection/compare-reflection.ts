import * as fs from "node:fs";
import * as path from "node:path";

interface ClrApi {
  Modules: Record<string, ClrModuleDefinition>;
}

interface ClrModuleDefinition {
  Name: string;
  Namespace?: string;
  IsClass: boolean;
  IsStatic: boolean;
  Methods: ClrMethodSignature[];
  Properties: ClrPropertySignature[];
}

interface ClrMethodSignature {
  Name: string;
}

interface ClrPropertySignature {
  Name: string;
}

interface RuntimeApiSnapshot {
  generatedAt: string;
  nodeVersion: string;
  modules: Record<string, RuntimeModuleApi>;
  loadErrors: Record<string, string>;
}

interface RuntimeModuleApi {
  name: string;
  exports: Record<string, RuntimeExportInfo>;
}

interface RuntimeExportInfo {
  name: string;
  kind: "function" | "class" | "object" | "primitive" | "accessor";
}

interface ModuleCoverage {
  moduleName: string;
  nodeExportCount: number;
  coveredCount: number;
  coveragePercent: number;
  coveredExports: readonly string[];
  missingExports: readonly string[];
  extraClrMembers: readonly string[];
  notes: readonly string[];
}

const CLR_API_PATH = path.resolve(__dirname, "../../nodejs-clr-api.json");
const NODE_RUNTIME_PATH = path.resolve(__dirname, "../../node-runtime-api.json");
const OUTPUT_REPORT_PATH = path.resolve(__dirname, "../../reflection-coverage-report.md");

const CLR_MODULE_NAME_OVERRIDES: Record<string, string> = {
  performance: "perf_hooks",
};

const NODE_CLASS_NAME_ALIASES: Record<string, Record<string, string>> = {
  dgram: { Socket: "DgramSocket" },
  tls: { Server: "TLSServer" },
};

const NODE_EXPORTS_TO_IGNORE: ReadonlySet<string> = new Set([
  "default",
  "module",
]);

function readJson<T>(filePath: string): T {
  return JSON.parse(fs.readFileSync(filePath, "utf8")) as T;
}

function normalizeClrModuleName(moduleDef: ClrModuleDefinition): string | undefined {
  if (!moduleDef.IsStatic) return undefined;

  const explicitOverride = CLR_MODULE_NAME_OVERRIDES[moduleDef.Name];
  if (explicitOverride) return explicitOverride;

  const namespaceName = moduleDef.Namespace ?? "nodejs";
  if (namespaceName === "nodejs.Http" && moduleDef.Name === "http") {
    return "http";
  }

  return moduleDef.Name;
}

function collectClrStaticModuleMembers(clrApi: ClrApi): Map<string, Set<string>> {
  const modules = new Map<string, Set<string>>();

  for (const moduleDef of Object.values(clrApi.Modules)) {
    const moduleName = normalizeClrModuleName(moduleDef);
    if (!moduleName) continue;

    const memberSet = modules.get(moduleName) ?? new Set<string>();
    for (const method of moduleDef.Methods) memberSet.add(method.Name);
    for (const property of moduleDef.Properties) memberSet.add(property.Name);
    modules.set(moduleName, memberSet);
  }

  return modules;
}

function collectClrTypeNames(clrApi: ClrApi): ReadonlySet<string> {
  return new Set(Object.values(clrApi.Modules).map((x) => x.Name));
}

function getNodeExports(moduleApi: RuntimeModuleApi): readonly RuntimeExportInfo[] {
  return Object.values(moduleApi.exports)
    .filter((entry) => !NODE_EXPORTS_TO_IGNORE.has(entry.name))
    .sort((left, right) => left.name.localeCompare(right.name));
}

function classAlias(moduleName: string, exportName: string): string {
  return NODE_CLASS_NAME_ALIASES[moduleName]?.[exportName] ?? exportName;
}

function compareModule(
  moduleName: string,
  nodeModule: RuntimeModuleApi,
  clrMembers: ReadonlySet<string>,
  clrTypeNames: ReadonlySet<string>
): ModuleCoverage {
  const nodeExports = getNodeExports(nodeModule);
  const coveredExports: string[] = [];
  const missingExports: string[] = [];

  for (const entry of nodeExports) {
    const coveredByMember = clrMembers.has(entry.name);
    const coveredByType = entry.kind === "class" && clrTypeNames.has(classAlias(moduleName, entry.name));

    if (coveredByMember || coveredByType) {
      coveredExports.push(entry.name);
    } else {
      missingExports.push(entry.name);
    }
  }

  const nodeExportNames = new Set(nodeExports.map((entry) => entry.name));
  const extraClrMembers = Array.from(clrMembers)
    .filter((name) => !nodeExportNames.has(name))
    .sort((left, right) => left.localeCompare(right));

  const notes: string[] = [];
  if (missingExports.length > 0) {
    notes.push("Missing exports are runtime surface differences (names only).");
  }
  if (extraClrMembers.length > 0) {
    notes.push("Extra CLR members may be intentional convenience APIs.");
  }

  return {
    moduleName,
    nodeExportCount: nodeExports.length,
    coveredCount: coveredExports.length,
    coveragePercent: nodeExports.length === 0 ? 100 : (coveredExports.length / nodeExports.length) * 100,
    coveredExports,
    missingExports,
    extraClrMembers,
    notes,
  };
}

function formatCoverageTable(rows: readonly ModuleCoverage[]): readonly string[] {
  const lines: string[] = [];
  lines.push("| Module | Covered / Node exports | Coverage |");
  lines.push("|---|---:|---:|");
  for (const row of rows) {
    lines.push(`| \`${row.moduleName}\` | ${row.coveredCount} / ${row.nodeExportCount} | ${row.coveragePercent.toFixed(1)}% |`);
  }
  return lines;
}

function main(): void {
  const clrApi = readJson<ClrApi>(CLR_API_PATH);
  const nodeRuntime = readJson<RuntimeApiSnapshot>(NODE_RUNTIME_PATH);

  const clrModules = collectClrStaticModuleMembers(clrApi);
  const clrTypeNames = collectClrTypeNames(clrApi);

  const moduleRows = Object.entries(nodeRuntime.modules)
    .map(([moduleName, nodeModule]) => {
      const members = clrModules.get(moduleName) ?? new Set<string>();
      return compareModule(moduleName, nodeModule, members, clrTypeNames);
    })
    .sort((left, right) => left.coveragePercent - right.coveragePercent || left.moduleName.localeCompare(right.moduleName));

  const report: string[] = [];
  report.push("# Reflection Coverage Report (Node runtime vs nodejs-clr reflection)");
  report.push("");
  report.push(`Generated: ${new Date().toISOString()}`);
  report.push(`Node runtime snapshot: ${nodeRuntime.generatedAt}`);
  report.push(`Node version: ${nodeRuntime.nodeVersion}`);
  report.push("");
  report.push("## Scope");
  report.push("");
  report.push("- Node side: runtime reflection of builtin modules.");
  report.push("- CLR side: reflection over public static module types in the `nodejs` assembly.");
  report.push("- Comparison is name-level API coverage (not overload/type compatibility).");
  report.push("");
  report.push("## Coverage Table");
  report.push("");
  report.push(...formatCoverageTable(moduleRows));
  report.push("");

  const loadErrors = Object.entries(nodeRuntime.loadErrors).sort((left, right) => left[0].localeCompare(right[0]));
  if (loadErrors.length > 0) {
    report.push("## Node Module Load Errors");
    report.push("");
    for (const [moduleName, error] of loadErrors) {
      report.push(`- \`${moduleName}\`: ${error}`);
    }
    report.push("");
  }

  report.push("## Per-module Gaps");
  report.push("");

  for (const row of moduleRows) {
    if (row.missingExports.length === 0 && row.extraClrMembers.length === 0) continue;

    report.push(`### ${row.moduleName}`);
    report.push("");
    report.push(`- Coverage: **${row.coveredCount} / ${row.nodeExportCount}** (${row.coveragePercent.toFixed(1)}%)`);
    if (row.notes.length > 0) {
      for (const note of row.notes) report.push(`- Note: ${note}`);
    }
    report.push("");

    if (row.missingExports.length > 0) {
      report.push(`#### Missing exports (${row.missingExports.length})`);
      report.push("");
      report.push(`- ${row.missingExports.map((name) => `\`${name}\``).join(", ")}`);
      report.push("");
    }

    if (row.extraClrMembers.length > 0) {
      report.push(`#### CLR-only members (${row.extraClrMembers.length})`);
      report.push("");
      report.push(`- ${row.extraClrMembers.map((name) => `\`${name}\``).join(", ")}`);
      report.push("");
    }
  }

  fs.writeFileSync(OUTPUT_REPORT_PATH, report.join("\n"));

  console.log(`Wrote reflection comparison report: ${OUTPUT_REPORT_PATH}`);
  console.log(`Modules compared: ${moduleRows.length}`);
  console.log(`Node load errors: ${loadErrors.length}`);
}

main();
