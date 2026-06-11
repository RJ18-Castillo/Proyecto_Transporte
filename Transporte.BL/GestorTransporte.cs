using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.EntityFrameworkCore;
using Transporte.Model;

namespace Transporte.BL
{/*
    public class GestorTransporte : IGestorTransporte
    {
        private readonly AppDbContext _context;

        public GestorTransporte(AppDbContext context)
        {
            _context = context;
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
    }*/
}