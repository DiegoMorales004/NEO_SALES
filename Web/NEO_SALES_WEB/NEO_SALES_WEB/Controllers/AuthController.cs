using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using NEO_SALES.CORE.Models.Configuration;
using NEO_SALES.CORE.Models.ViewModels;

namespace NEO_SALES_WEB.Controllers;

[Route("Auth")]
public class AuthController : Controller
{
    private readonly ApiConfiguration _apiConfiguration;

    public AuthController(IOptions<ApiConfiguration> apiConfiguration)
    {
        _apiConfiguration = apiConfiguration.Value;
    }

    [HttpGet("Login")]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewData["Title"] = "Iniciar sesión";
        ViewData["ReturnUrl"] = string.IsNullOrWhiteSpace(returnUrl) ? "/" : returnUrl;

        var model = new LoginViewModel
        {
            DefaultUserHint = _apiConfiguration.DefaultCredentials.User,
            DefaultPasswordHint = _apiConfiguration.DefaultCredentials.Password
        };

        return View(model);
    }
}
