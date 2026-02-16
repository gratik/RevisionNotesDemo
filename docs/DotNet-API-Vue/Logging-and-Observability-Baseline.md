# Logging and Observability Baseline

> Subject: [DotNet-API-Vue](../README.md)

## Logging and Observability Baseline

### Logging standards

1. Use structured logs with stable property names (`TraceId`, `Route`, `TenantId`).
2. Log at boundaries: request received, domain decision, external dependency call, response sent.
3. Avoid sensitive fields (tokens, passwords, raw personal data).
4. Use consistent event ids for alert mapping and post-incident analysis.

### Required telemetry signals

| Signal | API source | Vue source | Purpose |
| --- | --- | --- | --- |
| p95/p99 latency by endpoint | ASP.NET middleware metrics | page-level load timing | detect degradation quickly |
| Error rate by status code | exception/ProblemDetails pipeline | UI error tracking | reliability and incident severity |
| Dependency latency | HTTP/DB instrumentation | client retry/error distribution | isolate backend dependency bottlenecks |
| Trace correlation | W3C trace context + `TraceId` | `X-Trace-Id` propagation | cross-tier troubleshooting |

### Logging/tracing snippets

- API reference: `GoodLoggingAndTracing` in [Vue API Integration Examples](../../Learning/FrontEnd/VueApiIntegrationExamples.cs)
- Frontend reference: `GoodVueTelemetry` in [Vue API Integration Examples](../../Learning/FrontEnd/VueApiIntegrationExamples.cs)



