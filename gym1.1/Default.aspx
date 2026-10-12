<%@ Page Title="Inicio"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="gym1._1._Default" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <asp:PlaceHolder ID="inicioPlataforma" runat="server" Visible="false">
        <div class="main-card p-4 p-lg-5 mb-4">
            <span class="badge bg-success mb-3">Superadministrador</span>
            <h2 class="fw-bold">Administración de la plataforma</h2>
            <p class="text-secondary">Gestioná los gimnasios y sus cuentas de acceso desde un solo lugar.</p>
            <div class="d-flex flex-wrap gap-2">
                <a href="AdministrarGimnasios.aspx" class="btn btn-success">Administrar gimnasios</a>
                <a href="CrearAdministrador.aspx" class="btn btn-outline-success">Registrar gimnasio</a>
            </div>
        </div>
        <asp:Panel ID="errorResumen" runat="server" Visible="false" CssClass="alert alert-warning" role="alert">
            No pudimos cargar el resumen de gimnasios. Intentá nuevamente.
            Referencia: <asp:Literal ID="referenciaError" runat="server" Mode="Encode" />
        </asp:Panel>
        <div class="row g-3 mb-4">
            <div class="col-md-4"><div class="main-card p-4 h-100">
                <h3 class="h6 text-secondary">Gimnasios registrados</h3>
                <p class="display-6 mb-0"><asp:Literal ID="totalGimnasios" runat="server" Mode="Encode" Text="—" /></p>
            </div></div>
            <div class="col-md-4"><div class="main-card p-4 h-100">
                <h3 class="h6 text-secondary">Gimnasios activos</h3>
                <p class="display-6 text-success mb-0"><asp:Literal ID="gimnasiosActivos" runat="server" Mode="Encode" Text="—" /></p>
            </div></div>
            <div class="col-md-4"><div class="main-card p-4 h-100">
                <h3 class="h6 text-secondary">Gimnasios inactivos</h3>
                <p class="display-6 text-warning mb-0"><asp:Literal ID="gimnasiosInactivos" runat="server" Mode="Encode" Text="—" /></p>
            </div></div>
        </div>
        <div class="main-card p-4 mb-4">
            <h3 class="h5">Gimnasios y cuentas de acceso</h3>
            <p class="text-secondary">Consultá sus datos y administrá su estado de acceso. Al registrar un gimnasio se crea también su cuenta administradora.</p>
            <a href="AdministrarGimnasios.aspx" class="text-success">Ver gimnasios y cuentas</a>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="inicioGimnasio" runat="server" Visible="false">
        <div class="d-flex flex-wrap justify-content-between align-items-center gap-3 mb-4">
            <div><h2 class="h4 mb-1">Resumen del gimnasio</h2><p class="text-secondary mb-0">Tus clientes, cobros y próximos vencimientos.</p></div>
            <a href="Pagos.aspx" class="btn btn-success"><i class="bi bi-plus-circle" aria-hidden="true"></i> Registrar pago</a>
        </div>
        <asp:Panel ID="errorDashboard" runat="server" Visible="false" CssClass="alert alert-warning" role="alert"><asp:Literal ID="mensajeDashboard" runat="server" Mode="Encode" /></asp:Panel>
        <div class="row g-3 mb-4">
<div class="col-sm-6 col-xl-3"><div class="main-card p-4 h-100"><div class="d-flex justify-content-between"><h3 class="h6 text-secondary">Clientes registrados</h3><i class="bi bi-people text-success" aria-hidden="true"></i></div><p class="h2 mt-3"><asp:Literal ID="clientesRegistrados" runat="server" Mode="Encode" Text="—" /></p><p class="small text-secondary mb-0">Socios registrados en este gimnasio</p></div></div><div class="col-sm-6 col-xl-3"><div class="main-card p-4 h-100"><div class="d-flex justify-content-between"><h3 class="h6 text-secondary">Cobros del mes</h3><i class="bi bi-cash-coin text-success" aria-hidden="true"></i></div><p class="h2 mt-3"><asp:Literal ID="cobrosMes" runat="server" Mode="Encode" Text="—" /></p><p class="small text-secondary mb-0">Según la fecha de registro del pago</p></div></div><div class="col-sm-6 col-xl-3"><div class="main-card p-4 h-100"><div class="d-flex justify-content-between"><h3 class="h6 text-secondary">Próximos a vencer</h3><i class="bi bi-calendar-event text-success" aria-hidden="true"></i></div><p class="h2 mt-3"><asp:Literal ID="proximosVencer" runat="server" Mode="Encode" Text="—" /></p><p class="small text-secondary mb-0">Vencen hoy o en los próximos 5 días</p></div></div><div class="col-sm-6 col-xl-3"><div class="main-card p-4 h-100"><div class="d-flex justify-content-between"><h3 class="h6 text-secondary">Clientes deudores</h3><i class="bi bi-exclamation-triangle text-success" aria-hidden="true"></i></div><p class="h2 mt-3"><asp:Literal ID="clientesDeudores" runat="server" Mode="Encode" Text="—" /></p><p class="small text-secondary mb-0">Sin pagos o con vencimiento pasado</p></div></div>
        </div>
        <div class="d-flex flex-wrap gap-2 mb-4" aria-label="Accesos rápidos">
            <a href="Registro.aspx" class="btn btn-outline-success">Registrar cliente</a>
            <a href="Deudores.aspx" class="btn btn-outline-success">Consultar deudores</a>
            <a href="Clientes.aspx" class="btn btn-outline-success">Ver clientes</a>
        </div>
        <div class="main-card p-4 mb-4"><h3 class="h5 mb-3">Pagos recientes</h3>
            <div class="table-responsive" tabindex="0" role="region" aria-label="Pagos recientes">
                <asp:GridView ID="gvPagosRecientes" runat="server" AutoGenerateColumns="false" CssClass="table align-middle mb-0" EmptyDataText="Todavía no hay pagos registrados.">
                    <Columns><asp:BoundField DataField="Cliente" HeaderText="Cliente" /><asp:BoundField DataField="FechaPago" HeaderText="Cobrado el" DataFormatString="{0:dd/MM/yyyy}" /><asp:BoundField DataField="FechaDesde" HeaderText="Desde" DataFormatString="{0:dd/MM/yyyy}" /><asp:BoundField DataField="FechaHasta" HeaderText="Hasta" DataFormatString="{0:dd/MM/yyyy}" /><asp:BoundField DataField="Monto" HeaderText="Monto" DataFormatString="{0:N2}" /><asp:HyperLinkField Text="Ver historial" DataNavigateUrlFields="IdCliente" DataNavigateUrlFormatString="~/Pagos.aspx?cliente={0}" /></Columns>
                </asp:GridView>
            </div>
        </div>
        <div class="main-card p-4"><h3 class="h5 mb-3">Próximos vencimientos</h3>
            <div class="table-responsive" tabindex="0" role="region" aria-label="Próximos vencimientos"><asp:GridView ID="gvVencimientos" runat="server" AutoGenerateColumns="false" CssClass="table align-middle mb-0" EmptyDataText="No hay cuotas próximas a vencer."><Columns><asp:BoundField DataField="Nombre" HeaderText="Nombre" /><asp:BoundField DataField="Apellido" HeaderText="Apellido" /><asp:BoundField DataField="Vencimiento" HeaderText="Vence el" DataFormatString="{0:dd/MM/yyyy}" /><asp:HyperLinkField Text="Registrar pago" DataNavigateUrlFields="IdCliente" DataNavigateUrlFormatString="~/Pagos.aspx?cliente={0}" /></Columns></asp:GridView></div>
        </div>
    </asp:PlaceHolder>

    <section id="mi-cuenta" class="main-card p-4 mt-4">
        <h2 class="h4">Mi cuenta</h2>
        <dl class="row mb-0">
            <dt class="col-sm-3">Nombre</dt><dd class="col-sm-9"><asp:Literal ID="cuentaNombre" runat="server" Mode="Encode" /></dd>
            <dt class="col-sm-3">Correo de acceso</dt><dd class="col-sm-9"><asp:Literal ID="cuentaEmail" runat="server" Mode="Encode" /></dd>
            <dt class="col-sm-3">Tipo de cuenta</dt><dd class="col-sm-9"><asp:Literal ID="cuentaRol" runat="server" Mode="Encode" /></dd>
        </dl>
    </section>

</asp:Content>