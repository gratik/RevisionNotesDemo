# Performance Checklist

> Subject: [Performance](../README.md)

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


