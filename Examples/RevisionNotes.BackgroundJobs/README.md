# RevisionNotes.BackgroundJobs

Background processing demo showing queued jobs, retries, idempotency, and health reporting for worker-based workloads.

## Highlights

- In-memory queue with producer/consumer services
- Duplicate protection with idempotency store
- Bounded retry policy for transient failures
- Health check reporting for non-HTTP worker process

## Run

```bash
dotnet run --project Examples/RevisionNotes.BackgroundJobs/RevisionNotes.BackgroundJobs.csproj
```
