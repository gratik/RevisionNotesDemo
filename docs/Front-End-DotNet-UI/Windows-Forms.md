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



