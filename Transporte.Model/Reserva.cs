namespace Transporte.Model;

public partial class Reserva
{
    public int Id { get; set; }

    public int NumeroViaje { get; set; }

    public string CedulaPasajero { get; set; } = null!;

    public int NumeroAsiento { get; set; }

    public decimal MontoPagado { get; set; }

    public DateTime FechaHora { get; set; }

    public virtual Viaje Viaje { get; set; } = null!;

    public virtual Pasajero Pasajero { get; set; } = null!;
}
