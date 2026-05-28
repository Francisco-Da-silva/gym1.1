USE GYM_DB;
GO

DECLARE @Anio INT = 2026;

/* Revisa primero que pagos se van a borrar. */
SELECT
    p.IdPago,
    c.IdCliente,
    c.Nombre,
    c.Apellido,
    c.DNI,
    p.FechaPago,
    p.FechaDesde,
    p.FechaHasta,
    p.Monto,
    p.Observacion
FROM dbo.Pagos p
INNER JOIN dbo.Clientes c ON c.IdCliente = p.IdCliente
WHERE
    (c.Nombre = 'Francisco' OR c.Apellido = 'Francisco')
    AND (
        (p.FechaDesde >= DATEFROMPARTS(@Anio, 1, 1) AND p.FechaDesde < DATEFROMPARTS(@Anio, 4, 1))
        OR (p.FechaPago >= DATEFROMPARTS(@Anio, 1, 1) AND p.FechaPago < DATEFROMPARTS(@Anio, 4, 1))
    )
ORDER BY p.FechaDesde, p.FechaPago;
GO

DECLARE @Anio INT = 2026;

DELETE p
FROM dbo.Pagos p
INNER JOIN dbo.Clientes c ON c.IdCliente = p.IdCliente
WHERE
    (c.Nombre = 'Francisco' OR c.Apellido = 'Francisco')
    AND (
        (p.FechaDesde >= DATEFROMPARTS(@Anio, 1, 1) AND p.FechaDesde < DATEFROMPARTS(@Anio, 4, 1))
        OR (p.FechaPago >= DATEFROMPARTS(@Anio, 1, 1) AND p.FechaPago < DATEFROMPARTS(@Anio, 4, 1))
    );
GO

/* Verificacion: no deberian quedar pagos de Enero, Febrero o Marzo para Francisco. */
DECLARE @Anio INT = 2026;

SELECT
    p.IdPago,
    c.IdCliente,
    c.Nombre,
    c.Apellido,
    c.DNI,
    p.FechaPago,
    p.FechaDesde,
    p.FechaHasta,
    p.Monto,
    p.Observacion
FROM dbo.Pagos p
INNER JOIN dbo.Clientes c ON c.IdCliente = p.IdCliente
WHERE
    (c.Nombre = 'Francisco' OR c.Apellido = 'Francisco')
    AND (
        (p.FechaDesde >= DATEFROMPARTS(@Anio, 1, 1) AND p.FechaDesde < DATEFROMPARTS(@Anio, 4, 1))
        OR (p.FechaPago >= DATEFROMPARTS(@Anio, 1, 1) AND p.FechaPago < DATEFROMPARTS(@Anio, 4, 1))
    )
ORDER BY p.FechaDesde, p.FechaPago;
GO
