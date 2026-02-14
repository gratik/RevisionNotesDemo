// ============================================================================
// CONCURRENTDICTIONARY VS DICTIONARY
// Reference: Revision Notes - Page 10
// ============================================================================
// Dictionary: Not thread-safe for writes
// ConcurrentDictionary: Allows safe concurrent reads/writes
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
