using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using CatalogService.Application.Common;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Products.Events;
using CatalogService.Application.Products.Requests;
using CatalogService.Application.Products.Responses;
using CatalogService.Domain;
using DistributedCache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Products.Commands;

public class UpdateProductHandler : BuilderRequestHandler<UpdateProduct, ProductData>
{
    private readonly ILogger<UpdateProductHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public UpdateProductHandler(ILogger<UpdateProductHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductData> PreProcess(UpdateProduct request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductData>(null);
    }

    protected override async Task<ProductData> Process(UpdateProduct request, CancellationToken cancellationToken = default)
    {
        var result = await UpdateProduct(request.Details);
        _logger.LogInformation("Product with id {ProductID} updated successfully", request.Details.Id);

        return result;
    }

    protected override async Task PostProcess(UpdateProduct request, ProductData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductEvent { Details = response, Action = EventAction.Updated });
    }
    
    private async Task ClearCache(ProductData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Product)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Product)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductCategory)}:id:{data?.ProductCategoryId}", cancellationToken);
    }

    private async Task<ProductData> UpdateProduct(ProductData productData)
    {
        var entity = await _repository.GetAsSingleAsync<Product, string>(e => e.Id == productData.Id || e.Sku == productData.Sku);
        if (entity == null) return null;
        
        var changes = productData.Adapt(entity);
        await _repository.UpdateAsync(changes);
        return changes.Adapt<Product, ProductData>();
    }
}
