# Span<T> and Memory<T>

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Profiling basics, memory allocation awareness, and async flow fundamentals.
- Related examples: docs/Performance/README.md
> Subject: [Performance](../README.md)

## Span<T> and Memory<T>

### What Are They?

**Span<T>**: Stack-only view into contiguous memory (zero heap allocations)
**Memory<T>**: Heap-friendly view (async-compatible, slightly slower)

### Comparison

| Feature | Array | Span<T> | Memory<T> |
|---------|-------|---------|----------|
| **Allocation** | Heap | Stack (or view) | Heap |
| **Slicing** | Creates new array | Zero-copy view | Zero-copy view |
| **Async** | ✅ Yes | ❌ No (ref struct) | ✅ Yes |
| **Store in field** | ✅ Yes | ❌ No | ✅ Yes |
| **Performance** | Baseline | 10x faster | 5x faster |
| **Use Case** | General | Hot paths, parsing | Async hot paths |

### Span<T> Examples

```csharp
// ❌ BAD: String.Substring() allocates new strings
public (int hours, int minutes) ParseTime_Old(string time)  // "14:30"
{
    var parts = time.Split(':');  // ❌ Allocates array
    return (int.Parse(parts[0]), int.Parse(parts[1]));
}

// ✅ GOOD: Span<char> - zero allocations
public (int hours, int minutes) ParseTime_Span(string time)
{
    ReadOnlySpan<char> span = time;  // ✅ View into string
    
    int colonIndex = span.IndexOf(':');
    var hoursSpan = span[..colonIndex];        // ✅ Slice (no allocation)
    var minutesSpan = span[(colonIndex + 1)..]; // ✅ Slice (no allocation)
    
    return (int.Parse(hoursSpan), int.Parse(minutesSpan));
}

// ❌ BAD: Array slicing with LINQ allocates
public int SumFirstHalf(int[] data)
{
    return data.Take(data.Length / 2).Sum();  // ❌ Allocates new array
}

// ✅ GOOD: Span slicing - zero allocations
public int SumFirstHalf(int[] data)
{
    Span<int> span = data;
    Span<int> firstHalf = span[..(span.Length / 2)];  // ✅ View, no copy
    
    int sum = 0;
    foreach (var item in firstHalf)
        sum += item;
    return sum;
}
```

### stackalloc - Ultra-Fast Temporary Buffers

```csharp
// ✅ Small buffers on stack (instant, no GC)
public void ProcessSmallBuffer()
{
    Span<byte> buffer = stackalloc byte[256];  // ✅ Stack allocation
    
    // Use buffer
    buffer.Fill(0);
    
}  // ✅ Automatic cleanup (no Dispose needed)

// ✅ Dynamic: stack for small, ArrayPool for large
public void ProcessBuffer(int size)
{
    const int MaxStackSize = 256;
    
    Span<byte> buffer = size <= MaxStackSize
        ? stackalloc byte[size]  // ✅ Stack
        : ArrayPool<byte>.Shared.Rent(size);  // ✅ Pool
    
    try
    {
        // Use buffer
    }
    finally
    {
        if (size > MaxStackSize)
            ArrayPool<byte>.Shared.Return(buffer.ToArray());
    }
}
```

**stackalloc Guidelines:**
- ✅ Use for buffers < 1KB
- ❌ Don't use for large buffers (stack overflow)
- ✅ Perfect for parsing, formatting, temp calculations
- ❌ Can't escape method scope

---


## Interview Answer Block
30-second answer:
- Span<T> and Memory<T> is about throughput and latency optimization in .NET workloads. It matters because performance bottlenecks directly impact user experience and cost.
- Use it when profiling and tuning high-traffic endpoints or background workers.

2-minute answer:
- Start with the problem Span<T> and Memory<T> solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: raw speed improvements vs code clarity and maintenance cost.
- Close with one failure mode and mitigation: optimizing without measuring baseline and regression impact.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Span<T> and Memory<T> but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Span<T> and Memory<T>, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Span<T> and Memory<T> and map it to one concrete implementation in this module.
- 3 minutes: compare Span<T> and Memory<T> with an alternative, then walk through one failure mode and mitigation.