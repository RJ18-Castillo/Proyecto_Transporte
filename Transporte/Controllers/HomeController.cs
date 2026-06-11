using Microsoft.AspNetCore.Mvc;

namespace Transporte.UI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}