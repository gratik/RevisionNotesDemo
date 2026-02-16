# RevisionNotes.ApiGateway.BFF

Backend-for-Frontend demo showing response aggregation, downstream fallback, and a secure gateway surface.

## Run

```bash
dotnet run --project Examples/RevisionNotes.ApiGateway.BFF/RevisionNotes.ApiGateway.BFF.csproj
```

Endpoints:
- `POST /auth/token`
- `GET /api/bff/dashboard`
