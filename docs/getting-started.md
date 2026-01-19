# Getting Started

## Enable Node.js APIs in a Tsonic Project

### New project

```bash
tsonic init --nodejs
```

### Existing project

```bash
tsonic add nodejs
```

That will:

- Install the `@tsonic/nodejs` bindings package in your workspace (`package.json`) for `tsc` typechecking
- Copy the runtime DLLs into `libs/`:
  - `libs/Tsonic.JSRuntime.dll` (NodeJS depends on JSRuntime)
  - `libs/nodejs.dll`
- Add those DLLs to `tsonic.workspace.json` under `dotnet.libraries`

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
