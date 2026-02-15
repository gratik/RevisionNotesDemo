// ============================================================================
// CONCURRENT COLLECTIONS
// Reference: Revision Notes - Page 10
// ============================================================================
//
// WHAT ARE CONCURRENT COLLECTIONS?
// --------------------------------
// Thread-safe collection types for multi-threaded access, such as
// ConcurrentDictionary, ConcurrentQueue, and ConcurrentBag.
//
// WHY IT MATTERS
// --------------
// - Prevents race conditions and data corruption
// - Reduces manual locking complexity
// - Improves throughput under parallel workloads
//
// WHEN TO USE
// -----------
// - YES: When multiple threads read/write shared collections
// - YES: Parallel.For, Task.WhenAll, background worker patterns
//
// WHEN NOT TO USE
// ---------------
// - NO: Single-threaded code paths
// - NO: If a simple lock is more readable and sufficient
//
// REAL-WORLD EXAMPLE
// ------------------
// Telemetry aggregation:
// - Multiple threads update counters by key
// - ConcurrentDictionary safely stores counts
// ============================================================================

namespace RevisionNotesDemo.AsyncMultithreading;

using System.Collections.Concurrent;

public class ConcurrentCollectionsDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== CONCURRENTDICTIONARY DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 10\n");

        // ❌ Dictionary - NOT thread-safe for concurrent writes
        Console.WriteLine("--- Regular Dictionary (NOT thread-safe) ---");
        var dict = new Dictionary<int, int>();
        Console.WriteLine("[CONCURRENT] Regular Dictionary created");
        Console.WriteLine("[CONCURRENT] ⚠️ Would throw if used with Parallel.For\n");

        // ✅ ConcurrentDictionary - thread-safe
        Console.WriteLine("--- ConcurrentDictionary (Thread-safe) ---");
        var concurrent = new ConcurrentDictionary<int, int>();

        Parallel.For(0, 100, i =>
        {
            concurrent[i] = i * 2;
        });

        Console.WriteLine($"[CONCURRENT] ✅ Safely added {concurrent.Count} items using Parallel.For");
        Console.WriteLine($"[CONCURRENT] Sample values: {string.Join(", ", concurrent.Take(5).Select(kv => $"{kv.Key}={kv.Value}"))}");

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - Dictionary: NOT thread-safe");
        Console.WriteLine("   - ConcurrentDictionary: Thread-safe for concurrent ops");
    }
}
