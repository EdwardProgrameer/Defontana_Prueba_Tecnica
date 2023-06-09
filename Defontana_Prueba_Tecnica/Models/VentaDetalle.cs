using System;
using System.Collections.Generic;

namespace Defontana_Prueba_Tecnica.Models
{
    public partial class VentaDetalle
    {
        public long IdVentaDetalle { get; set; }
        public long IdVenta { get; set; }
        public int PrecioUnitario { get; set; }
        public int Cantidad { get; set; }
        public int TotalLinea { get; set; }
        public long IdProducto { get; set; }

        public virtual Producto IdProductoNavigation { get; set; } = null!;
        public virtual Venta IdVentaNavigation { get; set; } = null!;
    }
}
