// ==============================================================================
// FRONT-END UI - .NET MAUI (CROSS-PLATFORM)
// ==============================================================================
// PURPOSE: Show MAUI patterns with good/bad illustrative examples.
// NOTE: Examples are illustrative only and not meant to be run as-is.
// ==============================================================================

namespace RevisionNotesDemo.FrontEnd;

/// <summary>
/// Illustrative .NET MAUI UI snippets showing good vs bad patterns.
/// </summary>
public static class MauiUiExamples
{
    public static void RunDemo()
    {
        Console.WriteLine("MAUI UI examples are illustrative only.");
        Console.WriteLine("See Learning/docs/Front-End-DotNet-UI.md for details.");
    }

    /// <summary>
    /// BAD: Code-behind does heavy business logic and direct HTTP calls.
    /// </summary>
    private const string BadCodeBehind = @"public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        var json = new HttpClient().GetStringAsync(""https://api"").Result;
        TitleLabel.Text = json;
    }
}";

    /// <summary>
    /// GOOD: MVVM-friendly binding with async commands.
    /// </summary>
    private const string GoodXaml = @"<ContentPage>
    <VerticalStackLayout>
        <Label Text=""{Binding Title}"" />
        <Button Text=""Refresh"" Command=""{Binding RefreshCommand}"" />
    </VerticalStackLayout>
</ContentPage>";

    /// <summary>
    /// BAD: Parses input in the view without validation or feedback.
    /// </summary>
    private const string BadValidation = @"private void Save_Clicked(object sender, EventArgs e)
{
    var quantity = int.Parse(QuantityEntry.Text); // Crashes on invalid input
    _service.Save(quantity);
}";

    /// <summary>
    /// GOOD: Validates in the view model and returns friendly errors.
    /// </summary>
    private const string GoodValidation = @"public async Task SaveAsync()
{
    if (!int.TryParse(Quantity, out var quantity) || quantity <= 0)
    {
        ErrorMessage = ""Enter a positive quantity."";
        return;
    }

    await _service.SaveAsync(quantity);
    ErrorMessage = string.Empty;
}";
}
