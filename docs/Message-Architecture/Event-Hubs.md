# Event Hubs

> Subject: [Message-Architecture](../README.md)

## Event Hubs

Azure Event Hubs is designed for high-volume event ingestion and stream processing.
Use it when throughput and replay are the primary goals.

### Best fit

- Telemetry, logs, clickstreams, and IoT events
- Real-time analytics pipelines
- Multi-consumer stream processing through consumer groups

### Key design points

- Partition for scale and ordering per key
- Checkpoint only after durable processing
- Expect at-least-once delivery and duplicates
- Monitor consumer lag and partition hot spots

### Not the best fit

- Workflow commands requiring rich routing and DLQ semantics
- Low-volume business notifications where Event Grid is simpler

### Minimal consumer sketch

```csharp
// Install: Azure.Messaging.EventHubs, Azure.Messaging.EventHubs.Processor

public class TelemetryProcessor : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        // create EventProcessorClient with Blob checkpoint store
        // subscribe ProcessEventAsync + ProcessErrorAsync
        // start processor
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        // stop processor
        return Task.CompletedTask;
    }
}
```

---
