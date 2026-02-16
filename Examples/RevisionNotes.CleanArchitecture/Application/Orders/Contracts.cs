namespace RevisionNotes.CleanArchitecture.Application.Orders;

public sealed record CreateOrderCommand(string CustomerId, decimal TotalAmount);
public sealed record OrderDto(Guid Id, string CustomerId, decimal TotalAmount, string Status, DateTimeOffset CreatedAtUtc);

public interface IOrderRepository
{
    Task<OrderDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IReadOnlyList<OrderDto>> GetAllAsync(CancellationToken cancellationToken);
    Task<OrderDto> AddAsync(CreateOrderCommand command, CancellationToken cancellationToken);
}
