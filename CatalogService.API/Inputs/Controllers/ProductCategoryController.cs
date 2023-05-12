using System.Threading.Tasks;
using CatalogService.API.Outputs;
using CatalogService.API.Outputs.Base;
using CatalogService.Message.Contracts.ProductCategories.v1;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CatalogService.API.Inputs.Controllers;

[Produces("application/json")]
[Route("api/v1/")]
[ApiController]
public class ProductCategoryController : ControllerBase
{
    private readonly ProductCategoryOutput _productCategoryOutput;
    
    public ProductCategoryController(IMediator mediator)
    {
        _productCategoryOutput = new ProductCategoryOutput(mediator, OutputType.Controller);
    }

    /// <summary>
    /// Gets all the productCategories in the system.
    /// </summary>
    /// <returns>All productCategories</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("productCategories")]
    public async Task<IActionResult> GetAll() => await _productCategoryOutput.GetAllAsync<ActionResult>();

    /// <summary>
    /// Gets a productCategory by id (string).
    /// </summary>
    /// <param name="id">Id or Code</param>
    /// <returns>ProductCategory</returns>
    /// <response code="200">OK</response>
    /// <response code="404">Not Found</response>
    /// <response code="500">Internal Server error</response>
    [HttpGet("productCategory/{id}")]
    public async Task<IActionResult> Get(string id) => await _productCategoryOutput.GetAsync<ActionResult>(id);

    /// <summary>
    /// Creates an productCategory based in the given object. 
    /// </summary>
    /// <param name="data">ProductCategory Data</param>
    /// <returns>ProductCategory</returns>
    /// <response code="201">Created</response>
    /// <response code="409">Conflict</response>
    /// <response code="500">Internal Server error</response>
    [HttpPost("productCategory")]
    public async Task<IActionResult> Create([FromBody] ProductCategoryData data) => await _productCategoryOutput.CreateAsync<ActionResult>(data);

    /// <summary>
    /// Updates an productCategory based in the given object.
    /// </summary>
    /// <param name="data">Data</param>
    /// <returns>ProductCategory</returns>
    /// <response code="200">OK</response>
    /// <response code="500">Internal Server error</response>
    [HttpPut("productCategory")]
    public async Task<IActionResult> Update([FromBody] ProductCategoryData data) => await _productCategoryOutput.UpdateAsync<ActionResult>(data);

    /// <summary>
    /// Does a soft delete on the productCategory with the given id.
    /// </summary>
    /// <param name="id">Id or Code</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("productCategory/disable/{id}")]
    public async Task<IActionResult> Disable(string id) => await _productCategoryOutput.DisableAsync<ActionResult>(id);

    /// <summary>
    /// Does a physical delete on the productCategory with the given id.
    /// </summary>
    /// <param name="id">Id or Code</param>
    /// <response code="204">No Content</response>
    /// <response code="500">Internal Server error</response>
    [HttpDelete("productCategory/{id}")]
    public async Task<IActionResult> Delete(string id) => await _productCategoryOutput.DeleteAsync<ActionResult>(id);
}
