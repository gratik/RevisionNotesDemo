// ============================================================================
// ASYNC/AWAIT INTERNALS
// Reference: Revision Notes - Page 10
// ============================================================================
// DEFINITION:
//   async/await enables writing asynchronous code that looks synchronous.
//   The compiler generates a state machine that manages execution flow.
//
// HOW IT WORKS:
//   1. STATE MACHINE: Compiler transforms async method into a state machine
//   2. AWAIT POINT: Execution is suspended, thread is released back to pool
//   3. CONTINUATION: When awaited task completes, method resumes from suspension point
//   4. NO BLOCKING: Thread isn't blocked during await - it's free to do other work
//
// COMPILER TRANSFORMATION:
//   The compiler rewrites your async method into a generated struct that implements
//   IAsyncStateMachine with a MoveNext() method. Each await point becomes a state.
//
// EXAMPLE:
//   public async Task<string> FetchDataAsync() {
//       var data = await httpClient.GetStringAsync(url);  // Suspension point
//       return data.ToUpper();                            // Resumes here
//   }
//
// BEHIND THE SCENES:
//   1. Method starts executing synchronously
//   2. Hits 'await' - if not complete, method returns to caller
//   3. Task returned represents the incomplete operation
//   4. When awaited operation completes, continuation is scheduled
//   5. Method resumes from await point (possibly on different thread)
//
// KEY CONCEPTS:
//   • SUSPENSION POINT: 'await' keyword - method pauses, thread released
//   • CONTINUATION: Code after await - scheduler decides which thread runs it
//   • SYNCHRONIZATION CONTEXT: Captures context (e.g., UI thread) for resumption
//   • CONFIGUREAWAIT(FALSE): Don't capture context (better performance for libraries)
//
// async KEYWORD:
//   • Marks method for async transformation by compiler
//   • Allows use of 'await' keyword
//   • Method must return Task, Task<T>, or void (only for event handlers)
//
// await KEYWORD:
//   • Asynchronously waits for operation to complete
//   • Doesn't block thread (thread returned to pool)
//   • Unwraps Task<T> to T
//   • Propagates exceptions
//
// BENEFITS:
//   • No thread blocking = better scalability
//   • Readable code (looks synchronous)
//   • Exception handling with try/catch
//   • Cancellation support via CancellationToken
//   • Better resource utilization
//
// COMMON MISTAKES:
//   ❌ .Result or .Wait() = blocks thread, can cause deadlock
//   ❌ async void (except event handlers) = can't catch exceptions
//   ❌ Not awaiting = fire-and-forget, exceptions lost
//   ✅ Always async Task, use await, handle exceptions properly
//
// BEST PRACTICES:
//   • async all the way (don't mix sync and async)
//   • Use ConfigureAwait(false) in library code
//   • Never use async void (except event handlers)
//   • Always await async methods
//   • Use CancellationToken for long-running operations
//   • Avoid async over sync (if operation is fast, keep it sync)
//
// REAL-WORLD ANALOGY:
//   Like ordering at a restaurant - you place order (start async operation),
//   waiter doesn't stand and wait (thread released), does other work,
//   returns when food is ready (continuation).
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
