# Windows Forms

> Subject: [Front-End-DotNet-UI](../README.md)

## Windows Forms

### Strengths

- Designer-driven productivity
- Great for quick Windows tools

### Good vs Bad

```csharp
// BAD: One massive form with mixed concerns
public class MainForm : Form
{
    // 2,000 lines of UI + data access + rules
}
```

```csharp
// GOOD: Separate UI and logic via presenter
public class OrdersForm : Form
{
    public OrdersForm(IOrdersPresenter presenter)
    {
        InitializeComponent();
        presenter.Bind(this);
    }
}
```

### Pitfalls

- UI scaling issues on high-DPI screens
- Tight coupling between UI and data access

### Validation Examples

WinForms validation should prefer `TryParse` and `ErrorProvider` feedback.
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/WinFormsUiExamples.cs](../../Learning/FrontEnd/WinFormsUiExamples.cs).

---

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for Windows Forms before implementation work begins.
- Keep boundaries explicit so Windows Forms decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Windows Forms in production-facing code.
- When performance, correctness, or maintainability depends on consistent Windows Forms decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Windows Forms as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Windows Forms is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Windows Forms are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

