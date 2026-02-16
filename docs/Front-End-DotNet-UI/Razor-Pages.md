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

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for Razor Pages before implementation work begins.
- Keep boundaries explicit so Razor Pages decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Razor Pages in production-facing code.
- When performance, correctness, or maintainability depends on consistent Razor Pages decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Razor Pages as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Razor Pages is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Razor Pages are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

