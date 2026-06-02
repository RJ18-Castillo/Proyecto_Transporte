using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
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

        // ================= SEGURIDAD =================
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (HttpContext.Session.GetInt32("UsuarioId") == null)
            {
                context.Result = RedirectToAction("Index", "Login");
            }

            base.OnActionExecuting(context);
        }

        private bool EsAdministrador()
        {
            return HttpContext.Session.GetString("Rol") == "Administrador";
        }

        // ================= LISTADO =================
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

        // ================= CREAR =================
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
                return View("~/Views/Transporte/CrearChofer.cshtml", chofer);
            }

            _gestor.AgregarChofer(chofer);

            return RedirectToAction("Index");
        }

        // ================= EDITAR =================
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

        // ================= PERFIL DEL CHOFER =================
        public IActionResult Perfil()
        {
            var usuarioId = HttpContext.Session.GetInt32("UsuarioId");

            if (usuarioId == null)
                return RedirectToAction("Index", "Login");

            var chofer = _gestor
                .ListarChoferes("")
                .FirstOrDefault(c => c.UsuarioId == usuarioId);

            if (chofer == null)
            {
                ViewBag.Mensaje = "No se encontró el perfil del chofer.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            return View("~/Views/Transporte/PerfilChofer.cshtml", chofer);
        }
    }
}