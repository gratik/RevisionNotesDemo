using System.Collections.Concurrent;
using RevisionNotes.CleanArchitecture.Application.Orders;
using RevisionNotes.CleanArchitecture.Domain.Orders;

namespace RevisionNotes.CleanArchitecture.Infrastructure.Orders;

public sealed class InMemoryOrderRepository : IOrderRepository
{
    private readonly ConcurrentDictionary<Guid, Order> _orders = new();

    public InMemoryOrderRepository()
    {
        var seeded = new Order
        {
            CustomerId = "customer-001",
            TotalAmount = 149.99m
        };
        seeded.MarkPaid();
        _orders.TryAdd(seeded.Id, seeded);
    }

    public Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        _orders.TryGetValue(id, out var order);
        return Task.FromResult(order is null ? null : ToDto(order));
    }

    public Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        var list = _orders.Values
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(ToDto)
            .ToList();

        return Task.FromResult<IReadOnlyList<OrderDto>>(list);
    }

    public Task<OrderDto> AddAsync(CreateOrderCommand command, CancellationToken cancellationToken)
    {
        var order = new Order
        {
            CustomerId = command.CustomerId,
            TotalAmount = command.TotalAmount
        };

        _orders.TryAdd(order.Id, order);
        return Task.FromResult(ToDto(order));
    }

    private static OrderDto ToDto(Order order) =>
        new(order.Id, order.CustomerId, order.TotalAmount, order.Status, order.CreatedAtUtc);
}
