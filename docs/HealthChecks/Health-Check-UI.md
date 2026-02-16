# Health Check UI

> Subject: [HealthChecks](../README.md)

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


