using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductCategories.Requests;
using CatalogService.Application.ProductCategories.Responses;
using CatalogService.Domain;
using DistributedCache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.ProductCategories.Queries;

public class GetProductCategoryByIdHandler : BuilderRequestHandler<GetProductCategoryById, ProductCategoryData>
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
    
    protected override async Task<ProductCategoryData> PreProcess(GetProductCategoryById request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<ProductCategoryData>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {Key}", cacheKey);
        return cachedValue;
    }
    
    protected override async Task<ProductCategoryData> Process(GetProductCategoryById request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsSingleAsync<ProductCategory, string>(
            predicate: e => e.Id == request.Id,
            includeNavigationalProperties: true);
        
        return entity.Adapt<ProductCategory, ProductCategoryData>();
    }

    protected override Task PostProcess(GetProductCategoryById request, ProductCategoryData response, CancellationToken cancellationToken = default)
    {
        if(response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(request.Id), response, cancellationToken);
        }
        return Task.CompletedTask;
    }

    private static string GetCacheKey(string id) => $"{nameof(ProductCategory)}:id:{id}";
}
