using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductStock.Requests;
using CatalogService.Application.ProductStock.Responses;
using Distributed.Cache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.ProductStock.Queries;

public class GetProductStockByIdHandler : BuilderRequestHandler<GetProductStockById, ProductStockData>
{
    private readonly ILogger<GetProductStockByIdHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;

    public GetProductStockByIdHandler(IRepository repository, ICache cache, ILogger<GetProductStockByIdHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }
    
    protected override async Task<ProductStockData> PreProcess(GetProductStockById request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<ProductStockData>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
        return cachedValue;
    }
    
    protected override async Task<ProductStockData> Process(GetProductStockById request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsSingleAsync<Domain.ProductStock, string>(
            predicate: productStock => productStock.Id == request.Id,
            orderDescending: productStock => productStock.Id,
            includeNavigationalProperties: true);
        var resultDto = entity.Adapt<Domain.ProductStock, ProductStockData>();
        return resultDto;
    }

    protected override Task PostProcess(GetProductStockById request, ProductStockData response, CancellationToken cancellationToken = default)
    {
        if (response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(request.Id), response, cancellationToken);
        }

        return Task.CompletedTask;
    }
    
    private static string GetCacheKey(string id) => $"{nameof(Domain.ProductStock)}:id:{id}";
}
