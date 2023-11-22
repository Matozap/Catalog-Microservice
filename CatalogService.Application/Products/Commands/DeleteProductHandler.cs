using System.Linq;
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

public class DeleteProductHandler : BuilderRequestHandler<DeleteProduct, ProductData>
{
    private readonly ILogger<DeleteProductHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public DeleteProductHandler(ILogger<DeleteProductHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductData> PreProcess(DeleteProduct request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductData>(null);
    }

    protected override async Task<ProductData> Process(DeleteProduct request, CancellationToken cancellationToken = default)
    {
        var entity = await DeleteProductAsync(request.Id);
        _logger.LogInformation("Product with id {ProductID} deleted successfully", entity?.Id);

        return entity.Adapt<Product, ProductData>();
    }

    protected override async Task PostProcess(DeleteProduct request, ProductData response, CancellationToken cancellationToken = default)
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

    private async Task<Product> DeleteProductAsync(string id)
    {
        var entity = await _repository.GetAsSingleAsync<Product, string>(c => c.Id == id || c.Sku == id);

        if (entity == null) return null;
        
        if (entity.ProductImages?.Count > 0)
        {
            foreach (var productImage in entity.ProductImages.ToList())
            {
                await _repository.DeleteAsync(productImage);
            }
        }
                
        if (entity.ProductStocks?.Count > 0)
        {
            foreach (var productStock in entity.ProductStocks.ToList())
            {
                await _repository.DeleteAsync(productStock);
            }
        }
        
        await _repository.DeleteAsync(entity);

        return entity;
    }
}
