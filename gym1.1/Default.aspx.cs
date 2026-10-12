using System;
using System.Data;
using Conexcion;
using Conexxion;

namespace gym1._1
{
    public partial class _Default : PaginaProtegida
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            bool esSuper = string.Equals(RolActual.Trim(), "SuperAdministrador", StringComparison.OrdinalIgnoreCase);
            inicioPlataforma.Visible = esSuper;
            inicioGimnasio.Visible = !esSuper;
            Title = esSuper ? "Administración de la plataforma" : "Inicio";
            cuentaNombre.Text = NombreUsuarioActual;
            cuentaEmail.Text = Convert.ToString(Session["EmailUsuario"]);
            cuentaRol.Text = esSuper ? "Superadministrador" : "Administrador de gimnasio";
            if (!IsPostBack) { if (esSuper) CargarResumen(); else CargarDashboard(); }
        }

        private void CargarDashboard()
        {
            try
            {
                var ds=DashboardDAL.Obtener(IdGimnasioActual);
                var vencimientos=ds.Tables[0].Clone();
                int proximos=0,deudores=0;
                var filas=System.Linq.Enumerable.OrderBy(ds.Tables[0].AsEnumerable(), r=> r["Vencimiento"]==DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(r["Vencimiento"]));
                foreach(var fila in filas)
                {
                    string estado=Convert.ToString(fila["EstadoPago"]).Trim();
                    if (estado=="POR VENCER") { proximos++; if(vencimientos.Rows.Count<5) vencimientos.ImportRow(fila); }
                    else if (estado=="VENCIDO" || estado=="SIN PAGO") deudores++;
                }
                clientesRegistrados.Text=Convert.ToString(ds.Tables[1].Rows[0]["Total"]);
                cobrosMes.Text=Convert.ToDecimal(ds.Tables[2].Rows[0]["Total"]).ToString("C",new System.Globalization.CultureInfo("es-AR"));
                proximosVencer.Text=proximos.ToString(); clientesDeudores.Text=deudores.ToString();
                gvPagosRecientes.DataSource=ds.Tables[3]; gvPagosRecientes.DataBind();
                gvVencimientos.DataSource=vencimientos; gvVencimientos.DataBind();
            }
            catch(Exception ex)
            {
                string codigo=Guid.NewGuid().ToString("N").Substring(0,8).ToUpperInvariant();
                AppLogger.RegistrarError(codigo,"Default.CargarDashboard",ex);
                errorDashboard.Visible=true; mensajeDashboard.Text="No pudimos cargar el resumen. Intentá nuevamente. Referencia: "+codigo;
            }
        }

        private void CargarResumen()
        {
            try
            {
                // Un gimnasio puede tener varias cuentas; se cuenta una sola vez.
                DataTable gimnasios = UsuarioDAL.ListarGimnasios().DefaultView.ToTable(true, "IdGimnasio", "Activo");
                int activos = 0;
                foreach (DataRow gimnasio in gimnasios.Rows)
                    if (Convert.ToBoolean(gimnasio["Activo"])) activos++;
                totalGimnasios.Text = gimnasios.Rows.Count.ToString();
                gimnasiosActivos.Text = activos.ToString();
                gimnasiosInactivos.Text = (gimnasios.Rows.Count - activos).ToString();
            }
            catch (Exception ex)
            {
                string codigo = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpperInvariant();
                AppLogger.RegistrarError(codigo, "Default.CargarResumen", ex);
                referenciaError.Text = codigo;
                errorResumen.Visible = true;
            }
        }
    }
}
