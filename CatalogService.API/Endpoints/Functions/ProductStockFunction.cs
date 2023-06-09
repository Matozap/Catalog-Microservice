using System.Threading.Tasks;
using CatalogService.API.Outputs;
using CatalogService.API.Outputs.Base;
using CatalogService.Application.ProductStock.Requests;
using CatalogService.Application.ProductStock.Responses;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CatalogService.API.Endpoints.Functions;

public class ProductStockFunction
{
    private readonly ProductStockOutput _productStockOutput;
    
    public ProductStockFunction(IMediator mediator)
    {
        _productStockOutput = new ProductStockOutput(mediator, OutputType.AzureFunction);
    }
    
    [Function($"ProductStock-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/productStock/{productImageId}")] HttpRequestData req, string productImageId)
        => await _productStockOutput.GetAllAsync<HttpResponseData>(productImageId, req);
    
    [Function($"ProductStock-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/productStock/{id}")] HttpRequestData req, string id)
        => await _productStockOutput.GetAsync<HttpResponseData>(id, req);
    
    [Function($"ProductStock-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/productStock")] HttpRequestData req)
        => await _productStockOutput.CreateAsync<HttpResponseData>(await req.ReadFromJsonAsync<ProductStockData>(), req);
    
    [Function($"ProductStock-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/productStock")] HttpRequestData req)
        => await _productStockOutput.UpdateAsync<HttpResponseData>(await req.ReadFromJsonAsync<UpdateProductStock>(), req);
    
    [Function($"ProductStock-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/productStock/disable/{id}" )] HttpRequestData req, string id)
        => await _productStockOutput.DisableAsync<HttpResponseData>(id, req);
    
    [Function($"ProductStock-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/productStock/{id}")] HttpRequestData req, string id)
        => await _productStockOutput.DeleteAsync<HttpResponseData>(id, req);
    
    [Function($"ProductStock-{nameof(Book)}")]
    public async Task<HttpResponseData> Book([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/productStock/book")] HttpRequestData req)
        => await _productStockOutput.BookAsync<HttpResponseData>(await req.ReadFromJsonAsync<BookProductStock>(), req);
    
    [Function($"ProductStock-{nameof(Release)}")]
    public async Task<HttpResponseData> Release([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/productStock/release")] HttpRequestData req)
        => await _productStockOutput.ReleaseAsync<HttpResponseData>(await req.ReadFromJsonAsync<ReleaseProductStock>(), req);
}
