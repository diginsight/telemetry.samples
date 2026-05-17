# Context Engineering Principles for GitHub Copilot

**Purpose**: Consolidated best practices for crafting effective instructions, prompts, and agent personas for GitHub Copilot. This document serves as the single source of truth for context engineering principles across all file types.

**Referenced by**: `.github/instructions/prompts.instructions.md`, `.github/instructions/agents.instructions.md`, all prompt and agent files

---

## Core Principles

### 1. Narrow Scope, Specific Purpose

**Principle**: Each prompt, agent, or instruction file should have a single, well-defined responsibility.

**Why it matters**: 
- Reduces ambiguity in AI interpretation
- Makes files easier to maintain and debug
- Enables better composition through handoffs
- Improves caching and reuse efficiency

**Application**:
- ❌ **Bad**: "Create or update a prompt file, validate it, and update documentation"
- ✅ **Good**: Three separate agents: prompt-builder, prompt-validator, documentation-updater

**Guidelines**:
- One primary user goal per file
- Related sub-tasks are acceptable if they serve the primary goal
- Use handoffs for sequential workflows
- Use composition for parallel capabilities

### 2. Early Commands, Clear Structure

**Principle**: Place the most important instructions and commands at the beginning of your content.

**Why it matters**:
- AI models give more weight to earlier content
- Critical constraints must be established before detailed instructions
- Early structure aids AI's mental model formation

**Application**:
```markdown
<!-- HIGH PRIORITY - Place at top -->
1. Core objective statement
2. Critical constraints (NEVER/ALWAYS rules)
3. Primary workflow/process
4. Tool configuration
5. Handoff definitions

<!-- LOWER PRIORITY - Place after -->
6. Examples and edge cases
7. Quality checklists
8. Background context
9. Troubleshooting tips
```

**YAML Frontmatter Priority**:
Always place critical configuration in YAML frontmatter, not buried in Markdown:
```yaml
---
description: "Single-sentence role definition"  # AI reads this first
tools: ['read_file', 'editor']  # Tool scope defined immediately
agent: plan  # Behavioral constraints upfront
---
```

### 3. Imperative Language

**Principle**: Use direct, commanding language that leaves no room for interpretation.

**Why it matters**:
- Reduces probabilistic behavior in AI responses
- Creates enforceable boundaries
- Establishes clear expectations

**Command Hierarchy**:

| Strength | Phrase | Use Case | Example |
|----------|--------|----------|---------|
| **Absolute** | `NEVER`, `MUST NOT` | Hard boundaries, safety rules | "NEVER modify files outside .github/prompts/" |
| **Required** | `MUST`, `ALWAYS`, `WILL` | Core requirements | "You MUST validate YAML syntax before saving" |
| **Expected** | `SHOULD`, `PREFER` | Best practices, soft preferences | "You SHOULD use handoffs for multi-step workflows" |
| **Optional** | `CAN`, `MAY`, `CONSIDER` | Suggestions, alternatives | "You MAY ask for clarification if requirements are vague" |

**Examples**:

❌ **Weak** (probabilistic):
```markdown
It would be helpful if you could check for syntax errors.
Try to keep the file under 200 lines.
Consider using semantic search to find similar patterns.
```

✅ **Strong** (imperative):
```markdown
You MUST validate YAML syntax before saving any changes.
You WILL keep the file under 200 lines; factor into multiple files if needed.
You MUST use semantic_search to analyze at least 3 similar prompts before building.
```

### 4. Three-Tier Boundary System

**Principle**: Explicitly define what the AI MUST do, what requires approval, and what is forbidden.

**Why it matters**:
- Prevents overstepping boundaries (e.g., editing wrong files)
- Clarifies decision authority
- Reduces user interruptions for trivial decisions
- Establishes trust through predictable behavior

**Structure**:
```yaml
boundaries:
  always_do:  # No approval needed - execute immediately
    - "Validate YAML syntax before saving"
    - "Create backup files when updating"
    - "Use semantic_search before building new prompts"
  
  ask_first:  # Requires user approval before proceeding
    - "Modify existing validated prompts"
    - "Change agent handoff workflows"
    - "Delete any prompt files"
  
  never_do:  # Hard boundaries - refuse with explanation
    - "Modify instruction files without explicit request"
    - "Remove validation metadata from bottom YAML"
    - "Create prompts that violate repository conventions"
```

**Application to File Types**:

| File Type | Always Do | Ask First | Never Do |
|-----------|-----------|-----------|----------|
| **Prompts** | Validate syntax, check tool availability | Modify workflow logic, change tool lists | Break YAML format, remove metadata |
| **Agents** | Validate handoff targets, check tool scope | Add new tools, modify persona | Grant unrestricted file access |
| **Instructions** | Verify `applyTo` patterns | Change core principles | Create circular dependencies |

### 5. Context Minimization

**Principle**: Include only the context necessary for the specific task; reference external sources for additional details.

**Why it matters**:
- Reduces token usage and execution cost
- Improves AI focus on relevant information
- Makes files easier to maintain (single source of truth)
- Enables better composition and reuse

**Techniques**:

**Use References Instead of Duplication**:
```markdown
❌ **Bad**: Embed 100 lines of context engineering principles in every prompt

✅ **Good**: 
For context engineering principles, see `.copilot/context/00.00 prompt-engineering/01-context-engineering-principles.md`
For tool composition patterns, see `.copilot/context/00.00 prompt-engineering/02-tool-composition-guide.md`
```

**Group References Over Individual Files**:

When referencing context files from `.github/` files (prompts, agents, instructions), prefer **group references** (folder-level) over individual file references. This improves maintainability and allows context files to evolve without breaking references.

```markdown
✅ **Good** - Group reference (folder-level):
📖 **Complete guidance:** [.copilot/context/00.00 prompt-engineering/](.copilot/context/00.00 prompt-engineering/)

❌ **Avoid** - Individual file reference (unless specifically needed):
📖 **Complete guidance:** [.copilot/context/00.00 prompt-engineering/01-context-engineering-principles.md](.copilot/context/00.00 prompt-engineering/01-context-engineering-principles.md)
```

**When to use individual file references:**
- Linking to a specific section or concept (e.g., "See [validation-caching-pattern.md] for the 7-day rule")
- Cross-referencing between context files within the same folder
- Deep-linking to troubleshooting or examples

**Benefits of group references:**
- Files can be added, renamed, or reorganized without updating all references
- Readers discover all related content in the folder
- Reduces maintenance burden as context evolves

**Selective File Reads**:
```markdown
❌ **Bad**: "Read all files in .github/prompts/ before starting"

✅ **Good**: "Use semantic_search with query 'validation workflow' to find 3 relevant prompts"
```

**Lazy Loading Pattern**:
```markdown
## Process

1. **Gather Requirements** - Minimal context, user interaction only
2. **Research Phase** - NOW use semantic_search, read_file, grep_search
3. **Build Phase** - Access templates and examples
4. **Validate Phase** - Load validation rules and checklists
```

### 6. Tool Scoping and Security

**Principle**: Grant only the minimum tools necessary for the agent's role; use `agent: plan` for read-only operations.

**Why it matters**:
- Prevents accidental file modifications
- Reduces risk of cascading errors
- Makes agent behavior more predictable
- Aligns with principle of least privilege

**Tool Categories**:

| Category | Tools | Risk Level | Use For |
|----------|-------|------------|---------|
| **Read-Only** | `read_file`, `grep_search`, `semantic_search`, `list_dir`, `file_search` | ✅ Low | Research, analysis, validation |
| **Inspection** | `get_errors`, `copilot_getNotebookSummary` | ✅ Low | Quality checks, diagnostics |
| **Write** | `create_file`, `replace_string_in_file`, `multi_replace_string_in_file` | ⚠️ Medium | Content creation, targeted updates |
| **Destructive** | File deletion, workspace restructuring | 🚫 High | Avoid; use terminal with approval |
| **External** | `fetch_webpage`, `github_repo`, `run_in_terminal` | ⚠️ Medium | Research, environment setup |

**Agent Type Guidelines**:

```yaml
# Read-only agent (researcher, validator)
---
agent: plan
tools: ['read_file', 'semantic_search', 'grep_search', 'file_search']
---

# Builder agent (needs write access)
---
agent: agent  # Default, allows file creation
tools: ['read_file', 'semantic_search', 'create_file', 'replace_string_in_file']
---

# Orchestrator prompt (delegates, minimal tools)
---
tools: ['read_file', 'semantic_search']  # Only for Phase 1 analysis
handoffs:
  - label: "Research Requirements"
    agent: prompt-researcher
    send: true
---
```

**Security Boundaries**:
```markdown
## Tool Boundaries

✅ **ALWAYS ALLOWED**:
- Read any file in `.github/` directories
- Search workspace with semantic_search or grep_search
- Validate syntax without modifying files

⚠️ **ASK FIRST**:
- Create new files in `.github/agents/` or `.github/prompts/`
- Modify existing prompts or agents
- Install packages or extensions

🚫 **NEVER ALLOWED**:
- Modify files outside `.github/` and `.copilot/` directories
- Delete prompt or agent files without explicit user request
- Execute terminal commands that modify system configuration
```

---

### 7. Explicit Uncertainty Management

**Principle**: Define professional "I don't know" responses and data gap handling patterns that prevent hallucination and maintain user trust.

**Why it matters**:
- AI models have a bias toward providing answers even when lacking sufficient context
- Admitting uncertainty prevents confidently stated misinformation
- Explicit templates create consistent, professional uncertainty handling
- Missing data scenarios are inevitable in production systems
- Referenced from: **Mario Fontana's "6 VITAL Rules for Production-Ready Copilot Agents"** (Rule 1: Data Gaps, Rule 2: "I Don't Know")

---

### 8. Template-First Authoring (Token Efficiency)

**Principle**: **PREFER template files** over verbose embedded descriptions for output formats, input schemas, document structures, and workflows. Externalize to reusable template files rather than embedding inline.

**Core Rule**: If content exceeds 10 lines and could be reused, externalize it to a template.

**Why it matters**:
- **Token efficiency**: Large inline formats consume context budget; templates load only when needed
- **Flexibility**: Different templates for different conditions (simple vs. complex output, different audiences)
- **Consistency**: Single source of truth for output formatting across multiple prompts/agents
- **Maintainability**: Update format once, all references benefit
- **Composability**: Templates can be combined or swapped without modifying prompt logic

**When to Use Templates (Quick Reference)**:

| Content Type | ❌ Don't Embed | ✅ Use Template |
|--------------|----------------|------------------|
| **Output formats** | Multi-line output examples inline | `output-*.template.md` |
| **Input schemas** | Detailed input field descriptions | `input-*.template.md` |
| **Document structures** | Section-by-section layout specs | `*-structure.template.md` |
| **Validation checklists** | Long inline checklists (>12 items) | Instruction file or template |
| **Multi-step workflows** | Phase descriptions > 20 lines | Phase templates or context files |
| **Report layouts** | Summary formats > 8 lines | `*-summary.template.md` |

**What to externalize (with thresholds):**

| Content Type | Inline Size Threshold | Template Location |
|--------------|----------------------|-------------------|
| **Output Formats** | > 10 lines | `.github/templates/output-*.template.md` |
| **Validation Reports** | > 15 lines | `.github/templates/report-*.template.md` |
| **Input Collection Guides** | > 10 lines | `.github/templates/guidance-input-*.template.md` |
| **Summary Layouts** | > 8 lines | `.github/templates/*-summary-*.template.md` |
| **Checklists** | > 12 items | `.github/templates/checklist-*.template.md` |
| **Phase Output Formats** | > 15 lines per phase | `.copilot/context/*/phase-*.md` |

**Application pattern:**

❌ **Bad** (embedded verbose format):
```markdown
## Output Format

**Complete validation report with:**

```markdown
### Validation Results

**File:** [filename]
**Checked:** [timestamp]

#### Grammar Analysis
- **Score:** [X/100]
- **Issues Found:** [count]
  - Line [N]: [issue description]
  - Line [N]: [issue description]
  ...

#### Readability Analysis
- **Flesch-Kincaid Grade:** [X.X]
- **Reading Time:** [X min]
...
[50 more lines of format specification]
```

✅ **Good** (template reference with conditional):
```markdown
## Output Format

**📄 Format Templates:** `.github/templates/`

Select template based on validation depth:
- **Quick validation**: Use `report-validation-quick.template.md`
- **Standard validation**: Use `report-validation-standard.template.md`  
- **Deep validation**: Use `report-validation-comprehensive.template.md`

Load selected template with `read_file` and populate with results.
```

**Conditional template selection:**

```markdown
### Template Selection Logic

| Condition | Template | Purpose |
|-----------|----------|---------|
| Simple task, < 3 outputs | `output-minimal.template.md` | Fast response |
| Standard task | `output-standard.template.md` | Default format |
| Complex/orchestration | `output-detailed.template.md` | Full trace |
| Error scenario | `output-error.template.md` | Recovery focus |
| User-facing report | `output-report.template.md` | Polished format |
```

**Token savings estimation:**

| Inline Format Lines | Approx. Tokens | Template Reference | Savings |
|---------------------|----------------|-------------------|---------|
| 20 lines | ~120 tokens | 2 lines (~12 tokens) | ~90% |
| 50 lines | ~300 tokens | 3 lines (~18 tokens) | ~94% |
| 100 lines | ~600 tokens | 4 lines (~24 tokens) | ~96% |

**When to keep inline:**
- Format is < 10 lines (overhead of reference not worth it)
- Format is unique to this prompt (never reused)
- Format requires heavy dynamic interpolation with many variables

**Template Location & Naming Conventions:**

| Purpose | Location | Naming Pattern | Example |
|---------|----------|----------------|---------|
| **Output formats** | `.github/templates/` | `output-{purpose}.template.md` | `output-prompt-validation-phases.template.md` |
| **Input schemas** | `.github/templates/` | `input-{purpose}.template.md` | `input-article-metadata.template.md` |
| **Document structures** | `.github/templates/` | `{type}-structure.template.md` | `promptengineering-instruction-structure.template.md` |
| **Guidance sections** | `.github/templates/` | `guidance-{topic}.template.md` | `guidance-input-collection.template.md` |
| **Domain-specific** | `.github/templates/` | `{domain}-*.template.md` | `recording-summary-template.md` |

**Template Reference Pattern:**

```markdown
❌ **Don't** embed verbose format inline:
## Output Format
### Section 1: Summary
- Title (H1)
- Overview paragraph
[...50+ lines...]

✅ **Do** reference template:
## Output Format
**Use template:** `.github/templates/output-summary.template.md`
```

**Existing templates:** See `.github/templates/` (26+ reusable templates available)

**Integration with Principle 5 (Context Minimization):**
Template externalization is a specific application of context minimization for structured outputs. While Principle 5 focuses on *reference knowledge*, Principle 8 addresses *output scaffolding*—both reduce inline token consumption

**Three-Part "I Don't Know" Template**:

```markdown
I couldn't find [specific information] in [locations searched].
I did find [partial/related context] in [location].
Recommendation: [escalation path or alternative approach]
```

**Why this structure works**:
1. **Transparency**: Explicit about what's missing
2. **Partial Value**: Shares what *was* found
3. **Actionable**: Suggests next steps

**Application Examples**:

**Validation Agent** (missing metadata):
```markdown
I couldn't find validation metadata in the bottom YAML comment block.
I did find top-level Quarto metadata (title, author, date).
Recommendation: Add validation metadata block using template from 02-dual-yaml-metadata.md.
```

**Implementation Agent** (no pattern found):
```markdown
I couldn't find existing patterns for Azure Functions HTTP trigger authentication in .github/templates/.
I did find related patterns: Azure Functions timer trigger (tech/02.01 Azure/01. Azure Functions/timer-example.md).
Recommendation: Proceed with Microsoft official documentation pattern or request reference file.
```

**Orchestrator Agent** (agent doesn't exist):
```markdown
Agent @azure-deployment-specialist doesn't exist in .github/agents/.
Available Azure-related agents: @prompt-validator, @agent-builder.
Recommendation: Use @agent-builder to create azure-deployment-specialist or use generic implementation agent.
```

**Data Gap Scenarios to Define**:

| Scenario | Response Pattern |
|----------|------------------|
| **Information Missing** | "Couldn't find X in Y. Did find Z. Recommend: [action]" |
| **Ambiguous Requirements** | "Could mean A or B. Which interpretation? [list trade-offs]" |
| **Tool Failure** | "Tool failed: [reason]. Tried: [fallbacks]. Recommend: [escalation]" |
| **Out of Scope** | "This requires [capability not in tools]. Recommend: [manual step / alternative tool]" |
| **Conflicting Data** | "Found contradictory patterns: [list with evidence]. Recommend: [user guidance needed]" |

**Error Recovery Workflows**:

```markdown
### When `semantic_search` Returns Nothing
1. Try `grep_search` with specific keywords
2. Try `file_search` with glob patterns
3. Report: "Couldn't find matches. Searched: [tools tried]. Recommend: verify search terms or provide file path."

### When `read_file` Fails
1. Verify file path with `file_search`
2. Check if file exists with `list_dir`
3. Report: "Cannot read [file]. Reason: [error]. Tried: [verification steps]. Recommend: verify path or permissions."

### When Reference Files Don't Exist
1. Search for similar patterns in related directories
2. Check instruction files for guidance
3. Report: "Reference file [path] doesn't exist. Similar files: [list]. Recommend: use similar pattern or ask for correct path."
```

**Integration with Templates**:
All prompt templates now include:
- **Response Management section** with scenario-specific "I don't know" patterns
- **Embedded Test Scenarios** including "plausible trap" tests (user provides incorrect info, agent should catch it)

**Anti-Patterns to Avoid**:

❌ **Bad** (hallucination):
```markdown
Based on common patterns, I'll implement authentication using JWT tokens with Azure AD B2C...
```

✅ **Good** (explicit uncertainty):
```markdown
I couldn't find authentication patterns in this repository.
I did find Azure AD documentation reference in tech/01.01 Authentication/.
Recommendation: Review existing auth implementations or provide reference file before proceeding.
```

**Confidence Indicators**:
When conclusions must be drawn from partial data, qualify with confidence levels:

```markdown
**High Confidence**: Found 5+ consistent examples in codebase
**Medium Confidence**: Found 2-4 examples, some variation in approach
**Low Confidence**: Found 1 example or conflicting patterns
**No Confidence**: No examples found, proceeding with industry best practices
```

---

## Application by File Type

### For Prompt Files (.prompt.md)

**Focus**: Task-specific workflows with clear start and end conditions

**Context Engineering Priorities**:
1. **Narrow Scope**: Single task (e.g., "grammar review" not "full validation")
2. **Early Commands**: Process steps first, examples last
3. **Imperative Language**: "You MUST validate..." not "It's good to validate..."
4. **Three-Tier Boundaries**: Define always_do, ask_first, never_do
5. **Context Minimization**: Reference context files, don't embed principles
6. **Tool Scoping**: Only tools needed for this specific task

**Template Section Order**:
```markdown
---
# YAML frontmatter with tools, agent type
---

# [Prompt Name]

**YOUR ROLE**: [One sentence, imperative]

## Process
[Numbered steps, imperative verbs]

## Boundaries
### ✅ Always Do
### ⚠️ Ask First
### 🚫 Never Do

## Output Format
[Specific structure requirements]

## Quality Checklist
[Verification steps]

## Examples
[Edge cases and patterns]
```

### For Agent Files (.agent.md)

**Focus**: Persistent persona with role-specific expertise

**Context Engineering Priorities**:
1. **Narrow Scope**: Single role (e.g., "researcher" not "researcher and builder")
2. **Early Commands**: Persona definition first, then capabilities
3. **Imperative Language**: "You MUST handoff to..." not "Consider handing off..."
4. **Three-Tier Boundaries**: Especially critical for file access
5. **Context Minimization**: Reference style guides, don't embed full guides
6. **Tool Scoping**: Minimal set for role (researcher = read-only)

**Agent Persona Elements**:
1. **Core Identity**: "You are a research specialist..."
2. **Expertise**: "You excel at pattern discovery and requirement analysis..."
3. **Boundaries**: "You NEVER create or modify files; you prepare research reports..."
4. **Collaboration**: "You handoff to prompt-builder when research is complete..."
5. **Communication Style**: "You present findings in structured reports with evidence..."

### For Instruction Files (.instructions.md)

**Focus**: Context-specific rules auto-applied based on glob patterns

**Context Engineering Priorities**:
1. **Narrow Scope**: Rules for one file type or directory (via `applyTo`)
2. **Early Commands**: Critical constraints first, conventions last
3. **Imperative Language**: Universal rules that apply to all matching contexts
4. **Three-Tier Boundaries**: Universal never_do rules (e.g., "NEVER remove validation metadata")
5. **Context Minimization**: HIGH PRIORITY - Instructions should be concise, reference context files
6. **Tool Scoping**: Not applicable (instructions don't define tools directly)

**Optimal Length**: 80-120 lines (down from 200-400 with context consolidation)

**Content Guidelines**:
```markdown
✅ **INCLUDE IN INSTRUCTIONS**:
- Repository-specific conventions (e.g., "All prompts use dual YAML metadata")
- File type patterns (e.g., "Prompt files must end with .prompt.md")
- Universal boundaries (e.g., "NEVER modify top YAML block from validation prompts")
- Quality standards (e.g., "All agent files MUST have a description field")

❌ **MOVE TO CONTEXT FILES**:
- Context engineering principles (→ context-engineering-principles.md)
- Tool composition patterns (→ tool-composition-guide.md)
- Detailed examples and templates (→ templates/)
- Process workflows (→ prompts or agents)
```

---

## Token Budget Guidelines

### Consolidated Length Recommendations

| File Type | Token Budget | Word Count | Line Count | Use Case |
|-----------|-------------|------------|------------|----------|
| **Prompt Files** | | | | |
| ├─ Simple Task | < 500 tokens | ~375 words | ~75 lines | Fast execution, single action |
| ├─ Multi-Step Workflow | < 1,500 tokens | ~1,100 words | ~220 lines | Phased execution, coordination |
| ├─ Orchestrator | < 2,500 tokens | ~1,875 words | ~375 lines | Multi-agent, complex handoffs |
| ├─ Scaffolding | < 2,000 tokens | ~1,500 words | ~300 lines | Template generation with examples |
| └─ Review/Audit | < 2,000 tokens | ~1,500 words | ~300 lines | Checklists, validation criteria |
| **Agent Files** | | | | |
| ├─ Specialist Agent | < 1,000 tokens | ~750 words | ~150 lines | Focused role, narrow tool set |
| ├─ Planner Agent | < 1,500 tokens | ~1,125 words | ~225 lines | Research, output formatting |
| ├─ Review Agent | < 1,200 tokens | ~900 words | ~180 lines | Validation, quality checks |
| └─ Orchestrator Agent | < 2,000 tokens | ~1,500 words | ~300 lines | Multi-phase workflows, handoffs |
| **Instruction Files** | | | | |
| ├─ Language/Framework | < 800 tokens | ~600 words | ~120 lines | TypeScript, Python, React rules |
| ├─ Project-wide | < 500 tokens | ~375 words | ~75 lines | Company coding standards |
| ├─ Specific Feature | < 300 tokens | ~225 words | ~45 lines | Error handling, auth pattern |
| └─ Repository Root | < 600 tokens | ~450 words | ~90 lines | `.github/copilot-instructions.md` |
| **Skill Files** | | | | |
| ├─ Description (YAML) | 50-100 tokens | ~15-25 words | 2-3 lines | Discovery metadata |
| ├─ SKILL.md Body | 500-1,500 tokens | 500-1,000 words | ~100-200 lines | Workflow instructions |
| ├─ Templates | Variable | ~150-375 words | 20-50 lines | Boilerplate code |
| └─ Examples | Variable | ~225-600 words | 30-80 lines | Pattern demonstrations |
| **Context Files** | | | | |
| ├─ Core Principles | 800-1,200 tokens | ~600-900 words | ~120-180 lines | Referenced frequently |
| ├─ Pattern Libraries | 1,500-2,500 tokens | ~1,100-1,875 words | ~220-375 lines | Multiple patterns with examples |
| ├─ Workflow Docs | 1,000-2,000 tokens | ~750-1,500 words | ~150-300 lines | Step-by-step procedures |
| └─ Glossary | 500-1,000 tokens | ~375-750 words | ~75-150 lines | Terminology, definitions |

### Conversion Reference

**Quick token estimation without tools:**

| From | To | Formula | Example |
|------|-----|---------|----------|
| Words | Tokens | Multiply by 1.33 | 600 words → ~800 tokens |
| Lines | Tokens | Multiply by 5-8 | 120 lines → ~720 tokens (avg 6/line) |
| Characters | Tokens | Divide by 4 | 3,000 chars → ~750 tokens |
| Tokens | Words | Divide by 1.33 | 1,000 tokens → ~750 words |

### Critical Thresholds

**When to refactor:**

| File Type | Warning | Critical | Action Required |
|-----------|---------|----------|------------------|
| Prompts | > 2,000 tokens | > 2,500 tokens | Split with handoffs or create orchestrator |
| Agents | > 1,500 tokens | > 2,000 tokens | Extract to instructions or split by role |
| Instructions | > 800 tokens | > 1,000 tokens | Split by layer or extract to context |
| Skills (Body) | > 1,200 tokens | > 1,500 tokens | Use progressive disclosure or extract resources |
| Context Files | > 2,500 tokens | > 3,000 tokens | Split by topic or create index file |

### Performance Impact

**Combined context consumption:**

```
Typical Prompt Execution:
  Prompt file: 800 tokens
+ Referenced agent: 1,000 tokens
+ Active instructions (3 files): 900 tokens
+ User question: 50 tokens
+ Code context: 2,000 tokens
= 4,750 tokens before AI responds ✅ Comfortable

Warning Scenario:
  Prompt file: 1,800 tokens
+ Referenced agent: 1,500 tokens  
+ Active instructions (6 files): 1,800 tokens
+ User question: 100 tokens
+ Code context: 5,000 tokens
= 10,200 tokens before AI responds ⚠️ Refactor recommended
```

**Context Budget Guidelines:**
- **Optimal**: < 10,000 tokens pre-response
- **Warning**: 10,000-15,000 tokens
- **Critical**: > 15,000 tokens (degraded performance)

### Measurement Tools

**Quick Estimation:**
```bash
# Word count (multiply by 1.33 for tokens)
wc -w .github/prompts/*.prompt.md

# Line count (multiply by 6 for rough tokens)
wc -l .github/instructions/*.instructions.md
```

**Accurate Token Counting:**
```python
import tiktoken

def count_tokens(file_path):
    enc = tiktoken.get_encoding('cl100k_base')
    with open(file_path, 'r', encoding='utf-8') as f:
        return len(enc.encode(f.read()))

# Check specific file
tokens = count_tokens('.github/prompts/my-prompt.prompt.md')
print(f"Token count: {tokens}")

# Batch analyze
import glob
for file in glob.glob('.github/**/*.md', recursive=True):
    tokens = count_tokens(file)
    status = '✅' if tokens < 1500 else '⚠️' if tokens < 2500 else '🔴'
    print(f"{status} {tokens:>5} tokens - {file}")
```

---

## Common Anti-Patterns

### ❌ Anti-Pattern 1: Vague Instructions
```markdown
Try to keep prompts focused.
It's generally better to use handoffs.
Consider checking for errors.
```
**Problem**: Probabilistic language creates inconsistent behavior

**Fix**: Use imperative language with clear requirements
```markdown
You MUST limit prompts to one primary task; use handoffs for multi-step workflows.
You WILL validate YAML syntax before saving.
```

### ❌ Anti-Pattern 2: Scope Creep
```markdown
# prompt-validator.agent.md
You are a validation specialist. You check prompts for errors,
suggest improvements, update the files with fixes, and also
generate documentation about validation results.
```
**Problem**: Agent has 4 responsibilities (validate, suggest, update, document)

**Fix**: Split into specialized agents
- `prompt-validator`: Check and report (read-only)
- `prompt-updater`: Apply fixes (write access)
- `documentation-updater`: Generate docs (separate domain)

### ❌ Anti-Pattern 3: Embedded Documentation
```markdown
# Inside a prompt file:

## Context Engineering Principles
[100 lines of principles that belong in context files]

## Tool Usage Guide
[50 lines of tool documentation]

## Validation Rules
[80 lines of validation logic]
```
**Problem**: Duplicates content, increases maintenance burden, wastes tokens

**Fix**: Reference external context
```markdown
## Prerequisites

Before starting, review:
- Context engineering principles: `.copilot/context/00.00 prompt-engineering/01-context-engineering-principles.md`
- Tool usage patterns: `.copilot/context/00.00 prompt-engineering/02-tool-composition-guide.md`
```

### ❌ Anti-Pattern 4: Weak Boundaries
```markdown
## Boundaries
- Try not to modify files outside the target directory
- Probably should ask before deleting things
```
**Problem**: "Try not to" and "probably should" are not enforceable

**Fix**: Use three-tier system with imperative language
```markdown
## Boundaries

### 🚫 Never Do
- NEVER modify files outside `.github/prompts/` directory
- NEVER delete any files without explicit user approval
```

### ❌ Anti-Pattern 5: Late Critical Information
```markdown
[300 lines of examples and context]

## Important: NEVER modify the top YAML block
[Critical constraint buried at end]
```
**Problem**: AI may miss critical information placed late in content

**Fix**: Place critical constraints at the top
```yaml
---
description: "Validation specialist - read-only operations"
agent: plan  # Establishes read-only constraint immediately
---

# Prompt Validator Agent

**CRITICAL CONSTRAINTS**:
- You NEVER modify files; you only analyze and report
- You NEVER touch the top YAML block under any circumstances
```

---

## Validation Checklist

Use this checklist when creating or reviewing prompts, agents, or instructions:

### Scope and Purpose
- [ ] File has single, well-defined responsibility
- [ ] Purpose is stated in first sentence/YAML description
- [ ] Related to other files via handoffs/references, not duplication

### Structure and Priority
- [ ] YAML frontmatter contains critical configuration
- [ ] Most important instructions are in first 20% of content
- [ ] Process/workflow defined before examples
- [ ] Quality checklist or success criteria included

### Language and Clarity
- [ ] Uses imperative verbs (MUST, WILL, NEVER)
- [ ] Avoids weak phrases (try to, consider, probably)
- [ ] Commands are specific and actionable
- [ ] No ambiguous pronouns or references

### Boundaries and Safety
- [ ] Three-tier boundaries explicitly defined
- [ ] File access scope is clear (which directories)
- [ ] Destructive operations require approval
- [ ] Handoff targets are valid agent/prompt names

### Context and Efficiency
- [ ] References context files instead of embedding content
- [ ] Only includes context necessary for this specific task
- [ ] Uses lazy loading (tools called when needed, not preemptively)
- [ ] No duplicated content from other repository files

### Tools and Capabilities
- [ ] Tool list matches role (read-only for validators, write for builders)
- [ ] `agent: plan` used for analysis-only agents
- [ ] No unnecessary tools that increase risk
- [ ] External tools (fetch, terminal) have clear boundaries

---

## Version History

| Version | Date | Changes | Author |
|---------|------|---------|--------|
| 1.0.0 | 2025-12-10 | Initial consolidated version from analysis | System |

---

## References

- **Official Documentation**: [VS Code Copilot Custom Agents](https://code.visualstudio.com/docs/copilot/copilot-customization)
- **Repository Articles**: `tech/PromptEngineering/` series
- **Related Context**: 
  - [tool-composition-guide.md](tool-composition-guide.md) - Tool selection patterns
  - [validation-caching-pattern.md](validation-caching-pattern.md) - 7-day caching rules
  - [handoffs-pattern.md](handoffs-pattern.md) - Multi-agent orchestration
