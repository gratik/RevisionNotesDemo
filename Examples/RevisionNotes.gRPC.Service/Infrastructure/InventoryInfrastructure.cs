using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Memory;
using RevisionNotes.gRPC.Service.Domain;

namespace RevisionNotes.gRPC.Service.Infrastructure;

public interface IInventoryRepository
{
    Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken);
    Task<IReadOnlyList<InventoryItem>> GetLowStockAsync(int threshold, CancellationToken cancellationToken);
}

public sealed class InMemoryInventoryRepository(IMemoryCache cache) : IInventoryRepository
{
    private readonly ConcurrentDictionary<string, InventoryItem> _items = new(StringComparer.OrdinalIgnoreCase)
    {
        ["SKU-1001"] = new("SKU-1001", "Mechanical Keyboard", 42, 89.0m),
        ["SKU-1002"] = new("SKU-1002", "Wireless Mouse", 8, 39.0m),
        ["SKU-1003"] = new("SKU-1003", "USB-C Dock", 3, 129.0m),
        ["SKU-1004"] = new("SKU-1004", "4K Monitor", 2, 379.0m)
    };

    public Task<InventoryItem?> GetBySkuAsync(string sku, CancellationToken cancellationToken)
    {
        var key = $"inventory:sku:{sku}";
        if (cache.TryGetValue(key, out InventoryItem? cached) && cached is not null)
        {
            return Task.FromResult<InventoryItem?>(cached);
        }

        _items.TryGetValue(sku, out var item);
        if (item is not null)
        {
            cache.Set(key, item, TimeSpan.FromSeconds(20));
        }

        return Task.FromResult(item);
    }

    public Task<IReadOnlyList<InventoryItem>> GetLowStockAsync(int threshold, CancellationToken cancellationToken)
    {
        var lowStock = _items.Values
            .Where(x => x.Available <= threshold)
            .OrderBy(x => x.Available)
            .ToList();

        return Task.FromResult<IReadOnlyList<InventoryItem>>(lowStock);
    }
}

public sealed class GrpcExceptionInterceptor(ILogger<GrpcExceptionInterceptor> logger) : Grpc.Core.Interceptors.Interceptor
{
    public override async Task<TResponse> UnaryServerHandler<TRequest, TResponse>(
        TRequest request,
        Grpc.Core.ServerCallContext context,
        Grpc.Core.UnaryServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            logger.LogInformation("gRPC unary call {Method}", context.Method);
            return await continuation(request, context);
        }
        catch (Grpc.Core.RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in unary call {Method}", context.Method);
            throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Internal, "Unexpected server error"));
        }
    }

    public override async Task ServerStreamingServerHandler<TRequest, TResponse>(
        TRequest request,
        Grpc.Core.IServerStreamWriter<TResponse> responseStream,
        Grpc.Core.ServerCallContext context,
        Grpc.Core.ServerStreamingServerMethod<TRequest, TResponse> continuation)
    {
        try
        {
            logger.LogInformation("gRPC server-streaming call {Method}", context.Method);
            await continuation(request, responseStream, context);
        }
        catch (Grpc.Core.RpcException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception in streaming call {Method}", context.Method);
            throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.Internal, "Unexpected server error"));
        }
    }
}