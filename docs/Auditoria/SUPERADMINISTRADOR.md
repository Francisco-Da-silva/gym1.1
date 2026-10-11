# Separación de superadministrador — 11/10/2026

El inicio cambia según el rol autenticado. SuperAdministrador ve resumen de gimnasios (total/activos/inactivos), gestión de gimnasios y acceso a registro de gimnasio/cuenta administradora. Administrador y Gimnasio conservan las herramientas de clientes, pagos, registro y deudores. Mi cuenta muestra nombre, correo y tipo de cuenta; cerrar sesión permanece compartido.

PaginaGimnasio hereda los controles existentes de sesión, estado activo, CSRF y ViewStateUserKey, y autoriza sólo Administrador/Gimnasio en OnPreInit. Clientes, Registro, Pagos y Deudores heredan esta base. Las solicitudes de SuperAdministrador se redirigen al inicio antes de eventos o consultas operativas. PaginaSuperAdministrador mantiene la autorización inversa de las páginas de plataforma.

UsuarioDAL.ListarGimnasios excluye registros con cuentas exclusivamente SuperAdministrador; conserva gimnasios sin cuenta y gimnasios con cuentas Administrador/Gimnasio. El resumen cuenta IdGimnasio distintos para evitar duplicaciones por múltiples cuentas. No se borran registros ni se cambia el esquema SQL.

Validación: build completo Debug de gym1.1.slnx y aspnet_compiler correctos;30 comprobaciones HTTP sobre IIS Express/LocalDB con datos ficticios, todas aprobadas. Evidencia: evidencia/superadmin-roles-results.json. No se modificaron clientes, pagos ni cuentas reales para probar.
