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


