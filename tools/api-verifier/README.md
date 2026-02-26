# api-verifier layout

- `reflection/`
  - `extract-node-runtime.ts`: reflects Node builtin modules at runtime into `tools/node-runtime-api.json`.
  - `compare-reflection.ts`: compares Node runtime snapshot with CLR reflection snapshot and writes `tools/reflection-coverage-report.md`.
- `dts/`
  - `verify.ts`: legacy `@types/node` vs CLR reflection verifier.
  - `report-node-gaps.ts`: detailed module gap report from declaration files.

Package scripts in `package.json` are the entry points:

- `npm run verify:reflection` (primary)
- `npm run report:gaps` (declaration-based analysis)
