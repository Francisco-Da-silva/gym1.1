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
            if (esSuper && !IsPostBack) CargarResumen();
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
