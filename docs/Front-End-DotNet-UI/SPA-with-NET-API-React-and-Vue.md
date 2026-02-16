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



