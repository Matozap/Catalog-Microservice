using System;
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

public class CreateProductHandler : BuilderRequestHandler<CreateProduct, ProductData>
{
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public CreateProductHandler(ILogger<CreateProductHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductData> PreProcess(CreateProduct request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductData>(null);
    }

    protected override async Task<ProductData> Process(CreateProduct request, CancellationToken cancellationToken = default)
    {
        var entity = await CreateProduct(request.Details);
        if (entity == null) return null;
            
        _logger.LogInformation("Product with id {ProductID} created successfully", entity.Id);
        return entity.Adapt<Product, ProductData>();
    }

    protected override async Task PostProcess(CreateProduct request, ProductData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductEvent { Details = response, Action = EventAction.Created });
    }
    
    private async Task ClearCache(ProductData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Product)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Product)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductCategory)}:id:{data?.ProductCategoryId}", cancellationToken);
    }

    private async Task<Product> CreateProduct(ProductData product)
    {
        if (await _repository.GetAsSingleAsync<Product,string>(e => e.Sku == product.Sku || e.Name == product.Name) != null)
        {
            return null;
        }
        
        var entity = product.Adapt<ProductData, Product>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
