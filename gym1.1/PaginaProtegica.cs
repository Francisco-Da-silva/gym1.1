using System;
using System.Web;
using System.Web.UI;
using Conexcion;

namespace gym1._1
{
    public class PaginaProtegida : Page
    {
        protected virtual bool AutorizarRol(string rol) { return true; }

        protected override void OnPreInit(EventArgs e)
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddYears(-1));
            int idUsuario, idGimnasio;
            if (!int.TryParse(Convert.ToString(Session["IdUsuario"]), out idUsuario) ||
                !int.TryParse(Convert.ToString(Session["IdGimnasio"]), out idGimnasio) ||
                idUsuario <= 0 || idGimnasio <= 0)
            {
                Response.Redirect("~/Login.aspx", true);
                return;
            }
            string rol = UsuarioDAL.ObtenerRolActivo(idUsuario, idGimnasio);
            if (rol == null)
            {
                Session.Clear();
                Session.Abandon();
                Response.Redirect("~/Login.aspx", true);
                return;
            }
            Session["Rol"] = rol;
            if (!AutorizarRol(rol))
            {
                Response.Redirect("~/Default.aspx", true);
                return;
            }
            // Token ligado a la sesión: se verifica antes de procesar eventos.
            if (Session["GymCsrf"] == null)
                Session["GymCsrf"] = Conexxion.TokenHelper.GenerarToken();
            string token = (string)Session["GymCsrf"];
            ViewStateUserKey = token;
            if (Request.HttpMethod == "POST" &&
                !string.Equals(Request.Form["__GymCsrf"], token, StringComparison.Ordinal))
                throw new HttpException(403, "Solicitud no válida.");
            base.OnPreInit(e);
            ClientScript.RegisterHiddenField("__GymCsrf", token);
        }
        protected int IdUsuarioActual { get { return Convert.ToInt32(Session["IdUsuario"]); } }
        protected int IdGimnasioActual { get { return Convert.ToInt32(Session["IdGimnasio"]); } }
        protected string NombreUsuarioActual { get { return Convert.ToString(Session["NombreUsuario"]); } }
        protected string RolActual { get { return Convert.ToString(Session["Rol"]); } }
    }
}
