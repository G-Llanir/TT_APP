<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Usuarios_Perfil_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Usuarios_Perfil_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Recursos.ascx" TagPrefix="uc1" TagName="Recursos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="updDetalhe" runat="server">
        <ContentTemplate>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div class="form-stacked row">

                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </div>

                <div class="col-lg-12 form-group">
                    <label>ID</label>
                    <asp:TextBox ID="txtidPerfil" runat="server" class="form-control CaixaTextoMini" ReadOnly="true"></asp:TextBox>
                </div>

                <div class="col-lg-12 form-group">
                    <div class="col-lg-4" style="padding: 0;">
                        <label>Nome do Perfil</label>
                        <asp:TextBox ID="txtsDscPerfil" runat="server" class="form-control" MaxLength="100"></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-3 form-group">
                    <label>Tipo Pefil</label>
                    <asp:DropDownList ID="ddlsTipoPerfil" runat="server" AutoPostBack="true" class="form-control Caixa_Selecao" OnSelectedIndexChanged="ddlsTipoPerfil_SelectedIndexChanged">
                        <asp:ListItem Value="I">Usuário Interno</asp:ListItem>
                        <asp:ListItem Value="P">Usuário Externo (Parceiros)</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="col-lg-3 form-group">
                    <label>Página Inicial <i class="fa fa-info-circle" data-toggle="tooltip" title="Define a Página onde o Usuário chega ao realizar o Login"></i></label>
                    <asp:DropDownList ID="ddlDashboard" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                </div>

                <div class="col-lg-12" runat="server" id="DIV_TipoParceiro">
                    <div class="form-group checkbox-container checkBoxList">
                        <label>Tipo de Parceiro</label>
                        <asp:CheckBoxList ID="cblidTipoParceiro" runat="server" ToolTip="Tipo de Parceiro"></asp:CheckBoxList>
                    </div>
                </div>

                <div class="col-lg-12">
                    <uc1:Recursos runat="server" ID="ctrl_Recursos" />
                </div>

            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />&nbsp;                
                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field-cancel" title="Voltar" onclick="history.go(-1)">&nbsp;
            </fieldset>

            <asp:HiddenField ID="hddidPerfil" runat="server" />

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>