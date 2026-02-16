# Task, Thread, and ValueTask

> Subject: [Async-Multithreading](../README.md)

## Task, Thread, and ValueTask

### Comparison Table

| Feature       | Task             | Thread            | ValueTask                      |
| ------------- | ---------------- | ----------------- | ------------------------------ |
| **Purpose**   | Async operations | Direct threading  | High-perf async                |
| **Overhead**  | Moderate         | High (1MB stack)  | Minimal (struct)               |
| **Awaitable** | Yes              | No                | Yes                            |
| **Result**    | Task&lt;T&gt;    | N/A               | ValueTask&lt;T&gt;             |
| **Use Case**  | General async    | Rare, legacy code | Hot paths with sync completion |
| **Can Reuse** | No               | No                | No (await once!)               |

### When to Use Each

```csharp
// ✅ Task: Default choice for async operations
public async Task<User> GetUserAsync(int id)
{
    var user = await _db.Users.FindAsync(id);
    return user;
}

// ✅ ValueTask: When result is often synchronously available
public async ValueTask<User?> GetCachedUserAsync(int id)
{
    // Often returns immediately from cache (avoiding Task allocation)
    if (_cache.TryGetValue(id, out var cached))
        return cached;  // ✅ Synchronous completion

    // Only allocates Task if cache misses
    return await _db.Users.FindAsync(id);
}

// ❌ Thread: Avoid unless you need very specific control
public void LegacyThreadExample()
{
    var thread = new Thread(() =>
    {
        // Heavy computation
    });
    thread.Start();
    thread.Join();  // ❌ Prefer Task.Run instead
}
```

**ValueTask Rules:**

- ✅ Use for hot paths where sync completion is common (e.g., caching)
- ❌ Never await more than once
- ❌ Don't store in fields
- ❌ Never use `.Result` or `.GetAwaiter().GetResult()`

---


