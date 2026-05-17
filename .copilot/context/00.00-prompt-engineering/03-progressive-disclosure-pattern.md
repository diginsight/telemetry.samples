# Progressive Disclosure Pattern for Agent Skills

**Purpose**: Documents the three-level loading system used by Agent Skills to minimize context consumption while enabling powerful capabilities.

**Referenced by**: `.github/instructions/skills.instructions.md`, skill creation workflows

---

## Overview

Progressive disclosure is a design pattern where information is loaded only when needed, reducing initial context consumption and improving AI focus. Agent Skills use this pattern extensively.

## The Three-Level Loading System

```
┌─────────────────────────────────────────────────────────────────┐
│  Level 1: SKILL DISCOVERY (Always Loaded)                       │
│  ├── name: "webapp-testing"                                     │
│  └── description: "Automated testing workflow for web apps..."  │
│      (~50-100 tokens per skill)                                 │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼ (When prompt matches description)
┌─────────────────────────────────────────────────────────────────┐
│  Level 2: INSTRUCTIONS LOADING                                  │
│  └── SKILL.md body content                                      │
│      (~500-1500 tokens)                                         │
└─────────────────────────────────────────────────────────────────┘
                              │
                              ▼ (When referenced by Copilot)
┌─────────────────────────────────────────────────────────────────┐
│  Level 3: RESOURCE ACCESS                                       │
│  ├── templates/component.test.js                                │
│  ├── examples/login-form-tests.js                               │
│  └── scripts/setup-test-env.sh                                  │
│      (Loaded only when explicitly needed)                       │
└─────────────────────────────────────────────────────────────────┘
```

## Level Details

### Level 1: Skill Discovery

**What loads**: Only YAML frontmatter (`name` + `description`)

**When**: Always—Copilot maintains awareness of all available skills

**Token impact**: ~50-100 tokens per skill (very lightweight)

**Implications**:
- You can have many skills without context bloat
- Description quality is critical for accurate activation
- Poor descriptions cause false activations or missed matches

### Level 2: Instructions Loading

**What loads**: Full SKILL.md body content

**When**: User's prompt semantically matches the skill's description

**Token impact**: ~500-1500 tokens (skill-dependent)

**Implications**:
- Only triggered by matching prompts
- Keep body focused and actionable
- Use progressive detail structure (Quick Start → Standard → Advanced)

### Level 3: Resource Access

**What loads**: Templates, examples, scripts, documentation

**When**: Copilot explicitly references the resource

**Token impact**: Variable (20-150 lines per resource)

**Implications**:
- Resources never load unless needed
- Reference resources by relative path in SKILL.md
- Keep templates minimal—users will expand them

## Token Budget Comparison

| Customization Type | Loading Pattern | Token Impact |
|--------------------|-----------------|--------------|
| **Instruction files** | Always (when file matches `applyTo`) | Medium (always in context) |
| **Agent files** | When agent active | High (full persona loaded) |
| **Prompt files** | On-demand (user invokes) | Medium (single execution) |
| **Skills (Level 1)** | Always | Very low (~50-100 tokens) |
| **Skills (Level 2)** | On-demand | Medium (~500-1500 tokens) |
| **Skills (Level 3)** | Lazy load | Variable (as needed) |

## Optimizing for Discovery

The `description` field is the **key to effective progressive disclosure**. It determines:

1. **Whether Level 2 ever loads** - Poor description = skill never activates
2. **False activation rate** - Vague description = activates for wrong prompts
3. **Discovery accuracy** - Specific keywords help Copilot match intent

### Description Formula

```
[What it does] + [Technologies involved] + "Use when" + [Specific scenarios]
```

### Examples

**✅ Good: Specific with trigger scenarios**
```yaml
description: >
  Generate comprehensive test suites for React components using Jest 
  and React Testing Library. Use when writing unit tests, creating 
  integration tests, testing user interactions, or validating rendering.
```

**❌ Bad: Vague, no scenarios**
```yaml
description: Helps with testing
```

### Keyword Density

Include keywords users might type:
- Technology names: React, Jest, TypeScript, Kubernetes
- Action verbs: generate, create, debug, configure, deploy
- Use case phrases: "setting up", "debugging", "reviewing"

## When to Use Skills vs Other Types

Use the progressive disclosure pattern (skills) when:

| Scenario | Use Skill? | Reason |
|----------|-----------|--------|
| Cross-platform workflow | ✅ Yes | Skills work in VS Code, CLI, coding agent |
| Resource-rich task | ✅ Yes | Can include templates, scripts, examples |
| Always-on coding standards | ❌ No | Use instructions with `applyTo` |
| Persistent persona | ❌ No | Use agents with tool configuration |
| Complex tool orchestration | ❌ No | Use agents with handoffs |

## Capacity Guidelines

Given the token budget constraints:

| Workspace Size | Skill Count | Rationale |
|----------------|-------------|-----------|
| Small (1-3 projects) | 5-10 skills | Low discovery overhead |
| Medium (5-10 projects) | 10-20 skills | Still manageable |
| Large (10+ projects) | 20-50 skills | May need skill organization |
| Enterprise | Use skill registries | Consider shared skill repositories |

**Watchpoints**:
- Each skill adds ~50-100 tokens to every conversation (Level 1)
- 50 skills = ~2,500-5,000 tokens always in context
- Diminishing returns after ~30-40 skills
### Progressive Disclosure Token Budgets

**Quantitative guidelines for three-level loading system:**

| Level | Component | Target Size | Maximum Size | Token Impact |
|-------|-----------|------------|--------------|-------------|
| **Level 1** | YAML `description` | 50-70 tokens | 100 tokens | Always loaded |
| | YAML `name` | 3-8 tokens | 15 tokens | Always loaded |
| | **Level 1 Total** | **60-80 tokens** | **100 tokens** | **Per skill, every conversation** |
| **Level 2** | SKILL.md body | 500-1,000 tokens | 1,500 tokens | Loaded when matched |
| | Quick Reference section | 50-100 tokens | 150 tokens | Scanned first |
| | Detailed Workflow section | 300-600 tokens | 1,000 tokens | Full procedures |
| | Advanced Configuration | 100-200 tokens | 350 tokens | Links to resources |
| **Level 3** | Templates (per file) | 150-300 tokens | 500 tokens | Loaded on reference |
| | Examples (per file) | 200-400 tokens | 600 tokens | Loaded on reference |
| | Scripts (per file) | 100-300 tokens | 400 tokens | Loaded on reference |
| | Configuration files | 50-150 tokens | 300 tokens | Loaded on reference |

**Optimization Formula:**
```
Total Skill Impact = 
  Level 1 (always) + 
  Level 2 (when matched) + 
  Level 3 resources (when referenced)

Optimal: 70 + 800 + (2 × 250) = 1,370 tokens
Maximum: 100 + 1,500 + (4 × 500) = 3,600 tokens
```

**Budget Rules:**
1. **Description (Level 1)**: MUST be < 100 tokens (keep under 200 characters)
2. **Body (Level 2)**: SHOULD be < 1,000 tokens, MUST be < 1,500 tokens
3. **Resources (Level 3)**: Individual files SHOULD be < 300 tokens, MUST be < 600 tokens
4. **Total Skill**: Typical usage < 2,000 tokens, maximum < 4,000 tokens

**Performance Impact:**

| Scenario | Token Consumption | Performance |
|----------|------------------|-------------|
| **Discovery Only** (10 skills) | 700-1,000 tokens | ✅ Minimal |
| **One Skill Activated** | 700-1,000 + 800-1,500 | ✅ Low (~2,500 total) |
| **Skill + 2 Resources** | ~3,000-4,000 tokens | ✅ Acceptable |
| **Skill + 5+ Resources** | > 5,000 tokens | ⚠️ Consider splitting skill |

**Refactoring Triggers:**
- Level 1 > 100 tokens → Shorten description, remove unnecessary keywords
- Level 2 > 1,500 tokens → Split into sub-skills or extract to resources
- Level 3 file > 600 tokens → Simplify template or split example
- Total usage > 4,000 tokens → Redesign as multiple focused skills
## Advanced Patterns

### Progressive Detail Structure

Structure SKILL.md body for efficient Level 2 loading:

```markdown
## Quick Reference (Always scan first)
[Essential patterns - 5-10 lines]

## Detailed Workflow (Load when needed)
[Full procedure - 50-100 lines]

## Advanced Configuration (Reference only)
[Links to examples/, config/ directories]
```

### Skill Composition

Reference other skills to avoid duplication:

```markdown
## Prerequisites

This skill builds on [testing-core](../testing-core/SKILL.md) patterns.
```

### Environment-Specific Variants

Create focused skills rather than one mega-skill:

```
.github/skills/
├── k8s-deploy-dev/      # Development context
├── k8s-deploy-staging/  # Staging with checks
└── k8s-deploy-prod/     # Production with safeguards
```

## Anti-Patterns

### 1. Over-Broad Descriptions

**Problem**: Skill activates for unrelated prompts

```yaml
# ❌ Matches almost everything
description: General development workflow helper
```

**Solution**: Be specific about capabilities and use cases

### 2. Mega-Skills

**Problem**: Single skill tries to do everything, loading unnecessary content

```yaml
# ❌ Too broad
name: complete-development-workflow
description: Testing, deployment, review, docs, security, CI/CD...
```

**Solution**: Create focused, composable skills

### 3. Heavy Level 1 Content

**Problem**: Long descriptions bloat discovery phase

```yaml
# ❌ Description exceeds 1024 chars
description: [500-word essay about testing philosophy]
```

**Solution**: Keep description concise; move details to SKILL.md body

## References

- [VS Code: Agent Skills](https://code.visualstudio.com/docs/copilot/customization/agent-skills) - Official documentation
- [agentskills.io](https://agentskills.io/) - Open standard specification
- [context-engineering-principles.md](./context-engineering-principles.md) - Core principles including context minimization
