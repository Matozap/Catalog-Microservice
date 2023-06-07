using System;
using System.Threading.Tasks;
using CatalogService.Application.ProductCategories.Events;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace CatalogService.API.Endpoints.Consumers.Self;

public class ProductCategoryEventConsumer : IConsumer<ProductCategoryEvent>
{
    private readonly ILogger<ProductCategoryEventConsumer> _logger;

    public ProductCategoryEventConsumer(ILogger<ProductCategoryEventConsumer> logger)
    {
        _logger = logger;
    }
    
    public async Task Consume(ConsumeContext<ProductCategoryEvent> context)
    {
        var eventMessage = context.Message;
        _logger.LogInformation("Received own message of type {MessageType} with action '{Action}' from {Source} sent on {SentTime}", nameof(ProductCategoryEvent), Enum.GetName(eventMessage.Action), context.SourceAddress, context.SentTime.ToString());
        await Task.CompletedTask;
    }
}
