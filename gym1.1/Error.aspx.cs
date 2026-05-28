using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace gym1._1
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string titulo = Session["LastErrorTitle"] as string;
            string mensaje = Session["LastErrorMessage"] as string;

            lblTitulo.Text = string.IsNullOrWhiteSpace(titulo) ? "Error" : titulo;
            lblMensaje.Text = string.IsNullOrWhiteSpace(mensaje)
                ? "Se produjo un error inesperado."
                : mensaje;

            // opcional: limpiar para que no quede pegado el error
            Session.Remove("LastErrorTitle");
            Session.Remove("LastErrorMessage");
        }
    }
}
