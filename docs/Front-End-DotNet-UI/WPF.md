# WPF

> Subject: [Front-End-DotNet-UI](../README.md)

## WPF

### Strengths

- Powerful data binding and templating
- Rich Windows UI capabilities
- MVVM-friendly

### Good vs Bad

```csharp
// BAD: Code-behind owns all state
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        NameTextBox.Text = _service.GetName();
    }
}
```

```xml
<!-- GOOD: XAML bindings with commands -->
<Window>
  <StackPanel>
    <TextBox Text="{Binding Name, UpdateSourceTrigger=PropertyChanged}" />
    <Button Content="Save" Command="{Binding SaveCommand}" />
  </StackPanel>
</Window>
```

### Pitfalls

- Too much logic in code-behind
- Not leveraging data templates

### Validation Examples

WPF validation is strongest with `ValidationRule` or `IDataErrorInfo` in the view model.
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/WpfUiExamples.cs](../../Learning/FrontEnd/WpfUiExamples.cs).

---



