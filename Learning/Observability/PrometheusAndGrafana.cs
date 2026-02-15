// ==============================================================================
// Prometheus & Grafana - Metrics Collection & Visualization
// ==============================================================================
// WHAT IS THIS?
// Prometheus scrapes metrics (counter, gauge, histogram) from applications, stores as time-series. Grafana visualizes metrics as dashboards. Combined: system health visibility.
//
// WHY IT MATTERS
// ✅ REAL-TIME MONITORING: See system behavior now | ✅ HISTORICAL ANALYSIS: Trends over days/months | ✅ ALERTING: Trigger alerts on thresholds | ✅ EFFICIENT: Time-series compression saves storage | ✅ OPEN SOURCE: No vendor lock-in
//
// WHEN TO USE
// ✅ Production monitoring | ✅ Performance troubleshooting | ✅ Capacity planning | ✅ SLA tracking
//
// WHEN NOT TO USE
// ❌ Development/small scale | ❌ Logs (use ELK stack) | ❌ Need human-readable events
//
// REAL-WORLD EXAMPLE
// Kubernetes cluster: Every pod exposes /metrics endpoint. Prometheus scrapes every 15s. Stores CPU, memory, request rate. Grafana shows dashboards: green = healthy pods, red = failing. Alert fires if CPU > 80%.
// ==============================================================================

using System;
using System.Collections.Generic;

namespace RevisionNotesDemo.Observability;

public class PrometheusAndGrafana
{
    public static void RunAll()
    {
        Console.WriteLine("\n╔═══════════════════════════════════════════════════════╗");
        Console.WriteLine("║  Prometheus & Grafana Monitoring");
        Console.WriteLine("╚═══════════════════════════════════════════════════════╝\n");

        Overview();
        PrometheusMetrics();
        ScrapeConfiguration();
        GrafanaDashboards();
        Alerting();
        BestPractices();
    }

    private static void Overview()
    {
        Console.WriteLine("📖 OVERVIEW:\n");
        Console.WriteLine("Prometheus workflow:");
        Console.WriteLine("  1. App exposes /metrics endpoint");
        Console.WriteLine("  2. Prometheus scrapes periodically (every 15-60s)");
        Console.WriteLine("  3. Stores time-series data");
        Console.WriteLine("  4. Grafana queries Prometheus");
        Console.WriteLine("  5. Visualizes dashboards\n");
    }

    private static void PrometheusMetrics()
    {
        Console.WriteLine("📊 METRIC TYPES:\n");

        Console.WriteLine("1. COUNTER - Only increases");
        Console.WriteLine("   http_requests_total: 1,000,000");
        Console.WriteLine("   (useful for: total requests, total errors)\n");

        Console.WriteLine("2. GAUGE - Can go up or down");
        Console.WriteLine("   memory_usage_bytes: 500,000,000");
        Console.WriteLine("   (useful for: memory, connections, queue depth)\n");

        Console.WriteLine("3. HISTOGRAM - Counts in buckets");
        Console.WriteLine("   http_request_duration_seconds (buckets: 0.01, 0.1, 1, 10)");
        Console.WriteLine("   (useful for: latency distribution)\n");

        Console.WriteLine("4. SUMMARY - Like histogram, less storage");
        Console.WriteLine("   (useful for: percentiles 50th, 95th, 99th)\n");
    }

    private static void ScrapeConfiguration()
    {
        Console.WriteLine("🔧 PROMETHEUS CONFIG (prometheus.yml):\n");

        Console.WriteLine("global:");
        Console.WriteLine("  scrape_interval: 15s");
        Console.WriteLine("scrape_configs:");
        Console.WriteLine("  - job_name: 'api-server'");
        Console.WriteLine("    static_configs:");
        Console.WriteLine("    - targets: ['localhost:8080']");
        Console.WriteLine("    metrics_path: '/metrics'");
        Console.WriteLine("  - job_name: 'kubernetes'");
        Console.WriteLine("    kubernetes_sd_configs:");
        Console.WriteLine("    - role: pod\n");
    }

    private static void GrafanaDashboards()
    {
        Console.WriteLine("📈 GRAFANA DASHBOARD:\n");

        Console.WriteLine("Panel 1: Request Rate");
        Console.WriteLine("  Query: rate(http_requests_total[5m])");
        Console.WriteLine("  Shows: requests per second over time\n");

        Console.WriteLine("Panel 2: Error Rate");
        Console.WriteLine("  Query: rate(http_requests_total{status=\\\"500\\\"}[5m])");
        Console.WriteLine("  Shows: 500 errors per second\n");

        Console.WriteLine("Panel 3: P95 Latency");
        Console.WriteLine("  Query: histogram_quantile(0.95, http_request_duration_seconds)");
        Console.WriteLine("  Shows: 95% of requests < this latency\n");
    }

    private static void Alerting()
    {
        Console.WriteLine("⚠️  ALERTING RULES (alert.yml):\n");

        Console.WriteLine("groups:");
        Console.WriteLine("  - name: performance");
        Console.WriteLine("    rules:");
        Console.WriteLine("    - alert: HighErrorRate");
        Console.WriteLine("      expr: rate(http_requests_total{status=\\\"500\\\"}[5m]) > 0.05");
        Console.WriteLine("      for: 5m");
        Console.WriteLine("      annotations:");
        Console.WriteLine("        summary: 'High error rate detected'\n");
    }

    private static void BestPractices()
    {
        Console.WriteLine("✨ BEST PRACTICES:\n");

        Console.WriteLine("1. CARDINALITY MANAGEMENT");
        Console.WriteLine("   Avoid: http_requests_by_user_id (millions of values)");
        Console.WriteLine("   Use: http_requests by status code only\n");

        Console.WriteLine("2. RETENTION POLICY");
        Console.WriteLine("   High resolution (15s): 7 days");
        Console.WriteLine("   Downsampled (1m): 30 days");
        Console.WriteLine("   Downsampled (5m): 1 year\n");

        Console.WriteLine("3. RECORDING RULES");
        Console.WriteLine("   Pre-compute expensive queries");
        Console.WriteLine("   job:http_requests:rate5m = rate(http_requests_total[5m])\n");

        Console.WriteLine("4. BACKUP DASHBOARDS");
        Console.WriteLine("   Export JSON, version control\n");
    }
}
