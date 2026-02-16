using System.Collections.Concurrent;
using RevisionNotes.Ddd.ModularMonolith.Modules.Catalog;
using RevisionNotes.Ddd.ModularMonolith.Modules.Shared;

namespace RevisionNotes.Ddd.ModularMonolith.Modules.Billing;

public sealed record Invoice(Guid Id, string Reference, decimal Amount, DateTimeOffset CreatedAtUtc);

public interface IInvoiceRepository
{
    Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken cancellationToken);
    Task<Invoice> AddAsync(string reference, decimal amount, CancellationToken cancellationToken);
}

public sealed class InMemoryInvoiceRepository : IInvoiceRepository
{
    private readonly ConcurrentDictionary<Guid, Invoice> _invoices = new();

    public Task<IReadOnlyList<Invoice>> GetAllAsync(CancellationToken cancellationToken)
    {
        var values = _invoices.Values
            .OrderByDescending(x => x.CreatedAtUtc)
            .ToList();
        return Task.FromResult<IReadOnlyList<Invoice>>(values);
    }

    public Task<Invoice> AddAsync(string reference, decimal amount, CancellationToken cancellationToken)
    {
        var invoice = new Invoice(Guid.NewGuid(), reference, amount, DateTimeOffset.UtcNow);
        _invoices[invoice.Id] = invoice;
        return Task.FromResult(invoice);
    }
}

public sealed class BillingService(
    IInvoiceRepository repository)
{
    public Task<IReadOnlyList<Invoice>> GetInvoicesAsync(CancellationToken cancellationToken) =>
        repository.GetAllAsync(cancellationToken);

    public Task<Invoice> CreateInvoiceAsync(string reference, decimal amount, CancellationToken cancellationToken) =>
        repository.AddAsync(reference, amount, cancellationToken);
}

public sealed class CatalogToBillingProjection(
    IInvoiceRepository invoiceRepository,
    ILogger<CatalogToBillingProjection> logger)
{
    public void Register(IModularEventBus bus)
    {
        bus.Subscribe<CatalogItemCreatedEvent>(HandleAsync);
    }

    private async Task HandleAsync(CatalogItemCreatedEvent evt)
    {
        var reference = $"AUTO-{evt.CatalogItemId:N}";
        await invoiceRepository.AddAsync(reference, evt.Price, CancellationToken.None);
        logger.LogInformation("Billing projection created invoice for catalog item {CatalogItemId}", evt.CatalogItemId);
    }
}
