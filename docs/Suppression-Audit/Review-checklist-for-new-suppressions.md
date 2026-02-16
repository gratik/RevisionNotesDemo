# Review checklist for new suppressions

> Subject: [Suppression-Audit](../README.md)

## Review checklist for new suppressions

1. Confirm warning cannot be fixed without reducing educational value.
2. Scope suppression to the smallest feasible namespace/member/target.
3. Add/update `Justification` with concrete rationale.
4. Record the new suppression in this document.
5. Confirm CI warning gate still passes.

---

## Detailed Guidance

Delivery/platform guidance focuses on safe change velocity through policy gates, rollout controls, and clear ownership.

### Design Notes
- Define success criteria for Review checklist for new suppressions before implementation work begins.
- Keep boundaries explicit so Review checklist for new suppressions decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Review checklist for new suppressions in production-facing code.
- When performance, correctness, or maintainability depends on consistent Review checklist for new suppressions decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Review checklist for new suppressions as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Review checklist for new suppressions is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Review checklist for new suppressions are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

