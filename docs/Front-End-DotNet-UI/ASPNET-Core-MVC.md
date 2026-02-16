# ASP.NET Core MVC

> Subject: [Front-End-DotNet-UI](../README.md)

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
See `BadValidation` and `GoodValidation` in [Learning/FrontEnd/MvcUiExamples.cs](../../Learning/FrontEnd/MvcUiExamples.cs).

---

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for ASP.NET Core MVC before implementation work begins.
- Keep boundaries explicit so ASP.NET Core MVC decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring ASP.NET Core MVC in production-facing code.
- When performance, correctness, or maintainability depends on consistent ASP.NET Core MVC decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying ASP.NET Core MVC as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where ASP.NET Core MVC is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for ASP.NET Core MVC are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

