using Conexxion;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.SessionState;
 

namespace gym1._1
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);

            if (!IsPostBack)
            {
                OcultarError();

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

                if (string.IsNullOrWhiteSpace(email) ||
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

                Session.Clear();

                Session["IdUsuario"] = usuario.IdUsuario;
                Session["IdGimnasio"] = usuario.IdGimnasio;
                Session["NombreUsuario"] = usuario.Nombre;
                Session["NombreGimnasio"] = usuario.NombreGimnasio;
                Session["EmailUsuario"] = usuario.Email;
                Session["Rol"] = usuario.Rol;

                Response.Redirect("~/Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                string codigoError = Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8)
                    .ToUpper();

                System.Diagnostics.Trace.TraceError(
                    "Error al iniciar sesión. Código: {0}. Detalle: {1}",
                    codigoError,
                    ex
                );

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

        private void RegenerarIdSesion()
        {
            SessionIDManager administradorSesion =
                new SessionIDManager();

            string nuevoId =
                administradorSesion.CreateSessionID(Context);

            bool redireccionar;
            bool agregarCookie;

            administradorSesion.SaveSessionID(
                Context,
                nuevoId,
                out redireccionar,
                out agregarCookie);
        }


    }

}
