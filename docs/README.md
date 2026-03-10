# Node.js Compatibility (`@tsonic/nodejs`)

Tsonic targets the .NET BCL by default. If you want **Node-style APIs** (`fs`, `path`, `crypto`, `net`, `process`, ...), use `@tsonic/nodejs` in a workspace with `surface: "@tsonic/js"`.

This is **not** Node.js itself, and it is **not a byte-for-byte clone** of the Node standard library. It is a curated, Node-inspired API surface implemented on .NET for Tsonic projects.

## Table of Contents

### Getting Started

1. [Getting Started](getting-started.md) - enable `@tsonic/nodejs` in a Tsonic project
2. [Importing Modules](imports.md) - what to import from `@tsonic/nodejs/index.js` vs submodules

### Modules

3. [console](modules/console.md)
4. [path](modules/path.md)
5. [fs](modules/fs.md)
6. [Buffer](modules/buffer.md)
7. [events](modules/events.md)
8. [timers](modules/timers.md)
9. [process](modules/process.md)
10. [os](modules/os.md)
11. [util](modules/util.md)
12. [assert](modules/assert.md)
13. [performance](modules/performance.md)
14. [stream](modules/stream.md)
15. [readline](modules/readline.md)
16. [querystring](modules/querystring.md)
17. [zlib](modules/zlib.md)
18. [crypto](modules/crypto.md)
19. [tls](modules/tls.md)
20. [X509 / Certificate](modules/x509.md)
21. [dns](modules/dns.md)
22. [net](modules/net.md)
23. [dgram](modules/dgram.md)
24. [child_process](modules/child_process.md)
25. [http](modules/http.md) (separate submodule)

## Overview

In Node surface projects you can write natural Node imports:

```ts
import { readFileSync } from "node:fs";
import { join } from "node:path";

export function main(): void {
  const path = join("a", "b", "c");
  console.log(path);
  const text = readFileSync("./README.md", "utf-8");
  console.log(text);
}
```

Direct imports from `@tsonic/nodejs/index.js` remain supported.

Some namespaces are emitted as separate ESM entry points (for example `nodejs.Http`), and you import those via a subpath:

```ts
import { http } from "@tsonic/nodejs/nodejs.Http.js";
```

## Relationship to `@tsonic/js`

- `@tsonic/js` provides JavaScript runtime APIs (e.g. `JSON`, JS-style `console`, timers).
- `@tsonic/nodejs` provides Node-style APIs (e.g. `fs`, `path`, `crypto`, `http`).

You can enable either or both in a project.
