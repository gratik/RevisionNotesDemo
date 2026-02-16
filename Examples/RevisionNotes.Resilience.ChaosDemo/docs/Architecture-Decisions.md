# Architecture Decisions - RevisionNotes.Resilience.ChaosDemo

## WHAT IS THIS?
This is a resilience-focused API demo with fault injection, retry/timeout handling, circuit-breaker logic, and fallback caching.

## WHY IT MATTERS?
- Distributed dependencies fail in unpredictable ways and can cascade outages.
- This demo shows core resilience controls needed to keep services responsive under failure.

## WHEN TO USE?
- When your service depends on unstable or high-latency downstream systems.
- When teams need to validate fallback behavior before incidents.

## WHEN NOT TO USE?
- When the system has no external dependencies and fault-handling complexity is unnecessary.
- When strict consistency forbids cached fallback data.

## REAL-WORLD EXAMPLE?
An order summary API that depends on inventory and pricing services and must continue serving degraded responses during partial outages.

## ADR-01: Chaos controls built into runtime
- Decision: Expose adjustable fault injection settings via API.
- Why: Enables repeatable resilience testing in development/staging.
- Tradeoff: Must be strictly controlled or disabled in production.

## ADR-02: Retry + timeout before fallback
- Decision: Attempt dependency call up to three times with timeout guards.
- Why: Handles transient spikes while limiting request latency.
- Tradeoff: Poorly tuned retries can amplify load during incidents.

## ADR-03: Circuit-breaker with cached fallback
- Decision: Open circuit after repeated failures and serve last known good cached value.
- Why: Prevents constant hammering of failing dependencies.
- Tradeoff: Fallback data can become stale until dependency recovers.
