using System;
using System.Collections.Generic;

namespace Defontana_Prueba_Tecnica.Models
{
    public partial class Venta
    {
        public Venta()
        {
            VentaDetalles = new HashSet<VentaDetalle>();
        }

        public long IdVenta { get; set; }
        public int Total { get; set; }
        public DateTime Fecha { get; set; }
        public long IdLocal { get; set; }

        public virtual Local IdLocalNavigation { get; set; } = null!;
        public virtual ICollection<VentaDetalle> VentaDetalles { get; set; }
    }
}
