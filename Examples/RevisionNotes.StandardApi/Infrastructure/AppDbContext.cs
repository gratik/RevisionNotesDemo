using Microsoft.EntityFrameworkCore;
using RevisionNotes.StandardApi.Domain;

namespace RevisionNotes.StandardApi.Infrastructure;

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
            new TodoItem { Title = "Enforce API authorization", IsPriority = true, CreatedAtUtc = DateTimeOffset.UtcNow },
            new TodoItem { Title = "Tune response caching", IsPriority = false, CreatedAtUtc = DateTimeOffset.UtcNow });

        await db.SaveChangesAsync();
    }
}