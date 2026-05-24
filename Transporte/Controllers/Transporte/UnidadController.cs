using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Transporte.BL;
using Transporte.Model;

namespace Transporte.UI.Controllers.Transporte
{
    public class UnidadController : Controller
    {
        private readonly IGestorTransporte _gestor;

        public UnidadController(IGestorTransporte gestor)
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

        public IActionResult Index()
        {
            if (!EsChoferOAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            var unidades = _gestor.ListarUnidades();
            return View(unidades);
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
        public IActionResult Create(Unidad unidad)
        {
            if (!EsChoferOAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            if (!ModelState.IsValid)
            {
                return View(unidad);
            }

            bool registrado = _gestor.AgregarUnidad(unidad);

            if (!registrado)
            {
                ModelState.AddModelError("Placa", "Ya existe una unidad registrada con esta placa.");
                return View(unidad);
            }

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (!EsChoferOAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            var unidad = _gestor.ObtenerUnidad(id);

            if (unidad == null)
            {
                return NotFound();
            }

            return View(unidad);
        }

        [HttpPost]
        public IActionResult Edit(Unidad unidad)
        {
            if (!EsChoferOAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            if (!ModelState.IsValid)
            {
                return View(unidad);
            }

            bool actualizado = _gestor.EditarUnidad(unidad);

            if (!actualizado)
            {
                ModelState.AddModelError("Placa", "Ya existe otra unidad registrada con esta placa.");
                return View(unidad);
            }

            return RedirectToAction("Index");
        }
    }
}
