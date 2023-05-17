using System.Net;
using System.Threading.Tasks;
using CatalogService.API.Outputs.Base;
using CatalogService.Message.Contracts.Products.v1;
using CatalogService.Message.Contracts.Products.v1.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace CatalogService.API.Outputs;

public class ProductOutput : OutputBase
{
    private readonly IMediator _mediator;
    
    public ProductOutput(IMediator mediator, OutputType outputType) : base(outputType)
    {
        _mediator = mediator;
    }

    [NonAction]
    public async Task<T> GetAllAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var result = await _mediator.Send(new GetAllProducts
        {
            Id = id
        });
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> GetAllAsync<T>(HttpRequestData httpRequestData = null) where T: class
    {
        var result = await _mediator.Send(new GetAllProducts());
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> GetAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new GetProductById
        {
            Id = id
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> CreateAsync<T>(ProductData data, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new CreateProduct
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.Conflict : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> UpdateAsync<T>(ProductData data, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new UpdateProduct
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> DisableAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new SoftDeleteProduct
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> DeleteAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new DeleteProduct
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData) as T;
    }
}
