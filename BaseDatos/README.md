# Scripts de base de datos

Ejecutar los scripts sobre la base `GYM_DB` desde SQL Server Management Studio.

Orden sugerido:

1. `00_script_base.sql`: estructura y procedimientos principales de la base.
2. `01_actualizacion_fecha_alta_deudores.sql`: agrega `FechaAlta` y actualiza `Listar_Deudores`.
3. `02_actualizacion_historial_pagos.sql`: ajusta el historial de pagos para mostrar registros pagados.
4. `03_borrar_pagos_prueba_francisco.sql`: borra pagos de Enero, Febrero y Marzo de Francisco para pruebas.
5. `04_validar_un_pago_por_mes.sql`: limpia pagos duplicados y bloquea nuevos pagos repetidos por cliente y mes.

Notas:

- Si la base ya existe, revisar antes las secciones `ALTER TABLE` para evitar duplicar columnas.
- En `01_actualizacion_fecha_alta_deudores.sql` hay una seccion opcional para corregir fechas de alta reales de clientes existentes.
