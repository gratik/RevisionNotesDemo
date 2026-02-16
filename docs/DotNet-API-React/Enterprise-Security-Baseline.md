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

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for Enterprise Security Baseline before implementation work begins.
- Keep boundaries explicit so Enterprise Security Baseline decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Enterprise Security Baseline in production-facing code.
- When performance, correctness, or maintainability depends on consistent Enterprise Security Baseline decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Enterprise Security Baseline as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Enterprise Security Baseline is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Enterprise Security Baseline are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

