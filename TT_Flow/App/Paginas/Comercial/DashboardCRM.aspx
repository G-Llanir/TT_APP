<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/App/main.master" CodeBehind="DashboardCRM.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.DashboardCRM" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <script src='https://cdn.jsdelivr.net/npm/chart.js'></script>

    <script>
        Chart.defaults.responsive = true;
        Chart.defaults.maintainAspectRatio = false;
    </script>

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

        .dashboard-crm-container {
            display: grid;
            gap: 12px;
            padding: 8px;
            background-color: #f5f5f5;
            border-radius: 8px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin: 5px 0 15px;
            grid-template-columns: 1fr;
            grid-template-areas: "main";
            align-items: start;
        }

        .dashboard-crm-main {
            grid-area: main;
            min-width: 0;
            align-self: start;
            display: flex;
            flex-direction: column;
            gap: 10px;
        }

        .dashboard-crm-main > .panel.panel-primary {
            margin-bottom: 0;
        }

        .dashboard-crm-graficos {
            display: grid;
            grid-template-columns: minmax(0, 1fr);
            grid-auto-flow: row dense;
            gap: 16px;
            width: 100%;
            align-items: stretch;
        }

            .dashboard-crm-graficos > [id^="div_grafico"] {
                min-width: 0;
                height: 100%;
                display: flex;
            }

                .dashboard-crm-graficos > [id^="div_grafico"] > [class*="col-"] {
                    width: 100%;
                    max-width: 100%;
                    float: none;
                    padding-left: 0;
                    padding-right: 0;
                    min-width: 0;
                    display: flex;
                }

                    .dashboard-crm-graficos > [id^="div_grafico"] > [class*="col-"] > .panel-primary {
                        display: flex;
                        flex: 1 1 auto;
                        flex-direction: column;
                        min-width: 0;
                        width: 100%;
                    }

            .dashboard-crm-graficos .panel-primary.grafico,
            .dashboard-crm-graficos .panel.panel-primary.grafico {
                display: flex;
                flex: 1 1 auto;
                flex-direction: column;
                margin-bottom: 0;
                min-width: 0;
            }

            .dashboard-crm-graficos .panel-body {
                position: relative;
                min-height: 240px;
                height: clamp(240px, 18vw, 300px);
                display: flex;
                align-items: center;
                justify-content: center;
                overflow: hidden;
                padding: 12px;
            }

                .dashboard-crm-graficos .panel-body canvas {
                    display: block;
                    max-width: 100% !important;
                    max-height: 100% !important;
                    width: 100% !important;
                    height: 100% !important;
                }

            .dashboard-crm-graficos .cabecalho {
                margin-bottom: 0;
                min-height: 56px;
                display: flex;
                align-items: center;
            }

            .dashboard-crm-graficos .cabecalho .row {
                width: 100%;
            }

        #div_graficoRankingClientes .panel-body,
        #div_graficoVendaFamilia .panel-body {
            min-height: 300px;
            height: clamp(300px, 24vw, 400px);
        }

        @media (min-width: 600px) {
            .dashboard-crm-graficos {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }

        @media (min-width: 1100px) {
            .dashboard-crm-graficos {
                grid-template-columns: repeat(3, minmax(0, 1fr));
            }
        }

        @media (min-width: 1600px) {
            .dashboard-crm-graficos {
                grid-template-columns: repeat(4, minmax(0, 1fr));
            }

            #div_graficoRankingClientes,
            #div_graficoVendaFamilia {
                grid-column: span 2;
            }
        }

        @media (max-width: 1599px) {
            #div_graficoRankingClientes,
            #div_graficoVendaFamilia {
                grid-column: span 1;
            }
        }

        @media (max-width: 599px) {
            .dashboard-crm-graficos {
                gap: 12px;
            }

            .dashboard-crm-graficos .panel-body,
            #div_graficoRankingClientes .panel-body,
            #div_graficoVendaFamilia .panel-body {
                min-height: 260px;
                height: 260px;
                padding: 8px;
            }
        }

        .dashboard-crm-painel-config,
        .dashboard-crm-painel-filtros {
            padding-top: 12px;
        }

        .preferencias-painel {
            display: grid;
            grid-template-columns: 1fr auto;
            gap: 16px 24px;
            align-items: start;
        }

        .preferencias-acoes {
            justify-self: end;
            align-self: start;
        }

        .filtros-painel {
            display: grid;
            grid-template-columns: 1fr;
            gap: 24px;
            align-items: start;
        }

        @media (min-width: 992px) {
            .filtros-painel {
                grid-template-columns: 1fr 340px;
                gap: 32px;
            }
        }

        .filtros-controles {
            display: flex;
            flex-direction: column;
            gap: 16px;
            min-width: 0;
        }

            .filtros-controles .form-group {
                margin-bottom: 0;
            }

            .filtros-controles .form-control {
                width: 100%;
                max-width: 100%;
            }

        .filtros-acoes {
            display: flex;
            align-items: flex-start;
        }

        .preferencias-painel > .checkbox-grid,
        .filtros-painel > .checkbox-grid {
            min-width: 0;
            max-width: 100%;
        }

        .checkbox-grid {
            min-width: 0;
            max-width: 100%;
        }

        @media (max-width: 767px) {
            .preferencias-painel {
                grid-template-columns: 1fr;
            }

            .preferencias-acoes {
                justify-self: stretch;
            }
        }

        .checkbox-grid table.checkbox-list-source {
            width: 100%;
            max-width: 100%;
            box-sizing: border-box;
            margin: 0;
            border: none;
            border-collapse: collapse;
            display: grid;
            grid-template-columns: repeat(auto-fill, minmax(min(100%, 150px), 1fr));
            column-gap: 24px;
            row-gap: 12px;
            min-width: 0;
        }

            .checkbox-grid table.checkbox-list-source tbody,
            .checkbox-grid table.checkbox-list-source tr {
                display: contents;
            }

            .checkbox-grid table.checkbox-list-source td {
                display: flex;
                align-items: flex-start;
                gap: 8px;
                min-width: 0;
                max-width: 100%;
                border: none;
                padding: 0;
                overflow: hidden;
            }

                .checkbox-grid table.checkbox-list-source td label {
                    margin: 0;
                    padding: 0;
                    font-weight: normal;
                    line-height: 1.35;
                    min-width: 0;
                    flex: 1 1 auto;
                    overflow-wrap: anywhere;
                }

                .checkbox-grid table.checkbox-list-source td input[type="checkbox"] {
                    margin: 0;
                    flex: 0 0 auto;
                    margin-top: 2px;
                }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked">

        <div class="row">
            <div class="col-lg-12">
                <h1>
                    <asp:Label ID="lblTituloPagina" runat="server" Text="Dashboard CRM"></asp:Label>
                </h1>
                <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
            </div>
        </div>

        <div class="row">
            <div class="col-lg-12" style="text-align: end;">
                <asp:LinkButton runat="server" ID="lnkAtualiza_Servicos" OnClick="lnkAtualizar_Click" class="btn btn-info" data-toggle="tooltip" title="Atualizar Informações"><i class="fa fa-refresh"></i></asp:LinkButton>
            </div>
        </div>

        <div class="dashboard-container dashboard-crm-container">
            <div class="dashboard-crm-main">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            <div class="panel panel-primary">
                <div class="panel-heading" style="cursor: pointer;" id="togglePanelPreferencias">
                    <h3 class="panel-title">
                        <i class="fa fa-bar-chart-o"></i>&nbsp;Preferências
                        <span class="pull-right">
                            <i class="fa fa-chevron-down" id="iconDownPreferencia"></i>
                        </span>
                    </h3>
                </div>


                <div class="panel-body dashboard-crm-painel-config" id="panelPreferencias" style="display: none">
                    <div class="preferencias-painel">
                        <div class="checkbox-grid" data-checkbox-grid="preferencias">
                            <asp:CheckBoxList ID="cblsPreferencia" runat="server"
                                CssClass="checkbox-list-source"
                                RepeatLayout="Table"
                                RepeatColumns="1"
                                RepeatDirection="Vertical">
                                <asp:ListItem Value="0">Taxa de Fechamento</asp:ListItem>
                                <asp:ListItem Value="1">Perdidos por Motivos</asp:ListItem>
                                <asp:ListItem Value="2">Chance de Fechamento</asp:ListItem>
                                <asp:ListItem Value="3">Estado de Cotações</asp:ListItem>
                                <asp:ListItem Value="4">Tempo Médio Fechamento</asp:ListItem>
                                <asp:ListItem Value="5">Ranking de Clientes</asp:ListItem>
                                <asp:ListItem Value="6">Vendas por Família</asp:ListItem>
                                <asp:ListItem Value="7">Orçamentos Realizados</asp:ListItem>
                                <asp:ListItem Value="8">Vendas por Estado</asp:ListItem>
                            </asp:CheckBoxList>
                        </div>
                        <div class="preferencias-acoes">
                            <asp:Button ID="cmdSalvarPreferencia" class="btn btn-success" runat="server" Text="Salvar" OnClick="cmdSalvarPreferencia_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="panel panel-primary">
                <div class="panel-heading" style="cursor: pointer;" id="togglePanelExibeGrafico">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtros
                        <span class="pull-right">
                            <i class="fa fa-chevron-down" id="iconDownExibe"></i>
                        </span>
                    </h3>
                </div>
                <div class="panel-body dashboard-crm-painel-filtros" id="panelExibirGrafico" style="display: none">
                    <div class="filtros-painel">
                        <div class="checkbox-grid" data-checkbox-grid="filtros">
                            <asp:CheckBoxList ID="cblsExibicao" runat="server"
                                CssClass="checkbox-list-source"
                                RepeatLayout="Table"
                                RepeatColumns="1"
                                RepeatDirection="Vertical">
                                <asp:ListItem Value="0">Taxa de Fechamento</asp:ListItem>
                                <asp:ListItem Value="1">Perdidos por Motivos</asp:ListItem>
                                <asp:ListItem Value="2">Chance de Fechamento</asp:ListItem>
                                <asp:ListItem Value="3">Estado de Cotações</asp:ListItem>
                                <asp:ListItem Value="4">Tempo Médio Fechamento</asp:ListItem>
                                <asp:ListItem Value="5">Ranking de Clientes</asp:ListItem>
                                <asp:ListItem Value="6">Vendas por Família</asp:ListItem>
                                <asp:ListItem Value="7">Orçamentos Realizados</asp:ListItem>
                                <asp:ListItem Value="8">Vendas por Estado</asp:ListItem>
                            </asp:CheckBoxList>
                        </div>

                        <div class="filtros-controles">
                            <div class="form-group">
                                <label>Vendedor</label>
                                <asp:DropDownList ID="ddlidVendedor" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                            </div>
                            <div class="form-group">
                                <label>Período</label>
                                <asp:DropDownList ID="ddlPeriodo" runat="server" class="form-control Caixa_Selecao">
                                    <asp:ListItem Text="Mês" Value="0" />
                                    <asp:ListItem Text="Trimestre" Value="1" />
                                    <asp:ListItem Text="Ano" Value="2" Selected="True" />
                                </asp:DropDownList>
                            </div>
                            <div class="filtros-acoes">
                                <asp:Button ID="cmdFiltro" class="btn btn-primary" runat="server" Text="Aplicar Filtros" OnClick="cmdFiltro_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnGraficos" CssClass="dashboard-crm-graficos" runat="server">

                        <div id="div_graficoTaxaFechamento" runat="server" ClientIDMode="Static">
                            <div class="col-md-3">
                                <div class="panel-primary">
                                    <div class="panel-heading cabecalho">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <h3 class="panel-title">
                                                    <i class="fa fa-pie-chart"></i>
                                                    <label class="titulo">Taxa de Fechamento</label>
                                                </h3>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-primary grafico">
                                        <div class="panel-body">
                                            <canvas id="grafico_taxa_fechamento" width="250" height="190"></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="div_graficoPerdidos" runat="server" ClientIDMode="Static">
                            <div class="col-md-3">
                                <div class="panel-primary">
                                    <div class="panel-heading cabecalho">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <h3 class="panel-title">
                                                    <i class="fa fa-pie-chart"></i>
                                                    <label class="titulo">Taxa de Perdidos por Motivos</label>
                                                </h3>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-primary grafico">
                                        <div class="panel-body">
                                            <canvas id='grafico_perdidos' width='250' height='200'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="div_graficoChanceFechamento" runat="server" ClientIDMode="Static">
                            <div class="col-md-3">
                                <div class="panel-primary">
                                    <div class="panel-heading cabecalho">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <h3 class="panel-title">
                                                    <i class="fa fa-pie-chart"></i>
                                                    <label class="titulo">Chance de Fechamento</label>
                                                </h3>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-primary grafico">
                                        <div class="panel-body">
                                            <canvas id='grafico_chance_fechamento' width='250' height='200'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="div_graficoEstadoCotacao" runat="server" ClientIDMode="Static">
                            <div class="col-md-3">
                                <div class="panel-primary">
                                    <div class="panel-heading cabecalho">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <h3 class="panel-title">
                                                    <i class="fa fa-pie-chart"></i>
                                                    <label class="titulo">Estado de Cotações</label>
                                                </h3>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-primary grafico">
                                        <div class="panel-body">
                                            <canvas id='grafico_estado_cotacao' width='250' height='200'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="div_graficoTempoFechamento" runat="server" ClientIDMode="Static">
                            <div class="col-md-3">
                                <div class="panel-primary">
                                    <div class="panel-heading cabecalho">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <h3 class="panel-title">
                                                    <i class="fa fa-pie-chart"></i>
                                                    <label class="titulo">Tempo Médio de Fechamento</label>
                                                </h3>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-primary grafico">
                                        <div class="panel-body">
                                            <canvas id='grafico_tempo_fechamento' width='250' height='200'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="div_graficoQtdOrcamento" runat="server" ClientIDMode="Static">
                            <div class="col-md-3">
                                <div class="panel-primary">
                                    <div class="panel-heading cabecalho">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <h3 class="panel-title">
                                                    <i class="fa fa-pie-chart"></i>
                                                    <label class="titulo">Orçamentos Realizados</label>
                                                </h3>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-primary grafico">
                                        <div class="panel-body">
                                            <canvas id='grafico_quantidade_cotacoes' width='250' height='200'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="div_graficoVendaEstado" runat="server" ClientIDMode="Static">
                            <div class="col-md-3">
                                <div class="panel-primary">
                                    <div class="panel-heading cabecalho">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <h3 class="panel-title">
                                                    <i class="fa fa-pie-chart"></i>
                                                    <label class="titulo">Vendas por Estado</label>
                                                </h3>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-primary grafico">
                                        <div class="panel-body">
                                            <canvas id='grafico_vendas_estado' width='250' height='200'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="div_graficoRankingClientes" runat="server" ClientIDMode="Static">
                            <div class="col-lg-6">
                                <div class="panel-primary">
                                    <div class="panel-heading cabecalho">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <h3 class="panel-title">
                                                    <i class="fa fa-pie-chart"></i>
                                                    <label class="titulo">Ranking de Clientes</label>
                                                </h3>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-primary grafico">
                                        <div class="panel-body">
                                            <canvas id='grafico_ranking_clientes' width='800' height='600'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div id="div_graficoVendaFamilia" runat="server" ClientIDMode="Static">
                            <div class="col-lg-6">
                                <div class="panel-primary">
                                    <div class="panel-heading cabecalho">
                                        <div class="row">
                                            <div class="col-lg-12">
                                                <h3 class="panel-title">
                                                    <i class="fa fa-pie-chart"></i>
                                                    <label class="titulo">Vendas por Família</label>
                                                </h3>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel panel-primary grafico">
                                        <div class="panel-body">
                                            <canvas id='grafico_vendas_familia' width='800' height='600'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>


                        <%--<div class="col-md-3">
                            <div class="panel-primary">
                                <div class="panel-heading cabecalho">
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <h3 class="panel-title">
                                                <i class="fa-pie-chart"></i>
                                                <label class="titulo">Meta Vendedor</label>
                                            </h3>
                                        </div>
                                    </div>
                                </div>
                                <div id="div_metaVendedor" runat="server">
                                    <div class="panel panel-primary grafico">
                                        <div class="panel-body">
                                            <canvas id='grafico_meta_vendedor' width='250' height='200'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>--%>

                        <%--<div class="col-lg-12">
                            <div class="panel-primary">
                                <div class="panel-heading">
                                    <div class="row">
                                        <div class="col-lg-12">
                                            <h3 class="panel-title">
                                                <i class="fa-pie-chart"></i>
                                                <label>Forma de Pagamento</label>
                                            </h3>
                                        </div>
                                    </div>
                                </div>
                                <div id="div_graficoFormaPagamento" runat="server">
                                    <div class="panel panel-primary">
                                        <div class="panel-body">
                                            <canvas id='grafico_forma_pagamento' width='400' height='400'></canvas>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>--%>
                    </asp:Panel>

            </div>
        </div>
    </div>

    <asp:HiddenField runat="server" ID="hddidUsuario" Value="0" />
    <asp:HiddenField runat="server" ID="hddidVendedor" Value="0" />
    <asp:HiddenField runat="server" ID="hddsTipoPeriodo" Value="2" />
    <asp:HiddenField runat="server" ID="hddsPreferencia" Value="0" />


    <script>
        $(document).ready(function () {
            var collapsePreferencia = true;

            $('#togglePanelPreferencias').click(function () {
                if (collapsePreferencia) {
                    $('#panelPreferencias').slideDown();
                    $('#iconDownPreferencia').removeClass('fa-chevron-down').addClass('fa-chevron-up');
                } else {
                    $('#panelPreferencias').slideUp();
                    $('#iconDownPreferencia').removeClass('fa-chevron-up').addClass('fa-chevron-down');
                }
                collapsePreferencia = !collapsePreferencia;
            });

            var collapseExibeGrafico = true;

            $('#togglePanelExibeGrafico').click(function () {
                if (collapseExibeGrafico) {
                    $('#panelExibirGrafico').slideDown();
                    $('#iconDownExibe').removeClass('fa-chevron-down').addClass('fa-chevron-up');
                } else {
                    $('#panelExibirGrafico').slideUp();
                    $('#iconDownExibe').removeClass('fa-chevron-up').addClass('fa-chevron-down');
                }
                collapseExibeGrafico = !collapseExibeGrafico;
            });

        });
    </script>

</asp:Content>
