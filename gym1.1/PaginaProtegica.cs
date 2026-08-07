using System;
using System.Web.UI;

namespace gym1._1
{
    public class PaginaProtegida : Page
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (Session["IdUsuario"] == null ||
                Session["IdGimnasio"] == null)
            {
                Response.Redirect("~/Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }
        }

        protected int IdUsuarioActual
        {
            get
            {
                return Convert.ToInt32(Session["IdUsuario"]);
            }
        }

        protected int IdGimnasioActual
        {
            get
            {
                return Convert.ToInt32(Session["IdGimnasio"]);
            }
        }

        protected string NombreUsuarioActual
        {
            get
            {
                return Convert.ToString(Session["NombreUsuario"]);
            }
        }

        protected string RolActual
        {
            get
            {
                return Convert.ToString(Session["Rol"]);
            }
        }
    }
}