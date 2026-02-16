# Liveness vs Readiness Probes

> Subject: [HealthChecks](../README.md)

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


