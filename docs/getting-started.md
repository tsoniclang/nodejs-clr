# Getting Started

## Enable Node.js APIs in a Tsonic Project

### New project

```bash
npx --yes tsonic@latest init --surface @tsonic/js
npx --yes tsonic@latest add npm @tsonic/nodejs
```

### Existing project

```bash
npx --yes tsonic@latest add npm @tsonic/nodejs
```

That will:

- Install the `@tsonic/nodejs` bindings package in your workspace (`package.json`) for `tsc` typechecking
- Apply the package’s `.NET` dependency manifest (`tsonic.bindings.json`) to `tsonic.workspace.json`
  - Adds the required `dotnet.frameworkReferences` / `dotnet.packageReferences`
  - Installs any additional `types` packages referenced by the manifest

Then run `npx --yes tsonic@latest restore` (or `npx --yes tsonic@latest build`) to materialize .NET dependencies.

## Minimal Example

```ts
import { readFileSync } from "node:fs";
import { join } from "node:path";

export function main(): void {
  const fullPath = join("src", "App.ts");
  console.log(fullPath);
  console.log(readFileSync(fullPath, "utf-8"));
}
```

## Notes

- With `surface: "@tsonic/js"` and `@tsonic/nodejs` installed, prefer Node-style imports (`node:fs`, `node:path`, ...).
- Tsonic is ESM-first. Import submodules with `.js` for subpaths (example: `@tsonic/nodejs/nodejs.Http.js`).
- This library is Node-inspired, but many APIs intentionally follow .NET behavior where it improves ergonomics.
