# Architecture Decisions - RevisionNotes.EventSourcing.Cqrs

## WHAT IS THIS?
This is an event-sourcing and CQRS demo where command writes append domain events and query reads come from projections.

## WHY IT MATTERS?
- Event history provides full auditability and replay capability.
- CQRS enables read models optimized independently from write models.

## WHEN TO USE?
- When audit trail and temporal history are core requirements.
- When read and write workloads differ significantly.

## WHEN NOT TO USE?
- When domain complexity is low and CRUD is sufficient.
- When event schema/versioning operations are not feasible for the team yet.

## REAL-WORLD EXAMPLE?
A financial ledger where all balance changes are stored as immutable events and read views are rebuilt for reporting.

## ADR-01: Append-only event store
- Decision: Persist account changes as immutable events.
- Why: Enables replay and precise historical reconstruction.
- Tradeoff: Requires projection logic for efficient reads.

## ADR-02: Separate command and query endpoints
- Decision: Keep write and read API surfaces distinct.
- Why: Clarifies intent and supports independent scaling strategies.
- Tradeoff: Additional architectural complexity compared to CRUD APIs.
