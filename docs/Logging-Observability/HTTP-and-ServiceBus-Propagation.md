# HTTP and Service Bus Propagation

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Logging basics, distributed tracing concepts, and monitoring fundamentals.
- Related examples: docs/Logging-Observability/README.md
## HTTP hop

- Add `X-Correlation-ID` header to outgoing HttpClient requests.
- Rely on OpenTelemetry HttpClient instrumentation for W3C trace propagation.

## Service Bus hop

- Set `ServiceBusMessage.CorrelationId`.
- Add app property `correlationId` for query convenience.
- Add app property `traceparent` when present.
- On consume, continue Activity with incoming `traceparent`.

## Interview Answer Block
30-second answer:
- HTTP and Service Bus Propagation is about telemetry design for diagnostics and operations. It matters because good observability shortens detection and recovery times.
- Use it when correlating logs, traces, and metrics across service boundaries.

2-minute answer:
- Start with the problem HTTP and Service Bus Propagation solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: high-cardinality detail vs telemetry cost/noise.
- Close with one failure mode and mitigation: missing correlation context during incident response.
## Interview Bad vs Strong Answer
Bad answer:
- Defines HTTP and Service Bus Propagation but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose HTTP and Service Bus Propagation, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define HTTP and Service Bus Propagation and map it to one concrete implementation in this module.
- 3 minutes: compare HTTP and Service Bus Propagation with an alternative, then walk through one failure mode and mitigation.