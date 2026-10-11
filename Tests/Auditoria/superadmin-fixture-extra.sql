/* Sólo fixture ficticio de la auditoría; nunca ejecutar en GYM_DB. */
SET NOCOUNT ON;
IF DB_NAME()<>'GYM_QA_20261007' THROW 51099,'Fixture requerido',1;
IF NOT EXISTS(SELECT 1 FROM dbo.Gimnasios WHERE Email='qa-platform-only@example.invalid')
BEGIN
 INSERT dbo.Gimnasios(Nombre,Email,Activo) VALUES(N'QA Plataforma administrativa','qa-platform-only@example.invalid',1);
 DECLARE @g int=SCOPE_IDENTITY();
 INSERT dbo.Usuarios(IdGimnasio,Nombre,Email,PasswordHash,PasswordSalt,Rol)
 SELECT @g,N'QA Plataforma','qa-platform-only@example.invalid',PasswordHash,PasswordSalt,'SuperAdministrador'
 FROM dbo.Usuarios WHERE Email='qa-super@example.invalid';
END;
IF NOT EXISTS(SELECT 1 FROM dbo.Usuarios WHERE Email='qa-a-extra@example.invalid')
 INSERT dbo.Usuarios(IdGimnasio,Nombre,Email,PasswordHash,PasswordSalt,Rol)
 SELECT IdGimnasio,N'QA Cuenta adicional','qa-a-extra@example.invalid',PasswordHash,PasswordSalt,Rol
 FROM dbo.Usuarios WHERE Email='qa-a@example.invalid';
