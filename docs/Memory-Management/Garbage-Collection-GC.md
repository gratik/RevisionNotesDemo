# Garbage Collection (GC)

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

