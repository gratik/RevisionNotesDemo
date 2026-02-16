# RevisionNotes.RealTime.SignalR

SignalR demo for authenticated real-time group messaging with message history, health checks, and operational middleware.

## Highlights

- JWT-secured hub connections
- Group messaging with server-side validation
- In-memory message history API
- Global error handling and request logging

## Run

```bash
dotnet run --project Examples/RevisionNotes.RealTime.SignalR/RevisionNotes.RealTime.SignalR.csproj
```

## Key endpoints

- `POST /auth/token`
- `GET /api/chat/history`
- `GET /health`, `/health/live`, `/health/ready`
- Hub: `/hubs/notifications`
