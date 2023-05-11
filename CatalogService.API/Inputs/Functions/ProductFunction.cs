using System.Threading.Tasks;
using CatalogService.API.Outputs;
using CatalogService.API.Outputs.Base;
using CatalogService.Message.Contracts.Products.v1;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CatalogService.API.Inputs.Functions;

public class ProductFunction
{
    private readonly ProductOutput _productOutput;
    
    public ProductFunction(IMediator mediator)
    {
        _productOutput = new ProductOutput(mediator, OutputType.AzureFunction);
    }
    
    [Function($"Product-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/products")] HttpRequestData req) 
        => await _productOutput.GetAllAsync<HttpResponseData>(req);

    [Function($"Product-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/product/{id}")] HttpRequestData req, string id) 
        => await _productOutput.GetAsync<HttpResponseData>(id, req);

    [Function($"Product-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/product")] HttpRequestData req) 
        => await _productOutput.CreateAsync<HttpResponseData>(await req.ReadFromJsonAsync<ProductData>(), req);
    
    [Function($"Product-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/product")] HttpRequestData req)
        => await _productOutput.UpdateAsync<HttpResponseData>(await req.ReadFromJsonAsync<ProductData>(), req);
    
    [Function($"Product-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/product/disable/{id}" )] HttpRequestData req, string id)
        => await _productOutput.DisableAsync<HttpResponseData>(id, req);
    
    [Function($"Product-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/product/{id}")] HttpRequestData req, string id)
        => await _productOutput.DeleteAsync<HttpResponseData>(id, req);
}
