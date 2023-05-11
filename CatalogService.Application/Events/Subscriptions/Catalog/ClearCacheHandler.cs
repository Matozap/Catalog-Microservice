using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using CatalogService.Application.Handlers.ProductStock.v1.Queries;
using CatalogService.Application.Handlers.Products.v1.Queries;
using CatalogService.Application.Handlers.ProductImages.v1.Queries;
using CatalogService.Message.Events.Cache;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Events.Subscriptions.Catalog;

[ExcludeFromCodeCoverage]
public class ClearCacheHandler : IRequestHandler<ClearCache, bool>
{
    private readonly ILogger<ClearCacheHandler> _logger;
    private readonly ICache _cache;

    public ClearCacheHandler(ILogger<ClearCacheHandler> logger, ICache cache)
    {
        _logger = logger;
        _cache = cache;
    }

    public async Task<bool> Handle(ClearCache request, CancellationToken cancellationToken)
    {            
        if(request.ClearAll)
        {
            await _cache.ClearCacheAsync(cancellationToken);
        }
        else
        {
            
            if (!string.IsNullOrEmpty(request.ProductId))
            {
                const string message = "Clearing catalog by product cache";
                _logger.LogDebug(message);
                var cacheKey = GetProductByIdHandler.GetCacheKey(request.ProductId);
                await _cache.RemoveValueAsync(cacheKey, cancellationToken);
                
                cacheKey = GetAllProductsHandler.GetCacheKey();
                await _cache.RemoveValueAsync(cacheKey, cancellationToken);
            }
            
            if (!string.IsNullOrEmpty(request.ProductImageCode) || !string.IsNullOrEmpty(request.ProductImageId))
            {
                const string message = "Clearing catalog by productImage cache";
                _logger.LogDebug(message);
                var cacheKey = GetProductImageByIdHandler.GetCacheKey(request.ProductImageId);
                await _cache.RemoveValueAsync(cacheKey, cancellationToken);
                
                cacheKey = GetAllProductImagesHandler.GetCacheKey(request.ProductId);
                await _cache.RemoveValueAsync(cacheKey, cancellationToken);
            }
            
            if (!string.IsNullOrEmpty(request.ProductStockId))
            {
                const string message = "Clearing catalog by productStock cache";
                _logger.LogDebug(message);
                var cacheKey = GetProductStockByIdHandler.GetCacheKey(request.ProductStockId);
                await _cache.RemoveValueAsync(cacheKey, cancellationToken);
                
                cacheKey = GetAllProductStockHandler.GetCacheKey(request.ProductImageId);
                await _cache.RemoveValueAsync(cacheKey, cancellationToken);
            }
        }

        return true;
    }
}
