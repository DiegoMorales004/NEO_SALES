using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES_API.Controllers;

[Authorize]
[ApiController]
[Route("api/sale-status")]
public class SaleStatusController : ControllerBase
{
    private readonly ISaleStatusService _saleStatusService;

    public SaleStatusController(ISaleStatusService saleStatusService)
    {
        _saleStatusService = saleStatusService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var statuses = await _saleStatusService.GetAllAsync();
        return Ok(statuses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Details(int id)
    {
        var status = await _saleStatusService.GetByIdAsync(id);
        return Ok(status);
    }

    [HttpPost]
    public async Task<IActionResult> Create(SaleStatusDto dto)
    {
        var created = await _saleStatusService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(int id, SaleStatusEditDto dto)
    {
        var updated = await _saleStatusService.EditAsync(id, dto);
        return Ok(updated);
    }
}
