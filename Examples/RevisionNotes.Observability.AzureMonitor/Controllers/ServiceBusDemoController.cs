using Microsoft.AspNetCore.Mvc;
using RevisionNotes.Observability.AzureMonitor.Infrastructure;

namespace RevisionNotes.Observability.AzureMonitor.Controllers;

[ApiController]
[Route("api/demo/servicebus")]
public sealed class ServiceBusDemoController : ControllerBase
{
    private readonly ServiceBusCorrelationConsumerDemo _consumerDemo;

    public ServiceBusDemoController(ServiceBusCorrelationConsumerDemo consumerDemo)
    {
        _consumerDemo = consumerDemo;
    }

    [HttpPost("consume")]
    public IActionResult Consume([FromBody] ConsumeRequest request)
    {
        _consumerDemo.Process(request.Body, request.CorrelationId, request.TraceParent);

        return Ok(new
        {
            status = "processed",
            request.CorrelationId,
            request.TraceParent
        });
    }

    public sealed record ConsumeRequest(string Body, string? CorrelationId, string? TraceParent);
}
