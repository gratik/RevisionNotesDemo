// ==============================================================================
// Background Workers in .NET
// ==============================================================================
// WHAT IS THIS?
// Background workers are long-running hosted services in .NET that execute work
// outside request-response paths.
//
// WHY IT MATTERS
// - Moves heavy work off API request threads
// - Supports recurring jobs and queue-driven processing
// - Enables controlled shutdown through cancellation tokens
//
// WHEN TO USE
// - Polling integrations, cleanup jobs, and back-office processing
// - Consuming messages from brokers (Service Bus/RabbitMQ/Kafka)
//
// WHEN NOT TO USE
// - Immediate user-facing operations requiring synchronous response
// - Complex cron/orchestration needs better served by dedicated schedulers
//
// REAL-WORLD EXAMPLE
// An order API enqueues invoice generation and email notifications to a worker
// queue, returning HTTP 202 immediately.
// ==============================================================================

namespace RevisionNotesDemo.PracticalPatterns;

public class BackgroundWorkersInDotNet
{
    public static void RunAll()
    {
        Console.WriteLine("\n====================================================");
        Console.WriteLine("  Background Workers in .NET");
        Console.WriteLine("====================================================\n");

        WorkerTypes();
        LifecycleGuidance();
        ProductionChecklist();
    }

    private static void WorkerTypes()
    {
        Console.WriteLine("Worker Types:");
        Console.WriteLine("- IHostedService for explicit startup/shutdown behavior");
        Console.WriteLine("- BackgroundService for long-running ExecuteAsync loops");
        Console.WriteLine("- Queued worker pattern for bounded background execution\n");
    }

    private static void LifecycleGuidance()
    {
        Console.WriteLine("Lifecycle Guidance:");
        Console.WriteLine("- Always honor CancellationToken");
        Console.WriteLine("- Use IServiceScopeFactory for scoped dependencies");
        Console.WriteLine("- Apply retry with backoff for transient faults");
        Console.WriteLine("- Keep each iteration short and observable\n");
    }

    private static void ProductionChecklist()
    {
        Console.WriteLine("Production Checklist:");
        Console.WriteLine("- Idempotent handlers and replay-safe operations");
        Console.WriteLine("- Backpressure strategy (queue bounds, throttling)");
        Console.WriteLine("- Health probes and operational metrics");
        Console.WriteLine("- Graceful drain during deployment shutdown\n");
    }
}
