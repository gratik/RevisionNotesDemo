# Architecture Decisions - RevisionNotes.BackgroundJobs

## WHAT IS THIS?
This is a worker-service demo for background jobs with queue-based processing, retries, and idempotent execution.

## WHY IT MATTERS?
- Reliable background processing is critical for long-running or asynchronous workflows.
- Idempotency and retry control prevent duplicate side effects and runaway failures.

## WHEN TO USE?
- When work can be decoupled from request/response APIs.
- When eventual completion is acceptable and retry behavior is required.

## WHEN NOT TO USE?
- When operations must complete synchronously before responding to clients.
- When strict durable queue guarantees are required but no durable broker is available.

## REAL-WORLD EXAMPLE?
A billing pipeline that generates invoice PDFs and sends notifications asynchronously after orders are confirmed.

## ADR-01: Queue-based job processing
- Decision: Use producer/consumer services over an in-memory channel.
- Why: Simple demonstration of asynchronous background execution.
- Tradeoff: No persistence on process restart.

## ADR-02: Idempotency and retries by default
- Decision: Track processed job ids and retry transient failures up to three attempts.
- Why: Handles duplicate delivery and temporary faults safely.
- Tradeoff: Poison-job dead-lettering is not implemented in this lightweight sample.

## ADR-03: Worker-native health monitoring
- Decision: Run health checks internally and emit periodic status logs.
- Why: Worker templates do not expose HTTP endpoints by default.
- Tradeoff: External probe integration needs additional hosting surface.
