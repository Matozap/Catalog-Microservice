using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using CatalogService.Application.Handlers.Products.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.Products.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.Products.v1.Queries;

public class GetAllProductsHandler : IRequestHandler<GetAllProducts, List<ProductData>>
{
    private readonly ILogger<GetAllProductsHandler> _logger;
    private readonly ICache _cache;
    private readonly IRepository _repository;

    public GetAllProductsHandler(ICache cache, ILogger<GetAllProductsHandler> logger, IRepository repository)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
    }

    public async Task<List<ProductData>> Handle(GetAllProducts request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey();

        var cachedValue = await _cache.GetCacheValueAsync<List<ProductData>>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetAllProducts();

        _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);

        return dataValue;
    }

    private async Task<List<ProductData>> GetAllProducts()
    {
        var entities = await _repository.GetAsListAsync<Product, string>(
             predicate: product => !product.Disabled, 
             orderAscending: product => product.Name, 
             selectExpression: product => new Product
                {
                    Id = product.Id,
                    Code = product.Code,
                    Name = product.Name,
                    Currency = product.Currency,
                    CurrencyName = product.CurrencyName,
                    Region = product.Region,
                    SubRegion = product.SubRegion
                });
        return entities.Adapt<List<ProductData>>();
    }
    
    public static string GetCacheKey() => "Products:All";
}

public class ProductManager
{
    private readonly ICache _cache;

    public ProductManager(ICache cache)
    {
        _cache = cache;
    }

    // public async Task<List<ProductData>> GetAllProductsAsync(CancellationToken cancellationToken)
    // {
    //     var cacheKey = "some cache key";
    //
    //     var cachedValue = await _cache.GetCacheValueAsync<List<ProductData>>(cacheKey, cancellationToken);
    //     if (cachedValue != null)
    //     {
    //         return cachedValue;
    //     }
    //
    //     var dataValue = await GetAllProductsFromRepositoryAsync();
    //
    //     var ttl = new DistributedCacheEntryOptions
    //     {
    //         AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(60),
    //         SlidingExpiration = TimeSpan.FromSeconds(30)
    //     };
    //     _ = _cache.SetCacheValueAsync(cacheKey, dataValue, ttl, cancellationToken);
    //
    //     return dataValue;
    // }
}
