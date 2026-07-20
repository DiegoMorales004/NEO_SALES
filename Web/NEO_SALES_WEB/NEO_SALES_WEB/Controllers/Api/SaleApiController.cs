using Microsoft.AspNetCore.Mvc;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES_WEB.Controllers.Api;

[ApiController]
[Route("bff/sale")]
public class SaleApiController : ControllerBase
{
    private readonly ISaleService _saleService;

    public SaleApiController(ISaleService saleService)
    {
        _saleService = saleService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var sales = await _saleService.GetAllAsync();
        return Ok(sales);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var sale = await _saleService.GetByIdAsync(id);
        return Ok(sale);
    }

    [HttpGet("customer/{id:guid}")]
    public async Task<IActionResult> ByCustomer(Guid id)
    {
        var sales = await _saleService.GetByCustomerIdAsync(id);
        return Ok(sales);
    }

    [HttpGet("cart/{customerId:guid}")]
    public async Task<IActionResult> Cart(Guid customerId)
    {
        var cart = await _saleService.GetShoppingCartByCustomerAsync(customerId);
        return Ok(cart);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaleCreateDto dto)
    {
        var created = await _saleService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPost("confirm/{id:guid}")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var result = await _saleService.ConfirmAsync(id);
        return Ok(result);
    }
}
