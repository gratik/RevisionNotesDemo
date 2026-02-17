# Application Insights KQL Correlation Queries

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

