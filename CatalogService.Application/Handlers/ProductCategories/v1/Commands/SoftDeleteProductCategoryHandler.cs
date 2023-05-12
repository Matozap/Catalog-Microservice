using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Handlers.ProductCategories.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductCategories.v1.Commands;

public class SoftDeleteProductCategoryHandler : IRequestHandler<SoftDeleteProductCategory, string>
{
    private readonly ILogger<SoftDeleteProductCategoryHandler> _logger;
    private readonly IRepository _repository;

    public SoftDeleteProductCategoryHandler(ILogger<SoftDeleteProductCategoryHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(SoftDeleteProductCategory request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.Id);

        var entity = await DisableProductCategory(request.Id);
        _logger.LogInformation("ProductCategory with id {ProductCategoryID} disabled successfully", entity?.Id);
        return entity?.Id;
    }

    private async Task<ProductCategory> DisableProductCategory(string productId)
    {
        var entity = await _repository.GetAsSingleAsync<ProductCategory, string>(c => c.Id == productId);
        if (entity == null) return null;
        
        entity.Disabled = true;
        await _repository.UpdateAsync(entity);

        return entity;
    }
}
