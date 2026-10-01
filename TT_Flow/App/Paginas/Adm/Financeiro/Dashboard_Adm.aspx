<%@ Page Title="" Language="C#" MasterPageFile="~/app/main.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Dashboard_Adm.aspx.cs" Inherits="TT_Flow.Dashboards.Adm.Dashboard_Adm" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb_Pagina" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="Server">
    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>
    <style>
        .announcement-text2 {
            margin: 0;
            font-size: 11px;
        }

        .announcement-text {
            margin: 0;
            font-size: 15px;
        }

        .announcement-heading {
            font-size: 36px;
        }

        .panel-orange > .panel-heading {
            color: #8a6d3b;
            background-color: #FFCF8D;
            border-color: #faa236;
        }

        .caixaBotao {
        }

            .caixaBotao:hover {
                box-shadow: 3px 3px 7px #337ab7, -3px -3px 7px #337ab7;
                -webkit-transition: box-shadow ease-out 0.1s;
                transition: box-shadow ease-out 0.1s;
            }

        /*Botão switch*/
        .switch {
            position: relative;
            display: inline-block;
            width: 40px;
            height: 24px;
        }

            .switch input {
                opacity: 0;
                width: 0;
                height: 0;
            }

        .slider {
            position: absolute;
            cursor: pointer;
            top: 0;
            left: 0;
            right: 0;
            bottom: 0;
            background-color: #ccc;
            transition: .4s;
            border-radius: 24px;
        }

            .slider:before {
                position: absolute;
                content: "";
                height: 16px;
                width: 16px;
                left: 4px;
                bottom: 4px;
                background-color: white;
                transition: .4s;
                border-radius: 50%;
            }

        input:checked + .slider {
            background-color: #2196F3;
        }

        input:focus + .slider {
            box-shadow: 0 0 1px #2196F3;
        }

        input:checked + .slider:before {
            -webkit-transform: translateX(16px);
            transform: translateX(16px);
        }


        #tbRecebimentos tr, #tbAnual tr, #tbPagamentos tr {
            transition: all 0.1s ease;
        }

            #tbRecebimentos tr:hover, #tbAnual tr:hover, #tbPagamentos tr:hover {
                background-color: #e0e0e0 !important;
                cursor: pointer;
                box-shadow: 0 4px 8px rgba(0,0,0,0.1);
                transform: translateY(-2px);
                color: #333;
                font-weight: bold;
            }

        .panel-primary > .panel-heading {
            color: #fff;
            background-color: #009A22;
            border-color: #033712;
        }

        .adiantamento-atrasado {
            color:red;
        }

        .adiantamento-padrao{

        }

        .dashboard-adm-kpis {
            display: grid;
            grid-template-columns: 1fr 1fr;
            grid-template-rows: auto auto;
            gap: 15px;
            grid-auto-flow: row;
            grid-template-areas:
                "contas-pagar contas-receber"
                "ano-financeiro ano-financeiro";
            margin-bottom: 15px;
        }

        .dashboard-adm-kpi {
            min-width: 0;
            min-height: 0;
            display: flex;
            flex-direction: column;
        }

            .dashboard-adm-kpi > .panel.panel-primary,
            .dashboard-adm-kpi-receber .panel.panel-primary {
                flex: 1 1 auto;
                display: flex;
                flex-direction: column;
                min-height: 0;
                margin-bottom: 0;
            }

        .dashboard-adm-kpi-pagar {
            grid-area: contas-pagar;
        }

        .dashboard-adm-kpi-receber {
            grid-area: contas-receber;
        }

            .dashboard-adm-kpi-receber > .row {
                flex: 1 1 auto;
                display: flex;
                min-height: 0;
                margin-left: 0;
                margin-right: 0;
            }

                .dashboard-adm-kpi-receber > .row > [class*="col-"] {
                    flex: 1 1 auto;
                    display: flex;
                    flex-direction: column;
                    min-height: 0;
                    padding-left: 0;
                    padding-right: 0;
                }

        .dashboard-adm-kpi-ano {
            grid-area: ano-financeiro;
        }

        @media (max-width: 991px) {
            .dashboard-adm-kpis {
                grid-template-columns: 1fr;
                grid-template-rows: auto;
                grid-template-areas:
                    "contas-pagar"
                    "contas-receber"
                    "ano-financeiro";
            }
        }
       
    </style>

    <script type="text/javascript">


        document.addEventListener('DOMContentLoaded', function () {
            //updateLinks();
           // atualizaGraficoeRec();
            //atualizaGraficoePag();

            var ddlEmpresa = document.getElementById('<%= ddlidEmpresa.ClientID %>');       
                        
            ddlEmpresa.addEventListener('change', updateAll);
            

            function updateAll() {
                updateLinks();
                atualizaGraficoeRec();
                atualizaGraficoePag();
            }

            //document.querySelector('body').addEventListener('click', function (e) {
            //    if (e.target.closest('tr[data-url]')) {
            //        window.location.href = e.target.closest('tr[data-url]').getAttribute('data-url');
            //    }
            //});
        });

        function updateLinks() {
            
            var ddlEmpresa = document.getElementById('<%= ddlidEmpresa.ClientID %>');
            var lstFluxo = document.getElementById('<%= lstidFluxo.ClientID %>');

            //var selectedAno = ddlAno.value;
            var selectedEmpresa = ddlEmpresa.value;
            var selectedAno = Array.from(lstFluxo.selectedOptions).map(option => option.value).join(',');
            //console.log(selectedAno);

            var rows = document.querySelectorAll('table#tbAnual tr[data-url],table#tbRecebimentos tr[data-url], table#tbPagamentos tr[data-url]');

            rows.forEach(function (row) {
                var baseurl = row.getAttribute('data-url').split('?')[0];
                var dashboardType = row.getAttribute('data-dashboard');
                var newurl = `${baseurl}?dashboard=${dashboardType}&sAno=${selectedAno}&sEmpresa=${selectedEmpresa}`;
                row.setAttribute('data-url', newurl);
            });
        }

        //Script para sumir com o bar
        function updateChartData() {
            var newData = chartData.map(function (item) {
                return {
                    Mes: item.Mes,
                    Receber: $('#switchReceber').is(':checked') ? item.Receber : 0,
                    Pagar: $('#switchPagar').is(':checked') ? item.Pagar : 0
                };
            });
            $('#GraficoContabil').empty();
            renderChart(newData);
        }

        $('#switchReceber').change(updateChartData);
        $('#switchPagar').change(updateChartData);

        $(function () {
            $('[id*=lstidFluxo]').multiselect({
                buttonWidth: '195px',
                includeSelectAllOption: true,
                maxHeight: 300,
                dropRight: true,
                nSelectedText: ' - Anos Selecionados!',
                allSelectedText: 'Todos os Anos',
                enableFiltering: false
            });
        });

        function redirectToDashboard(row) {
            var dashboardType = row.getAttribute('data-dashboard');
            var url = generateUrl(dashboardType);
            window.open(url, '_blank'); 
        }

    </script>


    <div class="row">
        <div class="col-lg-8">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Dashboard Financeiro"></asp:Label>
            </h1>
            <%--          <h1>Dashboard <small>T-Flow - Painel Financeiro</small></h1>--%>
        </div>
        <div class="col-lg-4">
            <div class="form-inline" style="padding-top: 20px;">
                <div class="form-group">
                    <asp:DropDownList ID="ddlidEmpresa" runat="server" AutoPostBack="true" class="form-control yes_no select" OnSelectedIndexChanged="ddlidEmpresa_SelectedIndexChanged"></asp:DropDownList>
                </div>
                <div class="form-group" style="margin-left: 10px;">
<%--                <asp:DropDownList ID="ddlAnoFinanceiro" runat="server" AutoPostBack="true" class="form-control yes_no select" OnSelectedIndexChanged="ddlAnoFinanceiro_SelectedIndexChanged"></asp:DropDownList>--%>
                    <asp:ListBox ID="lstidFluxo" runat="server" SelectionMode="Multiple"></asp:ListBox>
                    <asp:LinkButton ID="lnkAtualizar" CssClass="btn btn-small btn-circle" runat="server" OnClick="lnkAtualizar_Click" OnClientClick="updateAll()"><i class="fa-refresh fa"></i></asp:LinkButton>
                </div>
                <div class="form-group">
                    <asp:DropDownList ID="ddlExibicao" runat="server" AutoPostBack="true" class="form-control yes_no select" OnSelectedIndexChanged="ddlExibicao_SelectedIndexChanged">
                        <asp:ListItem Selected="True" Value="0">Semana Atual</asp:ListItem>
                        <asp:ListItem Value="6">Mes Atual</asp:ListItem>
                        <asp:ListItem Value="1">Trimestre 1</asp:ListItem>
                        <asp:ListItem Value="2">Trimestre 2</asp:ListItem>
                        <asp:ListItem Value="3">Trimestre 3</asp:ListItem>
                        <asp:ListItem Value="4">Trimestre 4</asp:ListItem>
                        <asp:ListItem Value="5">Anual</asp:ListItem>
                    </asp:DropDownList>
                </div>
            </div>
        </div>
    </div>
    <div class="row">
        <div class="col-lg-12">
            <uc1:BreadCrumb_Pagina runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="DashBoard" />
        </div>
    </div>

    <!-- /.row -->

    <div class="dashboard-adm-kpis">

            <div class="dashboard-adm-kpi dashboard-adm-kpi-pagar">
                <div class="panel panel-primary">
                    <div class="panel-heading">
                        <div class="row">
                            <div class="col-lg-10">
                                <h3 class="panel-title">
                                    <i class="fa fa-bar-chart-o"></i>
                                    <asp:Label ID="lblTituloContasPagar" runat="server" Text="Contas a Pagar"></asp:Label>
                                </h3>
                            </div>
                            <div class="col-lg-2" runat="server" id="Div_Switch_Pag">
                                <div class="toggle-switch" style="padding-top: 7px;">
                                    <label class="switch">
                                        <input type="checkbox" id="switchPag" onchange="atualizaGraficoePag();">
                                        <span class="slider round"></span>
                                    </label>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel-body" runat="server" id="Div_Detalhe_Pag">
                        <div class="row">

                            <div class="col-lg-2">
                                <asp:HyperLink ID="lnkPagamentosEmAtraso" runat="server">
                                    <div class="panel panel-danger caixaBotao">
                                        <div class="panel-heading text-center" style="padding-bottom: 0;">
                                            <div class="panel-heading">
                                                <div class="row">
                                                    <div class="col-xs-12 text-center">
                                                        <p class="announcement-heading">
                                                            <asp:Label ID="lblQtdPagamentosEmAtraso" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text2">
                                                            <asp:Label ID="lblValorPagamentosEmAtraso" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text">Em Atraso</p>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:HyperLink>
                            </div>

                            <div class="col-lg-2">
                                <asp:HyperLink ID="lnklblPagamentosVencendoHoje" runat="server">
                                    <div class="panel panel-orange caixaBotao">
                                        <div class="panel-heading text-center" style="padding-bottom: 0;">
                                            <div class="panel-heading">
                                                <div class="row">
                                                    <div class="col-xs-12 text-center">
                                                        <p class="announcement-heading">
                                                            <asp:Label ID="lblQtdPagamentosVencendoHoje" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text2">
                                                            <asp:Label ID="lblValorPagamentosVencendoHoje" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text">Hoje</p>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:HyperLink>
                            </div>

                            <div class="col-lg-2">
                                <asp:HyperLink ID="lnkPagamentosVencendoSeteDias" runat="server">
                                    <div class="panel panel-warning caixaBotao">
                                        <div class="panel-heading text-center" style="padding-bottom: 0;">
                                            <div class="panel-heading">
                                                <div class="row">
                                                    <div class="col-xs-12 text-center">
                                                        <p class="announcement-heading">
                                                            <asp:Label ID="lblQtdPagamentosVencendoSeteDias" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text2">
                                                            <asp:Label ID="lblValorPagamentosVencendoSeteDias" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text">Até 7 dias</p>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:HyperLink>
                            </div>

                            <div class="col-lg-2">
                                <asp:HyperLink ID="lnkPagamentosVencerTrintaDias" runat="server">
                                    <div class="panel panel-info caixaBotao">
                                        <div class="panel-heading text-center" style="padding-bottom: 0;">
                                            <div class="panel-heading">
                                                <div class="row">
                                                    <div class="col-xs-12 text-center">
                                                        <p class="announcement-heading">
                                                            <asp:Label ID="lblQtdPagamentosVencerTrintaDias" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text2">
                                                            <asp:Label ID="lblValorPagamentosVencerTrintaDias" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text">Até 30 Dias</p>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:HyperLink>
                            </div>

                            <div class="col-lg-2">
                                <asp:HyperLink ID="lnkPagamentosVencerMaisdeTrinta" runat="server">
                                    <div class="panel panel-info caixaBotao">
                                        <div class="panel-heading text-center" style="padding-bottom: 0;">
                                            <div class="panel-heading">
                                                <div class="row">
                                                    <div class="col-xs-12 text-center">
                                                        <p class="announcement-heading">
                                                            <asp:Label ID="lblQtdPagamentosVencerMaisdeTrinta" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text2">
                                                            <asp:Label ID="lblValorPagamentosVencerMaisdeTrinta" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text">+30 Dias</p>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:HyperLink>
                            </div>

                            <div class="col-lg-2">
                                <asp:HyperLink ID="lnkPagamentosTotalEmAberto" runat="server">
                                    <div class="panel panel-info caixaBotao">
                                        <div class="panel-heading text-center" style="padding-bottom: 0;">
                                            <div class="panel-heading">
                                                <div class="row">
                                                    <div class="col-xs-12 text-center">
                                                        <p class="announcement-heading">
                                                            <asp:Label ID="lblQtdPagamentosTotalEmAberto" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text2">
                                                            <asp:Label ID="lblValorPagamentosTotalEmAberto" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text">Total</p>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:HyperLink>
                            </div>

                        </div>
                    </div>

                    <div class="panel-body" runat="server" id="Div_Grafico_Pag">

                        <div style="width: 100%; height: 300px; margin-bottom: 20px;">
                            <div id="GraficoPagamentos" style="width: 100%; height: 100%;"></div>
                        </div>

                        <div style="width: 100%; display: flex; justify-content: center;">

                           <asp:Literal ID="litTabPagamento" runat="server"></asp:Literal>

                            <%--<div class="table-responsive" style="width: 100%;">
                                <table class="table table-hover" id="tbPagamentos">
                                    <thead>
                                        <tr>
                                            <th>Categoria</th>
                                            <th>Quantidade</th>
                                            <th>Valor</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr style="background-color: #FB5B68;" data-url="/app/Paginas/Adm/Financeiro/ContasPagar.aspx?dashboard=EmAtraso" data-dashboard="EmAtraso">

                                            <td>Em Atraso</td>
                                            <td>
                                                <asp:Label ID="lblQtdPagamentosEmAtraso_Grafico" runat="server"></asp:Label></td>
                                            <td>
                                                <asp:Label ID="lblValorPagamentosEmAtraso_Grafico" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr style="background-color: #F88A30;" data-url="/app/Paginas/Adm/Financeiro/ContasPagar.aspx?dashboard=VenceHoje" data-dashboard="VenceHoje">
                                            <td>Vence Hoje</td>
                                            <td>
                                                <asp:Label ID="lblQtdPagamentosVencendoHoje_Grafico" runat="server"></asp:Label></td>
                                            <td>
                                                <asp:Label ID="lblValorPagamentosVencendoHoje_Grafico" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr style="background-color: #FEB172;" data-url="/app/Paginas/Adm/Financeiro/ContasPagar.aspx?dashboard=VenceEm7" data-dashboard="VenceEm7">
                                            <td>Até 7 dias</td>
                                            <td>
                                                <asp:Label ID="lblQtdPagamentosVencendoSeteDias_Grafico" runat="server"></asp:Label></td>
                                            <td>
                                                <asp:Label ID="lblValorPagamentosVencendoSeteDias_Grafico" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr style="background-color: #BADCEE;" data-url="/app/Paginas/Adm/Financeiro/ContasPagar.aspx?dashboard=VenceEm30" data-dashboard="VenceEm30">
                                            <td>Até 30 Dias</td>
                                            <td>
                                                <asp:Label ID="lblQtdPagamentosVencerTrintaDias_Grafico" runat="server"></asp:Label></td>
                                            <td>
                                                <asp:Label ID="lblValorPagamentosVencerTrintaDias_Grafico" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr style="background-color: #D0E6F0;" data-url="/app/Paginas/Adm/Financeiro/ContasPagar.aspx?dashboard=VencerMaisdeTrinta" data-dashboard="VencerMaisdeTrinta">
                                            <td>+30 Dias</td>
                                            <td>
                                                <asp:Label ID="lblQtdPagamentosVencerMaisdeTrinta_Grafico" runat="server"></asp:Label></td>
                                            <td>
                                                <asp:Label ID="lblValorPagamentosVencerMaisdeTrinta_Grafico" runat="server"></asp:Label></td>
                                        </tr>
                                        <tr style="background-color: #E0EEF5;" data-url="/app/Paginas/Adm/Financeiro/ContasPagar.aspx?dashboard=TotalAberto" data-dashboard="TotalAberto">

                                            <td>Total Em Aberto</td>
                                            <td>
                                                <asp:Label ID="lblQtdPagamentosTotalEmAberto_Grafico" runat="server"></asp:Label></td>
                                            <td>
                                                <asp:Label ID="lblValorPagamentosTotalEmAberto_Grafico" runat="server"></asp:Label></td>
                                        </tr>
                                    </tbody>
                                </table>

                            </div>--%>
                        </div>
                    </div>
                </div>

            </div>

            <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>
            <div class="dashboard-adm-kpi dashboard-adm-kpi-receber">
                <div class="row">
                    <div class="col-lg-12">
                        <div class="panel panel-primary">
                            <div class="panel-heading">
                                <div class="row">
                                    <div class="col-lg-10">
                                        <h3 class="panel-title">
                                            <i class="fa fa-bar-chart-o"></i>
                                            <asp:Label ID="lblContasReceber" runat="server" Text="Contas a Receber"></asp:Label>
                                        </h3>
                                    </div>
                                    <div class="col-lg-2" runat="server" id="Div_Switch_Rec">
                                        <div class="toggle-switch" style="padding-top: 7px;">
                                            <label class="switch">
                                                <input type="checkbox" id="switchRec" onchange="atualizaGraficoeRec();">
                                                <span class="slider round"></span>
                                            </label>
                                        </div>
                                    </div>
                                </div>

                            </div>
                            <div class="panel-body" runat="server" id="Div_Detalhe_Rec">
                                <div class="row">

                                    <div class="col-lg-2">
                                        <asp:HyperLink ID="lnkRecebimentosEmAtraso" runat="server">
                                            <div class="panel panel-danger caixaBotao">
                                                <div class="panel-heading text-center" style="padding-bottom: 0;">
                                                    <div class="panel-heading">
                                                        <div class="row">
                                                            <div class="col-xs-12 text-center">
                                                                <p class="announcement-heading">
                                                                    <asp:Label ID="lblQtdRecebimentosEmAtraso" runat="server" Text=""></asp:Label>
                                                                </p>
                                                                <p class="announcement-text2">
                                                                    <asp:Label ID="lblValorRecebimentosEmAtraso" runat="server" Text=""></asp:Label>
                                                                </p>
                                                                <p class="announcement-text">Em Atraso</p>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:HyperLink>
                                    </div>

                                    <div class="col-lg-2">
                                        <asp:HyperLink ID="lnklblRecebimentosVencendoHoje" runat="server">
                                            <div class="panel panel-orange caixaBotao">
                                                <div class="panel-heading text-center" style="padding-bottom: 0;">
                                                    <div class="panel-heading">
                                                        <div class="row">
                                                            <div class="col-xs-12 text-center">
                                                                <p class="announcement-heading">
                                                                    <asp:Label ID="lblQtdRecebimentosVencendoHoje" runat="server" Text=""></asp:Label>
                                                                </p>
                                                                <p class="announcement-text2">
                                                                    <asp:Label ID="lblValorRecebimentosVencendoHoje" runat="server" Text=""></asp:Label>
                                                                </p>
                                                                <p class="announcement-text">Hoje</p>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:HyperLink>
                                    </div>

                                    <div class="col-lg-2">
                                        <asp:HyperLink ID="lnkRecebimentosVencendoSeteDias" runat="server">
                                            <div class="panel panel-warning caixaBotao">
                                                <div class="panel-heading text-center" style="padding-bottom: 0;">
                                                    <div class="panel-heading">
                                                        <div class="row">
                                                            <div class="col-xs-12 text-center">
                                                                <p class="announcement-heading">
                                                                    <asp:Label ID="lblQtdRecebimentosVencendoSeteDias" runat="server" Text=""></asp:Label>
                                                                </p>
                                                                <p class="announcement-text2">
                                                                    <asp:Label ID="lblValorRecebimentosVencendoSeteDias" runat="server" Text=""></asp:Label>
                                                                </p>
                                                                <p class="announcement-text">Até 7 dias</p>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:HyperLink>
                                    </div>

                                    <div class="col-lg-2">
                                        <asp:HyperLink ID="lnkRecebimentosVencerTrintaDias" runat="server">
                                            <div class="panel panel-info caixaBotao">
                                                <div class="panel-heading text-center" style="padding-bottom: 0;">
                                                    <div class="panel-heading">
                                                        <div class="row">
                                                            <div class="col-xs-12 text-center">
                                                                <p class="announcement-heading">
                                                                    <asp:Label ID="lblQtdRecebimentosVencerTrintaDias" runat="server" Text=""></asp:Label>
                                                                </p>
                                                                <p class="announcement-text2">
                                                                    <asp:Label ID="lblValorRecebimentosVencerTrintaDias" runat="server" Text=""></asp:Label>
                                                                </p>
                                                                <p class="announcement-text">Até 30 Dias</p>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:HyperLink>
                                    </div>

                                    <div class="col-lg-2">
                                        <asp:HyperLink ID="lnkRecebimentosVencerMaisdeTrinta" runat="server">
                                            <div class="panel panel-info caixaBotao">
                                                <div class="panel-heading text-center" style="padding-bottom: 0;">
                                                    <div class="panel-heading">
                                                        <div class="row">
                                                            <div class="col-xs-12 text-center">
                                                                <p class="announcement-heading">
                                                                    <asp:Label ID="lblQtdRecebimentosVencerMaisdeTrinta" runat="server" Text=""></asp:Label>
                                                                </p>
                                                                <p class="announcement-text2">
                                                                    <asp:Label ID="lblValorRecebimentosVencerMaisdeTrinta" runat="server" Text=""></asp:Label>
                                                                </p>
                                                                <p class="announcement-text">+30 Dias</p>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:HyperLink>
                                    </div>

                                    <div class="col-lg-2">
                                        <asp:HyperLink ID="lnkRecebimentosTotalEmAberto" runat="server">
                                            <div class="panel panel-info caixaBotao">
                                                <div class="panel-heading text-center" style="padding-bottom: 0;">
                                                    <div class="panel-heading">
                                                        <div class="row">
                                                            <div class="col-xs-12 text-center">
                                                                <p class="announcement-heading">
                                                                    <asp:Label ID="lblQtdRecebimentosTotalEmAberto" runat="server" Text=""></asp:Label>
                                                                </p>
                                                                <p class="announcement-text2">
                                                                    <asp:Label ID="lblValorRecebimentosTotalEmAberto" runat="server" Text=""></asp:Label>
                                                                </p>
                                                                <p class="announcement-text">Total</p>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </asp:HyperLink>
                                    </div>

                                </div>
                                <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>
                            </div>

                            <div class="panel-body" runat="server" id="Div_Grafico_Rec">

                                <div style="width: 100%; height: 300px; margin-bottom: 20px;">
                                    <div id="GraficoRecebimentos" style="width: 100%; height: 100%;"></div>
                                </div>

                                <div style="width: 100%; display: flex; justify-content: center;">

                                    <asp:Literal ID="litTabRecebimento" runat="server"></asp:Literal>

                                    <%--<div class="table-responsive" style="width: 100%;">
                                        <table class="table table-hover" id="tbRecebimentos">
                                            <thead>
                                                <tr>
                                                    <th>Categoria</th>
                                                    <th>Quantidade</th>
                                                    <th>Valor</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <tr style="background-color: #FB5B68;" data-url="/app/Paginas/Adm/Financeiro/ContasReceber.aspx?dashboard=EmAtraso" data-dashboard="EmAtraso">
                                                    <td>Em Atraso</td>
                                                    <td>
                                                        <asp:Label ID="lblQtdRecebimentosEmAtraso_Grafico" runat="server"></asp:Label></td>
                                                    <td>
                                                        <asp:Label ID="lblValorRecebimentosEmAtraso_Grafico" runat="server"></asp:Label></td>
                                                </tr>
                                                <tr style="background-color: #F88A30;" data-url="/app/Paginas/Adm/Financeiro/ContasReceber.aspx?dashboard=VenceHoje" data-dashboard="VenceHoje">
                                                    <td>Vence Hoje</td>
                                                    <td>
                                                        <asp:Label ID="lblQtdRecebimentosVencendoHoje_Grafico" runat="server"></asp:Label></td>
                                                    <td>
                                                        <asp:Label ID="lblValorRecebimentosVencendoHoje_Grafico" runat="server"></asp:Label></td>
                                                </tr>
                                                <tr style="background-color: #FEB172;" data-url="/app/Paginas/Adm/Financeiro/ContasReceber.aspx?dashboard=VenceEm7" data-dashboard="VenceEm7">
                                                    <td>Até 7 dias</td>
                                                    <td>
                                                        <asp:Label ID="lblQtdRecebimentosVencendoSeteDias_Grafico" runat="server"></asp:Label></td>
                                                    <td>
                                                        <asp:Label ID="lblValorRecebimentosVencendoSeteDias_Grafico" runat="server"></asp:Label></td>
                                                </tr>
                                                <tr style="background-color: #BADCEE;" data-url="/app/Paginas/Adm/Financeiro/ContasReceber.aspx?dashboard=VenceEm30" data-dashboard="VenceEm30">
                                                    <td>Até 30 Dias</td>
                                                    <td>
                                                        <asp:Label ID="lblQtdRecebimentosVencerTrintaDias_Grafico" runat="server"></asp:Label></td>
                                                    <td>
                                                        <asp:Label ID="lblValorRecebimentosVencerTrintaDias_Grafico" runat="server"></asp:Label></td>
                                                </tr>
                                                <tr style="background-color: #D0E6F0;" data-url="/app/Paginas/Adm/Financeiro/ContasReceber.aspx?dashboard=VencerMaisdeTrinta" data-dashboard="VencerMaisdeTrinta">
                                                    <td>+30 Dias</td>
                                                    <td>
                                                        <asp:Label ID="lblQtdRecebimentosVencerMaisdeTrinta_Grafico" runat="server"></asp:Label></td>
                                                    <td>
                                                        <asp:Label ID="lblValorRecebimentosVencerMaisdeTrinta_Grafico" runat="server"></asp:Label></td>
                                                </tr>
                                                <tr style="background-color: #E0EEF5;" data-url="/app/Paginas/Adm/Financeiro/ContasReceber.aspx?dashboard=TotalAberto" data-dashboard="TotalAberto">
                                                    <td>Total Em Aberto</td>
                                                    <td>
                                                        <asp:Label ID="lblQtdRecebimentosTotalEmAberto_Grafico" runat="server"></asp:Label></td>
                                                    <td>
                                                        <asp:Label ID="lblValorRecebimentosTotalEmAberto_Grafico" runat="server"></asp:Label></td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>--%>
                                </div>
                            </div>
                        </div>

                    </div>
                    <%----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------%>
                </div>
            </div>



            <%----------------------------------------------------------------------------------------------------------------------------------------------%>

            <div class="dashboard-adm-kpi dashboard-adm-kpi-ano">
                <div class="panel panel-primary">
                    <div class="panel-heading">

                        <div class="row">
                            <div class="col-lg-10">

                                <h3 class="panel-title">
                                    <i class="fa fa-bar-chart-o"></i>
                                    <asp:Label ID="lblAnoFinanceiro" runat="server" Text="Ano Financeiro"></asp:Label>
                                </h3>
                            </div>

                            <div class="col-lg-2" runat="server" id="Div_Switch_Ano" style="padding-top: 7px;">
                                <label class="switch">
                                    <input type="checkbox" id="switchAno" onchange="atualizaGraficoeAno();">
                                    <span class="slider round"></span>
                                </label>
                            </div>
                        </div>
                    </div>

                    <%----------------------------------------------------------------------------------------------------------------------------------------------%>

                    <div class="panel-body" runat="server" id="DIV_Ano_Total">
                        <div class="row">


                            <div class="col-lg-2">
                                <asp:HyperLink ID="lnkPagamentosAnual" runat="server">
                                    <div class="panel panel-danger caixaBotao">
                                        <div class="panel-heading text-center" style="padding-bottom: 0;">
                                            <div class="panel-heading">
                                                <div class="row">
                                                    <div class="col-xs-12 text-center">
                                                        <p class="announcement-heading">
                                                            <asp:Label ID="lblQtdPagamentosAnual" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text2">
                                                            <asp:Label ID="lblValorPagamentosAnual" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text">Pagamentos</p>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:HyperLink>
                            </div>

                            <div class="col-lg-2">
                                <asp:HyperLink ID="lnkRecebimentosAnual" runat="server">
                                    <div class="panel panel-success caixaBotao">
                                        <div class="panel-heading text-center" style="padding-bottom: 0;">
                                            <div class="panel-heading">
                                                <div class="row">
                                                    <div class="col-xs-12 text-center">
                                                        <p class="announcement-heading">
                                                            <asp:Label ID="lblQtdRecebimentosAnual" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text2">
                                                            <asp:Label ID="lblValorRecebimentosAnual" runat="server" Text=""></asp:Label>
                                                        </p>
                                                        <p class="announcement-text">Recebimentos</p>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </asp:HyperLink>
                            </div>
                        </div>
                    </div>

                    <%----------------------------------------------------------------------------------------------------------------------------------------------%>

                    <div class="panel-body" runat="server" id="Div_Grafico_Ano">

                        <div style="width: 100%; height: 300px; margin-bottom: 20px;">
                            <div id="GraficoAnual" style="width: 100%; height: 100%;"></div>
                        </div>

                        <div style="width: 100%; display: flex; justify-content: center;">
                            <div class="table-responsive" style="width: 100%;">
                                <table class="table table-hover" id="tbAnual">
                                    <thead>
                                        <tr>
                                            <th>Categoria</th>
                                            <th>Quantidade</th>
                                            <th>Valor</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr style="background-color: #58cc5a; color: black;" data-url="/app/Paginas/Adm/Financeiro/ContasReceber.aspx?dashboard=RecebimentosNoAno" data-dashboard="RecebimentosNoAno" onclick="redirectToDashboard(this)">
                                            <td>Recebimentos Anuais</td>
                                            <td>
                                                <asp:Label ID="lblQtdRecebimentosAnual_Grafico" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblValorRecebimentosAnual_Grafico" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                        <tr style="background-color: #FB5B68; color: black;" data-url="/app/Paginas/Adm/Financeiro/ContasPagar.aspx?dashboard=" data-dashboard="PagosNoAno"  onclick="redirectToDashboard(this)">
                                            <td>Pagamentos Anuais</td>
                                            <td>
                                                <asp:Label ID="lblQtdPagamentosAnual_Grafico" runat="server"></asp:Label>
                                            </td>
                                            <td>
                                                <asp:Label ID="lblValorPagamentosAnual_Grafico" runat="server"></asp:Label>
                                            </td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

        </div>

        <%----------------------------------------------------------------------------------------------------------------------------------------------%>

        <div class="row">
            <div class="col-lg-12">
                <div class="row" runat="server" id="DIV_GRID_GRAFICOS">

                    <%----------------------------------------------------------------------------------------------------------------------------------------------%>

                    <div class="col-lg-12">
                        <div class="panel panel-primary">
                            <div class="panel-heading">
                                <div class="row">
                                    <div class="col-lg-4">
                                        <h3 class="panel-title">
                                            <i class="fa fa-bar-chart-o"></i>
                                            <asp:Label ID="lblGrafico_Caixa_Semanal_v2" runat="server" Text="Caixa: Recebimentos / Pagamentos Semana"></asp:Label>
                                        </h3>
                                    </div>

                                </div>
                            </div>

                            <div class="panel-body">
                                <div id="Div_Grafico_Caixa_Semanal" runat="server">
                                    <canvas id="Grafico_Caixa_Semanal_v2" style="width: 100%; height: 400px;"></canvas>
                                </div>
                                <div id="Div_Grafico_Caixa_Trimestre" runat="server">
                                    <canvas id="Grafico_Caixa_Trimestral_v2" style="width: 100%; height: 400px;"></canvas>
                                </div>
                                <div id="Div_Grafico_Caixa_Ano" runat="server">
                                    <canvas id="Grafico_Caixa_Ano_v2" style="width: 100%; height: 400px;"></canvas>
                                </div>

                            </div>
                        </div>
                    </div>
                    <%----------------------------------------------------------------------------------------------------------------------------------------------%>

                    <div class="col-lg-12">
                        <div class="panel panel-primary">
                            <div class="panel-heading">
                                <div class="row">
                                    <div class="col-lg-4">
                                        <h3 class="panel-title">
                                            <i class="fa fa-bar-chart-o"></i>
                                            <asp:Label ID="lblGrafico_Contabil_Ano_v2" runat="server" Text="Contábil: Receitas / Despesas Ano"></asp:Label>
                                        </h3>
                                    </div>

                                </div>
                            </div>

                            <div class="panel-body">

                                <div id="Div_Grafico_Contabil_Semana_V2" runat="server">
                                    <canvas id="Grafico_Contabil_Semana_V2" style="width: 100%; height: 400px;"></canvas>
                                </div>
                                <div id="Div_Grafico_Contabil_Trimestre_V2" runat="server">
                                    <canvas id="Grafico_Contabil_Trimestre_V2" style="width: 100%; height: 400px;"></canvas>
                                </div>
                                <div id="Div_Grafico_Contabil_Ano_v2" runat="server">
                                    <canvas id="Grafico_Contabil_Ano_v2" style="width: 100%; height: 400px;"></canvas>
                                </div>
                            </div>
                        </div>
                    </div>
                    <%----------------------------------------------------------------------------------------------------------------------------------------------%>

                    <div class="col-lg-12">
                        <div class="panel panel-primary">
                            <div class="panel-heading">
                                <div class="row">
                                    <div class="col-lg-4">
                                        <h3 class="panel-title">
                                            <i class="fa fa-bar-chart-o"></i>
                                            <asp:Label ID="lblGrafico_Emprestimo_Semana_v2" runat="server" Text="Empréstimo: Pagar / Receber Semana"></asp:Label>
                                        </h3>
                                    </div>

                                </div>
                            </div>

                            <div class="panel-body">
                                <div id="Div_Grafico_Emprestimo_Semana_v2" runat="server">
                                    <canvas id="Grafico_Emprestimo_Semana_v2" style="width: 100%; height: 400px;"></canvas>
                                </div>
                                <div id="Div_Grafico_Emprestimo_Trimestre_v2" runat="server">
                                    <canvas id="Grafico_Emprestimo_Trimestre_v2" style="width: 100%; height: 400px;"></canvas>
                                </div>
                                <div id="Div_Grafico_Emprestimo_Anual_v2" runat="server">
                                    <canvas id="Grafico_Emprestimo_Anual_v2" style="width: 100%; height: 400px;"></canvas>
                                </div>
                            </div>
                        </div>
                    </div>



                    <%----------------------------------------------------------------------------------------------------------------------------------------------%>



                    <div class="col-lg-12">
                        <div class="panel panel-primary">
                            <div class="panel-heading">
                                <div class="row">
                                    <div class="col-lg-4">
                                        <h3 class="panel-title">
                                            <i class="fa fa-bar-chart-o"></i>
                                            <asp:Label ID="lblGrafico_Comparativo_Semana_V2" runat="server" Text="Comparativo: Recebimentos e Pagamentos Semana"></asp:Label>
                                        </h3>
                                    </div>

                                </div>
                            </div>

                            <div class="panel-body">
                                <%--     <div id="Div_Grafico_Comparativo_Semana_V2" runat="server">
                                        <canvas id="Grafico_Comparativo_Semana_V2" style="width: 100%; height: 400px;"></canvas>
                                    </div>
                                    <div id="Div_Grafico_Comparativo_Trimestre_V2" runat="server">
                                        <canvas id="Grafico_Comparativo_Trimestre_V2" style="width: 100%; height: 400px;"></canvas>
                                    </div>--%>
                                <div id="Div_Grafico_Comparativo_Ano_V2" runat="server">
                                    <canvas id="Grafico_Comparativo_Ano_V2" style="width: 100%; height: 400px;"></canvas>
                                </div>
                            </div>
                        </div>
                    </div>






                    <%----------------------------------------------------------------------------------------------------------------------------------------------%>
                </div>
                <%----------------------------------------------------------------------------------------------------------------------------------------------%>
                <div class="row" runat="server" id="Div_Adiantamento">

                    <div class="col-lg-12">
                        <div class="panel panel-primary">
                            <div class="panel-heading">
                                <div class="row">
                                    <div class="col-lg-10">
                                        <h3 class="panel-title">
                                            <i class="fa fa-bar-chart-o"></i>
                                            <asp:Label ID="lblAdiantamento" runat="server" Text="Detalhes Adiantamento"></asp:Label>
                                        </h3>
                                    </div>
                                    <div class="col-lg-2">
                                        <asp:DropDownList ID="ddlidAdiantamentoSelecionado" runat="server" class="form-control yes_no select" AutoPostBack="true" OnSelectedIndexChanged="ddlidAdiantamentoSelecionado_SelectedIndexChanged">
                                            <asp:ListItem Selected="True" Value="1">Pendentes</asp:ListItem>
                                            <asp:ListItem Value="2">Liquidados</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="panel-body">
                                <div id="Grid_Adiantamento_Pendente" class="table-responsive">
                                    <asp:GridView ID="gvAdiantamento" runat="server" AutoGenerateColumns="false" CellPadding="0" CellSpacing="1"
                                        class="table table-striped table-bordered table-hover table-condensed table-responsive" GridLines="None"
                                        ShowFooter="True" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvAdiantamento_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="idContasPagar" HeaderText="ID">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Banco">
                                                <ItemTemplate>
                                                    <a href='<%# Eval("sLinkPagarAdiantado") %>'><%# Eval("sContaCredora") %></a>
                                                </ItemTemplate>
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="dtEmissao" HeaderText="Emissão"
                                                SortExpression="dtEmissao"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="8%" />

                                            <asp:BoundField DataField="nValorEmprestimo" HeaderText="Valor Operação"
                                                DataFormatString="{0:C2}" SortExpression="nValorEmprestimo"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="10%" />
                                            
                                            <asp:BoundField DataField="nTotalTitulos" HeaderText="Total Títulos"
                                                DataFormatString="{0:C2}" SortExpression="nTotalTitulos"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="10%" />

                                            <asp:BoundField DataField="nTaxaJuros" HeaderText="Total Juros"
                                                DataFormatString="{0:C2}" SortExpression="nTaxaJuros"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="9%" />

                                            <asp:BoundField DataField="nIOFTotal" HeaderText="Total IOF"
                                                DataFormatString="{0:C2}" SortExpression="nIOFTotal"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="9%" />

                                            <asp:BoundField DataField="nTarifas" HeaderText="Tarifas"
                                                DataFormatString="{0:C2}" SortExpression="nTarifas"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="9%" />

                                            <asp:BoundField DataField="nValorLiberado" HeaderText="Total Liberado"
                                                DataFormatString="{0:C2}" SortExpression="nValorLiberado"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="10%" />
                                            
                                            <asp:BoundField DataField="nDespesas" HeaderText="Total Despesas"
                                                DataFormatString="{0:C2}" SortExpression="nValorOriginal"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="10%">     
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nTitulosAberto" HeaderText="Títulos em Aberto"
                                                DataFormatString="{0:C2}" SortExpression="nSaldoPagar"
                                                ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="10%" />
                                            
                                            <asp:TemplateField HeaderText="ID Adiantados">
                                                <ItemTemplate>
                                                   <asp:Literal runat="server" ID="litLinkAdiantados"></asp:Literal>
                                                </ItemTemplate>
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>


                                        </Columns>

                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>



</asp:Content>


