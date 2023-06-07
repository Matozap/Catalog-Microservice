using System.Collections.Generic;
using System.Threading.Tasks;
using CatalogService.API.Outputs;
using CatalogService.API.Outputs.Base;
using CatalogService.Application.Common;
using CatalogService.Application.ProductImages;
using CatalogService.Application.ProductImages.Responses;
using MediatR;

namespace CatalogService.API.Endpoints.Grpc;

public class ProductImageService : IProductImageService
{
    private readonly ProductImageOutput _productImageOutput;
    
    public ProductImageService(IMediator mediator)
    {
        _productImageOutput = new ProductImageOutput(mediator, OutputType.Grpc);
    }

    public async Task<List<ProductImageData>> GetAll(StringWrapper productImageId) => await _productImageOutput.GetAllAsync<List<ProductImageData>>(productImageId.Value);
    
    public async Task<ProductImageData> Get(StringWrapper id) => await _productImageOutput.GetAsync<ProductImageData>(id.Value);
    
    public async Task<ProductImageData> Create(ProductImageData data) => await _productImageOutput.CreateAsync<ProductImageData>(data);
    
    public async Task<ProductImageData> Update(ProductImageData data) => await _productImageOutput.UpdateAsync<ProductImageData>(data);
    
    public async Task Disable(StringWrapper id) => await _productImageOutput.DisableAsync<ProductImageData>(id.Value);

    public async Task Delete(StringWrapper id) => await _productImageOutput.DeleteAsync<ProductImageData>(id.Value);
}
