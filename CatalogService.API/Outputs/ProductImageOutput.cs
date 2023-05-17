using System.Net;
using System.Threading.Tasks;
using CatalogService.API.Outputs.Base;
using CatalogService.Message.Contracts.ProductImages.v1;
using CatalogService.Message.Contracts.ProductImages.v1.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace CatalogService.API.Outputs;

public class ProductImageOutput : OutputBase
{
    private readonly IMediator _mediator;
    
    public ProductImageOutput(IMediator mediator, OutputType outputType) : base(outputType)
    {
        _mediator = mediator;
    }

    [NonAction]
    public async Task<T> GetAllAsync<T>(string productId, HttpRequestData httpRequestData = null) where T: class
    {
        var result = await _mediator.Send(new GetAllProductImages
        {
            ProductId = productId
        });
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> GetAsync<T>(string code, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new GetProductImageById
        {
            Id = code
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> CreateAsync<T>(ProductImageData data, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new CreateProductImage
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.Conflict : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> UpdateAsync<T>(ProductImageData data, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new UpdateProductImage
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> DisableAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new SoftDeleteProductImage
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> DeleteAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new DeleteProductImage
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData) as T;
    }
}
