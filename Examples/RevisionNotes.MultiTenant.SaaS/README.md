# RevisionNotes.MultiTenant.SaaS

Multi-tenant API demo showing tenant resolution, tenant-scoped data isolation, authenticated access, and operational baselines.

## Highlights

- Tenant resolution via `X-Tenant-Id` header
- Per-tenant in-memory data partitioning
- JWT auth and authorization policy
- Health checks, global exception handling, request logging

## Run

```bash
dotnet run --project Examples/RevisionNotes.MultiTenant.SaaS/RevisionNotes.MultiTenant.SaaS.csproj
```

## Usage

1. Get token from `POST /auth/token`.
2. Call APIs with:
   - `Authorization: Bearer <token>`
   - `X-Tenant-Id: tenant-a`

Endpoints:
- `GET /api/tenant/info`
- `GET /api/tenant/projects`
- `POST /api/tenant/projects`
