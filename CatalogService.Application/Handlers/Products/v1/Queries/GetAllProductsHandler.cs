using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.Products.v1;
using CatalogService.Message.Contracts.Products.v1.Requests;
using Mapster;
using MediatR;
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
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<List<ProductData>>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetAllProducts(request.Id);

        _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);

        return dataValue;
    }

    private async Task<List<ProductData>> GetAllProducts(string id)
    {
        var entities = await _repository.GetAsListAsync<Product, string>(
             predicate: product => !product.Disabled && (string.IsNullOrEmpty(id) || product.ProductCategoryId == id), 
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
    
    public static string GetCacheKey(string categoryId)
    {
        var id = string.IsNullOrEmpty(categoryId) ? "All" : categoryId;
        return $"Products:All:{id}";
    }
}