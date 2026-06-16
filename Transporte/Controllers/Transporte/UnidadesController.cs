using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transporte.DA;
using Transporte.Model;

namespace Transporte.UI.Controllers.Transporte
{
    public class UnidadesController : Controller
    {
        private readonly AppDbContext _db;

        public UnidadesController(AppDbContext db)
        {
            _db = db;
        }

        private bool TienePermiso()
        {
            var rol = HttpContext.Session.GetString("Rol");
            return rol == "Chofer" || rol == "Administrador";
        }

        public async Task<IActionResult> Index()
        {
            if (!TienePermiso())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            var unidades = await _db.Unidad
                .OrderBy(u => u.Placa)
                .ToListAsync();

            return View(unidades);
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
            string placa,
            string modelo,
            short anoFabricacion,
            short capacidad)
        {
            if (!TienePermiso())
            {
                return RedirectToAction("Login", "Auth");
            }

            placa = placa.Trim().ToUpper();
            modelo = modelo.Trim();

            try
            {
                if (await _db.Unidad.AnyAsync(u => u.Placa == placa))
                {
                    ViewBag.Error = "Ya existe una unidad registrada con esa placa.";
                    return View();
                }

                var anoActual = DateTime.Now.Year;

                if (anoFabricacion < 1900 || anoFabricacion > anoActual)
                {
                    ViewBag.Error = $"El año de fabricación debe estar entre 1900 y {anoActual}.";
                    return View();
                }

                if (capacidad <= 0)
                {
                    ViewBag.Error = "La capacidad debe ser mayor a cero.";
                    return View();
                }

                var unidad = new Unidad
                {
                    Placa = placa,
                    Modelo = modelo,
                    AnoFabricacion = anoFabricacion,
                    Capacidad = capacidad
                };

                _db.Unidad.Add(unidad);
                await _db.SaveChangesAsync();

                TempData["Exito"] = "Unidad agregada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException?.Message ?? ex.Message;
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(string placa)
        {
            if (!TienePermiso())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            var unidad = await _db.Unidad.FindAsync(placa);

            if (unidad == null)
            {
                return NotFound();
            }

            return View(unidad);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(
            string placa,
            string modelo,
            short anoFabricacion,
            short capacidad)
        {
            if (!TienePermiso())
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var unidad = await _db.Unidad.FindAsync(placa);

                if (unidad == null)
                {
                    return NotFound();
                }

                var anoActual = DateTime.Now.Year;

                if (anoFabricacion < 1900 || anoFabricacion > anoActual)
                {
                    ViewBag.Error = $"El año de fabricación debe estar entre 1900 y {anoActual}.";
                    return View(unidad);
                }

                if (capacidad <= 0)
                {
                    ViewBag.Error = "La capacidad debe ser mayor a cero.";
                    return View(unidad);
                }

                unidad.Modelo = modelo.Trim();
                unidad.AnoFabricacion = anoFabricacion;
                unidad.Capacidad = capacidad;

                await _db.SaveChangesAsync();

                TempData["Exito"] = "Unidad actualizada correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException?.Message ?? ex.Message;

                var unidad = await _db.Unidad.FindAsync(placa);
                return View(unidad);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(string placa)
        {
            if (!TienePermiso())
            {
                return RedirectToAction("Login", "Auth");
            }

            var unidad = await _db.Unidad.FindAsync(placa);

            if (unidad == null)
            {
                return NotFound();
            }

            _db.Unidad.Remove(unidad);
            await _db.SaveChangesAsync();

            TempData["Exito"] = "Unidad eliminada correctamente.";
            return RedirectToAction("Index");
        }
    }
}