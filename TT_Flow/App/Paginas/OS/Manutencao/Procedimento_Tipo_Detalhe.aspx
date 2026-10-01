<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Procedimento_Tipo_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.OS.Manutencao.Procedimento_Tipo_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_Procedimento_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height:600px" runat ="server" visible="false"></div>
    <asp:UpdatePanel ID="updDetalhe" runat="server">
    <ContentTemplate>
        <div class="form-stacked row">
            <div class="col-lg-12">
                <h1><asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                <uc1:BreadCrumb runat="server" ID="BreadCrumb"  NivelPagina="3" TitulodaPagina="Detalhe"/>
            </div>
        </div>
                   
        <div class="form-stacked row">
            <div class="col-lg-12">
                <uc1:MensagemPagina runat="server" id="MensagemPagina" />
            </div>
            
            <div class="col-lg-12">
                <div class="form-group">
                    <label>ID </label>
                    <asp:TextBox ID="txtidTipoProcedimento" class="form-control CaixaTextoMini" runat="server" disabled="0" ></asp:TextBox>
                </div>
            </div>
            <div class="col-lg-6">
                <div class="form-group">
                    <label>Nome do Tipo do Procedimento</label>
                    <asp:TextBox ID="txtsDscTipoProcedimento" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
                </div>
            </div>
            <div class="col-lg-12">
                <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
            </div> 
        </div>

        <uc1:PainelAtualizacao runat="server" id="PainelAtualizacao" />
             
            
        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
            &nbsp;
            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;
        </fieldset>

        <asp:HiddenField ID="hddidTipoProcedimento" runat="server" />

    </ContentTemplate>            
</asp:UpdatePanel>

</asp:Content>