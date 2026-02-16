# RevisionNotes.DataAccess.AdvancedEfCore

EF Core data-access demo focused on advanced patterns: compiled queries, split queries, soft delete, and optimistic concurrency.

## Highlights

- Compiled query for repeat read path
- Split query for aggregate loading with related data
- Soft-delete strategy via global query filter
- Optimistic concurrency using version checks

## Run

```bash
dotnet run --project Examples/RevisionNotes.DataAccess.AdvancedEfCore/RevisionNotes.DataAccess.AdvancedEfCore.csproj
```

Endpoints:
- `POST /auth/token`
- `GET|POST /api/products`
- `GET|PUT|DELETE /api/products/{id}`
