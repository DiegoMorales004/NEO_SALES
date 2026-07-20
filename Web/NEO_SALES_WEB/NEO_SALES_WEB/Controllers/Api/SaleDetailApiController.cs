using Microsoft.AspNetCore.Mvc;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES_WEB.Controllers.Api;

[ApiController]
[Route("bff/sale-detail")]
public class SaleDetailApiController : ControllerBase
{
    private readonly ISaleDetailService _saleDetailService;

    public SaleDetailApiController(ISaleDetailService saleDetailService)
    {
        _saleDetailService = saleDetailService;
    }

    [HttpGet("sale/{saleId:guid}")]
    public async Task<IActionResult> BySale(Guid saleId)
    {
        var details = await _saleDetailService.GetBySaleIdAsync(saleId);
        return Ok(details);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SaleDetailCreateDto dto)
    {
        var created = await _saleDetailService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] SaleDetailEditDto dto)
    {
        var updated = await _saleDetailService.EditAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _saleDetailService.DeleteAsync(id);
        return NoContent();
    }
}
