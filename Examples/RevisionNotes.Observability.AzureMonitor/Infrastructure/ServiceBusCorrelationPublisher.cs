using System.Diagnostics;
using Azure.Messaging.ServiceBus;

namespace RevisionNotes.Observability.AzureMonitor.Infrastructure;

public sealed class ServiceBusCorrelationPublisher
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ServiceBusCorrelationPublisher> _logger;

    public ServiceBusCorrelationPublisher(IConfiguration configuration, ILogger<ServiceBusCorrelationPublisher> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> PublishCheckoutAsync(string orderId, string correlationId, CancellationToken cancellationToken)
    {
        var connectionString = _configuration["ServiceBus:ConnectionString"];
        var topicOrQueue = _configuration["ServiceBus:EntityName"] ?? "orders-events";

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            _logger.LogWarning("Service Bus is not configured. Skipping publish for orderId={OrderId}", orderId);
            return false;
        }

        await using var client = new ServiceBusClient(connectionString);
        ServiceBusSender sender = client.CreateSender(topicOrQueue);

        var message = new ServiceBusMessage($"checkout:{orderId}")
        {
            CorrelationId = correlationId,
            Subject = "Order.CheckoutRequested"
        };

        message.ApplicationProperties["correlationId"] = correlationId;

        var traceParent = Activity.Current?.Id;
        if (!string.IsNullOrWhiteSpace(traceParent))
        {
            message.ApplicationProperties["traceparent"] = traceParent;
        }

        await sender.SendMessageAsync(message, cancellationToken);

        _logger.LogInformation("Published checkout event for orderId={OrderId} correlationId={CorrelationId}", orderId, correlationId);
        return true;
    }
}
