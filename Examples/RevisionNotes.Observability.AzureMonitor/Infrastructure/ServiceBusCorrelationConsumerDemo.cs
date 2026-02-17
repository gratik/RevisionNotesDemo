using System.Diagnostics;

namespace RevisionNotes.Observability.AzureMonitor.Infrastructure;

public sealed class ServiceBusCorrelationConsumerDemo
{
    private static readonly ActivitySource ActivitySource = new("RevisionNotes.Observability.AzureMonitor.Consumer");
    private readonly ILogger<ServiceBusCorrelationConsumerDemo> _logger;

    public ServiceBusCorrelationConsumerDemo(ILogger<ServiceBusCorrelationConsumerDemo> logger)
    {
        _logger = logger;
    }

    public void Process(string body, string? correlationId, string? traceParent)
    {
        correlationId ??= "unknown";

        if (!string.IsNullOrWhiteSpace(traceParent))
        {
            var parentContext = ActivityContext.Parse(traceParent, traceState: null);
            using var activity = ActivitySource.StartActivity("servicebus.process", ActivityKind.Consumer, parentContext);
            activity?.SetTag("messaging.system", "azure_service_bus");
            activity?.SetTag("app.correlation_id", correlationId);

            using (_logger.BeginScope(new Dictionary<string, object?>
            {
                ["correlationId"] = correlationId,
                ["traceId"] = Activity.Current?.TraceId.ToString()
            }))
            {
                _logger.LogInformation("Processed message body={Body}", body);
            }

            return;
        }

        using (_logger.BeginScope(new Dictionary<string, object?>
        {
            ["correlationId"] = correlationId
        }))
        {
            _logger.LogInformation("Processed message without traceparent body={Body}", body);
        }
    }
}
