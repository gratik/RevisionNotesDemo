using Microsoft.AspNetCore.Mvc;
using RevisionNotes.Observability.AzureMonitor.Infrastructure;

namespace RevisionNotes.Observability.AzureMonitor.Controllers;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController : ControllerBase
{
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(ILogger<PaymentsController> logger)
    {
        _logger = logger;
    }

    [HttpPost("{orderId}")]
    public IActionResult Pay(string orderId)
    {
        var correlationId = HttpContext.Items[CorrelationIdMiddleware.HeaderName]?.ToString();
        _logger.LogInformation("Payment received for orderId={OrderId} correlationId={CorrelationId}", orderId, correlationId);

        return Accepted(new
        {
            orderId,
            paymentStatus = "accepted",
            correlationId
        });
    }
}
