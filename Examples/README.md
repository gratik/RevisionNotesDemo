# Examples Portfolio

This solution now includes four standalone architecture examples:

1. `RevisionNotes.BlazorBestPractices`
   - Path: `Examples/RevisionNotes.BlazorBestPractices`
   - Focus: Accessible Blazor Server UI, secure cookie auth, data access, caching, performance controls

2. `RevisionNotes.MinimalApi`
   - Path: `Examples/RevisionNotes.MinimalApi`
   - Focus: Minimal API best practices with JWT auth, repository pattern, output caching, rate limiting

3. `RevisionNotes.StandardApi`
   - Path: `Examples/RevisionNotes.StandardApi`
   - Focus: Controller-based API architecture with explicit contracts, security, caching, rate limits

4. `RevisionNotes.Microservice.CatalogService`
   - Path: `Examples/RevisionNotes.Microservice.CatalogService`
   - Focus: Microservice service boundary, outbox pattern, idempotency, secure write APIs

## Run each project

```bash
dotnet run --project Examples/RevisionNotes.BlazorBestPractices/RevisionNotes.BlazorBestPractices.csproj
dotnet run --project Examples/RevisionNotes.MinimalApi/RevisionNotes.MinimalApi.csproj
dotnet run --project Examples/RevisionNotes.StandardApi/RevisionNotes.StandardApi.csproj
dotnet run --project Examples/RevisionNotes.Microservice.CatalogService/RevisionNotes.Microservice.CatalogService.csproj
```

Each project contains:

- `README.md` for usage and capabilities
- `docs/Architecture-Decisions.md` for architecture rationale and tradeoffs