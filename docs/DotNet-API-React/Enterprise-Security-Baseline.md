# Enterprise Security Baseline

> Subject: [DotNet-API-React](../README.md)

## Enterprise Security Baseline

### Identity and token strategy

- Prefer OAuth2/OIDC with short-lived access tokens.
- Validate issuer, audience, signature, and expiry on every API request.
- Enforce scope/role policies per endpoint (`orders.read`, `orders.write`).
- Do not log bearer tokens, refresh tokens, or raw secrets.

### SPA security controls

| Control | Why it matters | Typical implementation |
| --- | --- | --- |
| HTTPS + HSTS | Prevent downgrade and traffic interception | `UseHttpsRedirection`, `UseHsts` |
| Strict CORS origins | Limit browser cross-origin access | `WithOrigins(...)` only |
| Rate limiting | Reduce abuse and brute force pressure | `AddRateLimiter` policy by path |
| Input validation | Prevent malformed/abusive requests | model validation + bounded query params |
| Secret hygiene | Prevent credential leaks | Key Vault / secret manager + rotation |

### API security pipeline (reference)

See `GoodApiSecurityBaseline` in [React API Integration Examples](../../Learning/FrontEnd/ReactApiIntegrationExamples.cs).



