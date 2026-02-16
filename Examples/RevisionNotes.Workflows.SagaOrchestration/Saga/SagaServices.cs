using System.Collections.Concurrent;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.Workflows.SagaOrchestration.Contracts;

namespace RevisionNotes.Workflows.SagaOrchestration.Saga;

public enum SagaStatus
{
    Started,
    InventoryReserved,
    PaymentCharged,
    Completed,
    Compensating,
    Compensated,
    Failed
}

public sealed record SagaStep(string Name, string Status, DateTimeOffset AtUtc, string? Message = null);
public sealed record SagaState(Guid SagaId, SagaStatus Status, IReadOnlyList<SagaStep> Steps);

public sealed class SagaStateStore
{
    private readonly ConcurrentDictionary<Guid, List<SagaStep>> _steps = new();
    private readonly ConcurrentDictionary<Guid, SagaStatus> _status = new();

    public void Start(Guid sagaId)
    {
        _steps[sagaId] = [];
        _status[sagaId] = SagaStatus.Started;
        AddStep(sagaId, "Saga", "Started");
    }

    public void SetStatus(Guid sagaId, SagaStatus status, string step, string message)
    {
        _status[sagaId] = status;
        AddStep(sagaId, step, status.ToString(), message);
    }

    public SagaState? Get(Guid sagaId)
    {
        if (!_steps.TryGetValue(sagaId, out var steps) || !_status.TryGetValue(sagaId, out var status))
        {
            return null;
        }

        lock (steps)
        {
            return new SagaState(sagaId, status, [.. steps]);
        }
    }

    public int Count => _steps.Count;

    private void AddStep(Guid sagaId, string step, string status, string? message = null)
    {
        var list = _steps.GetOrAdd(sagaId, _ => []);
        lock (list)
        {
            list.Add(new SagaStep(step, status, DateTimeOffset.UtcNow, message));
        }
    }
}

public sealed class InventoryStepClient
{
    public Task ReserveAsync(string productCode, int quantity, CancellationToken cancellationToken) =>
        Task.Delay(40, cancellationToken);

    public Task ReleaseAsync(string productCode, int quantity, CancellationToken cancellationToken) =>
        Task.Delay(30, cancellationToken);
}

public sealed class PaymentStepClient
{
    public async Task ChargeAsync(decimal amount, bool forceFailure, CancellationToken cancellationToken)
    {
        await Task.Delay(50, cancellationToken);
        if (forceFailure)
        {
            throw new InvalidOperationException("Simulated payment gateway failure.");
        }
    }

    public Task RefundAsync(decimal amount, CancellationToken cancellationToken) =>
        Task.Delay(30, cancellationToken);
}

public sealed class OrderSagaOrchestrator(
    SagaStateStore store,
    InventoryStepClient inventory,
    PaymentStepClient payment,
    ILogger<OrderSagaOrchestrator> logger)
{
    public async Task<Guid> StartAsync(StartOrderSagaRequest request, CancellationToken cancellationToken)
    {
        var sagaId = Guid.NewGuid();
        store.Start(sagaId);

        try
        {
            await inventory.ReserveAsync(request.ProductCode, request.Quantity, cancellationToken);
            store.SetStatus(sagaId, SagaStatus.InventoryReserved, "Inventory", "Reserved stock");

            await payment.ChargeAsync(request.Amount, request.SimulatePaymentFailure, cancellationToken);
            store.SetStatus(sagaId, SagaStatus.PaymentCharged, "Payment", "Charged payment");

            store.SetStatus(sagaId, SagaStatus.Completed, "Saga", "Order saga completed");
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Saga {SagaId} failed. Starting compensation.", sagaId);
            store.SetStatus(sagaId, SagaStatus.Compensating, "Saga", ex.Message);

            await payment.RefundAsync(request.Amount, cancellationToken);
            await inventory.ReleaseAsync(request.ProductCode, request.Quantity, cancellationToken);

            store.SetStatus(sagaId, SagaStatus.Compensated, "Compensation", "Payment refunded and stock released");
            store.SetStatus(sagaId, SagaStatus.Failed, "Saga", "Completed with compensation");
        }

        return sagaId;
    }
}

public sealed class SagaHealthCheck(
    SagaStateStore store) : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult(HealthCheckResult.Healthy($"SagaCount={store.Count}"));
}
