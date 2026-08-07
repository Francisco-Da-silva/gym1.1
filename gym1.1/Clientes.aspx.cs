using Conexxion;
using System;

namespace gym1._1
{
    public partial class Clientes : PaginaProtegida
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarClientes();
            }
        }

        private void CargarClientes()
        {
            gvClientes.DataSource =
                SP.ListarClientes(IdGimnasioActual);

            gvClientes.DataBind();
        }
    }
}

