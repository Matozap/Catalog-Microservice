using System;
using System.Threading.Tasks;
using CatalogService.Application.ProductImages.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CatalogService.API.Endpoints.Consumers.Self;

public class ProductImageEventConsumer : IConsumer<ProductImageEvent>
{
    private readonly ILogger<ProductImageEventConsumer> _logger;

    public ProductImageEventConsumer(ILogger<ProductImageEventConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<ProductImageEvent> context)
    {
        var eventMessage = context.Message;
        _logger.LogInformation("Received own message of type {MessageType} with action '{Action}' from {Source} sent on {SentTime}", nameof(ProductImageEvent), Enum.GetName(eventMessage.Action), context.SourceAddress, context.SentTime.ToString());
        await Task.CompletedTask;
    }
}
