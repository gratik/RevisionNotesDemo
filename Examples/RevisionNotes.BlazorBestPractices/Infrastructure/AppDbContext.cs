using Microsoft.EntityFrameworkCore;
using RevisionNotes.BlazorBestPractices.Domain;

namespace RevisionNotes.BlazorBestPractices.Infrastructure;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TaskItem> Tasks => Set<TaskItem>();
}

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Tasks.AnyAsync())
        {
            return;
        }

        db.Tasks.AddRange(
            new TaskItem { Title = "Design accessible nav", IsHighPriority = true, CreatedAtUtc = DateTimeOffset.UtcNow },
            new TaskItem { Title = "Apply response caching", IsHighPriority = false, CreatedAtUtc = DateTimeOffset.UtcNow },
            new TaskItem { Title = "Validate auth flow", IsHighPriority = true, CreatedAtUtc = DateTimeOffset.UtcNow });

        await db.SaveChangesAsync();
    }
}