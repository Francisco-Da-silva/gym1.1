/* Auditoria gym1.1, 2026-10-07. No elimina ni corrige datos historicos. DNI/fechas legacy se revisan en 07.
   Ejecutar solo despues de backup e inspeccion, con sqlcmd -b o como un unico batch.
   Si el preflight falla, toda la migracion revierte. Compatible con multi-gimnasio.
   Los pagos futuros siguen permitidos. El indice agrupa por mes incluso si FechaDesde no es dia 1.
*/
SET NOCOUNT ON;
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET NUMERIC_ROUNDABORT OFF;
BEGIN TRY
 BEGIN TRANSACTION;
 IF COL_LENGTH('dbo.Pagos','IdGimnasio') IS NULL OR COL_LENGTH('dbo.Clientes','IdGimnasio') IS NULL
  THROW 51000, 'Faltan columnas multi-gimnasio; no se modifico el esquema.', 1;
 IF EXISTS(SELECT 1 FROM dbo.Pagos p LEFT JOIN dbo.Clientes c ON c.IdCliente=p.IdCliente
  WHERE c.IdCliente IS NULL OR p.IdGimnasio IS NULL OR p.IdGimnasio<>c.IdGimnasio)
  THROW 51001, 'Hay pagos huerfanos o cruzados. Revisar sin borrar datos.', 1;
 IF EXISTS(SELECT 1 FROM dbo.Pagos GROUP BY IdGimnasio,IdCliente,YEAR(FechaDesde),MONTH(FechaDesde) HAVING COUNT(*)>1)
  THROW 51002, 'Hay pagos mensuales duplicados. Revisar sin borrar datos.', 1;
 IF EXISTS(SELECT 1 FROM dbo.Pagos WHERE Monto IS NULL OR Monto<=0)
  THROW 51004, 'Hay montos o fechas invalidos. Revisar sin borrar datos.', 1;
 IF EXISTS(SELECT 1 FROM dbo.Pagos WHERE Monto>9999999999.99 OR Monto<>ROUND(Monto,2))
  THROW 51006, 'Hay montos incompatibles con decimal(12,2). No se modificaron datos.', 1;
 IF EXISTS(SELECT 1 FROM dbo.Clientes c LEFT JOIN dbo.Gimnasios g ON c.IdGimnasio=g.IdGimnasio WHERE g.IdGimnasio IS NULL)
  THROW 51005, 'Hay clientes sin gimnasio valido.', 1;
 -- Se amplia decimal sin reducir precision de datos existentes.
 IF EXISTS(SELECT 1 FROM sys.columns WHERE object_id=OBJECT_ID('dbo.Pagos') AND name='Monto' AND (precision<>12 OR scale<>2))
  ALTER TABLE dbo.Pagos ALTER COLUMN Monto decimal(12,2) NOT NULL;
 IF COL_LENGTH('dbo.Pagos','AnioPeriodo') IS NULL ALTER TABLE dbo.Pagos ADD AnioPeriodo AS YEAR(FechaDesde) PERSISTED;
 IF COL_LENGTH('dbo.Pagos','MesPeriodo') IS NULL ALTER TABLE dbo.Pagos ADD MesPeriodo AS MONTH(FechaDesde) PERSISTED;
 IF NOT EXISTS(
 SELECT 1 FROM sys.indexes i WHERE i.object_id=OBJECT_ID('dbo.Pagos') AND i.is_unique=1 AND i.is_disabled=0 AND i.has_filter=0
 AND (SELECT COUNT(*) FROM sys.index_columns ic WHERE ic.object_id=i.object_id AND ic.index_id=i.index_id AND ic.key_ordinal>0) IN(3,4)
 AND (SELECT COUNT(*) FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id
   WHERE ic.object_id=i.object_id AND ic.index_id=i.index_id AND ic.key_ordinal>0 AND c.name IN('IdCliente','AnioPeriodo','MesPeriodo'))=3
 AND NOT EXISTS(SELECT 1 FROM sys.index_columns ic JOIN sys.columns c ON c.object_id=ic.object_id AND c.column_id=ic.column_id
   WHERE ic.object_id=i.object_id AND ic.index_id=i.index_id AND ic.key_ordinal>0 AND c.name NOT IN('IdCliente','IdGimnasio','AnioPeriodo','MesPeriodo')))
  EXEC(N'CREATE UNIQUE INDEX UX_Pagos_Gimnasio_Cliente_Mes_QA ON dbo.Pagos(IdGimnasio,IdCliente,AnioPeriodo,MesPeriodo);');
 IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.Clientes') AND name='IX_Clientes_Gimnasio_DNI')
  CREATE INDEX IX_Clientes_Gimnasio_DNI ON dbo.Clientes(IdGimnasio,DNI);
 IF NOT EXISTS(SELECT 1 FROM sys.indexes WHERE object_id=OBJECT_ID('dbo.Clientes') AND name='UX_Clientes_Gimnasio_Id')
  CREATE UNIQUE INDEX UX_Clientes_Gimnasio_Id ON dbo.Clientes(IdGimnasio,IdCliente);
 IF OBJECT_ID('dbo.FK_QA_Pagos_Cliente_Gimnasio','F') IS NULL
  ALTER TABLE dbo.Pagos WITH CHECK ADD CONSTRAINT FK_QA_Pagos_Cliente_Gimnasio FOREIGN KEY(IdGimnasio,IdCliente) REFERENCES dbo.Clientes(IdGimnasio,IdCliente);
 IF NOT EXISTS(SELECT 1 FROM sys.foreign_keys WHERE parent_object_id=OBJECT_ID('dbo.Clientes') AND referenced_object_id=OBJECT_ID('dbo.Gimnasios') AND is_disabled=0)
  ALTER TABLE dbo.Clientes WITH CHECK ADD CONSTRAINT FK_QA_Clientes_Gimnasios FOREIGN KEY(IdGimnasio) REFERENCES dbo.Gimnasios(IdGimnasio);
 IF OBJECT_ID('dbo.CK_QA_Pagos_Monto','C') IS NULL
  ALTER TABLE dbo.Pagos WITH CHECK ADD CONSTRAINT CK_QA_Pagos_Monto CHECK(Monto>0);
 EXEC(N'CREATE OR ALTER PROCEDURE dbo.Registrar_Pago
 @IdGimnasio int, @IdCliente int, @FechaPago datetime,
 @FechaDesde date, @FechaHasta date, @Monto decimal(12,2), @Observacion nvarchar(max)=NULL
AS
BEGIN
 SET NOCOUNT ON;
 IF NOT EXISTS(SELECT 1 FROM dbo.Clientes WHERE IdCliente=@IdCliente AND IdGimnasio=@IdGimnasio)
  THROW 50001, ''El cliente no pertenece al gimnasio autenticado.'', 1;
 IF NOT EXISTS(SELECT 1 FROM dbo.Gimnasios WHERE IdGimnasio=@IdGimnasio AND Activo=1)
  THROW 50004, ''El gimnasio se encuentra inactivo.'', 1;
 IF @Monto IS NULL OR @Monto<=0 OR @FechaDesde IS NULL OR @FechaHasta IS NULL OR @FechaHasta<@FechaDesde OR @FechaPago IS NULL
  THROW 50003, ''El monto o las fechas del pago no son validos.'', 1;
 IF DATALENGTH(ISNULL(@Observacion,N''''))/2>200
  THROW 50003, ''La observacion supera los 200 caracteres.'', 1;
 IF EXISTS(SELECT 1 FROM dbo.Pagos WHERE IdCliente=@IdCliente AND IdGimnasio=@IdGimnasio
  AND YEAR(FechaDesde)=YEAR(@FechaDesde) AND MONTH(FechaDesde)=MONTH(@FechaDesde))
  THROW 50002, ''Ya existe un pago para el periodo seleccionado.'', 1;
 BEGIN TRY
  INSERT dbo.Pagos(IdGimnasio,IdCliente,FechaPago,FechaDesde,FechaHasta,Monto,Observacion)
  VALUES(@IdGimnasio,@IdCliente,@FechaPago,@FechaDesde,@FechaHasta,@Monto,NULLIF(LTRIM(RTRIM(@Observacion)),N''''));
 END TRY
 BEGIN CATCH
  IF ERROR_NUMBER() IN (2601,2627)
   THROW 50002, ''Ya existe un pago para el periodo seleccionado.'', 1;
  THROW;
 END CATCH;
END;');
 EXEC(N'CREATE OR ALTER PROCEDURE dbo.Marcar_Pago_Mes
 @IdGimnasio int,@IdCliente int,@FechaDesde date,@FechaHasta date,
 @Monto decimal(12,2),@Observacion nvarchar(max)=NULL
AS
BEGIN
 SET NOCOUNT ON;
 DECLARE @Ahora datetime=GETDATE();
 EXEC dbo.Registrar_Pago @IdGimnasio,@IdCliente,@Ahora,@FechaDesde,@FechaHasta,@Monto,@Observacion;
END;');
 EXEC(N'CREATE OR ALTER PROCEDURE dbo.Agregar_Cliente
 @IdGimnasio int,@Nombre nvarchar(max),@Apellido nvarchar(max),@DNI nvarchar(max),
 @Telefono nvarchar(max)=NULL,@Email nvarchar(max)=NULL,@FechaNacimiento date,@PlanPago nvarchar(max)
AS
BEGIN
 SET NOCOUNT ON;
 SET @DNI=LTRIM(RTRIM(@DNI));
 IF NOT EXISTS(SELECT 1 FROM dbo.Gimnasios WHERE IdGimnasio=@IdGimnasio AND Activo=1)
  THROW 50002, ''El gimnasio no existe o se encuentra inactivo.'', 1;
 IF @DNI IS NULL OR @DNI COLLATE Latin1_General_BIN2 LIKE N''%[^0-9]%''
  THROW 50003, ''El DNI solo puede contener numeros.'', 1;
 IF LEN(@DNI)<7 OR LEN(@DNI)>8
  THROW 50004, ''El DNI debe tener entre 7 y 8 digitos.'', 1;
 IF NULLIF(LTRIM(RTRIM(@Nombre)),N'''') IS NULL OR NULLIF(LTRIM(RTRIM(@Apellido)),N'''') IS NULL
  OR DATALENGTH(@Nombre)/2>50 OR DATALENGTH(@Apellido)/2>50
  OR DATALENGTH(ISNULL(@Telefono,N''''))/2>20 OR DATALENGTH(ISNULL(@Email,N''''))/2>100
  OR @FechaNacimiento IS NULL OR @FechaNacimiento<''19000101'' OR @FechaNacimiento>CONVERT(date,GETDATE())
  OR @PlanPago IS NULL OR @PlanPago NOT IN(N''3xSemana'',N''Full'')
  THROW 50005, ''Revisa los datos del cliente.'', 1;
 DECLARE @TranInicial int=@@TRANCOUNT;
 IF @TranInicial=0 BEGIN TRANSACTION;
 ELSE SAVE TRANSACTION AltaCliente;
 BEGIN TRY
  IF EXISTS(SELECT 1 FROM dbo.Clientes WITH(UPDLOCK,HOLDLOCK)
    WHERE IdGimnasio=@IdGimnasio AND DNI=@DNI)
   THROW 50001, ''Ya existe un cliente con ese DNI en este gimnasio.'', 1;
  INSERT dbo.Clientes(IdGimnasio,Nombre,Apellido,DNI,Telefono,Email,FechaNacimiento,PlanPago)
  VALUES(@IdGimnasio,LTRIM(RTRIM(@Nombre)),LTRIM(RTRIM(@Apellido)),@DNI,
   NULLIF(LTRIM(RTRIM(@Telefono)),N''''),NULLIF(LTRIM(RTRIM(@Email)),N''''),@FechaNacimiento,@PlanPago);
  IF @TranInicial=0 COMMIT TRANSACTION;
 END TRY
 BEGIN CATCH
  IF @TranInicial=0 AND XACT_STATE()<>0 ROLLBACK TRANSACTION;
  ELSE IF XACT_STATE()=1 ROLLBACK TRANSACTION AltaCliente;
  IF ERROR_NUMBER() IN(2601,2627)
   THROW 50001, ''Ya existe un cliente con ese DNI en este gimnasio.'', 1;
  THROW;
 END CATCH;
END;');
 COMMIT TRANSACTION;
END TRY
BEGIN CATCH
 IF XACT_STATE()<>0 ROLLBACK TRANSACTION;
 THROW;
END CATCH;
