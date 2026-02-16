# Health Check Status

> Subject: [HealthChecks](../README.md)

## Health Check Status

### Three States

| Status | HTTP Code | Meaning |
|--------|-----------|---------|
| **Healthy** | 200 OK | All systems operational |
| **Degraded** | 200 OK | Operational but with issues |
| **Unhealthy** | 503 Service Unavailable | Critical failure |

`csharp
// ✅ Return appropriate status
public async Task<HealthCheckResult> CheckHealthAsync(
    HealthCheckContext context,
    CancellationToken cancellationToken = default)
{
    var responseTime = await MeasureResponseTimeAsync();
    
    if (responseTime < 100)
        return HealthCheckResult.Healthy("Fast response");
    
    if (responseTime < 500)
        return HealthCheckResult.Degraded("Slow response");
    
    return HealthCheckResult.Unhealthy("Very slow response");
}
`

---


