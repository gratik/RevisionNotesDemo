# RevisionNotes.Microservice.CatalogService

A microservice-oriented API sample implementing catalog management with reliability and operational patterns that are common in distributed systems.

## Included capabilities

- JWT auth for write operations
- Catalog data access with EF Core InMemory
- Outbox pattern (append event + background dispatcher)
- Idempotency-key middleware for POST endpoints
- Output caching, response compression, rate limiting
- Health checks and secure headers

## Run

```bash
dotnet run --project Examples/RevisionNotes.Microservice.CatalogService/RevisionNotes.Microservice.CatalogService.csproj
```

## Write with idempotency

For `POST /api/catalog`, include `Idempotency-Key` header to prevent duplicate submissions.

## Architecture rationale

See `docs/Architecture-Decisions.md`.