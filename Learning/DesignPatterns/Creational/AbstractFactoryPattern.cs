// ============================================================================
// ABSTRACT FACTORY PATTERN - Create Families of Related Objects
// Reference: Revision Notes - Design Patterns (Creational) - Page 3
// ============================================================================
//
// WHAT IS THE ABSTRACT FACTORY PATTERN?
// --------------------------------------
// Provides an interface for creating families of related or dependent objects
// without specifying their concrete classes. It's a super-factory that creates
// other factories, ensuring that created objects are compatible with each other.
//
// Think of it as: "A furniture store (factory) that sells complete room sets
// (Victorian, Modern, ArtDeco) - all pieces match the style"
//
// Core Concepts:
//   • Abstract Factory: Interface declaring methods to create abstract products
//   • Concrete Factories: Implement creation methods to produce concrete products
//   • Abstract Products: Interfaces for each distinct product type
//   • Concrete Products: Implementations of abstract products
//   • Product Families: Groups of related products (e.g., Victorian chair + table)
//
// WHY IT MATTERS
// --------------
// ✅ CONSISTENCY: Ensures products from same family work together
// ✅ SINGLE RESPONSIBILITY: Separates product creation from usage
// ✅ OPEN/CLOSED: Add new product families without modifying existing code
// ✅ LOOSE COUPLING: Code depends on interfaces, not concrete classes
// ✅ PRODUCT COMPATIBILITY: Prevents mixing incompatible products
// ✅ CENTRALIZED CREATION: All creation logic in factory classes
//
// WHEN TO USE IT
// --------------
// ✅ System needs to work with multiple families of related products
// ✅ Products are designed to work together (UI themes, cross-platform UIs)
// ✅ Need to ensure products from same family are used together
// ✅ Want to expose only interfaces, not implementations
// ✅ Product families extended frequently
// ✅ Library/framework needs to provide products without revealing details
//
// WHEN NOT TO USE IT
// ------------------
// ❌ Only single product family exists (unnecessary complexity)
// ❌ Products don't need to be compatible with each other
// ❌ Simple object creation suffices
// ❌ Modern .NET DI + generics handle it better (per Revision Notes)
// ❌ Overhead of maintaining multiple factory classes not justified
//
// REAL-WORLD EXAMPLE
// ------------------
// Imagine a cross-platform UI framework (like Flutter or React Native):
//   • App needs to look native on iOS, Android, and Windows
//   • Each platform has its own Button, TextBox, Checkbox styles
//   • Can't mix iOS Button with Android TextBox (inconsistent UX)
//   • Need consistent look per platform
//
// Without Abstract Factory:
//   → Code littered with: if (iOS) new IOSButton(); if (Android) new AndroidButton();
//   → Easy to accidentally mix iOS button with Android textbox
//   → Adding new platform requires changes throughout codebase
//   → No guarantee of visual consistency
//
// With Abstract Factory:
//   → IUIFactory factory = GetPlatformFactory(); // iOS/Android/Windows
//   → IButton btn = factory.CreateButton();
//   → ITextBox txt = factory.CreateTextBox();
//   → All components guaranteed to match platform style
//   → Adding Windows platform = new WindowsFactory class (no other changes)
//   → 100% consistency within each platform
//
// MODERN .NET ALTERNATIVE
// -----------------------
// From Revision Notes - Page 4: "Abstract Factory – DI + generics already cover it"
//
// Traditional Abstract Factory:
//   IUIFactory factory = new DarkThemeFactory();
//   IButton btn = factory.CreateButton();
//
// Modern DI Approach:
//   // Register theme
//   builder.Services.AddScoped<IButton, DarkButton>();
//   builder.Services.AddScoped<ITextBox, DarkTextBox>();
//
//   // Constructor injection
//   public class MyController
//   {
//       public MyController(IButton button, ITextBox textBox) { }
//   }
//
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
