using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Transporte.BL;
using Transporte.Model;

namespace Transporte.UI.Controllers.Transporte
{
    public class PasajeroController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                context.Result = RedirectToAction("Index", "Login");
            }

            base.OnActionExecuting(context);
        }

        private readonly IGestorTransporte _gestor;

        public PasajeroController(IGestorTransporte gestor)
        {
            _gestor = gestor;
        }

        private bool EsChofer()
        {
            return HttpContext.Session.GetString("Rol") == "Chofer";
        }

        public IActionResult Index(string filtro)
        {
            if (!EsChofer())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            ViewBag.Filtro = filtro;
            var pasajeros = _gestor.ListarPasajeros(filtro);
            return View(pasajeros);
        }

        public IActionResult Create()
        {
            if (!EsChofer())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(Pasajero pasajero)
        {
            if (!EsChofer())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            if (!ModelState.IsValid)
            {
                return View(pasajero);
            }

            _gestor.AgregarPasajero(pasajero);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            if (!EsChofer())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            var pasajero = _gestor.ObtenerPasajero(id);

            if (pasajero == null)
            {
                return NotFound();
            }

            return View(pasajero);
        }

        [HttpPost]
        public IActionResult Edit(Pasajero pasajero)
        {
            if (!EsChofer())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            if (!ModelState.IsValid)
            {
                return View(pasajero);
            }

            _gestor.EditarPasajero(pasajero);
            return RedirectToAction("Index");
        }
    }
}
