# Common Pitfalls

> Subject: [Performance](../README.md)

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


