using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using Transporte.DA;
using Transporte.Model;

namespace Transporte.BL
{
    public class GestorTransporte : IGestorTransporte
    {
        private readonly AppDbContext _context;

        private const string CorreoEmisor = "transportesticobus@gmail.com";
        private const string ClaveAplicacion = "crlteimnkhukmgqh";

        public GestorTransporte(AppDbContext context)
        {
            _context = context;
        }

        public Usuario Login(string usuario, string clave)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == usuario);

            if (user == null)
                return null;

            if (user.Bloqueado)
            {
                if (user.FechaBloqueo.HasValue &&
                    DateTime.Now < user.FechaBloqueo.Value.AddMinutes(3))
                    return null;

                user.Bloqueado = false;
                user.IntentosFallidos = 0;
            }

            if (user.Clave != clave)
            {
                user.IntentosFallidos++;

                if (user.IntentosFallidos >= 2)
                {
                    user.Bloqueado = true;
                    user.FechaBloqueo = DateTime.Now;
                }

                _context.SaveChanges();
                return null;
            }

            user.IntentosFallidos = 0;
            user.Bloqueado = false;

            _context.SaveChanges();

            // 🔥 CORREO PROFESIONAL LOGIN
            EnviarCorreo(
                user.Correo,
                "Seguridad - Inicio de sesión detectado",
                $@"Estimado/a {user.NombreUsuario},

Le informamos que se ha realizado un inicio de sesión en su cuenta del sistema Gestor de Transporte.

Detalles del acceso:
- Fecha: {DateTime.Now:dd/MM/yyyy}
- Hora: {DateTime.Now:HH:mm:ss}

Si usted reconoce esta actividad, no es necesario realizar ninguna acción.

En caso contrario, le recomendamos cambiar su contraseña de inmediato y comunicarse con el administrador del sistema.

Atentamente,
Sistema Gestor de Transporte

Este es un mensaje automático, por favor no responder."
            );

            return user;
        }

        public bool CambiarClave(string usuario, string claveActual, string claveNueva)
        {
            var user = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == usuario);

            if (user == null || user.Clave != claveActual)
                return false;

            user.Clave = claveNueva;

            _context.SaveChanges();

            // 🔥 CORREO CAMBIO DE CLAVE
            EnviarCorreo(
                user.Correo,
                $"Cambio de clave — {user.NombreUsuario}",
                $@"Estimado/a {user.NombreUsuario},

Le confirmamos que la contraseña de su cuenta ha sido actualizada correctamente.

Detalles del cambio:
- Fecha: {DateTime.Now:dd/MM/yyyy}
- Hora: {DateTime.Now:HH:mm:ss}

Si usted no realizó este cambio, le recomendamos comunicarse de inmediato con el administrador del sistema.

Atentamente,
Sistema Gestor de Transporte

Este es un mensaje automático, por favor no responder."
            );

            return true;
        }

        public List<Chofer> ListarChoferes(string filtro)
        {
            var query = _context.Choferes.AsQueryable();

            if (!string.IsNullOrEmpty(filtro))
                query = query.Where(c => c.Nombre.Contains(filtro));

            return query.ToList();
        }

        public Chofer ObtenerChofer(int id)
        {
            return _context.Choferes.Find(id);
        }

        public void AgregarChofer(Chofer chofer)
        {
            string clave = Guid.NewGuid().ToString().Substring(0, 8);

            var usuario = new Usuario
            {
                NombreUsuario = chofer.Correo,
                Clave = clave,
                Correo = chofer.Correo,
                Rol = "Chofer",
                IntentosFallidos = 0,
                Bloqueado = false
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            chofer.UsuarioId = usuario.Id;

            _context.Choferes.Add(chofer);
            _context.SaveChanges();

            // 🔥 CORREO CON CREDENCIALES
            EnviarCorreo(
                chofer.Correo,
                "Creación de cuenta - Gestor de Transporte",
                $@"Estimado/a {chofer.Nombre},

Su cuenta ha sido creada en el sistema Gestor de Transporte.

Credenciales de acceso:
- Usuario: {chofer.Correo}
- Contraseña temporal: {clave}

Por seguridad, se recomienda cambiar su contraseña al iniciar sesión.

Atentamente,
Sistema Gestor de Transporte

Este es un mensaje automático, por favor no responder."
            );
        }

        public void EditarChofer(Chofer chofer)
        {
            var existente = _context.Choferes.Find(chofer.Id);

            if (existente != null)
            {
                existente.Identificacion = chofer.Identificacion;
                existente.Nombre = chofer.Nombre;
                existente.Apellidos = chofer.Apellidos;
                existente.Correo = chofer.Correo;

                _context.SaveChanges();
            }
        }

        public List<Pasajero> ListarPasajeros(string filtro)
        {
            var query = _context.Pasajeros.AsQueryable();

            if (!string.IsNullOrEmpty(filtro))
                query = query.Where(p => p.Nombre.Contains(filtro));

            return query.ToList();
        }

        public Pasajero ObtenerPasajero(int id)
        {
            return _context.Pasajeros.Find(id);
        }

        public void AgregarPasajero(Pasajero pasajero)
        {
            string clave = Guid.NewGuid().ToString().Substring(0, 8);

            var usuario = new Usuario
            {
                NombreUsuario = pasajero.Correo,
                Clave = clave,
                Correo = pasajero.Correo,
                Rol = "Pasajero",
                IntentosFallidos = 0,
                Bloqueado = false,
                FechaBloqueo = null
            };

            _context.Usuarios.Add(usuario);
            _context.SaveChanges();

            pasajero.UsuarioId = usuario.Id;

            _context.Pasajeros.Add(pasajero);
            _context.SaveChanges();

            EnviarCorreo(
                pasajero.Correo,
                "Creación de cuenta - Gestor de Transporte",
                $@"Estimado/a {pasajero.Nombre},

Su cuenta ha sido creada en el sistema Gestor de Transporte.

Credenciales de acceso:
- Usuario: {pasajero.Correo}
- Contraseña temporal: {clave}

Por seguridad, se recomienda cambiar su contraseña al iniciar sesión.

Atentamente,
Sistema Gestor de Transporte

Este es un mensaje automático, por favor no responder."
            );
        }

        public void EditarPasajero(Pasajero pasajero)
        {
            var existente = _context.Pasajeros.Find(pasajero.Id);

            if (existente != null)
            {
                existente.Identificacion = pasajero.Identificacion;
                existente.Nombre = pasajero.Nombre;
                existente.Apellidos = pasajero.Apellidos;

                _context.SaveChanges();
            }
        }

        private void EnviarCorreo(string correoDestino, string asunto, string cuerpo)
        {
            var mensaje = new MimeMessage();

            mensaje.From.Add(new MailboxAddress("Gestor de Transporte", CorreoEmisor));
            mensaje.To.Add(MailboxAddress.Parse(correoDestino));
            mensaje.Subject = asunto;

            mensaje.Body = new TextPart("plain") { Text = cuerpo };

            using var smtp = new SmtpClient();

            smtp.Connect("smtp.gmail.com", 587, SecureSocketOptions.StartTls);
            smtp.Authenticate(CorreoEmisor, ClaveAplicacion);
            smtp.Send(mensaje);
            smtp.Disconnect(true);
        }

        public List<Ruta> ListarRutas(string filtro)
        {
            var query = _context.Rutas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(ruta =>
                    ruta.Nombre.Contains(filtro) ||
                    ruta.Destino.Contains(filtro));
            }

            return query
                .OrderBy(ruta => ruta.Nombre)
                .ToList();
        }

        public Ruta ObtenerRuta(int id)
        {
            return _context.Rutas.FirstOrDefault(ruta => ruta.Id == id);
        }

        public void AgregarRuta(Ruta ruta)
        {
            _context.Rutas.Add(ruta);
            _context.SaveChanges();
        }

        public void EditarRuta(Ruta ruta)
        {
            var existente = _context.Rutas.FirstOrDefault(r => r.Id == ruta.Id);

            if (existente != null)
            {
                existente.Nombre = ruta.Nombre;
                existente.Origen = ruta.Origen;
                existente.Destino = ruta.Destino;
                existente.DuracionEstimada = ruta.DuracionEstimada;
                existente.PrecioBase = ruta.PrecioBase;

                _context.SaveChanges();
            }
        }

        public List<Unidad> ListarUnidades()
        {
            return _context.Unidades
                .OrderBy(unidad => unidad.Placa)
                .ToList();
        }

        public Unidad ObtenerUnidad(int id)
        {
            return _context.Unidades.FirstOrDefault(unidad => unidad.Id == id);
        }

        public bool ExistePlaca(string placa, int idIgnorar = 0)
        {
            return _context.Unidades.Any(unidad =>
                unidad.Placa == placa &&
                unidad.Id != idIgnorar);
        }

        public bool AgregarUnidad(Unidad unidad)
        {
            if (ExistePlaca(unidad.Placa))
            {
                return false;
            }

            _context.Unidades.Add(unidad);
            _context.SaveChanges();

            return true;
        }

        public bool EditarUnidad(Unidad unidad)
        {
            if (ExistePlaca(unidad.Placa, unidad.Id))
            {
                return false;
            }

            var existente = _context.Unidades.FirstOrDefault(u => u.Id == unidad.Id);

            if (existente == null)
            {
                return false;
            }

            existente.Placa = unidad.Placa;
            existente.Modelo = unidad.Modelo;
            existente.AnioFabricacion = unidad.AnioFabricacion;
            existente.CapacidadPasajeros = unidad.CapacidadPasajeros;

            _context.SaveChanges();

            return true;
        }

        public List<Viaje> ListarViajes(string filtro, DateTime? fecha) => new();
        public Viaje? ObtenerViajePorId(int id) => null;
        public void AgregarViaje(Viaje viaje) { }
        public void EditarViaje(Viaje viaje) { }
        public void IniciarViaje(int id) { }
        public void CancelarViaje(int id, string motivoCancelacion) { }
        public void CompletarViaje(int id) { }

        public List<Viaje> ListarViajesCancelados() => new();
        public List<Reserva> ListarReservasPorPasajero(int pasajeroId) => new();
        public Reserva? ObtenerDetalleReserva(int reservaId) => null;
    }
}