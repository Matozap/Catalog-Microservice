using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using CatalogService.Application.Handlers.ProductStock.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductStock.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductStock.v1.Queries;

public class GetAllProductStockHandler : IRequestHandler<GetAllProductStock, List<ProductStockData>>
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

    public async Task<List<ProductStockData>> Handle(GetAllProductStock request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(request.ProductImageId);

        var cachedValue = await _cache.GetCacheValueAsync<List<ProductStockData>>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetAllProductStock(request.ProductImageId);

        _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);

        return dataValue;
    }

    private async Task<List<ProductStockData>> GetAllProductStock(string productImageId)
    {
        var parentByCode = await _repository.GetAsSingleAsync<ProductImage,string>(productStockItem => productStockItem.Id == productImageId || productStockItem.Title == productImageId) ?? new ProductImage();
        var entities = await _repository.GetAsListAsync<Domain.ProductStock,string>(
            predicate: productStock => (productStock.ProductId == productImageId || productStock.ProductId == parentByCode.Id) && !productStock.Disabled,
            orderDescending: productStock => productStock.Id,
            includeNavigationalProperties: true);
        
        return entities.Adapt<List<ProductStockData>>();
    }
    
    public static string GetCacheKey(string id) => $"ProductStock:{id}";
}
