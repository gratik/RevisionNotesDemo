# RevisionNotes.NativeAot.Api

Native AOT-oriented API demo with trimming-friendly patterns, API-key security, health checks, and minimal runtime footprint.

## Run

```bash
dotnet run --project Examples/RevisionNotes.NativeAot.Api/RevisionNotes.NativeAot.Api.csproj
```

## Publish AOT

```bash
dotnet publish Examples/RevisionNotes.NativeAot.Api/RevisionNotes.NativeAot.Api.csproj -c Release
```

## Security

Use `X-Api-Key: ChangeMe-Aot-ApiKey` for `/api/*` endpoints.
