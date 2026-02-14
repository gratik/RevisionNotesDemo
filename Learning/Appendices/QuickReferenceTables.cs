// ============================================================================
// QUICK REFERENCE TABLES
// Reference: Revision Notes - Appendix B
// ============================================================================
// PURPOSE:
//   Provide compact lookup tables for common decisions and best practices.
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