using System.Net;
using System.Threading.Tasks;
using CatalogService.API.Outputs.Base;
using CatalogService.Application.Handlers.ProductStock.v1.Requests;
using CatalogService.Message.Contracts.ProductStock.v1;
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
            ProductImageId = productImageId
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
    public async Task<T> UpdateAsync<T>(ProductStockData data, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new UpdateProductStock
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
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
