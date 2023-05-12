using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Handlers.ProductCategories.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductCategories.v1.Commands;

public class DeleteProductCategoryHandler : IRequestHandler<DeleteProductCategory, string>
{
    private readonly ILogger<DeleteProductCategoryHandler> _logger;
    private readonly IRepository _repository;

    public DeleteProductCategoryHandler(ILogger<DeleteProductCategoryHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(DeleteProductCategory request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Id);

        var entity = await DeleteProductCategoryAsync(request.Id);
        _logger.LogInformation("ProductCategory with id {ProductCategoryID} deleted successfully", entity?.Id);

        return entity?.Id;
    }

    private async Task<ProductCategory> DeleteProductCategoryAsync(string id)
    {
        var entity = await _repository.GetAsSingleAsync<ProductCategory, string>(c => c.Id == id);
            
        if(entity != null)
        {                
            await _repository.DeleteAsync(entity);
        }

        return entity;
    }
}
