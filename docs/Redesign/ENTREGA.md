# Gym Manager — entrega del rediseño

Rama local: `codex/redesign-gym-manager`. No se publicó, desplegó ni fusionó.

## Alcance y decisiones

Se conserva la separación del SuperAdministrador autorizada por el usuario. Su cuenta administra gimnasios; las operaciones de clientes, pagos y deudores pertenecen a las cuentas de gimnasio. La protección se aplica también en el servidor.

| Fase | Archivos principales | Resultado y fundamento |
|---|---|---|
| 2 | Site.Master, Site.Master.cs, Site.Master.designer.cs, Content/Site.css, Scripts/gym-shell.js, gym1.1.csproj | Estilos centralizados, menú activo y adaptable, encabezado, navegación con teclado y foco. |
| 3 | Clientes.aspx y asociados, conexxion/ClientesDAL.cs, conexxion.csproj, Pagos.aspx y asociados, Deudores.aspx y asociados, Registro.aspx, CrearAdministrador.aspx, Login.aspx | Búsqueda parametrizada y páginas de 20 clientes, accesos al historial, búsqueda de deudores, formularios y tablas adaptables. Se mantienen eventos y validaciones del negocio. |
| 4 | Default.aspx y asociados, conexxion/DashboardDAL.cs | Indicadores por gimnasio, cobros del mes, pagos recientes y vencimientos obtenidos de SQL. No se inventó un estado de cliente activo. |
| 5 | Content/Site.css, Scripts/gym-theme.js, Site.Master, Login.aspx, RecuperarPassword.aspx, RestablecerPassword.aspx, Error.aspx | Tema claro predeterminado y oscuro opcional persistente; formularios públicos coherentes. |
| 6 | Tests/Auditoria/RedesignTests.cs, docs/Redesign/evidencia, Pagos.aspx, Registro.aspx, Content/Site.css | Evidencia de regresión, corrección de asociaciones de etiquetas en Pagos, título de Registro y colores de enlaces. |

## Verificación

La solución compiló correctamente y se precompilaron todas las páginas ASP.NET. Se ejecutaron 182 verificaciones automatizadas sobre una base LocalDB nueva y aislada: 61 unitarias, 23 de integración, 5 adicionales, 12 HTTP iniciales, 33 HTTP de integración, 3 HTTP adicionales, 30 de separación de roles y 15 específicas del rediseño. Los resultados se conservan en `evidencia`.

Se verificaron inicio/cierre de sesión, autorización, registro de clientes, DNI duplicado, selección y registro de pagos, pagos repetidos y futuros, historial, deudores y aislamiento entre gimnasios. Las 15 pruebas nuevas cubren búsqueda literal, paginación, filtros por gimnasio y consultas del dashboard.

En navegador se comprobaron tema claro/oscuro, persistencia al navegar, búsqueda con 25 resultados y segunda página de 5, menú móvil, Escape y foco, colapso de escritorio, tablas y formulario de pagos. No se observaron errores o advertencias JavaScript en las vistas consultadas. Se revisaron anchos de 390, 768 y 1265 píxeles; dashboard y Pagos no desbordaron el ancho de página. Se corrigió una etiqueta que asociaba Buscar cliente al mes del pago y se verificaron ambas etiquetas después del cambio. Las capturas contienen únicamente datos ficticios del entorno QA. Esta revisión no equivale a una certificación completa de accesibilidad.

## Base de datos y límites

No se requieren nuevas tablas ni migraciones para este rediseño. ClientesDAL y DashboardDAL contienen las consultas nuevas, parametrizadas por IdGimnasio; reutilizan tablas y el procedimiento Listar_Deudores existentes. Las pruebas SQL se realizaron únicamente en la base aislada. La consulta de metadatos de la instancia real agotó su tiempo de espera; queda pendiente comprobar el arranque y los indicadores contra esa instancia. No se presenta esa comprobación como aprobada. Tampoco se validó envío real de correo SMTP.

No se incorporó edición de clientes ni un estado activo/inactivo de socio porque el modelo actual no los define. Se conservan las limitaciones de períodos y las reglas de pagos existentes. Bootstrap sigue dependiendo de la carga de sus recursos CDN. Los pendientes históricos de infraestructura, respaldo y rotación de credenciales se mantienen en el informe de auditoría previo.

## Cómo probar localmente

1. Abrir `C:\Users\franc\source\repos\gym1.1\gym1.1.slnx` en Visual Studio y compilar la solución.
2. Confirmar que la instancia SQL configurada por el proyecto está disponible. Iniciar la aplicación con IIS Express.
3. Ingresar con una cuenta de gimnasio y revisar Inicio, Clientes, Registro, Pagos y Deudores. Probar búsqueda, siguiente/anterior, historial y el botón de tema.
4. Cerrar sesión e ingresar como SuperAdministrador: comprobar menú administrativo, listado y alta de gimnasios, sin accesos operativos de gimnasio. Comprobar también las rutas directas protegidas.
5. Revisar el menú en móvil y repetir navegación en ambos temas.

Las credenciales, archivos MDF/LDF y configuración del entorno QA no deben copiarse a producción. `RedesignTests.cs` tiene guardas de catálogo e instancia de prueba; se ejecutó después de la preparación y regresión existentes, con sus datos ficticios. No debe ejecutarse sobre la base real.
