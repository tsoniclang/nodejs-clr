# nodejs-clr

`nodejs-clr` is a **.NET library** that provides Node-style APIs (filesystem, path, crypto, networking, etc.) for Tsonic projects.

It is Node-inspired (familiar ergonomics), but it is **not** Node.js itself and it is **not an exact replica** of the Node standard library.

## For Tsonic Users

Install and enable the bindings package:

```bash
# new project
tsonic init
tsonic add npm @tsonic/nodejs

# existing project
tsonic add npm @tsonic/nodejs
```

Then import Node-style modules from `@tsonic/nodejs/index.js`:

```ts
import { console, fs, path } from "@tsonic/nodejs/index.js";

export function main(): void {
  console.log(path.join("a", "b", "c"));
  console.log(fs.readFileSync("./README.md", "utf-8"));
}
```

Some namespaces are emitted as separate ESM entry points (for example `nodejs.Http`) and are imported via a subpath:

```ts
import { http } from "@tsonic/nodejs/nodejs.Http.js";
```

Documentation:

- `docs/README.md`
- https://tsonic.org/nodejs/

## For Contributors

Build:

```bash
dotnet build
```

If `dotnet build` fails with "Build FAILED" but no errors (some sandboxed environments block MSBuild node sockets), try:

```bash
dotnet build -- -maxcpucount:1
```

Test:

```bash
dotnet test
```

The `@tsonic/nodejs` package is generated from the compiled assembly via **tsbindgen**.

## License

MIT
