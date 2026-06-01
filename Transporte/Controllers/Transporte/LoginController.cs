using Microsoft.AspNetCore.Mvc;
using Transporte.BL;

namespace Transporte.UI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IGestorTransporte _gestor;

        public LoginController(IGestorTransporte gestor)
        {
            _gestor = gestor;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/Transporte/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Index(string usuario, string clave)
        {
            var user = _gestor.Login(usuario, clave);

            if (user == null)
            {
                ViewBag.Error = "Usuario o clave incorrectos.";
                return View("~/Views/Transporte/Login.cshtml");
            }

            var correoService = new CorreoService();

            await correoService.EnviarInicioSesion(
                user.Correo,
                user.NombreUsuario);

            HttpContext.Session.SetInt32("UsuarioId", user.Id);
            HttpContext.Session.SetString("NombreUsuario", user.NombreUsuario);
            HttpContext.Session.SetString("Rol", user.Rol);

            if (user.Rol == "Administrador")
            {
                return RedirectToAction("Index", "Chofer");
            }

            if (user.Rol == "Chofer")
            {
                return RedirectToAction("Index", "Pasajero");
            }

            if (user.Rol == "Pasajero")
            {
                return RedirectToAction("Index", "MisViaje");
            }

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}