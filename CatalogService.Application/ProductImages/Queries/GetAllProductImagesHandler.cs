using System.Collections.Generic;
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

public class GetAllProductImagesHandler : BuilderRequestHandler<GetAllProductImages, List<ProductImageData>>
{
    private readonly ILogger<GetAllProductImagesHandler> _logger;
    private readonly ICache _cache;
    private readonly IRepository _repository;

    public GetAllProductImagesHandler(ICache cache, ILogger<GetAllProductImagesHandler> logger, IRepository repository)
    {
        _logger = logger;
        _cache = cache;
        _repository = repository;
    }
    
    protected override async Task<List<ProductImageData>> PreProcess(GetAllProductImages request, CancellationToken cancellationToken = default)
    {
        var cacheKey = GetCacheKey(request.ProductId);

        var cachedValue = await _cache.GetCacheValueAsync<List<ProductImageData>>(cacheKey, cancellationToken);
        if (cachedValue == null) return null;
        
        _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
        return cachedValue;
    }
    
    protected override async Task<List<ProductImageData>> Process(GetAllProductImages request, CancellationToken cancellationToken = default)
    {
        var parentByCode = await _repository.GetAsSingleAsync<Product, string>(predicate: e => e.Id == request.ProductId || e.Sku == request.ProductId) ?? new Product();
        var entities = await _repository.GetAsListAsync<ProductImage, string>(
            predicate: productImage => (productImage.ProductId == request.ProductId || productImage.ProductId == parentByCode.Id) && !productImage.Disabled,
            orderAscending: productImage => productImage.Url,
            includeNavigationalProperties: true
        );
        return entities.Adapt<List<ProductImageData>>();
    }

    protected override Task PostProcess(GetAllProductImages request, List<ProductImageData> response, CancellationToken cancellationToken = default)
    {
        if (response != null)
        {
            _ = _cache.SetCacheValueAsync(GetCacheKey(request.ProductId), response, cancellationToken);
        }

        return Task.CompletedTask;
    }
    
    private static string GetCacheKey(string id) => $"{nameof(ProductImage)}:list:{id}";
    
}
