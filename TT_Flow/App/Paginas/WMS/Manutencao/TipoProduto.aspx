<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="TipoProduto.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Manutencao.TipoProduto" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <asp:UpdatePanel ID="UpdatePanel" runat="server">
        <ContentTemplate>
            <style>
                .grid-container {
                    overflow-x: auto;
                    max-width: 100%;
                }
            </style>


            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Manutenção</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">
                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" runat="server" placeholder="Pesquisar"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlFiltroPersonalizado" class="form-control" runat="server">
                                        <asp:ListItem Value="" Text="Selecione uma opção"></asp:ListItem>
                                        <asp:ListItem Value="COMP" Text="Exibe Composição"></asp:ListItem>
                                        <asp:ListItem Value="COMER" Text="Exibe em Comercial"></asp:ListItem>
                                        <asp:ListItem Value="EMB" Text="É Embalagem"></asp:ListItem>
                                        <asp:ListItem Value="EPI" Text="É EPI"></asp:ListItem>
                                        <asp:ListItem Value="IND" Text="É Industrializado"></asp:ListItem>
                                        <asp:ListItem Value="SIS" Text="É Sistema"></asp:ListItem>
                                        <asp:ListItem Value="PROD" Text="Apenas Produtos"></asp:ListItem>
                                        <asp:ListItem Value="SER" Text="Apenas Serviços"></asp:ListItem>
                                        <asp:ListItem Value="REC" Text="Apenas Recursos"></asp:ListItem>
                                        <asp:ListItem Value="FAB" Text="Apenas Fabricados"></asp:ListItem>
                                        <asp:ListItem Value="SUB-SER" Text="Apenas Sub-Serviços"></asp:ListItem>
                                    </asp:DropDownList>
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

            <div class="col-lg-12">
                <asp:Panel ID="pnResultado" CssClass="row" runat="server">
                    <div class="panel panel-primary">
                        <div class="panel-body  grid-container">
                            <asp:GridView ID="dtgvConsulta" runat="server" class="table table-striped table-bordered table-hover" GridLines="None"
                                CellSpacing="1" CellPadding="1" Width="100%" ShowFooter="False" Font-Names="Tahoma"
                                Font-Overline="False" Font-Size="Small" AutoGenerateColumns="false" OnRowDataBound="dtgvConsulta_RowDataBound">
                                <Columns>
                                    <asp:HyperLinkField DataNavigateUrlFields="idTipoProduto" DataTextField="idTipoProduto"
                                        DataNavigateUrlFormatString="TipoProduto_Detalhe.aspx?id={0}" HeaderText="ID">
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        <HeaderStyle Width="2%" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idTipoProduto" DataTextField="sDscTipoProduto"
                                        DataNavigateUrlFormatString="TipoProduto_Detalhe.aspx?id={0}" HeaderText="Descrição">
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        <HeaderStyle Width="20%" />
                                    </asp:HyperLinkField>

                                    <asp:BoundField DataField="sComposicaoCompleta" HeaderText="Composição">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sExibeComercialCompleta" HeaderText="Comercial">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sEmbalagemCompleta" HeaderText="Embalagem">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sEPICompleta" HeaderText="EPI">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sIndustrializadoCompleto" HeaderText="Industrializado">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sSistemaCompleto" HeaderText="Sistema">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sProdutoCompleto" HeaderText="Produto">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sServicoCompleto" HeaderText="Serviço">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sSubTipoCompleto" HeaderText="Sub-Serviço">
                                        <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sRecursoCompleto" HeaderText="Recurso">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sFabricacaoCompleta" HeaderText="Fabricação">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscTipoRegra" HeaderText="Tipo de Regra Fiscal">
                                        <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                        <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnMensagem" CssClass="row" runat="server">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </asp:Panel>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
