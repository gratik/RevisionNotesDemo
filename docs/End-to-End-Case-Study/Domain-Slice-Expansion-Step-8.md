# Domain Slice Expansion (Step 8)

> Subject: [End-to-End-Case-Study](../README.md)

## Domain Slice Expansion (Step 8)

The integrated case study now includes three cohesive domain slices, each with explicit API, data, resilience, observability, security, and deployment checkpoints.

### Slice A: Checkout API

- API: `POST /orders` with idempotency key
- Data: order aggregate + outbox in one write boundary
- Resilience: transient-store retry policy with bounded attempts
- Observability: trace + `orderId` correlation and latency histogram
- Security: JWT role checks and anti-replay validation
- Deployment: canary rollout with rollback threshold

### Slice B: Fulfillment Worker

- Contract: consumes `OrderPlaced` message
- Data: inbox dedupe + inventory reservation state
- Resilience: retry + dead-letter fallback
- Observability: queue lag and per-attempt processing metrics
- Security: managed identity access to queue/storage
- Deployment: blue/green worker handoff

### Slice C: Customer Notification

- Contract: notification command envelope with channel settings
- Data: delivery log and per-channel status tracking
- Resilience: provider circuit breaker + fallback provider route
- Observability: delivery success rate and provider latency
- Security: PII masking and encrypted payload storage
- Deployment: feature-flagged staged rollout by tenant cohort

---


