# Adaptive Validation Patterns for Prompt Engineering

This context file provides detailed patterns, templates, and examples for validating prompt requirements through challenge-based discovery. Use these patterns when creating or updating prompt files to ensure goals, roles, and workflows are reliable and well-scoped.

## Table of Contents

1. [Complexity Assessment Criteria](#complexity-assessment-criteria)
2. [Use Case Challenge Templates](#use-case-challenge-templates)
3. [Role Validation Methodology](#role-validation-methodology)
4. [Workflow Reliability Testing](#workflow-reliability-testing)
5. [Tool Requirement Mapping](#tool-requirement-mapping)
6. [Boundary Actionability Validation](#boundary-actionability-validation)

---

## Complexity Assessment Criteria

### Complexity Indicators

| Complexity Level | Indicators | Validation Depth |
|------------------|------------|------------------|
| **Simple** | Single clear task, well-defined scope, standard tools | **Quick** (3 use cases, basic role check) |
| **Moderate** | Multiple objectives, some ambiguity, tool selection unclear | **Standard** (5 use cases, role + workflow check) |
| **Complex** | Broad scope, multiple roles, novel workflow, >7 tools | **Deep** (7 use cases, full role/workflow/tool analysis) |

### Simple Prompts

✅ Goal has 1-2 clear objectives  
✅ Role is standard (reviewer, generator, analyzer)  
✅ Tools are obvious from task description  
✅ Workflow follows existing patterns  
**Example:** "Create a prompt for grammar checking"

### Moderate Prompts

⚠️ Goal has 3+ objectives or some ambiguity  
⚠️ Role requires domain expertise  
⚠️ Tools need discovery from use cases  
⚠️ Workflow has some novel phases  
**Example:** "Create a prompt for reviewing API documentation completeness"

### Complex Prompts

🔴 Goal could be interpreted multiple ways  
🔴 Role is novel or requires multiple personas  
🔴 Tools are unclear or >7 needed  
🔴 Workflow is entirely novel  
🔴 Multiple handoffs or orchestration needed  
**Example:** "Create a prompt for modernizing legacy codebases"

---

## Use Case Challenge Templates

### Use Case Template Structure

```markdown
**Scenario [N]:** [Realistic situation that goal should handle]
**Test Question:** [Specific question about goal's applicability]
**Current Goal Guidance:** [What does current goal say to do?]
**Gap Identified:** [What's missing, ambiguous, or contradictory]
**Tool Requirement Discovered:** [If scenario reveals need for specific tool]
**Scope Boundary Discovered:** [If scenario reveals in/out-of-scope question]
**Refinement Needed:** [Specific change to goal]
```

### Example 1: Simple Prompt - Grammar Checking

**Initial Goal:** "Check text for grammar and spelling errors"

**Use Case 1 (Common):**
- **Scenario:** User provides 500-word blog post with 5 typos
- **Test:** Does goal indicate what to check and how to report?
- **Current Guidance:** ✅ Clear - check grammar/spelling, report errors
- **Gap:** None for common case
- **Tool Discovered:** read_file (to load content)
- **Refinement:** None needed

**Use Case 2 (Edge Case):**
- **Scenario:** Technical article with code blocks and technical jargon
- **Test:** Should code blocks be checked? What about technical terms?
- **Current Guidance:** ⚠️ Ambiguous - "text" could mean all text including code
- **Gap:** Scope unclear for code blocks and technical terminology
- **Refinement:** "Check natural language text for grammar and spelling errors (skip code blocks, validate technical terms against glossary)"

**Use Case 3 (Failure Mode):**
- **Scenario:** Text is in multiple languages (English + Spanish)
- **Test:** Should all languages be checked?
- **Current Guidance:** ❌ Unclear - no language specification
- **Gap:** Language scope not defined
- **Scope Boundary:** English only (multilingual out of scope)
- **Refinement:** "Check **English** natural language text..."

**Refined Goal After Challenge:**
"Check English natural language text for grammar and spelling errors, skipping code blocks and validating technical terms against repository glossary"

**Validation Result:** ✅ Goal is now narrow, clear, and testable

### Example 2: Moderate Prompt - API Documentation Review

**Initial Goal:** "Review API documentation for completeness"

**Use Case 1 (Common - REST API):**
- **Scenario:** REST API with 50 endpoints, some missing parameter descriptions
- **Test:** What does "completeness" mean for REST endpoints?
- **Current Guidance:** ⚠️ Vague - "completeness" needs definition
- **Gap:** Need checklist: endpoints, parameters, responses, examples, auth
- **Tool Discovered:** grep_search (to compare docs vs. actual API code)
- **Refinement:** Define completeness criteria explicitly

**Use Case 2 (Edge Case - GraphQL):**
- **Scenario:** GraphQL API with schema but no query examples
- **Test:** Does goal apply to GraphQL or REST only?
- **Current Guidance:** ❌ Unclear - API type not specified
- **Gap:** Different validation rules for GraphQL vs. REST
- **Scope Boundary:** REST only, GraphQL out of scope
- **Refinement:** "Review **REST API** documentation..."

**Use Case 3 (Failure Mode - Versioning):**
- **Scenario:** API has v1 (deprecated) and v2 (current) docs
- **Test:** Should both versions be reviewed? How to handle deprecation?
- **Current Guidance:** ❌ Not addressed
- **Gap:** Version handling strategy missing
- **Scope Boundary:** Current version only
- **Refinement:** Add "for the current API version"

**Use Case 4 (Scale):**
- **Scenario:** 200 endpoints across 15 resource types
- **Test:** Review all or sample? How to prioritize?
- **Current Guidance:** ❌ Not addressed
- **Gap:** Needs strategy for large APIs
- **Workflow Discovery:** Need Phase 1 to inventory endpoints and prioritize
- **Refinement:** Add objective "Prioritize review based on endpoint usage/criticality"

**Use Case 5 (External Dependencies):**
- **Scenario:** Documentation references external OAuth provider docs
- **Test:** Should external docs be validated too?
- **Current Guidance:** ❌ Not addressed
- **Gap:** External dependency handling unclear
- **Tool Discovered:** fetch_webpage (to check external links)
- **Scope Boundary:** Check external links work, don't validate external content
- **Refinement:** Add "Verify external documentation links are valid"

**Refined Goal After Challenge:**
1. Inventory all REST API endpoints for the current version
2. Verify each endpoint has: complete parameters, response schemas, error codes, and working examples
3. Validate technical accuracy by comparing documentation against codebase
4. Verify external documentation links are valid (but don't validate external content)
5. Prioritize review based on endpoint criticality

**Tools Discovered:**
- grep_search (compare docs vs. code)
- read_file (load documentation files)
- fetch_webpage (validate external links)

**Validation Result:** ✅ Goal is now comprehensive, scoped, and actionable

### Example 3: Complex Prompt - Security Code Review

**Initial Goal:** "Review code for security issues"

**Use Case 1 (SQL Injection):**
- **Scenario:** Node.js app with raw SQL queries in 5 different files
- **Test:** What security issues should be detected?
- **Current Guidance:** ⚠️ Too broad - "security issues" could mean hundreds of things
- **Gap:** Need specific vulnerability categories
- **Tool Discovered:** grep_search (find SQL query patterns)
- **Refinement:** Narrow to specific categories (injection, XSS, auth, secrets)

**Use Case 2 (Exposed API Keys):**
- **Scenario:** Hardcoded AWS keys in config file
- **Test:** Should prompt detect and how to report?
- **Current Guidance:** ⚠️ Unclear if secrets detection in scope
- **Gap:** Secrets handling strategy needed
- **Tool Discovered:** grep_search (pattern match for key formats)
- **Boundary:** NEVER automatically remove secrets (risk of breaking code)
- **Refinement:** Add "Flag exposed secrets but NEVER modify code"

**Use Case 3 (Outdated Dependencies):**
- **Scenario:** package.json has dependencies with known CVEs
- **Test:** Is dependency vulnerability scanning in scope?
- **Current Guidance:** ❌ Not mentioned
- **Gap:** Dependency scanning is separate concern
- **Scope Boundary:** OUT OF SCOPE - recommend separate prompt
- **Refinement:** Explicitly exclude dependency scanning

**Use Case 4 (Input Validation):**
- **Scenario:** Express.js routes with no input sanitization
- **Test:** Should data flow be analyzed?
- **Current Guidance:** ⚠️ Unclear - requires tracing data flow
- **Gap:** Data flow analysis is complex, needs separate phase or prompt
- **Workflow Discovery:** If included, needs Phase 2 for data flow tracing
- **Decision Point:** ASK USER - include data flow analysis (adds complexity) or exclude?

**Use Case 5 (Authentication Bypass):**
- **Scenario:** Some routes missing auth middleware
- **Test:** Should authentication implementation be validated?
- **Current Guidance:** ⚠️ Requires understanding auth framework
- **Tool Discovered:** semantic_search (search for auth patterns)
- **Refinement:** Include authentication validation

**Use Case 6 (Scale - Microservices):**
- **Scenario:** 20 microservices with shared authentication
- **Test:** Review all services or per-service?
- **Current Guidance:** ❌ Not addressed
- **Gap:** Needs scope clarification
- **Decision Point:** ASK USER - which services to review?

**Use Case 7 (False Positives):**
- **Scenario:** Framework handles SQL injection prevention automatically
- **Test:** Should prompt understand framework protections?
- **Current Guidance:** ❌ Not addressed
- **Gap:** Needs framework-aware analysis or will generate false positives
- **Complexity:** Framework detection adds significant complexity
- **Decision Point:** ASK USER - framework-aware analysis or generic rules?

**Validation Result:** ⚠️ Goal is too complex - requires user clarifications

**Questions for User:**
1. ❌ **CRITICAL:** Scope too broad. Which vulnerability categories should be included?
   - Option A: Injection attacks only (SQL, XSS, command injection)
   - Option B: Injection + authentication issues
   - Option C: Comprehensive (injection, auth, secrets, input validation, XSS)

2. ⚠️ **HIGH PRIORITY:** Include data flow analysis for input validation?
   - YES: Adds Phase 2 for tracing user input → database (complex, slower)
   - NO: Only flag missing input validation at entry points (simple, faster)

3. ⚠️ **HIGH PRIORITY:** Framework-aware analysis?
   - YES: Understand framework protections (complex, fewer false positives)
   - NO: Generic pattern matching (simple, more false positives)

4. 📋 **MEDIUM:** Which services/files to review?
   - ALL: Review entire codebase (comprehensive, slow)
   - SPECIFIED: User specifies paths (targeted, fast)

**DO NOT PROCEED** until user answers Critical and High Priority questions.

---

## Role Validation Methodology

### Validation Tests

1. **Authority Test:** Can this role make necessary judgments?
2. **Expertise Test:** Does role imply required knowledge?
3. **Specificity Test:** Is role concrete or generic?
4. **Pattern Search:** Find similar roles in existing prompts
5. **Refinement:** Adjust role if needed

### Example 1: Generic Role → Specific Role

**Initial Role:** "Helpful documentation assistant"

**Authority Test:**
❌ Can "assistant" authoritatively identify missing API authentication sections? NO  
❌ Can "assistant" validate technical accuracy of code examples? NO  
**Result:** Role lacks authority for technical validation

**Expertise Test:**
❌ Does "helpful assistant" imply API expertise? NO  
❌ Does it imply understanding of REST patterns? NO  
**Result:** Role lacks necessary expertise signal

**Specificity Test:**
❌ "Helpful assistant" is generic placeholder  
✅ Need specific expertise: API documentation, technical writing  
**Result:** Role is too generic

**Pattern Search:**
Found in workspace: `.github/prompts/api-docs-review.prompt.md`
- Uses role: "Technical documentation reviewer with API expertise"
- This signals both writing skills AND technical knowledge

**Refined Role:**
"Technical documentation reviewer with API and REST architecture expertise"

**Why this is better:**
- ✅ Establishes authority for technical validation
- ✅ Signals API domain knowledge
- ✅ Implies understanding of documentation best practices
- ✅ Specific enough to guide behavior

### Example 2: Role Matches Goal

**Goal:** "Check English natural language text for grammar errors"  
**Initial Role:** "Grammar reviewer"

**Authority Test:**
✅ Can "grammar reviewer" judge if sentence structure is correct? YES  
✅ Can this role apply grammar rules authoritatively? YES  
**Result:** Role has sufficient authority

**Expertise Test:**
✅ Does "grammar reviewer" imply knowledge of grammar rules? YES  
✅ Does it imply English language expertise? YES (for this goal)  
**Result:** Role has necessary expertise

**Specificity Test:**
✅ "Grammar reviewer" is specific to task  
⚠️ Could add "English" for precision  
**Result:** Role is adequately specific

**Pattern Search:**
Found in workspace: `.github/prompts/grammar-review.prompt.md`
- Uses role: "English grammar and style editor"
- Adds "style" dimension (more comprehensive)

**Decision:**
**Refinement:** "English grammar and style reviewer"  
**Justification:** Adds precision (English) and expands scope slightly (style)

**Validation Result:** ✅ Role is appropriate for goal

### Example 3: Role Too Narrow for Goal

**Goal:** "Review API documentation for completeness AND technical accuracy"  
**Initial Role:** "Technical writer"

**Authority Test:**
✅ Can "technical writer" assess documentation completeness? YES  
⚠️ Can "technical writer" validate code examples work? MAYBE  
❌ Can "technical writer" verify API response schemas match implementation? NO  
**Result:** Role lacks authority for technical accuracy validation

**Expertise Test:**
✅ Does "technical writer" imply documentation expertise? YES  
❌ Does it imply programming/API implementation knowledge? NO  
**Result:** Role has writing expertise but lacks technical validation capability

**Gap Analysis:**
Goal requires TWO types of expertise:
1. Documentation quality (completeness, clarity, examples)
2. Technical accuracy (code validation, schema verification)

**Options:**
A. **Split into two prompts:**
   - Prompt 1: "Documentation completeness review" (technical writer role)
   - Prompt 2: "Technical accuracy validation" (software engineer role)

B. **Expand role to cover both:**
   - Role: "Technical documentation reviewer with software engineering background"

C. **Narrow goal to match role:**
   - Remove technical accuracy, keep completeness only

**Recommendation:** **Option A (Split)**  
**Justification:** Clean separation of concerns, reusable components

---

## Workflow Reliability Testing

### Failure Mode Analysis Process

1. **For each proposed phase:** Ask "What could go wrong?"
2. **Identify missing phases:** Input validation, error handling, dependency discovery
3. **Pattern validation:** Compare against similar prompts in workspace
4. **Refinement:** Add missing phases, adjust sequence

### Example 1: Simple Workflow - Grammar Review

**Proposed Workflow (Initial):**
- Phase 1: Read text
- Phase 2: Check for errors
- Phase 3: Generate report

**Failure Mode Analysis:**

**Phase 1 Test: What if input is malformed?**
- Scenario: User provides binary file instead of text
- Current handling: ❌ Not addressed
- **Missing Phase:** Input validation
- **Refinement:** Add Phase 1a: Validate input is text file

**Phase 2 Test: What if text is very long?**
- Scenario: 10,000-word document
- Current handling: ⚠️ May hit token limits
- **Missing Step:** Chunking strategy
- **Refinement:** Add to Phase 2: Process in chunks if >2000 words

**Phase 3 Test: What if no errors found?**
- Scenario: Perfect grammar
- Current handling: ✅ Report "no errors found"
- **No change needed**

**Pattern Validation:**
Search: `.github/prompts/grammar-review.prompt.md`  
Found workflow:
- Phase 1: Input validation + bottom YAML check (7-day caching)
- Phase 2: Grammar analysis
- Phase 3: Report generation

**Gap Identified:** Missing 7-day caching check for validation prompts

**Refined Workflow:**
- **Phase 1:** Input validation + 7-day cache check
- **Phase 2:** Grammar analysis (with chunking if needed)
- **Phase 3:** Report generation + update bottom metadata

**Validation Result:** ✅ Workflow is reliable with additions

### Example 2: Moderate Workflow - API Documentation Review

**Proposed Workflow (Initial):**
- Phase 1: Load documentation
- Phase 2: Check completeness
- Phase 3: Generate report

**Failure Mode Analysis:**

**Phase 1 Test: What if docs reference external schemas?**
- Scenario: OpenAPI spec references external $ref
- Current handling: ❌ Not addressed
- **Missing Step:** Dependency discovery
- **Refinement:** Add Phase 1b: Resolve external references

**Phase 1 Test: What if documentation is scattered across multiple files?**
- Scenario: README.md + /docs/*.md + inline code comments
- Current handling: ❌ Not addressed
- **Missing Phase:** Documentation discovery and aggregation
- **Refinement:** Add Phase 1a: Discover all documentation sources

**Phase 2 Test: What if code examples import undefined modules?**
- Scenario: Example uses `import { AuthClient } from './auth'` but auth.js missing
- Current handling: ❌ Not addressed
- **Missing Step:** Example validation
- **Refinement:** Add Phase 2b: Validate code examples against codebase

**Phase 2 Test: What if API versioning affects structure?**
- Scenario: v1 has different endpoint structure than v2
- Current handling: ❌ Not addressed
- **Missing Phase:** Version handling
- **Refinement:** Add Phase 0: Identify API version (or ask user)

**Pattern Validation:**
Search: Similar review prompts  
Found: `.github/prompts/article-review-for-consistency-and-gaps-v2.prompt.md`  
Pattern: Phase 1 includes comprehensive discovery before analysis

**Refined Workflow:**
- **Phase 0:** Identify API version and documentation sources (ask user if unclear)
- **Phase 1:** Discovery
  - 1a: Find all documentation files
  - 1b: Resolve external schema references
  - 1c: Inventory endpoints from code
- **Phase 2:** Completeness Analysis
  - 2a: Compare documented vs. actual endpoints
  - 2b: Validate code examples against codebase
  - 2c: Check for missing sections (auth, errors, examples)
- **Phase 3:** Report generation with prioritized findings

**Validation Result:** ✅ Workflow is comprehensive and handles edge cases

---

## Tool Requirement Mapping

### Phase → Tool Mapping Process

1. **For each phase:** What capabilities are needed?
2. **Cross-reference:** `.copilot/context/00.00 prompt-engineering/02-tool-composition-guide.md`
3. **Validate count:** 3-7 tools is optimal (>7 causes tool clash)
4. **Verify alignment:** agent mode matches tools (plan → read-only, agent → write)

### Example: API Documentation Review

**Workflow Phase Mapping:**

**Phase 1: Documentation Discovery**
- **Capability needed:** Find all .md files in workspace
- **Tool:** file_search (find files by pattern)

**Phase 2: Load Documentation**
- **Capability needed:** Read file contents
- **Tool:** read_file

**Phase 3: Inventory Endpoints from Code**
- **Capability needed:** Search code for route definitions
- **Tool:** grep_search (find patterns like `app.get(`, `@route`)

**Phase 4: Resolve External References**
- **Capability needed:** Fetch external schema files
- **Tool:** fetch_webpage (for http:// URLs)

**Phase 5: Validate Code Examples**
- **Capability needed:** Search codebase for imported modules
- **Tool:** semantic_search (find relevant code)

**Tool List (Initial):**
1. file_search
2. read_file
3. grep_search
4. fetch_webpage
5. semantic_search

**Tool Count Validation:**
- Count: 5 tools
- **Status:** ✅ Within optimal range (3-7)

**Agent Mode Validation:**
- **Proposed mode:** agent (needs read + write for report generation)
- **Tools proposed:** All read-only + (implied: create_file for report)
- **Alignment check:** ✅ agent mode can use read + write tools

**Cross-Reference tool-composition-guide.md:**
- **Pattern match:** "Research-first workflow"
  - Recommended: semantic_search → read_file → grep_search
  - **Our workflow:** ✅ Follows this pattern in Phase 3-5

**Final Tool List:**
1. file_search - Find documentation files
2. read_file - Load file contents
3. grep_search - Search for route patterns
4. fetch_webpage - Fetch external schemas
5. semantic_search - Find related code for validation
6. create_file - Generate report (implicit for agent mode)

**Validation Result:** ✅ Tools are necessary and well-composed

---

## Boundary Actionability Validation

### Validation Process

1. **For each boundary:** Can AI determine compliance?
2. **Refine vague boundaries:** Make specific and testable
3. **Ensure all three tiers populated:** Always Do / Ask First / Never Do
4. **Check coverage:** Do boundaries prevent failure modes identified in workflow testing?

### Example: API Documentation Review

**Initial Boundaries:**

**✅ Always Do:**
- Be thorough
- Check for issues

**⚠️ Ask First:**
- Before making changes

**🚫 Never Do:**
- Don't be careless

**Validation:**

**Always Do - Boundary 1: "Be thorough"**
- **Testability:** ❌ What does "thorough" mean? Subjective
- **Refinement:** "Check all 5 completeness criteria: endpoints, parameters, responses, examples, authentication"
- **Actionable:** ✅ Can verify checklist

**Always Do - Boundary 2: "Check for issues"**
- **Testability:** ❌ What types of issues? Vague
- **Refinement:** "Flag missing parameters, incorrect response schemas, and broken code examples"
- **Actionable:** ✅ Specific issue types

**Ask First - Boundary 1: "Before making changes"**
- **Testability:** ✅ Clear - ask before any modification
- **Refinement:** None needed
- **Actionable:** ✅ Can determine if about to modify

**Never Do - Boundary 1: "Don't be careless"**
- **Testability:** ❌ Subjective and vague
- **Refinement:** "NEVER skip endpoint validation even if documentation seems complete"
- **Actionable:** ✅ Can verify endpoint validation occurred

**Coverage Check (vs. workflow failure modes):**
- **Failure:** External references not resolved
- **Boundary needed:** "ALWAYS attempt to resolve external $ref before flagging as missing"
- **Add to Always Do**

- **Failure:** Code examples not validated
- **Boundary needed:** "ALWAYS validate code examples can execute (imports exist, syntax correct)"
- **Add to Always Do**

**Refined Boundaries:**

**✅ Always Do:**
- Check all 5 completeness criteria: endpoints, parameters, responses, examples, authentication
- Flag missing parameters, incorrect response schemas, and broken code examples
- ALWAYS attempt to resolve external $ref before flagging as missing
- ALWAYS validate code examples can execute (imports exist, syntax correct)

**⚠️ Ask First:**
- Before making changes to documentation files
- Before fetching >10 external references (may be slow)

**🚫 Never Do:**
- NEVER skip endpoint validation even if documentation seems complete
- NEVER modify documentation files (read-only analysis)
- NEVER assume external links work without validation

**Validation Result:** ✅ Boundaries are actionable and comprehensive

---

## Quick Reference: Validation Decision Trees

### Should I Ask User for Clarification?

```
Is goal clear and testable across all use cases?
├─ YES → Proceed to role validation
└─ NO → How severe is the ambiguity?
    ├─ CRITICAL (multiple interpretations, different tool sets)
    │   └─ BLOCK: Ask user to choose interpretation
    ├─ HIGH (affects scope or major workflow phases)
    │   └─ ASK: Present options with implications
    └─ MEDIUM (minor edge case handling)
        └─ PROPOSE: Suggest refinement, proceed if approved
```

### Should I Split This Prompt?

```
Does goal require >7 tools OR multiple distinct expertise areas?
├─ YES → Consider splitting
│   ├─ Different roles needed? → Split by role (orchestrator pattern)
│   ├─ Phases independent? → Split by phase (linear handoff)
│   └─ Scale issue? → Split by batch processing
└─ NO → Keep unified, optimize tool selection
```

### What Validation Depth Should I Use?

```
Assess complexity indicators:
├─ 1-2 objectives, standard role, obvious tools → SIMPLE (3 use cases)
├─ 3+ objectives, domain expertise, tool discovery needed → MODERATE (5 use cases)
└─ Broad scope, novel role, >7 tools, novel workflow → COMPLEX (7 use cases)
```

---

## Anti-Patterns to Avoid

### ❌ Passive Acceptance
**Bad:** "User wants validation, so I'll make a validation prompt"  
**Good:** "What does 'validation' mean here? Let me test with 3-5 scenarios"

### ❌ Feature Creep
**Bad:** "While validating API docs, let's also check security and performance"  
**Good:** "API completeness is scope. Security/performance are separate prompts"

### ❌ Generic Roles
**Bad:** "Helpful assistant"  
**Good:** "Technical documentation reviewer with REST API expertise"

### ❌ Untested Workflows
**Bad:** Phase 1: Read, Phase 2: Analyze, Phase 3: Report (no failure mode testing)  
**Good:** Test each phase with "What if [failure scenario]?" and add handling

### ❌ Vague Boundaries
**Bad:** "Be careful and thorough"  
**Good:** "ALWAYS validate all 5 criteria: X, Y, Z, A, B. NEVER skip validation even if docs seem complete"

---

## Usage Guidelines

**When to use this context file:**
- Creating new prompt files with complex requirements
- Updating existing prompts that need better validation
- Investigating why a prompt is failing or producing inconsistent results
- Teaching prompt engineering best practices

**How to reference in prompts:**
```markdown
## Requirements Validation

**📖 Validation Methodology:** `.copilot/context/00.00 prompt-engineering/adaptive-validation-patterns.md`

Follow the adaptive validation process:
1. Assess complexity (simple/moderate/complex)
2. Generate use cases (3/5/7 based on complexity)
3. Challenge goal with scenarios
4. Validate role appropriateness
5. Test workflow reliability
6. Map tool requirements
7. Verify boundary actionability
```

**Integration with other context files:**
- Pair with `context-engineering-principles.md` for overall prompt structure
- Use `tool-composition-guide.md` for tool selection patterns
- Apply `validation-caching-pattern.md` for validation prompts
- Combine with `handoffs-pattern.md` when orchestrating multiple agents
