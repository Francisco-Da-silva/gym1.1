<%@ Page Title="Inicio"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="gym1._1._Default" %>

<asp:Content ID="Content1"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <asp:PlaceHolder ID="inicioPlataforma" runat="server" Visible="false">
        <div class="main-card p-4 p-lg-5 mb-4">
            <span class="badge bg-success mb-3">Superadministrador</span>
            <h2 class="fw-bold">Administración de la plataforma</h2>
            <p class="text-secondary">Gestioná los gimnasios y sus cuentas de acceso desde un solo lugar.</p>
            <div class="d-flex flex-wrap gap-2">
                <a href="AdministrarGimnasios.aspx" class="btn btn-success">Administrar gimnasios</a>
                <a href="CrearAdministrador.aspx" class="btn btn-outline-success">Registrar gimnasio</a>
            </div>
        </div>
        <asp:Panel ID="errorResumen" runat="server" Visible="false" CssClass="alert alert-warning" role="alert">
            No pudimos cargar el resumen de gimnasios. Intentá nuevamente.
            Referencia: <asp:Literal ID="referenciaError" runat="server" Mode="Encode" />
        </asp:Panel>
        <div class="row g-3 mb-4">
            <div class="col-md-4"><div class="main-card p-4 h-100">
                <h3 class="h6 text-secondary">Gimnasios registrados</h3>
                <p class="display-6 mb-0"><asp:Literal ID="totalGimnasios" runat="server" Mode="Encode" Text="—" /></p>
            </div></div>
            <div class="col-md-4"><div class="main-card p-4 h-100">
                <h3 class="h6 text-secondary">Gimnasios activos</h3>
                <p class="display-6 text-success mb-0"><asp:Literal ID="gimnasiosActivos" runat="server" Mode="Encode" Text="—" /></p>
            </div></div>
            <div class="col-md-4"><div class="main-card p-4 h-100">
                <h3 class="h6 text-secondary">Gimnasios inactivos</h3>
                <p class="display-6 text-warning mb-0"><asp:Literal ID="gimnasiosInactivos" runat="server" Mode="Encode" Text="—" /></p>
            </div></div>
        </div>
        <div class="main-card p-4 mb-4">
            <h3 class="h5">Gimnasios y cuentas de acceso</h3>
            <p class="text-secondary">Consultá sus datos y administrá su estado de acceso. Al registrar un gimnasio se crea también su cuenta administradora.</p>
            <a href="AdministrarGimnasios.aspx" class="text-success">Ver gimnasios y cuentas</a>
        </div>
    </asp:PlaceHolder>

    <asp:PlaceHolder ID="inicioGimnasio" runat="server" Visible="false">
    <div class="container-fluid py-4">

        <!-- HERO -->
        <div class="main-card p-5 mb-4">

            <div class="row align-items-center">

                <div class="col-lg-8">

                    <span class="badge bg-success mb-3">
                        Sistema de gestión para gimnasios
                    </span>

                    <h1 class="display-5 fw-bold mb-3">
                        Bienvenido a
                        <span class="text-success">
                            Gym Admin
                        </span>
                    </h1>

                    <p class="lead text-secondary mb-4">
                        Gym Admin es una plataforma pensada para centralizar
                        la administración diaria de un gimnasio en un solo lugar.
                        Permite gestionar clientes, registrar pagos, controlar
                        vencimientos y consultar rápidamente el estado de cada socio.
                    </p>

                    <div class="d-flex flex-wrap gap-2">

                        <a href="Registro.aspx"
                           class="btn btn-success px-4">
                            <i class="bi bi-person-plus-fill me-2"></i>
                            Registrar cliente
                        </a>

                        <a href="Pagos.aspx"
                           class="btn btn-outline-success px-4">
                            <i class="bi bi-cash-coin me-2"></i>
                            Registrar pago
                        </a>

                    </div>

                </div>

                <div class="col-lg-4 text-center mt-4 mt-lg-0">

                    <div class="p-4 rounded-4 border border-success">

                        <i class="bi bi-barbell text-success"
                           style="font-size: 80px;">
                        </i>

                        <h4 class="mt-3">
                            Todo tu gimnasio
                        </h4>

                        <p class="text-secondary mb-0">
                            Clientes, pagos y vencimientos
                            organizados desde un mismo sistema.
                        </p>

                    </div>

                </div>

            </div>

        </div>

        <!-- FUNCIONES -->
        <div class="mb-3">
            <h2 class="fw-bold">
                Funciones principales
            </h2>

            <p class="text-secondary">
                Accedé rápidamente a las herramientas más importantes.
            </p>
        </div>

        <div class="row g-4 mb-5">

            <!-- CLIENTES -->
            <div class="col-md-6 col-xl-3">

                <div class="main-card h-100 p-4">

                    <div class="mb-3">
                        <i class="bi bi-people-fill text-success"
                           style="font-size: 38px;">
                        </i>
                    </div>

                    <h4>
                        Clientes
                    </h4>

                    <p class="text-secondary">
                        Registrá nuevos socios y consultá
                        sus datos personales y plan de pago.
                    </p>

                    <a href="Clientes.aspx"
                       class="text-success text-decoration-none">
                        Ver clientes
                        <i class="bi bi-arrow-right"></i>
                    </a>

                </div>

            </div>

            <!-- PAGOS -->
            <div class="col-md-6 col-xl-3">

                <div class="main-card h-100 p-4">

                    <div class="mb-3">
                        <i class="bi bi-wallet2 text-success"
                           style="font-size: 38px;">
                        </i>
                    </div>

                    <h4>
                        Pagos
                    </h4>

                    <p class="text-secondary">
                        Registrá pagos mensuales,
                        consultá períodos abonados
                        y revisá el historial de cada cliente.
                    </p>

                    <a href="Pagos.aspx"
                       class="text-success text-decoration-none">
                        Gestionar pagos
                        <i class="bi bi-arrow-right"></i>
                    </a>

                </div>

            </div>

            <!-- DEUDORES -->
            <div class="col-md-6 col-xl-3">

                <div class="main-card h-100 p-4">

                    <div class="mb-3">
                        <i class="bi bi-exclamation-triangle-fill text-warning"
                           style="font-size: 38px;">
                        </i>
                    </div>

                    <h4>
                        Deudores
                    </h4>

                    <p class="text-secondary">
                        Detectá rápidamente qué clientes
                        tienen cuotas pendientes o vencidas.
                    </p>

                    <a href="Deudores.aspx"
                       class="text-success text-decoration-none">
                        Ver deudores
                        <i class="bi bi-arrow-right"></i>
                    </a>

                </div>

            </div>

            <!-- SEGURIDAD -->
            <div class="col-md-6 col-xl-3">

                <div class="main-card h-100 p-4">

                    <div class="mb-3">
                        <i class="bi bi-shield-lock-fill text-success"
                           style="font-size: 38px;">
                        </i>
                    </div>

                    <h4>
                        Seguridad
                    </h4>

                    <p class="text-secondary">
                        Cada gimnasio accede únicamente
                        a sus propios clientes, pagos
                        y datos administrativos.
                    </p>

                    <span class="badge bg-success">
                        Acceso protegido
                    </span>

                </div>

            </div>

        </div>

        <!-- COMO FUNCIONA -->
        <div class="main-card p-4">

            <div class="row align-items-center">

                <div class="col-lg-5 mb-4 mb-lg-0">

                    <h2 class="fw-bold mb-3">
                        ¿Cómo funciona Gym Admin?
                    </h2>

                    <p class="text-secondary">
                        El sistema está diseñado para simplificar
                        las tareas administrativas que normalmente
                        se realizan de forma manual.
                    </p>

                </div>

                <div class="col-lg-7">

                    <div class="row g-3">

                        <div class="col-md-6">

                            <div class="border rounded-3 p-3 h-100">

                                <span class="badge bg-success mb-2">
                                    01
                                </span>

                                <h5>
                                    Registrá tus clientes
                                </h5>

                                <p class="text-secondary mb-0">
                                    Cargá nombre, DNI, contacto
                                    y plan de cada socio.
                                </p>

                            </div>

                        </div>

                        <div class="col-md-6">

                            <div class="border rounded-3 p-3 h-100">

                                <span class="badge bg-success mb-2">
                                    02
                                </span>

                                <h5>
                                    Registrá sus pagos
                                </h5>

                                <p class="text-secondary mb-0">
                                    Seleccioná el período,
                                    monto y registrá el pago.
                                </p>

                            </div>

                        </div>

                        <div class="col-md-6">

                            <div class="border rounded-3 p-3 h-100">

                                <span class="badge bg-success mb-2">
                                    03
                                </span>

                                <h5>
                                    Controlá vencimientos
                                </h5>

                                <p class="text-secondary mb-0">
                                    Identificá rápidamente
                                    quién está al día o adeuda cuotas.
                                </p>

                            </div>

                        </div>

                        <div class="col-md-6">

                            <div class="border rounded-3 p-3 h-100">

                                <span class="badge bg-success mb-2">
                                    04
                                </span>

                                <h5>
                                    Tomá mejores decisiones
                                </h5>

                                <p class="text-secondary mb-0">
                                    Consultá toda la información
                                    del gimnasio desde un solo lugar.
                                </p>

                            </div>

                        </div>

                    </div>

                </div>

            </div>

        </div>

    </div>

    </asp:PlaceHolder>

    <section id="mi-cuenta" class="main-card p-4 mt-4">
        <h2 class="h4">Mi cuenta</h2>
        <dl class="row mb-0">
            <dt class="col-sm-3">Nombre</dt><dd class="col-sm-9"><asp:Literal ID="cuentaNombre" runat="server" Mode="Encode" /></dd>
            <dt class="col-sm-3">Correo de acceso</dt><dd class="col-sm-9"><asp:Literal ID="cuentaEmail" runat="server" Mode="Encode" /></dd>
            <dt class="col-sm-3">Tipo de cuenta</dt><dd class="col-sm-9"><asp:Literal ID="cuentaRol" runat="server" Mode="Encode" /></dd>
        </dl>
    </section>

</asp:Content>