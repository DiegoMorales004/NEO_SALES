using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NEO_SALES.CORE.Interfaces.Services;
using NEO_SALES.CORE.Models.Dtos.Auth;

namespace NEO_SALES_API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public IActionResult Login(LoginRequestDto request)
    {
        var response = _authService.Login(request);

        return response is null ? Unauthorized() : Ok(response);
    }
}
