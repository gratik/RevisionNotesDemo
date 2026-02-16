# Razor Pages

> Subject: [Front-End-DotNet-UI](../README.md)

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
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/RazorPagesExamples.cs](../../Learning/FrontEnd/RazorPagesExamples.cs).

---



