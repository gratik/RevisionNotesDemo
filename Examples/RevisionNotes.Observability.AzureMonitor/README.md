# RevisionNotes.Observability.AzureMonitor

Concrete Azure-oriented observability example for .NET APIs with:

- OpenTelemetry traces and metrics
- Azure Application Insights export via Azure Monitor exporter
- Correlation middleware using `X-Correlation-ID`
- W3C trace context propagation (`traceparent`) through HTTP
- Explicit Service Bus message correlation + traceparent propagation pattern

## Endpoints

- `GET /observability/ping`
- `POST /api/orders/{orderId}/checkout`
- `POST /api/payments/{orderId}`
- `POST /api/demo/servicebus/consume`

## Run

```bash
dotnet run --project Examples/RevisionNotes.Observability.AzureMonitor/RevisionNotes.Observability.AzureMonitor.csproj
```

## Configuration

Set these values for Azure export/publish behavior:

- `APPLICATIONINSIGHTS_CONNECTION_STRING`
- `ServiceBus:ConnectionString`
- `ServiceBus:EntityName`

If App Insights connection string is omitted, telemetry still flows to console exporter (dev mode).
