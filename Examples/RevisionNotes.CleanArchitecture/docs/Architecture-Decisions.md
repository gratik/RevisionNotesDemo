# Architecture Decisions - RevisionNotes.CleanArchitecture

## WHAT IS THIS?
This is a clean-architecture-style API demo with explicit Domain, Application, and Infrastructure boundaries in a single deployable service.

## WHY IT MATTERS?
- Layered boundaries reduce coupling and make change safer as systems grow.
- This demo shows how to defer microservice complexity while preserving architectural discipline.

## WHEN TO USE?
- When you want clear separation of business rules from framework and persistence concerns.
- When the team needs testability and maintainability before scaling out deployment units.

## WHEN NOT TO USE?
- When the system is extremely small and extra layering adds unnecessary ceremony.
- When teams are not prepared to enforce boundary rules consistently.

## REAL-WORLD EXAMPLE?
A business operations API where order rules evolve frequently and must remain independent from UI and database implementation choices.

## Decision 1: Keep one deployable, separate layers in code

- Domain objects and application contracts live away from HTTP concerns.
- The API host composes dependencies and transports only.
- This keeps refactoring cost low while the product is still evolving.

## Decision 2: Application service owns orchestration

- `OrderService` controls use cases and cache invalidation.
- Controllers/endpoints stay thin and focus on request/response concerns.

## Decision 3: Use policy-based security

- Bearer authentication is configured once.
- Endpoints declare intent through `.RequireAuthorization("orders.readwrite")`.

## Decision 4: Build operational readiness into the baseline

- Global exception handling standardizes error responses.
- Health endpoints (`/health/live`, `/health/ready`) support orchestration platforms.
- Request logging captures method, path, status code, and duration.

## Decision 5: Start simple for data access and evolve safely

- In-memory repository keeps the sample focused on architecture concepts.
- Repository interface allows replacing storage without changing API contracts.
