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

        public IActionResult Index(string filtro)
        {
            ViewBag.Filtro = filtro;
            var rutas = _gestor.ListarRutas(filtro);
            return View(rutas);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Ruta ruta)
        {
            if (!ModelState.IsValid)
            {
                return View(ruta);
            }

            _gestor.AgregarRuta(ruta);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
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
            if (!ModelState.IsValid)
            {
                return View(ruta);
            }

            _gestor.EditarRuta(ruta);
            return RedirectToAction("Index");
        }
    }
}
