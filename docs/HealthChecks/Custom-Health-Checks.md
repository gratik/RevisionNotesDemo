# Custom Health Checks

## Metadata
- Owner: RevisionNotes Maintainers
- Last updated: February 17, 2026
- Prerequisites: See module README for sequencing guidance.
- Related examples: README.md


> Subject: [HealthChecks](../README.md)

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

