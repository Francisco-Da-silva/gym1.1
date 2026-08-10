<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="gym1._1.Login" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport"
          content="width=device-width, initial-scale=1" />

    <title>Iniciar sesión - Gym Manager</title>

    <link href="https://cdn.jsdelivr.net/npm/bootstrap@5.3.3/dist/css/bootstrap.min.css"
          rel="stylesheet" />

    <link href="https://cdn.jsdelivr.net/npm/bootstrap-icons@1.11.3/font/bootstrap-icons.css"
          rel="stylesheet" />

    <style>
        body {
            min-height: 100vh;
            background:
                radial-gradient(
                    circle at top left,
                    #22c55e 0,
                    #0f172a 38%,
                    #020617 100%
                );
        }

        .login-card {
            width: 100%;
            max-width: 430px;
            padding: 38px;
            border-radius: 22px;
            background: rgba(2, 6, 23, .95);
            border: 1px solid rgba(255, 255, 255, .1);
            box-shadow: 0 25px 70px rgba(0, 0, 0, .55);
        }

        .login-icon {
            width: 66px;
            height: 66px;
            margin: auto;
            display: flex;
            align-items: center;
            justify-content: center;
            border-radius: 50%;
            background: #22c55e;
            color: #020617;
            font-size: 30px;
        }

        .form-control {
            color: white;
            background: #0f172a;
            border-color: #334155;
        }

        .form-control:focus {
            color: white;
            background: #0f172a;
            border-color: #22c55e;
            box-shadow: 0 0 0 .25rem rgba(34, 197, 94, .2);
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <div class="container min-vh-100 d-flex
                align-items-center justify-content-center">

        <div class="login-card">

            <div class="login-icon mb-3">
                <i class="bi bi-person-fill-lock"></i>
            </div>

            <h2 class="text-white text-center mb-1">
                Gym Manager
            </h2>

            <p class="text-secondary text-center mb-4">
                Ingresá al panel de tu gimnasio
            </p>

            <asp:Panel ID="pnlError"
                runat="server"
                Visible="false"
                CssClass="alert alert-danger">

                <asp:Label ID="lblError" runat="server" />
            </asp:Panel>

            <div class="mb-3">
                <label class="form-label text-light">
                    Correo electrónico
                </label>

                <asp:TextBox ID="txtEmail"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Email"
                    MaxLength="100"
                    placeholder="usuario@gimnasio.com" />
            </div>

            <div class="mb-4">
                <label class="form-label text-light">
                    Contraseña
                </label>

                <asp:TextBox ID="txtPassword"
                    runat="server"
                    CssClass="form-control"
                    TextMode="Password"
                    MaxLength="100"
                    placeholder="Ingresá tu contraseña" />
            </div>

               <div class="text-end mb-3">
                   <a href="RecuperarPassword.aspx"
                      class="text-success text-decoration-none">
                       ¿Olvidaste tu contraseña?
                   </a>
               </div>

            <asp:Button ID="btnIngresar"
                runat="server"
                Text="Ingresar"
                CssClass="btn btn-success w-100 py-2"
                OnClick="btnIngresar_Click" />

</div>

 </div>
        

</form>
</body>
</html>