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



