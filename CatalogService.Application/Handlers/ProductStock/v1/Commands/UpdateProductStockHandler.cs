using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Handlers.ProductStock.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
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
        var result = await UpdateProductStock(request.Details);
        _logger.LogInformation("ProductStock with id {ProductStockID} updated successfully", request.Details.Id);
            
        return result;
    }

    private async Task<ProductStockData> UpdateProductStock(ProductStockData productStockData)
    {
        var entity = await _repository.GetAsSingleAsync<Domain.ProductStock, string>(productStock => productStock.Id == productStockData.Id);
        if (entity == null) return null;
        
        productStockData.ProductImageId = entity.ProductImageId;
        var changes = productStockData.Adapt(entity);
        
        await _repository.UpdateAsync(changes);
        return changes.Adapt<Domain.ProductStock, ProductStockData>();
    }
}
