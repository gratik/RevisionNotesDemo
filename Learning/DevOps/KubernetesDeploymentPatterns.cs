// ==============================================================================
// Kubernetes Deployment Patterns
// ==============================================================================
// WHAT IS THIS?
// Kubernetes (K8s) is an orchestration platform managing containerized apps. It handles scaling, self-healing, load balancing, and updates. Deployment patterns define how to roll out new versions.
//
// WHY IT MATTERS
// ✅ AUTO-SCALING: 100 → 1000 requests/sec → K8s scales pods automatically | ✅ SELF-HEALING: Pod crashes → new pod starts (same IP to clients) | ✅ ROLLING UPDATES: Deploy new version with zero downtime | ✅ RESOURCE EFFICIENCY: Bin-pack pods, minimize unused capacity | ✅ MULTI-REGION: Distribute across availability zones | ✅ DECLARATIVE: Describe desired state, K8s ensures it
//
// WHEN TO USE
// ✅ Microservices with stateless services | ✅ Need auto-scaling | ✅ Multi-node deployment | ✅ Cloud-native architectures | ✅ High-availability requirements
//
// WHEN NOT TO USE
// ❌ Single stateful database (K8s + stateful = complex) | ❌ Simple monolithic apps | ❌ Very low latency (K8s scheduling adds overhead)
//
// REAL-WORLD EXAMPLE
// E-commerce during holiday sale: K8s deployment specifies "always 10 pods running". Traffic spikes → metrics show 80% CPU → auto-scaler increases to 50 pods. Sale ends → scales back to 10. All automatic.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.DevOps;

public class KubernetesDeploymentPatterns
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Kubernetes Deployment Patterns");
        Console.WriteLine("╚════════════════════════════════════════════════════╝\n");

        Overview();
        DeploymentManifest();
        RollingUpdate();
        Scaling();
        HealthAndMonitoring();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Kubernetes Core Concepts:");
        Console.WriteLine("  • Cluster: Group of machines");
        Console.WriteLine("  • Node: Physical/virtual machine");
        Console.WriteLine("  • Pod: Smallest deployable unit (1+ containers)");
        Console.WriteLine("  • Deployment: Declares desired pod replicas");
        Console.WriteLine("  • Service: Stable IP/DNS for pods");
        Console.WriteLine("  • ConfigMap: Configuration data");
        Console.WriteLine("  • Secret: Sensitive data (passwords)\n");
    }

    private static void DeploymentManifest()
    {
        Console.WriteLine("📋 DEPLOYMENT MANIFEST (YAML):\n");

        Console.WriteLine("apiVersion: apps/v1");
        Console.WriteLine("kind: Deployment");
        Console.WriteLine("metadata:");
        Console.WriteLine("  name: api-service");
        Console.WriteLine("  labels:");
        Console.WriteLine("    app: api");
        Console.WriteLine("spec:");
        Console.WriteLine("  replicas: 3  # Always run 3 pods");
        Console.WriteLine("  selector:");
        Console.WriteLine("    matchLabels:");
        Console.WriteLine("      app: api");
        Console.WriteLine("  template:");
        Console.WriteLine("    metadata:");
        Console.WriteLine("      labels:");
        Console.WriteLine("        app: api");
        Console.WriteLine("    spec:");
        Console.WriteLine("      containers:");
        Console.WriteLine("      - name: api");
        Console.WriteLine("        image: myrepo/api:v1.2.0");
        Console.WriteLine("        ports:");
        Console.WriteLine("        - containerPort: 8080");
        Console.WriteLine("        env:");
        Console.WriteLine("        - name: DATABASE_URL");
        Console.WriteLine("          valueFrom:");
        Console.WriteLine("            configMapKeyRef:");
        Console.WriteLine("              name: app-config");
        Console.WriteLine("              key: database_url");
        Console.WriteLine("        resources:");
        Console.WriteLine("          requests:");
        Console.WriteLine("            cpu: 100m");
        Console.WriteLine("            memory: 128Mi");
        Console.WriteLine("          limits:");
        Console.WriteLine("            cpu: 500m");
        Console.WriteLine("            memory: 512Mi");
        Console.WriteLine("        livenessProbe:");
        Console.WriteLine("          httpGet:");
        Console.WriteLine("            path: /health");
        Console.WriteLine("            port: 8080");
        Console.WriteLine("          initialDelaySeconds: 30");
        Console.WriteLine("          periodSeconds: 10\n");
    }

    private static void RollingUpdate()
    {
        Console.WriteLine("🔄 ROLLING UPDATE STRATEGY:\n");

        Console.WriteLine("Scenario: New v1.3.0 available, currently running v1.2.0\n");

        Console.WriteLine("Deployment spec:");
        Console.WriteLine("  strategy:");
        Console.WriteLine("    type: RollingUpdate");
        Console.WriteLine("    rollingUpdate:");
        Console.WriteLine("      maxSurge: 1        # Max 1 extra pod during update");
        Console.WriteLine("      maxUnavailable: 0  # Min 3 pods always available\n");

        Console.WriteLine("Process:");
        Console.WriteLine("  1. Start new pod (v1.3.0) → 4 pods total");
        Console.WriteLine("  2. Send traffic to new pod (health checks) → success");
        Console.WriteLine("  3. Stop old pod (v1.2.0) → 3 pods");
        Console.WriteLine("  4. Repeat for remaining pods");
        Console.WriteLine("  5. Result: 0 downtime, gradual rollout\n");

        Console.WriteLine("Canary strategy (safer):");
        Console.WriteLine("  • maxSurge: 1 (run 1 new pod alongside 3 old)");
        Console.WriteLine("  • Monitor metrics (errors, latency)");
        Console.WriteLine("  • If bad metrics → rollback (kill new pod)\n");
    }

    private static void Scaling()
    {
        Console.WriteLine("📈 HORIZONTAL POD AUTOSCALER (HPA):\n");

        Console.WriteLine("Simple scaling:");
        Console.WriteLine("  kubectl scale deployment api-service --replicas=10\n");

        Console.WriteLine("Automatic scaling (based on metrics):");
        Console.WriteLine("  apiVersion: autoscaling/v2");
        Console.WriteLine("  kind: HorizontalPodAutoscaler");
        Console.WriteLine("  metadata:");
        Console.WriteLine("    name: api-hpa");
        Console.WriteLine("  spec:");
        Console.WriteLine("    scaleTargetRef:");
        Console.WriteLine("      apiVersion: apps/v1");
        Console.WriteLine("      kind: Deployment");
        Console.WriteLine("      name: api-service");
        Console.WriteLine("    minReplicas: 3");
        Console.WriteLine("    maxReplicas: 100");
        Console.WriteLine("    metrics:");
        Console.WriteLine("    - type: Resource");
        Console.WriteLine("      resource:");
        Console.WriteLine("        name: cpu");
        Console.WriteLine("        target:");
        Console.WriteLine("          type: Utilization");
        Console.WriteLine("          averageUtilization: 70  # Scale up if CPU > 70%\n");

        Console.WriteLine("Result: K8s automatically scales pods based on demand\n");
    }

    private static void HealthAndMonitoring()
    {
        Console.WriteLine("🩺 HEALTH & MONITORING:\n");

        Console.WriteLine("Liveness probe (is pod alive?):");
        Console.WriteLine("  False 3x → K8s kills and restarts pod\n");

        Console.WriteLine("Readiness probe (can pod handle traffic?):");
        Console.WriteLine("  False 1x → Pod removed from load balancer");
        Console.WriteLine("  True again → Pod added back\n");

        Console.WriteLine("Observability:");
        Console.WriteLine("  • kubectl logs pod-name");
        Console.WriteLine("  • kubectl describe pod pod-name");
        Console.WriteLine("  • kubectl top pods (CPU/memory usage)");
        Console.WriteLine("  • Prometheus/Grafana integration\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✅ BEST PRACTICES:\n");

        Console.WriteLine("Resource requests & limits:");
        Console.WriteLine("  ✅ Always set CPU/memory requests (K8s needs for scheduling)");
        Console.WriteLine("  ✅ Set limits (prevent noisy neighbor)");
        Console.WriteLine("  ✅ Example: requests: {cpu: 100m, memory: 128Mi}");
        Console.WriteLine("            limits: {cpu: 500m, memory: 512Mi}\n");

        Console.WriteLine("Probes:");
        Console.WriteLine("  ✅ Liveness: initialDelay=30s, period=10s, timeout=2s");
        Console.WriteLine("  ✅ Readiness: initialDelay=5s, period=3s, timeout=1s");
        Console.WriteLine("  ✅ Check dependent services (DB, cache)\n");

        Console.WriteLine("Updates:");
        Console.WriteLine("  ✅ Use RollingUpdate strategy (zero downtime)");
        Console.WriteLine("  ✅ Set maxUnavailable: 0 (always available)");
        Console.WriteLine("  ✅ Monitor metrics during rollout");
        Console.WriteLine("  ✅ Keep rollout history for quick rollback\n");

        Console.WriteLine("Scaling:");
        Console.WriteLine("  ✅ Use HPA for auto-scaling");
        Console.WriteLine("  ✅ Set realistic CPU targets (70-80%)");
        Console.WriteLine("  ✅ Monitor scale-up/scale-down events\n");
    }
}
