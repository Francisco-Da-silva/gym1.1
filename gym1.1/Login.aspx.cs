using Conexcion;
using Conexxion;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.SessionState;
 

namespace gym1._1
{
    public partial class Login : System.Web.UI.Page
    {
        protected override void OnInit(EventArgs e)
        {
            if (Session["LoginCsrf"] == null)
                Session["LoginCsrf"] = TokenHelper.GenerarToken();
            ViewStateUserKey = (string)Session["LoginCsrf"];
            base.OnInit(e);
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            if (!IsPostBack)
            {
                OcultarError();
                if (InicioSesionSeguro.Completar(Context))
                {
                    Response.Redirect("~/Default.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                if (Session["IdUsuario"] != null &&
                    Session["IdGimnasio"] != null)
                {
                    Response.Redirect("~/Default.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            OcultarError();

            try
            {
                string email = txtEmail.Text.Trim();
                string password = txtPassword.Text;

                if (email.Length > 100 || password.Length > 100 ||
                    string.IsNullOrWhiteSpace(email) ||
                    string.IsNullOrWhiteSpace(password))
                {
                    MostrarError("Ingresá el correo electrónico y la contraseña.");
                    return;
                }

                UsuarioSesion usuario = UsuarioDAL.BuscarPorEmail(email);

                if (usuario == null)
                {
                    MostrarError("Correo electrónico o contraseña incorrectos.");
                    return;
                }

                bool passwordCorrecta = PasswordHelper.VerificarPassword(
                    password,
                    usuario.PasswordSalt,
                    usuario.PasswordHash
                );

                if (!passwordCorrecta)
                {
                    MostrarError("Correo electrónico o contraseña incorrectos.");
                    return;
                }

                if (!usuario.Activo)
                {
                    MostrarError("No podés ingresar porque tu cuenta actualmente se encuentra inactiva. Contactá al administrador de la plataforma.");
                    return;
                }

                InicioSesionSeguro.Preparar(Context, usuario);
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                string codigoError = Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8)
                    .ToUpper();

                Conexxion.AppLogger.RegistrarError(codigoError, "Login.aspx", ex);

                Response.Redirect(
                    "~/Error.aspx?codigo=" + Server.UrlEncode(codigoError),
                    false
                );

                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            pnlError.Visible = true;
        }

        private void OcultarError()
        {
            lblError.Text = string.Empty;
            pnlError.Visible = false;
        }

    }
}
