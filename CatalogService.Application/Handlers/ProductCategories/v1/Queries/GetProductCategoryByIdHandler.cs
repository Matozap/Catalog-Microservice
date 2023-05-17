using System;
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

public class GetProductCategoryByIdHandler : IRequestHandler<GetProductCategoryById, ProductCategoryData>
{
    private readonly ILogger<GetProductCategoryByIdHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;

    public GetProductCategoryByIdHandler(IRepository repository, ICache cache, ILogger<GetProductCategoryByIdHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ProductCategoryData> Handle(GetProductCategoryById request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.Id);
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<ProductCategoryData>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {Key}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetProductCategoryById(request.Id);

        if(dataValue != null)
        {
            _ = _cache.SetCacheValueAsync(cacheKey, dataValue, cancellationToken);
        }
            
        return dataValue;
    }

    private async Task<ProductCategoryData> GetProductCategoryById(string id)
    {
        var entity = await _repository.GetAsSingleAsync<ProductCategory, string>(predicate: e => e.Id == id,
        includeNavigationalProperties: true);
        var resultDto = entity.Adapt<ProductCategory, ProductCategoryData>();
        return resultDto;
    }

    public static string GetCacheKey(string id) => $"ProductCategory:id:{id}";
}
