using Conexxion;
using System;

namespace gym1._1
{
    public partial class RecuperarPassword :
        System.Web.UI.Page
    {
        protected void Page_Load(
            object sender,
            EventArgs e)
        {
        }

        protected void btnEnviar_Click(
            object sender,
            EventArgs e)
        {
            pnlMensaje.Visible = false;

            string email =
                txtEmail.Text.Trim();

            if (string.IsNullOrWhiteSpace(email))
            {
                MostrarMensaje(
                    "Ingresá tu correo electrónico.",
                    "warning"
                );

                return;
            }

            try
            {
                UsuarioSesion usuario =
                    PasswordResetDAL
                        .BuscarUsuarioPorEmail(email);

                if (usuario != null)
                {
                    string token =
                        TokenHelper.GenerarToken();

                    string tokenHash =
                        TokenHelper.ObtenerHash(token);

                    DateTime vencimiento =
                        DateTime.Now.AddMinutes(30);

                    PasswordResetDAL.GuardarToken(
                        usuario.IdUsuario,
                        tokenHash,
                        vencimiento
                    );

                    string enlace =
                        Request.Url.GetLeftPart(
                            UriPartial.Authority
                        ) +
                        ResolveUrl(
                            "~/RestablecerPassword.aspx"
                        ) +
                        "?token=" +
                        Server.UrlEncode(token);

                    EmailService.EnviarRecuperacionPassword(
                        usuario.Email,
                        usuario.Nombre,
                        enlace
                    );
                }

                MostrarMensaje(
                    "Si el correo está registrado, te enviaremos un enlace para restablecer tu contraseña.",
                    "success"
                );

            }




            catch (Exception ex)
            {
                string codigoError =
                    Guid.NewGuid()
                        .ToString("N")
                        .Substring(0, 8)
                        .ToUpperInvariant();

                System.Diagnostics.Trace.TraceError(
                    "Error recuperación contraseña. Código: {0}. Detalle: {1}",
                    codigoError,
                    ex
                );

                MostrarMensaje(
                    "No se pudo completar la operación. Código: " + codigoError,
                    "danger"
                );
            }
        }

        //catch (Exception ex)
        //{
        //    MostrarMensaje(
        //        "ERROR: " +
        //        ex.GetType().FullName +
        //        "<br/>" +
        //        Server.HtmlEncode(ex.Message),
        //        "danger"
        //    );
        //}


    

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