# Coding Standards

This document defines the coding standards for the Tsonic Node API project.

## General Principles

1. **Consistency**: Follow existing patterns in the codebase
2. **Clarity**: Write self-documenting code with clear names
3. **Compatibility**: Match Node.js behavior exactly where possible
4. **Performance**: Use efficient .NET APIs without premature optimization
5. **Safety**: Prefer compile-time safety over runtime flexibility

## C# Code Style

### Naming Conventions

```csharp
// Static classes for modules - lowercase to match JavaScript
public static class path { }
public static class fs { }

// Instance classes - PascalCase
public class EventEmitter { }
public class ParsedPath { }

// Methods - lowercase for JavaScript compatibility
public static string join(params string[] paths) { }

// Properties - lowercase for JavaScript objects
public class ParsedPath
{
    public string root { get; set; }
    public string @base { get; set; }  // Use @ for C# keywords
}

// Private fields - camelCase with underscore
private readonly Dictionary<string, List<Delegate>> _events = new();
private int _maxListeners = 10;

// Static fields - camelCase with underscore
private static int _defaultMaxListeners = 10;
```

### Code Organization

```csharp
namespace Tsonic.StdLib;

/// <summary>
/// Module-level summary.
/// </summary>
public static class moduleName
{
    // 1. Constants/static readonly fields
    public static readonly string sep = "/";

    // 2. Public methods (alphabetical order)
    public static string basename(string path) { }
    public static string dirname(string path) { }

    // 3. Private helper methods
    private static void HelperMethod() { }
}
```

### Documentation

**All public members must have XML documentation:**

```csharp
/// <summary>
/// One-line summary of what the method does.
/// </summary>
/// <param name="paramName">Description of parameter.</param>
/// <returns>Description of return value.</returns>
/// <exception cref="ExceptionType">When this exception is thrown.</exception>
public static string method(string paramName)
{
    // Implementation
}
```

### Error Handling

```csharp
// Prefer specific exceptions
if (string.IsNullOrEmpty(path))
    throw new ArgumentNullException(nameof(path));

if (!File.Exists(path))
    throw new FileNotFoundException($"File not found: {path}");

// Let .NET exceptions bubble up (don't catch and rethrow)
// ❌ Bad
try
{
    File.ReadAllText(path);
}
catch (Exception ex)
{
    throw new Exception("Failed to read file", ex);
}

// ✅ Good
File.ReadAllText(path);  // Let IOException bubble up naturally
```

### Nullable Annotations

```csharp
// Use nullable reference types
public static string? basename(string path, string? suffix = null)
{
    if (suffix != null && name.EndsWith(suffix))
    {
        // Handle suffix
    }
    return name;
}

// Null-forgiving operator when you know value is not null
var dir = pathObject.dir ?? string.Empty;
var @base = pathObject.@base ?? string.Empty;
```

### Platform-Specific Code

```csharp
using System.Runtime.InteropServices;

if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
{
    // Windows-specific code
}
else
{
    // Unix/Linux/macOS code
}

// Or use platform-agnostic APIs
var separator = Path.DirectorySeparatorChar;
var delimiter = Path.PathSeparator;
```

## TypeScript Declarations

### File Structure

```typescript
/**
 * Module description.
 */
declare module "moduleName" {
    /**
     * Interface or type definitions first
     */
    export interface SomeInterface {
        property: string;
    }

    /**
     * Function declarations
     */
    export function functionName(param: string): string;

    /**
     * Class declarations
     */
    export class ClassName {
        method(): void;
    }
}

// Node.js module alias
declare module "node:moduleName" {
    export * from "moduleName";
}
```

### Type Annotations

```typescript
// Prefer explicit types over any
export function method(path: string, options?: Options): Result;

// Use union types for multiple possibilities
export function method(path: string | Buffer): string;

// Use optional parameters with ?
export function method(path: string, suffix?: string): string;

// Document with JSDoc
/**
 * Description of the function.
 *
 * @param path - The path to process
 * @param suffix - Optional suffix to remove
 * @returns The processed path
 *
 * @example
 * ```ts
 * basename("/foo/bar.txt", ".txt");
 * // Returns: "bar"
 * ```
 */
export function basename(path: string, suffix?: string): string;
```

## JSON Metadata

### Metadata Format

```json
{
  "assemblyName": "Tsonic.StdLib",
  "types": {
    "Tsonic.StdLib.TypeName": {
      "kind": "class",
      "isStatic": true,
      "members": {
        "methodName(string)": {
          "kind": "method",
          "isStatic": true
        },
        "methodName(string,string)": {
          "kind": "method",
          "isStatic": true
        },
        "propertyName": {
          "kind": "property",
          "isStatic": true,
          "isReadonly": true
        }
      }
    }
  }
}
```

**Rules:**
- Method signatures include parameter types: `methodName(type1,type2)`
- Use `params string[]` for varargs
- Properties don't include parentheses
- Alphabetical order for consistency

### Bindings Format

```json
{
  "bindings": {
    "moduleName": {
      "kind": "module",
      "assembly": "Tsonic.StdLib",
      "type": "Tsonic.StdLib.className"
    }
  }
}
```

**Rules:**
- Module names are lowercase (match JavaScript)
- Assembly and type names are PascalCase (match C#)
- One binding per module
- Alphabetical order

## Testing Standards

### Test Organization

```csharp
public class ModuleTests : IDisposable
{
    // Setup
    private readonly string _testDir;

    public ModuleTests()
    {
        _testDir = Path.Combine(Path.GetTempPath(), $"test-{Guid.NewGuid()}");
        Directory.CreateDirectory(_testDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDir))
            Directory.Delete(_testDir, recursive: true);
    }

    // Tests in alphabetical order by method name
    [Fact]
    public void MethodName_Scenario_ExpectedBehavior()
    {
        // Arrange
        var input = "test";

        // Act
        var result = module.method(input);

        // Assert
        Assert.Equal("expected", result);
    }
}
```

### Test Naming

**Pattern:** `MethodName_Scenario_ExpectedBehavior`

Examples:
- `Basename_ShouldReturnFileName`
- `Basename_WithSuffix_ShouldRemoveSuffix`
- `Basename_EmptyPath_ShouldReturnEmpty`
- `ReadFileSync_NonExistentFile_ShouldThrow`

### Assertions

```csharp
// Equality
Assert.Equal(expected, actual);
Assert.NotEqual(expected, actual);

// Truthiness
Assert.True(condition);
Assert.False(condition);

// Null checks
Assert.Null(value);
Assert.NotNull(value);

// Exceptions
Assert.Throws<ExceptionType>(() => method());

// Collections
Assert.Empty(collection);
Assert.NotEmpty(collection);
Assert.Contains(item, collection);
Assert.Equal(expectedCount, collection.Count);

// Same instance
Assert.Same(expected, actual);
```

## File Organization

### Solution Structure

```
Tsonic.StdLib.sln              # Solution file
├── Directory.Build.props        # Shared properties
├── src/
│   └── Tsonic.StdLib/
│       ├── Tsonic.StdLib.csproj
│       ├── module1.cs
│       └── module2.cs
├── types/
│   ├── module1.d.ts
│   ├── module1.metadata.json
│   ├── module2.d.ts
│   ├── module2.metadata.json
│   ├── Tsonic.StdLib.bindings.json
│   └── index.d.ts
├── tests/
│   └── Tsonic.StdLib.Tests/
│       ├── Tsonic.StdLib.Tests.csproj
│       ├── Module1Tests.cs
│       └── Module2Tests.cs
├── docs/                        # Documentation
│   ├── api-coverage.md
│   ├── claude.md
│   └── coding-standards.md
├── .analysis/                   # Temporary analysis files (gitignored)
├── samples/                     # Example usage
└── README.md
```

### File Naming

- C# files: `PascalCase.cs` (e.g., `EventEmitter.cs`, `path.cs`)
- TypeScript: `lowercase.d.ts` (e.g., `path.d.ts`, `fs.d.ts`)
- Metadata: `lowercase.metadata.json` (e.g., `path.metadata.json`)
- Tests: `PascalCaseTests.cs` (e.g., `PathTests.cs`)

## Git Practices

### Commit Messages

Format: `<type>: <description>`

Types:
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation only
- `test`: Adding/updating tests
- `refactor`: Code refactoring
- `chore`: Build/tooling changes

Examples:
- `feat: add path.matchesGlob() implementation`
- `fix: handle empty strings in path.basename()`
- `docs: update API coverage analysis`
- `test: add edge case tests for fs.readFileSync()`

### Branch Names

- `feature/<description>` - New features
- `fix/<description>` - Bug fixes
- `docs/<description>` - Documentation updates

### .gitignore

Always ignore:
- Build outputs: `bin/`, `obj/`, `*.dll`, `*.exe`
- Test outputs: `TestResults/`, `*.trx`
- Temporary analysis: `.analysis/`
- IDE files: `.vscode/`, `.vs/`
- Node modules: `node_modules/`
- NuGet packages: `*.nupkg`

## Build Configuration

### Directory.Build.props

```xml
<Project>
  <PropertyGroup>
    <TargetFramework>net10.0</TargetFramework>
    <LangVersion>latest</LangVersion>
    <Nullable>enable</Nullable>
    <TreatWarningsAsErrors>true</TreatWarningsAsErrors>

    <!-- Suppress CS8981 for lowercase names -->
    <NoWarn>$(NoWarn);CS8981</NoWarn>
  </PropertyGroup>
</Project>
```

### Project Files

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <AssemblyName>Tsonic.StdLib</AssemblyName>
    <RootNamespace>Tsonic.StdLib</RootNamespace>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
  </PropertyGroup>
</Project>
```

## Performance Guidelines

1. **Avoid allocations in hot paths**
   ```csharp
   // ❌ Bad - allocates new array each time
   public string join(params string[] paths)
   {
       var list = new List<string>(paths);
   }

   // ✅ Good - use array directly
   public string join(params string[] paths)
   {
       var validPaths = paths.Where(p => !string.IsNullOrEmpty(p));
   }
   ```

2. **Use Span<T> for string operations**
   ```csharp
   // ✅ Good - zero allocation substring
   var slice = path.AsSpan(0, length);
   ```

3. **Cache platform checks**
   ```csharp
   // ❌ Bad - checks every time
   if (Path.DirectorySeparatorChar == '/') { }

   // ✅ Good - check once
   private static readonly bool IsUnix = Path.DirectorySeparatorChar == '/';
   ```

## Review Checklist

Before submitting code:

- [ ] All public members have XML documentation
- [ ] Matches official Node.js API behavior
- [ ] Tests cover happy path and edge cases
- [ ] No compiler warnings (TreatWarningsAsErrors=true)
- [ ] Metadata JSON is valid and complete
- [ ] Type definitions match @types/node
- [ ] Binding added to Tsonic.StdLib.bindings.json
- [ ] Platform-specific code handles Windows and Unix
- [ ] Temp files cleaned up in tests
- [ ] Null safety annotations used correctly

## Resources

- [.NET Design Guidelines](https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/)
- [xUnit Best Practices](https://xunit.net/docs/comparisons)
- [TypeScript Handbook](https://www.typescriptlang.org/docs/handbook/intro.html)
- [Node.js API Docs](https://nodejs.org/api/)
