<%@ Page Language="C#" AutoEventWireup="true"
    CodeBehind="Error.aspx.cs"
    Inherits="gym1._1.Error" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1" />

    <title>Error - Gym Manager</title>

    <link href="Content/bootstrap.min.css" rel="stylesheet" />

    <style>
        body {
            min-height: 100vh;
            display: flex;
            align-items: center;
            justify-content: center;
            background-color: #f4f6f9;
        }

        .error-card {
            max-width: 550px;
            width: 100%;
            padding: 35px;
            border-radius: 15px;
            background-color: white;
            box-shadow: 0 8px 25px rgba(0, 0, 0, 0.10);
            text-align: center;
        }

        .error-icon {
            font-size: 60px;
            margin-bottom: 15px;
        }
    </style>
</head>

<body>
    <form id="form1" runat="server">

        <div class="error-card">

            <div class="error-icon">
                ⚠️
            </div>

            <h2>Ocurrió un error</h2>

            <p class="text-muted mt-3">
                No pudimos completar la operación.
                Por favor, intentá nuevamente.
            </p>

            <asp:Label
                ID="lblCodigoError"
                runat="server"
                CssClass="d-block text-muted small mb-4">
            </asp:Label>

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