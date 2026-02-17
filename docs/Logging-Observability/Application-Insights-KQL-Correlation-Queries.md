# Application Insights KQL Correlation Queries

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: Logging basics, distributed tracing concepts, and monitoring fundamentals.
- Related examples: docs/Logging-Observability/README.md
## Query by correlation id

```kql
union traces, requests, dependencies, exceptions
| where customDimensions.correlationId == "YOUR_ID_HERE"
| order by timestamp asc
```

## Query by operation id (trace id pivot)

```kql
union requests, dependencies, exceptions, traces
| where operation_Id == "OPERATION_ID_HERE"
| order by timestamp asc
```

## Support workflow

1. Get `X-Correlation-ID` from API response or ticket.
2. Query telemetry by correlation id.
3. Pivot to operation/trace view for cross-service timeline.

## Interview Answer Block
30-second answer:
- Application Insights KQL Correlation Queries is about telemetry design for diagnostics and operations. It matters because good observability shortens detection and recovery times.
- Use it when correlating logs, traces, and metrics across service boundaries.

2-minute answer:
- Start with the problem Application Insights KQL Correlation Queries solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: high-cardinality detail vs telemetry cost/noise.
- Close with one failure mode and mitigation: missing correlation context during incident response.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Application Insights KQL Correlation Queries but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Application Insights KQL Correlation Queries, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Application Insights KQL Correlation Queries and map it to one concrete implementation in this module.
- 3 minutes: compare Application Insights KQL Correlation Queries with an alternative, then walk through one failure mode and mitigation.