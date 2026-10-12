using System;
using Conexxion;
namespace gym1._1
{
    public partial class Clientes : PaginaGimnasio
    {
        private int Pagina { get { return (int)(ViewState["PaginaClientes"] ?? 0); } set { ViewState["PaginaClientes"] = Math.Max(0,value); } }
        protected void Page_Load(object sender, EventArgs e) { if (!IsPostBack) CargarClientes(); }
        private void CargarClientes()
        {
            var ds = ClientesDAL.BuscarPagina(IdGimnasioActual,clientSearch.Text,Pagina);
            int total = Convert.ToInt32(ds.Tables[0].Rows[0]["Total"]);
            int paginas = Math.Max(1,(total+19)/20);
            if (Pagina >= paginas) { Pagina=paginas-1; ds=ClientesDAL.BuscarPagina(IdGimnasioActual,clientSearch.Text,Pagina); }
            gvClientes.DataSource=ds.Tables[1]; gvClientes.DataBind();
            if (gvClientes.HeaderRow != null) gvClientes.HeaderRow.TableSection=System.Web.UI.WebControls.TableRowSection.TableHeader;
            resumenClientes.Text=total+" clientes encontrados · 20 por página";
            estadoPagina.Text="Página "+(Pagina+1)+" de "+paginas;
            btnAnterior.Enabled=Pagina>0; btnSiguiente.Enabled=Pagina+1<paginas;
        }
        protected void btnBuscar_Click(object sender, EventArgs e) { Pagina=0; CargarClientes(); }
        protected void btnAnterior_Click(object sender, EventArgs e) { Pagina--; CargarClientes(); }
        protected void btnSiguiente_Click(object sender, EventArgs e) { if (btnSiguiente.Enabled) Pagina++; CargarClientes(); }
    }
}
