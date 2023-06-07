using System.Collections.Generic;
using System.Threading.Tasks;
using CatalogService.API.Outputs;
using CatalogService.API.Outputs.Base;
using CatalogService.Application.Common;
using CatalogService.Application.ProductCategories;
using CatalogService.Application.ProductCategories.Responses;
using MediatR;

namespace CatalogService.API.Endpoints.Grpc;

public class ProductCategoryService : IProductCategoryService
{
    private readonly ProductCategoryOutput _productOutput;
    
    public ProductCategoryService(IMediator mediator)
    {
        _productOutput = new ProductCategoryOutput(mediator, OutputType.Grpc);
    }

    public async Task<List<ProductCategoryData>> GetAll(StringWrapper id) => await _productOutput.GetAllAsync<List<ProductCategoryData>>();
    
    public async Task<ProductCategoryData> Get(StringWrapper id) => await _productOutput.GetAsync<ProductCategoryData>(id.Value);
    
    public async Task<ProductCategoryData> Create(ProductCategoryData data) => await _productOutput.CreateAsync<ProductCategoryData>(data);
    
    public async Task<ProductCategoryData> Update(ProductCategoryData data) => await _productOutput.UpdateAsync<ProductCategoryData>(data);
    
    public async Task Disable(StringWrapper id) => await _productOutput.DisableAsync<ProductCategoryData>(id.Value);

    public async Task Delete(StringWrapper id) => await _productOutput.DeleteAsync<ProductCategoryData>(id.Value);
}
