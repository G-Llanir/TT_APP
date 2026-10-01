<%@ Page Title="" Language="C#" MasterPageFile="~/site.master" AutoEventWireup="true" CodeBehind="Avisos.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Avisos.Avisos" %>


<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmAvisos" ContentPlaceHolderID="MainContent" runat="server">
    <div id="DIV_ESPACO" class="h-100" runat="server" style="display: none;"></div>

    <div class="container mt-4">

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />



        <div id="tab" class="tab-content">
            <div role="tabpanel" class="tab-pane fade show active" id="Avisos" aria-labelledby="Avisos-tab">

                <asp:UpdatePanel ID="updDetalhe" runat="server">
                    <ContentTemplate>
                        <label>Popup de Avisos</label>
                        
                    </ContentTemplate>
                </asp:UpdatePanel>

                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                <br />
            </div>

        </div>

        <div id="dialog-Salvar" class="modal" title="Salvar">
            <p>
                <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
                <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
            </p>
        </div>

    </div>

    <asp:HiddenField ID="hddidColaborador" runat="server" />
</asp:Content>
