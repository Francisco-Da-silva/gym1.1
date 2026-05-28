USE GYM_DB;
GO

/* 1) Ver pagos duplicados por cliente y mes de FechaDesde. */
SELECT
    c.IdCliente,
    c.Nombre,
    c.Apellido,
    c.DNI,
    YEAR(p.FechaDesde) AS Anio,
    MONTH(p.FechaDesde) AS Mes,
    COUNT(*) AS CantidadPagos
FROM dbo.Pagos p
INNER JOIN dbo.Clientes c ON c.IdCliente = p.IdCliente
GROUP BY
    c.IdCliente,
    c.Nombre,
    c.Apellido,
    c.DNI,
    YEAR(p.FechaDesde),
    MONTH(p.FechaDesde)
HAVING COUNT(*) > 1
ORDER BY c.Apellido, c.Nombre, Anio, Mes;
GO

/* 2) Borra duplicados y conserva un solo pago por cliente y mes.
   Conserva el pago mas reciente por FechaPago y, ante empate, el IdPago mayor. */
;WITH PagosOrdenados AS (
    SELECT
        p.IdPago,
        ROW_NUMBER() OVER (
            PARTITION BY p.IdCliente, YEAR(p.FechaDesde), MONTH(p.FechaDesde)
            ORDER BY p.FechaPago DESC, p.IdPago DESC
        ) AS Fila
    FROM dbo.Pagos p
)
DELETE p
FROM dbo.Pagos p
INNER JOIN PagosOrdenados po ON po.IdPago = p.IdPago
WHERE po.Fila > 1;
GO

/* 3) Evita que vuelva a existir mas de un pago por cliente y mes. */
IF COL_LENGTH('dbo.Pagos', 'AnioPeriodo') IS NULL
BEGIN
    ALTER TABLE dbo.Pagos
    ADD AnioPeriodo AS YEAR(FechaDesde) PERSISTED;
END
GO

IF COL_LENGTH('dbo.Pagos', 'MesPeriodo') IS NULL
BEGIN
    ALTER TABLE dbo.Pagos
    ADD MesPeriodo AS MONTH(FechaDesde) PERSISTED;
END
GO

IF NOT EXISTS (
    SELECT 1
    FROM sys.indexes
    WHERE name = 'UX_Pagos_Cliente_Mes'
      AND object_id = OBJECT_ID('dbo.Pagos')
)
BEGIN
    CREATE UNIQUE INDEX UX_Pagos_Cliente_Mes
    ON dbo.Pagos
    (
        IdCliente,
        AnioPeriodo,
        MesPeriodo
    );
END
GO

/* 4) Refuerza la validacion desde el procedimiento. */
CREATE OR ALTER PROCEDURE dbo.Registrar_Pago
    @IdCliente   INT,
    @FechaPago   DATE,
    @FechaDesde  DATE,
    @FechaHasta  DATE,
    @Monto       DECIMAL(10,2),
    @Observacion NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @FechaHasta < CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('No se puede registrar un pago con vencimiento en fecha pasada.', 16, 1);
        RETURN;
    END

    IF @FechaHasta < @FechaDesde
    BEGIN
        RAISERROR('La fecha Hasta no puede ser menor que la fecha Desde.', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM dbo.Pagos
        WHERE IdCliente = @IdCliente
          AND YEAR(FechaDesde) = YEAR(@FechaDesde)
          AND MONTH(FechaDesde) = MONTH(@FechaDesde)
    )
    BEGIN
        RAISERROR('Este cliente ya tiene registrado un pago para ese mes.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Pagos
    (
        IdCliente,
        FechaPago,
        FechaDesde,
        FechaHasta,
        Monto,
        Observacion
    )
    VALUES
    (
        @IdCliente,
        @FechaPago,
        @FechaDesde,
        @FechaHasta,
        @Monto,
        @Observacion
    );
END
GO

CREATE OR ALTER PROCEDURE dbo.Marcar_Pago_Mes
    @IdCliente INT,
    @FechaDesde DATE,
    @FechaHasta DATE,
    @Monto DECIMAL(10,2),
    @Observacion NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @FechaHasta < CAST(GETDATE() AS DATE)
    BEGIN
        RAISERROR('No se puede registrar un pago con vencimiento en fecha pasada.', 16, 1);
        RETURN;
    END

    IF @FechaHasta < @FechaDesde
    BEGIN
        RAISERROR('La fecha Hasta no puede ser menor que la fecha Desde.', 16, 1);
        RETURN;
    END

    IF EXISTS (
        SELECT 1
        FROM dbo.Pagos
        WHERE IdCliente = @IdCliente
          AND YEAR(FechaDesde) = YEAR(@FechaDesde)
          AND MONTH(FechaDesde) = MONTH(@FechaDesde)
    )
    BEGIN
        RAISERROR('Este cliente ya tiene registrado un pago para ese mes.', 16, 1);
        RETURN;
    END

    INSERT INTO dbo.Pagos (IdCliente, FechaPago, FechaDesde, FechaHasta, Monto, Observacion)
    VALUES (@IdCliente, CAST(GETDATE() AS DATE), @FechaDesde, @FechaHasta, @Monto, @Observacion);
END
GO

/* 5) Verificacion: no deberian quedar duplicados. */
SELECT
    c.IdCliente,
    c.Nombre,
    c.Apellido,
    c.DNI,
    YEAR(p.FechaDesde) AS Anio,
    MONTH(p.FechaDesde) AS Mes,
    COUNT(*) AS CantidadPagos
FROM dbo.Pagos p
INNER JOIN dbo.Clientes c ON c.IdCliente = p.IdCliente
GROUP BY
    c.IdCliente,
    c.Nombre,
    c.Apellido,
    c.DNI,
    YEAR(p.FechaDesde),
    MONTH(p.FechaDesde)
HAVING COUNT(*) > 1
ORDER BY c.Apellido, c.Nombre, Anio, Mes;
GO
