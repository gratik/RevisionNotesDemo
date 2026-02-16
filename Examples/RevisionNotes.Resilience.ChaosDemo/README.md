# RevisionNotes.Resilience.ChaosDemo

Resilience demo API showing retry, timeout, circuit breaker behavior, fallback cache, and configurable fault injection.

## Highlights

- Configurable chaos settings for failure rate and delay
- Retry with timeout per call
- Simple circuit breaker state tracking
- Cached fallback response when dependency is unstable

## Run

```bash
dotnet run --project Examples/RevisionNotes.Resilience.ChaosDemo/RevisionNotes.Resilience.ChaosDemo.csproj
```

Endpoints:
- `POST /auth/token`
- `GET /api/resilience/value`
- `GET|POST /api/chaos/config`
