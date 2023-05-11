using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using CatalogService.Application.Handlers.ProductImages.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductImages.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductImages.v1.Queries;

public class GetAllProductImagesHandler : IRequestHandler<GetAllProductImages, object>
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

    public async Task<object> Handle(GetAllProductImages request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(request.ProductId);

        var cachedValue = await _cache.GetCacheValueAsync<List<ProductImageData>>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {CacheKey}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetAllProductImages(request.ProductId);

        _ = _cache.SetCacheValueAsync(cacheKey, dataValue, null, cancellationToken);

        return dataValue;
    }

    private async Task<List<ProductImageData>> GetAllProductImages(string productId)
    {
        var parentByCode = await _repository.GetAsSingleAsync<Product, string>(predicate: e => e.Code == productId) ?? new Product();
        var entities = await _repository.GetAsListAsync<ProductImage, string>(
            predicate: productImage => (productImage.ProductId == productId || productImage.ProductId == parentByCode.Id) && !productImage.Disabled,
            orderAscending: productImage => productImage.Name,
            includeNavigationalProperties: true
        );
        return entities.Adapt<List<ProductImageData>>();
    }
    
    public static string GetCacheKey(string id) => $"ProductImages:All:{id}";
}
