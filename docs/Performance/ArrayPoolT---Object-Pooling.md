# ArrayPool<T> - Object Pooling

> Subject: [Performance](../README.md)

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


