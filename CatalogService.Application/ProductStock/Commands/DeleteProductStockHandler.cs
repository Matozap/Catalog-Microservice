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

public class DeleteProductStockHandler : BuilderRequestHandler<DeleteProductStock, ProductStockData>
{
    private readonly ILogger<DeleteProductStockHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public DeleteProductStockHandler(ILogger<DeleteProductStockHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductStockData> PreProcess(DeleteProductStock request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductStockData>(null);
    }

    protected override async Task<ProductStockData> Process(DeleteProductStock request, CancellationToken cancellationToken = default)
    {
        var entity = await DeleteProductStockAsync(request.Id);
        _logger.LogInformation("ProductStock with id {ProductStockId} deleted successfully", entity?.Id);
        
        return entity?.Adapt<Domain.ProductStock, ProductStockData>();
    }

    protected override async Task PostProcess(DeleteProductStock request, ProductStockData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductStockEvent { Details = response, Action = EventAction.Deleted });
    }
    
    private async Task ClearCache(ProductStockData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.ProductStock)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.ProductStock)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.Product)}:id:{data?.ProductId}", cancellationToken);
    }

    private async Task<Domain.ProductStock> DeleteProductStockAsync(string productStockId)
    {
        var entity = await _repository.GetAsSingleAsync<Domain.ProductStock, string>(productStock => productStock.Id == productStockId);
        if (entity == null) return null;
        
        await _repository.DeleteAsync(entity);
        return entity;
    }
}
