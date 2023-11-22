using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductImages.Requests;
using CatalogService.Application.ProductImages.Responses;
using CatalogService.Domain;
using Distributed.Cache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.ProductImages.Queries;

public class GetProductImageByIdHandler : BuilderRequestHandler<GetProductImageById, ProductImageData>
{
    private readonly ILogger<GetProductImageByIdHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;

    public GetProductImageByIdHandler(IRepository repository, ICache cache, ILogger<GetProductImageByIdHandler> logger)
    {
        _repository = repository;
        _cache = cache;
        _logger = logger;
    }
    
    protected override async Task<ProductImageData> PreProcess(GetProductImageById request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<ProductImageData>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
        return cachedValue;
    }
    
    protected override async Task<ProductImageData> Process(GetProductImageById request, CancellationToken cancellationToken = default)
    {
        var entity = await _repository.GetAsSingleAsync<ProductImage, string>(
            predicate: productImage => (productImage.Id == request.Id || productImage.Title == request.Id) && !productImage.Disabled,
            orderAscending: productImage => productImage.Url,
            includeNavigationalProperties: true);
        
        return entity.Adapt<ProductImage, ProductImageData>();
    }

    protected override Task PostProcess(GetProductImageById request, ProductImageData response, CancellationToken cancellationToken = default)
    {
        if (response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(request.Id), response, cancellationToken);
        }

        return Task.CompletedTask;
    }
    
    private static string GetCacheKey(string id) => $"{nameof(ProductImage)}:id:{id}";
}
