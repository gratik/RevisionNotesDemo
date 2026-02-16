# RevisionNotes.gRPC.Service

gRPC demo service focused on secure RPC patterns, interceptor-based error handling, and operational readiness.

## What it demonstrates

- gRPC unary and server-streaming endpoints
- JWT auth policy on gRPC methods
- Interceptor for centralized exception handling and request logging
- In-memory repository with short-lived cache
- Health endpoints (`/health`, `/health/live`, `/health/ready`)

## Run

```bash
dotnet run --project Examples/RevisionNotes.gRPC.Service/RevisionNotes.gRPC.Service.csproj
```

## Auth

Get bearer token from `POST /auth/token`:

```json
{ "username": "demo", "password": "ChangeMe!123" }
```

Pass token as `Authorization: Bearer <token>` metadata in gRPC calls.

See `docs/Architecture-Decisions.md` for design rationale.