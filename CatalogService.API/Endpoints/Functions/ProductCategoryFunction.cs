using System.Threading.Tasks;
using CatalogService.API.Outputs;
using CatalogService.API.Outputs.Base;
using CatalogService.Application.ProductCategories.Responses;
using MediatR;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CatalogService.API.Endpoints.Functions;

public class ProductCategoryFunction
{
    private readonly ProductCategoryOutput _productOutput;
    
    public ProductCategoryFunction(IMediator mediator)
    {
        _productOutput = new ProductCategoryOutput(mediator, OutputType.AzureFunction);
    }
    
    [Function($"ProductCategory-{nameof(GetAll)}")]
    public async Task<HttpResponseData> GetAll([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/productCategories")] HttpRequestData req) 
        => await _productOutput.GetAllAsync<HttpResponseData>(req);

    [Function($"ProductCategory-{nameof(Get)}")]
    public async Task<HttpResponseData> Get([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "v1/productCategory/{id}")] HttpRequestData req, string id) 
        => await _productOutput.GetAsync<HttpResponseData>(id, req);

    [Function($"ProductCategory-{nameof(Create)}")]
    public async Task<HttpResponseData> Create([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "v1/productCategory")] HttpRequestData req) 
        => await _productOutput.CreateAsync<HttpResponseData>(await req.ReadFromJsonAsync<ProductCategoryData>(), req);
    
    [Function($"ProductCategory-{nameof(Update)}")]
    public async Task<HttpResponseData> Update([HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "v1/productCategory")] HttpRequestData req)
        => await _productOutput.UpdateAsync<HttpResponseData>(await req.ReadFromJsonAsync<ProductCategoryData>(), req);
    
    [Function($"ProductCategory-{nameof(Disable)}")]
    public async Task<HttpResponseData> Disable([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/productCategory/disable/{id}" )] HttpRequestData req, string id)
        => await _productOutput.DisableAsync<HttpResponseData>(id, req);
    
    [Function($"ProductCategory-{nameof(Delete)}")]
    public async Task<HttpResponseData> Delete([HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "v1/productCategory/{id}")] HttpRequestData req, string id)
        => await _productOutput.DeleteAsync<HttpResponseData>(id, req);
}
