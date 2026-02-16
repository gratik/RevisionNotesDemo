# When to Use gRPC

> Subject: [gRPC](../README.md)

## When to Use gRPC

✅ **Best for:**
- **Microservices** communication (inter-service calls)
- **Real-time** applications (streaming, live updates)
- **Low-latency** requirements
- **Internal APIs** (service mesh, backend services)
- **High-throughput** scenarios (1000s of requests/sec)
- **Type-safe contracts** between services

❌ **Avoid for:**
- Public web APIs (browser support limited)
- Simple CRUD APIs (REST more appropriate)
- Teams unfamiliar with Proto Buffers
- When JSON human-readability required

---

## Detailed Guidance

Realtime communication guidance focuses on contract evolution, latency budgets, and resilience under variable network conditions.

### Design Notes
- Define success criteria for When to Use gRPC before implementation work begins.
- Keep boundaries explicit so When to Use gRPC decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring When to Use gRPC in production-facing code.
- When performance, correctness, or maintainability depends on consistent When to Use gRPC decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying When to Use gRPC as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where When to Use gRPC is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for When to Use gRPC are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

