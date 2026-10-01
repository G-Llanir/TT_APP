<%@ Page Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="FluxoCaixa.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.FluxoCaixa" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .fluxo-tabela.table { width: 100%; border: 4px solid #8593a3; border-collapse: collapse; table-layout: auto; font: 12px Tahoma, sans-serif; }
        .fluxo-tabela.table th, .fluxo-tabela.table td { padding: 8px 4px; vertical-align: middle; }
        .fluxo-tabela.table td { white-space: nowrap; }
        .fluxo-tabela .fluxo-periodo { min-width: 155px; white-space: nowrap; }
        .fluxo-tabela .fluxo-descricao { min-width: 235px; white-space: nowrap; }
        .fluxo-tabela .fluxo-total, .fluxo-tabela .fluxo-saldo-valor { min-width: 130px; font-weight: bold; }
        .fluxo-tabela .fluxo-coluna-saldo { min-width: 130px; }
        .fluxo-tabela th { white-space: normal; }
        .fluxo-tabela th.fluxo-grupo { text-align: center; font-weight: bold; }
        .fluxo-tabela .fluxo-grupo-entradas, .fluxo-tabela .fluxo-celula-entradas { background: #eaf3ff; }
        .fluxo-tabela .fluxo-grupo-saidas, .fluxo-tabela .fluxo-celula-saidas { background: #fff0f0; }
        .fluxo-tabela .fluxo-grupo-adiantamento, .fluxo-tabela .fluxo-celula-adiantamento { background: #f0f0f0; }
        .fluxo-tabela .fluxo-grupo-entradas, .fluxo-tabela .entrada-azul, .fluxo-tabela .entrada-azul a { color: #0066cc !important; }
        .fluxo-tabela .fluxo-grupo-saidas, .fluxo-tabela .saida-vermelha, .fluxo-tabela .saida-vermelha a { color: #c82333 !important; }
        .fluxo-tabela a.fluxo-link, .fluxo-tabela a.fluxo-link:hover { text-decoration: underline !important; color: inherit; }
        .fluxo-tabela .fluxo-fim-bloco { border-right: 4px solid #8593a3 !important; }
        .fluxo-tabela.fluxo-transposta tr > th:not(:last-child),
        .fluxo-tabela.fluxo-transposta tr > td:not(:last-child) { border-right: 4px solid #8593a3 !important; }
        .fluxo-tabela .fluxo-rodape > td, .fluxo-tabela .fluxo-linha-separador { border-top: 3px solid #8593a3; font-weight: bold; }
        .fluxo-tabela.fluxo-transposta .fluxo-rodape > td,
        .fluxo-tabela.fluxo-transposta .fluxo-linha-separador { border-top: 1px solid #ddd; }
        .fluxo-painel, .fluxo-painel .panel-body { min-width: 0; max-width: 100%; box-sizing: border-box; }
        .fluxo-painel .panel-body { padding: 12px; }
        .fluxo-painel-banco { margin-left: 24px; }
        .fluxo-painel .table-responsive { display: block; width: 100%; min-width: 0; max-width: 100%; overflow-x: auto; overscroll-behavior-x: contain; }
        .fluxo-filtros { margin-top: 12px; }
        .fluxo-opcao { padding-top: 12px; display: flex; align-items: center; gap: 12px; }
        .fluxo-switch { position: relative; display: inline-block; width: 82px; height: 24px; margin: 0; }
        .fluxo-switch input { position: absolute; opacity: 0; width: 40px; height: 24px; }
        .fluxo-slider { position: absolute; cursor: pointer; inset: 0 auto 0 0; width: 40px; background: #888; border-radius: 24px; transition: .2s; }
        .fluxo-slider:before { content: ''; position: absolute; height: 16px; width: 16px; left: 4px; bottom: 4px; background: white; border-radius: 50%; transition: .2s; }
        .fluxo-switch input:checked + .fluxo-slider { background: #2196f3; }
        .fluxo-switch input:checked + .fluxo-slider:before { transform: translateX(16px); }
        .fluxo-switch input:focus + .fluxo-slider { outline: 2px solid #174a79; outline-offset: 2px; }
        .fluxo-switch-estado { position: absolute; left: 48px; top: 2px; }
        .fluxo-switch-estado:after { content: 'Não'; }
        .fluxo-switch input:checked ~ .fluxo-switch-estado:after { content: 'Sim'; }
        .tab-content { padding-top: 20px; }
        .panel-grafico { padding: 20px; }
        #divGraficoFluxoCaixa { position: relative; height: 400px; width: 100%; }
        @media (max-width: 767px) { .fluxo-painel-banco { margin-left: 8px; } }
    </style>
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <%--<uc1:Manual runat="server" ID="manual" />--%>

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Pool Bancário"></asp:Label>
                <small>Consulta</small>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>

        <div class="col-lg-12" runat="server" id="DIV_Filtro">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtros para pesquisa</h3>
                </div>

                <div class="panel-body">
                    <div class="form-group">
                        <div class="row">

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:TextBox ID="txtdtInicio" class="form-control" runat="server" MaxLength="10" type="date"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:TextBox ID="txtdtFinal" class="form-control" runat="server" MaxLength="10" type="date"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <asp:ListBox ID="lstEmpresa" class="form-control Caixa_Selecao" SelectionMode="Multiple" runat="server"></asp:ListBox>
                                </div>
                            </div>

                            <div class="col-lg-3">
                                <div class="form-group">
                                    <asp:ListBox ID="lstContaBancaria" class="form-control Caixa_Selecao" SelectionMode="Multiple" runat="server"></asp:ListBox>
                                </div>
                            </div>



                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                </div>
                            </div>
                        </div>
                        <div class="row fluxo-filtros">
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlPeriodicidade" runat="server" AutoPostBack="true" class="form-control Caixa_Selecao">
                                        <asp:ListItem  Text="Visão Diária" Value="D"></asp:ListItem>
                                        <asp:ListItem Value="S" Selected="True" Text="Visão Semanal"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>


                            <div class="col-md-2">
                                <asp:DropDownList ID="ddlVisualizacao" runat="server" AutoPostBack="true" CssClass="form-control Caixa_Selecao">
                                    <asp:ListItem Value="L" >Visualizar Período por Linhas</asp:ListItem>
                                    <asp:ListItem Value="C" Selected="True">Visualizar Período por Colunas</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                            <div class="col-md-2 fluxo-opcao">
                                <span id="rotuloEmpresa">Quebrar por empresa</span>
                                <label class="fluxo-switch">
                                    <input type="checkbox" id="chkPorEmpresa" runat="server" checked="checked" aria-labelledby="rotuloEmpresa" />
                                    <span class="fluxo-slider"></span>
                                    <span class="fluxo-switch-estado" aria-hidden="true"></span>
                                </label>
                            </div>
                            <div class="col-md-2 fluxo-opcao">
                                <span id="rotuloBanco">Quebrar por bancos</span>
                                <label class="fluxo-switch">
                                    <input type="checkbox" id="chkPorBanco" runat="server" aria-labelledby="rotuloBanco" />
                                    <span class="fluxo-slider"></span>
                                    <span class="fluxo-switch-estado" aria-hidden="true"></span>
                                </label>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-12">

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

            <%-- Sistema de Abas --%>
            <ul class="nav nav-tabs" id="abasFluxoCaixa">
                <li class="active"><a href="#abaConsulta" data-toggle="tab"><i class="fa fa-table"></i> Consulta</a></li>
                <li><a href="#abaGrafico" data-toggle="tab"><i class="fa fa-line-chart"></i> Gráfico</a></li>
            </ul>           

            <div class="tab-content" style="padding-top: 20px;">

                <%-- ================= ABA CONSULTA ================= --%>
                <div class="tab-pane active" id="abaConsulta">

                    <asp:PlaceHolder ID="phGrades" runat="server" EnableViewState="false" />
                </div>
                <%-- ================= ABA GRÁFICO ================= --%>
                <div class="tab-pane" id="abaGrafico">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-line-chart"></i> Evolução do Fluxo de Caixa</h3>
                        </div>
                        <div class="panel-body panel-grafico">
                            <div id="divTituloGrafico" style="text-align: center; margin-bottom: 15px;">
                                <strong id="lblTituloGrafico"></strong>
                            </div>
                            <div id="divGraficoFluxoCaixa">
                                <canvas id="graficoFluxoCaixa"></canvas>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <script type="text/javascript">
        var chartFluxoCaixa = null;

        function atualizarGraficoFluxoCaixa(dadosGrafico) {
            var ctx = document.getElementById('graficoFluxoCaixa').getContext('2d');

            if (chartFluxoCaixa) {
                chartFluxoCaixa.destroy();
            }

            chartFluxoCaixa = new Chart(ctx, {
                type: 'line',
                data: {
                    labels: dadosGrafico.labels,
                    datasets: [
                        {
                            label: 'Entradas',
                            data: dadosGrafico.entradas,
                            borderColor: '#28a745',
                            backgroundColor: 'rgba(40, 167, 69, 0.1)',
                            borderWidth: 2,
                            fill: false,
                            tension: 0.3
                        },
                        {
                            label: 'Saídas',
                            data: dadosGrafico.saidas,
                            borderColor: '#dc3545',
                            backgroundColor: 'rgba(220, 53, 69, 0.1)',
                            borderWidth: 2,
                            fill: false,
                            tension: 0.3
                        },
                        {
                            label: 'Saldo',
                            data: dadosGrafico.saldo,
                            borderColor: '#007bff',
                            backgroundColor: 'rgba(0, 123, 255, 0.1)',
                            borderWidth: 2,
                            fill: false,
                            tension: 0.3
                        }
                    ]
                },
                options: {
                    responsive: true,
                    maintainAspectRatio: false,
                    interaction: {
                        mode: 'index',
                        intersect: false
                    },
                    plugins: {
                        title: {
                            display: true,
                            text: dadosGrafico.titulo,
                            font: {
                                size: 16,
                                weight: 'bold'
                            }
                        },
                        legend: {
                            display: true,
                            position: 'top',
                            labels: {
                                usePointStyle: true,
                                padding: 15
                            }
                        },
                        tooltip: {
                            enabled: true,
                            callbacks: {
                                label: function(context) {
                                    var label = context.dataset.label || '';
                                    if (label) {
                                        label += ': ';
                                    }
                                    var valor = context.parsed.y;
                                    label += valor.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
                                    return label;
                                }
                            }
                        }
                    },
                    scales: {
                        x: {
                            display: true,
                            title: {
                                display: true,
                                text: 'Período'
                            },
                            ticks: {
                                maxRotation: 45,
                                minRotation: 45
                            }
                        },
                        y: {
                            display: true,
                            title: {
                                display: true,
                                text: 'Valores (R$)'
                            },
                            ticks: {
                                callback: function(value) {
                                    return value.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL', minimumFractionDigits: 0, maximumFractionDigits: 0 });
                                }
                            }
                        }
                    }
                }
            });
        }
    </script>
</asp:Content>
