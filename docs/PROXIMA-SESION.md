# Pendientes para retomar — Gym Manager

## Meses adeudados — pedido del usuario

Pendiente para la próxima sesión, posiblemente mañana. No implementar en esta entrega.

Recuperar una metodología similar a la anterior: al seleccionar un cliente, mostrar todos los meses del año e identificar claramente cuáles tiene pagos y cuáles adeuda. Conservar el historial, los pagos futuros y el aislamiento entre gimnasios.

Antes de modificar la lógica, revisar el estado mensual existente en Pagos.aspx y su cálculo de deuda. Mantener las reglas de períodos y fecha de alta; acordar cualquier diferencia funcional al retomar. El pedido es recuperar la visualización completa de meses y la deuda por cliente.

## Estado técnico al cerrar

- Se conserva la separación del SuperAdministrador: panel administrativo y permisos exclusivos, sin operaciones de gimnasio.
- Rediseño y corrección de logout en la rama `codex/redesign-gym-manager`.
- Logout sin consultas SQL, con validación CSRF. Compilación correcta y regresión específica + separación de roles aprobadas.
- Error de login EB5DC5A4: SQL Express no respondía; eventos 701 por falta de memoria y fallos de tareas internas. Se recuperó la instancia tras un reinicio bloqueado.
- GYM_DB quedó ONLINE, AUTO_CLOSE desactivado y consultas de usuarios respondieron. Se configuró max server memory (MB) en 512; el valor previo era 2147483647. Este ajuste es de la instancia local, no una migración del repositorio ni una garantía de que no vuelva a existir presión de memoria.
- CHECKDB PHYSICAL_ONLY agotó el tiempo de espera y fue cancelado: comprobación de integridad pendiente, no aprobada.
- No se comprobó el login real con contraseña después de recuperar SQL; pedir al usuario confirmar el resultado al retomar si no lo confirmó antes.
- Instancias LocalDB QA detenidas. No copiar sus datos ni credenciales a la base real.

Consultar `docs/Redesign/ENTREGA.md` y la auditoría previa para evidencias y pendientes anteriores de infraestructura, respaldo y credenciales.
