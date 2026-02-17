# Liveness vs Readiness Probes

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: ASP.NET middleware basics and service dependency mapping.
- Related examples: docs/HealthChecks/README.md
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


## Interview Answer Block
30-second answer:
- Liveness vs Readiness Probes is about service health signaling and readiness strategy. It matters because accurate health endpoints prevent bad routing and noisy incidents.
- Use it when separating liveness/readiness/dependency checks by purpose.

2-minute answer:
- Start with the problem Liveness vs Readiness Probes solves in this module and the baseline implementation approach.
- Discuss a key tradeoff: signal depth vs probe execution overhead.
- Close with one failure mode and mitigation: health checks that are either too shallow or too expensive.
## Interview Bad vs Strong Answer
Bad answer:
- Defines Liveness vs Readiness Probes but skips constraints, alternatives, and production impact.

Strong answer:
- Explains when to choose Liveness vs Readiness Probes, what to compare it against, and how to validate it in tests/operations.
## Interview Timed Drill
- 60 seconds: define Liveness vs Readiness Probes and map it to one concrete implementation in this module.
- 3 minutes: compare Liveness vs Readiness Probes with an alternative, then walk through one failure mode and mitigation.