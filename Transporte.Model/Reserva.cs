namespace Transporte.Model
{
    public class Reserva
    {
        public int Id { get; set; }

       
        public int PasajeroId { get; set; }
        public Pasajero Pasajero { get; set; }

        
        public int ViajeId { get; set; }
        public Viaje Viaje { get; set; }

        
        public int NumeroAsiento { get; set; }
        public decimal MontoPagado { get; set; }

        public string Estado { get; set; } = "Activa";
    }
}

