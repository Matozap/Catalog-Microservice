using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Handlers.Products.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.Products.v1.Commands;

public class DeleteProductHandler : IRequestHandler<DeleteProduct, string>
{
    private readonly ILogger<DeleteProductHandler> _logger;
    private readonly IRepository _repository;

    public DeleteProductHandler(ILogger<DeleteProductHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(DeleteProduct request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Id);

        var entity = await DeleteProductAsync(request.Id);
        _logger.LogInformation("Product with id {ProductID} deleted successfully", entity?.Id);

        return entity?.Id;
    }

    private async Task<Product> DeleteProductAsync(string id)
    {
        var entity = await _repository.GetAsSingleAsync<Product, string>(c => c.Id == id || c.Code == id);
            
        if(entity != null)
        {                
            await _repository.DeleteAsync(entity);
        }

        return entity;
    }
}
