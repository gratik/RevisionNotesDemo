# ValueTask<T> - Async Performance

> Subject: [Performance](../README.md)

## ValueTask<T> - Async Performance

### When to Use ValueTask

**Use `Task<T>`** (default):
- Result is always async
- Method awaited multiple times
- Task stored or passed around

**Use `ValueTask<T>`** (hot paths only):
- Result often available synchronously (cached)
- Await exactly once
- Not stored in fields

### Example

```csharp
// ✅ Task: Always async
public async Task<User> GetUserAsync(int id)
{
    return await _db.Users.FindAsync(id);  // Always hits DB
}

// ✅ ValueTask: Often sync (cached)
public async ValueTask<User?> GetCachedUserAsync(int id)
{
    // Often returns immediately (no Task allocation)
    if (_cache.TryGetValue(id, out var cached))
        return cached;  // ✅ Synchronous, no allocation
    
    // Only allocates Task on cache miss
    var user = await _db.Users.FindAsync(id);
    _cache[id] = user;
    return user;
}
```

**ValueTask Rules:**
- ❌ Never await twice
- ❌ Never store in field
- ❌ Never access `.Result`
- ✅ Use when sync completion is common

---


