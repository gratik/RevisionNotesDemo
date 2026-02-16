# Reference Implementation

> Subject: [Distributed-Consistency](../README.md)

## Reference Implementation

- [Microservices/DistributedConsistencyPatterns.cs](../../Learning/Microservices/DistributedConsistencyPatterns.cs)

Includes:
- outbox + inbox simulation
- idempotency replay handling
- saga compensation walkthrough

---

## Detailed Guidance

Reference Implementation guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Reference Implementation before implementation work begins.
- Keep boundaries explicit so Reference Implementation decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Reference Implementation in production-facing code.
- When performance, correctness, or maintainability depends on consistent Reference Implementation decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Reference Implementation as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Reference Implementation is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Reference Implementation are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

