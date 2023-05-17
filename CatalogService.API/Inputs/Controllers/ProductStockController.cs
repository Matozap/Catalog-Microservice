using System.Threading.Tasks;
using CatalogService.API.Outputs;
using CatalogService.API.Outputs.Base;
using CatalogService.Message.Contracts.ProductStock.v1;
using CatalogService.Message.Contracts.ProductStock.v1.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Inputs.Controllers;

[Produces("application/json")]
[Route("api/v1/")]
[ApiController]
public class ProductStockController
{
    private readonly ProductStockOutput _productStockOutput;
    
    public ProductStockController(IMediator mediator)
    {
        _productStockOutput = new ProductStockOutput(mediator, OutputType.Controller);
    }
                
    /// <summary>
    /// Gets all the productStock in a productImage.
    /// </summary>
    /// <returns>All ProductStock</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("productStocks/{productId}")]
    public async Task<IActionResult> GetAll(string productId) => await _productStockOutput.GetAllAsync<ActionResult>(productId);

    /// <summary>
    /// Gets a productStock by id (number or string).
    /// </summary>
    /// <param name="id">Id</param>
    /// <returns>Product</returns>
    /// <response code="200">OK</response>
    /// <response code="404">Not Found</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("productStock/{id}")]
    public async Task<IActionResult> Get(string id) => await _productStockOutput.GetAsync<ActionResult>(id);
    
    /// <summary>
    /// Creates an productStock based in the given object. 
    /// </summary>
    /// <param name="data">ProductStock Data</param>
    /// <returns>ProductStock</returns>
    /// <response code="201">Created</response>
    /// <response code="409">Conflict</response>
    /// <response code="500">Internal Server error</response>
    [HttpPost("productStock")]
    public async Task<IActionResult> Create([FromBody] ProductStockData data) => await _productStockOutput.CreateAsync<ActionResult>(data);

    /// <summary>
    /// Updates an productStock based in the given object.
    /// </summary>
    /// <param name="data">ProductStock Data</param>
    /// <returns>Product</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpPut("productStock")]
    public async Task<IActionResult> Update([FromBody] UpdateProductStock data) => await _productStockOutput.UpdateAsync<ActionResult>(data);
    
    /// <summary>
    /// Does a soft delete on the productStock with the given id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("productStock/disable/{id}")]
    public async Task<IActionResult> Disable(string id) => await _productStockOutput.DisableAsync<ActionResult>(id);

    /// <summary>
    /// Does a physical delete on the productStock with the given id.
    /// </summary>
    /// <param name="id">Id</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("productStock/{id}")]
    public async Task<IActionResult> Delete(string id) => await _productStockOutput.DeleteAsync<ActionResult>(id);
    
    /// <summary>
    /// Books productStock based in the given object.
    /// </summary>
    /// <param name="data">ProductStock Data</param>
    /// <returns>Product</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpPost("productStock/book")]
    public async Task<IActionResult> Book([FromBody] BookProductStock data) => await _productStockOutput.BookAsync<ActionResult>(data);
    
    /// <summary>
    /// Releases productStock based in the given object.
    /// </summary>
    /// <param name="data">ProductStock Data</param>
    /// <returns>Product</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpPost("productStock/release")]
    public async Task<IActionResult> Release([FromBody] ReleaseProductStock data) => await _productStockOutput.ReleaseAsync<ActionResult>(data);
}
