import * as fs from "node:fs";
import * as path from "node:path";
import { builtinModules } from "node:module";

type ExportKind = "function" | "class" | "object" | "primitive" | "accessor";

interface RuntimeExportInfo {
  name: string;
  kind: ExportKind;
  arity?: number;
  staticMembers?: string[];
  instanceMembers?: string[];
}

interface RuntimeModuleApi {
  name: string;
  exports: Record<string, RuntimeExportInfo>;
}

interface RuntimeApiSnapshot {
  generatedAt: string;
  nodeVersion: string;
  modules: Record<string, RuntimeModuleApi>;
  loadErrors: Record<string, string>;
}

const OUTPUT_PATH = path.resolve(__dirname, "../../node-runtime-api.json");

const INTERNAL_MODULE_EXACT = new Set([
  "sys",
  "inspector/promises",
  "test/reporters",
]);

const INTERNAL_MODULE_PREFIX = [
  "_",
  "internal/",
];

const NON_EXPORTABLE_PROPERTY_NAMES = new Set([
  "__esModule",
  "length",
  "name",
  "prototype",
  "arguments",
  "caller",
]);

function normalizeBuiltinName(name: string): string {
  return name.startsWith("node:") ? name.slice("node:".length) : name;
}

function isInternalBuiltin(name: string): boolean {
  if (INTERNAL_MODULE_EXACT.has(name)) return true;
  return INTERNAL_MODULE_PREFIX.some((prefix) => name.startsWith(prefix));
}

function getBuiltinModuleNames(): readonly string[] {
  const deduped = new Set<string>();

  for (const entry of builtinModules) {
    const normalized = normalizeBuiltinName(entry);
    if (isInternalBuiltin(normalized)) continue;
    deduped.add(normalized);
  }

  return Array.from(deduped).sort((a, b) => a.localeCompare(b));
}

function getSortedNames(value: object): readonly string[] {
  const names = new Set<string>();

  for (const key of Object.keys(value)) names.add(key);
  for (const key of Object.getOwnPropertyNames(value)) names.add(key);

  return Array.from(names)
    .filter((name) => !NON_EXPORTABLE_PROPERTY_NAMES.has(name))
    .sort((a, b) => a.localeCompare(b));
}

function getFunctionMemberNames(value: object): readonly string[] {
  return Object.getOwnPropertyNames(value)
    .filter((name) => !NON_EXPORTABLE_PROPERTY_NAMES.has(name))
    .filter((name) => {
      const descriptor = Object.getOwnPropertyDescriptor(value, name);
      return !!descriptor && "value" in descriptor && typeof descriptor.value === "function";
    })
    .sort((a, b) => a.localeCompare(b));
}

function isClassLike(fn: Function): boolean {
  const source = Function.prototype.toString.call(fn).trimStart();
  if (source.startsWith("class ")) return true;

  const prototype = fn.prototype;
  if (!prototype || typeof prototype !== "object") return false;

  const instanceMembers = getFunctionMemberNames(prototype).filter((name) => name !== "constructor");
  return instanceMembers.length > 0;
}

function describeExport(exportName: string, descriptor: PropertyDescriptor): RuntimeExportInfo {
  if (!("value" in descriptor)) {
    return { name: exportName, kind: "accessor" };
  }

  const value = descriptor.value;

  if (typeof value === "function") {
    const classLike = isClassLike(value);

    if (classLike) {
      const staticMembers = getFunctionMemberNames(value);
      const instanceMembers = value.prototype && typeof value.prototype === "object"
        ? getFunctionMemberNames(value.prototype).filter((name) => name !== "constructor")
        : [];

      return {
        name: exportName,
        kind: "class",
        arity: value.length,
        staticMembers,
        instanceMembers,
      };
    }

    return {
      name: exportName,
      kind: "function",
      arity: value.length,
      staticMembers: getFunctionMemberNames(value),
    };
  }

  if (value !== null && typeof value === "object") {
    return { name: exportName, kind: "object" };
  }

  return { name: exportName, kind: "primitive" };
}

function extractModuleApi(moduleName: string): RuntimeModuleApi {
  const loaded = require(`node:${moduleName}`) as unknown;
  const exportsRecord: Record<string, RuntimeExportInfo> = {};

  if (loaded === null || loaded === undefined) {
    return { name: moduleName, exports: exportsRecord };
  }

  if (typeof loaded !== "object" && typeof loaded !== "function") {
    exportsRecord.default = {
      name: "default",
      kind: "primitive",
    };
    return { name: moduleName, exports: exportsRecord };
  }

  for (const exportName of getSortedNames(loaded)) {
    const descriptor = Object.getOwnPropertyDescriptor(loaded, exportName);
    if (!descriptor) continue;
    exportsRecord[exportName] = describeExport(exportName, descriptor);
  }

  return { name: moduleName, exports: exportsRecord };
}

function main(): void {
  const snapshot: RuntimeApiSnapshot = {
    generatedAt: new Date().toISOString(),
    nodeVersion: process.version,
    modules: {},
    loadErrors: {},
  };

  for (const moduleName of getBuiltinModuleNames()) {
    try {
      snapshot.modules[moduleName] = extractModuleApi(moduleName);
    } catch (error) {
      const message = error instanceof Error ? error.message : String(error);
      snapshot.loadErrors[moduleName] = message;
    }
  }

  fs.writeFileSync(OUTPUT_PATH, JSON.stringify(snapshot, null, 2));
  console.log(`Wrote runtime API snapshot to ${OUTPUT_PATH}`);
  console.log(`Modules extracted: ${Object.keys(snapshot.modules).length}`);
  console.log(`Module load errors: ${Object.keys(snapshot.loadErrors).length}`);
}

main();
