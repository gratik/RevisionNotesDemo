# Azure Event Hubs Patterns

> Subject: [Azure-Hosting](../README.md)

## Azure Event Hubs Patterns

Azure Event Hubs is optimized for very high-throughput telemetry and streaming
pipelines where retention and replay are core requirements.

### Best fit

- Application and infrastructure telemetry streams
- IoT and device event ingestion
- Real-time analytics and stream processing workloads

### Key design points

- Plan partitions around expected throughput and ordering keys
- Use consumer groups for independent processing pipelines
- Checkpoint only after durable downstream commit
- Track consumer lag and hot partitions as primary health signals

### Production checklist

- Define partition key strategy to avoid skew
- Set retention to support replay and incident recovery
- Capacity-test peak ingest and downstream throughput
- Implement idempotent consumers and duplicate handling

---
