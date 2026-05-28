USE GYM_DB;
GO

/* El historial de pagos muestra pagos ya realizados.
   El estado actual del cliente se calcula con el ultimo FechaHasta en la pantalla. */
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
    ORDER BY FechaHasta DESC, FechaPago DESC;
END
GO

EXEC dbo.Listar_Pagos_Por_Cliente @IdCliente = 1;
GO
