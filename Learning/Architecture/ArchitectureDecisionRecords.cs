// ==============================================================================
// Architecture Decision Records (ADR)
// ==============================================================================
// WHAT IS THIS?
// ADR is a structured format for documenting decisions: what was decided, why, consequences, and alternatives considered. Creates decision history, aids onboarding, prevents recurring discussions.
//
// WHY IT MATTERS
// ✅ DECISION HISTORY: Why did we choose RabbitMQ over Kafka? Document once | ✅ KNOWLEDGE TRANSFER: New devs understand rationale, not just implementation | ✅ PREVENTS ARGUMENTS: "We discussed this in 2023, decision was X" | ✅ TRACEABILITY: Reason for each architectural choice | ✅ REVERSIBLE: If context changes, can revisit decision
//
// WHEN TO USE
// ✅ Significant technical decisions (database choice, messaging system) | ✅ Trade-offs between options | ✅ Team decisions affecting multiple services | ✅ Long-lived systems needing continuity
//
// WHEN NOT TO USE
// ❌ Trivial decisions (variable naming) | ❌ Decisions that won't be questioned later
//
// REAL-WORLD EXAMPLE
// Team debates: Message queue for events, REST polling, or WebSockets? ADR-5: "Selected RabbitMQ because of...". 6 months later, new dev sees "why RabbitMQ" without asking, understands tradeoffs.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Architecture;

public class ArchitectureDecisionRecords
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Architecture Decision Records (ADR)");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");
        
        Overview();
        ADRTemplate();
        ExampleADR();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("ADR format: Status | Title | Context | Decision | Consequences\n");
        Console.WriteLine("Status: Proposed, Accepted, Deprecated, Superseded\n");
    }

    private static void ADRTemplate()
    {
        Console.WriteLine("📋 ADR TEMPLATE:\n");
        
        Console.WriteLine("# ADR-001: Choose Database for User Data");
        Console.WriteLine("## Status");
        Console.WriteLine("Accepted\n");
        
        Console.WriteLine("## Context");
        Console.WriteLine("User data: millions of records, frequent reads, occasional writes");
        Console.WriteLine("Constraints: <100ms query, 99.9% uptime, cost-effective\n");
        
        Console.WriteLine("## Decision");
        Console.WriteLine("We will use PostgreSQL (not MongoDB or MySQL) because:");
        Console.WriteLine("  1. ACID transactions for consistency");
        Console.WriteLine("  2. Proven at scale (Uber, Instagram)");
        Console.WriteLine("  3. Familiar to existing team\n");
        
        Console.WriteLine("## Consequences");
        Console.WriteLine("Positive:");
        Console.WriteLine("  - Data consistency guaranteed");
        Console.WriteLine("  - Strong querying with JOINs");
        Console.WriteLine("Negative:");
        Console.WriteLine("  - Vertical scaling limits (max ~2TB single instance)");
        Console.WriteLine("  - Horizontal scaling via sharding complex\n");
        
        Console.WriteLine("## Alternatives Considered");
        Console.WriteLine("  - MongoDB: Lost consistency, schema flexibility");
        Console.WriteLine("  - MySQL: Less reliable transactions");
        Console.WriteLine("  - BigQuery: Overkill for this dataset size\n");
    }

    private static void ExampleADR()
    {
        Console.WriteLine("💾 REAL EXAMPLE: MESSAGING SYSTEM CHOICE\n");
        
        Console.WriteLine("# ADR-012: Event Messaging System");
        Console.WriteLine("## Status: Accepted");
        Console.WriteLine("## Decision: RabbitMQ (not Kafka)\n");
        
        Console.WriteLine("Because:");
        Console.WriteLine("  ✅ Simple pub/sub routing");
        Console.WriteLine("  ✅ Lower latency for immediate subscribers");
        Console.WriteLine("  ✅ Team expertise exists");
        Console.WriteLine("  ❌ Limited history (Kafka retention > 1 year, RabbitMQ ~hours)\n");
        
        Console.WriteLine("If we later need year-long event replay → ADR-012 superseded by ADR-025\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        
        Console.WriteLine("1. ONE DECISION PER ADR");
        Console.WriteLine("   Don't combine multiple decisions\n");
        
        Console.WriteLine("2. CAPTURE CONTEXT");
        Console.WriteLine("   Decision is only understood in context");
        Console.WriteLine("   \"Why PostgreSQL?\" depends on scale, team, budget\n");
        
        Console.WriteLine("3. REVIEW WITH STAKEHOLDERS");
        Console.WriteLine("   Team discussion before finalizing\n");
        
        Console.WriteLine("4. STORE IN VERSION CONTROL");
        Console.WriteLine("   /docs/adr/ directory");
        Console.WriteLine("   Git history = decision history\n");
        
        Console.WriteLine("5. UPDATE STATUS WHEN SUPERSEDED");
        Console.WriteLine("   Don't delete, mark Superseded");
        Console.WriteLine("   Link to replacement ADR\n");
    }
}
