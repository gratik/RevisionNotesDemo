# Interview Whiteboard Prompts

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Design Patterns, Distributed Consistency, Deployment and DevOps
- Related examples: Learning/Architecture/IntegratedDomainSlicesCaseStudy.cs, Learning/Microservices/DistributedConsistencyPatterns.cs

Use these prompts to practice architecture interviews with concrete evaluation criteria.

## Prompt 1: Order Platform (Core E-commerce)

- Design an order-placement workflow handling payment, inventory, and notifications.
- Constraints:
  - p95 latency target: < 250ms
  - at-least-once delivery on async edges
  - rollback path for failed deployment
- Expected sections in answer:
  - service boundaries
  - API contracts and idempotency strategy
  - consistency model (outbox/inbox, compensations)
  - observability and incident signals
  - rollout and rollback plan

## Prompt 2: Multi-tenant API Platform

- Design a tenant-aware API with isolation, authZ boundaries, and operational visibility.
- Constraints:
  - tenant data isolation
  - per-tenant rate limits
  - auditable access logs
- Expected sections in answer:
  - identity/authZ model
  - data partitioning strategy
  - throttling and abuse controls
  - key metrics and alert thresholds

## Prompt 3: Event-driven Fulfillment

- Design a fulfillment pipeline with retries, dead-letter handling, and replay safety.
- Constraints:
  - eventual consistency accepted
  - no duplicate side effects
  - operator-friendly recovery runbook
- Expected sections in answer:
  - event contracts and ownership
  - idempotency and deduplication keys
  - retry/backoff + DLQ strategy
  - replay and reconciliation process

## Scoring rubric (10 points)

- Requirements framing and assumptions: 0-2
- Boundary/contract clarity: 0-2
- Failure and recovery strategy: 0-2
- Security and operability coverage: 0-2
- Tradeoff communication: 0-2

## Interview Answer Block

- 30-second answer: Strong whiteboard answers are constraint-first, boundary-clear, and explicit about failure handling and recovery.
- 2-minute deep dive: I capture assumptions, define service/data boundaries, choose a consistency strategy, then explain observability and rollout controls.
- Common follow-up: What would you change if traffic doubles in six months?
- Strong response: Prioritize bottleneck-aware scaling, contract stability, and progressive rollout with measurable SLO impact.
- Tradeoff callout: Over-detailing implementation internals early can hide architecture risk discussion.

## Interview Bad vs Strong Answer

- Bad answer: "I’d use microservices and queues and scale horizontally."
- Strong answer: "I’d define order/payment/inventory boundaries, use outbox+idempotency for async consistency, and enforce rollout/rollback gates with p95 and error-rate guardrails."
- Why strong wins: It links design choices to constraints, failure modes, and measurable outcomes.

## Interview Timed Drill

- Time box: 15 minutes.
- Prompt: Pick one whiteboard prompt and produce a one-page architecture response.
- Required outputs:
  - one boundary diagram summary (text form is fine)
  - one explicit failure and recovery path
  - one measurable acceptance metric
- Self-check score (0-3 each): structure, tradeoff quality, operational realism.
