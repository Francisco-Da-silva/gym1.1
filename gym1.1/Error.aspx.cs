
using System.Collections.Generic;
using System;

namespace gym1._1
{
    public partial class Error : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Response.Cache.SetNoStore();

            if (!IsPostBack)
            {
                string codigo = Request.QueryString["codigo"];

                if (!string.IsNullOrWhiteSpace(codigo))
                {
                    lblCodigoError.Text =
                        "Código de referencia: " + codigo;
                }
            }
        }

        protected void btnVolver_Click(object sender, EventArgs e)
        {
            if (Session["IdUsuario"] != null)
            {
                Response.Redirect("~/Default.aspx");
            }
            else
            {
                Response.Redirect("~/Login.aspx");
            }
        }
    }
}