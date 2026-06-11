using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transporte.DA;
using Transporte.Model;
using Transporte.UI.Services;

namespace Transporte.UI.Controllers;

public class ChoferesController : Controller
{
    private readonly AppDbContext _db;
    private readonly EmailService _email;

    public ChoferesController(AppDbContext db, EmailService email)
    {
        _db = db;
        _email = email;
    }

    private bool EsAdmin() =>
        HttpContext.Session.GetString("Rol") == "Administrador";

    public async Task<IActionResult> Index(string? filtro)
    {
        if (!EsAdmin()) return RedirectToAction("Login", "Auth");

        var choferes = _db.Chofer
            .Include(c => c.CedulaNavigation)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(filtro))
            choferes = choferes.Where(c =>
                c.Nombre1.Contains(filtro) ||
                c.Apellido1.Contains(filtro) ||
                (c.Nombre2 != null && c.Nombre2.Contains(filtro)) ||
                (c.Apellido2 != null && c.Apellido2.Contains(filtro)));

        ViewBag.Filtro = filtro;
        return View(await choferes.ToListAsync());
    }

    [HttpGet]
    public IActionResult Agregar()
    {
        if (!EsAdmin()) return RedirectToAction("Login", "Auth");
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Agregar(string cedula, string nombre1,
        string? nombre2, string apellido1, string? apellido2, string correo)
    {
        if (!EsAdmin()) return RedirectToAction("Login", "Auth");

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
            TipoUsuario = "Chofer",
            IntentosFallidos = 0,
            Bloqueado = false,
            FechaRegistro = DateTime.Now
        };

        var chofer = new Chofer
        {
            Cedula = cedula,
            Nombre1 = nombre1,
            Nombre2 = nombre2,
            Apellido1 = apellido1,
            Apellido2 = apellido2
        };

        _db.Usuario.Add(usuario);
        _db.Chofer.Add(chofer);
        await _db.SaveChangesAsync();

        await _email.EnviarAsync(correo,
            "Bienvenido a TicoBus — Sus credenciales de acceso",
            $"Hola {nombre1} {apellido1},\n\n" +
            $"Su cuenta ha sido creada en el sistema TicoBus.\n" +
            $"Usuario: {cedula}\n" +
            $"Clave: {clave}\n\n" +
            $"Por seguridad, cambie su clave al iniciar sesión.");

        TempData["Exito"] = "Chofer agregado correctamente.";
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Editar(string id)
    {
        if (!EsAdmin()) return RedirectToAction("Login", "Auth");

        var chofer = await _db.Chofer.FindAsync(id);
        if (chofer == null) return NotFound();

        return View(chofer);
    }

    [HttpPost]
    public async Task<IActionResult> Editar(string id, string nombre1,
        string? nombre2, string apellido1, string? apellido2)
    {
        if (!EsAdmin()) return RedirectToAction("Login", "Auth");

        var chofer = await _db.Chofer.FindAsync(id);
        if (chofer == null) return NotFound();

        chofer.Nombre1 = nombre1;
        chofer.Nombre2 = nombre2;
        chofer.Apellido1 = apellido1;
        chofer.Apellido2 = apellido2;

        await _db.SaveChangesAsync();

        TempData["Exito"] = "Chofer actualizado correctamente.";
        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Eliminar(string cedula)
    {
        if (!EsAdmin()) return RedirectToAction("Login", "Auth");

        var tieneViajes = await _db.Viaje.AnyAsync(v => v.CedulaChofer == cedula);
        if (tieneViajes)
        {
            TempData["Error"] = "No se puede eliminar un chofer que tiene viajes registrados.";
            return RedirectToAction("Index");
        }

        var chofer = await _db.Chofer.FindAsync(cedula);
        var usuario = await _db.Usuario.FindAsync(cedula);

        if (chofer != null) _db.Chofer.Remove(chofer);
        if (usuario != null) _db.Usuario.Remove(usuario);

        await _db.SaveChangesAsync();

        TempData["Exito"] = "Chofer eliminado correctamente.";
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