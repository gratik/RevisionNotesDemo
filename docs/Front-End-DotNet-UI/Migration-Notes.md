# Migration Notes

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Frontend fundamentals and basic .NET web/UI application structure.
- Related examples: docs/Front-End-DotNet-UI/README.md
> Subject: [Front-End-DotNet-UI](../README.md)

## Migration Notes

- **Web Forms -> MVC/Razor Pages**: Move server controls to tag helpers, replace view state with explicit models, and use data annotations + `ModelState`.
- **WinForms -> WPF**: Keep business logic, move UI to XAML + MVVM, and use `ValidationRule` or `IDataErrorInfo` for input.
- **WPF/WinForms -> .NET MAUI**: Reuse view models, rewrite views in MAUI XAML, and validate in the VM with user-friendly errors.
- **MVC -> Blazor**: Promote view models to component parameters and shift UI logic into components; keep server validation on APIs.

---

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for Migration Notes before implementation work begins.
- Keep boundaries explicit so Migration Notes decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Migration Notes in production-facing code.
- When performance, correctness, or maintainability depends on consistent Migration Notes decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Migration Notes as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Migration Notes is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Migration Notes are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

## Interview Answer Block
30-second answer:
- Migration Notes is about .NET UI stack patterns and frontend integration choices. It matters because UI architecture affects usability, testability, and delivery speed.
- Use it when choosing the right .NET UI approach for product constraints.

2-minute answer:
- Start with the problem Migration Notes solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: rapid UI iteration vs maintainable component structure.
- Close with one failure mode and mitigation: tight coupling between UI and data access concerns.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Migration Notes but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Migration Notes, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Migration Notes and map it to one concrete implementation in this module.
- 3 minutes: compare Migration Notes with an alternative, then walk through one failure mode and mitigation.