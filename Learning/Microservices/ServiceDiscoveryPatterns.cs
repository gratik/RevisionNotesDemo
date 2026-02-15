// ==============================================================================
// Service Discovery Patterns in Distributed Systems
// ==============================================================================
// WHAT IS THIS?
// Service discovery is the mechanism allowing services to find and communicate with each other without hardcoded IP addresses. As services scale (100s of instances), manual configuration becomes impossible.
//
// WHY IT MATTERS
// ✅ AUTO-REGISTRATION: Service registers when it starts | ✅ HEALTH CHECKS: Unhealthy instances removed automatically | ✅ LOAD BALANCING: Requests distributed across healthy instances | ✅ RESILIENCE: Service moved to new server? Automatically discovered | ✅ ELIMINATES HARDCODING: Servers added/removed dynamically
//
// WHEN TO USE
// ✅ Microservices with dynamic scaling | ✅ Container orchestration (Kubernetes) | ✅ Serverless with variable endpoints | ✅ Multi-region deployments
//
// WHEN NOT TO USE
// ❌ Monolithic architecture | ❌ Static server list
//
// REAL-WORLD EXAMPLE
// Netflix handles 200 million users across 100+ services. Services scale from 1 to 1000 instances per day. Service-A needs Product-Service: Asks, "where is Product-Service?" Consul returns 50 healthy IPs. Requests distributed via load balancing.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Microservices;

public class ServiceDiscoveryPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Service Discovery Patterns in Distributed Systems");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");
        
        Overview();
        ClientSideDiscovery();
        ServerSideDiscovery();
        HealthChecks();
        RealWorldExample();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Problem: Service-A (100 instances) needs to know Service-B's IPs");
        Console.WriteLine("  Hardcode IPs? Breaks when servers change");
        Console.WriteLine("  Solution: Service Registry (Consul, Eureka, etcd)\n");
    }

    private static void ClientSideDiscovery()
    {
        Console.WriteLine("🔍 CLIENT-SIDE DISCOVERY:\n");
        Console.WriteLine("Service-A logic:");
        Console.WriteLine("  1. Query service registry: GET /services/product-service");
        Console.WriteLine("  2. Registry returns: [\"10.0.1.5:8080\", \"10.0.2.3:8080\"]");
        Console.WriteLine("  3. Client picks one, sends request\n");
        
        Console.WriteLine("Pros:");
        Console.WriteLine("  - Simple discovery logic");
        Console.WriteLine("  - Direct connection (no extra hop)\n");
        
        Console.WriteLine("Cons:");
        Console.WriteLine("  - Discovery logic in every client");
        Console.WriteLine("  - Client must handle failures\n");
    }

    private static void ServerSideDiscovery()
    {
        Console.WriteLine("🔚 SERVER-SIDE DISCOVERY (Load Balancer):\n");
        Console.WriteLine("Service-A logic:");
        Console.WriteLine("  1. Send request to load balancer: product.service:8080");
        Console.WriteLine("  2. Load balancer resolves: Query registry");
        Console.WriteLine("  3. Load balancer picks instance, forwards\n");
        
        Console.WriteLine("Pros:");
        Console.WriteLine("  - Client is simple (ask load balancer)");
        Console.WriteLine("  - Centralized routing and policies\n");
        
        Console.WriteLine("Cons:");
        Console.WriteLine("  - Extra hop (latency)");
        Console.WriteLine("  - Load balancer must be HA\n");
    }

    private static void HealthChecks()
    {
        Console.WriteLine("💓 HEALTH CHECKS:\n");
        Console.WriteLine("Registry periodically tests each instance:");
        Console.WriteLine("  GET /health\n");
        
        Console.WriteLine("Responses:");
        Console.WriteLine("  - 200 OK: Instance healthy, keep registered");
        Console.WriteLine("  - 503 Service Unavailable: Instance unhealthy, deregister");
        Console.WriteLine("  - Timeout: Instance dead, deregister\n");
        
        Console.WriteLine("Frequency: Every 10-30 seconds");
        Console.WriteLine("Cleanup: Unhealthy instances removed within 1 minute\n");
    }

    private static void RealWorldExample()
    {
        Console.WriteLine("🌐 NETFLIX EUREKA EXAMPLE:\n");
        Console.WriteLine("Service startup:");
        Console.WriteLine("  POST /eureka/apps/PRODUCT-SERVICE");
        Console.WriteLine("    { \"instance\": { \"hostName\": \"prod-1\", \"port\": 8080 } }\n");
        
        Console.WriteLine("Service-A discovers Product-Service:");
        Console.WriteLine("  GET /eureka/apps/PRODUCT-SERVICE");
        Console.WriteLine("  Response: { \"application\": { \"instance\": [");
        Console.WriteLine("    { \"hostName\": \"prod-1\", \"port\": 8080 },");
        Console.WriteLine("    { \"hostName\": \"prod-2\", \"port\": 8080 }");
        Console.WriteLine("  ]}}\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        Console.WriteLine("1. HEARTBEAT with TTL");
        Console.WriteLine("   Instance registers with 30s TTL");
        Console.WriteLine("   Sends heartbeat every 10s to extend TTL\n");
        
        Console.WriteLine("2. GRACEFUL SHUTDOWN");
        Console.WriteLine("   On shutdown: deregister before stopping\n");
        
        Console.WriteLine("3. CACHE REGISTRY LOCALLY");
        Console.WriteLine("   Reduce registry queries");
        Console.WriteLine("   Fallback if registry down\n");
        
        Console.WriteLine("4. USE LOAD BALANCING");
        Console.WriteLine("   Round-robin, least-connections, sticky sessions\n");
    }
}
