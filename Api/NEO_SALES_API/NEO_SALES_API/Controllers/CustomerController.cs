using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Dtos;

namespace NEO_SALES_API.Controllers;

[Authorize]
[ApiController]
[Route("api/customer")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;

    public CustomerController(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var customers = await _customerService.GetAllAsync();
        return Ok(customers);
    }

    [HttpGet("{id}")]
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
    public async Task<IActionResult> Create(CustomerCreateDto dto)
    {
        var actor = User.FindFirstValue(ClaimTypes.Name) ?? "SISTEMA";
        var created = await _customerService.CreateAsync(dto, actor);
        return Ok(created);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Edit(Guid id, CustomerEditDto dto)
    {
        var actor = User.FindFirstValue(ClaimTypes.Name) ?? "SISTEMA";
        var updated = await _customerService.EditAsync(id, dto, actor);
        return Ok(updated);
    }
}
