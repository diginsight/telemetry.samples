# Validation Criteria for Technical Documentation

This document defines general validation dimensions and quality thresholds for technical documentation. These criteria apply to any article regardless of the specific documentation site or repository.

## 🎯 Overall Quality Threshold

Content is considered **publish-ready** when:
- All critical validations passed
- No broken links or errors
- Facts verified within last 30 days
- Metadata complete and current
- Template structure followed
- Target audience appropriately served

## 📋 Validation Dimensions

### 1. Grammar and Mechanics

**Pass Criteria:**
- Zero spelling errors
- No grammar mistakes
- Proper punctuation
- Consistent capitalization
- No sentence fragments (unless intentional)
- Proper use of technical terms

**Acceptable Minor Issues:**
- Stylistic preferences (Oxford comma, etc.)
- Technical jargon proper to domain
- Intentional fragments for emphasis

**Automatic Fail:**
- Multiple spelling errors (> 5)
- Grammar errors that impede understanding
- Inconsistent terminology usage

### 2. Readability

**Target Metrics:**
- Flesch Reading Ease: 50-70 (fairly easy to standard)
- Grade Level: 9-10 for general content, 11-12 for advanced technical
- Average Sentence Length: 15-25 words
- Paragraph Length: 3-5 sentences

**Pass Criteria:**
- Readability score within range for target audience
- Clear paragraph structure
- Logical flow between sentences
- Appropriate use of transitions
- No redundancy or repetition

**Acceptable Variance:**
- Higher complexity justified by technical depth
- Lower readability for code-heavy content (code examples excluded from calculation)
- Audience-specific adjustments (beginner vs. advanced)

**Automatic Fail:**
- Readability significantly off-target without justification
- Excessive redundancy (same information repeated multiple times)
- Confusing or contradictory statements

### 3. Structure

**Required Elements (All Articles):**
- ✅ Single H1 title
- ✅ Table of Contents (if > 500 words)
- ✅ Introduction section
- ✅ Body with logical section breaks
- ✅ Conclusion section
- ✅ References section (if sources cited)

**Heading Requirements:**
- ✅ Proper hierarchy (no skipped levels)
- ✅ Descriptive headings
- ✅ Consistent formatting
- ✅ H2 headings have emoji prefixes for visual scanning

**Markdown Quality:**
- ✅ Code blocks have language specified
- ✅ Links formatted properly
- ✅ Images have alt text
- ✅ Lists properly formatted
- ✅ No broken Markdown syntax

**Pass Criteria:**
All required elements present, proper hierarchy, valid Markdown

**Automatic Fail:**
- Missing required sections
- Broken Markdown that affects rendering
- Skipped heading levels
- Code blocks without language specification

### 4. Logical Flow

**Pass Criteria:**
- Concepts introduced before used
- Ideas build upon each other
- No circular dependencies
- Smooth transitions between sections
- Conclusion follows from content
- Prerequisites clearly stated

**Evaluation Points:**
- Does introduction set up the content?
- Do sections follow a logical sequence?
- Are jumps in complexity justified?
- Are connections between concepts explicit?
- Does conclusion summarize effectively?

**Automatic Fail:**
- Critical prerequisite not explained
- Concepts used before introduction
- Contradictory statements
- Disconnected sections without context

### 5. Factual Accuracy

**Pass Criteria:**
- All factual claims verified against authoritative sources
- Version information current
- Links to official documentation
- Statistics/benchmarks cited
- Code examples tested and functional
- No outdated information

**Source Requirements:**
- Official documentation preferred
- GitHub repositories for open source
- Academic papers for algorithms/theory
- Industry-standard resources
- Sources dated within reasonable timeframe

**Verification Frequency:**
- Technical details: Verify quarterly or with major version changes
- Statistics: Re-verify annually
- General concepts: Verify at publication, review annually

**Automatic Fail:**
- Factually incorrect information
- Broken or outdated links
- Uncited statistics or benchmarks
- Code examples that don't work
- Security vulnerabilities in examples

### 6. Completeness (Gap Analysis)

**Pass Criteria (Comprehensive):**
- All major aspects of topic covered
- Common use cases explained
- Prerequisites adequately addressed
- Common questions answered
- Examples included where needed

**Acceptable (Minor Gaps):**
- Some nice-to-have topics not covered
- Advanced edge cases not fully explored
- Optional enhancements not detailed
- Links to external resources for deep dives

**Needs Work (Major Gaps):**
- Critical concepts not explained
- Common use cases missing
- No examples for complex topics
- Prerequisites assumed but not stated

**Evaluation Questions:**
- Are readers' likely questions answered?
- Are common use cases covered?
- Is prerequisite knowledge addressed?
- Are examples sufficient?
- Are limitations or caveats noted?

### 7. Understandability

**Pass Criteria:**
- Target audience can comprehend content
- Technical depth appropriate for audience level
- Jargon defined or linked
- Examples clarify concepts
- Progressive complexity manageable

**Jargon and New Terms (REQUIRED):**

New terms and jargon MUST be introduced following these rules:

1. **Mark new terms visually** — Use `<mark>` tags to highlight jargon when first introduced
2. **Explain in context** — The sentence introducing jargon MUST explain its meaning (don't just drop terms)
3. **Teach the shorthand** — Once explained, the jargon can be used freely throughout the article

**Examples:**

❌ **Wrong:** "Use short persistence for handoff transfers."
✅ **Correct:** "Context with <mark>short persistence</mark> (only lasts within a single chat session) requires explicit handoff transfers when switching agents."

❌ **Wrong:** "The System → Model direction applies here."
✅ **Correct:** "<mark>System → Model</mark> direction means the content is auto-injected by the system based on file patterns—you don't manually include it."

**Table Introduction (REQUIRED):**

Tables MUST be properly introduced, especially when:
- They appear at the beginning of a section
- Column meanings aren't self-explanatory
- The table contains domain-specific terminology

Rules for table introduction:
1. **Provide context before the table** — A sentence explaining what the table shows and why it matters
2. **Explain non-obvious columns** — When column headers like "Direction" or "Persistence" aren't self-explanatory, provide a brief explanation of what each column represents
3. **Connect to narrative** — Show how the table relates to the preceding or following content

**Example of proper table introduction:**

> The table below describes GitHub Copilot customization files and how context information flows into the model. For each component:
> - **Direction** indicates whether you explicitly include the content (User → Model) or it's auto-injected by the system (System → Model)
> - **Persistence** shows whether context lasts only within a session (short) or accumulates over time (long)
>
> | Component | What It Provides | Direction | Persistence |
> |-----------|------------------|-----------|-------------|
> | ... | ... | ... | ... |

**Audience-Specific Thresholds:**

**Beginner Content:**
- Every technical term defined with `<mark>` on first use
- Multiple examples provided
- Step-by-step explanations
- Links to prerequisite learning
- No assumed knowledge
- All tables introduced with context

**Intermediate Content:**
- Common terms referenced, not redefined
- New terms marked and explained
- Examples show realistic usage
- Some prior knowledge assumed but stated
- Links to foundational concepts
- Non-obvious tables introduced

**Advanced Content:**
- Domain expertise assumed
- New jargon still marked on first use
- Focus on optimization, edge cases
- Complex examples appropriate
- References to theory or research
- Tables may have minimal introduction if context is clear

**Automatic Fail:**
- Audience mismatch not addressed
- Unexplained jargon blocks understanding
- Jargon introduced without `<mark>` highlighting
- Jargon introduced without explanatory sentence
- Tables dropped without introduction
- Non-obvious table columns unexplained
- Examples don't clarify concepts
- Sudden jumps in complexity without bridge

## 📊 Validation Outcomes

### Status Values
- **passed**: Meets all criteria
- **minor_issues**: Acceptable issues noted
- **needs_revision**: Must be fixed before publishing
- **failed**: Critical errors, major revision needed

### Quality Scoring

**Validation Outcome Definitions:**

**Passed:**
- Meets all criteria for dimension
- No critical issues
- Minor issues acceptable and noted

**Minor Issues:**
- Meets most criteria
- Issues don't block understanding
- Improvements suggested but not required
- Can publish with notes

**Needs Revision:**
- Fails some criteria
- Issues impede understanding or usability
- Must be addressed before publication
- Re-validation required after fixes

**Failed:**
- Fails critical criteria
- Significant errors or omissions
- Cannot publish in current state
- Major revision required

### Overall Readiness

**Ready to Publish:**
- All critical validations: Passed
- Important validations: Passed or Minor Issues
- No broken links/errors
- Metadata complete

**Needs Minor Work:**
- Critical validations: Passed
- Some important validations: Needs Revision
- Can publish after addressing specific issues

**Not Ready:**
- Any critical validation: Failed or Needs Revision
- Multiple important validations: Failed
- Significant work required

## 💡 Special Considerations

### Code-Heavy Content

- Readability metrics adjusted (code excluded)
- Structure may deviate for reference docs
- Examples must be tested and working
- Comments and explanations required

### Series Articles

- Consistency with series more important than standalone perfection
- Cross-reference validation critical
- Terminology must match across series
- Can trade some comprehensiveness for focused scope

### Time-Sensitive Content

- Fact-checking more frequent
- Version info prominently displayed
- Regular review schedule
- Deprecation plan if outdated

### Reference Material

- Completeness more critical than narrative flow
- Structure allows for quick lookup
- Examples exhaustive
- Updates tracked meticulously

## ✅ Pre-Publication Checklist

### Critical (Must Pass)
- [ ] Grammar validated: passed
- [ ] Structure validated: passed
- [ ] Facts checked: passed (<30 days)
- [ ] Links verified: all working
- [ ] Code tested: functional
- [ ] No placeholder content (TODO, TBD, etc.)

### Important (Should Pass)
- [ ] Readability validated: passed or justified
- [ ] Logic validated: passed
- [ ] Gap analysis: comprehensive or minor_gaps
- [ ] Understandability: passed for target audience
- [ ] Metadata complete and current

### Recommended
- [ ] Series validated (if applicable)
- [ ] Correlated topics identified
- [ ] Cross-references added
- [ ] Examples tested in multiple environments

## 🔄 Review Triggers

Re-validation required when:
- Content modified
- Technology version updates
- Links break or sources move
- Reader feedback identifies issues
- More than 90 days since last review
