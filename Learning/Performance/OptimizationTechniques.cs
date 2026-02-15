// ==============================================================================
// OPTIMIZATION TECHNIQUES - Practical Performance Patterns
// ==============================================================================
// WHAT IS THIS?
// -------------
// Real-world performance patterns (pooling, ValueTask, async tuning).
//
// WHY IT MATTERS
// --------------
// ✅ Lowers latency and GC pressure under load
// ✅ Improves throughput on hot paths
//
// WHEN TO USE
// -----------
// ✅ Profiled bottlenecks and high-throughput scenarios
// ✅ Large buffer or object allocation hotspots
//
// WHEN NOT TO USE
// ---------------
// ❌ Unprofiled code or low-traffic workloads
// ❌ Micro-optimizations that reduce readability
//
// REAL-WORLD EXAMPLE
// ------------------
// Use ArrayPool for large temporary buffers.
// ==============================================================================

using System.Buffers;
using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Extensions.ObjectPool;

namespace RevisionNotesDemo.Performance;

/// <summary>
/// EXAMPLE 1: ARRAYPOOL - Reuse Arrays, Reduce Allocations
/// 
/// THE PROBLEM:
/// Creating large temporary arrays allocates heap memory → GC pressure.
/// 
/// THE SOLUTION:
/// ArrayPool<T> rents/returns arrays - reuse instead of allocate.
/// 
/// WHY IT MATTERS:
/// - 95% less memory allocations
/// - No GC pauses in hot paths
/// - Essential for high-throughput scenarios
/// </summary>
public class ArrayPoolExamples
{
    // ❌ BÁD: Create new array every time
    public byte[] ProcessData_Traditional(int size)
    {
        var buffer = new byte[size];  // ❌ Heap allocation every call

        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = (byte)(i % 256);

        return buffer;  // Caller owns array
    }

    // ✅ GOOD: ArrayPool - rent and return
    public byte[] ProcessData_Pooled(int size)
    {
        var buffer = ArrayPool<byte>.Shared.Rent(size);  // ✅ Rent from pool
        try
        {
            // ⚠️ Buffer may be larger than requested!
            var span = buffer.AsSpan(0, size);  // ✅ Use only requested size

            for (int i = 0; i < span.Length; i++)
                span[i] = (byte)(i % 256);

            // Copy to exact-sized array for return
            var result = span.ToArray();
            return result;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer, clearArray: true);  // ✅ Return to pool
        }
    }

    // ✅ BEST: Process in-place without allocation
    public void ProcessData_InPlace(byte[] buffer)
    {
        // Caller provides buffer (possibly from pool)
        for (int i = 0; i < buffer.Length; i++)
            buffer[i] = (byte)(i % 256);
    }

    // ✅ GOOD: Custom pool for specific sizes
    private static readonly ArrayPool<char> CustomCharPool = ArrayPool<char>.Create(
        maxArrayLength: 4096,     // Max size to pool
        maxArraysPerBucket: 50    // Max arrays per size bucket
    );

    public string FormatData(int count)
    {
        var buffer = CustomCharPool.Rent(count * 10);  // ✅ Custom pool
        try
        {
            int position = 0;
            for (int i = 0; i < count; i++)
            {
                if (i.TryFormat(buffer.AsSpan(position), out int written, provider: CultureInfo.InvariantCulture))
                    position += written;
            }

            return new string(buffer, 0, position);
        }
        finally
        {
            CustomCharPool.Return(buffer);
        }
    }

    // GOTCHA: clearArray parameter
    // - clearArray: true => Zero out array (security/correctness)
    // - clearArray: false => Faster, but array contains old data
}

/// <summary>
/// EXAMPLE 2: OBJECT POOLING - Reuse Expensive Objects
/// 
/// THE PROBLEM:
/// Creating objects like HttpClient, DbConnection repeatedly is expensive.
/// 
/// THE SOLUTION:
/// ObjectPool reuses objects, reducing allocation and initialization cost.
/// 
/// WHY IT MATTERS:
/// - Faster than new()
/// - Reduces GC pressure
/// - Amortizes expensive initialization
/// </summary>
public class ObjectPoolingExamples
{
    // ✅ GOOD: Pool expensive objects
    public class ExpensiveObjectPool
    {
        private readonly ObjectPool<ExpensiveObject> _pool;

        public ExpensiveObjectPool()
        {
            var policy = new DefaultPooledObjectPolicy<ExpensiveObject>();
            var provider = new DefaultObjectPoolProvider();
            _pool = provider.Create(policy);
        }

        public string UsePooledObject(string input)
        {
            var obj = _pool.Get();  // ✅ Rent from pool
            try
            {
                return obj.Process(input);
            }
            finally
            {
                _pool.Return(obj);  // ✅ Return to pool
            }
        }
    }

    private class ExpensiveObject
    {
        // Imagine expensive initialization
        private readonly byte[] _buffer = new byte[4096];

        public string Process(string input)
        {
            // Do work
            _ = _buffer.Length;
            return input.ToUpper(CultureInfo.InvariantCulture);
        }
    }

    // ✅ GOOD: Custom pooling policy
    public class StringBuilderPool
    {
        private static readonly ObjectPool<StringBuilder> Pool =
            new DefaultObjectPoolProvider().Create(new StringBuilderPooledObjectPolicy());

        public static string BuildString(string[] parts)
        {
            var sb = Pool.Get();
            try
            {
                foreach (var part in parts)
                    sb.Append(part);

                return sb.ToString();
            }
            finally
            {
                Pool.Return(sb);  // Policy clears StringBuilder
            }
        }
    }

    // Custom policy: Reset object when returned
    public class StringBuilderPooledObjectPolicy : IPooledObjectPolicy<StringBuilder>
    {
        public StringBuilder Create() => new StringBuilder(capacity: 256);

        public bool Return(StringBuilder obj)
        {
            if (obj.Capacity > 4096)
                return false;  // ❌ Don't pool huge stringbuilders

            obj.Clear();  // ✅ Reset state
            return true;   // ✅ Return to pool
        }
    }

    // ❌ ANTI-PATTERN: Manual pooling with ConcurrentBag
    // Use ObjectPool instead - handles thread safety and lifecycle
}

/// <summary>
/// EXAMPLE 3: VALUETASK - Avoid Task Allocation for Sync Completion
/// 
/// THE PROBLEM:
/// Task<T> allocates on heap even if result is already available.
/// 
/// THE SOLUTION:
/// ValueTask<T> can return synchronously without allocation.
/// 
/// WHY IT MATTERS:
/// - 80% cache hit → 80% fewer allocations
/// - Lower latency
/// - Better throughput
/// 
/// GOTCHA: Only for hot paths. Task<T> is simpler for normal code.
/// </summary>
public class ValueTaskExamples
{
    private readonly Dictionary<int, string> _cache = new();

    // ❌ BAD: Task always allocates, even for cache hits
    public async Task<string> GetData_Task(int id)
    {
        if (_cache.TryGetValue(id, out var cached))
            return cached;  // ❌ Still allocates Task<string>

        await Task.Delay(100);  // Simulate async work
        var data = $"Data_{id}";
        _cache[id] = data;
        return data;
    }

    // ✅ GOOD: ValueTask avoids allocation for sync path
    public ValueTask<string> GetData_ValueTask(int id)
    {
        if (_cache.TryGetValue(id, out var cached))
            return new ValueTask<string>(cached);  // ✅ No allocation!

        return GetAsync(id);  // Async path

        async ValueTask<string> GetAsync(int id)
        {
            await Task.Delay(100);
            var data = $"Data_{id}";
            _cache[id] = data;
            return data;
        }
    }

    // ✅ BEST: ValueTask with inline async
    public async ValueTask<string> GetData_Optimized(int id)
    {
        if (_cache.TryGetValue(id, out var cached))
            return cached;  // ✅ Sync return, compiler optimizes

        // Slow path: actually async
        await Task.Delay(100);
        var data = $"Data_{id}";
        _cache[id] = data;
        return data;
    }

    // WHEN TO USE:
    // ✅ High-frequency calls (1000s/sec)
    // ✅ Often completes synchronously (cache, pooling)
    // ✅ Performance-critical path
    // ❌ Normal CRUD operations
    // ❌ UI event handlers

    // GOTCHA: ValueTask can only be awaited ONCE
    public async Task ValueTaskGotcha()
    {
        var vt = GetData_ValueTask(1);

        await vt;  // ✅ First await - OK
                   // await vt;  // ❌ Second await - INVALID!

        // If you need multiple awaits, convert to Task:
        Task<string> task = vt.AsTask();
        await task;  // OK
        await task;  // OK
    }
}

/// <summary>
/// EXAMPLE 4: ASYNC OPTIMIZATION - State Machine Efficiency
/// 
/// THE PROBLEM:
/// Async state machines have overhead.
/// 
/// THE SOLUTION:
/// Minimize state machine complexity.
/// </summary>
public class AsyncOptimizationExamples
{
    // ❌ BAD: Async all the way for simple sync work
    public async Task<int> Calculate_Async(int x, int y)
    {
        var result = x + y;  // ❌ Sync work, why async?
        return await Task.FromResult(result);  // ❌ Unnecessary
    }

    // ✅ GOOD: Sync when possible
    public Task<int> Calculate_Sync(int x, int y)
    {
        var result = x + y;
        return Task.FromResult(result);  // ✅ Fast sync path
    }

    // ✅ BETTER: Return int directly if always sync
    public int Calculate_Direct(int x, int y)
    {
        return x + y;  // ✅ Simplest and fastest
    }

    // ✅ GOOD: ConfigureAwait(false) in library code
    public async Task<string> LibraryMethodAsync()
    {
        var data = await FetchDataAsync().ConfigureAwait(false);  // ✅ Avoid context capture
        var processed = ProcessData(data);
        await SaveDataAsync(processed).ConfigureAwait(false);  // ✅ Avoid context capture
        return "Done";
    }

    // ❌ BAD: Async void (except event handlers)
    public async void BadAsyncVoid()  // ❌ Can't catch exceptions properly
    {
        await Task.Delay(100);
    }

    // ✅ GOOD: Return Task
    public async Task GoodAsync()
    {
        await Task.Delay(100);
    }

    // ✅ OPTIMIZATION: Avoid closures in async
    public async Task WithClosure(List<string> items)
    {
        foreach (var item in items)
        {
            // ❌ Closure over item → allocation per iteration
            await Task.Run(() => Process(item));
        }
    }

    public async Task WithoutClosure(List<string> items)
    {
        foreach (var item in items)
        {
            // ✅ Pass as parameter → no closure
            await Task.Run(() => ProcessParam(item));
        }
    }

    private static Task<string> FetchDataAsync() => Task.FromResult("data");
    private static string ProcessData(string data) => data.ToUpper(CultureInfo.InvariantCulture);
    private static Task SaveDataAsync(string data) => Task.CompletedTask;
    private static void Process(string item) { }
    private static void ProcessParam(string item) { }
}

/// <summary>
/// EXAMPLE 5: COLLECTION PERFORMANCE - Choose the Right Collection
/// 
/// THE PROBLEM:
/// List<T>, Dictionary<K,V>, HashSet<T> have different perf characteristics.
/// </summary>
public class CollectionPerformanceExamples
{
    // ✅ Pre-size collections when you know capacity
    public List<int> CreateList_Sized(int expectedCount)
    {
        return new List<int>(capacity: expectedCount);  // ✅ Avoid resizing
    }

    public Dictionary<string, int> CreateDict_Sized(int expectedCount)
    {
        return new Dictionary<string, int>(capacity: expectedCount);  // ✅ Avoid rehashing
    }

    // ✅ Use HashSet for lookups
    public bool Contains_HashSet(int value, HashSet<int> set)
    {
        return set.Contains(value);  // ✅ O(1)
    }

    public bool Contains_List(int value, List<int> list)
    {
        return list.Contains(value);  // ❌ O(n) - slow for large lists
    }

    // ✅ Use Dictionary over List<KeyValuePair>
    public class ProductLookup
    {
        // ❌ BAD: Linear search
        private readonly List<(int Id, string Name)> _productsBAD = new();

        // ✅ GOOD: O(1) lookup
        private readonly Dictionary<int, string> _productsGOOD = new();

        public string? GetProductBAD(int id)
        {
            var product = _productsBAD.FirstOrDefault(p => p.Id == id);  // ❌ O(n)
            return product.Name;
        }

        public string? GetProductGOOD(int id)
        {
            return _productsGOOD.TryGetValue(id, out var name) ? name : null;  // ✅ O(1)
        }
    }

    // ✅ Span optimization for arrays/lists
    public int SumArray(int[] numbers)
    {
        Span<int> span = numbers;  // ✅ Zero-copy
        int sum = 0;
        foreach (var n in span)
            sum += n;
        return sum;
    }

    public int SumList(List<int> numbers)
    {
        Span<int> span = CollectionsMarshal.AsSpan(numbers);  // ✅ Zero-copy .NET 5+
        int sum = 0;
        foreach (var n in span)
            sum += n;
        return sum;
    }
}

/// <summary>
/// EXAMPLE 6: STRING OPTIMIZATION - Avoid Allocations
/// </summary>
public class StringOptimizationExamples
{
    // ❌ BAD: String concatenation in loop
    public string BuildReport_Bad(List<string> items)
    {
        string result = "";
        foreach (var item in items)
        {
            result += item + "\n";  // ❌ Creates new string each time
        }
        return result;
    }

    // ✅ GOOD: StringBuilder
    public string BuildReport_Good(List<string> items)
    {
        var sb = new StringBuilder(capacity: items.Count * 50);  // ✅ Pre-size
        foreach (var item in items)
        {
            sb.Append(item);
            sb.Append('\n');
        }
        return sb.ToString();
    }

    // ✅ BEST: String.Join for simple concatenation
    public string BuildReport_Best(List<string> items)
    {
        return string.Join('\n', items);  // ✅ Optimized internally
    }

    // ✅ String interning for repeated strings
    private static readonly string CachedStatus = string.Intern("Active");

    public string GetStatus()
    {
        return CachedStatus;  // ✅ Same instance reused
    }

    // ✅ Avoid string.Format in hot paths
    public string Format_Slow(int id, string name)
    {
        return string.Format(CultureInfo.InvariantCulture, "User {0}: {1}", id, name);  // ❌ Slow
    }

    public string Format_Fast(int id, string name)
    {
        return $"User {id}: {name}";  // ✅ Faster interpolation
    }

    // ✅ BEST: Span for parsing without allocations
    public int ParseInt_Span(string text)
    {
        ReadOnlySpan<char> span = text.AsSpan().Trim();  // ✅ No allocation
        return int.Parse(span, CultureInfo.InvariantCulture);
    }
}

// SUMMARY - Optimization Decision Tree:
//
// ALLOCATIONS:
// Q: Need large temporary buffer?
//    → Yes: Use ArrayPool<T>
// Q: Creating expensive objects repeatedly?
//    → Yes: Use ObjectPool<T>
// Q: Hot path with sync completions?
//    → Yes: Use ValueTask<T>
//
// COLLECTIONS:
// Q: Know final size?
//    → Yes: Pre-allocate capacity
// Q: Need fast lookups?
//    → Yes: Dictionary/HashSet (not List)
// Q: Iterating large array/list?
//    → Yes: Use Span<T>
//
// STRINGS:
// Q: Loop concatenation?
//    → Yes: StringBuilder (pre-sized)
// Q: Simple join?
//    → Yes: string.Join
// Q: Parsing hot path?
//    → Yes: ReadOnlySpan<char>
//
// ASYNC:
// Q: Library code?
//    → Yes: ConfigureAwait(false)
// Q: Actually async work?
//    → No: Return Task.FromResult or direct value
// Q: Event handler?
//    → Yes: OK to use async void, else use async Task

// WHEN TO OPTIMIZE:
// ✅ DO optimize:
//   - Hot paths (profiled as bottleneck)
//   - High-throughput APIs
//   - Real-time systems
//   - After measuring with BenchmarkDotNet
//
// ❌ DON'T optimize:
//   - Without profiling first
//   - Code called <100 times/sec
//   - At the expense of readability
//   - Based on assumptions

// PROFILING TOOLS:
// - BenchmarkDotNet: Micro-benchmarking
// - dotTrace: JetBrains profiler
// - PerfView: Microsoft profiler (free)
// - Visual Studio Profiler: Built-in
// - Application Insights: Production monitoring
