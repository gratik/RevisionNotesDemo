using System.Collections.Concurrent;

namespace RevisionNotes.Testing.Pyramid.Infrastructure;

public sealed record OrderReadModel(int Id, string CustomerId, decimal TotalAmount, string Status);

public interface IOrderReadStore
{
    Task<OrderReadModel?> GetByIdAsync(int id, CancellationToken cancellationToken);
}

public sealed class InMemoryOrderReadStore : IOrderReadStore
{
    private readonly ConcurrentDictionary<int, OrderReadModel> _orders = new(
        new[]
        {
            new KeyValuePair<int, OrderReadModel>(1, new OrderReadModel(1, "customer-001", 219.50m, "Created")),
            new KeyValuePair<int, OrderReadModel>(2, new OrderReadModel(2, "customer-099", 1200.00m, "Paid"))
        });

    public Task<OrderReadModel?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order);
    }
}
