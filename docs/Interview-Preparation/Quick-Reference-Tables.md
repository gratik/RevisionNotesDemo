# Quick Reference Tables

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Comfort with core module topics and deliberate timed practice.
- Related examples: docs/Interview-Preparation/README.md
> Subject: [Interview-Preparation](../README.md)

## Quick Reference Tables

### Do's and Don'ts

| Topic            | ✅ Do                               | ❌ Avoid                          |
| ---------------- | ----------------------------------- | --------------------------------- |
| **Async I/O**    | Use async/await end-to-end          | Blocking with .Result or .Wait()  |
| **CPU Work**     | Use Task.Run for expensive work     | Marking CPU methods as async      |
| **Logging**      | Use structured templates            | String interpolation in hot paths |
| **EF Core**      | Use AsNoTracking for reads          | Tracking every query by default   |
| **Caching**      | Set expirations and size limits     | Unbounded cache growth            |
| **DI Lifetimes** | Scoped for request services         | Singleton for per-request state   |
| **Exceptions**   | Use specific exception types        | Catching Exception (too broad)    |
| **Strings**      | Use StringBuilder for concatenation | += in loops                       |
| **Collections**  | Use appropriate collection type     | List<T> for everything            |
| **Nullability**  | Enable nullable reference types     | Ignoring null warnings            |

---

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for Quick Reference Tables before implementation work begins.
- Keep boundaries explicit so Quick Reference Tables decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Quick Reference Tables in production-facing code.
- When performance, correctness, or maintainability depends on consistent Quick Reference Tables decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Quick Reference Tables as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Quick Reference Tables is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Quick Reference Tables are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Quick Reference Tables is about communication structure for technical interviews. It matters because clear articulation of tradeoffs improves interview signal quality.
- Use it when translating implementation knowledge into concise answers.

2-minute answer:
- Start with the problem Quick Reference Tables solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: brevity vs sufficient technical depth.
- Close with one failure mode and mitigation: memorized answers that ignore problem context.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Quick Reference Tables but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Quick Reference Tables, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Quick Reference Tables and map it to one concrete implementation in this module.
- 3 minutes: compare Quick Reference Tables with an alternative, then walk through one failure mode and mitigation.