namespace Transporte.Model.Entities
{
    public class Chofer
    {
        public int Id { get; set; }

        public string Identificacion { get; set; }

        public string Nombre { get; set; }

        public string Apellidos { get; set; }

        public string Correo { get; set; }

        public int UsuarioId { get; set; }

        public Usuario Usuario { get; set; }
    }
}