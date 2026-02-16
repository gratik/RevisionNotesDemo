# Common Failure Modes

> Subject: [DotNet-API-Vue](../README.md)

## Common Failure Modes

| Failure mode | Root cause | Prevention |
| --- | --- | --- |
| API logic scattered across components | No service/composable layer | Centralized API service + composables |
| Inconsistent auth behavior | Token handling duplicated | Request interceptor policy |
| Unexpected 400s on paging | Missing backend parameter guardrails | Validate query parameters and return clear validation details |
| Environment-specific integration bugs | Different local/prod routing assumptions | Vite proxy in dev + explicit production API base config |

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for Common Failure Modes before implementation work begins.
- Keep boundaries explicit so Common Failure Modes decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Common Failure Modes in production-facing code.
- When performance, correctness, or maintainability depends on consistent Common Failure Modes decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Common Failure Modes as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Common Failure Modes is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Common Failure Modes are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

