namespace RevisionNotes.EventSourcing.Cqrs.Contracts;

public sealed record LoginRequest(string Username, string Password);
public sealed record OpenAccountRequest(string OwnerName, decimal InitialDeposit);
public sealed record DepositFundsRequest(decimal Amount);
public sealed record AccountReadModel(Guid AccountId, string OwnerName, decimal Balance, DateTimeOffset UpdatedAtUtc);
