USE GYM_DB;
GO

CREATE TABLE Pagos (
    IdPago       INT IDENTITY(1,1) PRIMARY KEY,
    IdCliente    INT NOT NULL,
    FechaPago    DATE NOT NULL,
    FechaDesde   DATE NOT NULL,
    FechaHasta   DATE NOT NULL,
    Monto        DECIMAL(10,2) NOT NULL,
    Observacion  NVARCHAR(200) NULL,

    CONSTRAINT FK_Pagos_Clientes
        FOREIGN KEY (IdCliente) REFERENCES Clientes(IdCliente)
);
GO

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

CREATE OR ALTER PROCEDURE dbo.Estado_Pagos_Clientes
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH UltimoPago AS (
        SELECT
            c.IdCliente,
            c.Nombre,
            c.Apellido,
            c.PlanPago,
            MAX(p.FechaHasta) AS UltimaFechaHasta
        FROM dbo.Clientes c
        LEFT JOIN dbo.Pagos p ON c.IdCliente = p.IdCliente
        GROUP BY c.IdCliente, c.Nombre, c.Apellido, c.PlanPago
    )
    SELECT
        IdCliente,
        Nombre,
        Apellido,
        PlanPago,
        UltimaFechaHasta,
        CASE
            WHEN UltimaFechaHasta IS NULL THEN 'SIN PAGO'
            WHEN UltimaFechaHasta < CAST(GETDATE() AS DATE) THEN 'VENCIDO'
            WHEN DATEDIFF(DAY, CAST(GETDATE() AS DATE), UltimaFechaHasta) <= 5 THEN 'POR VENCER'
            ELSE 'AL DIA'
        END AS EstadoPago,
        DATEDIFF(DAY, CAST(GETDATE() AS DATE), UltimaFechaHasta) AS DiasHastaVto
    FROM UltimoPago;
END
GO

CREATE OR ALTER PROCEDURE dbo.Pagos_PorVencerOVencidos
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH UltimoPago AS (
        SELECT
            c.IdCliente,
            c.Nombre,
            c.Apellido,
            c.PlanPago,
            MAX(p.FechaHasta) AS UltimaFechaHasta
        FROM dbo.Clientes c
        LEFT JOIN dbo.Pagos p ON c.IdCliente = p.IdCliente
        GROUP BY c.IdCliente, c.Nombre, c.Apellido, c.PlanPago
    )
    SELECT
        IdCliente,
        Nombre,
        Apellido,
        PlanPago,
        UltimaFechaHasta,
        CASE
            WHEN UltimaFechaHasta IS NULL THEN 'SIN PAGO'
            WHEN UltimaFechaHasta < CAST(GETDATE() AS DATE) THEN 'VENCIDO'
            WHEN DATEDIFF(DAY, CAST(GETDATE() AS DATE), UltimaFechaHasta) <= 5 THEN 'POR VENCER'
        END AS EstadoPago
    FROM UltimoPago
    WHERE
        UltimaFechaHasta IS NULL
        OR UltimaFechaHasta < CAST(GETDATE() AS DATE)
        OR DATEDIFF(DAY, CAST(GETDATE() AS DATE), UltimaFechaHasta) <= 5;
END
GO

CREATE OR ALTER PROCEDURE dbo.Listar_Pagos_Por_Cliente
    @IdCliente INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        FechaPago,
        FechaDesde,
        FechaHasta,
        Monto,
        Observacion,
        'PAGADO' AS EstadoPago
    FROM dbo.Pagos
    WHERE IdCliente = @IdCliente
    ORDER BY FechaHasta DESC;
END
GO

IF COL_LENGTH('dbo.Clientes', 'EstadoPago') IS NULL
BEGIN
    ALTER TABLE dbo.Clientes
    ADD EstadoPago VARCHAR(20) NOT NULL
        CONSTRAINT DF_Clientes_EstadoPago DEFAULT('SIN PAGO');
END
GO

IF COL_LENGTH('dbo.Clientes', 'Vencimiento') IS NULL
BEGIN
    ALTER TABLE dbo.Clientes
    ADD Vencimiento DATE NULL;
END
GO

IF COL_LENGTH('dbo.Clientes', 'FechaAlta') IS NULL
BEGIN
    ALTER TABLE dbo.Clientes
    ADD FechaAlta DATE NOT NULL
        CONSTRAINT DF_Clientes_FechaAlta DEFAULT (CONVERT(date, GETDATE()));
END
GO

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

    INSERT INTO dbo.Pagos (IdCliente, FechaPago, FechaDesde, FechaHasta, Monto, Observacion)
    VALUES (@IdCliente, CAST(GETDATE() AS DATE), @FechaDesde, @FechaHasta, @Monto, @Observacion);
END
GO
