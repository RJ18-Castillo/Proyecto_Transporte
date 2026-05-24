using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Transporte.BL;
using Transporte.Model;

namespace Transporte.UI.Controllers.Transporte
{
    public class RutaController : Controller
    {
        private readonly IGestorTransporte _gestor;

        public RutaController(IGestorTransporte gestor)
        {
            _gestor = gestor;
        }

        private bool EsChoferOAdministrador()
        {
            if (HttpContext.Session.GetString("Rol") == "Chofer" ||
                HttpContext.Session.GetString("Rol") == "Administrador")
            {
                return true;
            }
            return false;
        }

        public IActionResult Index(string filtro)
        {
            if (!EsChoferOAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            ViewBag.Filtro = filtro;
            var rutas = _gestor.ListarRutas(filtro);
            return View(rutas);
        }

        public IActionResult Create()
        {
            if (!EsChoferOAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(Ruta ruta)
        {
            if (!EsChoferOAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            if (!ModelState.IsValid)
            {
                return View(ruta);
            }

            _gestor.AgregarRuta(ruta);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (!EsChoferOAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            var ruta = _gestor.ObtenerRuta(id);

            if (ruta == null)
            {
                return NotFound();
            }

            return View(ruta);
        }

        [HttpPost]
        public IActionResult Edit(Ruta ruta)
        {
            if (!EsChoferOAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            if (!ModelState.IsValid)
            {
                return View(ruta);
            }

            _gestor.EditarRuta(ruta);
            return RedirectToAction("Index");
        }
    }
}
