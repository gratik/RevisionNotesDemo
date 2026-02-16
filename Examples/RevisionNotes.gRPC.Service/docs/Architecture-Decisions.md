# Architecture Decisions - RevisionNotes.gRPC.Service

## WHAT IS THIS?
This is a secure gRPC service demo with protobuf contracts, interceptor-based error handling, and cached inventory data access.

## WHY IT MATTERS?
- Internal service-to-service communication benefits from strict contracts and efficient serialization.
- This demo shows how to combine gRPC performance with operational controls.

## WHEN TO USE?
- When internal clients need high-throughput, low-latency RPC calls.
- When strongly typed contracts across services are a priority.

## WHEN NOT TO USE?
- When browser-first public APIs without gRPC-Web are the primary requirement.
- When clients cannot support HTTP/2 or protobuf tooling.

## REAL-WORLD EXAMPLE?
An internal inventory service called by checkout and fulfillment services for stock reservation and streaming updates.

## ADR-01: gRPC for internal service communication
- Decision: Use gRPC unary + streaming methods.
- Why: Efficient binary protocol and strong contracts via protobuf.
- Tradeoff: Browser-native access is limited without gRPC-Web setup.

## ADR-02: Interceptor for centralized error policy
- Decision: Capture and map unhandled exceptions in one interceptor.
- Why: Consistent failure behavior and cleaner service methods.
- Tradeoff: Domain-specific error mapping still belongs in service layer for precision.

## ADR-03: JWT auth for RPC authorization
- Decision: Require valid bearer token for inventory operations.
- Why: Explicit client identity and policy enforcement.
- Tradeoff: Token issuance and rotation require secure operational process.

## ADR-04: Cached in-memory repository for demo data access
- Decision: Use in-memory data + IMemoryCache.
- Why: Fast local runs while illustrating read optimization.
- Tradeoff: No persistence or distributed consistency.

## ADR-05: HTTP health endpoints alongside gRPC
- Decision: Expose liveness/readiness over regular HTTP endpoints.
- Why: Works with standard orchestrator probes.
- Tradeoff: Requires securing health routes at network boundary in production.
