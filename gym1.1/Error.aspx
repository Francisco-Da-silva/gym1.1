<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Error.aspx.cs"
    Inherits="gym1._1.Error" %>

<!DOCTYPE html>

<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <title>Error - Gym Manager</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css" rel="stylesheet" />

    <link href="Content/Site.css" rel="stylesheet" />
<script src="Scripts/gym-theme.js"></script>
</head>

<body class="auth-page">
<div class="auth-theme"><button id="theme-toggle" type="button" class="icon-button" aria-label="Activar modo oscuro" aria-pressed="false" hidden><span aria-hidden="true">◐</span></button></div>
    <form id="form1" runat="server">

        <div class="error-card">

            <div class="error-icon">
                ⚠️
            </div>

            <h2>Ocurrió un error</h2>

            <p class="text-muted mt-3">
                <%= Request.QueryString["entrada"] == "invalida"
                    ? "La entrada contiene contenido no permitido. Volvé al formulario y revisá los datos ingresados."
                    : "No pudimos completar la operación. Por favor, intentá nuevamente." %>
            </p>

            <asp:Label
                ID="lblCodigoError"
                runat="server"
                CssClass="d-block text-muted small mb-4">
            </asp:Label>

            <asp:Panel
    ID="pnlDetalleError"
    runat="server"
    Visible="false"
    CssClass="alert alert-warning mt-4">

    <h5>Detalle técnico (desarrollo)</h5>

    <asp:Label
        ID="lblDetalleError"
        runat="server"
        style="white-space: pre-wrap; font-family: monospace;">
    </asp:Label>

</asp:Panel>
            <asp:Button
                ID="btnVolver"
                runat="server"
                Text="Volver al inicio"
                CssClass="btn btn-primary"
                OnClick="btnVolver_Click" />



        </div>


    </form>
</body>
</html>