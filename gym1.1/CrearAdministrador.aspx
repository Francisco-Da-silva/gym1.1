<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="CrearAdministrador.aspx.cs"
    Inherits="gym1._1.CrearAdministrador" %>

<!DOCTYPE html>
<html lang="es">
<head runat="server">
    <meta charset="utf-8" />
    <meta name="viewport"
          content="width=device-width, initial-scale=1" />

    <title>Crear cuenta - Gym Manager</title>

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

        .registro-card {
            width: 100%;
            max-width: 760px;
            padding: 38px;
            border-radius: 22px;
            background: rgba(2, 6, 23, .96);
            border: 1px solid rgba(255, 255, 255, .1);
            box-shadow: 0 25px 70px rgba(0, 0, 0, .55);
        }

        .registro-icon {
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

        .seccion-titulo {
            color: #22c55e;
            font-size: 1rem;
            font-weight: 600;
            text-transform: uppercase;
            letter-spacing: .08em;
            margin-bottom: 18px;
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

        .form-control::placeholder {
            color: #64748b;
        }

        .separador {
            border-color: rgba(255, 255, 255, .12);
        }

        .login-link {
            color: #22c55e;
            text-decoration: none;
        }

        .login-link:hover {
            color: #4ade80;
            text-decoration: underline;
        }
    </style>
</head>

<body>
<form id="form1" runat="server">

    <div class="container min-vh-100 d-flex
                justify-content-center align-items-center py-5">

        <div class="registro-card">

            <div class="registro-icon mb-3">
                <i class="bi bi-building-add"></i>
            </div>

            <h2 class="text-white text-center mb-1">
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
                    <label class="form-label text-light">
                        Nombre del gimnasio
                    </label>

                    <asp:TextBox ID="txtNombreGimnasio"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="100"
                        placeholder="Ej: Gym Pancho" />
                </div>

                <div class="col-md-6">
                    <label class="form-label text-light">
                        Correo del gimnasio
                    </label>

                    <asp:TextBox ID="txtEmailGimnasio"
                        runat="server"
                        CssClass="form-control"
                        TextMode="Email"
                        MaxLength="100"
                        placeholder="contacto@gimnasio.com" />
                </div>

                <div class="col-md-6">
                    <label class="form-label text-light">
                        Teléfono
                    </label>

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
                    <label class="form-label text-light">
                        Nombre del administrador
                    </label>

                    <asp:TextBox ID="txtNombre"
                        runat="server"
                        CssClass="form-control"
                        MaxLength="100"
                        placeholder="Nombre y apellido" />
                </div>

                <div class="col-md-6">
                    <label class="form-label text-light">
                        Correo para iniciar sesión
                    </label>

                    <asp:TextBox ID="txtEmail"
                        runat="server"
                        CssClass="form-control"
                        TextMode="Email"
                        MaxLength="100"
                        placeholder="administrador@gimnasio.com" />
                </div>

                <div class="col-md-6">
                    <label class="form-label text-light">
                        Contraseña
                    </label>

                    <asp:TextBox ID="txtPassword"
                        runat="server"
                        CssClass="form-control"
                        TextMode="Password"
                        MaxLength="100"
                        placeholder="Mínimo 8 caracteres" />
                </div>

                <div class="col-md-6">
                    <label class="form-label text-light">
                        Repetir contraseña
                    </label>

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

</form>
</body>
</html>