using Microsoft.EntityFrameworkCore;
using RevisionNotes.MinimalApi.Domain;

namespace RevisionNotes.MinimalApi.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> Todos => Set<TodoItem>();
}

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Todos.AnyAsync())
        {
            return;
        }

        db.Todos.AddRange(
            new TodoItem { Title = "Document architecture decisions", IsPriority = true, CreatedAtUtc = DateTimeOffset.UtcNow },
            new TodoItem { Title = "Enable output caching", IsPriority = false, CreatedAtUtc = DateTimeOffset.UtcNow },
            new TodoItem { Title = "Add authentication and authorization", IsPriority = true, CreatedAtUtc = DateTimeOffset.UtcNow });

        await db.SaveChangesAsync();
    }
}