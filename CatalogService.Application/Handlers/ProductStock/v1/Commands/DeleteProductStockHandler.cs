using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Interfaces;
using CatalogService.Message.Contracts.ProductStock.v1.Requests;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductStock.v1.Commands;

public class DeleteProductStockHandler : IRequestHandler<DeleteProductStock, string>
{
    private readonly ILogger<DeleteProductStockHandler> _logger;
    private readonly IRepository _repository;

    public DeleteProductStockHandler(ILogger<DeleteProductStockHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(DeleteProductStock request, CancellationToken cancellationToken)
    {
        var entity = await DeleteProductStockAsync(request.Id);
        _logger.LogInformation("ProductStock with id {ProductStockId} deleted successfully", entity?.Id);
        
        return entity?.Id;
    }

    private async Task<Domain.ProductStock> DeleteProductStockAsync(string productStockId)
    {
        var entity = await _repository.GetAsSingleAsync<Domain.ProductStock, string>(productStock => productStock.Id == productStockId);
        if (entity == null) return null;
        
        await _repository.DeleteAsync(entity);
        return entity;
    }
}
