# Suppression Audit

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Analyzer and warning policy context
- Related examples: Directory.Build.props, Properties/GlobalSuppressions.cs


This audit documents intentional warning suppressions used for educational/demo scenarios in this repository.

## Scope and principles

- Suppressions are allowed only when examples intentionally demonstrate alternatives, anti-patterns, migration paths, or readability-first teaching code.
- Prefer fixing warnings in production-style examples.
- Keep suppressions narrow: project-level `NoWarn` should be limited to compiler/tooling noise that is intentional across demos; analyzer suppressions should be scoped with `Target`.
- Every new suppression should include a clear justification and should be reviewed periodically.
- CI warning gate is enforced for `RevisionNotesDemo.csproj` and `RevisionNotesDemo.UnitTests.csproj`; `TestingExamples` remains build-validated but not warning-gated because it intentionally contains analyzer-triggering teaching patterns.

## Project-level `NoWarn` entries

Source: `Directory.Build.props`

| Rule ID | Scope | Justification | Review owner | Review cadence |
| --- | --- | --- | --- | --- |
| `CS0219` | All projects under repo | Some demos intentionally keep assigned-but-unused locals to show intermediate states in teaching flows. | Maintainers | Quarterly |
| `CS0414` | All projects under repo | Certain class-level fields are intentionally retained to explain state transitions and contrasts. | Maintainers | Quarterly |
| `CS0168` | All projects under repo | Some examples intentionally declare exception placeholders while discussing error handling evolution. | Maintainers | Quarterly |
| `CS0169` | All projects under repo | Some demos include placeholder fields to compare design alternatives. | Maintainers | Quarterly |
| `CS8763` | All projects under repo | Nullable-flow edge cases are intentionally demonstrated in specific educational snippets. | Maintainers | Quarterly |
| `ASPDEPR002` | Web API educational paths | Migration examples intentionally include deprecated API usage for contrast. | Maintainers | Quarterly |
| `IDE0005` | All projects under repo | Teaching snippets preserve imports in places where examples are copy/paste friendly across sections. | Maintainers | Quarterly |

## Analyzer suppressions (`GlobalSuppressions.cs`)

Source: `Properties/GlobalSuppressions.cs`

| Rule ID | Scope/Target | Justification summary | Review owner | Review cadence |
| --- | --- | --- | --- | --- |
| `CA1848` | `RevisionNotesDemo.Logging` | Demonstrates baseline logging APIs before optimized delegate patterns. | Maintainers | Quarterly |
| `CA1707` | `RevisionNotesDemo.Performance` | Uses underscore naming for side-by-side benchmark readability. | Maintainers | Quarterly |
| `CA1707` | `RevisionNotesDemo.DataAccess` | Uses underscore labels to distinguish anti-pattern vs improved variants. | Maintainers | Quarterly |
| `CA1707` | `RevisionNotesDemo.DesignPatterns.Behavioral` | Uses underscore suffixes to separate contrasting variants. | Maintainers | Quarterly |
| `CA1707` | `RevisionNotesDemo.Logging` | Uses test-style naming to match scenario-focused examples. | Maintainers | Quarterly |
| `CA1707` | `RevisionNotesDemo.WebAPI.Versioning` | Uses scenario labels for demo readability. | Maintainers | Quarterly |
| `CA1711` | `RevisionNotesDemo` | Keeps conventional suffixes (`Queue`, `Check`, `Event`) for discoverability. | Maintainers | Quarterly |
| `CA1716` | `RevisionNotesDemo` | Mirrors common API naming patterns used in tutorials. | Maintainers | Quarterly |
| `CA1710` | `RevisionNotesDemo` | Preserves concise names used across teaching samples. | Maintainers | Quarterly |
| `CA1805` | `RevisionNotesDemo` | Keeps explicit defaults for pedagogy. | Maintainers | Quarterly |
| `CA1859` | `RevisionNotesDemo` | Demonstrates interface-first abstraction boundaries. | Maintainers | Quarterly |
| `CA1840` | `RevisionNotesDemo` | Uses `Thread.CurrentThread` intentionally in thread concept demos. | Maintainers | Quarterly |
| `CA1860` | `RevisionNotesDemo` | Prefers LINQ readability in learning-focused samples. | Maintainers | Quarterly |
| `CA1861` | `RevisionNotesDemo` | Keeps inline literals for concise teaching examples. | Maintainers | Quarterly |
| `CA1825` | `RevisionNotesDemo` | Shows explicit array patterns intentionally in some snippets. | Maintainers | Quarterly |
| `CA1847` | `RevisionNotesDemo` | Keeps string-literal forms to match narrative examples. | Maintainers | Quarterly |
| `CA1866` | `RevisionNotesDemo` | Varies overload usage intentionally for API teaching. | Maintainers | Quarterly |
| `CA1845` | `RevisionNotesDemo` | Prioritizes readability over span-level micro-optimization in baseline demos. | Maintainers | Quarterly |
| `CA1829` | `RevisionNotesDemo` | Uses query-style operators intentionally in LINQ-focused sections. | Maintainers | Quarterly |
| `CA1051` | `RevisionNotesDemo` | Some DTO/sample types expose fields for brevity. | Maintainers | Quarterly |
| `CA1000` | `RevisionNotesDemo` | Generic static-member behavior is demonstrated intentionally. | Maintainers | Quarterly |
| `CA1510` | `RevisionNotesDemo` | Includes classic guard clauses for comparison with modern alternatives. | Maintainers | Quarterly |
| `CA2012` | `RevisionNotesDemo` | Includes ValueTask misuse examples to teach pitfalls. | Maintainers | Quarterly |
| `CA5350` | `RevisionNotesDemo.Security` | Uses HMAC-SHA1 in TOTP RFC-compatibility examples. | Maintainers | Quarterly |
| `ASPDEPR002` | `RevisionNotesDemo.WebAPI` | Includes deprecated ASP.NET APIs in migration-oriented examples. | Maintainers | Quarterly |

## Review checklist for new suppressions

1. Confirm warning cannot be fixed without reducing educational value.
2. Scope suppression to the smallest feasible namespace/member/target.
3. Add/update `Justification` with concrete rationale.
4. Record the new suppression in this document.
5. Confirm CI warning gate still passes.

---

## Interview Answer Block

- 30-second answer: This topic covers Suppression governance and review cadence and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know suppression governance and audit discipline and I would just follow best practices."
- Strong answer: "For suppression governance and audit discipline, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply suppression governance and audit discipline in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.
