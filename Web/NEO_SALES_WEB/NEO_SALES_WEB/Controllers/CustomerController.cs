using Microsoft.AspNetCore.Mvc;
using NEO_SALES.CORE.Models.ViewModels;

namespace NEO_SALES_WEB.Controllers;

public class CustomerController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Clientes";
        return View();
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Nuevo cliente";
        return View(new CustomerFormViewModel());
    }

    public IActionResult Edit(Guid id)
    {
        ViewData["Title"] = "Editar cliente";
        var model = new CustomerFormViewModel { Id = id };
        return View(model);
    }
}
