# Tsonic Standard Library

A Node.js-like standard library for .NET, enabling TypeScript compiled to C# via Tsonic to use familiar APIs for file system, path manipulation, events, and more.

## Overview

Tsonic compiles TypeScript to C# so code can run on .NET. While the [Tsonic Runtime](https://github.com/tsoniclang/tsonic-runtime) provides JavaScript standard library functionality (console, Math, JSON, etc.), **Tsonic.StdLib** implements Node.js-inspired APIs like `fs`, `path`, and `events`.

**Note:** This library is inspired by Node.js APIs but is **not an exact replica**. It provides similar, familiar APIs that work naturally with .NET while maintaining the Node.js developer experience. APIs may deviate where .NET offers better approaches or where exact Node.js compatibility is impractical.

This repository provides:
- **C# Implementation** (`Tsonic.StdLib`) - A .NET library with Node.js-inspired APIs optimized for .NET
- **TypeScript Declarations** (`.d.ts` files) - Type definitions for IDE support
- **Metadata Files** (`.metadata.json`) - C# semantic information for the Tsonic compiler
- **Bindings** (`.bindings.json`) - Maps JavaScript module names to CLR types

## Installation

### For TypeScript Projects (using Tsonic)

```bash
npm install --save-dev @tsonic/node-types
```

### For .NET Projects

```bash
dotnet add package Tsonic.StdLib
```

## Usage

### In TypeScript (compiled via Tsonic)

```typescript
import * as path from "path";
import * as fs from "fs";
import { EventEmitter } from "events";

// Path operations
const fullPath = path.join(__dirname, "config", "settings.json");
const ext = path.extname(fullPath);  // ".json"
const dir = path.dirname(fullPath);

// File system operations
const content = fs.readFileSync("./package.json", "utf-8");
fs.writeFileSync("./output.txt", "Hello from Tsonic!");

// Event handling
class MyEmitter extends EventEmitter {}
const emitter = new MyEmitter();
emitter.on("event", (data) => console.log(data));
emitter.emit("event", "Hello World!");
```

### Tsonic Configuration

Add to your `tsconfig.json` or Tsonic configuration:

```json
{
  "$schema": "https://tsonic.org/schema/v1.json",
  "rootNamespace": "MyApp",
  "entryPoint": "src/index.ts",
  "dotnet": {
    "typeRoots": [
      "node_modules/@tsonic/dotnet-types/10.0.0/types",
      "../tsonic-runtime/src/Tsonic.Runtime/types",
      "node_modules/@tsonic/node-types/types"
    ],
    "packages": [
      { "name": "Tsonic.Runtime", "version": "1.0.0" },
      { "name": "Tsonic.StdLib", "version": "1.0.0" }
    ]
  }
}
```

The Tsonic compiler will:
1. Load type definitions from `@tsonic/node-types`
2. Use bindings to map `import "fs"` → `Tsonic.StdLib.fs`
3. Generate C# code that calls the .NET implementation
4. Add NuGet package references automatically

## Implemented Modules

For detailed API coverage compared to official Node.js types, see [API Coverage](docs/api-coverage.md).

### `path` - Path manipulation

Provides utilities for working with file and directory paths.

```typescript
import * as path from "path";

path.join("/foo", "bar", "baz");       // "/foo/bar/baz"
path.basename("/foo/bar/file.txt");    // "file.txt"
path.extname("index.html");            // ".html"
path.resolve("wwwroot", "static");     // Absolute path
path.isAbsolute("/foo");               // true
```

**Available functions:**
- `basename(path, suffix?)` - Get the last portion of a path
- `dirname(path)` - Get the directory name
- `extname(path)` - Get the file extension
- `join(...paths)` - Join path segments
- `normalize(path)` - Normalize a path
- `resolve(...paths)` - Resolve to an absolute path
- `isAbsolute(path)` - Test if path is absolute
- `relative(from, to)` - Get relative path
- `parse(path)` - Parse a path into an object
- `format(pathObject)` - Format a path from an object
- `matchesGlob(path, pattern)` - Test glob pattern matching
- `toNamespacedPath(path)` - Windows namespace prefix (Windows only)

**Properties:**
- `sep` - Platform-specific path separator (`/` or `\`)
- `delimiter` - Platform-specific path delimiter (`:` or `;`)
- `posix` - POSIX path utilities
- `win32` - Windows path utilities

### `fs` - File system operations

Synchronous file system operations for reading, writing, and manipulating files and directories.

```typescript
import * as fs from "fs";

// Read/write files
const data = fs.readFileSync("./config.json", "utf-8");
fs.writeFileSync("./output.txt", "Hello!");
fs.appendFileSync("./log.txt", "New entry\n");

// Directory operations
fs.mkdirSync("./temp", { recursive: true });
const files = fs.readdirSync("./src");
fs.rmdirSync("./temp", { recursive: true });

// File info
const stats = fs.statSync("./package.json");
console.log(stats.size, stats.mtime);

// Other operations
fs.existsSync("./file.txt");
fs.copyFileSync("./src.txt", "./dest.txt");
fs.renameSync("./old.txt", "./new.txt");
fs.unlinkSync("./file.txt");
```

**Available functions:**
- `readFileSync(path, encoding?)` - Read file contents
- `writeFileSync(path, data, encoding?)` - Write to file
- `appendFileSync(path, data, encoding?)` - Append to file
- `existsSync(path)` - Check if path exists
- `mkdirSync(path, options?)` - Create directory
- `readdirSync(path)` - Read directory contents
- `statSync(path)` - Get file/directory stats
- `unlinkSync(path)` - Delete file
- `rmdirSync(path, options?)` - Remove directory
- `copyFileSync(src, dest)` - Copy file
- `renameSync(oldPath, newPath)` - Rename/move file

### `events` - Event emitter

Node.js EventEmitter for implementing event-driven architectures.

```typescript
import { EventEmitter } from "events";

class DataStream extends EventEmitter {
  processData() {
    this.emit("data", { chunk: 1 });
    this.emit("end");
  }
}

const stream = new DataStream();
stream.on("data", (chunk) => console.log("Data:", chunk));
stream.once("end", () => console.log("Complete"));
stream.processData();
```

**Available methods:**
- `on(eventName, listener)` - Register event listener
- `once(eventName, listener)` - Register one-time listener
- `emit(eventName, ...args)` - Trigger event
- `off(eventName, listener)` / `removeListener()` - Remove listener
- `removeAllListeners(eventName?)` - Remove all listeners
- `listeners(eventName)` - Get listener array
- `listenerCount(eventName)` - Count listeners
- `eventNames()` - Get registered event names
- `setMaxListeners(n)` / `getMaxListeners()` - Listener limits
- `prependListener()` / `prependOnceListener()` - Add to beginning

## Architecture

### How It Works

```
TypeScript Source Code
         ↓
    Tsonic Compiler
         ↓
  (reads .d.ts, .metadata.json, .bindings.json)
         ↓
    Generated C# Code
         ↓
  .NET Runtime (NativeAOT)
         ↓
   Tsonic.StdLib (this library)
         ↓
   .NET BCL (File, Path, etc.)
```

### File Structure

```
tsonic-node/
├── src/Tsonic.StdLib/        # C# implementation
│   ├── path.cs                # Path module
│   ├── fs.cs                  # File system module
│   └── EventEmitter.cs        # Event emitter
├── types/                     # TypeScript declarations
│   ├── path.d.ts              # Path type definitions
│   ├── fs.d.ts                # FS type definitions
│   ├── events-simple.d.ts     # Events type definitions
│   ├── *.metadata.json        # CLR metadata
│   ├── Tsonic.StdLib.bindings.json  # Module bindings
│   └── index.d.ts             # Main entry point
├── tests/Tsonic.StdLib.Tests/  # Unit tests
└── Tsonic.StdLib.sln         # Solution file
```

### Metadata and Bindings

**Bindings** (`Tsonic.StdLib.bindings.json`) map JavaScript imports to C# types:

```json
{
  "bindings": {
    "path": {
      "kind": "module",
      "assembly": "Tsonic.StdLib",
      "type": "Tsonic.StdLib.path"
    }
  }
}
```

**Metadata** (`.metadata.json`) provides C# semantic information:

```json
{
  "assemblyName": "Tsonic.StdLib",
  "types": {
    "Tsonic.StdLib.path": {
      "kind": "class",
      "isStatic": true,
      "members": {
        "join(params string[])": {
          "kind": "method",
          "isStatic": true
        }
      }
    }
  }
}
```

## Development

### Prerequisites

- .NET 10 SDK (or later)
- Node.js 18+ (for npm package)

### Building

```bash
# Build C# library
dotnet build

# Run tests
dotnet test

# Pack NuGet package
dotnet pack src/Tsonic.StdLib/Tsonic.StdLib.csproj -c Release

# Pack npm package
npm pack
```

### Testing

```bash
cd tests/Tsonic.StdLib.Tests
dotnet test
```

## Differences from Node.js

**Important:** This is **not a Node.js compatibility layer**. It's a **.NET-native standard library** inspired by Node.js APIs. Expect differences where .NET offers better approaches or where exact compatibility is impractical.

### Design Philosophy

- **Familiar, not identical** - APIs follow Node.js conventions but adapt to .NET idioms
- **Pragmatic** - We deviate from Node.js when .NET offers better solutions
- **Performance** - Leverages .NET BCL for optimal performance
- **NativeAOT first** - Designed for ahead-of-time compilation

### Not Yet Implemented

- **Asynchronous operations** - Only synchronous methods are implemented currently
- **Streams** - `ReadStream`, `WriteStream`, etc.
- **Promises** - `fs/promises` module
- **Advanced features** - File watchers, advanced stat info, symbolic links
- **Buffer** - Binary data handling (partially - use `Uint8Array` instead)
- **Process module** - Not yet implemented

### Platform Differences

- **File permissions** - Limited support on Windows (mode/chmod operations may not work as expected)
- **Path separators** - Automatically handled by .NET APIs
- **Case sensitivity** - Follows platform conventions (case-insensitive on Windows)

## Contributing

Contributions are welcome! This project follows the patterns established in [tsonic-runtime](https://github.com/tsoniclang/tsonic-runtime).

### Adding a New Module

1. Create C# implementation in `src/Tsonic.StdLib/`
2. Add TypeScript declarations in `types/`
3. Create metadata file (`types/<module>.metadata.json`)
4. Add binding in `types/Tsonic.StdLib.bindings.json`
5. Add reference in `types/index.d.ts`
6. Write tests in `tests/Tsonic.StdLib.Tests/`

## License

MIT License - see [LICENSE](LICENSE) file for details.

## Related Projects

- [Tsonic](https://github.com/tsoniclang/tsonic) - TypeScript to C# compiler
- [Tsonic Runtime](https://github.com/tsoniclang/tsonic-runtime) - JavaScript standard library for .NET

## Support

- Documentation: https://tsonic.org
- Issues: https://github.com/tsoniclang/tsonic-node/issues
- Discussions: https://github.com/tsoniclang/tsonic/discussions
