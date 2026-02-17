# gRPC vs REST

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: HTTP/2 basics, protobuf awareness, and service contract versioning.
- Related examples: docs/gRPC/README.md
> Subject: [gRPC](../README.md)

## gRPC vs REST

| Feature | gRPC | REST |
|---------|------|------|
| **Protocol** | HTTP/2 | HTTP/1.1 |
| **Format** | Protocol Buffers (binary) | JSON (text) |
| **Performance** | 7-10x faster | Baseline |
| **Streaming** | First-class support | Limited |
| **Browser Support** | gRPC-Web required | Native |
| **Type Safety** | Strong (generated code) | Weak (runtime) |
| **Payload Size** | Smaller (binary) | Larger (text) |
| **Use Case** | Internal services | Public APIs |

---

## Detailed Guidance

Realtime communication guidance focuses on contract evolution, latency budgets, and resilience under variable network conditions.

### Design Notes
- Define success criteria for gRPC vs REST before implementation work begins.
- Keep boundaries explicit so gRPC vs REST decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring gRPC vs REST in production-facing code.
- When performance, correctness, or maintainability depends on consistent gRPC vs REST decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying gRPC vs REST as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where gRPC vs REST is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for gRPC vs REST are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- gRPC vs REST is about contract-first RPC communication. It matters because it optimizes service-to-service communication and typed contracts.
- Use it when high-throughput internal service calls with strict schemas.

2-minute answer:
- Start with the problem gRPC vs REST solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: performance and typing vs ecosystem/browser constraints.
- Close with one failure mode and mitigation: choosing gRPC for scenarios where REST interoperability is required.
## Interview Bad vs Strong Answer
Bad answer:
- Defines gRPC vs REST but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose gRPC vs REST, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define gRPC vs REST and map it to one concrete implementation in this module.
- 3 minutes: compare gRPC vs REST with an alternative, then walk through one failure mode and mitigation.