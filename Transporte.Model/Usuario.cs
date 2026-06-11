namespace Transporte.Model;

public partial class Usuario
{
    public string Cedula { get; set; } = null!;

    public string NombreUsuario { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string Clave { get; set; } = null!;

    public string TipoUsuario { get; set; } = null!;

    public int IntentosFallidos { get; set; }

    public bool Bloqueado { get; set; }

    public DateTime FechaRegistro { get; set; }

    public virtual Chofer? Chofere { get; set; }

    public virtual Pasajero? Pasajero { get; set; }
}
