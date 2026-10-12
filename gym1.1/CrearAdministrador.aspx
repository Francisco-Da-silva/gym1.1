<%@ Page Title="Registrar gimnasio" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CrearAdministrador.aspx.cs" Inherits="gym1._1.CrearAdministrador" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server"><div class="main-card p-4">

    <div class="container min-vh-100 d-flex
                justify-content-center align-items-center py-5">

        <div class="registro-card">

            <div class="registro-icon mb-3">
                <i class="bi bi-building-add"></i>
            </div>

            <h2 class=" text-center mb-1">
                Crear cuenta
            </h2>

            <p class="text-secondary text-center mb-4">
                Registrá tu gimnasio y el usuario administrador
            </p>

            <asp:Panel ID="pnlMensaje"
                runat="server"
                Visible="false"
                CssClass="alert">

                <asp:Label ID="lblMensaje"
                    runat="server" />
            </asp:Panel>

            <div class="seccion-titulo">
                <i class="bi bi-building me-2"></i>
                Datos del gimnasio
            </div>

            <div class="row g-3">

                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="txtNombreGimnasio" class="form-label ">
                        Nombre del gimnasio
                    </asp:Label>

                    <asp:TextBox ID="txtNombreGimnasio"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="100"
                        placeholder="Ej: Gym Pancho" />
                </div>

                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="txtEmailGimnasio" class="form-label ">
                        Correo del gimnasio
                    </asp:Label>

                    <asp:TextBox ID="txtEmailGimnasio"
                        runat="server"
                        CssClass="form-control"
                        TextMode="Email"
                        MaxLength="100"
                        placeholder="contacto@gimnasio.com" />
                </div>

                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="txtTelefono" class="form-label ">
                        Teléfono
                    </asp:Label>

                    <asp:TextBox ID="txtTelefono"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="30"
                        placeholder="Ej: 11 1234-5678" />
                </div>

            </div>

            <hr class="my-4 separador" />

            <div class="seccion-titulo">
                <i class="bi bi-person-gear me-2"></i>
                Datos del administrador
            </div>

            <div class="row g-3">

                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="txtNombre" class="form-label ">
                        Nombre del administrador
                    </asp:Label>

                    <asp:TextBox ID="txtNombre"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="100"
                        placeholder="Nombre y apellido" />
                </div>

                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="txtEmail" class="form-label ">
                        Correo para iniciar sesión
                    </asp:Label>

                    <asp:TextBox ID="txtEmail"
                        runat="server"
                        CssClass="form-control"
                        TextMode="Email"
                        MaxLength="100"
                        placeholder="administrador@gimnasio.com" />
                </div>

                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="txtPassword" class="form-label ">
                        Contraseña
                    </asp:Label>

                    <asp:TextBox ID="txtPassword"
                        runat="server"
                        CssClass="form-control"
                        TextMode="Password"
                        MaxLength="100"
                        placeholder="Mínimo 8 caracteres" />
                </div>

                <div class="col-md-6">
                    <asp:Label runat="server" AssociatedControlID="txtRepetirPassword" class="form-label ">
                        Repetir contraseña
                    </asp:Label>

                    <asp:TextBox ID="txtRepetirPassword"
                        runat="server"
                        CssClass="form-control"
                        TextMode="Password"
                        MaxLength="100"
                        placeholder="Volvé a ingresar la contraseña" />
                </div>

            </div>

            <asp:Button ID="btnCrear"
                runat="server"
                Text="Crear cuenta"
                CssClass="btn btn-success w-100 py-2 mt-4"
                OnClick="btnCrear_Click" />

            <div class="text-center mt-4">
                <span class="text-secondary">
                    ¿Ya tenés una cuenta?
                </span>

                <a href="Login.aspx"
                   class="login-link ms-1">
                    Iniciar sesión
                </a>
            </div>

        </div>
    </div>

</div></asp:Content>