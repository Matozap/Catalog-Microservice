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

public class BookProductStockHandler : BuilderRequestHandler<BookProductStock, BookProductStockResponse>
{
    private readonly ILogger<BookProductStockHandler> _logger;
    private readonly IRepository _repository;
    private readonly ICache _cache;
    private readonly IEventBus _eventBus;

    public BookProductStockHandler(ILogger<BookProductStockHandler> logger, IRepository repository, ICache cache, IEventBus eventBus)
    {
        _logger = logger;
        _repository = repository;
        _cache = cache;
        _eventBus = eventBus;
    }
    
    protected override Task<BookProductStockResponse> PreProcess(BookProductStock request, CancellationToken cancellationToken = default)
    {
        return Task.FromResult<BookProductStockResponse>(null);
    }

    protected override async Task<BookProductStockResponse> Process(BookProductStock request, CancellationToken cancellationToken = default)
    {
        var result = await UpdateProductStock(request);
        _logger.LogInformation("ProductStock booking for product with id {ProductStockID} completed with status {Status}", request.ProductId, result.StatusMessage);
            
        return result;
    }

    protected override async Task PostProcess(BookProductStock request, BookProductStockResponse response, CancellationToken cancellationToken = default)
    {
        await ClearCache(response, cancellationToken);
        await _eventBus.PublishAsync(new ProductStockBookEvent { Details = response, Action = EventAction.Created });
    }
    
    private async Task ClearCache(BookProductStockResponse data, CancellationToken cancellationToken)
    {
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.ProductStock)}:list", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.ProductStock)}:id:{data?.Id}", cancellationToken);
        await _cache.ClearCacheWithPrefixAsync($"{nameof(Domain.Product)}:id:{data?.ProductId}", cancellationToken);
    }

    private async Task<BookProductStockResponse> UpdateProductStock(BookProductStock request)
    {
        var stock = new BookProductStockResponse
        {
            ProductId = request.ProductId,
            Value = 0,
            Success = false
        };
        
        var entity = await _repository.GetAsSingleAsync<Domain.ProductStock, string>(productStock => productStock.ProductId == request.ProductId, orderDescending: stockOrder => stockOrder.Id);
        if (entity == null)
        {
            stock.StatusMessage = $"There is no stock loaded for product {request.ProductId}";
            return stock;
        }

        if (entity.Current - request.Value < 0)
        {
            stock.StatusMessage = $"Not enough stock to book {request.Value} items of product {request.ProductId} - Current stock is {entity.Current}";
            return stock;
        }

        var stockCreate = new Domain.ProductStock
        {
            ProductId = request.ProductId,
            Current = entity.Current - request.Value,
            Previous = entity.Previous,
            Booked = entity.Booked + request.Value,
            Released = entity.Released,
            Action = "Booking",
            ActionValue = request.Value
        };

        await _repository.AddAsync(stockCreate);
        
        return new BookProductStockResponse
        {
            Id = stockCreate.Id,
            ProductId = request.ProductId,
            Value = request.Value,
            Success = true,
            StatusMessage = "Successfully booked product"
        };
    }
}
