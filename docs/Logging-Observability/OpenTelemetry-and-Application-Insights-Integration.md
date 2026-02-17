# OpenTelemetry and Application Insights Integration

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Logging basics, distributed tracing concepts, and monitoring fundamentals.
- Related examples: docs/Logging-Observability/README.md
## Integration checklist

- Configure OpenTelemetry resource (`service.name`, `service.version`, `environment`).
- Add ASP.NET Core and HttpClient instrumentation.
- Export traces and metrics to Azure Monitor using `APPLICATIONINSIGHTS_CONNECTION_STRING`.
- Keep structured logging with correlation fields (`correlationId`, `traceId`).

## .NET API baseline

- `AddOpenTelemetry().WithTracing(...).WithMetrics(...)`
- `AddAzureMonitorTraceExporter(...)`
- `AddAzureMonitorMetricExporter(...)`
- Correlation middleware enforcing `X-Correlation-ID`

## Interview Answer Block
30-second answer:
- OpenTelemetry and Application Insights Integration is about telemetry design for diagnostics and operations. It matters because good observability shortens detection and recovery times.
- Use it when correlating logs, traces, and metrics across service boundaries.

2-minute answer:
- Start with the problem OpenTelemetry and Application Insights Integration solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: high-cardinality detail vs telemetry cost/noise.
- Close with one failure mode and mitigation: missing correlation context during incident response.
## Interview Bad vs Strong Answer
Bad answer:
- Defines OpenTelemetry and Application Insights Integration but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose OpenTelemetry and Application Insights Integration, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define OpenTelemetry and Application Insights Integration and map it to one concrete implementation in this module.
- 3 minutes: compare OpenTelemetry and Application Insights Integration with an alternative, then walk through one failure mode and mitigation.