using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transporte.DA;
using Transporte.Model;

namespace Transporte.UI.Controllers.Transporte
{
    public class RutasController : Controller
    {
        private readonly AppDbContext _db;

        public RutasController(AppDbContext db)
        {
            _db = db;
        }

        private bool TienePermiso()
        {
            var rol = HttpContext.Session.GetString("Rol");
            return rol == "Chofer" || rol == "Administrador";
        }

        public async Task<IActionResult> Index(string filtro)
        {
            if (!TienePermiso())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            ViewBag.Filtro = filtro;

            var rutas = _db.Ruta.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                rutas = rutas.Where(r =>
                    r.Nombre.Contains(filtro) ||
                    r.Destino.Contains(filtro));
            }

            var lista = await rutas
                .OrderBy(r => r.Nombre)
                .ToListAsync();

            return View(lista);
        }

        [HttpGet]
        public IActionResult Agregar()
        {
            if (!TienePermiso())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(
            string nombre,
            string origen,
            string destino,
            string duracionEstimada,
            decimal precioBase)
        {
            if (!TienePermiso())
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                if (!TimeSpan.TryParse(duracionEstimada, out TimeSpan duracion))
                {
                    ViewBag.Error = "La duración estimada debe tener formato hh:mm. Ejemplo: 02:30";
                    return View();
                }

                var ruta = new Ruta
                {
                    Nombre = nombre.Trim(),
                    Origen = origen.Trim(),
                    Destino = destino.Trim(),
                    DuracionEstimada = duracion,
                    PrecioBase = precioBase
                };

                _db.Ruta.Add(ruta);
                await _db.SaveChangesAsync();

                TempData["Exito"] = "Ruta agregada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException?.Message ?? ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            if (!TienePermiso())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            var ruta = await _db.Ruta.FindAsync(id);

            if (ruta == null)
            {
                return NotFound();
            }

            return View(ruta);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(
            int id,
            string nombre,
            string origen,
            string destino,
            string duracionEstimada,
            decimal precioBase)
        {
            if (!TienePermiso())
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var ruta = await _db.Ruta.FindAsync(id);

                if (ruta == null)
                {
                    return NotFound();
                }

                if (!TimeSpan.TryParse(duracionEstimada, out TimeSpan duracion))
                {
                    ViewBag.Error = "La duración estimada debe tener formato hh:mm. Ejemplo: 02:30";
                    return View(ruta);
                }

                ruta.Nombre = nombre.Trim();
                ruta.Origen = origen.Trim();
                ruta.Destino = destino.Trim();
                ruta.DuracionEstimada = duracion;
                ruta.PrecioBase = precioBase;

                await _db.SaveChangesAsync();

                TempData["Exito"] = "Ruta actualizada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException?.Message ?? ex.Message;

                var ruta = await _db.Ruta.FindAsync(id);
                return View(ruta);
            }
        }
        [HttpPost]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!TienePermiso())
            {
                return RedirectToAction("Login", "Auth");
            }

            var ruta = await _db.Ruta.FindAsync(id);

            if (ruta == null)
            {
                return NotFound();
            }

            _db.Ruta.Remove(ruta);
            await _db.SaveChangesAsync();

            TempData["Exito"] = "Ruta eliminada correctamente.";
            return RedirectToAction("Index");
        }
    }
}