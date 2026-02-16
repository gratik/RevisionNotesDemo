# Design-Patterns

This landing page summarizes the Design-Patterns documentation area and links into topic-level guides.

## Start Here

- [Subject README](Design-Patterns/README.md)
- [Docs Index](README.md)

## What You Will Learn

- Core principles and operational tradeoffs for this area.
- Practical implementation guidance with production-safe defaults.
- Validation and troubleshooting checklists for real systems.

## Key Topics

- [Behavioral-Patterns](Design-Patterns/Behavioral-Patterns.md)
- [Best-Practices](Design-Patterns/Best-Practices.md)
- [Common-Pitfalls](Design-Patterns/Common-Pitfalls.md)
- [Creational-Patterns](Design-Patterns/Creational-Patterns.md)
- [Modern-Patterns](Design-Patterns/Modern-Patterns.md)
- [Structural-Patterns](Design-Patterns/Structural-Patterns.md)

## Study Flow

1. Read the subject README for scope and boundaries.
2. Work through topic files relevant to your current project constraints.
3. Capture decisions as measurable changes, then validate in runtime telemetry.

## Detailed Guidance

Design-Patterns guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Design-Patterns before implementation work begins.
- Keep boundaries explicit so Design-Patterns decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Design-Patterns in production-facing code.
- When performance, correctness, or maintainability depends on consistent Design-Patterns decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Design-Patterns as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Design-Patterns is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Design-Patterns are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

