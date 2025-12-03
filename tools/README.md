# Tsonic.Node API Verification Tools

This directory contains tools for verifying that Tsonic.Node's API signatures match Node.js official type definitions.

## Overview

The verification system consists of two parts:

1. **API Extractor** (C#): Uses reflection to extract Tsonic.Node's API signatures
2. **API Verifier** (TypeScript): Parses Node.js @types definitions and compares against extracted API

## Workflow

```
┌─────────────────────┐
│  Tsonic.Node DLL    │
│  (C# Assembly)      │
└──────────┬──────────┘
           │
           │ Reflection
           ▼
┌─────────────────────┐
│ ApiExtractor (C#)   │
│                     │
└──────────┬──────────┘
           │
           │ Generates
           ▼
┌─────────────────────┐
│ nodejs-clr-api.json│  ◄────┐
└──────────┬──────────┘        │
           │                   │
           │                   │ Compares
           ▼                   │
┌─────────────────────┐        │
│ API Verifier (TS)   │────────┘
│                     │
└──────────┬──────────┘
           │ Parses
           ▼
┌─────────────────────┐
│  @types/node/*.d.ts │
│  (Official Types)   │
└──────────┬──────────┘
           │
           │ Generates
           ▼
┌─────────────────────┐
│ verification-       │
│ report.md           │
└─────────────────────┘
```

## Usage

### Step 1: Extract Tsonic.Node API

```bash
# Build and run the API extractor
dotnet run --project tools/Tsonic.Node.ApiExtractor/Tsonic.Node.ApiExtractor.csproj -- tools/nodejs-clr-api.json
```

This generates `tools/nodejs-clr-api.json` containing all public APIs from the Tsonic.Node assembly.

### Step 2: Verify Against Node.js Types

```bash
# Run the TypeScript verifier
cd tools/api-verifier
npm run verify
```

This generates `tools/verification-report.md` showing which APIs match, which are missing, and which are extra.

## API Extractor (C#)

**Project**: `Tsonic.Node.ApiExtractor`
**Location**: `tools/Tsonic.Node.ApiExtractor/`

### What it does:

- Uses .NET reflection to inspect the Tsonic.Node assembly
- Extracts all public types, methods, properties
- Converts C# types to TypeScript-like type names
- Outputs structured JSON

### Type Mapping:

| C# Type | JSON Output |
|---------|-------------|
| `void` | `"void"` |
| `string` | `"string"` |
| `int`, `double` | `"number"` |
| `bool` | `"boolean"` |
| `object` | `"any"` |
| `string[]` | `"string[]"` |
| `Action<string>` | `"(arg0: string) => void"` |
| `Func<string, int>` | `"(arg0: string) => number"` |

### JSON Schema:

```json
{
  "Modules": {
    "path": {
      "Name": "path",
      "IsClass": true,
      "IsStatic": true,
      "Methods": [
        {
          "Name": "join",
          "IsStatic": true,
          "ReturnType": "string",
          "Parameters": [
            {
              "Name": "paths",
              "Type": "string[]",
              "IsOptional": false
            }
          ]
        }
      ],
      "Properties": []
    }
  }
}
```

## API Verifier (TypeScript)

**Project**: `api-verifier`
**Location**: `tools/api-verifier/`

### What it does:

- Uses TypeScript Compiler API to parse @types/node definitions
- Loads the extracted Tsonic.Node API JSON
- Compares module-level functions, classes, methods, properties
- Generates markdown report

### Current Limitations:

1. **Namespace Parsing**: Some modules like `path` use namespace declarations which need special handling
2. **Method Overloads**: Currently checks only presence, not all overload signatures
3. **Type Compatibility**: Doesn't verify parameter/return type compatibility, only names
4. **Module Mapping**: Manual mapping between Tsonic module names and Node.js module names

### Comparison Logic:

For static modules (like `path`, `fs`, `crypto`):
- Compares Tsonic static methods against Node.js module-level functions

For instance classes (like `EventEmitter`, `Buffer`):
- Compares Tsonic class methods against Node.js class methods
- Compares Tsonic properties against Node.js properties

## Output

###Human: i hve to leave. commit and push