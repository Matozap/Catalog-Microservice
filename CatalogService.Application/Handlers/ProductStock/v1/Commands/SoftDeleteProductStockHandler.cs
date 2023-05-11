using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Handlers.ProductStock.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductStock.v1.Commands;

public class SoftDeleteProductStockHandler : IRequestHandler<SoftDeleteProductStock, string>
{
    private readonly ILogger<SoftDeleteProductStockHandler> _logger;
    private readonly IRepository _repository;

    public SoftDeleteProductStockHandler(ILogger<SoftDeleteProductStockHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(SoftDeleteProductStock request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.Id);
        
        var entity = await DisableProductStock(request.Id);
        _logger.LogInformation("ProductStock with id {ProductStockId} disabled successfully", entity?.Id);
        return entity?.Id;
    }

    private async Task<Domain.ProductStock> DisableProductStock(string productStockId)
    {
        var entity = await _repository.GetAsSingleAsync<Domain.ProductStock, string>(productStock => productStock.Id == productStockId);
        if (entity == null) return null;
        
        entity.Disabled = true;
        await _repository.UpdateAsync(entity);

        return entity;
    }
}
