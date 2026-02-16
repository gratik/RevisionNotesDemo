namespace RevisionNotes.gRPC.Service.Domain;

public sealed record InventoryItem(string Sku, string Name, int Available, decimal Price);