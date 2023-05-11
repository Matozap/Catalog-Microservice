using System;
using System.Threading.Tasks;
using CatalogService.Message.Events;
using CatalogService.Message.Events.Cache;
using CatalogService.Message.Events.ProductImages.v1;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.API.Inputs.Consumers.Self;

public class ProductImageEventConsumer : IConsumer<ProductImageEvent>
{
    private readonly ILogger<ProductImageEventConsumer> _logger;
    private readonly IMediator _mediator;

    public ProductImageEventConsumer(ILogger<ProductImageEventConsumer> logger, IMediator mediator)
    {
        _logger = logger;
        _mediator = mediator;
    }
    
    public async Task Consume(ConsumeContext<ProductImageEvent> context)
    {
        try
        {
            _logger.LogInformation("Received message of type {MessageType} from {Source} sent on {SentTime}", nameof(ProductImageEvent), context.SourceAddress, context.SentTime.ToString());
            var catalogEvent = context.Message;
            switch (catalogEvent.Action)
            {
                case EventAction.Created:
                case EventAction.Updated: 
                case EventAction.Deleted:
                    _logger.LogDebug("Cache key removal triggered by {Event} for id {Id}", nameof(ProductImageEvent), catalogEvent.Details.Id);
                    _ = _mediator.Send(new ClearCache
                    {
                        ProductImageId = catalogEvent.Details.Id,
                        ProductImageCode = catalogEvent.Details.Code,
                        ProductId = catalogEvent.Details.ProductId
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
            _logger.LogError("Cannot consume {Event} event - {Error}",nameof(ProductImageEvent), e.Message);
            throw;
        }
    }
}
