<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="RestablecerPassword.aspx.cs"
    Inherits="gym1._1.RestablecerPassword" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">

    <meta charset="utf-8" />

    <meta name="viewport"
          content="width=device-width, initial-scale=1" />

    <title>Nueva contraseña - Gym Manager</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
          rel="stylesheet" />

<link href="Content/Site.css" rel="stylesheet" />
<script src="Scripts/gym-theme.js"></script>
</head>

<body class="auth-page">
<div class="auth-theme"><button id="theme-toggle" type="button" class="icon-button" aria-label="Activar modo oscuro" aria-pressed="false" hidden><span aria-hidden="true">◐</span></button></div>

<form id="form1" runat="server">

    <div class="container min-vh-100
                d-flex align-items-center
                justify-content-center">

        <div class="card p-4 shadow"
             style="width:100%;
                    max-width:450px;">

            <h3 class="mb-3">
                Nueva contraseña
            </h3>

            <asp:Panel
                ID="pnlMensaje"
                runat="server"
                Visible="false"
                CssClass="alert">

                <asp:Label
                    ID="lblMensaje"
                    runat="server" />

            </asp:Panel>

            <div class="mb-3">

                <asp:Label runat="server" AssociatedControlID="txtPassword" class="form-label">
                    Nueva contraseña
                </asp:Label>

                <asp:TextBox
                    ID="txtPassword"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Password"
                    MaxLength="100" />

            </div>

            <div class="mb-3">

                <asp:Label runat="server" AssociatedControlID="txtRepetirPassword" class="form-label">
                    Repetir contraseña
                </asp:Label>

                <asp:TextBox
                    ID="txtRepetirPassword"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Password"
                    MaxLength="100" />

            </div>

            <asp:Button
                ID="btnCambiar"
                runat="server"
                Text="Cambiar contraseña"
                CssClass="btn btn-success w-100"
                OnClick="btnCambiar_Click" />

        </div>

    </div>

</form>

</body>
</html>