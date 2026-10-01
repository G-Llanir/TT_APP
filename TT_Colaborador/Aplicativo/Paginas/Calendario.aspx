<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Calendario.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Calendario" %>

<%@ Register Src="~/Aplicativo/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb_Pagina" %>
<%@ Register Src="~/Aplicativo/Controles/Calendario.ascx" TagPrefix="uc1" TagName="Calendario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <div class="card">

        <div class="card-header card-tt">
            <h3 class="card-title" style="color: white">
                <b>
                    <asp:Label ID="lblTituloPagina" runat="server" Text="Calendário"></asp:Label></b>
            </h3>
        </div>

        <div class="card-body">
            <uc1:Calendario runat="server" ID="Calendario_" />
        </div>

    </div>

</asp:Content>

