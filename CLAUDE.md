# CLAUDE.md

This file provides guidance to Claude Code when working with the Tsonic Standard Library project.

## Project Philosophy

**This is NOT a Node.js compatibility layer.** This is a **.NET-native standard library** inspired by Node.js APIs.

- **Node.js-like, not Node.js** - APIs are familiar but adapted to .NET idioms
- **Pragmatic over pure** - Deviate from Node.js when .NET offers better solutions
- **Performance matters** - Leverage .NET BCL for optimal performance
- **NativeAOT first** - All code must be NativeAOT compatible

When implementing or modifying APIs:
1. Start with Node.js API as inspiration
2. Adapt to .NET conventions and capabilities
3. Document differences clearly
4. Prioritize .NET performance and idioms over exact Node.js compatibility

## Critical Guidelines

### NEVER ACT WITHOUT EXPLICIT USER APPROVAL

**YOU MUST ALWAYS ASK FOR PERMISSION BEFORE:**

- Making architectural decisions or changes
- Implementing new features or functionality
- Modifying API implementations or type mappings
- Changing metadata structure or bindings format
- Adding new dependencies or packages
- Making changes to the three-file system (types/metadata/bindings)

**ONLY make changes AFTER the user explicitly approves.** When you identify issues or potential improvements, explain them clearly and wait for the user's decision. Do NOT assume what the user wants or make "helpful" changes without permission.

### ANSWER QUESTIONS AND STOP

**CRITICAL RULE**: If the user asks you a question - whether as part of a larger text or just the question itself - you MUST:

1. **Answer ONLY that question**
2. **STOP your response completely**
3. **DO NOT continue with any other tasks or implementation**
4. **DO NOT proceed with previous tasks**
5. **Wait for the user's next instruction**

This applies to ANY question, even if it seems like part of a larger task or discussion.

### NEVER USE AUTOMATED SCRIPTS FOR FIXES

**🚨 CRITICAL RULE: NEVER EVER attempt automated fixes via scripts or mass updates. 🚨**

- **NEVER** create scripts to automate replacements (PowerShell, bash, Python, etc.)
- **NEVER** use sed, awk, grep, or other text processing tools for bulk changes
- **NEVER** write code that modifies multiple files automatically
- **ALWAYS** make changes manually using the Edit tool
- **Even if there are hundreds of similar changes, do them ONE BY ONE**

Automated scripts break syntax in unpredictable ways and destroy codebases.

### NEVER USE GH PR COMMANDS

**🚨 CRITICAL RULE: NEVER use `gh pr` commands for creating pull requests. 🚨**

- **NEVER** run `gh pr create` or any other `gh pr` commands
- **NEVER** attempt to create pull requests via CLI
- **ONLY** push branches to remote with `git push`
- The user will create pull requests manually through the GitHub web interface

When asked to commit and push changes:
1. Commit changes with detailed commit messages
2. Push to a feature branch (NOT directly to main)
3. STOP - do not attempt to create pull requests

### GIT SAFETY RULES

#### NEVER DISCARD UNCOMMITTED WORK

**🚨 CRITICAL RULE: NEVER use commands that permanently delete uncommitted changes. 🚨**

These commands cause **PERMANENT DATA LOSS** that cannot be recovered:

- **NEVER** use `git reset --hard`
- **NEVER** use `git reset --soft`
- **NEVER** use `git reset --mixed`
- **NEVER** use `git reset HEAD`
- **NEVER** use `git checkout -- .`
- **NEVER** use `git checkout -- <file>`
- **NEVER** use `git restore` to discard changes
- **NEVER** use `git clean -fd`

**Why this matters for AI sessions:**
- Uncommitted work is invisible to future AI sessions
- Once discarded, changes cannot be recovered
- AI cannot help fix problems it cannot see

**What to do instead:**

| Situation | ❌ WRONG | ✅ CORRECT |
|-----------|---------|-----------|
| Need to switch branches | `git checkout main` (loses changes) | Commit first, then switch |
| Made mistakes | `git reset --hard` | Commit to temp branch, start fresh |
| Want clean slate | `git restore .` | Commit current state, then revert |
| On wrong branch | `git checkout --` | Commit here, then cherry-pick |

**Safe workflow:**
```bash
# Always commit before switching context
git add -A
git commit -m "wip: current progress on feature X"
git checkout other-branch

# If commit was wrong, fix with new commit or revert
git revert HEAD  # Creates new commit that undoes last commit
# OR
git commit -m "fix: correct the previous commit"
```

#### NEVER USE GIT STASH

**🚨 CRITICAL RULE: NEVER use git stash - it hides work and causes data loss. 🚨**

- **NEVER** use `git stash`
- **NEVER** use `git stash push`
- **NEVER** use `git stash pop`
- **NEVER** use `git stash apply`
- **NEVER** use `git stash drop`

**Why stash is dangerous:**
- Stashed changes are invisible to AI sessions
- Easy to forget what's stashed
- Stash can be accidentally dropped
- Causes merge conflicts when applied
- No clear history of when/why stashed

**What to do instead - Use WIP branches:**

```bash
# Instead of stash, create a timestamped WIP branch
git checkout -b wip/feature-name-$(date +%Y%m%d-%H%M%S)
git add -A
git commit -m "wip: in-progress work on feature X"
git push -u origin wip/feature-name-$(date +%Y%m%d-%H%M%S)

# Now switch to other work safely
git checkout main
# ... do other work ...

# Return to your WIP later
git checkout wip/feature-name-20251108-084530
# Continue working...

# When done, squash WIP commits or rebase
git rebase -i main
```

**Benefits of WIP branches over stash:**
- ✅ Work is visible in git history
- ✅ Work is backed up on remote
- ✅ AI can see the work in future sessions
- ✅ Can have multiple WIP branches
- ✅ Clear timestamps show when work was done
- ✅ Can share WIP with others if needed

#### NEVER DELETE BRANCHES

**🚨 CRITICAL RULE: NEVER delete branches unless explicitly requested by the user. 🚨**

- **NEVER** use `git branch -d` or `git branch -D`
- **NEVER** use `git push --delete` or `git push origin :branchname`
- **NEVER** delete branches "to clean up" or "for tidiness"

**Why:**
- Branches preserve work history
- User may want to reference old branches
- Deleted branches are hard to recover
- Let the user decide what to keep

**What to do instead:**
- Leave branches alone after merging
- User will clean up branches when they want
- If you think a branch should be deleted, suggest it but don't do it

#### Safe Branch Switching

**ALWAYS commit before switching branches:**

```bash
# Check current status
git status

# If there are changes, commit them first
git add -A
git commit -m "wip: current state before switching"

# NOW safe to switch
git checkout other-branch
```

**If you accidentally started work on wrong branch:**

```bash
# DON'T use git reset or git checkout --
# Instead, commit the work here
git add -A
git commit -m "wip: work started on wrong branch"

# Create correct branch from current state
git checkout -b correct-branch-name

# Previous branch will still have the commit
# You can cherry-pick it or just continue on new branch
```

#### Recovery from Mistakes

If you realize you made a mistake AFTER committing:

```bash
# ✅ CORRECT: Create a fix commit
git commit -m "fix: correct the mistake from previous commit"

# ✅ CORRECT: Revert the bad commit
git revert HEAD

# ❌ WRONG: Try to undo with reset
git reset --hard HEAD~1  # NEVER DO THIS - loses history
```

**If you accidentally committed to main:**

```bash
# DON'T panic or use git reset
# Just create a feature branch from current position
git checkout -b feat/your-feature-name

# Push the branch
git push -u origin feat/your-feature-name

# When merged, it will fast-forward (no conflicts)
# Main will catch up to the same commit
```

### WORKING DIRECTORIES

**IMPORTANT**: Never create temporary files in the project root or src directories. Use dedicated gitignored directories for different purposes.

#### .tests/ Directory (Test Output Capture)

**Purpose:** Save test run output for analysis without re-running tests

**Usage:**
```bash
# Create directory (gitignored)
mkdir -p .tests

# Run tests with tee - shows output AND saves to file
dotnet test | tee .tests/run-$(date +%s).txt

# Analyze saved output later without re-running:
grep "Failed" .tests/run-*.txt
tail -50 .tests/run-*.txt
grep -A10 "specific test name" .tests/run-*.txt
```

**Benefits:**
- See test output in real-time (unlike `>` redirection)
- Analyze failures without expensive re-runs
- Keep historical test results for comparison
- Search across multiple test runs

**Key Rule:** ALWAYS use `tee` for test output, NEVER plain redirection (`>` or `2>&1`)

#### .analysis/ Directory (Research & Documentation)

**Purpose:** Keep analysis artifacts separate from source code

**Usage:**
```bash
# Create directory (gitignored)
mkdir -p .analysis

# Use for:
# - API coverage reports
# - Type definition comparisons
# - Metadata validation output
# - Performance benchmarking results
# - Architecture diagrams and documentation
# - Temporary debugging scripts
```

**Benefits:**
- Keeps analysis work separate from source code
- Allows iterative analysis without cluttering repository
- Safe place for temporary debugging scripts
- Gitignored - no risk of committing debug artifacts

#### .todos/ Directory (Persistent Task Tracking)

**Purpose:** Track multi-step tasks across conversation sessions

**Usage:**
```bash
# Create task file: YYYY-MM-DD-task-name.md
# Example: 2025-11-03-buffer-implementation.md

# Task file must include:
# - Task overview and objectives
# - Current status (completed work)
# - Detailed remaining work list
# - Important decisions made
# - Files affected (C#, .d.ts, metadata, bindings)
# - Testing requirements
# - Special considerations

# Mark complete: YYYY-MM-DD-task-name-COMPLETED.md
```

**Benefits:**
- Resume complex tasks across sessions with full context
- No loss of progress or decisions
- Gitignored for persistence

**Note:** All three directories (`.tests/`, `.analysis/`, `.todos/`) should be added to `.gitignore`

## Session Startup

### First Steps When Starting a Session

When you begin working on this project, you MUST:

1. **Read this entire CLAUDE.md file** to understand the project conventions
2. **Review the three-file system** architecture (types/*.d.ts, types/*.metadata.json, types/nodejs.bindings.json)
3. **Check @types/node** as source of truth for API definitions
4. **Review api-coverage.md** to understand implementation status

Only after reading these documents should you proceed with implementation tasks.

## Project Overview

**Tsonic Node API** is a .NET implementation of Node.js APIs for the Tsonic compiler. It enables TypeScript code compiled to C# to use familiar Node.js modules like `fs`, `path`, and `events`.

## Architecture

### Three-File System

Every module requires three companion files:

1. **TypeScript Declarations** (`types/*.d.ts`)
   - Standard TypeScript type definitions
   - Must match official @types/node definitions as closely as possible
   - Enable IDE type-checking and autocomplete

2. **Metadata** (`types/*.metadata.json`)
   - Encodes C# semantics (virtual/override, static, struct vs class)
   - Used by Tsonic compiler for correct C# code generation
   - Format matches tsonic-runtime pattern
   - **AUTOGENERATED**: Do NOT manually create these files. They are generated by `../generatedts` tool

3. **Bindings** (`types/nodejs.bindings.json`)
   - Maps JavaScript module names to CLR types
   - Single file for all modules
   - Example: `"fs"` → `"nodejs.fs"`

### Code Organization

```
src/nodejs/          # C# implementation
├── path.cs                   # Static class with lowercase methods
├── fs.cs                     # Static class for file operations
└── EventEmitter.cs           # Instance class

types/                        # TypeScript view
├── path.d.ts                 # Copied from @types/node
├── fs.d.ts                   # Simplified (sync + async, no fs/promises namespace)
├── events-simple.d.ts        # Simplified EventEmitter
├── *.metadata.json           # Per-module metadata
├── nodejs.bindings.json  # Single bindings file
└── index.d.ts                # Triple-slash references

tests/nodejs.Tests/   # xUnit tests
├── PathTests.cs
├── FsTests.cs
└── EventEmitterTests.cs
```

## Implementation Guidelines

### C# Code Style

1. **Static classes for modules**: `path`, `fs` are static classes
2. **Lowercase method names**: Use JavaScript conventions (e.g., `join()`, `readFileSync()`)
3. **Suppress CS8981**: Warning for lowercase names (done in Directory.Build.props)
4. **XML documentation**: Required for all public members
5. **Nullable annotations**: Use `string?` for optional parameters
6. **Platform-specific**: Use `Path.DirectorySeparatorChar`, `RuntimeInformation.IsOSPlatform()`

### Type Definitions

1. **Match @types/node**: Use official Node.js type definitions as source of truth
2. **Simplify when needed**: We implement both sync and async APIs. Remove features not yet implemented (e.g., streams in some modules, fs/promises namespace)
3. **Triple-slash references**: Use in `index.d.ts` to aggregate modules
4. **Module declarations**: Always use `declare module "moduleName"`

### Metadata Format

```json
{
  "assemblyName": "nodejs",
  "types": {
    "nodejs.moduleName": {
      "kind": "class",
      "isStatic": true,
      "members": {
        "methodName(params)": {
          "kind": "method",
          "isStatic": true
        }
      }
    }
  }
}
```

### Testing

1. **Use xUnit**: Standard .NET testing framework
2. **Temp directories**: Create unique temp dirs for file tests (see `FsTests.cs`)
3. **IDisposable cleanup**: Clean up test files/dirs in Dispose()
4. **Test edge cases**: Empty strings, null, non-existent files
5. **Platform-specific**: Guard Windows/Unix-specific tests

## Common Tasks

### Adding a New Module

1. Create C# implementation in `src/nodejs/<module>.cs`
2. Copy or create `.d.ts` file in `types/<module>.d.ts`
3. Add binding to `types/nodejs.bindings.json`
4. Add reference in `types/index.d.ts`
5. Create tests in `tests/nodejs.Tests/<Module>Tests.cs`
6. Update `docs/api-coverage.md`

Note: Metadata files (`types/<module>.metadata.json`) are autogenerated by `../generatedts` - do NOT create them manually.

### Updating Official Type Definitions

When Node.js types change:

```bash
npm update @types/node
cp node_modules/@types/node/<module>.d.ts types/
# Metadata will be regenerated automatically by ../generatedts
```

### Running Tests

**IMPORTANT:** Always use `tee` to capture test output for later analysis:

```bash
# Run all tests with output capture
dotnet test | tee .tests/run-$(date +%s).txt

# Run specific test class with output capture
dotnet test --filter "ClassName=PathTests" | tee .tests/path-$(date +%s).txt

# Run with verbose output and capture
dotnet test --logger "console;verbosity=detailed" | tee .tests/verbose-$(date +%s).txt

# Analyze previous test runs without re-running
grep "Failed" .tests/run-*.txt
tail -100 .tests/run-*.txt
```

### Building and Packaging

```bash
dotnet build                            # Build solution
dotnet pack -c Release                  # Create NuGet package
npm pack                                # Create npm package
```

## Reference Projects

- **tsonic-runtime**: Main runtime at `../tsonic-runtime`
  - Follow same patterns for metadata/bindings
  - Reference for console, Math, JSON implementations
- **@types/node**: Official Node.js type definitions
  - Source of truth for API surface
  - Installed in `node_modules/@types/node/`

## Current Status (v1.0)

### Implemented ✅
- **path**: 100% coverage (all 16 methods)
- **fs**: Core sync methods (13 methods)
- **events**: Full EventEmitter (17 methods)

### Not Implemented ⏳
- Async fs operations (fs/promises)
- File descriptor operations (open, read, write, close)
- Symbolic links (symlink, readlink, lchmod, lchown)
- Advanced permissions (chmod, chown, access)
- Process module
- Streams (ReadStream, WriteStream)
- Other modules (os, timers, util, etc.)

See `api-coverage.md` for detailed analysis.

## Known Issues / Limitations

1. **matchesGlob**: Simplified implementation, not full glob syntax
2. **File permissions**: Limited on Windows (mode/chmod may not work)
3. **Stats properties**: Missing some Unix-specific fields (dev, ino, uid, gid)
4. **EventEmitter generics**: No TypeScript generic support yet
5. **Path separators**: Automatically uses platform defaults (can't force Windows/POSIX style)

## Best Practices

1. **Read official types first**: Always check @types/node before implementing
2. **Test with real files**: Use temp directories, verify file contents
3. **Match Node.js behavior**: Even edge cases (empty strings, null handling)
4. **Document differences**: Note any deviations from Node.js in XML comments
5. **NativeAOT compatible**: No reflection, trim-safe code only

## Debugging Tips

1. **Check bindings**: Verify module name maps to correct C# type
2. **Verify metadata**: Ensure method signatures match exactly (params, types)
3. **Type definition errors**: Run `npx tsc --noEmit` to validate `.d.ts` files
4. **Build errors**: Check `TreatWarningsAsErrors` - all warnings must be fixed
5. **Test failures**: Use `--logger "console;verbosity=detailed"` for full output

## Performance Notes

- Use `StringBuilder` for string concatenation in loops
- Prefer `AsSpan()` for substring operations when possible
- File operations are already optimized by .NET BCL
- EventEmitter uses `List<Delegate>` - efficient for typical listener counts

## Security Considerations

- Path traversal: Use `Path.GetFullPath()` to resolve paths safely
- File permissions: Respect system file permissions
- No shell commands: Pure .NET APIs only (no Process.Start)
- Input validation: Validate all string inputs (non-null, non-empty when required)

## Git Workflow

### Branch Strategy

1. **NEVER commit to main directly**
2. **Create feature branches**: `feat/feature-name` or `fix/bug-name`
3. **Verify branch before commit**: `git branch --show-current`

### Commit Process

1. **Commit before switching contexts**: See Git Safety Rules above
2. **Run tests**: Ensure all tests pass with `dotnet test`
3. **Clear commit message**: Describe what and why
4. **No force push**: Never use `git push --force`

### Workflow Summary

**Critical rules (see detailed Git Safety Rules section above):**
1. ✅ **ALWAYS commit before switching contexts** - Even if work is incomplete
2. ✅ **NEVER discard uncommitted work** - Use WIP branches instead
3. ✅ **NEVER use git stash** - Use timestamped WIP branches
4. ✅ **NEVER use git reset --hard** - Use git revert for fixes
5. ✅ **NEVER delete branches** - Let user decide when to clean up
6. ✅ **Verify branch**: `git branch --show-current` before committing
7. ✅ **Push WIP branches**: Backup work on remote
8. ✅ **Use git revert not git reset** - To undo commits

**Standard workflow:**
```bash
# 1. Verify you're on correct branch
git branch --show-current

# 2. Make changes and commit frequently
git add -A
git commit -m "feat: descriptive message"

# 3. Test before pushing
dotnet test

# 4. Push to remote
git push
```

## Future Enhancements

Priority order if expanding:

1. **fs**: Add file descriptor operations (open, close, read, write)
2. **fs**: Add symbolic link support (symlink, readlink)
3. **process**: Implement basic process global (env, argv, cwd)
4. **events**: Add static utility methods (once, on)
5. **os**: Operating system utilities (platform, arch, tmpdir)
6. **util**: Utility functions (promisify, inherits, inspect)
