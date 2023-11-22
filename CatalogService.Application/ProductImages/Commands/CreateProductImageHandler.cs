using System;
using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using CatalogService.Application.Common;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductImages.Events;
using CatalogService.Application.ProductImages.Requests;
using CatalogService.Application.ProductImages.Responses;
using CatalogService.Domain;
using Distributed.Cache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.ProductImages.Commands;

public class CreateProductImageHandler : BuilderRequestHandler<CreateProductImage, ProductImageData>
{
    private readonly ILogger<CreateProductImageHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public CreateProductImageHandler(ILogger<CreateProductImageHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductImageData> PreProcess(CreateProductImage request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductImageData>(null);
    }

    protected override async Task<ProductImageData> Process(CreateProductImage request, CancellationToken cancellationToken = default)
    {
        if (await _repository.GetAsSingleAsync<ProductImage, string>(e => e.Title == request.Details.Title && e.ProductId == request.Details.ProductId) != null)
        {
            return null;
        }
        
        var entity = request.Details.Adapt<ProductImageData, ProductImage>();
        
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        var resultEntity = await _repository.AddAsync(entity);
        if (resultEntity == null) return null;
        
        _logger.LogInformation("Product Image with id {ProductImageID} created successfully", resultEntity.Id);
        var resultDto = resultEntity.Adapt<ProductImage, ProductImageData>();

        return resultDto;
    }

    protected override async Task PostProcess(CreateProductImage request, ProductImageData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductImageEvent { Details = response, Action = EventAction.Created });
    }
    
    private async Task ClearCache(ProductImageData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductImage)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(ProductImage)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Product)}:id:{data?.ProductId}", cancellationToken);
    }
}
