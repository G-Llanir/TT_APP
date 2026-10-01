<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="CategoriaVendas.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.Manutencao.CategoriaVendas" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Manutenção</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                        </div>  
                        <div class="panel-body">
                            <div class="col-lg-6">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" runat="server" placeholder="Pesquisar"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                    <asp:Button ID="cmdNovoCadastro" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovoCadastro_Click" />
                                </div>
                            </div>
                            </div>
                        </div>
                    </div>
            </div>


            <div class="form-stacked row">
                <asp:Panel ID="pnResultado" runat="server">
                    <div class="col-lg-12">
                        <div class="panel panel-primary">
                            <div class="panel-body">
                                <asp:GridView ID="dtgvConsulta" runat="server" class="table table-striped table-bordered table-hover" GridLines="None"
                                    CellSpacing="1" CellPadding="1" Width="100%" ShowFooter="False" Font-Names="Tahoma"
                                    Font-Overline="False" Font-Size="Small" AutoGenerateColumns="false">
                                    <Columns>
                                        <asp:HyperLinkField DataNavigateUrlFields="idCategoriaVendas" DataTextField="idCategoriaVendas"
                                            DataNavigateUrlFormatString="CategoriaVendas_Detalhe.aspx?id={0}" HeaderText="ID">
                                            <HeaderStyle  Width="5%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>
                                        <asp:HyperLinkField DataNavigateUrlFields="idCategoriaVendas" DataTextField="sCodigoCategoriaVendas"
                                            DataNavigateUrlFormatString="CategoriaVendas_Detalhe.aspx?id={0}" HeaderText="Código">
                                            <HeaderStyle  Width="10%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>
                                        <asp:HyperLinkField DataNavigateUrlFields="idCategoriaVendas" DataTextField="sDscCategoriaVendas"
                                            DataNavigateUrlFormatString="CategoriaVendas_Detalhe.aspx?id={0}" HeaderText="Categoria de Vendas">
                                            <HeaderStyle  Width="30%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>
                                        <asp:BoundField DataField="sPadraoHHInstalacao_Completo" HeaderText="Padrão HH Instalação">
                                            <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="sSituacao_Completa" HeaderText="Ativo">
                                            <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </asp:Panel>
                <asp:Panel ID="pnMensagem"  runat="server">
                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>

