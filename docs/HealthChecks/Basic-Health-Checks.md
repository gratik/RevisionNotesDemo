# Basic Health Checks

> Subject: [HealthChecks](../README.md)

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


