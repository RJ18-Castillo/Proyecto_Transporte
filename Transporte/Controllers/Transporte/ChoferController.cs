using Microsoft.AspNetCore.Mvc;
using Transporte.BL;
using Transporte.Model;

namespace Transporte.UI.Controllers
{
    public class ChoferController : Controller
    {
        private readonly IGestorTransporte _gestor;

        public ChoferController(IGestorTransporte gestor)
        {
            _gestor = gestor;
        }

        private bool EsAdministrador()
        {
            return HttpContext.Session.GetString("Rol") == "Administrador";
        }

        public IActionResult Index(string filtro)
        {
            if (!EsAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            var lista = _gestor.ListarChoferes(filtro);

            return View("~/Views/Transporte/Chofer.cshtml", lista);
        }

        public IActionResult Create()
        {
            if (!EsAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            return View("~/Views/Transporte/CrearChofer.cshtml");
        }

        [HttpPost]
        public IActionResult Create(Chofer chofer)
        {
            if (!EsAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            if (!ModelState.IsValid)
            {
                return View( "~/Views/Transporte/CrearChofer.cshtml", chofer);
            }

            _gestor.AgregarChofer(chofer);

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (!EsAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            var chofer = _gestor.ObtenerChofer(id);

            if (chofer == null)
            {
                return NotFound();
            }

            return View("~/Views/Transporte/EditChofer.cshtml", chofer);
        }

        [HttpPost]
        public IActionResult Edit(Chofer chofer)
        {
            if (!EsAdministrador())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            if (!ModelState.IsValid)
            {
                return View("~/Views/Transporte/EditChofer.cshtml", chofer);
            }

            _gestor.EditarChofer(chofer);

            return RedirectToAction("Index");
        }
    }
}