using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES_API.Controllers;

[Authorize]
[ApiController]
[Route("api/product")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var products = await _productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var product = await _productService.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpGet("search/{search}")]
    public async Task<IActionResult> Search(string search)
    {
        var products = await _productService.SearchAsync(search);
        return Ok(products);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductCreateDto dto)
    {
        var actor = User.FindFirstValue(ClaimTypes.Name) ?? "SISTEMA";
        var created = await _productService.CreateAsync(dto, actor);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit([FromRoute]Guid id, ProductEditDto dto)
    {
        var actor = User.FindFirstValue(ClaimTypes.Name) ?? "SISTEMA";
        var updated = await _productService.EditAsync(id, dto, actor);
        return Ok(updated);
    }

}
