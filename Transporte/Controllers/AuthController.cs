using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Transporte.DA;
using Transporte.UI.Services;

namespace TicoBus.UI.Controllers;

public class AuthController : Controller
{
    private readonly AppDbContext _db;
    private readonly EmailService _email;
    private readonly IServiceScopeFactory _scopeFactory;

    public AuthController(AppDbContext db, EmailService email, IServiceScopeFactory scopeFactory)
    {
        _db = db;
        _email = email;
        _scopeFactory = scopeFactory;
    }

    public IActionResult Login() => View();

    [HttpPost]
    public async Task<IActionResult> Login(string nombreUsuario, string clave)
    {
        var usuario = await _db.Usuario
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

        if (usuario == null)
        {
            ViewBag.Error = "Usuario o clave incorrectos.";
            return View();
        }

        if (usuario.Bloqueado == true && usuario.TipoUsuario != "Administrador")
        {
            await _email.EnviarAsync(usuario.Correo,
                "Cuenta bloqueada",
                $"La cuenta {usuario.NombreUsuario} está bloqueada. Intente más tarde.");

            ViewBag.Error = "Su cuenta está bloqueada temporalmente.";
            return View();
        }

        if (usuario.Clave != clave)
        {
            if (usuario.TipoUsuario != "Administrador")
            {
                usuario.IntentosFallidos++;

                if (usuario.IntentosFallidos >= 2)
                {
                    usuario.Bloqueado = true;
                    var desbloqueo = DateTime.Now.AddMinutes(3);

                    await _db.SaveChangesAsync();

                    await _email.EnviarAsync(usuario.Correo,
                        "Cuenta bloqueada",
                        $"La cuenta {usuario.NombreUsuario} está bloqueada por 3 minutos. " +
                        $"Puede reintentar el {desbloqueo:dd/MM/yyyy} a las {desbloqueo:HH:mm}.");

                    _ = Task.Run(async () =>
                    {
                        await Task.Delay(TimeSpan.FromMinutes(3));
                        using var scope = _scopeFactory.CreateScope();
                        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                        var u = await db.Usuario.FindAsync(usuario.Cedula);
                        if (u != null)
                        {
                            u.Bloqueado = false;
                            u.IntentosFallidos = 0;
                            await db.SaveChangesAsync();
                        }
                    });
                }
                else
                {
                    await _db.SaveChangesAsync();
                }
            }

            ViewBag.Error = "Usuario o clave incorrectos.";
            return View();
        }

        // Login exitoso
        usuario.IntentosFallidos = 0;
        usuario.Bloqueado = false;
        await _db.SaveChangesAsync();

        var ahora = DateTime.Now;
        await _email.EnviarAsync(usuario.Correo,
            $"Inicio de sesión — {usuario.NombreUsuario}",
            $"Usted inició sesión el día {ahora:dd/MM/yyyy} a las {ahora:HH:mm}.");

        HttpContext.Session.SetString("Cedula", usuario.Cedula);
        HttpContext.Session.SetString("NombreUsuario", usuario.NombreUsuario);
        HttpContext.Session.SetString("Rol", usuario.TipoUsuario);

        return usuario.TipoUsuario switch
        {
            "Administrador" => RedirectToAction("Index", "Admin"),
            "Chofer" => RedirectToAction("Index", "Chofer"),
            "Pasajero" => RedirectToAction("Index", "Pasajero"),
            _ => RedirectToAction("Login")
        };
    }

    public IActionResult CambioClave() => View();

    [HttpPost]
    public async Task<IActionResult> CambioClave(string nombreUsuario, string claveActual, string nuevaClave)
    {
        var usuario = await _db.Usuario
            .FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario);

        if (usuario == null || usuario.Clave != claveActual)
        {
            ViewBag.Error = "Nombre de usuario o clave actual incorrectos.";
            return View();
        }

        usuario.Clave = nuevaClave;
        await _db.SaveChangesAsync();

        var ahora = DateTime.Now;
        await _email.EnviarAsync(usuario.Correo,
            $"Cambio de clave — {usuario.NombreUsuario}",
            $"La clave de su cuenta fue actualizada el día {ahora:dd/MM/yyyy} a las {ahora:HH:mm}.");

        ViewBag.Exito = "Clave actualizada correctamente.";
        return View();
    }

    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        return RedirectToAction("Login");
    }
}