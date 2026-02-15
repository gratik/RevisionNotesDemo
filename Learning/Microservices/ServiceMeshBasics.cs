// ==============================================================================
// Service Mesh Basics and Communication Infrastructure
// ==============================================================================
// WHAT IS THIS?
// A service mesh is a dedicated infrastructure layer that handles service-to-service communication. It uses sidecar proxies (e.g., Envoy) deployed alongside each service to manage traffic, security (mTLS), and observability without changing application code.
//
// WHY IT MATTERS
// ✅ TRAFFIC MANAGEMENT: Canary deployments, A/B testing, circuit breakers | ✅ SECURITY: mTLS between services, no need for TLS in app code | ✅ OBSERVABILITY: Automatic tracing, metrics, logging | ✅ RESILIENCE: Automatic retries, timeouts, bulkheads | ✅ DECOUPLES CONCERNS: Network logic separated from business logic
//
// WHEN TO USE
// ✅ Microservices at scale (100+ services) | ✅ Kubernetes clusters | ✅ Complex traffic requirements | ✅ Observability critical
//
// WHEN NOT TO USE
// ❌ Few services | ❌ Operational complexity unwelcome | ❌ Monolithic architecture
//
// REAL-WORLD EXAMPLE
// Istio mesh on Kubernetes: Deploy Payment Service v2. Route 10% traffic to v2, 90% to v1, monitor errors. If error rate > 5%, rollback automatically. All without changing service code. Traffic rules applied in sidecar proxies.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Microservices;

public class ServiceMeshBasics
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Service Mesh Basics and Communication Infrastructure");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");
        
        Overview();
        SidecarProxyArchitecture();
        TrafficManagement();
        SecurityWithMTLS();
        ObservabilityFeatures();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Without Service Mesh:");
        Console.WriteLine("  Each service handles: routing, retry, timeout, TLS, logging\n");
        
        Console.WriteLine("With Service Mesh (Istio/Linkerd):");
        Console.WriteLine("  Sidecar proxy handles: routing, retry, timeout, mTLS, observability");
        Console.WriteLine("  Service code: just business logic\n");
    }

    private static void SidecarProxyArchitecture()
    {
        Console.WriteLine("🔧 SIDECAR PROXY ARCHITECTURE:\n");
        
        Console.WriteLine("Without mesh:");
        Console.WriteLine("  Service-A → directly calls Service-B:8080\n");
        
        Console.WriteLine("With mesh (Istio):");
        Console.WriteLine("  Service-A → localhost:15000 (Envoy proxy)");
        Console.WriteLine("  Proxy handles auth, routing");
        Console.WriteLine("  Proxy → Service-B (via mTLS)");
        Console.WriteLine("  Service-B → localhost:15000 (Envoy proxy)");
        Console.WriteLine("  Proxy decrypts, hands to Service-B\n");
        
        Console.WriteLine("Control plane (Istiod) programs all proxies:");
        Console.WriteLine("  Traffic rules, retries, timeouts, mTLS policies\n");
    }

    private static void TrafficManagement()
    {
        Console.WriteLine("🚦 TRAFFIC MANAGEMENT:\n");
        
        Console.WriteLine("Canary Deployment:");
        Console.WriteLine("  Deploy Payment v2");
        Console.WriteLine("  VirtualService rule: 90% → v1, 10% → v2");
        Console.WriteLine("  Monitor metrics from sidecar proxies");
        Console.WriteLine("  If v2 error rate > 5%: auto-rollback\n");
        
        Console.WriteLine("A/B Testing:");
        Console.WriteLine("  Route users with header X-User-Group: A → Service-v1");
        Console.WriteLine("  Route users with header X-User-Group: B → Service-v2\n");
        
        Console.WriteLine("Circuit Breaker:");
        Console.WriteLine("  If Service-B fails 5x consecutively");
        Console.WriteLine("  Stop sending requests for 30 seconds");
        Console.WriteLine("  Then slowly retry\n");
    }

    private static void SecurityWithMTLS()
    {
        Console.WriteLine("🔐 SECURITY WITH mTLS:\n");
        
        Console.WriteLine("Without mTLS:");
        Console.WriteLine("  Service-A → HTTP → Service-B (unencrypted)");
        Console.WriteLine("  Microservices must handle TLS themselves\n");
        
        Console.WriteLine("With mTLS (Istio):");
        Console.WriteLine("  Service-A → Envoy proxy (connection: localhost)");
        Console.WriteLine("  Envoy proxy → Envoy proxy (mTLS with certificate)");
        Console.WriteLine("  Envoy proxy → Service-B (connection: localhost)\n");
        
        Console.WriteLine("Certificates managed automatically:");
        Console.WriteLine("  Istiod generates unique cert for each service");
        Console.WriteLine("  Proxies validate peer certificate");
        Console.WriteLine("  Application code: unchanged\n");
    }

    private static void ObservabilityFeatures()
    {
        Console.WriteLine("📊 OBSERVABILITY:\n");
        
        Console.WriteLine("Automatic Metrics:");
        Console.WriteLine("  Sidecar counts: requests, responses, errors, latency");
        Console.WriteLine("  No code instrumentation needed\n");
        
        Console.WriteLine("Distributed Tracing:");
        Console.WriteLine("  Request passes through 5 services");
        Console.WriteLine("  Each sidecar logs request/response");
        Console.WriteLine("  Traces correlated to see full flow\n");
        
        Console.WriteLine("Visualization (Kiali):");
        Console.WriteLine("  See service graph: which services call which");
        Console.WriteLine("  Color-code by health (green: healthy, red: errors)\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");
        
        Console.WriteLine("1. GRADUAL ROLLOUT");
        Console.WriteLine("   Don't deploy to all services at once");
        Console.WriteLine("   Namespace by namespace\n");
        
        Console.WriteLine("2. MONITOR PERFORMANCE");
        Console.WriteLine("   Sidecar adds <5ms latency");
        Console.WriteLine("   Monitor for regressions\n");
        
        Console.WriteLine("3. RESOURCE LIMITS");
        Console.WriteLine("   Proxy: 100MB memory per service");
        Console.WriteLine("   Plan resource requirements\n");
        
        Console.WriteLine("4. USE POLICIES");
        Console.WriteLine("   Define traffic policies (retries, timeouts)");
        Console.WriteLine("   Enforce security policies (mTLS requirement)\n");
    }
}
