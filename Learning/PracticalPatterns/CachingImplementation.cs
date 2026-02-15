// ============================================================================
// CACHING IMPLEMENTATION
// Reference: Revision Notes - Practical & Scenario-Based - Page 13
// ============================================================================
// WHAT IS THIS?
// -------------
// In-memory and distributed caching patterns in .NET.
//
// WHY IT MATTERS
// --------------
// ✅ Reduces latency for repeated reads
// ✅ Protects backends during traffic spikes
//
// WHEN TO USE
// -----------
// ✅ Read-heavy data with acceptable staleness
// ✅ Expensive computations or shared lookups
//
// WHEN NOT TO USE
// ---------------
// ❌ Highly volatile data needing strict consistency
// ❌ Per-user secrets stored in shared caches
//
// REAL-WORLD EXAMPLE
// ------------------
// Cache user profiles and feature flags with short TTLs.
// ============================================================================

namespace RevisionNotesDemo.PracticalPatterns;

using Microsoft.Extensions.Caching.Memory;

// From Revision Notes - Page 13
public class CachingService
{
    private readonly IMemoryCache _cache;

    public CachingService(IMemoryCache cache)
    {
        _cache = cache;
    }

    public string GetData(string key)
    {
        return _cache.GetOrCreate(key, entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
            Console.WriteLine($"[CACHE] Cache miss - generating data for key: {key}");
            return $"Cached Data for {key}";
        }) ?? string.Empty;
    }
}

public class CachingDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== CACHING IMPLEMENTATION DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 13\n");

        var cache = new MemoryCache(new MemoryCacheOptions());
        var service = new CachingService(cache);

        // First access - cache miss
        Console.WriteLine("First access:");
        var data1 = service.GetData("user:123");
        Console.WriteLine($"[CACHE] Retrieved: {data1}");

        // Second access - cache hit
        Console.WriteLine("\nSecond access (cached):");
        var data2 = service.GetData("user:123");
        Console.WriteLine($"[CACHE] Retrieved: {data2}");

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - Use IMemoryCache for in-memory caching");
        Console.WriteLine("   - Set expiration times appropriately");
        Console.WriteLine("   - For distributed: Use Redis or SQL Server cache");
    }
}
