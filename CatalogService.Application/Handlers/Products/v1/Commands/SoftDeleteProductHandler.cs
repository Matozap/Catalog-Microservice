using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Handlers.Products.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.Products.v1.Commands;

public class SoftDeleteProductHandler : IRequestHandler<SoftDeleteProduct, string>
{
    private readonly ILogger<SoftDeleteProductHandler> _logger;
    private readonly IRepository _repository;

    public SoftDeleteProductHandler(ILogger<SoftDeleteProductHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(SoftDeleteProduct request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.Id);

        var entity = await DisableProduct(request.Id);
        _logger.LogInformation("Product with id {ProductID} disabled successfully", entity?.Id);
        return entity?.Id;
    }

    private async Task<Product> DisableProduct(string productId)
    {
        var entity = await _repository.GetAsSingleAsync<Product, string>(c => c.Id == productId || c.Code == productId);
        if (entity == null) return null;
        
        entity.Disabled = true;
        await _repository.UpdateAsync(entity);

        return entity;
    }
}
