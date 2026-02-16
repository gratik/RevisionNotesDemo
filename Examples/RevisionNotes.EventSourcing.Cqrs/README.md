# RevisionNotes.EventSourcing.Cqrs

CQRS + event sourcing demo with command endpoints writing append-only events and query endpoints reading from projections.

## Run

```bash
dotnet run --project Examples/RevisionNotes.EventSourcing.Cqrs/RevisionNotes.EventSourcing.Cqrs.csproj
```

Endpoints:
- `POST /auth/token`
- `POST /api/commands/accounts`
- `POST /api/commands/accounts/{accountId}/deposit`
- `GET /api/queries/accounts/{accountId}`
- `GET /api/queries/accounts/{accountId}/events`
