# === agents.md (root template) ================================================
# AGENTS CONFIG v0.4

## Root Agent Tasks for BDD & Tests
- Keep all tests green.
- Maintain **≥ 80 %** line-coverage (`make coverage` gate).
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
| Install deps         | `make install` |
| Run unit tests       | `make test`    |
| Run BDD scenarios    | `dotnet test`  |
| Coverage threshold   | `make coverage`|

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

## Project Plan Lookup
Find the active epic’s plan under `docs/goals/*.md` and follow its Feature → Story → Task hierarchy.

# === LLM PROMPT : EpicDecomposer+PlanGenerator v0.4 ============================
**ROLE**: EpicDecomposer+PlanGenerator

**INPUTS**
- `EPIC_TITLE`: <string>
- `EPIC_DESCRIPTION`: <paragraph>

**OBJECTIVE**
Break the epic into Features → Stories → Tasks and emit a commit-ready plan file.

**GUIDELINES**
1. 1–5 stories per Feature; 3–8 tasks per Story.  
2. Each Story references a `.feature` filename in at least one task (place under `Common.Tests/BDD/<feature-slug>/`).  
3. Tasks are imperative, ≤ 1 method each, and cite files/classes when possible.  
4. Use nested Markdown list format:  

	•	Feature X: …
	•	Story X1: …
	•	Task 1: …

5. Insert an empty **Status Table** at the top of the file:  

Status Table (auto-updated)

Feature	Story	Task	State


6. No commentary outside the required output block.

**OUTPUT**
Return *exactly one* fenced code block tagged **markdown** containing the full content for  
`docs/goals/<slugified-epic-title>.md`.  
Provide nothing else.