namespace RevisionNotes.CleanArchitecture.Domain.Orders;

public sealed class Order
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public required string CustomerId { get; init; }
    public required decimal TotalAmount { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; } = DateTimeOffset.UtcNow;
    public string Status { get; private set; } = "Created";

    public void MarkPaid() => Status = "Paid";
}
