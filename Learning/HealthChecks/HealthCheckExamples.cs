// ==============================================================================
// HEALTH CHECKS - Application Health Monitoring
// ==============================================================================
// WHAT IS THIS?
// -------------
// Health endpoints for liveness/readiness and dependency checks.
//
// WHY IT MATTERS
// --------------
// ✅ Enables orchestration and proactive monitoring
// ✅ Provides visibility into dependency health
//
// WHEN TO USE
// -----------
// ✅ Any service running in production or containers
// ✅ Apps requiring load balancer health probes
//
// WHEN NOT TO USE
// ---------------
// ❌ Never; health checks are a baseline requirement
// ❌ Exposing sensitive details on public endpoints
//
// REAL-WORLD EXAMPLE
// ------------------
// /health/ready for database readiness.
// ==============================================================================

using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Data.SqlClient;
// Note: Extended health checks require additional packages:
// - AspNetCore.HealthChecks.SqlServer
// - AspNetCore.HealthChecks.Redis
// - AspNetCore.HealthChecks.Network
// - AspNetCore.HealthChecks.UI

namespace RevisionNotesDemo.HealthChecks;

/// <summary>
/// EXAMPLE 1: BASIC HEALTH CHECKS - Built-In Endpoints
/// 
/// THE PROBLEM:
/// Load balancer needs to know if app is healthy.
/// 
/// THE SOLUTION:
/// /health endpoint returns 200 OK if healthy, 503 if unhealthy.
/// 
/// WHY IT MATTERS:
/// - Load balancer routes traffic to healthy instances
/// - Kubernetes restarts unhealthy pods
/// - Monitoring alerts on failures
/// </summary>
public class BasicHealthChecksExamples
{
    // ✅ GOOD: Configure basic health check
    public static void ConfigureBasic(WebApplicationBuilder builder)
    {
        // ✅ Add health checks
        builder.Services.AddHealthChecks();
    }

    public static void MapBasic(WebApplication app)
    {
        // ✅ Map endpoint
        app.MapHealthChecks("/health");

        // Returns:
        // 200 OK: "Healthy"
        // 503 Service Unavailable: "Unhealthy"
    }

    // ✅ GOOD: Detailed health check response
    public static void MapDetailed(WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = async (context, report) =>
            {
                context.Response.ContentType = "application/json";

                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    duration = report.TotalDuration,
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        duration = e.Value.Duration,
                        description = e.Value.Description,
                        exception = e.Value.Exception?.Message
                    })
                });

                await context.Response.WriteAsync(result);
            }
        });

        // Returns JSON:
        // {
        //   "status": "Healthy",
        //   "duration": "00:00:00.0234567",
        //   "checks": [
        //     { "name": "database", "status": "Healthy", "duration": "00:00:00.0123" }
        //   ]
        // }
    }
}

/// <summary>
/// EXAMPLE 2: BUILT-IN DEPENDENCY CHECKS - Database, Redis, etc.
/// 
/// THE PROBLEM:
/// App depends on database, cache, external APIs.
/// Need to check if dependencies are healthy.
/// 
/// THE SOLUTION:
/// Microsoft.Extensions.Diagnostics.HealthChecks.* NuGet packages.
/// </summary>
public class DependencyHealthChecksExamples
{
    /*
    // NOTE: This example requires additional NuGet packages:
    // - dotnet add package AspNetCore.HealthChecks.SqlServer
    // - dotnet add package AspNetCore.HealthChecks.Redis  
    // - dotnet add package AspNetCore.HealthChecks.Network
    // - dotnet add package AspNetCore.HealthChecks.System
    
    // ✅ GOOD: Add dependency health checks
    public static void ConfigureDependencies(WebApplicationBuilder builder)
    {
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var redisConnection = builder.Configuration.GetConnectionString("Redis");
        
        builder.Services.AddHealthChecks()
            // ✅ SQL Server check
            .AddSqlServer(
                connectionString: connectionString ?? "",
                name: "database",
                failureStatus: HealthStatus.Unhealthy,
                tags: new[] { "db", "sql" })
            
            // ✅ Redis check
            .AddRedis(
                redisConnectionString: redisConnection ?? "",
                name: "redis-cache",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "cache", "redis" })
            
            // ✅ HTTP endpoint check
            .AddUrlGroup(
                uri: new Uri("https://api.example.com/health"),
                name: "external-api",
                failureStatus: HealthStatus.Degraded,
                tags: new[] { "external" })
            
            // ✅ Disk storage check
            .AddDiskStorageHealthCheck(
                setup: options =>
                {
                    options.AddDrive("C:\\", 1024);  // Warn if < 1GB free
                },
                name: "disk-storage",
                failureStatus: HealthStatus.Degraded);
    }
    */

    // NuGet Packages:
    // - AspNetCore.HealthChecks.SqlServer
    // - AspNetCore.HealthChecks.Redis
    // - AspNetCore.HealthChecks.Uris
    // - AspNetCore.HealthChecks.System
}

/// <summary>
/// EXAMPLE 3: CUSTOM HEALTH CHECKS - Business Logic Health
/// 
/// THE PROBLEM:
/// Need to check custom conditions (queue length, license validity).
/// 
/// THE SOLUTION:
/// Implement IHealthCheck interface.
/// </summary>
public class CustomHealthCheckExamples
{
    // ✅ GOOD: Custom health check for message queue
    public class MessageQueueHealthCheck : IHealthCheck
    {
        private readonly IMessageQueue _messageQueue;

        public MessageQueueHealthCheck(IMessageQueue messageQueue)
        {
            _messageQueue = messageQueue;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var queueLength = await _messageQueue.GetLengthAsync();

                // ✅ Healthy: Queue is processing normally
                if (queueLength < 100)
                {
                    return HealthCheckResult.Healthy(
                        $"Queue length is {queueLength}");
                }

                // ⚠️ Degraded: Queue is backed up but functional
                if (queueLength < 1000)
                {
                    return HealthCheckResult.Degraded(
                        $"Queue length is {queueLength} (above threshold)");
                }

                // ❌ Unhealthy: Queue is severely backed up
                return HealthCheckResult.Unhealthy(
                    $"Queue length is {queueLength} (critical)");
            }
            catch (Exception ex)
            {
                // ❌ Exception = Unhealthy
                return HealthCheckResult.Unhealthy(
                    "Failed to check queue",
                    exception: ex);
            }
        }
    }

    // ✅ GOOD: Custom health check for license validity
    public class LicenseHealthCheck : IHealthCheck
    {
        private readonly ILicenseService _licenseService;

        public LicenseHealthCheck(ILicenseService licenseService)
        {
            _licenseService = licenseService;
        }

        public Task<HealthCheckResult> CheckHealthAsync(
            HealthCheckContext context,
            CancellationToken cancellationToken = default)
        {
            var license = _licenseService.GetLicense();

            if (license.IsExpired)
            {
                return Task.FromResult(HealthCheckResult.Unhealthy(
                    $"License expired on {license.ExpirationDate}"));
            }

            if (license.ExpiresInDays < 7)
            {
                return Task.FromResult(HealthCheckResult.Degraded(
                    $"License expires in {license.ExpiresInDays} days"));
            }

            return Task.FromResult(HealthCheckResult.Healthy(
                $"License valid until {license.ExpirationDate}"));
        }
    }

    // ✅ Register custom health checks
    public static void RegisterCustomChecks(WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IMessageQueue, MessageQueue>();
        builder.Services.AddSingleton<ILicenseService, LicenseService>();

        builder.Services.AddHealthChecks()
            .AddCheck<MessageQueueHealthCheck>("message-queue", tags: new[] { "queue" })
            .AddCheck<LicenseHealthCheck>("license", tags: new[] { "license" });
    }

    // Supporting interfaces
    public interface IMessageQueue
    {
        Task<int> GetLengthAsync();
    }

    public interface ILicenseService
    {
        License GetLicense();
    }

    public class MessageQueue : IMessageQueue
    {
        public Task<int> GetLengthAsync() => Task.FromResult(50);
    }

    public class LicenseService : ILicenseService
    {
        public License GetLicense() => new License
        {
            ExpirationDate = DateTime.UtcNow.AddDays(30),
            IsExpired = false,
            ExpiresInDays = 30
        };
    }

    public class License
    {
        public DateTime ExpirationDate { get; set; }
        public bool IsExpired { get; set; }
        public int ExpiresInDays { get; set; }
    }
}

/// <summary>
/// EXAMPLE 4: LIVENESS VS READINESS - Kubernetes Probes
/// 
/// THE PROBLEM:
/// Kubernetes needs to know:
/// - Should I restart this pod? (Liveness)
/// - Can this pod receive traffic? (Readiness)
/// 
/// THE SOLUTION:
/// Separate endpoints with different checks.
/// 
/// WHY IT MATTERS:
/// - Liveness failure → Pod restart
/// - Readiness failure → Remove from load balancer (temp)
/// </summary>
public class LivenessReadinessExamples
{
    // ✅ GOOD: Separate liveness and readiness
    public static void ConfigureKubernetes(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks()
            // Liveness: Basic app health (can it even run?)
            .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "liveness" });

        /* Requires: dotnet add package AspNetCore.HealthChecks.SqlServer
        //           dotnet add package AspNetCore.HealthChecks.Redis

        // Readiness: Dependencies must be healthy
        .AddSqlServer(
            connectionString: builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
            name: "database",
            tags: new[] { "readiness" })
        .AddRedis(
            redisConnectionString: builder.Configuration.GetConnectionString("Redis") ?? "",
            name: "redis",
            tags: new[] { "readiness" });
        */
    }

    public static void MapKubernetesProbes(WebApplication app)
    {
        // ✅ Liveness probe: /health/live
        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("liveness")
        });

        // ✅ Readiness probe: /health/ready
        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains("readiness")
        });

        // Kubernetes deployment.yaml:
        // livenessProbe:
        //   httpGet:
        //     path: /health/live
        //     port: 80
        //   initialDelaySeconds: 30
        //   periodSeconds: 10
        // 
        // readinessProbe:
        //   httpGet:
        //     path: /health/ready
        //     port: 80
        //   initialDelaySeconds: 5
        //   periodSeconds: 5
    }

    // LIVENESS CHECKS (should almost never fail):
    // ✅ App can respond to HTTP
    // ✅ Core services initialized
    // ❌ NOT dependency health (database, cache)

    // READINESS CHECKS (can fail temporarily):
    // ✅ Database is reachable
    // ✅ Cache is available
    // ✅ Required external APIs responding
    // ✅ Message queue is accessible
}

/// <summary>
/// EXAMPLE 5: HEALTH CHECK UI - Visual Dashboard
/// 
/// THE PROBLEM:
/// Want visual dashboard to see health of all dependencies.
/// 
/// THE SOLUTION:
/// AspNetCore.HealthChecks.UI package provides web UI.
/// </summary>
public class HealthCheckUIExamples
{
    // ✅ GOOD: Configure Health Check UI
    public static void ConfigureUI(WebApplicationBuilder builder)
    {
        // Install: dotnet add package AspNetCore.HealthChecks.UI
        //          dotnet add package AspNetCore.HealthChecks.UI.InMemory.Storage

        builder.Services.AddHealthChecks();
        /* Requires additional packages:
        .AddSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? "")
        .AddRedis(builder.Configuration.GetConnectionString("Redis") ?? "");
        */

        /* Requires: dotnet add package AspNetCore.HealthChecks.UI
        //          dotnet add package AspNetCore.HealthChecks.UI.InMemory.Storage
        // ✅ Add UI
        builder.Services.AddHealthChecksUI(options =>
        {
            options.SetEvaluationTimeInSeconds(10);  // Refresh every 10 seconds
            options.MaximumHistoryEntriesPerEndpoint(50);
            options.AddHealthCheckEndpoint("API", "/health");
        })
        .AddInMemoryStorage();  // Store results in-memory
        */
    }

    public static void MapUI(WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (context, report) =>
            {
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString()
                    })
                });
                await context.Response.WriteAsync(result);
            }
        });

        /* Requires: dotnet add package AspNetCore.HealthChecks.UI
        // ✅ UI endpoints
        app.MapHealthChecksUI(options =>
        {
            options.UIPath = "/health-ui";  // Dashboard at /health-ui
            options.ApiPath = "/health-ui-api";
        });
        */

        // Visit: https://localhost:5001/health-ui
        // Beautiful dashboard showing:
        // - Current status (Healthy/Degraded/Unhealthy)
        // - Response time history
        // - Failure history
        // - Per-check status
    }
}

/// <summary>
/// EXAMPLE 6: PRODUCTION PATTERNS - Monitoring and Alerts
/// </summary>
public class ProductionPatternsExamples
{
    // ✅ GOOD: Filter sensitive data from health checks
    public static void ConfigureProduction(WebApplicationBuilder builder)
    {
        builder.Services.AddHealthChecks();
        /* Requires: dotnet add package AspNetCore.HealthChecks.SqlServer
        .AddSqlServer(
            connectionString: builder.Configuration.GetConnectionString("DefaultConnection") ?? "",
            name: "database");
        */
    }

    public static void MapProduction(WebApplication app)
    {
        // ✅ Public endpoint: Limited info
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (context, report) =>
            {
                // ✅ Only return status, no sensitive details
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(report.Status.ToString());
            }
        });

        // ✅ Internal endpoint: Full details (behind auth/firewall)
        app.MapHealthChecks("/health/detailed", new HealthCheckOptions
        {
            Predicate = _ => true,
            ResponseWriter = async (context, report) =>
            {
                // Full diagnostic info for operations team
                var result = System.Text.Json.JsonSerializer.Serialize(new
                {
                    status = report.Status.ToString(),
                    duration = report.TotalDuration,
                    checks = report.Entries.Select(e => new
                    {
                        name = e.Key,
                        status = e.Value.Status.ToString(),
                        duration = e.Value.Duration,
                        description = e.Value.Description,
                        exception = e.Value.Exception?.Message
                    }),
                    timestamp = DateTime.UtcNow
                });

                await context.Response.WriteAsync(result);
            }
        });
    }

    // ✅ MONITORING INTEGRATION:
    // - Application Insights: Auto-tracks /health endpoint
    // - Prometheus: Use AspNetCore.HealthChecks.Prometheus
    // - DataDog: Use AspNetCore.HealthChecks.Publisher.DataDog
    // - Custom: Poll /health endpoint, alert on 503
}

// SUMMARY - Health Check Best Practices:
//
// ✅ DO:
// - Implement /health endpoint for all apps
// - Use tags to separate liveness/readiness
// - Check all critical dependencies
// - Set appropriate timeouts (5-10 seconds)
// - Return detailed errors in internal endpoints
// - Monitor health check history
// - Test health check failure scenarios
//
// ❌ DON'T:
// - Expose sensitive data in public /health endpoint
// - Make health checks too slow (>10 seconds)
// - Include non-critical dependencies in liveness
// - Return 200 OK when dependencies are down
// - Forget to test health check failures
//
// HEALTH STATUS LEVELS:
// - Healthy: All checks passed
// - Degraded: Non-critical issues (cache down, queue backed up)
// - Unhealthy: Critical failures (database unreachable)
//
// KUBERNETES CONFIGURATION:
// livenessProbe:
//   - Path: /health/live
//   - Only basic checks (app running)
//   - Failure → Restart pod
//   - initialDelaySeconds: 30 (allow startup)
//   - failureThreshold: 3 (avoid flapping)
//
// readinessProbe:
//   - Path: /health/ready
//   - Check all dependencies
//   - Failure → Remove from service
//   - initialDelaySeconds: 5
//   - failureThreshold: 1 (fast removal)
//
// TYPICAL CHECKS:
// ✅ Database connectivity
// ✅ Cache availability
// ✅ Message queue accessibility
// ✅ External API responsiveness
// ✅ Disk space
// ✅ Memory usage
// ✅ License validity
// ✅ Configuration validity
