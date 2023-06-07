using System.Linq;
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

public class DeleteProductCategoryHandler : BuilderRequestHandler<DeleteProductCategory, ProductCategoryData>
{
    private readonly ILogger<DeleteProductCategoryHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public DeleteProductCategoryHandler(ILogger<DeleteProductCategoryHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductCategoryData> PreProcess(DeleteProductCategory request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductCategoryData>(null);
    }

    protected override async Task<ProductCategoryData> Process(DeleteProductCategory request, CancellationToken cancellationToken = default)
    {
        var entity = await DeleteProductCategoryAsync(request.Id);
        _logger.LogInformation("ProductCategory with id {ProductCategoryID} deleted successfully", entity?.Id);

        return entity.Adapt<ProductCategory, ProductCategoryData>();
    }

    protected override async Task PostProcess(DeleteProductCategory request, ProductCategoryData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductCategoryEvent { Details = response, Action = EventAction.Deleted });
    }
    
    private async Task ClearCache(ProductCategoryData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductCategory)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductCategory)}:id:{data?.Id}", cancellationToken);
    }

    private async Task<ProductCategory> DeleteProductCategoryAsync(string id)
    {
        var entity = await _repository.GetAsSingleAsync<ProductCategory, string>(c => c.Id == id, includeNavigationalProperties: true);

        if (entity == null) return null;
        
        if (entity.Products != null)
        {
            foreach (var product in entity.Products.ToList())
            {
                if (product.ProductImages?.Count > 0)
                {
                    foreach (var productImage in product.ProductImages.ToList())
                    {
                        await _repository.DeleteAsync(productImage);
                    }
                }
                
                if (product.ProductStocks?.Count > 0)
                {
                    foreach (var productStock in product.ProductStocks.ToList())
                    {
                        await _repository.DeleteAsync(productStock);
                    }
                }
                
                await _repository.DeleteAsync(product);
            }
        }
            
        await _repository.DeleteAsync(entity);

        return entity;
    }
}
