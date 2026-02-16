# RevisionNotes.Ddd.ModularMonolith

DDD-oriented modular monolith demo with separate `Catalog` and `Billing` modules in one deployable API.

## Highlights

- Explicit module boundaries in code
- In-process domain event bus between modules
- JWT auth and policy-based endpoint protection
- Health checks, global exception handling, and request logging

## Run

```bash
dotnet run --project Examples/RevisionNotes.Ddd.ModularMonolith/RevisionNotes.Ddd.ModularMonolith.csproj
```

## Key endpoints

- `POST /auth/token`
- `GET|POST /api/catalog`
- `GET|POST /api/billing/invoices`
- `GET /api/events`
