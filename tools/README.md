# Node.js API Verification Tools

This directory has one verification path.

## Command

Run:

```bash
npm run verify:api
```

This executes `tools/api-verifier/check-node-api.ts`.

## What the unified verifier checks

- parses `@types/node` as the source-of-truth authoring surface
- parses the generated `@tsonic/nodejs` package output from `../nodejs/versions/10`
- scans public C# source types under `src/nodejs`
- verifies internal consistency between:
  - `node-aliases.d.ts`
  - generated `bindings.json`
  - generated facades / internal declarations
- reports name-level API coverage against Node's builtin modules
- writes:
  - `tools/VERIFICATION-SUMMARY.md`
  - `tools/verification-report.md`
  - `tools/verification-report.json`

## Failure policy

The verifier fails the build on internal contract errors such as:

- missing `node:` or bare alias declarations
- missing module bindings
- divergence between `node:fs` and `fs`
- divergence between declarations and generated bindings

It does **not** fail merely because Node coverage is incomplete. Coverage gaps are reported, not silently ignored.

## Why this is the right split

- internal consistency is a correctness invariant and must be green before publish
- raw Node parity is a roadmap/coverage metric and needs a report, not a blanket fail

## Publish gate

`../nodejs/scripts/selftest.sh` runs this verifier before runtime tests and
end-to-end consumer tests. That makes API-shape regressions publish-blocking.
