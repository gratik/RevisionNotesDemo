# Architecture Decisions

## Scope

This demo focuses on practical correlation in Azure-centric .NET services using OpenTelemetry and Application Insights.

## Decisions

1. Use OpenTelemetry as the primary telemetry pipeline.
2. Export traces/metrics with Azure Monitor exporter to Application Insights.
3. Preserve an explicit `X-Correlation-ID` header alongside W3C `traceparent` propagation.
4. Propagate correlation across HTTP and Service Bus message boundaries.

## Tradeoffs

- Keeping both correlation ID and trace context improves support workflows but adds minor middleware complexity.
- Service Bus publish is optional at runtime when connection string is missing to keep local development simple.
- Console exporter is enabled by default for local signal visibility; production should rely on Azure Monitor exporter.
