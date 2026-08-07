using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI;
using System.Web.UI.WebControls;
using Conexxion;

namespace gym1._1
{
    public partial class Deudores : PaginaProtegida
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtDesde.Text = DateTime.Today.ToString("yyyy-MM-dd");
                txtHasta.Text = DateTime.Today.AddMonths(1)
                    .ToString("yyyy-MM-dd");

                try
                {
                    CargarDeudores();
                }
                catch (SqlException ex)
                {
                    RedirigirAError(
                        "Error de base de datos",
                        ex
                    );
                }
                catch (Exception ex)
                {
                    RedirigirAError(
                        "Error inesperado",
                        ex
                    );
                }
            }
        }

        private void CargarDeudores()
        {
            // El gimnasio se obtiene de la sesión mediante PaginaProtegida.
            DataTable dt =
                DeudoresDAL.ListarDeudores(IdGimnasioActual);

            AgregarMesesAdeudados(dt);

            gvDeudores.DataSource = dt;
            gvDeudores.DataBind();
        }

        private void AgregarMesesAdeudados(DataTable dt)
        {
            if (dt == null)
                return;

            if (!dt.Columns.Contains("MesesAdeudados"))
            {
                dt.Columns.Add(
                    "MesesAdeudados",
                    typeof(int)
                );
            }

            if (!dt.Columns.Contains("MesesAdeudadosTexto"))
            {
                dt.Columns.Add(
                    "MesesAdeudadosTexto",
                    typeof(string)
                );
            }

            if (!dt.Columns.Contains("DetalleMesesAdeudados"))
            {
                dt.Columns.Add(
                    "DetalleMesesAdeudados",
                    typeof(string)
                );
            }

            foreach (DataRow row in dt.Rows)
            {
                object fechaAlta =
                    dt.Columns.Contains("FechaAlta")
                        ? row["FechaAlta"]
                        : DBNull.Value;

                List<string> mesesAdeudados =
                    ObtenerMesesAdeudados(
                        row["Vencimiento"],
                        fechaAlta
                    );

                int cantidadMeses =
                    mesesAdeudados.Count;

                row["MesesAdeudados"] =
                    cantidadMeses;

                row["MesesAdeudadosTexto"] =
                    FormatearMesesAdeudados(
                        cantidadMeses
                    );

                row["DetalleMesesAdeudados"] =
                    FormatearDetalleMeses(
                        mesesAdeudados
                    );
            }
        }

        private List<string> ObtenerMesesAdeudados(
            object vencimientoValue,
            object fechaAltaValue)
        {
            List<string> meses =
                new List<string>();

            DateTime mesActual =
                new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1
                );

            if (vencimientoValue == null ||
                vencimientoValue == DBNull.Value)
            {
                DateTime fechaAlta =
                    ObtenerFechaAlta(fechaAltaValue);

                DateTime cursor =
                    new DateTime(
                        fechaAlta.Year,
                        fechaAlta.Month,
                        1
                    );

                while (cursor <= mesActual)
                {
                    meses.Add(FormatearMes(cursor));
                    cursor = cursor.AddMonths(1);
                }

                return meses;
            }

            DateTime vencimiento =
                Convert.ToDateTime(vencimientoValue)
                    .Date;

            if (vencimiento >= DateTime.Today)
                return meses;

            DateTime cursorVencimiento =
                new DateTime(
                    vencimiento.Year,
                    vencimiento.Month,
                    1
                ).AddMonths(1);

            while (cursorVencimiento <= mesActual)
            {
                meses.Add(
                    FormatearMes(cursorVencimiento)
                );

                cursorVencimiento =
                    cursorVencimiento.AddMonths(1);
            }

            return meses;
        }

        private DateTime ObtenerFechaAlta(
            object fechaAltaValue)
        {
            if (fechaAltaValue == null ||
                fechaAltaValue == DBNull.Value)
            {
                return DateTime.Today;
            }

            return Convert
                .ToDateTime(fechaAltaValue)
                .Date;
        }

        private string FormatearMesesAdeudados(
            int cantidadMeses)
        {
            if (cantidadMeses == 0)
                return "0 meses";

            return cantidadMeses == 1
                ? "1 mes"
                : cantidadMeses + " meses";
        }

        private string FormatearDetalleMeses(
            List<string> meses)
        {
            if (meses == null ||
                meses.Count == 0)
            {
                return "No adeuda meses";
            }

            return string.Join(", ", meses);
        }

        private string FormatearMes(DateTime fecha)
        {
            CultureInfo cultura =
                new CultureInfo("es-AR");

            string nombreMes =
                cultura.DateTimeFormat
                    .GetMonthName(fecha.Month);

            return char.ToUpper(
                       nombreMes[0],
                       cultura
                   ) +
                   nombreMes.Substring(1) +
                   " " +
                   fecha.Year;
        }

        private void MostrarMensaje(
            string texto,
            string tipoBootstrap)
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass =
                "alert alert-" + tipoBootstrap;

            lblMsg.Text =
                Server.HtmlEncode(texto);
        }

        protected void btnMes_Click(
            object sender,
            EventArgs e)
        {
            txtDesde.Text =
                DateTime.Today
                    .ToString("yyyy-MM-dd");

            txtHasta.Text =
                DateTime.Today
                    .AddMonths(1)
                    .ToString("yyyy-MM-dd");
        }

        protected void gvDeudores_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "PAGAR")
                return;

            try
            {
                int rowIndex =
                    Convert.ToInt32(
                        e.CommandArgument
                    );

                int idCliente =
                    Convert.ToInt32(
                        gvDeudores
                            .DataKeys[rowIndex]
                            .Value
                    );

                if (string.IsNullOrWhiteSpace(
                        txtDesde.Text) ||
                    string.IsNullOrWhiteSpace(
                        txtHasta.Text) ||
                    string.IsNullOrWhiteSpace(
                        txtMonto.Text))
                {
                    MostrarMensaje(
                        "Completá Desde, Hasta y Monto antes de marcar pagado.",
                        "warning"
                    );

                    return;
                }

                DateTime desde;

                if (!DateTime.TryParse(
                        txtDesde.Text,
                        out desde))
                {
                    MostrarMensaje(
                        "La fecha Desde no es válida.",
                        "warning"
                    );

                    return;
                }

                DateTime hasta;

                if (!DateTime.TryParse(
                        txtHasta.Text,
                        out hasta))
                {
                    MostrarMensaje(
                        "La fecha Hasta no es válida.",
                        "warning"
                    );

                    return;
                }

                if (hasta.Date < DateTime.Today)
                {
                    MostrarMensaje(
                        "No se puede registrar un pago con vencimiento en una fecha pasada.",
                        "warning"
                    );

                    return;
                }

                if (hasta.Date < desde.Date)
                {
                    MostrarMensaje(
                        "La fecha Hasta no puede ser menor que la fecha Desde.",
                        "warning"
                    );

                    return;
                }

                decimal monto;

                string montoNormalizado =
                    txtMonto.Text
                        .Trim()
                        .Replace(".", ",");

                if (!decimal.TryParse(
                        montoNormalizado,
                        out monto) ||
                    monto <= 0)
                {
                    MostrarMensaje(
                        "Monto inválido. Ejemplo: 15000 o 15000,50.",
                        "warning"
                    );

                    return;
                }

                string observacion =
                    txtObs.Text.Trim();

                DeudoresDAL.MarcarPagoMes(
                    IdGimnasioActual,
                    idCliente,
                    desde,
                    hasta,
                    monto,
                    observacion
                );

                MostrarMensaje(
                    "Pago registrado correctamente.",
                    "success"
                );

                CargarDeudores();
            }
            catch (SqlException ex)
            {
                RedirigirAError(
                    "Error de base de datos",
                    ex
                );
            }
            catch (Exception ex)
            {
                RedirigirAError(
                    "Error inesperado",
                    ex
                );
            }
        }

        protected void gvDeudores_RowDataBound(
            object sender,
            GridViewRowEventArgs e)
        {
            if (e.Row.RowType !=
                DataControlRowType.DataRow)
            {
                return;
            }

            Label lblEstado =
                e.Row.FindControl("lblEstado")
                as Label;

            Label lblMeses =
                e.Row.FindControl(
                    "lblMesesAdeudados"
                ) as Label;

            int meses =
                Convert.ToInt32(
                    DataBinder.Eval(
                        e.Row.DataItem,
                        "MesesAdeudados"
                    )
                );

            string estado =
                Convert.ToString(
                    DataBinder.Eval(
                        e.Row.DataItem,
                        "EstadoPago"
                    )
                )
                .ToUpper()
                .Trim();

            if (lblMeses != null)
            {
                lblMeses.CssClass = "badge";

                if (estado == "SIN PAGO")
                {
                    lblMeses.CssClass +=
                        " bg-danger";
                }
                else if (meses == 0)
                {
                    lblMeses.CssClass +=
                        " bg-success";
                }
                else if (meses == 1)
                {
                    lblMeses.CssClass +=
                        " bg-warning text-dark";
                }
                else
                {
                    lblMeses.CssClass +=
                        " bg-danger";
                }
            }

            if (lblEstado == null)
                return;

            lblEstado.CssClass = "badge";

            switch (estado)
            {
                case "AL DÍA":
                case "AL DIA":
                    lblEstado.CssClass +=
                        " bg-success";
                    break;

                case "POR VENCER":
                    lblEstado.CssClass +=
                        " bg-warning text-dark";
                    break;

                case "VENCIDO":
                    lblEstado.CssClass +=
                        " bg-danger";
                    break;

                case "SIN PAGO":
                default:
                    lblEstado.CssClass +=
                        " bg-secondary";
                    break;
            }
        }

        private void RedirigirAError(
            string titulo,
            Exception ex)
        {
            Session["LastErrorTitle"] =
                titulo;

            Session["LastErrorMessage"] =
                ex.Message;

            Response.Redirect(
                "~/Error.aspx",
                false
            );

            Context.ApplicationInstance
                .CompleteRequest();
        }
    }
}