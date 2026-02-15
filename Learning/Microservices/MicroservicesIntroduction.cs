// ==============================================================================
// Microservices Introduction: Core Principles and Patterns
// ==============================================================================
// WHAT IS THIS?
// An introduction to microservices architecture, covering core principles, distributed system challenges, common patterns, anti-patterns, and decision questions for adoption.
//
// WHY IT MATTERS
// ✅ PRINCIPLES: Single responsibility, autonomous, fault isolation, business capability | ✅ CHALLENGES: Network latency, consistency, debugging complexity | ✅ PATTERNS: Circuit breaker, retry, timeout | ✅ ANTI-PATTERNS: Chatty services, shared databases | ✅ GOVERNANCE: Standards, monitoring, contract testing
//
// WHEN TO USE
// ✅ Microservices: Multiple delivery teams | ✅ Scale different features differently | ✅ Technology diversity required | ✅ Multi-region deployments | ✅ Independent release cadence
//
// WHEN NOT TO USE
// ❌ Team <10 developers (overhead too high) | ❌ Simple CRUD domain | ❌ Real-time consistency required | ❌ Latency <1ms critical
//
// REAL-WORLD EXAMPLE
// Uber: 600+ services, 1000+ engineers (microservices essential). Stripe: Still mostly monolith with few extracted services (works for their model). Amazon: Everything microservices (P2P mandate in 2002).
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Microservices;

public class MicroservicesIntroduction
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Microservices: Core Principles and Patterns");
        Console.WriteLine("╚════════════════════════════════════════════════════════╝\n");
        
        Overview();
        CorePrinciples();
        DistributedSystemChallenges();
        CommonPatterns();
        CommonAntiPatterns();
        GovernanceAndGoals();
        DecisionQuestions();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Microservices = Independent services, deployed separately, owned by small teams\n");
        Console.WriteLine("Definition (Sam Newman):");
        Console.WriteLine("  'The microservice architectural style is an approach to");
        Console.WriteLine("  developing a single application as a suite of small");
        Console.WriteLine("  services, each running in its own process and communicating");
        Console.WriteLine("  with lightweight mechanisms.'\n");
        
        Console.WriteLine("Three critical aspects:");
        Console.WriteLine("  1. Small, focused scope (single responsibility)");
        Console.WriteLine("  2. Independent deployment (not coupled to other services)");
        Console.WriteLine("  3. Team ownership (small team responsible end-to-end)\n");
    }

    private static void CorePrinciples()
    {
        Console.WriteLine("🎯 CORE PRINCIPLES:\n");
        
        Console.WriteLine("1. SINGLE RESPONSIBILITY PRINCIPLE");
        Console.WriteLine("   Service = one reason to change");
        Console.WriteLine("   OrderService: Only order logic");
        Console.WriteLine("   PaymentService: Only payment logic");
        Console.WriteLine("   When: Order reason to change ≠ Payment reason to change\n");
        
        Console.WriteLine("2. AUTONOMOUS SERVICES");
        Console.WriteLine("   Service changes don't require coordinating other services");
        Console.WriteLine("   PaymentService v2 deploy ≠ OrderService needs update");
        Console.WriteLine("   Uses API versioning, contracts, backward compatibility\n");
        
        Console.WriteLine("3. FAULT ISOLATION");
        Console.WriteLine("   PaymentService down ≠ User can't browse Products");
        Console.WriteLine("   Implement circuit breakers, fallbacks, timeouts");
        Console.WriteLine("   Service failure contained, doesn't cascade\n");
        
        Console.WriteLine("4. BUSINESS CAPABILITY ALIGNMENT");
        Console.WriteLine("   Services map to business domains");
        Console.WriteLine("   Product Team → ProductService");
        Console.WriteLine("   Order Team → OrderService");
        Console.WriteLine("   Prevents Conway's Law (org structure = system structure)\n");
        
        Console.WriteLine("5. DECENTRALIZED DATA MANAGEMENT");
        Console.WriteLine("   Each service owns its data");
        Console.WriteLine("   OrderService DB ≠ ProductService DB");
        Console.WriteLine("   Eliminates single-database bottleneck\n");
        
        Console.WriteLine("6. INDEPENDENT DEPLOYMENT");
        Console.WriteLine("   Deploy one service without deploying others");
        Console.WriteLine("   10x daily deployments possible");
        Console.WriteLine("   Fast feedback loops\n");
    }

    private static void DistributedSystemChallenges()
    {
        Console.WriteLine("⚠️ DISTRIBUTED SYSTEM CHALLENGES:\n");
        
        Console.WriteLine("1. NETWORK LATENCY");
        Console.WriteLine("   In-process call: <1ms");
        Console.WriteLine("   Over network: 10-100ms");
        Console.WriteLine("   Chain 5 services: 50-500ms latency");
        Console.WriteLine("   Solution: Service composition, caching, async patterns\n");
        
        Console.WriteLine("2. PARTIAL FAILURES");
        Console.WriteLine("   What if PaymentService responds slowly?");
        Console.WriteLine("   What if OrderService crashes mid-request?");
        Console.WriteLine("   Solution: Timeouts, circuit breakers, fallbacks\n");
        
        Console.WriteLine("3. CONSISTENCY CHALLENGES");
        Console.WriteLine("   Monolith: One transaction spans all operations");
        Console.WriteLine("   Microservices: Each service in separate transaction");
        Console.WriteLine("   Order created but Payment fails → handle in Saga");
        Console.WriteLine("   Solution: Eventual consistency, sagas, compensating transactions\n");
        
        Console.WriteLine("4. DEBUGGING COMPLEXITY");
        Console.WriteLine("   Bug manifests in OrderService");
        Console.WriteLine("   Root cause in ProductService response");
        Console.WriteLine("   Need distributed tracing across services");
        Console.WriteLine("   Solution: Correlation IDs, structured logging, APM tools\n");
        
        Console.WriteLine("5. DATA CONSISTENCY ACROSS SERVICES");
        Console.WriteLine("   Join data from 3 services in single query");
        Console.WriteLine("   What if service returns stale data?");
        Console.WriteLine("   Solution: CQRS (read models), event sourcing\n");
        
        Console.WriteLine("6. TESTING COMPLEXITY");
        Console.WriteLine("   Unit test: 1 service (fast)");
        Console.WriteLine("   Integration test: 5 services (slow, flaky)");
        Console.WriteLine("   Solution: Consumer-driven contract tests, mock external services\n");
    }

    private static void CommonPatterns()
    {
        Console.WriteLine("✅ COMMON PATTERNS:\n");
        
        Console.WriteLine("CIRCUIT BREAKER");
        Console.WriteLine("  Problem: If PaymentService down, call it 5000 times/sec?");
        Console.WriteLine("  Solution: Detect failure, fail fast, return fallback");
        Console.WriteLine("  States: Closed (normal) → Open (fail fast) → Half-open (retry)\n");
        
        Console.WriteLine("RETRY WITH EXPONENTIAL BACKOFF");
        Console.WriteLine("  Problem: Temporary network blip causes failure");
        Console.WriteLine("  Solution: Retry with increasing delays");
        Console.WriteLine("  Attempt 1: immediate");
        Console.WriteLine("  Attempt 2: wait 100ms");
        Console.WriteLine("  Attempt 3: wait 1000ms\n");
        
        Console.WriteLine("TIMEOUT");
        Console.WriteLine("  Problem: Wait forever for slow service response?");
        Console.WriteLine("  Solution: Abort after N seconds");
        Console.WriteLine("  Example: Wait max 5 seconds, then fail gracefully\n");
        
        Console.WriteLine("BULKHEAD PATTERN");
        Console.WriteLine("  Problem: One service consumes all connection pool threads");
        Console.WriteLine("  Solution: Separate thread pools per downstream service");
        Console.WriteLine("  10 threads → Payment, 10 threads → Product, 10 threads → Shipping\n");
        
        Console.WriteLine("API GATEWAY");
        Console.WriteLine("  Single entry point for all clients");
        Console.WriteLine("  Handles cross-cutting concerns:");
        Console.WriteLine("    - Authentication (JWT, OAuth)");
        Console.WriteLine("    - Rate limiting");
        Console.WriteLine("    - Request routing");
        Console.WriteLine("    - Response aggregation\n");
        
        Console.WriteLine("SAGA PATTERN");
        Console.WriteLine("  Problem: Distributed transaction (Order + Payment + Shipping)");
        Console.WriteLine("  Solution: Multi-step workflow with compensation");
        Console.WriteLine("  If Payment fails → OrderService compensates (cancel order)\n");
        
        Console.WriteLine("EVENT SOURCING");
        Console.WriteLine("  Store state as immutable events");
        Console.WriteLine("  Event: OrderCreatedEvent, PaymentProcessedEvent");
        Console.WriteLine("  Rebuild current state from event stream\n");
    }

    private static void CommonAntiPatterns()
    {
        Console.WriteLine("❌ COMMON ANTI-PATTERNS:\n");
        
        Console.WriteLine("CHATTY SERVICES");
        Console.WriteLine("  ❌ ServiceA calls ServiceB, which calls ServiceC");
        Console.WriteLine("  ❌ ServiceC calls ServiceD, etc. (chain of 10 calls)");
        Console.WriteLine("  Problem: Latency sums up, cascading failures");
        Console.WriteLine("  Fix: Combine into single API call (API gateway pattern)\n");
        
        Console.WriteLine("SHARED DATABASE");
        Console.WriteLine("  ❌ OrderService and ProductService share same DB");
        Console.WriteLine("  Problem: Tight coupling, cannot scale independently");
        Console.WriteLine("  Fix: Each service owns its own database\n");
        
        Console.WriteLine("LACK OF MONITORING");
        Console.WriteLine("  ❌ Deploy microservices without observability");
        Console.WriteLine("  Problem: Can't see which service is failing");
        Console.WriteLine("  Fix: Implement distributed tracing, structured logs, metrics\n");
        
        Console.WriteLine("SYNCHRONOUS EVERYTHING");
        Console.WriteLine("  ❌ All service calls are REST request-response");
        Console.WriteLine("  Problem: Cascading failures, tight coupling");
        Console.WriteLine("  Fix: Use async patterns (events, queues) for non-critical paths\n");
        
        Console.WriteLine("DISTRIBUTED MONOLITH");
        Console.WriteLine("  ❌ Microservices that can't deploy independently");
        Console.WriteLine("  ❌ Must always deploy ServiceA + ServiceB together");
        Console.WriteLine("  Problem: Worst of both worlds (complexity + coupling)");
        Console.WriteLine("  Fix: True independent deployment\n");
    }

    private static void GovernanceAndGoals()
    {
        Console.WriteLine("🏛️ GOVERNANCE AND GOALS:\n");
        
        Console.WriteLine("ORGANIZATIONAL:");
        Console.WriteLine("  ✅ Small teams (5-9 people) own each service");
        Console.WriteLine("  ✅ Team structure matches service structure");
        Console.WriteLine("  ✅ Autonomous decisions (which database, language)\n");
        
        Console.WriteLine("TECHNICAL STANDARDS:");
        Console.WriteLine("  ✅ All services implement health checks");
        Console.WriteLine("  ✅ All services emit structured logs");
        Console.WriteLine("  ✅ All services use consistent authentication");
        Console.WriteLine("  ✅ Version APIs (maintain backward compatibility)\n");
        
        Console.WriteLine("DEPLOYMENT STANDARDS:");
        Console.WriteLine("  ✅ Every service has CI/CD pipeline");
        Console.WriteLine("  ✅ Deploy without coordinating other services");
        Console.WriteLine("  ✅ Automated rollback capability\n");
        
        Console.WriteLine("OBSERVABILITY STANDARDS:");
        Console.WriteLine("  ✅ Distributed tracing (correlation IDs)");
        Console.WriteLine("  ✅ Metrics (response times, error rates)");
        Console.WriteLine("  ✅ Logging (structured, searchable)\n");
        
        Console.WriteLine("KEY METRICS:");
        Console.WriteLine("  ✅ Deployment frequency (per service)");
        Console.WriteLine("  ✅ Lead time for changes");
        Console.WriteLine("  ✅ Mean time to recovery (MTTR)");
        Console.WriteLine("  ✅ Change failure rate\n");
    }

    private static void DecisionQuestions()
    {
        Console.WriteLine("❓ DECISION QUESTIONS:\n");
        
        Console.WriteLine("1. TEAM SIZE & STRUCTURE");
        Console.WriteLine("   Q: Do we have 50+ engineers?");
        Console.WriteLine("   Q: Can we organize teams around business capabilities?");
        Console.WriteLine("   → Small teams can't manage distributed complexity\n");
        
        Console.WriteLine("2. SCALING REQUIREMENTS");
        Console.WriteLine("   Q: Do different features need to scale independently?");
        Console.WriteLine("   Q: Is 1 server per feature enough?");
        Console.WriteLine("   → If not, monolith scales everything equally\n");
        
        Console.WriteLine("3. DEPLOYMENT INDEPENDENCE");
        Console.WriteLine("   Q: How often do we need to deploy independently?");
        Console.WriteLine("   Q: Can we coordinate deployments (weekly, monthly)?");
        Console.WriteLine("   → Monolith better if coordinated deployments OK\n");
        
        Console.WriteLine("4. TECHNOLOGY DIVERSITY");
        Console.WriteLine("   Q: Must we use different tech stacks?");
        Console.WriteLine("   Q: Is forced tech unification a constraint?");
        Console.WriteLine("   → Monolith can use diverse tools in one language\n");
        
        Console.WriteLine("5. COMPLEXITY TOLERANCE");
        Console.WriteLine("   Q: Can we operate distributed systems?");
        Console.WriteLine("   Q: Can we implement observability infrastructure?");
        Console.WriteLine("   Q: Can we handle eventual consistency?\n");
        
        Console.WriteLine("6. DATA CONSISTENCY REQUIREMENTS");
        Console.WriteLine("   Q: Do we need strict ACID across features?");
        Console.WriteLine("   Q: Can we accept eventual consistency?");
        Console.WriteLine("   → If ACID required, monolith easier\n");
        
        Console.WriteLine("\n✅ ANSWER CHECKLIST:");
        Console.WriteLine("   ☑ Team >50 engineers");
        Console.WriteLine("   ☑ Independent scaling needed");
        Console.WriteLine("   ☑ Different teams own different features");
        Console.WriteLine("   ☑ Multi-region deployments");
        Console.WriteLine("   ☑ Technology diversity required");
        Console.WriteLine("   ☑ Can handle distributed complexity");
        Console.WriteLine("   ☑ Can invest in observability");
        Console.WriteLine("   → If 5+ checks, consider microservices\n");
    }
}
