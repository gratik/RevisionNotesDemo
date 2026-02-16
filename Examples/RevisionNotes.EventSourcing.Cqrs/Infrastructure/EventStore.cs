using System.Collections.Concurrent;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.EventSourcing.Cqrs.Contracts;
using RevisionNotes.EventSourcing.Cqrs.Events;

namespace RevisionNotes.EventSourcing.Cqrs.Infrastructure;

public interface IEventStore
{
    Task AppendAsync(Guid aggregateId, IDomainEvent domainEvent, CancellationToken cancellationToken);
    IReadOnlyList<IDomainEvent> GetEvents(Guid aggregateId);
    int Count { get; }
}

public sealed class InMemoryEventStore : IEventStore
{
    private readonly ConcurrentDictionary<Guid, List<IDomainEvent>> _streams = new();
    private int _count;

    public int Count => Volatile.Read(ref _count);

    public Task AppendAsync(Guid aggregateId, IDomainEvent domainEvent, CancellationToken cancellationToken)
    {
        var stream = _streams.GetOrAdd(aggregateId, _ => []);
        lock (stream)
        {
            stream.Add(domainEvent);
        }

        Interlocked.Increment(ref _count);
        return Task.CompletedTask;
    }

    public IReadOnlyList<IDomainEvent> GetEvents(Guid aggregateId)
    {
        if (!_streams.TryGetValue(aggregateId, out var stream))
        {
            return [];
        }

        lock (stream)
        {
            return [.. stream];
        }
    }
}

public sealed class AccountProjectionStore
{
    private readonly ConcurrentDictionary<Guid, AccountReadModel> _accounts = new();

    public void Apply(AccountOpenedEvent evt)
    {
        _accounts[evt.AccountId] = new AccountReadModel(
            evt.AccountId,
            evt.OwnerName,
            evt.InitialDeposit,
            evt.OccurredAtUtc);
    }

    public void Apply(FundsDepositedEvent evt)
    {
        if (_accounts.TryGetValue(evt.AccountId, out var current))
        {
            _accounts[evt.AccountId] = current with
            {
                Balance = current.Balance + evt.Amount,
                UpdatedAtUtc = evt.OccurredAtUtc
            };
        }
    }

    public AccountReadModel? Get(Guid accountId)
    {
        _accounts.TryGetValue(accountId, out var value);
        return value;
    }
}

public sealed class EventStoreHealthCheck(
    IEventStore eventStore) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult(HealthCheckResult.Healthy($"EventCount={eventStore.Count}"));
}
