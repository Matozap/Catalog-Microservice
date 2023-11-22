using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using CatalogService.Application.Common;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductCategories.Events;
using CatalogService.Application.ProductCategories.Requests;
using CatalogService.Application.ProductCategories.Responses;
using CatalogService.Domain;
using Distributed.Cache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.ProductCategories.Commands;

public class SoftDeleteProductCategoryHandler : BuilderRequestHandler<SoftDeleteProductCategory, ProductCategoryData>
{
    private readonly ILogger<SoftDeleteProductCategoryHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public SoftDeleteProductCategoryHandler(ILogger<SoftDeleteProductCategoryHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductCategoryData> PreProcess(SoftDeleteProductCategory request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductCategoryData>(null);
    }

    protected override async Task<ProductCategoryData> Process(SoftDeleteProductCategory request, CancellationToken cancellationToken = default)
    {
        var entity = await DisableProductCategory(request.Id);
        _logger.LogInformation("ProductCategory with id {ProductCategoryID} disabled successfully", entity?.Id);

        return entity.Adapt<ProductCategory, ProductCategoryData>();
    }

    protected override async Task PostProcess(SoftDeleteProductCategory request, ProductCategoryData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductCategoryEvent { Details = response, Action = EventAction.Deleted });
    }
    
    private async Task ClearCache(ProductCategoryData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductCategory)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductCategory)}:id:{data?.Id}", cancellationToken);
    }

    private async Task<ProductCategory> DisableProductCategory(string productId)
    {
        var entity = await _repository.GetAsSingleAsync<ProductCategory, string>(c => c.Id == productId);
        if (entity == null) return null;
        
        entity.Disabled = true;
        await _repository.UpdateAsync(entity);

        return entity;
    }
}
