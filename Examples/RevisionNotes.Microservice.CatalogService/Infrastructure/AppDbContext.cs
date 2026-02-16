using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RevisionNotes.Microservice.CatalogService.Domain;

namespace RevisionNotes.Microservice.CatalogService.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<CatalogItem> CatalogItems => Set<CatalogItem>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
}

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.CatalogItems.AnyAsync())
        {
            return;
        }

        db.CatalogItems.AddRange(
            new CatalogItem { Sku = "SKU-1001", Name = "Keyboard", Price = 49.90m, CreatedAtUtc = DateTimeOffset.UtcNow },
            new CatalogItem { Sku = "SKU-1002", Name = "Mouse", Price = 29.90m, CreatedAtUtc = DateTimeOffset.UtcNow });

        db.OutboxMessages.Add(new OutboxMessage
        {
            EventType = "Catalog.Seeded",
            Payload = JsonSerializer.Serialize(new { Count = 2 }),
            CreatedAtUtc = DateTimeOffset.UtcNow,
            ProcessedAtUtc = DateTimeOffset.UtcNow
        });

        await db.SaveChangesAsync();
    }
}

public sealed class IdempotencyMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IMemoryCache cache)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
        {
            await next(context);
            return;
        }

        var key = context.Request.Headers["Idempotency-Key"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(key))
        {
            await next(context);
            return;
        }

        if (cache.TryGetValue($"idempotency:{key}", out _))
        {
            context.Response.StatusCode = StatusCodes.Status409Conflict;
            await context.Response.WriteAsJsonAsync(new { message = "Duplicate idempotency key." });
            return;
        }

        cache.Set($"idempotency:{key}", true, TimeSpan.FromMinutes(10));
        await next(context);
    }
}

public interface IOutboxWriter
{
    Task EnqueueAsync(string eventType, object payload, CancellationToken cancellationToken);
}

public sealed class OutboxWriter(AppDbContext dbContext) : IOutboxWriter
{
    public async Task EnqueueAsync(string eventType, object payload, CancellationToken cancellationToken)
    {
        dbContext.OutboxMessages.Add(new OutboxMessage
        {
            EventType = eventType,
            Payload = JsonSerializer.Serialize(payload),
            CreatedAtUtc = DateTimeOffset.UtcNow
        });

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}

public sealed class OutboxDispatcher(IServiceScopeFactory scopeFactory, ILogger<OutboxDispatcher> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var pending = await dbContext.OutboxMessages
                    .Where(x => x.ProcessedAtUtc == null)
                    .OrderBy(x => x.CreatedAtUtc)
                    .Take(20)
                    .ToListAsync(stoppingToken);

                foreach (var message in pending)
                {
                    logger.LogInformation("Dispatching outbox event {EventType} with payload {Payload}", message.EventType, message.Payload);
                    message.ProcessedAtUtc = DateTimeOffset.UtcNow;
                }

                if (pending.Count > 0)
                {
                    await dbContext.SaveChangesAsync(stoppingToken);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Outbox dispatch failed");
            }

            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
        }
    }
}
