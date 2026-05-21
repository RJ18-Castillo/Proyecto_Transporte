using Microsoft.AspNetCore.Mvc;
using Transporte.BL;

namespace Transporte.UI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IGestorTransporte _gestor;

        public LoginController(
            IGestorTransporte gestor)
        {
            _gestor = gestor;
        }

        public IActionResult Index()
        {
            return View("~/Views/Transporte/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Index(
            string usuario,
            string clave)
        {
            var user =
                _gestor.Login(
                    usuario,
                    clave);

            if (user == null)
            {
                ViewBag.Error =
                    "Credenciales inválidas";

                return View(
                    "~/Views/Transporte/Login.cshtml");
            }

            return RedirectToAction(
                "Index",
                "Chofer");
        }
    }
}