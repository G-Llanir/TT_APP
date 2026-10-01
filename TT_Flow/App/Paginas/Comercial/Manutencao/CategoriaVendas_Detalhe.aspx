<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="CategoriaVendas_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.Manutencao.CategoriaVendas_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="content_frmCadastroFamilias" ContentPlaceHolderID="cphCorpo" runat="server">
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
                        <asp:TextBox ID="txtidCategoriaVendas" runat="server" class="form-control CaixaTextoMini" disabled=""></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Código</label>
                        <asp:TextBox ID="txtsCodigoCategoriaVendas" runat="server" class="form-control CaixaTextoPequeno" MaxLength="20"></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Descrição</label>
                        <asp:TextBox ID="txtsDscCategoriaVendas" runat="server" class="form-control CaixaTextoGigante" MaxLength="200"></asp:TextBox>
                    </div>
                </div>


                <div class="col-lg-12">
                    <div class="form-group">
                        <label>
                            Qtd HH Padrão Instalação       
                            <i class="fa fa-question-circle" data-toggle="tooltip" title="Valor utilizado em orçamento, caso não seja definido no produto."></i>
                        </label>
                        <asp:TextBox ID="txtnPadraoHHInstalacao" runat="server" class="form-control CaixaTextoPequeno" MaxLength="12"></asp:TextBox>
                    </div>
                </div>

               <%-- Agnes Partal * 04/07/2024 --------------------------------------------------%>
                <div class="col-lg-12">
                    <div class="form-group">
                        <asp:Label ID="lblExibeAcervo" runat="server" Text="Exibe Acervo" Font-Bold="true"></asp:Label>
                        <asp:DropDownList ID="ddlsExibeAcervo" runat="server" class="form-control yes_no select CaixaTextoPequeno" Style="margin-top: 5px">
                            <asp:ListItem Value="S">Sim</asp:ListItem>
                            <asp:ListItem Selected="True" Value="N">Não</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
               <%-------------------------------------------------------------------------------%>

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
            <asp:HiddenField ID="hddidCategoriaVendas" runat="server" />

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

