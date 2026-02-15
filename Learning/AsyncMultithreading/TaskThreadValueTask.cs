// ============================================================================
// TASK VS THREAD VS VALUETASK
// Reference: Revision Notes - Multithreading & Async - Page 10
// ============================================================================
//
// WHAT ARE THEY?
// --------------
// Thread: OS-level execution unit, heavy and expensive to create.
// Task: TPL abstraction over threads, integrates with async/await.
// ValueTask: Lightweight struct for cases where results are often synchronous.
//
// WHY IT MATTERS
// --------------
// - Threads are costly; tasks are preferred for async workflows
// - ValueTask can reduce allocations in hot paths
// - Choosing the right tool improves scalability
//
// WHEN TO USE
// -----------
// - YES: Task for most async operations
// - YES: Thread for long-running, dedicated work
// - YES: ValueTask when results are frequently cached/synchronous
//
// WHEN NOT TO USE
// ---------------
// - NO: Avoid Thread for I/O-bound work
// - NO: Avoid ValueTask unless performance data justifies it
//
// REAL-WORLD EXAMPLE
// ------------------
// Caching service:
// - Cache hit returns ValueTask synchronously
// - Cache miss awaits Task from data store
// ============================================================================

namespace RevisionNotesDemo.AsyncMultithreading;

public class TaskThreadValueTaskDemo
{
    public static async Task RunDemo()
    {
        Console.WriteLine("\n=== TASK VS THREAD VS VALUETASK DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 10\n");

        // THREAD - OS-level
        Console.WriteLine("--- Thread (OS-level) ---");
        var t = new Thread(() =>
        {
            Console.WriteLine($"[THREAD] Running on thread ID: {Thread.CurrentThread.ManagedThreadId}");
        });
        t.Start();
        t.Join();

        // TASK - TPL abstraction
        Console.WriteLine("\n--- Task (TPL) ---");
        await Task.Run(() =>
        {
            Console.WriteLine($"[TASK] Running on thread ID: {Thread.CurrentThread.ManagedThreadId}");
        });

        // VALUETASK - lightweight
        Console.WriteLine("\n--- ValueTask (Performance-critical) ---");
        int n = await GetNumberAsync(fast: true);
        Console.WriteLine($"[VALUETASK] Result: {n}");

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - Thread: Heavy, direct OS control");
        Console.WriteLine("   - Task: Preferred, supports async/await");
        Console.WriteLine("   - ValueTask: When result often synchronous");
    }

    // From Revision Notes - Page 10
    static ValueTask<int> GetNumberAsync(bool fast)
        => fast ? new ValueTask<int>(42) : new ValueTask<int>(Task.Run(() => 42));
}
