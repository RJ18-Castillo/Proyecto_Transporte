using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Transporte.Model
{
    public class Unidad
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Placa { get; set; }

        [Required]
        [StringLength(100)]
        public string Modelo { get; set; }

        [Required]
        public int AnioFabricacion { get; set; }

        [Required]
        public int CapacidadPasajeros { get; set; }
    }
}
