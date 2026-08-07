using System;
using System.Data.SqlClient;
using Conexxion;

namespace gym1._1
{
    public partial class CrearAdministrador : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Evita que esta página quede guardada en caché.
            Response.Cache.SetNoStore();

            if (!IsPostBack)
            {
                OcultarMensaje();
            }
        }

        protected void btnCrear_Click(object sender, EventArgs e)
        {
            OcultarMensaje();

            string nombreGimnasio = txtNombreGimnasio.Text.Trim();
            string emailGimnasio = txtEmailGimnasio.Text.Trim();
            string telefono = txtTelefono.Text.Trim();

            string nombreAdministrador = txtNombre.Text.Trim();
            string emailAdministrador = txtEmail.Text.Trim();
            string password = txtPassword.Text;
            string repetirPassword = txtRepetirPassword.Text;

            if (string.IsNullOrWhiteSpace(nombreGimnasio) ||
                string.IsNullOrWhiteSpace(emailGimnasio) ||
                string.IsNullOrWhiteSpace(nombreAdministrador) ||
                string.IsNullOrWhiteSpace(emailAdministrador) ||
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
                int idGimnasio =
                    UsuarioDAL.RegistrarGimnasioConAdministrador(
                        nombreGimnasio,
                        emailGimnasio,
                        telefono,
                        nombreAdministrador,
                        emailAdministrador,
                        password
                    );

                Response.Redirect(
                    "~/Login.aspx?registro=ok",
                    false
                );

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (InvalidOperationException ex)
            {
                // Por ejemplo: correo ya registrado.
                MostrarMensaje(
                    ex.Message,
                    "warning"
                );
            }
            catch (SqlException ex)
            {
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
                    "Error de base de datos al crear la cuenta.",
                    ex
                );
            }
            catch (Exception ex)
            {
                RedirigirAError(
                    "Error inesperado al crear la cuenta.",
                    ex
                );
            }
        }

        private void MostrarMensaje(
            string mensaje,
            string tipo)
        {
            pnlMensaje.Visible = true;
            pnlMensaje.CssClass =
                "alert alert-" + tipo;

            lblMensaje.Text =
                Server.HtmlEncode(mensaje);
        }

        private void OcultarMensaje()
        {
            pnlMensaje.Visible = false;
            lblMensaje.Text = string.Empty;
        }

        private void RedirigirAError(
            string descripcion,
            Exception ex)
        {
            string codigoError =
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8)
                    .ToUpper();

            System.Diagnostics.Trace.TraceError(
                "{0} Código: {1}. Detalle: {2}",
                descripcion,
                codigoError,
                ex
            );

            Response.Redirect(
                "~/Error.aspx?codigo=" + codigoError,
                false
            );

            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
