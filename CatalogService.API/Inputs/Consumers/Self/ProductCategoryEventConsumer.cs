using System;
using System.Threading.Tasks;
using CatalogService.Message.Events;
using CatalogService.Message.Events.Cache;
using CatalogService.Message.Events.ProductCategories.v1;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.API.Inputs.Consumers.Self;

public class ProductCategoryEventConsumer : IConsumer<ProductCategoryEvent>
{
    private readonly ILogger<ProductCategoryEventConsumer> _logger;
    private readonly IMediator _mediator;

    public ProductCategoryEventConsumer(ILogger<ProductCategoryEventConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<ProductCategoryEvent> context)
    {
        try
        {
            _logger.LogInformation("Received message of type {MessageType} from {Source} sent on {SentTime}", nameof(ProductCategoryEvent), context.SourceAddress, context.SentTime.ToString());
            var catalogEvent = context.Message;
            switch (catalogEvent.Action)
            {
                case EventAction.Created:
                case EventAction.Updated: 
                case EventAction.Deleted: 
                    _logger.LogDebug("Cache key removal triggered by {Event} for id {Id}", nameof(ProductCategoryEvent), catalogEvent.Details.Id);
                    _ = _mediator.Send(new ClearCache
                    {
                        ProductCategoryId = catalogEvent.Details.Id
                    });
                    break;

                case EventAction.None:
                default:
                    await Task.CompletedTask;
                    break;
            }
        }
        catch (Exception e)
        {
            _logger.LogError("Cannot consume {Event} event - {Error}",nameof(ProductCategoryEvent), e.Message);
            throw;
        }
    }
}
