# Best Practices

> Subject: [DotNet-API-Vue](../README.md)

## Best Practices

1. Use a shared axios instance with request/response interceptors.
2. Keep token injection and error normalization in one place.
3. Implement composables (`useProducts`, `useOrders`) for loading/error lifecycle.
4. Use Vite proxy in local dev to reduce CORS drift.
5. Return validation and fault details as standard `ProblemDetails`.
6. Enforce server-side pagination boundaries (`pageSize` limits).
7. Keep response payloads small via list-specific DTOs.


