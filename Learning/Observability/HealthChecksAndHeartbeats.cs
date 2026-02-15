// ==============================================================================
// Health Checks & Heartbeats
// ==============================================================================
// WHAT IS THIS?
// Health checks are endpoints that verify if a service is alive and healthy. Heartbeats are periodic signals sent by services to registries. Orchestrators (K8s, Service Fabric) use these to detect failures and restart services.
//
// WHY IT MATTERS
// ✅ AUTO-HEALING: Detect dead pods, restart automatically | ✅ LOAD BALANCER: Remove unhealthy instances from traffic | ✅ SCALE DECISIONS: Only count healthy instances | ✅ DEBUGGING: Understand why service marked unhealthy | ✅ CASCADING FAILURE PREVENTION: Know dependencies are working | ✅ QUICK RECOVERY: Detect failure in seconds, not hours
//
// WHEN TO USE
// ✅ Kubernetes deployments (liveness, readiness, startup probes) | ✅ Service discovery (Consul, Eureka) | ✅ Load balancers | ✅ Microservices with failing dependencies | ✅ Any distributed system
//
// WHEN NOT TO USE
// ❌ Monolithic apps (less critical) | ❌ Synchronous health checks causing delays
//
// REAL-WORLD EXAMPLE
// Kubernetes pod: Liveness probe checks every 10s if app responds. Probe fails for 30s → K8s kills pod, starts new one. Without health checks: failed pod keeps receiving traffic, customers see errors for hours.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Observability;

public class HealthChecksAndHeartbeats
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Health Checks & Heartbeats");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

        Overview();
        KubernetesProbes();
        Implementation();
        ServiceDiscoveryHeartbeats();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Three types of health signals:\n");

        Console.WriteLine("1. LIVENESS: Is service alive? If NO → kill pod");
        Console.WriteLine("2. READINESS: Can service handle requests? If NO → remove from LB");
        Console.WriteLine("3. STARTUP: Did service initialize? Wait before other probes\n");

        Console.WriteLine("Typical scenario:");
        Console.WriteLine("  Service starts → STARTUP probe (wait for DB connection) → ready");
        Console.WriteLine("  Running → READINESS probe (10s) → still healthy");
        Console.WriteLine("  Running → LIVENESS probe (10s) → process alive\n");
    }

    private static void KubernetesProbes()
    {
        Console.WriteLine("🚀 KUBERNETES PROBES:\n");

        Console.WriteLine("HTTP Probe (most common):");
        Console.WriteLine("  K8s: GET /health");
        Console.WriteLine("  Response: 200 OK → healthy");
        Console.WriteLine("  Response: 500+ → unhealthy\n");

        Console.WriteLine("Example deployment:");
        Console.WriteLine("  livenessProbe:");
        Console.WriteLine("    httpGet:");
        Console.WriteLine("      path: /health/live");
        Console.WriteLine("      port: 8080");
        Console.WriteLine("    initialDelaySeconds: 10  (wait before first probe)");
        Console.WriteLine("    periodSeconds: 10       (probe every 10s)");
        Console.WriteLine("    failureThreshold: 3     (3 failures → kill pod)\n");

        Console.WriteLine("Readiness (similar):");
        Console.WriteLine("  readinessProbe:");
        Console.WriteLine("    httpGet:");
        Console.WriteLine("      path: /health/ready");
        Console.WriteLine("      port: 8080");
        Console.WriteLine("    periodSeconds: 5\n");

        Console.WriteLine("Startup (for slow-starting apps):");
        Console.WriteLine("  startupProbe:");
        Console.WriteLine("    httpGet: { path: /health/startup, port: 8080 }");
        Console.WriteLine("    failureThreshold: 30    (allow 30 × 3s = 90s to start)\n");
    }

    private static void Implementation()
    {
        Console.WriteLine("💻 HEALTH ENDPOINT IMPLEMENTATION:\n");

        Console.WriteLine("// Minimal health endpoint");
        Console.WriteLine("app.MapGet(\"/health/live\", () =>");
        Console.WriteLine("{");
        Console.WriteLine("    return Results.Ok(new { status = \"alive\" });");
        Console.WriteLine("});\n");

        Console.WriteLine("// Readiness endpoint (checks dependencies)");
        Console.WriteLine("app.MapGet(\"/health/ready\", async (IServiceCollection services) =>");
        Console.WriteLine("{");
        Console.WriteLine("    try");
        Console.WriteLine("    {");
        Console.WriteLine("        // Check database connectivity");
        Console.WriteLine("        var context = services.GetRequiredService<DbContext>();");
        Console.WriteLine("        await context.Database.ExecuteSqlAsync($\"SELECT 1\");");
        Console.WriteLine("        ");
        Console.WriteLine("        // Check message queue");
        Console.WriteLine("        var queue = services.GetRequiredService<IMessageQueue>();");
        Console.WriteLine("        await queue.CheckHealthAsync();\n");
        Console.WriteLine("        return Results.Ok(new { status = \"ready\" });");
        Console.WriteLine("    }");
        Console.WriteLine("    catch");
        Console.WriteLine("    {");
        Console.WriteLine("        return Results.StatusCode(503); // Service Unavailable");
        Console.WriteLine("    }");
        Console.WriteLine("});\n");

        Console.WriteLine("🎯 Health check what:");
        Console.WriteLine("  • Liveness: Minimal (process alive?)");
        Console.WriteLine("  • Readiness: Database, cache, queues");
        Console.WriteLine("  • Don't: Make expensive calls (logging all queries, etc.)\n");
    }

    private static void ServiceDiscoveryHeartbeats()
    {
        Console.WriteLine("💓 SERVICE DISCOVERY HEARTBEATS:\n");

        Console.WriteLine("Consul example:");
        Console.WriteLine("  Service registers: POST /v1/agent/service/register");
        Console.WriteLine("  Consul checks health: GET /health every 10s");
        Console.WriteLine("  If fails 3× → Service deregistered (removed from discovery)");
        Console.WriteLine("  Other services: Ask Consul for service list → don't see unhealthy ones\n");

        Console.WriteLine("Eureka (Netflix):");
        Console.WriteLine("  App heartbeats: Send heartbeat every 30s");
        Console.WriteLine("  Miss 3 heartbeats? → Eureka removes from registry");
        Console.WriteLine("  Clients cache list, also listen for changes\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✅ BEST PRACTICES:\n");

        Console.WriteLine("Endpoint design:");
        Console.WriteLine("  ✅ Fast (<100ms response)");
        Console.WriteLine("  ✅ No external calls (only check local state)");
        Console.WriteLine("  ✅ Return same status for same logical state");
        Console.WriteLine("  ✅ Include version info for debugging\n");

        Console.WriteLine("Configuration:");
        Console.WriteLine("  ✅ Liveness: initialDelay=10s, period=10s, threshold=3");
        Console.WriteLine("  ✅ Readiness: initialDelay=5s, period=5s, threshold=1");
        Console.WriteLine("  ✅ Startup: period=1s, threshold=30");
        Console.WriteLine("  ✅ Timeout: 2-3 seconds (faster to detect failure)\n");

        Console.WriteLine("Response format:");
        Console.WriteLine("  { \"status\": \"healthy\", \"timestamp\": \"2026-02-15T...\", \"issues\": [] }\n");
    }
}
