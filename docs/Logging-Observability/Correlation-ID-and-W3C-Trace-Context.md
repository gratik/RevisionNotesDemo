# Correlation ID and W3C Trace Context

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Logging basics, distributed tracing concepts, and monitoring fundamentals.
- Related examples: docs/Logging-Observability/README.md
## Why both

- `traceparent` gives standards-based distributed tracing.
- `X-Correlation-ID` gives support-friendly cross-system lookup.

## Practical guidance

- Accept incoming `X-Correlation-ID` or generate one.
- Return correlation id in response headers.
- Put `correlationId` and `traceId` into logging scope.
- Let OpenTelemetry auto-propagate `traceparent` for HTTP calls.

## Interview Answer Block
30-second answer:
- Correlation ID and W3C Trace Context is about telemetry design for diagnostics and operations. It matters because good observability shortens detection and recovery times.
- Use it when correlating logs, traces, and metrics across service boundaries.

2-minute answer:
- Start with the problem Correlation ID and W3C Trace Context solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: high-cardinality detail vs telemetry cost/noise.
- Close with one failure mode and mitigation: missing correlation context during incident response.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Correlation ID and W3C Trace Context but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Correlation ID and W3C Trace Context, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Correlation ID and W3C Trace Context and map it to one concrete implementation in this module.
- 3 minutes: compare Correlation ID and W3C Trace Context with an alternative, then walk through one failure mode and mitigation.