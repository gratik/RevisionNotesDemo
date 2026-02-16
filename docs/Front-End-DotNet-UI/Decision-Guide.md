# Decision Guide

> Subject: [Front-End-DotNet-UI](../README.md)

## Decision Guide

- Choose **MVC** when you need complex routing, controllers, and server-rendered SEO.
- Choose **Razor Pages** for page-focused apps with a simpler mental model.
- Choose **Blazor** when you want component-based UI in C# across web (and hybrid).
- Choose **.NET MAUI** for a single codebase targeting mobile + desktop.
- Choose **WPF** for modern Windows-only desktop UI with rich data binding.
- Choose **WinForms** for quick Windows desktop tools and legacy maintenance.
- Choose **Web Forms** only for legacy systems on .NET Framework.

---

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for Decision Guide before implementation work begins.
- Keep boundaries explicit so Decision Guide decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Decision Guide in production-facing code.
- When performance, correctness, or maintainability depends on consistent Decision Guide decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Decision Guide as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Decision Guide is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Decision Guide are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

