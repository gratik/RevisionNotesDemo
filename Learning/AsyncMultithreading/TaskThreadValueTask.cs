// ============================================================================
// TASK VS THREAD VS VALUETASK
// Reference: Revision Notes - Multithreading & Async - Page 10
// ============================================================================
// Thread: OS-level thread, heavy resource usage
// Task: Abstraction over threads, supports async/await, part of TPL
// ValueTask: Lightweight alternative for performance-critical scenarios
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
