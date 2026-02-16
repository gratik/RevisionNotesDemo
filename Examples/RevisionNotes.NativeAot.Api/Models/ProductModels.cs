namespace RevisionNotes.NativeAot.Api.Models;

public sealed record CreateProductRequest(string Name, decimal Price);
public sealed record ProductResponse(int Id, string Name, decimal Price, DateTimeOffset CreatedAtUtc);
