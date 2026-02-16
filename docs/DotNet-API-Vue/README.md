# .NET API to Vue Front End

## Metadata
- Owner: Maintainers
- Last updated: February 15, 2026
- Prerequisites: Web API and MVC, Front-End DotNet UI
- Related examples: Learning/FrontEnd/VueApiIntegrationExamples.cs, Learning/WebAPI/Middleware/MiddlewareBestPractices.cs, Learning/Security/SecureAPIDesignPatterns.cs, Learning/Observability/OpenTelemetrySetup.cs

Common production scenario: a Vue SPA consumes an ASP.NET Core API as an independent backend service.

## Module Metadata

- **Prerequisites**: `Web-API-MVC.md`, `Front-End-DotNet-UI.md`
- **When to Study**: When building Vue SPA integrations with robust API contracts and operational guardrails.
- **Related Files**: `../Learning/FrontEnd/VueApiIntegrationExamples.cs`, `../Learning/WebAPI/Middleware/MiddlewareBestPractices.cs`
- **Estimated Time**: 45-60 minutes

<!-- STUDY-NAV-START -->
## Navigation

- **Start Here**: [Learning Path](../Learning-Path.md) | [Track Start](../Web-API-MVC.md)
- **Next Step**: [Interview Preparation](../Interview-Preparation.md)
<!-- STUDY-NAV-END -->

## Scenario Architecture

```text
Vue SPA (Vite) -> API Service/Composable Layer -> ASP.NET Core API -> Application/Domain -> Data Access
```

Prefer composables for request/state orchestration and keep view components presentation-focused.

## Best Practices

1. Use a shared axios instance with request/response interceptors.
2. Keep token injection and error normalization in one place.
3. Implement composables (`useProducts`, `useOrders`) for loading/error lifecycle.
4. Use Vite proxy in local dev to reduce CORS drift.
5. Return validation and fault details as standard `ProblemDetails`.
6. Enforce server-side pagination boundaries (`pageSize` limits).
7. Keep response payloads small via list-specific DTOs.

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

## Logging and Observability Baseline

### Logging standards

1. Use structured logs with stable property names (`TraceId`, `Route`, `TenantId`).
2. Log at boundaries: request received, domain decision, external dependency call, response sent.
3. Avoid sensitive fields (tokens, passwords, raw personal data).
4. Use consistent event ids for alert mapping and post-incident analysis.

### Required telemetry signals

| Signal | API source | Vue source | Purpose |
| --- | --- | --- | --- |
| p95/p99 latency by endpoint | ASP.NET middleware metrics | page-level load timing | detect degradation quickly |
| Error rate by status code | exception/ProblemDetails pipeline | UI error tracking | reliability and incident severity |
| Dependency latency | HTTP/DB instrumentation | client retry/error distribution | isolate backend dependency bottlenecks |
| Trace correlation | W3C trace context + `TraceId` | `X-Trace-Id` propagation | cross-tier troubleshooting |

### Logging/tracing snippets

- API reference: `GoodLoggingAndTracing` in [Vue API Integration Examples](../../Learning/FrontEnd/VueApiIntegrationExamples.cs)
- Frontend reference: `GoodVueTelemetry` in [Vue API Integration Examples](../../Learning/FrontEnd/VueApiIntegrationExamples.cs)

## Structural Necessities (Enterprise)

### Recommended architecture slices

```text
frontend/vue-app/
  src/features/         # feature modules (catalog/orders/profile)
  src/services/         # API service layer + interceptors
  src/composables/      # view-model and lifecycle orchestration
  src/components/       # reusable UI units
backend/api/
  Controllers/          # transport and auth boundary
  Application/          # use-case orchestration
  Domain/               # business invariants
  Infrastructure/       # persistence/integration adapters
shared/contracts/
  openapi/              # contract artifacts and changelog
```

### Enterprise delivery requirements

- Contract checks in CI for breaking schema changes.
- Deployment with automated health-gated rollback.
- Environment baseline for CORS, auth authority, and API base URL documented.
- Observability dashboard with release markers and alert runbooks.
- Error budget tracking to guide release cadence decisions.

### Testing minimum for this architecture

| Layer | Required tests |
| --- | --- |
| API | authorization policy tests + validation tests + ProblemDetails response tests |
| Application | business workflow unit tests + boundary condition tests |
| Contract | OpenAPI diff checks and consumer compatibility tests |
| Vue frontend | composable tests + component behavior tests + API mock integration tests |
| E2E | auth flow + list/search/paging + fault-path behavior |

## Backend Patterns (.NET)

### SPA-friendly query endpoint

```csharp
[HttpGet]
public async Task<ActionResult<IReadOnlyList<ProductListItemDto>>> Search(
    [FromQuery] string? q,
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 25,
    CancellationToken ct = default)
{
    if (page <= 0 || pageSize is < 1 or > 100)
    {
        return ValidationProblem(new Dictionary<string, string[]>
        {
            ["pagination"] = new[] { "Invalid page or pageSize." }
        });
    }

    var result = await _service.SearchAsync(q, page, pageSize, ct);
    return Ok(result);
}
```

### ProblemDetails for predictable client handling

```csharp
builder.Services.AddProblemDetails();
app.UseExceptionHandler();
```

## Frontend Patterns (Vue)

### Shared axios client

```ts
export const api = axios.create({
  baseURL: import.meta.env.VITE_API_BASE_URL,
  timeout: 8000,
  headers: { Accept: "application/json" },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem("access_token");
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});
```

### Composable for list page

```ts
export function useProducts() {
  const products = ref<ProductListItemDto[]>([]);
  const isLoading = ref(true);
  const error = ref<string | null>(null);
  const controller = new AbortController();

  onMounted(async () => {
    try {
      const response = await api.get("/api/products", { signal: controller.signal });
      products.value = response.data;
    } catch (err: any) {
      if (err.name !== "CanceledError") error.value = err.message ?? "Request failed";
    } finally {
      isLoading.value = false;
    }
  });

  onUnmounted(() => controller.abort());
  return { products, isLoading, error };
}
```

### Vite local proxy

```ts
export default defineConfig({
  server: {
    proxy: {
      "/api": {
        target: "https://localhost:5001",
        changeOrigin: true,
        secure: false,
      },
    },
  },
});
```

## Common Failure Modes

| Failure mode | Root cause | Prevention |
| --- | --- | --- |
| API logic scattered across components | No service/composable layer | Centralized API service + composables |
| Inconsistent auth behavior | Token handling duplicated | Request interceptor policy |
| Unexpected 400s on paging | Missing backend parameter guardrails | Validate query parameters and return clear validation details |
| Environment-specific integration bugs | Different local/prod routing assumptions | Vite proxy in dev + explicit production API base config |

## Validation Checklist

- Vue components call composables/services, not raw API endpoints.
- Axios instance handles auth and normalized errors centrally.
- Backend validates pagination/filter inputs explicitly.
- ProblemDetails is enabled for consistent non-2xx responses.
- Local and production API base routing are documented and tested.
- Authorization policies and scopes are explicit and covered by tests.
- Logs are structured with correlation ids and sensitive data redaction.
- Dashboards include latency, error-rate, and dependency health views.

## References

- Example code: [Vue API Integration Examples](../../Learning/FrontEnd/VueApiIntegrationExamples.cs)
- Related API middleware guidance: [Web API and MVC](../Web-API-MVC.md)
- Related UI comparisons: [Front-End DotNet UI](../Front-End-DotNet-UI.md)
- Related security guidance: [Security](../Security.md)
- Related observability guidance: [Logging and Observability](../Logging-Observability.md)

## Interview Answer Block

- 30-second answer: For Vue + .NET API, I focus on composables and shared API services on the frontend, plus DTO/validation/ProblemDetails discipline on the backend.
- 2-minute deep dive: I enforce one API client with interceptors, keep components presentation-focused, validate server inputs, and measure latency/error trends to verify reliability.
- Common follow-up: How do you manage local vs production integration differences?
- Strong response: Use Vite proxy for local parity, environment-based API base URL config, and contract checks in CI for backend changes.
- Tradeoff callout: Too many frontend abstractions can hide simple request flow and complicate debugging in early phases.

## Interview Bad vs Strong Answer

- Bad answer: "Vue calls the endpoint and we handle errors with try/catch."
- Strong answer: "Vue uses a shared axios client and composables for state/error flow, while the API returns stable DTOs and ProblemDetails with validated query constraints."
- Why strong wins: It demonstrates maintainable architecture and production-oriented failure handling.

## Interview Timed Drill

- Time box: 12 minutes.
- Prompt: Design a Vue product search page backed by .NET API with pagination and auth.
- Required outputs:
  - one composable design and responsibility boundary
  - one API validation/error strategy
  - one monitoring signal for release confidence
- Self-check score (0-3 each): architecture clarity, correctness, operability.

## Topic Files

- [Scenario Architecture](Scenario-Architecture.md)
- [Best Practices](Best-Practices.md)
- [Enterprise Security Baseline](Enterprise-Security-Baseline.md)
- [Logging and Observability Baseline](Logging-and-Observability-Baseline.md)
- [Structural Necessities (Enterprise)](Structural-Necessities-Enterprise.md)
- [Backend Patterns (.NET)](Backend-Patterns-NET.md)
- [Frontend Patterns (Vue)](Frontend-Patterns-Vue.md)
- [Common Failure Modes](Common-Failure-Modes.md)
- [Validation Checklist](Validation-Checklist.md)
- [References](References.md)



