// ============================================================================
// GARBAGE COLLECTION IN .NET
// Reference: Revision Notes - Memory Management & Performance - Page 8
// ============================================================================
// DEFINITION:
//   Automatic memory management system that reclaims memory occupied by objects
//   no longer in use. Eliminates manual memory management bugs like memory leaks
//   and dangling pointers.
//
// HOW IT WORKS:
//   1. MARK PHASE: GC traces from root references, marking reachable objects
//   2. SWEEP PHASE: Unreachable objects are identified as garbage
//   3. COMPACT PHASE: Moves surviving objects together to reduce fragmentation
//
// GENERATIONAL GC:
//   
//   GENERATION 0 (Gen 0):
//     • Short-lived objects (temporary variables, new allocations)
//     • Collected most frequently (very fast)
//     • Most objects die young (generational hypothesis)
//     • Size: Few MB
//   
//   GENERATION 1 (Gen 1):
//     • "Buffer" between short-lived and long-lived
//     • Objects that survived one Gen0 collection
//     • Collected less frequently
//   
//   GENERATION 2 (Gen 2):
//     • Long-lived objects (static data, singletons, caches)
//     • Collected infrequently (expensive)
//     • Full GC collects all generations
//     • Size: Can be very large (GBs)
//   
//   LARGE OBJECT HEAP (LOH):
//     • Objects > 85,000 bytes go directly here
//     • Not compacted by default (too expensive)
//     • Collected with Gen2
//     • Can cause fragmentation
//
// GC ROOTS:
//   • Static fields
//   • Local variables on stack
//   • CPU registers
//   • GC handles
//   • Finalization queue
//
// WHEN GC RUNS:
//   • Gen0 is full
//   • Memory pressure (system running low on memory)
//   • Explicitly called (GC.Collect() - not recommended)
//
// FINALIZATION:
//   Objects with finalizers (~ClassName) go to finalization queue and survive
//   one more collection. Finalizers run on separate thread. Slows down GC.
//
// BEST PRACTICES:
//   ❌ DON'T: Call GC.Collect() manually (except specific testing scenarios)
//   ✅ DO: Let GC manage itself - it's optimized and self-tuning
//   ❌ DON'T: Create many short-lived large objects (LOH fragmentation)
//   ✅ DO: Reuse large buffers (ArrayPool<T>)
//   ✅ DO: Implement IDisposable for unmanaged resources
//   ✅ DO: Use 'using' statements for automatic cleanup
//   ❌ DON'T: Use finalizers unless you absolutely need them
//   ✅ DO: Dispose of event handlers (unsubscribe)
//
// PERFORMANCE TIPS:
//   • Reduce Gen0 allocations (use object pooling for high-frequency scenarios)
//   • Keep Gen2 small (avoid long-lived references)
//   • Avoid LOH allocations when possible
//   • Use ArrayPool<T> for large temporary buffers
//   • Avoid boxing value types
//
// MONITORING:
//   • Performance counters (% Time in GC, Gen 0/1/2 Collections)
//   • Visual Studio Diagnostic Tools
//   • PerfView
//   • dotMemory (JetBrains)
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
