# Entity-Framework

This landing page summarizes the Entity-Framework documentation area and links into topic-level guides.

## Start Here

- [Subject README](Entity-Framework/README.md)
- [Docs Index](README.md)

## What You Will Learn

- Core principles and operational tradeoffs for this area.
- Practical implementation guidance with production-safe defaults.
- Validation and troubleshooting checklists for real systems.

## Key Topics

- [Advanced-Patterns](Entity-Framework/Advanced-Patterns.md)
- [Best-Practices](Entity-Framework/Best-Practices.md)
- [Common-Pitfalls](Entity-Framework/Common-Pitfalls.md)
- [DbContext-Configuration](Entity-Framework/DbContext-Configuration.md)
- [Migrations](Entity-Framework/Migrations.md)
- [Multi-Tenancy-with-Global-Query-Filters](Entity-Framework/Multi-Tenancy-with-Global-Query-Filters.md)
- [Query-Performance](Entity-Framework/Query-Performance.md)
- [Relationships-and-Navigation-Properties](Entity-Framework/Relationships-and-Navigation-Properties.md)
- [Shadow-Properties-Table-Splitting](Entity-Framework/Shadow-Properties-Table-Splitting.md)
- [Tracking-vs-No-Tracking-Queries](Entity-Framework/Tracking-vs-No-Tracking-Queries.md)

## Study Flow

1. Read the subject README for scope and boundaries.
2. Work through topic files relevant to your current project constraints.
3. Capture decisions as measurable changes, then validate in runtime telemetry.

## Detailed Guidance

Entity-Framework guidance focuses on turning this topic into explicit, measurable engineering decisions rather than abstract rules.

### Design Notes
- Define success criteria for Entity-Framework before implementation work begins.
- Keep boundaries explicit so Entity-Framework decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Entity-Framework in production-facing code.
- When performance, correctness, or maintainability depends on consistent Entity-Framework decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Entity-Framework as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Entity-Framework is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Entity-Framework are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

