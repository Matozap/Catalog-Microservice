using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductCategories.v1;
using CatalogService.Message.Contracts.ProductCategories.v1.Requests;
using DistributedCache.Core;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductCategories.v1.Queries;

public class GetAllProductCategoriesHandler : IRequestHandler<GetAllProductCategories, List<ProductCategoryData>>
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

    public async Task<List<ProductCategoryData>> Handle(GetAllProductCategories request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey();

        var cachedValue = await _cache.GetCacheValueAsync<List<ProductCategoryData>>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetAllProductCategories();

        _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);

        return dataValue;
    }

    private async Task<List<ProductCategoryData>> GetAllProductCategories()
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
    
    public static string GetCacheKey() => "ProductCategories:All";
}