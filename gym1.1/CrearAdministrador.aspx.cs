
using Conexcion;
using System;
    using System.Data.SqlClient;

    namespace gym1._1
    {
        public partial class CrearAdministrador : PaginaSuperAdministrador
        {
            protected void Page_Load(object sender, EventArgs e)
            {
                if (!IsPostBack)
                {
                    OcultarMensaje();
                }
            }


            // ==============================================
            // REGISTRAR NUEVO GIMNASIO
            // ==============================================

            protected void btnCrear_Click(
                object sender,
                EventArgs e)
            {
                OcultarMensaje();

                // Datos del gimnasio
                string nombreGimnasio =
                    txtNombreGimnasio.Text.Trim();

                string emailGimnasio =
                    txtEmailGimnasio.Text.Trim();

                string telefono =
                    txtTelefono.Text.Trim();

                // Datos de acceso del gimnasio
                string nombreUsuario =
                    txtNombre.Text.Trim();

                string emailUsuario =
                    txtEmail.Text.Trim();

                string password =
                    txtPassword.Text;

                string repetirPassword =
                    txtRepetirPassword.Text;


                // ==========================================
                // VALIDACIONES
                // ==========================================

                if (string.IsNullOrWhiteSpace(nombreGimnasio) ||
                    string.IsNullOrWhiteSpace(emailGimnasio) ||
                    string.IsNullOrWhiteSpace(nombreUsuario) ||
                    string.IsNullOrWhiteSpace(emailUsuario) ||
                    string.IsNullOrWhiteSpace(password) ||
                    string.IsNullOrWhiteSpace(repetirPassword))
                {
                    MostrarMensaje(
                        "Completá todos los campos obligatorios.",
                        "warning"
                    );

                    return;
                }


                if (password.Length < 8)
                {
                    MostrarMensaje(
                        "La contraseña debe tener al menos 8 caracteres.",
                        "warning"
                    );

                    return;
                }


                if (password != repetirPassword)
                {
                    MostrarMensaje(
                        "Las contraseñas no coinciden.",
                        "warning"
                    );

                    return;
                }


                try
                {
                    // ======================================
                    // CREAR GIMNASIO + USUARIO DE ACCESO
                    // ======================================

                    int idGimnasio =
                        UsuarioDAL
                            .RegistrarGimnasioConAdministrador(
                                nombreGimnasio,
                                emailGimnasio,
                                telefono,
                                nombreUsuario,
                                emailUsuario,
                                password
                            );


                    // ======================================
                    // LIMPIAR FORMULARIO
                    // ======================================

                    LimpiarFormulario();


                    // ======================================
                    // MOSTRAR RESULTADO
                    // ======================================

                    MostrarMensaje(
                        "Gimnasio registrado correctamente. " +
                        "ID del gimnasio: " +
                        idGimnasio +
                        ". La cuenta ya puede iniciar sesión.",
                        "success"
                    );
                }
                catch (InvalidOperationException ex)
                {
                    // Errores esperados de negocio.
                    // Ejemplo: email ya registrado.

                    MostrarMensaje(
                        ex.Message,
                        "warning"
                    );
                }
                catch (SqlException ex)
                {
                    // Índice UNIQUE / registro duplicado
                    if (ex.Number == 2601 ||
                        ex.Number == 2627)
                    {
                        MostrarMensaje(
                            "Ya existe una cuenta con ese correo electrónico.",
                            "warning"
                        );

                        return;
                    }

                    RedirigirAError(
                        "Error de base de datos al registrar el gimnasio.",
                        ex
                    );
                }
                catch (Exception ex)
                {
                    RedirigirAError(
                        "Error inesperado al registrar el gimnasio.",
                        ex
                    );
                }
            }


            // ==============================================
            // LIMPIAR FORMULARIO
            // ==============================================

            private void LimpiarFormulario()
            {
                txtNombreGimnasio.Text =
                    string.Empty;

                txtEmailGimnasio.Text =
                    string.Empty;

                txtTelefono.Text =
                    string.Empty;

                txtNombre.Text =
                    string.Empty;

                txtEmail.Text =
                    string.Empty;

                txtPassword.Text =
                    string.Empty;

                txtRepetirPassword.Text =
                    string.Empty;
            }


            // ==============================================
            // MOSTRAR MENSAJE
            // ==============================================

            private void MostrarMensaje(
                string mensaje,
                string tipo)
            {
                pnlMensaje.Visible =
                    true;

                pnlMensaje.CssClass =
                    "alert alert-" + tipo;

                lblMensaje.Text =
                    Server.HtmlEncode(mensaje);
            }


            // ==============================================
            // OCULTAR MENSAJE
            // ==============================================

            private void OcultarMensaje()
            {
                pnlMensaje.Visible =
                    false;

                lblMensaje.Text =
                    string.Empty;
            }


            // ==============================================
            // ERROR INESPERADO
            // ==============================================

            private void RedirigirAError(
                string descripcion,
                Exception ex)
            {
                string codigoError =
                    Guid.NewGuid()
                        .ToString("N")
                        .Substring(0, 8)
                        .ToUpperInvariant();

                Conexxion.AppLogger.RegistrarError(codigoError, "CrearAdministrador.aspx", ex);

                Response.Redirect(
                    "~/Error.aspx?codigo=" +
                    Server.UrlEncode(codigoError),
                    false
                );

                Context.ApplicationInstance
                    .CompleteRequest();
            }
        }
    }