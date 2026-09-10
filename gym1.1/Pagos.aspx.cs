
using Conexxion;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web.Script.Serialization;
using System.Web.UI.WebControls;


namespace gym1._1
{
    public partial class Pagos : PaginaProtegida
    {
        protected string ClientesJson { get; private set; } = "[]";

        protected void Page_Load(object sender, EventArgs e)
        {
            CargarClientesParaBuscador();

            if (!IsPostBack)
            {
                InicializarPantalla();
                CargarOpcionesPeriodoPago();
            }
        }

        private void InicializarPantalla()
        {
            lblEstadoActual.Text = "";
            lblEstadoActual.CssClass = "badge bg-secondary";

            lblFechaUltimoPago.Text = "-";
            lblVencimiento.Text = "-";
            lblMesesAdeudados.Text = "-";

            lblClienteSeleccionado.Text = "";
            lblClienteTitulo.Text = "Seleccioná un cliente";

            gvEstadoAnual.DataSource = null;
            gvEstadoAnual.DataBind();

            gvPagos.DataSource = null;
            gvPagos.DataBind();

            pnlMsg.Visible = false;
            lblMsg.Text = "";
        }

        private void CargarOpcionesPeriodoPago()
        {
            ddlMesPago.Items.Clear();
            ddlAnioPago.Items.Clear();

            CultureInfo cultura = new CultureInfo("es-AR");

            for (int mes = 1; mes <= 12; mes++)
            {
                string nombreMes =
                    cultura.DateTimeFormat.GetMonthName(mes);

                nombreMes =
                    char.ToUpper(nombreMes[0], cultura) +
                    nombreMes.Substring(1);

                ddlMesPago.Items.Add(
                    new ListItem(nombreMes, mes.ToString())
                );
            }

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

            ddlMesPago.SelectedValue =
                DateTime.Today.Month.ToString();

            ddlAnioPago.SelectedValue =
                anioActual.ToString();
        }

        // ==================================================
        // BUSCADOR DE CLIENTES
        // ==================================================

        private void CargarClientesParaBuscador()
        {
            // Antes estaba SP.ListarClientes(1).
            // Ahora toma el gimnasio que inició sesión.
            DataTable dt =
                SP.ListarClientes(IdGimnasioActual);

            if (dt == null || dt.Rows.Count == 0)
            {
                ClientesJson = "[]";
                return;
            }

            var clientes = dt.AsEnumerable()
                .Select(row =>
                {
                    string id =
                        row["IdCliente"].ToString();

                    string dni =
                        row["DNI"].ToString();

                    string nombre =
                        row["Nombre"].ToString();

                    string apellido =
                        row["Apellido"].ToString();

                    string telefono =
                        dt.Columns.Contains("Telefono")
                            ? row["Telefono"].ToString()
                            : "";

                    string email =
                        dt.Columns.Contains("Email")
                            ? row["Email"].ToString()
                            : "";

                    // Ahora usamos el nombre real PlanPago.
                    string planPago =
                        dt.Columns.Contains("PlanPago")
                            ? row["PlanPago"].ToString()
                            : "";

                    return new
                    {
                        id,
                        dni,
                        nombre,
                        apellido,
                        telefono,
                        email,
                        plan = planPago,

                        searchable =
                            $"{dni} {nombre} {apellido} " +
                            $"{telefono} {email} {planPago}"
                            .ToLowerInvariant()
                    };
                });

            ClientesJson =
                new JavaScriptSerializer()
                    .Serialize(clientes);
        }

        protected void btnSeleccionarCliente_Click(
            object sender,
            EventArgs e)
        {
            OcultarMensaje();

            if (!int.TryParse(
                hdnIdClienteSeleccionado.Value,
                out int idCliente))
            {
                MostrarMensaje(
                    "No se pudo identificar el cliente seleccionado.",
                    "warning"
                );

                return;
            }

            string textoCliente =
                hdnTextoClienteSeleccionado.Value;

            ViewState["IdClienteSeleccionado"] =
                idCliente;

            lblClienteSeleccionado.Text =
                Server.HtmlEncode(textoCliente);

            lblClienteTitulo.Text =
                Server.HtmlEncode(
                    ObtenerNombreClienteParaTitulo(
                        textoCliente
                    )
                );

            CargarPagosCliente(idCliente);
        }

        // ==================================================
        // PAGOS Y ESTADO DEL CLIENTE
        // ==================================================

        private void CargarPagosCliente(int idCliente)
        {
            DataTable dt =
                PagoDAL.ListarPagosPorCliente(
                    IdGimnasioActual,
                    idCliente
                );

            DateTime fechaAlta =
                SP.ObtenerFechaAltaCliente(
                    IdGimnasioActual,
                    idCliente
                );

            AgregarPeriodoCubierto(dt);

            CargarEstadoAnual(
                dt,
                fechaAlta
            );

            gvPagos.DataSource = dt;
            gvPagos.DataBind();

            if (dt != null && dt.Rows.Count > 0)
            {
                DataRow ultimo = dt.Rows[0];

                string estado =
                    ultimo["EstadoPago"] == DBNull.Value
                        ? ""
                        : ultimo["EstadoPago"]
                            .ToString()
                            .Trim();

                DateTime fechaPago =
                    Convert.ToDateTime(
                        ultimo["FechaPago"]
                    );

                DateTime fechaHasta =
                    Convert.ToDateTime(
                        ultimo["FechaHasta"]
                    );

                lblEstadoActual.Text =
                    estado.ToUpperInvariant();

                lblEstadoActual.CssClass =
                    ObtenerClaseEstado(estado);

                lblFechaUltimoPago.Text =
                    fechaPago.ToString("dd/MM/yyyy");

                lblVencimiento.Text =
                    fechaHasta.ToString("dd/MM/yyyy");

                lblMesesAdeudados.Text =
                    FormatearMesesAdeudados(
                        fechaHasta
                    );
            }
            else
            {
                lblEstadoActual.Text = "SIN PAGO";

                lblEstadoActual.CssClass =
                    "badge bg-secondary";

                lblFechaUltimoPago.Text = "-";
                lblVencimiento.Text = "-";

                lblMesesAdeudados.Text =
                    FormatearMesesAdeudadosDesdeAlta(
                        fechaAlta
                    );
            }
        }


        private string FormatearMesesAdeudadosDesdeAlta(
    DateTime fechaAlta)
        {
            List<string> meses =
                new List<string>();

            DateTime inicio =
                new DateTime(
                    fechaAlta.Year,
                    fechaAlta.Month,
                    1
                );

            DateTime actual =
                new DateTime(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1
                );

            while (inicio <= actual)
            {
                meses.Add(
                    FormatearMes(inicio)
                );

                inicio =
                    inicio.AddMonths(1);
            }

            return meses.Count > 0
                ? string.Join(", ", meses)
                : "-";
        }
        private string ObtenerClaseEstado(string estado)
        {
            switch ((estado ?? "")
                .Trim()
                .ToUpperInvariant())
            {
                case "AL DÍA":
                case "AL DIA":
                    return "badge bg-success";

                case "POR VENCER":
                    return "badge bg-warning text-dark";

                case "VENCIDO":
                    return "badge bg-danger";

                default:
                    return "badge bg-secondary";
            }
        }

        private void CargarEstadoAnual(
    DataTable pagos,
    DateTime fechaAlta)
        {
            int anioActual = DateTime.Today.Year;
            int mesActual = DateTime.Today.Month;

            // Guardamos los meses pagados
            HashSet<int> mesesPagados =
                new HashSet<int>();

            if (pagos != null)
            {
                foreach (DataRow row in pagos.Rows)
                {
                    if (row["FechaDesde"] == DBNull.Value)
                        continue;

                    DateTime fechaDesde =
                        Convert.ToDateTime(
                            row["FechaDesde"]
                        );

                    if (fechaDesde.Year == anioActual)
                    {
                        mesesPagados.Add(
                            fechaDesde.Month
                        );
                    }
                }
            }

            DataTable dtEstado =
                new DataTable();

            dtEstado.Columns.Add(
                "Anio",
                typeof(int)
            );

            string[] columnas =
            {
        "Enero",
        "Febrero",
        "Marzo",
        "Abril",
        "Mayo",
        "Junio",
        "Julio",
        "Agosto",
        "Septiembre",
        "Octubre",
        "Noviembre",
        "Diciembre"
    };

            foreach (string columna in columnas)
            {
                dtEstado.Columns.Add(
                    columna,
                    typeof(string)
                );
            }

            DataRow fila =
                dtEstado.NewRow();

            fila["Anio"] =
                anioActual;

            // Primer día del mes en el que
            // el cliente fue dado de alta
            DateTime mesFechaAlta =
                new DateTime(
                    fechaAlta.Year,
                    fechaAlta.Month,
                    1
                );

            for (int mes = 1;
                 mes <= 12;
                 mes++)
            {
                DateTime fechaMes =
                    new DateTime(
                        anioActual,
                        mes,
                        1
                    );

                string estado;

                // ==========================================
                // ANTES DEL ALTA
                // ==========================================
                if (fechaMes < mesFechaAlta)
                {
                    estado =
                        "No aplica";
                }

                // ==========================================
                // MES PAGADO
                // ==========================================
                else if (mesesPagados.Contains(mes))
                {
                    estado =
                        "Pagado";
                }

                // ==========================================
                // MES ACTUAL O ANTERIOR SIN PAGO
                // ==========================================
                else if (mes <= mesActual)
                {
                    estado =
                        "No pago";
                }

                // ==========================================
                // MES FUTURO
                // ==========================================
                else
                {
                    estado =
                        "Pendiente";
                }

                fila[columnas[mes - 1]] =
                    estado;
            }

            dtEstado.Rows.Add(fila);

            gvEstadoAnual.DataSource =
                dtEstado;

            gvEstadoAnual.DataBind();
        }

        private void AgregarPeriodoCubierto(
            DataTable dt)
        {
            if (dt == null)
            {
                return;
            }

            if (!dt.Columns.Contains(
                "PeriodoCubierto"))
            {
                dt.Columns.Add(
                    "PeriodoCubierto",
                    typeof(string)
                );
            }

            if (!dt.Columns.Contains(
                "EstadoHistorial"))
            {
                dt.Columns.Add(
                    "EstadoHistorial",
                    typeof(string)
                );
            }

            foreach (DataRow row in dt.Rows)
            {
                DateTime desde =
                    Convert.ToDateTime(
                        row["FechaDesde"]
                    );

                DateTime hasta =
                    Convert.ToDateTime(
                        row["FechaHasta"]
                    );

                row["PeriodoCubierto"] =
                    FormatearRangoMeses(
                        desde,
                        hasta
                    );

                row["EstadoHistorial"] =
                    "Pagado";
            }
        }

        private string ObtenerNombreClienteParaTitulo(
            string textoSeleccionado)
        {
            if (string.IsNullOrWhiteSpace(
                textoSeleccionado))
            {
                return "Seleccioná un cliente";
            }

            int separadorDni =
                textoSeleccionado.LastIndexOf(
                    " - ",
                    StringComparison.Ordinal
                );

            return separadorDni > 0
                ? textoSeleccionado.Substring(
                    0,
                    separadorDni
                )
                : textoSeleccionado;
        }

        private string FormatearMesesAdeudados(
            DateTime vencimiento)
        {
            List<string> meses =
                ObtenerMesesAdeudados(
                    vencimiento
                );

            return meses.Count == 0
                ? "No adeuda meses"
                : string.Join(", ", meses);
        }

        private List<string> ObtenerMesesAdeudados(
            DateTime vencimiento)
        {
            List<string> meses =
                new List<string>();

            if (vencimiento.Date >=
                DateTime.Today)
            {
                return meses;
            }

            DateTime cursor =
                new DateTime(
                    vencimiento.Year,
                    vencimiento.Month,
                    1
                ).AddMonths(1);

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

        private string FormatearRangoMeses(
            DateTime desde,
            DateTime hasta)
        {
            if (desde.Year == hasta.Year &&
                desde.Month == hasta.Month)
            {
                return FormatearMes(desde);
            }

            return FormatearMes(desde) +
                   " - " +
                   FormatearMes(hasta);
        }

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

            return char.ToUpper(
                       mes[0],
                       cultura
                   ) +
                   mes.Substring(1) +
                   " " +
                   fecha.Year;
        }

        // ==================================================
        // REGISTRAR PAGO
        // ==================================================

        protected void btnRegistrar_Click(
            object sender,
            EventArgs e)
        {
            try
            {
                OcultarMensaje();

                int idCliente;

                if (ViewState[
                        "IdClienteSeleccionado"
                    ] != null)
                {
                    idCliente = Convert.ToInt32(
                        ViewState[
                            "IdClienteSeleccionado"
                        ]
                    );
                }
                else if (int.TryParse(
                    hdnIdClienteSeleccionado.Value,
                    out int idClienteOculto))
                {
                    idCliente =
                        idClienteOculto;

                    ViewState[
                        "IdClienteSeleccionado"
                    ] = idCliente;
                }
                else
                {
                    MostrarMensaje(
                        "Seleccioná un cliente antes de registrar un pago.",
                        "warning"
                    );

                    return;
                }

                if (string.IsNullOrWhiteSpace(
                        ddlMesPago.SelectedValue) ||
                    string.IsNullOrWhiteSpace(
                        ddlAnioPago.SelectedValue) ||
                    string.IsNullOrWhiteSpace(
                        txtMonto.Text))
                {
                    MostrarMensaje(
                        "Completá mes, año y monto.",
                        "warning"
                    );

                    return;
                }

                DateTime fechaPago =
                    DateTime.Now;

                DateTime fechaDesde =
                    ObtenerFechaDesdePeriodoSeleccionado();

                DateTime fechaHasta =
                    ObtenerFechaHastaPeriodoSeleccionado(
                        fechaDesde
                    );

                bool yaExiste =
                    PagoDAL.ExistePagoEnMes(
                        IdGimnasioActual,
                        idCliente,
                        fechaDesde
                    );

                if (yaExiste)
                {
                    MostrarMensaje(
                        "Este cliente ya tiene registrado un pago para " +
                        FormatearMes(fechaDesde) +
                        ".",
                        "warning"
                    );

                    return;
                }

                if (!decimal.TryParse(
                        txtMonto.Text
                            .Trim()
                            .Replace(".", ","),
                        out decimal monto) ||
                    monto <= 0)
                {
                    MostrarMensaje(
                        "El monto es inválido. Ejemplo: 15000 o 15000,50.",
                        "warning"
                    );

                    return;
                }

                string observacion =
                    txtObs.Text.Trim();

                PagoDAL.RegistrarPago(
                    IdGimnasioActual,
                    idCliente,
                    fechaPago,
                    fechaDesde,
                    fechaHasta,
                    monto,
                    observacion
                );

                CargarPagosCliente(idCliente);
                LimpiarCamposPago();

                MostrarMensaje(
                    "Pago registrado correctamente.",
                    "success"
                );
            }
            catch (SqlException ex)
            {
                RedirigirAError(
                    "Error SQL al registrar el pago.",
                    ex
                );
            }
            catch (Exception ex)
            {
                RedirigirAError(
                    "Error inesperado al registrar el pago.",
                    ex
                );
            }
        }

        private void RedirigirAError(
            string descripcion,
            Exception ex)
        {
            string codigoError =
                Guid.NewGuid()
                    .ToString("N")
                    .Substring(0, 8)
                    .ToUpperInvariant();

            System.Diagnostics.Trace.TraceError(
                "{0} Código: {1}. Detalle: {2}",
                descripcion,
                codigoError,
                ex
            );

            Session["LastErrorTitle"] =
                "No se pudo registrar el pago";

            Session["LastErrorMessage"] =
                "Ocurrió un error al procesar la operación.";

            Response.Redirect(
                "~/Error.aspx?codigo=" +
                Server.UrlEncode(codigoError),
                false
            );

            Context.ApplicationInstance
                .CompleteRequest();
        }

        // ==================================================
        // LIMPIEZA
        // ==================================================

        protected void btnLimpiar_Click(
            object sender,
            EventArgs e)
        {
            OcultarMensaje();

            ViewState[
                "IdClienteSeleccionado"
            ] = null;

            hdnIdClienteSeleccionado.Value =
                "";

            hdnTextoClienteSeleccionado.Value =
                "";

            lblClienteSeleccionado.Text =
                "";

            lblClienteTitulo.Text =
                "Seleccioná un cliente";

            lblEstadoActual.Text =
                "";

            lblEstadoActual.CssClass =
                "badge bg-secondary";

            lblFechaUltimoPago.Text =
                "-";

            lblVencimiento.Text =
                "-";

            lblMesesAdeudados.Text =
                "-";

            gvPagos.DataSource = null;
            gvPagos.DataBind();

            gvEstadoAnual.DataSource = null;
            gvEstadoAnual.DataBind();

            LimpiarCamposPago();
        }

        private void LimpiarCamposPago()
        {
            txtMonto.Text = "";
            txtObs.Text = "";
        }

        private DateTime ObtenerFechaDesdePeriodoSeleccionado()
        {
            if (!int.TryParse(
                    ddlMesPago.SelectedValue,
                    out int mes) ||
                !int.TryParse(
                    ddlAnioPago.SelectedValue,
                    out int anio))
            {
                throw new InvalidOperationException(
                    "El período seleccionado no es válido."
                );
            }

            return new DateTime(
                anio,
                mes,
                1
            );
        }

        private DateTime ObtenerFechaHastaPeriodoSeleccionado(
            DateTime fechaDesde)
        {
            int ultimoDia =
                DateTime.DaysInMonth(
                    fechaDesde.Year,
                    fechaDesde.Month
                );

            return new DateTime(
                fechaDesde.Year,
                fechaDesde.Month,
                ultimoDia
            );
        }

        private void MostrarMensaje(
            string texto,
            string tipoBootstrap = "danger")
        {
            pnlMsg.Visible = true;

            pnlMsg.CssClass =
                "alert alert-" +
                tipoBootstrap;

            lblMsg.Text =
                Server.HtmlEncode(texto);
        }

        private void OcultarMensaje()
        {
            pnlMsg.Visible = false;
            lblMsg.Text = "";
        }

        // ==================================================
        // COLORES DE LAS GRILLAS
        // ==================================================

        protected void gvPagos_RowDataBound(
            object sender,
            GridViewRowEventArgs e)
        {
            if (e.Row.RowType !=
                DataControlRowType.DataRow)
            {
                return;
            }

            Label lblEstadoPago =
                e.Row.FindControl(
                    "lblEstadoPago"
                ) as Label;

            if (lblEstadoPago == null)
            {
                return;
            }

            string estado =
                (lblEstadoPago.Text ?? "")
                .Trim()
                .ToUpperInvariant();

            lblEstadoPago.CssClass =
                estado == "PAGADO"
                    ? "badge bg-success"
                    : "badge bg-secondary";
        }

        protected void gvEstadoAnual_RowDataBound(
            object sender,
            GridViewRowEventArgs e)
        {
            if (e.Row.RowType !=
                DataControlRowType.DataRow)
            {
                return;
            }

            for (int i = 1;
                 i < e.Row.Cells.Count;
                 i++)
            {
                string estado =
                    (e.Row.Cells[i].Text ?? "")
                    .Trim();

                string css;

                switch (estado
                    .ToUpperInvariant())
                {
                    case "PAGADO":
                        css =
                            "badge bg-success";
                        break;

                    case "NO PAGO":
                        css =
                            "badge bg-danger";
                        break;

                    default:
                        css =
                            "badge bg-secondary";
                        break;
                }

                e.Row.Cells[i].Text =
                    "<span class=\"" +
                    css +
                    "\">" +
                    Server.HtmlEncode(estado) +
                    "</span>";
            }
        }
    }
}