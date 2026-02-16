# Architecture Decisions - RevisionNotes.ApiGateway.BFF

## WHAT IS THIS?
This is a Backend-for-Frontend gateway demo that aggregates data from downstream services into client-focused responses.

## WHY IT MATTERS?
- Frontend clients often need stitched responses from multiple APIs.
- A BFF can reduce client complexity and centralize auth/aggregation behavior.

## WHEN TO USE?
- When web/mobile clients need tailored payloads and orchestration logic.
- When downstream services should stay focused on domain concerns.

## WHEN NOT TO USE?
- When a single domain API already provides all needed data.
- When adding a gateway layer would create unnecessary latency and operational overhead.

## REAL-WORLD EXAMPLE?
A customer dashboard endpoint that combines profile, orders, and notifications into one response optimized for a web app.

## ADR-01: Aggregate profile and orders in one endpoint
- Decision: Build `/api/bff/dashboard` using fan-out calls to internal clients.
- Why: Reduce round trips and simplify frontend data composition.
- Tradeoff: Gateway latency depends on the slowest downstream call.

## ADR-02: Fallback to cached aggregate on failure
- Decision: Return recent cached dashboard on downstream exceptions.
- Why: Improves resilience and UX continuity.
- Tradeoff: Fallback payload can be stale.
