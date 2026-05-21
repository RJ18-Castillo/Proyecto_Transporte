using Microsoft.AspNetCore.Mvc;
using Transporte.BL;
using Transporte.Model;

namespace Transporte.UI.Controllers
{
    public class ChoferController : Controller
    {
        private readonly IGestorTransporte _gestor;

        public ChoferController(
            IGestorTransporte gestor)
        {
            _gestor = gestor;
        }

        public IActionResult Index(
            string filtro)
        {
            var lista =
                _gestor.ListarChoferes(
                    filtro);

            return View(
                "~/Views/Transporte/Chofer.cshtml",
                lista);
        }

        public IActionResult Create()
        {
            return View(
                "~/Views/Transporte/CrearChofer.cshtml");
        }

        [HttpPost]
        public IActionResult Create(
            Chofer chofer)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/Transporte/CrearChofer.cshtml",
                    chofer);
            }

            _gestor.AgregarChofer(
                chofer);

            return RedirectToAction(
                "Index");
        }

        public IActionResult Edit(
            int id)
        {
            var chofer =
                _gestor.ObtenerChofer(id);

            return View(
                "~/Views/Transporte/EditChofer.cshtml",
                chofer);
        }

        [HttpPost]
        public IActionResult Edit(
            Chofer chofer)
        {
            if (!ModelState.IsValid)
            {
                return View(
                    "~/Views/Transporte/EditChofer.cshtml",
                    chofer);
            }

            _gestor.EditarChofer(
                chofer);

            return RedirectToAction(
                "Index");
        }
    }
}