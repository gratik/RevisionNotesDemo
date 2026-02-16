# Deadlock Prevention

> Subject: [Async-Multithreading](../README.md)

## Deadlock Prevention

### The Classic ASP.NET Deadlock

```csharp
// ❌ DEADLOCK: Blocking on async code in synchronization context
public void DeadlockExample()
{
    var result = GetDataAsync().Result;  // ❌ DEADLOCK!
    // UI thread waits for task, task waits for UI thread
}

private async Task<string> GetDataAsync()
{
    await Task.Delay(100);  // Tries to resume on UI thread (blocked!)
    return "data";
}
```

### Solution 1: ConfigureAwait(false)

```csharp
// ✅ GOOD: Library code uses ConfigureAwait(false)
public async Task<string> GetDataAsync()
{
    // Don't capture synchronization context
    await Task.Delay(100).ConfigureAwait(false);
    return "data";
}

// Safe to block (but still not recommended)
var result = GetDataAsync().Result;  // ✅ Won't deadlock
```

### Solution 2: Async All The Way

```csharp
// ✅ BEST: Never block on async code
public async Task ProcessAsync()
{
    var result = await GetDataAsync();  // ✅ Properly awaited
}
```

### ConfigureAwait Guidelines

| Context          | Use ConfigureAwait(false)?            |
| ---------------- | ------------------------------------- |
| **Library code** | ✅ Yes (don't need sync context)      |
| **ASP.NET Core** | ⚠️ Optional (no sync context anyway)  |
| **WPF/WinForms** | ❌ No (need UI thread)                |
| **Console apps** | ⚠️ Optional (usually no sync context) |

---


