# Auditoría gym1.1 — inicial

Fecha: 2026-10-02. Sin commit.

Total: 74. APROBADO: 24, MEJORABLE: 8, FALLÓ: 17, NO VERIFICABLE AUTOMÁTICAMENTE: 25.

Las aprobaciones estáticas indican revisión explícita del flujo y no equivalen a ejecución HTTP o SQL. Las evidencias identifican el método utilizado. Los valores de secretos y datos personales se omiten.

| ID | Prueba | Resultado inicial | Severidad | Evidencia | Archivo/método/SP | Corrección necesaria |
|---|---|---|---|---|---|---|
| A01 | Login vacío | ✅ APROBADO | ⚪ INFORMATIVO | Login.btnIngresar_Click valida ambos campos antes de consultar (estático). | gym1.1/Login.aspx.cs | — |
| A02 | Usuario inexistente | ✅ APROBADO | ⚪ INFORMATIVO | DAL parametrizado y mensaje genérico antes de crear sesión (estático). | Login.aspx.cs; UsuarioDAL.BuscarPorEmail | — |
| A03 | Contraseña incorrecta | ✅ APROBADO | ⚪ INFORMATIVO | Verificación previa al estado activo y a asignar sesión (estático). | Login.btnIngresar_Click | — |
| A04 | SQL Injection login | ✅ APROBADO | ⚪ INFORMATIVO | SqlParameter para email; password nunca se envía a SQL; sin SQL dinámico en login (estático). | UsuarioDAL.BuscarPorEmail | — |
| A05 | Hash de contraseñas | ⚠️ MEJORABLE | 🟡 MEDIO | PBKDF2 SHA256, 100000 iteraciones, salt aleatorio 16 bytes, hash 32 bytes, comparación XOR. Incrementar requiere compatibilidad de hashes. | PasswordHelper | Documentar actualización gradual; no invalidar hashes existentes. |
| A06 | Credenciales versionadas | ❌ FALLÓ | 🔴 CRÍTICO | Contraseña SMTP literal en tres archivos versionados; valores omitidos. | Web.config; .config; Web.config.example | Retirar secreto y leerlo del entorno; revocar credencial expuesta. |
| B01 | Páginas sin sesión | ❌ FALLÓ | 🟠 ALTO | Default hereda Page. CompleteRequest+return en OnInit no termina el ciclo de eventos Page. | Default.aspx.cs; PaginaProtegica.cs | Proteger inicio y detener ejecución antes de eventos. |
| B02 | Logout | ✅ APROBADO | ⚪ INFORMATIVO | Clear, Abandon, cookie expirada, no-cache (estático); falta comprobar HTTP real. | Site.Master.btnCerrarSesion_Click | — |
| B03 | Atrás tras logout | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Caché deshabilitada en master; interacción de historial real pendiente. | Site.Master.Page_Load | Prueba de navegador. |
| B04 | URL tras logout | ❌ FALLÓ | 🟠 ALTO | Default sin protección; otras páginas pueden continuar su ciclo tras redirigir. | PaginaProtegica.cs | Misma corrección B01. |
| B05 | Session Fixation | ❌ FALLÓ | 🟠 ALTO | Login solo Clear; RegenerarIdSesion no se llama. No existe nuevo contexto autenticado tras abandonar sesión anterior. | Login.aspx.cs | Login de dos solicitudes con entrega de un solo uso y nueva sesión. |
| B06 | Roles servidor | ❌ FALLÓ | 🔴 CRÍTICO | CrearAdministrador comprueba rol en Load y solo redirige sin detener evento Click; super página misma debilidad. | CrearAdministrador.aspx.cs; PaginaSuperAdministrador.cs | Autorización en base antes del ciclo; detener solicitudes no autorizadas. |
| C01 | Listado clientes | ✅ APROBADO | ⚪ INFORMATIVO | UI usa IdGimnasioActual; DAL @IdGimnasio; SP real WHERE IdGimnasio (estático completo). | Clientes/Registro; SP.ListarClientes; Listar_Clientes | — |
| C02 | Buscador | ✅ APROBADO | ⚪ INFORMATIVO | Usa IdGimnasioActual; ListarClientes(1) solo aparece en comentario (estático). | Pagos.CargarClientesParaBuscador | — |
| C03 | Pagos por cliente | ✅ APROBADO | ⚪ INFORMATIVO | SP real une cliente y pago por gimnasio y filtra ambos parámetros (estático). | PagoDAL.ListarPagosPorCliente; Listar_Pagos_Por_Cliente | — |
| C04 | Fecha alta | ✅ APROBADO | ⚪ INFORMATIVO | SELECT simultáneo por IdCliente e IdGimnasio parametrizados (estático). | SP.ObtenerFechaAltaCliente | — |
| C05 | HiddenField manipulado | ⚠️ MEJORABLE | 🟡 MEDIO | SP impide lectura ajena; selección inválida lanza excepción en fecha de alta; falta rechazo amigable previo. | Pagos.btnSeleccionarCliente_Click | Validar pertenencia antes de guardar selección. |
| C06 | Pago ajeno | ✅ APROBADO | ⚪ INFORMATIVO | Registrar_Pago real valida ambos IDs y THROW 50001 antes de INSERT (estático). | Registrar_Pago | — |
| D01 | Pago normal | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Flujo revisado; INSERT real y actualización de UI requieren fixture aislado. | Pagos.btnRegistrar_Click; Registrar_Pago | Ejecutar integración. |
| D02 | Pago duplicado | ❌ FALLÓ | 🟡 MEDIO | Precheck existe; SqlException 50002 no se maneja en Pagos y va a Error.aspx. | Pagos.btnRegistrar_Click | Manejar 50002 y violaciones UNIQUE. |
| D03 | Doble clic / carrera | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | 🟠 ALTO | SP IF EXISTS seguido por INSERT; consulta de índices no terminó. No asumir garantía de unicidad. | Registrar_Pago; índices Pagos | Verificar índices/duplicados y migración no destructiva. |
| D04 | Monto cero | ✅ APROBADO | ⚪ INFORMATIVO | UI rechaza <=0 (estático); SP no valida monto. | Pagos; Deudores; Registrar_Pago | Añadir defensa DAL/SQL. |
| D05 | Monto negativo | ✅ APROBADO | ⚪ INFORMATIVO | UI rechaza <=0 (estático); SP no valida monto. | Pagos; Deudores | Añadir defensa DAL/SQL. |
| D06 | Monto inválido | ✅ APROBADO | ⚪ INFORMATIVO | TryParse sin excepción para texto (estático). | Pagos; Deudores | — |
| D07 | Monto decimal es-AR | ❌ FALLÓ | 🟠 ALTO | TryParse utiliza cultura del servidor, reemplaza punto por coma. En en-US 15000,50 puede convertirse a 1500050. | Pagos; Deudores | Cultura explícita y rechazo de ambigüedades. |
| D08 | Monto fuera DECIMAL(12,2) | ❌ FALLÓ | 🟡 MEDIO | No límite máximo ni escala: SQL puede fallar o redondear. Máximo 9999999999,99. | Pagos; Deudores; PagoDAL | Validar rango y máximo dos decimales. |
| D09 | Pago futuro | ✅ APROBADO | ⚪ INFORMATIVO | UI permite año próximo; SP real no bloquea pagos futuros (estático). | Pagos; Registrar_Pago | — |
| D10 | Años dinámicos | ⚠️ MEJORABLE | 🟢 BAJO | Rango implementado -2/+1 en ambas pantallas; requerimiento -3/+3 condicionado a regla actual. | CargarOpcionesPeriodoPago | Documentar rango actual; no cambiar sin confirmación. |
| D11 | Estado de pago | ✅ APROBADO | ⚪ INFORMATIVO | SP real compara fecha con hoy y DATEDIFF <=5, incluidos 0 y 5; vencido antes de hoy (estático). | Listar_Pagos_Por_Cliente; Listar_Deudores | — |
| D12 | Cliente sin pagos | ❌ FALLÓ | 🟡 MEDIO | Pagos calcula desde FechaAlta; Deudores muestra solo mes actual cuando vencimiento NULL, pese a SP devolver FechaAlta. | Deudores.ObtenerMesesAdeudados | Usar FechaAlta para deuda inicial. |
| D13 | Actualización de Deudores | ✅ APROBADO | ⚪ INFORMATIVO | Recarga tras INSERT y SP MAX(FechaHasta); conserva POR VENCER <=5 como deudor (estático). | Deudores; Listar_Deudores | — |
| E01 | Observación longitud | ❌ FALLÓ | 🟡 MEDIO | Sin límite UI/servidor; parámetro 200 puede truncar silenciosamente. | Pagos/Deudores txtObs; PagoDAL | Máximo 200 en servidor y DAL. |
| E02 | Stored XSS | ❌ FALLÓ | 🟠 ALTO | BoundField Observacion codifica; ClientesJson serializado crudo en script admite cierre </script> desde datos almacenados. | Pagos.CargarClientesParaBuscador; Pagos.aspx | Escapar < > & y separadores Unicode antes de insertar JSON. |
| E03 | HTML inesperado | ⚠️ MEJORABLE | 🟢 BAJO | RequestValidation permanece activa; excepción termina en error genérico, no mensaje de entrada inválida. | Global.Application_Error | Mensaje amigable sin desactivar validación. |
| F01 | Alta normal | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Flujo completo revisado; requiere INSERT en fixture aislado. | Registro; SP.AgregarCliente; Agregar_Cliente | Ejecutar integración. |
| F02 | DNI duplicado | ✅ APROBADO | ⚪ INFORMATIVO | SP real filtra gimnasio+DNI y THROW 50001, UI lo maneja (estático); carrera depende de índice. | Agregar_Cliente; Registro | Verificar UNIQUE. |
| F03 | DNI inválido | ✅ APROBADO | ⚪ INFORMATIVO | Obligatorio, dígitos y 7-8; SP ASCII y mismos límites (estático). | Registro; Agregar_Cliente | — |
| F04 | Email | ❌ FALLÓ | 🟡 MEDIO | TextMode Email es cliente; no hay validación servidor. Vacío permitido por DAL y SQL. | Registro; SP.AgregarCliente | Validar formato y longitud; conservar opcional. |
| F05 | Nacimiento futuro | ✅ APROBADO | ⚪ INFORMATIVO | Page y DAL comprueban >Today antes de INSERT (estático). | Registro; SP.AgregarCliente | — |
| F06 | Nacimiento anterior 1900 | ✅ APROBADO | ⚪ INFORMATIVO | Page y DAL rechazan Year<1900 (estático). | Registro; SP.AgregarCliente | — |
| F07 | Nombres reales | ✅ APROBADO | ⚪ INFORMATIVO | NVARCHAR sin regex restrictiva; acepta acentos, apóstrofo y guión (estático). | Registro; SP.AgregarCliente | — |
| F08 | Nombres largos | ❌ FALLÓ | 🟡 MEDIO | UI MaxLength50; servidor no comprueba, SqlParameter50 puede truncar. | Registro; SP.AgregarCliente | Validar nombre/apellido en servidor y DAL. |
| F09 | Teléfono flexible | ✅ APROBADO | ⚪ INFORMATIVO | UI y servidor 20; DAL SQL30; formatos flexibles conservados (estático). | Registro | — |
| F10 | PlanPago manipulado | ❌ FALLÓ | 🟡 MEDIO | Planes reales 3xSemana y Full; Page/DAL sin allowlist explícita, event validation solo defensa de UI. | Registro; SP.AgregarCliente | Allowlist con valores reales en servidor/DAL. |
| G01 | Foreign keys | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | 🟠 ALTO | Inspección de catálogo no terminó; no afirmar constraints existentes. | SQL Server sys.foreign_keys | Reintentar / migración con preflight. |
| G02 | Constraints | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | 🟡 MEDIO | SP de alta/pago carecen de varias defensas; catálogo pendiente. | SQL Server; Agregar_Cliente; Registrar_Pago | Inspeccionar datos antes de CHECK/UNIQUE. |
| G03 | Pagos huérfanos | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | 🟠 ALTO | Consulta inicial pendiente por timeout SQL. | Pagos -> Clientes | Ejecutar contador. |
| G04 | Datos cruzados | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | 🔴 CRÍTICO | Consulta inicial pendiente por timeout SQL. | Pagos.IdGimnasio vs Clientes.IdGimnasio | Ejecutar contador. |
| G05 | Duplicados históricos | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | 🟠 ALTO | Consulta inicial pendiente; script histórico 04 borra pagos y reemplaza SP sin tenant. No ejecutarlo. | BaseDatos/04_validar_un_pago_por_mes.sql | Migración no destructiva con bloqueo si hay duplicados. |
| H01 | debug desarrollo/producción | ✅ APROBADO | ⚪ INFORMATIVO | Local true; Release elimina atributo debug, default false (estático). | Web.config; Web.Release.config | — |
| H02 | customErrors producción | ⚠️ MEJORABLE | 🟡 MEDIO | RemoteOnly local; Release no fuerza On. | Web.Release.config | On en transformación Release. |
| H03 | Cookies | ⚠️ MEJORABLE | 🟡 MEDIO | HttpOnly, SameSite Lax, InProc30; no Secure en local; Release no endurece HTTPS. | Web.config; Web.Release.config | Secure solo Release; sesión SameSite explícito. |
| H04 | Conexión SQL | ⚠️ MEJORABLE | 🟡 MEDIO | Conexión local sin TLS; no mostrar cadena. Producción requiere certificado y cifrado. | Web.config | Documentar configuración remota. |
| H05 | SMTP | ❌ FALLÓ | 🔴 CRÍTICO | Secreto real versionado (mismo hallazgo A06). | Web.config; .config; Web.config.example | Eliminar y usar configuración externa; rotar en proveedor. |
| I01 | Error.aspx | ✅ APROBADO | ⚪ INFORMATIVO | Mensaje genérico y referencia; panel detalle nunca visible. Código query sin encoding explícito en label. | Error.aspx.cs | Codificar código y permitir solo referencia válida. |
| I02 | Logging | ⚠️ MEJORABLE | 🟡 MEDIO | Trace.TraceError genera referencia pero no tiene listener persistente. | Global y redirecciones | Log persistente interno sin contenido sensible. |
| I03 | SqlException conocidas | ❌ FALLÓ | 🟡 MEDIO | Deudores maneja50001/50002; Pagos redirige ambas a Error. | Pagos.btnRegistrar_Click | Mensajes amigables. |
| I04 | Errores inesperados | ❌ FALLÓ | 🟠 ALTO | AdministrarGimnasios muestra ex.Message y almacena excepción completa en Session. | AdministrarGimnasios.CargarGimnasios/RedirigirAError | Mensaje genérico con código; no detalle interno al usuario. |
| J01 | Login correcto | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J02 | Login incorrecto | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J03 | Logout | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J04 | Acceso directo sin sesión | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J05 | Listado de clientes | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J06 | Alta de cliente | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J07 | DNI duplicado | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J08 | Fecha futura | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J09 | Selección de cliente | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J10 | Listado de pagos | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J11 | Registro de pago | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J12 | Pago duplicado | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J13 | Pago futuro | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J14 | Deudores | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J15 | Separación IdGimnasio | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
| J16 | Error.aspx | ⏸️ NO VERIFICABLE AUTOMÁTICAMENTE | ⚪ INFORMATIVO | Regresión pendiente al completar las correcciones. | Suite de regresión | — |
