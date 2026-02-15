// ==============================================================================
// FRONT-END UI - ASP.NET CORE MVC (SERVER-RENDERED VIEWS)
// ==============================================================================
// WHAT IS THIS?
// -------------
// MVC UI patterns with controllers, view models, and server-rendered views.
//
// WHY IT MATTERS
// --------------
// ✅ Encourages separation of concerns
// ✅ Keeps controllers testable and focused
//
// WHEN TO USE
// -----------
// ✅ Traditional server-rendered web apps
// ✅ SEO-friendly pages with minimal client logic
//
// WHEN NOT TO USE
// ---------------
// ❌ SPA-first apps or pure API backends
// ❌ Highly interactive clients better served by Blazor/SPA
//
// REAL-WORLD EXAMPLE
// ------------------
// Controller returns a view model to a Razor view.
// ==============================================================================

namespace RevisionNotesDemo.FrontEnd;

/// <summary>
/// Illustrative MVC UI snippets showing good vs bad patterns.
/// </summary>
public static class MvcUiExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("MVC UI examples are illustrative only.");
        Console.WriteLine("See Learning/docs/Front-End-DotNet-UI.md for details.");
    }

    /// <summary>
    /// BAD: Controller mixes data access and formatting logic.
    /// </summary>
    private const string BadController = @"public IActionResult Details(int id)
{
    var product = _db.Products.First(p => p.Id == id);
    return Content($""<h1>{product.Name}</h1><p>{product.Price}</p>"");
}";

    /// <summary>
    /// GOOD: Controller delegates to a service and returns a view model.
    /// </summary>
    private const string GoodController = @"public IActionResult Details(int id)
{
    var model = _catalog.GetProductDetails(id);
    if (model is null) return NotFound();
    return View(model);
}";

    /// <summary>
    /// GOOD: Strongly typed view keeps logic minimal.
    /// </summary>
    private const string GoodView = @"@model ProductDetailsViewModel

<h1>@Model.Name</h1>
<p>@Model.Price</p>
<a asp-action=""Edit"" asp-route-id=""@Model.Id"">Edit</a>";

    /// <summary>
    /// BAD: Skips model validation and trusts unvalidated input.
    /// </summary>
    private const string BadValidation = @"[HttpPost]
public IActionResult Create(ProductInput input)
{
    _catalog.Create(input); // No validation or ModelState checks
    return RedirectToAction(""Index"");
}";

    /// <summary>
    /// GOOD: Validates input with annotations and ModelState.
    /// </summary>
    private const string GoodValidation = @"public sealed class ProductInput
{
    [Required]
    public string Name { get; set; } = """";

    [Range(0.01, 10000)]
    public decimal Price { get; set; }
}

[HttpPost]
public IActionResult Create(ProductInput input)
{
    if (!ModelState.IsValid) return View(input);
    _catalog.Create(input);
    return RedirectToAction(""Index"");
}";
}
