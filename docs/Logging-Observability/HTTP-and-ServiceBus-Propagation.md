# HTTP and Service Bus Propagation

## HTTP hop

- Add `X-Correlation-ID` header to outgoing HttpClient requests.
- Rely on OpenTelemetry HttpClient instrumentation for W3C trace propagation.

## Service Bus hop

- Set `ServiceBusMessage.CorrelationId`.
- Add app property `correlationId` for query convenience.
- Add app property `traceparent` when present.
- On consume, continue Activity with incoming `traceparent`.
