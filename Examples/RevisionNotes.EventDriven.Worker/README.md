# RevisionNotes.EventDriven.Worker

Background worker demo for event-driven processing patterns.

## What it demonstrates

- In-memory queue using channels
- Idempotency handling for duplicate events
- Retry with incremental backoff
- Structured logging for producer/consumer flow
- Health checks (`live` + `ready`) evaluated by a health reporter service

## Run

```bash
dotnet run --project Examples/RevisionNotes.EventDriven.Worker/RevisionNotes.EventDriven.Worker.csproj
```

## Notes

This project intentionally simulates transient failures to demonstrate retry behavior.

See `docs/Architecture-Decisions.md` for design rationale.