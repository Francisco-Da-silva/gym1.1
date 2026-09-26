<%@ Page Language="C#" MasterPageFile="~/Site.Master"AutoEventWireup="true"CodeBehind="Registro.aspx.cs"Inherits="gym1._1.Registro" %> <asp:Content ID="Content1" ContentPlaceHolderID="MainContent"
    runat="server">

    <h2 class="my-3 d-flex align-items-center gap-2">
        <i class="bi bi-person-plus text-success"></i>
        <span class="text-success">Gestión de Clientes</span>
    </h2>

    <div class="main-card p-4 mb-4">

        <asp:Panel ID="pnlMensaje"
            runat="server"
            Visible="false"
            CssClass="alert mb-4">

            <asp:Label ID="lblMensaje"
                runat="server">
            </asp:Label>

        </asp:Panel>

        <div class="row g-3">

            <div class="col-md-6">
                <label class="form-label">Nombre</label>

                <asp:TextBox ID="txtNombre"
                    runat="server"
                    CssClass="form-control"
                    MaxLength="50" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Apellido</label>

                <asp:TextBox ID="txtApellido"
                    runat="server"
                    CssClass="form-control"
                    MaxLength="50" />
            </div>

            <div class="col-md-6">
                <label class="form-label">DNI</label>

              <asp:TextBox
                   ID="txtDni"
                   runat="server"
                   CssClass="form-control"
                   TextMode="Number"
                   MaxLength="8"
                   placeholder="Solo números, 7 u 8 dígitos"
                   oninput="this.value = this.value.replace(/[^0-9]/g, '').slice(0, 8);" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Teléfono</label>

                <asp:TextBox ID="txtTelefono"
                    runat="server"
                    CssClass="form-control"
                    MaxLength="20" />
            </div>

            <div class="col-md-6">
                <label class="form-label">Email</label>

                <asp:TextBox ID="txtEmail"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Email"
                    MaxLength="100" />
            </div>

            <div class="col-md-6">
                <label class="form-label">
                    Fecha de nacimiento
                </label>

                <asp:TextBox ID="txtFechaNac"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Date" />
            </div>

            <div class="col-md-6">
                <label class="form-label">
                    Plan de pago
                </label>

                <asp:DropDownList ID="Planpago"
                    runat="server"
                    CssClass="form-select">

                    <asp:ListItem Value="3xSemana">
                        3 veces por semana
                    </asp:ListItem>

                    <asp:ListItem Value="Full">
                        Full
                    </asp:ListItem>

                </asp:DropDownList>
            </div>

        </div>

        <div class="mt-4 d-flex justify-content-end">

            <asp:Button ID="btnAgregar"
                runat="server"
                Text="Agregar cliente"
                CssClass="btn btn-success px-4"
                OnClick="btnAgregar_Click" />

        </div>
    </div>

    <div class="main-card p-4">

        <h3 class="mb-3 d-flex align-items-center gap-2">
            <i class="bi bi-people-fill text-success"></i>
            <span>Clientes registrados</span>
        </h3>

        <asp:GridView ID="gvClientes"
            runat="server"
            CssClass="table table-bordered table-striped mb-0"
            AutoGenerateColumns="false"
            DataKeyNames="IdCliente"
            OnRowDeleting="gvClientes_RowDeleting"
            EmptyDataText="No hay clientes registrados.">

            <Columns>

                <asp:BoundField
                    DataField="IdCliente"
                    HeaderText="ID" />

                <asp:BoundField
                    DataField="Nombre"
                    HeaderText="Nombre" />

                <asp:BoundField
                    DataField="Apellido"
                    HeaderText="Apellido" />

                <asp:BoundField
                    DataField="DNI"
                    HeaderText="DNI" />

                <asp:BoundField
                    DataField="Telefono"
                    HeaderText="Teléfono" />

                <asp:BoundField
                    DataField="Email"
                    HeaderText="Email" />

                <asp:BoundField
                    DataField="PlanPago"
                    HeaderText="Plan" />

                <asp:CommandField
                    ShowDeleteButton="true"
                    DeleteText="Eliminar"
                    ButtonType="Button"
                    ControlStyle-CssClass="btn btn-danger btn-sm" />

            </Columns>

        </asp:GridView>

    </div>

</asp:Content>