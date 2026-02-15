# 📊 Observability & Monitoring Patterns

**Sections 19-20 Partial - Monitoring, Logging, Tracing, and Metrics**

This folder contains patterns for observing, monitoring, and troubleshooting distributed systems in production.

## 📂 Contents

- **StructuredLoggingAdvanced.cs** - JSON logs, correlation IDs, searchable logging
- **PrometheusAndGrafana.cs** - Metrics collection, time-series storage, dashboards, alerting
- **OpenTelemetrySetup.cs** - Vendor-agnostic instrumentation, traces, metrics, logs
- **DistributedTracingJaegerZipkin.cs** - Request tracing across services, latency insights
- **ApplicationInsightsIntegration.cs** - Azure Application Insights, auto-tracking, dashboards
- **HealthChecksAndHeartbeats.cs** - Liveness and readiness probes, dependency checks

## 🎯 Key Concepts

### What You'll Learn
- Structured logging best practices
- Metrics collection and aggregation
- Distributed tracing across services
- Observability as code
- Production troubleshooting
- Real-time alerting

### Real-World Scenarios
- Production incident triage using logs
- Kubernetes liveness probe configuration
- Monitoring multi-region deployment
- Real-time performance dashboards

## 💡 Usage

Each file demonstrates:
- Setup and configuration
- Integration with popular tools
- Real-world telemetry examples
- Cost optimization strategies
- Common troubleshooting patterns

Run demonstrations:
```bash
dotnet run
# Select Observability patterns from menu
```

## 🔗 Related Sections
- [Microservices](../Microservices/README.md) - Observing distributed systems
- [DevOps](../DevOps/README.md) - Infrastructure monitoring
- [Security](../Security/README.md) - Audit logging

---
_Updated: February 15, 2026_
