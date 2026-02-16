# .NET API to React Front End

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Web API and MVC, Front-End DotNet UI
- Related examples: Learning/FrontEnd/ReactApiIntegrationExamples.cs, Learning/WebAPI/Middleware/MiddlewareBestPractices.cs, Learning/Security/SecureAPIDesignPatterns.cs, Learning/Observability/OpenTelemetrySetup.cs

Common production scenario: a React SPA consumes an ASP.NET Core API hosted as a separate service.

## Module Metadata

- **Prerequisites**: `Web-API-MVC.md`, `Front-End-DotNet-UI.md`
- **When to Study**: When designing a SPA + API boundary or preparing for integration interviews.
- **Related Files**: `../Learning/FrontEnd/ReactApiIntegrationExamples.cs`, `../Learning/WebAPI/Middleware/MiddlewareBestPractices.cs`
- **Estimated Time**: 45-60 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../Web-API-MVC.md)
- **Next Step**: [DotNet API to Vue Front End](../DotNet-API-Vue.md)
<!-- STUDY-NAV-END -->

## Scenario Architecture

```text
React SPA (Vite) -> API Client Layer -> ASP.NET Core API -> Application/Domain -> Data Access
```

Keep contracts at the API boundary stable through DTOs and explicit versioning policy.

## Best Practices

1. Use a shared API client (`fetch` wrapper or axios) instead of calling endpoints directly in components.
2. Handle cancellation via `AbortController` for unmounted components.
3. Return `ProblemDetails` from API for predictable frontend error rendering.
4. Keep CORS origin list explicit for production and local development.
5. Pass `CancellationToken` through API -> service -> repository chain.
6. Use DTO projection for list pages to avoid over-fetching.
7. Treat auth token handling and refresh behavior as infrastructure concerns.

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

## Logging and Observability Baseline

### Structured logging requirements

1. Include `TraceId`, `TenantId` (if multi-tenant), `Route`, and operation name in log scope.
2. Log business-significant events (order created, payment failed) with stable event ids.
3. Keep log payload bounded; redact PII and credential-bearing fields.
4. Use warning/error levels for actionable states only to reduce noise.

### End-to-end telemetry model

| Signal | API source | React source | Purpose |
| --- | --- | --- | --- |
| Request latency (p95/p99) | middleware + OpenTelemetry | page timing metric | User-perceived performance |
| Error rate by route | exception/problem details pipeline | UI error boundary metrics | Reliability and incident detection |
| Throughput | request counters | page interaction counters | Capacity and release impact |
| Trace correlation | `Activity.TraceId` | `X-Trace-Id` header propagation | Fast cross-tier incident triage |

### Logging/tracing snippets

- API reference: `GoodApiLoggingBaseline` in [React API Integration Examples](../../Learning/FrontEnd/ReactApiIntegrationExamples.cs)
- Frontend reference: `GoodReactTelemetry` in [React API Integration Examples](../../Learning/FrontEnd/ReactApiIntegrationExamples.cs)

## Structural Necessities (Enterprise)

### Recommended solution boundaries

```text
src/
  Api/                 # Controllers/endpoints + transport concerns
  Application/         # Use cases, orchestration, DTO mapping
  Domain/              # Business rules and invariants
  Infrastructure/      # EF, external services, messaging, auth adapters
  Frontend.React/      # React app (features, services, shared UI)
  Contracts/           # API contracts/OpenAPI schemas
tests/
  Api.Tests/
  Application.Tests/
  Contract.Tests/      # API compatibility checks
  Frontend.Tests/      # component + integration tests
```

### Operational and governance requirements

- Contract governance: OpenAPI contract published and versioned in CI artifacts.
- Release safety: blue/green or canary deployment with rollback trigger thresholds.
- Security gates: SAST, dependency scanning, and secret scanning in pipeline.
- Configuration governance: environment-specific config with audited overrides.
- Incident readiness: runbooks for auth failures, CORS incidents, and latency regressions.

### Testing minimum for this architecture

| Layer | Required tests |
| --- | --- |
| API | authz policy tests, input validation tests, error envelope tests |
| Application | business rule unit tests, idempotency tests |
| Contract | schema compatibility tests across versions |
| Frontend | component state tests + API integration tests with mock server |
| E2E | login + critical user journey + failure state path |

## Backend Patterns (.NET)

### API contract and error envelope

```csharp
[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IReadOnlyList<OrderDto>> Get(CancellationToken ct)
        => await _service.GetOrdersAsync(ct);
}
```

### CORS with explicit origins

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("SpaPolicy", policy => policy
        .WithOrigins("http://localhost:5173", "https://app.example.com")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials());
});

app.UseRouting();
app.UseCors("SpaPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
```

## Frontend Patterns (React)

### Shared API client

```ts
const API_BASE = import.meta.env.VITE_API_BASE_URL;

export async function apiGet(path: string, token?: string, signal?: AbortSignal) {
  const response = await fetch(`${API_BASE}${path}`, {
    headers: {
      Accept: "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {}),
    },
    signal,
  });

  if (!response.ok) {
    const details = await response.json().catch(() => ({ title: "Request failed", status: response.status }));
    throw new Error(`${details.title} (${details.status})`);
  }

  return await response.json();
}
```

### Hook with cancellation

```ts
export function useOrders(token?: string) {
  const [orders, setOrders] = useState<OrderDto[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    const controller = new AbortController();
    setIsLoading(true);

    apiGet("/api/orders", token, controller.signal)
      .then(setOrders)
      .catch((err) => err.name !== "AbortError" && setError(err.message))
      .finally(() => setIsLoading(false));

    return () => controller.abort();
  }, [token]);

  return { orders, isLoading, error };
}
```

## Common Failure Modes

| Failure mode | Root cause | Prevention |
| --- | --- | --- |
| Random CORS errors | Middleware order or missing origin | Explicit policy + correct middleware order |
| Inconsistent errors in UI | Mixed backend error format | Standard `ProblemDetails` envelope |
| Memory leaks/warnings in React | Uncanceled async requests | `AbortController` in effect cleanup |
| Slow list pages | Returning full entities | API-side projection + paging DTO |

## Validation Checklist

- Endpoint returns typed DTOs and explicit status annotations.
- React client path does not duplicate base URL string literals.
- Non-2xx responses are parsed into user-facing error state.
- Local dev and production origins are both represented in CORS policy.
- Paging/filter constraints are validated server-side.
- Token validation and authorization policies are explicit and test-covered.
- Logs include correlation identifiers and avoid sensitive payload data.
- Telemetry dashboard includes p95 latency, route error rate, and deployment markers.

## References

- Example code: [React API Integration Examples](../../Learning/FrontEnd/ReactApiIntegrationExamples.cs)
- Related API middleware guidance: [Web API and MVC](../Web-API-MVC.md)
- Related UI comparisons: [Front-End DotNet UI](../Front-End-DotNet-UI.md)
- Related security guidance: [Security](../Security.md)
- Related observability guidance: [Logging and Observability](../Logging-Observability.md)

## Interview Answer Block

- 30-second answer: For React + .NET API, I enforce contract-first DTOs, explicit CORS, shared client utilities, and predictable error envelopes.
- 2-minute deep dive: I keep components thin, centralize API access, propagate cancellation tokens end-to-end, and validate behavior with latency/error metrics from both UI and API.
- Common follow-up: How do you avoid frontend/backend drift?
- Strong response: Use OpenAPI-generated clients or typed contract tests, version intentionally, and fail CI when contract checks break.
- Tradeoff callout: Over-abstracting the frontend client too early can slow iteration if the API surface is still unstable.

## Interview Bad vs Strong Answer

- Bad answer: "React calls the API with fetch and we show data on the page."
- Strong answer: "We use a shared API client with auth/cancellation/error normalization, explicit CORS policy, and DTO-first contracts with measurable reliability checks."
- Why strong wins: It shows system-level thinking beyond simple request/response mechanics.

## Interview Timed Drill

- Time box: 12 minutes.
- Prompt: Design the React-to-.NET integration for an orders dashboard with auth and pagination.
- Required outputs:
  - one API DTO and endpoint contract choice
  - one frontend state/error/cancellation strategy
  - one production metric and alert threshold
- Self-check score (0-3 each): contract clarity, reliability design, operational readiness.

## Topic Files

- [Scenario Architecture](Scenario-Architecture.md)
- [Best Practices](Best-Practices.md)
- [Enterprise Security Baseline](Enterprise-Security-Baseline.md)
- [Logging and Observability Baseline](Logging-and-Observability-Baseline.md)
- [Structural Necessities (Enterprise)](Structural-Necessities-Enterprise.md)
- [Backend Patterns (.NET)](Backend-Patterns-NET.md)
- [Frontend Patterns (React)](Frontend-Patterns-React.md)
- [Common Failure Modes](Common-Failure-Modes.md)
- [Validation Checklist](Validation-Checklist.md)
- [References](References.md)



