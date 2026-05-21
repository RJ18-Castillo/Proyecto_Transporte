using Transporte.DA;
using Transporte.Model;

namespace Transporte.BL
{
    public class GestorTransporte
        : IGestorTransporte
    {
        private readonly AppDbContext _context;

        public GestorTransporte(
            AppDbContext context)
        {
            _context = context;
        }

        public Usuario Login(
            string usuario,
            string clave)
        {
            Usuario user =
                _context.Usuarios
                .FirstOrDefault(u =>
                    u.NombreUsuario == usuario);

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
                        DateTime desbloqueo =
                            user.FechaBloqueo.Value
                            .AddMinutes(3);

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

                        user.FechaBloqueo =
                            DateTime.Now;
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

        public bool CambiarClave(
            string usuario,
            string claveActual,
            string claveNueva)
        {
            Usuario user =
                _context.Usuarios
                .FirstOrDefault(u =>
                    u.NombreUsuario == usuario);

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

        public List<Chofer> ListarChoferes(
            string filtro)
        {
            IQueryable<Chofer> query =
                _context.Choferes;

            if (!string.IsNullOrEmpty(filtro))
            {
                query = query.Where(c =>
                    c.Nombre.Contains(filtro));
            }

            return query.ToList();
        }

        public Chofer ObtenerChofer(
            int id)
        {
            return _context.Choferes
                .FirstOrDefault(c =>
                    c.Id == id);
        }

        public void AgregarChofer(
            Chofer chofer)
        {
            string claveTemporal =
                Guid.NewGuid()
                .ToString()
                .Substring(0, 8);

            Usuario usuario =
                new Usuario
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

        public void EditarChofer(
            Chofer chofer)
        {
            Chofer existente =
                _context.Choferes
                .FirstOrDefault(c =>
                    c.Id == chofer.Id);

            if (existente != null)
            {
                existente.Identificacion =
                    chofer.Identificacion;

                existente.Nombre =
                    chofer.Nombre;

                existente.Apellidos =
                    chofer.Apellidos;

                _context.SaveChanges();
            }
        }
    }
}