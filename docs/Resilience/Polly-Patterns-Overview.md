# Polly Patterns Overview

> Subject: [Resilience](../README.md)

## Polly Patterns Overview

| Pattern | Purpose | When to Use |
|---------|---------|-------------|
| **Retry** | Try again after failure | Transient errors (network blips) |
| **Circuit Breaker** | Stop calling failing service | Persistent failures (service down) |
| **Timeout** | Bound operation duration | Prevent hanging |
| **Bulkhead** | Limit concurrent operations | Prevent resource exhaustion |
| **Fallback** | Return default on failure | Provide degraded experience |

---

## Detailed Guidance

Resilience guidance focuses on bounded degradation, dependency isolation, and measurable recovery behavior.

### Design Notes
- Define success criteria for Polly Patterns Overview before implementation work begins.
- Keep boundaries explicit so Polly Patterns Overview decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Polly Patterns Overview in production-facing code.
- When performance, correctness, or maintainability depends on consistent Polly Patterns Overview decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Polly Patterns Overview as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Polly Patterns Overview is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Polly Patterns Overview are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

