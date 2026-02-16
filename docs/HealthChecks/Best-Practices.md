# Best Practices

> Subject: [HealthChecks](../README.md)

## Best Practices

### ✅ Health Check Design
- Liveness: Keep simple (is app running?)
- Readiness: Check dependencies (can serve traffic?)
- Use tags to categorize checks
- Return meaningful descriptions
- Include timing information

### ✅ Performance
- Keep checks fast (< 1 second)
- Use timeouts for external dependencies
- Don't check on every request (cache results)
- Use separate endpoints for liveness/readiness

### ✅ Kubernetes Configuration
- Liveness: Restart if failing
- Readiness: Remove from load balancer if failing
- Startup: Wait for initialization
- Set appropriate timeouts and thresholds

---


