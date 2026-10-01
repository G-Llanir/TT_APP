<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Cliente_Detalhe.aspx.cs" Inherits="TT_Hub.App.Paginas.Cadastros.Cliente_Detalhe" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
<div id="DIV_ESPACO" style="height:600px" runat ="server" visible="false"> </div>


<asp:UpdatePanel ID="UpdatePanel1" runat="server">
    <ContentTemplate>
        <div class="form-stacked row">
            <div class="col-lg-12">
                <h1><asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                <uc1:BreadCrumb runat="server" ID="BreadCrumb"  NivelPagina="3" TitulodaPagina="Detalhe"/>
            </div>
            
            <uc1:MensagemPagina runat="server" id="MensagemPagina" />
            
        </div>
                   
        <div class="form-stacked row">
            <div class="col-lg-12">
                <div class="form-group">
                    <label>Descrição </label>
                    <asp:TextBox ID="txtssDscCliente" class="form-control CaixaTextoGrande" runat="server" MaxLength="200"></asp:TextBox>
                </div>
            </div>

            <div class="col-lg-12" runat="server" id="div_sChaveGUI">
                <div class="form-group">
                    <label>Chave de Segurança </label>
                    <asp:TextBox ID="txtsChaveGUI" class="form-control CaixaTextoGrande" runat="server" MaxLength="200" disabled></asp:TextBox>
                </div>
            </div>

<%--            <div class="col-lg-12">
                <div class="form-group">
                    <label>Código SW </label>
                    <asp:TextBox ID="txtsw_Account" class="form-control CaixaTextoPequeno" runat="server" MaxLength="10"></asp:TextBox>
                </div>
            </div>--%>

        
            <div class="col-lg-6">
                <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
            </div> 

        </div>   
        
        <uc1:PainelAtualizacao runat="server" id="PainelAtualizacao" />
               
        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdSalvar" class="btn  btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
            &nbsp;
            <input type="submit" name="cancel" class="btn" value="Cancelar" " id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;
        </fieldset>

        <asp:HiddenField ID="hddidCliente" runat="server" />

    </ContentTemplate>            
</asp:UpdatePanel>

</asp:Content>
