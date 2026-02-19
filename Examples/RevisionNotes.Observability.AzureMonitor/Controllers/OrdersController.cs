using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RevisionNotes.Observability.AzureMonitor.Infrastructure;

namespace RevisionNotes.Observability.AzureMonitor.Controllers;

[ApiController]
[Route("api/orders")]
public sealed class OrdersController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ServiceBusCorrelationPublisher _publisher;
    private readonly ILogger<OrdersController> _logger;

    public OrdersController(
        IHttpClientFactory httpClientFactory,
        ServiceBusCorrelationPublisher publisher,
        ILogger<OrdersController> logger)
    {
        _httpClientFactory = httpClientFactory;
        _publisher = publisher;
        _logger = logger;
    }

    [HttpPost("{orderId}/checkout")]
    public async Task<IActionResult> Checkout(string orderId, CancellationToken cancellationToken)
    {
        var correlationId = HttpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString() ?? Guid.NewGuid().ToString("N");

        _logger.LogInformation("Checkout started for orderId={OrderId}", orderId);

        var client = _httpClientFactory.CreateClient("downstream");
        client.DefaultRequestHeaders.Remove(CorrelationIdMiddleware.HeaderName);
        client.DefaultRequestHeaders.Add(CorrelationIdMiddleware.HeaderName, correlationId);

        var downstreamResponse = await client.PostAsync($"api/payments/{orderId}", content: null, cancellationToken);

        var published = await _publisher.PublishCheckoutAsync(orderId, correlationId, cancellationToken);

        _logger.LogInformation(
            "Checkout completed for orderId={OrderId} status={StatusCode} serviceBusPublished={Published}",
            orderId,
            (int)downstreamResponse.StatusCode,
            published);

        return Ok(new
        {
            orderId,
            correlationId,
            traceId = Activity.Current?.TraceId.ToString(),
            downstreamStatus = (int)downstreamResponse.StatusCode,
            serviceBusPublished = published
        });
    }
}
