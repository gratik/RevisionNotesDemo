# Data-Access

This landing page summarizes the Data-Access documentation area and links into topic-level guides.

## Start Here

- [Subject README](Data-Access/README.md)
- [Docs Index](README.md)

## What You Will Learn

- Core principles and operational tradeoffs for this area.
- Practical implementation guidance with production-safe defaults.
- Validation and troubleshooting checklists for real systems.

## Key Topics

- [ADONET-Fundamentals](Data-Access/ADONET-Fundamentals.md)
- [Best-Practices](Data-Access/Best-Practices.md)
- [Choosing-the-Right-Tool](Data-Access/Choosing-the-Right-Tool.md)
- [Common-Pitfalls](Data-Access/Common-Pitfalls.md)
- [Dapper-The-Micro-ORM](Data-Access/Dapper-The-Micro-ORM.md)
- [SQL-Server-Deep-Dive](Data-Access/SQL-Server-Deep-Dive.md)
- [Transaction-Patterns](Data-Access/Transaction-Patterns.md)

## Study Flow

1. Read the subject README for scope and boundaries.
2. Work through topic files relevant to your current project constraints.
3. Capture decisions as measurable changes, then validate in runtime telemetry.

## Detailed Guidance

Data-Access guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Data-Access before implementation work begins.
- Keep boundaries explicit so Data-Access decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Data-Access in production-facing code.
- When performance, correctness, or maintainability depends on consistent Data-Access decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Data-Access as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Data-Access is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Data-Access are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

