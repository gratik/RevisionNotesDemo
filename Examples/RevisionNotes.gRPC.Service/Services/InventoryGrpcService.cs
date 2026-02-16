using Microsoft.AspNetCore.Authorization;
using RevisionNotes.gRPC.Service.Infrastructure;

namespace RevisionNotes.gRPC.Service.Services;

[Authorize(Policy = "grpc.read")]
public sealed class InventoryGrpcService(IInventoryRepository repository) : InventoryService.InventoryServiceBase
{
    public override async Task<GetItemReply> GetItem(GetItemRequest request, Grpc.Core.ServerCallContext context)
    {
        var sku = request.Sku.Trim();
        if (string.IsNullOrWhiteSpace(sku))
        {
            throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.InvalidArgument, "sku is required"));
        }

        var item = await repository.GetBySkuAsync(sku, context.CancellationToken);
        if (item is null)
        {
            throw new Grpc.Core.RpcException(new Grpc.Core.Status(Grpc.Core.StatusCode.NotFound, "item not found"));
        }

        return new GetItemReply
        {
            Sku = item.Sku,
            Name = item.Name,
            Available = item.Available,
            Price = Convert.ToDouble(item.Price)
        };
    }

    public override async Task StreamLowStock(LowStockRequest request, Grpc.Core.IServerStreamWriter<StockEvent> responseStream, Grpc.Core.ServerCallContext context)
    {
        var threshold = request.Threshold <= 0 ? 10 : request.Threshold;
        var items = await repository.GetLowStockAsync(threshold, context.CancellationToken);

        foreach (var item in items)
        {
            await responseStream.WriteAsync(new StockEvent
            {
                Sku = item.Sku,
                Available = item.Available,
                Message = $"{item.Name} is below threshold {threshold}"
            }, context.CancellationToken);

            await Task.Delay(TimeSpan.FromMilliseconds(200), context.CancellationToken);
        }
    }
}