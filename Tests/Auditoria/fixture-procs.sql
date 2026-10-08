CREATE PROCEDURE dbo.Listar_Clientes
    @IdGimnasio INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        IdCliente,
        IdGimnasio,
        Nombre,
        Apellido,
        DNI,
        Telefono,
        Email,
        FechaNacimiento,
        PlanPago
    FROM dbo.Clientes
    WHERE IdGimnasio = @IdGimnasio
    ORDER BY Apellido, Nombre;
END;
GO

CREATE PROCEDURE dbo.Listar_Pagos_Por_Cliente
    @IdGimnasio INT,
    @IdCliente INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        p.IdPago,
        p.IdCliente,
        p.IdGimnasio,
        p.FechaPago,
        p.FechaDesde,
        p.FechaHasta,
        p.Monto,
        p.Observacion,
        CASE
            WHEN p.FechaHasta < CAST(GETDATE() AS DATE)
                THEN 'VENCIDO'
            WHEN DATEDIFF(
                    DAY,
                    CAST(GETDATE() AS DATE),
                    p.FechaHasta
                 ) <= 5
                THEN 'POR VENCER'
            ELSE 'AL DÍA'
        END AS EstadoPago
    FROM dbo.Pagos p
    INNER JOIN dbo.Clientes c
        ON c.IdCliente = p.IdCliente
       AND c.IdGimnasio = p.IdGimnasio
    WHERE p.IdCliente = @IdCliente
      AND p.IdGimnasio = @IdGimnasio
    ORDER BY p.FechaHasta DESC;
END;
GO

CREATE PROCEDURE dbo.Listar_Deudores
    @IdGimnasio INT
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH UltimoPago AS
    (
        SELECT
            c.IdCliente,
            c.IdGimnasio,
            c.DNI,
            c.Nombre,
            c.Apellido,
            c.Telefono,
            c.Email,
            c.PlanPago,
            c.FechaAlta,
            MAX(p.FechaHasta) AS Vencimiento
        FROM dbo.Clientes AS c
        LEFT JOIN dbo.Pagos AS p
            ON p.IdCliente = c.IdCliente
           AND p.IdGimnasio = c.IdGimnasio
        WHERE c.IdGimnasio = @IdGimnasio
        GROUP BY
            c.IdCliente,
            c.IdGimnasio,
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
        IdGimnasio,
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
            WHEN Vencimiento < CAST(GETDATE() AS DATE)
                THEN 'VENCIDO'
            WHEN DATEDIFF(
                DAY,
                CAST(GETDATE() AS DATE),
                Vencimiento
            ) <= 5
                THEN 'POR VENCER'
            ELSE 'AL DÍA'
        END AS EstadoPago
    FROM UltimoPago
    WHERE Vencimiento IS NULL
       OR Vencimiento < CAST(GETDATE() AS DATE)
       OR DATEDIFF(
            DAY,
            CAST(GETDATE() AS DATE),
            Vencimiento
       ) <= 5
    ORDER BY Apellido, Nombre;
END;
GO

:EXIT
