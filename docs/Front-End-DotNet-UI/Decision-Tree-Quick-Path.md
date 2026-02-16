# Decision Tree (Quick Path)

> Subject: [Front-End-DotNet-UI](../README.md)

## Decision Tree (Quick Path)

1. **Target platform?**
   - **Web only** -> MVC / Razor Pages / Blazor
   - **Windows desktop** -> WPF / WinForms
   - **Cross-platform desktop + mobile** -> .NET MAUI
2. **UI model preference?**
   - **Page + controller** -> MVC
   - **Page-focused handlers** -> Razor Pages
   - **Component-based UI** -> Blazor
3. **Interactivity level?**
   - **Mostly server-rendered, simple interactions** -> MVC / Razor Pages
   - **Rich interactivity in C#** -> Blazor
4. **Legacy constraints?**
   - **Existing Web Forms / WinForms** -> Modernize incrementally (see Migration Notes)

---

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for Decision Tree (Quick Path) before implementation work begins.
- Keep boundaries explicit so Decision Tree (Quick Path) decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Decision Tree (Quick Path) in production-facing code.
- When performance, correctness, or maintainability depends on consistent Decision Tree (Quick Path) decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Decision Tree (Quick Path) as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Decision Tree (Quick Path) is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Decision Tree (Quick Path) are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

