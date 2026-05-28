using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Transporte.BL;
using Transporte.Model;

namespace Transporte.UI.Controllers
{
    public class ViajeController : Controller
    {
        private readonly IGestorTransporte _gestor;

        public ViajeController(IGestorTransporte gestor)
        {
            _gestor = gestor;
        }

        public IActionResult Index(string filtro, DateTime? fecha)
            {
                ViewBag.Filtro = filtro;
                ViewBag.Fecha = fecha?.ToString("yyyy-MM-dd");

                var viajes = _gestor.ListarViajes(filtro, fecha);

                return View(viajes);
            }

        public IActionResult Create()
        {
            CargarListas();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Viaje viaje)
        {
            try
            {
                _gestor.AgregarViaje(viaje);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException != null
                ? ex.InnerException.Message
                : ex.Message;
                CargarListas();
                return View(viaje);
            }
        }

        public IActionResult Edit(int id)
        {
            var viaje = _gestor.ObtenerViajePorId(id);

            if (viaje == null)
                return NotFound();

            CargarListas();
            return View(viaje);
        }

        [HttpPost]
        public IActionResult Edit(Viaje viaje)
        {
            try
            {
                _gestor.EditarViaje(viaje);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                 ViewBag.Error = ex.InnerException != null
                ? ex.InnerException.Message
                : ex.Message;
                CargarListas();
                return View(viaje);
            }
        }

        public IActionResult Iniciar(int id)
        {
            _gestor.IniciarViaje(id);
            return RedirectToAction("Index");
        }

        public IActionResult Cancelar(int id)
        {
            var viaje = _gestor.ObtenerViajePorId(id);

            if (viaje == null)
                return NotFound();

            return View(viaje);
        }

        [HttpPost]
public IActionResult Cancelar(int id, string motivoCancelacion)
{
    if (string.IsNullOrWhiteSpace(motivoCancelacion))
    {
        ViewBag.Error = "Debe ingresar un motivo de cancelación.";

        var viaje = _gestor.ObtenerViajePorId(id);

        if (viaje == null)
            return NotFound();

        return View(viaje);
    }

    try
    {
        _gestor.CancelarViaje(id, motivoCancelacion);
        return RedirectToAction("Index");
    }
    catch (Exception ex)
    {
        ViewBag.Error = ex.Message;

        var viaje = _gestor.ObtenerViajePorId(id);

        if (viaje == null)
            return NotFound();

        return View(viaje);
    }
}
       private void CargarListas()
            {
                var rutas = _gestor.ListarRutas("");
                var unidades = _gestor.ListarUnidades();
                var choferes = _gestor.ListarChoferes("");

                ViewBag.Rutas = new SelectList(rutas, "Id", "Nombre");
                ViewBag.Unidades = new SelectList(unidades, "Id", "Placa");
                ViewBag.Choferes = new SelectList(choferes, "Id", "Nombre");

            }
        public IActionResult Completar(int id)
            {
                _gestor.CompletarViaje(id);

                return RedirectToAction("Index");
            }
        
    }
}
