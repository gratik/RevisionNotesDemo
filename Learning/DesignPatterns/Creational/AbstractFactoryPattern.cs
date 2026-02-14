// ============================================================================
// ABSTRACT FACTORY PATTERN
// Reference: Revision Notes - Design Patterns (Creational) - Page 3
// ============================================================================
// PURPOSE: "Provides an interface for creating families of related objects without specifying their concrete classes."
// EXAMPLE: UI themes (light/dark) with matching controls.
// NOTE: From Revision Notes - "Abstract Factory – DI + generics already cover it" in modern .NET
// ============================================================================

namespace RevisionNotesDemo.DesignPatterns.Creational;

// Abstract products
public interface IButton
{
    void Render();
    void Click();
}

public interface ITextBox
{
    void Render();
    void SetText(string text);
}

public interface ICheckbox
{
    void Render();
    void Check(bool isChecked);
}

// Concrete products - Light theme
public class LightButton : IButton
{
    public void Render() => Console.WriteLine("[ABSTRACT FACTORY] Rendering light theme button (white background)");
    public void Click() => Console.WriteLine("[ABSTRACT FACTORY] Light button clicked");
}

public class LightTextBox : ITextBox
{
    public void Render() => Console.WriteLine("[ABSTRACT FACTORY] Rendering light theme textbox (white background)");
    public void SetText(string text) => Console.WriteLine($"[ABSTRACT FACTORY] Light textbox text: {text}");
}

public class LightCheckbox : ICheckbox
{
    public void Render() => Console.WriteLine("[ABSTRACT FACTORY] Rendering light theme checkbox");
    public void Check(bool isChecked) => Console.WriteLine($"[ABSTRACT FACTORY] Light checkbox: {(isChecked ? "✓" : "☐")}");
}

// Concrete products - Dark theme
public class DarkButton : IButton
{
    public void Render() => Console.WriteLine("[ABSTRACT FACTORY] Rendering dark theme button (black background)");
    public void Click() => Console.WriteLine("[ABSTRACT FACTORY] Dark button clicked");
}

public class DarkTextBox : ITextBox
{
    public void Render() => Console.WriteLine("[ABSTRACT FACTORY] Rendering dark theme textbox (dark background)");
    public void SetText(string text) => Console.WriteLine($"[ABSTRACT FACTORY] Dark textbox text: {text}");
}

public class DarkCheckbox : ICheckbox
{
    public void Render() => Console.WriteLine("[ABSTRACT FACTORY] Rendering dark theme checkbox");
    public void Check(bool isChecked) => Console.WriteLine($"[ABSTRACT FACTORY] Dark checkbox: {(isChecked ? "✓" : "☐")}");
}

// Abstract factory interface
public interface IUIFactory
{
    IButton CreateButton();
    ITextBox CreateTextBox();
    ICheckbox CreateCheckbox();
    string GetThemeName();
}

// Concrete factories
public class LightThemeFactory : IUIFactory
{
    public IButton CreateButton() => new LightButton();
    public ITextBox CreateTextBox() => new LightTextBox();
    public ICheckbox CreateCheckbox() => new LightCheckbox();
    public string GetThemeName() => "Light Theme";
}

public class DarkThemeFactory : IUIFactory
{
    public IButton CreateButton() => new DarkButton();
    public ITextBox CreateTextBox() => new DarkTextBox();
    public ICheckbox CreateCheckbox() => new DarkCheckbox();
    public string GetThemeName() => "Dark Theme";
}

// Client code that uses the abstract factory
public class Application
{
    private readonly IButton _button;
    private readonly ITextBox _textBox;
    private readonly ICheckbox _checkbox;
    private readonly string _themeName;

    public Application(IUIFactory factory)
    {
        // Creating a family of related objects
        _button = factory.CreateButton();
        _textBox = factory.CreateTextBox();
        _checkbox = factory.CreateCheckbox();
        _themeName = factory.GetThemeName();
    }

    public void RenderUI()
    {
        Console.WriteLine($"\n[ABSTRACT FACTORY] Creating UI with {_themeName}");
        _button.Render();
        _textBox.Render();
        _checkbox.Render();
    }

    public void InteractWithUI()
    {
        Console.WriteLine($"\n[ABSTRACT FACTORY] Interacting with {_themeName} UI");
        _button.Click();
        _textBox.SetText("Hello World");
        _checkbox.Check(true);
    }
}

// Usage demonstration
public class AbstractFactoryDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== ABSTRACT FACTORY PATTERN DEMO ===\n");

        Console.WriteLine("--- Creating application with Light Theme ---");
        var lightFactory = new LightThemeFactory();
        var lightApp = new Application(lightFactory);
        lightApp.RenderUI();
        lightApp.InteractWithUI();

        Console.WriteLine("\n--- Creating application with Dark Theme ---");
        var darkFactory = new DarkThemeFactory();
        var darkApp = new Application(darkFactory);
        darkApp.RenderUI();
        darkApp.InteractWithUI();

        Console.WriteLine("\n💡 Benefit: Creates families of related objects (matching theme controls)");
        Console.WriteLine("💡 From Revision Notes: In modern .NET, DI + generics often cover this pattern");
    }
}
