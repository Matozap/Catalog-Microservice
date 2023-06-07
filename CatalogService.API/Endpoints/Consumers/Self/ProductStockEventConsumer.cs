using System;
using System.Threading.Tasks;
using CatalogService.Application.ProductStock.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CatalogService.API.Endpoints.Consumers.Self;

public class ProductStockEventConsumer : IConsumer<ProductStockEvent>
{
    private readonly ILogger<ProductStockEventConsumer> _logger;

    public ProductStockEventConsumer(ILogger<ProductStockEventConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<ProductStockEvent> context)
    {
        var eventMessage = context.Message;
        _logger.LogInformation("Received own message of type {MessageType} with action '{Action}' from {Source} sent on {SentTime}", nameof(ProductStockEvent), Enum.GetName(eventMessage.Action), context.SourceAddress, context.SentTime.ToString());
        await Task.CompletedTask;
    }
}
