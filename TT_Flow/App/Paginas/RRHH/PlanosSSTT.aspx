<%@ Page Title="Planos SSTT" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="PlanosSSTT.aspx.cs" Inherits="TT_Hub.App.Paginas.RRHH.PlanosSSTT" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Planos SSTT"></asp:Label>
                        <small>
                            <asp:Label ID="lblSubTituloPagina" runat="server" Text="Manutenção "></asp:Label></small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Planos SSTT" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtsPesquisa" class="form-control" placeholder="Pesquisar" runat="server" MaxLength="200"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                        <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo Plano" OnClick="cmdNovo_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnResultado" class="" runat="server" Visible="false">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <div class="table-responsive">
                            <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover"
                                runat="server" Width="100%" AutoGenerateColumns="False" GridLines="None"
                                Font-Size="Small">
                                <Columns>
                                    <asp:HyperLinkField DataNavigateUrlFields="idPlano"
                                        DataTextField="idPlano" HeaderText="ID"
                                        DataNavigateUrlFormatString="PlanosSSTT_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="5%" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:HyperLinkField>
                                    <asp:HyperLinkField DataNavigateUrlFields="idPlano"
                                        DataTextField="sDscPlano" HeaderText="Descrição do Plano"
                                        DataNavigateUrlFormatString="PlanosSSTT_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="40%" />
                                    </asp:HyperLinkField>
                                    <asp:BoundField DataField="nValor" HeaderText="Valor Mensal" DataFormatString="{0:C}">
                                        <HeaderStyle Width="20%" />
                                        <ItemStyle HorizontalAlign="Left" />
                                    </asp:BoundField>

                           <%--         <asp:BoundField DataField="sObservacao" HeaderText="Observações">
                                        <HeaderStyle Width="35%" />
                                    </asp:BoundField>--%>

                                    <asp:BoundField DataField="sDscUsuario" HeaderText="Usuario Atualização">
                                        <HeaderStyle Width="10%" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Última Atualização">
                                        <HeaderStyle Width="15%" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnMensagem" runat="server" Visible="false">
                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            </asp:Panel>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

