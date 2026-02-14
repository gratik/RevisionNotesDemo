// ==============================================================================
// FRONT-END UI - WINDOWS FORMS (WINDOWS DESKTOP)
// ==============================================================================
// PURPOSE: Show WinForms patterns with good/bad illustrative examples.
// NOTE: Examples are illustrative only and not meant to be run as-is.
// ==============================================================================

namespace RevisionNotesDemo.FrontEnd;

/// <summary>
/// Illustrative WinForms UI snippets showing good vs bad patterns.
/// </summary>
public static class WinFormsUiExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("WinForms UI examples are illustrative only.");
        Console.WriteLine("See Learning/docs/Front-End-DotNet-UI.md for details.");
    }

    /// <summary>
    /// BAD: Single mega-form with unrelated responsibilities.
    /// </summary>
    private const string BadForm = @"public class MainForm : Form
{
    // 2,000 lines of UI + data access + business rules
}";

    /// <summary>
    /// GOOD: Compose UI with user controls and presenters.
    /// </summary>
    private const string GoodForm = @"public class OrdersForm : Form
{
    public OrdersForm(IOrdersPresenter presenter)
    {
        InitializeComponent();
        presenter.Bind(this);
    }
}";

    /// <summary>
    /// BAD: Parses input directly and crashes on invalid values.
    /// </summary>
    private const string BadValidation = @"private void Save_Click(object sender, EventArgs e)
{
    var count = int.Parse(countTextBox.Text);
    _service.Save(count);
}";

    /// <summary>
    /// GOOD: Uses TryParse and ErrorProvider for feedback.
    /// </summary>
    private const string GoodValidation = @"private void Save_Click(object sender, EventArgs e)
{
    if (!int.TryParse(countTextBox.Text, out var count) || count <= 0)
    {
        errorProvider.SetError(countTextBox, ""Enter a positive number."");
        return;
    }

    errorProvider.SetError(countTextBox, string.Empty);
    _service.Save(count);
}";
}
