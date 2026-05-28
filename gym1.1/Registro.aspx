<%@ Page Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Registro.aspx.cs" Inherits="gym1._1.Registro" %>


<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <h2 class="my-3 d-flex align-items-center gap-2">
        <i class="bi bi-person-plus text-success"></i>
        <span class="text-success">Gestión de Clientes</span>
    </h2>

    <!-- CARD FORMULARIO ALTA CLIENTE -->
    <div class="main-card p-4 mb-4">
       <%-- <h3 class="mb-4 d-flex align-items-center gap-2">
            <i class="bi bi-person-plus-fill text-success"></i>
            <span>Agregar Cliente</span>
        </h3>--%>

        <div class="row g-3">

            <div class="col-md-6">
                <label class="form-label">Nombre</label>
                <asp:TextBox ID="txtNombre" runat="server" CssClass="form-control" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Apellido</label>
                <asp:TextBox ID="txtApellido" runat="server" CssClass="form-control" />
            </div>

            <div class="col-md-6">
                <label class="form-label">DNI</label>
                <asp:TextBox ID="txtDni" runat="server" CssClass="form-control" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Teléfono</label>
                <asp:TextBox ID="txtTelefono" runat="server" CssClass="form-control" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="form-control" TextMode="Email" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Fecha de Nacimiento</label>
                <asp:TextBox ID="txtFechaNac" runat="server" CssClass="form-control" TextMode="Date" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Plan</label>
                <asp:DropDownList ID="ddlPlan" runat="server" CssClass="form-select">
                    <asp:ListItem Value="3xSemana">3 veces por semana</asp:ListItem>
                    <asp:ListItem Value="Full">Full</asp:ListItem>
                </asp:DropDownList>
            </div>

        </div>

        <div class="mt-4 d-flex justify-content-end">
            <asp:Button ID="btnAgregar" runat="server" Text="Agregar Cliente"
                CssClass="btn btn-success px-4" OnClick="btnAgregar_Click" />
        </div>
    </div>

    <!-- CARD LISTADO CLIENTES -->
    <div class="main-card p-4">
        <h3 class="mb-3 d-flex align-items-center gap-2">
            <i class="bi bi-people-fill text-success"></i>
            <span>Clientes Registrados</span>
        </h3>

        <asp:GridView ID="gvClientes" runat="server"
            CssClass="table table-bordered table-striped mb-0"
            AutoGenerateColumns="false"
            DataKeyNames="IdCliente"
            OnRowDeleting="gvClientes_RowDeleting">

            <Columns>
                <asp:BoundField DataField="IdCliente" HeaderText="ID" />
                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                <asp:BoundField DataField="Apellido" HeaderText="Apellido" />
                <asp:BoundField DataField="DNI" HeaderText="DNI" />
                <asp:BoundField DataField="Telefono" HeaderText="Teléfono" />
                <asp:BoundField DataField="Email" HeaderText="Email" />
                <asp:BoundField DataField="Plan" HeaderText="Plan" />

                <asp:CommandField ShowDeleteButton="true"
                    DeleteText="Eliminar"
                    ButtonType="Button"
                    ControlStyle-CssClass="btn btn-danger btn-sm" />
            </Columns>

        </asp:GridView>
    </div>

</asp:Content>
