# nodejs-clr

`nodejs-clr` is a **.NET library** that provides Node-style APIs (filesystem, path, crypto, networking, etc.) for Tsonic projects.

It is Node-inspired (familiar ergonomics), but it is **not** Node.js itself and it is **not an exact replica** of the Node standard library.

The runtime keeps public contracts stable for Tsonic callers:

- integer-backed APIs stay integer-backed where the TypeScript surface declares
  `int` or `long`
- HTTP request/response wrappers expose Node-style fields over ASP.NET Core
  primitives
- `crypto` key generation and signing helpers use deterministic CLR key object
  handling
- timers and immediates dispatch through managed scheduler primitives

## For Tsonic Users

Install and enable the bindings package:

```bash
# new project
npx --yes tsonic@latest init --surface @tsonic/js
npx --yes tsonic@latest add npm @tsonic/nodejs

# existing project
npx --yes tsonic@latest add npm @tsonic/nodejs
```

Then write natural Node-style code:

```ts
import { readFileSync } from "node:fs";
import { join } from "node:path";

export function main(): void {
  const p = join("a", "b", "c");
  console.log(p);
  console.log(readFileSync("./README.md", "utf-8"));
}
```

Some namespaces are emitted as separate ESM entry points (for example `nodejs.Http`) and are imported via a subpath:

```ts
import { http } from "@tsonic/nodejs/nodejs.Http.js";
```

Direct imports from `@tsonic/nodejs/index.js` remain supported.

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

End-to-end package validation lives in the sibling `@tsonic/nodejs` repo:

```bash
cd ../nodejs
npm run selftest
```

The `@tsonic/nodejs` package is generated from the compiled assembly via **tsbindgen**.

## License

MIT
