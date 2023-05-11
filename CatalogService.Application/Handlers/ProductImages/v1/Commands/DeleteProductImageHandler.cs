using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Handlers.ProductImages.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductImages.v1.Commands;

public class DeleteProductImageHandler : IRequestHandler<DeleteProductImage, string>
{
    private readonly ILogger<DeleteProductImageHandler> _logger;
    private readonly IRepository _repository;

    public DeleteProductImageHandler(ILogger<DeleteProductImageHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(DeleteProductImage request, CancellationToken cancellationToken)
    {
        var entity = await DeleteProductImageAsync(request.Id);
        _logger.LogInformation("ProductImage with id {ProductImageId} deleted successfully", entity?.Id);
        
        return entity?.Id;
    }

    private async Task<ProductImage> DeleteProductImageAsync(string productImageId)
    {
        var entity = await _repository.GetAsSingleAsync<ProductImage, string>(c => c.Id == productImageId || c.Code == productImageId);
        if (entity == null) return null;
            
        await _repository.DeleteAsync(entity);
        return entity;
    }
}
