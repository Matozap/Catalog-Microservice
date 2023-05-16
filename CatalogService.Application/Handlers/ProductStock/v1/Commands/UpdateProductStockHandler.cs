using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Handlers.ProductStock.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductStock.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductStock.v1.Commands;

public class UpdateProductStockHandler : IRequestHandler<UpdateProductStock, ProductStockData>
{
    private readonly ILogger<UpdateProductStockHandler> _logger;
    private readonly IRepository _repository;

    public UpdateProductStockHandler(ILogger<UpdateProductStockHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ProductStockData> Handle(UpdateProductStock request, CancellationToken cancellationToken)
    {
        var result = await UpdateProductStock(request);
        _logger.LogInformation("ProductStock for product with id {ProductStockID} updated to {CurrentStock}", request.Id, result.Current.ToString("F2"));
            
        return result;
    }

    private async Task<ProductStockData> UpdateProductStock(UpdateProductStock request)
    {
        var stockValue = new Domain.ProductStock()
        {
            ProductId = request.Id,
            Previous = 0,
            Booked = 0
        };
        var entity = await _repository.GetAsSingleAsync<Domain.ProductStock, string>(productStock => productStock.ProductId == request.Id, orderDescending: stock => stock.Id);
        if (entity == null)
        {
            stockValue.Current = request.Value;
        }
        else
        {
            stockValue.Previous = stockValue.Current;
            stockValue.Current += request.Value;
        }
        
        await _repository.AddAsync(stockValue);
        return stockValue.Adapt<Domain.ProductStock, ProductStockData>();
    }
}
