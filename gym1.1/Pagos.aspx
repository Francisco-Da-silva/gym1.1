<%@ Page Title="Pagos" Language="C#" MasterPageFile="~/Site.Master"
    AutoEventWireup="true" CodeBehind="Pagos.aspx.cs" Inherits="gym1._1.Pagos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <!-- CARD: FORMULARIO DE REGISTRO DE PAGO -->
    <div class="main-card p-4 mb-4">
        <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="alert alert-danger">
            <asp:Label ID="lblMsg" runat="server"></asp:Label>
        </asp:Panel>

        <h3 class="mb-4 d-flex align-items-center gap-2">
            <i class="bi bi-wallet2 text-success"></i>
            <span>Registrar pago</span>
        </h3>

        <div class="row g-3">

            <!-- BUSCADOR CENTRADO -->
            <div class="col-12 d-flex justify-content-center">
                <div class="client-search-panel p-3 w-100" style="max-width: 720px;">
                    <div class="row g-3 align-items-center">
                        <div class="col-lg-8">
                            <label for="paymentClientSearch" class="form-label text-success fw-semibold mb-2">Buscar cliente</label>
                            <div class="client-search-box">
                                <i class="bi bi-search"></i>
                                <input id="paymentClientSearch"
                                    type="search"
                                    class="form-control form-control-lg client-search-input"
                                    autocomplete="off"
                                    placeholder="Nombre, apellido, DNI, teléfono, email o plan" />
                            </div>
                        </div>
                        <div class="col-lg-4">
                            <div class="d-flex flex-lg-column gap-2 justify-content-between justify-content-lg-center text-lg-end">
                                <span class="text-secondary small">Resultados visibles</span>
                                <strong id="paymentClientResultCount" class="fs-4 text-success">0</strong>
                            </div>
                        </div>
                    </div>

                    <div id="paymentClientSuggestions" class="client-suggestions mt-3" aria-label="Sugerencias de clientes" aria-live="polite"></div>
                    <div id="paymentClientEmptySearchState" class="empty-search-state p-3 mt-3 text-center">
                        <i class="bi bi-search d-block fs-3 text-success mb-2"></i>
                        No se encontraron clientes con esa búsqueda.
                    </div>

                    <asp:HiddenField ID="hdnIdClienteSeleccionado" runat="server" />
                    <asp:HiddenField ID="hdnTextoClienteSeleccionado" runat="server" />
                    <asp:Button ID="btnSeleccionarCliente" runat="server"
                        CssClass="d-none"
                        CausesValidation="false"
                        OnClick="btnSeleccionarCliente_Click" />

                    <!-- Cliente seleccionado (solo para mostrar) -->
                    <div class="mt-2">
                        <span class="text-secondary small">Cliente seleccionado:</span>
                        <asp:Label ID="lblClienteSeleccionado" runat="server" CssClass="ms-1 fw-semibold"></asp:Label>
                    </div>
                </div>
            </div>

            <!-- Periodo -->
            <div class="col-md-3">
                <asp:Label runat="server" AssociatedControlID="ddlMesPago" CssClass="form-label">Mes a pagar</asp:Label>
                <asp:DropDownList ID="ddlMesPago" runat="server" CssClass="form-select" />
            </div>

            <div class="col-md-3">
                <asp:Label runat="server" AssociatedControlID="ddlAnioPago" class="form-label">Año</asp:Label>
                <asp:DropDownList ID="ddlAnioPago" runat="server" CssClass="form-select" />
            </div>

            <!-- Monto -->
            <div class="col-md-3">
                <asp:Label runat="server" AssociatedControlID="txtMonto" class="form-label">Monto</asp:Label>
                <asp:TextBox ID="txtMonto" runat="server" CssClass="form-control" />
            </div>

            <!-- Observación -->
            <div class="col-md-9">
                <asp:Label runat="server" AssociatedControlID="txtObs" class="form-label">Observación (opcional)</asp:Label>
                <asp:TextBox ID="txtObs" runat="server" CssClass="form-control" TextMode="MultiLine" Rows="2"  MaxLength="200" />
            </div>

        </div>

        <div class="mt-4 d-flex justify-content-end gap-2">
            <asp:Button ID="btnLimpiar" runat="server" Text="Limpiar"
                CssClass="btn btn-outline-light btn-sm"
                OnClick="btnLimpiar_Click" />

            <asp:Button ID="btnRegistrar" data-submit-once="true" runat="server" Text="Registrar pago"
                CssClass="btn btn-success px-4"
                OnClick="btnRegistrar_Click" />
        </div>
    </div>

    <!-- CARD: ESTADO Y LISTADO DE PAGOS DEL CLIENTE -->
    <div class="main-card p-4">
        <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-3">
            <div class="d-flex flex-column">
                <asp:Label ID="lblClienteTitulo" runat="server"
                    CssClass="display-6 fw-semibold lh-1 mb-2 text-white"
                    Text="Seleccioná un cliente"></asp:Label>
                <h3 class="mb-1 d-flex align-items-center gap-2">
                    <i class="bi bi-activity text-success"></i>
                    <span>Estado del cliente</span>
                </h3>
                <small class="text-secondary">
                    Último período pagado y estado respecto a la fecha de hoy.
                </small>
            </div>

            <!-- resumen de estado -->
            <div class="text-end">
                <div>
                    <span class="text-secondary small">Estado:</span>
                    <asp:Label ID="lblEstadoActual" runat="server" CssClass="badge bg-secondary ms-1"></asp:Label>
                </div>
                <div class="mt-1">
                    <span class="text-secondary small">Último pago:</span>
                    <asp:Label ID="lblFechaUltimoPago" runat="server" CssClass="ms-1"></asp:Label>
                </div>
                <div class="mt-1">
                    <span class="text-secondary small">Vence:</span>
                    <asp:Label ID="lblVencimiento" runat="server" CssClass="ms-1"></asp:Label>
                </div>
                <div class="mt-1">
                    <span class="text-secondary small">Meses adeudados:</span>
                    <asp:Label ID="lblMesesAdeudados" runat="server" CssClass="ms-1"></asp:Label>
                </div>
            </div>
        </div>

        <hr class="border-secondary" />

        <h5 class="mb-3 d-flex align-items-center gap-2">
            <i class="bi bi-calendar-check text-success"></i>
            <span>Estado mensual del año</span>
        </h5>

        <div class="table-responsive" tabindex="0" role="region" aria-label="Estado mensual"><asp:GridView ID="gvEstadoAnual" runat="server"
            CssClass="table table-bordered table-striped mb-4 text-center align-middle"
            AutoGenerateColumns="False"
            EmptyDataText="Seleccioná un cliente para ver el estado mensual."
            OnRowDataBound="gvEstadoAnual_RowDataBound">

            <Columns>
                <asp:BoundField DataField="Anio" HeaderText="Año" />
                <asp:BoundField DataField="Enero" HeaderText="Ene" />
                <asp:BoundField DataField="Febrero" HeaderText="Feb" />
                <asp:BoundField DataField="Marzo" HeaderText="Mar" />
                <asp:BoundField DataField="Abril" HeaderText="Abr" />
                <asp:BoundField DataField="Mayo" HeaderText="May" />
                <asp:BoundField DataField="Junio" HeaderText="Jun" />
                <asp:BoundField DataField="Julio" HeaderText="Jul" />
                <asp:BoundField DataField="Agosto" HeaderText="Ago" />
                <asp:BoundField DataField="Septiembre" HeaderText="Sep" />
                <asp:BoundField DataField="Octubre" HeaderText="Oct" />
                <asp:BoundField DataField="Noviembre" HeaderText="Nov" />
                <asp:BoundField DataField="Diciembre" HeaderText="Dic" />
            </Columns>

        </asp:GridView></div>

        <hr class="border-secondary" />

        <h5 class="mb-3 d-flex align-items-center gap-2">
            <i class="bi bi-receipt-cutoff text-success"></i>
            <span>Historial de pagos del cliente</span>
        </h5>

        <div class="table-responsive" tabindex="0" role="region" aria-label="Historial de pagos"><asp:GridView ID="gvPagos" runat="server"
            CssClass="table table-bordered table-striped mb-0"
            AutoGenerateColumns="False"
            EmptyDataText="Este cliente aún no tiene pagos registrados."
            OnRowDataBound="gvPagos_RowDataBound">

            <Columns>
                <asp:BoundField DataField="FechaPago" HeaderText="Realizado el" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="FechaDesde" HeaderText="Desde" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="FechaHasta" HeaderText="Hasta" DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="PeriodoCubierto" HeaderText="Meses cubiertos" />
                <asp:BoundField DataField="Monto" HeaderText="Monto" DataFormatString="{0:C}" />
                <asp:BoundField DataField="Observacion" HeaderText="Observación" HtmlEncode="true"  />

                <asp:TemplateField HeaderText="Registro">
                    <ItemTemplate>
                        <asp:Label ID="lblEstadoPago" runat="server"
                            Text='<%# Eval("EstadoHistorial") %>' CssClass="badge"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>

        </asp:GridView></div>
    </div>

    <script>
        (function () {
            const searchInput = document.getElementById('paymentClientSearch');
            const suggestions = document.getElementById('paymentClientSuggestions');
            const resultCount = document.getElementById('paymentClientResultCount');
            const emptyState = document.getElementById('paymentClientEmptySearchState');
            const selectedId = document.getElementById('<%= hdnIdClienteSeleccionado.ClientID %>');
            const selectedText = document.getElementById('<%= hdnTextoClienteSeleccionado.ClientID %>');
            const postBackTarget = '<%= btnSeleccionarCliente.UniqueID %>';
            const clients = <%= ClientesJson %>
            ;

            if (!searchInput || !suggestions || !resultCount || !emptyState || !selectedId || !selectedText) {
                return;
            }

            const escapeHtml = value => String(value || '').replace(/[&<>"']/g, char => ({
                '&': '&amp;',
                '<': '&lt;',
                '>': '&gt;',
                '"': '&quot;',
                "'": '&#39;'
            }[char]));

            const renderSuggestions = matches => {
                suggestions.innerHTML = '';

                if (!matches.length || !searchInput.value.trim()) {
                    suggestions.classList.remove('is-visible');
                    return;
                }

                matches.slice(0, 8).forEach(client => {
                    const button = document.createElement('button');
                    button.type = 'button';
                    button.className = 'btn client-suggestion';
                    button.innerHTML = '<i class="bi bi-person"></i><span>' +
                        escapeHtml((client.nombre + ' ' + client.apellido).trim()) +
                        '</span><small class="text-secondary">' + escapeHtml(client.dni) + '</small>';
                    button.addEventListener('click', () => {
                        selectedId.value = client.id;
                        selectedText.value = client.apellido + ', ' + client.nombre + ' - ' + client.dni;
                        searchInput.value = (client.nombre + ' ' + client.apellido).trim();
                        suggestions.classList.remove('is-visible');
                        __doPostBack(postBackTarget, '');
                    });
                    suggestions.appendChild(button);
                });

                suggestions.classList.add('is-visible');
            };

            function applySearch() {
                const query = searchInput.value.trim().toLowerCase();
                const matches = query
                    ? clients.filter(client => client.searchable.indexOf(query) !== -1)
                    : clients;

                resultCount.textContent = matches.length.toString();
                emptyState.classList.toggle('is-visible', query && matches.length === 0);
                renderSuggestions(matches);
            }

            searchInput.addEventListener('input', applySearch);
            searchInput.addEventListener('keydown', event => {
                if (event.key === 'Enter' && suggestions.querySelector('button')) { event.preventDefault(); suggestions.querySelector('button').click(); }
                if (event.key === 'Escape') {
                    searchInput.value = '';
                    applySearch();
                }
            });

            applySearch();
        })();
    </script>

</asp:Content>
