using System;
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

public class CreateProductImageHandler : IRequestHandler<CreateProductImage, ProductImageData>
{
    private readonly ILogger<CreateProductImageHandler> _logger;
    private readonly IRepository _repository;

    public CreateProductImageHandler(ILogger<CreateProductImageHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ProductImageData> Handle(CreateProductImage request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Details?.Url);
        
        var resultEntity = await CreateProductImage(request.Details);
        if (resultEntity == null) return null;
        
        _logger.LogInformation("ProductImage with id {ProductImageID} created successfully", resultEntity.Id);
        var resultDto = resultEntity.Adapt<ProductImage, ProductImageData>();

        return resultDto;
    }

    private async Task<ProductImage> CreateProductImage(ProductImageData productImage)
    {
        if (await _repository.GetAsSingleAsync<ProductImage, string>(e => e.Title == productImage.Title && e.ProductId == productImage.ProductId) != null)
        {
            return null;
        }
        
        var entity = productImage.Adapt<ProductImageData, ProductImage>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
