# SPA with .NET API: React and Vue

> Subject: [Front-End-DotNet-UI](../README.md)

## SPA with .NET API: React and Vue

This is a common real-world pattern: a JavaScript SPA consumes a .NET API over HTTP.

### React + .NET API (summary)

- Use a shared API client (`fetch` wrapper or axios instance).
- Normalize API errors (`ProblemDetails`) into stable UI states.
- Use `AbortController` for request cancellation in unmounted components.
- Keep CORS origins explicit and environment-specific.

Detailed guide: [DotNet API to React Front End](../DotNet-API-React.md)  
Code examples: [Learning/FrontEnd/ReactApiIntegrationExamples.cs](../../Learning/FrontEnd/ReactApiIntegrationExamples.cs)

### Vue + .NET API (summary)

- Use one axios client with request/response interceptors.
- Keep API calls in composables/services, not inside page components.
- Use Vite proxy for local development consistency.
- Validate paging/filter input on API endpoints and return clear validation details.

Detailed guide: [DotNet API to Vue Front End](../DotNet-API-Vue.md)  
Code examples: [Learning/FrontEnd/VueApiIntegrationExamples.cs](../../Learning/FrontEnd/VueApiIntegrationExamples.cs)

---

## Detailed Guidance

API guidance focuses on contract stability, secure input/output handling, and production-ready observability.

### Design Notes
- Define success criteria for SPA with .NET API: React and Vue before implementation work begins.
- Keep boundaries explicit so SPA with .NET API: React and Vue decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring SPA with .NET API: React and Vue in production-facing code.
- When performance, correctness, or maintainability depends on consistent SPA with .NET API: React and Vue decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying SPA with .NET API: React and Vue as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where SPA with .NET API: React and Vue is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for SPA with .NET API: React and Vue are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

