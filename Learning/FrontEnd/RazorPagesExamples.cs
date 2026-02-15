// ==============================================================================
// FRONT-END UI - ASP.NET CORE RAZOR PAGES (PAGE-FOCUSED MODEL)
// ==============================================================================
// WHAT IS THIS?
// -------------
// Page-focused UI patterns using Razor Pages and PageModel classes.
//
// WHY IT MATTERS
// --------------
// ✅ Reduces ceremony for page-centric apps
// ✅ Keeps handlers close to page intent
//
// WHEN TO USE
// -----------
// ✅ Simple page workflows and CRUD screens
// ✅ Sites with minimal routing complexity
//
// WHEN NOT TO USE
// ---------------
// ❌ Large SPAs or highly interactive client apps
// ❌ Complex API-first architectures
//
// REAL-WORLD EXAMPLE
// ------------------
// PageModel delegates to a service with ModelState checks.
// ==============================================================================

namespace RevisionNotesDemo.FrontEnd;

/// <summary>
/// Illustrative Razor Pages snippets showing good vs bad patterns.
/// </summary>
public static class RazorPagesExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("Razor Pages examples are illustrative only.");
        Console.WriteLine("See Learning/docs/Front-End-DotNet-UI.md for details.");
    }

    /// <summary>
    /// BAD: PageModel does heavy domain logic directly.
    /// </summary>
    private const string BadPageModel = @"public class EditModel : PageModel
{
    public void OnPost()
    {
        // Long transactional logic and direct SQL calls here...
    }
}";

    /// <summary>
    /// GOOD: PageModel delegates and uses handler naming conventions.
    /// </summary>
    private const string GoodPageModel = @"public class EditModel : PageModel
{
    private readonly ICustomerService _service;

    [BindProperty]
    public CustomerInput Input { get; set; } = new();

    public async Task<IActionResult> OnPostSaveAsync()
    {
        if (!ModelState.IsValid) return Page();
        await _service.SaveAsync(Input);
        return RedirectToPage(""./Index"");
    }
}";

    /// <summary>
    /// GOOD: Page markup uses @page and minimal logic.
    /// </summary>
    private const string GoodPage = @"@page
@model EditModel

<form method=""post"">
    <input asp-for=""Input.Name"" />
    <button type=""submit"" asp-page-handler=""Save"">Save</button>
</form>";

    /// <summary>
    /// BAD: Skips ModelState checks and trusts raw form data.
    /// </summary>
    private const string BadValidation = @"public IActionResult OnPost()
{
    var email = Request.Form[""Email""];
    _service.Subscribe(email); // No validation
    return RedirectToPage(""./Thanks"");
}";

    /// <summary>
    /// GOOD: Uses binding, validation attributes, and ModelState.
    /// </summary>
    private const string GoodValidation = @"public sealed class SubscribeInput
{
    [Required, EmailAddress]
    public string Email { get; set; } = """";
}

[BindProperty]
public SubscribeInput Input { get; set; } = new();

public IActionResult OnPost()
{
    if (!ModelState.IsValid) return Page();
    _service.Subscribe(Input.Email);
    return RedirectToPage(""./Thanks"");
}";
}
