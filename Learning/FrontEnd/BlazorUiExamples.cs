// ==============================================================================
// FRONT-END UI - BLAZOR (COMPONENT-BASED UI)
// ==============================================================================
// WHAT IS THIS?
// -------------
// Component-based UI patterns for Blazor.
//
// WHY IT MATTERS
// --------------
// ✅ Enables reuse and reactive UI updates
// ✅ Shares .NET code across client and server
//
// WHEN TO USE
// -----------
// ✅ Interactive web UIs with .NET components
// ✅ Apps needing strong component reuse
//
// WHEN NOT TO USE
// ---------------
// ❌ Simple static pages with no interactivity
// ❌ Ultra-lightweight pages where SSR only is enough
//
// REAL-WORLD EXAMPLE
// ------------------
// Async data loading in `OnInitializedAsync`.
// ==============================================================================

namespace RevisionNotesDemo.FrontEnd;

/// <summary>
/// Illustrative Blazor UI snippets showing good vs bad patterns.
/// </summary>
public static class BlazorUiExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("Blazor UI examples are illustrative only.");
        Console.WriteLine("See Learning/docs/Front-End-DotNet-UI.md for details.");
    }

    /// <summary>
    /// BAD: Long-running sync work blocks rendering.
    /// </summary>
    private const string BadComponent = @"@page ""/report""

<h1>Report</h1>

@code {
    protected override void OnInitialized()
    {
        Thread.Sleep(5000); // Blocks UI render
    }
}";

    /// <summary>
    /// GOOD: Async data loading and state updates.
    /// </summary>
    private const string GoodComponent = @"@page ""/report""

<h1>Report</h1>

@if (isLoading)
{
    <p>Loading...</p>
}
else
{
    <ReportTable Items=""items"" />
}

@code {
    private bool isLoading = true;
    private IReadOnlyList<ReportRow> items = Array.Empty<ReportRow>();

    protected override async Task OnInitializedAsync()
    {
        items = await service.GetRowsAsync();
        isLoading = false;
    }
}";

    /// <summary>
    /// BAD: Bypasses validation and posts arbitrary values.
    /// </summary>
    private const string BadValidation = @"<EditForm Model=""input"" OnValidSubmit=""Save"">
    <InputText @bind-Value=""input.Email"" />
    <button type=""submit"">Save</button>
</EditForm>

@code {
    private SubscribeInput input = new();

    private Task Save()
        => service.SubscribeAsync(input.Email); // No validation layer
}";

    /// <summary>
    /// GOOD: Uses data annotations + validation components.
    /// </summary>
    private const string GoodValidation = @"<EditForm Model=""input"" OnValidSubmit=""Save"">
    <DataAnnotationsValidator />
    <ValidationSummary />
    <InputText @bind-Value=""input.Email"" />
    <button type=""submit"">Save</button>
</EditForm>

@code {
    private SubscribeInput input = new();

    public sealed class SubscribeInput
    {
        [Required, EmailAddress]
        public string Email { get; set; } = """";
    }

    private Task Save()
        => service.SubscribeAsync(input.Email);
}";
}
