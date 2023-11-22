using System;
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

public class CreateProductCategoryHandler : BuilderRequestHandler<CreateProductCategory, ProductCategoryData>
{
    private readonly ILogger<CreateProductCategoryHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public CreateProductCategoryHandler(ILogger<CreateProductCategoryHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }

    protected override Task<ProductCategoryData> PreProcess(CreateProductCategory request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductCategoryData>(null);
    }

    protected override async Task<ProductCategoryData> Process(CreateProductCategory request, CancellationToken cancellationToken = default)
    {
        var resultEntity = await CreateProductCategory(request.Details);
        if (resultEntity == null) return null;
            
        _logger.LogInformation("ProductCategory with id {ProductCategoryID} created successfully", resultEntity.Id);
        return resultEntity.Adapt<ProductCategory, ProductCategoryData>();
    }

    protected override async Task PostProcess(CreateProductCategory request, ProductCategoryData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductCategoryEvent { Details = response, Action = EventAction.Created });
    }
    
    private async Task ClearCache(ProductCategoryData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductCategory)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductCategory)}:id:{data?.Id}", cancellationToken);
    }

    private async Task<ProductCategory> CreateProductCategory(ProductCategoryData product)
    {
        if (await _repository.GetAsSingleAsync<ProductCategory,string>(e => e.Name == product.Name) != null)
        {
            return null;
        }
        
        var entity = product.Adapt<ProductCategoryData, ProductCategory>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
