# RevisionNotes.Observability.Showcase

Observability-focused API demo for practical tracing, metrics, logs, and error analysis workflows.

## What it demonstrates

- Request-level structured logging middleware
- Manual tracing using `ActivitySource`
- Metrics using `Meter`, `Counter`, and `Histogram`
- Centralized exception handling with ProblemDetails responses
- Liveness/readiness health check endpoints

## Key endpoints

- `GET /observability/success`
- `GET /observability/slow?delayMs=750`
- `GET /observability/failure`
- `GET /observability/log-levels`
- `GET /observability/metrics/snapshot`
- `GET /health`, `/health/live`, `/health/ready`

## Run

```bash
dotnet run --project Examples/RevisionNotes.Observability.Showcase/RevisionNotes.Observability.Showcase.csproj
```

See `docs/Architecture-Decisions.md` for design rationale.