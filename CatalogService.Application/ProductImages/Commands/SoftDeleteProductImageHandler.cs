using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using CatalogService.Application.Common;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductImages.Events;
using CatalogService.Application.ProductImages.Requests;
using CatalogService.Application.ProductImages.Responses;
using CatalogService.Domain;
using DistributedCache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.ProductImages.Commands;

public class SoftDeleteProductImageHandler : BuilderRequestHandler<SoftDeleteProductImage, ProductImageData>
{
    private readonly ILogger<SoftDeleteProductImageHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public SoftDeleteProductImageHandler(ILogger<SoftDeleteProductImageHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductImageData> PreProcess(SoftDeleteProductImage request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductImageData>(null);
    }

    protected override async Task<ProductImageData> Process(SoftDeleteProductImage request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsSingleAsync<ProductImage, string>(c => c.Id == request.Id);
        if (entity == null) return null;
        
        entity.Disabled = true;
        await _repository.UpdateAsync(entity);
        _logger.LogInformation("ProductImage with id {ProductImageId} disabled successfully", entity?.Id);
        
        return entity.Adapt<ProductImage, ProductImageData>();
    }

    protected override async Task PostProcess(SoftDeleteProductImage request, ProductImageData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductImageEvent { Details = response, Action = EventAction.Deleted });
    }
    
    private async Task ClearCache(ProductImageData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductImage)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductImage)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Product)}:id:{data?.ProductId}", cancellationToken);
    }
}
