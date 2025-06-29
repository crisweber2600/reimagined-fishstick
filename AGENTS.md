# AGENTS CONFIG v0.3

## Root Agent Tasks for BDD & Tests
- **Always keep tests green.**
- **Maintain ≥ 80 % line coverage** (`make coverage` will fail CI otherwise).
- Require all human instructions in **Given / When / Then** format.  
  - On free-form prose, **translate to BDD**, ask the human to confirm, then create a follow-up *Task* for implementation.
- **Propose a design first** (class diagram, API sketch, etc.); wait for feedback before writing code.
- Ensure every BDD scenario contains at least one concrete **Examples** table row.
- Store all new feature files under `Common.Tests/BDD/`.

## Conventions
- **Epics:** `docs/goals/<slug>.md`
- **Features:** `/features/<feature-slug>/`
- **Stories:** `.feature` files (Reqnroll, Gherkin 6)
- **Tasks:** `public` / `internal` methods flagged with `/// TASK:` doc-comment

## Build & Test
| Action                 | Command        |
|------------------------|----------------|
| Install dependencies   | `make install` |
| Run unit tests         | `make test`    |
| Run BDD (Reqnroll)     | `dotnet test`  |
| Coverage threshold     | `make coverage`|

Codex must run the full suite (including coverage) before proposing a commit.

## Autonomous-Agent Rules
1. Parse unchecked `[ ]` tasks in order. `@parallel` → concurrent OK.  
2. For each task  
   a. Draft **design** doc & request feedback.  
   b. On approval, implement code + tests until all scenarios pass & coverage ≥ 80 %.  
   c. Mark task `[x]`, commit, push.  
3. Never modify `[x]` or `[@blocked]` items.  
4. After a *Story* is complete, confirm its `.feature` file passes and mark story **Done**.  
5. Obey **Root Agent Tasks** at all times.

## Project Plan Lookup
> Locate the appropriate plan in `docs/goals/*.md` that matches the active epic.  
> Follow that markdown file to know which Feature, Story, and Task to execute next.