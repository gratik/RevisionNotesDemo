namespace RevisionNotes.CleanArchitecture.Contracts;

public sealed record LoginRequest(string Username, string Password);
public sealed record CreateOrderRequest(string CustomerId, decimal TotalAmount);
