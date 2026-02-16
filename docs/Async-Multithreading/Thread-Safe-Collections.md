# Thread-Safe Collections

> Subject: [Async-Multithreading](../README.md)

## Thread-Safe Collections

### Comparison Table

| Collection                          | Use Case                        | Performance                   |
| ----------------------------------- | ------------------------------- | ----------------------------- |
| **ConcurrentBag&lt;T&gt;**          | Unordered producer/consumer     | Fast adds, moderate reads     |
| **ConcurrentQueue&lt;T&gt;**        | FIFO ordered                    | Fast, minimal contention      |
| **ConcurrentStack&lt;T&gt;**        | LIFO ordered                    | Fast, minimal contention      |
| **ConcurrentDictionary&lt;K,V&gt;** | Key-value lookup                | Good, uses fine-grained locks |
| **BlockingCollection&lt;T&gt;**     | Producer-consumer with blocking | Slower, but convenient        |
| **Channel&lt;T&gt;**                | Async producer-consumer         | Best for async scenarios      |

### ConcurrentDictionary - Most Common

```csharp
var cache = new ConcurrentDictionary<int, User>();

// ✅ Thread-safe add
var user = cache.GetOrAdd(userId, id => _db.GetUser(id));

// ✅ Thread-safe update
cache.AddOrUpdate(
    userId,
    addValue: new User { Id = userId },
    updateValueFactory: (id, existing) =>
    {
        existing.LastUpdated = DateTime.UtcNow;
        return existing;
    });

// ✅ Thread-safe remove
cache.TryRemove(userId, out var removed);
```

### Channel&lt;T&gt; - Modern Async Producer-Consumer

```csharp
// ✅ Create bounded channel
var channel = Channel.CreateBounded<WorkItem>(new BoundedChannelOptions(100)
{
    FullMode = BoundedChannelFullMode.Wait
});

// Producer
await channel.Writer.WriteAsync(workItem);

// Consumer
await foreach (var item in channel.Reader.ReadAllAsync())
{
    await ProcessAsync(item);
}
```

---


