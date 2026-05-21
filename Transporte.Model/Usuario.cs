using System.ComponentModel.DataAnnotations;

namespace Transporte.Model
{
    public class Usuario
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreUsuario { get; set; }

        [Required]
        [StringLength(100)]
        public string Clave { get; set; }

        [Required]
        [StringLength(150)]
        public string Correo { get; set; }

        [Required]
        [StringLength(50)]
        public string Rol { get; set; }

        public int IntentosFallidos { get; set; } = 0;

        public bool Bloqueado { get; set; } = false;

        public DateTime? FechaBloqueo { get; set; }
    }
}