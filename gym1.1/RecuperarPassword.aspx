<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="RecuperarPassword.aspx.cs"
    Inherits="gym1._1.RecuperarPassword" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">

    <meta charset="utf-8" />

    <meta name="viewport"
          content="width=device-width, initial-scale=1" />

    <title>Recuperar contraseña - Gym Manager</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
          rel="stylesheet" />

</head>

<body class="bg-dark">

<form id="form1" runat="server">

    <div class="container min-vh-100
                d-flex align-items-center
                justify-content-center">

        <div class="card p-4 shadow"
             style="width:100%;
                    max-width:450px;">

            <h3 class="mb-3">
                Recuperar contraseña
            </h3>

            <p class="text-secondary">
                Ingresá el correo asociado a tu cuenta.
            </p>

            <asp:Panel ID="pnlMensaje"
                runat="server"
                Visible="false"
                CssClass="alert">

                <asp:Label ID="lblMensaje"
                    runat="server" />

            </asp:Panel>

            <div class="mb-3">

                <label class="form-label">
                    Correo electrónico
                </label>

                <asp:TextBox
                    ID="txtEmail"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Email"
                    MaxLength="100" />

            </div>

            <asp:Button
                ID="btnEnviar"
                runat="server"
                Text="Enviar enlace"
                CssClass="btn btn-success w-100"
                OnClick="btnEnviar_Click" />

            <div class="text-center mt-3">

                <a href="Login.aspx">
                    Volver al inicio de sesión
                </a>

            </div>

        </div>

    </div>

</form>

</body>
</html>