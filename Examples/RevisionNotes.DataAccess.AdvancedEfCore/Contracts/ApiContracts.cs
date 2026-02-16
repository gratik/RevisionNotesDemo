namespace RevisionNotes.DataAccess.AdvancedEfCore.Contracts;

public sealed record LoginRequest(string Username, string Password);
public sealed record CreateProductRequest(string Name, decimal Price, string[] Tags);
public sealed record UpdateProductRequest(string Name, decimal Price, int ExpectedVersion);
public sealed record ProductResponse(int Id, string Name, decimal Price, int Version, string[] Tags);
