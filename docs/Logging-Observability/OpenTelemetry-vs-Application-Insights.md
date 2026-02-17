# OpenTelemetry vs Application Insights

## Roles

- OpenTelemetry: instrumentation standard and SDK pipeline for traces/metrics/logs.
- Application Insights: Azure backend for storing, correlating, querying, and alerting on telemetry.

## Recommended model

- Instrument with OpenTelemetry.
- Export to Application Insights (Azure Monitor exporter).
- Keep schema and service naming conventions consistent across services.
