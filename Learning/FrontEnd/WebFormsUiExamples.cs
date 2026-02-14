// ==============================================================================
// FRONT-END UI - ASP.NET WEB FORMS (.NET FRAMEWORK)
// ==============================================================================
// PURPOSE: Show Web Forms patterns with good/bad illustrative examples.
// NOTE: Examples are illustrative only and not meant to be run as-is.
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
        Console.WriteLine("See Learning/docs/Front-End-DotNet-UI.md for details.");
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
