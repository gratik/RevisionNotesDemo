# I/O-Bound vs CPU-Bound Work

> Subject: [Async-Multithreading](../README.md)

## I/O-Bound vs CPU-Bound Work

### The Fundamental Difference

**I/O-Bound:** Waiting for external operations (network, disk, database)

- **Use**: `async`/`await` with `Task`
- **Why**: Thread is freed while waiting, doesn't block
- **Examples**: HTTP calls, file I/O, database queries

**CPU-Bound:** Computing results (calculations, encryption, parsing)

- **Use**: `Task.Run()` or `Parallel` APIs
- **Why**: Distributes work across multiple cores
- **Examples**: Image processing, data analysis, cryptography

```csharp
// ✅ I/O-Bound: Use async/await
public async Task<string> GetDataAsync()
{
    // Thread is freed during HTTP call
    return await _httpClient.GetStringAsync("https://api.example.com/data");
}

// ✅ CPU-Bound: Use Task.Run to offload to thread pool
public async Task<int> CalculatePrimesAsync(int max)
{
    // Offload heavy computation to thread pool
    return await Task.Run(() =>
    {
        var primes = new List<int>();
        for (int i = 2; i < max; i++)
            if (IsPrime(i)) primes.Add(i);
        return primes.Count;
    });
}
```

### Decision Tree

```
Does work involve waiting (network/disk/DB)?
    → Yes: Use async/await (I/O-bound)
    → No: Is it heavy computation?
        → Yes: Use Task.Run or Parallel (CPU-bound)
        → No: Run synchronously
```

---


