# Best Practices

> Subject: [Web-API-MVC](../README.md)

## Best Practices

### ✅ API Design
- Use RESTful conventions (GET, POST, PUT, DELETE)
- Return appropriate status codes (200, 201, 400, 404, 500)
- Use DTOs for requests/responses (don't expose domain models)
- Implement pagination for large collections
- Add API versioning from day 1

### ✅ Controllers
- Keep controllers thin (delegate to services)
- Use action filters for cross-cutting concerns
- Validate input with model binding and validation attributes
- Use ProducesResponseType for Swagger documentation
- Return ActionResult<T> for better type safety

### ✅ Middleware
- Order middleware correctly (see pipeline order above)
- Use short-circuiting when appropriate
- Keep middleware focused (single responsibility)
- Use scoped services via HttpContext.RequestServices

### ✅ Performance
- Use async/await for I/O operations
- Enable response compression
- Cache responses where appropriate
- Use output caching (ASP.NET Core 7+)
- Minimize allocations in hot paths

---


