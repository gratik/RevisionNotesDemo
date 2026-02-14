# Front-End .NET UI Technologies

> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

## Overview

This guide compares front-end .NET UI technologies and shows when to choose each.
Examples are illustrative only and focus on good vs bad patterns rather than full templates.

Covered technologies:

- ASP.NET Core MVC
- Razor Pages
- Blazor (Server, WebAssembly, Hybrid)
- .NET MAUI
- WPF
- Windows Forms
- ASP.NET Web Forms

---

## Quick Comparison

| Technology           | Platforms                     | Rendering Model                | Best For                            | Tradeoffs                                |
| -------------------- | ----------------------------- | ------------------------------ | ----------------------------------- | ---------------------------------------- |
| **ASP.NET Core MVC** | Web                           | Server-rendered HTML           | SEO-friendly sites, complex routing | More boilerplate than Razor Pages        |
| **Razor Pages**      | Web                           | Server-rendered HTML           | Page-focused sites, CRUD            | Less flexible for complex routing        |
| **Blazor**           | Web, Desktop, Mobile (Hybrid) | Components (SSR, Server, WASM) | Interactive UIs in C#               | Payload size, interop complexity         |
| **.NET MAUI**        | Windows, macOS, iOS, Android  | Native UI via XAML             | Cross-platform apps                 | Platform-specific tuning                 |
| **WPF**              | Windows                       | XAML, data binding             | Rich Windows desktop apps           | Windows-only                             |
| **WinForms**         | Windows                       | Event-driven, designer         | Rapid desktop apps                  | UI scaling, legacy patterns              |
| **Web Forms**        | Web (.NET Framework)          | Server controls + view state   | Legacy apps                         | View state bloat, limited modern tooling |

---

## Extended Comparison

| Technology           | Startup/Perf              | Deployment Model              | Testability                  | Offline Support  | Accessibility Defaults       |
| -------------------- | ------------------------- | ----------------------------- | ---------------------------- | ---------------- | ---------------------------- |
| **ASP.NET Core MVC** | Fast server response      | Server app + static assets    | High (controllers, services) | N/A (server web) | Good with proper markup      |
| **Razor Pages**      | Fast server response      | Server app + static assets    | High (page models)           | N/A (server web) | Good with proper markup      |
| **Blazor Server**    | Low client payload        | Server app + SignalR circuit  | High (components + services) | Limited          | Good with component patterns |
| **Blazor WASM**      | Higher initial download   | Static site + API backend     | Medium (UI + integration)    | Possible         | Good with component patterns |
| **.NET MAUI**        | Native startup varies     | App store / MSIX / installers | Medium (UI + VM tests)       | Strong           | Depends on platform tooling  |
| **WPF**              | Strong on Windows         | MSI/MSIX/ClickOnce            | High (VM + UI automation)    | Strong           | Good with WPF automation     |
| **WinForms**         | Very fast on Windows      | MSI/MSIX/ClickOnce            | Medium (UI automation heavy) | Strong           | Depends on control usage     |
| **Web Forms**        | Server-bound + view state | IIS-hosted .NET Framework     | Low (legacy patterns)        | N/A (server web) | Mixed, often manual fixes    |

---

## Decision Guide

- Choose **MVC** when you need complex routing, controllers, and server-rendered SEO.
- Choose **Razor Pages** for page-focused apps with a simpler mental model.
- Choose **Blazor** when you want component-based UI in C# across web (and hybrid).
- Choose **.NET MAUI** for a single codebase targeting mobile + desktop.
- Choose **WPF** for modern Windows-only desktop UI with rich data binding.
- Choose **WinForms** for quick Windows desktop tools and legacy maintenance.
- Choose **Web Forms** only for legacy systems on .NET Framework.

---

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

## Validation Approaches by Framework

| Framework   | Primary Mechanism                                      | Typical UI Feedback                       |
| ----------- | ------------------------------------------------------ | ----------------------------------------- |
| MVC         | Data annotations + `ModelState`                        | `asp-validation-for` tag helpers          |
| Razor Pages | Data annotations + `ModelState` in handlers            | `asp-validation-for` tag helpers          |
| Blazor      | `EditForm` + `DataAnnotationsValidator`                | `ValidationSummary` + `ValidationMessage` |
| .NET MAUI   | MVVM validation + `TryParse`/custom errors             | Bound error text + visual states          |
| WPF         | `ValidationRule`/`IDataErrorInfo`                      | Error template + tooltip                  |
| WinForms    | `TryParse` + `ErrorProvider`                           | ErrorProvider icon + message              |
| Web Forms   | `RequiredFieldValidator`, `RegularExpressionValidator` | ValidationSummary + validator text        |

---

## ASP.NET Core MVC

### Strengths

- Clear separation of concerns (Model, View, Controller)
- Strong testability for controller logic
- Full control over routes, filters, and middleware

### Good vs Bad

```csharp
// BAD: Controller handles data access and formatting
public IActionResult Details(int id)
{
    var product = _db.Products.First(p => p.Id == id);
    return Content($"<h1>{product.Name}</h1><p>{product.Price}</p>");
}
```

```csharp
// GOOD: Controller delegates to a service and returns a view model
public IActionResult Details(int id)
{
    var model = _catalog.GetProductDetails(id);
    if (model is null) return NotFound();
    return View(model);
}
```

### Pitfalls

- Overloaded controllers with business logic
- Using `ViewBag` for complex data instead of view models

### Validation Examples

MVC validation should lean on model binding + data annotations with `ModelState` checks.
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/MvcUiExamples.cs](../FrontEnd/MvcUiExamples.cs).

---

## Razor Pages

### Strengths

- Page-focused design with `@page`
- Simple handler-based flow (`OnGet`, `OnPost`)
- Less ceremony than MVC

### Good vs Bad

```csharp
// BAD: PageModel performs heavy domain logic
public class EditModel : PageModel
{
    public void OnPost()
    {
        // Large transactional logic and direct SQL here...
    }
}
```

```csharp
// GOOD: PageModel stays thin and uses handlers
public class EditModel : PageModel
{
    private readonly ICustomerService _service;

    [BindProperty]
    public CustomerInput Input { get; set; } = new();

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _service.SaveAsync(Input);
        return RedirectToPage("./Index");
    }
}
```

### Pitfalls

- Massive PageModels with controller-like complexity
- Mixing HTML formatting logic in handlers

### Validation Examples

Razor Pages validation should use `[BindProperty]` + annotations and guard on `ModelState`.
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/RazorPagesExamples.cs](../FrontEnd/RazorPagesExamples.cs).

---

## Blazor

### Strengths

- Component-based UI in C#
- Multiple render modes (SSR, Server, WebAssembly)
- Reusable components across web and hybrid

### Good vs Bad

```razor
@page "/report"

<!-- BAD: Synchronous work blocks rendering -->
@code {
    protected override void OnInitialized()
    {
        Thread.Sleep(5000);
    }
}
```

```razor
@page "/report"

@if (isLoading)
{
    <p>Loading...</p>
}
else
{
    <ReportTable Items="items" />
}

@code {
    private bool isLoading = true;
    private IReadOnlyList<ReportRow> items = Array.Empty<ReportRow>();

    protected override async Task OnInitializedAsync()
    {
        items = await service.GetRowsAsync();
        isLoading = false;
    }
}
```

### Pitfalls

- Heavy JS interop or unnecessary client-side state
- Large WebAssembly payloads without trimming

### Validation Examples

Blazor validation should use `EditForm` with `DataAnnotationsValidator` and display summaries.
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/BlazorUiExamples.cs](../FrontEnd/BlazorUiExamples.cs).

---

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
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/MauiUiExamples.cs](../FrontEnd/MauiUiExamples.cs).

---

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
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/WpfUiExamples.cs](../FrontEnd/WpfUiExamples.cs).

---

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
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/WinFormsUiExamples.cs](../FrontEnd/WinFormsUiExamples.cs).

---

## ASP.NET Web Forms

### Strengths

- Event-driven model
- Rich server control library
- Familiar for legacy apps

### Good vs Bad

```aspx
<!-- BAD: View state bloat -->
<asp:GridView ID="Grid1" runat="server" EnableViewState="true">
  <!-- Complex row events and heavy view state -->
</asp:GridView>
```

```aspx
<!-- GOOD: Disable view state where possible -->
<asp:GridView ID="Grid1" runat="server" EnableViewState="false" />
```

### Pitfalls

- Large view state payloads
- Limited modern tooling compared to ASP.NET Core

### Validation Examples

Web Forms validation should use server validators instead of raw `Request` access.
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/WebFormsUiExamples.cs](../FrontEnd/WebFormsUiExamples.cs).

---

## Migration Notes

- **Web Forms -> MVC/Razor Pages**: Move server controls to tag helpers, replace view state with explicit models, and use data annotations + `ModelState`.
- **WinForms -> WPF**: Keep business logic, move UI to XAML + MVVM, and use `ValidationRule` or `IDataErrorInfo` for input.
- **WPF/WinForms -> .NET MAUI**: Reuse view models, rewrite views in MAUI XAML, and validate in the VM with user-friendly errors.
- **MVC -> Blazor**: Promote view models to component parameters and shift UI logic into components; keep server validation on APIs.

---

## Testing Stack Cheat-Sheet

| Framework   | Unit Tests                       | UI/Integration Tests                    |
| ----------- | -------------------------------- | --------------------------------------- |
| MVC         | xUnit + controller/service tests | Playwright (web) or Selenium            |
| Razor Pages | xUnit + PageModel tests          | Playwright (web) or Selenium            |
| Blazor      | bUnit for components + xUnit     | Playwright (Web) / MAUI UITest (Hybrid) |
| .NET MAUI   | xUnit + VM tests                 | Appium or MAUI UITest (if available)    |
| WPF         | xUnit + VM tests                 | FlaUI or WinAppDriver                   |
| WinForms    | xUnit + presenter tests          | FlaUI or WinAppDriver                   |
| Web Forms   | xUnit + utility tests            | Selenium (legacy web UI)                |

---

## Related Files

- [Learning/FrontEnd/MvcUiExamples.cs](../FrontEnd/MvcUiExamples.cs)
- [Learning/FrontEnd/RazorPagesExamples.cs](../FrontEnd/RazorPagesExamples.cs)
- [Learning/FrontEnd/BlazorUiExamples.cs](../FrontEnd/BlazorUiExamples.cs)
- [Learning/FrontEnd/MauiUiExamples.cs](../FrontEnd/MauiUiExamples.cs)
- [Learning/FrontEnd/WpfUiExamples.cs](../FrontEnd/WpfUiExamples.cs)
- [Learning/FrontEnd/WinFormsUiExamples.cs](../FrontEnd/WinFormsUiExamples.cs)
- [Learning/FrontEnd/WebFormsUiExamples.cs](../FrontEnd/WebFormsUiExamples.cs)
- [Learning/WebAPI/MVC/MVCBestPractices.cs](../WebAPI/MVC/MVCBestPractices.cs)

---

## See Also

- [Web API & MVC](Web-API-MVC.md)
- [Security](Security.md)
- [Performance](Performance.md)
- [Testing](Testing.md)
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14
