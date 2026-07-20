using Microsoft.AspNetCore.Mvc;
using NEO_SALES.CORE.Interfaces.Services;

namespace NEO_SALES_WEB.Controllers.Api;

[ApiController]
[Route("bff/sale-status")]
public class SaleStatusApiController : ControllerBase
{
    private readonly ISaleStatusService _saleStatusService;

    public SaleStatusApiController(ISaleStatusService saleStatusService)
    {
        _saleStatusService = saleStatusService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var statuses = await _saleStatusService.GetAllAsync();
        return Ok(statuses);
    }
}
