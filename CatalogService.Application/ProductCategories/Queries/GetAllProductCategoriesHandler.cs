using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductCategories.Requests;
using CatalogService.Application.ProductCategories.Responses;
using CatalogService.Domain;
using DistributedCache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.ProductCategories.Queries;

public class GetAllProductCategoriesHandler : BuilderRequestHandler<GetAllProductCategories, List<ProductCategoryData>>
{
    private readonly ILogger<GetAllProductCategoriesHandler> _logger;
    private readonly ICache _cache;
    private readonly IRepository _repository;

    public GetAllProductCategoriesHandler(ICache cache, ILogger<GetAllProductCategoriesHandler> logger, IRepository repository)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
    }
    
    protected override async Task<List<ProductCategoryData>> PreProcess(GetAllProductCategories request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey();

        var cachedValue = await _cache.GetCacheValueAsync<List<ProductCategoryData>>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
        return cachedValue;
    }
    
    protected override async Task<List<ProductCategoryData>> Process(GetAllProductCategories request, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAsListAsync<ProductCategory, string>(
            predicate: product => !product.Disabled, 
            orderAscending: product => product.Name, 
            selectExpression: product => new ProductCategory
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description
            });
        
        return entities.Adapt<List<ProductCategoryData>>();
    }

    protected override Task PostProcess(GetAllProductCategories request, List<ProductCategoryData> response, CancellationToken cancellationToken = default)
    {
        if (response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(), response, cancellationToken);
        }

        return Task.CompletedTask;
    }
    
    private static string GetCacheKey() => $"{nameof(ProductCategory)}:list";
}