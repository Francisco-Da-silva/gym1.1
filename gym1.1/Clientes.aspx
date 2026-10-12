<%@ Page Title="Clientes" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Clientes.aspx.cs" Inherits="gym1._1.Clientes" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="d-flex flex-wrap justify-content-between gap-3 align-items-center mb-4">
        <div><h2 class="h4 mb-1">Tus clientes</h2><p class="text-secondary mb-0">Buscá un socio y consultá sus datos, pagos e historial.</p></div>
        <a href="Registro.aspx" class="btn btn-success"><i class="bi bi-person-plus" aria-hidden="true"></i> Registrar cliente</a>
    </div>
    <div class="main-card p-4">
        <asp:Panel ID="buscador" runat="server" DefaultButton="btnBuscar" CssClass="client-search-panel p-3 mb-4">
            <asp:Label runat="server" AssociatedControlID="clientSearch" CssClass="form-label" Text="Buscar cliente" />
            <div class="d-flex flex-wrap gap-2">
                <asp:TextBox ID="clientSearch" runat="server" ClientIDMode="Static" TextMode="Search" MaxLength="100" CssClass="form-control flex-grow-1" style="flex-basis:240px" placeholder="Nombre, apellido, DNI, teléfono, email o plan" />
                <asp:Button ID="btnBuscar" runat="server" Text="Buscar" CssClass="btn btn-success" OnClick="btnBuscar_Click" />
            </div>
        </asp:Panel>
        <p class="text-secondary small" role="status" aria-live="polite"><asp:Literal ID="resumenClientes" runat="server" Mode="Encode" /></p>
        <div class="table-responsive" tabindex="0" role="region" aria-label="Listado de clientes">
            <asp:GridView ID="gvClientes" runat="server" ClientIDMode="Static" CssClass="table table-striped align-middle mb-0" AutoGenerateColumns="false" EmptyDataText="No se encontraron clientes. Probá otra búsqueda o registrá el primero.">
                <Columns>
                    <asp:BoundField DataField="DNI" HeaderText="DNI" />
                    <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                    <asp:BoundField DataField="Apellido" HeaderText="Apellido" />
                    <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                    <asp:BoundField DataField="Email" HeaderText="Email" />
                    <asp:BoundField DataField="PlanPago" HeaderText="Plan" />
                    <asp:HyperLinkField HeaderText="Acciones" Text="Pagos e historial" DataNavigateUrlFields="IdCliente" DataNavigateUrlFormatString="~/Pagos.aspx?cliente={0}" ControlStyle-CssClass="btn btn-outline-success btn-sm" />
                </Columns>
            </asp:GridView>
        </div>
        <div class="d-flex flex-wrap align-items-center justify-content-between gap-3 mt-3">
            <asp:Button ID="btnAnterior" runat="server" Text="Anterior" CssClass="btn btn-outline-success" OnClick="btnAnterior_Click" />
            <asp:Literal ID="estadoPagina" runat="server" Mode="Encode" />
            <asp:Button ID="btnSiguiente" runat="server" Text="Siguiente" CssClass="btn btn-outline-success" OnClick="btnSiguiente_Click" />
        </div>
    </div>
</asp:Content>
