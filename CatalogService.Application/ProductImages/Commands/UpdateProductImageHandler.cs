using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using CatalogService.Application.Common;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductImages.Events;
using CatalogService.Application.ProductImages.Requests;
using CatalogService.Application.ProductImages.Responses;
using CatalogService.Domain;
using Distributed.Cache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.ProductImages.Commands;

public class UpdateProductImageHandler : BuilderRequestHandler<UpdateProductImage, ProductImageData>
{
    private readonly ILogger<UpdateProductImageHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public UpdateProductImageHandler(ILogger<UpdateProductImageHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductImageData> PreProcess(UpdateProductImage request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductImageData>(null);
    }

    protected override async Task<ProductImageData> Process(UpdateProductImage request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsSingleAsync<ProductImage, string>(productImage => productImage.Id == request.Details.Id);
        if (entity == null) return null;

        request.Details.ProductId = entity.ProductId;
        var changes = request.Details.Adapt(entity);
        
        await _repository.UpdateAsync(changes);
        
        _logger.LogInformation("Product Image with id {ProductImageID} updated successfully", request.Details.Id);
        
        return changes.Adapt<ProductImage, ProductImageData>();
    }

    protected override async Task PostProcess(UpdateProductImage request, ProductImageData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductImageEvent { Details = response, Action = EventAction.Updated });
    }
    
    private async Task ClearCache(ProductImageData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductImage)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductImage)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Product)}:id:{data?.ProductId}", cancellationToken);
    }
}
