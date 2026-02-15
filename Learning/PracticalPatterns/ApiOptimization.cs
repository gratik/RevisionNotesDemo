// ============================================================================
// API OPTIMIZATION
// Reference: Revision Notes - Practical Scenarios - Page 13
// ============================================================================
// WHAT IS THIS?
// -------------
// Performance patterns for APIs (async I/O, pagination, pooling).
//
// WHY IT MATTERS
// --------------
// ✅ Reduces latency and resource usage under load
// ✅ Improves scalability for busy endpoints
//
// WHEN TO USE
// -----------
// ✅ High-traffic endpoints or large datasets
// ✅ API responses that can be streamed or paginated
//
// WHEN NOT TO USE
// ---------------
// ❌ Low-traffic prototypes where simplicity wins
// ❌ Premature optimization without measurements
//
// REAL-WORLD EXAMPLE
// ------------------
// Paginated list endpoints with async data access.
// ============================================================================

using System.Diagnostics;

namespace RevisionNotesDemo.PracticalPatterns;

// ============================================================================
// ❌ BAD EXAMPLE - Synchronous, blocking operations
// ============================================================================

public class UnoptimizedApiService
{
    public List<string> GetAllRecords()
    {
        // Simulating synchronous database call - BLOCKS the thread
        Thread.Sleep(2000); // Blocking operation - wastes thread pool resources

        // Returns all records without pagination - memory intensive
        return Enumerable.Range(1, 10000)
            .Select(i => $"Record {i}")
            .ToList();
    }

    public string ProcessMultipleRequests()
    {
        var sw = Stopwatch.StartNew();

        // Sequential processing - slow
        var result1 = GetAllRecords();
        var result2 = GetAllRecords();
        var result3 = GetAllRecords();

        sw.Stop();
        return $"Processed {result1.Count + result2.Count + result3.Count} records in {sw.ElapsedMilliseconds}ms";
    }
}

// ============================================================================
// ✅ GOOD EXAMPLE - Async I/O, caching, pagination
// ============================================================================

public class OptimizedApiService
{
    private static readonly HttpClient _httpClient = new HttpClient(); // Reused instance
    private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(10); // Limit concurrent operations

    // Async I/O - doesn't block threads
    public async Task<List<string>> GetRecordsPagedAsync(int page, int pageSize)
    {
        // Simulating async database call - releases thread while waiting
        await Task.Delay(100); // Non-blocking operation

        // Pagination - only returns requested page
        var skip = (page - 1) * pageSize;
        return Enumerable.Range(skip + 1, pageSize)
            .Select(i => $"Record {i}")
            .ToList();
    }

    // Parallel async operations with throttling
    public async Task<string> ProcessMultipleRequestsAsync()
    {
        var sw = Stopwatch.StartNew();

        // Parallel processing with throttling
        var tasks = new[]
        {
            GetRecordsPagedAsync(1, 100),
            GetRecordsPagedAsync(2, 100),
            GetRecordsPagedAsync(3, 100)
        };

        var results = await Task.WhenAll(tasks);

        sw.Stop();
        return $"Processed {results.Sum(r => r.Count)} records in {sw.ElapsedMilliseconds}ms";
    }

    // Efficient HTTP client usage (connection pooling)
    public async Task<string> FetchExternalDataAsync(string url)
    {
        await _semaphore.WaitAsync(); // Throttle concurrent requests
        try
        {
            var response = await _httpClient.GetStringAsync(url);
            return response;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    // Streaming large responses instead of buffering
    public async IAsyncEnumerable<string> StreamRecordsAsync(int totalRecords)
    {
        const int batchSize = 100;
        for (int i = 0; i < totalRecords; i += batchSize)
        {
            await Task.Delay(10); // Simulate database fetch

            for (int j = i; j < Math.Min(i + batchSize, totalRecords); j++)
            {
                yield return $"Record {j + 1}";
            }
        }
    }
}

// ============================================================================
// DEMONSTRATION
// ============================================================================

public class ApiOptimizationDemo
{
    public static async Task RunDemoAsync()
    {
        Console.WriteLine("\n=== API OPTIMIZATION DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Practical Scenarios - Page 13\n");

        // Compare synchronous vs asynchronous approaches
        Console.WriteLine("--- Unoptimized (Synchronous) API ---");
        var unoptimized = new UnoptimizedApiService();
        var sw1 = Stopwatch.StartNew();
        var result1 = unoptimized.ProcessMultipleRequests();
        sw1.Stop();
        Console.WriteLine($"[UNOPTIMIZED] {result1}");
        Console.WriteLine($"[UNOPTIMIZED] Total time: {sw1.ElapsedMilliseconds}ms\n");

        Console.WriteLine("--- Optimized (Async + Pagination) API ---");
        var optimized = new OptimizedApiService();
        var sw2 = Stopwatch.StartNew();
        var result2 = await optimized.ProcessMultipleRequestsAsync();
        sw2.Stop();
        Console.WriteLine($"[OPTIMIZED] {result2}");
        Console.WriteLine($"[OPTIMIZED] Total time: {sw2.ElapsedMilliseconds}ms");
        Console.WriteLine($"[OPTIMIZED] Performance improvement: {((float)sw1.ElapsedMilliseconds / sw2.ElapsedMilliseconds):F1}x faster!\n");

        // Demonstrate pagination
        Console.WriteLine("--- Pagination Example ---");
        var page1 = await optimized.GetRecordsPagedAsync(1, 5);
        Console.WriteLine($"[PAGINATION] Page 1: {string.Join(", ", page1.Take(3))}... ({page1.Count} records)");
        var page2 = await optimized.GetRecordsPagedAsync(2, 5);
        Console.WriteLine($"[PAGINATION] Page 2: {string.Join(", ", page2.Take(3))}... ({page2.Count} records)\n");

        // Demonstrate streaming
        Console.WriteLine("--- Streaming Large Dataset ---");
        Console.Write("[STREAMING] ");
        int count = 0;
        await foreach (var record in optimized.StreamRecordsAsync(1000))
        {
            count++;
            if (count % 100 == 0)
            {
                Console.Write($"{count}...");
            }
        }
        Console.WriteLine($" Done! Streamed {count} records without loading all into memory.\n");

        Console.WriteLine("💡 From Revision Notes - Optimization Techniques:");
        Console.WriteLine("   ✅ Use async/await for I/O operations");
        Console.WriteLine("   ✅ Implement pagination for large datasets");
        Console.WriteLine("   ✅ Reuse HttpClient instances (connection pooling)");
        Console.WriteLine("   ✅ Use streaming for large responses");
        Console.WriteLine("   ✅ Throttle concurrent operations with SemaphoreSlim");
        Console.WriteLine("   ✅ Response caching for frequently accessed data");
    }
}
