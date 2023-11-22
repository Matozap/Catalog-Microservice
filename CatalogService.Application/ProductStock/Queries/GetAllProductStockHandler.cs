using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductStock.Requests;
using CatalogService.Application.ProductStock.Responses;
using CatalogService.Domain;
using Distributed.Cache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.ProductStock.Queries;

public class GetAllProductStockHandler : BuilderRequestHandler<GetAllProductStock, List<ProductStockData>>
{
    private readonly ILogger<GetAllProductStockHandler> _logger;
    private readonly ICache _cache;
    private readonly IRepository _repository;

    public GetAllProductStockHandler(ICache cache, ILogger<GetAllProductStockHandler> logger, IRepository repository)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
    }
    
    protected override async Task<List<ProductStockData>> PreProcess(GetAllProductStock request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(request.ProductId);

        var cachedValue = await _cache.GetCacheValueAsync<List<ProductStockData>>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
        return cachedValue;
    }
    
    protected override async Task<List<ProductStockData>> Process(GetAllProductStock request, CancellationToken cancellationToken = default)
    {
        var parentByCode = await _repository.GetAsSingleAsync<Product,string>(productItem => productItem.Id == request.ProductId || productItem.Sku == request.ProductId) ?? new Product();
        var entities = await _repository.GetAsListAsync<Domain.ProductStock,string>(
            predicate: productStock => productStock.ProductId == request.ProductId || productStock.ProductId == parentByCode.Id,
            orderDescending: productStock => productStock.Id,
            includeNavigationalProperties: true);
        
        return entities.Adapt<List<ProductStockData>>();
    }

    protected override Task PostProcess(GetAllProductStock request, List<ProductStockData> response, CancellationToken cancellationToken = default)
    {
        if (response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(request.ProductId), response, cancellationToken);
        }

        return Task.CompletedTask;
    }
    
    private static string GetCacheKey(string id) => $"{nameof(Domain.ProductStock)}:list:{id}";
}
