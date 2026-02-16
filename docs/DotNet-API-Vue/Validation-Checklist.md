# Validation Checklist

> Subject: [DotNet-API-Vue](../README.md)

## Validation Checklist

- Vue components call composables/services, not raw API endpoints.
- Axios instance handles auth and normalized errors centrally.
- Backend validates pagination/filter inputs explicitly.
- ProblemDetails is enabled for consistent non-2xx responses.
- Local and production API base routing are documented and tested.
- Authorization policies and scopes are explicit and covered by tests.
- Logs are structured with correlation ids and sensitive data redaction.
- Dashboards include latency, error-rate, and dependency health views.

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

