# Best Practices

> Subject: [Performance](../README.md)

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


