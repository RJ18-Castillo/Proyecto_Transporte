using Microsoft.AspNetCore.Mvc;
using Transporte.BL;  
using Transporte.Model;

namespace Transporte.UI.Controllers
{
    public class ReservasController : Controller
    {
        private readonly IGestorTransporte _gestor;

        
        public ReservasController(IGestorTransporte gestor)
        {
            _gestor = gestor;
        }

        public IActionResult MisReservas()
        {
            int pasajeroId = ObtenerUsuarioActualId();
            var reservas = _gestor.ListarReservasPorPasajero(pasajeroId);
            return View(reservas);
        }

        public IActionResult DetalleReserva(int id)
        {
            var reserva = _gestor.ObtenerDetalleReserva(id);
            if (reserva == null)
                return NotFound();

            return View(reserva);
        }

        
        private int ObtenerUsuarioActualId()
        {
        
            return int.Parse(User.FindFirst("PasajeroId").Value);
        }
    }
}

