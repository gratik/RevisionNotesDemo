# RevisionNotes.Workflows.SagaOrchestration

Saga orchestration demo with explicit step sequencing, failure handling, and compensation actions.

## Run

```bash
dotnet run --project Examples/RevisionNotes.Workflows.SagaOrchestration/RevisionNotes.Workflows.SagaOrchestration.csproj
```

Endpoints:
- `POST /auth/token`
- `POST /api/sagas/orders`
- `GET /api/sagas/orders/{sagaId}`
