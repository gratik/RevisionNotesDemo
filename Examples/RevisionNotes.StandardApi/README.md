# RevisionNotes.StandardApi

A controller-based API sample demonstrating enterprise API conventions while preserving the same security/performance baselines as the minimal API variant.

## Included capabilities

- Controller routing and explicit DTO contracts
- JWT authentication and authorization policy
- EF Core InMemory repository abstraction
- Memory caching + output cache invalidation on writes
- Rate limiting and response compression
- Health checks, OpenAPI, secure headers

## Run

```bash
dotnet run --project Examples/RevisionNotes.StandardApi/RevisionNotes.StandardApi.csproj
```

## Token endpoint

`POST /auth/token`

```json
{ "username": "demo", "password": "ChangeMe!123" }
```

Use the bearer token for `/api/todos`.

## Architecture rationale

See `docs/Architecture-Decisions.md`.