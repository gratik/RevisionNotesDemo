namespace RevisionNotes.EventSourcing.Cqrs.Events;

public interface IDomainEvent
{
    string EventType { get; }
    DateTimeOffset OccurredAtUtc { get; }
}

public sealed record AccountOpenedEvent(Guid AccountId, string OwnerName, decimal InitialDeposit, DateTimeOffset OccurredAtUtc) : IDomainEvent
{
    public string EventType => "AccountOpened";
}

public sealed record FundsDepositedEvent(Guid AccountId, decimal Amount, DateTimeOffset OccurredAtUtc) : IDomainEvent
{
    public string EventType => "FundsDeposited";
}
