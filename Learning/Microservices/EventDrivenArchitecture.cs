// ==============================================================================
// Event-Driven Architecture Patterns
// ==============================================================================
// WHAT IS THIS?
// Event-driven architecture decouples services via events. When something happens (user registers), a service publishes event, and multiple subscribers react independently without knowing each other.
//
// WHY IT MATTERS
// ✅ LOOSE COUPLING: Services don't call each other directly | ✅ ASYNC: Subscribers process at own pace, no blocking | ✅ SCALABILITY: Add new subscribers without changing publisher | ✅ RESILIENCE: Publisher doesn't care if subscriber fails | ✅ FLEXIBILITY: Add/remove business logic without touching existing services
//
// WHEN TO USE
// ✅ Multiple services react to same event | ✅ Async processing acceptable | ✅ Need loose coupling | ✅ High throughput, many subscribers
//
// WHEN NOT TO USE
// ❌ Strict ordering required | ❌ Immediate confirmation needed | ❌ Simple request-response good enough
//
// REAL-WORLD EXAMPLE
// User registration: UserService publishes "UserRegistered" event. Email sends welcome email, Analytics tracks signup, Recommendation initializes profile, Permission grants trial access. All independent, all triggered by one event.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Microservices;

public class EventDrivenArchitecture
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Event-Driven Architecture Patterns");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");
        
        Overview();
        EventFlow();
        ChoreographyVsOrchestration();
        EventSourcingConcept();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Direct coupling (bad):");
        Console.WriteLine("  UserService → Email Service (direct call)");
        Console.WriteLine("  UserService → Analytics (direct call)");
        Console.WriteLine("  UserService → Permission Service (direct call)\n");
        
        Console.WriteLine("Event-driven (good):");
        Console.WriteLine("  UserService → publishes UserRegistered event");
        Console.WriteLine("  Email, Analytics, Permission subscribe independently\n");
    }

    private static void EventFlow()
    {
        Console.WriteLine("📤 EVENT FLOW: USER REGISTRATION EXAMPLE\n");
        
        Console.WriteLine("1. User registers:");
        Console.WriteLine("   POST /register → UserService");
        Console.WriteLine("   UserService saves to database");
        Console.WriteLine("   UserService publishes: UserRegistered { userId: 123, email: alice@example.com }\n");
        
        Console.WriteLine("2. Event published to broker (RabbitMQ/Kafka):");
        Console.WriteLine("   Topic: user.events");
        Console.WriteLine("   Message: { event: \"UserRegistered\", data: {...} }\n");
        
        Console.WriteLine("3. Subscribers receive independently:");
        Console.WriteLine("   Email Subscriber:");
        Console.WriteLine("     Receives UserRegistered");
        Console.WriteLine("     Sends welcome email");
        Console.WriteLine("     Publishes EmailSent\n");
        
        Console.WriteLine("   Analytics Subscriber:");
        Console.WriteLine("     Receives UserRegistered");
        Console.WriteLine("     Increments signup counter");
        Console.WriteLine("     Updates dashboard\n");
        
        Console.WriteLine("   Permission Subscriber:");
        Console.WriteLine("     Receives UserRegistered");
        Console.WriteLine("     Grants trial access for 30 days\n");
    }

    private static void ChoreographyVsOrchestration()
    {
        Console.WriteLine("💃 CHOREOGRAPHY vs ORCHESTRATION:\n");
        
        Console.WriteLine("CHOREOGRAPHY (services know each other):");
        Console.WriteLine("  UserService publishes UserRegistered");
        Console.WriteLine("  Email listens → publishes EmailSent");
        Console.WriteLine("  Permission listens → publishes PermissionGranted\n");
        
        Console.WriteLine("Pros: Simple, fast");
        Console.WriteLine("Cons: Tight coupling, hard to trace flow\n");
        
        Console.WriteLine("ORCHESTRATION (central coordinator):");
        Console.WriteLine("  UserService publishes UserRegistered");
        Console.WriteLine("  UserSaga listens → calls Email service (sync)");
        Console.WriteLine("  UserSaga listens → calls Permission service (sync)\n");
        
        Console.WriteLine("Pros: Clear flow, easy to understand");
        Console.WriteLine("Cons: Central point of failure, single orchestrator bottleneck\n");
    }

    private static void EventSourcingConcept()
    {
        Console.WriteLine("📝 EVENT SOURCING CONCEPT:\n");
        
        Console.WriteLine("Traditional: Store current state");
        Console.WriteLine("  user = { id: 123, email: alice@example.com, name: Alice }\n");
        
        Console.WriteLine("Event sourcing: Store all events");
        Console.WriteLine("  Event 1: UserCreated { id: 123, email: alice@example.com }");
        Console.WriteLine("  Event 2: UserRenamed { id: 123, name: Alice }");
        Console.WriteLine("  Event 3: UserEmailUpdated { id: 123, email: alice.smith@example.com }\n");
        
        Console.WriteLine("Current state = replay all events\n");
        
        Console.WriteLine("Benefits:");
        Console.WriteLine("  - Full audit trail");
        Console.WriteLine("  - Replay to any point in time");
        Console.WriteLine("  - Natural integration with events\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        
        Console.WriteLine("1. IDEMPOTENT SUBSCRIBERS");
        Console.WriteLine("   Event received twice → same result");
        Console.WriteLine("   Use event ID to deduplicate\n");
        
        Console.WriteLine("2. DEAD LETTER QUEUE");
        Console.WriteLine("   Subscriber fails? Move to DLQ for retry\n");
        
        Console.WriteLine("3. EVENT SCHEMA VERSIONING");
        Console.WriteLine("   Event v1: { userId, email }");
        Console.WriteLine("   Event v2: { userId, email, name }");
        Console.WriteLine("   Old subscribers handle missing fields\n");
        
        Console.WriteLine("4. MONITOR LAG");
        Console.WriteLine("   Track: time from event → subscriber processed");
        Console.WriteLine("   Alert if lag > threshold\n");
    }
}
