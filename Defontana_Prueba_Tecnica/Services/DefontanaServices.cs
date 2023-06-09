using Defontana_Prueba_Tecnica.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Defontana_Prueba_Tecnica.Services
{
    internal class DefontanaServices
    {
        public async Task Run()
        {
            using (var db = new DefontanaDbContext())
            {
                // Obtener la fecha de hace 30 días
                var fechaInicio = DateTime.Now.Date.AddDays(-30);
                              
                // Obtener las ventas de los últimos 30 días con todas las entidades relacionadas
                var ventasUltimos30Dias = await db.Venta
                    .Include(v => v.VentaDetalles)
                        .ThenInclude(vd => vd.IdProductoNavigation)
                            .ThenInclude(p => p.IdMarcaNavigation)
                    .Include(v => v.IdLocalNavigation)
                    .Where(v => v.Fecha >= fechaInicio)
                    .ToListAsync();

                // El total de ventas de los últimos 30 días (monto total y cantidad de ventas)
                var montoTotalVentas = ventasUltimos30Dias.Sum(v => v.Total);
                var cantidadVentas = ventasUltimos30Dias.Count;

                Console.WriteLine("Total de ventas en los últimos 30 días:");
                Console.WriteLine($"Monto total: {montoTotalVentas}");
                Console.WriteLine($"Cantidad de ventas: {cantidadVentas}");
                Console.WriteLine();

                // El día y hora en que se realizó la venta con el monto más alto
                var ventaMasAlta = ventasUltimos30Dias.OrderByDescending(v => v.Total).FirstOrDefault();

                Console.WriteLine("Venta con el monto más alto:");
                Console.WriteLine($"Día y hora: {ventaMasAlta.Fecha}");
                Console.WriteLine($"Monto: {ventaMasAlta.Total}");
                Console.WriteLine();

                // El producto con mayor monto total de ventas
                var productoMayorVenta = ventasUltimos30Dias
                    .SelectMany(v => v.VentaDetalles)
                    .GroupBy(vd => vd.IdProducto)
                    .OrderByDescending(g => g.Sum(vd => vd.TotalLinea))
                    .Select(g => g.Key)
                    .FirstOrDefault();

                var nombreProductoMayorVenta = ventasUltimos30Dias
                    .SelectMany(v => v.VentaDetalles)
                    .Where(vd => vd.IdProducto == productoMayorVenta)
                    .Select(vd => vd.IdProductoNavigation.Nombre)
                    .FirstOrDefault();

                Console.WriteLine("Producto con mayor monto total de ventas:");
                Console.WriteLine($"Producto: {nombreProductoMayorVenta}");
                Console.WriteLine();


                // El local con mayor monto de ventas
                var localMayorVenta = ventasUltimos30Dias
                    .GroupBy(v => v.IdLocal)
                    .OrderByDescending(g => g.Sum(v => v.Total))
                    .Select(g => g.Key)
                    .FirstOrDefault();

                var nombreLocalMayorVenta = ventasUltimos30Dias
                    .Where(v => v.IdLocal == localMayorVenta)
                    .Select(v => v.IdLocalNavigation.Nombre)
                    .FirstOrDefault();

                Console.WriteLine("Local con mayor monto de ventas:");
                Console.WriteLine($"Local: {nombreLocalMayorVenta}");
                Console.WriteLine();

                // Obtener la marca con mayor margen de ganancias
                var marcaMayorMargen = ventasUltimos30Dias
                    .SelectMany(v => v.VentaDetalles)
                    .GroupBy(vd => vd.IdProductoNavigation.IdMarca)
                    .Select(g => new
                    {
                        MarcaId = g.Key,
                        MargenGanancias = g.Sum(vd => vd.IdProductoNavigation.CostoUnitario - vd.PrecioUnitario) // Cálculo del margen de ganancias
                    })
                    .OrderByDescending(m => m.MargenGanancias)
                    .FirstOrDefault();

                var marca = marcaMayorMargen != null
                    ? ventasUltimos30Dias
                        .SelectMany(v => v.VentaDetalles)
                        .FirstOrDefault(vd => vd.IdProductoNavigation.IdMarca == marcaMayorMargen.MarcaId)
                        ?.IdProductoNavigation?.IdMarcaNavigation.Nombre
                    : null;

                Console.WriteLine("Marca con mayor margen de ganancias:");
                Console.WriteLine($"Marca: {marca}");
                Console.WriteLine();

                // El producto que más se vende en cada local
                var productosMasVendidos = ventasUltimos30Dias
                    .SelectMany(v => v.VentaDetalles)
                    .GroupBy(vd => new { vd.IdVentaNavigation.IdLocal, vd.IdProducto })
                    .OrderByDescending(g => g.Sum(vd => vd.Cantidad))
                    .Select(g => new { g.Key.IdLocal, g.Key.IdProducto, TotalCantidad = g.Sum(vd => vd.Cantidad) })
                    .ToList();

                foreach (var producto in productosMasVendidos)
                {
                    var nombreProducto = ventasUltimos30Dias
                        .SelectMany(v => v.VentaDetalles)
                        .Where(vd => vd.IdProducto == producto.IdProducto)
                        .Select(vd => vd.IdProductoNavigation.Nombre)
                        .FirstOrDefault();

                    var nombreLocal = ventasUltimos30Dias
                        .Where(v => v.IdLocal == producto.IdLocal)
                        .Select(v => v.IdLocalNavigation.Nombre)
                        .FirstOrDefault();

                    Console.WriteLine($"Producto más vendido en el local {nombreLocal}:");
                    Console.WriteLine($"Producto: {nombreProducto}");
                    Console.WriteLine($"Cantidad vendida: {producto.TotalCantidad}");
                    Console.WriteLine("-----------------------------");


                }
            }
        }
    }
}