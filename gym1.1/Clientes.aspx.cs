using System;
using System.Data;
using Conexxion;

namespace gym1._1
{
    public partial class Clientes : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                CargarClientes();
        }

        private void CargarClientes()
        {
            gvClientes.DataSource = SP.ListarClientes();
            gvClientes.DataBind();
        }
    }
}
