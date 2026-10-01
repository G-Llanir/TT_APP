<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Faturamento.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Faturamento.Faturamento" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <script type="text/javascript">
        $(document).ready(function () {
            $('#togglePanelExibeGrafico').click(function () {
                $('#panelExibirGrafico').slideToggle();
                $('#iconDownExibe').toggleClass('fa-chevron-up fa-chevron-down');
            });

            $('#togglePanelExibeOPI').click(function () {
                $('#panelExibirOPI').slideToggle();
                $('#iconDownExibeOPI').toggleClass('fa-chevron-up fa-chevron-down');
            });

            $('#togglePanelExibePedidos').click(function () {
                $('#panelExibirPedidos').slideToggle();
                $('#iconDownExibePedidos').toggleClass('fa-chevron-up fa-chevron-down');
            });
        });
    </script>

    <div id="hdd">
        <asp:HiddenField runat="server" ID="hddPedido_Filtro" Value="0" />
    </div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Faturamento"></asp:Label>
                <small>Consulta</small>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">

            <div runat="server" id="pnFiltros" class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-filter"></i>&nbsp;Filtro para pesquisa</h3>
                </div>
                <div class="panel-body">
                    <div class="row">

                        <div class="col-lg-3 form-group">
                            <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                        </div>

                        <div class="col-lg-4 form-group">
                            <asp:DropDownList ID="ddlCliente" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                        </div>

                        <div class="col-lg-3 form-group" runat="server" id="DIV_Empresa">
                            <asp:DropDownList ID="ddlidEmpresa" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                        </div>

                        <div class="col-lg-2 form-group" runat="server" id="DIV_Finalizado">
                            <asp:DropDownList ID="ddlFinalizado" runat="server" class="Caixa_Selecao form-control">
                                <asp:ListItem Value="" Text="Todos os Status" />
                                <asp:ListItem Value="S" Text="Faturados" />
                                <asp:ListItem Selected="True" Value="N" Text="Em Aberto" />
                            </asp:DropDownList>
                        </div>

                    </div>
                    <div class="row">

                        <div class="col-lg-3" runat="server" id="DIV1">
                            <asp:DropDownList ID="ddlFluxo" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                        </div>

                        <div class="col-lg-4">
                            <asp:ListBox ID="lstAno" runat="server" SelectionMode="Multiple" CssClass="Caixa_Selecao"></asp:ListBox>
                        </div>

                        <div class="col-lg-2">
                            <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisa(this)" />
                        </div>

                    </div>
                </div>
            </div>

            <div id="graficos" class="panel panel-primary">
                <div class="panel-heading" style="cursor: pointer;" id="togglePanelExibeGrafico">
                    <h3 class="panel-title">
                        <i class="fa fa-pie-chart"></i>&nbsp;Gráficos
                        <span class="pull-right"><i class="fa fa-chevron-down" id="iconDownExibe"></i></span>
                    </h3>
                </div>
                <div class="panel-body" id="panelExibirGrafico">
                    <div class="form-group">
                        <div class="col-lg-12 row">

                            <div class="col-lg-4" id="DIV_GRAFICO" runat="server">
                                <div id="div_graficoTaxaFechamento" runat="server">
                                    <canvas id="grafico_Faturamento" style="width: 450px; height: 450px;"></canvas>
                                </div>
                            </div>

                            <div class="col-lg-4" id="div_gvMes" runat="server">
                                <asp:GridView ID="gvMes" class="table table-striped table-bordered table-hover"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvMes_RowDataBound">
                                    <Columns>

                                        <asp:BoundField DataField="sMes" HeaderText="Mês">
                                            <ItemStyle Width="50%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:HyperLinkField HeaderText="Previsão Faturamento"
                                            DataNavigateUrlFields="sMes" DataTextField="nValor" DataTextFormatString="{0:N2}"
                                            DataNavigateUrlFormatString="/App/Paginas/Pedidos.aspx?dashboard={0}">
                                            <ItemStyle Width="25%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField HeaderText="Valor Faturado"
                                            DataNavigateUrlFields="sMes" DataTextField="nValorFaturado" DataTextFormatString="{0:N2}"
                                            DataNavigateUrlFormatString="/App/Paginas/Pedidos.aspx?dashboard={0}">
                                            <ItemStyle Width="25%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                    </Columns>
                                </asp:GridView>
                            </div>

                            <div class="col-lg-4" id="DIV3" runat="server">
                                <div id="div4" runat="server">
                                    <canvas id="Faturamento_Tipos" style="width: 450px; height: 450px;"></canvas>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div id="pedidos_OPI" class="panel panel-primary">
                <div class="panel-heading" style="cursor: pointer;" id="togglePanelExibeOPI">
                    <h3 class="panel-title"><i class="fa fa-list-ul"></i>&nbsp;Pedidos / OPI
                         <span class="pull-right"><i class="fa fa-chevron-up" id="iconDownExibeOPI"></i></span>
                    </h3>
                </div>
                <div class="panel-body" id="panelExibirOPI" style="display: none;">
                    <div class="form-group">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_OPI" />
                            <div class="table-responsive">
                                <asp:GridView ID="gvOPI" class="table table-striped table-bordered table-hover "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvOPI_RowDataBound">
                                    <Columns>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="idOPI" HeaderText="ID"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="3%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="nNumeroPedido" HeaderText="N° Pedido"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="5%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="nControleTT" HeaderText="N° Controle"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="7%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="sReferencia" HeaderText="Referência"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="18%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="sDscCliente" HeaderText="Cliente"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="18%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="sDscStatus" HeaderText="Status OPI"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="8%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="nVlrProdutos" HeaderText="Valor Produtos"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="8%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="nVlrServicos" HeaderText="Valor Serviços"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="8%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="nVlrTotal" HeaderText="Valor Total"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="8%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="sDscCondicaoPagamento" HeaderText="Condição Pagamento"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="10%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="sDscEmpresas" HeaderText="Empresa"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="10%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="sDscTipoFaturamento" HeaderText="Faturamento"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="11%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="sDscFluxo" HeaderText="Fluxo"
                                            DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="15%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="sFinalizada" HeaderText="Faturada">
                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="idOPI" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="pedidos_Servicos" class="panel panel-primary">
                <div class="panel-heading" style="cursor: pointer;" id="togglePanelExibePedidos">
                    <h3 class="panel-title"><i class="fa fa-list-ul"></i>&nbsp;Serviços
                        <span class="pull-right"><i class="fa fa-chevron-up" id="iconDownExibePedidos"></i></span>
                    </h3>
                </div>
                <div class="panel-body" id="panelExibirPedidos" style="display: none;">
                    <div class="form-group">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_Servicos" />
                            <asp:Panel ID="Panel1" class="" runat="server">
                                <div class="table-responsive">
                                    <asp:GridView ID="gvPedidos" class="table table-striped table-bordered table-hover "
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvPedidos_RowDataBound">
                                        <Columns>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                DataTextField="idPedido" HeaderText="ID"
                                                DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?idPedido={0}">
                                                <HeaderStyle Width="3%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                DataTextField="nNumeroPedido" HeaderText="N° Pedido"
                                                DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?idPedido={0}">
                                                <HeaderStyle Width="5%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                DataTextField="nControleTT" HeaderText="N° Controle"
                                                DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?idPedido={0}">
                                                <HeaderStyle Width="7%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                DataTextField="sReferencia" HeaderText="Referência"
                                                DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?idPedido={0}">
                                                <HeaderStyle Width="18%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                DataTextField="sDscCliente" HeaderText="Cliente"
                                                DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?idPedido={0}">
                                                <HeaderStyle Width="18%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                DataTextField="nVlrProdutos" HeaderText="Valor Produtos"
                                                DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?idPedido={0}">
                                                <HeaderStyle Width="8%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                DataTextField="nVlrServicos" HeaderText="Valor Serviços"
                                                DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?idPedido={0}">
                                                <HeaderStyle Width="8%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                DataTextField="nVlrTotal" HeaderText="Valor Total"
                                                DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?idPedido={0}">
                                                <HeaderStyle Width="8%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                DataTextField="sDscCondicaoPagamento" HeaderText="Condição Pagamento"
                                                DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?idPedido={0}">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                DataTextField="sDscEmpresas" HeaderText="Empresa"
                                                DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?idPedido={0}">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                DataTextField="sDscTipoFaturamento" HeaderText="Faturamento"
                                                DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?idPedido={0}">
                                                <HeaderStyle Width="11%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                DataTextField="sDscFluxo" HeaderText="Fluxo"
                                                DataNavigateUrlFormatString="Faturamento_Detalhe.aspx?idPedido={0}">
                                                <HeaderStyle Width="15%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:BoundField DataField="sFinalizada" HeaderText="Faturada">
                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>

        </div>
    </div>

</asp:Content>
