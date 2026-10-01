<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Familia_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Manutencao.Familia_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="content_frmCadastroFamilias" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 100px" runat="server" visible="false"></div>
    <script src="/app/js/vanilla-masker.js"></script>
    <asp:UpdatePanel ID="UpdDetalhe" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </div>
                <div class="col-lg-12">
                    <div class="form-group">
                        <label>ID</label>
                        <asp:TextBox ID="txtidFamilia" runat="server" class="form-control CaixaTextoMini" disabled=""></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Descrição</label>
                        <asp:TextBox ID="txtsDscFamilia" runat="server" class="form-control CaixaTextoGigante"  MaxLength="50"></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Familia (Grupo)</label>
                        <asp:DropDownList ID="ddlidFamiliaPai" runat="server" class="form-control CaixaTextoGigante"></asp:DropDownList>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                    </div>
                </div> 

            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
                <input type="submit" name="cancel" class="btn btn-lg btn-warning" id="field-cancel" value="Cancelar" title="Voltar" onclick="history.go(-1)" />
            </fieldset>
            <asp:HiddenField ID="hddidFamilia" runat="server" />

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
