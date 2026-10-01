<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="DashboardComercial.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.DashboardComercial" %> 
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %> 
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

  <style>
    .cabecalho {
      background: none !important;
      color: black !important;
      border-color: black !important;
    }

    .titulo {
      font-size: larger !important;
    }

    .grafico {
      border: none !important;
    }

    .grafico-meta {
      max-width: 100%;
      height: 600px;
    }

    .grafico-faturamento {
      max-width: 100%;
      height: 500px;
    }

    .dashboard-main-content {
      min-width: 0;
    }

    #panelExibirGrafico .dashboard-filtros-linha {
      display: flex;
      flex-wrap: wrap;
      align-items: flex-end;
      gap: 10px 12px;
      margin-left: 0;
      margin-right: 0;
    }

    #panelExibirGrafico .dashboard-filtros-linha > [class*="col-"] {
      float: none;
      width: auto;
      padding-left: 0;
      padding-right: 0;
      min-width: 0;
    }

    #panelExibirGrafico .dashboard-filtro-data {
      flex: 0 1 140px;
      max-width: 160px;
    }

    #panelExibirGrafico .dashboard-filtro-multi {
      flex: 1 1 180px;
      min-width: 150px;
    }

    #panelExibirGrafico .dashboard-filtro-acao {
      flex: 0 0 auto;
    }

    #panelExibirGrafico .dashboard-filtro-data .form-control,
    #panelExibirGrafico .dashboard-filtro-multi .form-control {
      width: 100%;
      max-width: 100%;
    }

    #panelExibirGrafico .dashboard-filtro-multi .chosen-container {
      width: 100% !important;
      max-width: 100%;
    }

  </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
  <div
    id="DIV_ESPACO"
    style="height: 600px"
    runat="server"
    visible="false"
  ></div>

  <div class="form-stacked">
    <div class="row">
      <div class="col-lg-12">
        <h1>
          <asp:Label
            ID="lblTituloPagina"
            runat="server"
            Text="Dashboard Comercial"
          ></asp:Label>
        </h1>
        <uc1:BreadCrumb
          runat="server"
          ID="BreadCrumb_Pagina"
          NivelPagina="2"
          TitulodaPagina=""
        />
      </div>
    </div>

    <div class="row">
      <div class="col-md-12 dashboard-main-content">
        <div id="div_pagina">
          <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
          <div class="panel panel-primary" id="div_panelFiltros">
            <div class="panel-heading">
              <h3 class="panel-title">
                <i class="fa fa-bar-chart-o"></i>&nbsp;Filtros
              </h3>
            </div>
            <div class="panel-body" id="panelExibirGrafico">
              <div class="form-group">
                <div class="col-lg-12" style="padding: 0px">
                  <div class="row dashboard-filtros-linha">
                    <div class="col-lg-1 dashboard-filtro-data">
                      <div class="form-group">
                        <label>Data Início</label>
                        <asp:TextBox
                          ID="txtdtInicio"
                          class="form-control"
                          runat="server"
                          TextMode="Date"
                        ></asp:TextBox>
                      </div>
                    </div>

                    <div class="col-lg-1 dashboard-filtro-data">
                      <div class="form-group">
                        <label>Data Final</label>
                        <asp:TextBox
                          ID="txtdtFinal"
                          class="form-control"
                          runat="server"
                          TextMode="Date"
                        ></asp:TextBox>
                      </div>
                    </div>

                    <div class="col-lg-3 dashboard-filtro-multi">
                      <div class="form-group">
                        <label>Vendedor</label>
                        <asp:ListBox
                          ID="lstVendedor"
                          class="form-control Caixa_Selecao"
                          SelectionMode="Multiple"
                          runat="server"
                        ></asp:ListBox>
                      </div>
                    </div>

                    <div class="col-lg-3 dashboard-filtro-multi">
                      <div class="form-group">
                        <label>Fluxo</label>
                        <asp:ListBox
                          ID="lstFluxo"
                          class="form-control Caixa_Selecao"
                          SelectionMode="Multiple"
                          runat="server"
                        ></asp:ListBox>
                      </div>
                    </div>

                    <div class="col-lg-3 dashboard-filtro-multi">
                      <div class="form-group">
                        <label>Empresa</label>
                        <asp:ListBox
                          ID="lstEmpresa"
                          class="form-control Caixa_Selecao"
                          SelectionMode="Multiple"
                          runat="server"
                        ></asp:ListBox>
                      </div>
                    </div>

                    <div class="col-lg-1 dashboard-filtro-acao">
                      <div class="form-group">
                        <asp:Button
                          ID="cmdFiltro"
                          class="btn btn-primary"
                          runat="server"
                          Text="Aplicar Filtros"
                          OnClick="cmdFiltro_Click"
                          Style="margin-top: 23px"
                        />
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>

          <div class="panel panel-primary" id="div_panelPedidos">
            <div class="panel-heading" style="cursor: pointer;" id="togglePanelPedidos">
                <h3 class="panel-title">
                    <i class="fa fa-bar-chart-o"></i>&nbsp;Pedidos
                <span class="pull-right">
                    <i class="fa fa-chevron-up" id="iconDownPedidos"></i>
                </span>
                </h3>
            </div>
            <div class="panel-body" id="panelPedidos">


                <%-- <div id="div_graficoPedidosGeral" runat="server">
                <div class="col-lg-6">
                    <div class="panel-primary">
                        <div class="panel-heading cabecalho">
                            <div class="row">
                                <div class="col-lg-12">
                                    <h3 class="panel-title">                                                            
                                        <label class="titulo">Pedidos por Status</label>
                                    </h3>
                                </div>
                            </div>
                        </div>
                        <div class="panel panel-primary grafico">
                            <div class="panel-body table-responsive">
                                <asp:GridView ID="gvPedidosGeral" class="table table-striped table-bordered table-hover tabela-dataTable" DataKeyNames="idStatus"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvPedidosGeral_RowDataBound">
                                    <Columns>

                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <asp:HyperLink runat="server" ID="hlsStatusPedidos" Text='<%# Eval("sDscStatus") %>' Target="_blank"></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="nQtd" HeaderText="Quantidade">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nTotal" HeaderText="Valor Total" DataFormatString="{0:N2}">
                                            <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </div>--%>

                <div id="div_graficoPedidosClientes" runat="server" class="col-lg-6" style="max-height: 600px; overflow:auto">
                    <div class="panel-primary">
                        <div class="panel-heading cabecalho">
                            <div class="row">
                                <div class="col-lg-12">
                                    <h3 class="panel-title">
                                        <i class="fa fa-table"></i>
                                        <label class="titulo">Ranking Clientes Pedidos</label>
                                    </h3>
                                </div>
                            </div>
                        </div>
                        <div class="panel panel-primary grafico">
                            <div class="panel-body table-responsive">
                                <div class="table-responsive">
                                    <asp:GridView ID="gvPedidosClientes" class="table table-striped table-bordered table-hover tabela-dataTable"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idCliente"
                                        ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvPedidosClientes_RowDataBound">
                                        <Columns>

                                            <asp:TemplateField HeaderText="Cliente">
                                                <ItemTemplate>
                                                    <asp:HyperLink runat="server" ID="hlsClientePedido" Text='<%# Eval("sRazaoSocial") %>' Target="_blank"></asp:HyperLink>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="nQtd" HeaderText="Quantidade de Pedidos">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nTotal" HeaderText="Valor" DataFormatString="{0:N2}">
                                                <ItemStyle Width="15%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nPrevisaoFaturamento" HeaderText="A Faturar" DataFormatString="{0:N2}">
                                                <ItemStyle Width="15%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nValorFaturado" HeaderText="Faturado" DataFormatString="{0:N2}">
                                                <ItemStyle Width="15%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="div_graficoPedidosFluxo" runat="server" class="col-lg-6" style="max-height: 600px; min-height: 600px; overflow:auto">
                    <div class="panel-primary">
                        <div class="panel-heading cabecalho">
                            <div class="row">
                                <div class="col-lg-12">
                                    <h3 class="panel-title">
                                        <i class="fa fa-table"></i>
                                        <label class="titulo">Pedidos por Fluxo</label>
                                    </h3>
                                </div>
                            </div>
                        </div>
                        <div class="panel panel-primary grafico">
                            <div class="panel-body table-responsive">
                                <asp:GridView ID="gvPedidosFluxo" class="table table-striped table-bordered table-hover tabela-dataTable"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idFluxo"
                                    ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvPedidosFluxo_RowDataBound">
                                    <Columns>

                                        <asp:TemplateField HeaderText="Fluxo">
                                            <ItemTemplate>
                                                <asp:HyperLink runat="server" ID="hlsFluxoPedido" Text='<%# Eval("sDscFluxo") %>' Target="_blank"></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="nQtd" HeaderText="Quantidade">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nTotal" HeaderText="Valor Total" DataFormatString="{0:N2}">
                                            <ItemStyle Width="20%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nValorFaturado" HeaderText="Faturado" DataFormatString="{0:N2}">
                                            <ItemStyle Width="15%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nPrevisaoFaturamento" HeaderText="A Faturar" DataFormatString="{0:N2}">
                                            <ItemStyle Width="15%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                </div>

                <div id="div_graficoPedidosVendedor" runat="server" class="col-lg-6">
                    <div class="panel-primary">
                        <div class="panel-heading cabecalho">
                            <div class="row">
                                <div class="col-lg-12">
                                    <h3 class="panel-title">
                                        <i class="fa fa-table"></i>
                                        <label class="titulo">Pedidos por Vendedor</label>
                                    </h3>
                                </div>
                            </div>
                        </div>
                        <div class="panel panel-primary grafico">
                            <div class="panel-body table-responsive">
                                <asp:GridView ID="gvPedidosVendedor" class="table table-striped table-bordered table-hover tabela-dataTable"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idVendedor"
                                    ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvPedidosVendedor_RowDataBound">
                                    <Columns>

                                        <asp:TemplateField HeaderText="Vendedor">
                                            <ItemTemplate>
                                                <asp:HyperLink runat="server" ID="hlsDscVendedor" Text='<%# Eval("sDscVendedor") %>' Target="_blank"></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="nQtd" HeaderText="Quantidade">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nTotal" HeaderText="Valor Total" DataFormatString="{0:N2}">
                                            <ItemStyle Width="20%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nValorFaturado" HeaderText="Faturado" DataFormatString="{0:N2}">
                                            <ItemStyle Width="15%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nPrevisaoFaturamento" HeaderText="A Faturar" DataFormatString="{0:N2}">
                                            <ItemStyle Width="15%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                </div>

                <div id="div_graficoOrcamentosGeral" runat="server" class="col-lg-6">
                    <div class="panel-primary">
                        <div class="panel-heading cabecalho">
                            <div class="row">
                                <div class="col-lg-12">
                                    <h3 class="panel-title">
                                        <i class="fa fa-table"></i>
                                        <label class="titulo">Orçamentos por Status</label>
                                    </h3>
                                </div>
                            </div>
                        </div>
                        <div class="panel panel-primary grafico">
                            <div class="panel-body table-responsive">
                                <asp:GridView ID="gvOrcamentoGeral" class="table table-striped table-bordered table-hover tabela-dataTable" DataKeyNames="idStatus"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvOrcamentoGeral_RowDataBound">
                                    <Columns>

                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <asp:HyperLink runat="server" ID="hlsStatusOrcamento" Text='<%# Eval("sDscStatus") %>' Target="_blank"></asp:HyperLink>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="nQtd" HeaderText="Quantidade">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nTotal" HeaderText="Valor Total" DataFormatString="{0:N2}">
                                            <ItemStyle Width="30%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="div_graficoPedidosMetaVendedor" runat="server">
                    <div class="col-lg-12">
                        <div class="panel-primary">
                            <div class="panel-heading cabecalho">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <h3 class="panel-title">
                                            <i class="fa fa-bar-chart"></i>
                                            <label class="titulo">Metas Vendedor x Pedidos</label>
                                        </h3>
                                    </div>
                                </div>
                            </div>
                            <div class="panel panel-primary grafico">
                                <div class="panel-body grafico-meta">
                                    <canvas id='grafico_pedidos_meta_vendedor'></canvas>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>

        <div class="panel panel-primary" id="div_panelFaturamento">
            <div class="panel-heading" style="cursor: pointer;" id="togglePanelFaturamento">
                <h3 class="panel-title">
                    <i class="fa fa-bar-chart-o"></i>&nbsp;Faturamento
                <span class="pull-right">
                    <i class="fa fa-chevron-up" id="iconDownFaturamento"></i>
                </span>
                </h3>
            </div>
            <div class="panel-body" id="panelFaturamento">

                <div id="div_graficoFaturamentoGeral" runat="server">
                    <div class="col-lg-12">
                        <div class="panel-primary">
                            <div class="panel-heading cabecalho">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <h3 class="panel-title">
                                            <label class="titulo">Faturamento</label>
                                        </h3>
                                    </div>
                                </div>
                            </div>
                            <div class="panel panel-primary grafico">
                                <div class="panel-body">
                                    <div class="col-lg-4 table-responsive">
                                        <asp:GridView ID="gvFaturamento" class="table table-striped table-bordered table-hover tabela-dataTable"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvFaturamento_RowDataBound">
                                            <Columns>

                                                <asp:BoundField DataField="nMes" HeaderText="Mês">
                                                    <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sMes" HeaderText="Mês/Ano">
                                                    <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Previsão Faturamento">
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="nValor"
                                                            NavigateUrl='<%# Eval("sMes", "Pedidos.aspx?dashboard={0}") %>'
                                                            Text='<%# Eval("nValor") %>'
                                                            runat="server" Target="_blank" />
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="2%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Valor Faturado">
                                                    <ItemTemplate>
                                                        <asp:HyperLink ID="nValorFaturado"
                                                            NavigateUrl='<%# Eval("sMes", "Pedidos.aspx?dashboard={0}") %>'
                                                            Text='<%# Eval("nValorFaturado") %>'
                                                            runat="server" Target="_blank" />
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="2%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                    <div class="col-lg-8 grafico-faturamento">
                                        <canvas id='grafico_faturamento_geral'></canvas>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div id="div_graficoFaturamentoFluxo" runat="server">
                    <div class="col-lg-12">
                        <div class="panel-primary">
                            <div class="panel-heading cabecalho">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <h3 class="panel-title">
                                            <i class="fa fa-table"></i>
                                            <label class="titulo">Faturamento por Fluxo</label>
                                        </h3>
                                    </div>
                                </div>
                            </div>
                            <div class="panel panel-primary grafico">
                                <div class="panel-body table-responsive">
                                    <asp:GridView ID="gvFaturamentoFluxo" runat="server"
                                        AutoGenerateColumns="False"
                                        ShowFooter="True"
                                        CssClass="table table-striped table-bordered"
                                        OnRowDataBound="gvFaturamentoFluxo_RowDataBound">
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
        </div>
      </div>
    </div>
  </div>

  <asp:HiddenField runat="server" ID="hddidUsuario" Value="0" />
  <asp:HiddenField runat="server" ID="hddidVendedor" Value="0" />

  <script type="text/javascript" title="togglePanel_script">
    $(document).ready(function () {
      var collapsePedidos = false;

      $("#togglePanelPedidos").click(function () {
        if (collapsePedidos) {
          $("#panelPedidos").slideDown();
          $("#iconDownPedidos")
            .removeClass("fa-chevron-down")
            .addClass("fa-chevron-up");
        } else {
          $("#panelPedidos").slideUp();
          $("#iconDownPedidos")
            .removeClass("fa-chevron-up")
            .addClass("fa-chevron-down");
        }
        collapsePedidos = !collapsePedidos;
      });

      var collapseFaturamento = false;

      $("#togglePanelFaturamento").click(function () {
        if (collapseFaturamento) {
          $("#panelFaturamento").slideDown();
          $("#iconDownFaturamento")
            .removeClass("fa-chevron-down")
            .addClass("fa-chevron-up");
        } else {
          $("#panelFaturamento").slideUp();
          $("#iconDownFaturamento")
            .removeClass("fa-chevron-up")
            .addClass("fa-chevron-down");
        }
        collapseFaturamento = !collapseFaturamento;
      });

    });
  </script>
</asp:Content>
