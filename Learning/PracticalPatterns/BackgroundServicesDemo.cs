// ==============================================================================
// BACKGROUND SERVICES
// Reference: Revision Notes - Practical Scenarios
// ==============================================================================
// WHAT IS THIS?
// -------------
// Hosted services for long-running or scheduled background tasks.
//
// WHY IT MATTERS
// --------------
// ✅ Offloads work from request threads
// ✅ Supports periodic jobs and queue processing
//
// WHEN TO USE
// -----------
// ✅ Cache warming, cleanup jobs, and message processing
// ✅ Continuous monitoring or polling tasks
//
// WHEN NOT TO USE
// ---------------
// ❌ Short-lived tasks triggered per request
// ❌ Work better handled by serverless scheduled jobs
//
// REAL-WORLD EXAMPLE
// ------------------
// Nightly data cleanup and email batch processing.
// ==============================================================================

namespace RevisionNotesDemo.PracticalPatterns;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;

// ========================================================================
// EXAMPLE 1: SIMPLE TIMED BACKGROUND SERVICE
// ========================================================================

/// <summary>
/// Background service that executes periodically
/// </summary>
public class HealthCheckService : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(5);
    private int _executionCount = 0;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("[HEALTH CHECK] 🏥 Service started\n");

        while (!stoppingToken.IsCancellationRequested)
        {
            _executionCount++;

            Console.WriteLine($"[HEALTH CHECK] ⏰ Execution #{_executionCount} at {DateTime.Now:HH:mm:ss}");
            Console.WriteLine($"  Checking system health...");
            Console.WriteLine($"  ✅ All systems operational\n");

            try
            {
                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("[HEALTH CHECK] 🛑 Service stopping...\n");
                break;
            }
        }

        Console.WriteLine("[HEALTH CHECK] 🛑 Service stopped\n");
    }
}

// ========================================================================
// EXAMPLE 2: QUEUE PROCESSING SERVICE
// ========================================================================

public interface IBackgroundTaskQueue
{
    void QueueTask(Func<CancellationToken, Task> workItem);
    Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken);
}

public class BackgroundTaskQueue : IBackgroundTaskQueue, IDisposable
{
    private readonly System.Collections.Concurrent.ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();
    private readonly SemaphoreSlim _signal = new(0);

    public void QueueTask(Func<CancellationToken, Task> workItem)
    {
        _workItems.Enqueue(workItem);
        _signal.Release();
        Console.WriteLine($"[QUEUE] ➕ Task queued (queue size: {_workItems.Count})");
    }

    public async Task<Func<CancellationToken, Task>> DequeueAsync(CancellationToken cancellationToken)
    {
        await _signal.WaitAsync(cancellationToken);
        _workItems.TryDequeue(out var workItem);
        return workItem!;
    }

    public void Dispose()
    {
        _signal.Dispose();
        GC.SuppressFinalize(this);
    }
}

/// <summary>
/// Background service that processes queued tasks
/// </summary>
public class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;

    public QueuedHostedService(IBackgroundTaskQueue taskQueue)
    {
        _taskQueue = taskQueue;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("[QUEUE PROCESSOR] 🔄 Started processing queue\n");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var workItem = await _taskQueue.DequeueAsync(stoppingToken);

                Console.WriteLine($"[QUEUE PROCESSOR] ⚙️  Processing task...");
                await workItem(stoppingToken);
                Console.WriteLine($"[QUEUE PROCESSOR] ✅ Task completed\n");
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[QUEUE PROCESSOR] ❌ Error: {ex.Message}\n");
            }
        }

        Console.WriteLine("[QUEUE PROCESSOR] 🛑 Stopped\n");
    }
}

// ========================================================================
// EXAMPLE 3: DATA CLEANUP SERVICE
// ========================================================================

/// <summary>
/// Periodically cleans up old data
/// </summary>
public class DataCleanupService : BackgroundService
{
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(10);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("[CLEANUP] 🧹 Service started\n");

        // Wait for initial delay
        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                Console.WriteLine($"[CLEANUP] 🗑️  Running cleanup at {DateTime.Now:HH:mm:ss}");

                // Simulate cleanup work
                await Task.Delay(1000, stoppingToken);

                var deletedCount = Random.Shared.Next(0, 50);
                Console.WriteLine($"  Deleted {deletedCount} expired records");
                Console.WriteLine($"  ✅ Cleanup complete\n");

                await Task.Delay(_interval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        Console.WriteLine("[CLEANUP] 🛑 Service stopped\n");
    }
}

// ========================================================================
// EXAMPLE 4: CACHE WARMING SERVICE
// ========================================================================

public class CacheWarmupService : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("[CACHE WARMUP] 🔥 Warming up caches...");

        // Simulate cache warming
        await Task.Delay(2000, cancellationToken);

        Console.WriteLine("  ✅ Product cache loaded (500 items)");
        Console.WriteLine("  ✅ User cache loaded (1000 items)");
        Console.WriteLine("  ✅ Configuration cache loaded");
        Console.WriteLine("[CACHE WARMUP] ✅ Warmup complete!\n");

        return;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("[CACHE WARMUP] 🛑 Service stopped\n");
        return Task.CompletedTask;
    }
}

// ========================================================================
// DEMONSTRATION
// ========================================================================

public class BackgroundServicesDemo
{
    public static async Task RunDemoAsync()
    {
        Console.WriteLine("\n=== BACKGROUND SERVICES DEMO ===\n");
        Console.WriteLine("Reference: Revision Notes - Practical Scenarios\n");

        // Setup host with background services
        var host = Host.CreateDefaultBuilder()
            .ConfigureServices(services =>
            {
                // Register background services
                services.AddHostedService<CacheWarmupService>();
                services.AddHostedService<HealthCheckService>();
                services.AddHostedService<DataCleanupService>();

                // Register queue processor
                services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
                services.AddHostedService<QueuedHostedService>();
            })
            .Build();

        // Start all background services
        Console.WriteLine("=== Starting Background Services ===\n");
        var hostTask = host.RunAsync();

        // Give services time to start
        await Task.Delay(3000);

        // Queue some tasks
        Console.WriteLine("\n=== Queuing Background Tasks ===\n");
        var queue = host.Services.GetRequiredService<IBackgroundTaskQueue>();

        queue.QueueTask(async (ct) =>
        {
            Console.WriteLine($"  [TASK 1] Processing email batch...");
            await Task.Delay(1000, ct);
            Console.WriteLine($"  [TASK 1] Sent 50 emails");
        });

        queue.QueueTask(async (ct) =>
        {
            Console.WriteLine($"  [TASK 2] Generating report...");
            await Task.Delay(1500, ct);
            Console.WriteLine($"  [TASK 2] Report generated: sales_2024.pdf");
        });

        queue.QueueTask(async (ct) =>
        {
            Console.WriteLine($"  [TASK 3] Processing image uploads...");
            await Task.Delay(800, ct);
            Console.WriteLine($"  [TASK 3] Processed 10 images");
        });

        // Let services run for a bit
        Console.WriteLine("\n--- Services running (will auto-stop in 15 seconds) ---\n");
        await Task.Delay(15000);

        // Shutdown
        Console.WriteLine("\n=== Shutting Down Services ===\n");
        await host.StopAsync();

        Console.WriteLine("💡 Background Services Benefits:");
        Console.WriteLine("   ✅ Long-running tasks - run continuously in background");
        Console.WriteLine("   ✅ Periodic execution - scheduled jobs (health checks, cleanup)");
        Console.WriteLine("   ✅ Queue processing - async task processing");
        Console.WriteLine("   ✅ Startup tasks - cache warming, migrations");
        Console.WriteLine("   ✅ Graceful shutdown - proper cancellation handling");
        Console.WriteLine("   ✅ Dependency injection - full DI support");

        Console.WriteLine("\n💡 Types of Hosted Services:");
        Console.WriteLine("   🔹 IHostedService: Start/Stop lifecycle methods");
        Console.WriteLine("   🔹 BackgroundService: Long-running ExecuteAsync loop");
        Console.WriteLine("   🔹 Timed services: Execute periodically");
        Console.WriteLine("   🔹 Queue processors: Process queued work items");

        Console.WriteLine("\n💡 Real-World Examples:");
        Console.WriteLine("   • Health checks and monitoring");
        Console.WriteLine("   • Data cleanup and archival");
        Console.WriteLine("   • Email/notification sending");
        Console.WriteLine("   • Report generation");
        Console.WriteLine("   • Cache warming");
        Console.WriteLine("   • Message queue processing");
        Console.WriteLine("   • Scheduled data synchronization");

        Console.WriteLine("\n💡 Best Practices:");
        Console.WriteLine("   ✅ Always respect CancellationToken");
        Console.WriteLine("   ✅ Use try-catch for error handling");
        Console.WriteLine("   ✅ Log execution and errors");
        Console.WriteLine("   ✅ Don't block - use async/await");
        Console.WriteLine("   ✅ Consider using libraries: Hangfire, Quartz.NET");
    }
}
