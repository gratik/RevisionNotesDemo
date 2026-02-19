// ==============================================================================
// Azure Service Bus Patterns
// ==============================================================================
// WHAT IS THIS?
// Azure Service Bus is a managed enterprise messaging broker for reliable
// asynchronous communication between distributed services.
//
// WHY IT MATTERS
// - Decouples producers from consumers with durable queues/topics
// - Supports retries, dead-lettering, sessions, and duplicate detection
// - Handles burst traffic without overwhelming downstream services
//
// WHEN TO USE
// - Commands and workflows that require guaranteed delivery
// - Integration between microservices and back-office systems
// - Long-running processing with retry and DLQ handling
//
// WHEN NOT TO USE
// - High-volume telemetry streams (prefer Event Hubs)
// - Lightweight webhook-style notifications (consider Event Grid)
//
// REAL-WORLD EXAMPLE
// Checkout service publishes OrderPlaced; inventory, billing, and email consumers
// process independently with retries and dead-letter safety.
// ==============================================================================

namespace RevisionNotesDemo.Cloud;

public class AzureServiceBusPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n====================================================");
        Console.WriteLine("  Azure Service Bus Patterns");
        Console.WriteLine("====================================================\n");

        Overview();
        CoreCapabilities();
        DesignGuidance();
    }

    private static void Overview()
    {
        Console.WriteLine("Overview:");
        Console.WriteLine("- Reliable brokered messaging for commands and events");
        Console.WriteLine("- Queue for point-to-point, topic/subscription for pub/sub\n");
    }

    private static void CoreCapabilities()
    {
        Console.WriteLine("Core Capabilities:");
        Console.WriteLine("- At-least-once delivery with retries");
        Console.WriteLine("- Dead-letter queue for poison messages");
        Console.WriteLine("- Sessions for ordered, correlated processing");
        Console.WriteLine("- Duplicate detection and scheduled delivery\n");
    }

    private static void DesignGuidance()
    {
        Console.WriteLine("Design Guidance:");
        Console.WriteLine("- Make handlers idempotent");
        Console.WriteLine("- Use correlation IDs across hops");
        Console.WriteLine("- Monitor queue depth, age, and DLQ growth");
        Console.WriteLine("- Separate command topics from event topics\n");
    }
}
