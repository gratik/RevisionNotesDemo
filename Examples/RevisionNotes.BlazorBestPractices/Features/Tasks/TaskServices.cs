using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RevisionNotes.BlazorBestPractices.Domain;
using RevisionNotes.BlazorBestPractices.Infrastructure;

namespace RevisionNotes.BlazorBestPractices.Features.Tasks;

public sealed record TaskResponse(int Id, string Title, bool IsCompleted, bool IsHighPriority, DateTimeOffset CreatedAtUtc);
public sealed record CreateTaskRequest(string Title, bool IsHighPriority);

public interface ITaskRepository
{
    Task<IReadOnlyList<TaskResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken);
    Task ToggleAsync(int id, CancellationToken cancellationToken);
}

public sealed class TaskRepository(IDbContextFactory<AppDbContext> contextFactory) : ITaskRepository
{
    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        return await db.Tasks
            .OrderByDescending(x => x.IsHighPriority)
            .ThenBy(x => x.CreatedAtUtc)
            .Select(x => new TaskResponse(x.Id, x.Title, x.IsCompleted, x.IsHighPriority, x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskResponse> CreateAsync(CreateTaskRequest request, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);

        var entity = new TaskItem
        {
            Title = request.Title.Trim(),
            IsHighPriority = request.IsHighPriority,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        db.Tasks.Add(entity);
        await db.SaveChangesAsync(cancellationToken);

        return new TaskResponse(entity.Id, entity.Title, entity.IsCompleted, entity.IsHighPriority, entity.CreatedAtUtc);
    }

    public async Task ToggleAsync(int id, CancellationToken cancellationToken)
    {
        await using var db = await contextFactory.CreateDbContextAsync(cancellationToken);
        var entity = await db.Tasks.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return;
        }

        entity.IsCompleted = !entity.IsCompleted;
        await db.SaveChangesAsync(cancellationToken);
    }
}

public interface ICachedTaskQueryService
{
    Task<IReadOnlyList<TaskResponse>> GetAllAsync(CancellationToken cancellationToken);
    void Invalidate();
}

public sealed class CachedTaskQueryService(ITaskRepository repository, IMemoryCache cache) : ICachedTaskQueryService
{
    private const string CacheKey = "blazor:tasks";

    public async Task<IReadOnlyList<TaskResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(CacheKey, out IReadOnlyList<TaskResponse>? cached) && cached is not null)
        {
            return cached;
        }

        var tasks = await repository.GetAllAsync(cancellationToken);
        cache.Set(CacheKey, tasks, TimeSpan.FromSeconds(20));
        return tasks;
    }

    public void Invalidate() => cache.Remove(CacheKey);
}
