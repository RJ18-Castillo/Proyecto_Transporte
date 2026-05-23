using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Transporte.BL;
using Transporte.Model;

namespace Transporte.UI.Controllers.Transporte
{
    public class PasajeroController : Controller
    {
        private readonly IGestorTransporte _gestor;

        public PasajeroController(IGestorTransporte gestor)
        {
            _gestor = gestor;
        }

        public IActionResult Index(string filtro)
        {
            ViewBag.Filtro = filtro;
            var pasajeros = _gestor.ListarPasajeros(filtro);
            return View(pasajeros);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Pasajero pasajero)
        {
            if (!ModelState.IsValid)
            {
                return View(pasajero);
            }

            _gestor.AgregarPasajero(pasajero);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
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
            if (!ModelState.IsValid)
            {
                return View(pasajero);
            }

            _gestor.EditarPasajero(pasajero);
            return RedirectToAction("Index");
        }
    }
}
