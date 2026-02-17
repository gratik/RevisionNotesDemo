# Azure Service Bus Patterns

> Subject: [Azure-Hosting](../README.md)

## Azure Service Bus Patterns

Azure Service Bus is the default choice for reliable enterprise messaging
between .NET services when you need queues/topics, retries, and dead-lettering.

### Best fit

- Command processing between services
- Workflow steps requiring durable delivery
- Decoupling APIs from slower back-end processors

### Key design points

- Use queues for point-to-point work distribution
- Use topics/subscriptions for fan-out events
- Make consumers idempotent for at-least-once delivery
- Monitor queue depth, oldest message age, and DLQ growth

### Production checklist

- Configure retries and lock renewal for handlers
- Set dead-letter strategy and alert thresholds
- Add correlation IDs and distributed tracing context
- Use managed identity and RBAC for access control

---
