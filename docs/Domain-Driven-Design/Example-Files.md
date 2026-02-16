# Example Files

> Subject: [Domain-Driven-Design](../README.md)

## Example Files

📁 **Learning/DomainDrivenDesign/EntityValueObjectExamples.cs**
- Anemic vs Rich domain models
- Entity patterns
- Value Object patterns
- Domain validation
- Equality semantics

📁 **Learning/DomainDrivenDesign/AggregateRootExamples.cs**
- Aggregate boundaries
- Enforcing invariants
- Aggregate design rules
- Repository pattern for aggregates

---

## Detailed Guidance

Example Files guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Example Files before implementation work begins.
- Keep boundaries explicit so Example Files decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Example Files in production-facing code.
- When performance, correctness, or maintainability depends on consistent Example Files decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Example Files as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Example Files is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Example Files are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

