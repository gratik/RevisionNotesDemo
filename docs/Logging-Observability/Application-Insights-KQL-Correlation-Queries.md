# Application Insights KQL Correlation Queries

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
