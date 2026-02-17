# Event Grid

> Subject: [Message-Architecture](../README.md)

## Event Grid

Azure Event Grid provides push-based event routing between publishers and subscribers.
Use it for reactive integrations and cloud automation.

### Best fit

- Azure resource events (blob created/deleted, provisioning events)
- Webhook and Azure Function triggers
- Fan-out notifications to multiple independent handlers

### Key design points

- Filter on event type and subject to reduce noise
- Configure dead-letter destination for failed deliveries
- Keep subscribers idempotent and fast
- Offload heavy processing to queues/workers

### Not the best fit

- High-ingest telemetry streams (use Event Hubs)
- Ordered command workflows with complex broker features (use Service Bus)

### Minimal handler sketch

```csharp
app.MapPost("/events", async (HttpRequest request) =>
{
    // Parse CloudEvents/EventGrid schema
    // Handle subscription validation handshake
    // Process event idempotently and return 200
    await Task.CompletedTask;
    return Results.Ok();
});
```

---
