using Conexxion;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;




namespace gym1._1
{
    public partial class Registro : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                CargarClientes();
            }
        }

        protected void btnAgregar_Click(object sender, EventArgs e)
        {
            SP.AgregarCliente(
                txtNombre.Text,
                txtApellido.Text,
                txtDni.Text,
                txtTelefono.Text,
                txtEmail.Text,
                DateTime.Parse(txtFechaNac.Text),
                ddlPlan.SelectedValue
            );

            CargarClientes();
        }

        private void CargarClientes()
        {
            gvClientes.DataSource = SP.ListarClientes();
            gvClientes.DataBind();
        }

        protected void gvClientes_RowDeleting(object sender, System.Web.UI.WebControls.GridViewDeleteEventArgs e)
        {
            int id = Convert.ToInt32(gvClientes.DataKeys[e.RowIndex].Value);

            string cn = "Server=.;Database=GYM_DB;Trusted_Connection=True";

            using (SqlConnection con = new SqlConnection(cn))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM Clientes WHERE IdCliente=@id", con);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
            }

            CargarClientes();
        }
    }
}
