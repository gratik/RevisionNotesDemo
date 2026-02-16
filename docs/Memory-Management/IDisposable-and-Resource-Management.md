# IDisposable and Resource Management

> Subject: [Memory-Management](../README.md)

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



