# Enterprise Security Baseline

> Subject: [DotNet-API-Vue](../README.md)

## Enterprise Security Baseline

### Identity and authorization model

- Use OAuth2/OIDC with short-lived access tokens and explicit audience validation.
- Enforce route-level authorization policies (`products.read`, `products.write`).
- Treat token refresh as an authentication infrastructure concern, not a view concern.
- Keep API authorization independent from UI role labels to avoid coupling.

### SPA and API hardening controls

| Control | Why it matters | Typical implementation |
| --- | --- | --- |
| HTTPS everywhere | Prevent credential/session interception | HSTS + TLS termination standards |
| Narrow CORS policy | Prevent untrusted browser origins | explicit `WithOrigins(...)` |
| Rate limiting | Bound abusive traffic and bot bursts | route-based limiter policy |
| Validation guardrails | Prevent expensive malformed queries | bounded `page/pageSize/filter` inputs |
| Secret rotation | Lower blast radius of leaked credentials | managed secret store + rotation cadence |

### Security baseline snippet

See `GoodApiSecurityPosture` in [Vue API Integration Examples](../../Learning/FrontEnd/VueApiIntegrationExamples.cs).



