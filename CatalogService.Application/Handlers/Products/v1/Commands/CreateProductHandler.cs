using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Handlers.Products.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.Products.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.Products.v1.Commands;

public class CreateProductHandler : IRequestHandler<CreateProduct, ProductData>
{
    private readonly ILogger<CreateProductHandler> _logger;
    private readonly IRepository _repository;

    public CreateProductHandler(ILogger<CreateProductHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ProductData> Handle(CreateProduct request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Details?.Name);
        
        var resultEntity = await CreateProduct(request.Details);
        if (resultEntity == null) return null;
            
        _logger.LogInformation("Product with id {ProductID} created successfully", resultEntity.Id);
        return resultEntity.Adapt<Product, ProductData>();
    }

    private async Task<Product> CreateProduct(ProductData product)
    {
        if (await _repository.GetAsSingleAsync<Product,string>(e => e.Code == product.Code || e.Name == product.Name) != null)
        {
            return null;
        }
        
        var entity = product.Adapt<ProductData, Product>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
