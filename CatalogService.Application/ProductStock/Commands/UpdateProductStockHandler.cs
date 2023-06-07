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

public class UpdateProductStockHandler : BuilderRequestHandler<UpdateProductStock, ProductStockData>
{
    private readonly ILogger<UpdateProductStockHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public UpdateProductStockHandler(ILogger<UpdateProductStockHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<ProductStockData> PreProcess(UpdateProductStock request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<ProductStockData>(null);
    }

    protected override async Task<ProductStockData> Process(UpdateProductStock request, CancellationToken cancellationToken = default)
    {
        var result = await UpdateProductStock(request);
        _logger.LogInformation("ProductStock for product with id {ProductStockID} updated to {CurrentStock}", request.ProductId, result.Current.ToString("F2"));
            
        return result;
    }

    protected override async Task PostProcess(UpdateProductStock request, ProductStockData response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductStockEvent { Details = response, Action = EventAction.Updated });
    }
    
    private async Task ClearCache(ProductStockData data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.ProductStock)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.ProductStock)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.Product)}:id:{data?.ProductId}", cancellationToken);
    }

    private async Task<ProductStockData> UpdateProductStock(UpdateProductStock request)
    {
        var stockValue = new Domain.ProductStock
        {
            ProductId = request.ProductId,
            Previous = 0,
            Booked = 0,
            Action = "Update",
            ActionValue = request.Value
        };
        var entity = await _repository.GetAsSingleAsync<Domain.ProductStock, string>(productStock => productStock.ProductId == request.ProductId, orderDescending: stock => stock.Id);
        if (entity == null)
        {
            stockValue.Current = request.Value;
        }
        else
        {
            stockValue.Current = entity.Current + request.Value;
            stockValue.Previous = entity.Current;
            stockValue.Booked = entity.Booked;
            stockValue.Released = entity.Released;
        }
        
        await _repository.AddAsync(stockValue);
        return stockValue.Adapt<Domain.ProductStock, ProductStockData>();
    }
}
