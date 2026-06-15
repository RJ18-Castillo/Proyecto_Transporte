using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using Transporte.Model;
using Transporte.DA;

namespace Transporte.BL
{
    public class GestorTransporte : IGestorTransporte
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public GestorTransporte(AppDbContext context)
        {
            _context = context;
        }

        public GestorTransporte(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public List<Pasajero> ListarPasajeros(string filtro)
        {
            var query = _context.Pasajero
        .Include(p => p.CedulaNavigation)
        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filtro))
            {
                query = query.Where(p =>
                    p.Nombre1.Contains(filtro) ||
                    p.Apellido1.Contains(filtro) ||
                    p.Cedula.Contains(filtro));
            }

            return query
                .OrderBy(p => p.Nombre1)
                .ThenBy(p => p.Apellido1)
                .ToList();
        }

        public Pasajero ObtenerPasajero(string cedula)
        {
            return _context.Pasajero.Find(cedula);
        }

        public void AgregarPasajero(Pasajero pasajero)
        {
            string clave = Guid.NewGuid().ToString().Substring(0, 8);

            var usuario = new Usuario
            {
                Cedula = pasajero.CedulaNavigation.Correo,
                NombreUsuario = pasajero.Cedula,
                Correo = pasajero.CedulaNavigation.Correo,
                Clave = clave,
                TipoUsuario = "Pasajero",
                IntentosFallidos = 0,
                Bloqueado = false,
                FechaRegistro = DateTime.Now
            };

            pasajero.CedulaNavigation = usuario;

            _context.Usuario.Add(usuario);
            _context.Pasajero.Add(pasajero);
            _context.SaveChanges();

            EnviarCorreo(
                pasajero.CedulaNavigation.Correo,
                "Creación de cuenta - Gestor de Transporte",
                $@"Estimado/a {pasajero.CedulaNavigation.Correo},

Su cuenta ha sido creada en el sistema Gestor de Transporte.

Credenciales de acceso:
- Usuario: {pasajero.CedulaNavigation.Correo}
- Contraseña temporal: {clave}

Por seguridad, se recomienda cambiar su contraseña al iniciar sesión.

Atentamente,
Sistema Gestor de Transporte

Este es un mensaje automático, por favor no responder."
            );
        }

        public void EditarPasajero(Pasajero pasajero)
        {
            var existente = _context.Pasajero.Find(pasajero.Cedula);

            if (existente != null)
            {
                existente.Nombre1 = pasajero.Nombre1;
                existente.Nombre2 = pasajero.Nombre2;
                existente.Apellido1 = pasajero.Apellido1;
                existente.Apellido2 = pasajero.Apellido2;

                _context.SaveChanges();
            }
        }

        private void EnviarCorreo(string correoDestino, string asunto, string cuerpo)
        {
            string host = _configuration["EmailSettings:Host"];
            int port = int.Parse(_configuration["EmailSettings:Port"]);
            string correo = _configuration["EmailSettings:User"];
            string password = _configuration["EmailSettings:Password"];

            var mensaje = new MimeMessage();

            mensaje.From.Add(
                new MailboxAddress("Gestor de Transporte", correo));

            mensaje.To.Add(MailboxAddress.Parse(correoDestino));
            mensaje.Subject = asunto;

            mensaje.Body = new TextPart("plain")
            {
                Text = cuerpo
            };

            using var smtp = new SmtpClient();

            smtp.Connect(host, port, SecureSocketOptions.StartTls);
            smtp.Authenticate(correo, password);
            smtp.Send(mensaje);
            smtp.Disconnect(true);
        }

        public List<Ruta> ListarRutas(string filtro)
        {
            var query = _context.Ruta.AsQueryable();

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
            return _context.Ruta.FirstOrDefault(ruta => ruta.Id == id);
        }

        public void AgregarRuta(Ruta ruta)
        {
            _context.Ruta.Add(ruta);
            _context.SaveChanges();
        }

        public void EditarRuta(Ruta ruta)
        {
            var existente = _context.Ruta.FirstOrDefault(r => r.Id == ruta.Id);

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
            return _context.Unidad
                .OrderBy(unidad => unidad.Placa)
                .ToList();
        }

        public Unidad ObtenerUnidad(int id)
        {
            return _context.Unidad.FirstOrDefault(unidad => unidad.Id == id);
        }

        public bool ExistePlaca(string placa, int idIgnorar = 0)
        {
            return _context.Unidad.Any(unidad =>
                unidad.Placa == placa &&
                unidad.Id != idIgnorar);
        }

        public bool AgregarUnidad(Unidad unidad)
        {
            if (ExistePlaca(unidad.Placa))
            {
                return false;
            }

            _context.Unidad.Add(unidad);
            _context.SaveChanges();

            return true;
        }

        public bool EditarUnidad(Unidad unidad)
        {
            if (ExistePlaca(unidad.Placa, unidad.Id))
            {
                return false;
            }

            var existente = _context.Unidad.FirstOrDefault(u => u.Id == unidad.Id);

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