using System.Collections.Generic;
using System.Threading.Tasks;
using CatalogService.API.Outputs;
using CatalogService.API.Outputs.Base;
using CatalogService.Application.Common;
using CatalogService.Application.Products;
using CatalogService.Application.Products.Responses;
using MediatR;

namespace CatalogService.API.Endpoints.Grpc;

public class ProductService : IProductService
{
    private readonly ProductOutput _productOutput;
    
    public ProductService(IMediator mediator)
    {
        _productOutput = new ProductOutput(mediator, OutputType.Grpc);
    }

    public async Task<List<ProductData>> GetAll(StringWrapper id) => await _productOutput.GetAllAsync<List<ProductData>>(id.Value);
    
    public async Task<ProductData> Get(StringWrapper id) => await _productOutput.GetAsync<ProductData>(id.Value);
    
    public async Task<ProductData> Create(ProductData data) => await _productOutput.CreateAsync<ProductData>(data);
    
    public async Task<ProductData> Update(ProductData data) => await _productOutput.UpdateAsync<ProductData>(data);
    
    public async Task Disable(StringWrapper id) => await _productOutput.DisableAsync<ProductData>(id.Value);

    public async Task Delete(StringWrapper id) => await _productOutput.DeleteAsync<ProductData>(id.Value);
}
