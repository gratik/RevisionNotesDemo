namespace RevisionNotes.Ddd.ModularMonolith.Contracts;

public sealed record LoginRequest(string Username, string Password);
public sealed record CreateCatalogItemRequest(string Name, decimal Price);
public sealed record CreateInvoiceRequest(string Reference, decimal Amount);
