// ==============================================================================
// FRONT-END UI - WINDOWS FORMS (WINDOWS DESKTOP)
// ==============================================================================
// WHAT IS THIS?
// -------------
// WinForms UI patterns for classic desktop apps.
//
// WHY IT MATTERS
// --------------
// ✅ Encourages separation of UI and business logic
// ✅ Helps maintain legacy Windows apps
//
// WHEN TO USE
// -----------
// ✅ Legacy desktop apps or simple Windows tools
// ✅ Apps that already depend on WinForms ecosystem
//
// WHEN NOT TO USE
// ---------------
// ❌ Modern cross-platform or web-first solutions
// ❌ New development where WPF/MAUI is preferred
//
// REAL-WORLD EXAMPLE
// ------------------
// Presenter pattern with user controls.
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
        Console.WriteLine("See docs/Front-End-DotNet-UI.md for details.");
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
