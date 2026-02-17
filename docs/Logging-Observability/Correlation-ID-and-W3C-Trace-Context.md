# Correlation ID and W3C Trace Context

## Why both

- `traceparent` gives standards-based distributed tracing.
- `X-Correlation-ID` gives support-friendly cross-system lookup.

## Practical guidance

- Accept incoming `X-Correlation-ID` or generate one.
- Return correlation id in response headers.
- Put `correlationId` and `traceId` into logging scope.
- Let OpenTelemetry auto-propagate `traceparent` for HTTP calls.
