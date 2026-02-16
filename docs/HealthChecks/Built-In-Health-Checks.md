# Built-In Health Checks

> Subject: [HealthChecks](../README.md)

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

## Detailed Guidance

UI integration guidance focuses on boundary contracts, predictable state flow, and release-safe cross-layer changes.

### Design Notes
- Define success criteria for Built-In Health Checks before implementation work begins.
- Keep boundaries explicit so Built-In Health Checks decisions do not leak accidental complexity into adjacent layers.
- Prefer simpler implementations first, then optimize based on measured constraints.
- Make failure behavior explicit (timeouts, retries, validation, rollback, or compensation) where applicable.

### When To Use
- When introducing or refactoring Built-In Health Checks in production-facing code.
- When performance, correctness, or maintainability depends on consistent Built-In Health Checks decisions.
- When design reviews require concrete tradeoffs and validation signals.

### Anti-Patterns To Avoid
- Applying Built-In Health Checks as a checklist item without tying it to workload and constraints.
- Large, multi-axis changes that make regression root-cause analysis difficult.
- Shipping without measurable before/after signals for the chosen approach.

## Practical Example

- Choose one high-impact path where Built-In Health Checks is currently weak or inconsistent.
- Apply one bounded improvement and document the expected behavior change.
- Validate with tests and runtime metrics, then capture rollback conditions.

## Validation Checklist

- Design assumptions for Built-In Health Checks are documented and reviewable.
- Tests cover both happy path and at least one realistic failure path.
- Metrics/logging expose the primary risk this topic addresses.
- Operational ownership is clear if behavior regresses in production.

## Cross References

- [Subject Overview](README.md)
- [Docs Index](../README.md)

