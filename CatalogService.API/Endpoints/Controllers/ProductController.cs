using System.Threading.Tasks;
using CatalogService.API.Outputs;
using CatalogService.API.Outputs.Base;
using CatalogService.Application.Products.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Endpoints.Controllers;

[Produces("application/json")]
[Route("api/v1/")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly ProductOutput _productOutput;
    
    public ProductController(IMediator mediator)
    {
        _productOutput = new ProductOutput(mediator, OutputType.Controller);
    }

    /// <summary>
    /// Gets all the products in the system.
    /// </summary>
    /// <returns>All products</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("products/")]
    public async Task<IActionResult> GetAll() => await _productOutput.GetAllAsync<ActionResult>();
    
    /// <summary>
    /// Gets all the products in the system.
    /// </summary>
    /// <returns>All products</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("products/{id}")]
    public async Task<IActionResult> GetAll(string id) => await _productOutput.GetAllAsync<ActionResult>(id);

    /// <summary>
    /// Gets a product by id (string).
    /// </summary>
    /// <param name="id">Id or Code</param>
    /// <returns>Product</returns>
    /// <response code="200">OK</response>
    /// <response code="404">Not Found</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("product/{id}")]
    public async Task<IActionResult> Get(string id) => await _productOutput.GetAsync<ActionResult>(id);

    /// <summary>
    /// Creates an product based in the given object. 
    /// </summary>
    /// <param name="data">Product Data</param>
    /// <returns>Product</returns>
    /// <response code="201">Created</response>
    /// <response code="409">Conflict</response>
    /// <response code="500">Internal Server error</response>
    [HttpPost("product")]
    public async Task<IActionResult> Create([FromBody] ProductData data) => await _productOutput.CreateAsync<ActionResult>(data);

    /// <summary>
    /// Updates an product based in the given object.
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>Product</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpPut("product")]
    public async Task<IActionResult> Update([FromBody] ProductData data) => await _productOutput.UpdateAsync<ActionResult>(data);

    /// <summary>
    /// Does a soft delete on the product with the given id.
    /// </summary>
    /// <param name="id">Id or Code</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("product/disable/{id}")]
    public async Task<IActionResult> Disable(string id) => await _productOutput.DisableAsync<ActionResult>(id);

    /// <summary>
    /// Does a physical delete on the product with the given id.
    /// </summary>
    /// <param name="id">Id or Code</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("product/{id}")]
    public async Task<IActionResult> Delete(string id) => await _productOutput.DeleteAsync<ActionResult>(id);
}
