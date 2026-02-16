# Common Pitfalls

> Subject: [HealthChecks](../README.md)

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


