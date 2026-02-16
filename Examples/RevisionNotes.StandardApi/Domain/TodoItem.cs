namespace RevisionNotes.StandardApi.Domain;

public sealed class TodoItem
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public bool IsDone { get; set; }
    public bool IsPriority { get; set; }
    public DateTimeOffset CreatedAtUtc { get; set; }
}