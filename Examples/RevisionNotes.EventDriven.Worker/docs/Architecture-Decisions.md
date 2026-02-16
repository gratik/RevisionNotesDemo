# Architecture Decisions - RevisionNotes.EventDriven.Worker

## WHAT IS THIS?
This is an event-processing worker demo that focuses on producer/consumer flow, retries, idempotency, and non-HTTP operational monitoring.

## WHY IT MATTERS?
- Background workers are easy to build but hard to operate safely under duplicate delivery and transient failures.
- This demo shows resilience and observability patterns that prevent silent data corruption.

## WHEN TO USE?
- When you need asynchronous event handling outside request/response APIs.
- When work can be retried and eventually processed.

## WHEN NOT TO USE?
- When business operations require immediate synchronous response semantics.
- When event durability and delivery guarantees are required but no durable broker/storage exists yet.

## REAL-WORLD EXAMPLE?
An order-processing worker that consumes status events from a queue, applies idempotent state transitions, and retries transient integration failures.

## ADR-01: Channel-based in-memory queue
- Decision: Use `Channel<T>` for producer/consumer coordination.
- Why: Simple back-pressure capable queue for demo purposes.
- Tradeoff: No durability across process restarts.

## ADR-02: Idempotency gate before processing
- Decision: Track in-flight/completed event ids and skip duplicates.
- Why: Demonstrates safe consumer behavior under duplicate delivery.
- Tradeoff: In-memory store is non-distributed; production needs durable/shared storage.

## ADR-03: Retry with bounded attempts
- Decision: Retry failed events up to 3 attempts with delay.
- Why: Handles transient errors without infinite retry storms.
- Tradeoff: Poison messages eventually drop and require dead-letter strategy in production.

## ADR-04: Health checks + periodic health reporting
- Decision: Evaluate health inside worker and log results periodically.
- Why: Worker templates have no HTTP surface by default; logs still expose operational state.
- Tradeoff: External probing requires additional endpoint/sidecar in production.

## ADR-05: Structured logs for all pipeline transitions
- Decision: Log publish, process success/failure, retry, duplicate skip.
- Why: Enables timeline reconstruction and faster incident diagnosis.
- Tradeoff: Verbose logging should be sampled/tuned under high throughput.
