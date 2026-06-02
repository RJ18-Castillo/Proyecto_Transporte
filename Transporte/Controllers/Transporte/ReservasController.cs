using Microsoft.AspNetCore.Mvc;
using Transporte.BL;


namespace Transporte.UI.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IGestorTransporte _gestor;

        public ReservasController(IGestorTransporte gestor)
        {
            _gestor = gestor;
        }

     
        private int? ObtenerPasajeroActualId()
        {
            var claim = User?.FindFirst("PasajeroId");
            if (claim == null) return null;

            if (int.TryParse(claim.Value, out int id))
                return id;

            return null;
        }

       
        public IActionResult MisReservas()
        {
            var pasajeroId = ObtenerPasajeroActualId();

            if (!pasajeroId.HasValue)
            {
                TempData["Error"] = "Debe iniciar sesión como pasajero para ver sus reservas.";
                return RedirectToAction("Index", "Login");
            }

            var reservas = _gestor.ListarReservasPorPasajero(pasajeroId.Value);
            return View(reservas);
        }
    }
}

