# Architecture Decisions - RevisionNotes.Microservice.CatalogService

## WHAT IS THIS?
This is a microservice-focused catalog API demo showing service boundaries, outbox-based event reliability, idempotency, and secure write operations.

## WHY IT MATTERS?
- Microservices fail in practice when boundaries and reliability patterns are vague.
- This demo highlights core patterns needed for safe distributed behavior.

## WHEN TO USE?
- When catalog ownership should be isolated from other domains.
- When asynchronous integration and independent deployment are required.

## WHEN NOT TO USE?
- When the product is early-stage and one modular monolith is still sufficient.
- When operational maturity for distributed systems is not yet available.

## REAL-WORLD EXAMPLE?
An e-commerce platform where the catalog service publishes product-change events consumed by search indexing and recommendation services.

## ADR-01: Single-responsibility service boundary
- Decision: Scope the service to catalog data only.
- Why: Keeps ownership clear and deployment independent.
- Tradeoff: Cross-domain workflows require asynchronous coordination.

## ADR-02: Outbox for reliable event publication
- Decision: Persist integration events to an outbox table and dispatch asynchronously.
- Why: Avoids lost events between DB commit and message publish.
- Tradeoff: Event delivery is eventually consistent, not immediate.

## ADR-03: Idempotency key protection on writes
- Decision: Middleware rejects duplicate POST requests with same key.
- Why: Handles retries from clients/gateways safely.
- Tradeoff: Requires clients to generate and persist keys.

## ADR-04: JWT and policy-based authorization
- Decision: Allow anonymous reads, authenticated writes.
- Why: Common public-catalog/private-admin model.
- Tradeoff: Fine-grained role mapping may need expansion.

## ADR-05: Operational defaults
- Decision: Add health checks, secure headers, rate limiting, and compression.
- Why: Improves safety and platform readiness from day one.
- Tradeoff: Policy thresholds require environment-specific tuning.

## ADR-06: InMemory database for learning project
- Decision: Keep sample self-contained and dependency-free.
- Why: Fast onboarding and deterministic local runs.
- Tradeoff: No relational or transactional behavior equivalent to production stores.
