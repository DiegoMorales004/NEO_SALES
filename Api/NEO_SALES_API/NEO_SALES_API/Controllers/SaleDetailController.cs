using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES_API.Controllers;

[Authorize]
[ApiController]
[Route("api/sale-detail")]
public class SaleDetailController : ControllerBase
{
    private readonly ISaleDetailService _saleDetailService;

    public SaleDetailController(ISaleDetailService saleDetailService)
    {
        _saleDetailService = saleDetailService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var details = await _saleDetailService.GetAllAsync();
        return Ok(details);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var detail = await _saleDetailService.GetByIdAsync(id);
        return Ok(detail);
    }

    [HttpGet("sale/{id}")]
    public async Task<IActionResult> BySale(Guid id)
    {
        var details = await _saleDetailService.GetBySaleIdAsync(id);
        return Ok(details);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaleDetailCreateDto dto)
    {
        var created = await _saleDetailService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(Guid id, SaleDetailEditDto dto)
    {
        var updated = await _saleDetailService.EditAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Remove(Guid id)
    {
        await _saleDetailService.RemoveAsync(id);
        return NoContent();
    }
}
