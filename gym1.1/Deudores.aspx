<%@ Page Title="Deudores"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Deudores.aspx.cs"
    Inherits="gym1._1.Deudores" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="container-fluid py-4">

        <div class="main-card p-4">

            <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">

                <div>
                    <h2 class="mb-1 d-flex align-items-center gap-2">
                        <i class="bi bi-exclamation-triangle-fill text-warning"></i>
                        <span>Deudores / Por vencer</span>
                    </h2>

                    <p class="text-secondary mb-0">
                        Consultá clientes con pagos pendientes y registrá el mes abonado.
                    </p>
                </div>

            </div>

            <asp:Panel ID="pnlMsg"
                runat="server"
                Visible="false"
                CssClass="alert mb-4">

                <asp:Label ID="lblMsg"
                    runat="server">
                </asp:Label>

            </asp:Panel>

            <!-- DATOS DEL PAGO -->
            <div class="border rounded-3 p-3 mb-4">

                <h5 class="mb-3 d-flex align-items-center gap-2">
                    <i class="bi bi-cash-coin text-success"></i>
                    Registrar pago
                </h5>

                <div class="row g-3">

                    <div class="col-md-3">

                        <label class="form-label">
                            Mes a pagar
                        </label>

                        <asp:DropDownList
                            ID="ddlMesPago"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </div>

                    <div class="col-md-3">

                        <label class="form-label">
                            Año
                        </label>

                        <asp:DropDownList
                            ID="ddlAnioPago"
                            runat="server"
                            CssClass="form-select">
                        </asp:DropDownList>

                    </div>

                    <div class="col-md-3">

                        <label class="form-label">
                            Monto
                        </label>

                        <asp:TextBox
                            ID="txtMonto"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Ej: 25000">
                        </asp:TextBox>

                    </div>

                    <div class="col-md-3">

                        <label class="form-label">
                            Observación
                        </label>

                        <asp:TextBox
                            ID="txtObs"
                            runat="server"
                            CssClass="form-control"
                            placeholder="Opcional">
                        </asp:TextBox>

                    </div>

                </div>

                <div class="mt-3">

                    <small class="text-secondary">

                        <i class="bi bi-info-circle me-1"></i>

                        Seleccioná el período abonado y luego presioná
                        <strong>Marcar pagado</strong>
                        sobre el cliente correspondiente.

                    </small>

                </div>

            </div>

            <!-- LISTADO -->
            <div class="table-responsive">

                <asp:GridView
                    ID="gvDeudores"
                    runat="server"
                    CssClass="table table-bordered table-striped align-middle mb-0"
                    AutoGenerateColumns="False"
                    DataKeyNames="IdCliente"
                    OnRowCommand="gvDeudores_RowCommand"
                    OnRowDataBound="gvDeudores_RowDataBound"
                    EmptyDataText="No hay deudores ni vencimientos próximos.">

                    <Columns>

                        <asp:BoundField
                            DataField="DNI"
                            HeaderText="DNI" />

                        <asp:BoundField
                            DataField="Apellido"
                            HeaderText="Apellido" />

                        <asp:BoundField
                            DataField="Nombre"
                            HeaderText="Nombre" />

                        <asp:BoundField
                            DataField="PlanPago"
                            HeaderText="Plan" />

                        <asp:BoundField
                            DataField="FechaAlta"
                            HeaderText="Alta"
                            DataFormatString="{0:dd/MM/yyyy}" />

                        <asp:BoundField
                            DataField="Vencimiento"
                            HeaderText="Vence"
                            DataFormatString="{0:dd/MM/yyyy}" />

                        <asp:TemplateField HeaderText="Meses adeudados">

                            <ItemTemplate>

                                <asp:Label
                                    ID="lblMesesAdeudados"
                                    runat="server"
                                    Text='<%# Eval("MesesAdeudadosTexto") %>'
                                    CssClass="badge">
                                </asp:Label>

                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Meses que debe">

                            <ItemTemplate>

                                <asp:Label
                                    ID="lblDetalleMesesAdeudados"
                                    runat="server"
                                    Text='<%# Eval("DetalleMesesAdeudados") %>'
                                    CssClass="small">
                                </asp:Label>

                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Estado">

                            <ItemTemplate>

                                <asp:Label
                                    ID="lblEstado"
                                    runat="server"
                                    Text='<%# Eval("EstadoPago") %>'
                                    CssClass="badge">
                                </asp:Label>

                            </ItemTemplate>

                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Acción">

                            <ItemTemplate>

                                <asp:Button
                                    ID="btnPagar"
                                    runat="server"
                                    Text="Marcar pagado"
                                    CssClass="btn btn-success btn-sm"
                                    CommandName="PAGAR"
                                    CommandArgument="<%# Container.DataItemIndex %>" />

                            </ItemTemplate>

                        </asp:TemplateField>

                    </Columns>

                </asp:GridView>

            </div>

        </div>

    </div>

</asp:Content>