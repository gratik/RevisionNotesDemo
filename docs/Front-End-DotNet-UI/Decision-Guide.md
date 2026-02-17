# Decision Guide

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Frontend fundamentals and basic .NET web/UI application structure.
- Related examples: docs/Front-End-DotNet-UI/README.md
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

## Interview Answer Block
30-second answer:
- Decision Guide is about .NET UI stack patterns and frontend integration choices. It matters because UI architecture affects usability, testability, and delivery speed.
- Use it when choosing the right .NET UI approach for product constraints.

2-minute answer:
- Start with the problem Decision Guide solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: rapid UI iteration vs maintainable component structure.
- Close with one failure mode and mitigation: tight coupling between UI and data access concerns.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Decision Guide but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Decision Guide, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Decision Guide and map it to one concrete implementation in this module.
- 3 minutes: compare Decision Guide with an alternative, then walk through one failure mode and mitigation.