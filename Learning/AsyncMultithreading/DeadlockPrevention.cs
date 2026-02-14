// ============================================================================
// DEADLOCKS & PREVENTION
// Reference: Revision Notes - Page 10
// ============================================================================
// Deadlock: Threads waiting for each other's resources
// Prevention: ConfigureAwait(false), avoid blocking (.Result, .Wait()), async all the way
// ============================================================================

namespace RevisionNotesDemo.AsyncMultithreading;

public class DeadlockPreventionDemo
{
    public static async Task RunDemo()
    {
        Console.WriteLine("\n=== DEADLOCK PREVENTION DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 10\n");

        // ✅ CORRECT: async all the way
        Console.WriteLine("--- Correct Pattern: Async All the Way ---");
        string ok = await SomeAsync();
        Console.WriteLine($"[DEADLOCK] Result: {ok}");

        // ConfigureAwait example
        Console.WriteLine("\n--- Library Code: ConfigureAwait(false) ---");
        await LibraryMethodAsync();

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - DON'T use .Result or .Wait() (causes deadlocks)");
        Console.WriteLine("   - DO use async/await all the way");
        Console.WriteLine("   - Library code: use ConfigureAwait(false)");
    }

    static async Task<string> SomeAsync()
    {
        await Task.Delay(10);
        return "done";
    }

    // From Revision Notes - Page 10
    static async Task LibraryMethodAsync()
    {
        // Library code tip: ConfigureAwait(false) to avoid capturing context
        await Task.Delay(10).ConfigureAwait(false);
        Console.WriteLine("[DEADLOCK] Library method completed without context capture");
    }
}
