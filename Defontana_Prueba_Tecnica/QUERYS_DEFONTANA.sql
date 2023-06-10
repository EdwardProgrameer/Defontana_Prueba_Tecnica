


DECLARE @fechaInicio DATE = DATEADD(DAY, -30, GETDATE());

-- El total de ventas de los últimos 30 días (monto total y cantidad de ventas)
SELECT SUM(Total) AS MontoTotal, COUNT(*) AS CantidadVentas
FROM Venta
WHERE Fecha >= @fechaInicio;

-- El día y hora en que se realizó la venta con el monto más alto
SELECT TOP 1 Fecha, Total
FROM Venta
WHERE Fecha >= @fechaInicio
ORDER BY Total DESC;

-- El producto con mayor monto total de ventas
SELECT TOP 1 P.Nombre, SUM(V.TotalLinea) AS MontoTotalVentas
FROM VentaDetalle V
INNER JOIN Producto P ON P.ID_Producto = V.ID_Producto
WHERE V.ID_Venta IN (SELECT ID_Venta FROM Venta WHERE Fecha >= @fechaInicio)
GROUP BY P.Nombre
ORDER BY MontoTotalVentas DESC;

-- El local con mayor monto de ventas
SELECT TOP 1 L.Nombre, SUM(Total) AS MontoTotalVentas
FROM Venta V
INNER JOIN Local L ON L.ID_Local = V.ID_Local
WHERE Fecha >= @fechaInicio
GROUP BY L.Nombre
ORDER BY MontoTotalVentas DESC;

-- Obtener la marca con mayor margen de ganancias
SELECT TOP 1 M.Nombre, SUM(Precio_Unitario - Costo_Unitario) AS MargenGanancias
FROM VentaDetalle VD
INNER JOIN Producto P ON VD.ID_Producto = P.ID_Producto
INNER JOIN Marca M ON M.ID_Marca = P.ID_Marca
WHERE VD.ID_Venta IN (SELECT V.ID_Venta FROM Venta V WHERE Fecha >= @fechaInicio)
GROUP BY M.Nombre
ORDER BY MargenGanancias DESC;

-- El producto que más se vende en cada local

SELECT 
    NombreLocal,
    NombreProducto,
    SUM(TotalVendido) AS TotalVendido
FROM
    (
    SELECT
        l.Nombre AS NombreLocal,
        p.Nombre AS NombreProducto,
        SUM(vd.Cantidad) AS TotalVendido,
        ROW_NUMBER() OVER (PARTITION BY l.Nombre ORDER BY SUM(vd.Cantidad) DESC) AS RN
    FROM
        Venta v
        INNER JOIN VentaDetalle vd ON v.ID_Venta = vd.ID_Venta
        INNER JOIN Producto p ON vd.ID_Producto = p.ID_Producto
        INNER JOIN Marca m ON p.ID_Marca = m.ID_Marca
        INNER JOIN Local l ON v.ID_Local = l.ID_Local
    WHERE
        v.Fecha >= @fechaInicio
    GROUP BY
        l.Nombre,
        p.Nombre
    ) AS subquery
WHERE
    RN = 1
GROUP BY
    NombreLocal,
    NombreProducto;
