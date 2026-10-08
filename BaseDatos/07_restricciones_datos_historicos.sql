SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
/* No ejecutar hasta resolver duplicados y fechas historicas con el responsable.
   Transaccion completa: si hay inconsistencias, no cambia nada ni borra filas. */
BEGIN TRY
 BEGIN TRANSACTION;
 IF EXISTS(SELECT 1 FROM dbo.Clientes WITH(TABLOCKX,HOLDLOCK) GROUP BY IdGimnasio,DNI HAVING COUNT(*)>1)
  THROW 51003, 'Hay DNI duplicados por gimnasio. No se modifico ningun registro.', 1;
 IF EXISTS(SELECT 1 FROM dbo.Pagos WITH(TABLOCKX,HOLDLOCK) WHERE FechaHasta<FechaDesde)
  THROW 51004, 'Hay pagos con fechas inconsistentes. No se modifico ningun registro.', 1;
 IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.Clientes') AND name='UX_Clientes_Gimnasio_DNI')
  CREATE UNIQUE INDEX UX_Clientes_Gimnasio_DNI ON dbo.Clientes(IdGimnasio,DNI);
 IF OBJECT_ID('dbo.CK_QA_Pagos_Fechas','C') IS NULL
  ALTER TABLE dbo.Pagos WITH CHECK ADD CONSTRAINT CK_QA_Pagos_Fechas CHECK(FechaHasta>=FechaDesde);
 COMMIT TRANSACTION;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
