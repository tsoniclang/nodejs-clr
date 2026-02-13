# Getting Started

## Enable Node.js APIs in a Tsonic Project

### New project

```bash
tsonic init
tsonic add npm @tsonic/nodejs
```

### Existing project

```bash
tsonic add npm @tsonic/nodejs
```

That will:

- Install the `@tsonic/nodejs` bindings package in your workspace (`package.json`) for `tsc` typechecking
- Apply the package’s `.NET` dependency manifest (`tsonic.bindings.json`) to `tsonic.workspace.json`
  - Adds the required `dotnet.frameworkReferences` / `dotnet.packageReferences`
  - Installs any additional `types` packages referenced by the manifest

Then run `tsonic restore` (or just `tsonic build`, which will restore via `dotnet`) to materialize the .NET dependencies.

## Minimal Example

```ts
import { console, fs, path } from "@tsonic/nodejs/index.js";

export function main(): void {
  const fullPath = path.join("src", "App.ts");
  console.log(fullPath);

  if (fs.existsSync(fullPath)) {
    console.log(fs.readFileSync(fullPath, "utf-8"));
  }
}
```

## Notes

- Tsonic is ESM-first. Import submodules with `.js` when you use a subpath (example: `@tsonic/nodejs/nodejs.Http.js`).
- This library is Node-inspired, but many APIs intentionally follow .NET behavior where it improves ergonomics.
