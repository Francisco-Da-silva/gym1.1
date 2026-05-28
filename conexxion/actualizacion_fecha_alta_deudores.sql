USE GYM_DB;
GO

/* 1) Agrega FechaAlta a Clientes si todavia no existe.
   Para clientes existentes, SQL Server cargara la fecha de hoy como valor inicial. */
IF COL_LENGTH('dbo.Clientes', 'FechaAlta') IS NULL
BEGIN
    ALTER TABLE dbo.Clientes
    ADD FechaAlta DATE NOT NULL
        CONSTRAINT DF_Clientes_FechaAlta DEFAULT (CONVERT(date, GETDATE()));
END
GO

/* 2) Opcional: si queres corregir clientes ya existentes con una fecha real,
   descomentá y ajustá estas lineas antes de ejecutar.

UPDATE dbo.Clientes
SET FechaAlta = '2026-01-01'
WHERE DNI = '12345678';

UPDATE dbo.Clientes
SET FechaAlta = '2026-02-15'
WHERE IdCliente = 1;
*/
GO

/* 3) Recrea el procedimiento de deudores para que devuelva FechaAlta. */
CREATE OR ALTER PROCEDURE dbo.Listar_Deudores
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH UltimoPago AS (
        SELECT
            c.IdCliente,
            c.DNI,
            c.Nombre,
            c.Apellido,
            c.Telefono,
            c.Email,
            c.PlanPago,
            c.FechaAlta,
            MAX(p.FechaHasta) AS Vencimiento
        FROM dbo.Clientes c
        LEFT JOIN dbo.Pagos p ON p.IdCliente = c.IdCliente
        GROUP BY
            c.IdCliente,
            c.DNI,
            c.Nombre,
            c.Apellido,
            c.Telefono,
            c.Email,
            c.PlanPago,
            c.FechaAlta
    )
    SELECT
        IdCliente,
        DNI,
        Nombre,
        Apellido,
        Telefono,
        Email,
        PlanPago,
        FechaAlta,
        Vencimiento,
        CASE
            WHEN Vencimiento IS NULL THEN 'SIN PAGO'
            WHEN Vencimiento < CAST(GETDATE() AS DATE) THEN 'VENCIDO'
            WHEN DATEDIFF(DAY, CAST(GETDATE() AS DATE), Vencimiento) <= 5 THEN 'POR VENCER'
            ELSE 'AL DIA'
        END AS EstadoPago
    FROM UltimoPago
    WHERE
        Vencimiento IS NULL
        OR Vencimiento < CAST(GETDATE() AS DATE)
        OR DATEDIFF(DAY, CAST(GETDATE() AS DATE), Vencimiento) <= 5
    ORDER BY
        CASE
            WHEN Vencimiento IS NULL THEN 0
            WHEN Vencimiento < CAST(GETDATE() AS DATE) THEN 1
            ELSE 2
        END,
        Vencimiento;
END
GO

/* 4) Verificacion rapida: debe aparecer la columna FechaAlta. */
EXEC dbo.Listar_Deudores;
GO
