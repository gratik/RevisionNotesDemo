# Best Practices

> Subject: [DotNet-API-React](../README.md)

## Best Practices

1. Use a shared API client (`fetch` wrapper or axios) instead of calling endpoints directly in components.
2. Handle cancellation via `AbortController` for unmounted components.
3. Return `ProblemDetails` from API for predictable frontend error rendering.
4. Keep CORS origin list explicit for production and local development.
5. Pass `CancellationToken` through API -> service -> repository chain.
6. Use DTO projection for list pages to avoid over-fetching.
7. Treat auth token handling and refresh behavior as infrastructure concerns.


