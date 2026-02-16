namespace RevisionNotes.BlazorBestPractices.Domain;

public sealed class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsCompleted { get; set; }
    public bool IsHighPriority { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}