import * as ts from 'typescript';
import * as fs from 'fs';
import * as path from 'path';

interface TsonicApi {
  Modules: Record<string, ModuleDefinition>;
}

interface ModuleDefinition {
  Name: string;
  IsClass: boolean;
  IsStatic: boolean;
  Methods: MethodSignature[];
  Properties: PropertySignature[];
}

interface MethodSignature {
  Name: string;
  IsStatic: boolean;
  ReturnType: string;
  Parameters: ParameterSignature[];
}

interface ParameterSignature {
  Name: string;
  Type: string;
  IsOptional: boolean;
  DefaultValue?: string;
}

interface PropertySignature {
  Name: string;
  Type: string;
  IsStatic: boolean;
  CanRead: boolean;
  CanWrite: boolean;
}

interface NodeModule {
  name: string;
  functions: Map<string, FunctionInfo>;
  classes: Map<string, ClassInfo>;
}

interface FunctionInfo {
  name: string;
  signatures: SignatureInfo[];
}

interface SignatureInfo {
  parameters: ParamInfo[];
  returnType: string;
}

interface ParamInfo {
  name: string;
  type: string;
  optional: boolean;
}

interface ClassInfo {
  name: string;
  methods: Map<string, FunctionInfo>;
  properties: Map<string, PropertyInfo>;
}

interface PropertyInfo {
  name: string;
  type: string;
  readonly: boolean;
}

function parseNodeTypes(moduleName: string): NodeModule | null {
  const typesPath = path.join(__dirname, `../../node_modules/@types/node/${moduleName}.d.ts`);

  if (!fs.existsSync(typesPath)) {
    return null;
  }

  const program = ts.createProgram([typesPath], {
    target: ts.ScriptTarget.ES2020,
    module: ts.ModuleKind.CommonJS,
  });

  const checker = program.getTypeChecker();
  const sourceFile = program.getSourceFile(typesPath);

  if (!sourceFile) {
    return null;
  }

  const nodeModule: NodeModule = {
    name: moduleName,
    functions: new Map(),
    classes: new Map(),
  };

  function visit(node: ts.Node) {
    // Look for declare module statements
    if (ts.isModuleDeclaration(node) && node.name.text === moduleName) {
      if (node.body && ts.isModuleBlock(node.body)) {
        node.body.statements.forEach(statement => {
          if (ts.isFunctionDeclaration(statement) && statement.name) {
            const funcInfo = extractFunctionInfo(statement, checker);
            if (funcInfo) {
              nodeModule.functions.set(funcInfo.name, funcInfo);
            }
          } else if (ts.isClassDeclaration(statement) && statement.name) {
            const classInfo = extractClassInfo(statement, checker);
            if (classInfo) {
              nodeModule.classes.set(classInfo.name, classInfo);
            }
          }
        });
      }
    }

    ts.forEachChild(node, visit);
  }

  visit(sourceFile);
  return nodeModule;
}

function extractFunctionInfo(node: ts.FunctionDeclaration, checker: ts.TypeChecker): FunctionInfo | null {
  if (!node.name) return null;

  const signatures: SignatureInfo[] = [];

  const params: ParamInfo[] = node.parameters.map(param => ({
    name: param.name.getText(),
    type: param.type ? param.type.getText() : 'any',
    optional: !!param.questionToken || !!param.initializer,
  }));

  const returnType = node.type ? node.type.getText() : 'void';

  signatures.push({
    parameters: params,
    returnType,
  });

  return {
    name: node.name.text,
    signatures,
  };
}

function extractClassInfo(node: ts.ClassDeclaration, checker: ts.TypeChecker): ClassInfo | null {
  if (!node.name) return null;

  const classInfo: ClassInfo = {
    name: node.name.text,
    methods: new Map(),
    properties: new Map(),
  };

  node.members.forEach(member => {
    if (ts.isMethodDeclaration(member) && member.name) {
      const methodInfo = extractMethodInfo(member, checker);
      if (methodInfo) {
        classInfo.methods.set(methodInfo.name, methodInfo);
      }
    } else if (ts.isPropertyDeclaration(member) && member.name) {
      const propInfo = extractPropertyInfo(member);
      if (propInfo) {
        classInfo.properties.set(propInfo.name, propInfo);
      }
    }
  });

  return classInfo;
}

function extractMethodInfo(node: ts.MethodDeclaration, checker: ts.TypeChecker): FunctionInfo | null {
  const name = node.name.getText();

  const params: ParamInfo[] = node.parameters.map(param => ({
    name: param.name.getText(),
    type: param.type ? param.type.getText() : 'any',
    optional: !!param.questionToken || !!param.initializer,
  }));

  const returnType = node.type ? node.type.getText() : 'void';

  return {
    name,
    signatures: [{
      parameters: params,
      returnType,
    }],
  };
}

function extractPropertyInfo(node: ts.PropertyDeclaration): PropertyInfo | null {
  const name = node.name.getText();
  const type = node.type ? node.type.getText() : 'any';
  const readonly = node.modifiers?.some(m => m.kind === ts.SyntaxKind.ReadonlyKeyword) || false;

  return {
    name,
    type,
    readonly,
  };
}

function compareApis(tsonicApi: TsonicApi, nodeModules: Map<string, NodeModule>) {
  const report: string[] = [];
  report.push('# API Verification Report\n');
  report.push(`Generated: ${new Date().toISOString()}\n`);

  // Map Tsonic module names to Node module names (lowercase)
  const moduleNameMap: Record<string, string> = {
    'path': 'path',
    'fs': 'fs',
    'crypto': 'crypto',
    'buffer': 'buffer',
    'events': 'events',
    // Add more mappings as needed
  };

  for (const [tsonicName, tsonicModule] of Object.entries(tsonicApi.Modules)) {
    const nodeName = moduleNameMap[tsonicName.toLowerCase()];

    if (!nodeName) {
      continue; // Skip modules we don't map
    }

    const nodeModule = nodeModules.get(nodeName);

    if (!nodeModule) {
      report.push(`\n## ⚠️  ${tsonicName} - No Node.js definition found\n`);
      continue;
    }

    report.push(`\n## ${tsonicName}\n`);

    // For static classes (like path), compare methods as module-level functions
    if (tsonicModule.IsStatic) {
      for (const method of tsonicModule.Methods) {
        const nodeFunc = nodeModule.functions.get(method.Name);

        if (!nodeFunc) {
          report.push(`❌ **${method.Name}** - Missing in Node.js\n`);
          continue;
        }

        // Simple comparison - just check if function exists
        report.push(`✅ **${method.Name}** - Present\n`);
      }
    }

    // Compare classes
    for (const tsonicClass of Object.values(tsonicApi.Modules).filter(m => m.Name === tsonicName && !m.IsStatic)) {
      const nodeClass = nodeModule.classes.get(tsonicClass.Name);

      if (!nodeClass) {
        report.push(`❌ **class ${tsonicClass.Name}** - Missing in Node.js\n`);
        continue;
      }

      report.push(`✅ **class ${tsonicClass.Name}** - Present\n`);

      // Compare methods
      for (const method of tsonicClass.Methods) {
        const nodeMethod = nodeClass.methods.get(method.Name);

        if (!nodeMethod) {
          report.push(`  ❌ **${method.Name}()** - Missing in Node.js\n`);
        } else {
          report.push(`  ✅ **${method.Name}()** - Present\n`);
        }
      }

      // Compare properties
      for (const prop of tsonicClass.Properties) {
        const nodeProp = nodeClass.properties.get(prop.Name);

        if (!nodeProp) {
          report.push(`  ❌ **${prop.Name}** - Missing in Node.js\n`);
        } else {
          report.push(`  ✅ **${prop.Name}** - Present\n`);
        }
      }
    }
  }

  return report.join('');
}

function main() {
  // Load Tsonic API from JSON
  const tsonicApiPath = path.join(__dirname, '../nodejs-clr-api.json');
  const tsonicApi: TsonicApi = JSON.parse(fs.readFileSync(tsonicApiPath, 'utf-8'));

  // Parse Node.js type definitions
  const nodeModules = new Map<string, NodeModule>();
  const modulesToCheck = ['path', 'fs', 'crypto', 'buffer', 'events'];

  for (const moduleName of modulesToCheck) {
    const nodeModule = parseNodeTypes(moduleName);
    if (nodeModule) {
      nodeModules.set(moduleName, nodeModule);
    }
  }

  // Compare and generate report
  const report = compareApis(tsonicApi, nodeModules);

  // Write report
  const reportPath = path.join(__dirname, '../verification-report.md');
  fs.writeFileSync(reportPath, report);

  console.log(`Verification report generated: ${reportPath}`);
  console.log('\n' + report);
}

main();
