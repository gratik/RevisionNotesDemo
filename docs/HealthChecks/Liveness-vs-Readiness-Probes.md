# Liveness vs Readiness Probes

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


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
- Summarize the core concept in one sentence and name one practical use case.

2-minute answer:
- Explain the concept, key tradeoffs, and one implementation detail or pitfall.

## Interview Bad vs Strong Answer
Bad answer:
- Gives a definition only without tradeoffs, examples, or failure modes.

Strong answer:
- Defines the concept, compares alternatives, and cites a concrete production scenario.

## Interview Timed Drill
- 60 seconds: define the topic and one reason it matters.
- 3 minutes: explain architecture, tradeoffs, and one troubleshooting example.

