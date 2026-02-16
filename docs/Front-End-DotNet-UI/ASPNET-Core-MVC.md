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



