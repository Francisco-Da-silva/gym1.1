<%@ Page Title="Administrar Gimnasios"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="AdministrarGimnasios.aspx.cs"
    Inherits="gym1._1.AdministrarGimnasios" %>

<asp:Content
    ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <div class="container py-4">

        <!-- CABECERA -->
        <div class="d-flex justify-content-between
                    align-items-center mb-4">

            <div>
                <h2 class="mb-1">
                    Administración de gimnasios
                </h2>

                <p class="text-muted mb-0">
                    Gestioná los gimnasios registrados
                    en la plataforma.
                </p>
            </div>

            <asp:HyperLink
                ID="lnkNuevoGimnasio"
                runat="server"
                NavigateUrl="~/CrearAdministrador.aspx"
                CssClass="btn btn-success">

                + Registrar gimnasio

            </asp:HyperLink>

        </div>


        <!-- MENSAJES -->
        <asp:Panel
            ID="pnlMensaje"
            runat="server"
            Visible="false"
            CssClass="alert">

            <asp:Label
                ID="lblMensaje"
                runat="server">
            </asp:Label>

        </asp:Panel>


        <!-- TABLA -->
        <div class="card shadow-sm">

            <div class="card-header bg-dark text-white">

                <strong>
                    Gimnasios registrados
                </strong>

            </div>


            <div class="card-body">

                <div class="table-responsive">

                    <asp:GridView
                        ID="gvGimnasios"
                        runat="server"

                        AutoGenerateColumns="False"

                        DataKeyNames="IdGimnasio"

                        CssClass="table table-hover
                                  table-striped
                                  align-middle"

                        GridLines="None"

                        EmptyDataText="No hay gimnasios registrados."

                        OnRowCommand="gvGimnasios_RowCommand"

                        OnRowDataBound="gvGimnasios_RowDataBound">


                        <Columns>

                            <%-- ID --%>

                            <asp:BoundField
                                DataField="IdGimnasio"
                                HeaderText="ID" />


                            <%-- GIMNASIO --%>

                            <asp:BoundField
                                DataField="Nombre"
                                HeaderText="Gimnasio" />


                            <%-- EMAIL --%>

                            <asp:BoundField
                                DataField="Email"
                                HeaderText="Email" />


                            <%-- TELÉFONO --%>

                            <asp:BoundField
                                DataField="Telefono"
                                HeaderText="Teléfono"
                                NullDisplayText="-" />


                            <%-- CUENTA DE ACCESO --%>

                            <asp:BoundField
                                DataField="EmailAcceso"
                                HeaderText="Usuario de acceso"
                                NullDisplayText="-" />


                            <%-- FECHA ALTA --%>

                            <asp:BoundField
                                DataField="FechaAlta"
                                HeaderText="Fecha de alta"
                                DataFormatString="{0:dd/MM/yyyy}" />


                            <%-- ESTADO --%>

                            <asp:TemplateField
                                HeaderText="Estado">

                                <ItemTemplate>

                                    <asp:Label
                                        ID="lblEstado"
                                        runat="server">
                                    </asp:Label>

                                </ItemTemplate>

                            </asp:TemplateField>


                            <%-- ACCIONES --%>

                            <asp:TemplateField
                                HeaderText="Acciones">

                                <ItemTemplate>

                                    <asp:LinkButton
                                        ID="btnCambiarEstado"
                                        runat="server"

                                        CommandArgument='<%# Eval("IdGimnasio") %>'

                                        CssClass="btn btn-sm">

                                    </asp:LinkButton>

                                </ItemTemplate>

                            </asp:TemplateField>

                        </Columns>

                    </asp:GridView>

                </div>

            </div>

        </div>

    </div>

</asp:Content>