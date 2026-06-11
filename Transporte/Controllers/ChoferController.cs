using Microsoft.AspNetCore.Mvc;

namespace Transporte.Controllers;

public class ChoferController : Controller
{
    public IActionResult Index() => View();
}