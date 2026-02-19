# Performance Optimization Techniques

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Collections and memory basics
- Related examples: Learning/Performance/BenchmarkingExamples.cs, Learning/Performance/OptimizationTechniques.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: Memory Management, Async Multithreading
- **When to Study**: After correctness and stability are in place.
- **Related Files**: `../Performance/*.cs`
- **Estimated Time**: 120-150 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../Design-Patterns.md)
- **Next Step**: [Message-Architecture.md](../Message-Architecture.md)
<!-- STUDY-NAV-END -->


## Overview

Modern C# provides powerful tools for high-performance code: `Span<T>`, `Memory<T>`, `ArrayPool<T>`,
`stackalloc`, and more. This guide covers zero-allocation techniques, benchmarking, and when to optimize.
Remember: **measure first, optimize second**. Premature optimization wastes time on code that doesn't matter.

---

## The Performance Hierarchy

**Priority Order:**
1. **Algorithm** - O(n²) → O(n log n) matters most
2. **Data Structures** - Right collection (Dictionary vs List)
3. **Allocations** - Reduce GC pressure
4. **CPU Cache** - Memory access patterns
5. **Micro-optimizations** - Last resort, measure first

**Rule**: Optimize algorithms first, then allocations, then micro-optimizations.

---

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

## ArrayPool<T> - Object Pooling

### The Problem

```csharp
// ❌ BAD: Allocates array every time
public void ProcessData(int size)
{
    var buffer = new byte[size];  // ❌ Heap allocation + GC
    // Use buffer
}  // ❌ GC must collect this
```

### The Solution

```csharp
// ✅ GOOD: Rent from pool, return when done
public void ProcessData(int size)
{
    var buffer = ArrayPool<byte>.Shared.Rent(size);  // ✅ Reuses arrays
    try
    {
        Span<byte> span = buffer.AsSpan(0, size);  // ✅ Use exact size
        // Use buffer
    }
    finally
    {
        ArrayPool<byte>.Shared.Return(buffer);  // ✅ Return to pool
    }
}
```

**ArrayPool Benefits:**
- 10-100x fewer allocations
- Reduced GC pressure (fewer collections)
- Reuses memory (better CPU cache)
- Thread-safe (concurrent access)

**When to Use:**
- Large temporary buffers (> 1KB)
- Hot paths with frequent allocations
- Buffer sizes vary (pool handles different sizes)

---

## String Building Performance

### Comparison

```csharp
// ❌ WORST: String concatenation (1000x slower)
public string BuildMessage_Bad(int count)
{
    string result = "";
    for (int i = 0; i < count; i++)
        result += $"Item {i},";  // ❌ New string each iteration!
    return result;
}

// ✅ GOOD: StringBuilder (100x faster)
public string BuildMessage_Better(int count)
{
    var sb = new StringBuilder(count * 10);  // ✅ Preallocate
    for (int i = 0; i < count; i++)
        sb.Append($"Item {i},");
    return sb.ToString();
}

// ✅ BEST: Span + stackalloc (zero allocations until final string)
public string BuildMessage_Best(int count)
{
    var chars = ArrayPool<char>.Shared.Rent(count * 10);
    try
    {
        Span<char> buffer = chars;
        int position = 0;
        
        for (int i = 0; i < count; i++)
        {
            // Write directly to buffer
            if (!i.TryFormat(buffer[position..], out int written))
                break;
            position += written;
            buffer[position++] = ',';
        }
        
        return new string(buffer[..position]);
    }
    finally
    {
        ArrayPool<char>.Shared.Return(chars);
    }
}
```

---

## ValueTask<T> - Async Performance

### When to Use ValueTask

**Use `Task<T>`** (default):
- Result is always async
- Method awaited multiple times
- Task stored or passed around

**Use `ValueTask<T>`** (hot paths only):
- Result often available synchronously (cached)
- Await exactly once
- Not stored in fields

### Example

```csharp
// ✅ Task: Always async
public async Task<User> GetUserAsync(int id)
{
    return await _db.Users.FindAsync(id);  // Always hits DB
}

// ✅ ValueTask: Often sync (cached)
public async ValueTask<User?> GetCachedUserAsync(int id)
{
    // Often returns immediately (no Task allocation)
    if (_cache.TryGetValue(id, out var cached))
        return cached;  // ✅ Synchronous, no allocation
    
    // Only allocates Task on cache miss
    var user = await _db.Users.FindAsync(id);
    _cache[id] = user;
    return user;
}
```

**ValueTask Rules:**
- ❌ Never await twice
- ❌ Never store in field
- ❌ Never access `.Result`
- ✅ Use when sync completion is common

---

## Benchmarking with BenchmarkDotNet

### Why Benchmark?

**Don't guess, measure!**
- Intuition is often wrong
- Micro-optimizations can slow things down
- Compiler optimizes differently than you expect

### Example Benchmark

```csharp
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

[MemoryDiagnoser]  // ✅ Show allocations
[SimpleJob(warmupCount: 3, iterationCount: 5)]
public class StringBenchmarks
{
    [Params(10, 100, 1000)]  // ✅ Test multiple sizes
    public int Count { get; set; }
    
    [Benchmark(Baseline = true)]  // ✅ Baseline comparison
    public string Concatenation()
    {
        string result = "";
        for (int i = 0; i < Count; i++)
            result += "Item";
        return result;
    }
    
    [Benchmark]
    public string StringBuilder()
    {
        var sb = new System.Text.StringBuilder(Count * 4);
        for (int i = 0; i < Count; i++)
            sb.Append("Item");
        return sb.ToString();
    }
}

// Run: BenchmarkRunner.Run<StringBenchmarks>();
```

**Typical Results:**
```
|        Method | Count |        Mean | Allocated |
|-------------- |------ |------------:|----------:|
| Concatenation |    10 |   250.0 ns |     568 B |
|  StringBuilder |    10 |    50.0 ns |     168 B |
| Concatenation |   100 | 25000.0 ns |  50,000 B |
|  StringBuilder |   100 |   500.0 ns |   1,000 B |
```

---

## Best Practices

### ✅ Measure First
- Use BenchmarkDotNet for micro-benchmarks
- Use profilers (dotTrace, PerfView) for real apps
- Identify hot paths (where CPU time is spent)
- Don't optimize code that runs once

### ✅ Allocation Reduction
- Use `Span<T>` for slicing without copying
- Use `stackalloc` for small temporary buffers (<1KB)
- Use `ArrayPool<T>` for large temporary buffers
- Reuse objects instead of recreating
- Avoid boxing (object x = 42)

### ✅ Async Performance
- Use `ValueTask<T>` when sync completion is common
- Use `ConfigureAwait(false)` in library code
- Cache tasks that return same result
- Avoid `async void` except event handlers

### ✅ Collection Performance
- Preallocate collections (capacity parameter)
- Use `Dictionary<K,V>` for lookups (not List)
- Use `HashSet<T>` for uniqueness checks
- Clear and reuse collections in loops

---

## Common Pitfalls

### ❌ Premature Optimization

```csharp
// ❌ BAD: Optimizing code that runs once
public void InitializeApp()
{
    // This runs once at startup - don't optimize
    LoadConfiguration();
    ConnectToDatabase();
}

// ✅ GOOD: Optimize hot paths
public void ProcessRequest()  // Called 10,000x/second
{
    // This is worth optimizing
}
```

### ❌ Span Lifetime Violations

```csharp
// ❌ BAD: Storing Span in field
public class BadExample
{
    private Span<int> _data;  // ❌ Compiler error: ref struct in class
}

// ❌ BAD: Returning Span from stackalloc
public Span<int> GetData()
{
    Span<int> buffer = stackalloc int[10];
    return buffer;  // ❌ Compiler error: escaping stack reference
}
```

### ❌ Large stackalloc

```csharp
// ❌ BAD: Stack overflow risk
Span<byte> huge = stackalloc byte[100_000];  // ❌ Too large for stack

// ✅ GOOD: Use threshold
const int MaxStackSize = 256;
Span<byte> buffer = size <= MaxStackSize
    ? stackalloc byte[size]
    : ArrayPool<byte>.Shared.Rent(size);
```

### ❌ Not Returning to ArrayPool

```csharp
// ❌ BAD: Rent but forget to return
var buffer = ArrayPool<byte>.Shared.Rent(1024);
// ... use buffer ...
// ❌ Forgot to Return() - pool depleted!

// ✅ GOOD: Always use try/finally
var buffer = ArrayPool<byte>.Shared.Rent(1024);
try
{
    // Use buffer
}
finally
{
    ArrayPool<byte>.Shared.Return(buffer);  // ✅ Always returned
}
```

---

## Performance Checklist

**Before Optimizing:**
- [ ] Profiled to identify hot paths?
- [ ] Benchmarked current performance?
- [ ] Verified this code actually matters?

**Optimization Targets:**
- [ ] Use Span<T> for slicing/parsing
- [ ] Use stackalloc for small buffers
- [ ] Use ArrayPool for large temp arrays
- [ ] Preallocate collections with capacity
- [ ] Use StringBuilder for string building
- [ ] Use ValueTask for cached results
- [ ] Avoid LINQ in hot paths (allocations)

**After Optimizing:**
- [ ] Benchmarked improvement?
- [ ] Verified no correctness regressions?
- [ ] Code still readable?

---

## Related Files

- [Performance/SpanAndMemory.cs](../../Learning/Performance/SpanAndMemory.cs) - Span<T>, Memory<T>, stackalloc examples
- [Performance/BenchmarkingExamples.cs](../../Learning/Performance/BenchmarkingExamples.cs) - BenchmarkDotNet patterns
- [Performance/OptimizationTechniques.cs](../../Learning/Performance/OptimizationTechniques.cs) - ArrayPool, StringBuilder, caching

---

## See Also

- [Async and Multithreading](../Async-Multithreading.md) - ValueTask and async performance
- [Memory Management](../Memory-Management.md) - GC, allocations, struct vs class
- [Data Access](../Data-Access.md) - Database query performance
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Message-Architecture.md](../Message-Architecture.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers Performance and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Performance and I would just follow best practices."
- Strong answer: "For Performance, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Performance in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.

## Topic Files

- [The Performance Hierarchy](The-Performance-Hierarchy.md)
- [Span<T> and Memory<T>](SpanT-and-MemoryT.md)
- [ArrayPool<T> - Object Pooling](ArrayPoolT---Object-Pooling.md)
- [String Building Performance](String-Building-Performance.md)
- [ValueTask<T> - Async Performance](ValueTaskT---Async-Performance.md)
- [Benchmarking with BenchmarkDotNet](Benchmarking-with-BenchmarkDotNet.md)
- [Best Practices](Best-Practices.md)
- [Common Pitfalls](Common-Pitfalls.md)
- [Performance Checklist](Performance-Checklist.md)



