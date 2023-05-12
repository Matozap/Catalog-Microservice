using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Handlers.Products.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.Products.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.Products.v1.Commands;

public class UpdateProductHandler : IRequestHandler<UpdateProduct, ProductData>
{
    private readonly ILogger<UpdateProductHandler> _logger;
    private readonly IRepository _repository;

    public UpdateProductHandler(ILogger<UpdateProductHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ProductData> Handle(UpdateProduct request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Details?.Id);
        
        var result = await UpdateProduct(request.Details);
        _logger.LogInformation("Product with id {ProductID} updated successfully", request.Details.Id);
            
        return result;
    }

    private async Task<ProductData> UpdateProduct(ProductData productData)
    {
        var entity = await _repository.GetAsSingleAsync<Product, string>(e => e.Id == productData.Id || e.Sku == productData.Sku);
        if (entity == null) return null;
        
        var changes = productData.Adapt(entity);
        await _repository.UpdateAsync(changes);
        return changes.Adapt<Product, ProductData>();
    }
}
