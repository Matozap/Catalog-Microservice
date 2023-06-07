using System.Net;
using System.Threading.Tasks;
using CatalogService.API.Outputs.Base;
using CatalogService.Application.ProductStock.Requests;
using CatalogService.Application.ProductStock.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace CatalogService.API.Outputs;

public class ProductStockOutput : OutputBase
{
    private readonly IMediator _mediator;
    
    public ProductStockOutput(IMediator mediator, OutputType outputType) : base(outputType)
    {
        _mediator = mediator;
    }

    [NonAction]
    public async Task<T> GetAllAsync<T>(string productImageId, HttpRequestData httpRequestData = null) where T: class
    {
        var result = await _mediator.Send(new GetAllProductStock
        {
            ProductId = productImageId
        });
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> GetAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new GetProductStockById
        {
            Id = id
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> CreateAsync<T>(ProductStockData data, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new CreateProductStock
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.Conflict : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> UpdateAsync<T>(UpdateProductStock data, HttpRequestData httpRequestData = null) where T: class
    {
        var result = await _mediator.Send(data);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> BookAsync<T>(BookProductStock data, HttpRequestData httpRequestData = null) where T: class
    {
        var result = await _mediator.Send(data);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> ReleaseAsync<T>(ReleaseProductStock data, HttpRequestData httpRequestData = null) where T: class
    {
        var result = await _mediator.Send(data);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> DisableAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new SoftDeleteProductStock
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> DeleteAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new DeleteProductStock
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData) as T;
    }
}
