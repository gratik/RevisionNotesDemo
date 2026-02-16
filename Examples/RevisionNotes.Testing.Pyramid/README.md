# RevisionNotes.Testing.Pyramid

API demo intentionally shaped for unit, integration, and contract testing exercises.

## Highlights

- Deterministic contract endpoint for stable consumer tests
- Isolated domain service for straightforward unit tests
- In-memory store for fast integration test bootstrapping
- JWT-protected contract surface
- Health checks, request logging, and centralized error handling

## Run

```bash
dotnet run --project Examples/RevisionNotes.Testing.Pyramid/RevisionNotes.Testing.Pyramid.csproj
```

## Suggested test layers

- Unit: `OrderScoringService.CalculateRiskScore`
- Integration: `/contract/v1/orders/{id}` with in-memory host
- Contract: response schema and status behavior for success/not-found

## Demo endpoints

- `POST /auth/token`
- `GET /contract/v1/orders/{id}`
- `POST /unit/score-order`
