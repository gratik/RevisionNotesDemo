# Validation Checklist

> Subject: [DotNet-API-React](../README.md)

## Validation Checklist

- Endpoint returns typed DTOs and explicit status annotations.
- React client path does not duplicate base URL string literals.
- Non-2xx responses are parsed into user-facing error state.
- Local dev and production origins are both represented in CORS policy.
- Paging/filter constraints are validated server-side.
- Token validation and authorization policies are explicit and test-covered.
- Logs include correlation identifiers and avoid sensitive payload data.
- Telemetry dashboard includes p95 latency, route error rate, and deployment markers.


