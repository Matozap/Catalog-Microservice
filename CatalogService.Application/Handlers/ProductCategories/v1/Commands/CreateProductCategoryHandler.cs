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

public class CreateProductCategoryHandler : IRequestHandler<CreateProductCategory, ProductCategoryData>
{
    private readonly ILogger<CreateProductCategoryHandler> _logger;
    private readonly IRepository _repository;

    public CreateProductCategoryHandler(ILogger<CreateProductCategoryHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ProductCategoryData> Handle(CreateProductCategory request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Details?.Name);
        
        var resultEntity = await CreateProductCategory(request.Details);
        if (resultEntity == null) return null;
            
        _logger.LogInformation("ProductCategory with id {ProductCategoryID} created successfully", resultEntity.Id);
        return resultEntity.Adapt<ProductCategory, ProductCategoryData>();
    }

    private async Task<ProductCategory> CreateProductCategory(ProductCategoryData product)
    {
        if (await _repository.GetAsSingleAsync<ProductCategory,string>(e => e.Name == product.Name) != null)
        {
            return null;
        }
        
        var entity = product.Adapt<ProductCategoryData, ProductCategory>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
