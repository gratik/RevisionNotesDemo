// ============================================================================
// DEADLOCKS & PREVENTION
// Reference: Revision Notes - Page 10
// ============================================================================
//
// WHAT IS A DEADLOCK?
// -------------------
// A situation where threads wait on each other indefinitely, so no progress
// can be made. In async code, blocking on tasks can also cause deadlocks.
//
// WHY IT MATTERS
// --------------
// - Requests hang and time out
// - Thread pool starvation under load
// - Hard-to-debug production incidents
//
// WHEN TO USE PREVENTION PATTERNS
// -------------------------------
// - YES: ASP.NET Core services under load
// - YES: UI apps with synchronization contexts
// - YES: Library code called by UI or ASP.NET
//
// WHEN NOT TO USE
// ---------------
// - NO: Do not block on async code with .Result or .Wait()
// - NO: Do not mix sync locks with async code without care
//
// REAL-WORLD EXAMPLE
// ------------------
// Web app call chain:
// - Controller calls service.Result (blocks)
// - Service awaits HTTP call and tries to resume on captured context
// - Context is blocked, causing deadlock
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
