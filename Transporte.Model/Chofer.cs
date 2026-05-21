using System.ComponentModel.DataAnnotations;

namespace Transporte.Model
{
    public class Chofer
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Identificacion { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [Required]
        [StringLength(150)]
        public string Correo { get; set; }

        public int UsuarioId { get; set; }

        public Usuario? Usuario { get; set; }
    }
}