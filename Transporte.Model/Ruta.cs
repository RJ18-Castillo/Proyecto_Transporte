using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Transporte.Model
{
    public class Ruta
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Origen { get; set; }

        [Required]
        [StringLength(100)]
        public string Destino { get; set; }

        [Required]
        public TimeSpan DuracionEstimada { get; set; }

        [Required]
        public decimal PrecioBase { get; set; }
    }
}
