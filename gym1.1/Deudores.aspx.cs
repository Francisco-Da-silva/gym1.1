using System;
using System.Collections.Generic;
using System.Globalization;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Conexxion; 

namespace gym1._1
{
    public partial class Deudores : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Defaults: desde hoy y hasta +1 mes
                txtDesde.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtHasta.Text = DateTime.Today.AddMonths(1).ToString("yyyy-MM-dd");

                try
                {
                    CargarDeudores();
                }
                catch (SqlException ex)
                {
                    Session["LastErrorTitle"] = "Error de base de datos";
                    Session["LastErrorMessage"] = ex.Message;
                    Response.Redirect("~/Error.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
                catch (Exception ex)
                {
                    Session["LastErrorTitle"] = "Error inesperado";
                    Session["LastErrorMessage"] = ex.Message;
                    Response.Redirect("~/Error.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                }
            }
        }

        private void CargarDeudores()
        {
            DataTable dt = Conexxion.DeudoresDAL.ListarDeudores();
            AgregarMesesAdeudados(dt);
            gvDeudores.DataSource = dt;
            gvDeudores.DataBind();
        }

        private void AgregarMesesAdeudados(DataTable dt)
        {
            if (!dt.Columns.Contains("MesesAdeudados"))
                dt.Columns.Add("MesesAdeudados", typeof(int));

            if (!dt.Columns.Contains("MesesAdeudadosTexto"))
                dt.Columns.Add("MesesAdeudadosTexto", typeof(string));

            if (!dt.Columns.Contains("DetalleMesesAdeudados"))
                dt.Columns.Add("DetalleMesesAdeudados", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                object fechaAlta = dt.Columns.Contains("FechaAlta") ? row["FechaAlta"] : (object)DBNull.Value;
                List<string> mesesAdeudados = ObtenerMesesAdeudados(row["Vencimiento"], fechaAlta);
                int meses = mesesAdeudados.Count;

                row["MesesAdeudados"] = meses;
                row["MesesAdeudadosTexto"] = FormatearMesesAdeudados(row["Vencimiento"], meses);
                row["DetalleMesesAdeudados"] = FormatearDetalleMeses(row["Vencimiento"], mesesAdeudados);
            }
        }

        private List<string> ObtenerMesesAdeudados(object vencimientoValue, object fechaAltaValue)
        {
            List<string> meses = new List<string>();

            if (vencimientoValue == DBNull.Value || vencimientoValue == null)
            {
                DateTime fechaAlta = ObtenerFechaAlta(fechaAltaValue);
                DateTime cursorAlta = new DateTime(fechaAlta.Year, fechaAlta.Month, 1);
                DateTime mesActualAlta = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

                while (cursorAlta <= mesActualAlta)
                {
                    meses.Add(FormatearMes(cursorAlta));
                    cursorAlta = cursorAlta.AddMonths(1);
                }

                return meses;
            }

            DateTime vencimiento = Convert.ToDateTime(vencimientoValue).Date;
            if (vencimiento >= DateTime.Today)
                return meses;

            DateTime primerDiaAdeudado = vencimiento.AddDays(1);
            DateTime cursor = new DateTime(primerDiaAdeudado.Year, primerDiaAdeudado.Month, 1);
            DateTime mesActual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            while (cursor <= mesActual)
            {
                meses.Add(FormatearMes(cursor));
                cursor = cursor.AddMonths(1);
            }

            return meses;
        }

        private DateTime ObtenerFechaAlta(object fechaAltaValue)
        {
            if (fechaAltaValue == DBNull.Value || fechaAltaValue == null)
                return DateTime.Today;

            return Convert.ToDateTime(fechaAltaValue).Date;
        }

        private string FormatearMesesAdeudados(object vencimientoValue, int meses)
        {
            if (meses == 0)
                return "0 meses";

            return meses == 1 ? "1 mes" : meses + " meses";
        }

        private string FormatearDetalleMeses(object vencimientoValue, List<string> meses)
        {
            if (meses.Count == 0)
                return "No adeuda meses";

            return string.Join(", ", meses);
        }

        private string FormatearMes(DateTime fecha)
        {
            CultureInfo cultura = new CultureInfo("es-AR");
            string mes = cultura.DateTimeFormat.GetMonthName(fecha.Month);
            return char.ToUpper(mes[0], cultura) + mes.Substring(1) + " " + fecha.Year;
        }

        private void MostrarMensaje(string texto, string tipoBootstrap)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = "alert alert-" + tipoBootstrap;
            lblMsg.Text = texto;
        }

        protected void btnMes_Click(object sender, EventArgs e)
        {
            txtDesde.Text = DateTime.Today.ToString("yyyy-MM-dd");
            txtHasta.Text = DateTime.Today.AddMonths(1).ToString("yyyy-MM-dd");
        }

        protected void gvDeudores_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName != "PAGAR") return;

            try
            {
                int rowIndex = Convert.ToInt32(e.CommandArgument);
                int idCliente = Convert.ToInt32(gvDeudores.DataKeys[rowIndex].Value);

                if (string.IsNullOrWhiteSpace(txtDesde.Text) ||
                    string.IsNullOrWhiteSpace(txtHasta.Text) ||
                    string.IsNullOrWhiteSpace(txtMonto.Text))
                {
                    MostrarMensaje("Completá Desde, Hasta y Monto antes de marcar pagado.", "warning");
                    return;
                }

                DateTime desde = DateTime.Parse(txtDesde.Text);
                DateTime hasta = DateTime.Parse(txtHasta.Text);

                if (hasta.Date < DateTime.Today)
                {
                    MostrarMensaje("No se puede registrar un pago con vencimiento en una fecha ya pasada.", "warning");
                    return;
                }

                if (hasta.Date < desde.Date)
                {
                    MostrarMensaje("La fecha Hasta no puede ser menor que la fecha Desde.", "warning");
                    return;
                }

                if (!decimal.TryParse(txtMonto.Text.Replace('.', ','), out decimal monto) || monto <= 0)
                {
                    MostrarMensaje("Monto inválido. Ej: 15000 o 15000,50", "warning");
                    return;
                }

                string obs = txtObs.Text;

                DeudoresDAL.MarcarPagoMes(idCliente, desde, hasta, monto, obs);

                MostrarMensaje("Pago registrado ✅", "success");
                CargarDeudores();
            }
            catch (SqlException ex)
            {
                Session["LastErrorTitle"] = "Error de base de datos";
                Session["LastErrorMessage"] = ex.Message;
                Response.Redirect("~/Error.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                Session["LastErrorTitle"] = "Error inesperado";
                Session["LastErrorMessage"] = ex.Message;
                Response.Redirect("~/Error.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected void gvDeudores_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow) return;

            Label lbl = (Label)e.Row.FindControl("lblEstado");
            Label lblMeses = (Label)e.Row.FindControl("lblMesesAdeudados");

            if (lblMeses != null)
            {
                int meses = Convert.ToInt32(DataBinder.Eval(e.Row.DataItem, "MesesAdeudados"));
                string estadoMeses = (DataBinder.Eval(e.Row.DataItem, "EstadoPago") ?? "").ToString().ToUpper().Trim();

                lblMeses.CssClass = "badge";
                if (estadoMeses == "SIN PAGO")
                    lblMeses.CssClass += " bg-danger";
                else if (meses == 0)
                    lblMeses.CssClass += " bg-success";
                else if (meses == 1)
                    lblMeses.CssClass += " bg-warning text-dark";
                else
                    lblMeses.CssClass += " bg-danger";
            }

            if (lbl == null) return;

            string estado = (lbl.Text ?? "").ToUpper().Trim();
            lbl.CssClass = "badge";
            switch (estado)
            {
                case "AL DÍA":
                case "AL DIA":
                    lbl.CssClass += " bg-success";
                    break;
                case "POR VENCER":
                    lbl.CssClass += " bg-warning text-dark";
                    break;
                case "VENCIDO":
                    lbl.CssClass += " bg-danger";
                    break;
                case "SIN PAGO":
                default:
                    lbl.CssClass += " bg-secondary";
                    break;
            }
        }
    }
}
