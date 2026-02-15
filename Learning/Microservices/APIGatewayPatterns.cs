// ==============================================================================
// API Gateway Patterns and Request Routing
// ==============================================================================
// WHAT IS THIS?
// An API Gateway is a single entry point for all client requests. It routes requests to appropriate backend services, handles cross-cutting concerns (auth, logging, rate limiting), and provides a unified interface.
//
// WHY IT MATTERS
// ✅ SINGLE ENTRY POINT: Clients don't know backend service locations | ✅ CROSS-CUTTING CONCERNS: Auth, logging, rate limiting in one place | ✅ PROTOCOL TRANSLATION: REST frontend, gRPC backend | ✅ AGGREGATION: Combine data from multiple services | ✅ API VERSION MANAGEMENT: Route v1 and v2 to different backends
//
// WHEN TO USE
// ✅ Microservices architecture | ✅ Multiple client types | ✅ Need authentication enforced centrally | ✅ Complex routing rules
//
// WHEN NOT TO USE
// ❌ Monolithic application | ❌ Simple single-service API
//
// REAL-WORLD EXAMPLE
// Netflix API Gateway: 1000s of client requests incoming. Gateway routes /movies → movie-service, /users → user-service, /watching → viewing-history-service. Enforces auth, tracks quotas, logs. Clients only know gateway URL.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Microservices;

public class APIGatewayPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  API Gateway Patterns and Request Routing");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");
        
        Overview();
        RequestRoutingFlow();
        CrossCuttingConcerns();
        AggregationPattern();
        CustomizationPerClient();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Without Gateway (problematic):");
        Console.WriteLine("  Client → Service-A: 10.0.1.1:8080");
        Console.WriteLine("  Client → Service-B: 10.0.2.2:8080");
        Console.WriteLine("  Each client knows all service locations\n");
        
        Console.WriteLine("With Gateway (clean):");
        Console.WriteLine("  Client → API Gateway: api.example.com");
        Console.WriteLine("  Gateway routes internally");
        Console.WriteLine("  Client unaware of backend topology\n");
    }

    private static void RequestRoutingFlow()
    {
        Console.WriteLine("🔀 REQUEST ROUTING FLOW:\n");
        
        Console.WriteLine("Client request: GET /api/products?category=electronics");
        Console.WriteLine("  ↓");
        Console.WriteLine("Gateway receives (api.example.com/api/products?...)\n");
        
        Console.WriteLine("1. Path matching:");
        Console.WriteLine("   /api/products → route to product-service");
        Console.WriteLine("   /api/users → route to user-service");
        Console.WriteLine("   /api/orders → route to order-service\n");
        
        Console.WriteLine("2. Routing logic:");
        Console.WriteLine("   Route rule: /api/products* → http://product-service:8080\n");
        
        Console.WriteLine("3. Request transformation:");
        Console.WriteLine("   Strip /api prefix");
        Console.WriteLine("   Forward: GET /products?category=electronics\n");
        
        Console.WriteLine("4. Service handles:");
        Console.WriteLine("   Product-Service returns JSON\n");
        
        Console.WriteLine("5. Response forwarding:");
        Console.WriteLine("   Gateway returns response to client\n");
    }

    private static void CrossCuttingConcerns()
    {
        Console.WriteLine("🔐 CROSS-CUTTING CONCERNS:\n");
        
        Console.WriteLine("Authentication:");
        Console.WriteLine("  Check JWT token before routing");
        Console.WriteLine("  Invalid? Return 401 Unauthorized\n");
        
        Console.WriteLine("Rate Limiting:");
        Console.WriteLine("  User 123 limit: 100 req/min");
        Console.WriteLine("  Track counters");
        Console.WriteLine("  Reject if > limit\n");
        
        Console.WriteLine("Logging:");
        Console.WriteLine("  Log: timestamp, client, path, service, response time\n");
        
        Console.WriteLine("Caching:");
        Console.WriteLine("  GET /products → cache 5 min");
        Console.WriteLine("  Same request from client → return cached\n");
    }

    private static void AggregationPattern()
    {
        Console.WriteLine("🔗 AGGREGATION PATTERN:\n");
        
        Console.WriteLine("Client: GET /api/product-details/123");
        Console.WriteLine("  Needs: product info + reviews + recommendations\n");
        
        Console.WriteLine("Without aggregation (client does 3 requests):");
        Console.WriteLine("  GET /products/123");
        Console.WriteLine("  GET /reviews?productId=123");
        Console.WriteLine("  GET /recommendations?productId=123\n");
        
        Console.WriteLine("With aggregation (gateway does it):");
        Console.WriteLine("  Gateway receives single request");
        Console.WriteLine("  Gateway calls 3 services in parallel");
        Console.WriteLine("  Combines: { product, reviews, recommendations }");
        Console.WriteLine("  Returns single response\n");
        
        Console.WriteLine("Benefit: Single request, better UX\n");
    }

    private static void CustomizationPerClient()
    {
        Console.WriteLine("📱 CUSTOMIZATION PER CLIENT:\n");
        
        Console.WriteLine("Mobile client:");
        Console.WriteLine("  GET /product/123 → minimal (id, name, price)\n");
        
        Console.WriteLine("Web client:");
        Console.WriteLine("  GET /product/123 → full (id, name, price, description, images)\n");
        
        Console.WriteLine("Backend returns same. Gateway customizes:");
        Console.WriteLine("  If User-Agent: mobile → strip images,description");
        Console.WriteLine("  If User-Agent: web → include everything\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        Console.WriteLine("1. DESIGN FOR FAILURE");
        Console.WriteLine("   Service timeout? Return cached or partial response\n");
        
        Console.WriteLine("2. VERSIONING");
        Console.WriteLine("   /api/v1/products → old service");
        Console.WriteLine("   /api/v2/products → new service\n");
        
        Console.WriteLine("3. MONITORING");
        Console.WriteLine("   Track: response time, error rate, backend latency\n");
        
        Console.WriteLine("4. TESTING");
        Console.WriteLine("   Mock backend services");
        Console.WriteLine("   Test routing rules\n");
    }
}
