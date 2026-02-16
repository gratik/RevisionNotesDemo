using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using RevisionNotes.ApiGateway.BFF.Contracts;

namespace RevisionNotes.ApiGateway.BFF.Infrastructure;

public interface IProfileClient
{
    Task<ProfileSummary> GetProfileAsync(string userId, CancellationToken cancellationToken);
}

public interface IOrdersClient
{
    Task<IReadOnlyList<OrderSummary>> GetRecentOrdersAsync(string userId, CancellationToken cancellationToken);
}

public sealed class FakeProfileClient : IProfileClient
{
    public async Task<ProfileSummary> GetProfileAsync(string userId, CancellationToken cancellationToken)
    {
        await Task.Delay(70, cancellationToken);
        return new ProfileSummary(userId, $"{userId} display", "Gold");
    }
}

public sealed class FakeOrdersClient : IOrdersClient
{
    public async Task<IReadOnlyList<OrderSummary>> GetRecentOrdersAsync(string userId, CancellationToken cancellationToken)
    {
        await Task.Delay(90, cancellationToken);
        return
        [
            new OrderSummary("ord-1001", 149.90m, "Paid"),
            new OrderSummary("ord-1002", 49.00m, "Shipped")
        ];
    }
}

public sealed class DashboardAggregatorService(
    IProfileClient profileClient,
    IOrdersClient ordersClient,
    IMemoryCache cache,
    ILogger<DashboardAggregatorService> logger)
{
    public async Task<DashboardResponse> GetDashboardAsync(string userId, CancellationToken cancellationToken)
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(TimeSpan.FromMilliseconds(600));

        try
        {
            var profileTask = profileClient.GetProfileAsync(userId, cts.Token);
            var ordersTask = ordersClient.GetRecentOrdersAsync(userId, cts.Token);

            await Task.WhenAll(profileTask, ordersTask);
            var response = new DashboardResponse(profileTask.Result, ordersTask.Result, UsedFallback: false);
            cache.Set($"bff:dashboard:{userId}", response, TimeSpan.FromSeconds(20));
            return response;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            logger.LogWarning(ex, "Downstream call failed; returning cached fallback if available.");
            if (cache.TryGetValue($"bff:dashboard:{userId}", out DashboardResponse? cached) && cached is not null)
            {
                return cached with { UsedFallback = true };
            }

            return new DashboardResponse(
                new ProfileSummary(userId, userId, "Unknown"),
                [],
                UsedFallback: true);
        }
    }
}

public sealed class GatewayHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default) =>
        Task.FromResult(HealthCheckResult.Healthy("Gateway and simulated downstream clients are available."));
}
