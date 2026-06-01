using Microsoft.EntityFrameworkCore;
using Transporte.DA;
using Transporte.Model;

namespace Transporte.BL
{
    public class GestorTransporte : IGestorTransporte
    {
        private readonly AppDbContext _context;

        public GestorTransporte(AppDbContext context)
        {
            _context = context;
        }

        public Usuario Login(string usuario, string clave)
        {
            Usuario user = _context.Usuarios.FirstOrDefault(user => user.NombreUsuario == usuario);

            if (user == null)
            {
                return null;
            }

            // VALIDAR BLOQUEO

            if (user.Rol != "Administrador")
            {
                if (user.Bloqueado)
                {
                    if (user.FechaBloqueo.HasValue)
                    {
                        DateTime desbloqueo = user.FechaBloqueo.Value.AddMinutes(3);

                        if (DateTime.Now < desbloqueo)
                        {
                            return null;
                        }
                        else
                        {
                            user.Bloqueado = false;

                            user.IntentosFallidos = 0;

                            _context.SaveChanges();
                        }
                    }
                }
            }

            // VALIDAR CLAVE

            if (user.Clave != clave)
            {
                if (user.Rol != "Administrador")
                {
                    user.IntentosFallidos++;

                    if (user.IntentosFallidos >= 2)
                    {
                        user.Bloqueado = true;

                        user.FechaBloqueo = DateTime.Now;
                    }

                    _context.SaveChanges();
                }

                return null;
            }

            // LOGIN CORRECTO

            user.IntentosFallidos = 0;

            user.Bloqueado = false;

            _context.SaveChanges();

            return user;
        }

        public bool CambiarClave(string usuario, string claveActual, string claveNueva)
        {
            Usuario user = _context.Usuarios.FirstOrDefault(user=> user.NombreUsuario == usuario);

            if (user == null)
            {
                return false;
            }

            if (user.Clave != claveActual)
            {
                return false;
            }

            user.Clave = claveNueva;

            _context.SaveChanges();

            return true;
        }

        public List<Chofer> ListarChoferes(string filtro)
        {
            IQueryable<Chofer> query = _context.Choferes;

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(chofer => chofer.Nombre.Contains(filtro));
            }

            return query.ToList();
        }

        public Chofer ObtenerChofer(int id)
        {
            return _context.Choferes.FirstOrDefault(chofer => chofer.Id == id);
        }

        public void AgregarChofer(Chofer chofer)
        {
            string claveTemporal = Guid.NewGuid().ToString().Substring(0, 8);

            Usuario usuario = new Usuario
                {
                    NombreUsuario = chofer.Correo,
                    Clave = claveTemporal,
                    Correo = chofer.Correo,
                    Rol = "Chofer"
                };

            _context.Usuarios.Add(usuario);

            _context.SaveChanges();

            chofer.UsuarioId = usuario.Id;

            _context.Choferes.Add(chofer);

            _context.SaveChanges();
        }

        public void EditarChofer(Chofer chofer)
        {
            Chofer existente = _context.Choferes.FirstOrDefault(choferConsultado =>
            choferConsultado.Id == chofer.Id);

            if (existente != null)
            {
                existente.Identificacion = chofer.Identificacion;

                existente.Nombre = chofer.Nombre;

                existente.Apellidos = chofer.Apellidos;

                _context.SaveChanges();
            }
        }

        public List<Pasajero> ListarPasajeros(string filtro)
        {
            IQueryable<Pasajero> query = _context.Pasajeros;

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(pasajero => pasajero.Nombre.Contains(filtro));
            }

            return query.ToList();
        }

        public Pasajero ObtenerPasajero(int id)
        {
            return _context.Pasajeros.FirstOrDefault(pasajero => pasajero.Id == id);
        }

        public void AgregarPasajero(Pasajero pasajero)
        {
            string claveTemporal = Guid.NewGuid().ToString().Substring(0, 8);

            Usuario usuario = new Usuario
            {
                NombreUsuario = pasajero.Correo,
                Clave = claveTemporal,
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
        }

        public void EditarPasajero(Pasajero pasajero)
        {
            Pasajero existente = _context.Pasajeros.FirstOrDefault(pasajeroConsultado =>
            pasajeroConsultado.Id == pasajero.Id);

            if (existente != null)
            {
                existente.Identificacion = pasajero.Identificacion;
                existente.Nombre = pasajero.Nombre;
                existente.Apellidos = pasajero.Apellidos;

                _context.SaveChanges();
            }
        }

        public List<Ruta> ListarRutas(string filtro)
        {
            IQueryable<Ruta> query = _context.Rutas;

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(ruta => ruta.Nombre.Contains(filtro) || ruta.Destino.Contains(filtro));
            }

            return query.ToList();
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
            Ruta existente = _context.Rutas.FirstOrDefault(rutaConsultada => rutaConsultada.Id == ruta.Id);

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
            return _context.Unidades.ToList();
        }

        public Unidad ObtenerUnidad(int id)
        {
            return _context.Unidades.FirstOrDefault(unidad => unidad.Id == id);
        }

        public bool ExistePlaca(string placa, int idIgnorar = 0)
        {
            return _context.Unidades.Any(unidad => unidad.Placa == placa && unidad.Id != idIgnorar);
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

            Unidad existente = _context.Unidades.FirstOrDefault(unidadConsultada =>
            unidadConsultada.Id == unidad.Id);

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
        public List<Viaje> ListarViajes(string filtro, DateTime? fecha)
        {
            var viajes = _context.Viajes
                .Select(v => new Viaje
                {
                    Id = v.Id,
                    RutaId = v.RutaId,
                    UnidadId = v.UnidadId,
                    ChoferId = v.ChoferId,
                    EstadoViajeId = v.EstadoViajeId,
                    FechaHoraSalida = v.FechaHoraSalida,
                    FechaHoraLlegadaEstimada = v.FechaHoraLlegadaEstimada,
                    MotivoCancelacion = v.MotivoCancelacion,
                    FechaCancelacion = v.FechaCancelacion
                })
                .ToList();

            foreach (var viaje in viajes)
            {
                var ruta = _context.Rutas.Find(viaje.RutaId);
                var unidad = _context.Unidades.Find(viaje.UnidadId);
                var chofer = _context.Choferes.Find(viaje.ChoferId);
                var estado = _context.EstadosViaje.Find(viaje.EstadoViajeId);

                viaje.NombreRuta = ruta != null ? ruta.Nombre : "";
                viaje.PlacaUnidad = unidad != null ? unidad.Placa : "";
                viaje.NombreChofer = chofer != null ? chofer.Nombre + " " + chofer.Apellidos : "";
                viaje.Estado = estado != null ? estado.Nombre : "";
            }

            if (!string.IsNullOrEmpty(filtro))
            {
                viajes = viajes
                    .Where(v =>
                        v.NombreRuta.Contains(filtro) ||
                        v.PlacaUnidad.Contains(filtro) ||
                        v.NombreChofer.Contains(filtro))
                    .ToList();
            }
            if (fecha.HasValue)
                {
                    viajes = viajes
                        .Where(v => v.FechaHoraSalida.Date == fecha.Value.Date)
                        .ToList();
                }
            return viajes;
        }

public Viaje? ObtenerViajePorId(int id)
{
    return _context.Viajes
        .Where(v => v.Id == id)
        .Select(v => new Viaje
        {
            Id = v.Id,
            RutaId = v.RutaId,
            UnidadId = v.UnidadId,
            ChoferId = v.ChoferId,
            EstadoViajeId = v.EstadoViajeId,
            FechaHoraSalida = v.FechaHoraSalida,
            FechaHoraLlegadaEstimada = v.FechaHoraLlegadaEstimada,
            MotivoCancelacion = v.MotivoCancelacion,
            FechaCancelacion = v.FechaCancelacion
        })
        .FirstOrDefault();
}

public void AgregarViaje(Viaje viaje)
{
    bool choferOcupado = _context.Viajes.Any(v =>
        v.ChoferId == viaje.ChoferId &&
        (v.EstadoViajeId == 1 || v.EstadoViajeId == 2) &&
        viaje.FechaHoraSalida < v.FechaHoraLlegadaEstimada &&
        viaje.FechaHoraLlegadaEstimada > v.FechaHoraSalida
    );

    bool unidadOcupada = _context.Viajes.Any(v =>
        v.UnidadId == viaje.UnidadId &&
        (v.EstadoViajeId == 1 || v.EstadoViajeId == 2) &&
        viaje.FechaHoraSalida < v.FechaHoraLlegadaEstimada &&
        viaje.FechaHoraLlegadaEstimada > v.FechaHoraSalida
    );

    if (choferOcupado)
        throw new Exception("El chofer ya tiene un viaje activo en ese rango de fechas.");

    if (unidadOcupada)
        throw new Exception("La unidad ya tiene un viaje activo en ese rango de fechas.");

    viaje.EstadoViajeId = 1;

    _context.Viajes.Add(viaje);

    _context.SaveChanges();
}

public void EditarViaje(Viaje viaje)
{
    var viajeBD = _context.Viajes.Find(viaje.Id);

    if (viajeBD == null)
        throw new Exception("El viaje no existe.");

    if (viajeBD.EstadoViajeId != 1)
        throw new Exception("Solo se pueden editar viajes en estado Programado.");

    viajeBD.RutaId = viaje.RutaId;
    viajeBD.UnidadId = viaje.UnidadId;
    viajeBD.ChoferId = viaje.ChoferId;
    viajeBD.EstadoViajeId = viaje.EstadoViajeId;
    viajeBD.FechaHoraSalida = viaje.FechaHoraSalida;
    viajeBD.FechaHoraLlegadaEstimada = viaje.FechaHoraLlegadaEstimada;

    _context.SaveChanges();
}

public void IniciarViaje(int id)
{
    var viaje = _context.Viajes.Find(id);

    if (viaje == null)
        throw new Exception("El viaje no existe.");

    if (viaje.EstadoViajeId != 1)
        throw new Exception("Solo se pueden iniciar viajes en estado Programado.");

    viaje.EstadoViajeId = 2;
    _context.SaveChanges();
}

        public void CancelarViaje(int id, string motivoCancelacion)
        {
            var viaje = _context.Viajes.Find(id);

            if (viaje == null)
                throw new Exception("El viaje no existe.");

            if (viaje.EstadoViajeId != 1)
                throw new Exception("Solo se pueden cancelar viajes en estado Programado.");

            viaje.EstadoViajeId = 4;
            viaje.MotivoCancelacion = motivoCancelacion;
            viaje.FechaCancelacion = DateTime.Now;

            _context.SaveChanges();
        }

        public List<Viaje> ListarViajesCancelados()
        {
            var viajesCancelados = _context.Viajes
                .Where(v => v.EstadoViajeId == 4) // 4 = Cancelado
                .Select(v => new Viaje
                {
                    Id = v.Id,
                    RutaId = v.RutaId,
                    UnidadId = v.UnidadId,
                    ChoferId = v.ChoferId,
                    EstadoViajeId = v.EstadoViajeId,
                    FechaHoraSalida = v.FechaHoraSalida,
                    MotivoCancelacion = v.MotivoCancelacion,
                    FechaCancelacion = v.FechaCancelacion
                })
                .ToList();

            foreach (var viaje in viajesCancelados)
            {
                var ruta = _context.Rutas.Find(viaje.RutaId);
                var unidad = _context.Unidades.Find(viaje.UnidadId);
                var chofer = _context.Choferes.Find(viaje.ChoferId);

                viaje.NombreRuta = ruta?.Nombre ?? "";
                viaje.PlacaUnidad = unidad?.Placa ?? "";
                viaje.NombreChofer = chofer != null ? $"{chofer.Nombre} {chofer.Apellidos}" : "";
                viaje.Estado = "Cancelado";
            }

            return viajesCancelados;
        }

        public void CompletarViaje(int id)
            {
                var viaje = _context.Viajes.Find(id);

                if (viaje == null)
                    throw new Exception("El viaje no existe.");

                // Solo viajes en curso
                if (viaje.EstadoViajeId != 2)
                    throw new Exception("Solo se pueden completar viajes en curso.");

                // 3 = Completado
                viaje.EstadoViajeId = 3;

                _context.SaveChanges();
        }

        public List<Reserva> ListarReservasPorPasajero(int pasajeroId)
        {
            return _context.Reservas
                .Where(r => r.PasajeroId == pasajeroId)
                .Include(r => r.Viaje)
                .ThenInclude(v => v.Ruta)
                .Include(r => r.Viaje.Unidad)
                .Include(r => r.Viaje.Chofer)
                .ToList();
        }

        public Reserva? ObtenerDetalleReserva(int reservaId)
        {
            return _context.Reservas
                .Include(r => r.Viaje)
                .ThenInclude(v => v.Ruta)
                .Include(r => r.Viaje.Unidad)
                .Include(r => r.Viaje.Chofer)
                .FirstOrDefault(r => r.Id == reservaId);
        }

    }
}