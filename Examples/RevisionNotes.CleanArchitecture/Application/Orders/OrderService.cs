using Microsoft.Extensions.Caching.Memory;

namespace RevisionNotes.CleanArchitecture.Application.Orders;

public sealed class OrderService(
    IOrderRepository repository,
    IMemoryCache cache)
{
    private const string OrdersCacheKey = "clean-arch:orders:all";

    public async Task<IReadOnlyList<OrderDto>> GetOrdersAsync(CancellationToken cancellationToken)
    {
        if (cache.TryGetValue(OrdersCacheKey, out IReadOnlyList<OrderDto>? cached) && cached is not null)
        {
            return cached;
        }

        var orders = await repository.GetAllAsync(cancellationToken);
        cache.Set(OrdersCacheKey, orders, TimeSpan.FromSeconds(20));
        return orders;
    }

    public Task<OrderDto?> GetOrderAsync(Guid id, CancellationToken cancellationToken) =>
        repository.GetByIdAsync(id, cancellationToken);

    public async Task<OrderDto> CreateOrderAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var created = await repository.AddAsync(command, cancellationToken);
        cache.Remove(OrdersCacheKey);
        return created;
    }
}
