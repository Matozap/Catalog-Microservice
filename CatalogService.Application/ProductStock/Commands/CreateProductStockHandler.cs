using System;
using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using CatalogService.Application.Common;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductStock.Events;
using CatalogService.Application.ProductStock.Requests;
using CatalogService.Application.ProductStock.Responses;
using DistributedCache.Core;
using Mapster;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.ProductStock.Commands;

public class CreateProductStockHandler : BuilderRequestHandler<CreateProductStock, ProductStockData>
{
    private readonly ILogger<CreateProductStockHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public CreateProductStockHandler(ILogger<CreateProductStockHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductStockData> PreProcess(CreateProductStock request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductStockData>(null);
    }

    protected override async Task<ProductStockData> Process(CreateProductStock request, CancellationToken cancellationToken = default)
    {
        var resultEntity = await CreateProductStock(request.Details);
        if (resultEntity == null) return null;
        
        _logger.LogInformation("Product Stock with id {ProductStockID} created successfully", resultEntity.Id);
        var resultDto = resultEntity.Adapt<Domain.ProductStock, ProductStockData>();

        return resultDto;
    }

    protected override async Task PostProcess(CreateProductStock request, ProductStockData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductStockEvent { Details = response, Action = EventAction.Created });
    }
    
    private async Task ClearCache(ProductStockData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.ProductStock)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.ProductStock)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.Product)}:id:{data?.ProductId}", cancellationToken);
    }
    
    private async Task<Domain.ProductStock> CreateProductStock(ProductStockData productStock)
    {
        if (await _repository.GetAsSingleAsync<Domain.ProductStock, string>(e => e.Current == productStock.Current && e.ProductId == productStock.ProductId) != null)
        {
            return null;
        }
        
        var entity = productStock.Adapt<ProductStockData, Domain.ProductStock>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
