# Best Practices

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: REST API patterns and Vue component/state fundamentals.
- Related examples: docs/DotNet-API-Vue/README.md
> Subject: [DotNet-API-Vue](../README.md)

## Best Practices

1. Use a shared axios instance with request/response interceptors.
2. Keep token injection and error normalization in one place.
3. Implement composables (`useProducts`, `useOrders`) for loading/error lifecycle.
4. Use Vite proxy in local dev to reduce CORS drift.
5. Return validation and fault details as standard `ProblemDetails`.
6. Enforce server-side pagination boundaries (`pageSize` limits).
7. Keep response payloads small via list-specific DTOs.

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

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

## Interview Answer Block
30-second answer:
- Best Practices is about backend/frontend integration design for Vue clients. It matters because consistent contracts reduce frontend complexity and defects.
- Use it when implementing API patterns that scale with Vue feature growth.

2-minute answer:
- Start with the problem Best Practices solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: strict contracts vs rapid iteration flexibility.
- Close with one failure mode and mitigation: frontend workarounds due to ambiguous backend conventions.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Best Practices but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Best Practices, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Best Practices and map it to one concrete implementation in this module.
- 3 minutes: compare Best Practices with an alternative, then walk through one failure mode and mitigation.