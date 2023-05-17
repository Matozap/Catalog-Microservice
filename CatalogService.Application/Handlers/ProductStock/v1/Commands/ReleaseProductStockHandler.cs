using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductStock.v1.Requests;
using CatalogService.Message.Contracts.ProductStock.v1.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductStock.v1.Commands;

public class ReleaseProductStockHandler : IRequestHandler<ReleaseProductStock, ReleaseProductStockResponse>
{
    private readonly ILogger<ReleaseProductStockHandler> _logger;
    private readonly IRepository _repository;

    public ReleaseProductStockHandler(ILogger<ReleaseProductStockHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ReleaseProductStockResponse> Handle(ReleaseProductStock request, CancellationToken cancellationToken)
    {
        var result = await UpdateProductStock(request);
        _logger.LogInformation("ProductStock release for product with id {ProductStockID} completed with status {Status}", request.ProductId, result.StatusMessage);
            
        return result;
    }

    private async Task<ReleaseProductStockResponse> UpdateProductStock(ReleaseProductStock request)
    {
        var entity = await _repository.GetAsSingleAsync<Domain.ProductStock, string>(productStock => productStock.ProductId == request.ProductId, orderDescending: stock => stock.Id);
        if (entity == null)
        {
            return new ReleaseProductStockResponse
            {
                ProductId = request.ProductId,
                Value = 0,
                Success = false,
                StatusMessage = $"There is no booked stock for product {request.ProductId} to be released"
            };
        }

        if (entity.Booked - request.Value < 0)
        {
            return new ReleaseProductStockResponse
            {
                ProductId = request.ProductId,
                Value = 0,
                Success = false,
                StatusMessage = $"Not enough booked stock to release {request.Value} items of product {request.ProductId} - Current booked stock is {entity.Current}"
            };
        }

        entity.Booked -= request.Value;
        
        await _repository.UpdateAsync(entity);
        return new ReleaseProductStockResponse
        {
            ProductId = request.ProductId,
            Value = request.Value,
            Success = true,
            StatusMessage = "Successfully released booked product"
        };
    }
}
