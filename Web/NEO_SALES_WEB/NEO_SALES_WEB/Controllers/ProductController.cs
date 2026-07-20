using Microsoft.AspNetCore.Mvc;
using NEO_SALES.CORE.Models.ViewModels;

namespace NEO_SALES_WEB.Controllers;

public class ProductController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Productos";
        return View();
    }

    public IActionResult Create()
    {
        ViewData["Title"] = "Nuevo producto";
        return View(new ProductFormViewModel());
    }

    public IActionResult Edit(Guid id)
    {
        ViewData["Title"] = "Editar producto";
        var model = new ProductFormViewModel { Id = id };
        return View(model);
    }
}
