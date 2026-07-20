using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES_API.Controllers;

[Authorize]
[ApiController]
[Route("api/sale")]
public class SaleController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SaleController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var sales = await _saleService.GetAllAsync();
        return Ok(sales);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var sale = await _saleService.GetByIdAsync(id);
        return Ok(sale);
    }

    [HttpGet("customer/{id}")]
    public async Task<IActionResult> ByCustomer(Guid id)
    {
        var sales = await _saleService.GetByCustomerIdAsync(id);
        return Ok(sales);
    }

    [HttpGet("cart/{id}")]
    public async Task<IActionResult> Cart(Guid id)
    {
        var cart = await _saleService.GetShoppingCartByCustomerAsync(id);
        return Ok(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaleCreateDto dto)
    {
        var created = await _saleService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(Guid id, SaleEditDto dto)
    {
        var updated = await _saleService.EditAsync(id, dto);
        return Ok(updated);
    }

    [HttpPost("confirm/{id}")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var actor = User.FindFirstValue(ClaimTypes.Name) ?? "SISTEMA";
        var result = await _saleService.ConfirmAsync(id, actor);

        return result.Success ? Ok(result) : BadRequest(result);
    }
}
