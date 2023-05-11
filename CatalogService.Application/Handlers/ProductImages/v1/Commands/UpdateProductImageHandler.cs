using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Handlers.ProductImages.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductImages.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductImages.v1.Commands;

public class UpdateProductImageHandler : IRequestHandler<UpdateProductImage, ProductImageData>
{
    private readonly ILogger<UpdateProductImageHandler> _logger;
    private readonly IRepository _repository;

    public UpdateProductImageHandler(ILogger<UpdateProductImageHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ProductImageData> Handle(UpdateProductImage request, CancellationToken cancellationToken)
    {
        var result = await UpdateProductImage(request.Details);
        _logger.LogInformation("ProductImage with id {ProductImageID} updated successfully", request.Details.Id);
            
        return result;
    }

    private async Task<ProductImageData> UpdateProductImage(ProductImageData productImageData)
    {
        var entity = await _repository.GetAsSingleAsync<ProductImage, string>(productImage => productImage.Id == productImageData.Id || productImage.Code == productImageData.Id);
        if (entity == null) return null;

        productImageData.ProductId = entity.ProductId;
        var changes = productImageData.Adapt(entity);
        
        await _repository.UpdateAsync(changes);
        return changes.Adapt<ProductImage, ProductImageData>();
    }
}
