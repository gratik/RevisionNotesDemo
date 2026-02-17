# Garbage Collection (GC)

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Value/reference type basics and runtime execution model familiarity.
- Related examples: docs/Memory-Management/README.md
> Subject: [Memory-Management](../README.md)

## Garbage Collection (GC)

### How It Works

The .NET GC uses a **generational** approach:

**Gen 0** (Youngest):

- New objects start here
- Collected most frequently (~10ms)
- Very fast collection
- Most objects die quickly (ephemeral objects)

**Gen 1** (Middle):

- Survived Gen 0 collections
- Buffer between Gen 0 and Gen 2
- Collected less frequently
- Medium speed collection

**Gen 2** (Oldest):

- Long-lived objects
- Collected rarely (~100ms+)
- Slowest, most thorough collection
- Large Object Heap (LOH) for objects >85KB

### GC Triggers

```csharp
// Automatic triggers:
// 1. Gen 0 full (allocation fails)
// 2. Explicit GC.Collect() call (rarely needed)
// 3. Low memory notification from OS
// 4. Process idle time (background GC)

// ❌ BAD: Never force GC in production code
GC.Collect();
GC.WaitForPendingFinalizers();
GC.Collect();

// ✅ GOOD: Let GC manage itself
// (Only exception: after dropping large caches)
```

### GC Modes

```csharp
// Workstation GC (default for client apps)
// - Single GC thread
// - Optimized for UI responsiveness
// - Lower throughput

// Server GC (default for ASP.NET)
// - Multiple GC threads (one per CPU)
// - Higher throughput
// - More memory usage

// Configure in .csproj
<ServerGarbageCollection>true</ServerGarbageCollection>
<ConcurrentGarbageCollection>true</ConcurrentGarbageCollection>
```

### Reducing GC Pressure

```csharp
// ❌ BAD: Excessive allocations
public string BuildMessage(int count)
{
    string result = "";
    for (int i = 0; i < count; i++)
    {
        result += $"Item {i},";  // ❌ Creates new string each iteration
    }
    return result;
}

// ✅ GOOD: Reuse memory
public string BuildMessage(int count)
{
    var sb = new StringBuilder(count * 10);  // ✅ Preallocate
    for (int i = 0; i < count; i++)
    {
        sb.Append($\"Item {i},\");
    }
    return sb.ToString();
}

// ✅ BETTER: Use ArrayPool or Span<T> for hot paths
public void ProcessData(int[] data)
{
    var buffer = ArrayPool<int>.Shared.Rent(1000);
    try
    {
        // Use buffer
    }
    finally
    {
        ArrayPool<int>.Shared.Return(buffer);
    }
}
```

---


## Interview Answer Block
30-second answer:
- Garbage Collection (GC) is about allocation, lifetime, and garbage collection behavior. It matters because memory patterns directly affect latency, throughput, and stability.
- Use it when reducing allocation pressure in hot execution paths.

2-minute answer:
- Start with the problem Garbage Collection (GC) solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: micro-optimizations vs maintainable code.
- Close with one failure mode and mitigation: premature optimization without profiling evidence.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Garbage Collection (GC) but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Garbage Collection (GC), what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Garbage Collection (GC) and map it to one concrete implementation in this module.
- 3 minutes: compare Garbage Collection (GC) with an alternative, then walk through one failure mode and mitigation.