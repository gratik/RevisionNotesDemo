# RevisionNotes.CleanArchitecture

Clean Architecture demo using a single deployable API while keeping Domain, Application, and Infrastructure concerns isolated in code.

## Highlights

- Layered structure for maintainability and testability
- JWT authentication and explicit authorization policy
- Cached read path in application service
- Centralized exception handling with `ProblemDetails`
- Health checks for liveness/readiness and request logging

## Run

```bash
dotnet run --project Examples/RevisionNotes.CleanArchitecture/RevisionNotes.CleanArchitecture.csproj
```

## Quick start

1. Request a token:
```http
POST /auth/token
{
  "username": "architect",
  "password": "ChangeMe!123"
}
```
2. Use the bearer token against:
- `GET /api/orders`
- `GET /api/orders/{id}`
- `POST /api/orders`

## Notes

- `InMemoryOrderRepository` stands in for data access; replace with EF Core or Dapper in real deployments.
- `docs/Architecture-Decisions.md` explains why this architecture is useful before splitting into microservices.
