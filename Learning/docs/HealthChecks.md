# Health Checks and Monitoring

> Part of: [C# & OOP Revision Notes - Comprehensive Demonstration Project](../../README.md)

## Overview

Health checks enable monitoring of application and dependency health for load balancers, Kubernetes,
and monitoring systems. This guide covers basic health checks, custom checks, liveness vs readiness
probes, and production patterns.

---

## Basic Health Checks

### Simple Health Endpoint

`csharp
// ✅ Register health checks
builder.Services.AddHealthChecks();

// ✅ Map endpoint
app.MapHealthChecks("/health");

// Returns:
// 200 OK with "Healthy" if all checks pass
// 503 Service Unavailable with "Unhealthy" if any check fails
`

### Detailed Health Report

`csharp
// ✅ Custom response with detailed information
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        
        var result = JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            duration = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                duration = e.Value.Duration.TotalMilliseconds,
                description = e.Value.Description,
                error = e.Value.Exception?.Message
            })
        });
        
        await context.Response.WriteAsync(result);
    }
});

// Returns JSON:
// {
//   "status": "Healthy",
//   "duration": 123.45,
//   "checks": [
//     {
//       "name": "database",
//       "status": "Healthy",
//       "duration": 45.6,
//       "description": "Database is responsive"
//     }
//   ]
// }
`

---

## Custom Health Checks

### Implementing IHealthCheck

`csharp
// ✅ Custom database health check
public class DatabaseHealthCheck : IHealthCheck
{
    private readonly AppDbContext _context;
    
    public DatabaseHealthCheck(AppDbContext context)
    {
        _context = context;
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to query database
            await _context.Database.CanConnectAsync(cancellationToken);
            
            return HealthCheckResult.Healthy("Database is responsive");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "Database is not responding",
                exception: ex);
        }
    }
}

// ✅ Register custom health check
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database");
`

### External Service Health Check

`csharp
// ✅ Check external API
public class ExternalApiHealthCheck : IHealthCheck
{
    private readonly HttpClient _httpClient;
    
    public ExternalApiHealthCheck(IHttpClientFactory httpClientFactory)
    {
        _httpClient = httpClientFactory.CreateClient();
    }
    
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync(
                "https://api.example.com/health",
                cancellationToken);
            
            if (response.IsSuccessStatusCode)
            {
                return HealthCheckResult.Healthy("External API is available");
            }
            
            return HealthCheckResult.Degraded(
                External API returned {(int)response.StatusCode});
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy(
                "External API is unavailable",
                exception: ex);
        }
    }
}
`

### Memory Health Check

`csharp
// ✅ Check memory usage
public class MemoryHealthCheck : IHealthCheck
{
    private const long MaxMemoryBytes = 1024L * 1024L * 1024L;  // 1 GB
    
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var allocated = GC.GetTotalMemory(forceFullCollection: false);
        
        var data = new Dictionary<string, object>
        {
            { "AllocatedMB", allocated / 1024 / 1024 },
            { "Gen0Collections", GC.CollectionCount(0) },
            { "Gen1Collections", GC.CollectionCount(1) },
            { "Gen2Collections", GC.CollectionCount(2) }
        };
        
        if (allocated >= MaxMemoryBytes)
        {
            return Task.FromResult(
                HealthCheckResult.Unhealthy(
                    Memory usage is {allocated / 1024 / 1024} MB,
                    data: data));
        }
        
        if (allocated >= MaxMemoryBytes * 0.8)
        {
            return Task.FromResult(
                HealthCheckResult.Degraded(
                    "Memory usage is getting high",
                    data: data));
        }
        
        return Task.FromResult(
            HealthCheckResult.Healthy("Memory usage is normal", data: data));
    }
}
`

---

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

## Liveness vs Readiness Probes

### Liveness Probe

**Question**: "Should this instance be restarted?"  
**Purpose**: Detect deadlocks, infinite loops, unrecoverable errors

`csharp
// ✅ Liveness: Basic application health
builder.Services.AddHealthChecks()
    .AddCheck("self", () => HealthCheckResult.Healthy());

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live")
});

// Kubernetes liveness probe
// livenessProbe:
//   httpGet:
//     path: /health/live
//     port: 80
//   initialDelaySeconds: 10
//   periodSeconds: 10
`

### Readiness Probe

**Question**: "Can this instance serve traffic?"  
**Purpose**: Check dependencies (database, cache, external APIs)

`csharp
// ✅ Readiness: Dependencies must be available
builder.Services.AddHealthChecks()
    .AddCheck<DatabaseHealthCheck>("database", tags: new[] { "ready" })
    .AddCheck<CacheHealthCheck>("cache", tags: new[] { "ready" })
    .AddCheck<ExternalApiHealthCheck>("external-api", tags: new[] { "ready" });

app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

// Kubernetes readiness probe
// readinessProbe:
//   httpGet:
//     path: /health/ready
//     port: 80
//   initialDelaySeconds: 5
//   periodSeconds: 5
`

### Startup Probe

**Question**: "Has the application finished starting?"  
**Purpose**: Allow slow startup without killing container

`csharp
// ✅ Startup: Check initialization
builder.Services.AddHealthChecks()
    .AddCheck("startup", () => 
    {
        // Check if application has fully initialized
        return MyApp.IsInitialized 
            ? HealthCheckResult.Healthy() 
            : HealthCheckResult.Unhealthy();
    }, tags: new[] { "startup" });

app.MapHealthChecks("/health/startup", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("startup")
});

// Kubernetes startup probe
// startupProbe:
//   httpGet:
//     path: /health/startup
//     port: 80
//   failureThreshold: 30
//   periodSeconds: 10
`

---

## Built-In Health Checks

### Using AspNetCore.HealthChecks.* Packages

`csharp
// ✅ Install packages:
// - AspNetCore.HealthChecks.SqlServer
// - AspNetCore.HealthChecks.Redis
// - AspNetCore.HealthChecks.Npgsql
// - AspNetCore.HealthChecks.Rabbit RabbitMQ
// - AspNetCore.HealthChecks.Uris

builder.Services.AddHealthChecks()
    // ✅ SQL Server
    .AddSqlServer(
        connectionString: builder.Configuration.GetConnectionString("DefaultConnection"),
        name: "sql-server",
        tags: new[] { "db", "ready" })
    
    // ✅ Redis
    .AddRedis(
        redisConnectionString: "localhost:6379",
        name: "redis",
        tags: new[] { "cache", "ready" })
    
    // ✅ URL check
    .AddUrlGroup(
        uri: new Uri("https://api.example.com/health"),
        name: "external-api",
        tags: new[] { "external", "ready" })
    
    // ✅ Disk storage
    .AddDiskStorageHealthCheck(
        setup => setup.AddDrive("C:\\", minimumFreeMegabytes: 1024),
        name: "disk",
        tags: new[] { "storage" });
`

---

## Health Check UI

### Adding UI Dashboard

`csharp
// ✅ Install: AspNetCore.HealthChecks.UI

builder.Services.AddHealthChecksUI(settings =>
{
    settings.AddHealthCheckEndpoint("API Health", "/health");
    settings.SetEvaluationTimeInSeconds(10);  // Check every 10 seconds
    settings.MaximumHistoryEntriesPerEndpoint(50);
})
.AddInMemoryStorage();

// ✅ Add UI middleware
app.UseHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";  // Dashboard at /health-ui
});

// Access dashboard: https://localhost:5001/health-ui
`

---

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

## Common Pitfalls

### ❌ Slow Health Checks

`csharp
// ❌ BAD: Expensive operation
public async Task<HealthCheckResult> CheckHealthAsync(
    HealthCheckContext context,
    CancellationToken cancellationToken = default)
{
    var allUsers = await _context.Users.ToListAsync();  // ❌ Loads all users!
    return HealthCheckResult.Healthy();
}

// ✅ GOOD: Quick check
public async Task<HealthCheckResult> CheckHealthAsync(
    HealthCheckContext context,
    CancellationToken cancellationToken = default)
{
    var canConnect = await _context.Database.CanConnectAsync(cancellationToken);
    return canConnect 
        ? HealthCheckResult.Healthy() 
        : HealthCheckResult.Unhealthy();
}
`

### ❌ No Timeout on External Checks

`csharp
// ❌ BAD: No timeout, can hang
public async Task<HealthCheckResult> CheckHealthAsync(
    HealthCheckContext context,
    CancellationToken cancellationToken = default)
{
    await _httpClient.GetAsync("https://api.example.com/health");  // ❌ Hangs forever
}

// ✅ GOOD: Use cancellation token and timeout
public async Task<HealthCheckResult> CheckHealthAsync(
    HealthCheckContext context,
    CancellationToken cancellationToken = default)
{
    using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
    cts.CancelAfter(TimeSpan.FromSeconds(5));  // ✅ 5 second timeout
    
    try
    {
        await _httpClient.GetAsync("https://api.example.com/health", cts.Token);
        return HealthCheckResult.Healthy();
    }
    catch (OperationCanceledException)
    {
        return HealthCheckResult.Unhealthy("Timeout");
    }
}
`

---

## Related Files

- [HealthChecks/HealthCheckExamples.cs](../HealthChecks/HealthCheckExamples.cs)

---

## See Also

- [Web API and MVC](Web-API-MVC.md) - Health check endpoints
- [Resilience](Resilience.md) - Handling unhealthy dependencies
- [Logging and Observability](Logging-Observability.md) - Monitoring health
- [Project Summary](../../PROJECT_SUMMARY.md)

---

Generated: 2026-02-14
