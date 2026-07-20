using Microsoft.AspNetCore.Mvc;

namespace NEO_SALES_WEB.Controllers;

public class SaleController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Ventas";
        return View();
    }

    public IActionResult Details(Guid id)
    {
        ViewData["Title"] = "Detalle de venta";
        ViewBag.SaleId = id;
        return View();
    }
}
