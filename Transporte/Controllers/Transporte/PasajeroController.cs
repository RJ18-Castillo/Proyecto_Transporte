using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Transporte.BL;
using Transporte.DA;
using Transporte.Model;
using Transporte.UI.Services;

namespace Transporte.UI.Controllers.Transporte
{
    public class PasajeroController : Controller
    {
        //public override void OnActionExecuting(ActionExecutingContext context)
        //{
        //    if (HttpContext.Session.GetInt32("UsuarioId") == null)
        //    {
        //        context.Result = RedirectToAction("Index", "Auth");
        //    }

        //    base.OnActionExecuting(context);
        //}

        private readonly IGestorTransporte _gestor;
        private readonly AppDbContext _db;
        private readonly EmailService _email;

        public PasajeroController(
        IGestorTransporte gestor,
        AppDbContext db,
        EmailService email)
        {
            _gestor = gestor;
            _db = db;
            _email = email;
        }

        private bool EsChofer()
        {
            return HttpContext.Session.GetString("Rol") == "Chofer";
        }

        public IActionResult Index(string filtro)
        {
            if (!EsChofer())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            ViewBag.Filtro = filtro;
            var pasajeros = _gestor.ListarPasajeros(filtro);
            return View(pasajeros);
        }

        [HttpGet]
        public IActionResult Agregar()
        {
            if (!EsChofer())
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Agregar(
    string cedula,
    string nombre1,
    string? nombre2,
    string apellido1,
    string? apellido2,
    string correo)
        {
            if (!EsChofer())
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                if (await _db.Usuario.AnyAsync(u => u.Cedula == cedula))
                {
                    ViewBag.Error = "Ya existe un usuario con esa identificación.";
                    return View();
                }

                if (await _db.Usuario.AnyAsync(u => u.Correo == correo))
                {
                    ViewBag.Error = "Ya existe un usuario con ese correo electrónico.";
                    return View();
                }

                var clave = GenerarClave();

                var usuario = new Usuario
                {
                    Cedula = cedula,
                    NombreUsuario = cedula,
                    Correo = correo,
                    Clave = clave,
                    TipoUsuario = "Pasajero",
                    IntentosFallidos = 0,
                    Bloqueado = false,
                    FechaRegistro = DateTime.Now
                };

                var pasajero = new Pasajero
                {
                    Cedula = cedula,
                    Nombre1 = nombre1,
                    Nombre2 = nombre2,
                    Apellido1 = apellido1,
                    Apellido2 = apellido2
                };

                _db.Usuario.Add(usuario);
                _db.Pasajero.Add(pasajero);

                await _db.SaveChangesAsync();

                await _email.EnviarAsync(
                    correo,
                    "Bienvenido a TicoBus — Sus credenciales de acceso",
                    $"Hola {nombre1} {apellido1},\n\n" +
                    $"Su cuenta ha sido creada en el sistema TicoBus.\n" +
                    $"Usuario: {cedula}\n" +
                    $"Clave: {clave}\n\n" +
                    $"Por seguridad, cambie su clave al iniciar sesión.");

                TempData["Exito"] = "Pasajero agregado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error =
                ex.Message + "<br><br>" +
                ex.InnerException?.Message;

                return View();
            }
        }

        [HttpGet]
        public IActionResult Editar(string cedula)
        {
            if (!EsChofer())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            var pasajero = _db.Pasajero
                .Include(p => p.CedulaNavigation)
                .FirstOrDefault(p => p.Cedula == cedula);

            if (pasajero == null)
            {
                return NotFound();
            }

            return View(pasajero);
        }

        [HttpPost]
        public async Task<IActionResult> Editar(
            string cedula,
            string nombre1,
            string? nombre2,
            string apellido1,
            string? apellido2)
        {
            if (!EsChofer())
            {
                ViewBag.Mensaje = "No posee permisos para realizar esta acción.";
                return View("~/Views/Transporte/SinPermisos.cshtml");
            }

            try
            {
                var pasajero = await _db.Pasajero.FindAsync(cedula);

                if (pasajero == null)
                {
                    return NotFound();
                }

                pasajero.Nombre1 = nombre1;
                pasajero.Nombre2 = nombre2;
                pasajero.Apellido1 = apellido1;
                pasajero.Apellido2 = apellido2;

                await _db.SaveChangesAsync();

                TempData["Exito"] = "Pasajero actualizado correctamente.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.InnerException?.Message ?? ex.Message;

                var pasajero = _db.Pasajero
                    .Include(p => p.CedulaNavigation)
                    .FirstOrDefault(p => p.Cedula == cedula);

                return View(pasajero);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Eliminar(string cedula)
        {
            if (!EsChofer())
            {
                return RedirectToAction("Login", "Auth");
            }

            var tieneReservas = await _db.Reserva.AnyAsync(r => r.CedulaPasajero == cedula);

            if (tieneReservas)
            {
                TempData["Error"] = "No se puede eliminar el pasajero porque tiene reservas registradas.";
                return RedirectToAction("Index");
            }

            var pasajero = await _db.Pasajero.FindAsync(cedula);
            var usuario = await _db.Usuario.FindAsync(cedula);

            if (pasajero != null)
            {
                _db.Pasajero.Remove(pasajero);
            }

            if (usuario != null)
            {
                _db.Usuario.Remove(usuario);
            }

            await _db.SaveChangesAsync();

            TempData["Exito"] = "Pasajero eliminado correctamente.";
            return RedirectToAction("Index");
        }
        private static string GenerarClave()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnpqrstuvwxyz23456789!@#$";
            var random = new Random();
            return new string(Enumerable.Range(0, 10)
                .Select(_ => chars[random.Next(chars.Length)])
                .ToArray());
        }
    }
}
