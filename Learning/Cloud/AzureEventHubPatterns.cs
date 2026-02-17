// ==============================================================================
// Azure Event Hubs Patterns
// ==============================================================================
// WHAT IS THIS?
// Azure Event Hubs is a high-throughput event ingestion platform for telemetry
// and streaming pipelines.
//
// WHY IT MATTERS
// - Handles millions of events per second with partitioned throughput
// - Enables real-time analytics and downstream stream processing
// - Supports retention and replay for troubleshooting and backfill
//
// WHEN TO USE
// - Application telemetry, clickstreams, IoT, device events
// - Large append-only event streams consumed by many processors
//
// WHEN NOT TO USE
// - Per-message business workflows needing rich routing and DLQ semantics
// - Simple low-rate business notifications
//
// REAL-WORLD EXAMPLE
// Ingest platform telemetry into Event Hubs, process with stream jobs, and load
// aggregates into analytics stores for near-real-time dashboards.
// ==============================================================================

namespace RevisionNotesDemo.Cloud;

public class AzureEventHubPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n====================================================");
        Console.WriteLine("  Azure Event Hubs Patterns");
        Console.WriteLine("====================================================\n");

        Overview();
        ThroughputModel();
        ConsumerGuidance();
    }

    private static void Overview()
    {
        Console.WriteLine("Overview:");
        Console.WriteLine("- Event Hubs is optimized for high-ingest append-only streams");
        Console.WriteLine("- Scale is partition-based, not queue-worker based\n");
    }

    private static void ThroughputModel()
    {
        Console.WriteLine("Throughput Model:");
        Console.WriteLine("- Partitions spread load and preserve per-partition ordering");
        Console.WriteLine("- Consumer groups allow independent processing pipelines");
        Console.WriteLine("- Retention enables replay from offset/checkpoint\n");
    }

    private static void ConsumerGuidance()
    {
        Console.WriteLine("Consumer Guidance:");
        Console.WriteLine("- Pick partition keys to avoid hot partitions");
        Console.WriteLine("- Checkpoint after durable downstream commit");
        Console.WriteLine("- Design for at-least-once processing and duplicates");
        Console.WriteLine("- Track lag and processing delay as primary SLOs\n");
    }
}
