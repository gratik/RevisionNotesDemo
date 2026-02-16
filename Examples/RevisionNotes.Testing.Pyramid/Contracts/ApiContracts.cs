namespace RevisionNotes.Testing.Pyramid.Contracts;

public sealed record LoginRequest(string Username, string Password);
public sealed record OrderContractResponse(int Id, string CustomerId, decimal TotalAmount, string Status);
public sealed record OrderScoreRequest(decimal TotalAmount, bool IsPriorityCustomer, bool HasFraudSignals);
