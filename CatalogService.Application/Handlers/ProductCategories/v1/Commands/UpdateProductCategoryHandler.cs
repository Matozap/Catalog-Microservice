using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductCategories.v1;
using CatalogService.Message.Contracts.ProductCategories.v1.Requests;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductCategories.v1.Commands;

public class UpdateProductCategoryHandler : IRequestHandler<UpdateProductCategory, ProductCategoryData>
{
    private readonly ILogger<UpdateProductCategoryHandler> _logger;
    private readonly IRepository _repository;

    public UpdateProductCategoryHandler(ILogger<UpdateProductCategoryHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ProductCategoryData> Handle(UpdateProductCategory request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Details?.Id);
        
        var result = await UpdateProductCategory(request.Details);
        _logger.LogInformation("ProductCategory with id {ProductCategoryID} updated successfully", request.Details.Id);
            
        return result;
    }

    private async Task<ProductCategoryData> UpdateProductCategory(ProductCategoryData productData)
    {
        var entity = await _repository.GetAsSingleAsync<ProductCategory, string>(e => e.Id == productData.Id);
        if (entity == null) return null;
        
        var changes = productData.Adapt(entity);
        await _repository.UpdateAsync(changes);
        return changes.Adapt<ProductCategory, ProductCategoryData>();
    }
}
