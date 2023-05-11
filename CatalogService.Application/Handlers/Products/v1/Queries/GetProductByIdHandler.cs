using System;
using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using CatalogService.Application.Handlers.Products.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.Products.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.Products.v1.Queries;

public class GetProductByIdHandler : IRequestHandler<GetProductById, ProductData>
{
    private readonly ILogger<GetProductByIdHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;

    public GetProductByIdHandler(IRepository repository, ICache cache, ILogger<GetProductByIdHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ProductData> Handle(GetProductById request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.Id);
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<ProductData>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {Key}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetProductById(request.Id);

        if(dataValue != null)
        {
            _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);
        }
            
        return dataValue;
    }

    private async Task<ProductData> GetProductById(string id)
    {
        var entity = await _repository.GetAsSingleAsync<Product, string>(predicate: e => e.Id == id || e.Code == id,
        includeNavigationalProperties: true);
        var resultDto = entity.Adapt<Product, ProductData>();
        return resultDto;
    }

    public static string GetCacheKey(string id) => $"Product:id:{id}";
}
