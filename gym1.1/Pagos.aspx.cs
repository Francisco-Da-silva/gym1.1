using Conexxion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Conexxion.SP;




namespace gym1._1
{
    public partial class Pagos : System.Web.UI.Page
    {
        protected string ClientesJson { get; private set; } = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            CargarClientesParaBuscador();

            if (!IsPostBack)
            {
                // Estado inicial
                lblEstadoActual.Text = "";
                lblEstadoActual.CssClass = "badge bg-secondary";
                lblFechaUltimoPago.Text = "-";
                lblVencimiento.Text = "-";
                lblMesesAdeudados.Text = "-";
                lblClienteSeleccionado.Text = "";
                lblClienteTitulo.Text = "Seleccioná un cliente";
                gvEstadoAnual.DataSource = null;
                gvEstadoAnual.DataBind();

                // Opcional: setear fechas por defecto
                // txtDesde.Text = DateTime.Today.ToString("yyyy-MM-dd");
                // txtHasta.Text = DateTime.Today.AddMonths(1).ToString("yyyy-MM-dd");
            }
        }

        // =========================
        // BUSCADOR DE CLIENTES
        // =========================
        private void CargarClientesParaBuscador()
        {
            DataTable dt = SP.ListarClientes();

            if (dt == null || dt.Rows.Count == 0)
            {
                ClientesJson = "[]";
                return;
            }

            var clientes = dt.AsEnumerable().Select(row =>
            {
                string id = row["IdCliente"].ToString();
                string dni = row["DNI"].ToString();
                string nombre = row["Nombre"].ToString();
                string apellido = row["Apellido"].ToString();
                string telefono = dt.Columns.Contains("Telefono") ? row["Telefono"].ToString() : "";
                string email = dt.Columns.Contains("Email") ? row["Email"].ToString() : "";
                string plan = dt.Columns.Contains("Plan") ? row["Plan"].ToString() : "";

                return new
                {
                    id,
                    dni,
                    nombre,
                    apellido,
                    telefono,
                    email,
                    plan,
                    searchable = $"{dni} {nombre} {apellido} {telefono} {email} {plan}".ToLower()
                };
            });

            ClientesJson = new JavaScriptSerializer().Serialize(clientes);
        }

        protected void btnSeleccionarCliente_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(hdnIdClienteSeleccionado.Value))
                return;

            int idCliente = int.Parse(hdnIdClienteSeleccionado.Value);
            string textoCliente = hdnTextoClienteSeleccionado.Value;

            // Guardamos el id seleccionado para usarlo al registrar pago
            ViewState["IdClienteSeleccionado"] = idCliente;

            // Mostramos el cliente seleccionado (solo visual)
            lblClienteSeleccionado.Text = textoCliente;
            lblClienteTitulo.Text = ObtenerNombreClienteParaTitulo(textoCliente);

            // Cargamos pagos + estado
            CargarPagosCliente(idCliente);
        }

        // =========================
        // CARGA DE PAGOS / ESTADO
        // =========================
        private void CargarPagosCliente(int idCliente)
        {
            DataTable dt = PagoDAL.ListarPagosPorCliente(idCliente);
            AgregarPeriodoCubierto(dt);
            CargarEstadoAnual(dt);

            gvPagos.DataSource = dt;
            gvPagos.DataBind();

            if (dt != null && dt.Rows.Count > 0)
            {
                // primer registro (debe venir ordenado por FechaHasta DESC)
                DataRow ultimo = dt.Rows[0];

                string estado = (ultimo["EstadoPago"] ?? "").ToString();
                DateTime fechaPago = Convert.ToDateTime(ultimo["FechaPago"]);
                DateTime fechaHasta = Convert.ToDateTime(ultimo["FechaHasta"]);

                lblEstadoActual.Text = estado.ToUpper();
                lblEstadoActual.CssClass = "badge";

                switch (estado.ToUpper())
                {
                    case "AL DÍA":
                    case "AL DIA":
                        lblEstadoActual.CssClass += " bg-success";
                        break;

                    case "POR VENCER":
                        lblEstadoActual.CssClass += " bg-warning text-dark";
                        break;

                    case "VENCIDO":
                        lblEstadoActual.CssClass += " bg-danger";
                        break;

                    default:
                        lblEstadoActual.CssClass += " bg-secondary";
                        break;
                }

                lblFechaUltimoPago.Text = fechaPago.ToString("dd/MM/yyyy");
                lblVencimiento.Text = fechaHasta.ToString("dd/MM/yyyy");
                lblMesesAdeudados.Text = FormatearMesesAdeudados(fechaHasta);
            }
            else
            {
                lblEstadoActual.Text = "SIN PAGO";
                lblEstadoActual.CssClass = "badge bg-secondary";
                lblFechaUltimoPago.Text = "-";
                lblVencimiento.Text = "-";
                lblMesesAdeudados.Text = FormatearMes(DateTime.Today);
            }
        }

        private void CargarEstadoAnual(DataTable pagos)
        {
            int anio = DateTime.Today.Year;
            HashSet<int> mesesPagados = new HashSet<int>();

            if (pagos != null)
            {
                foreach (DataRow row in pagos.Rows)
                {
                    DateTime desde = Convert.ToDateTime(row["FechaDesde"]);
                    if (desde.Year == anio)
                        mesesPagados.Add(desde.Month);
                }
            }

            DataTable resumen = new DataTable();
            resumen.Columns.Add("Anio", typeof(int));
            resumen.Columns.Add("Enero", typeof(string));
            resumen.Columns.Add("Febrero", typeof(string));
            resumen.Columns.Add("Marzo", typeof(string));
            resumen.Columns.Add("Abril", typeof(string));
            resumen.Columns.Add("Mayo", typeof(string));
            resumen.Columns.Add("Junio", typeof(string));
            resumen.Columns.Add("Julio", typeof(string));
            resumen.Columns.Add("Agosto", typeof(string));
            resumen.Columns.Add("Septiembre", typeof(string));
            resumen.Columns.Add("Octubre", typeof(string));
            resumen.Columns.Add("Noviembre", typeof(string));
            resumen.Columns.Add("Diciembre", typeof(string));

            DataRow fila = resumen.NewRow();
            fila["Anio"] = anio;

            string[] columnasMeses = {
                "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio",
                "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre"
            };

            for (int i = 0; i < columnasMeses.Length; i++)
            {
                int mes = i + 1;
                if (mesesPagados.Contains(mes))
                    fila[columnasMeses[i]] = "Pagado";
                else if (mes <= DateTime.Today.Month)
                    fila[columnasMeses[i]] = "No pago";
                else
                    fila[columnasMeses[i]] = "Pendiente";
            }

            resumen.Rows.Add(fila);
            gvEstadoAnual.DataSource = resumen;
            gvEstadoAnual.DataBind();
        }

        private void AgregarPeriodoCubierto(DataTable dt)
        {
            if (dt == null)
                return;

            if (!dt.Columns.Contains("PeriodoCubierto"))
                dt.Columns.Add("PeriodoCubierto", typeof(string));

            if (!dt.Columns.Contains("EstadoHistorial"))
                dt.Columns.Add("EstadoHistorial", typeof(string));

            foreach (DataRow row in dt.Rows)
            {
                DateTime desde = Convert.ToDateTime(row["FechaDesde"]);
                DateTime hasta = Convert.ToDateTime(row["FechaHasta"]);
                row["PeriodoCubierto"] = FormatearRangoMeses(desde, hasta);
                row["EstadoHistorial"] = "Pagado";
            }
        }

        private string ObtenerNombreClienteParaTitulo(string textoSeleccionado)
        {
            if (string.IsNullOrWhiteSpace(textoSeleccionado))
                return "Seleccioná un cliente";

            int separadorDni = textoSeleccionado.LastIndexOf(" - ", StringComparison.Ordinal);
            return separadorDni > 0 ? textoSeleccionado.Substring(0, separadorDni) : textoSeleccionado;
        }

        private string FormatearMesesAdeudados(DateTime vencimiento)
        {
            List<string> meses = ObtenerMesesAdeudados(vencimiento);
            return meses.Count == 0 ? "No adeuda meses" : string.Join(", ", meses);
        }

        private List<string> ObtenerMesesAdeudados(DateTime vencimiento)
        {
            List<string> meses = new List<string>();

            if (vencimiento.Date >= DateTime.Today)
                return meses;

            DateTime primerDiaAdeudado = vencimiento.Date.AddDays(1);
            DateTime cursor = new DateTime(primerDiaAdeudado.Year, primerDiaAdeudado.Month, 1);
            DateTime mesActual = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            while (cursor <= mesActual)
            {
                meses.Add(FormatearMes(cursor));
                cursor = cursor.AddMonths(1);
            }

            return meses;
        }

        private string FormatearRangoMeses(DateTime desde, DateTime hasta)
        {
            return FormatearMes(desde);
        }

        private string FormatearMes(DateTime fecha)
        {
            CultureInfo cultura = new CultureInfo("es-AR");
            string mes = cultura.DateTimeFormat.GetMonthName(fecha.Month);
            return char.ToUpper(mes[0], cultura) + mes.Substring(1) + " " + fecha.Year;
        }

        // =========================
        // REGISTRAR / LIMPIAR
        // =========================
        protected void btnRegistrar_Click(object sender, EventArgs e)
        {
            try
            {
                OcultarMensaje();

                // ✅ Validaciones "suaves" (se muestran en la misma página)
                if (ViewState["IdClienteSeleccionado"] == null && !string.IsNullOrWhiteSpace(hdnIdClienteSeleccionado.Value))
                    ViewState["IdClienteSeleccionado"] = int.Parse(hdnIdClienteSeleccionado.Value);

                if (ViewState["IdClienteSeleccionado"] == null)
                {
                    MostrarMensaje("Seleccioná un cliente antes de registrar un pago.", "warning");
                    return;
                }

                if (string.IsNullOrWhiteSpace(txtDesde.Text) ||
                    string.IsNullOrWhiteSpace(txtHasta.Text) ||
                    string.IsNullOrWhiteSpace(txtMonto.Text))
                {
                    MostrarMensaje("Completá Desde, Hasta y Monto.", "warning");
                    return;
                }

                int idCliente = (int)ViewState["IdClienteSeleccionado"];

                DateTime fechaPago = DateTime.Now;
                DateTime fechaDesde = DateTime.Parse(txtDesde.Text);
                DateTime fechaHasta = DateTime.Parse(txtHasta.Text);

                if (fechaHasta.Date < DateTime.Today)
                {
                    MostrarMensaje("No se puede registrar un pago con vencimiento en una fecha ya pasada.", "warning");
                    return;
                }

                if (fechaHasta.Date < fechaDesde.Date)
                {
                    MostrarMensaje("La fecha 'Hasta' no puede ser menor que la fecha 'Desde'.", "warning");
                    return;
                }

                if (PagoDAL.ExistePagoEnMes(idCliente, fechaDesde))
                {
                    MostrarMensaje("Este cliente ya tiene registrado un pago para " + FormatearMes(fechaDesde) + ".", "warning");
                    return;
                }

                // Monto
                if (!decimal.TryParse(txtMonto.Text.Replace('.', ','), out decimal monto) || monto <= 0)
                {
                    MostrarMensaje("El monto es inválido. Ej: 15000 o 15000,50", "warning");
                    return;
                }

                string obs = txtObs.Text;

                // ✅ Registrar en BD
                PagoDAL.RegistrarPago(idCliente, fechaPago, fechaDesde, fechaHasta, monto, obs);

                // ✅ Actualizar vista
                CargarPagosCliente(idCliente);
                LimpiarCamposPago();

                MostrarMensaje("Pago registrado correctamente ✅", "success");
            }
            catch (System.Data.SqlClient.SqlException ex)
            {
                // 🚨 errores SQL: redirigimos a Error.aspx (grave)
                Session["LastErrorTitle"] = "Error de base de datos";
                Session["LastErrorMessage"] = ex.Message;

                Response.Redirect("~/Error.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (Exception ex)
            {
                // 🚨 cualquier otro error inesperado: redirigir
                Session["LastErrorTitle"] = "Error inesperado";
                Session["LastErrorMessage"] = ex.Message;

                Response.Redirect("~/Error.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }



        protected void btnLimpiar_Click(object sender, EventArgs e)
        {
            // Limpia TODO (cliente seleccionado + grilla)
            OcultarMensaje();

            ViewState["IdClienteSeleccionado"] = null;
            hdnIdClienteSeleccionado.Value = "";
            hdnTextoClienteSeleccionado.Value = "";

            lblClienteSeleccionado.Text = "";
            lblClienteTitulo.Text = "Seleccioná un cliente";

            lblEstadoActual.Text = "";
            lblEstadoActual.CssClass = "badge bg-secondary";
            lblFechaUltimoPago.Text = "-";
            lblVencimiento.Text = "-";
            lblMesesAdeudados.Text = "-";

            gvPagos.DataSource = null;
            gvPagos.DataBind();

            gvEstadoAnual.DataSource = null;
            gvEstadoAnual.DataBind();

            LimpiarCamposPago();
        }

        private void LimpiarCamposPago()
        {
            txtDesde.Text = "";
            txtHasta.Text = "";
            txtMonto.Text = "";
            txtObs.Text = "";
        }



        private void MostrarMensaje(string texto, string tipoBootstrap = "danger")
        {
            pnlMsg.Visible = true;
            pnlMsg.CssClass = "alert alert-" + tipoBootstrap;
            lblMsg.Text = texto;
        }

        private void OcultarMensaje()
        {
            pnlMsg.Visible = false;
            lblMsg.Text = "";
        }


        // =========================
        // COLORES EN GRIDVIEW
        // =========================
        protected void gvPagos_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                Label lblEstadoPago = (Label)e.Row.FindControl("lblEstadoPago");
                if (lblEstadoPago != null)
                {
                    string estado = (lblEstadoPago.Text ?? "").ToUpper().Trim();
                    lblEstadoPago.CssClass = "badge";

                    switch (estado)
                    {
                        case "PAGADO":
                            lblEstadoPago.CssClass += " bg-success";
                            break;

                        default:
                            lblEstadoPago.CssClass += " bg-secondary";
                            break;
                    }
                }
            }
        }

        protected void gvEstadoAnual_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType != DataControlRowType.DataRow)
                return;

            for (int i = 1; i < e.Row.Cells.Count; i++)
            {
                string estado = (e.Row.Cells[i].Text ?? "").Trim();
                string css = "badge ";

                switch (estado.ToUpper())
                {
                    case "PAGADO":
                        css += "bg-success";
                        break;
                    case "NO PAGO":
                        css += "bg-danger";
                        break;
                    default:
                        css += "bg-secondary";
                        break;
                }

                e.Row.Cells[i].Text = "<span class=\"" + css + "\">" + estado + "</span>";
            }
        }
    }
}
