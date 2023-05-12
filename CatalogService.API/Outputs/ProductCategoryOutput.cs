using System.Net;
using System.Threading.Tasks;
using CatalogService.API.Outputs.Base;
using CatalogService.Application.Handlers.ProductCategories.v1.Requests;
using CatalogService.Message.Contracts.ProductCategories.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker.Http;

namespace CatalogService.API.Outputs;

public class ProductCategoryOutput : OutputBase
{
    private readonly IMediator _mediator;
    
    public ProductCategoryOutput(IMediator mediator, OutputType outputType) : base(outputType)
    {
        _mediator = mediator;
    }

    [NonAction]
    public async Task<T> GetAllAsync<T>(HttpRequestData httpRequestData = null) where T: class
    {
        var result = await _mediator.Send(new GetAllProductCategories());
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> GetAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new GetProductCategoryById
        {
            Id = id
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.NotFound : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> CreateAsync<T>(ProductCategoryData data, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new CreateProductCategory
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, result == null ? HttpStatusCode.Conflict : HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> UpdateAsync<T>(ProductCategoryData data, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new UpdateProductCategory
        {
            Details = data
        };
        
        var result = await _mediator.Send(query);
        return await TransformToOutputAsync(result, HttpStatusCode.OK, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> DisableAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new SoftDeleteProductCategory
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData) as T;
    }
    
    [NonAction]
    public async Task<T> DeleteAsync<T>(string id, HttpRequestData httpRequestData = null) where T: class
    {
        var query = new DeleteProductCategory
        {
            Id = id
        };
        
        await _mediator.Send(query);
        return await TransformToOutputAsync(null, HttpStatusCode.NoContent, httpRequestData) as T;
    }
}
