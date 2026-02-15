// ============================================================================
// EXTENSION METHODS
// Reference: Revision Notes - Page 7
// ============================================================================
// WHAT IS THIS?
// -------------
// Static methods that extend existing types via the `this` parameter.
//
// WHY IT MATTERS
// --------------
// ✅ Adds helpers without modifying or inheriting types
// ✅ Improves call-site readability
//
// WHEN TO USE
// -----------
// ✅ Utilities for shared primitives or third-party types
// ✅ Fluent helpers for collections and strings
//
// WHEN NOT TO USE
// ---------------
// ❌ When behavior is surprising or expensive
// ❌ When it conflicts with instance methods or causes ambiguity
//
// REAL-WORLD EXAMPLE
// ------------------
// `string.ToTitleCase()` helper.
// ============================================================================

namespace RevisionNotesDemo.CoreCSharpFeatures;

// From Revision Notes - Page 7: Extension methods for string
public static class StringExtensions
{
    public static bool IsNullOrWhiteSpace(this string? value)
        => string.IsNullOrWhiteSpace(value);

    public static string ToTitleCase(this string value)
        => System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(value.ToLower(System.Globalization.CultureInfo.CurrentCulture));

    public static string Reverse(this string value)
    {
        char[] chars = value.ToCharArray();
        Array.Reverse(chars);
        return new string(chars);
    }
}

public class ExtensionMethodsDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== EXTENSION METHODS DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 7\n");

        string? name = "john doe";

        Console.WriteLine($"[EXT] Original: '{name}'");
        Console.WriteLine($"[EXT] IsNullOrWhiteSpace: {name.IsNullOrWhiteSpace()}");
        Console.WriteLine($"[EXT] ToTitleCase: '{name.ToTitleCase()}'");
        Console.WriteLine($"[EXT] Reverse: '{name.Reverse()}'");

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - Static methods with 'this' first parameter");
        Console.WriteLine("   - Extend types without modifying them");
        Console.WriteLine("   - Instance-like syntax but compiled as static calls");
    }
}
