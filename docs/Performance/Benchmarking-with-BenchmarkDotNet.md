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

## Detailed Guidance

Performance guidance focuses on bottleneck-first optimization supported by representative measurements and guardrails.

### Design Notes
- Define success criteria for Benchmarking with BenchmarkDotNet before implementation work begins.
- Keep boundaries explicit so Benchmarking with BenchmarkDotNet decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Benchmarking with BenchmarkDotNet in production-facing code.
- When performance, correctness, or maintainability depends on consistent Benchmarking with BenchmarkDotNet decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Benchmarking with BenchmarkDotNet as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Benchmarking with BenchmarkDotNet is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Benchmarking with BenchmarkDotNet are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

