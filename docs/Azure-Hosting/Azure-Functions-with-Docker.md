# Azure Functions with Docker

> Subject: [Azure-Hosting](../README.md)

## Azure Functions with Docker

### When to Dockerize Functions

- Native dependencies are required
- You need strict runtime version control
- Local and cloud parity must be exact

### Hosting Notes

- Consumption: cheapest for spiky traffic
- Premium: warm instances, VNet, lower cold start impact
- Dedicated: predictable baseline and long-running workloads

---

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for Azure Functions with Docker before implementation work begins.
- Keep boundaries explicit so Azure Functions with Docker decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Azure Functions with Docker in production-facing code.
- When performance, correctness, or maintainability depends on consistent Azure Functions with Docker decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Azure Functions with Docker as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Azure Functions with Docker is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Azure Functions with Docker are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

