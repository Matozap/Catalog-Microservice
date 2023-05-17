using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductImages.v1.Requests;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductImages.v1.Commands;

public class SoftDeleteProductImageHandler : IRequestHandler<SoftDeleteProductImage, string>
{
    private readonly ILogger<SoftDeleteProductImageHandler> _logger;
    private readonly IRepository _repository;

    public SoftDeleteProductImageHandler(ILogger<SoftDeleteProductImageHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<string> Handle(SoftDeleteProductImage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request.Id);
        
        var entity = await DisableProductImage(request.Id);
        _logger.LogInformation("ProductImage with id {ProductImageId} disabled successfully", entity?.Id);
        return entity?.Id;
    }

    private async Task<ProductImage> DisableProductImage(string productImageId)
    {
        var entity = await _repository.GetAsSingleAsync<ProductImage, string>(c => c.Id == productImageId || c.Title == productImageId);
        if (entity == null) return null;
        
        entity.Disabled = true;
        await _repository.UpdateAsync(entity);

        return entity;
    }
}
