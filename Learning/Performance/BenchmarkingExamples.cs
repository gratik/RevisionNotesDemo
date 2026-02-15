// ==============================================================================
// BENCHMARKING WITH BENCHMARKDOTNET - Measure Real Performance
// ==============================================================================
// WHAT IS THIS?
// -------------
// Repeatable performance measurement with BenchmarkDotNet.
//
// WHY IT MATTERS
// --------------
// ✅ Avoids false assumptions about speed
// ✅ Captures allocations and variability
//
// WHEN TO USE
// -----------
// ✅ Comparing algorithms or validating performance claims
// ✅ Tuning hot paths with measurable impact
//
// WHEN NOT TO USE
// ---------------
// ❌ I/O-bound operations with high variance
// ❌ Unprofiled business logic where correctness matters most
//
// REAL-WORLD EXAMPLE
// ------------------
// Compare StringBuilder vs string concatenation.
// ==============================================================================

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Environments;
using System.Runtime.InteropServices;
using System.Text;

namespace RevisionNotesDemo.Performance;

/// <summary>
/// EXAMPLE 1: BASIC BENCHMARK - String Concatenation
/// 
/// THE PROBLEM:
/// Need to measure which string concatenation approach is fastest.
/// 
/// THE SOLUTION:
/// Create benchmark class with [Benchmark] methods.
/// 
/// WHY IT MATTERS:
/// - Data-driven optimization
/// - Avoid premature optimization
/// - See JIT effects
/// 
/// USAGE:
///   dotnet run -c Release
///   Or: BenchmarkRunner.Run<StringConcatBenchmarks>();
/// </summary>
[MemoryDiagnoser]  // ✅ Shows allocations
[Orderer(SummaryOrderPolicy.FastestToSlowest)]  // ✅ Sort by speed
public class StringConcatBenchmarks
{
    private const int Iterations = 100;

    // ❌ BAD: String concatenation with +
    [Benchmark(Baseline = true)]  // ✅ Compare others to this
    public string StringConcat()
    {
        string result = "";
        for (int i = 0; i < Iterations; i++)
        {
            result += i.ToString();  // ❌ Creates new string each time
            result += ", ";
        }
        return result;
    }

    // ✅ GOOD: StringBuilder
    [Benchmark]
    public string StringBuilder_Default()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < Iterations; i++)
        {
            sb.Append(i);
            sb.Append(", ");
        }
        return sb.ToString();
    }

    // ✅ BETTER: StringBuilder with capacity
    [Benchmark]
    public string StringBuilder_WithCapacity()
    {
        var sb = new StringBuilder(capacity: 500);  // ✅ Pre-allocate
        for (int i = 0; i < Iterations; i++)
        {
            sb.Append(i);
            sb.Append(", ");
        }
        return sb.ToString();
    }

    // ✅ BEST: StringBuilder with span (C# 10+)
    [Benchmark]
    public string StringBuilder_WithSpan()
    {
        var sb = new StringBuilder(capacity: 500);
        Span<char> buffer = stackalloc char[20];

        for (int i = 0; i < Iterations; i++)
        {
            if (i.TryFormat(buffer, out int written))
            {
                sb.Append(buffer[..written]);
            }
            sb.Append(", ");
        }
        return sb.ToString();
    }
}

// RESULTS INTERPRETATION:
// |                      Method |      Mean |    Error |   StdDev | Ratio |    Gen0 |  Allocated |
// |---------------------------- |----------:|---------:|---------:|------:|--------:|-----------:|
// |          StringBuilder_Span |  4.234 μs | 0.021 μs | 0.019 μs |  0.05 |  0.0610 |      528 B |
// |  StringBuilder_WithCapacity |  4.987 μs | 0.034 μs | 0.031 μs |  0.06 |  0.0763 |      656 B |
// |       StringBuilder_Default |  6.123 μs | 0.045 μs | 0.042 μs |  0.07 |  0.1526 |     1312 B |
// |               StringConcat* | 87.456 μs | 0.567 μs | 0.530 μs |  1.00 | 12.5732 |   105896 B |
// 
// * = Baseline
// → StringBuilder_Span is 20x faster and 200x less memory!

/// <summary>
/// EXAMPLE 2: PARAMETRIZED BENCHMARKS - Test Multiple Scenarios
/// 
/// THE PROBLEM:
/// Behavior changes with different input sizes.
/// 
/// THE SOLUTION:
/// Use [Params] to test multiple values.
/// </summary>
[MemoryDiagnoser]
public class CollectionBenchmarks
{
    [Params(10, 100, 1000)]  // ✅ Test with different sizes
    public int ItemCount { get; set; }

    private List<int> _items = new();

    [GlobalSetup]  // ✅ Run once before all benchmarks
    public void Setup()
    {
        _items = Enumerable.Range(0, ItemCount).ToList();
    }

    // Test: LINQ vs for loop
    [Benchmark(Baseline = true)]
    public int Sum_ForLoop()
    {
        int sum = 0;
        for (int i = 0; i < _items.Count; i++)
        {
            sum += _items[i];
        }
        return sum;
    }

    [Benchmark]
    public int Sum_Foreach()
    {
        int sum = 0;
        foreach (var item in _items)
        {
            sum += item;
        }
        return sum;
    }

    [Benchmark]
    public int Sum_Linq()
    {
        return _items.Sum();  // Is LINQ slower?
    }

    [Benchmark]
    public int Sum_Span()
    {
        Span<int> span = CollectionsMarshal.AsSpan(_items);  // ✅ Zero-copy
        int sum = 0;
        for (int i = 0; i < span.Length; i++)
        {
            sum += span[i];
        }
        return sum;
    }
}

// RESULTS:
// ItemCount=10:
//   ForLoop: 5.2 ns, Foreach: 5.4 ns, LINQ: 24.3 ns, Span: 4.8 ns
// ItemCount=1000:
//   ForLoop: 412 ns, Foreach: 410 ns, LINQ: 1,234 ns, Span: 389 ns
//
// INSIGHT: Span fastest, LINQ has overhead, foreach nearly same as for

/// <summary>
/// EXAMPLE 3: MEMORY DIAGNOSTICS - Track Allocations
/// 
/// THE PROBLEM:
/// Hidden allocations cause GC pressure.
/// 
/// THE SOLUTION:
/// [MemoryDiagnoser] shows Gen0/Gen1/Gen2 collections and bytes allocated.
/// </summary>
[MemoryDiagnoser]
public class AllocationBenchmarks
{
    private const int Size = 1000;

    // ❌ Hidden allocation: LINQ ToArray
    [Benchmark]
    public int[] Filter_Linq()
    {
        return Enumerable.Range(0, Size)
            .Where(x => x % 2 == 0)  // ❌ IEnumerable allocation
            .ToArray();              // ❌ Array allocation
    }

    // ✅ Less allocation: pre-sized list
    [Benchmark]
    public List<int> Filter_List()
    {
        var result = new List<int>(capacity: Size / 2);  // ✅ Pre-allocate
        for (int i = 0; i < Size; i++)
        {
            if (i % 2 == 0)
                result.Add(i);
        }
        return result;
    }

    // ✅ Zero allocation: Span (if caller can accept Span)
    [Benchmark]
    public int Filter_Span_Count()
    {
        Span<int> numbers = stackalloc int[Size];
        for (int i = 0; i < Size; i++)
            numbers[i] = i;

        int count = 0;
        foreach (var n in numbers)
        {
            if (n % 2 == 0)
                count++;
        }
        return count;  // Just return count (in real code, might process in-place)
    }
}

// RESULTS:
// |          Method |     Mean |   Gen0 |  Allocated |
// |---------------- |---------:|-------:|-----------:|
// |  Filter_Linq    | 12.34 μs | 0.9155 |    7.62 KB |
// |  Filter_List    |  8.67 μs | 0.4883 |    4.08 KB |
// |  Filter_Span    |  3.21 μs |      - |          - |  ✅ Zero allocations!

/// <summary>
/// EXAMPLE 4: COMPARING ALGORITHMS - Which sorting is fastest?
/// </summary>
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]  // ✅ Specify runtime
public class SortingBenchmarks
{
    [Params(100, 1000, 10000)]
    public int Size { get; set; }

    private int[] _data = Array.Empty<int>();
    private readonly Random _random = new(42);  // ✅ Fixed seed for reproducibility

    [GlobalSetup]
    public void Setup()
    {
        _data = Enumerable.Range(0, Size)
            .OrderBy(_ => _random.Next())
            .ToArray();
    }

    [IterationSetup]  // ✅ Run before EACH iteration
    public void IterationSetup()
    {
        // Restore data order (each benchmark modifies array)
        _random.Next();  // Re-shuffle logic would go here
    }

    [Benchmark(Baseline = true)]
    public void Array_Sort()
    {
        var copy = (int[])_data.Clone();
        Array.Sort(copy);  // Built-in quicksort
    }

    [Benchmark]
    public void Linq_OrderBy()
    {
        var sorted = _data.OrderBy(x => x).ToArray();
    }

    [Benchmark]
    public void BubbleSort()
    {
        var copy = (int[])_data.Clone();
        BubbleSortImpl(copy);
    }

    private void BubbleSortImpl(int[] arr)
    {
        int n = arr.Length;
        for (int i = 0; i < n - 1; i++)
        {
            for (int j = 0; j < n - i - 1; j++)
            {
                if (arr[j] > arr[j + 1])
                    (arr[j], arr[j + 1]) = (arr[j + 1], arr[j]);
            }
        }
    }
}

// RESULTS (Size=1000):
// |         Method |      Mean | Ratio |
// |--------------- |----------:|------:|
// |     Array_Sort |  45.23 μs |  1.00 |
// |   Linq_OrderBy |  98.67 μs |  2.18 |  ← LINQ has overhead
// |     BubbleSort | 892.45 μs | 19.73 |  ← O(n²) is slow!

/// <summary>
/// EXAMPLE 5: CONFIGURATION - Custom Benchmark Settings
/// </summary>
[MemoryDiagnoser]
[Config(typeof(CustomConfig))]
public class ConfiguredBenchmarks
{
    // Custom configuration class
    private class CustomConfig : ManualConfig
    {
        public CustomConfig()
        {
            // ✅ Add multiple runtimes
            AddJob(Job.Default.WithRuntime(CoreRuntime.Core80).WithId(".NET 8"));
            AddJob(Job.Default.WithRuntime(CoreRuntime.Core70).WithId(".NET 7"));

            // ✅ Custom columns
            AddColumn(StatisticColumn.P95);  // 95th percentile
            AddColumn(RankColumn.Arabic);

            // ✅ Export results
            AddExporter(BenchmarkDotNet.Exporters.MarkdownExporter.GitHub);
        }
    }

    [Benchmark]
    public int TestMethod()
    {
        return Enumerable.Range(0, 1000).Sum();
    }
}

/// <summary>
/// EXAMPLE 6: COMMON PITFALLS AND BEST PRACTICES
/// </summary>
public class BenchmarkingPitfalls
{
    // ❌ PITFALL 1: Dead code elimination
    [Benchmark]
    public void DeadCodeExample()
    {
        int x = 5 + 3;  // ❌ JIT removes this - result unused!
        // Benchmark shows 0.1 ns - unrealistic
    }

    // ✅ FIX: Return or consume result
    [Benchmark]
    public int DeadCodeFixed()
    {
        return 5 + 3;  // ✅ Return value
    }

    // ❌ PITFALL 2: Not running in Release mode
    // Always: dotnet run -c Release
    // Debug mode has optimizations disabled

    // ❌ PITFALL 3: Benchmarking async methods incorrectly
    [Benchmark]
    public async Task AsyncWrong()
    {
        await Task.Delay(10);  // ❌ Measures Task.Delay, not your code
    }

    // ✅ Use async only for I/O-bound work you're testing
    [Benchmark]
    public async Task<string> AsyncCorrect()
    {
        // Real async work you want to measure
        using var client = new HttpClient();
        return await client.GetStringAsync("https://example.com");
    }

    // ❌ PITFALL 4: Not enough iterations
    // BenchmarkDotNet handles this automatically
    // Runs warmup, then pilot, then actual iterations
    // Achieves statistical significance

    // ❌ PITFALL 5: External factors (disk I/O, network)
    // Benchmarks should be CPU/memory-bound
    // Network calls have too much variance
}

// HOW TO RUN BENCHMARKS:
//
// 1. Install package:
//    dotnet add package BenchmarkDotNet
//
// 2. Create benchmark class with [Benchmark] methods
//
// 3. Run in Release mode:
//    dotnet run -c Release
//    
//    Or in Main():
//    BenchmarkRunner.Run<StringConcatBenchmarks>();
//
// 4. View results in BenchmarkDotNet.Artifacts/results/
//
// 5. Interpret:
//    - Mean: Average time
//    - Error: Standard error
//    - StdDev: Standard deviation
//    - Ratio: Compared to baseline
//    - Gen0: GC collections per 1000 operations
//    - Allocated: Bytes allocated per operation

// SUMMARY - Benchmarking Best Practices:
//
// ✅ DO:
// - Run in Release mode
// - Use [MemoryDiagnoser] to see allocations
// - Set [Baseline = true] for comparison
// - Use [Params] for different scenarios
// - Return or consume benchmark results
// - Warm up (BenchmarkDotNet does this)
// - Run 20+ iterations for statistics
//
// ❌ DON'T:
// - Benchmark in Debug mode
// - Ignore allocations (Gen0/Allocated columns)
// - Test network/disk directly (too variable)
// - Trust "Console.WriteLine(stopwatch)"
// - Optimize without measuring first
// - Compare across different machines
//
// WHEN TO BENCHMARK:
// ✅ Hot paths (called 1000s of times)
// ✅ Before micro-optimizing
// ✅ Comparing algorithms (LINQ vs for)
// ✅ Validating perf claims
// ❌ Not for business logic (focus on correctness)
// ❌ Not for I/O-bound code (network is bottleneck)
