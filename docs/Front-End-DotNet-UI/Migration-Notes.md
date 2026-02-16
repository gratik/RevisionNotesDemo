# Migration Notes

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

