# DotNet-Concepts

This landing page summarizes the DotNet-Concepts documentation area and links into topic-level guides.

## Start Here

- [Subject README](DotNet-Concepts/README.md)
- [Docs Index](README.md)

## What You Will Learn

- Core principles and operational tradeoffs for this area.
- Practical implementation guidance with production-safe defaults.
- Validation and troubleshooting checklists for real systems.

## Key Topics

- [Best-Practices](DotNet-Concepts/Best-Practices.md)
- [Common-Pitfalls](DotNet-Concepts/Common-Pitfalls.md)
- [Constructor-Injection](DotNet-Concepts/Constructor-Injection.md)
- [Keyed-Services-C-12-NET-8](DotNet-Concepts/Keyed-Services-C-12-NET-8.md)
- [Registration-Methods](DotNet-Concepts/Registration-Methods.md)
- [Scoped-Services-in-Singletons-Captive-Dependency](DotNet-Concepts/Scoped-Services-in-Singletons-Captive-Dependency.md)
- [Service-Lifetimes](DotNet-Concepts/Service-Lifetimes.md)
- [Service-Location-Anti-Pattern](DotNet-Concepts/Service-Location-Anti-Pattern.md)
- [Validation](DotNet-Concepts/Validation.md)
- [What-is-Dependency-Injection](DotNet-Concepts/What-is-Dependency-Injection.md)

## Study Flow

1. Read the subject README for scope and boundaries.
2. Work through topic files relevant to your current project constraints.
3. Capture decisions as measurable changes, then validate in runtime telemetry.

## Detailed Guidance

DotNet-Concepts guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for DotNet-Concepts before implementation work begins.
- Keep boundaries explicit so DotNet-Concepts decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring DotNet-Concepts in production-facing code.
- When performance, correctness, or maintainability depends on consistent DotNet-Concepts decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying DotNet-Concepts as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where DotNet-Concepts is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for DotNet-Concepts are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

