# Core Patterns

> Subject: [Distributed-Consistency](../README.md)

## Core Patterns

### Outbox Pattern

Write domain state and integration event in one local transaction, then publish later from an outbox processor.

### Inbox Pattern

Track processed message ids at consumers so duplicate deliveries do not create duplicate business effects.

### Idempotency Keys

Require retry-safe request keys on write APIs and cache/return the first successful response.

### Deduplication Window

Retain processed keys/ids long enough to cover realistic retry/replay windows.

### Saga Compensation

Model reversible steps so partial success can be safely compensated after downstream failures.

---


