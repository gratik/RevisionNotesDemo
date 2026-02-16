# RevisionNotes.MinimalApi

A best-practice Minimal API sample focused on secure defaults, data access patterns, performance controls, and clear architecture rationale.

## Included capabilities

- JWT authentication and authorization policy (`api.readwrite`)
- EF Core InMemory data access with repository abstraction
- In-memory query caching + output caching
- Rate limiting for write endpoints
- Response compression and security headers
- Health checks and OpenAPI

## Run

```bash
dotnet run --project Examples/RevisionNotes.MinimalApi/RevisionNotes.MinimalApi.csproj
```

## Get a token

`POST /auth/token` with:

```json
{ "username": "demo", "password": "ChangeMe!123" }
```

Use returned bearer token against `/api/todos` endpoints.

## Architecture rationale

See `docs/Architecture-Decisions.md`.