using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using CatalogService.Application.Common;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductCategories.Events;
using CatalogService.Application.ProductCategories.Requests;
using CatalogService.Application.ProductCategories.Responses;
using CatalogService.Domain;
using DistributedCache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.ProductCategories.Commands;

public class UpdateProductCategoryHandler : BuilderRequestHandler<UpdateProductCategory, ProductCategoryData>
{
    private readonly ILogger<UpdateProductCategoryHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public UpdateProductCategoryHandler(ILogger<UpdateProductCategoryHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductCategoryData> PreProcess(UpdateProductCategory request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductCategoryData>(null);
    }

    protected override async Task<ProductCategoryData> Process(UpdateProductCategory request, CancellationToken cancellationToken = default)
    {
        var result = await UpdateProductCategory(request.Details);
        _logger.LogInformation("ProductCategory with id {ProductCategoryID} updated successfully", request.Details.Id);
            
        return result;
    }

    protected override async Task PostProcess(UpdateProductCategory request, ProductCategoryData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductCategoryEvent { Details = response, Action = EventAction.Updated });
    }
    
    private async Task ClearCache(ProductCategoryData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductCategory)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductCategory)}:id:{data?.Id}", cancellationToken);
    }

    private async Task<ProductCategoryData> UpdateProductCategory(ProductCategoryData productData)
    {
        var entity = await _repository.GetAsSingleAsync<ProductCategory, string>(e => e.Id == productData.Id);
        if (entity == null) return null;
        
        var changes = productData.Adapt(entity);
        await _repository.UpdateAsync(changes);
        return changes.Adapt<ProductCategory, ProductCategoryData>();
    }
}
