using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Transporte.Model;

public partial class Unidad
{
    public string Placa { get; set; } = null!;

    public string Modelo { get; set; } = null!;

    public short AnoFabricacion { get; set; }

    public short Capacidad { get; set; }

    public virtual ICollection<Viaje> Viajes { get; set; } = new List<Viaje>();
}
