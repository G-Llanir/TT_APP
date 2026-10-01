<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Pedidos_Detalhe.aspx.cs" Inherits="TT_Flow.App.Pedidos_Detalhe" EnableEventValidation="false" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/app/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/DetalheModalProduto.ascx" TagPrefix="uc1" TagName="DetalheModalProduto" %>
<%@ Register Src="~/App/Controles/DropDownList_Padrao.ascx" TagPrefix="uc1" TagName="DropDownList_Padrao" %>
<%@ Register Src="~/App/Controles/TextBox_Padrao.ascx" TagPrefix="uc1" TagName="TextBox_Padrao" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>
<%@ Register Src="~/App/Controles/Calendario.ascx" TagPrefix="uc1" TagName="Calendario" %>
<%@ Register Src="~/App/Controles/IA_Widget.ascx" TagPrefix="uc1" TagName="IAWidget" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="RV" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .embed-responsive-16by9 {
            padding-bottom: 200px;
        }

        .toUpper {
            text-transform: uppercase;
        }

        /* CSS-Composição */
        .excel-details-btn {
            padding: 0;
            font-size: 10px; /* Tamanho do ícone */
            text-align: center;
            text-decoration: none;
            display: inline-block;
            cursor: pointer;
            background: none;
            border: none;
        }

            .excel-details-btn:focus {
                outline: none;
            }

        .details-column {
            display: none; /* Inicia oculto, será exibido quando clicar no botão "+" */
        }

        /*CSS - Sugestão*/

        .text-navy {
            color: #1ab394;
        }

        .cart-product-imitation {
            text-align: center;
            background-color: #f8f8f9;
            display: flex;
            justify-content: center;
            align-items: center;
        }

            .cart-product-imitation img {
                max-width: 100%;
                max-height: 100%;
            }

        .product-imitation.xl {
            padding: 120px 0;
        }

        .product-desc {
            padding: 20px;
            position: relative;
        }

        table.shoping-cart-table {
            margin-bottom: 0;
        }

            table.shoping-cart-table tr td {
                border: none;
                text-align: right;
            }

                table.shoping-cart-table tr td.desc,
                table.shoping-cart-table tr td:first-child {
                    text-align: left;
                }

                table.shoping-cart-table tr td:last-child {
                    width: 80px;
                }

        .table.shoping-cart-table tr td {
            padding: 10px; /* ou qualquer valor de espaçamento que desejar */
        }

            /* Se desejar espaçamento específico para uma coluna em particular */
            .table.shoping-cart-table tr td:nth-child(3) {
                padding-left: 20px; /* Especifica o espaçamento apenas para a terceira coluna */
            }

        .ibox {
            clear: both;
            margin-bottom: 25px;
            margin-top: 0;
            padding: 0;
        }

            .ibox.collapsed .ibox-content {
                display: none;
            }

            .ibox:after,
            .ibox:before {
                display: table;
            }

        .ibox-title {
            -moz-border-bottom-colors: none;
            -moz-border-left-colors: none;
            -moz-border-right-colors: none;
            -moz-border-top-colors: none;
            background-color: #ffffff;
            border-color: #e7eaec;
            border-image: none;
            border-style: solid solid none;
            border-width: 3px 0 0;
            color: inherit;
            margin-bottom: 0;
            padding: 14px 15px 7px;
            min-height: 48px;
        }

        .ibox-content {
            background-color: #ffffff;
            color: inherit;
            padding: 15px 20px 20px 20px;
            border-color: #e7eaec;
            border-image: none;
            border-style: solid solid none;
            border-width: 1px 0;
        }

        .ibox-footer {
            color: inherit;
            border-top: 1px solid #e7eaec;
            font-size: 90%;
            background: #ffffff;
            padding: 10px 15px;
        }

        .removed-item {
            background-color: #f8d7da; /* cor de fundo para indicar remoção */
            /* Outros estilos de formatação ou indicadores visuais, se necessário */
        }

        /*Agnes Partal * 12/07/2024 -------------------------*/
        .card {
            box-shadow: 0 4px 8px 0 rgba(0,0,0,0.2);
            transition: 0.3s;
            width: 100%;
            border-radius: 5px;
        }

        .card-body {
            padding: 10px;
            display: flex;
            align-items: flex-start;
        }

        .flex-grow-1 {
            flex-grow: 1;
            margin-left: 15px;
        }

        .img-thumbnail {
            border: none;
            width: 100px;
            height: auto;
        }

        .me-3 {
            margin-right: 1rem;
        }

        .modal-title {
            text-shadow: 1px 2px 3px rgba(0, 0, 0, 0.2);
            font-size: 20px;
            font-weight: bold;
            margin: 0;
        }

        .modal-STSO {
            width: 70% !important;
            height: 70% !important; /* 40% da altura da janela de visualização */
            max-width: none !important;
        }

        .visible {
            display: none;
        }

        .pages table {
            position: relative;
            float: right;
        }

        .pages span {
            display: flex;
            justify-content: center;
            font-weight: bold;
            border: 1px solid black;
            border-radius: 5px;
            background-color: #ddd;
            color: black;
            margin: 2px 5px 2px 5px;
            padding: 2.5px 7.5px 2.5px 7.5px;
        }

        .pages a {
            display: flex;
            justify-content: center;
            font-weight: bold;
            border: 1px solid skyblue;
            border-radius: 5px;
            background-color: white;
            margin: 2px 5px 2px 5px;
            padding: 2.5px 7.5px 2.5px 7.5px;
        }


        /* Estilos para a Grid de CheckList */

        .checklist th {
            background-color: forestgreen;
            color: white;
        }

        .checkList_Title {
            background-color: forestgreen;
            color: white;
            font-size: 15px;
            font-family: Tahoma;
            font-weight: bold;
        }
        /*---------------------------------------------------*/

        .gvChildHeader th {
            padding: 4px;
            background-color: rgba(50, 150, 50);
            color: #fff;
            border: 1px solid #bbb;
            font-weight: bold;
        }

        .gvRow td {
            padding: 7px;
            background-color: #ffffff;
            border: 1px solid #bbb;
        }

        .gvAltRow td {
            padding: 7px;
            background-color: #f1f1f1;
            border: 1px solid #bbb;
        }

        .collapsed-row {
            display: none;
            padding: 1px;
            margin: 1px;
        }

        .infoAlterada {
            color: red;
            font-weight: bold;
        }

        .infoNova {
            color: green;
            font-weight: bold;
        }

        .upload-container {
            width: fit-content;
            height: fit-content;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto;
        }

        .file-upload-label input {
            display: none;
        }

        .file-upload-label svg {
            height: 50px;
            fill: rgb(82, 82, 82);
            margin-bottom: 20px;
        }

        .file-upload-label {
            cursor: pointer;
            background-color: #ddd;
            padding: 30px 70px;
            border-radius: 40px;
            border: 2px dashed rgb(82, 82, 82);
            box-shadow: 0px 0px 20px -50px rgba(0, 0, 0, 0.719);
            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;
            gap: 5px;
            text-align: center;
        }

            .file-upload-label.dragover {
                border-color: #000;
                background-color: #e6f7ff;
            }

        .browse-button {
            background-color: rgb(82, 82, 82);
            padding: 5px 15px;
            border-radius: 10px;
            color: white;
            transition: all 0.3s;
        }

            .browse-button:hover {
                background-color: rgb(14, 14, 14);
            }

        .file-name {
            margin-top: 10px;
            font-size: 14px;
            color: rgb(82, 82, 82);
        }

        .form-check input {
            margin: 0;
        }
    </style>

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row" style="">
        <div class="col-lg-12">
            <br />
            <asp:UpdatePanel ID="updPanel_BreadCrumb" runat="server">
                <ContentTemplate>
                    <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
                    <div class="well bg-danger" runat="server" id="caixaTitulo">
                        <h4><span id="MainContent_lblTituloPagina">
                            <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                            <br />
                        </span></h4>
                    </div>
                    <uc1:IAWidget runat="server" ID="ucIAWidget" />
                </ContentTemplate>
            </asp:UpdatePanel>
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

            <div class="embed-responsive embed-responsive-16by9 table-responsive" id="DIV_Kanban" runat="server">
                <embed type="text/html" runat="server" id="frmKanban" />
            </div>
        </div>
    </div>

    <div id="abas">
        <ul id="tab_Pedido" class="nav nav-tabs" role="tablist">
            <li role="presentation" runat="server" id="aba_Dados" class="tabpanel active">
                <a href="#pedido" id="pedido-tab" role="tabpanel" data-toggle="tab" aria-controls="pedido" aria-expanded="false"><b>
                    <asp:Label ID="DADOS_lblAba01" runat="server" Text="Pedido"></asp:Label>
                </b></a>
            </li>
            <li role="presentation" runat="server" id="aba_ValoresDatas">
                <a href="#valoresdatas" role="tab" id="ValoresDatas-tab" data-toggle="tab" aria-controls="ValoresDatas"><b>Valores e Datas</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Resultado">
                <a href="#resultado" role="tab" id="Resultado_tab" data-toggle="tab" aria-controls="Resultado"><b>Resultado</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Acoes" class="tabpanel">
                <a href="#acoes" id="acoes_tab" role="tabpanel" data-toggle="tab" aria-controls="Ações"><b>Ações</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Arquivos">
                <a href="#arquivos" role="tab" id="arquivos-tab" data-toggle="tab" aria-controls="Arquivos"><b>Arquivos</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Documentos">
                <a href="#documentos" role="tab" id="documentos-tab" data-toggle="tab" aria-controls="Documentos"><b>Documentos</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Tarefas">
                <a href="#tarefas" role="tab" id="tarefas-tab" data-toggle="tab" aria-controls="Tarefas"><b>Tarefas</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_LM">
                <a href="#lm" role="tab" id="lm-tab" data-toggle="tab" aria-controls="LM"><b>LM</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_OPI">
                <a href="#OPI" role="tab" id="OPI-tab" data-toggle="tab" aria-controls="OPI"><b>OPI/Envios</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_depara">
                <a href="#depara" role="tab" id="depara-tab" data-toggle="tab" aria-controls="depara"><b>De / Para</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_STSO">
                <a href="#STSO" role="tab" id="STSO-tab" data-toggle="tab" aria-controls="STSO"><b>STSO</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_ART">
                <a href="#ART" role="tab" id="ART-tab" data-toggle="tab" aria-controls="ART"><b>ART</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Cronograma">
                <a href="#Cronograma" role="tab" id="Cronograma-tab" data-toggle="tab" aria-controls="Cronograma"><b>Cronograma / OS</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Faturamento">
                <a href="#Faturamento" role="tab" id="Faturamento-tab" data-toggle="tab" aria-controls="Faturamento"><b>Faturamento</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_Historico">
                <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
            </li>
            <li role="presentation" runat="server" id="aba_ArquivoMorto">
                <a href="#arquivoMorto" role="tab" id="arquivoMorto-tab" data-toggle="tab" aria-controls="ArquivoMorto"><b>Arquivo Morto</b></a>
            </li>

        </ul>
    </div>

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="pedido" aria-labelledby="pedido-tab">
            <br />
            <div id="DIV_PEDIDO_GERAL" runat="server">

                <asp:UpdatePanel ID="upd_Pedido" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="panel panel-default" runat="server" id="div_Pedido">
                            <div class="panel-heading" style="display: flex; flex-wrap: nowrap; justify-content: space-between;">
                                <h3 class="panel-title"><b>Dados</b></h3>
                                <div runat="server" id="div_NavegaPedidos" style="display: flex; flex-wrap: nowrap; justify-content: end; gap: .25em;">
                                    <asp:HyperLink runat="server" ID="cmdPedidoAnterior" CssClass="btn btn-xs btn-circle btn-info" data-toggle="tooltip_top" title="Pedido Anterior"><i class="fa fa-arrow-left"></i></asp:HyperLink>
                                    <asp:HyperLink runat="server" ID="cmdProximoPedido" CssClass="btn btn-xs btn-circle btn-info" data-toggle="tooltip_top" title="Próximo Pedido"><i class="fa fa-arrow-right"></i></asp:HyperLink>
                                </div>
                            </div>
                            <div class="panel-body">
                                <div class="form-stacked row">

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>
                                                <asp:Label ID="DADOS_lblData" runat="server" Text="Data Pedido"></asp:Label>
                                            </label>
                                            <asp:TextBox ID="txtdtPedido" class="form-control " runat="server" MaxLength="10" onkeypress="mascaraData(this, event)" ReadOnly="true"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-3" runat="server" style="z-index: 6" id="DivTipoCompra">
                                        <div class="form-group">
                                            <label>Tipo de Compra</label>
                                            <asp:DropDownList ID="ddlsTipoCompra" runat="server" class="form-control yes_no select Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlsTipoCompra_SelectedIndexChanged">
                                                <asp:ListItem Value="0">Selecione o Tipo</asp:ListItem>
                                                <asp:ListItem Value="N">Nacional</asp:ListItem>
                                                <asp:ListItem Value="I">Internacional</asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-3" id="DivOrigem" runat="server">
                                        <div class="form-group" runat="server" id="DIV_DADOS_NPedido">
                                            <label>
                                                <asp:Label ID="lblnPedido" runat="server" Text="Nº Pedido (Cliente)"></asp:Label></label>
                                            <asp:TextBox ID="txtsPedidoCliente" class="form-control uppercase" runat="server" MaxLength="20"></asp:TextBox>
                                        </div>

                                        <div class="form-group" runat="server" id="DIV_DADOS_Origem">
                                            <label>Origem</label>
                                            <asp:DropDownList ID="ddlidPais" runat="server" class="form-control yes_no select" OnSelectedIndexChanged="ddlidPais_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-1" id="DivPrev" runat="server">
                                        <div class="form-group" id="div_PrevisaoEntrega" runat="server">
                                            <label>
                                                <asp:Label ID="DADOS_lblEntrega" runat="server" Text="Prev Entrega"></asp:Label>
                                                <i class="fa fa-question-circle" data-toggle="tooltip" title="A Previsão de Entrega é a data prevista para a entrega, já considerando previsões do almoxarifado / serviços."></i>
                                            </label>
                                            <span title="Previsão de Entrega">
                                                <h4>
                                                    <asp:Label ID="lbldtPrevisaoEntrega" runat="server" TabIndex="100"></asp:Label>
                                                </h4>
                                            </span>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="DIV_nControleTT">
                                        <div class="form-group">
                                            <label>N° Controle TT</label>
                                            <asp:TextBox ID="txtnControleTT" class="form-control uppercase" runat="server" MaxLength="50"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="DIV_Referencia">
                                        <div class="form-group">
                                            <label>Referência</label>
                                            <asp:TextBox ID="txtsReferencia" class="form-control uppercase" runat="server" MaxLength="50"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" id="DIV_OrderNumber" runat="server">
                                        <div class="form-group">
                                            <label>Order Number</label>
                                            <asp:TextBox ID="txtnOrderNumber" class="form-control uppercase" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" id="InvoiceNumber" runat="server">
                                        <div class="form-group">
                                            <label>Proforma Invoice Number </label>
                                            <asp:TextBox ID="TextInvoiceNumber" class="form-control uppercase " runat="server" MaxLength="21"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-6" runat="server" id="DIV_DADOS_Cliente">
                                        <div class="form-group">
                                            <label>Cliente </label>
                                            <asp:TextBox ID="txtsRazaoSocial" class="form-control uppercase" runat="server" MaxLength="200"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-6" id="DIV_Fornecedor" style="z-index: 5" runat="server">
                                        <div class="form-group">
                                            <label>Fornecedor</label>
                                            <asp:DropDownList ID="ddlFornecedor" runat="server" AutoPostBack="True" class="form-control yes_no select Caixa_Selecao" OnSelectedIndexChanged="ddlFornecedor_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-6" runat="server" id="DIV_Exportador">
                                        <div class="row">
                                            <div class="col-lg-8" id="DIV_DADOS_Importacao_Fornecedor" runat="server">
                                                <div class="form-group">
                                                    <label>Exportador</label>
                                                    <asp:DropDownList ID="ddlImportacao_idFornecedor" runat="server" AutoPostBack="True" class="form-control yes_no select" OnSelectedIndexChanged="ddlExportador_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-lg-4" runat="server" id="Contato_Exportador">
                                                <div class="form-group">
                                                    <label>Contato Exportador</label>
                                                    <asp:DropDownList ID="ddlContato_Exportador" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-6" runat="server" id="DIV_17">
                                        <div class="row">
                                            <div class="col-lg-4" style="z-index: 6">
                                                <div class="form-group">
                                                    <label>Fluxo</label>
                                                    <asp:DropDownList ID="ddlFluxoPedido" runat="server" AutoPostBack="True" class="form-control yes_no select Caixa_Selecao" OnSelectedIndexChanged="ddlFluxoPedido_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-4" runat="server" id="DIV_DADOS_Vendedor" style="z-index: 6">
                                                <div class="form-group">
                                                    <label>
                                                        <asp:Label ID="lblVendedor" runat="server" Text="Vendedor"></asp:Label></label>
                                                    <asp:DropDownList ID="ddlVendedor" runat="server" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao" OnSelectedIndexChanged="ddlVendedor_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-4" runat="server" id="DIV_DADOS_Modal">
                                                <div class="form-group">
                                                    <label>Modal</label>
                                                    <asp:DropDownList ID="ddlidModal" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidModal_SelectedIndexChanged" class="form-control yes_no select"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-4" runat="server" id="DIV_DADOS_CondPagamento" style="z-index: 6">
                                                <div class="form-group">
                                                    <label>Condição Pagamento</label>
                                                    <asp:DropDownList ID="ddlCondPagamento" runat="server" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-4" runat="server" id="DIV_DADOS_Despacho">
                                                <div class="form-group">
                                                    <label>Despacho</label>
                                                    <asp:DropDownList ID="ddlImportacao_idDespacho" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-6" runat="server" id="DIV_Despachante_C">
                                        <div class="row">
                                            <div class="col-lg-8" runat="server" id="DIV_DADOS_Despachante">
                                                <div class="form-group">
                                                    <label>Despachante </label>
                                                    <asp:DropDownList ID="ddlImportacao_idDespachante" runat="server" AutoPostBack="True" class="form-control yes_no select" OnSelectedIndexChanged="ddlFluxoPedido_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-lg-4" runat="server" id="Contato_Despachante">
                                                <div class="form-group">
                                                    <label>Contato Despachante</label>
                                                    <asp:DropDownList ID="ddlContato_Despachante" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div runat="server" id="DIV16">
                                        <div runat="server" id="DIV17">
                                            <div runat="server" id="DIV15">
                                                <div runat="server" id="DIV18">
                                                    <div class="col-lg-2" id="div_EstimativaEntrega" runat="server">
                                                        <div class="form-group">
                                                            <label>
                                                                <asp:Label ID="DADOS_lblEstimativa" runat="server" Text="Estimativa Entrega"></asp:Label>
                                                                <i class="fa fa-question-circle" data-toggle="tooltip" title="A Estimativa de Entrega é a data estimada para a entrega, sem considerar almoxarifado ou serviços."></i>
                                                            </label>
                                                            <asp:TextBox ID="txtdtEstimativaEntrega" class="form-control uppercase" runat="server" MaxLength="10" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-4" id="Div_FormadeEnvio" runat="server">
                                                        <div class="form-group">
                                                            <label>
                                                                <asp:Label ID="lblFormadeEnvio" runat="server" Text="Forma de Envio"></asp:Label>
                                                            </label>
                                                            <asp:DropDownList ID="ddlTipoEnvio" runat="server" AutoPostBack="true" class="form-control yes_no select Caixa_Selecao" OnSelectedIndexChanged="ddlTipoEnvio_SelectedIndexChanged"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2" id="Div_FreteVendas" runat="server" visible="false">
                                                        <label>Valor Frete</label>
                                                        <div class="input-group" runat="server" id="Div_FreteVendasBox">
                                                            <asp:TextBox ID="txtnFreteVendas" class="form-control" runat="server" MaxLength="10" AutoPostBack="True" OnTextChanged="txtFrete_TextChanged" ToolTip="Valor do Frete" Style="z-index: 0;"></asp:TextBox>
                                                            <span class="input-group-addon" id="MoedaFreteVendas" runat="server"></span>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>



                                            <div class="col-lg-2" id="DIV_Zona" runat="server">
                                                <div class="form-group">
                                                    <label>Zona Envio Material</label>
                                                    <asp:TextBox ID="txtsZonaEnvio" class="form-control uppercase" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-6" runat="server" id="DIV_EMPRESA" style="z-index: 4">
                                                <div class="row">
                                                    <div class="col-lg-8" runat="server" id="Div_ddlEmpresa">
                                                        <div class="form-group">
                                                            <label>Empresa</label>
                                                            <asp:DropDownList ID="ddlidEmpresa" runat="server" AutoPostBack="True" class="form-control yes_no select Caixa_Selecao" OnSelectedIndexChanged="ddlEmpresa_SelectedIndexChanged"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-4" runat="server" id="Contato_Empresa">
                                                        <div class="form-group">
                                                            <label>Contato Empresa</label>
                                                            <asp:DropDownList ID="ddlContato_Empresa" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-4" id="DIV_DADOS_TipoFaturamento" runat="server">
                                                        <div class="form-group">
                                                            <label>Tipo de Faturamento</label>
                                                            <asp:DropDownList ID="ddlidTipoFaturamento" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-6" runat="server" id="DIV_Importador">
                                        <div class="row">
                                            <div class="col-lg-8">
                                                <div class="form-group">
                                                    <label>Importador</label>
                                                    <asp:DropDownList ID="ddlImportador" runat="server" AutoPostBack="True" class="form-control yes_no select" OnSelectedIndexChanged="ddlImportador_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-4" runat="server" id="Contato_Importador">
                                                <div class="form-group">
                                                    <label>Contato Importador</label>
                                                    <asp:DropDownList ID="ddlContato_Importador" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="DIV_TermosPagamento">
                                        <div class="form-group">
                                            <label>Termos de Pagamentos</label>
                                            <asp:DropDownList ID="ddlTermosPagamento" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-6" runat="server" id="DIV_EnderecoEntrega" style="z-index: 5">
                                        <div class="form-group">
                                            <label>Endereço de Entrega</label>
                                            <asp:DropDownList ID="ddlsEnderecoEntrega" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                            <asp:TextBox ID="txtsEnderecoEntrega" class="form-control uppercase " runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <asp:HiddenField runat="server" ID="hddAltera_Endereco" Value="0" />

                                    <div runat="server" id="DIV_20" visible="true">
                                        <div id="DIV_Conceitual" runat="server" visible="false">
                                            <div class="row">
                                                <div class="col-lg-4" id="DivConceito" runat="server" visible="false">
                                                    <div class="form-group">
                                                        <label>Centro de Custos </label>
                                                        <asp:DropDownList ID="ddlCentroCusto" runat="server" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2" id="DivCentroCustos" style="z-index: 3;" runat="server" visible="false">
                                                    <div class="form-group">
                                                        <label>Conceito </label>
                                                        <asp:DropDownList ID="ddlConceito" runat="server" AutoPostBack="True" class="form-control yes_no select Caixa_Selecao" OnSelectedIndexChanged="ddlConceito_SelectedIndexChanged"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2" id="divGrupoPatrimonio" style="z-index: 3;" runat="server" visible="false">
                                                    <div class="form-group">
                                                        <label>Grupo de Patrimônio </label>
                                                        <asp:DropDownList ID="ddlGrupoPatrimonio" runat="server" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2" id="divFrete" style="z-index: 3;" runat="server">
                                                    <div class="form-group">
                                                        <label>Frete</label>
                                                        <asp:DropDownList ID="ddlFrete" runat="server" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div runat="server" id="DIV_21">

                                            <div class="col-lg-6" runat="server" id="DIV_Valores">
                                                <div class="row">
                                                    <div class="col-lg-2" id="div_nVlrFrete" runat="server">
                                                        <label>Valor Frete</label>
                                                        <div class="input-group" runat="server" id="DIV_Frete">
                                                            <asp:TextBox ID="txtFrete" class="form-control" runat="server" MaxLength="10" AutoPostBack="True" OnTextChanged="txtFrete_TextChanged" ToolTip="Valor do Frete" Style="z-index: 0;"></asp:TextBox>
                                                            <span class="input-group-addon" id="MoedaFrete" runat="server"></span>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-2" id="div_nVlrImpostos" runat="server">
                                                        <label>DIFAL / ST</label>
                                                        <div class="btn-group dropup float-r">
                                                            <a class="btn btn-link dropdown-toggle m-0 padd-0" data-toggle="dropdown"><i class="fa fa-plus"></i></a>
                                                            <div class="dropdown-menu m-0 padd-0">
                                                                <asp:Table runat="server" ID="tbImpostos" CssClass="table table-bordered table-condensed table-hover m-0">
                                                                    <asp:TableRow>
                                                                        <asp:TableCell><b>ST</b></asp:TableCell>
                                                                        <asp:TableCell runat="server" ID="txtImposto_ST" CssClass="text-right"></asp:TableCell>
                                                                    </asp:TableRow>
                                                                    <asp:TableRow>
                                                                        <asp:TableCell><b>DIFAL</b></asp:TableCell>
                                                                        <asp:TableCell runat="server" ID="txtImposto_DIFAL" CssClass="text-right"></asp:TableCell>
                                                                    </asp:TableRow>
                                                                </asp:Table>
                                                            </div>
                                                        </div>
                                                        <div class="input-group">
                                                            <asp:TextBox ID="txtImpostos" class="form-control" runat="server" MaxLength="10" ToolTip="Valor do ST" ReadOnly="true" Style="z-index: 0;"></asp:TextBox>
                                                            <span class="input-group-addon" id="MoedaCompra3" runat="server"></span>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-4" id="DIV_nVlrProdutos" runat="server">
                                                        <label>Valor Produtos </label>
                                                        <div class="input-group" runat="server" id="DIV_Produtos">
                                                            <asp:TextBox ID="txtnVlrProdutos" class="form-control uppercase " runat="server" MaxLength="21" AutoPostBack="True" OnTextChanged="txtnVlrProdutos_TextChanged" ToolTip="Valor dos Produtos" Style="z-index: 0;"></asp:TextBox>
                                                            <span class="input-group-addon" id="MoedaCompra2" runat="server"></span>
                                                        </div>
                                                    </div>
                                                    <div runat="server" id="div_produtosEnvios">
                                                        <div class="col-lg-4 form-group" id="DIV_nVlrServicos" runat="server">
                                                            <label>
                                                                <asp:Label ID="lblnVlrServicos" runat="server" Text="Valor Serviços"></asp:Label>
                                                            </label>
                                                            <div class="input-group" runat="server" id="DIV_Servicos">
                                                                <asp:TextBox ID="txtnVlrServicos" class="form-control uppercase " runat="server" MaxLength="21" AutoPostBack="True" OnTextChanged="txtnVlrServicos_TextChanged" ToolTip="Valor dos Serviços" Style="z-index: 0;"></asp:TextBox>
                                                                <span class="input-group-addon" id="MoedaCompra1" runat="server"></span>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-2" id="DIV_nDesconto" runat="server">
                                                        <label>Desconto %</label>
                                                        <asp:TextBox ID="txtnDesconto" class="form-control" runat="server" MaxLength="21" AutoPostBack="True" OnTextChanged="txtnDesconto_TextChanged" ToolTip="Porcentagem de Desconto"></asp:TextBox>
                                                    </div>
                                                    <div class="col-lg-2" id="DIV_nVlrDesconto" runat="server">
                                                        <label>Valor Desconto</label>
                                                        <div runat="server" id="div_txtnVlrDesconto" class="input-group">
                                                            <asp:TextBox ID="txtnVlrDesconto" class="form-control" runat="server" MaxLength="21" AutoPostBack="True" OnTextChanged="txtnDesconto_TextChanged" ToolTip="Valor de Desconto" Style="z-index: 0;"></asp:TextBox>
                                                            <span class="input-group-addon" id="MoedaCompra4" runat="server"></span>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-4" id="DIV_nVlrTotal" runat="server" style="float: right;">
                                                        <label>Total</label>
                                                        <div class="input-group" runat="server" id="DIV_txtnVlrTotal">
                                                            <asp:TextBox ID="txtnVlrTotal" class="form-control uppercase " runat="server" MaxLength="21" disabled="disabled" ToolTip="Valor Total do Pedido" Style="z-index: 0; font-weight: bold;"></asp:TextBox>
                                                            <asp:HiddenField runat="server" ID="hddnVlrTotal" />
                                                            <span class="input-group-addon" id="MoedaCompra" runat="server" style="z-index: 0; font-weight: bold;"></span>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3" runat="server" id="div_MoedaConsulta" visible="false">
                                                        <div class="form-group" runat="server" id="div68">
                                                            <label>Moeda</label>
                                                            <asp:TextBox ID="txtMoedaConsulta" class="form-control" runat="server" disabled="disabled"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>

                                            </div>

                                            <div class="col-lg-2" runat="server" id="div_Moeda">
                                                <div class="col-lg-12 padd-0" runat="server" id="div_MoedaSelecao">
                                                    <uc1:DropDownList_Padrao runat="server" ID="ddlsMoeda" Titulo="Moeda" AutoPostBack="true" OnSelectedIndexChanged="ddlsMoeda_SelectedIndexChanged" />
                                                </div>
                                                <div class="col-lg-6 padd-r-0" runat="server" id="div_CambioMoeda" visible="false">
                                                    <uc1:TextBox_Padrao runat="server" ID="txtCambioMoeda" Titulo="Taxa de Câmbio" Classe="nCambio" ReadOnly="true" />
                                                </div>
                                            </div>

                                            <div class="col-lg-6" id="DIV_Comissao" runat="server">
                                                <div class="row">

                                                    <div class="col-lg-5" runat="server" id="div_AgenteVendas">
                                                        <div class="form-group">
                                                            <label>Agente de Vendas</label>
                                                            <asp:DropDownList ID="ddlidParceiro_Comissionador" runat="server" class="form-control yes_no select" AutoPostBack="true" OnSelectedIndexChanged="ddlidParceiro_Comissionador_SelectedIndexChanged"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-4" id="div_ComissaoValor" runat="server">
                                                        <div class="form-group">
                                                            <label>Valor Comissão</label>
                                                            <asp:TextBox ID="txtnVlrComissao" class="form-control uppercase" runat="server" MaxLength="21" ToolTip="Valor Total da Comissão"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-3" id="div_ComissaoPaga" runat="server">
                                                        <uc1:SwitchAtivo ID="ComissaoPaga" runat="server" />
                                                    </div>

                                                    <%--<div class="col-lg-3" id="div_ComissaoPaga" runat="server">
                                                        <div class="form-group">
                                                            <label></label>
                                                            <asp:DropDownList ID="ddlsComissaoPaga" runat="server" class="form-control yes_no select">
                                                                <asp:ListItem Selected="True" Value="S">Sim</asp:ListItem>
                                                                <asp:ListItem Value="N">Não</asp:ListItem>
                                                            </asp:DropDownList>
                                                        </div>
                                                    </div>--%>
                                                </div>
                                            </div>

                                            <div class="col-lg-6" id="DIV_Departamento" runat="server">
                                                <div class="row">
                                                    <div class="col-lg-6" id="DIV_STATUS_ATUAL" runat="server">
                                                        <div class="form-group">
                                                            <label>Status Atual </label>
                                                            <asp:TextBox ID="txtsDscStatus" class="form-control uppercase" runat="server" MaxLength="100"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-6" id="DIV_DEPARTAMENTO_ATUAL" runat="server">
                                                        <div class="form-group">
                                                            <label>Departamento Atual </label>
                                                            <asp:TextBox ID="txtsDepartamentoAtual" class="form-control uppercase " runat="server" MaxLength="10"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-6" id="DIV_PedidoClienteFinal" runat="server">
                                                <div class="row">

                                                    <div class="col-lg-2" id="div_sClienteFinal" runat="server">
                                                        <uc1:SwitchAtivo ID="sClienteFinalswt" runat="server" />
                                                    </div>

                                                    <div class="col-lg-10" runat="server" id="div_idClienteFinal">
                                                        <div class="form-group">
                                                            <label>Cliente Final</label>
                                                            <asp:DropDownList ID="ddlidClienteFinal" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>

                                            <div class="col-lg-6" id="DIV_Instalador" runat="server">
                                                <div class="row">
                                                    <div class="col-lg-6" id="DIV_ddlInstalador" runat="server">
                                                        <div class="form-group">
                                                            <label>Instalador</label>
                                                            <asp:DropDownList runat="server" ID="ddlidInstalador" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-2" id="div_CustoAduaneiro" runat="server" visible="false">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtCustoAduaneiro" Titulo="Custo Aduaneiro" MascaraDinamica="$money($input, ',')" ReadOnly="true" Grupo="true" AutoPostBack="true" OnTextChanged="txtCustoAdicional_TextChanged" />
                                                    </div>
                                                    <div class="col-lg-2" id="div_CustoDespachante" runat="server" visible="false">
                                                        <uc1:TextBox_Padrao runat="server" ID="txtCustoDespachante" Titulo="Custo Despachante" MascaraDinamica="$money($input, ',')" ReadOnly="true" Grupo="true" AutoPostBack="true" OnTextChanged="txtCustoAdicional_TextChanged" />
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <div id="Div_ContratoVendas" class="col-lg-12" runat="server" visible="false">
                                        <div class="row">

                                            <div class="col-lg-2" id="DIV_TempoContrato" runat="server">
                                                <div class="form-group">
                                                    <label>Duração do Contrato</label>
                                                    <asp:TextBox ID="txtnTempoContrato" class="form-control" runat="server" TextMode="Number" placeholder="0"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-1" id="DIV_TipoPeriodo" runat="server">
                                                <div class="form-group">
                                                    <label>Período</label>
                                                    <asp:DropDownList ID="ddlTipoPeriodo" runat="server" class="form-control Caixa_Selecao">
                                                        <asp:ListItem Value="D" Text="Dia(s)"></asp:ListItem>
                                                        <asp:ListItem Value="M" Text="Mês(es)" Selected="True"></asp:ListItem>
                                                        <asp:ListItem Value="A" Text="Ano(s)"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="ddlidParceiro_Comissionador" EventName="SelectedIndexChanged" />
                        <asp:AsyncPostBackTrigger ControlID="ddlFluxoPedido" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>

                <asp:UpdatePanel ID="updPanel_Itens" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="panel panel-default" runat="server" id="Div_Itens">
                            <div class="panel-heading">
                                <div style="display: flex; justify-content: space-between; flex-wrap: nowrap;">
                                    <div>
                                        <h3 class="panel-title"><b>Itens</b></h3>
                                    </div>
                                    <div style="display: flex; flex-wrap: nowrap; gap: 1rem;">
                                        <div runat="server" id="div_ImportarItens">
                                            <asp:LinkButton ID="cmdImportarItens" CssClass="btn btn-primary" Text="Importar Itens" runat="server" OnClientClick="$('#Modal_ImportarItens').modal('show'); return false;"></asp:LinkButton>
                                        </div>
                                        <div runat="server" id="div_Ordem">
                                            <div class="pull-right">
                                                <asp:DropDownList ID="ddlOrdem" runat="server" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlOrdem_SelectedIndexChanged">
                                                    <asp:ListItem Value="0" Text="Ordenar por" />
                                                    <asp:ListItem Value="1" Text="Ordem" />
                                                    <asp:ListItem Value="2" Text="Código" />
                                                    <asp:ListItem Value="3" Text="Produto" />
                                                    <asp:ListItem Value="4" Text="NCM" />
                                                    <asp:ListItem Value="5" Text="Quantidade" />
                                                    <asp:ListItem Value="6" Text="Destino" />
                                                    <asp:ListItem Value="7" Text="DrawBack" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div runat="server" id="div_btdesc">
                                            <asp:LinkButton runat="server" ID="btDesc" CssClass="btn" data-toggle="tooltip_top" title="Crescente / Decrescente" OnClick="btDesc_Click"><i class="fa fa-sort"></i></asp:LinkButton>
                                        </div>
                                        <div runat="server" id="div_btnItens">
                                            <asp:LinkButton ID="btn_ItensFornecedor" CssClass="btn btn-primary" Text="Produtos do Fornecedor" OnClick="btn_ItensFornecedor_Click" runat="server" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="panel-body">
                                <div class="form-group row">
                                    <div class="col-lg-12">
                                        <div class="form-group row" runat="server" id="div_SelecaoItens">
                                            <div class="col-lg-12">
                                                <uc1:MensagemPagina runat="server" ID="MensagemPagina5" />
                                            </div>

                                            <div class="col-lg-2">
                                                <label>Código</label>
                                                <div class="input-group" runat="server" id="DIV_CodigoInput">
                                                    <span class="input-group-btn" runat="server" id="lnkAddProduto">
                                                        <asp:LinkButton runat="server" class="btn btn-default" type="button" data-toggle="tooltip_top" title="Adicionar Novo Produto" OnClick="lnkAddProduto_Click"><i class="fa fa-plus"></i></asp:LinkButton>
                                                    </span>
                                                    <asp:TextBox ID="Item_txtsCodigoProduto" class="form-control uppercase " runat="server" MaxLength="50" OnTextChanged="txtsCodigoProduto_TextChanged" AutoPostBack="True" ValidationGroup="Item"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div runat="server" id="div_Item_txtsDscProduto" class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Produto </label>
                                                    <asp:TextBox ID="Item_txtsDscProduto" class="form-control uppercase " runat="server" MaxLength="200" OnTextChanged="txtsDscProduto_TextChanged" ValidationGroup="Item"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-1" id="DIV_Unidade" runat="server">
                                                <div class="form-group">
                                                    <label>Unidade</label>
                                                    <asp:DropDownList ID="Item_ddlsUnidade" runat="server" class="form-control" attrname="Unidade" ValidationGroup="Item"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" id="DIV_UnidadeEntrega" runat="server">
                                                <div class="form-group">
                                                    <label>Unidade de Entrega</label>
                                                    <asp:DropDownList ID="Item_ddlsUnidadeEntrega" runat="server" class="form-control" attrname="Unidade" ValidationGroup="Item"></asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" id="DIV_Item_dtPrevisaoEntrega" runat="server">
                                                <div class="form-group">
                                                    <label>
                                                        <asp:Label ID="lblPrevisão" runat="server" Text="Previsão Entrega"></asp:Label></label>
                                                    <asp:TextBox ID="txtdtPrevisaoEntrega" class="form-control uppercase" runat="server" MaxLength="10" Type="date"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="DIV_Item_Quantidade">
                                                <div class="form-group">
                                                    <label runat="server" id="lbl_Item_Quantidade">Quantidade</label>
                                                    <asp:TextBox ID="Item_txtnQuantidade" class="form-control uppercase " runat="server" MaxLength="10" ValidationGroup="Item"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" id="DIV_Item_ValorUnitario" runat="server">
                                                <div class="form-group">
                                                    <label>Valor Unitário</label>
                                                    <asp:TextBox ID="Item_txtnValorUnitario" class="form-control uppercase" runat="server" MaxLength="10" ValidationGroup="Item"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-1">
                                                <label>&nbsp;</label>
                                                <asp:Button ID="Item_cmdIncluirItem" class="form-control btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirItem_Click" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="Item_MensagemPagina" />
                                    </div>

                                    <asp:HiddenField ID="Item_hddsDscCategoriaVendas" runat="server" />
                                    <asp:HiddenField ID="Item_hddidNCM" runat="server" />
                                    <asp:HiddenField ID="Item_hddsCodigoNCM" runat="server" />
                                    <asp:HiddenField ID="Item_hddPesquisaPor" runat="server" />
                                    <asp:HiddenField ID="Item_hddidProduto" runat="server" />
                                    <asp:HiddenField ID="Envio_hddEditarEnvios" runat="server" Value="N" />
                                    <asp:HiddenField ID="Item_hddsTipoProduto_Servico" runat="server" Value="N" />
                                    <asp:HiddenField ID="Item_hddssDscDescricaoIdioma" runat="server" />
                                    <asp:HiddenField ID="Item_hddsValortblPreco" runat="server" />
                                </div>

                                <div class="modal fade" id="modalSugestao" tabindex="-1" role="dialog" aria-labelledby="modalSugestaoLabel" aria-hidden="true">
                                    <div class="modal-dialog" role="document">
                                        <div class="modal-content">
                                            <div class="modal-header">
                                                <h5 class="modal-title" id="modalSugestaoLabel"><b>Deseja adicionar também, os Itens Abaixo?</b></h5>
                                                <p class="small">OBS: Para Remover 1 dos itens apenas, basta digitar "0" na quantidade</p>
                                                <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                                    <span aria-hidden="true">&times;</span>
                                                </button>
                                            </div>

                                            <div class="modal-body">
                                                <asp:Repeater ID="rptItensSugestao" runat="server" OnItemDataBound="rptItemSugerido_ItemDataBound">
                                                    <ItemTemplate>
                                                        <div class="ibox-content">
                                                            <div class="table-responsive">
                                                                <table class="table shoping-cart-table">
                                                                    <tbody>
                                                                        <tr>
                                                                            <td width="90">
                                                                                <div class="cart-product-imitation">
                                                                                    <asp:Image ID="imgProdutoPrincipal" runat="server" class="cart-product-imitation img" />
                                                                                </div>

                                                                                <asp:TextBox runat="server" ID="txtIdProdutoSugestao" Visible="false" Text='<%#Eval("idProdutoSugestao") %>'></asp:TextBox>

                                                                            </td>
                                                                            <td class="desc">
                                                                                <h3>
                                                                                    <a class="text-navy"><%#Eval( "sDscProduto") %>
                                                                                    </a>
                                                                                </h3>

                                                                                <p class="small">
                                                                                    <%#Eval( "sCodigo") %>
                                                                                </p>
                                                                            </td>
                                                                            <td class="col-lg-3">
                                                                                <label>Quantidade</label>
                                                                                <asp:TextBox ID="txtQuantidade" runat="server" CssClass="form-control" Text='<%# Bind("nQuantidade", "{0:N2}") %>'></asp:TextBox>
                                                                                <asp:HiddenField ID="hddsCodigo" runat="server" Value='<%# Eval("sCodigo") %>' />
                                                                                <asp:HiddenField ID="hddsUnidade" runat="server" Value='<%# Eval("sUnidade") %>' />
                                                                                <asp:HiddenField ID="hddsDscProduto" runat="server" Value='<%# Eval("sDscProduto") %>' />
                                                                                <%--  <asp:LinkButton CssClass="btn btn-danger" Text="Remover" runat="server" OnClick="Remover_Click" CommandArgument='<%# Eval("idProdutoSugestao") %>'/>--%>
                                                                            </td>
                                                                        </tr>
                                                                    </tbody>
                                                                </table>
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </div>

                                            <div class="modal-footer">
                                                <asp:LinkButton CssClass="btn btn-success" Text="Adicionar" runat="server" OnClick="Adicionar_Click" />
                                                <asp:LinkButton CssClass="btn btn-danger" Text="Cancelar" runat="server" OnClick="Cancelar_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <uc1:DetalheModalProduto runat="server" ID="DetalheModalProduto" />

                                <div class="row" id="div_btnExcluirSelecionados" runat="server">
                                    <div class="col-lg-10">&nbsp;</div>

                                    <div class="col-lg-2" style="margin-bottom: 10px; display: flex; justify-content: end;">
                                        <asp:Button runat="server" ID="cmd_ExcluirItensSelecionados" class="form-control btn-danger" Text="Excluir Selecionados" OnClick="cmd_ExcluirItensSelecionados_Click" Style="inline-size: auto;" />
                                    </div>
                                </div>

                                <div class="form-group row">
                                    <div class="col-lg-12">
                                        <asp:GridView ID="dtgItens" class="table table-striped table-bordered table-hover table-condensed"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idContador,sTipoProduto_Servico, sTipo, idProduto, sMoedaVenda"
                                            OnRowDataBound="dtgItens_RowDataBound" OnRowDeleting="dtgItens_RowDeleting" OnSorting="gridView_Sorting">
                                            <Columns>

                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <asp:CheckBox ID="cb_Excluir_Todos" runat="server" CssClass="excluirTodos"></asp:CheckBox>
                                                    </HeaderTemplate>
                                                    <HeaderStyle CssClass="text-center" />
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="cb_Excluir" runat="server" CssClass="form-control excluir"></asp:CheckBox>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sDscCategoriaVendas" HeaderText="Categoria">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sTipo" HeaderText="Categoria">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="idContador" HeaderText="Contador" DataFormatString="{0:N0}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Ordem">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtnOrdem_pedidoItem" class="form-control CaixaTextoMicro" MaxLength="4" runat="server" Text='<%# Bind("nOrdem") %>'></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="nOrdem" HeaderText="Ordem">
                                                    <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                </asp:BoundField>

                                                <%--Agnes Partal * 19/07/2024---------%>
                                                <asp:TemplateField HeaderText="Código">
                                                    <ItemTemplate>
                                                        <%# MostrarLink(Eval("idProduto"), Eval("sCodigoProduto"))%>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                </asp:TemplateField>
                                                <%------------------------------------%>

                                                <asp:TemplateField HeaderText="Produto">
                                                    <ItemTemplate>
                                                        <div>
                                                            <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_Sistema" + Eval("idContador") %>' />
                                                            <asp:LinkButton ID="lnkDetalhes" runat="server" CssClass="excel-details-btn" data-toggle="tooltip_top" title="Composição" OnClientClick='<%# "toggleDetails(this); return false;" %>'><i class="fa fa-plus"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="lnkServico" runat="server" CssClass="excel-details-btn" data-toggle="tooltip_top" title="Composição do Serviço" OnClientClick='<%# "Servico(this, \"" + Eval("idProduto") + "\", \"" + Eval("sDscProduto") + "\"); return false;" %>'><i class="fa fa-plus"></i></asp:LinkButton>

                                                            <%--Agnes Partal * 10/07/2024---------%>
                                                            <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                OnClientClick='<%# "openModal(\"" + Eval("idProduto") + "\"); return false;" %>'
                                                                OnMouseOver='<%# "mostraCard(this, \"" + Eval("idProduto") + "\", \"produto\");" %>'
                                                                OnMouseOut='<%# "escondeCard( \"" + Eval("idProduto") + "\", \"produto\");" %>'
                                                                CssClass="produto-detalhe-link"
                                                                ClientIDMode="Static"
                                                                data-idproduto='<%# Eval("idProduto") %>'
                                                                data-tabela="produto" />

                                                            <div id='<%# Eval("idProduto") + "_produto" %>' class="product-card" style="display: none;">
                                                                <!--conteudo via script -->
                                                            </div>
                                                            <%------------------------------------%>

                                                            <span class="details-column">
                                                                <br />
                                                                <br />
                                                                <div class="col-lg-12">
                                                                    <div class="form-group">
                                                                        <label>Composição do Produto:</label>
                                                                        <!-- PlaceHolder para adicionar o GridView de detalhes -->
                                                                        <asp:PlaceHolder ID="phDetalhesComposicao" runat="server"></asp:PlaceHolder>
                                                                    </div>
                                                                </div>
                                                            </span>
                                                        </div>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Previsão Entrega">
                                                    <ItemTemplate>
                                                        <asp:Label ID="dtPrevisaoEntrega" runat="server" MaxLength="10" Text='<%# Bind("dtPrevisaoEntrega") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="9%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Previsão Entrega">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtdtEntrega" runat="server" class="form-control" MaxLength="10" OnTextChanged="Item_GV_txtnValorUnitario_TextChanged" AutoPostBack="true" Text='<%# Bind("dtPrevisaoEntrega") %>'></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="9%" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sCodigoNCM" HeaderText="NCM">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sUnidade" HeaderText="UN">
                                                    <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Qtde">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="Item_GV_txtnQuantidade" runat="server" class="form-control" MaxLength="20" OnTextChanged="Item_GV_txtnValorUnitario_TextChanged" AutoPostBack="true" Text='<%# Bind("nQuantidade", "{0:N2}") %>'></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="7%" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="nQuantidade" HeaderText="Qtde" DataFormatString="{0:N2}">
                                                    <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="% ICMS">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="Item_GV_txtnICMS" runat="server" class="form-control" MaxLength="5" Text='<%# Bind("nICMS", "{0:N2}") %>'></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="6%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="% ICMS">
                                                    <ItemTemplate>
                                                        <asp:Label ID="nICMS" runat="server" MaxLength="5" Text='<%# Bind("nICMS", "{0:N2}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="6%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="% IPI">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="Item_GV_txtnIPI" runat="server" class="form-control" AutoPostBack="true" OnTextChanged="Item_GV_txtnValorUnitario_TextChanged" Text='<%# Bind("nIPI", "{0:N2}") %>'></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="6%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="% IPI">
                                                    <ItemTemplate>
                                                        <asp:Label ID="nIPI" runat="server" MaxLength="5" Text='<%# Bind("nIPI", "{0:N2}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="6%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Unitário">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="Item_GV_txtnValorUnitario" runat="server" OnTextChanged="Item_GV_txtnValorUnitario_TextChanged" class="form-control" AutoPostBack="true" MaxLength="20" Text='<%# Bind("nValorUnitario") %>'></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="9%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Unitário">
                                                    <ItemTemplate>
                                                        <asp:Label ID="nValorUnitario" runat="server" Text='<%# Bind("nValorUnitario") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" Width="9%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Desconto">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="Item_GV_txtnDesconto" runat="server" OnTextChanged="Item_GV_txtnValorUnitario_TextChanged" class="form-control" AutoPostBack="true" MaxLength="20" Text='<%# Bind("nDesconto", "{0:N4}") %>'></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="7%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Desconto">
                                                    <ItemTemplate>
                                                        <asp:Label ID="nDesconto" runat="server" Text='<%# Bind("nDesconto", "{0:N4}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" Width="7%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Total">
                                                    <ItemTemplate>
                                                        <asp:Label ID="nValorTotal" runat="server" Text='<%# Bind("nValorTotal", "{0:N2}") %>'></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="9%" HorizontalAlign="Right" VerticalAlign="Middle" CssClass="_nValorTotal" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Destino">
                                                    <HeaderTemplate>
                                                        <div id="ReplicarSelecao_Destino" class="row">
                                                            <div class="col-lg-12" style="display: flex;">
                                                                <div class="col-lg-9" style="padding: 0">
                                                                    <label>Destino</label>
                                                                </div>
                                                                <div class="col-lg-3" style="display: flex; justify-content: right; padding: 0;">
                                                                    <div class="col-lg-9" style="padding: 0;">
                                                                        <asp:LinkButton runat="server" ID="cmdReplicaSelecao_Destino" ClientIDMode="Static" class="form-control btn-primary" data-toggle="tooltip" title="Replicar Seleção da Primeira Linha"><i class="fa fa-arrow-down" style="position: relative; right: 25%;"></i></asp:LinkButton>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlItem_Importacao_idDestino" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="Importacao_sDscDestino" HeaderText="Destino">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="DrawBack">
                                                    <HeaderTemplate>
                                                        <div id="ReplicarSelecao_DrawBack" class="row">
                                                            <div class="col-lg-12" style="display: flex;">
                                                                <div class="col-lg-9" style="padding: 0">
                                                                    <label>DrawBack</label>
                                                                </div>
                                                                <div class="col-lg-3" style="display: flex; justify-content: right; padding: 0;">
                                                                    <div class="col-lg-9" style="padding: 0;">
                                                                        <asp:LinkButton runat="server" ID="cmdReplicarSelecao_DrawBack" ClientIDMode="Static" class="form-control btn-primary" data-toggle="tooltip" title="Replicar Seleção da Primeira Linha"><i class="fa fa-arrow-down" style="position: relative; right: 25%;"></i></asp:LinkButton>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:DropDownList ID="ddlItem_Importacao_idAtoConcessorio" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="Importacao_sCodigoAtoConcessorio" HeaderText="DrawBack">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" CommandName="Delete" CommandArgument="idContador"><i class='fa-eraser fa'></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <%# NovaLinha(Eval("idContador"), "_Sistema") %>

                                                        <asp:GridView ID="gvComposicaoSistema" class="table table-striped table-bordered table-hover table-condensed padd-0 m-0"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            HeaderStyle-CssClass="gvChildHeader" CssClass="gvRow" AlternatingRowStyle-CssClass="gvAltRow"
                                                            Font-Names="Tahoma" Font-Overline="False">
                                                            <Columns>

                                                                <asp:TemplateField HeaderText="Código">
                                                                    <ItemTemplate>
                                                                        <%# MostrarLink(Eval("idProduto"), Eval("sCodigo"))%>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Produto">
                                                                    <ItemTemplate>
                                                                        <div>
                                                                            <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                                OnClientClick='<%# "openModal(\"" + Eval("idProduto") + "\"); return false;" %>'
                                                                                OnMouseOver='<%# "mostraCard(this, \"" + Eval("idProduto") + "\", \"produto\");" %>'
                                                                                OnMouseOut='<%# "escondeCard( \"" + Eval("idProduto") + "\", \"produto\");" %>'
                                                                                CssClass="produto-detalhe-link"
                                                                                ClientIDMode="Static"
                                                                                data-idproduto='<%# Eval("idProduto") %>'
                                                                                data-tabela="produto" />

                                                                            <div id='<%# Eval("idProduto") + "_produto" %>' class="product-card" style="display: none;">
                                                                                <!--conteudo via script -->
                                                                            </div>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="sCodigoNCM" HeaderText="NCM">
                                                                    <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sDscTipoProduto" HeaderText="Tipo">
                                                                    <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sUnidade" HeaderText="Unidade">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" DataFormatString="{0:N2}">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                            </Columns>
                                                        </asp:GridView>
                                                    </ItemTemplate>
                                                    <HeaderStyle CssClass="invisivel" />
                                                    <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="invisivel" />
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="Item_cmdIncluirItem" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="Item_txtsCodigoProduto" EventName="TextChanged" />
                        <asp:AsyncPostBackTrigger ControlID="gvImportarItens" EventName="RowCommand" />
                        <asp:AsyncPostBackTrigger ControlID="ddlFornecedor" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>

                <asp:UpdatePanel ID="UpdEnvios" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="panel panel-default" runat="server" id="DIV_Envios">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Envios</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="form-group row">
                                    <div class="col-lg-12">
                                        <div class="form-group row" runat="server" id="DIV_Envios_Selecao">
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Tipo Envio</label>
                                                    <asp:DropDownList ID="ddlImportacao_TipoEnvio" AutoPostBack="true" OnSelectedIndexChanged="ddlImportacao_TipoEnvios_SelectedIndexChanged" runat="server" class="form-control" attrname="Tipo Envio" ValidationGroup="Envio">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-3" id="DIV_Importacao_Envio_Parceiro" runat="server">
                                                <div class="form-group">
                                                    <label>Courier</label>
                                                    <asp:DropDownList ID="ddlImportacao_TipoEnvioParceiro" runat="server" class="form-control" attrname="Parceiro" ValidationGroup="Envio">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-3" id="DIV_Importacao_Envio_Transportadora" runat="server">
                                                <div class="form-group">
                                                    <label>Transportadora</label>
                                                    <asp:TextBox ID="txtImportacao_Envio_Transportadora" class="form-control " runat="server" MaxLength="50" ValidationGroup="Envio"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Rastreamento </label>
                                                    <asp:TextBox ID="txtImportacao_TipoEnvioCodigo" class="form-control " runat="server" MaxLength="50" ValidationGroup="Envio"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Método Envio</label>
                                                    <asp:DropDownList ID="ddlImportacao_MetodoEnvio" runat="server" class="form-control" attrname="Método de Envio" ValidationGroup="Envio">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Meio de Envio</label>
                                                    <asp:DropDownList ID="ddlImportacao_MeioEnvio" runat="server" class="form-control" attrname="Meio de Envio" ValidationGroup="Envio">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-1">
                                                <br />
                                                <asp:Button ID="cmdIncluirEnvios" class="btn btn-info" runat="server" ValidationGroup="Envio" Text="Incluir" OnClick="cmdIncluirEnvios_Click" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                                    </div>
                                </div>


                                <div class="form-group row">

                                    <div class="col-lg-12">

                                        <asp:HiddenField runat="server" ID="HiddenField1" />

                                        <asp:GridView ID="gv_Envios" class="table table-striped table-bordered table-hover table-condensed"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idContador, idRegistro"
                                            OnRowDataBound="gvImportacao_Envios_RowDataBound" OnRowDeleting="gvImportacao_Envios_RowDeleting" OnRowCommand="gvImportacao_Envios_RowCommand">
                                            <Columns>

                                                <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="idContador" HeaderText="Ordem">
                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscTipoEnvio" HeaderText="Tipo do Envio">
                                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscParceiro" HeaderText="Courrier/Transportadora">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sCodigo" HeaderText="Rastreamento">
                                                    <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscMetodoEnvio" HeaderText="Método Envio">
                                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscMeioEnvio" HeaderText="Meio de Envio">
                                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkDelete" runat="server" CommandName="Delete" CssClass="btn btn-small"><i class='fa-eraser fa'></i></asp:LinkButton>
                                                        <asp:LinkButton ID="lnkEdit" runat="server" CommandName="Alterar" CssClass="btn btn-small" CommandArgument='<%# Eval("idContador") %>'><i class='fa fa-pencil'></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="4%" />
                                                </asp:TemplateField>

                                                <%--<asp:CommandField ButtonType="Link" DeleteText="<i class='fa-eraser fa'></i>" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="4%" ShowDeleteButton="true" />--%>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cmdIncluirEnvios" EventName="Click" />
                        <asp:AsyncPostBackTrigger ControlID="ddlImportacao_TipoEnvio" EventName="SelectedIndexChanged" />
                    </Triggers>
                </asp:UpdatePanel>

                <asp:UpdatePanel ID="updVolumes" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <div class="panel panel-default" runat="server" id="DIV_Volumes">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Volumes</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="form-group row">
                                    <div class="col-lg-12">
                                        <div class="form-group row" runat="server" id="DIV_Volumes_Selecao">

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Meio de Volumes</label>
                                                    <asp:DropDownList ID="ddlImportacao_MeioVolume" runat="server" class="form-control" attrname="Meio de Envio" ValidationGroup="Envio">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Peso (kgs) </label>
                                                    <asp:TextBox ID="txtPeso" class="form-control " runat="server" MaxLength="50" ValidationGroup="Envio"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Largura (M) </label>
                                                    <asp:TextBox ID="txtLargura" class="form-control " runat="server" MaxLength="50" ValidationGroup="Envio"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Comprimento (M) </label>
                                                    <asp:TextBox ID="txtComprimento" class="form-control " runat="server" MaxLength="50" ValidationGroup="Envio"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Altura (M)</label>
                                                    <asp:TextBox ID="txtAltura" class="form-control " runat="server" MaxLength="50" ValidationGroup="Envio"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-1">
                                                <div class="form-group">
                                                    <label>Quantidade</label>
                                                    <asp:TextBox ID="txtQuantidade" class="form-control " runat="server" MaxLength="50" ValidationGroup="Envio"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-1">
                                                <br />
                                                <asp:Button ID="cmdIncluirVolumes" class="btn btn-info" runat="server" ValidationGroup="Envio" Text="Incluir" OnClick="cmdIncluirVolumes_Click" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="msgImportacao_Envio" />
                                    </div>
                                </div>

                                <div class="form-group row">
                                    <div class="col-lg-12">

                                        <asp:HiddenField runat="server" ID="hddidLinha_Envio" />

                                        <asp:GridView ID="gvImportacao_Volumes" class="table table-striped table-bordered table-hover table-condensed"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idContador, idRegistro"
                                            OnRowDataBound="gvImportacao_Volumes_RowDataBound" OnRowDeleting="gvImportacao_Volumes_RowDeleting" OnRowCommand="gvImportacao_Volumes_RowCommand">
                                            <Columns>

                                                <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="idContador" HeaderText="Ordem">
                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Ordem">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtidContador" runat="server" class="form-control" Text='<%# Bind("idContador") %>' ClientIDMode="Static"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="2%" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sDscTipoEnvio" HeaderText="Tipo do Envio">
                                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscParceiro" HeaderText="Courrier/Transportadora">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sCodigo" HeaderText="Rastreamento">
                                                    <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscMetodoEnvio" HeaderText="Método Envio">
                                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Meio de Envio">
                                                    <ItemTemplate>
                                                        <asp:Label ID="sDscMeioEnvio" runat="server" Text='<%# string.Format("{0} - {1}", Eval( "idMeioEnvio"), Eval("sDscMeioEnvio")) %>' ClientIDMode="Static"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="7%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Peso (kgs)">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtnPesoEnvio" runat="server" class="form-control" Text='<%# Bind("nPesoEnvio", "{0:N4}") %>' ClientIDMode="Static"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="7%" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="nPesoEnvio" HeaderText="Peso (kgs)" ItemStyle-CssClass="_nPesoEnvio" DataFormatString="{0:N4}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Largura (M)">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtnLarguraEnvio" runat="server" class="form-control" Text='<%# Bind("nLarguraEnvio", "{0:N4}") %>' ClientIDMode="Static"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="7%" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="nLarguraEnvio" HeaderText="Largura (M)" ItemStyle-CssClass="_nLarguraEnvio" DataFormatString="{0:N4}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Comprimento (M)">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtnComprimentoEnvio" runat="server" class="form-control" Text='<%# Bind("nComprimentoEnvio", "{0:N4}") %>' ClientIDMode="Static"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="9%" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="nComprimentoEnvio" HeaderText="Comprimento (M)" ItemStyle-CssClass="_nComprimentoEnvio" DataFormatString="{0:N4}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Altura (M)">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtnAlturaEnvio" runat="server" class="form-control" Text='<%# Bind("nAlturaEnvio", "{0:N4}") %>' ClientIDMode="Static"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="7%" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="nAlturaEnvio" HeaderText="Altura (M)" ItemStyle-CssClass="_nAlturaEnvio" DataFormatString="{0:N4}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Volume (M³)">
                                                    <ItemTemplate>
                                                        <asp:Label ID="nVolume" runat="server" Text='<%# Bind("nVolume", "{0:N4}") %>' ClientIDMode="Static"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="7%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkEnvio_UpLoad" CommandArgument='<%# Bind("idRegistro") %>' runat="server" ToolTip="Efetuar Upload de Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                        <asp:LinkButton ID="lnkEnvio_Download" CommandArgument='<%# Bind("idRegistro") %>' runat="server" ToolTip='<%# Bind("sNomeArquivo", "Download do Arquivo: {0}") %>' CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                        <asp:LinkButton ID="lnkEnvio_Excluir" CommandArgument='<%# Bind("idRegistro") %>' runat="server" CssClass="btn btn-small" TabIndex="100" ToolTip="Apagar Registro" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="Envio_Download" CommandArgument='<%# Bind("idRegistro") %>' runat="server" ToolTip='<%# Bind("sNomeArquivo", "Download do Arquivo: {0}") %>' CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <%--<asp:CommandField ButtonType="Link" DeleteText="<i class='fa-eraser fa'></i>" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="4%" ShowDeleteButton="True" />--%>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </ContentTemplate>
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="cmdIncluirVolumes" EventName="Click" />
                    </Triggers>
                </asp:UpdatePanel>

                <div class="row">
                    <div class="col-lg-12">
                        <div class="form-group">
                            <label>Observações</label>
                            <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="150px"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
                <asp:HiddenField runat="server" ID="hddidPedido_Vinculado" Value="0" />

                <fieldset class="form-stacked actions">
                    <asp:Button ID="cmdGravarPedido" class="btn btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="Pedido" />
                    <asp:Button ID="cmdEditar" class="btn btn-lg btn-danger" runat="server" Text="Editar" ValidationGroup="Pedido" />
                    <asp:Button ID="cmdDuplicar" class="btn btn-lg btn-info" runat="server" Text="Duplicar" ValidationGroup="Pedido" />
                    <asp:Button ID="cmdGerarOPI" class="btn btn-lg btn-primary" runat="server" Text="Gerar OPI" OnClick="cmdGerarOPI_Click" />
                    <a href="javascript:history.back()" class="btn btn-lg btn-warning" title="Voltar">Voltar</a>
                    <asp:HyperLink ID="cmdPedido_Vinculado" class="btn btn-lg btn-primary" runat="server" Target="_blank" Style="float: right;">Orçamento <i class="fa fa-arrow-right"></i></asp:HyperLink>
                </fieldset>

            </div>

        </div>

        <div role="tabpanel" class="tab-pane fade" id="valoresdatas" aria-labelledby="valoresdatas-tab">
            <br />
            <asp:UpdatePanel ID="updValoresDatas" runat="server" UpdateMode="Conditional">
                <ContentTemplate>

                    <div class="panel panel-default" runat="server" id="DIV2">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Valores</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">
                                    <div class="row">
                                        <%--<div class="col-lg-2" runat="server">
                                        <div class="form-group">
                                            <label>Ex-Works Value</label>
                                            <asp:TextBox ID="txtnExWorks" class="form-control " runat="server"></asp:TextBox>
                                        </div>
                                    </div>--%>

                                        <div class="col-lg-2" runat="server" id="Div_InlandF">
                                            <div class="form-group">
                                                <label>Inland Freight Fees</label>
                                                <asp:TextBox ID="txtnInlandF" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="Div_Handling">
                                            <div class="form-group">
                                                <label>Handling Fees</label>
                                                <asp:TextBox ID="txtnHandling" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="Div_Consular">
                                            <div class="form-group">
                                                <label>Consular Fees</label>
                                                <asp:TextBox ID="txtnConsular" class="form-control " runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server">
                                            <label>Ocean/Air Fees</label>
                                            <div class="input-group">
                                                <asp:TextBox ID="txtnOcean_Air" class="form-control " runat="server" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                <span class="input-group-addon" id="sMoedaOrigem0" runat="server"></span>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server">
                                            <label>Insurance Fees</label>
                                            <div class="input-group">
                                                <asp:TextBox ID="txtnInsurance" class="form-control " runat="server" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                <span class="input-group-addon" id="sMoedaOrigem1" runat="server"></span>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server">
                                            <label>Other Charges</label>
                                            <div class="input-group">
                                                <asp:TextBox ID="txtnOther_Charges" class="form-control " runat="server" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                <span class="input-group-addon" id="sMoedaOrigem2" runat="server"></span>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server">
                                            <label>Total Envio</label>
                                            <div class="input-group">
                                                <asp:TextBox ID="txtnTotalEnvio" class="form-control " runat="server" ReadOnly="true" aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                <span class="input-group-addon" id="sMoedaOrigem3" runat="server"></span>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="DIV1">
                        <div class="panel-heading">
                            <div class="row">
                                <div class="col-lg-9" style="margin-right: 110px;">
                                    <h3 class="panel-title"><b>Valores e Datas</b></h3>
                                </div>
                                <div class="col-lg-2" runat="server" id="div25">
                                    <div class="pull-right">
                                        <asp:LinkButton ID="btAtualizarVlr" CssClass="btn btn-small" runat="server" OnClick="btAtualizarVlr_Click" AutoPostBack="true"><i class="fa fa-refresh"></i></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">

                                    <asp:GridView ID="gv_Itens_ValoresDatas" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idContador">
                                        <Columns>


                                            <asp:BoundField DataField="idContador" HeaderText="Ordem" DataFormatString="{0:N0}">
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nOrdem" HeaderText="Ordem">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sCodigoProduto" HeaderText="Código">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscProdutoIdioma" HeaderText="Produto">
                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sUnidade" HeaderText="UN">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Quantidade">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="Itens_ValoresDatas_txtnQuantidade" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("nQuantidade", "{0:N4}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>


                                            <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" ItemStyle-CssClass="_nQuantidade" DataFormatString="{0:N4}">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Unitário">
                                                <ItemTemplate>
                                                    <div class="input-group">
                                                        <asp:TextBox ID="Itens_ValoresDatas_txtnValorUnitario" runat="server" ClientIDMode="Static" class="form-control" MaxLength="20" Text='<%# Bind("nValorUnitario", "{0:N4}") %>' aria-describedby="sMoedaOrigem" Style="z-index: 0;"></asp:TextBox>
                                                        <span class="input-group-addon" id="sMoedaOrigem" runat="server"></span>
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Unitário">
                                                <ItemTemplate>
                                                    <div style="display: flex;">
                                                        <asp:Label ID="nValorUnitario" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nValorUnitario", "{0:N4}") %>'></asp:Label>
                                                        <span id="sMoedaOrigem5" runat="server" style="margin-left: 10px;"></span>
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <%--<asp:BoundField DataField="nValorUnitario" HeaderText="Unitário" DataFormatString="{0:N4}">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle"/>
                                            </asp:BoundField>--%>

                                            <asp:TemplateField HeaderText="Total">
                                                <ItemTemplate>
                                                    <div style="display: flex;">
                                                        <asp:Label ID="nValorTotal" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nValorTotal", "{0:N4}") %>'></asp:Label>
                                                        <span id="sMoedaOrigem6" runat="server" style="margin-left: 10px;"></span>
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <%--<asp:BoundField DataField="nValorTotal" HeaderText="Total" ItemStyle-CssClass="_nValorTotal" DataFormatString="{0:N4}">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>--%>

                                            <asp:TemplateField HeaderText="PO">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="Itens_ValoresDatas_txtdtPO" runat="server" class="form-control" MaxLength="10" Text='<%# Bind("Importacao_dtPO") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="Importacao_dtPO" HeaderText="PO">
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="ETD">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="Itens_ValoresDatas_txtdtETD" runat="server" class="form-control" MaxLength="10" Text='<%# Bind("Importacao_dtETD", "{0:dd/MM/yyyy}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="Importacao_dtETD" HeaderText="ETD">
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="ETA">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="Itens_ValoresDatas_txtdtETA" runat="server" class="form-control" MaxLength="10" Text='<%# Bind("Importacao_dtETA", "{0:dd/MM/yyyy}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="Importacao_dtETA" HeaderText="ETA">
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>


                                            <asp:BoundField DataField="dtChegada" HeaderText="Chegada">
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </div>

                            <fieldset class="form-stacked actions">
                                <asp:Button ID="ValoresDatas_cmdGravarPedido" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="Pedido" />
                                <asp:Button ID="ValoresDatas_cmdEditar" class="btn  btn-lg  btn-danger" runat="server" Text="Editar" ValidationGroup="Pedido" />
                                <asp:Button ID="ValoresDatas_cmdDuplicar" class="btn  btn-lg  btn-info" runat="server" Text="Duplicar" ValidationGroup="Pedido" />
                                <a href="javascript:history.back()" class="btn btn-lg btn-warning" title="Voltar">Voltar</a>
                                <%--<input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel2" title="Cancelar" onclick="history.go(-1)" ValidationGroup="Pedido" >--%>
                            </fieldset>

                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="resultado" aria-labelledby="Resultado_tab">
            <br />
            <asp:UpdatePanel ID="UpdResultado" runat="server">
                <ContentTemplate>

                    <div class="panel panel-default" runat="server" id="DIV6">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Resultado</b></h3>
                        </div>

                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina3" />
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12" runat="server" id="DIV_Envio">
                                    <div class="row">
                                        <div class="col-lg-3" runat="server" id="Div_PagoEnvio">
                                            <div class="form-group">
                                                <label>Envio</label>
                                                <asp:DropDownList ID="ddlEnvio" runat="server" class="form-control yes_no select" AutoPostBack="true">
                                                    <asp:ListItem Value="0" Text="Selecione o Envio" />
                                                    <asp:ListItem Value="1" Text="Pago TTL" />
                                                    <asp:ListItem Value="2" Text="Pago Exportador" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">

                                    <asp:GridView ID="gv_resultado" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idContador" ShowFooter="true" OnRowDataBound="gv_resultado_RowDataBound">
                                        <Columns>

                                            <asp:BoundField DataField="idContador" HeaderText="Contador" DataFormatString="{0:N0}">
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nOrdem" HeaderText="Ordem">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sCodigoProduto" HeaderText="Código">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscProduto" HeaderText="Produto">
                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sUnidade" HeaderText="UN">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Quantidade">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtnQuantidade" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("nQuantidade", "{0:N4}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" ItemStyle-CssClass="_nQuantidade" DataFormatString="{0:N4}">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Valor Unitário">
                                                <HeaderTemplate>
                                                    <div style="display: flex; align-items: center;">
                                                        <span>Valor Unitário</span>
                                                        <div style="margin-left: 10px;">
                                                            <asp:LinkButton ID="TodoslnkAplicaValorReal" runat="server" ClientIDMode="Static" OnClick="TodoslnkAplicaValorReal_Click" data-toggle="tooltip" title="Aplicar Valor Real" Style="margin-left: 5px;"><i class="fa fa-arrow-right"></i></asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <div style="display: flex; align-items: center;">
                                                        <div class="input-group">
                                                            <asp:TextBox ID="Itens_Resultado_txtnValorUnitario" runat="server" AutoPostBack="true" ClientIDMode="Static" OnTextChanged="Itens_Resultado_txtnValorUnitario_TextChanged" class="form-control" MaxLength="20" Text='<%# Bind("nValorUnitario", "{0:N4}") %>' aria-describedby="sMoedaOrigem17" Style="z-index: 0;"></asp:TextBox>
                                                            <span class="input-group-addon" id="sMoedaOrigem17" runat="server"></span>
                                                        </div>
                                                        <div style="margin-left: 5px;">
                                                            <asp:LinkButton ID="lnkAplicaValorReal" runat="server" ClientIDMode="Static" OnClick="lnkAplicaValorReal_Click" data-toggle="tooltip" title="Aplicar Valor Real"><i class="fa fa-arrow-right"></i></asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Valor Unitário">
                                                <ItemTemplate>
                                                    <div style="display: flex;">
                                                        <asp:Label ID="nValorUnitario" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nValorUnitario", "{0:N4}") %>' aria-describedby="sMoedaOrigem18" Style="z-index: 0;"></asp:Label>
                                                        <span id="sMoedaOrigem18" runat="server" style="margin-left: 10px;"></span>
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <%--<asp:BoundField DataField="nValorUnitario" HeaderText="Valor Unitário" DataFormatString="{0:N4}">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>--%>

                                            <asp:TemplateField HeaderText="Valor Total">
                                                <ItemTemplate>
                                                    <div style="display: flex;">
                                                        <asp:Label ID="nValorTotal" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nValorTotal", "{0:N4}") %>' aria-describedby="sMoedaOrigem19" Style="z-index: 0;"></asp:Label>
                                                        <span id="sMoedaOrigem19" runat="server" style="margin-left: 10px;"></span>
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <%--<asp:BoundField DataField="nValorTotal" HeaderText="Valor Total" ItemStyle-CssClass="_nValorTotal" DataFormatString="{0:N4}">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>--%>

                                            <asp:TemplateField HeaderText="Valor Real">
                                                <ItemTemplate>
                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtnValorReal" runat="server" AutoPostBack="true" ClientIDMode="Static" OnTextChanged="Itens_Resultado_txtnValorUnitario_TextChanged" class="form-control" MaxLength="20" Text='<%# Bind("nValorReal", "{0:N4}") %>' aria-describedby="sMoedaOrigem20" Style="z-index: 0;"></asp:TextBox>
                                                        <span class="input-group-addon" id="sMoedaOrigem20" runat="server"></span>
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Valor Real">
                                                <ItemTemplate>
                                                    <div style="display: flex;">
                                                        <asp:Label ID="nValorReal" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nValorReal", "{0:N4}") %>' aria-describedby="sMoedaOrigem21" Style="z-index: 0;"></asp:Label>
                                                        <span id="sMoedaOrigem21" runat="server" style="margin-left: 10px;"></span>
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <%--<asp:BoundField DataField="nValorReal" HeaderText="Valor Real" DataFormatString="{0:N4}">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>--%>

                                            <asp:TemplateField HeaderText="Total Real">
                                                <ItemTemplate>
                                                    <div style="display: flex;">
                                                        <asp:Label ID="nValorRealTotal" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nValorRealTotal", "{0:N4}") %>' aria-describedby="sMoedaOrigem22" Style="z-index: 0;"></asp:Label>
                                                        <span id="sMoedaOrigem22" runat="server" style="margin-left: 10px;"></span>
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <%--<asp:BoundField DataField="nValorRealTotal" HeaderText="Total Real" ItemStyle-CssClass="_nValorRealTotal" DataFormatString="{0:N4}">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>--%>

                                            <asp:TemplateField HeaderText="Resultado">
                                                <ItemTemplate>
                                                    <div style="display: flex;">
                                                        <asp:Label ID="nResultado" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nResultado", "{0:N4}") %>' aria-describedby="sMoedaOrigem23" Style="z-index: 0;"></asp:Label>
                                                        <span id="sMoedaOrigem23" runat="server" style="margin-left: 10px;"></span>
                                                    </div>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <%--<asp:BoundField DataField="nResultado" HeaderText="Resultado" ItemStyle-CssClass="_nResultado" DataFormatString="{0:N4}">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>--%>
                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </div>

                            <div class="panel panel-default" runat="server" id="DIV7">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Cambio</b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="form-stacked row">
                                        <div class="col-lg-12 table-responsive">
                                            <div class="row">
                                                <div class="col-lg-2" runat="server">
                                                    <label>Envio Remessas</label>
                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtPago" class="form-control " runat="server" ReadOnly="true" aria-describedby="sMoedaOrigem16" Style="z-index: 0;"></asp:TextBox>
                                                        <span class="input-group-addon" id="sMoedaOrigem16" runat="server"></span>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2" runat="server">
                                                    <label>Valor Invoice</label>
                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtCambio" class="form-control " runat="server" ReadOnly="true" aria-describedby="sMoedaOrigem14" Style="z-index: 0;"></asp:TextBox>
                                                        <span class="input-group-addon" id="sMoedaOrigem14" runat="server"></span>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2" runat="server">
                                                    <label>Resultado Produtos+Frete</label>
                                                    <div class="input-group">
                                                        <asp:TextBox ID="txtPagar" class="form-control " runat="server" ReadOnly="true" aria-describedby="sMoedaOrigem15" Style="z-index: 0;"></asp:TextBox>
                                                        <span class="input-group-addon" id="sMoedaOrigem15" runat="server"></span>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <h3>
                                                            <span class="status" title="Status">
                                                                <asp:Label ID="lblsDscTipoStatus" runat="server" TabIndex="100"></asp:Label>
                                                            </span>
                                                        </h3>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="panel panel-default" runat="server" id="Div_Pagamento">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Informações sobre Pagamento</b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="form-group row">
                                        <div class="col-lg-12">
                                            <uc1:MensagemPagina runat="server" ID="MensagemLancamento" />
                                        </div>

                                        <div runat="server" id="Div_CadastroPagamento">

                                            <div class="col-lg-2" runat="server" id="Div_dtPagamento_Info_Pag">
                                                <div class="form-group">
                                                    <label>Data</label>
                                                    <asp:TextBox ID="txtdtPagamento_Info_Pag" class="form-control" runat="server" type="date"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <label>Valor Pago </label>
                                                <div class="input-group">
                                                    <asp:TextBox ID="txtnValorPagamento_Info_Pag" class="form-control" runat="server" aria-describedby="sMoedaOrigem25" Style="z-index: 0;"></asp:TextBox>
                                                    <span class="input-group-addon" id="sMoedaOrigem25" runat="server"></span>
                                                </div>
                                            </div>

                                            <%--<div class="col-lg-3" runat="server" id="Div_idConta_Info_Pag">
                                                <div class="form-group">
                                                    <label>Contrato de Cambio</label>
                                                    <asp:DropDownList ID="ddlidConta_Info_Pag" class="form-control  Caixa_Selecao" runat="server" attrname="idContasPagar"></asp:DropDownList>
                                                </div>
                                            </div>--%>

                                            <div class="col-lg-3" runat="server" id="Div_idConta_Info_Pag">
                                                <div class="form-group">
                                                    <label>Contrato de Cambio</label>
                                                    <asp:TextBox ID="txtsContratoCambio" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-3" runat="server" id="Div_idFormaPagamento_Info_Pag">
                                                <div class="form-group">
                                                    <label>Corretora</label>
                                                    <asp:TextBox ID="txtsCorretora" class="form-control" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <%--<div class="col-lg-3" runat="server" id="Div_idFormaPagamento_Info_Pag">
                                                <div class="form-group">
                                                    <label>Corretora</label>
                                                    <asp:DropDownList ID="ddlidFormaPagamento_Info_Pag" class="form-control  Caixa_Selecao" runat="server" attrname="idContasPagar"></asp:DropDownList>
                                                </div>
                                            </div>--%>

                                            <div class="col-lg-2" runat="server" id="Div_BotaoIncluirPag">
                                                <div class="form-group">
                                                    <br />
                                                    <asp:Button ID="cmdPagamento_Incluir" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdPagamento_Incluir_Click" />
                                                    <asp:Button ID="BtnEdicaoPagamento" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Salvar" OnClick="BtnEdicaoPagamento_Click" />
                                                </div>
                                            </div>
                                            <br />
                                            <asp:HiddenField ID="hddPagamento_idLinha" runat="server" />
                                            <asp:HiddenField ID="hddGarantia_idLinhaArquivo" runat="server" />
                                            <asp:HiddenField ID="hddValorEnvio" runat="server" />
                                            <asp:HiddenField ID="hddValorEnvioReal" runat="server" />

                                        </div>

                                        <div class="col-lg-12">
                                            <asp:GridView ID="dtgPagamento" class="table table-striped table-bordered table-hover table-condensed"
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                                OnRowDeleting="dtgPagamento_RowDeleting" OnRowDataBound="dtgPagamento_RowDataBound" OnRowCommand="dtgPagamento_RowCommand">

                                                <%--
                                                    ========================
                                                    =========índice=========
                                                    ========================
                                                    N° Célula ------- Info
                                                    ========================
                                                    0 --------------- idLinha
                                                    1 --------------- Nº Pagamento
                                                    2 --------------- Data do Pagamento
                                                    3 --------------- Valor Pago
                                                    4 --------------- Valor Multa
                                                    5 --------------- Valor Juros
                                                    6 --------------- Valor Desconto
                                                    7 --------------- Valor Total
                                                    8 --------------- Conta
                                                    9 --------------- Forma de Pagamento
                                                    ========================
                                                --%>
                                                <Columns>

                                                    <asp:BoundField DataField="idlinha" HeaderText="idLinha">
                                                        <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="nNumeroParcela_Info_Pag" HeaderText="Nº Pagamento">
                                                        <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="dtPagamento_Info_Pag" HeaderText="Data do Pagamento">
                                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:TemplateField HeaderText="Valor Pago">
                                                        <ItemTemplate>
                                                            <div style="display: flex;">
                                                                <asp:Label ID="nValorPagamento_Info_Pag" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nValorPagamento_Info_Pag") %>'></asp:Label>
                                                                <span id="sMoedaOrigem26" runat="server" style="margin-left: 10px;"></span>
                                                            </div>
                                                        </ItemTemplate>
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="5%" />
                                                    </asp:TemplateField>
                                                    <asp:BoundField DataField="sContratoCambio" HeaderText="Contrato de Cambio">
                                                        <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>
                                                    <asp:BoundField DataField="sCorretora" HeaderText="Corretora">
                                                        <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:TemplateField HeaderText="">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkPagamento_Editar" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip="Editar" CssClass="btn btn-small" TabIndex="100" CommandName="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="lnkPagamento_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip="Efetuar Upload de Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="lnkPagamento_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip='<%# Bind("sNomeArquivo", "Download do Arquivo: {0}") %>' CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                            <asp:LinkButton ID="lnkPagamento_Excluir" CommandArgument='<%# Bind("idLinha") %>' runat="server" CssClass="btn btn-small" TabIndex="100" ToolTip="Apagar Registro" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>

                                                        </ItemTemplate>
                                                        <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="Pagamento_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip='<%# Bind("sNomeArquivo", "Download do Arquivo: {0}") %>' CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:TemplateField>
                                                </Columns>
                                            </asp:GridView>
                                            <asp:HiddenField ID="hddValorPagar" runat="server" />
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="panel panel-default" runat="server" id="DIV13">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>Garantia</b></h3>
                                </div>
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina4" />
                                </div>
                                <div class="panel-body">
                                    <div class="form-stacked row">
                                        <div class="col-lg-12 table-responsive">
                                            <div class="row" runat="server" id="Div_Garantia">
                                                <div class="col-lg-12">
                                                    <div class="row">
                                                        <div class="col-lg-2">
                                                            <div class="form-group">
                                                                <label>Garantia Estendida</label>
                                                                <asp:DropDownList ID="ddlsGarantia" runat="server" class="form-control">
                                                                    <asp:ListItem Value="0" Text="Garantia Estendida" />
                                                                    <asp:ListItem Value="1" Text="Sim" />
                                                                    <asp:ListItem Value="2" Text="Não" />
                                                                </asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-3" runat="server" id="Div11">
                                                            <div class="form-group">
                                                                <label>Descrição Garantia</label>
                                                                <asp:TextBox ID="txtsDscGarantia" class="form-control " runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="Div9">
                                                            <div class="form-group">
                                                                <label>Envio Remessa</label>
                                                                <asp:TextBox ID="txtsEnvioRemesa" class="form-control " runat="server"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2">
                                                            <div class="form-group">
                                                                <label>Vencimento Garantia</label>
                                                                <asp:TextBox ID="txtdtGarantia" class="form-control" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2" runat="server" id="Div12">
                                                            <label>Valor Garantia</label>
                                                            <div class="input-group">
                                                                <asp:TextBox ID="txtnValorGarantia" class="form-control " runat="server" aria-describedby="sMoedaOrigem24" Style="z-index: 0;"></asp:TextBox>
                                                                <span class="input-group-addon" id="sMoedaOrigem24" runat="server"></span>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-1 form-group">
                                                            <label>&nbsp;</label>
                                                            <asp:Button ID="cmdGarantia" runat="server" CssClass="btn btn-info form-control" OnClick="cmdGarantia_Click" Text="Adicionar" />
                                                        </div>
                                                        <asp:HiddenField ID="hddGarantia_idLinha" runat="server" />
                                                        <asp:HiddenField ID="hddMoeda" runat="server" />
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="table-responsive col-lg-12" runat="server" id="DIV10">
                                                <div class="row">
                                                    <asp:GridView ID="gv_Garantia" class="table table-striped table-bordered table-hover table-condensed"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" DataKeyNames="idLinha" OnRowDeleting="gv_Garantia_RowDeleting"
                                                        OnRowDataBound="gv_Garantia_RowDataBound" OnRowCommand="gv_Garantia_RowCommand">
                                                        <Columns>
                                                            <asp:BoundField DataField="idlinha" HeaderText="Ordem">
                                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="sGarantia" HeaderText="Garantia Estendida">
                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="sDscGarantia" HeaderText="Descrição Garantia">
                                                                <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="sEnvioRemesa" HeaderText="Envio Remessa">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:BoundField DataField="dtGarantia" HeaderText="Vencimento Garantia">
                                                                <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>
                                                            <asp:TemplateField HeaderText="Valor Garantia">
                                                                <ItemTemplate>
                                                                    <div style="display: flex;">
                                                                        <asp:Label ID="nValorGarantia" runat="server" ClientIDMode="Static" MaxLength="20" Text='<%# Bind("nValorGarantia") %>'></asp:Label>
                                                                        <span id="sMoedaOrigem7" runat="server" style="margin-left: 10px;"></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="15%" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="lnkGarantia_UpLoad" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip="Efetuar Upload de Arquivo" CssClass="btn btn-small" TabIndex="100" CommandName="Upload_Arquivo"><i class="fa-upload fa"></i></asp:LinkButton>
                                                                    <asp:LinkButton ID="lnkGarantia_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip='<%# Bind("sNomeArquivo", "Download do Arquivo: {0}") %>' CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                                    <asp:LinkButton ID="lnkEnvio_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" ToolTip="Apagar Registro" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>
                                                            <asp:TemplateField HeaderText="">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton ID="Garantia_Download" CommandArgument='<%# Bind("idLinha") %>' runat="server" ToolTip='<%# Bind("sNomeArquivo", "Download do Arquivo: {0}") %>' CssClass="btn btn-small" TabIndex="100" CommandName="Download_Arquivo"><i class="fa-file fa"></i></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <fieldset class="form-stacked actions">
                                <asp:Button ID="Resultados_cmdGravarPedido" class="btn  btn-lg btn-success" runat="server" Text="Salvar" ValidationGroup="Pedido" />
                                <asp:Button ID="Resultados_cmdEditar" class="btn  btn-lg  btn-danger" runat="server" Text="Editar" ValidationGroup="Pedido" />
                                <asp:Button ID="Resultados_cmdDuplicar" class="btn  btn-lg  btn-info" runat="server" Text="Duplicar" ValidationGroup="Pedido" />
                                <a href="javascript:history.back()" class="btn btn-lg btn-warning" title="Voltar">Voltar</a>
                                <%--<input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel3" title="Cancelar" onclick="history.go(-1)" ValidationGroup="Pedido" >--%>
                            </fieldset>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico-taba">
            <br />
            <div class="panel panel-default" runat="server" id="DIV_historico">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Histórico</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">
                        <div class="col-lg-12 table-responsive">
                            <asp:GridView
                                ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" ShowFooter="False">
                                <Columns>
                                    <asp:BoundField DataField="dtLog" HeaderText="Data">
                                        <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                        <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sTipoAcao" HeaderText="Tipo">
                                        <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sAcao" HeaderText="Ação" HtmlEncode="false">
                                        <ItemStyle Width="68%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="arquivoMorto" aria-labelledby="arquivoMorto-tab">
            <br />
            <div class="panel panel-default" runat="server" id="DIV_ArquivoMorto">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Arquivados</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">
                        <div class="col-lg-12 table-responsive">
                            <asp:GridView ID="gv_ArquivoMorto" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                DataKeyNames="idArquivo" OnRowCommand="gv_ArquivoMorto_RowCommand" OnRowDataBound="gv_ArquivoMorto_RowDataBound">
                                <Columns>
                                    <asp:BoundField DataField="sDscTipoArquivo" HeaderText="Tipo do Arquivo">
                                        <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sNomeArquivo" HeaderText="Nome do Arquivo">
                                        <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscArquivo" HeaderText="Descrição">
                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscUsuarioEnvio" HeaderText="Enviado por">
                                        <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscUsuarioArquivou" HeaderText="Arquivado por">
                                        <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="dtExclusao" HeaderText="Data da Exclusão">
                                        <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:TemplateField HeaderText="">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkArquivoMorto_Download" runat="server" CommandName="Download_Arquivo"
                                                CommandArgument='<%# Eval("idArquivo") %>' data-toggle="tooltip" title="Download do Arquivo"
                                                CssClass="btn btn-small" TabIndex="100"><i class="fa fa-file"></i></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="acoes" aria-labelledby="acoes-tab">
            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 600px">
                <embed type="text/html" runat="server" id="frmacoes" width="800" height="500" />
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="arquivos-tab">
            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px">
                <embed type="text/html" runat="server" id="frmArquivos" width="800" height="500" />
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="tarefas">
            <div class="col-lg-12  embed-responsive embed-responsive-16by9" style="min-height: 2000px" runat="server" id="DIV_Tarefas">
                <embed type="text/html" runat="server" id="frmTarefas" />
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="documentos" aria-labelledby="documentos-tab">
            <br />
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h3 class="panel-title"><b>Documentos</b></h3>
                </div>

                <div class="panel-body">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Documentos" />

                    <div class="form-group" runat="server" id="DIV_Documentos">
                        <div class="btn-group">
                            <asp:CheckBox ID="PI" Text="Proforma Invoice" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="CI" Text="Commercial Invoice" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="PL" Text="Packing List" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="CO" Text="Certificate of Origin" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="IP" Text="Despachante" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="PC" Text="Pedido de Compras" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="PCI" Text="Pedido de Compras" runat="server" CssClass="btn btn-primary" />
                        </div>
                    </div>

                    <div class="form-group" runat="server" id="DIV_Documentos_Tipos">
                        <div class="btn-group">
                            <asp:CheckBox ID="cblsCEST" Text="CEST" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="cblsDecimal" Text="Precisão de Decimais" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="cblsEnvio" Text="Envio" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="cblsDrawback" Text="Drawback" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="cblsBancario" Text="Canal Bancário" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="cblCarimbos_Original" Text="Carimbo Original" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="cblCarimbos_Aprovado" Text="Carimbo Aprovado" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="cblCarimbos_CNPJ" Text="Carimbo CNPJ" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="cblAssinatura" Text="Assinatura" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="cblNome" Text="Nome, Data e Endereço" runat="server" CssClass="btn btn-primary" />
                            <asp:CheckBox ID="cblProdutoCliente" Text="Dados de Produtos do Cliente" runat="server" CssClass="btn btn-primary" />
                        </div>
                    </div>

                    <fieldset class="form-stacked actions" style="margin: 0;">
                        <asp:Button ID="ExportarPDF" CssClass="btn  btn-danger" runat="server" Text="PDF" OnClick="ExportarPDF_Click" />
                        <asp:Button ID="ExportarExcel" CssClass="btn  btn-success" runat="server" Text="Excel" OnClick="ExportarExcel_Click" />
                    </fieldset>
                </div>
            </div>

        </div>

        <div role="tabpanel" class="tab-pane fade" id="OPI" aria-labelledby="tab">
            <br />
            <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server" id="DIV_Aba_OPI">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>OPI/Envios</b></h3>
                        </div>
                        <div class="col-lg-12">
                            <div class="form-group">
                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaOPI" />
                            </div>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">
                                    <asp:Repeater ID="rptOPIs" runat="server" OnItemDataBound="rptOPIs_ItemDataBound">
                                        <ItemTemplate>
                                            <div class='panel panel-<%# Eval("sCor") %>' style="margin-bottom: 20px;">
                                                <div class="panel-heading">
                                                    <div class="row">
                                                        <div class="col-md-3">
                                                            <i class="fa fa-list-alt"></i>&nbsp<b>OPI:</b>
                                                            <asp:HyperLink ID="lnkOPI" runat="server" Target="_blank"
                                                                NavigateUrl='<%# Eval("idOPI", "/App/Paginas/WMS/OPI_Detalhe.aspx?id={0}") %>'
                                                                CssClass="btn btn-xs btn-link" Style="font-size: 14px; text-decoration: underline; color: inherit; font-weight: bold;"
                                                                Text='<%# Eval("idOPI") %>'>
                                                            </asp:HyperLink>
                                                            <i class="fa fa-external-link" style="font-size: 10px;"></i>
                                                        </div>
                                                        <div class="col-md-3">
                                                            <b>Data:</b> <%# Eval("dtOPI", "{0:dd/MM/yyyy}") %>
                                                        </div>
                                                        <div class="col-md-3">
                                                            <b>Descrição:</b> <%# Eval("sDscOPI") %>
                                                        </div>
                                                        <div class="col-md-3">
                                                            <b>Status:</b>
                                                            <span class='label label-<%# Eval("sCor") %>'><%# Eval("sStatus") %></span>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="panel-body">
                                                    <asp:Repeater ID="rptEnvios" runat="server" OnItemDataBound="rptEnvios_ItemDataBound">
                                                        <HeaderTemplate>
                                                            <table class="table table-bordered table-condensed table-hover" style="margin-bottom: 0;">
                                                                <thead>
                                                                    <tr class="active">
                                                                        <th style="width: 3%;"></th>
                                                                        <th style="width: 7%">Envio</th>
                                                                        <th style="width: 10%">Data</th>
                                                                        <th style="width: 20%">Embalagem</th>
                                                                        <th style="width: 10%">Peso Líq.</th>
                                                                        <th style="width: 10%">Peso Bruto</th>
                                                                        <th style="width: 15%">Medidas (CxLxA)</th>
                                                                        <th style="width: 5%; text-align: center;">Foto</th>
                                                                    </tr>
                                                                </thead>
                                                                <tbody>
                                                        </HeaderTemplate>
                                                        <ItemTemplate>
                                                            <tr>
                                                                <td align="center" style="vertical-align: middle;">
                                                                    <button type="button" class="btn btn-default btn-xs" data-toggle="collapse"
                                                                        data-target="#collapseProd_<%# Eval("idEnvioOPI") %>" aria-expanded="false">
                                                                        <i class="fa fa-plus"></i>
                                                                    </button>
                                                                </td>
                                                                <td align="center"><%# Eval("idEnvioOPI") %></td>
                                                                <td align="center"><%# Eval("dtEnvio") %></td>
                                                                <td>
                                                                    <asp:Literal ID="litEmbalagem" runat="server" Text='<%# Eval("sEmbalagemHTML") %>'></asp:Literal>
                                                                </td>
                                                                <td align="right"><%# Eval("nPesoLiquido", "{0:N2}") %></td>
                                                                <td align="right"><%# Eval("nPesoBruto", "{0:N2}") %></td>
                                                                <td align="center"><%# Eval("nComprimento", "{0:N0}") %>x<%# Eval("nLargura", "{0:N0}") %>x<%# Eval("nAltura", "{0:N0}") %></td>
                                                                <td align="center">
                                                                    <asp:LinkButton ID="cmdAbrirModalArquivo" runat="server"
                                                                        OnClick="AbrirModalArquivo_Click"
                                                                        CommandArgument='<%# Eval("idEnvioOPI") %>'
                                                                        Visible='<%# Eval("sTemArquivo").ToString() == "S" %>'
                                                                        CssClass="btn btn-default btn-xs" ToolTip="Ver Fotos do Envio">
                                                                        <i class="fa fa-camera"></i>
                                                                    </asp:LinkButton>
                                                                </td>
                                                            </tr>

                                                            <tr>
                                                                <td colspan="8" style="padding: 0; border-top: none;">
                                                                    <div id="collapseProd_<%# Eval("idEnvioOPI") %>" class="collapse" style="padding: 10px 20px; background-color: #fcfcfc; border-bottom: 2px solid #ddd;">
                                                                        <h5 style="margin-top: 0; color: #555;"><i class="fa fa-cube"></i>Produtos deste envio:</h5>

                                                                        <asp:GridView ID="dtgProdutosEnvio" runat="server"
                                                                            CssClass="table table-striped table-bordered table-condensed"
                                                                            AutoGenerateColumns="false" GridLines="None" ShowHeader="true"
                                                                            Style="margin-bottom: 0; background-color: #fff; font-size: 11px; width: 100%;">
                                                                            <Columns>
                                                                                <asp:BoundField DataField="sCodigo" HeaderText="Cód." ItemStyle-Width="10%" />
                                                                                <asp:BoundField DataField="sDscProduto" HeaderText="Produto" />
                                                                                <asp:BoundField DataField="sUnidade" HeaderText="UN" ItemStyle-Width="5%" />
                                                                                <asp:BoundField DataField="nQuantidade" HeaderText="Qtd" DataFormatString="{0:N4}" ItemStyle-Width="10%" />
                                                                            </Columns>
                                                                        </asp:GridView>
                                                                        <asp:HiddenField ID="hdnIdEnvioOPI" runat="server" Value='<%# Eval("idEnvioOPI") %>' />
                                                                    </div>
                                                                </td>
                                                            </tr>
                                                        </ItemTemplate>
                                                        <FooterTemplate>
                                                            </tbody>
                        </table>
                   
                                                        </FooterTemplate>
                                                    </asp:Repeater>

                                                    <asp:Label ID="lblSemEnvios" runat="server" Text="Nenhum envio registrado para esta OPI."
                                                        Visible="false" CssClass="text-muted" Font-Italic="true" Style="padding: 10px; display: block;"></asp:Label>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>


                                    <%--      <asp:GridView ID="dtgOPI" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                        Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" DataKeyNames="idProduto" OnSorting="dtgOPI_Sorting" OnRowDataBound="dtgOPI_RowDataBound">
                                        <Columns>

                                            <asp:BoundField DataField="idOPI" HeaderText="N° OPI" ItemStyle-Width="10%" />

                                            <asp:BoundField DataField="OPI" HeaderText="ID" ItemStyle-Width="10%" />

                                            <asp:HyperLinkField DataNavigateUrlFields="OPI"
                                                DataTextField="sCodigo" HeaderText="Código"
                                                DataNavigateUrlFormatString="/App/Paginas/WMS/OPI_Detalhe.aspx?id={0}">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:BoundField DataField="nOrdem" HeaderText="Ordem" ItemStyle-Width="2%" />

                                            <asp:HyperLinkField DataNavigateUrlFields="OPI"
                                                DataTextField="sDscProduto" HeaderText="Produto"
                                                DataNavigateUrlFormatString="/App/Paginas/WMS/OPI_Detalhe.aspx?id={0}">
                                                <HeaderStyle Width="20%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:BoundField DataField="sUnidade" HeaderText="UN" ItemStyle-Width="3%" />

                                            <asp:BoundField DataField="nQuantidade" HeaderText="Qtd Projeto" ItemStyle-Width="7%" />

                                        </Columns>
                                    </asp:GridView>--%>
                                </div>
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="depara" aria-labelledby="tab">
            <br />
            <asp:UpdatePanel ID="Upddepara" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server" id="div_depara">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>De / Para</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">

                                    <asp:GridView ID="gv_depara" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        Font-Names="Tahoma" Font-Overline="False" Font-Size="11px" DataKeyNames="idContador" OnRowDataBound="gv_depara_RowDataBound">
                                        <Columns>

                                            <asp:BoundField DataField="sCodigoProduto" HeaderText="Código">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Produto">
                                                <ItemTemplate>
                                                    <div>
                                                        <asp:LinkButton ID="lnkDetalhes" runat="server" CssClass="excel-details-btn" OnClientClick='<%# "toggleDetails(this); return false;" %>'>
                                                            <i class="fa fa-plus"></i>
                                                            <!-- Certifique-se de que sua versão do Font Awesome seja 4.x -->
                                                        </asp:LinkButton>

                                                        <asp:Label ID="lblProduto" runat="server" Text='<%# Eval("sDscProduto") %>'></asp:Label>
                                                        <span class="details-column">
                                                            <br />
                                                            <br />
                                                            <div class="col-lg-12">
                                                                <div class="form-group">
                                                                    <label>Composição do Produto:</label>
                                                                    <!-- PlaceHolder para adicionar o GridView de detalhes -->
                                                                    <asp:PlaceHolder ID="phDetalhesComposicao" runat="server"></asp:PlaceHolder>
                                                                </div>
                                                            </div>
                                                        </span>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sUnidade" HeaderText="UN">
                                                <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade PV" DataFormatString="{0:N2}">
                                                <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nQuantidadeE" HeaderText="Quantidade LME" DataFormatString="{0:N2}">
                                                <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nQuantidadeO" HeaderText="Quantidade LMO" DataFormatString="{0:N2}">
                                                <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nQuantidadeRO" HeaderText="Quantidade LMRO" DataFormatString="{0:N2}">
                                                <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nQuantidadeCO" HeaderText="Quantidade LMCO" DataFormatString="{0:N2}">
                                                <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nTotal" HeaderText="Total" DataFormatString="{0:N2}">
                                                <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="idProduto" Visible="false">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblidProduto" runat="server" Text='<%# Eval("idProduto") %>'></asp:Label>
                                                    <asp:Label ID="lblnQuantidade" runat="server" Text='<%# Eval("nQuantidade") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="STSO" aria-labelledby="tab">
            <br />
            <asp:UpdatePanel ID="UpdatePanel2" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server" id="div26">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>STSO</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina6" />
                                    </div>

                                    <div class="col-lg-4" runat="server" id="Div27">
                                        <div class="form-group">
                                            <label>Empresa</label>
                                            <asp:DropDownList ID="ddlidEmpresaSTSO" runat="server" class="form-control yes_no select Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidEmpresaSTSO_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-7" runat="server" id="DIV_DocumentoEmpresa">
                                        <div class="form-group">
                                            <label>Documentos Empresa</label>
                                            <asp:ListBox ID="ddlDocumentoEmpresa" runat="server" class="form-control yes_no select" SelectionMode="Multiple"></asp:ListBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-4" runat="server" id="DIV_ColaboradorSTSO">
                                        <div class="form-group">
                                            <label>Colaboradores</label>
                                            <asp:DropDownList ID="ddlidColaborador" runat="server" class="form-control yes_no select Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlidColaborador_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-7" runat="server" id="DIV_DocumentoColaboradores">
                                        <div class="form-group">
                                            <label>Documentos Colaboradores</label>
                                            <asp:ListBox ID="ddlDocumentoColaboradores" runat="server" class="form-control yes_no select" SelectionMode="Multiple"></asp:ListBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-1" runat="server" id="DIV_InserirDocumento">
                                        <div class="form-group">
                                            <label>&nbsp;</label>
                                            <br />
                                            <asp:Button ID="btnInserirDocumento" class="btn  btn-info" runat="server" OnClick="btnInserirDocumento_Click" Text="Inserir" />
                                        </div>
                                    </div>

                                    <div class="col-lg-12" runat="server" id="DIV39">
                                        <div class="col-lg-1 row" runat="server" id="DIV38">
                                            <div class="form-group">
                                                <asp:Button ID="btAprovar" class="btn  btn-success btAprovar" runat="server" Text="Aprovar" />
                                            </div>
                                        </div>
                                        <div class="col-lg-1 row" runat="server" id="DIV37">
                                            <div class="form-group">
                                                <asp:Button ID="btExcluir" class="btn  btn-danger btExcluir" runat="server" Text="Excluir" />
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-12 table-responsive">
                                        <asp:GridView ID="gvDocumentosSTSO" class="table table-striped table-bordered table-hover table-condensed"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            Font-Names="Tahoma" Font-Overline="False" DataKeyNames="idObjeto, sTipo" OnRowDataBound="gvDocumentosSTSO_RowDataBound" OnSorting="gvDocumentosSTSO_Sorting">
                                            <Columns>

                                                <asp:TemplateField>
                                                    <HeaderTemplate>
                                                        <div style="text-align: center;">
                                                            <asp:CheckBox ID="chkSTSO_Seleciona_Todos" runat="server" CssClass="Todos" />
                                                        </div>
                                                    </HeaderTemplate>
                                                    <ItemTemplate>
                                                        <asp:CheckBox ID="chkSTSO_Seleciona" runat="server" CssClass="Individual" />
                                                    </ItemTemplate>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="3%" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Nome">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_1_STSO" + Eval("idObjeto") %>'></asp:LinkButton>
                                                        <asp:Label runat="server" ID="sNome" Text='<%# Bind("sNome") %>'></asp:Label>

                                                        <%# NovaLinha(Eval("idObjeto"), "_1_STSO") %>

                                                        <asp:GridView ID="gvDocumentosSTSO_1" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            HeaderStyle-CssClass="gvChildHeader" CssClass="gvRow" Style="padding: 0; margin: 0" AlternatingRowStyle-CssClass="gvAltRow"
                                                            Font-Names="Tahoma" Font-Overline="False" DataKeyNames="idContador, idArquivo, idArquvoSTSO" OnRowDataBound="gvDocumentosSTSO_1_RowDataBound"
                                                            OnRowCommand="gvDocumentosSTSO_1_RowCommand">
                                                            <Columns>

                                                                <asp:TemplateField>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox ID="chkSTSO_Seleciona" runat="server" CssClass="ArquivoIndividual" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="3%" />
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="idContador" HeaderText="idContador">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idContador visible" />
                                                                    <HeaderStyle CssClass="visible" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="idArquivo" HeaderText="ID">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idArquivo visible" />
                                                                    <HeaderStyle CssClass="visible" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="idArquvoSTSO" HeaderText="idArquvoSTSO">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idArquvoSTSO visible" />
                                                                    <HeaderStyle CssClass="visible" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sEmpresaxColaborador" HeaderText="sEmpresaxColaborador">
                                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="visible" />
                                                                    <HeaderStyle CssClass="visible" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sNome" HeaderText="Nome">
                                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="visible" />
                                                                    <HeaderStyle CssClass="visible" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Nome Documento">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkDownload" runat="server" Text='<%# Eval("sDscsNomeArquivo") %>'
                                                                            CommandName="Download" CommandArgument='<%# Eval("idArquivo") %>' />
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="sDescricao" HeaderText="Descrição">
                                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Documento Aprovado">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="Aprovado" runat="server" Text='<%# Bind("sAprovacao") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>
                                                            </Columns>
                                                        </asp:GridView>

                                                    </ItemTemplate>
                                                    <ItemStyle Width="97%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>

                                    <div class="col-lg-12" runat="server" id="DIV28">
                                        <div class="form-group">
                                            <asp:Button ID="cmdSalvarSTSO" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvarSTSO_Click" />
                                        </div>
                                    </div>

                                    <div class="col-lg-2" runat="server" id="DIV29">
                                        <div class="form-group">
                                            <asp:Button ID="cmdGerarLinkSTSO" class="btn  btn-lg btn-info cmdGerarLinkSTSO" runat="server" Text="Gerar Link" />
                                        </div>
                                    </div>

                                    <div class="col-lg-12 table-responsive">
                                        <asp:GridView ID="gvDownloadSTSO" class="table table-striped table-bordered table-hover table-condensed"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            Font-Names="Tahoma" Font-Overline="False" DataKeyNames="idLink, sChave, sSenha" OnRowDataBound="gvDownloadSTSO_RowDataBound">
                                            <Columns>

                                                <asp:BoundField DataField="idLink" HeaderText="idLink">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idLink visible" />
                                                    <HeaderStyle CssClass="visible" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkVisualizar" runat="server" CssClass="btn btn-small VisualizarSTSO" CommandArgument='<%#Eval("idLink")%>' TabIndex="100" OnClick="lnkVisualizar_Click"><i class="fa fa-eye"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sChave" HeaderText="Chave">
                                                    <ItemStyle Width="72%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="sChave" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sSenha" HeaderText="Senha">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="sSenha" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="dtCriacao" HeaderText="Data Criação">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Enviar Email">
                                                    <ItemTemplate>
                                                        <asp:Button ID="btnEnviarEmail" runat="server" Text="Enviar Email" CommandName="EnviarEmail" CssClass="EnviarEmail" />
                                                    </ItemTemplate>
                                                    <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkDownloadSTSO" runat="server" CssClass="btn btn-small DownloadSTSO" TabIndex="100"><i class="fa fa-download"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemEnviarEmail" />
                                    </div>

                                    <div class="col-lg-2" runat="server" id="DIV33">
                                        <div class="form-group">
                                            <label>Nome do Destinatário</label>
                                            <asp:TextBox ID="txtsNomeEmail" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-4" runat="server" id="DIV34">
                                        <div class="form-group">
                                            <label>Email do Destinatário</label>
                                            <asp:TextBox ID="txtsEmail" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-5" runat="server" id="DIV44">
                                        <div class="form-group">
                                            <label>Email em Copia</label>
                                            <asp:TextBox ID="txtsEmailCopia" class="form-control" runat="server" placeholder="ex: tt@tecandtec.com.br;tt@tecandtec.com.br"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-1" runat="server" id="DIV35">
                                        <div class="form-group">
                                            <br />
                                            <label>&nbsp;</label>
                                            <asp:Button ID="cmdEnviarEmailSTSO" class="btn btn-lg btn-info" runat="server" Text="Enviar" OnClick="cmdEnviarEmailSTSO_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="div30">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Histórico</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <div class="col-lg-12 table-responsive">
                                        <asp:GridView
                                            ID="gvHistoricoSTSO" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" OnRowDataBound="gvHistoricoSTSO_RowDataBound">
                                            <Columns>
                                                <asp:BoundField DataField="dtLog" HeaderText="Data">
                                                    <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                                    <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sAcao" HeaderText="Ação" HtmlEncode="false">
                                                    <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sAcao" HeaderText="Ação" HtmlEncode="false">
                                                    <ItemStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="ART" aria-labelledby="tab">
            <br />
            <asp:UpdatePanel ID="UpdatePanel4" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server" id="div31">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>ART</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <div class="row">

                                        <div class="col-lg-2" runat="server" id="DIV32">
                                            <div class="form-group">
                                                <label>CNPJ do Contratante</label>
                                                <asp:TextBox ID="txtsCNPJContratante" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-4" runat="server" id="DIV48">
                                            <div class="form-group">
                                                <label>Razão Social Contratante</label>
                                                <asp:TextBox ID="txtsRazaoContratante" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-6" runat="server" id="DIV36">
                                            <div class="form-group">
                                                <label>Endereço do Contratante</label>
                                                <asp:TextBox ID="txtsEndereçoContratante" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV40">
                                            <div class="form-group">
                                                <label>N° do Contrato</label>
                                                <asp:TextBox ID="txtnContrato" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV41">
                                            <div class="form-group">
                                                <label>N° Controle TT</label>
                                                <asp:TextBox ID="txtnControle" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV42">
                                            <div class="form-group">
                                                <label>Valor do Contrato</label>
                                                <asp:TextBox ID="txtnValorContrato" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV43">
                                            <div class="form-group">
                                                <label>Data do Contrato</label>
                                                <asp:TextBox ID="txtdtContrato" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-2" runat="server" id="DIV67">
                                            <div class="form-group">
                                                <label>Prazo do Contrato</label>

                                                <div class="input-group">

                                                    <asp:TextBox ID="txtnTempoContratoART" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>

                                                    <span id="spPeriodo" class="input-group-addon" runat="server" style="font-weight: bold"></span>

                                                </div>

                                            </div>
                                        </div>


                                        <%--<div class="col-lg-2" runat="server" id="DIV44">
                                          <div class="form-group">
                                              <label>Escopo</label>
                                              <asp:TextBox ID="txtsEscopo" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                          </div>
                                       </div>--%>

                                        <div class="col-lg-12" runat="server" id="DIV_ClienteFinal">
                                            <div class="row">

                                                <div class="col-lg-2" runat="server" id="DIV46">
                                                    <div class="form-group">
                                                        <label>CNPJ do Cliente Final</label>
                                                        <asp:TextBox ID="txtsCNPJClienteFinal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-4" runat="server" id="DIV45">
                                                    <div class="form-group">
                                                        <label>Razão Social do Cliente Final</label>
                                                        <asp:TextBox ID="txtsRazaoClienteFinal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-6" runat="server" id="DIV47">
                                                    <div class="form-group">
                                                        <label>Endereço do Cliente Final</label>
                                                        <asp:TextBox ID="txtsEnderecoClienteFinal" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="Div50">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Escopo</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group row">

                                <div runat="server" id="div_CheckList_View" class="col-lg-12">

                                    <div runat="server" id="div_rptCheckList_View" class="row">
                                        <div class="col-lg-12">
                                            <div>

                                                <asp:Repeater runat="server" ID="rptCheckList_View" OnItemDataBound="rptCheckList_View_ItemDataBound">
                                                    <ItemTemplate>

                                                        <asp:GridView ID="gvCheckList_View" class="table table-striped table-bordered table-hover checklist"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idEscopo, idCategoria, sDscCategoria"
                                                            OnRowDataBound="gvCheckList_View_RowDataBound" OnRowCreated="gvCheckList_View_RowCreated">
                                                            <Columns>

                                                                <asp:BoundField DataField="idEscopo" HeaderText="Escopo">
                                                                    <HeaderStyle CssClass="visible escopo" />
                                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="visible escopo" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="idCategoria" HeaderText="Categoria">
                                                                    <HeaderStyle CssClass="visible categoria" />
                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="visible categoria" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sPergunta" HeaderText="Tópico">
                                                                    <ItemStyle Width="70%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="pergunta" />
                                                                </asp:BoundField>

                                                            </Columns>
                                                        </asp:GridView>

                                                    </ItemTemplate>
                                                </asp:Repeater>

                                            </div>
                                        </div>
                                    </div>

                                </div>

                            </div>
                        </div>
                    </div>

                    <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px">
                        <embed type="text/html" runat="server" id="frmArquivosART" width="800" height="500" />
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="Faturamento" aria-labelledby="Faturamento-tab">
            <br />
            <asp:UpdatePanel ID="Upd_Faturamento" runat="server">
                <ContentTemplate>

                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <div class="row d-flex">
                                 
                                <div class="col-lg-5">
                                    <h3 class="panel-title"><b>Faturamentos Parciais</b></h3>
                                </div>

                                <div class="col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon" style="font-weight: bold;">Total Pedido</span>
                                        <asp:TextBox ID="txtTotal_Pedido" class="form-control" runat="server" MaxLength="21" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon" style="font-weight: bold;">Total Faturado</span>
                                        <asp:TextBox ID="txtTotal_Faturado" class="form-control" runat="server" MaxLength="21" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="input-group">
                                        <span class="input-group-addon" style="font-weight: bold;">Saldo a Faturar</span>
                                        <asp:TextBox ID="txtSaldo_Faturar" class="form-control" runat="server" MaxLength="21" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                            </div>
                        </div>
                        <div class="panel-body">
                            <div class="col-lg-12">
                                <div class="row">

                                    <div runat="server" id="div_IncluirFaturamento">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Faturamento" />

                                        <div runat="server" id="div_FaturaServicos" class="row">
                                            <div class="col-lg-6 form-group">
                                                <uc1:DropDownList_Padrao runat="server" ID="ddlServico" Titulo="Serviço" AutoPostBack="true" OnSelectedIndexChanged="ddlServico_SelectedIndexChanged" Link="/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id=" SufixoLink="&stp=1" />
                                            </div>

                                            <div class="col-lg-6 form-group">
                                                <uc1:DropDownList_Padrao runat="server" ID="ddlEmpresa_Faturamento" Titulo="Empresa" Link="/App/Paginas/Manutencao/Empresas_Detalhe.aspx?id=" />
                                            </div>
                                        </div>

                                        <div class="row">

                                            <div class="col-lg-2 form-group">
                                                <uc1:TextBox_Padrao runat="server" ID="txtdtFaturamento" Titulo="Faturar em" Obrigatorio="true" Validacao="true" Modo="Data" />
                                            </div>

                                            <div runat="server" id="div_ddlTipoFaturamento" class="col-lg-3 form-group">
                                                <uc1:DropDownList_Padrao runat="server" ID="ddlTipoFaturamento" Titulo="Tipo de Faturamento" Obrigatorio="true" Validacao="true" AutoPostBack="true" OnSelectedIndexChanged="ddlTipoFaturamento_SelectedIndexChanged">
                                                    <asp:ListItem Value="0" Text="Selecione o Tipo" Selected="True"></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Inicio do Pedido"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="Durante (Conclusão Tarefas)"></asp:ListItem>
                                                    <asp:ListItem Value="3" Text="Durante (Medição de Obra)"></asp:ListItem>
                                                    <asp:ListItem Value="4" Text="Finalização do Pedido"></asp:ListItem>
                                                </uc1:DropDownList_Padrao>
                                            </div>

                                            <div class="col-lg-3 form-group" runat="server" id="div_DeptoFaturamento">
                                                <uc1:DropDownList_Padrao runat="server" ID="ddlDepartamento" Titulo="Departamento / Tarefa" Obrigatorio="true" Validacao="true" Link="/App/Paginas/Manutencao/Departamentos_Detalhe.aspx?id=" />
                                            </div>

                                            <div class="col-lg-7 form-group">
                                                <uc1:TextBox_Padrao runat="server" ID="txtsDescricao" Titulo="Descrição" Obrigatorio="true" Validacao="true" />
                                            </div>

                                            <div class="col-lg-6">
                                                <div class="col-lg-4 form-group padd-l-0">
                                                    <uc1:DropDownList_Padrao runat="server" ID="ddlBase_Porcentagem" Titulo="Base a Faturar" Obrigatorio="true" Validacao="true" AutoPostBack="true" OnSelectedIndexChanged="ddlBase_Porcentagem_SelectedIndexChanged">
                                                        <asp:ListItem Value="T" Text="Total do Pedido" Selected="True"></asp:ListItem>
                                                        <asp:ListItem Value="S" Text="Total dos Serviços"></asp:ListItem>
                                                        <asp:ListItem Value="SS" Text="Total do Serviço selecionado"></asp:ListItem>
                                                        <asp:ListItem Value="P" Text="Total dos Produtos"></asp:ListItem>
                                                    </uc1:DropDownList_Padrao>
                                                </div>
                                                <div class="col-lg-4">
                                                    <uc1:TextBox_Padrao runat="server" ID="txtPorcentagem_Faturamento" Titulo="% a Faturar" Obrigatorio="true" Validacao="true" MascaraDinamica="$money($input, ',', '.', 4)" Grupo_Simbolo="<b class='symbol'><i class='fa fa-percent'></i></b>" AutoPostBack="true" OnTextChanged="txtPorcentagem_Faturamento_TextChanged" />
                                                </div>
                                                <div class="col-lg-4 padd-r-0">
                                                    <uc1:TextBox_Padrao runat="server" ID="txtVlrFaturamento" Titulo="Valor a Faturar" Obrigatorio="true" Validacao="true" MascaraDinamica="$money($input, ',', '.', 2)" AutoPostBack="true" OnTextChanged="txtVlrFaturamento_TextChanged" />
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <uc1:TextBox_Padrao runat="server" ID="txtParcelas_Faturamento" Titulo="Nº Parcelas" Modo="Inteiro" MaxLength="3" />
                                            </div>

                                            <div class="col-lg-2">
                                                <label>&nbsp;</label>
                                                <asp:Button ID="btnIncluirFaturamento" class="form-control btn-info" runat="server" Text="Incluir" OnClick="btnIncluirFaturamento_Click" />
                                            </div>

                                        </div>
                                    </div>

                                    <div id="div_panel_gvFaturamento" class="panel panel-default">
                                        <div class="panel-heading">
                                            <div class="row" style="display: flex;">
                                                <div runat="server" id="lblPanel_gvFaturamento" class="col-lg-8">
                                                    <h3 class="panel-title"><b>Faturamentos Configurados</b></h3>
                                                </div>

                                                <%--<div class="col-lg-3">
                                                    <div class="input-group input-symbol">
                                                        <span class="input-group-addon" style="font-weight: bold;">Configurado</span>
                                                        <asp:TextBox ID="txtPorcentagemParcial" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                        <b class="symbol"><i class="fa fa-percent"></i></b>
                                                    </div>
                                                </div>--%>

                                                <div class="col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon" style="font-weight: bold;">Valor Configurado</span>
                                                        <asp:TextBox ID="txtTotalParcial" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div runat="server" id="div_txtSaldoParcial" class="col-lg-3">
                                                    <div class="input-group">
                                                        <span class="input-group-addon" style="font-weight: bold;">Valor a Configurar</span>
                                                        <asp:TextBox ID="txtSaldoParcial" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="padd-r">
                                                    <asp:LinkButton runat="server" ID="cmdExcluir_Faturamento" class="btn btn-md btn-danger" OnClick="cmdExcluir_Faturamento_Click" data-toggle="tooltip_top" title="Excluir Itens selecionados"><i class="fa fa-trash"></i></asp:LinkButton>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="panel-body">

                                            <div id="div_gvFaturamento" class="row">
                                                <div class="col-lg-12 table-responsive">
                                                    <asp:GridView ID="gvFaturamento" class="table table-striped table-bordered table-hover m-0"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="false" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                                        OnRowDataBound="gvFaturamento_RowDataBound" OnRowCommand="gvFaturamento_RowCommand">
                                                        <Columns>

                                                            <asp:BoundField DataField="idContador" HeaderText="Ordem">
                                                                <HeaderStyle CssClass="invisivel" />
                                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="invisivel" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="dtFaturamento" HeaderText="Faturar em">
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sDscTipo" HeaderText="Tipo">
                                                                <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sDscFaturamento" HeaderText="Descrição">
                                                                <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sDscServico" HeaderText="Serviço">
                                                                <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa">
                                                                <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="nValor" HeaderText="Valor" DataFormatString="{0:N2}" HtmlEncode="false">
                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="nPorcentagem" HeaderText="Porcentagem" DataFormatString="{0:N2}" HtmlEncode="false">
                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sFaturado" HeaderText="Faturado">
                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Ação">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton runat="server" ID="cmdImportarNota_Faturamento" class="btn btn-sm btn-info" Text="Importar Nota" CommandName="Importar" CommandArgument='<%# Eval("idContador") %>'></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField>
                                                                <ItemTemplate>
                                                                    <asp:LinkButton runat="server" ID="cmdEdita_Faturamento" CommandName="Editar" CommandArgument='<%# Eval("idContador") %>' data-toggle="tooltip" title="Editar"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="content-center" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <asp:CheckBox runat="server" ID="chkExcluir_Todos" ClientIDMode="Static" data-check-todos="Faturamento" CssClass="form-check form-check-danger form-check-x form-check-todos" />
                                                                </HeaderTemplate>
                                                                <ItemTemplate>
                                                                    <asp:CheckBox runat="server" ID="chkExcluir" ClientIDMode="Static" data-check-todos="Faturamento" CssClass="form-check form-check-danger form-check-x form-check-unico" />
                                                                </ItemTemplate>
                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="text-center" />
                                                            </asp:TemplateField>

                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>

                                        </div>
                                    </div>

                                    <fieldset runat="server" id="div_cmdSalvarFaturamento" class="form-stacked actions m-0">
                                        <asp:Button runat="server" ID="cmdSalvarFaturamento" class="btn btn-lg btn-success" Text="Salvar" OnClick="cmdSalvarFaturamento_Click" />
                                    </fieldset>

                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="div_FaturamentosEfetuados">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Faturamento Efetuado</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">
                                    <asp:GridView ID="gvFaturamentosEfetuados" class="table table-striped table-bordered table-hover "
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="chNFe, idEnvioOPI, sXML_Autorizado, idXML"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvFaturamentosEfetuados_RowDataBound">
                                        <Columns>

                                            <asp:TemplateField HeaderText="ID">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_XML" + Eval("idXML") %>'></asp:LinkButton>
                                                    <asp:Label runat="server" ID="lblidXML" Text='<%# Eval("idXML") %>' ClientIDMode="Static"></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="idEnvioOPI" HeaderText="ID Envio">
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="invisivel" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sdtEvento_Formatada" HeaderText="Data">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nSerieNF" HeaderText="Série">
                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nNumeroNF" HeaderText="Número">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="chNFe" HeaderText="Chave NFe">
                                                <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sStatus" HeaderText="Status">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="xMotivo" HeaderText="Observação">
                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Documentos">
                                                <ItemTemplate>

                                                    <asp:LinkButton runat="server" ID="cmdDownloadXML" class="btn btn-sm btn-primary" Text="XML" OnClick="cmdDownloadXML_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="cmdDownloadDANFE" class="btn btn-sm btn-warning" Text="DANFE" OnClick="cmdDownloadDANFE_Click"></asp:LinkButton>

                                                    <asp:LinkButton runat="server" ID="cmdGerarPDFEspelho" class="btn btn-sm btn-primary" Text="NFS-e Espelho" OnClick="cmdGerarPDFEspelho_Click"></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="cmdGerarTXT" class="btn btn-sm btn-info" Text="TXT" OnClick="cmdGerarTXT_Click"></asp:LinkButton>

                                                </ItemTemplate>
                                                <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="idFaturamento" HeaderText="Faturamento">
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle CssClass="invisivel" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nNumeroCartaCorrecao" HeaderText="Qtd Cartas">
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle CssClass="invisivel" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sCancelamento" HeaderText="Tem Cancelamento">
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle CssClass="invisivel" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sChaveNFE" HeaderText="ChaveNFe">
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle CssClass="invisivel" />
                                            </asp:BoundField>

                                            <asp:TemplateField>
                                                <ItemTemplate>

                                                    <%# NovaLinha(Eval("idXML"), "_XML") %>

                                                    <asp:GridView ID="gv_ContasReceber" class="gvRow table table-striped table-bordered table-hover padd-0 m-0" DataKeyNames="idContasReceber"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" ShowFooter="false" GridLines="None"
                                                        HeaderStyle-CssClass="gvChildHeader" AlternatingRowStyle-CssClass="gvAltRow" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small"
                                                        OnRowDataBound="gv_ContasReceber_RowDataBound">
                                                        <Columns>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                                                DataTextField="idContasReceber" HeaderText="ID"
                                                                DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:BoundField DataField="dtEmissao_ordem" HeaderText="Data Emissão" DataFormatString="{0:u}" SortExpression="dtEmissao_ordem" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="8%" />

                                                            <asp:BoundField DataField="dtVencimento_ordem" HeaderText="Data Vencimento" DataFormatString="{0:u}" SortExpression="dtVencimento_ordem" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="8%" />

                                                            <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                                                DataTextField="sDiasParaVencer" HeaderText="Dias Até Venc."
                                                                DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                                                DataTextField="sDocumento" HeaderText="Pedido"
                                                                DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                                                DataTextField="sQuantidadeParcela" HeaderText="Parcela"
                                                                DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                                                DataTextField="sCodigo" HeaderText="Nota Fiscal"
                                                                DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}"
                                                                DataTextField="nValorBruto" HeaderText="Valor Bruto"
                                                                DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="11%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:BoundField DataField="nValorOriginal" HeaderText="Valor Líquido" DataFormatString="{0:C2}" SortExpression="nValorOriginal" ItemStyle-HorizontalAlign="Center" HeaderStyle-Width="12%" />

                                                            <asp:BoundField DataField="nTotal" HeaderText="Valor Recebido" DataFormatString="{0:C2}">
                                                                <ItemStyle Width="11%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}"
                                                                DataTextField="nSaldo" HeaderText="Saldo em Aberto"
                                                                DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                                                DataTextField="sDscCategoriaReceber" HeaderText="Categoria"
                                                                DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                                                DataTextField="sStatus" HeaderText="Status"
                                                                DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                                                DataTextField="sDscFormaRecebimento" HeaderText="Forma de Recebimento"
                                                                DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:BoundField DataField="dtLiquidado" HeaderText="Data Liquidação">
                                                                <ItemStyle Width="1%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                                                DataTextField="sDscEmpresa" HeaderText="Empresa"
                                                                DataNavigateUrlFormatString="/App/Paginas/Adm/Financeiro/ContasReceber_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                        </Columns>
                                                    </asp:GridView>

                                                </ItemTemplate>
                                                <HeaderStyle CssClass="invisivel" />
                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="invisivel" />
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>

                    <fieldset runat="server" id="div_cmdFaturamento" class="form-stacked actions m-0">
                        <asp:HyperLink runat="server" ID="cmdFaturamento" class="btn btn-lg btn-primary" NavigateUrl="/App/Paginas/Adm/Faturamento/Faturamento.aspx?idPedido=" Target="_blank" Style="float: right;">Faturamento <i class="fa fa-arrow-right"></i></asp:HyperLink>
                    </fieldset>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="Cronograma" aria-labelledby="tab">
            <br />
            <asp:UpdatePanel ID="UpdatePanel5" runat="server" UpdateMode="Conditional">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server" id="div49">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Cronograma / OS</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12">
                                    <div class="row">

                                        <div class="col-lg-12">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPagina8" />
                                        </div>

                                        <div class="col-lg-3" runat="server" id="DIV_DataCronograma">
                                            <div class="form-group row">
                                                <div class="col-lg-6">
                                                    <label>Data Inicial</label>
                                                    <asp:TextBox ID="txtdtInicio" class="form-control" placeholder="Data Inícial" runat="server" MaxLength="10" data-mask="00/00/0000" type="datetime-local"></asp:TextBox>
                                                </div>
                                                <div class="col-lg-6">
                                                    <label>Data Final</label>
                                                    <asp:TextBox ID="txtdtFinal" class="form-control " runat="server" placeholder="Data Final" MaxLength="10" data-mask="00/00/0000" type="datetime-local"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-4" id="DIV51" runat="server">
                                            <div class="form-group">
                                                <label>Colaborador</label>
                                                <asp:DropDownList ID="ddlidColaboradorOS" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-4" runat="server" id="DIV52">
                                            <div class="form-group">
                                                <label>Observação</label>
                                                <asp:TextBox ID="txtsObservacaoOS" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-1" runat="server" id="DIV_btnInserir" style="padding: 0">
                                            <div class="col-lg-12" runat="server" id="div55">
                                                <label>&nbsp;</label>
                                                <asp:LinkButton ID="lnkInserir" CssClass="btn btn-primary form-control" Text="Inserir" OnClick="lnkInserir_Click" runat="server" />
                                            </div>
                                        </div>

                                        <div class="col-lg-12 table-responsive" runat="server" id="DIV56">
                                            <br />
                                            <asp:GridView ID="gvCronograma" class="table table-striped table-bordered table-hover table-condensed"
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                Font-Names="Tahoma" Font-Overline="False">
                                                <Columns>

                                                    <asp:BoundField DataField="dtInicial" HeaderText="Data Início">
                                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="dtFinal" HeaderText="Data Final">
                                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sDscUsuario" HeaderText="Colaborador">
                                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sDscEvento" HeaderText="Observação">
                                                        <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                </Columns>
                                            </asp:GridView>
                                        </div>

                                        <div class="col-lg-12" runat="server" id="DIV54">
                                            <div style="margin: 20px 0 50px;">
                                                <uc1:Calendario runat="server" ID="Calendario" />
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="lm" aria-labelledby=" -tab">
            <br />

            <asp:UpdatePanel ID="LM_upd" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server" id="LM_DIV_Geral">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Lista de Material</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 row">
                                    <div class="form-group">
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <uc1:MensagemPagina runat="server" ID="MensagemPagina2" />
                                            </div>
                                        </div>
                                        <div class="col-lg-4">
                                            <div class="btn-group">
                                                <label class="btn btn-primary" id="chkEngenharia" runat="server">
                                                    <asp:CheckBox ID="LM_chkEngenharia" Text=" Engenharia" AutoPostBack="true" OnCheckedChanged="LME_chk_CheckedChanged" runat="server" />
                                                </label>
                                                <label class="btn btn-primary" id="chkObras" runat="server">
                                                    <asp:CheckBox ID="LM_chkObras" Text=" Obras" AutoPostBack="true" OnCheckedChanged="LME_chk_CheckedChanged" runat="server" />
                                                </label>
                                                <label class="btn btn-primary" id="chkRetornoObras" runat="server">
                                                    <asp:CheckBox ID="LM_chkRetornoObras" Text=" Retorno Obras" AutoPostBack="true" OnCheckedChanged="LME_chk_CheckedChanged" runat="server" />
                                                </label>
                                                <label class="btn btn-primary" id="chkCompraEmObras" runat="server">
                                                    <asp:CheckBox ID="LM_chkCompraEmObras" Text=" Compra em Obras" AutoPostBack="true" OnCheckedChanged="LME_chk_CheckedChanged" runat="server" />
                                                </label>
                                            </div>
                                        </div>
                                        <div class="col-lg-5">
                                            <div class="form-group">
                                                <asp:DropDownList ID="LM_ddlLista" runat="server" class="form-control yes_no select" AutoPostBack="true" OnSelectedIndexChanged="LM_ddlLista_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <asp:Button ID="LM_cmdNova" class="btn  btn-info" runat="server" OnClick="LM_cmdNova_Click" Text="Nova LM" />
                                                <asp:Button ID="LM_cmdImportar" class="btn  btn-info" runat="server" OnClick="LM_cmdImportar_Click" Text="Importar" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="form-stacked row" runat="server" id="LM_divImportar">
                                <br />
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Importar Lista de Material</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="col-lg-12">
                                                    <div class="form-group row">
                                                        <div class="col-lg-6">
                                                            <div class="form-group">
                                                                <label>Número do Pedido</label>
                                                                <asp:TextBox ID="LM_Importar_txtNumeroPedido" class="form-control" runat="server" MaxLength="10"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-lg-12">
                                                            <div class="form-group">
                                                                <uc1:MensagemPagina runat="server" ID="LM_Importar_Mensagem" />

                                                            </div>
                                                        </div>
                                                        <div class="col-lg-12">
                                                            <div class="form-group">
                                                                <asp:Button ID="LM_cmdImportarLista" class="btn  btn-info" runat="server" Text="Importar" OnClick="LM_cmdImportarLista_Click" />
                                                                <asp:Button ID="LM_cmdImportarVoltar" class="btn  btn-warning" runat="server" Text="Cancelar" OnClick="LM_cmdImportarVoltar_Click" />
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

                    <asp:Panel ID="LM_pnExportacaoExcel" runat="server">
                        <div class="panel panel-default" runat="server" id="LM_divLista">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>
                                    <asp:Label ID="LM_lblTitulo" runat="server" Text="TITULO_LISTA"></asp:Label>
                                </b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="panel panel-default" runat="server" id="LM_DIV_Dados">
                                    <div class="panel-heading">
                                        <h3 class="panel-title"><b>Dados</b></h3>
                                    </div>
                                    <div class="panel-body row">
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <uc1:MensagemPagina runat="server" ID="LM_Mensagem" />
                                            </div>
                                        </div>
                                        <div class="col-lg-12">
                                            <div class="form-group row">
                                                <div class="col-lg-8">
                                                    <div class="form-group">
                                                        <label>Descrição</label>
                                                        <asp:TextBox ID="LM_txtsDscLM" class="form-control" runat="server" MaxLength="200"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <label>Observação</label>
                                                <asp:TextBox ID="LM_txtsObservacao" class="form-control" runat="server" Height="150px" MaxLength="800" TextMode="MultiLine"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                <asp:UpdatePanel ID="LM_updPanel_Itens" runat="server" UpdateMode="Conditional">
                                    <ContentTemplate>
                                        <div class="panel panel-default" runat="server" id="Div3">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Itens da LM</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="form-group row" id="LM_DIV_SelecaoItens" runat="server">
                                                    <div class="col-lg-12">
                                                        <div class="form-group row">
                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <label>Código</label>
                                                                    <asp:TextBox ID="LM_txtsCodigoProduto" class="form-control uppercase " runat="server" MaxLength="50" OnTextChanged="LM_txtsCodigoProduto_TextChanged" AutoPostBack="True" ValidationGroup="LM_Item"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-5">
                                                                <div class="form-group">
                                                                    <label>Produto</label>
                                                                    <asp:TextBox ID="LM_txtsDscProduto" class="form-control uppercase " runat="server" MaxLength="200" OnTextChanged="LM_txtsDscProduto_TextChanged" ValidationGroup="LM_Item"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-2">
                                                                <div class="form-group">
                                                                    <label>Unidade</label>
                                                                    <asp:DropDownList ID="LM_ddlsUnidade" runat="server" class="form-control" attrname="Unidade" ValidationGroup="LM_Item">
                                                                    </asp:DropDownList>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-3">
                                                                <div class="form-group">
                                                                    <label>Quantidade</label>
                                                                    <asp:TextBox ID="LM_txtnQuantidade" class="form-control " runat="server" MaxLength="10" ValidationGroup="LM_Item"></asp:TextBox>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>
                                                    <div class="col-lg-12">
                                                        <div class="form-group row">
                                                            <div class="col-lg-4">
                                                                <div class="form-group">
                                                                    <label>Agrupamento</label>
                                                                    <asp:TextBox ID="LM_txtsAgrupamento" class="form-control " runat="server" MaxLength="100" ValidationGroup="LM_Item"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-5">
                                                                <div class="form-group">
                                                                    <label>Observação</label>
                                                                    <asp:TextBox ID="LM_txtItens_sObservacao" class="form-control " runat="server" MaxLength="200" ValidationGroup="LM_Item"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                            <div class="col-lg-1" style="margin-top: 5px;">
                                                                <br />
                                                                <asp:Button ID="LM_cmdItens_Incluir" class="btn btn-info" runat="server" ValidationGroup="LM_Item" Text="Incluir" OnClick="LM_cmdItens_Incluir_Click" />
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-12">
                                                        <uc1:MensagemPagina runat="server" ID="LM_Mensagem_Itens" />
                                                    </div>
                                                    <asp:HiddenField ID="LM_hddPesquisaPor" runat="server" />
                                                    <asp:HiddenField ID="LM_hddidProduto" runat="server" />

                                                    <br />
                                                    <br />
                                                    <br />

                                                </div>

                                                <div class="form-group row">
                                                    <div class="col-lg-12 table-responsive">
                                                        <asp:GridView ID="LM_gvItens" class="table table-striped table-bordered table-hover table-condensed"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            Font-Names="Tahoma" Font-Overline="False" Font-Size="11px" DataKeyNames="idContador" OnSorting="LM_gvItens_Sorting" OnRowDeleting="LM_gvItens_RowDeleting"
                                                            OnRowDataBound="LM_gvItens_OnRowDataBound">
                                                            <Columns>

                                                                <asp:BoundField DataField="sDscCategoriaVendas" HeaderText="Agrupamento">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Agrupamento">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="LM_gvItens_txtsAgrupamento" class="form-control" MaxLength="200" runat="server" Text='<%# Bind("sDscCategoriaVendas") %>'></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>


                                                                <asp:TemplateField HeaderText="Ordem">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="LM_gvItens_txtnOrdem" class="form-control CaixaTextoMicro" MaxLength="4" runat="server" Text='<%# Bind("nOrdem") %>'></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <%--Agnes Partal * 19/07/2024---------%>
                                                                <asp:TemplateField HeaderText="Código">
                                                                    <ItemTemplate>
                                                                        <%# MostrarLink(Eval("idProduto"),Eval("sCodigoProduto"))%>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                                                </asp:TemplateField>
                                                                <%------------------------------------%>

                                                                <asp:TemplateField HeaderText="Produto">
                                                                    <ItemTemplate>
                                                                        <div>
                                                                            <asp:LinkButton ID="lnkDetalhes" runat="server" CssClass="excel-details-btn" OnClientClick='<%# "toggleDetails(this); return false;" %>'>
                                                                                <i class="fa fa-plus"></i>
                                                                                <!-- Certifique-se de que sua versão do Font Awesome seja 4.x -->
                                                                            </asp:LinkButton>

                                                                            <%--Agnes Partal * 15/07/2024---------%>
                                                                            <asp:LinkButton ID="lnkLMProdutoDetalhe" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                                OnClientClick='<%# "openModal(\"" + Eval("idProduto") + "\"); return false;" %>'
                                                                                OnMouseOver='<%# "mostraCard(this, \"" + Eval("idProduto") + "\", \"LM\");" %>'
                                                                                OnMouseOut='<%# "escondeCard( \"" + Eval("idProduto") + "\", \"LM\");" %>'
                                                                                CssClass="produto-detalhe-link"
                                                                                ClientIDMode="Static"
                                                                                data-idproduto='<%# Eval("idProduto") %>'
                                                                                data-tabela="LM" />

                                                                            <div id='<%# Eval("idProduto") + "_LM" %>' class="product-card" style="display: none;">
                                                                                <!--conteudo via script -->
                                                                            </div>
                                                                            <%------------------------------------%>
                                                                            <span class="details-column">
                                                                                <br />
                                                                                <br />
                                                                                <div class="col-lg-12">
                                                                                    <div class="form-group">
                                                                                        <label>Composição do Produto:</label>
                                                                                        <!-- PlaceHolder para adicionar o GridView de detalhes -->
                                                                                        <asp:PlaceHolder ID="phDetalhesComposicao" runat="server"></asp:PlaceHolder>
                                                                                    </div>
                                                                                </div>
                                                                            </span>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                </asp:TemplateField>
                                                                <asp:BoundField DataField="sEscopo" HeaderText="Escopo">
                                                                    <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Escopo">
                                                                    <ItemTemplate>
                                                                        <asp:DropDownList ID="LM_gvItens_ddlsEscopo" runat="server" class="form-control" SelectedValue='<%# Bind("sEscopo") %>'>
                                                                            <asp:ListItem Selected="True">TTL</asp:ListItem>
                                                                            <asp:ListItem>Cliente</asp:ListItem>
                                                                            <asp:ListItem>Projeto</asp:ListItem>
                                                                        </asp:DropDownList>

                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="sUnidade" HeaderText="UN">
                                                                    <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" DataFormatString="{0:N2}">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Quantidade">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="LM_gvItens_txtnQuantidade" class="form-control" MaxLength="10" runat="server" Text='<%# Bind("nQuantidade", "{0:N2}") %>'></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="nValorUnitario" HeaderText="Valor Unitário" DataFormatString="{0:N2}">
                                                                    <ItemStyle Width="7%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Valor Unitário">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="LM_gvItens_txtnValorUnitario" class="form-control" MaxLength="10" runat="server" Text='<%# Bind("nValorUnitario", "{0:N2}") %>'></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="nIPI" HeaderText="IPI" DataFormatString="{0:N2}">
                                                                    <ItemStyle Width="5%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="IPI">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="LM_gvItens_txtnIPI" class="form-control" MaxLength="10" runat="server" Text='<%# Bind("nIPI", "{0:N2}") %>'></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Total">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal" class="lblTotal" Text='<%# Eval("nTotal", "{0:N2}") %>' runat="server"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                    <ItemStyle Width="15%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Observação">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="LM_gvItens_txtsObservacao" class="form-control" MaxLength="200" runat="server" Text='<%# Bind("sObservacao") %>'></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:CommandField ButtonType="Link" DeleteText="<i class='fa-eraser fa'></i>" ItemStyle-HorizontalAlign="Center" ItemStyle-VerticalAlign="Middle" ItemStyle-Width="4%" ShowDeleteButton="True" />

                                                                <asp:BoundField DataField="idContador" HeaderText="Contador" DataFormatString="{0:N0}">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="idProduto" Visible="false">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblidProduto" runat="server" Text='<%# Eval("idProduto") %>'></asp:Label>
                                                                        <asp:Label ID="lblnQuantidade" runat="server" Text='<%# Eval("nQuantidade") %>'></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                            </Columns>
                                                        </asp:GridView>
                                                        <asp:HiddenField ID="LM_hddidLM" runat="server" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <!-- Modal Sugestão LM-->

                                        <div class="modal fade" id="modalSugestaoLM" tabindex="-1" role="dialog" aria-labelledby="modalSugestaoLabel" aria-hidden="true">
                                            <div class="modal-dialog" role="document">
                                                <div class="modal-content">
                                                    <div class="modal-header">
                                                        <h5 class="modal-title" id="modalSugestaoLabelLM"><b>Deseja adicionar também, os Itens Abaixo?</b></h5>
                                                        <p class="small">OBS: Para Remover 1 dos itens apenas, basta digitar "0" na quantidade</p>
                                                        <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                                            <span aria-hidden="true">&times;</span>
                                                        </button>
                                                    </div>
                                                    <div class="modal-body">

                                                        <asp:Repeater ID="rptItensSugestaoLM" runat="server" OnItemDataBound="rptItemSugeridoLM_ItemDataBound">
                                                            <ItemTemplate>
                                                                <div class="ibox-content">
                                                                    <div class="table-responsive">
                                                                        <table class="table shoping-cart-table">
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td width="90">
                                                                                        <div class="cart-product-imitation">
                                                                                            <asp:Image ID="imgProdutoPrincipal" runat="server" class="cart-product-imitation img" />
                                                                                        </div>

                                                                                        <asp:TextBox runat="server" ID="txtIdProdutoSugestao" Visible="false" Text='<%#Eval("idProdutoSugestao") %>'></asp:TextBox>

                                                                                    </td>
                                                                                    <td class="desc">
                                                                                        <h3>
                                                                                            <a class="text-navy"><%#Eval( "sDscProduto") %>
                                                                                            </a>
                                                                                        </h3>

                                                                                        <p class="small">
                                                                                            <%#Eval( "sDscProduto") %>
                                                                                        </p>
                                                                                    </td>
                                                                                    <td class="col-lg-3">
                                                                                        <label>Quantidade</label>
                                                                                        <asp:TextBox ID="txtQuantidade" runat="server" CssClass="form-control" Text='<%# Bind("nQuantidade", "{0:N2}") %>'></asp:TextBox>
                                                                                        <asp:HiddenField ID="hddsCodigo" runat="server" Value='<%# Eval("sCodigo") %>' />
                                                                                        <asp:HiddenField ID="hddsUnidade" runat="server" Value='<%# Eval("sUnidade") %>' />
                                                                                        <asp:HiddenField ID="hddsDscProduto" runat="server" Value='<%# Eval("sDscProduto") %>' />
                                                                                        <asp:HiddenField ID="hddsCategoriaVendas" runat="server" Value='<%# Eval("sDscCategoriaVendas") %>' />
                                                                                        <%--  <asp:LinkButton CssClass="btn btn-danger" Text="Remover" runat="server" OnClick="Remover_Click" CommandArgument='<%# Eval("idProdutoSugestao") %>'/>--%>
                                                                                    </td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>
                                                        </asp:Repeater>
                                                    </div>
                                                    <asp:HiddenField ID="hddidLM" runat="server" Value="0" />
                                                    <div class="modal-footer">
                                                        <asp:LinkButton CssClass="btn btn-success" Text="Adicionar" runat="server" OnClick="AdicionarLM_Click" />
                                                        <asp:LinkButton CssClass="btn btn-danger" Text="Cancelar" runat="server" OnClick="CancelarLM_Click" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="LM_cmdItens_Incluir" EventName="Click" />
                                        <asp:AsyncPostBackTrigger ControlID="LM_txtsCodigoProduto" EventName="TextChanged" />
                                    </Triggers>
                                </asp:UpdatePanel>

                                <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px" id="LM_DIV_Arquivos" runat="server">
                                    <embed type="text/html" runat="server" id="LM_frmArquivos" width="800" height="500" />
                                </div>

                                <div class="col-lg-12 form-group padd-0">
                                    <div class="col-lg-2 padd-0">
                                        <label>Exportar Excel com </label>
                                        <asp:CheckBox ID="cbDadosCliente" Text="Dados de Produtos do Cliente" runat="server" CssClass="btn btn-primary" />
                                        <asp:HiddenField runat="server" ID="hdd_dtLM" />
                                    </div>
                                </div>

                                <uc1:PainelAtualizacao runat="server" ID="LM_PainelAtualizacao" />

                                <fieldset class="form-stacked actions">
                                    <asp:Button ID="LM_cmdSalvarLM" CssClass="btn btn-lg  btn-success" runat="server" Text="Salvar LM" ValidationGroup="LM_Item" />
                                    <asp:Button ID="LM_cmdRecuperarSugestoes" CssClass="btn btn-lg  btn-primary" runat="server" OnClick="LM_cmdRecuperar_Click" Text="Recuperar Sugestões" ValidationGroup="LM_Item" />
                                    <asp:Button ID="LM_cmdEditaLM" CssClass="btn btn-lg  btn-info" runat="server" Text="Editar LM" OnClick="LM_cmdEditarLM_Click" ValidationGroup="LM_Item" />
                                    <asp:Button ID="LM_cmdSalvaLM" CssClass="btn btn-lg  btn-success" runat="server" Text="Salvar LM" OnClick="LM_cmdSalvaLM_Click" ValidationGroup="LM_Item" />
                                    <asp:Button ID="LM_cmdGerarOPI" CssClass="btn btn-lg  btn-warning" runat="server" Text="Gerar OPI" OnClick="LM_cmdGerarOPI_Click" ValidationGroup="LM_Item" />
                                    <asp:Button ID="LM_cmdExportarExcel" CssClass="btn btn-lg  btn-danger" runat="server" Text="Exportar Excel" OnClick="LM_cmdExportarExcel_Click" ValidationGroup="LM_Item" />
                                    <asp:Button ID="LM_cmdVoltar" CssClass="btn  btn-lg btn-info" runat="server" Text="Voltar" OnClick="LM_cmdVoltar_Click" ValidationGroup="LM_Item" />
                                </fieldset>
                            </div>
                        </div>
                    </asp:Panel>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>


        </div>

    </div>

    <div id="dialog-Salvar" class="modal" title="Salvar">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
        </p>
    </div>

    <div id="dialog-Editar" class="modal" title="Editar Pedido">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloEdiar" runat="server" Text="Confirma a Edição do Pedido?"></asp:Label>
        </p>
    </div>

    <div id="dialog-Duplicar" class="modal" title="Duplicar Pedido">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="lblTituloDuplicar" runat="server" Text="Confirma a Duplicação do Pedido?"></asp:Label>
        </p>
    </div>

    <div id="dialog_Apagar" class="modal" title="Apagar Arquivo STSO">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="Label2" runat="server" Text="Confirmar Apagar Arquivo STSO ?"></asp:Label>
        </p>
    </div>

    <div id="LM_dialog_Salvar" class="modal" title="Salvar LM">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="LM_lblTituloDialog" runat="server" Text="Deseja Salvar a Lista de Material?"></asp:Label>
        </p>
    </div>

    <div id="dialog_Aprovar" class="modal" title="Aprovar Documento">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="Label3" runat="server" Text="Deseja Aprovar o Documento?"></asp:Label>
        </p>
    </div>

    <div class="modal fade" id="Modal_Itens_Tabela" tabindex="-1" data-backdrop="static" role="dialog">
        <div class="modal-dialog modal-dialog-centered modal-largo" role="document">
            <div class="modal-content">
                <asp:UpdatePanel ID="updTituloModal" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" id="btnModalFechar" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>

                            <div class="modal-title-container">
                                <asp:Label runat="server" class="modal-title" ID="lblModalTitulo" Text="Tabela de Preço Itens" Font-Bold="true"></asp:Label>
                            </div>

                            <br />
                            <div class="col-lg-12">
                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaModal" />
                            </div>

                            <div class="row">
                                <div class="col-lg-9 row" style="margin-right: 80px;">
                                    <div runat="server" id="div_nItens_dtgItens" class="col-lg-2 form-group" style="margin-right: -110px; margin-top: 5px">
                                        <label>N° de Itens</label>
                                    </div>
                                    <div runat="server" id="div22" class="col-lg-1 form-group">
                                        <asp:DropDownList runat="server" ID="ddl_nItens" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddl_nItens_SelectedIndexChanged">
                                            <asp:ListItem Value="10" Text="10"></asp:ListItem>
                                            <asp:ListItem Value="15" Text="15"></asp:ListItem>
                                            <asp:ListItem Value="25" Text="25"></asp:ListItem>
                                            <asp:ListItem Value="50" Text="50"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                    <div runat="server" id="div23" class="col-lg-2 form-group" style="margin-right: -110px; margin-top: 5px">
                                        <label>Pesquisar</label>
                                    </div>
                                    <div runat="server" id="div24" class="col-lg-3 form-group">
                                        <asp:TextBox ID="txtsPesquisar" class="form-control toUpper" MaxLength="200" runat="server" AutoPostBack="true" OnTextChanged="sPesquisar_TextChanged"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2" runat="server" id="div20">
                                    <div class="pull-right">
                                        <asp:DropDownList ID="ddlOrdemTabela" runat="server" class="form-control yes_no select" AutoPostBack="true" OnSelectedIndexChanged="ddlOrdemTabela_SelectedIndexChanged">
                                            <asp:ListItem Value="0" Text="Ordenar Itens" />
                                            <asp:ListItem Value="1" Text="Código" />
                                            <asp:ListItem Value="2" Text="Produto" />
                                            <asp:ListItem Value="3" Text="UN" />
                                            <asp:ListItem Value="4" Text="Valor Unitário" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div runat="server" id="div21">
                                    <asp:LinkButton ID="btOrdemTabela" CssClass="btn btn-small" runat="server" OnClick="btOrdemTabela_Click" AutoPostBack="true"><i class="fa fa-sort"></i></asp:LinkButton>
                                </div>
                            </div>
                            <br />

                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <asp:GridView ID="gvItens" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" AllowPaging="true" AllowCustomPaging="true" PageSize="25" PagerStyle-CssClass="pages"
                                        Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvItens_RowDataBound" OnPageIndexChanging="gvItens_PageIndexChanging">
                                        <Columns>

                                            <asp:BoundField DataField="idItem" HeaderText="ID">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper visible" />
                                                <HeaderStyle CssClass="visible" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="idNCM" HeaderText="NCM">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper visible" />
                                                <HeaderStyle CssClass="visible" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sCodigo" HeaderText="Código">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscProduto" HeaderText="Produto">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="toUpper" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sUnidade" HeaderText="UN">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nTotal" HeaderText="Valor Unitário" DataFormatString="{0:N2}">
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="Tabela_Valor" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="UN Entrega">
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="Tabela_ddlsUnidadeEntrega" runat="server" class="form-control yes_no select"></asp:DropDownList>
                                                </ItemTemplate>
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Previsão Entrega">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="Tabela_txtdtPrevisao" runat="server" class="form-control" type="date" MaxLength="20" Text='<%# Bind("dtPrevisao") %>' AutoPostBack="true" OnTextChanged="Tabela_txtdtPrevisao_TextChanged"></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Quantidade">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="Tabela_txtnQuantidade" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("nQuantidade", "{0:N2}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="% ICMS">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="Tabela_txtnICMS" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("nICMS", "{0:N2}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="7%" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="% IPI">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="Tabela_txtnIPI" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("nIPI", "{0:N2}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="7%" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Valor Unitário">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="Tabela_txtnValorUnitario" runat="server" class="form-control" MaxLength="20" Text='<%# Bind("nValorUnitario", "{0:N2}") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" Width="10%" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="% Diferença">
                                                <ItemTemplate>
                                                    <asp:Label ID="nPorcentagem" runat="server" MaxLength="10" Text='<%# Bind("nPorcentagem", "{0:N2}") %>'></asp:Label>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" Width="3%" />
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>

                            <div class="col-lg-6">
                                <div class="form-group">
                                    <asp:Button ID="btnInserirItem" class="btn btn-sm btn-primary" runat="server" Text="Inserir Item" OnClick="btnInserirItem_Click" />
                                    <asp:Button ID="btnFechar" class="btn btn-sm btn-danger" data-dismiss="modal" aria-label="Fechar" runat="server" Text="Fechar" />
                                </div>
                            </div>

                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <div class="modal fade" id="Modal_Add_Produtos" tabindex="-1" data-backdrop="static" role="dialog">
        <div class="modal-dialog modal-dialog-centered modal-medio" role="document">
            <div class="modal-content">
                <asp:UpdatePanel ID="updProdutos" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" id="btnModalFecharProdutos" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>

                            <div class="modal-title-container">
                                <asp:Label runat="server" class="modal-title" ID="Label1" Text="Adicionar Novo Produto" Font-Bold="true"></asp:Label>
                            </div>

                            <br />

                            <div class="col-lg-12">
                                <uc1:MensagemPagina runat="server" ID="msgProduto" />
                            </div>

                            <div class="form-group row">
                                <div class="col-lg-12">

                                    <div class="col-lg-3">
                                        <div class="form-group">
                                            <label>Código</label>
                                            <asp:TextBox ID="txtAddCodigo" class="form-control uppercase " runat="server" MaxLength="50" ValidationGroup="Item"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <label>Descrição Produto</label>
                                            <asp:TextBox ID="txtAddDescricao" class="form-control uppercase " runat="server" MaxLength="200" ValidationGroup="Item"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-3" id="DIV19" runat="server">
                                        <div class="form-group">
                                            <label>Unidade</label>
                                            <asp:DropDownList ID="ddlAddUnidade" runat="server" class="form-control" attrname="Unidade" ValidationGroup="Item">
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                </div>
                            </div>

                            <div class="col-lg-6">
                                <div class="form-group">
                                    <asp:Button ID="btnAdicionar" class="btn btn-sm btn-primary" runat="server" Text="Adicionar Produto" OnClick="btnAdicionar_Click" />
                                    <asp:Button ID="Button3" class="btn btn-sm btn-danger" data-dismiss="modal" aria-label="Fechar" runat="server" Text="Fechar" />
                                </div>
                            </div>

                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <div class="modal fade" id="Modal_ArquivosSTSO" tabindex="-1" data-backdrop="static" role="dialog">
        <div class="modal-dialog modal-dialog-centered modal-largo" role="document">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <button type="button" id="btnArquivosFechar" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>

                            <div class="modal-title-container">
                                <asp:Label runat="server" class="modal-title" ID="Label4" Text="Arquivos" Font-Bold="true"></asp:Label>
                            </div>

                            <br />

                            <div class="col-lg-12">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina7" />
                            </div>

                            <div class="form-group row">

                                <div class="col-lg-12">
                                    <asp:GridView ID="gvArquivosSTSO" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        Font-Names="Tahoma" Font-Overline="False" DataKeyNames="idArquivo, idArquvoSTSO" OnSorting="gvArquivosSTSO_Sorting">
                                        <Columns>

                                            <asp:BoundField DataField="idArquivo" HeaderText="ID">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idArquivo visible" />
                                                <HeaderStyle CssClass="visible" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="idArquvoSTSO" HeaderText="idArquvoSTSO">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="idArquvoSTSO visible" />
                                                <HeaderStyle CssClass="visible" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscsArquivo" HeaderText="sDscsArquivo">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="sDscsArquivo visible" />
                                                <HeaderStyle CssClass="visible" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sEmpresaxColaborador" HeaderText="sEmpresaxColaborador">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="visible" />
                                                <HeaderStyle CssClass="visible" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sNome" HeaderText="Nome">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscsNomeArquivo" HeaderText="Nome Documento">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDescricao" HeaderText="Descrição">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <%--UploadArquivos_Modal-----------------------------------%>
    <asp:UpdatePanel ID="upd_EnvioArquivos" runat="server">
        <ContentTemplate>
            <div class="modal fade" id="UploadArquivos_Modal" tabindex="-1" role="dialog" aria-hidden="true">
                <div class="modal-dialog modal-dialog-centered" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title" id="exampleModalLongTitle">
                                <b>
                                    <asp:Label ID="lblEnviarArquivos_Titulo" runat="server" Text="TITULO"></asp:Label>
                                </b></h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_EnviarArquivo" />
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-stacked">
                                        <div class="col-lg-12" id="div4" runat="server">
                                            <div class="form-group">
                                                <label>Selecione o Arquivo</label>
                                                <asp:FileUpload ID="fu_EnviarArquivo" runat="server" />

                                            </div>
                                        </div>
                                        <div class="col-lg-12" id="div8" runat="server">
                                            <div class="form-group">
                                                <label>Selecione o Arquivo</label>
                                                <asp:FileUpload ID="fu_EnviarArquivoPagamento" runat="server" />

                                            </div>
                                        </div>
                                        <div class="col-lg-12" id="div14" runat="server">
                                            <div class="form-group">
                                                <label>Selecione o Arquivo</label>
                                                <asp:FileUpload ID="fu_EnviarArquivoGarantia" runat="server" />

                                            </div>
                                        </div>
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="lblEnviarArquivo_sDscArquivo" runat="server" Text="Observação"></asp:Label>
                                                </label>
                                                <asp:TextBox ID="txtEnviarArquivo_sDscArquivo" class="form-control" runat="server" MaxLength="300"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="cmdEnviarArquivos" CssClass="btn-success btn" runat="server" Text="Enviar" OnClick="cmdEnviarArquivos_Click" UseSubmitBehavior="False" />
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Close</button>
                        </div>
                    </div>
                </div>
                <asp:HiddenField ID="hddIdLinha" runat="server" />
                <asp:HiddenField ID="hddsBloco" runat="server" />
                <asp:HiddenField ID="hddEndereco" runat="server" />
            </div>

            <!-- Modal /-->
        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdEnviarArquivos" />
            <%--<asp:AsyncPostBackTrigger ControlID="cmdEnviarArquivos" EventName="Click" />--%>
        </Triggers>
    </asp:UpdatePanel>
    <%---------------------------------------------------------%>

    <%--Agnes Partal * 19/07/2024-------------------------------%>
    <div class="modal fade" id="produtoDetalheModal" tabindex="-1" role="dialog" aria-labelledby="detailsModalLabel" aria-hidden="true">
        <div class="modal-dialog" role="document">
            <div class="modal-content" style="display: table;">
                <div class="modal-header" id="modalHeader">
                    <%--<h5 class="modal-title" id="detailsModalLabel">Detalhes do Produto</h5>--%>
                    <%--<button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span>
                    </button>--%>
                </div>
                <div class="modal-body" id="modalBody">
                    <!-- conteudo via javascript-->
                </div>
                <%--<div class="modal-footer">
                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
                </div>--%>
            </div>
        </div>
    </div>
    <%----------------------------------------------------------%>

    <%--Higor Maestrello * 24/07/2024-------------------------%>
    <div class="modal fade" id="modalComposicao_1" data-backdrop="static">
        <div class="modal-dialog" style="width: 85%;">
            <div class="modal-content">
                <div class="modal-header">
                    <%--<button type="button" class="close" data-dismiss="modal">&times;</button>--%>
                    <asp:Button ID="Button1" runat="server" CssClass="close" Text="&times;" OnClick="cmdFecharModal_Click" />
                    <h4 runat="server" id="h4_titleModalComposicao_1" class="modal-title"></h4>
                </div>
                <div class="modal-body">

                    <div class="row">
                        <div class="col-lg-12 form-group">
                            <div>

                                <asp:GridView ID="gvServicos_Recursos_Composicao_1" class="table table-striped table-bordered table-hover tablePai"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvServicos_Recursos_Composicao_1_RowDataBound">
                                    <Columns>

                                        <asp:TemplateField HeaderText="ID">
                                            <ItemTemplate>
                                                <asp:Label runat="server" ID="lblidItem" Text='<%# Eval("idItem") %>' class="idItem" ClientIDMode="Static"></asp:Label>
                                                <asp:LinkButton ID="lnkComposicaoServico" runat="server" CssClass="excel-details-btn" OnClientClick='<%# "ComposicaoServico(this); return false;" %>'>
                                                <i class="fa fa-plus"></i>
                                                </asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="nOrdem" HeaderText="Item">
                                            <HeaderStyle CssClass="id" />
                                            <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id" />
                                        </asp:BoundField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                            DataTextField="sCodigo" HeaderText="Código"
                                            DataNavigateUrlFormatString="/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}" Target="_blank">
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="sCodigo" />
                                        </asp:HyperLinkField>

                                        <%--<asp:HyperLinkField DataNavigateUrlFields="idItem"
                                            DataTextField="sDscProduto" HeaderText="Descrição"
                                            DataNavigateUrlFormatString="/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}" Target="_blank">
                                            <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="sDscProduto" />
                                        </asp:HyperLinkField>--%>

                                        <asp:TemplateField HeaderText="Descrição">
                                            <ItemTemplate>
                                                <div>

                                                    <%--Agnes Partal * 10/07/2024---------%>
                                                    <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                        OnClientClick='<%# "openProductDetail(\"" + Eval("iditem") + "\"); return false;" %>'
                                                        OnMouseOver='<%# "mostraCard(this, \"" + Eval("iditem") + "\", \"ProdutoModal\");" %>'
                                                        OnMouseOut='<%# "escondeCard( \"" + Eval("iditem") + "\", \"ProdutoModal\");" %>'
                                                        CssClass="produto-detalhe-link sDscProduto"
                                                        ClientIDMode="Static"
                                                        data-idproduto='<%# Eval("iditem") %>'
                                                        data-tabela="ProdutoModal" />

                                                    <div id='<%# Eval("iditem") + "_ProdutoModal" %>' class="product-card" style="display: none;">
                                                        <!--conteudo via script -->
                                                    </div>
                                                    <%------------------------------------%>

                                                    <span class="details-column">
                                                        <br />
                                                        <br />
                                                        <div class="col-lg-12">
                                                            <div class="form-group">
                                                                <label>Composição do Produto:</label>
                                                                <!-- PlaceHolder para adicionar o GridView de detalhes -->
                                                                <asp:PlaceHolder ID="phDetalhesComposicao" runat="server"></asp:PlaceHolder>
                                                            </div>
                                                        </div>
                                                    </span>
                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>

                                        <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>

                    <div class="modal-footer">
                        <asp:Button ID="cmdFecharModal" runat="server" CssClass="btn btn-danger" Text="Fechar" OnClick="cmdFecharModal_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>
    <%--------------------------------------------------------%>

    <div class="modal fade" id="Modal_Arquivo_STSO" tabindex="-1" data-backdrop="static" role="dialog">
        <div class="modal-dialog modal-dialog-centered modal-STSO" role="document">
            <div class="modal-content">
                <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                    <ContentTemplate>
                        <div class="modal-header">
                            <%--<button type="button" id="btnFecharSTSO" class="close" data-dismiss="modal" aria-label="Fechar">
                                <span aria-hidden="true">&times;</span>
                            </button>--%>

                            <div class="modal-title-container">
                                <asp:Label runat="server" class="modal-title" ID="Label5" Text="Enviar Arquivos STSO" Font-Bold="true"></asp:Label>
                            </div>

                            <br />
                            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px">
                                <embed type="text/html" runat="server" id="frmArquivosSTSO" width="800" height="500" />
                            </div>

                            <div class="col-lg-6">
                                <div class="form-group">
                                    <asp:Button ID="Button4" class="btn btn-sm btn-danger" runat="server" Text="Fechar" OnClick="Button4_Click" />
                                </div>
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalArquivosEnvio" tabindex="-1" role="dialog" aria-labelledby="modalEnvioLabel" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog modal-lg" role="document" style="width: 80%;">
            <div class="modal-content">
                <div class="modal-header" style="background-color: #f5f5f5;">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                        <span aria-hidden="true">&times;</span>
                    </button>
                    <h4 class="modal-title" style="color: #009A22; font-weight: bold;">
                        <i class="fa fa-camera"></i>Galeria de Fotos do Envio
                    </h4>
                </div>

                <div class="modal-body" style="background-color: #e9e9e9; padding: 20px; max-height: 70vh; overflow-y: auto;">
                    <asp:UpdatePanel ID="updGaleria" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <div class="row">
                                <asp:Repeater ID="rptGaleriaFotos" runat="server">
                                    <ItemTemplate>
                                        <div class="col-md-3 col-sm-4 col-xs-6" style="margin-bottom: 20px;">
                                            <div class="thumbnail" style="border-radius: 4px; box-shadow: 0 1px 3px rgba(0,0,0,0.12);">

                                                <%--<a href='<%# ResolveUrl("~/App/Paginas/Arquivo.aspx?id=") + Eval("idArquivo") %>' target="_blank" title="Clique para baixar">--%>

                                                <img src='<%# ConverterImagemBase64(Eval("vbArquivo")) %>'
                                                    alt="Foto"
                                                    style="height: 180px; width: 100%; object-fit: cover; display: block;">
                                                </a>

                                                <div class="caption" style="padding: 9px; color: #333;">
                                                    <h5 style="margin: 0; font-weight: bold; font-size: 12px; white-space: nowrap; overflow: hidden; text-overflow: ellipsis;">
                                                        <%# Eval("sTitulo") %>
                                                    </h5>
                                                    <p style="margin: 5px 0 0; font-size: 11px; color: #777;">
                                                        <i class="fa fa-calendar"></i><%# Eval("dtUpload", "{0:dd/MM HH:mm}") %>
                                                    </p>
                                                </div>
                                            </div>
                                        </div>
                                    </ItemTemplate>
                                    <FooterTemplate>
                                        <asp:Label ID="lblVazio" runat="server" Visible='<%# rptGaleriaFotos.Items.Count == 0 %>'
                                            Text="<div class='alert alert-warning text-center'>Nenhuma foto encontrada nos volumes deste envio.</div>" />
                                    </FooterTemplate>
                                </asp:Repeater>

                                <asp:Panel ID="pnlSemFotos" runat="server" Visible="false" CssClass="col-md-12">
                                    <div class="alert alert-warning text-center">
                                        <i class="fa fa-exclamation-triangle"></i>Nenhuma foto encontrada para os volumes deste envio.
                               
                                    </div>
                                </asp:Panel>
                            </div>

                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade in" id="Modal_ImportarItens">
        <div class="modal-dialog modal-dialog-centered" role="document" style="width: 70%;">
            <div class="modal-content">

                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Fechar"><span aria-hidden="true">&times;</span></button>
                    <div class="modal-header-content">
                        <div class="modal-logo">
                            <img src="/App/img/LogoTT.png" alt="Logo TT" />
                        </div>
                        <div class="modal-title-container">
                            <label class="modal-title">Importar Itens</label>
                        </div>
                    </div>
                </div>

                <asp:UpdatePanel ID="UpdImportarItens" runat="server">
                    <ContentTemplate>

                        <div class="modal-body">
                            <div class="panel-body">
                                <div class="form-stacked row">
                                    <div class="col-lg-12 row">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_ImportarItens" />
                                    </div>

                                    <div class="col-lg-12 form-group">
                                        <asp:RadioButtonList runat="server" ID="rbTipo_ImportarItens" CssClass="btn-group" AutoPostBack="true" OnSelectedIndexChanged="rbTipo_ImportarItens_SelectedIndexChanged">
                                            <asp:ListItem class="btn btn-md btn-primary" Value="PEDIDO_COMPRAS"> Pedidos de Compras</asp:ListItem>
                                        </asp:RadioButtonList>
                                    </div>

                                    <asp:Panel ID="pnImportarItens" runat="server" class="col-lg-12">
                                        <div class="panel panel-primary">
                                            <div class="panel-body">
                                                <div class="table-responsive">
                                                    <asp:GridView ID="gvImportarItens" class="table table-striped table-bordered table-hover "
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="id"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowCommand="gvImportarItens_RowCommand">
                                                        <Columns>
                                                            <asp:ButtonField DataTextField="id" HeaderText="ID" Visible="false" />
                                                            <asp:ButtonField DataTextField="idObjeto" HeaderText="ID" ItemStyle-Width="6%" />
                                                            <asp:ButtonField DataTextField="sTipo" HeaderText="Tipo" ItemStyle-Width="8%" />
                                                            <asp:ButtonField DataTextField="sDscCliente" HeaderText="Cliente" ItemStyle-Width="20%" />
                                                            <asp:ButtonField DataTextField="sControleTT" HeaderText="Controle TT" ItemStyle-Width="10%" />
                                                            <asp:ButtonField DataTextField="sReferencia" HeaderText="Referência" ItemStyle-Width="20%" />
                                                            <asp:ButtonField DataTextField="sExtra" HeaderText="Tipo de Compra" ItemStyle-Width="10%" />
                                                        </Columns>
                                                    </asp:GridView>
                                                </div>
                                            </div>
                                        </div>
                                    </asp:Panel>
                                </div>
                            </div>
                        </div>

                    </ContentTemplate>
                </asp:UpdatePanel>

            </div>
        </div>
    </div>

    <div class="modal fade" id="modalImportarNota" tabindex="-2" role="dialog" data-backdrop="static" aria-labelledby="modalImportarNota" aria-hidden="true">
        <div class="modal-dialog modal-sm" role="document" style="width: 30%">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-header-content">
                        <div class="modal-logo">
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                        </div>

                        <div class="modal-title-container">
                            <h4 class="modal-title">Importar PDF da Nota</h4>
                        </div>
                    </div>
                    <div class="modal-body">
                        <div class="panel panel-default" runat="server" id="div_DetalheNegocio">

                            <div class="panel-body ">
                                <div class="col-lg-12 row" style="width: 100%; margin-left: 0px; padding: 0px;">
                                    <asp:UpdatePanel ID="uptPanelImportar" runat="server">
                                        <ContentTemplate>
                                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaModalImportar" style="margin-left: 0px !important" />
                                        </ContentTemplate>
                                    </asp:UpdatePanel>
                                </div>

                                <div class="row" runat="server" id="div_envioArquivo">
                                    <div class="col-lg-12">
                                        <div class="form-stacked">
                                            <div class="col-lg-12" id="div5" runat="server" style="text-align: center;">
                                                <div class="form-group" style="display: inline-block">
                                                    <div class="upload-container">
                                                        <div class="file-upload-label" id="uploadContainer">
                                                            <svg viewBox="0 0 640 512" height="1em">
                                                                <path
                                                                    d="M144 480C64.5 480 0 415.5 0 336c0-62.8 40.2-116.2 96.2-135.9c-.1-2.7-.2-5.4-.2-8.1c0-88.4 71.6-160 160-160c59.3 0 111 32.2 138.7 80.2C409.9 102 428.3 96 448 96c53 0 96 43 96 96c0 12.2-2.3 23.8-6.4 34.6C596 238.4 640 290.1 640 352c0 70.7-57.3 128-128 128H144zm79-217c-9.4 9.4-9.4 24.6 0 33.9s24.6 9.4 33.9 0l39-39V392c0 13.3 10.7 24 24 24s24-10.7 24-24V257.9l39 39c9.4 9.4 24.6 9.4 33.9 0s9.4-24.6 0-33.9l-80-80c-9.4-9.4-24.6-9.4-33.9 0l-80 80z">
                                                                </path>
                                                            </svg>
                                                            <p>Arraste e solte aqui o Arquivo</p>
                                                            <p>ou</p>
                                                            <span class="browse-button">Escolher Arquivo</span>
                                                            <asp:FileUpload ID="fu_ImportarNota" runat="server" Style="display: none;" />
                                                        </div>
                                                    </div>
                                                    <div class="file-name" id="fileName"></div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="modal-footer" style="text-align: left;">
                                    <div class="form-group">
                                        <div class="col-lg-12">
                                            <div class="row">
                                                <div class="col-lg-6" style="padding: 0px;">
                                                    <div class="form-group">
                                                        <asp:Button ID="btnImportarNota" CssClass="btn-success btn" runat="server" Text="Enviar" OnClick="btnImportarNota_Click" UseSubmitBehavior="False" />
                                                        <button type="button" id="btnFecharFooterImportar" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
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
    </div>

    <div id="hdd">
        <asp:HiddenField ID="hddidPedido" runat="server" />
        <asp:HiddenField ID="hddidTipo" runat="server" />
        <asp:HiddenField ID="hddIdItem" runat="server" />
        <asp:HiddenField ID="hddidCliente" runat="server" />
        <asp:HiddenField ID="hddididStatus" runat="server" />
        <asp:HiddenField ID="hddsAlteracaoItens" runat="server" />
        <asp:HiddenField ID="hddsModalComposicao" runat="server" />
        <asp:HiddenField ID="hddsDscComposicao" runat="server" />
        <asp:HiddenField ID="hddsComposicao" runat="server" />
        <asp:HiddenField ID="hddsDscComposicaoServico" runat="server" />
        <asp:HiddenField ID="hddContagemModal" runat="server" />
        <asp:HiddenField ID="hddFornecedor" runat="server" />
        <asp:HiddenField ID="hddidCliente_Produto" runat="server" />
        <asp:HiddenField ID="hddnNumeroPedido" runat="server" />
        <asp:HiddenField ID="hddPageIndex" runat="server" Value="S" />
        <asp:HiddenField ID="hddExcluirArquivo" runat="server" />
        <asp:HiddenField ID="hddsLink" runat="server" />
        <asp:HiddenField ID="hddDirecionaLink" runat="server" />
        <asp:HiddenField ID="hddidLink" runat="server" />
        <asp:HiddenField ID="hddsEmail" runat="server" />
        <asp:HiddenField ID="hddAprovarDoc" runat="server" />
        <asp:HiddenField ID="hddChecked" runat="server" />
        <asp:HiddenField ID="hddsidArquivo" runat="server" />
        <asp:HiddenField ID="hddDownloadSTSO" runat="server" />
        <asp:HiddenField ID="hddsChave" runat="server" />
        <asp:HiddenField ID="hddssSenha" runat="server" />
        <asp:HiddenField ID="hddsClienteFinal" runat="server" />

        <asp:HiddenField ID="hddidConceito" runat="server" />
        <asp:HiddenField ID="hddidGrupoPatrimonio" runat="server" />
        <asp:HiddenField ID="hddidCentroCusto" runat="server" />
        <asp:HiddenField ID="hddDupliacado" runat="server" />

        <asp:HiddenField ID="hddsPossuiProdutos" runat="server" Value="N" />
        <asp:HiddenField ID="hddsPossuiServicos" runat="server" Value="N" />

        <asp:HiddenField ID="hddsDuplicar" runat="server" Value="N" />
        
        <asp:HiddenField ID="hddMoeda_Simbolo" runat="server" Value="R$" />
    </div>

    <script>
        $(document).ready(function () {
            $(document).on('show.bs.collapse', '.collapse', function () {
                var id = $(this).attr('id');
                $('button[data-target="#' + id + '"]').find('i').removeClass('fa-plus').addClass('fa-minus');
            });
            $(document).on('hide.bs.collapse', '.collapse', function () {
                var id = $(this).attr('id');
                $('button[data-target="#' + id + '"]').find('i').removeClass('fa-minus').addClass('fa-plus');
            });
        });

        document.addEventListener('DOMContentLoaded', function () {
            var uploadContainer = document.getElementById('uploadContainer');
            var fileInput = document.getElementById('<%= fu_ImportarNota.ClientID %>');
            var fileNameDisplay = document.getElementById('fileName');

            uploadContainer.addEventListener('click', function () {
                fileInput.click();
            });

            uploadContainer.addEventListener('dragover', function (e) {
                e.preventDefault();
                e.stopPropagation();
                uploadContainer.classList.add('dragover');
            });

            uploadContainer.addEventListener('dragleave', function (e) {
                e.preventDefault();
                e.stopPropagation();
                uploadContainer.classList.remove('dragover');
            });

            uploadContainer.addEventListener('drop', function (e) {
                e.preventDefault();
                e.stopPropagation();
                uploadContainer.classList.remove('dragover');
                fileInput.files = e.dataTransfer.files;
                displayFileName(fileInput.files[0].name);
            });

            fileInput.addEventListener('change', function () {
                if (fileInput.files.length > 0) {
                    displayFileName(fileInput.files[0].name);
                }
            });

            function displayFileName(name) {
                fileNameDisplay.textContent = name;
            }
        });
    </script>

</asp:Content>
