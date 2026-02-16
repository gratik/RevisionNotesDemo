# Quick Reference

> Subject: [LINQ-Queries](../README.md)

## Quick Reference

```csharp
var active = users
    .Where(u => u.IsActive)
    .Select(u => new { u.Id, u.Name });
```

## Detailed Guidance

LINQ guidance focuses on readable query composition while avoiding hidden multiple enumerations and expensive client-side execution.

### Design Notes
- Define success criteria for Quick Reference before implementation work begins.
- Keep boundaries explicit so Quick Reference decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Quick Reference in production-facing code.
- When performance, correctness, or maintainability depends on consistent Quick Reference decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Quick Reference as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Quick Reference is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Quick Reference are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

