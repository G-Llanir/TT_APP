<%@ Page Title="" Language="C#" MasterPageFile="~/app/main.master" AutoEventWireup="true" EnableEventValidation="false" CodeBehind="Dashboard.aspx.cs" Inherits="TT_Flow.App.Dashboard" %>

<%@ Register Src="~/app/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb_Pagina" %>
<%@ Register Src="~/App/Controles/Calendario.ascx" TagPrefix="uc1" TagName="Calendario" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="Server">

    <style>
        .caixaBotao_Cima_Esquerda:hover {
            box-shadow: -3px -3px 7px green; /* cima + esquerda */
            transition: box-shadow ease-out 0.1s;
        }

        .caixaBotao_Cima_Direita:hover {
            box-shadow: 3px -3px 7px green; /* cima + direita */
            transition: box-shadow ease-out 0.1s;
        }

        .caixaBotao_Baixo_Esquerda:hover {
            box-shadow: -3px 3px 7px green; /* baixo + esquerda */
            transition: box-shadow ease-out 0.1s;
        }

        .caixaBotao_Baixo_Direita:hover {
            box-shadow: 3px 3px 7px green; /* baixo + direita */
            transition: box-shadow ease-out 0.1s;
        }

        .caixaBotao_Cima:hover {
            box-shadow: -3px -3px 7px green, 3px -3px 7px green; /* cima + esquerda + direita */
            transition: box-shadow ease-out 0.1s;
        }

        .caixaBotao_Baixo:hover {
            box-shadow: -3px 3px 7px green, 3px 3px 7px green; /* baixo + esquerda + direita */
            transition: box-shadow ease-out 0.1s;
        }

        #GraficoDepartamento path {
            cursor: pointer;
        }

        .hover-effect {
            stroke: #0a0a0a;
            stroke-width: 2;
            fill-opacity: 0.8;
        }

        .hover-cursor {
            cursor: pointer;
        }

        #GraficoDepartamento, #GraficosStatus, #Grafico_Fluxo {
            width: 100%;
            height: auto;
            min-height: 300px;
        }

        .CaixaDividida {
            padding: 0;
            box-sizing: border-box;
        }

            .CaixaDividida .qtd {
                font-size: 2em;
            }

        .btn-verde {
            background-color: #d6e9c6;
            border-color: #d6e9c6;
            color: #3c763d !important;
            padding: 5px 10px !important;
        }

        .btn-vermelho {
            background-color: #ebccd1;
            border-color: #ebccd1;
            color: #a94442 !important;
            padding: 5px 10px !important;
        }

        .btn-amarelo {
            background-color: #faebcc;
            border-color: #faebcc;
            color: #8a6d3b !important;
            padding: 5px 10px !important;
        }

        .btn-azul {
            background-color: #bce8f1;
            border-color: #bce8f1;
            color: #31708f !important;
            padding: 5px 10px !important;
        }

        .botao {
            display: flex;
            flex-wrap: wrap;
            padding: 20px 2.5px;
        }

            .botao .fa:before {
                vertical-align: middle;
            }

        .panel-flex {
            display: flex;
            flex-wrap: wrap;
        }

            .panel-flex .horizontalMetade {
                width: 50%;
            }

                .panel-flex .horizontalMetade .botao {
                    padding: 7.5px 2.5px 0 2.5px !important;
                    margin-top: 0;
                }

            .panel-flex .horizontalCompleto {
                width: 100%;
            }

                .panel-flex .horizontalCompleto .botao {
                    padding: 2.5px 2.5px 0 2.5px !important;
                    margin-top: 0;
                }

        .divisoria {
            height: 1px;
            background-color: #bbb;
        }

        .fixo {
            position: absolute;
            cursor: pointer;
        }

        .font-3 {
            font-size: 3em;
        }

        .announcement-text {
            font-size: 1.15em;
        }

        .usuarios {
            display: flex;
            flex-wrap: wrap;
        }

            .usuarios > .label {
                margin: 0 2.5px 2.5px 0;
                padding: .35em;
                font-size: 100%;
                overflow: hidden;
                text-overflow: ellipsis;
                max-width: 100%;
            }

        .bordaDivisoria {
            border-left: 1px solid #bbb;
        }

        .escondeCaixa {
            margin: 0;
        }

            .escondeCaixa > .panel-heading {
                display: flex;
                justify-content: space-between;
                cursor: pointer;
            }

            .escondeCaixa > .panel-body {
                padding: 15px 0;
            }

            .escondeCaixa > .panel-body > .col-lg-5 {
                padding: 0 5px 0 10px;
            }

            .escondeCaixa > .panel-body > .col-lg-7 {
                padding: 0 10px 0 5px;
            }

            .tira-form-group {
                align-content: center;
            }

            .tira-form-group .form-group {
                margin: 0;
            }

            .tira-form-group .form-group label {
                margin: 0;
            }

        #div_Calendario_View {
            display: flex;
            flex-direction: column;
            min-height: 0;
            height: 52.75rem;
            max-height: 52.75rem;
        }

            #div_Calendario_View > [id*="upAvisosCalendario"] {
                flex: 1 1 auto;
                min-height: 0;
                display: flex;
                flex-direction: column;
            }

        .dashboard-calendario-lateral {
            flex: 1 1 auto;
            min-height: 0;
            height: 100%;
        }

        .dashboard-calendario-lateral [id*="div_calendario"] {
            min-height: 0;
            height: 100% !important;
            max-height: 100% !important;
        }
    </style>

    <div class="row">
        <div class="col-lg-9">
            <h1>Dashboard <small>T-Flow - Painel Administrativo</small> </h1>
        </div>
        <div class="col-lg-3 form-group" style="display: flex; justify-content: start; margin-top: 20px;">
            <asp:ListBox ID="lstidFluxo" runat="server" SelectionMode="Multiple"></asp:ListBox>
            <asp:LinkButton ID="cmdAtualizar" CssClass="btn btn-small btn-circle" runat="server" OnClick="cmdAtualizar_Click"><i class="fa-refresh fa"></i></asp:LinkButton>
        </div>
        <div class="col-lg-12">
            <uc1:BreadCrumb_Pagina runat="server" ID="BreadCrumb_Pagina" NivelPagina="1" TitulodaPagina="DashBoard" />
        </div>
    </div>

    <div id="abas">
        <ul id="tab_Dashboard" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#dashboard" id="aba_dashboard" role="tabpanel" data-toggle="tab" aria-controls="dashboard" aria-expanded="false"><b>Dashboard</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_calendario">
                <a href="#calendario" role="tab" id="aba_calendario-tab" data-toggle="tab" aria-controls="calendario"><b>Calendário</b></a>
            </li>
        </ul>
    </div>

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="dashboard" aria-labelledby="dashboard-tab">
            <div style="margin-top: 20px;">

                <div class="col-lg-12" style="padding: 0; display: flex; width: 100%; height: 100%;">
                    <div class="col-lg-9" style="display: flex; flex-direction: column; padding: 0;">

                        <asp:UpdatePanel ID="upDashboard_Pedidos" runat="server" UpdateMode="Conditional" ChildrenAsTriggers="false">
                            <ContentTemplate>

                                <div class="col-lg-12">
                                    <div class="row">
                                        <div class="panel panel-primary caixaBotao">
                                            <div class="panel-heading">
                                                <div class="row">
                                                    <div class="col-lg-3">
                                                        <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Resumo de Pedidos</h3>
                                                    </div>
                                                    <div class="col-lg-5" style="display: flex; justify-content: right; gap: 10px;">
                                                        <div runat="server" id="div_DashboardSeparado" class="tira-form-group">
                                                            <uc1:SwitchAtivo runat="server" ID="Switch_DashboardSeparado" />
                                                        </div>
                                                        <div runat="server" id="div_Empresas" class="btn-group Lista_Radio">
                                                            <asp:RadioButton runat="server" ID="rbTTL" Text="TTL" CssClass="btn btn-success" GroupName="Empresas"></asp:RadioButton>
                                                            <asp:RadioButton runat="server" ID="rbTodos" Text="Todos" Checked="true" CssClass="btn btn-success" GroupName="Empresas"></asp:RadioButton>
                                                            <asp:RadioButton runat="server" ID="rbTTS" Text="TTS" CssClass="btn btn-success" GroupName="Empresas"></asp:RadioButton>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-4">
                                                        <asp:DropDownList ID="ddlsClienteEspecial" runat="server" class="form-control Caixa_Selecao">
                                                            <asp:ListItem Selected="True" Value="T">Todos os Clientes</asp:ListItem>
                                                            <asp:ListItem Value="N">Excluir Clientes Visualização Diferenciada</asp:ListItem>
                                                            <asp:ListItem Value="S">Apenas Clientes Visualização Diferenciada</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="panel-body">

                                                <div class="row" style="margin-bottom: 15px;">

                                                    <div id="divGeral_Total" class="col-lg-3">
                                                        <asp:HyperLink ID="lnkTotalPedidos" runat="server" CssClass="btn btn-verde btn-block text-left caixaBotao">
                                                            <div class="botao">
                                                                <div class="col-lg-7 text-left" style="padding: 0;">
                                                                    <i class="fa fa-list fa-5x"></i>
                                                                </div>
                                                                <div class="col-lg-5 text-right font-3" style="padding: 0;">
                                                                    <asp:Label ID="lblTotalPedidos" runat="server" Text=""></asp:Label>
                                                                </div>
                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                    <p class="announcement-text">Total de Pedidos</p>
                                                                </div>
                                                            </div>
                                                        </asp:HyperLink>
                                                        <div class="panel" id="div_TotalPedidos_Todos" runat="server" style="box-shadow: none; -webkit-box-shadow: none; margin: 0;">
                                                            <div class="panel-heading" style="position: relative; padding: 0;">
                                                                <div class="panel-flex">

                                                                    <div class="CaixaDividida horizontalCompleto caixaBotao_Cima">
                                                                        <asp:HyperLink ID="lnkTotalPedidos_TTL" runat="server" CssClass="btn btn-verde btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                            <div class="botao">
                                                                                <div class="col-lg-7 text-left" style="padding: 0;">
                                                                                    <i class="fa fa-list fa-3x"></i>
                                                                                </div>
                                                                                <div class="col-lg-5 text-right" style="padding: 0;">
                                                                                    <asp:Label runat="server" ID="lblTotalPedidos_TTL" class="qtd" Text="0"></asp:Label>
                                                                                </div>
                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                    <span>Total de Pedidos - </span><b>TTL</b>
                                                                                </div>
                                                                            </div>
                                                                        </asp:HyperLink>
                                                                    </div>

                                                                    <div class="CaixaDividida horizontalCompleto caixaBotao_Baixo">
                                                                        <asp:HyperLink ID="lnkTotalPedidos_TTS" runat="server" CssClass="btn btn-verde btn-block text-left" Style="border-radius: 0 0 4px 4px;">
                                                                            <div class="botao">
                                                                                <div class="col-lg-7 text-left" style="padding: 0;">
                                                                                    <i class="fa fa-list fa-3x"></i>
                                                                                </div>
                                                                                <div class="col-lg-5 text-right" style="padding: 0;">
                                                                                    <asp:Label runat="server" ID="lblTotalPedidos_TTS" class="qtd" Text="0"></asp:Label>
                                                                                </div>
                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                    <span>Total de Pedidos - </span><b>TTS</b>
                                                                                </div>
                                                                            </div>
                                                                        </asp:HyperLink>
                                                                    </div>

                                                                    <div class="divisoria fixo" style="left: 0; right: 0; top: 50%;"></div>

                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div id="divGeral_Depto_7Dias" class="col-lg-3">
                                                        <div class="panel" style="box-shadow: none; -webkit-box-shadow: none; margin: 0;">
                                                            <div class="panel-heading" style="position: relative; padding: 0;">
                                                                <div class="panel-flex">

                                                                    <div runat="server" id="div_Depto_7Dias" class="panel-flex">
                                                                        <div class="CaixaDividida horizontalCompleto caixaBotao_Cima">
                                                                            <asp:HyperLink ID="lnkTarefas_TotalDepto" runat="server" CssClass="btn btn-azul btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                                <div class="botao">
                                                                                    <div class="col-lg-7 text-left" style="padding: 0;">
                                                                                        <i class="fa fa-tasks fa-3x"></i>
                                                                                    </div>
                                                                                    <div class="col-lg-5 text-right" style="padding: 0;">
                                                                                        <asp:Label runat="server" ID="lblTarefas_TotalDepto" class="qtd" Text="0"></asp:Label>
                                                                                    </div>
                                                                                    <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                        <span>Pedidos em seu Depto.</span>
                                                                                    </div>
                                                                                </div>
                                                                            </asp:HyperLink>
                                                                        </div>

                                                                        <div class="CaixaDividida horizontalCompleto caixaBotao_Baixo">
                                                                            <asp:HyperLink ID="lnkTotalVencerSeteDias" runat="server" CssClass="btn btn-amarelo btn-block text-left" Style="border-radius: 0 0 4px 4px;">
                                                                                <div class="botao">
                                                                                    <div class="col-lg-7 text-left" style="padding: 0;">
                                                                                        <i class="fa fa-exclamation fa-3x"></i>
                                                                                    </div>
                                                                                    <div class="col-lg-5 text-right" style="padding: 0;">
                                                                                        <asp:Label runat="server" ID="lblTotalVencerSeteDias" class="qtd" Text="0"></asp:Label>
                                                                                    </div>
                                                                                    <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                        <span>Vence em 7 Dias</span>
                                                                                    </div>
                                                                                </div>
                                                                            </asp:HyperLink>
                                                                        </div>
                                                                    </div>

                                                                    <div runat="server" id="div_Depto_7Dias_Todos" class="panel-flex">
                                                                        <div class="CaixaDividida horizontalMetade caixaBotao_Cima_Esquerda">
                                                                            <asp:HyperLink ID="lnkTarefas_TotalDepto_TTL" runat="server" CssClass="btn btn-azul btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                                <div class="botao">
                                                                                    <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                        <i class="fa fa-tasks fa-2x"></i>
                                                                                        <asp:Label runat="server" ID="lblTarefas_TotalDepto_TTL" class="qtd" Text="0"></asp:Label>
                                                                                    </div>
                                                                                    <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                        <span>Seu Depto.</span>
                                                                                    </div>
                                                                                </div>
                                                                                <label class="fixo" style="top: 0; right: 50.5%;">TT</label>
                                                                            </asp:HyperLink>
                                                                        </div>

                                                                        <div class="CaixaDividida horizontalMetade caixaBotao_Cima_Direita">
                                                                            <asp:HyperLink ID="lnkTotalVencerSeteDias_TTL" runat="server" CssClass="btn btn-amarelo btn-block text-left" Style="border-radius: 0 0 4px 4px;">
                                                                                <div class="botao">
                                                                                    <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                        <i class="fa fa-exclamation fa-2x"></i>
                                                                                        <asp:Label runat="server" ID="lblTotalVencerSeteDias_TTL" class="qtd" Text="0"></asp:Label>
                                                                                    </div>
                                                                                    <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                        <span>Venc. 7 Dias</span>
                                                                                    </div>
                                                                                </div>
                                                                                <label class="fixo" style="top: 0; left: 50%;">L</label>
                                                                            </asp:HyperLink>
                                                                        </div>

                                                                        <div class="CaixaDividida horizontalMetade caixaBotao_Baixo_Esquerda">
                                                                            <asp:HyperLink ID="lnkTarefas_TotalDepto_TTS" runat="server" CssClass="btn btn-azul btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                                <div class="botao">
                                                                                    <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                        <i class="fa fa-tasks fa-2x"></i>
                                                                                        <asp:Label runat="server" ID="lblTarefas_TotalDepto_TTS" class="qtd" Text="0"></asp:Label>
                                                                                    </div>
                                                                                    <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                        <span>Seu Depto.</span>
                                                                                    </div>
                                                                                </div>
                                                                                <label class="fixo" style="top: 50%; right: 50.5%;">TT</label>
                                                                            </asp:HyperLink>
                                                                        </div>

                                                                        <div class="CaixaDividida horizontalMetade caixaBotao_Baixo_Direita">
                                                                            <asp:HyperLink ID="lnkTotalVencerSeteDias_TTS" runat="server" CssClass="btn btn-amarelo btn-block text-left" Style="border-radius: 0 0 4px 4px;">
                                                                                <div class="botao">
                                                                                    <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                        <i class="fa fa-exclamation fa-2x"></i>
                                                                                        <asp:Label runat="server" ID="lblTotalVencerSeteDias_TTS" class="qtd" Text="0"></asp:Label>
                                                                                    </div>
                                                                                    <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                        <span>Venc. 7 Dias</span>
                                                                                    </div>
                                                                                </div>
                                                                                <label class="fixo" style="top: 50%; left: 49.5%;">S</label>
                                                                            </asp:HyperLink>
                                                                        </div>
                                                                    </div>

                                                                    <div class="divisoria fixo" style="left: 0; right: 0; top: 50%;"></div>

                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div id="divGeral_EmAtraso" class="col-lg-3">
                                                        <asp:HyperLink ID="lnkTotalemAtraso" runat="server" CssClass="btn btn-vermelho btn-block text-left caixaBotao">
                                                            <div class="botao">
                                                                <div class="col-lg-7 text-left" style="padding: 0;">
                                                                    <i class="fa fa-times fa-5x"></i>
                                                                </div>
                                                                <div class="col-lg-5 text-right font-3" style="padding: 0;">
                                                                    <asp:Label runat="server" ID="lblTotalemAtraso" class="qtd" Text="0"></asp:Label>
                                                                </div>
                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                    <p class="announcement-text">Pedidos em Atraso</p>
                                                                </div>
                                                            </div>
                                                        </asp:HyperLink>
                                                        <div class="panel" id="div_TotalemAtraso_Todos" runat="server" style="box-shadow: none; -webkit-box-shadow: none; margin: 0;">
                                                            <div class="panel-heading" style="position: relative; padding: 0;">
                                                                <div class="panel-flex">

                                                                    <div class="CaixaDividida horizontalCompleto caixaBotao_Cima">
                                                                        <asp:HyperLink ID="lnkTotalemAtraso_TTL" runat="server" CssClass="btn btn-vermelho btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                            <div class="botao">
                                                                                <div class="col-lg-7 text-left" style="padding: 0;">
                                                                                    <i class="fa fa-times fa-3x"></i>
                                                                                </div>
                                                                                <div class="col-lg-5 text-right" style="padding: 0;">
                                                                                    <asp:Label runat="server" ID="lblTotalemAtraso_TTL" class="qtd" Text="0"></asp:Label>
                                                                                </div>
                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                    <span>Em Atraso - </span><b>TTL</b>
                                                                                </div>
                                                                            </div>
                                                                        </asp:HyperLink>
                                                                    </div>

                                                                    <div class="CaixaDividida horizontalCompleto caixaBotao_Baixo">
                                                                        <asp:HyperLink ID="lnkTotalemAtraso_TTS" runat="server" CssClass="btn btn-vermelho btn-block text-left" Style="border-radius: 0 0 4px 4px;">
                                                                            <div class="botao">
                                                                                <div class="col-lg-7 text-left" style="padding: 0;">
                                                                                    <i class="fa fa-times fa-3x"></i>
                                                                                </div>
                                                                                <div class="col-lg-5 text-right" style="padding: 0;">
                                                                                    <asp:Label runat="server" ID="lblTotalemAtraso_TTS" class="qtd" Text="0"></asp:Label>
                                                                                </div>
                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                    <span>Em Atraso - </span><b>TTS</b>
                                                                                </div>
                                                                            </div>
                                                                        </asp:HyperLink>
                                                                    </div>

                                                                    <div class="divisoria fixo" style="left: 0; right: 0; top: 50%;"></div>

                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div id="divGeral_Entregas_Coletas" class="col-lg-3">
                                                        <div class="panel" style="box-shadow: none; -webkit-box-shadow: none; margin: 0;">
                                                            <div class="panel-heading" style="position: relative; padding: 0;">
                                                                <div class="panel-flex">

                                                                    <div class="CaixaDividida horizontalMetade caixaBotao_Cima_Esquerda">
                                                                        <asp:HyperLink ID="lnkEntregas_EmDia" runat="server" CssClass="btn btn-verde btn-block text-left" Style="border-radius: 4px 0 0 0;">
                                                                            <div class="botao">
                                                                                <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                    <i class="fa fa-truck fa-2x"></i>
                                                                                    <asp:Label runat="server" ID="lblEntregas_EmDia" class="qtd" Text="0"></asp:Label>
                                                                                </div>
                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                    <span>Em Dia</span>
                                                                                </div>
                                                                            </div>
                                                                            <label class="fixo" style="top: 0; right: 50.5%;">Entr</label>
                                                                        </asp:HyperLink>
                                                                    </div>

                                                                    <div class="CaixaDividida horizontalMetade caixaBotao_Cima_Direita">
                                                                        <asp:HyperLink ID="lnkEntregas_EmAtraso" runat="server" CssClass="btn btn-vermelho btn-block text-left" Style="border-radius: 0 4px 0 0;">
                                                                            <div class="botao">
                                                                                <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                    <i class="fa fa-truck fa-2x"></i>
                                                                                    <asp:Label runat="server" ID="lblEntregas_EmAtraso" class="qtd" Text="0"></asp:Label>
                                                                                </div>
                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                    <span>Atraso</span>
                                                                                </div>
                                                                            </div>
                                                                            <label class="fixo" style="top: 0; left: 49.5%;">egas</label>
                                                                        </asp:HyperLink>
                                                                    </div>

                                                                    <div class="CaixaDividida horizontalMetade caixaBotao_Baixo_Esquerda">
                                                                        <asp:HyperLink ID="lnkColetas_EmDia" runat="server" CssClass="btn btn-verde btn-block text-left" Style="border-radius: 0 0 0 4px;">
                                                                            <div class="botao">
                                                                                <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                    <i class="fa fa-inbox fa-2x"></i>
                                                                                    <asp:Label runat="server" ID="lblColetas_EmDia" class="qtd" Text="0"></asp:Label>
                                                                                </div>
                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                    <span>Em Dia</span>
                                                                                </div>
                                                                            </div>
                                                                            <label class="fixo" style="top: 50.5%; right: 50.5%;">Col</label>
                                                                        </asp:HyperLink>
                                                                    </div>

                                                                    <div class="CaixaDividida horizontalMetade caixaBotao_Baixo_Direita">
                                                                        <asp:HyperLink ID="lnkColetas_EmAtraso" runat="server" CssClass="btn btn-vermelho btn-block text-left" Style="border-radius: 0 0 4px 0;">
                                                                            <div class="botao">
                                                                                <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                    <i class="fa fa-inbox fa-2x"></i>
                                                                                    <asp:Label runat="server" ID="lblColetas_EmAtraso" class="qtd" Text="0"></asp:Label>
                                                                                </div>
                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                    <span>Atraso</span>
                                                                                </div>
                                                                            </div>
                                                                            <label class="fixo" style="top: 50.5%; left: 49.5%;">etas</label>
                                                                        </asp:HyperLink>
                                                                    </div>

                                                                    <div class="divisoria fixo" style="left: 0; right: 0; top: 50%;"></div>

                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                </div>

                                                <div runat="server" id="div_FluxosSeparados" class="row">

                                                    <div class="col-lg-6">
                                                        <div class="panel panel-primary caixaBotao escondeCaixa">
                                                            <div class="panel-heading">
                                                                <h3 class="panel-title">B2B</h3><i class="fa fa-chevron-up"></i>
                                                            </div>
                                                            <div class="panel-body">
                                                                <div id="divGeral_Total_B2B" class="col-lg-5">
                                                                    <div class="panel" style="box-shadow: none; -webkit-box-shadow: none; margin: 0;">
                                                                        <div class="panel-heading" style="position: relative; padding: 0;">
                                                                            <div class="panel-flex">

                                                                                <div class="CaixaDividida horizontalCompleto caixaBotao_Cima">
                                                                                    <asp:HyperLink ID="lnkTotalPedidos_B2B" runat="server" CssClass="btn btn-verde btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                                        <div class="botao">
                                                                                            <div class="col-lg-7 text-left" style="padding: 0;">
                                                                                                <i class="fa fa-list fa-3x"></i>
                                                                                            </div>
                                                                                            <div class="col-lg-5 text-right" style="padding: 0;">
                                                                                                <asp:Label runat="server" ID="lblTotalPedidos_B2B" class="qtd" Text="0"></asp:Label>
                                                                                            </div>
                                                                                            <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                                <span>Total de Pedidos</span>
                                                                                            </div>
                                                                                        </div>
                                                                                    </asp:HyperLink>
                                                                                </div>

                                                                                <div class="CaixaDividida horizontalCompleto caixaBotao_Baixo">
                                                                                    <asp:HyperLink ID="lnkEntrega_B2B" runat="server" CssClass="btn btn-verde btn-block text-left" Style="border-radius: 0 0 4px 4px;">
                                                                                        <div class="botao">
                                                                                            <div class="col-lg-7 text-left" style="padding: 0;">
                                                                                                <i class="fa fa-truck fa-3x"></i>
                                                                                            </div>
                                                                                            <div class="col-lg-5 text-right" style="padding: 0;">
                                                                                                <asp:Label runat="server" ID="lblEntrega_B2B" class="qtd" Text="0"></asp:Label>
                                                                                            </div>
                                                                                            <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                                <span>Entregas</span>
                                                                                            </div>
                                                                                        </div>
                                                                                    </asp:HyperLink>
                                                                                </div>

                                                                                <div class="divisoria fixo" style="left: 0; right: 0; top: 50%;"></div>

                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div id="divGeral_Depto_7Dias_Atraso_B2B" class="col-lg-7">
                                                                    <div class="panel" style="box-shadow: none; -webkit-box-shadow: none; margin: 0;">
                                                                        <div class="panel-heading" style="position: relative; padding: 0;">
                                                                            <div class="panel-flex">

                                                                                <div class="panel-flex">
                                                                                    <div class="CaixaDividida horizontalMetade caixaBotao_Cima_Esquerda">
                                                                                        <asp:HyperLink ID="lnkDepto_B2B" runat="server" CssClass="btn btn-azul btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                                            <div class="botao">
                                                                                                <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                                    <i class="fa fa-tasks fa-2x"></i>
                                                                                                    <asp:Label runat="server" ID="lblDepto_B2B" class="qtd" Text="0"></asp:Label>
                                                                                                </div>
                                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                                    <span>Seu Depto.</span>
                                                                                                </div>
                                                                                            </div>
                                                                                        </asp:HyperLink>
                                                                                    </div>

                                                                                    <div class="CaixaDividida horizontalMetade caixaBotao_Cima_Direita">
                                                                                        <asp:HyperLink ID="lnk7Dias_B2B" runat="server" CssClass="btn btn-amarelo btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                                            <div class="botao">
                                                                                                <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                                    <i class="fa fa-exclamation fa-2x"></i>
                                                                                                    <asp:Label runat="server" ID="lbl7Dias_B2B" class="qtd" Text="0"></asp:Label>
                                                                                                </div>
                                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                                    <span>Venc. 7 Dias</span>
                                                                                                </div>
                                                                                            </div>
                                                                                        </asp:HyperLink>
                                                                                    </div>

                                                                                    <div class="CaixaDividida horizontalMetade caixaBotao_Baixo_Esquerda">
                                                                                        <asp:HyperLink ID="lnkAtraso_B2B" runat="server" CssClass="btn btn-vermelho btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                                            <div class="botao">
                                                                                                <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                                    <i class="fa fa-times fa-2x"></i>
                                                                                                    <asp:Label runat="server" ID="lblAtraso_B2B" class="qtd" Text="0"></asp:Label>
                                                                                                </div>
                                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                                    <span>Atraso</span>
                                                                                                </div>
                                                                                            </div>
                                                                                        </asp:HyperLink>
                                                                                    </div>

                                                                                    <div class="CaixaDividida horizontalMetade caixaBotao_Baixo_Direita">
                                                                                        <asp:HyperLink ID="lnkConfirmacao_B2B" runat="server" CssClass="btn btn-vermelho btn-block text-left bordaDivisoria" Style="border-radius: 0 0 4px 4px;">
                                                                                            <div class="botao">
                                                                                                <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                                    <i class="fa fa-question fa-2x"></i>
                                                                                                    <asp:Label runat="server" ID="lblConfirmacao_B2B" class="qtd" Text="0"></asp:Label>
                                                                                                </div>
                                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                                    <span>Confirmação</span>
                                                                                                </div>
                                                                                            </div>
                                                                                        </asp:HyperLink>
                                                                                    </div>
                                                                                </div>

                                                                                <div class="divisoria fixo" style="left: 0; right: 0; top: 50.5%;"></div>

                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <div class="panel panel-primary caixaBotao escondeCaixa">
                                                            <div class="panel-heading">
                                                                <h3 class="panel-title">E-Commerce</h3><i class="fa fa-chevron-up"></i>
                                                            </div>
                                                            <div class="panel-body">
                                                                <div id="divGeral_Total_eCommerce" class="col-lg-5">
                                                                    <div class="panel" style="box-shadow: none; -webkit-box-shadow: none; margin: 0;">
                                                                        <div class="panel-heading" style="position: relative; padding: 0;">
                                                                            <div class="panel-flex">

                                                                                <div class="CaixaDividida horizontalCompleto caixaBotao_Cima">
                                                                                    <asp:HyperLink ID="lnkTotalPedidos_eCommerce" runat="server" CssClass="btn btn-verde btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                                        <div class="botao">
                                                                                            <div class="col-lg-7 text-left" style="padding: 0;">
                                                                                                <i class="fa fa-list fa-3x"></i>
                                                                                            </div>
                                                                                            <div class="col-lg-5 text-right" style="padding: 0;">
                                                                                                <asp:Label runat="server" ID="lblTotalPedidos_eCommerce" class="qtd" Text="0"></asp:Label>
                                                                                            </div>
                                                                                            <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                                <span>Total</span>
                                                                                            </div>
                                                                                        </div>
                                                                                    </asp:HyperLink>
                                                                                </div>

                                                                                <div class="CaixaDividida horizontalCompleto caixaBotao_Baixo">
                                                                                    <asp:HyperLink ID="lnkEntrega_eCommerce" runat="server" CssClass="btn btn-verde btn-block text-left" Style="border-radius: 0 0 4px 4px;">
                                                                                        <div class="botao">
                                                                                            <div class="col-lg-7 text-left" style="padding: 0;">
                                                                                                <i class="fa fa-truck fa-3x"></i>
                                                                                            </div>
                                                                                            <div class="col-lg-5 text-right" style="padding: 0;">
                                                                                                <asp:Label runat="server" ID="lblEntrega_eCommerce" class="qtd" Text="0"></asp:Label>
                                                                                            </div>
                                                                                            <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                                <span>Entregas</span>
                                                                                            </div>
                                                                                        </div>
                                                                                    </asp:HyperLink>
                                                                                </div>

                                                                                <div class="divisoria fixo" style="left: 0; right: 0; top: 50%;"></div>

                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div id="divGeral_Depto_7Dias_Atraso_eCommerce" class="col-lg-7">
                                                                    <div class="panel" style="box-shadow: none; -webkit-box-shadow: none; margin: 0;">
                                                                        <div class="panel-heading" style="position: relative; padding: 0;">
                                                                            <div class="panel-flex">

                                                                                <div class="panel-flex">
                                                                                    <div class="CaixaDividida horizontalMetade caixaBotao_Cima">
                                                                                        <asp:HyperLink ID="lnkDepto_eCommerce" runat="server" CssClass="btn btn-azul btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                                            <div class="botao">
                                                                                                <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                                    <i class="fa fa-tasks fa-2x"></i>
                                                                                                    <asp:Label runat="server" ID="lblDepto_eCommerce" class="qtd" Text="0"></asp:Label>
                                                                                                </div>
                                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                                    <span>Seu Depto.</span>
                                                                                                </div>
                                                                                            </div>
                                                                                        </asp:HyperLink>
                                                                                    </div>

                                                                                    <div class="CaixaDividida horizontalMetade caixaBotao_Cima">
                                                                                        <asp:HyperLink ID="lnk7Dias_eCommerce" runat="server" CssClass="btn btn-amarelo btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                                            <div class="botao">
                                                                                                <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                                    <i class="fa fa-exclamation fa-2x"></i>
                                                                                                    <asp:Label runat="server" ID="lbl7Dias_eCommerce" class="qtd" Text="0"></asp:Label>
                                                                                                </div>
                                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                                    <span>Venc. 7 Dias</span>
                                                                                                </div>
                                                                                            </div>
                                                                                        </asp:HyperLink>
                                                                                    </div>

                                                                                    <div class="CaixaDividida horizontalMetade caixaBotao_Baixo_Esquerda">
                                                                                        <asp:HyperLink ID="lnkAtraso_eCommerce" runat="server" CssClass="btn btn-vermelho btn-block text-left" Style="border-radius: 4px 4px 0 0;">
                                                                                            <div class="botao">
                                                                                                <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                                    <i class="fa fa-times fa-2x"></i>
                                                                                                    <asp:Label runat="server" ID="lblAtraso_eCommerce" class="qtd" Text="0"></asp:Label>
                                                                                                </div>
                                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                                    <span>Atraso</span>
                                                                                                </div>
                                                                                            </div>
                                                                                        </asp:HyperLink>
                                                                                    </div>

                                                                                    <div class="CaixaDividida horizontalMetade caixaBotao_Baixo_Direita">
                                                                                        <asp:HyperLink ID="lnkConfirmacao_eCommerce" runat="server" CssClass="btn btn-vermelho btn-block text-left bordaDivisoria" Style="border-radius: 0 0 4px 4px;">
                                                                                            <div class="botao">
                                                                                                <div class="col-lg-12" style="display: flex; flex-wrap: nowrap; justify-content: space-between; vertical-align: middle; padding: 0;">
                                                                                                    <i class="fa fa-question fa-2x"></i>
                                                                                                    <asp:Label runat="server" ID="lblConfirmacao_eCommerce" class="qtd" Text="0"></asp:Label>
                                                                                                </div>
                                                                                                <div class="col-lg-12 text-right" style="padding: 0;">
                                                                                                    <span>Confirmação</span>
                                                                                                </div>
                                                                                            </div>
                                                                                        </asp:HyperLink>
                                                                                    </div>
                                                                                </div>

                                                                                <div class="divisoria fixo" style="left: 0; right: 0; top: 50.5%;"></div>

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
                                </div>

                                <div class="col-lg-12" runat="server" id="DIV_TAREFAS">
                                    <div class="row">
                                        <div class="panel panel-primary caixaBotao">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Tarefas</h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="row">
                                                    <div class="col-lg-6">
                                                        <asp:HyperLink ID="lnkTarefas_TotalUsuario" runat="server">
                                                            <div class="panel panel-success caixaBotao">
                                                                <div class="panel-heading">

                                                                    <div class="row">
                                                                        <div class="col-xs-4">
                                                                            <i class="fa fa-comments fa-5x"></i>
                                                                        </div>
                                                                        <div class="col-xs-8 text-right">
                                                                            <p class="announcement-heading">
                                                                                <asp:Label ID="lblTarefas_TotalUsuario" runat="server" Text=""></asp:Label>
                                                                            </p>
                                                                            <p class="announcement-text">Suas Tarefas</p>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="panel-footer announcement-bottom">
                                                                    <div class="row">
                                                                        <div class="col-xs-6">
                                                                            Ver 
                                                                        </div>
                                                                        <div class="col-xs-6 text-right">
                                                                            <i class="fa fa-arrow-circle-right"></i>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                            </div>

                                                        </asp:HyperLink>
                                                    </div>

                                                    <div class="col-lg-6">
                                                        <asp:HyperLink ID="lnkTarefas_TotalDepartamento" runat="server">
                                                            <div class="panel panel-warning caixaBotao">
                                                                <div class="panel-heading">
                                                                    <div class="row">
                                                                        <div class="col-xs-4">
                                                                            <i class="fa fa-tasks fa-5x"></i>
                                                                        </div>
                                                                        <div class="col-xs-8 text-right">
                                                                            <p class="announcement-heading">
                                                                                <asp:Label ID="lblTarefas_TotalDepartamento" runat="server" Text=""></asp:Label>
                                                                            </p>
                                                                            <p class="announcement-text">Tarefas seu Depto</p>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="panel-footer announcement-bottom">
                                                                    <div class="row">
                                                                        <div class="col-xs-6">
                                                                            Ver
                                                                        </div>
                                                                        <div class="col-xs-6 text-right">
                                                                            <i class="fa fa-arrow-circle-right"></i>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </asp:HyperLink>
                                                    </div>
                                                </div>
                                                <!-- /.row -->
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="timer_Atualizar" EventName="Tick" />
                                <asp:AsyncPostBackTrigger ControlID="cmdPesquisar_Calendario" EventName="Click" />
                            </Triggers>
                        </asp:UpdatePanel>

                    </div>

                    <div id="div_Calendario_View" class="col-lg-3" style="padding-right: 0;">
                        <asp:UpdatePanel ID="upAvisosCalendario" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <div class="dashboard-calendario-lateral">
                                    <uc1:Calendario runat="server" ID="Calendario_View" />
                                </div>
                            </ContentTemplate>
                            <Triggers>
                                <asp:AsyncPostBackTrigger ControlID="timer_Atualizar" EventName="Tick" />
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                </div>

                <asp:UpdatePanel ID="upDashboard" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>

                        <div class="row" runat="server" id="DIV_GRID_PEDIDOS">
                            <div class="col-lg-12">
                                <div class="panel panel-primary">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Pedidos</h3>
                                    </div>
                                    <div class="panel-body">
                                        <div id="UltimosEventos" class="table-responsive">

                                            <asp:GridView ID="gvUltimosPedidos" runat="server" AutoGenerateColumns="false" CellPadding="0" CellSpacing="1"
                                                class="table table-striped table-bordered table-hover table-condensed table-responsive" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvUltimasOcorrencias_RowDataBound">
                                                <Columns>

                                                    <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                        DataTextField="nNumeroPedido" HeaderText="Pedido"
                                                        DataNavigateUrlFormatString="Paginas/Pedidos_Detalhe.aspx?id={0}">
                                                        <HeaderStyle Width="8%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:HyperLinkField>

                                                    <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                        DataTextField="nControleTT" HeaderText="N° Controle TT"
                                                        DataNavigateUrlFormatString="Paginas/Pedidos_Detalhe.aspx?id={0}">
                                                        <HeaderStyle Width="15%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:HyperLinkField>

                                                    <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                        DataTextField="sReferencia" HeaderText="Referência"
                                                        DataNavigateUrlFormatString="Paginas/Pedidos_Detalhe.aspx?id={0}">
                                                        <HeaderStyle Width="20%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:HyperLinkField>


                                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscCliente" HeaderText="Cliente" HtmlEncode="False" HtmlEncodeFormatString="False">
                                                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Left" Width="18%" />
                                                    </asp:BoundField>

                                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtPedido" HeaderText="Data Pedido" HtmlEncode="False" HtmlEncodeFormatString="False">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" Width="8%" />
                                                    </asp:BoundField>

                                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscFluxo" HeaderText="Fluxo" HtmlEncode="False" HtmlEncodeFormatString="False">
                                                        <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Left" Width="10%" />
                                                    </asp:BoundField>

                                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscStatus" HeaderText="Status" HtmlEncode="False" HtmlEncodeFormatString="False">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" Width="12%" />
                                                    </asp:BoundField>


                                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscDepartamento" HeaderText="Departamento" HtmlEncode="False" HtmlEncodeFormatString="False">
                                                        <HeaderStyle HorizontalAlign="Left" />
                                                        <ItemStyle HorizontalAlign="Left" Width="12%" />
                                                    </asp:BoundField>

                                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtEstimativaEntrega" HeaderText="Estimativa Entrega" HtmlEncode="False" HtmlEncodeFormatString="False">
                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Center" Width="8%" />
                                                    </asp:BoundField>

                                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtPrevisaoEntregaFormatada" HeaderText="Previsão Entrega" HtmlEncode="False" HtmlEncodeFormatString="False">
                                                        <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        <ItemStyle HorizontalAlign="Center" Width="8%" />
                                                    </asp:BoundField>
                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="row" runat="server" id="DIV_GRID_GRAFICOS">

                            <div class="col-lg-4">
                                <div class="panel panel-primary">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i>Pedidos por Departamento</h3>
                                    </div>
                                    <div class="panel-body">
                                        <div id="GraficoDepartamento"></div>
                                        <div class="text-right">
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="panel panel-primary">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i>Pedidos por Fluxo</h3>
                                    </div>
                                    <div class="panel-body">
                                        <div id="Grafico_Fluxo">
                                        </div>
                                        <div class="text-right">
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="panel panel-primary">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i>Pedidos por Status</h3>
                                    </div>
                                    <div class="panel-body">
                                        <div id="GraficosStatus">
                                        </div>
                                        <div class="text-right">
                                        </div>
                                    </div>
                                </div>
                            </div>


                            <div class="col-lg-4" runat="server" id="div_Outros">
                                <div class="panel panel-primary">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><i class="fa fa-long-arrow-right"></i>Outros Avisos</h3>
                                    </div>
                                    <div class="panel-body">
                                        <div id="Grafico_Origem">
                                        </div>
                                        <div class="text-right">
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>

                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="timer_Atualizar" EventName="Tick" />
                        <asp:AsyncPostBackTrigger ControlID="cmdPesquisar_Calendario" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>

                <asp:Timer ID="timer_Atualizar" runat="server" Interval="60000" OnTick="timer_Atualizar_Tick"></asp:Timer>

            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="calendario" aria-labelledby="aba_calendario">
            <br />

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtros de Eventos</h3>
                </div>
                <div class="panel-body">
                    <div class="row">

                        <div class="col-lg-3 form-group">
                            <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server" MaxLength="200"></asp:TextBox>
                        </div>

                        <div class="col-lg-3 form-group">
                            <asp:DropDownList ID="ddlidStatus" runat="server" class="form-control Caixa_Selecao">
                                <asp:ListItem Value="0" Text="Todos os Status de Atividades"></asp:ListItem>
                                <asp:ListItem Value="1" Text="Não Iniciada"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Em andamento"></asp:ListItem>
                                <asp:ListItem Value="3" Text="Pausada"></asp:ListItem>
                                <asp:ListItem Value="4" Text="Excluída"></asp:ListItem>
                                <asp:ListItem Value="5" Text="Finalizada"></asp:ListItem>
                                <asp:ListItem Value="6" Text="Pausada por Impeditivo"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-lg-3 form-group">
                            <asp:DropDownList ID="ddlTipoEvento" runat="server" class="form-control Caixa_Selecao">
                                <asp:ListItem Value="-1" Text="Todos os Eventos do Calendário"></asp:ListItem>
                                <asp:ListItem Value="1" Text="Apenas Tarefas de Pedidos"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Apenas Atividades"></asp:ListItem>
                                <asp:ListItem Value="0" Text="Outros"></asp:ListItem>
                            </asp:DropDownList>
                        </div>

                        <div class="col-lg-1 form-group">
                            <asp:Button ID="cmdPesquisar_Calendario" class="btn btn-primary" runat="server" Text="Pesquisar" OnClick="cmdPesquisar_Calendario_Click" OnClientClick="MudarBotao_Pesquisar(this)" />
                        </div>

                    </div>
                </div>
            </div>

            <div class="panel panel-primary">
                <div class="panel-body">
                    <uc1:Calendario runat="server" ID="Calendario" />
                </div>
            </div>

        </div>

    </div>

</asp:Content>
