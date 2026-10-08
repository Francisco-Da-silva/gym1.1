# Reproducir QA en una base aislada

Las fuentes están en `Tests/Auditoria`. Requieren Windows, .NET Framework4.8, SQLCMD, LocalDB, IIS Express y Python3. No usar GYM_DB: las suites SQL y Seed exigen el catálogo GYM_QA_20261007. Los datos y la contraseña de fixture son ficticios. No se envían correos.

1. Compilar la solución Debug con MSBuild. Copiar `conexxion/bin/Debug/conexxion.dll` al directorio de las pruebas.
2. Crear una instancia LocalDB exclusiva y una base NUEVA GYM_QA_20261007. No reutilizar una fixture modificada: los tests insertan datos y esperan estado inicial.
3. Con SQLCMD ejecutar en esa base, por separado: `fixture.sql`, `BaseDatos/06_auditoria_seguridad_integridad.sql` y `fixture-procs.sql`. Nunca ejecutar estos seeds en una base real.
4. Compilar UnitTests.cs, Seed.cs, IntegrationTests.cs y ExtraTests.cs con `C:\Windows\Microsoft.NET\Framework64\v4.0.30319\csc.exe`, cada uno en su propio exe, referenciando conexxion.dll, System.Web.dll, System.Configuration.dll, System.Data.dll y System.Data.DataSetExtensions.dll.
5. Para Seed.exe, IntegrationTests.exe y ExtraTests.exe crear un archivo `<exe>.config` con conexión `GymDb` a la instancia exclusiva, catálogo GYM_QA_20261007, autenticación integrada. No versionar ese archivo.
6. Ejecutar UnitTests.exe, Seed.exe, IntegrationTests.exe y ExtraTests.exe, en ese orden. Resultados esperados:61,23,5 PASS; cero FAIL. Seed debe ejecutarse una sola vez.
7. Crear una COPIA de la aplicación para IIS Express. Cambiar exclusivamente la conexión de esa copia al fixture; no cambiar Web.config de la aplicación original. Servir la copia en `http://localhost:51877`.
8. Ejecutar `test_http.py`, `http-integration.py` y `extra-http.py`, en ese orden:12,33,3 PASS. ExtraTests prepara el cliente B sin pagos con FechaAlta tres meses antes. Las pruebas HTTP escriben únicamente en el fixture previamente configurado.
9. Detener sólo IIS Express y LocalDB creados para esta ejecución.

## Estado de la base real

Inspección anterior confirmó FKs simples, índices UNIQUE de pagos, cero huérfanos/cruces/duplicados mensuales, un grupo DNI repetido y un pago con fechas invertidas. En el intento de respaldo del07/10 SQL Server reportó945/701 y GYM_DB quedó RECOVERY_PENDING. No se aplicó06 ni07. No reparar con opciones de pérdida de datos ni eliminar registros automáticamente.

Una vez recuperada la disponibilidad: obtener respaldo COPY_ONLY CHECKSUM y RESTORE VERIFYONLY correcto antes de06. Revisar históricos con su responsable antes de07; el script aborta si persisten inconsistencias.

## Producción

Configurar GYM_SMTP_USER, GYM_SMTP_PASSWORD y GYM_PUBLIC_BASE_URL desde el entorno del proceso. Revocar/reemplazar la credencial SMTP previamente versionada; retirar un valor del código no lo elimina del historial Git. Release necesita HTTPS (cookies Secure); SQL remoto necesita TLS con certificado validado. Reiniciar el proceso después de actualizar variables. No se probó correo real ni publicación Release.
