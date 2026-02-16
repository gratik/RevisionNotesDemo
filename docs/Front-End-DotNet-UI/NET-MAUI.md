# .NET MAUI

> Subject: [Front-End-DotNet-UI](../README.md)

## .NET MAUI

### Strengths

- Single project targeting mobile + desktop
- XAML data binding with MVVM
- Native UI performance

### Good vs Bad

```csharp
// BAD: Blocking calls and heavy logic in code-behind
public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        var json = new HttpClient().GetStringAsync("https://api").Result;
        TitleLabel.Text = json;
    }
}
```

```xml
<!-- GOOD: Bindings and commands -->
<ContentPage>
  <VerticalStackLayout>
    <Label Text="{Binding Title}" />
    <Button Text="Refresh" Command="{Binding RefreshCommand}" />
  </VerticalStackLayout>
</ContentPage>
```

### Pitfalls

- Putting platform-specific code in shared layers
- Not using handlers or DI for platform services

### Validation Examples

MAUI validation should stay in view models with `TryParse` and user-friendly errors.
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/MauiUiExamples.cs](../../Learning/FrontEnd/MauiUiExamples.cs).

---

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for .NET MAUI before implementation work begins.
- Keep boundaries explicit so .NET MAUI decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring .NET MAUI in production-facing code.
- When performance, correctness, or maintainability depends on consistent .NET MAUI decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying .NET MAUI as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where .NET MAUI is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for .NET MAUI are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

