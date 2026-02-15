// ============================================================================
// QUICK REFERENCE TABLES
// Reference: Revision Notes - Appendix B
// ============================================================================
// WHAT IS THIS?
// -------------
// Compact tables for quick decisions and best practices.
//
// WHY IT MATTERS
// --------------
// ✅ Speeds up recall during design and reviews
// ✅ Provides consistent defaults for teams
//
// WHEN TO USE
// -----------
// ✅ Fast lookup for common tradeoffs
// ✅ Checklists during reviews
//
// WHEN NOT TO USE
// ---------------
// ❌ When deeper context and constraints are required
// ❌ As a replacement for performance testing
//
// REAL-WORLD EXAMPLE
// ------------------
// Choose async vs blocking I/O in a service.
// ============================================================================

namespace RevisionNotesDemo.Appendices;

public static class QuickReferenceTablesDemo
{
    private static readonly (string Topic, string Do, string Avoid)[] Entries =
    {
        ("Async I/O", "Use async/await end-to-end", "Blocking with .Result or .Wait()"),
        ("CPU work", "Use Task.Run for expensive CPU-bound work", "Async without offloading"),
        ("Logging", "Use structured templates", "String interpolation in hot paths"),
        ("EF Core", "Use AsNoTracking for reads", "Tracking every query by default"),
        ("Caching", "Set expirations and size limits", "Unbounded cache growth"),
        ("DI lifetimes", "Scoped for request services", "Singleton for per-request state")
    };

    public static void RunDemo()
    {
        Console.WriteLine("\n=== QUICK REFERENCE TABLES ===\n");

        foreach (var entry in Entries)
        {
            Console.WriteLine($"- {entry.Topic}");
            Console.WriteLine($"  Do:    {entry.Do}");
            Console.WriteLine($"  Avoid: {entry.Avoid}\n");
        }

        Console.WriteLine("Tip: Treat these as defaults, then adapt to your context.");
    }
}