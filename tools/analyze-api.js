const fs = require('fs');
const path = require('path');

const api = JSON.parse(fs.readFileSync(path.join(__dirname, 'nodejs-clr-api.json'), 'utf-8'));

const report = [];

report.push('# COMPREHENSIVE TSONIC.NODE API ANALYSIS REPORT');
report.push(`Generated: ${new Date().toISOString()}`);
report.push(`\nTotal Types/Modules: ${Object.keys(api.Modules).length}\n`);

// Categorize modules
const staticModules = {};
const instanceClasses = {};
const dataTypes = {};

for (const [name, module] of Object.entries(api.Modules)) {
  if (module.IsStatic && module.Methods.length > 0) {
    staticModules[name] = module;
  } else if (!module.IsStatic && (module.Methods.length > 0 || module.Properties.length > 0)) {
    instanceClasses[name] = module;
  } else {
    dataTypes[name] = module;
  }
}

// ============= STATIC MODULES =============
report.push('---\n');
report.push('## PART 1: STATIC MODULES (Node.js-style APIs)\n');
report.push(`Found ${Object.keys(staticModules).length} static modules\n`);

const sortedStatic = Object.entries(staticModules).sort((a, b) => b[1].Methods.length - a[1].Methods.length);

for (const [name, module] of sortedStatic) {
  report.push(`\n### ${name}`);
  report.push(`**Methods**: ${module.Methods.length}`);
  report.push(`**Properties**: ${module.Properties.length}`);

  if (module.Methods.length > 0) {
    report.push('\n**Method Signatures:**\n');
    module.Methods.forEach(method => {
      const params = method.Parameters.map(p =>
        `${p.Name}${p.IsOptional ? '?' : ''}: ${p.Type}`
      ).join(', ');
      report.push(`- \`${method.Name}(${params}): ${method.ReturnType}\``);
    });
  }

  if (module.Properties.length > 0) {
    report.push('\n**Properties:**\n');
    module.Properties.forEach(prop => {
      const access = prop.CanWrite ? 'get/set' : 'readonly';
      report.push(`- \`${prop.Name}: ${prop.Type}\` (${access})`);
    });
  }
}

// ============= INSTANCE CLASSES =============
report.push('\n\n---\n');
report.push('## PART 2: INSTANCE CLASSES\n');
report.push(`Found ${Object.keys(instanceClasses).length} instance classes\n`);

const sortedClasses = Object.entries(instanceClasses).sort((a, b) =>
  (b[1].Methods.length + b[1].Properties.length) - (a[1].Methods.length + a[1].Properties.length)
);

for (const [name, module] of sortedClasses.slice(0, 40)) { // Top 40
  report.push(`\n### ${name}`);
  report.push(`**Methods**: ${module.Methods.length} | **Properties**: ${module.Properties.length}`);

  if (module.Methods.length > 0 && module.Methods.length <= 20) {
    report.push('\n**Methods:**\n');
    module.Methods.forEach(method => {
      const params = method.Parameters.map(p =>
        `${p.Name}${p.IsOptional ? '?' : ''}: ${p.Type}`
      ).join(', ');
      report.push(`- \`${method.Name}(${params}): ${method.ReturnType}\``);
    });
  } else if (module.Methods.length > 20) {
    report.push(`\n**Methods:** (${module.Methods.length} total - listing first 10)\n`);
    module.Methods.slice(0, 10).forEach(method => {
      const params = method.Parameters.map(p =>
        `${p.Name}${p.IsOptional ? '?' : ''}: ${p.Type}`
      ).join(', ');
      report.push(`- \`${method.Name}(${params}): ${method.ReturnType}\``);
    });
    report.push(`- ... and ${module.Methods.length - 10} more methods`);
  }

  if (module.Properties.length > 0 && module.Properties.length <= 15) {
    report.push('\n**Properties:**\n');
    module.Properties.forEach(prop => {
      const access = prop.CanWrite ? 'get/set' : 'readonly';
      report.push(`- \`${prop.Name}: ${prop.Type}\` (${access})`);
    });
  } else if (module.Properties.length > 15) {
    report.push(`\n**Properties:** (${module.Properties.length} total - listing first 10)\n`);
    module.Properties.slice(0, 10).forEach(prop => {
      const access = prop.CanWrite ? 'get/set' : 'readonly';
      report.push(`- \`${prop.Name}: ${prop.Type}\` (${access})`);
    });
    report.push(`- ... and ${module.Properties.length - 10} more properties`);
  }
}

// ============= KEY NODE.JS MODULES DETAIL =============
report.push('\n\n---\n');
report.push('## PART 3: CORE NODE.JS MODULES - DETAILED ANALYSIS\n');

const coreModules = ['path', 'fs', 'crypto', 'buffer', 'events', 'os', 'util', 'child_process', 'stream'];

for (const moduleName of coreModules) {
  const module = staticModules[moduleName] || instanceClasses[moduleName];
  if (!module) {
    report.push(`\n### ${moduleName} - ❌ NOT FOUND`);
    continue;
  }

  report.push(`\n### ${moduleName}`);
  report.push(`**Type**: ${module.IsStatic ? 'Static Module' : 'Instance Class'}`);
  report.push(`**API Count**: ${module.Methods.length} methods, ${module.Properties.length} properties`);

  report.push('\n**Complete API:**\n');
  module.Methods.forEach(method => {
    const params = method.Parameters.map(p => {
      const optional = p.IsOptional ? '?' : '';
      const defaultVal = p.DefaultValue ? ` = ${p.DefaultValue}` : '';
      return `${p.Name}${optional}: ${p.Type}${defaultVal}`;
    }).join(', ');
    report.push(`- \`${method.Name}(${params}): ${method.ReturnType}\``);
  });

  if (module.Properties.length > 0) {
    report.push('\n**Properties:**\n');
    module.Properties.forEach(prop => {
      const access = prop.CanWrite ? 'get/set' : 'readonly';
      report.push(`- \`${prop.Name}: ${prop.Type}\` (${access})`);
    });
  }
}

// ============= STATISTICS =============
report.push('\n\n---\n');
report.push('## PART 4: STATISTICS\n');

report.push('\n### Module Counts');
report.push(`- Static Modules: ${Object.keys(staticModules).length}`);
report.push(`- Instance Classes: ${Object.keys(instanceClasses).length}`);
report.push(`- Data Types: ${Object.keys(dataTypes).length}`);
report.push(`- **Total: ${Object.keys(api.Modules).length}**`);

report.push('\n### Method Counts (Top 10 Static Modules)');
Object.entries(staticModules)
  .sort((a, b) => b[1].Methods.length - a[1].Methods.length)
  .slice(0, 10)
  .forEach(([name, module]) => {
    report.push(`- ${name}: ${module.Methods.length} methods`);
  });

report.push('\n### Method Counts (Top 10 Instance Classes)');
Object.entries(instanceClasses)
  .sort((a, b) => b[1].Methods.length - a[1].Methods.length)
  .slice(0, 10)
  .forEach(([name, module]) => {
    report.push(`- ${name}: ${module.Methods.length} methods, ${module.Properties.length} properties`);
  });

// ============= CUSTOM/NON-STANDARD APIS =============
report.push('\n\n---\n');
report.push('## PART 5: POTENTIALLY CUSTOM APIs (Not in Standard Node.js)\n');

const suspectApis = [
  { module: 'crypto', method: 'getDefaultCipherList' },
  { module: 'crypto', method: 'setDefaultEncoding' },
  { module: 'fs', method: 'readFileBytes' },
  { module: 'fs', method: 'readFileSyncBytes' },
  { module: 'fs', method: 'writeFileBytes' },
  { module: 'fs', method: 'writeFileSyncBytes' },
  { module: 'path', method: 'matchesGlob' },
];

report.push('\nThese methods were found in nodejs but may not exist in standard Node.js:\n');
for (const { module: moduleName, method: methodName } of suspectApis) {
  const module = staticModules[moduleName];
  if (module) {
    const method = module.Methods.find(m => m.Name === methodName);
    if (method) {
      const params = method.Parameters.map(p =>
        `${p.Name}${p.IsOptional ? '?' : ''}: ${p.Type}`
      ).join(', ');
      report.push(`- **${moduleName}.${methodName}(${params}): ${method.ReturnType}**`);
      report.push(`  → Custom addition or requires verification`);
    }
  }
}

// Write report
const reportPath = path.join(__dirname, 'COMPREHENSIVE-API-REPORT.md');
fs.writeFileSync(reportPath, report.join('\n'));

console.log(`\nComprehensive report generated: ${reportPath}`);
console.log(`\nReport contains ${report.length} lines`);
console.log(`\nStatistics:`);
console.log(`- Static Modules: ${Object.keys(staticModules).length}`);
console.log(`- Instance Classes: ${Object.keys(instanceClasses).length}`);
console.log(`- Data Types: ${Object.keys(dataTypes).length}`);
console.log(`- Total Types: ${Object.keys(api.Modules).length}`);
