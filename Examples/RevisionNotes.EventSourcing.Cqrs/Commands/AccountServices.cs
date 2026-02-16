using RevisionNotes.EventSourcing.Cqrs.Contracts;
using RevisionNotes.EventSourcing.Cqrs.Events;
using RevisionNotes.EventSourcing.Cqrs.Infrastructure;

namespace RevisionNotes.EventSourcing.Cqrs.Commands;

public sealed class AccountCommandService(
    IEventStore eventStore,
    AccountProjectionStore projectionStore)
{
    public async Task<Guid> OpenAccountAsync(string ownerName, decimal initialDeposit, CancellationToken cancellationToken)
    {
        var id = Guid.NewGuid();
        var evt = new AccountOpenedEvent(id, ownerName.Trim(), initialDeposit, DateTimeOffset.UtcNow);
        await eventStore.AppendAsync(id, evt, cancellationToken);
        projectionStore.Apply(evt);
        return id;
    }

    public async Task DepositAsync(Guid accountId, decimal amount, CancellationToken cancellationToken)
    {
        if (amount <= 0)
        {
            throw new InvalidOperationException("Deposit amount must be greater than zero.");
        }

        var exists = projectionStore.Get(accountId) is not null;
        if (!exists)
        {
            throw new KeyNotFoundException("Account not found.");
        }

        var evt = new FundsDepositedEvent(accountId, amount, DateTimeOffset.UtcNow);
        await eventStore.AppendAsync(accountId, evt, cancellationToken);
        projectionStore.Apply(evt);
    }
}

public sealed class AccountQueryService(
    AccountProjectionStore projectionStore)
{
    public AccountReadModel? GetAccount(Guid accountId) => projectionStore.Get(accountId);
}
