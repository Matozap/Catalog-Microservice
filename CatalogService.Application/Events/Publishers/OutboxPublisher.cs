using System;
using System.Text.Json;
using System.Threading.Tasks;
using Bustr.Bus;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductCategories.v1;
using CatalogService.Message.Contracts.ProductStock.v1;
using CatalogService.Message.Contracts.Products.v1;
using CatalogService.Message.Contracts.ProductImages.v1;
using CatalogService.Message.Events;
using CatalogService.Message.Events.ProductCategories.v1;
using CatalogService.Message.Events.ProductStock.v1;
using CatalogService.Message.Events.Products.v1;
using CatalogService.Message.Events.ProductImages.v1;
using Mapster;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Events.Publishers;

public class OutboxPublisher : IOutboxPublisher
{
    private readonly IEventBus _eventBus;
    private readonly IRepository _repository;
    private readonly ILogger<OutboxPublisher> _logger;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new() { PropertyNameCaseInsensitive = true };

    public OutboxPublisher(IEventBus eventBus, IRepository repository, ILogger<OutboxPublisher> logger)
    {
        _eventBus = eventBus;
        _repository = repository;
        _logger = logger;
    }

    public async Task PublishOutboxAsync()
    {
        var outboxMessages = await _repository.GetAsListAsync<Outbox, DateTime>(outbox => outbox.Id != "", outbox => outbox.LastUpdateDate);
        if (!(outboxMessages?.Count > 0)) return;
        
        _logger.LogInformation("Publishing events from outbox - Count: {MessageCount}", outboxMessages.Count.ToString());
        foreach (var outboxMessage in outboxMessages)
        {
            switch (outboxMessage.ObjectType)
            {
                case nameof(ProductCategory):
                    await PublishProductCategoryEvent(outboxMessage);
                    break;
                
                case nameof(Product):
                    await PublishProductEvent(outboxMessage);
                    break;
                    
                case nameof(ProductImage):
                    await PublishProductImageEvent(outboxMessage);
                    break;
                
                case nameof(ProductStock):
                    await PublishProductStockEvent(outboxMessage);
                    break;
                default:
                    _logger.LogWarning("Unknown entity found in EventBusOutbox - Entity Name: {ObjectType}", outboxMessage.ObjectType);
                    break;
            }

            await _repository.DeleteAsync(outboxMessage, skipOutbox: true);
        }
    } 
    
    private async Task PublishProductCategoryEvent(Outbox outboxMessage)
    {
        var product =  JsonSerializer.Deserialize<ProductCategory>(outboxMessage.JsonObject, _jsonSerializerOptions);
        var productData = product.Adapt<ProductCategory, ProductCategoryData>();
        
        switch (outboxMessage.Operation)
        {
            case Operation.Create:
                await _eventBus.PublishAsync(new ProductCategoryEvent { Details = productData, Action = EventAction.Created });
                break;
            case Operation.Update:
                await _eventBus.PublishAsync(new ProductCategoryEvent { Details = productData, Action = EventAction.Updated });
                break;
            case Operation.Delete:
                await _eventBus.PublishAsync(new ProductCategoryEvent { Details = productData, Action = EventAction.Deleted });
                break;
            case Operation.None:
            default:
                _logger.LogWarning("Unknown operation found in EventBusOutbox - Entity Name: {ObjectType}, Operation: {Operation}", outboxMessage.ObjectType, Enum.GetName(outboxMessage.Operation));
                break;
        }
    }
    
    private async Task PublishProductEvent(Outbox outboxMessage)
    {
        var product =  JsonSerializer.Deserialize<Product>(outboxMessage.JsonObject, _jsonSerializerOptions);
        var productData = product.Adapt<Product, ProductData>();
        
        switch (outboxMessage.Operation)
        {
            case Operation.Create:
                await _eventBus.PublishAsync(new ProductEvent { Details = productData, Action = EventAction.Created });
                break;
            case Operation.Update:
                await _eventBus.PublishAsync(new ProductEvent { Details = productData, Action = EventAction.Updated });
                break;
            case Operation.Delete:
                await _eventBus.PublishAsync(new ProductEvent { Details = productData, Action = EventAction.Deleted });
                break;
            case Operation.None:
            default:
                _logger.LogWarning("Unknown operation found in EventBusOutbox - Entity Name: {ObjectType}, Operation: {Operation}", outboxMessage.ObjectType, Enum.GetName(outboxMessage.Operation));
                break;
        }
    }
    
    private async Task PublishProductImageEvent(Outbox outboxMessage)
    {
        var productImage = JsonSerializer.Deserialize<ProductImage>(outboxMessage.JsonObject, _jsonSerializerOptions);
        var productImageData = productImage.Adapt<ProductImage, ProductImageData>();
        
        switch (outboxMessage.Operation)
        {
            case Operation.Create:
                await _eventBus.PublishAsync(new ProductImageEvent { Details = productImageData, Action = EventAction.Created});
                break;
            case Operation.Update:
                await _eventBus.PublishAsync(new ProductImageEvent { Details = productImageData, Action = EventAction.Updated});
                break;
            case Operation.Delete:
                await _eventBus.PublishAsync(new ProductImageEvent { Details = productImageData, Action = EventAction.Deleted});
                break;
            case Operation.None:
            default:
                _logger.LogWarning("Unknown operation found in EventBusOutbox - Entity Name: {ObjectType}, Operation: {Operation}", outboxMessage.ObjectType, Enum.GetName(outboxMessage.Operation));
                break;
        }
    }
    
    private async Task PublishProductStockEvent(Outbox outboxMessage)
    {
        var productStock = JsonSerializer.Deserialize<ProductStock>(outboxMessage.JsonObject, _jsonSerializerOptions);
        var productStockData = productStock.Adapt<ProductStock, ProductStockData>();
        
        switch (outboxMessage.Operation)
        {
            case Operation.Create:
                await _eventBus.PublishAsync(new ProductStockEvent { Details = productStockData, Action = EventAction.Created});
                break;
            case Operation.Update:
                await _eventBus.PublishAsync(new ProductStockEvent { Details = productStockData, Action = EventAction.Updated});
                break;
            case Operation.Delete:
                await _eventBus.PublishAsync(new ProductStockEvent { Details = productStockData, Action = EventAction.Deleted});
                break;
            case Operation.None:
            default:
                _logger.LogWarning("Unknown operation found in EventBusOutbox - Entity Name: {ObjectType}, Operation: {Operation}", outboxMessage.ObjectType, Enum.GetName(outboxMessage.Operation));
                break;
        }
    }
}
