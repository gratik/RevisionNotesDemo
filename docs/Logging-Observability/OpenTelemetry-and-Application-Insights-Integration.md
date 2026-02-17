# OpenTelemetry and Application Insights Integration

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

