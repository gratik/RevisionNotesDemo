# Benchmarking with BenchmarkDotNet

> Subject: [Performance](../README.md)

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


