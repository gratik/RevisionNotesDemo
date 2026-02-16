# Architecture Decisions - RevisionNotes.Ddd.ModularMonolith

## WHAT IS THIS?
This is a modular monolith demo using DDD-inspired module boundaries (`Catalog`, `Billing`) and in-process domain events.

## WHY IT MATTERS?
- Teams can enforce clear business boundaries without immediate microservice overhead.
- Domain event flow between modules is explicit and testable.

## WHEN TO USE?
- When domains are distinct but operational simplicity of one deployable is preferred.
- When teams want microservice-ready boundaries without distributed complexity yet.

## WHEN NOT TO USE?
- When one module already needs independent scaling and deployment cadence.
- When boundaries are still unclear and heavy DDD structure would slow discovery.

## REAL-WORLD EXAMPLE?
A commerce platform where catalog and billing evolve separately, but are still shipped as one application during early growth.

## ADR-01: Modular boundaries in a single process
- Decision: Keep domain logic in separate module folders and services.
- Why: Preserves ownership and reduces accidental coupling.
- Tradeoff: Runtime isolation is weaker than separate services.

## ADR-02: In-process domain events
- Decision: Use a modular event bus to react across module boundaries.
- Why: Decouples module workflows while staying easy to debug locally.
- Tradeoff: No delivery durability across process restarts.

## ADR-03: Auth policy for module writes
- Decision: Protect module endpoints with one policy (`modules.readwrite`).
- Why: Keeps sensitive operations behind authentication by default.
- Tradeoff: Fine-grained role segmentation can be added later.
