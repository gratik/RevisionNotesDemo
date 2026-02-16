# Parallel Execution Patterns

> Subject: [Async-Multithreading](../README.md)

## Parallel Execution Patterns

### Task.WhenAll - Execute Multiple Tasks Concurrently

```csharp
// ✅ Parallel HTTP requests (all start immediately)
var urls = new[] { "url1", "url2", "url3" };
var tasks = urls.Select(url => _httpClient.GetStringAsync(url));
var results = await Task.WhenAll(tasks);
// All 3 requests happen concurrently

// ❌ Sequential execution (one at a time)
var results = new List<string>();
foreach (var url in urls)
{
    results.Add(await _httpClient.GetStringAsync(url));  // ❌ Waits for each
}
```

### Task.WhenAny - First Completed Wins

```csharp
// Timeout pattern
var dataTask = FetchDataAsync();
var timeoutTask = Task.Delay(TimeSpan.FromSeconds(5));

var completedTask = await Task.WhenAny(dataTask, timeoutTask);
if (completedTask == timeoutTask)
    throw new TimeoutException("Operation timed out");

return await dataTask;
```

### Parallel.ForEach - CPU-Bound Parallel Work

```csharp
// ✅ Process large collection in parallel (CPU-bound)
var images = GetImages();
Parallel.ForEach(images, image =>
{
    image.Resize(800, 600);
    image.ApplyFilter();
});

// Control parallelism
var options = new ParallelOptions { MaxDegreeOfParallelism = 4 };
Parallel.ForEach(items, options, item => ProcessItem(item));
```

---


