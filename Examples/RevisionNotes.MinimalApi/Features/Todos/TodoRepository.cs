using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using RevisionNotes.MinimalApi.Domain;
using RevisionNotes.MinimalApi.Infrastructure;

namespace RevisionNotes.MinimalApi.Features.Todos;

public sealed record TodoResponse(int Id, string Title, bool IsDone, bool IsPriority, DateTimeOffset CreatedAtUtc);
public sealed record CreateTodoRequest(string Title, bool IsPriority);
public sealed record UpdateTodoRequest(string Title, bool IsDone, bool IsPriority);

public interface ITodoRepository
{
    Task<IReadOnlyList<TodoResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<TodoResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<TodoResponse> CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken);
    Task<TodoResponse?> UpdateAsync(int id, UpdateTodoRequest request, CancellationToken cancellationToken);
}

public sealed class TodoRepository(AppDbContext dbContext) : ITodoRepository
{
    public async Task<IReadOnlyList<TodoResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Todos
            .OrderByDescending(x => x.IsPriority)
            .ThenBy(x => x.CreatedAtUtc)
            .Select(x => new TodoResponse(x.Id, x.Title, x.IsDone, x.IsPriority, x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<TodoResponse?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        return await dbContext.Todos
            .Where(x => x.Id == id)
            .Select(x => new TodoResponse(x.Id, x.Title, x.IsDone, x.IsPriority, x.CreatedAtUtc))
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<TodoResponse> CreateAsync(CreateTodoRequest request, CancellationToken cancellationToken)
    {
        var entity = new TodoItem
        {
            Title = request.Title.Trim(),
            IsPriority = request.IsPriority,
            CreatedAtUtc = DateTimeOffset.UtcNow
        };

        dbContext.Todos.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new TodoResponse(entity.Id, entity.Title, entity.IsDone, entity.IsPriority, entity.CreatedAtUtc);
    }

    public async Task<TodoResponse?> UpdateAsync(int id, UpdateTodoRequest request, CancellationToken cancellationToken)
    {
        var entity = await dbContext.Todos.SingleOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (entity is null)
        {
            return null;
        }

        entity.Title = request.Title.Trim();
        entity.IsDone = request.IsDone;
        entity.IsPriority = request.IsPriority;

        await dbContext.SaveChangesAsync(cancellationToken);
        return new TodoResponse(entity.Id, entity.Title, entity.IsDone, entity.IsPriority, entity.CreatedAtUtc);
    }
}

public interface ICachedTodoQueryService
{
    Task<IReadOnlyList<TodoResponse>> GetAllAsync(CancellationToken cancellationToken);
    void Invalidate();
}

public sealed class CachedTodoQueryService(ITodoRepository repository, IMemoryCache memoryCache) : ICachedTodoQueryService
{
    private static readonly string CacheKey = "todos:all";

    public async Task<IReadOnlyList<TodoResponse>> GetAllAsync(CancellationToken cancellationToken)
    {
        if (memoryCache.TryGetValue(CacheKey, out IReadOnlyList<TodoResponse>? cached) && cached is not null)
        {
            return cached;
        }

        var todos = await repository.GetAllAsync(cancellationToken);
        memoryCache.Set(CacheKey, todos, TimeSpan.FromSeconds(30));
        return todos;
    }

    public void Invalidate() => memoryCache.Remove(CacheKey);
}
