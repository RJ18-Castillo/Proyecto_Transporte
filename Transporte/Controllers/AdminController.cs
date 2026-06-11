using Microsoft.AspNetCore.Mvc;

namespace Transporte.UI.Controllers;

public class AdminController : Controller
{
    public IActionResult Index() => View();
}