# Patterns: When to Use vs Overused

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [Interview-Preparation](../README.md)

## Patterns: When to Use vs Overused

### Singleton Pattern

**Overused because**: Hidden global state, hard to test, lifetime coupling

**Prefer instead**: Use DI with `AddSingleton` and pass dependencies explicitly

**Still valid when**: Truly global infrastructure with stable lifecycle (logging, configuration)

---

### Abstract Factory Pattern

**Overused because**: Too many layers for simple object creation

**Prefer instead**: Use DI, configuration, or simple factory methods

**Still valid when**: Multiple product families must stay consistent

---

### Service Locator Pattern

**Overused because**: Hidden dependencies, runtime failures

**Prefer instead**: Constructor injection with explicit dependencies

**Still valid when**: Legacy frameworks where DI is not possible

---

### Repository Pattern (for every entity)

**Overused because**: Extra abstraction when EF Core DbSet already acts as repository

**Prefer instead**: Use DbContext directly with query-focused services

**Still valid when**: Complex domain rules or multiple data sources

---

## Detailed Guidance

Patterns: When to Use vs Overused guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Patterns: When to Use vs Overused before implementation work begins.
- Keep boundaries explicit so Patterns: When to Use vs Overused decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Patterns: When to Use vs Overused in production-facing code.
- When performance, correctness, or maintainability depends on consistent Patterns: When to Use vs Overused decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Patterns: When to Use vs Overused as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Patterns: When to Use vs Overused is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Patterns: When to Use vs Overused are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

