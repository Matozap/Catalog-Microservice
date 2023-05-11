using System;
using System.Threading;
using System.Threading.Tasks;
using CatalogService.Application.Handlers.ProductStock.v1.Requests;
using CatalogService.Application.Interfaces;
using CatalogService.Domain;
using CatalogService.Message.Contracts.ProductStock.v1;
using Mapster;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CatalogService.Application.Handlers.ProductStock.v1.Commands;

public class CreateProductStockHandler : IRequestHandler<CreateProductStock, ProductStockData>
{
    private readonly ILogger<CreateProductStockHandler> _logger;
    private readonly IRepository _repository;

    public CreateProductStockHandler(ILogger<CreateProductStockHandler> logger, IRepository repository)
    {
        _logger = logger;
        _repository = repository;
    }

    public async Task<ProductStockData> Handle(CreateProductStock request, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrEmpty(request.Details?.Name);
        
        var resultEntity = await CreateProductStock(request.Details);
        if (resultEntity == null) return null;
        
        _logger.LogInformation("ProductStock with id {ProductStockID} created successfully", resultEntity.Id);
        var resultDto = resultEntity.Adapt<Domain.ProductStock, ProductStockData>();

        return resultDto;
    }

    private async Task<Domain.ProductStock> CreateProductStock(ProductStockData productStock)
    {
        if (await _repository.GetAsSingleAsync<Domain.ProductStock, string>(e => e.Name == productStock.Name && e.ProductImageId == productStock.ProductImageId) != null)
        {
            return null;
        }
        
        var entity = productStock.Adapt<ProductStockData, Domain.ProductStock>();
        entity.LastUpdateUserId ??= "system";
        entity.LastUpdateDate = DateTime.Now;
        
        return await _repository.AddAsync(entity);
    }
}
