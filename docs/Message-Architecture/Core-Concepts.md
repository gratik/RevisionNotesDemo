# Core Concepts

> Subject: [Message-Architecture](../README.md)

## Core Concepts

### Message Queue vs Event Bus

| Feature       | Message Queue            | Event Bus (Pub/Sub)             |
| ------------- | ------------------------ | ------------------------------- |
| **Pattern**   | Point-to-point           | Publish-subscribe               |
| **Consumers** | One consumer per message | Multiple subscribers            |
| **Delivery**  | Exactly once             | At least once (all subscribers) |
| **Example**   | RabbitMQ queues          | Azure Service Bus Topics        |
| **Use Case**  | Task distribution        | Event notification              |

### Terms Explained

**Producer**: Service that sends messages
**Consumer**: Service that receives and processes messages
**Queue**: Storage for messages (FIFO)
**Exchange**: Routes messages to queues (RabbitMQ term)
**Topic**: Pub/sub channel (Azure Service Bus term)
**Subscription**: Consumer connection to topic
**Dead Letter Queue (DLQ)**: Failed messages storage

---

## Detailed Guidance

Distributed systems guidance focuses on idempotent workflows, eventual consistency, and replay-safe failure handling.

### Design Notes
- Define success criteria for Core Concepts before implementation work begins.
- Keep boundaries explicit so Core Concepts decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Core Concepts in production-facing code.
- When performance, correctness, or maintainability depends on consistent Core Concepts decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Core Concepts as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Core Concepts is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Core Concepts are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

