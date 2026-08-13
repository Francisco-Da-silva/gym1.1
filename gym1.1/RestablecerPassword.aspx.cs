using Conexxion;
using System;

namespace gym1._1
{
    public partial class RestablecerPassword :
        System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
            if (!IsPostBack)
            {
                ValidarToken();
            }
        }

        private void ValidarToken()
        {
            string token =
                Request.QueryString["token"];

            if (string.IsNullOrWhiteSpace(token))
            {
                MostrarMensaje(
                    "El enlace no es válido.",
                    "danger"
                );

                btnCambiar.Enabled = false;
                return;
            }

            string tokenHash =
                TokenHelper.ObtenerHash(token);

            int? idUsuario =
                PasswordResetDAL
                    .ObtenerUsuarioPorToken(
                        tokenHash
                    );

            if (!idUsuario.HasValue)
            {
                MostrarMensaje(
                    "El enlace no es válido o ya venció.",
                    "danger"
                );

                btnCambiar.Enabled = false;
            }
        }

        protected void btnCambiar_Click(
            object sender,
            EventArgs e)
        {
            string token =
                Request.QueryString["token"];

            if (string.IsNullOrWhiteSpace(token))
            {
                MostrarMensaje(
                    "El enlace no es válido.",
                    "danger"
                );

                return;
            }

            string password =
                txtPassword.Text;

            string repetir =
                txtRepetirPassword.Text;

            if (string.IsNullOrWhiteSpace(password))
            {
                MostrarMensaje(
                    "Ingresá una nueva contraseña.",
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

            if (password != repetir)
            {
                MostrarMensaje(
                    "Las contraseñas no coinciden.",
                    "warning"
                );

                return;
            }

            try
            {
                string tokenHash =
                    TokenHelper.ObtenerHash(token);

                int? idUsuario =
                    PasswordResetDAL
                        .ObtenerUsuarioPorToken(
                            tokenHash
                        );

                if (!idUsuario.HasValue)
                {
                    MostrarMensaje(
                        "El enlace no es válido o ya venció.",
                        "danger"
                    );

                    return;
                }

                string nuevoSalt =
                    PasswordHelper.CrearSalt();

                string nuevoHash =
                    PasswordHelper.CrearHash(
                        password,
                        nuevoSalt
                    );

                PasswordResetDAL
                    .ActualizarPassword(
                        idUsuario.Value,
                        nuevoHash,
                        nuevoSalt,
                        tokenHash
                    );

                Session.Clear();
                Session.Abandon();

                Response.Redirect(
                    "~/Login.aspx?password=ok",
                    false
                );

                Context.ApplicationInstance
                    .CompleteRequest();
            }
            catch (Exception ex)
            {
                string codigo =
                    Guid.NewGuid()
                        .ToString("N")
                        .Substring(0, 8)
                        .ToUpperInvariant();

                System.Diagnostics.Trace.TraceError(
                    "Error restableciendo contraseña. Código {0}: {1}",
                    codigo,
                    ex
                );

                Response.Redirect(
                    "~/Error.aspx?codigo=" +
                    codigo,
                    false
                );

                Context.ApplicationInstance
                    .CompleteRequest();
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
    }
}