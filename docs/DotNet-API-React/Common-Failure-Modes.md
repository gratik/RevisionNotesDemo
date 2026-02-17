# Common Failure Modes

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: REST API basics and React data-fetching/state management familiarity.
- Related examples: docs/DotNet-API-React/README.md
> Subject: [DotNet-API-React](../README.md)

## Common Failure Modes

| Failure mode | Root cause | Prevention |
| --- | --- | --- |
| Random CORS errors | Middleware order or missing origin | Explicit policy + correct middleware order |
| Inconsistent errors in UI | Mixed backend error format | Standard `ProblemDetails` envelope |
| Memory leaks/warnings in React | Uncanceled async requests | `AbortController` in effect cleanup |
| Slow list pages | Returning full entities | API-side projection + paging DTO |

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

## Interview Answer Block
30-second answer:
- Common Failure Modes is about backend/frontend integration design for React clients. It matters because contract and state decisions affect delivery speed and reliability.
- Use it when building resilient API surfaces consumed by React applications.

2-minute answer:
- Start with the problem Common Failure Modes solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: rich backend contracts vs frontend adaptability.
- Close with one failure mode and mitigation: inconsistent API error/validation contracts across endpoints.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Failure Modes but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Failure Modes, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Failure Modes and map it to one concrete implementation in this module.
- 3 minutes: compare Common Failure Modes with an alternative, then walk through one failure mode and mitigation.