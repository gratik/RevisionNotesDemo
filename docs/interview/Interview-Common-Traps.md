# Interview Common Traps

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Interview Answer Blocks
- Related examples: Learning/Resilience/PollyRetryPatterns.cs, Learning/DataAccess/EntityFramework/EntityFrameworkBestPractices.cs

Use this checklist to avoid weak answers during technical interviews.

## Trap 1: Generic “best practices” answer

- Weak pattern: “I follow best practices for everything.”
- Better move: Name the concrete constraint, decision, and tradeoff.

## Trap 2: No measurable outcomes

- Weak pattern: “Performance improved a lot.”
- Better move: Provide specific metrics (for example p95 latency, error rate, throughput).

## Trap 3: Pattern-first, problem-second thinking

- Weak pattern: “We used clean architecture because it’s standard.”
- Better move: Explain the problem first, then why the pattern was justified.

## Trap 4: Retry-only resilience strategy

- Weak pattern: “We added retries so failures were handled.”
- Better move: Combine timeout budgets, breaker behavior, and retry caps with jitter.

## Trap 5: Security treated as authentication only

- Weak pattern: “JWT solved security.”
- Better move: Cover authorization, secret handling, data protection, and auditing.

## Trap 6: Data access with no provider awareness

- Weak pattern: “EF was slow so we moved everything to raw SQL.”
- Better move: Show EF tuning first, then justify targeted raw SQL for hot paths.

## Trap 7: Missing rollback and recovery plan

- Weak pattern: “We deployed and monitored.”
- Better move: Describe rollback trigger, rollback method, and data recovery path.

## Trap 8: No ownership boundaries in system design

- Weak pattern: “Services communicate over events.”
- Better move: Define contract ownership, idempotency strategy, and failure-handling ownership.

## Detailed Guidance

Interview Common Traps guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Interview Common Traps before implementation work begins.
- Keep boundaries explicit so Interview Common Traps decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Interview Common Traps in production-facing code.
- When performance, correctness, or maintainability depends on consistent Interview Common Traps decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Interview Common Traps as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Interview Common Traps is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Interview Common Traps are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
- 30-second answer: Common interview traps are generic answers, missing tradeoffs, and no measurable outcomes.
- 2-minute deep dive: I avoid traps by using structured responses: context, decision, alternatives, risks, mitigation, and measurable validation.
- Common follow-up: How do you recover if you start giving a weak answer?
- Strong response: Reset quickly by stating assumptions and providing one concrete decision with tradeoff and metric.
- Tradeoff callout: Overly long answers reduce clarity even if technically correct.

## Interview Bad vs Strong Answer

- Bad answer: "I did best practices and improved everything."
- Strong answer: "I addressed p95 latency from 420ms to 180ms by fixing N+1 queries and adding bounded retries with timeout budgets."
- Why strong wins: It provides specific action, rationale, and measurable outcome.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Pick one trap from this page and show how you would answer the same question in weak vs strong form.
- Required outputs:
  - One weak answer example
  - One improved strong answer
  - One validation metric or follow-up proof point
- Self-check score (0-3 each): specificity, tradeoff depth, communication clarity.


