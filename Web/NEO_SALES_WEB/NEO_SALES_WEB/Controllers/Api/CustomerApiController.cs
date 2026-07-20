using Microsoft.AspNetCore.Mvc;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES_WEB.Controllers.Api;

[ApiController]
[Route("bff/customer")]
public class CustomerApiController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerApiController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Details(Guid id)
    {
        var customer = await _customerService.GetByIdAsync(id);
        return Ok(customer);
    }

    [HttpGet("search/{term}")]
    public async Task<IActionResult> Search(string term)
    {
        var customers = await _customerService.SearchAsync(term);
        return Ok(customers);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CustomerCreateDto dto)
    {
        var created = await _customerService.CreateAsync(dto);
        return Ok(created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, [FromBody] CustomerEditDto dto)
    {
        var updated = await _customerService.EditAsync(id, dto);
        return Ok(updated);
    }
}
