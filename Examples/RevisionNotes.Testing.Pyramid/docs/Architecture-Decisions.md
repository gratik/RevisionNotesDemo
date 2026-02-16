# Architecture Decisions - RevisionNotes.Testing.Pyramid

## WHAT IS THIS?
This is a testing-oriented API demo structured to teach the test pyramid through unit, integration, and contract-friendly design.

## WHY IT MATTERS?
- Many projects struggle with brittle tests because architecture is not designed for testability.
- This demo provides stable contracts and isolated domain logic to support reliable automation.

## WHEN TO USE?
- When you want a reference implementation for layered testing strategy.
- When you need deterministic endpoints for consumer contract tests.

## WHEN NOT TO USE?
- When speed of prototyping is the only goal and long-term test maintenance is not needed.
- When interfaces and deterministic fixtures would be unnecessary overhead.

## REAL-WORLD EXAMPLE?
A payments integration API where contract tests protect downstream consumers while unit tests validate risk scoring rules.

## Decision 1: Design endpoints for testability first

- Contract endpoints are deterministic and versioned (`/contract/v1/...`).
- Error payloads are explicit to avoid brittle parsing in tests.

## Decision 2: Keep domain logic framework-agnostic

- `OrderScoringService` has no HTTP or persistence dependencies.
- Unit tests can run without host startup.

## Decision 3: Use in-memory data for repeatable integration tests

- Startup is fast and deterministic.
- Test suites avoid external service flakiness by default.

## Decision 4: Treat security and operations as part of the pyramid

- Contract endpoints still require auth to test real behavior.
- Health checks and consistent exception handling are part of integration test scope.

## Decision 5: Enable incremental hardening

- In-memory store abstraction can be swapped to EF Core/Testcontainers without changing endpoint contracts.
