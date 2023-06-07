using System.Threading;
using System.Threading.Tasks;
using Bustr.Bus;
using CatalogService.Application.Common;
using CatalogService.Application.Common.Interfaces;
using CatalogService.Application.ProductStock.Events;
using CatalogService.Application.ProductStock.Requests;
using CatalogService.Application.ProductStock.Responses;
using DistributedCache.Core;
using MediatrBuilder;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.ProductStock.Commands;

public class ReleaseProductStockHandler : BuilderRequestHandler<ReleaseProductStock, ReleaseProductStockResponse>
{
    private readonly ILogger<ReleaseProductStockHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public ReleaseProductStockHandler(ILogger<ReleaseProductStockHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ReleaseProductStockResponse> PreProcess(ReleaseProductStock request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ReleaseProductStockResponse>(null);
    }

    protected override async Task<ReleaseProductStockResponse> Process(ReleaseProductStock request, CancellationToken cancellationToken = default)
    {
        var result = await UpdateProductStock(request);
        _logger.LogInformation("ProductStock release for product with id {ProductStockID} completed with status {Status}", request.ProductId, result.StatusMessage);
            
        return result;
    }

    protected override async Task PostProcess(ReleaseProductStock request, ReleaseProductStockResponse response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductStockReleaseEvent { Details = response, Action = EventAction.Created });
    }
    
    private async Task ClearCache(ReleaseProductStockResponse data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.ProductStock)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.ProductStock)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.Product)}:id:{data?.ProductId}", cancellationToken);
    }

    private async Task<ReleaseProductStockResponse> UpdateProductStock(ReleaseProductStock request)
    {
        var stock = new ReleaseProductStockResponse
        {
            ProductId = request.ProductId,
            Value = 0,
            Success = false
        };
        
        var entity = await _repository.GetAsSingleAsync<Domain.ProductStock, string>(productStock => productStock.ProductId == request.ProductId, orderDescending: stock => stock.Id);
        if (entity == null)
        {
            stock.StatusMessage = $"There is no booked stock for product {request.ProductId} to be released";
            return stock;
        }

        if (entity.Booked - request.Value < 0)
        {
            stock.StatusMessage = $"Not enough booked stock to release {request.Value} items of product {request.ProductId} - Current booked stock is {entity.Current}";
            return stock;
        }
        var stockCreate = new Domain.ProductStock
        {
            ProductId = request.ProductId,
            Current = entity.Current,
            Previous = entity.Previous,
            Booked = entity.Booked - request.Value,
            Released = entity.Released + request.Value,
            Action = "Release",
            ActionValue = request.Value
        };
        
        await _repository.AddAsync(stockCreate);
        
        return new ReleaseProductStockResponse
        {
            Id = stockCreate.Id,
            ProductId = request.ProductId,
            Value = request.Value,
            Success = true,
            StatusMessage = "Successfully released booked product"
        };
    }
}
