using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Transporte.BL;
using Transporte.Model;

namespace Transporte.UI.Controllers.Transporte
{
    public class UnidadeController : Controller
    {
        private readonly IGestorTransporte _gestor;

        public UnidadeController(IGestorTransporte gestor)
        {
            _gestor = gestor;
        }

        public IActionResult Index()
        {
            var unidades = _gestor.ListarUnidades();
            return View(unidades);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Unidad unidad)
        {
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
