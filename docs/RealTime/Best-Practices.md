# Best Practices

> Subject: [RealTime](../README.md)

## Best Practices

### ✅ Hub Design
- Keep hub methods simple and fast
- Don't do heavy work in hub methods
- Use background services for long-running tasks
- Return Task for all hub methods
- Use strongly-typed hubs when possible

### ✅ Groups
- Clean up groups on disconnect
- Use meaningful group names
- Consider group size (thousands per group is OK)
- Don't store state in hub (use database or cache)

### ✅ Security
- Always authenticate sensitive operations
- Validate all inputs
- Rate limit hub method invocations
- Don't trust client data

### ✅ Performance
- Use Redis for multi-server scenarios
- Limit message size
- Batch messages when possible
- Monitor connection count

---

## Detailed Guidance

Realtime communication guidance focuses on contract evolution, latency budgets, and resilience under variable network conditions.

### Design Notes
- Define success criteria for Best Practices before implementation work begins.
- Keep boundaries explicit so Best Practices decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Best Practices in production-facing code.
- When performance, correctness, or maintainability depends on consistent Best Practices decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Best Practices as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Best Practices is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Best Practices are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

