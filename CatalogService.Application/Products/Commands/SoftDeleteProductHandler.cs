using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using CatalogService.Application.Common;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Products.Events;
using CatalogService.Application.Products.Requests;
using CatalogService.Application.Products.Responses;
using CatalogService.Domain;
using Distributed.Cache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Products.Commands;

public class SoftDeleteProductHandler : BuilderRequestHandler<SoftDeleteProduct, ProductData>
{
    private readonly ILogger<SoftDeleteProductHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public SoftDeleteProductHandler(ILogger<SoftDeleteProductHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductData> PreProcess(SoftDeleteProduct request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductData>(null);
    }

    protected override async Task<ProductData> Process(SoftDeleteProduct request, CancellationToken cancellationToken = default)
    {
        var entity = await DisableProduct(request.Id);
        _logger.LogInformation("Product with id {ProductID} disabled successfully", entity?.Id);

        return entity.Adapt<Product, ProductData>();
    }

    protected override async Task PostProcess(SoftDeleteProduct request, ProductData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductEvent { Details = response, Action = EventAction.Deleted });
    }
    
    private async Task ClearCache(ProductData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Product)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Product)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductCategory)}:id:{data?.ProductCategoryId}", cancellationToken);
    }

    private async Task<Product> DisableProduct(string productId)
    {
        var entity = await _repository.GetAsSingleAsync<Product, string>(c => c.Id == productId || c.Sku == productId);
        if (entity == null) return null;
        
        entity.Disabled = true;
        await _repository.UpdateAsync(entity);

        return entity;
    }
}
