<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Deudores.aspx.cs" Inherits="gym1._1.Deudores" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">


    <div class="main-card p-4">
        <h3 class="mb-3 d-flex align-items-center gap-2">
            <i class="bi bi-exclamation-triangle-fill text-warning"></i>
            <span>Deudores / Por vencer</span>
        </h3>

        <asp:Panel ID="pnlMsg" runat="server" Visible="false" CssClass="alert">
            <asp:Label ID="lblMsg" runat="server"></asp:Label>
        </asp:Panel>

        <div class="row g-3 mb-3">
            <div class="col-md-3">
                <label class="form-label">Desde</label>
                <asp:TextBox ID="txtDesde" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Hasta (vencimiento)</label>
                <asp:TextBox ID="txtHasta" runat="server" CssClass="form-control" TextMode="Date" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Monto</label>
                <asp:TextBox ID="txtMonto" runat="server" CssClass="form-control" />
            </div>
            <div class="col-md-3">
                <label class="form-label">Obs (opcional)</label>
                <asp:TextBox ID="txtObs" runat="server" CssClass="form-control" />
            </div>

            <div class="col-12 d-flex align-items-center gap-2">
                <asp:Button ID="btnMes" runat="server" Text="+1 mes (rápido)"
                    CssClass="btn btn-outline-light btn-sm" OnClick="btnMes_Click" />
                <small class="text-secondary">Elegí un cliente en la grilla y tocá “Marcar pagado”.</small>
            </div>
        </div>

        <asp:GridView ID="gvDeudores" runat="server"
            CssClass="table table-bordered table-striped"
            AutoGenerateColumns="False"
            DataKeyNames="IdCliente"
            OnRowCommand="gvDeudores_RowCommand"
            OnRowDataBound="gvDeudores_RowDataBound"
            EmptyDataText="No hay deudores ni vencimientos próximos.">

            <Columns>
                <asp:BoundField DataField="DNI" HeaderText="DNI" />
                <asp:BoundField DataField="Apellido" HeaderText="Apellido" />
                <asp:BoundField DataField="Nombre" HeaderText="Nombre" />
                <asp:BoundField DataField="PlanPago" HeaderText="PlanPago" />
                <asp:BoundField DataField="FechaAlta" HeaderText="Alta"
                    DataFormatString="{0:dd/MM/yyyy}" />
                <asp:BoundField DataField="Vencimiento" HeaderText="Vence"
                    DataFormatString="{0:dd/MM/yyyy}" />

                <asp:TemplateField HeaderText="Meses adeudados">
                    <ItemTemplate>
                        <asp:Label ID="lblMesesAdeudados" runat="server"
                            Text='<%# Eval("MesesAdeudadosTexto") %>' CssClass="badge"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Meses que debe">
                    <ItemTemplate>
                        <asp:Label ID="lblDetalleMesesAdeudados" runat="server"
                            Text='<%# Eval("DetalleMesesAdeudados") %>' CssClass="small"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Estado">
                    <ItemTemplate>
                        <asp:Label ID="lblEstado" runat="server"
                            Text='<%# Eval("EstadoPago") %>' CssClass="badge"></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>

                <asp:TemplateField HeaderText="Acción">
                    <ItemTemplate>
                        <asp:Button ID="btnPagar" runat="server"
                            Text="Marcar pagado"
                            CssClass="btn btn-success btn-sm"
                            CommandName="PAGAR"
                            CommandArgument="<%# Container.DataItemIndex %>" />
                    </ItemTemplate>
                </asp:TemplateField>
            </Columns>

        </asp:GridView>
    </div>

</asp:Content>
