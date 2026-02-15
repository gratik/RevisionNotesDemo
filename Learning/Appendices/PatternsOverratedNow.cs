// ============================================================================
// PATTERNS THAT ARE OVERUSED TODAY
// Reference: Revision Notes - Appendix A
// ============================================================================
// WHAT IS THIS?
// -------------
// A short list of patterns that are often overapplied.
//
// WHY IT MATTERS
// --------------
// ✅ Encourages simpler, more maintainable designs
// ✅ Reduces unnecessary abstraction layers
//
// WHEN TO USE
// -----------
// ✅ Design reviews and architectural discussions
// ✅ Evaluating whether a pattern adds value
//
// WHEN NOT TO USE
// ---------------
// ❌ As a hard rule; context still matters
// ❌ When constraints require the pattern
//
// REAL-WORLD EXAMPLE
// ------------------
// Prefer DI over Service Locator.
// ============================================================================

namespace RevisionNotesDemo.Appendices;

public static class PatternsOverratedNowDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== PATTERNS THAT ARE OVERUSED TODAY ===\n");

        PrintPattern(
            name: "Singleton",
            whyOverused: "Hidden global state, hard to test, lifetime coupling",
            betterWhen: "Use DI with AddSingleton and pass dependencies explicitly",
            keepWhen: "Truly global infrastructure with stable lifecycle");

        PrintPattern(
            name: "Abstract Factory",
            whyOverused: "Too many layers for simple object creation",
            betterWhen: "Use DI, configuration, or simple factory methods",
            keepWhen: "Multiple product families must stay consistent");

        PrintPattern(
            name: "Service Locator",
            whyOverused: "Hidden dependencies, runtime failures",
            betterWhen: "Constructor injection with explicit dependencies",
            keepWhen: "Legacy frameworks where DI is not possible");

        PrintPattern(
            name: "Repository (for every entity)",
            whyOverused: "Extra abstraction around EF Core DbSet already acts as repository",
            betterWhen: "Use DbContext directly with query-focused services",
            keepWhen: "Complex domain rules or multiple data sources");

        Console.WriteLine("\nGuiding rule: apply patterns to remove pain, not to add structure.");
    }

    private static void PrintPattern(string name, string whyOverused, string betterWhen, string keepWhen)
    {
        Console.WriteLine($"- {name}");
        Console.WriteLine($"  Overused because: {whyOverused}");
        Console.WriteLine($"  Prefer instead: {betterWhen}");
        Console.WriteLine($"  Still valid when: {keepWhen}\n");
    }
}