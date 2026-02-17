// ==============================================================================
// Azure Event Grid Patterns
// ==============================================================================
// WHAT IS THIS?
// Azure Event Grid is an event routing service for reactive integrations across
// Azure services, custom applications, and webhooks.
//
// WHY IT MATTERS
// - Push-based event delivery with rich filtering
// - Native integration with Azure resource events
// - Simplifies fan-out automation without polling
//
// WHEN TO USE
// - Resource lifecycle notifications (blob created, subscription events)
// - Lightweight event notifications to multiple subscribers
// - Triggering Functions/Logic Apps/webhooks
//
// WHEN NOT TO USE
// - Heavy command processing with strict ordering and transactions
// - Very high-volume telemetry ingestion (prefer Event Hubs)
//
// REAL-WORLD EXAMPLE
// Blob-created events route through Event Grid to trigger virus scan,
// thumbnail generation, and metadata indexing functions.
// ==============================================================================

namespace RevisionNotesDemo.Cloud;

public class AzureEventGridPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n====================================================");
        Console.WriteLine("  Azure Event Grid Patterns");
        Console.WriteLine("====================================================\n");

        Overview();
        RoutingAndFilters();
        ReliabilityNotes();
    }

    private static void Overview()
    {
        Console.WriteLine("Overview:");
        Console.WriteLine("- Event Grid routes events from producers to many handlers");
        Console.WriteLine("- Ideal for reactive notification-style integration\n");
    }

    private static void RoutingAndFilters()
    {
        Console.WriteLine("Routing and Filters:");
        Console.WriteLine("- Subject and event-type filters reduce subscriber load");
        Console.WriteLine("- Advanced filters support property-based matching");
        Console.WriteLine("- Dead-letter destinations capture failed deliveries\n");
    }

    private static void ReliabilityNotes()
    {
        Console.WriteLine("Reliability Notes:");
        Console.WriteLine("- Verify webhook endpoints and retries");
        Console.WriteLine("- Make handlers idempotent for duplicate deliveries");
        Console.WriteLine("- Keep handlers short; delegate heavy work to queues");
        Console.WriteLine("- Monitor delivery failures and dead-letter counts\n");
    }
}
