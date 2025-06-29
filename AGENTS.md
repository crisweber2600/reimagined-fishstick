# AGENTS CONFIG v0.4

## Root Agent Tasks for BDD & Tests
- Keep all tests green.
- Maintain **≥ 80 %** line-coverage (`dotnet test /p:CollectCoverage=true` gate).
- Require every human instruction in **Given / When / Then** BDD form.  
  - On free-form prose, translate to BDD, ask the human to confirm, then generate an implementation *Task*.
- Always propose a design first and request feedback before coding.
- Every BDD scenario must include a concrete **Examples** table.
- Store all new `.feature` files under `Common.Tests/BDD/`.

## Conventions
- **Epics** → `docs/goals/<slug>.md`
- **Features** → `features/<feature-slug>/`
- **Stories** → `.feature` files (Reqnroll / Gherkin 6)
- **Tasks**  → `public` / `internal` methods preceded by `/// TASK:` doc-comment

## Build & Test
| Action               | Command        |
|----------------------|----------------|
| Install deps         | `dotnet restore` |
| Run unit tests       | `dotnet test --no-build` |
| Run BDD scenarios    | `dotnet test`  |
| Coverage threshold   | `dotnet test /p:CollectCoverage=true` |

Codex **must** execute the full suite (incl. coverage) before proposing a commit.

## Autonomous-Agent Rules
1. Work through unchecked `[ ]` tasks in order (`@parallel` tasks may run concurrently).  
2. For each task  
   a. Draft a design (class / API sketch) and request human approval.  
   b. When approved, implement code & tests until scenarios pass and coverage ≥ 80 %.  
   c. Mark the task `[x]`, commit, and push.  
3. Never alter `[x]` or `[@blocked]` items.  
4. When all tasks of a *Story* are done, ensure its `.feature` file passes and mark the story **Done**.  
5. Always follow the **Root Agent Tasks** above.  
6. **Incoming Markdown Plans:**  
   - If a chat message contains **exactly one fenced code block tagged `markdown`**, treat the block’s contents as a plan file.  
   - Derive its path from the first level-1 heading (slugify to `docs/goals/<slug>.md`).
   - Write/overwrite that file, commit, then resume rule 1 with the refreshed plan.
   - **Verify the plan lists at least one unchecked `[ ]` task for each feature.**
     If any feature lacks an implementation step, append `[ ] Implement <feature>`
     under that feature and add `[ ] Ensure next step is clear for Codex` at the
     bottom.
   - When generating a new epic plan, include a final `## Codex Tasks` section so
     these tasks can be executed from the Codex interface.

## Project Plan Lookup
Find the active epic’s plan under `docs/goals/*.md` and follow its Feature → Story → Task hierarchy.

