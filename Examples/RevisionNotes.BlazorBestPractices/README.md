# RevisionNotes.BlazorBestPractices

A Blazor Server sample showing UI accessibility, secure defaults, data access patterns, and performance/caching controls.

## Included capabilities

- Server-side Blazor with cookie authentication and role policy
- Accessible layout (skip link, semantic structure, aria live status)
- EF Core InMemory with repository + cached query service
- API endpoints with output cache and rate limiting
- Security headers, HTTPS/HSTS, antiforgery
- Health checks and documented architecture decisions

## Run

```bash
dotnet run --project Examples/RevisionNotes.BlazorBestPractices/RevisionNotes.BlazorBestPractices.csproj
```

## Demo auth flow

Use the nav button **Demo Sign-In** to create an admin cookie. Then browse `/admin`.

## Architecture rationale

See `docs/Architecture-Decisions.md`.