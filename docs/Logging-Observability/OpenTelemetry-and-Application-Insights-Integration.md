# OpenTelemetry and Application Insights Integration

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
