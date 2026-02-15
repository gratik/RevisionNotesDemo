// ============================================================================
// ASYNC/AWAIT INTERNALS
// Reference: Revision Notes - Page 10
// ============================================================================
//
// WHAT IS ASYNC/AWAIT?
// --------------------
// Syntax that lets you write asynchronous code in a readable way. The compiler
// transforms async methods into state machines that resume after awaits.
//
// WHY IT MATTERS
// --------------
// - Non-blocking I/O improves scalability
// - Clear control flow with try/catch and cancellation
// - Avoids thread starvation under load
//
// WHEN TO USE
// -----------
// - YES: I/O-bound work (HTTP, database, file, network)
// - YES: Server applications that need high concurrency
// - YES: UI apps that must stay responsive
//
// WHEN NOT TO USE
// ---------------
// - NO: CPU-bound work (use Task.Run or parallelism instead)
// - NO: Fast, synchronous operations where async adds overhead
//
// REAL-WORLD EXAMPLE
// ------------------
// Web API request:
// - Await database and HTTP calls
// - Thread returns to the pool while I/O completes
// - Service scales to more concurrent requests
// ============================================================================

namespace RevisionNotesDemo.AsyncMultithreading;

public class AsyncAwaitDemo
{
    // From Revision Notes - Page 10: Async state machine usage
    public static async Task RunDemo()
    {
        Console.WriteLine("\n=== ASYNC/AWAIT INTERNALS DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 10\n");

        Console.WriteLine($"[ASYNC] Starting on thread: {Thread.CurrentThread.ManagedThreadId}");

        int sum = await FetchSumAsync();
        Console.WriteLine($"[ASYNC] Result: {sum}");
        Console.WriteLine($"[ASYNC] Completed on thread: {Thread.CurrentThread.ManagedThreadId}");

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - async: Marks method for async execution");
        Console.WriteLine("   - await: Suspension points without blocking");
        Console.WriteLine("   - Compiler creates state machine");
    }

    static async Task<int> FetchSumAsync()
    {
        var a = Task.FromResult(20);
        var b = Task.FromResult(22);
        // Suspension points without blocking
        return await a + await b;
    }
}
