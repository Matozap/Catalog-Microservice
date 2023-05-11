using System.Threading.Tasks;
using CatalogService.API.Outputs;
using CatalogService.API.Outputs.Base;
using CatalogService.Message.Contracts.ProductImages.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Inputs.Controllers;

[Produces("application/json")]
[Route("api/v1/")]
[ApiController]
public class ProductImageController
{
    private readonly ProductImageOutput _productImageOutput;

    public ProductImageController(IMediator mediator)
    {
        _productImageOutput = new ProductImageOutput(mediator, OutputType.Controller);
    }

    /// <summary>
    /// Gets all the productImages in a product.
    /// </summary>
    /// <returns>All ProductImages</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("productImages/{productId}")]
    public async Task<IActionResult> GetAll(string productId) => await _productImageOutput.GetAllAsync<ActionResult>(productId);

    /// <summary>
    /// Gets s productImage by id (number or string).
    /// </summary>
    /// <param name="code">ProductImage Id or code</param>
    /// <returns>ProductImage</returns>
    /// <response code="200">OK</response>
    /// <response code="404">Not Found</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("productImage/{code}")]
    public async Task<IActionResult> Get(string code) => await _productImageOutput.GetAsync<ActionResult>(code);
    
    /// <summary>
    /// Creates an productImage based in the given object. 
    /// </summary>
    /// <param name="data">ProductImage Data</param>
    /// <returns>ProductImage</returns>
    /// <response code="201">Created</response>
    /// <response code="409">Conflict</response>
    /// <response code="500">Internal Server error</response>
    [HttpPost("productImage")]
    public async Task<IActionResult> Create([FromBody] ProductImageData data) => await _productImageOutput.CreateAsync<ActionResult>(data);

    /// <summary>
    /// Updates an productImage based in the given object.
    /// </summary>
    /// <param name="data">ProductImage Data</param>
    /// <returns>Product</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpPut("productImage")]
    public async Task<IActionResult> Update([FromBody] ProductImageData data) => await _productImageOutput.UpdateAsync<ActionResult>(data);

    /// <summary>
    /// Does a soft delete on the productImage with the given id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("productImage/disable/{id}")]
    public async Task<IActionResult> Disable(string id) => await _productImageOutput.DisableAsync<ActionResult>(id);

    /// <summary>
    /// Does a physical delete on the productImage with the given id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("productImage/{id}")]
    public async Task<IActionResult> Delete(string id) => await _productImageOutput.DeleteAsync<ActionResult>(id);
}
