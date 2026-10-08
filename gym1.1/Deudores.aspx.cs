
using Conexxion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.UI.WebControls;

namespace gym1._1
{
    public partial class Deudores : PaginaProtegida
    {  
        protected void Page_Load(object sender, EventArgs e)
        {
            txtObs.Attributes["maxlength"] = "200";
            if (!IsPostBack)
            {
                CargarOpcionesPeriodoPago();
                CargarDeudores();
                OcultarMensaje();
            }
        }

        // =====================================================
        // CARGAR MESES Y AÑOS
        // =====================================================

        private void CargarOpcionesPeriodoPago()
        {
            ddlMesPago.Items.Clear();
            ddlAnioPago.Items.Clear();

            CultureInfo cultura =
                new CultureInfo("es-AR");

            // Meses
            for (int mes = 1; mes <= 12; mes++)
            {
                string nombreMes =
                    cultura.DateTimeFormat.GetMonthName(mes);

                nombreMes =
                    char.ToUpper(
                        nombreMes[0],
                        cultura
                    ) +
                    nombreMes.Substring(1);

                ddlMesPago.Items.Add(
                    new ListItem(
                        nombreMes,
                        mes.ToString()
                    )
                );
            }

            // Años
            int anioActual = DateTime.Today.Year;

            for (int anio = anioActual - 2;
                 anio <= anioActual + 1;
                 anio++)
            {
                ddlAnioPago.Items.Add(
                    new ListItem(
                        anio.ToString(),
                        anio.ToString()
                    )
                );
            }

            // Período actual por defecto
            ddlMesPago.SelectedValue =
                DateTime.Today.Month.ToString();

            ddlAnioPago.SelectedValue =
                anioActual.ToString();
        }

        // =====================================================
        // CARGAR DEUDORES
        // =====================================================

        private void CargarDeudores()
        {
            try
            {
                DataTable dt =
                    DeudoresDAL.ListarDeudores(
                        IdGimnasioActual
                    );

                AgregarDatosMesesAdeudados(dt);

                gvDeudores.DataSource = dt;
                gvDeudores.DataBind();
            }
            catch (Exception ex)
            {
                string codigoError = RegistrarError(
                    "Error al cargar deudores",
                    ex
                );

                MostrarMensaje(
                    "No se pudo cargar el listado de deudores. Código: " + codigoError,
                    "danger"
                );
            }
        }


        private void AgregarDatosMesesAdeudados(DataTable dt)
        {
            if (dt == null)
                return;

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
                DateTime? vencimiento = null;

                if (dt.Columns.Contains("Vencimiento") &&
                    row["Vencimiento"] != DBNull.Value)
                {
                    vencimiento =
                        Convert.ToDateTime(
                            row["Vencimiento"]
                        );
                }

                List<string> meses =
                    ObtenerMesesAdeudados(vencimiento, Convert.ToDateTime(row["FechaAlta"]));

                row["MesesAdeudadosTexto"] =
                    meses.Count == 0
                        ? "0"
                        : meses.Count.ToString();

                row["DetalleMesesAdeudados"] =
                    meses.Count == 0
                        ? "No adeuda"
                        : string.Join(", ", meses);
            }
        }

        private List<string> ObtenerMesesAdeudados(
    DateTime? vencimiento, DateTime fechaAlta)
        {
            List<string> meses =
                new List<string>();

            if (vencimiento.HasValue && vencimiento.Value.Date >= DateTime.Today)
                return meses;
            DateTime inicio = vencimiento.HasValue ? vencimiento.Value : fechaAlta;
            DateTime cursor = new DateTime(inicio.Year, inicio.Month, 1);
            if (vencimiento.HasValue) cursor = cursor.AddMonths(1);

            DateTime mesActual =
                new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1
                );

            while (cursor <= mesActual)
            {
                meses.Add(
                    FormatearMes(cursor)
                );

                cursor =
                    cursor.AddMonths(1);
            }

            return meses;
        }
        // =====================================================
        // BOTÓN "MARCAR PAGADO"
        // =====================================================

        protected void gvDeudores_RowCommand(
            object sender,
            GridViewCommandEventArgs e)
        {
            if (e.CommandName != "PAGAR")
                return;

            OcultarMensaje();

            try
            {
                // CommandArgument contiene el índice
                int rowIndex;

                if (!int.TryParse(
                    e.CommandArgument.ToString(),
                    out rowIndex))
                {
                    MostrarMensaje(
                        "No se pudo identificar al cliente.",
                        "warning"
                    );

                    return;
                }

                if (rowIndex < 0 ||
                    rowIndex >= gvDeudores.Rows.Count)
                {
                    MostrarMensaje(
                        "El cliente seleccionado no es válido.",
                        "warning"
                    );

                    return;
                }

                // Obtenemos IdCliente desde DataKeys
                int idCliente =
                    Convert.ToInt32(
                        gvDeudores.DataKeys[rowIndex].Value
                    );

                // =============================================
                // VALIDAR PERÍODO
                // =============================================

                if (string.IsNullOrWhiteSpace(
                        ddlMesPago.SelectedValue) ||
                    string.IsNullOrWhiteSpace(
                        ddlAnioPago.SelectedValue))
                {
                    MostrarMensaje(
                        "Seleccioná el mes y el año del pago.",
                        "warning"
                    );

                    return;
                }

                int mes;
                int anio;

                if (!int.TryParse(
                        ddlMesPago.SelectedValue,
                        out mes) ||
                    !int.TryParse(
                        ddlAnioPago.SelectedValue,
                        out anio))
                {
                    MostrarMensaje(
                        "El período seleccionado no es válido.",
                        "warning"
                    );

                    return;
                }

                if (mes < 1 || mes > 12 || anio < DateTime.Today.Year - 2 || anio > DateTime.Today.Year + 1)
                {
                    MostrarMensaje("El período seleccionado no es válido.", "warning");
                    return;
                }
                DateTime fechaDesde =
                    new DateTime(
                        anio,
                        mes,
                        1
                    );

                DateTime fechaHasta =
                    new DateTime(
                        anio,
                        mes,
                        DateTime.DaysInMonth(
                            anio,
                            mes
                        )
                    );

                DateTime fechaPago =
                    DateTime.Now;

                // =============================================
                // VALIDAR MONTO
                // =============================================

                if (string.IsNullOrWhiteSpace(
                    txtMonto.Text))
                {
                    MostrarMensaje(
                        "Ingresá el monto del pago.",
                        "warning"
                    );

                    txtMonto.Focus();
                    return;
                }

                decimal monto;

                if (!ValidacionDatos.TryMonto(txtMonto.Text, out monto))
                {
                    MostrarMensaje("El monto es inválido. Usá hasta dos decimales y un máximo de 9999999999,99.", "warning");
                    return;
                }

                // =============================================
                // VALIDAR PAGO DUPLICADO
                // =============================================

                bool existePago =
                    PagoDAL.ExistePagoEnMes(
                        IdGimnasioActual,
                        idCliente,
                        fechaDesde
                    );

                if (existePago)
                {
                    MostrarMensaje(
                        "Este cliente ya tiene registrado el pago de " +
                        FormatearMes(fechaDesde) +
                        ".",
                        "warning"
                    );

                    return;
                }

                // =============================================
                // REGISTRAR PAGO
                // =============================================

                string observacion =
                    txtObs.Text.Trim();

                string errorPago = ValidacionDatos.ErrorPago(monto, observacion);
                if (errorPago != null)
                {
                    MostrarMensaje(errorPago, "warning");
                    return;
                }
                PagoDAL.RegistrarPago(
                    IdGimnasioActual,
                    idCliente,
                    fechaPago,
                    fechaDesde,
                    fechaHasta,
                    monto,
                    observacion
                );

                // Recargamos la grilla.
                CargarDeudores();

                LimpiarCamposPago();

                MostrarMensaje(
                    "Pago de " +
                    FormatearMes(fechaDesde) +
                    " registrado correctamente.",
                    "success"
                );
            }
            catch (SqlException ex)
            {
                // Pago duplicado desde SQL
                if (ex.Number == 50002 || ex.Number == 2601 || ex.Number == 2627)
                {
                    MostrarMensaje(
                        "Ese mes ya se encuentra registrado como pagado.",
                        "warning"
                    );

                    return;
                }

                // Cliente no pertenece al gimnasio
                if (ex.Number == 50001)
                {
                    MostrarMensaje(
                        "No se pudo validar el cliente seleccionado.",
                        "danger"
                    );

                    return;
                }

                string codigoError = RegistrarError(
                    "Error SQL al registrar pago desde Deudores",
                    ex
                );

                MostrarMensaje(
                    "No se pudo registrar el pago. Código: " + codigoError,
                    "danger"
                );
            }
            catch (ArgumentException ex)
            {
                MostrarMensaje(ex.Message, "warning");
            }
            catch (Exception ex)
            {
                string codigoError = RegistrarError(
                    "Error inesperado al registrar pago desde Deudores",
                    ex
                );

                MostrarMensaje(
                    "Ocurrió un error inesperado al registrar el pago. Código: " + codigoError,
                    "danger"
                );
            }
        }

        // =====================================================
        // COLORES / ESTADO GRIDVIEW
        // =====================================================

        protected void gvDeudores_RowDataBound(
            object sender,
            GridViewRowEventArgs e)
        {
            if (e.Row.RowType !=
                DataControlRowType.DataRow)
            {
                return;
            }

            // Estado
            Label lblEstado =
                e.Row.FindControl(
                    "lblEstado"
                ) as Label;

            if (lblEstado != null)
            {
                string estado =
                    (lblEstado.Text ?? "")
                    .Trim()
                    .ToUpperInvariant();

                switch (estado)
                {
                    case "AL DÍA":
                    case "AL DIA":

                        lblEstado.CssClass =
                            "badge bg-success";
                        break;

                    case "POR VENCER":

                        lblEstado.CssClass =
                            "badge bg-warning text-dark";
                        break;

                    case "VENCIDO":

                        lblEstado.CssClass =
                            "badge bg-danger";
                        break;

                    case "SIN PAGO":

                        lblEstado.CssClass =
                            "badge bg-secondary";
                        break;

                    default:

                        lblEstado.CssClass =
                            "badge bg-secondary";
                        break;
                }
            }

            // Meses adeudados
            Label lblMeses =
                e.Row.FindControl(
                    "lblMesesAdeudados"
                ) as Label;

            if (lblMeses != null)
            {
                string texto =
                    (lblMeses.Text ?? "")
                    .Trim();

                if (texto == "0" ||
                    texto.Equals(
                        "No adeuda",
                        StringComparison.OrdinalIgnoreCase))
                {
                    lblMeses.CssClass =
                        "badge bg-success";
                }
                else
                {
                    lblMeses.CssClass =
                        "badge bg-danger";
                }
            }
        }

        // =====================================================
        // UTILIDADES
        // =====================================================

        private string FormatearMes(
            DateTime fecha)
        {
            CultureInfo cultura =
                new CultureInfo("es-AR");

            string mes =
                cultura.DateTimeFormat
                    .GetMonthName(
                        fecha.Month
                    );

            mes =
                char.ToUpper(
                    mes[0],
                    cultura
                ) +
                mes.Substring(1);

            return mes +
                   " " +
                   fecha.Year;
        }

        private void LimpiarCamposPago()
        {
            txtMonto.Text = "";
            txtObs.Text = "";

            // Dejamos seleccionado el período actual.
            if (ddlMesPago.Items.Count > 0)
            {
                ddlMesPago.SelectedValue =
                    DateTime.Today.Month.ToString();
            }

            if (ddlAnioPago.Items.Count > 0)
            {
                ddlAnioPago.SelectedValue =
                    DateTime.Today.Year.ToString();
            }
        }

        // =====================================================
        // MENSAJES
        // =====================================================

        private void MostrarMensaje(
            string mensaje,
            string tipoBootstrap)
        {
            pnlMsg.Visible = true;

            pnlMsg.CssClass =
                "alert alert-" +
                tipoBootstrap;

            lblMsg.Text =
                Server.HtmlEncode(mensaje);
        }

        private void OcultarMensaje()
        {
            pnlMsg.Visible = false;
            lblMsg.Text = "";
        }

        // =====================================================
        // REGISTRO DE ERRORES
        // =====================================================

        private string RegistrarError(
            string descripcion,
            Exception ex)
        {
            string codigoError =
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8)
                    .ToUpperInvariant();

            Conexxion.AppLogger.RegistrarError(codigoError, "Deudores.aspx", ex);
            return codigoError;
        }
    }
}