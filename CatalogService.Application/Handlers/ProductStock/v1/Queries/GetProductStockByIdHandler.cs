using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using CatalogService.Application.Handlers.ProductStock.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductStock.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductStock.v1.Queries;

public class GetProductStockByIdHandler : IRequestHandler<GetProductStockById, ProductStockData>
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

    public async Task<ProductStockData> Handle(GetProductStockById request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<ProductStockData>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {Key}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetProductImageById(request.Id);

        if(dataValue != null)
        {
            _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);
        }
            
        return dataValue;
    }

    private async Task<ProductStockData> GetProductImageById(string id)
    {
        var entity = await _repository.GetAsSingleAsync<Domain.ProductStock, string>(
            predicate: productStock => productStock.Id == id && !productStock.Disabled,
            orderDescending: productStock => productStock.Id,
            includeNavigationalProperties: true);
        var resultDto = entity.Adapt<Domain.ProductStock, ProductStockData>();
        return resultDto;
    }

    public static string GetCacheKey(string id) => $"ProductStock:id:{id}";
}
