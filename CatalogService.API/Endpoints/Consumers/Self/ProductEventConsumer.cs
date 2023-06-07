using System;
using System.Threading.Tasks;
using CatalogService.Application.Products.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CatalogService.API.Endpoints.Consumers.Self;

public class ProductEventConsumer : IConsumer<ProductEvent>
{
    private readonly ILogger<ProductEventConsumer> _logger;

    public ProductEventConsumer(ILogger<ProductEventConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<ProductEvent> context)
    {
        var eventMessage = context.Message;
        _logger.LogInformation("Received own message of type {MessageType} with action '{Action}' from {Source} sent on {SentTime}", nameof(ProductEvent), Enum.GetName(eventMessage.Action), context.SourceAddress, context.SentTime.ToString());
        await Task.CompletedTask;
    }
}
