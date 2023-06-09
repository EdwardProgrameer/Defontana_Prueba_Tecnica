using System;
using System.Collections.Generic;

namespace Defontana_Prueba_Tecnica.Models
{
    public partial class Local
    {
        public Local()
        {
            Venta = new HashSet<Venta>();
        }

        public long IdLocal { get; set; }
        public string Nombre { get; set; } = null!;
        public string Direccion { get; set; } = null!;

        public virtual ICollection<Venta> Venta { get; set; }
    }
}
