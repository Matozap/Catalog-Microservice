using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductCategories.v1.Requests;
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
        var entity = await _repository.GetAsSingleAsync<ProductCategory, string>(c => c.Id == id, includeNavigationalProperties: true);

        if (entity == null) return null;
        
        if (entity.Products != null)
        {
            foreach (var product in entity.Products.ToList())
            {
                if (product.ProductImages?.Count > 0)
                {
                    foreach (var productImage in product.ProductImages.ToList())
                    {
                        await _repository.DeleteAsync(productImage);
                    }
                }
                
                if (product.ProductStocks?.Count > 0)
                {
                    foreach (var productStock in product.ProductStocks.ToList())
                    {
                        await _repository.DeleteAsync(productStock);
                    }
                }
                
                await _repository.DeleteAsync(product);
            }
        }
            
        await _repository.DeleteAsync(entity);

        return entity;
    }
}
