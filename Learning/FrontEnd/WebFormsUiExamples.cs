// ==============================================================================
// FRONT-END UI - ASP.NET WEB FORMS (.NET FRAMEWORK)
// ==============================================================================
// WHAT IS THIS?
// -------------
// Web Forms UI patterns and pitfalls in legacy ASP.NET.
//
// WHY IT MATTERS
// --------------
// ✅ Avoids heavy view state and tangled code-behind
// ✅ Improves maintainability of legacy apps
//
// WHEN TO USE
// -----------
// ✅ Maintaining existing Web Forms apps
// ✅ Incremental improvements without full rewrite
//
// WHEN NOT TO USE
// ---------------
// ❌ New development; prefer MVC/Razor Pages/Blazor
// ❌ Apps requiring modern client-side interactivity
//
// REAL-WORLD EXAMPLE
// ------------------
// Disable view state on large grids.
// ==============================================================================

namespace RevisionNotesDemo.FrontEnd;

/// <summary>
/// Illustrative Web Forms snippets showing good vs bad patterns.
/// </summary>
public static class WebFormsUiExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("Web Forms UI examples are illustrative only.");
        Console.WriteLine("See docs/Front-End-DotNet-UI.md for details.");
    }

    /// <summary>
    /// BAD: Heavy view state and tightly coupled code-behind.
    /// </summary>
    private const string BadAscx = @"<asp:GridView ID=""Grid1"" runat=""server"" EnableViewState=""true"">
    <!-- Complex row events and view state bloating -->
</asp:GridView>";

    /// <summary>
    /// GOOD: Keep code-behind thin and reduce view state.
    /// </summary>
    private const string GoodAscx = @"<asp:GridView ID=""Grid1"" runat=""server"" EnableViewState=""false"" />";

    /// <summary>
    /// BAD: Accepts raw input without validators.
    /// </summary>
    private const string BadValidation = @"<asp:TextBox ID=""EmailTextBox"" runat=""server"" />
<asp:Button ID=""SaveButton"" runat=""server"" OnClick=""SaveButton_Click"" />";

    /// <summary>
    /// GOOD: Uses validation controls before code-behind executes.
    /// </summary>
    private const string GoodValidation = @"<asp:TextBox ID=""EmailTextBox"" runat=""server"" />
<asp:RequiredFieldValidator ControlToValidate=""EmailTextBox"" ErrorMessage=""Email required"" runat=""server"" />
<asp:RegularExpressionValidator ControlToValidate=""EmailTextBox"" ValidationExpression=""\S+@\S+\.\S+"" ErrorMessage=""Invalid email"" runat=""server"" />
<asp:Button ID=""SaveButton"" runat=""server"" OnClick=""SaveButton_Click"" />";
}
