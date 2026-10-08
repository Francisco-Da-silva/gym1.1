# Informe QA y seguridad — gym1.1



Cierre: 7 de octubre de 2026. Matriz inicial conservada como fotografía de la inspección del 2/3 de octubre; las evidencias finales incorporan verificaciones posteriores.

Alcance: ASP.NET Web Forms .NET Framework 4.8, DAL, procedimientos SQL, autenticación, autorización, clientes, pagos, deudores y errores. Sin envío de correos reales; pruebas de escritura únicamente sobre datos ficticios en LocalDB.

Resultado final de la matriz: 65 aprobado, 7 mejorable, 1 no verificable automáticamente, 1 falló. Son 74 casos de aceptación; las 137 comprobaciones automatizadas son ejecuciones de apoyo y no equivalen a 137 requisitos distintos.

Ejecución: 61 unitarias, 23 integración, 5 SQL adicionales, 12 HTTP sin sesión, 33 HTTP con sesión y 3 HTTP adicionales; todas pasaron. Compilación Debug completa (cuatro proyectos) y precompilación de todas las páginas ASP.NET correctas.

Estado de despliegue: código aplicado al repositorio original. Migración06 probada en fixture, NO aplicada a GYM_DB: respaldo COPY_ONLY falló (945/701), la base quedó RECOVERY_PENDING y el log registró memoria insuficiente en pool internal. No se obtuvo respaldo válido ni se continuó con ALTER. La inspección real de datos procede de la lectura exitosa anterior; no representa una verificación posterior al fallo.

Pendientes: recuperar disponibilidad SQL y obtener respaldo verificado antes de aplicar06; revisar un DNI duplicado por gimnasio y un pago con fechas invertidas antes de07; revocar y reemplazar SMTP expuesto en historial; revisar PBKDF2 con compatibilidad de hashes; verificar Atrás en navegador; configurar TLS/SMTP/origen público antes de producción.



## Cambios y métodos

### Autenticación e inactividad
UsuarioDAL.BuscarPorEmail; Login.btnIngresar_Click; InicioSesionSeguro
La contraseña se valida antes del aviso de cuenta inactiva. Login entrega una sesión nueva mediante token de un solo uso. Cada petición protegida revalida usuario/gimnasio y rol actual.

### Autorización y CSRF
PaginaProtegica.cs; PaginaSuperAdministrador.cs; CrearAdministrador; Default
Control en OnPreInit, detención del ciclo de eventos, ViewStateUserKey y token POST de sesión. Corrige ejecución de eventos tras una redirección insuficiente.

### Aislamiento multi-tenant
SP; PagoDAL; Clientes/Registro/Pagos/Deudores
Se comprueba pertenencia por IdCliente+IdGimnasio. La eliminación y las rutas heredadas exigen el gimnasio autenticado. FK compuesto agregado en06, pendiente real.

### Clientes y pagos
ValidacionDatos; Registro; Pagos; Deudores; SP; SQL06
Longitudes y planes reales validados; dinero es-AR sin redondeo silencioso; observación opcional200; futuros permitidos; deuda inicial desde FechaAlta. Altas concurrentes protegidas con bloqueo de rango.

### XSS y manejo de errores
Pagos JSON; Error; Global; AppLogger; AdministrarGimnasios
JSON escapa cierres script; referencias hex codificadas; errores conocidos amigables. Log interno persistente sin mensajes/entradas/secretos; detalle técnico retirado de la UI.

### Recuperación de contraseña
PasswordResetDAL; RecuperarPassword; RestablecerPassword
Token consumido atómicamente y vencimiento revalidado; enlace toma origen confiable GYM_PUBLIC_BASE_URL. Integración prueba uso único; correo externo no ejecutado.

### Configuración
Web.config; .config; Web.config.example; Web.Release.config; Emailservice
SMTP lee GYM_SMTP_USER y GYM_SMTP_PASSWORD. Release fuerza errores genéricos, debugfalse y Secure. Conexión original conservada al copiar desde fixture.

### Datos históricos
BaseDatos/06 y07
06 amplía monto a12,2, agrega defensas sin borrar históricos, reutiliza UNIQUE mensual existente. 07 aborta transaccionalmente ante DNI duplicado/fechas invertidas. No ejecutar04 destructivo para corregir estos casos.

## Matriz inicial y final

| ID | Caso | Inicial | Final | Severidad inicial | Evidencia final | Archivo/método |
|---|---|---|---|---|---|---|
| A01 | Login vacío | APROBADO | APROBADO | INFORMATIVO | HTTP: formulario vacío rechazado. | gym1.1/Login.aspx.cs |
| A02 | Usuario inexistente | APROBADO | APROBADO | INFORMATIVO | HTTP + DAL: usuario inexistente rechazado. | Login.aspx.cs; UsuarioDAL.BuscarPorEmail |
| A03 | Contraseña incorrecta | APROBADO | APROBADO | INFORMATIVO | HTTP: contraseña incorrecta rechazada. | Login.btnIngresar_Click |
| A04 | SQL Injection login | APROBADO | APROBADO | INFORMATIVO | HTTP + DAL: payload SQL no autentica. | UsuarioDAL.BuscarPorEmail |
| A05 | Hash de contraseñas | MEJORABLE | MEJORABLE | MEDIO | Revisión: PBKDF2-SHA256/100000; aumentar con migración versionada pendiente. | PasswordHelper |
| A06 | Credenciales versionadas | FALLÓ | MEJORABLE | CRÍTICO | Secreto retirado de los tres archivos; revocación externa e historial Git pendientes. | Web.config; .config; Web.config.example |
| B01 | Páginas sin sesión | FALLÓ | APROBADO | ALTO | HTTP GET de siete páginas sin sesión redirige a login. | Default.aspx.cs; PaginaProtegica.cs |
| B02 | Logout | APROBADO | APROBADO | INFORMATIVO | HTTP: logout invalida sesión. | Site.Master.btnCerrarSesion_Click |
| B03 | Atrás tras logout | NO VERIFICABLE AUTOMÁTICAMENTE | NO VERIFICABLE AUTOMÁTICAMENTE | INFORMATIVO | Cabeceras no-cache revisadas; botón Atrás en navegador requiere prueba manual. | Site.Master.Page_Load |
| B04 | URL tras logout | FALLÓ | APROBADO | ALTO | HTTP: acceso a URLs después de logout denegado. | PaginaProtegica.cs |
| B05 | Session Fixation | FALLÓ | APROBADO | ALTO | HTTP: cambia SID al autenticar; SID anterior no accede. | Login.aspx.cs |
| B06 | Roles servidor | FALLÓ | APROBADO | CRÍTICO | HTTP GET/POST: rol Administrador no accede a funciones SuperAdministrador. | CrearAdministrador.aspx.cs; PaginaSuperAdministrador.cs |
| C01 | Listado clientes | APROBADO | APROBADO | INFORMATIVO | SQL + HTTP: listados filtrados por gimnasio. | Clientes/Registro; SP.ListarClientes; Listar_Clientes |
| C02 | Buscador | APROBADO | APROBADO | INFORMATIVO | HTTP: buscador contiene clientes del tenant autenticado. | Pagos.CargarClientesParaBuscador |
| C03 | Pagos por cliente | APROBADO | APROBADO | INFORMATIVO | SQL: cliente ajeno no devuelve pagos. | PagoDAL.ListarPagosPorCliente; Listar_Pagos_Por_Cliente |
| C04 | Fecha alta | APROBADO | APROBADO | INFORMATIVO | SQL: fecha de alta requiere ambos IDs. | SP.ObtenerFechaAltaCliente |
| C05 | HiddenField manipulado | MEJORABLE | APROBADO | MEDIO | HTTP: HiddenField ajeno rechazado sin pago. | Pagos.btnSeleccionarCliente_Click |
| C06 | Pago ajeno | APROBADO | APROBADO | INFORMATIVO | SQL: pago a cliente ajeno arroja 50001. | Registrar_Pago |
| D01 | Pago normal | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | SQL + HTTP: pago normal y monto exacto. | Pagos.btnRegistrar_Click; Registrar_Pago |
| D02 | Pago duplicado | FALLÓ | APROBADO | MEDIO | SQL + HTTP: duplicado rechazado con mensaje. | Pagos.btnRegistrar_Click |
| D03 | Doble clic / carrera | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | ALTO | Ocho pagos concurrentes: uno creado y siete rechazados. Catálogo real confirmó UNIQUE mensual preexistente. | Registrar_Pago; índices Pagos |
| D04 | Monto cero | APROBADO | APROBADO | INFORMATIVO | Unitario + HTTP: cero rechazado. | Pagos; Deudores; Registrar_Pago |
| D05 | Monto negativo | APROBADO | APROBADO | INFORMATIVO | Unitario + HTTP: negativo rechazado. | Pagos; Deudores |
| D06 | Monto inválido | APROBADO | APROBADO | INFORMATIVO | Unitario + HTTP: texto rechazado. | Pagos; Deudores |
| D07 | Monto decimal es-AR | FALLÓ | APROBADO | ALTO | Unitario + SQL: cultura es-AR; 15000,50 conserva importe. | Pagos; Deudores |
| D08 | Monto fuera DECIMAL(12,2) | FALLÓ | MEJORABLE | MEDIO | Código valida DECIMAL(12,2); real conserva DECIMAL(10,2) hasta aplicar 06. | Pagos; Deudores; PagoDAL |
| D09 | Pago futuro | APROBADO | APROBADO | INFORMATIVO | SQL: pago futuro permitido. | Pagos; Registrar_Pago |
| D10 | Años dinámicos | MEJORABLE | MEJORABLE | BAJO | Rango dinámico vigente -2/+1 conservado; -3/+3 requiere regla confirmada. | CargarOpcionesPeriodoPago |
| D11 | Estado de pago | APROBADO | APROBADO | INFORMATIVO | SQL: límites -1/0/5/6 días correctos. | Listar_Pagos_Por_Cliente; Listar_Deudores |
| D12 | Cliente sin pagos | FALLÓ | APROBADO | MEDIO | HTTP: sin pagos, meses desde FechaAlta. | Deudores.ObtenerMesesAdeudados |
| D13 | Actualización de Deudores | APROBADO | APROBADO | INFORMATIVO | HTTP + SQL: consulta de deudores actualizada. | Deudores; Listar_Deudores |
| E01 | Observación longitud | FALLÓ | APROBADO | MEDIO | Unitario + HTTP: 201 rechazado; maxlength=200 en ambas pantallas. | Pagos/Deudores txtObs; PagoDAL |
| E02 | Stored XSS | FALLÓ | APROBADO | ALTO | HTTP: cierre script almacenado escapado; BoundField codifica HTML. | Pagos.CargarClientesParaBuscador; Pagos.aspx |
| E03 | HTML inesperado | MEJORABLE | APROBADO | BAJO | HTTP: RequestValidation mantiene rechazo y aviso amigable. | Global.Application_Error |
| F01 | Alta normal | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | HTTP + SQL: alta normal. | Registro; SP.AgregarCliente; Agregar_Cliente |
| F02 | DNI duplicado | APROBADO | APROBADO | INFORMATIVO | HTTP + ocho altas concurrentes: una creada, siete duplicadas rechazadas. | Agregar_Cliente; Registro |
| F03 | DNI inválido | APROBADO | APROBADO | INFORMATIVO | Unitario: DNI ASCII 7-8, entradas inválidas rechazadas. | Registro; Agregar_Cliente |
| F04 | Email | FALLÓ | APROBADO | MEDIO | Unitario: email opcional, formato/longitud validado. | Registro; SP.AgregarCliente |
| F05 | Nacimiento futuro | APROBADO | APROBADO | INFORMATIVO | Unitario + HTTP: nacimiento futuro rechazado. | Registro; SP.AgregarCliente |
| F06 | Nacimiento anterior 1900 | APROBADO | APROBADO | INFORMATIVO | Unitario: anterior a 1900 rechazado. | Registro; SP.AgregarCliente |
| F07 | Nombres reales | APROBADO | APROBADO | INFORMATIVO | Unitario + SQL: acentos y apóstrofos permitidos. | Registro; SP.AgregarCliente |
| F08 | Nombres largos | FALLÓ | APROBADO | MEDIO | Unitario: longitud >50 rechazada. | Registro; SP.AgregarCliente |
| F09 | Teléfono flexible | APROBADO | APROBADO | INFORMATIVO | Unitario: teléfono flexible hasta20. | Registro |
| F10 | PlanPago manipulado | FALLÓ | APROBADO | MEDIO | Unitario + SQL: allowlist Full/3xSemana. | Registro; SP.AgregarCliente |
| G01 | Foreign keys | NO VERIFICABLE AUTOMÁTICAMENTE | MEJORABLE | ALTO | Catálogo real: FKs simples presentes. FK compuesto tenant-cliente probado en fixture; aplicar06 pendiente. | SQL Server sys.foreign_keys |
| G02 | Constraints | NO VERIFICABLE AUTOMÁTICAMENTE | FALLÓ | MEDIO | Real: un grupo DNI duplicado y un pago con fechas invertidas. 07 pendiente de corrección humana; no se borró información. | SQL Server; Agregar_Cliente; Registrar_Pago |
| G03 | Pagos huérfanos | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | ALTO | Inspección real anterior: cero pagos huérfanos. Fixture: cero. | Pagos -> Clientes |
| G04 | Datos cruzados | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | CRÍTICO | Inspección real anterior: cero pagos cruzados. Fixture: FK compuesto rechaza cruce. | Pagos.IdGimnasio vs Clientes.IdGimnasio |
| G05 | Duplicados históricos | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | ALTO | Inspección real: cero pagos mensuales duplicados; índice UNIQUE presente. DNI histórico documentado en G02. | BaseDatos/04_validar_un_pago_por_mes.sql |
| H01 | debug desarrollo/producción | APROBADO | APROBADO | INFORMATIVO | Build + revisión Release: debug false. | Web.config; Web.Release.config |
| H02 | customErrors producción | MEJORABLE | APROBADO | MEDIO | Revisión: Release fuerza customErrors On. | Web.Release.config |
| H03 | Cookies | MEJORABLE | APROBADO | MEDIO | HTTP + revisión: HttpOnly/SameSite; Secure en Release HTTPS. | Web.config; Web.Release.config |
| H04 | Conexión SQL | MEJORABLE | MEJORABLE | MEDIO | Local SQL sin TLS; producción remota exige certificado y Encrypt=True. | Web.config |
| H05 | SMTP | FALLÓ | MEJORABLE | CRÍTICO | SMTP por variables de entorno; revocar credencial histórica externamente. | Web.config; .config; Web.config.example |
| I01 | Error.aspx | APROBADO | APROBADO | INFORMATIVO | HTTP: Error.aspx con referencia válida/inválida; no muestra excepción. | Error.aspx.cs |
| I02 | Logging | MEJORABLE | APROBADO | MEDIO | Revisión + ejecución: log persistente con referencia/tipo/código/stack; omite mensajes y valores sensibles. | Global y redirecciones |
| I03 | SqlException conocidas | FALLÓ | APROBADO | MEDIO | HTTP + revisión: 50001/50002/50003/50004/50005 y UNIQUE reciben respuesta controlada. | Pagos.btnRegistrar_Click |
| I04 | Errores inesperados | FALLÓ | APROBADO | ALTO | Compilación + HTTP superadmin: sin ex.Message público ni excepción en Session. | AdministrarGimnasios.CargarGimnasios/RedirigirAError |
| J01 | Login correcto | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | HTTP: cuenta ficticia activa autentica y accede al inicio. | Suite de regresión |
| J02 | Login incorrecto | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | HTTP: contraseña incorrecta rechazada. | Suite de regresión |
| J03 | Logout | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | HTTP: logout invalida sesión. | Suite de regresión |
| J04 | Acceso directo sin sesión | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | HTTP GET de siete páginas sin sesión redirige a login. | Suite de regresión |
| J05 | Listado de clientes | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | SQL + HTTP: listados filtrados por gimnasio. | Suite de regresión |
| J06 | Alta de cliente | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | HTTP + SQL: alta normal. | Suite de regresión |
| J07 | DNI duplicado | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | HTTP + ocho altas concurrentes: una creada, siete duplicadas rechazadas. | Suite de regresión |
| J08 | Fecha futura | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | Unitario + HTTP: nacimiento futuro rechazado. | Suite de regresión |
| J09 | Selección de cliente | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | HTTP: HiddenField ajeno rechazado sin pago. | Suite de regresión |
| J10 | Listado de pagos | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | SQL: cliente ajeno no devuelve pagos. | Suite de regresión |
| J11 | Registro de pago | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | SQL + HTTP: pago normal y monto exacto. | Suite de regresión |
| J12 | Pago duplicado | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | SQL + HTTP: duplicado rechazado con mensaje. | Suite de regresión |
| J13 | Pago futuro | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | SQL: pago futuro permitido. | Suite de regresión |
| J14 | Deudores | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | HTTP + SQL: consulta de deudores actualizada. | Suite de regresión |
| J15 | Separación IdGimnasio | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | SQL + HTTP: listados filtrados por gimnasio. | Suite de regresión |
| J16 | Error.aspx | NO VERIFICABLE AUTOMÁTICAMENTE | APROBADO | INFORMATIVO | HTTP: Error.aspx con referencia válida/inválida; no muestra excepción. | Suite de regresión |

## Seguridad y operación

No afirmar que todos los casos están cerrados: quedan riesgos operativos y datos históricos. No se reescribió historial Git ni se alteraron registros reales. Credenciales y datos personales omitidos. La baja de un gimnasio también impide nuevas solicitudes de sesiones existentes por revalidación en servidor; ese escenario se revisó estáticamente, no mediante desactivación HTTP en esta ejecución.

Configuración externa requerida: GYM_SMTP_USER, GYM_SMTP_PASSWORD y GYM_PUBLIC_BASE_URL (URL HTTPS propia). Después de recuperada GYM_DB: respaldar/verificar, aplicar06, recompilar/reiniciar la aplicación y comprobar acceso. Resolver históricos con su responsable antes de07.

Build: MSBuild gym1.1.slnx /t:Build /p:Configuration=Debug /v:minimal /nologo; aspnet_compiler -v / -p gym1.1 <salida-nueva>. No se ejecutó publicación Release ni SMTP real.

Referencia: https://cheatsheetseries.owasp.org/cheatsheets/Password_Storage_Cheat_Sheet.html (PBKDF2-SHA256 600000 iteraciones recomendado, requiere estrategia compatible); https://learn.microsoft.com/en-us/dotnet/fundamentals/code-analysis/quality-rules/ca5368 (ViewStateUserKey).

## Archivos de auditoría modificados

| Archivo | Método, problema y solución |
|---|---|
| BaseDatos/06_auditoria_seguridad_integridad.sql | Registrar_Pago / Agregar_Cliente: defensas SQL, transacciones, índices y FK compuesto. Pendiente real. |
| BaseDatos/07_restricciones_datos_historicos.sql | Preflight transaccional: UNIQUE DNI y CHECK fechas sólo tras resolver históricos. |
| conexxion/AppLogger.cs | LogError: registro persistente sin mensajes o valores sensibles. |
| conexxion/DeudoresDal.cs | ListarDeudores: control tenant y log seguro. |
| conexxion/Emailservice.cs | EnviarRecuperacionPassword / ObtenerOrigenRecuperacion: entorno, encoding y origen confiable. |
| conexxion/PagoDAL.cs | RegistrarPago / ListarPagosPorCliente: tenant y validación de importes, fechas y observación. |
| conexxion/PasswordResetDAL.cs | CambiarPasswordConToken: consumo atómico y expiración. |
| conexxion/SP.cs | AgregarCliente / ClientePertenece / EliminarCliente / PagoDAL: validación y aislamiento. |
| conexxion/UsuarioDAL.cs | BuscarPorEmail / ObtenerRolActivo: estado conjunto usuario-gym y rol vigente. |
| conexxion/ValidacionDatos.cs | ErrorCliente / validación dinero: límites, cultura es-AR y planes reales. |
| conexxion/conexxion.csproj | Incluye AppLogger y ValidacionDatos. |
| gym1.1/.config | Secreto SMTP eliminado. |
| gym1.1/AdministrarGimnasios.aspx.cs | CargarGimnasios / RedirigirAError: logging interno y errores sin detalle público. |
| gym1.1/CrearAdministrador.aspx.cs | Herencia PaginaSuperAdministrador; validación servidor y log. |
| gym1.1/Default.aspx.cs | Herencia PaginaProtegida protege inicio. |
| gym1.1/Deudores.aspx | txtObs: maxlength200. |
| gym1.1/Deudores.aspx.cs | ObtenerMesesAdeudados / registrar pago: FechaAlta y validación. |
| gym1.1/Error.aspx | Texto genérico y referencia sin detalle técnico. |
| gym1.1/Error.aspx.cs | Page_Load: sólo referencia hex8 codificada y aviso de entrada inválida. |
| gym1.1/Global.asax.cs | Application_Error: log seguro y respuesta controlada. |
| gym1.1/InicioSesionSeguro.cs | Entrega de login de un solo uso; abandona SID previo. |
| gym1.1/Login.aspx.cs | btnIngresar_Click / Page_Load: contraseña, inactividad y nuevo contexto de sesión. |
| gym1.1/PaginaProtegica.cs | OnPreInit: sesión/rol/estado actual, CSRF, ViewStateUserKey y no-cache. |
| gym1.1/PaginaSuperAdministrador.cs | AutorizarRol: autorización antes de eventos. |
| gym1.1/Pagos.aspx | txtObs: maxlength200. |
| gym1.1/Pagos.aspx.cs | CargarClientesParaBuscador / seleccionar / registrar: JSON seguro, pertenencia y validaciones. |
| gym1.1/RecuperarPassword.aspx.cs | Origen confiable de enlace y log seguro. |
| gym1.1/Registro.aspx.cs | btnGuardar_Click: validación cliente y errores SQL conocidos. |
| gym1.1/RestablecerPassword.aspx.cs | Token inválido/vencido/reutilizado y log seguro. |
| gym1.1/Web.Release.config | Transformación: debugfalse, customErrorsOn, cookiesSecure. |
| gym1.1/Web.config | SMTP sin secreto; SameSite; conexión original preservada. |
| gym1.1/Web.config.example | Ejemplo sin secreto SMTP. |
| gym1.1/gym1.1.csproj | Incluye páginas/bases nuevas y entrega segura de login. |

Cambios previos del usuario preservados: Site.Master y sus code-behind/designer, Error.aspx.designer, AdministrarGimnasios.aspx/designer y gestión de administradores. No se descartaron al aplicar la auditoría. .gitignore excluye logs/bases/binarios QA. Tests/Auditoria contiene fixtures y suites; docs/Auditoria contiene matriz, evidencia y este informe.

Build completo y aspnet_compiler: éxito, sin errores ni warnings mostrados. Regresión repetida tras correcciones: login, logout, sesión, roles, tenant, selección, altas/duplicados/fechas, pagos/duplicados/futuros, deudores y Error.aspx. Vulnerabilidades altas/críticas originales: secreto SMTP, autorización de roles, acceso sin sesión, fijación de sesión, XSS JSON y exposición de excepciones; correcciones y pendientes descritos arriba.