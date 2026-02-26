# nodejs-clr API Verification Tools

This folder now supports two complementary coverage reports:

1. **Reflection vs reflection** (primary): Node runtime exports vs `nodejs` CLR assembly public surface.
2. **Type definitions vs reflection** (secondary): `@types/node` declarations vs `nodejs` CLR assembly.

The reflection path gives the fastest answer to “what APIs are actually present on both ends”.

## Commands

Run from `tools/api-verifier`:

```bash
# Full reflection report (Node runtime ↔ CLR)
npm run verify:reflection

# Legacy typings report (@types/node ↔ CLR)
npm run report:gaps
```

Code layout under `tools/api-verifier`:

- `reflection/*` for runtime-reflection comparison scripts.
- `dts/*` for declaration-file comparison scripts.

`verify:reflection` performs three steps:

1. `npm run extract:clr`
   - Runs `tools/nodejs.ApiExtractor`.
   - Writes `tools/nodejs-clr-api.json`.
2. `npm run extract:node-runtime`
   - Reflects Node builtin modules at runtime.
   - Writes `tools/node-runtime-api.json`.
3. `npm run compare:reflection`
   - Compares both snapshots.
   - Writes `tools/reflection-coverage-report.md`.

## Outputs

- `tools/nodejs-clr-api.json`
  - Reflection snapshot of public CLR API from assembly `nodejs`.
  - Includes types under `nodejs` and nested namespaces like `nodejs.Http`.
- `tools/node-runtime-api.json`
  - Reflection snapshot of Node builtin module runtime exports.
- `tools/reflection-coverage-report.md`
  - Per-module coverage table.
  - Missing Node exports.
  - CLR-only members.
  - Node module load errors (if any).

## Scope and limitations

The reflection report is intentionally **name-level**:

- It compares exported member names, not full overload/type compatibility.
- It is ideal for surface coverage tracking and regression detection.
- Use `report:gaps` when you need an `@types/node`-oriented view.

## API extractor

Project: `tools/nodejs.ApiExtractor/nodejs.ApiExtractor.csproj`

The extractor uses .NET reflection to gather:

- public types
- public methods
- public properties

It serializes these as JSON for downstream comparison.
