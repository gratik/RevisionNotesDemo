// ==============================================================================
// Service Communication Patterns (Sync vs Async)
// ==============================================================================
// WHAT IS THIS?
// Microservices communicate via synchronous (REST, gRPC - request/response) or asynchronous (message queues, events) patterns. Each has tradeoffs in latency, coupling, and complexity.
//
// WHY IT MATTERS
// ✅ SYNC: Immediate responses, simple debugging | ✅ ASYNC: Decoupled, scalable, survives failures | ✅ MIXED: Combines benefits (sync for user-facing, async for background) | ✅ THROUGHPUT: Async handles spikes with queuing | ✅ RESILIENCE: Async survives service unavailability
//
// WHEN TO USE
// ✅ Sync (REST): Real-time data needed, simple flow | ✅ Async (Queues): Complex workflows, heavy processing, loose coupling | ✅ Mixed: Checkout (sync payment) + fulfillment (async shipping)
//
// WHEN NOT TO USE
// ❌ Sync for everything (low resilience, tight coupling) | ❌ Async for everything (increased complexity, harder debugging)
//
// REAL-WORLD EXAMPLE
// Uber: Real-time request sync to driver (REST) for immediate update. Background async: update ratings, record analytics, send email receipt. Driver gets response immediately, other work happens in background.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Microservices;

public class ServiceCommunicationPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Service Communication Patterns (Sync vs Async)");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        Overview();
        SynchronousCommunication();
        AsynchronousCommunication();
        MonolithContext();
        MixedApproach();
        DetailedComparison();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Communication spectrum:");
        Console.WriteLine("  Fast + Sync = REST (direct coupling)");
        Console.WriteLine("  Slow + Sync = REST with timeout risk");
        Console.WriteLine("  Fast + Async = Message queue (decoupled)\n");
    }

    private static void SynchronousCommunication()
    {
        Console.WriteLine("🔗 SYNCHRONOUS (REST, gRPC):\n");

        Console.WriteLine("Request flow:");
        Console.WriteLine("  Service-A calls Service-B (blocking)");
        Console.WriteLine("  Wait for response");
        Console.WriteLine("  Return to caller\n");

        Console.WriteLine("Pros:");
        Console.WriteLine("  - Simple request-response");
        Console.WriteLine("  - Immediate errors detected");
        Console.WriteLine("  - No queuing complexity\n");

        Console.WriteLine("Cons:");
        Console.WriteLine("  - Tight coupling (know endpoint)");
        Console.WriteLine("  - Latency adds up (100-500ms per call)");
        Console.WriteLine("  - If Service-B down → Service-A blocked\n");

        Console.WriteLine("Use: Quick operations (user lookup, payment)\n");
    }

    private static void AsynchronousCommunication()
    {
        Console.WriteLine("📨 ASYNCHRONOUS (Message Queues):\n");

        Console.WriteLine("Request flow:");
        Console.WriteLine("  Service-A publishes message to queue");
        Console.WriteLine("  Returns immediately");
        Console.WriteLine("  Service-B consumes from queue");
        Console.WriteLine("  Processes at own pace\n");

        Console.WriteLine("Pros:");
        Console.WriteLine("  - Decoupled (don't know Service-B details)");
        Console.WriteLine("  - Scalable (queue buffers spikes)");
        Console.WriteLine("  - Resilient (Service-B down? Message waits)\n");

        Console.WriteLine("Cons:");
        Console.WriteLine("  - Eventual consistency (lag)");
        Console.WriteLine("  - Error handling complex");
        Console.WriteLine("  - Debugging harder (async tracing)\n");

        Console.WriteLine("Use: Reports, emails, analytics, non-critical updates\n");
    }

    private static void MixedApproach()
    {
        Console.WriteLine("🔀 MIXED APPROACH (Best of both):\n");

        Console.WriteLine("E-commerce checkout:");
        Console.WriteLine("  1. Payment check (SYNC REST)");
        Console.WriteLine("     → must succeed immediately");
        Console.WriteLine("     → returns status to user\n");

        Console.WriteLine("  2. Inventory update (ASYNC queue)");
        Console.WriteLine("     → eventual consistency OK");
        Console.WriteLine("     → email/shipment happens in background\n");

        Console.WriteLine("  3. Analytics tracking (ASYNC queue)");
        Console.WriteLine("     → fire-and-forget");
        Console.WriteLine("     → doesn't affect user experience\n");

        Console.WriteLine("Rule of thumb:");
        Console.WriteLine("  If user waits → SYNC (payment, login)");
        Console.WriteLine("  If background → ASYNC (email, reports)\n");
    }

    private static void MonolithContext()
    {
        Console.WriteLine("🏢 MONOLITH COMMUNICATION (for comparison):\n");

        Console.WriteLine("In monolithic architecture:");
        Console.WriteLine("  Everything runs in same process");
        Console.WriteLine("  Method calls: <1ms (in-memory)");
        Console.WriteLine("  Direct database access: 50-100ms\n");

        Console.WriteLine("Example (monolith): Checkout process");
        Console.WriteLine("  OrderService.CreateOrder() → calls");
        Console.WriteLine("  PaymentService.ChargeCard() → calls");
        Console.WriteLine("  InventoryService.ReserveItems() → calls");
        Console.WriteLine("  All in same process, same transaction\n");

        Console.WriteLine("Tradeoff: Fast but tightly coupled");
        Console.WriteLine("  One module refactored → others affected");
        Console.WriteLine("  All features deploy together");
        Console.WriteLine("  One bug affects everything\n");

        Console.WriteLine("Why microservices need async:");
        Console.WriteLine("  Monolith latency: 150ms (1ms + 50ms + 100ms)");
        Console.WriteLine("  Microservices sync latency: 600ms+ (network overhead)");
        Console.WriteLine("  Microservices async: Same 150ms work, but decoupled\n");
    }

    private static void DetailedComparison()
    {
        Console.WriteLine("⚡ PERFORMANCE COMPARISON:\n");

        Console.WriteLine("Scenario: 1M users per day, 10 requests each = 10M requests\n");

        Console.WriteLine("REST (Sync) latency:");
        Console.WriteLine("  - Service call: 100-500ms");
        Console.WriteLine("  - Database: 50ms");
        Console.WriteLine("  - Total: 150-550ms per request");
        Console.WriteLine("  - Queue time: 5-10 seconds (congestion)\n");

        Console.WriteLine("Message Queue (Async) latency:");
        Console.WriteLine("  - Enqueue: 1-5ms");
        Console.WriteLine("  - Consumer picks up: <100ms");
        Console.WriteLine("  - Consumer process: 50-200ms");
        Console.WriteLine("  - Total end-to-end: <500ms (much less queue buildup)\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");

        Console.WriteLine("1. IDENTIFY CRITICAL PATH");
        Console.WriteLine("   Sync for user-blocking operations");
        Console.WriteLine("   Async for background work\n");

        Console.WriteLine("2. CIRCUIT BREAKER FOR SYNC");
        Console.WriteLine("   Service-B down 3x? Stop calling");
        Console.WriteLine("   Retry after 30s");
        Console.WriteLine("   Return fallback (cache, default)\n");

        Console.WriteLine("3. DEAD LETTER QUEUE FOR ASYNC");
        Console.WriteLine("   Consumer fails repeatedly? Move to DLQ");
        Console.WriteLine("   Alert team for manual investigation\n");

        Console.WriteLine("4. TIMEOUT PATTERNS");
        Console.WriteLine("   Don't wait forever (REST timeout: 5-30s)");
        Console.WriteLine("   Too short: premature failures");
        Console.WriteLine("   Too long: cascading delays\n");

        Console.WriteLine("5. MONITOR QUEUE DEPTH");
        Console.WriteLine("   If growing unbounded: add consumers");
        Console.WriteLine("   If empty: reduce consumer instances\n");

        Console.WriteLine("6. VERSIONING & COMPATIBILITY");
        Console.WriteLine("   REST: Support v1 + v2 simultaneously");
        Console.WriteLine("   Queue: Use schema registry or content-based routing\n");

        Console.WriteLine("7. TRACING ACROSS SERVICE BOUNDARIES");
        Console.WriteLine("   Include correlation ID in every call");
        Console.WriteLine("   Track request through entire system");
        Console.WriteLine("   Essential for debugging microservices\n");
    }
}
