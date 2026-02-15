// ============================================================================
// GARBAGE COLLECTION IN .NET
// Reference: Revision Notes - Memory Management & Performance - Page 8
// ============================================================================
// WHAT IS THIS?
// -------------
// The .NET GC that reclaims memory for unreachable objects.
//
// WHY IT MATTERS
// --------------
// ✅ Explains allocation costs and GC pauses
// ✅ Informs performance tuning decisions
//
// WHEN TO USE
// -----------
// ✅ Diagnosing memory pressure or allocation hotspots
// ✅ Understanding Gen0/Gen2 behavior
//
// WHEN NOT TO USE
// ---------------
// ❌ Manual GC.Collect in production code
// ❌ Tuning without profiling data
//
// REAL-WORLD EXAMPLE
// ------------------
// Gen0 vs Gen2 collections during load spikes.
// ============================================================================

namespace RevisionNotesDemo.MemoryManagement;

public class Demo
{
    public byte[] BigBuffer = new byte[10_000_000]; // 10MB
}

public class GarbageCollectionDemo
{
    public static void RunDemo()
    {
        Console.WriteLine("\n=== GARBAGE COLLECTION DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Page 8\n");

        Console.WriteLine("--- GC Generations ---");
        Console.WriteLine($"[GC] Gen 0 collections: {GC.CollectionCount(0)}");
        Console.WriteLine($"[GC] Gen 1 collections: {GC.CollectionCount(1)}");
        Console.WriteLine($"[GC] Gen 2 collections: {GC.CollectionCount(2)}\n");

        Console.WriteLine("--- Creating objects ---");
        var list = new List<Demo>();
        for (int i = 0; i < 5; i++)
        {
            list.Add(new Demo());
            Console.WriteLine($"[GC] Created object {i + 1}");
        }

        Console.WriteLine($"\n[GC] Memory before cleanup: ~{GC.GetTotalMemory(false) / 1024 / 1024} MB");

        // Drop references to make objects eligible for GC
        list.Clear();
        Console.WriteLine("[GC] References cleared - objects now eligible for collection");

        // Force GC (for demonstration only - not recommended in production!)
        Console.WriteLine("[GC] Forcing garbage collection...");
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Console.WriteLine($"[GC] Memory after GC: ~{GC.GetTotalMemory(false) / 1024 / 1024} MB");

        Console.WriteLine($"\n[GC] Gen 0 collections: {GC.CollectionCount(0)}");
        Console.WriteLine($"[GC] Gen 1 collections: {GC.CollectionCount(1)}");
        Console.WriteLine($"[GC] Gen 2 collections: {GC.CollectionCount(2)}");

        Console.WriteLine("\n💡 From Revision Notes:");
        Console.WriteLine("   - Gen 0: Short-lived objects (frequent collection)");
        Console.WriteLine("   - Gen 1: Medium-lived objects");
        Console.WriteLine("   - Gen 2: Long-lived objects (infrequent collection)");
        Console.WriteLine("   - Don't call GC.Collect() in production (let GC manage itself)");
    }
}
