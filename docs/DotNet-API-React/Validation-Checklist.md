# Validation Checklist

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: REST API basics and React data-fetching/state management familiarity.
- Related examples: docs/DotNet-API-React/README.md
> Subject: [DotNet-API-React](../README.md)

## Validation Checklist

- Endpoint returns typed DTOs and explicit status annotations.
- React client path does not duplicate base URL string literals.
- Non-2xx responses are parsed into user-facing error state.
- Local dev and production origins are both represented in CORS policy.
- Paging/filter constraints are validated server-side.
- Token validation and authorization policies are explicit and test-covered.
- Logs include correlation identifiers and avoid sensitive payload data.
- Telemetry dashboard includes p95 latency, route error rate, and deployment markers.

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for Validation Checklist before implementation work begins.
- Keep boundaries explicit so Validation Checklist decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Validation Checklist in production-facing code.
- When performance, correctness, or maintainability depends on consistent Validation Checklist decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Validation Checklist as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Validation Checklist is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Validation Checklist are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Validation Checklist is about backend/frontend integration design for React clients. It matters because contract and state decisions affect delivery speed and reliability.
- Use it when building resilient API surfaces consumed by React applications.

2-minute answer:
- Start with the problem Validation Checklist solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: rich backend contracts vs frontend adaptability.
- Close with one failure mode and mitigation: inconsistent API error/validation contracts across endpoints.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Validation Checklist but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Validation Checklist, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Validation Checklist and map it to one concrete implementation in this module.
- 3 minutes: compare Validation Checklist with an alternative, then walk through one failure mode and mitigation.