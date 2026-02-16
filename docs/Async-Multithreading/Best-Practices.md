# Best Practices

> Subject: [Async-Multithreading](../README.md)

## Best Practices

### ✅ Async Best Practices

- Use `async`/`await` for I/O-bound work (network, disk, database)
- Use `Task.Run()` for CPU-bound work you want to offload
- Always pass `CancellationToken` through
- Never use `.Result` or `.Wait()` (blocks thread, risks deadlock)
- Avoid `async void` except for event handlers
- Use `ConfigureAwait(false)` in library code

### ✅ Thread Safety Best Practices

- Prefer immutable data structures (no sharing = no problems)
- Use concurrent collections for shared state
- Minimize shared state (use message passing instead)
- Use `async`/`await` instead of manual threading
- Never lock on `this`, `typeof(MyClass)`, or strings

### ✅ Performance Best Practices

- Use `ValueTask<T>` only when profiling shows benefit
- Avoid excessive allocations in hot async paths
- Use `Task.WhenAll` for parallel I/O operations
- Use `Parallel.ForEach` for CPU-bound parallel work
- Cache frequently-used Tasks when appropriate

---


