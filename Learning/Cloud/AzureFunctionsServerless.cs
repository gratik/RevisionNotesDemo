// ==============================================================================
// Azure Functions & Serverless Computing
// ==============================================================================
// WHAT IS THIS?
// Azure Functions enables event-driven serverless computing where you write 
// code triggered by events (HTTP requests, timers, queue messages, blob uploads)
// and Azure manages execution, scaling, and infrastructure automatically.
//
// WHY IT MATTERS
// ✅ PAY-PER-EXECUTION: Zero cost when code isn't running
// ✅ AUTO-SCALING: Handles 1 to 1M concurrent executions instantly
// ✅ EVENT-DRIVEN: React to Azure Storage, Service Bus, CosmosDB events
// ✅ ORCHESTRATION: Durable Functions coordinate complex workflows
// ✅ RAPID ITERATION: Deploy single function independently
//
// WHEN TO USE
// ✅ Event processors (file uploads, queue messages)
// ✅ Scheduled tasks (cleanup at 3am, reports at 6am)
// ✅ Webhooks and third-party integrations
// ✅ Microservice endpoints with variable traffic
// ✅ Backend for mobile/SPA applications
//
// WHEN NOT TO USE
// ❌ Always-on workloads (dedicated app service cheaper)
// ❌ Long-running operations (>10 minutes without Durable)
// ❌ Real-time requirements <10ms
//
// REAL-WORLD EXAMPLE
// Image upload workflow: User uploads photo → Blob Storage trigger → Function
// resizes image, generates thumbnail → Stores metadata in Cosmos DB. All
// completes in <2 seconds, cost only for processing time (not idle).
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Cloud;

public class AzureFunctionsServerless
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Azure Functions & Serverless Computing");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");
        
        Overview();
        TriggerTypes();
        DurableFunctions();
        CostBenefits();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Functions execute code in response to events without");
        Console.WriteLine("provisioning or maintaining servers. Perfect for event-");
        Console.WriteLine("driven, asynchronous workloads.\n");
    }

    private static void TriggerTypes()
    {
        Console.WriteLine("⚡ TRIGGER TYPES:\n");
        Console.WriteLine("  • HTTP: REST endpoints, webhooks");
        Console.WriteLine("  • Timer: Scheduled tasks (cron expressions)");
        Console.WriteLine("  • Blob Storage: File uploads, changes");
        Console.WriteLine("  • Queue: Process messages from Service Bus/Storage");
        Console.WriteLine("  • CosmosDB: React to document changes\n");
    }

    private static void DurableFunctions()
    {
        Console.WriteLine("🔄 DURABLE FUNCTIONS:\n");
        Console.WriteLine("  • Orchestration: Coordinate (Approvals → Payment → Notify)");
        Console.WriteLine("  • Human Interaction: Wait for approval, resume");
        Console.WriteLine("  • Error Handling: Automatic retry with backoff");
        Console.WriteLine("  • Stateful Workflows: Persist state across calls\n");
    }

    private static void CostBenefits()
    {
        Console.WriteLine("💰 COST BENEFITS:\n");
        Console.WriteLine("  • Consumption Plan: Pay per execution (free tier: 1M/month)");
        Console.WriteLine("  • Premium Plan: Reserved capacity, VNet integration");
        Console.WriteLine("  • ASP (Shared): Lowest cost for Always-On");
        Console.WriteLine("  • No Cost: During idle periods\n");
    }
}
