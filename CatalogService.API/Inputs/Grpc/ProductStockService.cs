using System.Collections.Generic;
using System.Threading.Tasks;
using CatalogService.API.Outputs;
using CatalogService.API.Outputs.Base;
using CatalogService.Message.Contracts.ProductStock.v1;
using CatalogService.Message.Contracts.Common;
using CatalogService.Message.Contracts.ProductStock.v1.Requests;
using CatalogService.Message.Contracts.ProductStock.v1.Responses;
using MediatR;

namespace CatalogService.API.Inputs.Grpc;

public class ProductStockService : IProductStockService
{
    private readonly ProductStockOutput _productStockOutput;
     
     public ProductStockService(IMediator mediator)
     {
         _productStockOutput = new ProductStockOutput(mediator, OutputType.Grpc);
     }
    
    public async Task<List<ProductStockData>> GetAll(StringWrapper productImageId) => await _productStockOutput.GetAllAsync<List<ProductStockData>>(productImageId.Value);
    
    public async Task<ProductStockData> Get(StringWrapper id) => await _productStockOutput.GetAsync<ProductStockData>(id.Value);
    
    public async Task<ProductStockData> Create(ProductStockData data) => await _productStockOutput.CreateAsync<ProductStockData>(data);
    
    public async Task<ProductStockData> Update(UpdateProductStock data) => await _productStockOutput.UpdateAsync<ProductStockData>(data);
    
    public async Task Disable(StringWrapper id) => await _productStockOutput.DisableAsync<ProductStockData>(id.Value);

    public async Task Delete(StringWrapper id) => await _productStockOutput.DeleteAsync<ProductStockData>(id.Value);
    
    public async Task<BookProductStockResponse> Book(BookProductStock data) => await _productStockOutput.BookAsync<BookProductStockResponse>(data);
    
    public async Task<ReleaseProductStockResponse> Release(ReleaseProductStock data) => await _productStockOutput.ReleaseAsync<ReleaseProductStockResponse>(data);
}
