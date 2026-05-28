<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="gym1._1.Error" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div class="main-card p-4">


        <h3 class="mb-3 d-flex align-items-center gap-2">
            <i class="bi bi-exclamation-triangle-fill text-warning"></i>
            <span>Ocurrió un problema</span>
        </h3>

        <asp:Label ID="lblTitulo" runat="server" CssClass="h5 d-block mb-2"></asp:Label>
        <asp:Label ID="lblMensaje" runat="server" CssClass="text-secondary d-block mb-4"></asp:Label>

        <a class="btn btn-success" href="~/Pagos">Volver a Pagos</a>
    </div>

</asp:Content>