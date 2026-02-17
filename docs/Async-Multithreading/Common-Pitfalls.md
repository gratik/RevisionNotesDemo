# Common Pitfalls

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Tasks/async-await basics and thread-safety fundamentals.
- Related examples: docs/Async-Multithreading/README.md
> Subject: [Async-Multithreading](../README.md)

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


## Interview Answer Block
30-second answer:
- Common Pitfalls is about concurrency and asynchronous flow control. It matters because it determines responsiveness and resource efficiency under load.
- Use it when handling I/O workloads safely in APIs and background jobs.

2-minute answer:
- Start with the problem Common Pitfalls solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: parallelism gains vs coordination complexity.
- Close with one failure mode and mitigation: blocking async code paths and causing deadlocks or thread starvation.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Common Pitfalls but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Common Pitfalls, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Common Pitfalls and map it to one concrete implementation in this module.
- 3 minutes: compare Common Pitfalls with an alternative, then walk through one failure mode and mitigation.