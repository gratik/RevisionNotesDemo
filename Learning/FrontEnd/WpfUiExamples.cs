// ==============================================================================
// FRONT-END UI - WPF (WINDOWS DESKTOP)
// ==============================================================================
// WHAT IS THIS?
// -------------
// WPF UI patterns with data binding and MVVM.
//
// WHY IT MATTERS
// --------------
// ✅ Keeps UI logic out of code-behind
// ✅ Enables testable view models
//
// WHEN TO USE
// -----------
// ✅ Windows desktop apps with rich UI
// ✅ Apps needing advanced WPF capabilities
//
// WHEN NOT TO USE
// ---------------
// ❌ Cross-platform or web-first apps
// ❌ Lightweight tools where WinForms is sufficient
//
// REAL-WORLD EXAMPLE
// ------------------
// Bind TextBox to view model property.
// ==============================================================================

namespace RevisionNotesDemo.FrontEnd;

/// <summary>
/// Illustrative WPF UI snippets showing good vs bad patterns.
/// </summary>
public static class WpfUiExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("WPF UI examples are illustrative only.");
        Console.WriteLine("See docs/Front-End-DotNet-UI.md for details.");
    }

    /// <summary>
    /// BAD: Code-behind owns all state and UI updates.
    /// </summary>
    private const string BadCodeBehind = @"public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        NameTextBox.Text = _service.GetName();
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        _service.Save(NameTextBox.Text);
    }
}";

    /// <summary>
    /// GOOD: Data binding with a view model.
    /// </summary>
    private const string GoodXaml = @"<Window>
    <StackPanel>
        <TextBox Text=""{Binding Name, UpdateSourceTrigger=PropertyChanged}"" />
        <Button Content=""Save"" Command=""{Binding SaveCommand}"" />
    </StackPanel>
</Window>";

    /// <summary>
    /// BAD: Parses input directly in code-behind with no feedback.
    /// </summary>
    private const string BadValidation = @"private void Save_Click(object sender, RoutedEventArgs e)
{
    var age = int.Parse(AgeTextBox.Text);
    _service.Save(age);
}";

    /// <summary>
    /// GOOD: Uses validation rules to block invalid data entry.
    /// </summary>
    private const string GoodValidation = @"<TextBox>
    <TextBox.Text>
        <Binding Path=""Age"" UpdateSourceTrigger=""PropertyChanged"">
            <Binding.ValidationRules>
                <local:PositiveIntRule />
            </Binding.ValidationRules>
        </Binding>
    </TextBox.Text>
</TextBox>";
}
