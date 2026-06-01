using System.ComponentModel.DataAnnotations.Schema;

namespace Transporte.Model
{
    public class Viaje
    {
        public int Id { get; set; }

        public int RutaId { get; set; }
        public int UnidadId { get; set; }
        public int ChoferId { get; set; }
        public int EstadoViajeId { get; set; }

        // Navegación a entidades relacionadas (necesarias para Include/ThenInclude)
        public Ruta? Ruta { get; set; }
        public Unidad? Unidad { get; set; }
        public Chofer? Chofer { get; set; }
        public EstadoViaje? EstadoViaje { get; set; }

        public DateTime FechaHoraSalida { get; set; }
        public DateTime FechaHoraLlegadaEstimada { get; set; }

        public string? MotivoCancelacion { get; set; }
        public DateTime? FechaCancelacion { get; set; }

        [NotMapped]
        public string? NombreRuta { get; set; }

        [NotMapped]
        public string? PlacaUnidad { get; set; }

        [NotMapped]
        public string? NombreChofer { get; set; }

        [NotMapped]
        public string? Estado { get; set; }
    }
}
