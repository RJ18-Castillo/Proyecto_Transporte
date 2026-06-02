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
        public IActionResult Index(string usuario, string clave)
        {
            if (string.IsNullOrEmpty(usuario) || string.IsNullOrEmpty(clave))
            {
                ViewBag.Error = "Debe ingresar usuario y clave.";
                return View("~/Views/Transporte/Login.cshtml");
            }

            var user = _gestor.Login(usuario, clave);

            if (user == null)
            {
                ViewBag.Error = "Usuario o clave incorrectos o cuenta bloqueada.";
                return View("~/Views/Transporte/Login.cshtml");
            }

            HttpContext.Session.SetInt32("UsuarioId", user.Id);
            HttpContext.Session.SetString("NombreUsuario", user.NombreUsuario);
            HttpContext.Session.SetString("Rol", user.Rol);

            if (user.Rol == "Administrador")
                return RedirectToAction("Index", "Chofer");

            if (user.Rol == "Chofer")
                return RedirectToAction("Perfil", "Chofer");

            if (user.Rol == "Pasajero")
                return RedirectToAction("Index", "MisViaje");

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult CambiarClave()
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
                return RedirectToAction("Index");

            return View("~/Views/Transporte/CambiarClave.cshtml");
        }

        [HttpPost]
        public IActionResult CambiarClave(string claveActual, string claveNueva)
        {
            var usuario = HttpContext.Session.GetString("NombreUsuario");

            if (usuario == null)
                return RedirectToAction("Index");

            var resultado = _gestor.CambiarClave(usuario, claveActual, claveNueva);

            if (!resultado)
            {
                ViewBag.Error = "Clave actual incorrecta.";
                return View("~/Views/Transporte/CambiarClave.cshtml");
            }

            ViewBag.Mensaje = "Clave actualizada correctamente.";
            return View("~/Views/Transporte/CambiarClave.cshtml");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}