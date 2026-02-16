# Architecture Decisions - RevisionNotes.Workflows.SagaOrchestration

## WHAT IS THIS?
This is a saga orchestration demo for long-running workflows with explicit step execution and compensating actions.

## WHY IT MATTERS?
- Multi-step business transactions across services cannot rely on single ACID transactions.
- Sagas provide a practical pattern for eventual consistency with failure recovery.

## WHEN TO USE?
- When workflows span inventory, payment, shipping, or other independent systems.
- When partial failure handling must be explicit and recoverable.

## WHEN NOT TO USE?
- When all operations are local and a single database transaction is sufficient.
- When eventual consistency is not acceptable for the use case.

## REAL-WORLD EXAMPLE?
An order placement flow that reserves stock, charges payment, and compensates by refunding/releasing stock if payment fails.

## ADR-01: Orchestrator-controlled saga
- Decision: Central orchestrator service drives step order and error handling.
- Why: Keeps workflow logic explicit and traceable.
- Tradeoff: Orchestrator can become a central dependency if not modularized.

## ADR-02: Compensating actions on failure
- Decision: On failed payment, run refund/release compensation.
- Why: Restores consistency across distributed operations.
- Tradeoff: Compensation complexity grows with workflow breadth.
