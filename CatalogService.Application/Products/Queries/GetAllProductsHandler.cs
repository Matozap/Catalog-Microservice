using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.Products.Requests;
using CatalogService.Application.Products.Responses;
using CatalogService.Domain;
using Distributed.Cache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Products.Queries;

public class GetAllProductsHandler : BuilderRequestHandler<GetAllProducts, List<ProductData>>
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
    
    protected override async Task<List<ProductData>> PreProcess(GetAllProducts request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<List<ProductData>>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
        return cachedValue;
    }
    
    protected override async Task<List<ProductData>> Process(GetAllProducts request, CancellationToken cancellationToken = default)
    {
        var entities = await _repository.GetAsListAsync<Product, string>(
            predicate: product => !product.Disabled && (string.IsNullOrEmpty(request.Id) || product.ProductCategoryId == request.Id), 
            orderAscending: product => product.Name, 
            selectExpression: product => new Product
            {
                Id = product.Id,
                Sku = product.Sku,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                ProductCategoryId = product.ProductCategoryId,
                Brand = product.Brand,
                Dimensions = product.Dimensions,
                Weight = product.Weight
            });
        return entities.Adapt<List<ProductData>>();
    }

    protected override Task PostProcess(GetAllProducts request, List<ProductData> response, CancellationToken cancellationToken = default)
    {
        if (response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(request.Id), response, cancellationToken);
        }

        return Task.CompletedTask;
    }
    
    private static string GetCacheKey(string id) => $"{nameof(Product)}:list:{id}";
}