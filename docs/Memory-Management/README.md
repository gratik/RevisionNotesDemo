# Memory Management, Stack vs Heap, Garbage Collection

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Value/reference type basics
- Related examples: Learning/MemoryManagement/StackVsHeap.cs, Learning/MemoryManagement/GarbageCollectionDemo.cs


> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../README.md)

## Module Metadata

- **Prerequisites**: Core C#
- **When to Study**: Before performance and high-throughput backend content.
- **Related Files**: `../Learning/MemoryManagement/*.cs`
- **Estimated Time**: 90-120 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../OOP-Principles.md)
- **Next Step**: [Async-Multithreading.md](../Async-Multithreading.md)
<!-- STUDY-NAV-END -->


## Overview

Understanding memory management in .NET is crucial for writing efficient, leak-free applications.
This guide covers stack vs heap allocation, garbage collection, disposal patterns, and value vs
reference type tradeoffs. While .NET's GC handles most memory management automatically, understanding
these concepts helps you write more efficient code and avoid common pitfalls.

---

## Stack vs Heap

### The Fundamental Difference

**Stack:**

- Fast allocation/deallocation (just move stack pointer)
- Automatic cleanup when method returns
- Fixed size per thread (~1MB)
- Stores value types and method call frames
- LIFO (Last In, First Out) structure

**Heap:**

- Slower allocation (GC managed)
- Cleanup happens during garbage collection
- Much larger (~TB available)
- Stores reference types (objects)
- Random access via references

### Allocation Patterns

```csharp
public void AllocationExample()
{
    // STACK: Value types allocated on stack
    int number = 42;              // ✅ Stack (4 bytes)
    DateTime date = DateTime.Now; // ✅ Stack (8 bytes)
    bool flag = true;             // ✅ Stack (1 byte)

    // HEAP: Reference types allocated on heap
    var user = new User();        // ✅ Heap (reference on stack)
    var list = new List<int>();   // ✅ Heap (reference on stack)
    var text = "Hello";           // ✅ Heap (string is reference type)

    // MIXED: Struct containing reference
    var point = new Point         // ✅ Stack (struct)
    {
        Name = "Origin"           // ✅ Heap (string reference)
    };
}

public struct Point  // Value type
{
    public int X;
    public int Y;
    public string Name;  // Reference to heap object
}
```

### What Goes Where?

| Type                         | Allocated On | Lifetime               | Examples                             |
| ---------------------------- | ------------ | ---------------------- | ------------------------------------ |
| **Value types** (local vars) | Stack        | Until method returns   | `int`, `bool`, `DateTime`, `struct`  |
| **Reference types**          | Heap         | Until GC collects      | `class`, `string`, `array`, `object` |
| **Value types in classes**   | Heap         | With containing object | Fields in classes                    |
| **Captured variables**       | Heap         | Closure lifetime       | Lambda captures                      |
| **Boxed value types**        | Heap         | Until GC collects      | `object x = 42;`                     |

---

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

## IDisposable and Resource Management

### Disposal Patterns

**Pattern 1: Using Statement (Most Common)**

```csharp
// ✅ GOOD: Automatic disposal
using var stream = new FileStream(\"file.txt\", FileMode.Open);
using var reader = new StreamReader(stream);
var content = await reader.ReadToEndAsync();
// Both disposed automatically at end of method

// ✅ GOOD: Traditional using block
using (var connection = new SqlConnection(connString))
{
    await connection.OpenAsync();
    // Use connection
}  // Disposed here
```

**Pattern 2: Implementing IDisposable**

````csharp
public class ResourceManager : IDisposable
{
    private readonly SqlConnection _connection;
    private bool _disposed;

    // ✅ Full dispose pattern
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);  // ✅ Prevent finalizer
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            // ✅ Dispose managed resources
            _connection?.Dispose();
        }

        // ✅ Clean up unmanaged resources
        // (e.g., IntPtr, file handles)

        _disposed = true;
    }

    // Finalizer only if you have unmanaged resources
    ~ResourceManager()
    {
        Dispose(false);
    }
}\n```\n\n**Pattern 3: Async Disposal**\n\n```csharp\npublic class AsyncResourceManager : IAsyncDisposable\n{\n    private readonly NetworkStream _stream;\n    \n    // ✅ Async disposal for I/O-bound cleanup\n    public async ValueTask DisposeAsync()\n    {\n        if (_stream != null)\n        {\n            await _stream.FlushAsync();  // ✅ Properly flush\n            await _stream.DisposeAsync();\n        }\n        \n        GC.SuppressFinalize(this);\n    }\n}\n\n// Usage\nawait using var manager = new AsyncResourceManager();\n```\n\n### Common Resources That Need Disposal\n\n| Resource | Why Dispose? | Impact if Not Disposed |\n|----------|--------------|------------------------|\n| **SqlConnection** | Return to pool | Pool exhaustion |\n| **FileStream** | Release file handle | Can't open file, handle leak |\n| **HttpClient** | ❌ Don't dispose (singleton) | N/A |\n| **MemoryStream** | Usually not critical | Minor memory delay |\n| **Timer** | Stop timer thread | Thread leak |\n| **CancellationTokenSource** | Release resources | Memory leak |\n| **Semaphore** | Release lock | Deadlock |\n\n---\n\n## Memory Leaks in .NET\n\n### Common Causes\n\n**1. Event Subscription Without Unsubscribe**\n\n```csharp\n// ❌ BAD: Event creates strong reference\npublic class Subscriber\n{\n    public Subscriber(Publisher publisher)\n    {\n        publisher.DataChanged += OnDataChanged;  // ❌ Leak!\n        // Publisher keeps Subscriber alive\n    }\n    \n    private void OnDataChanged(object sender, EventArgs e) { }\n}\n\n// ✅ GOOD: Unsubscribe in Dispose\npublic class Subscriber : IDisposable\n{\n    private readonly Publisher _publisher;\n    \n    public Subscriber(Publisher publisher)\n    {\n        _publisher = publisher;\n        _publisher.DataChanged += OnDataChanged;\n    }\n    \n    public void Dispose()\n    {\n        _publisher.DataChanged -= OnDataChanged;  // ✅ Unsubscribe\n    }\n    \n    private void OnDataChanged(object sender, EventArgs e) { }\n}\n```\n\n**2. Static Collections Never Cleared**\n\n```csharp\n// ❌ BAD: Static collection keeps references forever\npublic class UserCache\n{\n    private static readonly Dictionary<int, User> _cache = new();\n    \n    public static void Add(User user)\n    {\n        _cache[user.Id] = user;  // ❌ Never removed\n    }\n}\n\n// ✅ GOOD: Use weak references or implement eviction\npublic class UserCache\n{\n    private static readonly ConcurrentDictionary<int, WeakReference<User>> _cache = new();\n    \n    public static void Add(User user)\n    {\n        _cache[user.Id] = new WeakReference<User>(user);  // ✅ Can be GC'd\n    }\n    \n    public static User? Get(int id)\n    {\n        if (_cache.TryGetValue(id, out var weakRef) && \n            weakRef.TryGetTarget(out var user))\n        {\n            return user;\n        }\n        return null;\n    }\n}\n```\n\n**3. Timers Not Disposed**\n\n```csharp\n// ❌ BAD: Timer keeps object alive\npublic class PeriodicTask\n{\n    private Timer _timer;\n    \n    public PeriodicTask()\n    {\n        _timer = new Timer(DoWork, null, 0, 1000);  // ❌ Never disposed\n    }\n}\n\n// ✅ GOOD: Dispose timer\npublic class PeriodicTask : IDisposable\n{\n    private Timer? _timer;\n    \n    public PeriodicTask()\n    {\n        _timer = new Timer(DoWork, null, 0, 1000);\n    }\n    \n    public void Dispose()\n    {\n        _timer?.Dispose();  // ✅ Stop and dispose\n        _timer = null;\n    }\n}\n```\n\n---\n\n## Struct vs Class\n\n### Decision Matrix\n\n| Characteristic | Struct (Value Type) | Class (Reference Type) |\n|----------------|---------------------|------------------------|\n| **Allocation** | Stack (usually) | Heap |\n| **Copy behavior** | Deep copy (all values) | Shallow copy (reference) |\n| **Inheritance** | ❌ No | ✅ Yes |\n| **Null** | ❌ No (unless nullable) | ✅ Yes |\n| **Performance** | Better for small types | Better for large types |\n| **GC pressure** | None (stack) | Yes (heap) |\n| **Size guideline** | <16 bytes ideal | Any size |\n\n### When to Use Struct\n\n```csharp\n// ✅ GOOD: Small, immutable data\npublic readonly struct Point\n{\n    public int X { get; init; }\n    public int Y { get; init; }\n    \n    public Point(int x, int y) => (X, Y) = (x, y);\n}\n\n// ✅ GOOD: Performance-critical small types\npublic readonly struct Money\n{\n    public decimal Amount { get; init; }\n    public string Currency { get; init; }\n}\n\n// ❌ BAD: Large struct (causes copying overhead)\npublic struct LargeData  // ❌ Avoid\n{\n    public byte[] Data;  // Could be large\n    public string Name;\n    public DateTime Created;\n    // ... many fields\n}\n```\n\n### Struct Best Practices\n\n```csharp\n// ✅ Make structs immutable\npublic readonly struct Temperature\n{\n    public double Celsius { get; init; }\n    \n    public double Fahrenheit => Celsius * 9 / 5 + 32;\n}\n\n// ❌ Mutable structs lead to confusion\npublic struct MutablePoint  // ❌ Avoid\n{\n    public int X { get; set; }\n    public int Y { get; set; }\n}\n\nvar p = new MutablePoint { X = 1, Y = 2 };\nModify(p);\nConsole.WriteLine(p.X);  // Still 1! (copy was modified)\n\nvoid Modify(MutablePoint point)\n{\n    point.X = 10;  // Modifies copy, not original\n}\n```\n\n---\n\n## Best Practices\n\n### ✅ Memory Management\n- Understand when allocations happen (profiling tools)\n- Reuse objects in hot paths (ArrayPool, MemoryPool)\n- Avoid boxing value types (`object x = 42` creates heap allocation)\n- Use `Span<T>` and `Memory<T>` for zero-copy scenarios\n- Keep objects in Gen 0 when possible (short-lived)\n\n### ✅ Disposal\n- Always use `using` statements for `IDisposable` objects\n- Implement `IDisposable` for classes holding unmanaged resources\n- Use `IAsyncDisposable` for async cleanup\n- Unsubscribe from events in `Dispose()`\n- Don't call `GC.Collect()` in production code\n\n### ✅ Value vs Reference Types\n- Use `struct` for small (<16 bytes), immutable types\n- Make structs `readonly` to prevent defensive copies\n- Use `class` for anything with identity or lifecycle\n- Avoid large structs (copying overhead)\n- Don't box/unbox in loops (performance hit)\n\n---\n\n## Common Pitfalls\n\n### ❌ Forgetting to Dispose\n\n```csharp\n// ❌ BAD\nvar stream = new FileStream(\"file.txt\", FileMode.Open);\nvar reader = new StreamReader(stream);\n// Forgot to dispose - file handle leaked!\n\n// ✅ GOOD\nusing var stream = new FileStream(\"file.txt\", FileMode.Open);\nusing var reader = new StreamReader(stream);\n```\n\n### ❌ Capturing Large Objects in Closures\n\n```csharp\n// ❌ BAD: Captures entire object\npublic Func<int> CreateCounter(LargeObject obj)\n{\n    return () => obj.Counter++;  // ❌ Keeps entire obj alive\n}\n\n// ✅ GOOD: Capture only what you need\npublic Func<int> CreateCounter(LargeObject obj)\n{\n    int counter = obj.Counter;  // ✅ Capture value only\n    return () => counter++;\n}\n```\n\n### ❌ Large Object Heap Fragmentation\n\n```csharp\n// ❌ BAD: Large allocations (>85KB) go to LOH\nvar hugeArray = new byte[100_000];  // ❌ LOH, never compacted\n\n// ✅ GOOD: Use ArrayPool for large buffers\nvar buffer = ArrayPool<byte>.Shared.Rent(100_000);\ntry\n{\n    // Use buffer\n}\nfinally\n{\n    ArrayPool<byte>.Shared.Return(buffer);\n}\n```\n\n---\n\n## Related Files\n\n- [MemoryManagement/StackVsHeap.cs](../../Learning/MemoryManagement/StackVsHeap.cs) - Stack vs heap allocation examples\n- [MemoryManagement/GarbageCollectionDemo.cs](../../Learning/MemoryManagement/GarbageCollectionDemo.cs) - GC generations and behavior\n- [MemoryManagement/MemoryLeakDetection.cs](../../Learning/MemoryManagement/MemoryLeakDetection.cs) - Common leak patterns and detection\n- [MemoryManagement/StructVsClass.cs](../../Learning/MemoryManagement/StructVsClass.cs) - Value vs reference type tradeoffs\n\n---\n\n## See Also\n\n- [Performance](../Performance.md) - Span, Memory, and zero-allocation techniques\n- [Modern C#](../Modern-CSharp.md) - Records and init-only properties\n- [Async/Await](../Async-Multithreading.md) - Async disposal patterns\n- [Project Summary](../../PROJECT_SUMMARY.md)\n\n---\n\nGenerated: 2026-02-14
````

<!-- STUDY-NEXT-START -->
## Next Step

- Continue with [Async-Multithreading.md](../Async-Multithreading.md).
<!-- STUDY-NEXT-END -->

---

## Interview Answer Block

- 30-second answer: This topic covers Memory Management and focuses on clear decisions, practical tradeoffs, and production-safe defaults.
- 2-minute deep dive: Start with the core problem, explain the implementation boundary, show one failure mode, and describe the mitigation or optimization strategy.
- Common follow-up: How would you apply this in a real system with constraints?
- Strong response: State assumptions, compare at least two approaches, and justify the chosen option with reliability, maintainability, and performance impact.
- Tradeoff callout: Over-engineering this area too early can increase complexity without measurable delivery or runtime benefit.

## Interview Bad vs Strong Answer

- Bad answer: "I know Memory Management and I would just follow best practices."
- Strong answer: "For Memory Management, I first define the constraints, compare two viable approaches, justify the choice with concrete tradeoffs, and describe how I would validate outcomes in production."
- Why strong wins: It demonstrates structured reasoning, context awareness, and measurable execution rather than generic statements.

## Interview Timed Drill

- Time box: 10 minutes.
- Prompt: Explain how you would apply Memory Management in a real project with one concrete constraint (scale, security, latency, or team size).
- Required outputs:
  - One design or implementation decision
  - One risk and mitigation
  - One measurable validation signal
- Self-check score (0-3 each): correctness, tradeoff clarity, communication clarity.

## Topic Files

- [Stack vs Heap](Stack-vs-Heap.md)
- [Garbage Collection (GC)](Garbage-Collection-GC.md)
- [IDisposable and Resource Management](IDisposable-and-Resource-Management.md)



