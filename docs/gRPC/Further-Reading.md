# Further Reading

> Subject: [gRPC](../README.md)

## Further Reading

- [Official gRPC Documentation](https://grpc.io/)
- [gRPC for ASP.NET Core](https://learn.microsoft.com/aspnet/core/grpc/)
- [Protocol Buffers Guide](https://protobuf.dev/)

<!-- STUDY-NEXT-START -->

## Detailed Guidance

Realtime communication guidance focuses on contract evolution, latency budgets, and resilience under variable network conditions.

### Design Notes
- Define success criteria for Further Reading before implementation work begins.
- Keep boundaries explicit so Further Reading decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Further Reading in production-facing code.
- When performance, correctness, or maintainability depends on consistent Further Reading decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Further Reading as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Further Reading is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Further Reading are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

