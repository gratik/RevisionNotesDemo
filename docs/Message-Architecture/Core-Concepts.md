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


