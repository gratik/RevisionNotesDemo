# Logging and Observability Baseline

> Subject: [DotNet-API-React](../README.md)

## Logging and Observability Baseline

### Structured logging requirements

1. Include `TraceId`, `TenantId` (if multi-tenant), `Route`, and operation name in log scope.
2. Log business-significant events (order created, payment failed) with stable event ids.
3. Keep log payload bounded; redact PII and credential-bearing fields.
4. Use warning/error levels for actionable states only to reduce noise.

### End-to-end telemetry model

| Signal | API source | React source | Purpose |
| --- | --- | --- | --- |
| Request latency (p95/p99) | middleware + OpenTelemetry | page timing metric | User-perceived performance |
| Error rate by route | exception/problem details pipeline | UI error boundary metrics | Reliability and incident detection |
| Throughput | request counters | page interaction counters | Capacity and release impact |
| Trace correlation | `Activity.TraceId` | `X-Trace-Id` header propagation | Fast cross-tier incident triage |

### Logging/tracing snippets

- API reference: `GoodApiLoggingBaseline` in [React API Integration Examples](../../Learning/FrontEnd/ReactApiIntegrationExamples.cs)
- Frontend reference: `GoodReactTelemetry` in [React API Integration Examples](../../Learning/FrontEnd/ReactApiIntegrationExamples.cs)



