using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductStock.v1.Requests;
using CatalogService.Message.Contracts.ProductStock.v1.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductStock.v1.Commands;

public class BookProductStockHandler : IRequestHandler<BookProductStock, BookProductStockResponse>
{
    private readonly ILogger<BookProductStockHandler> _logger;
    private readonly IRepository _repository;

    public BookProductStockHandler(ILogger<BookProductStockHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<BookProductStockResponse> Handle(BookProductStock request, CancellationToken cancellationToken)
    {
        var result = await UpdateProductStock(request);
        _logger.LogInformation("ProductStock booking for product with id {ProductStockID} completed with status {Status}", request.ProductId, result.StatusMessage);
            
        return result;
    }

    private async Task<BookProductStockResponse> UpdateProductStock(BookProductStock request)
    {
        var entity = await _repository.GetAsSingleAsync<Domain.ProductStock, string>(productStock => productStock.ProductId == request.ProductId, orderDescending: stock => stock.Id);
        if (entity == null)
        {
            return new BookProductStockResponse
            {
                ProductId = request.ProductId,
                Value = 0,
                Success = false,
                StatusMessage = $"There is no stock loaded for product {request.ProductId}"
            };
        }

        if (entity.Current - request.Value < 0)
        {
            return new BookProductStockResponse
            {
                ProductId = request.ProductId,
                Value = 0,
                Success = false,
                StatusMessage = $"Not enough stock to book {request.Value} items of product {request.ProductId} - Current stock is {entity.Current}"
            };
        }

        entity.Booked += request.Value;
        entity.Current -= request.Value;
        
        await _repository.UpdateAsync(entity);
        return new BookProductStockResponse
        {
            ProductId = request.ProductId,
            Value = request.Value,
            Success = true,
            StatusMessage = "Success booked product"
        };
    }
}
