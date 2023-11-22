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

public class GetProductByIdHandler : BuilderRequestHandler<GetProductById, ProductData>
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
    
    protected override async Task<ProductData> PreProcess(GetProductById request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<ProductData>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
        return cachedValue;
    }
    
    protected override async Task<ProductData> Process(GetProductById request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsSingleAsync<Product, string>(predicate: e => e.Id == request.Id || e.Sku == request.Id,
            includeNavigationalProperties: true);
        return entity.Adapt<Product, ProductData>();
    }

    protected override Task PostProcess(GetProductById request, ProductData response, CancellationToken cancellationToken = default)
    {
        if (response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(request.Id), response, cancellationToken);
        }

        return Task.CompletedTask;
    }

    private static string GetCacheKey(string id) => $"{nameof(Product)}:id:{id}";
}
