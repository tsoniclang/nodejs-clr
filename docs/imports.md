# Importing Modules

## Preferred with `surface: "@tsonic/js"` and `@tsonic/nodejs`: Node-style specifiers

With `surface: "@tsonic/js"` and `@tsonic/nodejs` installed, import using Node module names:

```ts
import { readFileSync } from "node:fs";
import { join } from "node:path";
import * as process from "node:process";
```

Bare aliases are also supported:

```ts
import { readFileSync } from "fs";
import { join } from "path";
```

## Direct package imports remain supported

You can still import directly from `@tsonic/nodejs/index.js`:

```ts
import { fs, path, process, crypto } from "@tsonic/nodejs/index.js";
```

Types exported from the package root are also available there:

```ts
import { EventEmitter } from "@tsonic/nodejs/index.js";
```

## Submodules (separate namespaces)

Some namespaces are emitted as separate entry points. Example:

```ts
import { http, IncomingMessage, ServerResponse } from "@tsonic/nodejs/nodejs.Http.js";
```
