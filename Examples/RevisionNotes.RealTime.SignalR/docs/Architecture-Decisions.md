# Architecture Decisions - RevisionNotes.RealTime.SignalR

## WHAT IS THIS?
This is a real-time communication demo using SignalR with authenticated hub connections, group messaging, and message history retrieval.

## WHY IT MATTERS?
- Real-time features require connection lifecycle, auth, and backpressure considerations beyond normal REST endpoints.
- This demo shows secure defaults and operational patterns for hub-based workloads.

## WHEN TO USE?
- When users need live updates, chat, notifications, or collaborative signals.
- When server-push events improve UX over polling.

## WHEN NOT TO USE?
- When updates are infrequent and simple polling is sufficient.
- When clients cannot keep long-lived connections reliably.

## REAL-WORLD EXAMPLE?
A support dashboard where agents join team channels and receive live customer-event notifications in real time.

## ADR-01: SignalR hub for bidirectional messaging
- Decision: Use hub groups for channel-oriented message distribution.
- Why: Built-in connection management and efficient fan-out.
- Tradeoff: Requires connection and scale-out planning in production.

## ADR-02: JWT-based authentication for hub and APIs
- Decision: Require authenticated users for `/hubs/notifications` and history APIs.
- Why: Prevents anonymous broadcast abuse and improves traceability.
- Tradeoff: Token lifecycle management must be handled correctly by clients.

## ADR-03: In-memory history for learning simplicity
- Decision: Keep recent messages in an in-memory queue.
- Why: Easy local setup and deterministic behavior for demo usage.
- Tradeoff: No persistence across restarts and no cross-node consistency.
