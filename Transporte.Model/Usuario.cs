namespace Transporte.Model
{
    public class Usuario
    {
        public int Id { get; set; }

        public string NombreUsuario { get; set; }

        public string Clave { get; set; }

        public string Correo { get; set; }

        public string Rol { get; set; }

        public int IntentosFallidos { get; set; } = 0;

        public bool Bloqueado { get; set; } = false;

        public DateTime? FechaBloqueo { get; set; }
    }
}