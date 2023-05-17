using System.Threading;
using System.Threading.Tasks;
using DistributedCache.Core;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductImages.v1;
using CatalogService.Message.Contracts.ProductImages.v1.Requests;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductImages.v1.Queries;

public class GetProductImageByIdHandler : IRequestHandler<GetProductImageById, object>
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

    public async Task<object> Handle(GetProductImageById request, CancellationToken cancellationToken)
    {
        var cacheKey = GetCacheKey(request.Id);

        var cachedValue = await _cache.GetCacheValueAsync<ProductImageData>(cacheKey, cancellationToken);
        if (cachedValue != null)
        {
            _logger.LogInformation("Cache value found for {Key}", cacheKey);
            return cachedValue;
        }

        var dataValue = await GetProductImageById(request.Id);

        if(dataValue != null)
        {
            _ = _cache.SetCacheValueAsync(cacheKey, dataValue, null, cancellationToken);
        }
            
        return dataValue;
    }

    private async Task<ProductImageData> GetProductImageById(string id)
    {
        var entity = await _repository.GetAsSingleAsync<ProductImage, string>(
            predicate: productImage => (productImage.Id == id || productImage.Title == id) && !productImage.Disabled,
            orderAscending: productImage => productImage.Url,
            includeNavigationalProperties: true);
        var resultDto = entity.Adapt<ProductImage, ProductImageData>();
        return resultDto;
    }

    public static string GetCacheKey(string id) => $"ProductImage:id:{id}";
}
