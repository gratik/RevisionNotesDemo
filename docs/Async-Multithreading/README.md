# Async/Await and Multithreading

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Task and async/await basics
- Related examples: Learning/AsyncMultithreading/AsyncAwaitInternals.cs, Learning/AsyncMultithreading/DeadlockPrevention.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: Core C#, DotNet Concepts
- **When to Study**: Before Web API, resilience, and real-time modules.
- **Related Files**: `../Learning/AsyncMultithreading/*.cs`
- **Estimated Time**: 120-150 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../OOP-Principles.md)
- **Next Step**: [Advanced-CSharp.md](../Advanced-CSharp.md)
<!-- STUDY-NAV-END -->


## Overview

This guide covers asynchronous programming with async/await, Task Parallel Library (TPL),
cancellation, deadlock prevention, and thread-safe collections. Async programming is essential
for building responsive, scalable applications that don't waste threads waiting.

---

## I/O-Bound vs CPU-Bound Work

### The Fundamental Difference

**I/O-Bound:** Waiting for external operations (network, disk, database)

- **Use**: `async`/`await` with `Task`
- **Why**: Thread is freed while waiting, doesn't block
- **Examples**: HTTP calls, file I/O, database queries

**CPU-Bound:** Computing results (calculations, encryption, parsing)

- **Use**: `Task.Run()` or `Parallel` APIs
- **Why**: Distributes work across multiple cores
- **Examples**: Image processing, data analysis, cryptography

```csharp
// ✅ I/O-Bound: Use async/await
public async Task<string> GetDataAsync()
{
    // Thread is freed during HTTP call
    return await _httpClient.GetStringAsync("https://api.example.com/data");
}

// ✅ CPU-Bound: Use Task.Run to offload to thread pool
public async Task<int> CalculatePrimesAsync(int max)
{
    // Offload heavy computation to thread pool
    return await Task.Run(() =>
    {
        var primes = new List<int>();
        for (int i = 2; i < max; i++)
            if (IsPrime(i)) primes.Add(i);
        return primes.Count;
    });
}
```

### Decision Tree

```
Does work involve waiting (network/disk/DB)?
    → Yes: Use async/await (I/O-bound)
    → No: Is it heavy computation?
        → Yes: Use Task.Run or Parallel (CPU-bound)
        → No: Run synchronously
```

---

## Task, Thread, and ValueTask

### Comparison Table

| Feature       | Task             | Thread            | ValueTask                      |
| ------------- | ---------------- | ----------------- | ------------------------------ |
| **Purpose**   | Async operations | Direct threading  | High-perf async                |
| **Overhead**  | Moderate         | High (1MB stack)  | Minimal (struct)               |
| **Awaitable** | Yes              | No                | Yes                            |
| **Result**    | Task&lt;T&gt;    | N/A               | ValueTask&lt;T&gt;             |
| **Use Case**  | General async    | Rare, legacy code | Hot paths with sync completion |
| **Can Reuse** | No               | No                | No (await once!)               |

### When to Use Each

```csharp
// ✅ Task: Default choice for async operations
public async Task<User> GetUserAsync(int id)
{
    var user = await _db.Users.FindAsync(id);
    return user;
}

// ✅ ValueTask: When result is often synchronously available
public async ValueTask<User?> GetCachedUserAsync(int id)
{
    // Often returns immediately from cache (avoiding Task allocation)
    if (_cache.TryGetValue(id, out var cached))
        return cached;  // ✅ Synchronous completion

    // Only allocates Task if cache misses
    return await _db.Users.FindAsync(id);
}

// ❌ Thread: Avoid unless you need very specific control
public void LegacyThreadExample()
{
    var thread = new Thread(() =>
    {
        // Heavy computation
    });
    thread.Start();
    thread.Join();  // ❌ Prefer Task.Run instead
}
```

**ValueTask Rules:**

- ✅ Use for hot paths where sync completion is common (e.g., caching)
- ❌ Never await more than once
- ❌ Don't store in fields
- ❌ Never use `.Result` or `.GetAwaiter().GetResult()`

---

## Parallel Execution Patterns

### Task.WhenAll - Execute Multiple Tasks Concurrently

```csharp
// ✅ Parallel HTTP requests (all start immediately)
var urls = new[] { "url1", "url2", "url3" };
var tasks = urls.Select(url => _httpClient.GetStringAsync(url));
var results = await Task.WhenAll(tasks);
// All 3 requests happen concurrently

// ❌ Sequential execution (one at a time)
var results = new List<string>();
foreach (var url in urls)
{
    results.Add(await _httpClient.GetStringAsync(url));  // ❌ Waits for each
}
```

### Task.WhenAny - First Completed Wins

```csharp
// Timeout pattern
var dataTask = FetchDataAsync();
var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));

var completedTask = await Task.WhenAny(dataTask, timeoutTask);
if (completedTask == timeoutTask)
    throw new TimeoutException("Operation timed out");

return await dataTask;
```

### Parallel.ForEach - CPU-Bound Parallel Work

```csharp
// ✅ Process large collection in parallel (CPU-bound)
var images = GetImages();
Parallel.ForEach(images, image =>
{
    image.Resize(800, 600);
    image.ApplyFilter();
});

// Control parallelism
var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };
Parallel.ForEach(items, options, item => ProcessItem(item));
```

---

## Cancellation and Timeouts

### CancellationToken Pattern

```csharp
// ✅ Support cancellation in async methods
public async Task<List<Order>> GetOrdersAsync(CancellationToken ct)
{
    // Pass token to async operations
    var orders = await _db.Orders
        .Where(o => o.Status == "Active")
        .ToListAsync(ct);  // ✅ Operation can be cancelled

    // Check for cancellation in loops
    foreach (var order in orders)
    {
        ct.ThrowIfCancellationRequested();  // ✅ Early exit if cancelled
        await ProcessOrderAsync(order, ct);
    }

    return orders;
}

// Usage with timeout
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
try
{
    var orders = await GetOrdersAsync(cts.Token);
}
catch (OperationCanceledException)
{
    // Handle timeout or cancellation
}
```

### Cancellation Best Practices

```csharp
// ✅ GOOD: Always accept and pass CancellationToken
public async Task ProcessAsync(CancellationToken ct = default)
{
    await _httpClient.GetAsync("url", ct);  // Pass it through
}

// ❌ BAD: Ignoring CancellationToken
public async Task ProcessAsync(CancellationToken ct)
{
    await _httpClient.GetAsync("url");  // ❌ Can't be cancelled
}

// ✅ Combine multiple cancellation sources
var cts1 = new CancellationTokenSource();
var cts2 = new CancellationTokenSource(TimeSpan.FromSeconds(10));
var linked = CancellationTokenSource.CreateLinkedTokenSource(cts1.Token, cts2.Token);
```

---

## Deadlock Prevention

### The Classic ASP.NET Deadlock

```csharp
// ❌ DEADLOCK: Blocking on async code in synchronization context
public void DeadlockExample()
{
    var result = GetDataAsync().Result;  // ❌ DEADLOCK!
    // UI thread waits for task, task waits for UI thread
}

private async Task<string> GetDataAsync()
{
    await Task.Delay(100);  // Tries to resume on UI thread (blocked!)
    return "data";
}
```

### Solution 1: ConfigureAwait(false)

```csharp
// ✅ GOOD: Library code uses ConfigureAwait(false)
public async Task<string> GetDataAsync()
{
    // Don't capture synchronization context
    await Task.Delay(100).ConfigureAwait(false);
    return "data";
}

// Safe to block (but still not recommended)
var result = GetDataAsync().Result;  // ✅ Won't deadlock
```

### Solution 2: Async All The Way

```csharp
// ✅ BEST: Never block on async code
public async Task ProcessAsync()
{
    var result = await GetDataAsync();  // ✅ Properly awaited
}
```

### ConfigureAwait Guidelines

| Context          | Use ConfigureAwait(false)?            |
| ---------------- | ------------------------------------- |
| **Library code** | ✅ Yes (don't need sync context)      |
| **ASP.NET Core** | ⚠️ Optional (no sync context anyway)  |
| **WPF/WinForms** | ❌ No (need UI thread)                |
| **Console apps** | ⚠️ Optional (usually no sync context) |

---

## Thread-Safe Collections

### Comparison Table

| Collection                          | Use Case                        | Performance                   |
| ----------------------------------- | ------------------------------- | ----------------------------- |
| **ConcurrentBag&lt;T&gt;**          | Unordered producer/consumer     | Fast adds, moderate reads     |
| **ConcurrentQueue&lt;T&gt;**        | FIFO ordered                    | Fast, minimal contention      |
| **ConcurrentStack&lt;T&gt;**        | LIFO ordered                    | Fast, minimal contention      |
| **ConcurrentDictionary&lt;K,V&gt;** | Key-value lookup                | Good, uses fine-grained locks |
| **BlockingCollection&lt;T&gt;**     | Producer-consumer with blocking | Slower, but convenient        |
| **Channel&lt;T&gt;**                | Async producer-consumer         | Best for async scenarios      |

### ConcurrentDictionary - Most Common

```csharp
var cache = new ConcurrentDictionary<int, User>();

// ✅ Thread-safe add
var user = cache.GetOrAdd(userId, id => _db.GetUser(id));

// ✅ Thread-safe update
cache.AddOrUpdate(
    userId,
    addValue: new User { Id = userId },
    updateValueFactory: (id, existing) =>
    {
        existing.LastUpdated = DateTime.UtcNow;
        return existing;
    });

// ✅ Thread-safe remove
cache.TryRemove(userId, out var removed);
```

### Channel&lt;T&gt; - Modern Async Producer-Consumer

```csharp
// ✅ Create bounded channel
var channel = Channel.CreateBounded<WorkItem>(new BoundedChannelOptions(100)
{
    FullMode = BoundedChannelFullMode.Wait
});

// Producer
await channel.Writer.WriteAsync(workItem);

// Consumer
await foreach (var item in channel.Reader.ReadAllAsync())
{
    await ProcessAsync(item);
}
```

---

## Best Practices

### ✅ Async Best Practices

- Use `async`/`await` for I/O-bound work (network, disk, database)
- Use `Task.Run()` for CPU-bound work you want to offload
- Always pass `CancellationToken` through
- Never use `.Result` or `.Wait()` (blocks thread, risks deadlock)
- Avoid `async void` except for event handlers
- Use `ConfigureAwait(false)` in library code

### ✅ Thread Safety Best Practices

- Prefer immutable data structures (no sharing = no problems)
- Use concurrent collections for shared state
- Minimize shared state (use message passing instead)
- Use `async`/`await` instead of manual threading
- Never lock on `this`, `typeof(MyClass)`, or strings

### ✅ Performance Best Practices

- Use `ValueTask<T>` only when profiling shows benefit
- Avoid excessive allocations in hot async paths
- Use `Task.WhenAll` for parallel I/O operations
- Use `Parallel.ForEach` for CPU-bound parallel work
- Cache frequently-used Tasks when appropriate

---

## Common Pitfalls

### ❌ Async Void

```csharp
// ❌ BAD: Can't catch exceptions, can't await
public async void ProcessDataAsync()
{
    await _service.ProcessAsync();  // Exception caught where?
}

// ✅ GOOD: Return Task
public async Task ProcessDataAsync()
{
    await _service.ProcessAsync();
}

// ✅ EXCEPTION: Event handlers must be async void
button.Click += async (sender, e) =>
{
    await ProcessDataAsync();
};
```

### ❌ Blocking on Async Code

```csharp
// ❌ BAD: Blocks thread, risks deadlock
var result = GetDataAsync().Result;
var result = GetDataAsync().GetAwaiter().GetResult();
GetDataAsync().Wait();

// ✅ GOOD: Properly await
var result = await GetDataAsync();
```

### ❌ Not Passing CancellationToken

```csharp
// ❌ BAD: Can't cancel long-running operation
public async Task<List<Order>> GetOrdersAsync()
{
    return await _db.Orders.ToListAsync();  // Can't cancel
}

// ✅ GOOD: Support cancellation
public async Task<List<Order>> GetOrdersAsync(CancellationToken ct)
{
    return await _db.Orders.ToListAsync(ct);
}
```

### ❌ Capturing Variables in Loops

```csharp
// ❌ BAD: All tasks capture same variable
for (int i = 0; i < 10; i++)
{
    tasks.Add(Task.Run(() => Console.WriteLine(i)));  // ❌ Captures 'i'
}
// Prints "10" ten times!

// ✅ GOOD: Capture loop variable
for (int i = 0; i < 10; i++)
{
    int captured = i;  // ✅ Each task gets its own copy
    tasks.Add(Task.Run(() => Console.WriteLine(captured)));
}
```

### ❌ Fire-and-Forget

```csharp
// ❌ BAD: Unobserved exception will crash process
public void SaveLog(string message)
{
    _ = _db.SaveLogAsync(message);  // ❌ Exceptions ignored
}

// ✅ GOOD: Properly handle exceptions
public async Task SaveLogAsync(string message)
{
    try
    {
        await _db.SaveLogAsync(message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Failed to save log");
    }
}
```

---

## Related Files

- [AsyncMultithreading/TaskThreadValueTask.cs](../../Learning/AsyncMultithreading/TaskThreadValueTask.cs) - Task vs Thread vs ValueTask comparison
- [AsyncMultithreading/AsyncAwaitInternals.cs](../../Learning/AsyncMultithreading/AsyncAwaitInternals.cs) - How async state machines work
- [AsyncMultithreading/DeadlockPrevention.cs](../../Learning/AsyncMultithreading/DeadlockPrevention.cs) - ConfigureAwait and synchronization contexts
- [AsyncMultithreading/ConcurrentCollections.cs](../../Learning/AsyncMultithreading/ConcurrentCollections.cs) - Thread-safe collections and patterns

---

## See Also

- [Performance](../Performance.md) - Optimization techniques including ValueTask
- [Core C# Features](../Core-CSharp.md) - Delegates and events used in async patterns
- [Testing](../Testing.md) - Testing async code properly
- [Web API and MVC](../Web-API-MVC.md) - Async in ASP.NET Core
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Advanced-CSharp.md](../Advanced-CSharp.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: Async improves throughput for I/O-bound operations by freeing threads while work is waiting on external resources.
- 2-minute deep dive: I keep async end-to-end on I/O paths, propagate cancellation tokens, avoid sync-over-async, and instrument timeouts/retries to diagnose contention and thread-pool pressure.
- Common follow-up: When not to use async?
- Strong response: Pure CPU-bound work should use dedicated compute paths; async alone does not speed CPU execution.
- Tradeoff callout: Blindly adding `Task.Run` can hide architecture issues and hurt latency consistency.

## Interview Bad vs Strong Answer

- Bad answer: "I know Async Multithreading and I would just follow best practices."
- Strong answer: "For Async Multithreading, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Async Multithreading in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.

## Topic Files

- [I/O-Bound vs CPU-Bound Work](IO-Bound-vs-CPU-Bound-Work.md)
- [Task, Thread, and ValueTask](Task-Thread-and-ValueTask.md)
- [Parallel Execution Patterns](Parallel-Execution-Patterns.md)
- [Cancellation and Timeouts](Cancellation-and-Timeouts.md)
- [Deadlock Prevention](Deadlock-Prevention.md)
- [Thread-Safe Collections](Thread-Safe-Collections.md)
- [Best Practices](Best-Practices.md)
- [Common Pitfalls](Common-Pitfalls.md)



