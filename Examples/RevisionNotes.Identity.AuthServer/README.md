# RevisionNotes.Identity.AuthServer

Identity-focused API demo that shows token issuance, refresh token rotation, and protected resources.

## Highlights

- Password grant style token endpoint for learning scenarios
- Refresh token flow with one-time token consumption
- JWT bearer validation for protected APIs
- Global error handling and operational health endpoints
- Request logging for troubleshooting and auditing

## Run

```bash
dotnet run --project Examples/RevisionNotes.Identity.AuthServer/RevisionNotes.Identity.AuthServer.csproj
```

## Quick start

1. Request initial token:
```http
POST /connect/token
{
  "grantType": "password",
  "username": "identity-admin",
  "password": "ChangeMe!123"
}
```
2. Refresh token:
```http
POST /connect/refresh
{
  "grantType": "refresh_token",
  "refreshToken": "<refresh token>"
}
```
3. Call protected resource:
- `GET /api/profile` with `Authorization: Bearer <token>`

## Notes

- This project is intentionally simple and self-contained for learning.
- For production, use hardened key storage, full OIDC flows, and persistent refresh-token storage.
