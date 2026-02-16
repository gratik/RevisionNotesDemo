# Core-CSharp

This landing page summarizes the Core-CSharp documentation area and links into topic-level guides.

## Start Here

- [Subject README](Core-CSharp/README.md)
- [Docs Index](README.md)

## What You Will Learn

- Core principles and operational tradeoffs for this area.
- Practical implementation guidance with production-safe defaults.
- Validation and troubleshooting checklists for real systems.

## Key Topics

- [Best-Practices](Core-CSharp/Best-Practices.md)
- [Common-Pitfalls](Core-CSharp/Common-Pitfalls.md)
- [Covariance-and-Contravariance](Core-CSharp/Covariance-and-Contravariance.md)
- [Delegates-and-Events](Core-CSharp/Delegates-and-Events.md)
- [Extension-Methods](Core-CSharp/Extension-Methods.md)
- [Generics](Core-CSharp/Generics.md)
- [Interfaces](Core-CSharp/Interfaces.md)

## Study Flow

1. Read the subject README for scope and boundaries.
2. Work through topic files relevant to your current project constraints.
3. Capture decisions as measurable changes, then validate in runtime telemetry.

## Detailed Guidance

Core-CSharp guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Core-CSharp before implementation work begins.
- Keep boundaries explicit so Core-CSharp decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Core-CSharp in production-facing code.
- When performance, correctness, or maintainability depends on consistent Core-CSharp decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Core-CSharp as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Core-CSharp is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Core-CSharp are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

