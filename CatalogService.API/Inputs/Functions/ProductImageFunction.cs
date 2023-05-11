using System.Threading.Tasks;
using CatalogService.API.Outputs;
using CatalogService.API.Outputs.Base;
using CatalogService.Message.Contracts.ProductImages.v1;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CatalogService.API.Inputs.Functions;

public class ProductImageFunction
{
    private readonly ProductImageOutput _productImageOutput;
    
    public ProductImageFunction(IMediator mediator)
    {
        _productImageOutput = new ProductImageOutput(mediator, OutputType.AzureFunction);
    }
    
    [Function($"ProductImage-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/productImages/{productId}")] HttpRequestData req, string productId)
        => await _productImageOutput.GetAllAsync<HttpResponseData>(productId, req);
    
    [Function($"ProductImage-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/productImage/{id}")] HttpRequestData req, string id)
        => await _productImageOutput.GetAsync<HttpResponseData>(id, req);
    
    [Function($"ProductImage-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/productImage")] HttpRequestData req)
        => await _productImageOutput.CreateAsync<HttpResponseData>(await req.ReadFromJsonAsync<ProductImageData>(), req);
    
    [Function($"ProductImage-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/productImage")] HttpRequestData req)
        => await _productImageOutput.UpdateAsync<HttpResponseData>(await req.ReadFromJsonAsync<ProductImageData>(), req);
    
    [Function($"ProductImage-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/productImage/disable/{id}" )] HttpRequestData req, string id)
        => await _productImageOutput.DisableAsync<HttpResponseData>(id, req);
    
    [Function($"ProductImage-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/productImage/{id}")] HttpRequestData req, string id)
        => await _productImageOutput.DeleteAsync<HttpResponseData>(id, req);
}
