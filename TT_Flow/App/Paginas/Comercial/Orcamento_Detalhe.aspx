<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Orcamento_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.Orcamento_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/FiltroPesquisa.ascx" TagPrefix="uc1" TagName="FiltroPesquisa" %>
<%@ Register Src="~/App/Controles/ExcelImportar.ascx" TagPrefix="uc1" TagName="ExcelImportar" %>
<%@ Register Src="~/App/Controles/DropDownList_Padrao.ascx" TagPrefix="uc1" TagName="DropDownList_Padrao" %>
<%@ Register Src="~/App/Controles/TextBox_Padrao.ascx" TagPrefix="uc1" TagName="TextBox_Padrao" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="RV" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        /* ------------------------------------------------ */

        /* Estilos gerais da Página */

        .id {
            display: none;
        }

        .invisivel {
            display: none;
        }

        /* ------------------------------------------------ */

        /* Estilos para alinhar a página */

        #page-wrapper {
            padding: 0;
        }

        .page-wrapper {
            padding: 0 30px 0 30px !important;
        }

        .page {
            width: 100%;
            height: 100vh;
        }

        /* ------------------------------------------------ */

        /* Estilos dos Panel's das Etapas */

        .pnGeral {
            display: flex;
            height: 100vh;
            flex-wrap: wrap;
            justify-content: center;
        }

        .painel {
            position: relative;
            width: 90%;
            max-height: 100%;
        }

        /* ------------------------------------------------ */

        /* Estilos das regiões de hover e dos botões laterais */

        .regiao {
            position: absolute;
            width: 5%;
            height: 100%;
        }

            .regiao:hover > .b_voltar {
                left: 25%;
                transition: 0.5s;
            }

            .regiao:hover > .b_avancar {
                right: 25%;
                transition: 0.5s;
            }

        .r_voltar {
            float: left;
            left: 0;
        }

        .r_avancar {
            float: right;
            right: 0;
        }

        .botoes {
            position: absolute;
            width: 80px;
            height: 40px;
            border: none;
            border-radius: 25px;
            background-color: forestgreen;
            color: white;
            top: 15%;
            z-index: 1;
        }

            .botoes:hover {
                cursor: pointer;
                color: white;
            }

        .b_voltar {
            float: left;
            left: -20px;
        }

        .b_avancar {
            float: right;
            right: -20px;
        }

        /* ------------------------------------------------ */

        /* Estilos para a barra de Progresso Geral */

        .progress {
            position: relative;
            width: 90%;
            margin: 0;
            height: 2.5%;
            border: solid 1px black;
        }

        /* ------------------------------------------------ */

        /* Estilos para os Stories e Barras de Progresso dos Stories */

        .stories {
            display: flex;
            width: 100%;
            height: 5%;
            justify-content: center;
            z-index: 1;
        }

        .cmdStories {
            width: 7%;
            height: 27.5%;
            background-color: rgb(230, 256, 230);
            border: solid 1px black;
            border-radius: 10px;
            margin: 5px;
            cursor: pointer;
        }

        .cmdStories_focus {
            border-color: greenyellow;
            box-shadow: inset 0 1px 1px greenyellow,0 0 8px greenyellow;
        }

        .cmdStories_error {
            border-color: red;
            box-shadow: inset 0 1px 1px red,0 0 8px red;
            background-color: lightpink;
        }

        .progress_stories {
            border-radius: 10px;
        }

        /* ------------------------------------------------ */

        /* Estilos para manipular os Panel's e adicionar as animações de Navegação */

        .aparece {
            transition: 0.5s;
            right: -5%;
            top: 0;
        }

        .desaparece1 {
            transition: 0.5s;
            right: 100%;
        }

        .desaparece2 {
            transition: 0.5s;
            right: -100%;
        }

        .desaparece_stories {
            transition: 0.5s;
            top: -100%;
        }

        /* ------------------------------------------------ */

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

        /* ------------------------------------------------ */

        /* Estilo específico para o campo de ID */

        .col-xs-2 {
            width: 12.5%;
        }

        /* ------------------------------------------------ */

        /* Estilo específico para focar no campo com Erro */

        .erro {
            border-color: rgba(200, 0, 0);
            box-shadow: inset 0 1px 1px rgba(200, 0, 0), 0 0 8px rgba(200, 0, 0);
        }

        /* ------------------------------------------------ */

        /* Estilo específico para alinhar ao Centro a CheckBox de Comparativo de Produtos */

        .comparativo {
            text-align: center;
        }

        /* ------------------------------------------------ */

        /* Estilo específico para destacar o campo de Ajuste de Comparativo de Serviços */

        .positivo {
            color: rgba(0, 200, 0);
        }

        .negativo {
            color: red;
        }

        /* ------------------------------------------------ */

        /* Estilo específico para exibir os Segmentos do Cliente */

        .segmento {
            display: flex;
            max-width: 100%;
            border-radius: 8px;
            color: white;
            margin: 1px;
            padding: 10px;
            font-weight: bold;
            font-family: tahoma;
            font-size: 12px;
        }

        /* ------------------------------------------------ */

        /* Estilo geral para exibir os Tipos de Serviços e Escopos */

        .tag {
            display: flex;
            max-width: 100%;
            border-radius: 8px;
            color: white;
            margin: 1px;
            padding: 10px;
            font-weight: bold;
            font-family: tahoma;
            font-size: 12px;
        }

        /* ------------------------------------------------ */

        /* Estilo específico para a CheckBox utilizada para selecionar todas as opções de uma coluna em Escopos */

        .opcaoTodos {
            display: flex;
            justify-content: start;
            flex-wrap: wrap;
        }

        /* ------------------------------------------------ */

        /* Estilos para o Card de informações */

        .card {
            box-shadow: 0px 2px 8px 2px rgba(0,0,0,0.3);
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
            padding: 2px;
            border-bottom: 1px dashed black;
        }

        .modal-title {
            text-shadow: 1px 2px 3px rgba(0, 0, 0, 0.2);
            font-size: 20px;
            font-weight: bold;
            margin: 0;
        }

        /* ------------------------------------------------ */

        /* Estilos para as Linhas de Composição */

        .collapsed-row {
            display: none;
            padding: 1px;
            margin: 1px;
        }

        .cabecalho_Composicao_0 {
            background-color: rgba(0, 100, 0) !important;
            color: white !important;
        }

        .cabecalho_Composicao_1 {
            background-color: rgba(0, 125, 0) !important;
            color: white !important;
        }

        .cabecalho_Composicao_2 {
            background-color: rgba(0, 150, 0) !important;
            color: white !important;
        }

        .cabecalho_Composicao_3 {
            background-color: rgba(0, 175, 0) !important;
            color: white !important;
        }

        .table .table {
            margin: 0 !important;
        }

        /* ------------------------------------------------ */

        /* Estilos para botões quando estiverem desabilitados */

        .desabilitado {
            cursor: not-allowed;
            color: black;
            background-color: #eee;
            box-shadow: inset 0 1px 1px rgba(0,0,0,.075);
            border-color: #ccc;
        }

            .desabilitado:hover {
                cursor: not-allowed;
                color: black;
                background-color: #eee;
                box-shadow: inset 0 1px 1px rgba(0,0,0,.075);
                border-color: #ccc;
            }

            .desabilitado:focus {
                cursor: not-allowed;
                color: black;
                background-color: #eee;
                box-shadow: inset 0 1px 1px rgba(0,0,0,.075);
                border-color: #ccc;
            }

            .desabilitado:active {
                cursor: not-allowed;
                color: black;
                background-color: #eee;
                box-shadow: inset 0 1px 1px rgba(0,0,0,.075);
                border-color: #ccc;
            }

                .desabilitado:active:focus {
                    cursor: not-allowed;
                    color: black;
                    background-color: #eee;
                    box-shadow: inset 0 1px 1px rgba(0,0,0,.075);
                    border-color: #ccc;
                }

                .desabilitado:active:hover {
                    cursor: not-allowed;
                    color: black;
                    background-color: #eee;
                    box-shadow: inset 0 1px 1px rgba(0,0,0,.075);
                    border-color: #ccc;
                }

        .btn[disabled] {
            opacity: 1;
            background-color: #eee;
            box-shadow: inset 0 1px 1px rgba(0,0,0,.075);
        }

        .btn-default.disabled:hover {
            background-color: #eee;
            border-color: #ccc;
        }

        /* ------------------------------------------------ */

        /* Estilos específicos para aplicar cores aos campos DropDownList utilizando 'Caixa_Selecao' */

        .chosenColor.success .chosen-container .chosen-single {
            background-color: #dff0d8 !important;
        }

        .chosenColor.danger .chosen-container .chosen-single {
            background-color: #f2dede !important;
        }

        .chosenColor.warning .chosen-container .chosen-single {
            background-color: #fcf8e3 !important;
        }

        .chosenColor.info .chosen-container .chosen-single {
            background-color: #d9edf7 !important;
        }

        /* ------------------------------------------------ */

        /* Estilos específicos para Mensagens */

        .exibeItens_msg {
            border: 1px solid #337ab7;
            border-radius: 5px;
            padding: 2.5px;
            cursor: pointer;
            text-align: center;
        }

        .div_tableRegraFiscal_msg {
            transition: 0.25s ease-in-out;
        }

        .importarProdutos_msg {
            width: 100%;
        }

            .importarProdutos_msg th {
                padding: 5px;
                text-align: center;
                border: 1px solid black;
            }

            .importarProdutos_msg td {
                padding: 5px;
                text-align: center;
                border: 1px solid black;
            }

        .faltaRegraFiscal_msg {
            width: 100%;
            border-collapse: collapse;
            table-layout: fixed;
        }

            .faltaRegraFiscal_msg td {
                width: 33%;
                padding: 5px;
                text-align: start;
            }

                .faltaRegraFiscal_msg td ul {
                    margin: 0;
                    padding: 0;
                    list-style-position: inside;
                }

                    .faltaRegraFiscal_msg td ul li {
                        white-space: nowrap;
                        overflow: hidden;
                        text-overflow: ellipsis;
                    }

        /* ------------------------------------------------ */
    </style>

    <div id="importarExcel" style="display: none;">
        <asp:FileUpload runat="server" ID="ImportarArquivo" accept=".xls, .xlsx" />
    </div>

    <asp:UpdatePanel ID="UpdDetalhe" runat="server" class="page">
        <ContentTemplate>

            <uc1:Manual runat="server" ID="manual" />

            <div runat="server" id="div_BreadCrumb" class="form-stacked row" style="padding: 0 30px 0 30px;">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Novo" />
                </div>
            </div>

            <div id="hdd">
                <asp:HiddenField runat="server" ID="hddidStatus" Value="0" />
                <asp:HiddenField runat="server" ID="hddidCliente" Value="0" />
                <asp:HiddenField runat="server" ID="hddidEmpresa" Value="0" />
                <asp:HiddenField runat="server" ID="hddidContato" Value="0" />
                <asp:HiddenField runat="server" ID="hddidEndereco_Fiscal" Value="0" />
                <asp:HiddenField runat="server" ID="hddidEndereco_Entrega" Value="0" />
                <asp:HiddenField runat="server" ID="hddProgresso" Value="|" />
                <asp:HiddenField runat="server" ID="hddsTipoCliente" />
                <asp:HiddenField runat="server" ID="hddidProduto" />
                <asp:HiddenField runat="server" ID="hddTipoProduto" />
                <asp:HiddenField runat="server" ID="hddsUnidadeProduto" />
                <asp:HiddenField runat="server" ID="hddidGrupoProduto" />
                <asp:HiddenField runat="server" ID="hddidFamiliaProduto" />
                <asp:HiddenField runat="server" ID="hddsDscGrupoProduto" />
                <asp:HiddenField runat="server" ID="hddsDscFamiliaProduto" />
                <asp:HiddenField runat="server" ID="hddAtualiza_IE_Parceiro" Value="0" />
                <asp:HiddenField runat="server" ID="hddidItemPai" Value="0|" />
                <asp:HiddenField runat="server" ID="hddidItemAvo" Value="0|" />
                <asp:HiddenField runat="server" ID="hddidItemBisavo" Value="0|" />
                <asp:HiddenField runat="server" ID="hddVincula_CRM" Value="0" />
                <asp:HiddenField runat="server" ID="hddidVendedor" Value="0" />
                <asp:HiddenField runat="server" ID="hddNumeroPedido" Value="0" />
                <asp:HiddenField runat="server" ID="hddOpcaoSelecionada" Value="" />
                <asp:HiddenField runat="server" ID="hddDtPedido" />
                <asp:HiddenField runat="server" ID="hddDtAtualizacaoPedido" />
                <asp:HiddenField runat="server" ID="hddsEdicao" Value="N" />
                <asp:HiddenField runat="server" ID="hddOpcaoSelecionada_Focus" />
                <asp:HiddenField runat="server" ID="hddUF_Entrega_SelectedValue" Value="0" />
                <asp:HiddenField runat="server" ID="hddUF_Fiscal_SelectedValue" Value="0" />
                <asp:HiddenField runat="server" ID="hddMunicipio_Entrega_SelectedValue" Value="0" />
                <asp:HiddenField runat="server" ID="hdd_ID_cmdSalvar" />
                <asp:HiddenField runat="server" ID="hddComparativoSistemas" Value="N" />
                <asp:HiddenField runat="server" ID="hddidTipoOrcamento" />
                <asp:HiddenField runat="server" ID="hddComposicao_Sistema" Value="|" />
                <asp:HiddenField runat="server" ID="hddnRevisao" Value="0" />
                <asp:HiddenField runat="server" ID="hddidFluxo" />
                <asp:HiddenField runat="server" ID="hddidCondicaoPagamento" />
                <asp:HiddenField runat="server" ID="hddidFormaEnvio" />
                <asp:HiddenField runat="server" ID="hddidTabela" />
                <asp:HiddenField runat="server" ID="hddidDestinoVenda" />
                <asp:HiddenField runat="server" ID="hddComparativoProdutos" Value="N" />
                <asp:HiddenField runat="server" ID="hddComparativos_Empreitada" Value="false" />
                <asp:HiddenField runat="server" ID="hddOcamento_Empreitada" Value="false" />
                <asp:HiddenField runat="server" ID="hddidTipoCliente" />
                <asp:HiddenField runat="server" ID="hddidSegmentos" Value="|" />
                <asp:HiddenField runat="server" ID="hddidTiposServicos" Value="|" />
                <asp:HiddenField runat="server" ID="hddsDscTiposServicos" Value="|" />
                <asp:HiddenField runat="server" ID="hddidEscopos" Value="|" />
                <asp:HiddenField runat="server" ID="hddsDscEscopos" Value="|" />
                <asp:HiddenField runat="server" ID="hddIncluirServicos" Value="[]" />
                <asp:HiddenField runat="server" ID="hddidIncluirServico" Value="0" />
                <asp:HiddenField runat="server" ID="hddMunicipio_Fiscal" />
                <asp:HiddenField runat="server" ID="hddMunicipio_Origem" />
                <asp:HiddenField runat="server" ID="hddMax_Material_Empreitada" Value="50" />
                <asp:HiddenField runat="server" ID="hddPedidoVinculado" Value="0" />
                <asp:HiddenField runat="server" ID="hddPedidoVinculado_TarefaFinalizada" Value="N" />
                <asp:HiddenField runat="server" ID="hddVincularProduto" />
                <asp:HiddenField runat="server" ID="hddUnificarProdutos" />
                <asp:HiddenField runat="server" ID="hddsDrawback" Value="false" />
                <asp:HiddenField runat="server" ID="hddsTipoDrawback" />
                <asp:HiddenField runat="server" ID="hddCondPgto_Alterada" Value="false" />
                <asp:HiddenField runat="server" ID="hddTipoSituacao_Parceiro" Value="0" />
                <asp:HiddenField runat="server" ID="hddNovaCondPgto_Pers_Dados" />
                <asp:HiddenField runat="server" ID="hddidInstalador" />
                <asp:HiddenField runat="server" ID="hddVincula_Cotacao" Value="0" />
                <asp:HiddenField runat="server" ID="hddMoeda" Value="0" />
                <asp:HiddenField runat="server" ID="hddMoeda_Simbolo" Value="R$" />
            </div>

            <div runat="server" id="div_MensagemPaginaGeral" class="col-lg-12">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaGeral" />
                </div>
            </div>

            <asp:Panel runat="server" ID="pnGeral" class="pnGeral">

                <div runat="server" id="div_divProgresso" class="progress">
                    <div id="divProgresso" runat="server" class="progress-bar progress-bar-success progress-bar-striped active" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%"></div>
                </div>

                <div runat="server" id="divStories" class="stories">
                    <a runat="server" id="painelCollapse1" class="cmdStories cmdStories_focus" data-toggle="tooltip" title="Cadastro Inicial">
                        <div id="divProgressoStories1" runat="server" class="progress-bar progress-bar-success progress_stories" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%"></div>
                    </a>
                    <a runat="server" id="painelCollapse2" class="cmdStories" data-toggle="tooltip" title="Informações do Orçamento">
                        <div id="divProgressoStories2" runat="server" class="progress-bar progress-bar-success progress_stories" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%"></div>
                    </a>
                    <a runat="server" id="painelCollapse3" class="cmdStories" data-toggle="tooltip" title="Serviços e Recursos">
                        <div id="divProgressoStories3" runat="server" class="progress-bar progress-bar-success progress_stories" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%"></div>
                    </a>
                    <a runat="server" id="painelCollapse4" class="cmdStories" data-toggle="tooltip" title="Seleção de Produtos">
                        <div id="divProgressoStories4" runat="server" class="progress-bar progress-bar-success progress_stories" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%"></div>
                    </a>
                    <a runat="server" id="painelCollapse5" class="cmdStories" data-toggle="tooltip" title="Visualização Geral e Confirmação">
                        <div id="divProgressoStories5" runat="server" class="progress-bar progress-bar-success progress_stories" role="progressbar" aria-valuenow="0" aria-valuemin="0" aria-valuemax="100" style="width: 0%"></div>
                    </a>
                </div>

                <div runat="server" id="div_Paineis" style="display: flex; height: 80%; width: 100%;">

                    <div runat="server" id="divRegiaoVoltar" class="regiao r_voltar">
                        <a runat="server" id="cmdVoltarEtapa" class="botoes b_voltar"><i style="position: absolute; left: 42.5%; top: 36%;" class="fa fa-chevron-left"></i></a>
                    </div>

                    <asp:Panel runat="server" ID="pn1" class="painel aparece">

                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <div style="padding: 0; width: 100%; display: flex;">
                                    <div style="padding: 0; width: 90%">
                                        <h3 class="panel-title"><b>Cadastro Inicial</b></h3>
                                    </div>
                                    <div style="display: flex; justify-content: right; padding: 0; width: 10%">
                                        <asp:LinkButton runat="server" ID="lnkAtualizaInicial" OnClick="cmdAtualiza_Click" class="form-control btn-info" Style="width: 22.5%" data-toggle="tooltip" title="Atualizar Informações do Orçamento"><i class="fa fa-refresh"></i></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <div class="panel-body">
                                <div class="row">

                                    <div class="col-lg-12">

                                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaInfoInicial" />

                                        <div runat="server" id="div_cliente" class="row">
                                            <div class="col-lg-10">
                                                <uc1:FiltroPesquisa runat="server" ID="FiltroPesquisaParceiros" />
                                            </div>
                                            <div class="col-lg-2">
                                                <label>&nbsp;</label>
                                                <asp:Button runat="server" ID="cmdSelecionarParceiro" class="form-control btn-info" Text="Selecionar Parceiro" OnClick="cmdSelecionarParceiro_Click"></asp:Button>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_Enderecos" class="row">
                                            <div class="col-lg-12" style="padding: 0;">

                                                <div class="col-lg-8 form-group">
                                                    <label>Endereço Fiscal</label>
                                                    <asp:DropDownList runat="server" ID="ddlEndereco" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlEndereco_SelectedIndexChanged"></asp:DropDownList>
                                                </div>
                                                <div runat="server" id="divs_End_Fiscal">
                                                    <div class="col-lg-2 form-group">
                                                        <label>UF Fiscal</label>
                                                        <asp:TextBox runat="server" ID="txtUF_Fiscal" class="form-control" placeholder="UF Fiscal" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                    <div class="col-lg-2 form-group">
                                                        <label>Município Fiscal</label>
                                                        <asp:TextBox runat="server" ID="txtMunicipioFiscal" class="form-control" placeholder="Município Fiscal" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                            </div>

                                            <div class="col-lg-12" style="padding: 0;">

                                                <div class="col-lg-8 form-group" runat="server" id="div_ddlEndereco_Entrega">
                                                    <label>Endereço de Entrega</label>
                                                    <div style="display: flex;">
                                                        <asp:DropDownList runat="server" ID="ddlEndereco_Entrega" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlEndereco_Entrega_SelectedIndexChanged"></asp:DropDownList>
                                                        <div class="col-lg-1" style="display: flex; padding: 0; justify-content: right;" runat="server" id="div_cmdNovoEndereco">
                                                            <div class="col-lg-6" style="padding: 0;">
                                                                <asp:LinkButton runat="server" ID="cmdNovoEndereco" class="form-control novoEndereco btn-info" data-toggle="tooltip" title="Novo Endereço"><i class="fa fa-plus"></i></asp:LinkButton>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div runat="server" id="divs_End_Entrega">
                                                    <div class="col-lg-2 form-group">
                                                        <label>UF de Entrega</label>
                                                        <asp:DropDownList runat="server" ID="ddlUF_Entrega" class="form-control" disabled=""></asp:DropDownList>
                                                    </div>
                                                    <div class="col-lg-2 form-group">
                                                        <label>Município de Entrega</label>
                                                        <asp:DropDownList runat="server" ID="ddlMunicipio_Entrega" class="form-control" disabled=""></asp:DropDownList>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>

                                        <div runat="server" id="div_IE" class="row">

                                            <div class="col-lg-3 form-group">
                                                <label>IE</label>
                                                <asp:TextBox runat="server" ID="txtIE" class="form-control" placeholder="Inscrição Estadual"></asp:TextBox>
                                            </div>

                                            <div class="col-lg-2 form-group">
                                                <label>Tipo de Venda</label>
                                                <asp:DropDownList runat="server" ID="ddlDestinoVenda" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlDestinoVenda_SelectedIndexChanged">
                                                    <asp:ListItem Value="C" Text="Consumo"></asp:ListItem>
                                                    <asp:ListItem Value="R" Text="Revenda"></asp:ListItem>
                                                    <asp:ListItem Value="I" Text="Industrialização"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>

                                            <div runat="server" id="div_Moeda" class="col-lg-5 padd-0">
                                                <div runat="server" class="col-lg-8 form-group">
                                                    <uc1:DropDownList_Padrao runat="server" ID="ddlMoeda" Titulo="Moeda" AutoPostBack="true" OnSelectedIndexChanged="ddlMoeda_SelectedIndexChanged" />
                                                </div>

                                                <div class="col-lg-4 form-group">
                                                    <uc1:TextBox_Padrao runat="server" ID="txtCambio" Titulo="Taxa de Câmbio" Classe="nCambio" ReadOnly="true" />
                                                </div>
                                            </div>

                                        </div>

                                        <div runat="server" id="div_Contato" class="row">

                                            <div runat="server" id="div_Contato_contato" class="col-lg-3 form-group">
                                                <label>Contato do Cliente</label>
                                                <asp:DropDownList runat="server" ID="ddlContato" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>

                                            <div class="col-lg-2 form-group">
                                                <label>Tipo de Cliente</label>
                                                <asp:DropDownList runat="server" ID="ddlTipoCliente" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>

                                            <div class="col-lg-3 form-group">
                                                <label>Segmentos do Cliente</label>
                                                <asp:ListBox runat="server" ID="lstSegmentosCliente" SelectionMode="Multiple" CssClass="Caixa_Selecao"></asp:ListBox>
                                            </div>

                                        </div>

                                        <div runat="server" id="div_tabela" class="row">
                                            <div class="col-lg-3 form-group">
                                                <label>Tabela de Preços</label>
                                                <asp:DropDownList runat="server" ID="ddlTabela" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlTabela_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div runat="server" id="div_Tabela_Obs" class="col-lg-9 form-group">
                                                <label>Observação <small>(Obrigatório)</small></label>
                                                <asp:TextBox runat="server" ID="txtTabelaObs" class="form-control" MaxLength="100"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_orcamento" class="row">

                                            <div class="col-lg-3 form-group">
                                                <label>Tipo de Orçamento</label>
                                                <asp:DropDownList runat="server" ID="ddlTipoOrcamento" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlTipoOrcamento_SelectedIndexChanged"></asp:DropDownList>
                                            </div>

                                            <div runat="server" id="div_Fluxo" class="col-lg-2 form-group">
                                                <label>Fluxo</label>
                                                <asp:DropDownList runat="server" ID="ddlFluxo" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlFluxo_SelectedIndexChanged"></asp:DropDownList>
                                            </div>

                                            <div class="col-lg-3 form-group" runat="server" id="div_lstTipoServicos">
                                                <label>Tipos de Serviços</label>
                                                <asp:ListBox runat="server" ID="lstTipoServicos_TipoOrcamento" SelectionMode="Multiple" CssClass="Caixa_Selecao"></asp:ListBox>
                                            </div>

                                            <div class="col-lg-3 form-group" runat="server" id="div_lstEscopos">
                                                <label>Escopos</label>
                                                <asp:ListBox runat="server" ID="lstEscopos_TipoOrcamento" SelectionMode="Multiple" CssClass="Caixa_Selecao"></asp:ListBox>
                                            </div>

                                            <div runat="server" id="div_AtualizarServicos" class="col-lg-1 form-group">
                                                <label>&nbsp;</label>
                                                <div style="display: flex; justify-content: end; width: 100%">
                                                    <div>
                                                        <asp:LinkButton runat="server" ID="lnkAtualizarServicos_Escopos" class="form-control btn-info" data-toggle="tooltip" title="Atualizar Serviços e Escopos"><i class="fa fa-refresh"></i></asp:LinkButton>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>

                                        <div runat="server" id="div_empresa" class="row">
                                            <div class="col-lg-3 form-group">
                                                <label>Empresa</label>
                                                <asp:DropDownList runat="server" ID="ddlEmpresa_Orcamento" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlEmpresa_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_Confidencial" class="row">
                                            <div class="col-lg-1 form-group">
                                                <label>Confidencial?</label>
                                                <asp:DropDownList runat="server" ID="ddlConfidencial" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlConfidencial_SelectedIndexChanged">
                                                    <asp:ListItem Value="N">Não</asp:ListItem>
                                                    <asp:ListItem Value="S">Sim</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_Revisao" class="row">
                                            <div class="col-lg-1 form-group">
                                                <label>Revisão</label>
                                                <asp:TextBox runat="server" ID="txtRevisao" class="form-control" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                    </div>

                                    <div runat="server" id="div_Salvar_Sempre_1" class="col-lg-12">
                                        <fieldset class="form-stacked actions">
                                            <asp:Button ID="cmdSalvar_Sempre_1" class="btn btn-lg btn-success" runat="server" Text="Salvar" />
                                            <asp:Button ID="cancelar_1" class="btn btn-lg btn-warning" runat="server" Text="Cancelar" OnClick="voltar_Click" />
                                        </fieldset>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <div id="dialog-AtualizarServicos_Escopos" class="modal" style="padding: 10px;" title="Atualizar Serviços e Escopos">
                            <b>Deseja atualizar os Serviços e Escopos do Orçamento?</b>
                            <br />
                            <br />
                            <small><b>Aviso: </b>Executar esta ação irá repopular todos os Serviços e Escopos, excluindo quaisquer alterações que já tenham sido feitas nos Serviços e Escopos já presentes.</small>
                        </div>

                        <div class="modal fade" id="modal_NovoEndereco" data-backdrop="static">
                            <div class="modal-dialog" style="width: 65%;">
                                <div class="modal-content" style="overflow: visible !important;">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                                        <h4 class="modal-title">Novo Endereço</h4>
                                    </div>
                                    <div class="modal-body">

                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_NovoEndereco_Fixa" />
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_NovoEndereco" />

                                        <div class="row">
                                            <div class="col-lg-2 form-group">
                                                <label>Tipo</label>
                                                <asp:TextBox runat="server" ID="txtTipo_NovoEndereco" Text="Entrega" class="form-control" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-2 form-group">
                                                <label>CEP</label>
                                                <asp:TextBox runat="server" ID="txtCEP_NovoEndereco" placeholder="CEP" class="form-control" AutoPostBack="true" OnTextChanged="txtCEP_NovoEndereco_TextChanged"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-6 form-group">
                                                <label>Logradouro</label>
                                                <asp:TextBox runat="server" ID="txtLogradouro_NovoEndereco" placeholder="Logradouro do Endereço" class="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-lg-2 form-group">
                                                <label>Número</label>
                                                <asp:TextBox runat="server" ID="txtNumero_NovoEndereco" placeholder="Número" class="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-lg-4 form-group">
                                                <label>Complemento</label>
                                                <asp:TextBox runat="server" ID="txtComplemento_NovoEndereco" placeholder="Complemento" class="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="row">
                                            <div class="col-lg-3 form-group">
                                                <label>Bairro</label>
                                                <asp:TextBox runat="server" ID="txtBairro_NovoEndereco" placeholder="Bairro" class="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-lg-3 form-group">
                                                <label>País</label>
                                                <asp:TextBox runat="server" ID="txtPais_NovoEndereco" placeholder="País" class="form-control"></asp:TextBox>
                                            </div>
                                            <div class="col-lg-2 form-group">
                                                <label>Estado</label>
                                                <asp:DropDownList runat="server" ID="ddlEstado_NovoEndereco" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlEstado_NovoEndereco_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                            <div runat="server" id="div_Cidade_NovoEndereco" class="col-lg-4 form-group">
                                                <label>Cidade</label>
                                                <asp:DropDownList runat="server" ID="ddlCidade_NovoEndereco" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="modal-footer">
                                            <asp:Button ID="cmdNovoEndereco_Salvar" class="btn btn-success" runat="server" Text="Salvar" OnClick="cmdNovoEndereco_Salvar_Click" />
                                            <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>

                    </asp:Panel>

                    <asp:Panel runat="server" ID="pn2" class="painel desaparece2">

                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <div style="padding: 0; width: 100%; display: flex;">
                                    <div style="padding: 0; width: 90%">
                                        <h3 class="panel-title"><b>Informações do Orçamento</b></h3>
                                    </div>
                                    <div style="display: flex; justify-content: right; padding: 0; width: 10%">
                                        <asp:LinkButton runat="server" ID="lnkAtualizaInfo" OnClick="cmdAtualiza_Click" class="form-control btn-info" Style="width: 22.5%" data-toggle="tooltip" title="Atualizar Informações do Orçamento"><i class="fa fa-refresh"></i></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <div class="panel-body">
                                <div class="row">

                                    <div class="col-lg-12">

                                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaSegundaInfo" />

                                        <div runat="server" id="div_Referencia" class="row">
                                            <div class="col-lg-2 form-group">
                                                <label>N° Controle TT</label>
                                                <asp:TextBox runat="server" ID="txtControle_TT" class="form-control" placeholder="N° de Controle" MaxLength="30"></asp:TextBox>
                                            </div>
                                            <div class="col-lg-4 form-group">
                                                <label>Referência</label>
                                                <asp:TextBox runat="server" ID="txtReferencia" class="form-control" placeholder="Referência" MaxLength="60"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_pagamento" class="row">

                                            <div class="col-lg-3 form-group">
                                                <label>Condição de Pagamento</label>
                                                <div style="display: flex;">
                                                    <asp:DropDownList runat="server" ID="ddlCondicaoPagamento" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                    <div class="col-lg-2" style="display: flex; padding: 0; justify-content: right;" runat="server" id="div_cmdNovaCondicaoPagamento">
                                                        <div class="col-lg-8" style="padding: 0;">
                                                            <asp:LinkButton runat="server" ID="cmdNovaCondicaoPagamento" class="form-control novaCondPgto btn-info" data-toggle="tooltip" title="Nova Condição de Pagamento" Style="text-align: center;"><i class="fa fa-plus"></i></asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>

                                            <div runat="server" id="div_NovaCondPgto_Motivo" class="col-lg-5 form-group">
                                                <label>Motivo da Alteração da Cond. de Pgto. <small>(Obrigatório)</small></label>
                                                <asp:TextBox runat="server" ID="txtNovaCondPgto_Motivo" class="form-control" MaxLength="100"></asp:TextBox>
                                            </div>

                                            <div class="col-lg-2 form-group">
                                                <label>Vendedor</label>
                                                <asp:DropDownList runat="server" ID="ddlVendedor" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlVendedor_SelectedIndexChanged"></asp:DropDownList>
                                            </div>

                                        </div>

                                        <div runat="server" id="div_envio" class="row">
                                            <div class="col-lg-3 form-group">
                                                <label>Forma de Envio</label>
                                                <asp:DropDownList runat="server" ID="ddlFormaEnvio" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                            <div class="col-lg-3 form-group">
                                                <label>Empresa de Transporte</label>
                                                <asp:TextBox runat="server" ID="txtTransporte" class="form-control" placeholder="Transportadora" MaxLength="200"></asp:TextBox>
                                            </div>
                                            <div class="col-lg-6" style="padding: 0;">

                                                <div class="col-lg-3 form-group">
                                                    <label>Estimativa de Entrega</label>
                                                    <asp:TextBox runat="server" ID="txtEstimativaEntrega" class="form-control" placeholder="" MaxLength="10" data-mask="00/00/0000" type="date"></asp:TextBox>
                                                </div>
                                                <div class="col-lg-3 form-group">
                                                    <label>Dias Prev.</label>
                                                    <asp:TextBox runat="server" ID="txtDiasPrevisao" class="form-control" placeholder="Dias de Execução"></asp:TextBox>
                                                </div>
                                                <div class="col-lg-3 form-group">
                                                    <label>Validade em Dias</label>
                                                    <asp:TextBox runat="server" ID="txtValidade" class="form-control" placeholder="Validade do Orçamento"></asp:TextBox>
                                                </div>
                                                <div class="col-lg-3 form-group">
                                                    <label>Frete</label>
                                                    <asp:TextBox runat="server" ID="txtFrete" class="form-control" placeholder="Valor de Frete"></asp:TextBox>
                                                </div>

                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="col-lg-6 form-group">
                                                <label>Instalador</label>
                                                <asp:DropDownList runat="server" ID="ddlidInstalador" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>

                                            <div runat="server" id="div_Custo" class="col-lg-4 padd-0">
                                                <div class="col-lg-6 form-group">
                                                    <uc1:TextBox_Padrao runat="server" ID="txtCusto_Aduaneiro" Titulo="Custo Aduaneiro" MascaraDinamica="$money($input, ',')" />
                                                </div>

                                                <div class="col-lg-6 form-group">
                                                    <uc1:TextBox_Padrao runat="server" ID="txtCusto_Despachante" Titulo="Custo Despachante" MascaraDinamica="$money($input, ',')" />
                                                </div>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_obs" class="row">
                                            <div class="col-lg-12 form-group">
                                                <label>Observações</label>
                                                <asp:TextBox runat="server" ID="txtObservacao" class="form-control" placeholder="Observação..." TextMode="MultiLine" Height="200px"></asp:TextBox>
                                            </div>
                                        </div>

                                    </div>

                                    <div runat="server" id="div_Salvar_Sempre_2" class="col-lg-12">
                                        <fieldset class="form-stacked actions">
                                            <asp:Button ID="cmdSalvar_Sempre_2" class="btn btn-lg btn-success" runat="server" Text="Salvar" />
                                            <asp:Button ID="cancelar_2" class="btn btn-lg btn-warning" runat="server" Text="Cancelar" OnClick="voltar_Click" />
                                        </fieldset>
                                    </div>

                                </div>
                            </div>
                        </div>

                        <div class="modal fade" id="modal_novaCondicaoPagamento" data-backdrop="static">
                            <div class="modal-dialog" style="width: 40%;">
                                <div class="modal-content" style="overflow: visible !important;">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                                        <h4 class="modal-title">Nova Condição de Pagamento</h4>
                                    </div>
                                    <div class="modal-body">

                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_NovaCondicaoPagamento" />

                                        <div id="abas_Modal_CondicaoPagamento">
                                            <ul id="abas_CondicaoPagamento" class="nav nav-tabs" role="tablist">

                                                <li role="presentation" class="tabpanel active" id="liCondicao" runat="server">
                                                    <a href="#Condicao" id="aba-Condicao" role="tab" data-toggle="tab" aria-controls="Condicao" aria-expanded="false"><b>Condição de Pagamento</b></a>
                                                </li>
                                                <li role="presentation" runat="server" id="liPersonalizada">
                                                    <a href="#Personalizada" id="aba-Personalizada" role="tab" data-toggle="tab" aria-controls="Personalizada" aria-expanded="false"><b>Nova Personalizada</b></a>
                                                </li>

                                            </ul>
                                        </div>

                                        <br />

                                        <div id="tab_CondicaoPagamento" class="tab-content">

                                            <div role="tabpanel" class="tab-pane fade in active" id="Condicao" aria-labelledby="Condicao-tab">

                                                <div class="row">
                                                    <div class="col-lg-12">

                                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_NovaCondicaoPagamento_Fixa" />

                                                        <div class="form-group">
                                                            <label>Condição de Pagamento</label>
                                                            <asp:DropDownList runat="server" ID="ddlNovaCondicaoPagamento" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                        </div>

                                                    </div>
                                                </div>

                                                <div class="modal-footer">
                                                    <asp:Button ID="cmdNovaCondicaoPagamento_Salvar" class="btn btn-success" runat="server" Text="Salvar no Paceiro" OnClick="cmdNovaCondicaoPagamento_Salvar_Click" />
                                                    <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                                                </div>

                                            </div>

                                            <div role="tabpanel" class="tab-pane fade" id="Personalizada" aria-labelledby="Personalizada-tab">

                                                <div class="col-lg-12">
                                                    <div class="row">

                                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_NovaCondicaoPagamento_Fixa_Personalizada" />

                                                        <div class="col-lg-8 form-group">
                                                            <label>Descrição</label>
                                                            <asp:TextBox runat="server" ID="txtDescricao_Nova_CondicaoPagamento" class="form-control"></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-4 form-group">
                                                            <label>Qtd Parcelas</label>
                                                            <asp:TextBox runat="server" ID="txtQtdParcelas_Nova_CondicaoPagamento" class="form-control"></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-12 form-group" style="overflow-y: scroll; max-height: 450px;">
                                                            <table id="tblParcelas_Nova_CondicaoPagamento" class="table table-striped table-bordered table-hover invisivel" style="margin: 0;">

                                                                <thead>
                                                                    <tr>
                                                                        <th style="width: 50%;">Tipo de Pagamento</th>
                                                                        <th style="width: 25%;">Porcentagem</th>
                                                                        <th style="width: 25%;">DDL</th>
                                                                        <th style="width: 10%;"></th>
                                                                    </tr>
                                                                </thead>

                                                                <tbody class="bodyReal">
                                                                </tbody>

                                                                <tbody class="bodyModelo invisivel">

                                                                    <tr class="tr_Modelo">
                                                                        <td style="width: 50%;">
                                                                            <asp:DropDownList ID="ddlTipos_Parcelas_Nova_CondicaoPagamento" runat="server" CssClass="form-control tipoParcela"></asp:DropDownList></td>
                                                                        <td style="width: 20%;">
                                                                            <div style="display: flex; align-items: center; gap: 5px;">
                                                                                <asp:TextBox runat="server" CssClass="form-control porcentagemParcela" Text="0,00"></asp:TextBox>
                                                                                %
                                                                            </div>
                                                                        </td>
                                                                        <td style="width: 20%;">
                                                                            <asp:TextBox runat="server" CssClass="form-control ddlParcela" Text="0"></asp:TextBox></td>
                                                                        <td style="vertical-align: middle; text-align: center; width: 10%;">
                                                                            <asp:LinkButton ID="lnkExcluirParcela" runat="server" data-toggle="tooltip" title="Excluir Parcela" TabIndex="100"><i class="fa fa-eraser"></i></asp:LinkButton></td>
                                                                    </tr>
                                                                    <tr class="tr_Botao">
                                                                        <td colspan="4" align="right" valign="middle">
                                                                            <asp:LinkButton ID="lnkAdicionarParcela" runat="server" data-toggle="tooltip" title="Adicionar Parcela"><i class="fa fa-plus"></i></asp:LinkButton></td>
                                                                    </tr>

                                                                </tbody>

                                                            </table>
                                                        </div>

                                                    </div>
                                                </div>

                                                <div class="modal-footer">
                                                    <asp:Button ID="cmdNovaCondicaoPagamento_Personalizada" class="btn btn-success invisivel" runat="server" Text="Nova Condição Personalizada" OnClick="cmdNovaCondicaoPagamento_Personalizada_Click" />
                                                    <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                                                </div>

                                            </div>

                                        </div>

                                    </div>
                                </div>
                            </div>

                        </div>

                    </asp:Panel>

                    <asp:Panel runat="server" ID="pn3" class="painel desaparece2">

                        <div class="col-lg-12 form-group" style="padding: 0;">
                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaServicos_Recursos" />
                            <div runat="server" id="div_recarregaServicos" style="display: flex; justify-content: center; width: 100%">
                                <div>
                                    <asp:LinkButton runat="server" ID="cmdRecarregaServicos" OnClick="cmdRecarregaServicos_Click" class="form-control btn-info" data-toggle="tooltip" title="Recarregar Serviços e Escopos"><i class="fa fa-refresh"></i></asp:LinkButton>
                                </div>
                            </div>
                        </div>

                        <div class="panel panel-default" runat="server" id="div_pn3">
                            <div class="panel-heading">
                                <div style="padding: 0; width: 100%; display: flex;">
                                    <div style="padding: 0; width: 90%">
                                        <h3 class="panel-title"><b>Serviços e Recursos</b></h3>
                                    </div>
                                    <div style="display: flex; justify-content: right; padding: 0; width: 10%">
                                        <asp:LinkButton runat="server" ID="lnkAtualiza_Servicos" OnClick="cmdAtualiza_Click" class="form-control btn-info" Style="width: 22.5%" data-toggle="tooltip" title="Atualizar Informações do Orçamento"><i class="fa fa-refresh"></i></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <div class="panel-body">
                                <div class="row">

                                    <div class="col-lg-12">

                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Dentro_Servicos_Recursos" />

                                        <div id="abas_Servicos_Recursos">
                                            <ul id="tabs_Servicos_Recursos" class="nav nav-tabs" role="tablist">

                                                <li role="presentation" class="tabpanel active" id="aba_Servicos_Recursos" runat="server">
                                                    <a href="#Servicos_Recursos" role="tab" id="aba-Servicos_Recursos" data-toggle="tab" aria-controls="Servicos_Recursos"><b>Serviços / Recursos</b></a>
                                                </li>
                                                <li role="presentation" runat="server" id="aba_CheckList">
                                                    <a href="#CheckList" id="aba-CheckList" role="tab" data-toggle="tab" aria-controls="CheckList" aria-expanded="false"><b>Escopos</b></a>
                                                </li>

                                            </ul>
                                        </div>

                                        <br />

                                        <div id="tab_Servicos_Recursos" class="tab-content">

                                            <div role="tabpanel" class="tab-pane fade in active" id="Servicos_Recursos" aria-labelledby="Servicos_Recursos-tab">

                                                <div id="Desconto_&_Margem_Servicos">

                                                    <div class="row">

                                                        <div runat="server" id="div_DescontoServico" class="col-lg-2">
                                                            <div class="form-group">
                                                                <label>Desconto Global (%) <small runat="server" id="smDesconto_Servicos_Recursos"></small></label>
                                                                <asp:TextBox runat="server" ID="txtDescontoServico" Text="0.00" class="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div runat="server" id="divPrazo_Servicos" class="col-lg-2">
                                                            <div class="form-group">
                                                                <label>Prazo de Entrega em Dias</label>
                                                                <asp:TextBox runat="server" ID="txtPrazoServico" Text="15" class="form-control"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-6">&nbsp;</div>

                                                        <div runat="server" id="div_TotalServico" class="col-lg-2">
                                                            <div class="form-group">
                                                                <label>Total Serviços</label>
                                                                <asp:TextBox runat="server" ID="txtTotalServico" Text="0,00" class="form-control totalGeral" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                    </div>

                                                </div>

                                                <div runat="server" id="div_gvServicos_Recursos" class="row">
                                                    <div class="col-lg-12 form-group">

                                                        <asp:GridView ID="gvServicos_Recursos" class="table table-striped table-bordered table-hover tablePai"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvServicos_Recursos_RowDataBound">
                                                            <Columns>

                                                                <asp:TemplateField HeaderText="ID">
                                                                    <ItemTemplate>
                                                                        <asp:Label runat="server" ID="lblidRegistro" Text='<%# Eval("idRegistro") %>' class="id idItem" ClientIDMode="Static"></asp:Label>
                                                                        <asp:Label runat="server" ID="lblidPais" Text='<%# string.Format("{0}-{1}-{2}", Eval("idItemPai"), Eval("idItemAvo"), Eval("idItemBisavo")) %>' class="id idPais" ClientIDMode="Static"></asp:Label>
                                                                        <asp:Label runat="server" ID="lblidItem" Text='<%# Eval("idItem") %>' ClientIDMode="Static"></asp:Label>
                                                                        <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-toggle="tooltip" title="Exibir Composição" data-div-id='<%# string.Format("tr_1_Servico-{0}-{1}-{2}-{3}", Eval("idRegistro"), Eval("idItemPai"), Eval("idItemAvo"), Eval("idItemBisavo")) %>'></asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Item">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox runat="server" ID="nOrdem" Text='<%# Eval("nOrdem") %>' CssClass="form-control" ClientIDMode="Static"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                    DataTextField="sCodigo" HeaderText="Código"
                                                                    DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link codigo" />
                                                                </asp:HyperLinkField>

                                                                <asp:TemplateField HeaderText="Descrição">
                                                                    <ItemTemplate>
                                                                        <div>
                                                                            <asp:LinkButton ID="lnkCard" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                                OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"servico\");" %>'
                                                                                OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"servico\");" %>'
                                                                                CssClass="produto-detalhe-link descricao"
                                                                                ClientIDMode="Static"
                                                                                data-idproduto='<%# Eval("idItem") %>'
                                                                                data-tabela="produto" />

                                                                            <div id='<%# Eval("idItem") + "_servico" %>' class="product-card" style="display: none;">
                                                                                <!--conteudo via script -->
                                                                            </div>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="17%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="TipoProduto" HeaderText="Tipo">
                                                                    <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Qtd">
                                                                    <ItemTemplate>
                                                                        <uc1:TextBox_Padrao runat="server" ID="nQtd" IDMode="Static" Text='<%# Bind("NQuantidade", "{0:N2}") %>' Classe="qtd" MascaraDinamica="$money($input, ',')" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Prazo">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox runat="server" ID="nPrazo" Text='<%# Bind("dtInclusao") %>' class="form-control prazo"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Valor Unitário">
                                                                    <ItemTemplate>
                                                                        <asp:Label runat="server" ID="nPreco" Text='<%# Bind("Preco", "{0:N2}") %>' class="valor"></asp:Label>
                                                                        <asp:HiddenField runat="server" ID="hddPreco" Value='<%# Bind("Preco", "{0:N2}") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Margem">
                                                                    <ItemTemplate>
                                                                        <asp:Label runat="server" ID="nMargem" Text='<%# Bind("nMargem", "{0:N2}") %>' class="margem"></asp:Label>
                                                                        <asp:HiddenField runat="server" ID="hddMargem" Value='<%# Bind("nMargem", "{0:N2}") %>' />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Desconto">
                                                                    <ItemTemplate>
                                                                        <uc1:TextBox_Padrao runat="server" ID="nDesconto" Text='<%# Bind("nFator", "{0:N2}") %>' MascaraDinamica="$money($input, ',')" Grupo_Simbolo="<i class='fa fa-percent'></i>" />
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Ajuste">
                                                                    <ItemTemplate>
                                                                        <div runat="server" id="divServico_Comparativo" class="" style="display: inline; padding: 1px; width: 100%; justify-content: center; vertical-align: middle;">
                                                                            <i runat="server" id="iconeServico_Comparativo" class="fa fa-plus"></i>
                                                                            &nbsp;
                                                                            <asp:Label runat="server" ID="nAjusteServicos_Comparativos" ClientIDMode="Static" Text='<%# Bind("NAjuste", "{0:N2}") %>' CssClass="ajuste"></asp:Label>
                                                                            &nbsp;%
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Total">
                                                                    <ItemTemplate>
                                                                        <asp:Label runat="server" ID="nTotal" Text='<%# Bind("NTotal", "{0:N2}") %>' class="total"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField>
                                                                    <HeaderTemplate>
                                                                        <asp:CheckBox runat="server" ID="cbValida_Todos" class="btn btn-small" />
                                                                    </HeaderTemplate>
                                                                    <ItemTemplate>
                                                                        <asp:CheckBox runat="server" ID="cbValidado" class="btn btn-small" />

                                                                        <%# NovaLinha(string.Format("tr_1_Servico-{0}-{1}-{2}-{3}", Eval("idRegistro"), Eval("idItemPai"), Eval("idItemAvo"), Eval("idItemBisavo"))) %>

                                                                        <asp:GridView ID="gvServicos_Recursos_Composicao_1" class="table table-striped table-bordered table-hover tablePai"
                                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvServicos_Recursos_Composicao_1_RowDataBound">
                                                                            <Columns>

                                                                                <asp:TemplateField HeaderText="ID">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label runat="server" ID="lblidRegistro" Text='<%# Eval("idRegistro") %>' class="id idItem" ClientIDMode="Static"></asp:Label>
                                                                                        <asp:Label runat="server" ID="lblidPais" Text='<%# string.Format("{0}-{1}-{2}", Eval("idItemPai"), Eval("idItemAvo"), Eval("idItemBisavo")) %>' class="id idPais" ClientIDMode="Static"></asp:Label>
                                                                                        <asp:Label runat="server" ID="lblidItem" Text='<%# Eval("idItem") %>' ClientIDMode="Static"></asp:Label>
                                                                                        <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-toggle="tooltip" title="Exibir Composição" data-div-id='<%# string.Format("tr_2_Servico-{0}-{1}-{2}-{3}", Eval("idRegistro"), Eval("idItemPai"), Eval("idItemAvo"), Eval("idItemBisavo")) %>'></asp:LinkButton>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>

                                                                                <asp:BoundField DataField="nOrdem" HeaderText="Item">
                                                                                    <HeaderStyle CssClass="id" />
                                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id" />
                                                                                </asp:BoundField>

                                                                                <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                    DataTextField="sCodigo" HeaderText="Código"
                                                                                    DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link codigo" />
                                                                                </asp:HyperLinkField>

                                                                                <asp:TemplateField HeaderText="Descrição">
                                                                                    <ItemTemplate>
                                                                                        <div>
                                                                                            <asp:LinkButton ID="lnkCard" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                                                OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"subServico\");" %>'
                                                                                                OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"subServico\");" %>'
                                                                                                CssClass="produto-detalhe-link descricao"
                                                                                                ClientIDMode="Static"
                                                                                                data-idproduto='<%# Eval("idItem") %>'
                                                                                                data-tabela="produto" />

                                                                                            <div id='<%# Eval("idItem") + "_subServico" %>' class="product-card" style="display: none;">
                                                                                                <!--conteudo via script -->
                                                                                            </div>
                                                                                        </div>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>

                                                                                <asp:BoundField DataField="TipoProduto" HeaderText="Tipo">
                                                                                    <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                </asp:BoundField>

                                                                                <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                                    <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                </asp:BoundField>

                                                                                <asp:TemplateField HeaderText="Quantidade">
                                                                                    <ItemTemplate>
                                                                                        <uc1:TextBox_Padrao runat="server" ID="nQtd" IDMode="Static" Text='<%# Bind("NQuantidade", "{0:N2}") %>' Classe="qtd" MascaraDinamica="$money($input, ',')" />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>

                                                                                <asp:TemplateField HeaderText="Valor Unitário">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label runat="server" ID="nPreco" Text='<%# Bind("Preco", "{0:N2}") %>' class="valor"></asp:Label>
                                                                                        <asp:HiddenField runat="server" ID="hddPreco" Value='<%# Bind("Preco", "{0:N2}") %>' />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>

                                                                                <asp:TemplateField HeaderText="Margem">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label runat="server" ID="nMargem" Text='<%# Bind("nMargem", "{0:N2}") %>' class="margem"></asp:Label>
                                                                                        <asp:HiddenField runat="server" ID="hddMargem" Value='<%# Bind("nMargem", "{0:N2}") %>' />
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>

                                                                                <asp:TemplateField HeaderText="Total">
                                                                                    <ItemTemplate>
                                                                                        <asp:Label runat="server" ID="nTotal" Text='<%# Bind("NTotal", "{0:N2}") %>' class="total"></asp:Label>
                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>

                                                                                <asp:TemplateField>
                                                                                    <HeaderTemplate>
                                                                                        <asp:CheckBox runat="server" ID="cbValida_Todos" class="btn btn-small" />
                                                                                    </HeaderTemplate>
                                                                                    <ItemTemplate>
                                                                                        <asp:CheckBox runat="server" ID="cbValidado" class="btn btn-small" />

                                                                                        <%# NovaLinha(string.Format("tr_2_Servico-{0}-{1}-{2}-{3}", Eval("idRegistro"), Eval("idItemPai"), Eval("idItemAvo"), Eval("idItemBisavo"))) %>

                                                                                        <asp:GridView ID="gvServicos_Recursos_Composicao_2" class="table table-striped table-bordered table-hover tablePai"
                                                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvServicos_Recursos_Composicao_2_RowDataBound">
                                                                                            <Columns>

                                                                                                <asp:TemplateField HeaderText="ID">
                                                                                                    <ItemTemplate>
                                                                                                        <asp:Label runat="server" ID="lblidRegistro" Text='<%# Eval("idRegistro") %>' class="id idItem" ClientIDMode="Static"></asp:Label>
                                                                                                        <asp:Label runat="server" ID="lblidPais" Text='<%# string.Format("{0}-{1}-{2}", Eval("idItemPai"), Eval("idItemAvo"), Eval("idItemBisavo")) %>' class="id idPais" ClientIDMode="Static"></asp:Label>
                                                                                                        <asp:Label runat="server" ID="lblidItem" Text='<%# Eval("idItem") %>' ClientIDMode="Static"></asp:Label>
                                                                                                        <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-toggle="tooltip" title="Exibir Composição" data-div-id='<%# string.Format("tr_3_Servico-{0}-{1}-{2}-{3}", Eval("idRegistro"), Eval("idItemPai"), Eval("idItemAvo"), Eval("idItemBisavo")) %>'></asp:LinkButton>
                                                                                                    </ItemTemplate>
                                                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                </asp:TemplateField>

                                                                                                <asp:BoundField DataField="nOrdem" HeaderText="Item">
                                                                                                    <HeaderStyle CssClass="id" />
                                                                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id" />
                                                                                                </asp:BoundField>

                                                                                                <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                                    DataTextField="sCodigo" HeaderText="Código"
                                                                                                    DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link codigo" />
                                                                                                </asp:HyperLinkField>

                                                                                                <asp:TemplateField HeaderText="Descrição">
                                                                                                    <ItemTemplate>
                                                                                                        <div>
                                                                                                            <asp:LinkButton ID="lnkCard" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                                                                OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"recursos_1\");" %>'
                                                                                                                OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"recursos_1\");" %>'
                                                                                                                CssClass="produto-detalhe-link descricao"
                                                                                                                ClientIDMode="Static"
                                                                                                                data-idproduto='<%# Eval("idItem") %>'
                                                                                                                data-tabela="produto" />

                                                                                                            <div id='<%# Eval("idItem") + "_recursos_1" %>' class="product-card" style="display: none;">
                                                                                                                <!--conteudo via script -->
                                                                                                            </div>
                                                                                                        </div>
                                                                                                    </ItemTemplate>
                                                                                                    <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                                </asp:TemplateField>

                                                                                                <asp:BoundField DataField="TipoProduto" HeaderText="Tipo">
                                                                                                    <ItemStyle Width="13%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                </asp:BoundField>

                                                                                                <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                </asp:BoundField>

                                                                                                <asp:TemplateField HeaderText="Quantidade">
                                                                                                    <ItemTemplate>
                                                                                                        <uc1:TextBox_Padrao runat="server" ID="nQtd" IDMode="Static" Text='<%# Bind("NQuantidade", "{0:N2}") %>' Classe="qtd" MascaraDinamica="$money($input, ',')" />
                                                                                                    </ItemTemplate>
                                                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                </asp:TemplateField>

                                                                                                <asp:TemplateField HeaderText="Valor Unitário">
                                                                                                    <ItemTemplate>
                                                                                                        <uc1:TextBox_Padrao runat="server" ID="nPreco" Text='<%# Bind("Preco", "{0:N2}") %>' Classe="valor" MascaraDinamica="$money($input, ',')" />
                                                                                                    </ItemTemplate>
                                                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                </asp:TemplateField>

                                                                                                <asp:TemplateField HeaderText="Margem">
                                                                                                    <ItemTemplate>
                                                                                                        <asp:TextBox runat="server" ID="nMargem" Text='<%# Bind("nMargem", "{0:N2}") %>' class="form-control" x-mask:dynamic="$money($input, ',')" />
                                                                                                    </ItemTemplate>
                                                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                </asp:TemplateField>

                                                                                                <asp:TemplateField HeaderText="Total">
                                                                                                    <ItemTemplate>
                                                                                                        <asp:Label runat="server" ID="nTotal" Text='<%# Bind("NTotal", "{0:N2}") %>' class="total"></asp:Label>
                                                                                                    </ItemTemplate>
                                                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                </asp:TemplateField>

                                                                                                <asp:TemplateField>
                                                                                                    <HeaderTemplate>
                                                                                                        <asp:CheckBox runat="server" ID="cbValida_Todos" class="btn btn-small" />
                                                                                                    </HeaderTemplate>
                                                                                                    <ItemTemplate>
                                                                                                        <asp:CheckBox runat="server" ID="cbValidado" class="btn btn-small" />

                                                                                                        <%# NovaLinha(string.Format("tr_3_Servico-{0}-{1}-{2}-{3}", Eval("idRegistro"), Eval("idItemPai"), Eval("idItemAvo"), Eval("idItemBisavo"))) %>

                                                                                                        <asp:GridView ID="gvServicos_Recursos_Composicao_3" class="table table-striped table-bordered table-hover tablePai"
                                                                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvServicos_Recursos_Composicao_3_RowDataBound">
                                                                                                            <Columns>

                                                                                                                <asp:TemplateField HeaderText="ID">
                                                                                                                    <ItemTemplate>
                                                                                                                        <asp:Label runat="server" ID="lblidRegistro" Text='<%# Eval("idRegistro") %>' class="id idItem" ClientIDMode="Static"></asp:Label>
                                                                                                                        <asp:Label runat="server" ID="lblidPais" Text='<%# string.Format("{0}-{1}-{2}", Eval("idItemPai"), Eval("idItemAvo"), Eval("idItemBisavo")) %>' class="id idPais" ClientIDMode="Static"></asp:Label>
                                                                                                                        <asp:Label runat="server" ID="lblidItem" Text='<%# Eval("idItem") %>' ClientIDMode="Static"></asp:Label>
                                                                                                                    </ItemTemplate>
                                                                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                                </asp:TemplateField>

                                                                                                                <asp:BoundField DataField="nOrdem" HeaderText="Item">
                                                                                                                    <HeaderStyle CssClass="id" />
                                                                                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id" />
                                                                                                                </asp:BoundField>

                                                                                                                <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                                                    DataTextField="sCodigo" HeaderText="Código"
                                                                                                                    DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link codigo" />
                                                                                                                </asp:HyperLinkField>

                                                                                                                <asp:TemplateField HeaderText="Descrição">
                                                                                                                    <ItemTemplate>
                                                                                                                        <div>
                                                                                                                            <asp:LinkButton ID="lnkCard" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                                                                                OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"recursos_2\");" %>'
                                                                                                                                OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"recursos_2\");" %>'
                                                                                                                                CssClass="produto-detalhe-link descricao"
                                                                                                                                ClientIDMode="Static"
                                                                                                                                data-idproduto='<%# Eval("idItem") %>'
                                                                                                                                data-tabela="produto" />

                                                                                                                            <div id='<%# Eval("idItem") + "_recursos_2" %>' class="product-card" style="display: none;">
                                                                                                                                <!--conteudo via script -->
                                                                                                                            </div>
                                                                                                                        </div>
                                                                                                                    </ItemTemplate>
                                                                                                                    <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                                                </asp:TemplateField>

                                                                                                                <asp:BoundField DataField="TipoProduto" HeaderText="Tipo">
                                                                                                                    <ItemStyle Width="13%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                                </asp:BoundField>

                                                                                                                <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                                </asp:BoundField>

                                                                                                                <asp:TemplateField HeaderText="Quantidade">
                                                                                                                    <ItemTemplate>
                                                                                                                        <uc1:TextBox_Padrao runat="server" ID="nQtd" IDMode="Static" Text='<%# Bind("NQuantidade", "{0:N2}") %>' Classe="qtd" MascaraDinamica="$money($input, ',')" />
                                                                                                                    </ItemTemplate>
                                                                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                                </asp:TemplateField>

                                                                                                                <asp:TemplateField HeaderText="Valor Unitário">
                                                                                                                    <ItemTemplate>
                                                                                                                        <uc1:TextBox_Padrao runat="server" ID="nPreco" Text='<%# Bind("Preco", "{0:N2}") %>' Classe="valor" MascaraDinamica="$money($input, ',')" />
                                                                                                                    </ItemTemplate>
                                                                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                                </asp:TemplateField>

                                                                                                                <asp:TemplateField HeaderText="Margem">
                                                                                                                    <ItemTemplate>
                                                                                                                        <asp:TextBox runat="server" ID="nMargem" Text='<%# Bind("nMargem", "{0:N2}") %>' class="form-control" x-mask:dynamic="$money($input, ',')" />
                                                                                                                    </ItemTemplate>
                                                                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                                </asp:TemplateField>

                                                                                                                <asp:TemplateField HeaderText="Total">
                                                                                                                    <ItemTemplate>
                                                                                                                        <asp:Label runat="server" ID="nTotal" Text='<%# Bind("NTotal", "{0:N2}") %>' class="total"></asp:Label>
                                                                                                                    </ItemTemplate>
                                                                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                                </asp:TemplateField>

                                                                                                                <asp:TemplateField>
                                                                                                                    <HeaderTemplate>
                                                                                                                        <asp:CheckBox runat="server" ID="cbValida_Todos" class="btn btn-small" />
                                                                                                                    </HeaderTemplate>
                                                                                                                    <ItemTemplate>
                                                                                                                        <asp:CheckBox runat="server" ID="cbValidado" class="btn btn-small" />
                                                                                                                    </ItemTemplate>
                                                                                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                                </asp:TemplateField>

                                                                                                            </Columns>
                                                                                                        </asp:GridView>

                                                                                                    </ItemTemplate>
                                                                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                </asp:TemplateField>

                                                                                            </Columns>
                                                                                        </asp:GridView>

                                                                                    </ItemTemplate>
                                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                </asp:TemplateField>

                                                                            </Columns>
                                                                        </asp:GridView>

                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="1%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                            </Columns>
                                                        </asp:GridView>

                                                    </div>
                                                </div>

                                            </div>

                                            <div role="tabpanel" class="tab-pane fade" id="CheckList" aria-labelledby="CheckList-tab">

                                                <div runat="server" id="div_CheckList" class="row">
                                                    <div class="col-lg-12 form-group">
                                                        <div>

                                                            <asp:Repeater runat="server" ID="rptCategoriaEscopos" OnItemDataBound="rptCategoriaEscopos_ItemDataBound">
                                                                <ItemTemplate>

                                                                    <%--<div class="panel panel-default" runat="server" id="div_panel">
                                                                        <div class="panel-heading">
                                                                            <h3 class="panel-title"><b><%# Eval("sDscEscopo") %></b></h3>
                                                                        </div>
                                                                        <div class="panel-body">
                                                                            <div class="row">
                                                                                <div class="col-lg-12">--%>

                                                                    <asp:GridView ID="gvCheckList" class="table table-striped table-bordered table-hover checklist"
                                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idEscopo, idCategoria, sDscCategoria" OnRowDataBound="gvCheckList_RowDataBound" OnRowCreated="gvCheckList_RowCreated">
                                                                        <Columns>

                                                                            <asp:BoundField DataField="idEscopo" HeaderText="Escopo">
                                                                                <HeaderStyle CssClass="id escopo" />
                                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id escopo" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="idCategoria" HeaderText="Categoria">
                                                                                <HeaderStyle CssClass="id categoria" />
                                                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id categoria" />
                                                                            </asp:BoundField>

                                                                            <asp:TemplateField HeaderText="Tópico">
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="sPergunta" Text='<%# Bind("sPergunta", "{0:N2}") %>' class="pergunta" ClientIDMode="AutoID" TabIndex="1000"></asp:Label>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="70%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>

                                                                        </Columns>
                                                                    </asp:GridView>

                                                                    <%--</div>
                                                                            </div>
                                                                        </div>
                                                                    </div>--%>
                                                                </ItemTemplate>
                                                            </asp:Repeater>

                                                        </div>
                                                    </div>
                                                </div>

                                            </div>

                                        </div>

                                    </div>

                                    <div runat="server" id="div_Salvar_Sempre_3" class="col-lg-12">
                                        <fieldset class="form-stacked actions">
                                            <asp:Button ID="cmdSalvar_Sempre_3" class="btn btn-lg btn-success" runat="server" Text="Salvar" />
                                            <asp:Button ID="cancelar_3" class="btn btn-lg btn-warning" runat="server" Text="Cancelar" OnClick="voltar_Click" />
                                        </fieldset>
                                    </div>

                                </div>
                            </div>
                        </div>

                    </asp:Panel>

                    <asp:Panel runat="server" ID="pn4" class="painel desaparece2">

                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_ForaProdutos" />

                        <div class="panel panel-default" runat="server" id="div_pn4">
                            <div class="panel-heading">
                                <div style="padding: 0; width: 100%; display: flex;">
                                    <div style="padding: 0; width: 90%">
                                        <h3 class="panel-title"><b>Seleção de Produtos</b></h3>
                                    </div>
                                    <div style="display: flex; justify-content: right; padding: 0; width: 10%">
                                        <asp:LinkButton runat="server" ID="lnkAtualiza_Produtos" OnClick="cmdAtualiza_Click" class="form-control btn-info" Style="width: 22.5%" data-toggle="tooltip" title="Atualizar Informações do Orçamento"><i class="fa fa-refresh"></i></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <div class="panel-body">
                                <div class="row">

                                    <div class="col-lg-12">

                                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaProdutos" />

                                        <div runat="server" id="div_incluirProduto" class="row">
                                            <div class="col-lg-10">
                                                <uc1:FiltroPesquisa runat="server" ID="FiltroPesquisaProdutos" />
                                            </div>
                                            <div class="col-lg-2">
                                                <br />
                                                <label>&nbsp;</label>
                                                <asp:Button runat="server" ID="cmdIncluirProduto" class="form-control btn-info" Text="Incluir Produto" OnClick="cmdIncluirProduto_Click"></asp:Button>
                                            </div>
                                        </div>

                                        <div id="Desconto_Prazo" class="row">

                                            <div runat="server" id="div_DescontoProduto" class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Desconto Global (%) <small runat="server" id="smDesconto_Produtos">- Máx: 5%</small></label>
                                                    <uc1:TextBox_Padrao runat="server" ID="txtDescontoProduto" Text="0,0000" MascaraDinamica="$money($input, ',', '.', 4)" AutoPostBack="true" OnTextChanged="txtDescontoProduto_TextChanged" />
                                                </div>
                                            </div>

                                            <div runat="server" id="div_PrazoProduto" class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Prazo de Entrega em Dias</label>
                                                    <asp:TextBox runat="server" ID="txtPrazoProduto" Text="15" class="form-control"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-4">&nbsp;</div>

                                            <div runat="server" id="div_TotalProdutos" class="col-lg-2" style="padding: 0; display: flex; justify-content: end;">
                                                <div class="form-group col-lg-10">
                                                    <label>Total Produtos</label>
                                                    <asp:TextBox runat="server" ID="txtTotalProdutos" Text="0,00" class="form-control totalGeral" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <label>&nbsp;</label>
                                                <asp:Button runat="server" ID="cmdImportarProdutos" class="form-control btn-primary importar" Text="Importar Produtos" />
                                            </div>

                                        </div>

                                        <div runat="server" id="divExcluirTodos_Produtos" class="row">

                                            <div class="col-lg-10">&nbsp;</div>

                                            <div class="col-lg-2 form-group">
                                                <label>&nbsp;</label>
                                                <asp:Button runat="server" ID="cmdExcluir_Produtos" class="form-control btn-danger excluirTodosProdutos" Text="Excluir selecionados" />
                                            </div>

                                        </div>

                                        <div runat="server" id="div_gvProdutos" class="row">
                                            <div class="col-lg-12">

                                                <asp:GridView ID="gvProdutos" class="table table-striped table-bordered table-hover "
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvProdutos_RowDataBound" OnRowCommand="gvProdutos_RowCommand">
                                                    <Columns>

                                                        <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                            <HeaderStyle CssClass="id" />
                                                            <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id idItem" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="Item">
                                                            <ItemTemplate>
                                                                <asp:TextBox runat="server" ID="nOrdem" Text='<%# Bind("nOrdem") %>' class="form-control ordem"></asp:TextBox>
                                                                <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-toggle="tooltip" title="Exibir Composição" data-div-id='<%# "tr_1_Produto" + Eval("idRegistro") %>'></asp:LinkButton>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <div style="display: grid;">
                                                                    <label>Código do Produto</label>
                                                                    <div>
                                                                        <asp:LinkButton runat="server" ID="cmdUnificarTodos" class="btn btn-md btn-primary unificarTodos" TabIndex="10000" ClientIDMode="Static" data-toggle="tooltip" title="Unificar Todos os Produtos Duplicados"><i class="fa fa-edit" style="cursor: pointer;"></i></asp:LinkButton>
                                                                        <asp:LinkButton runat="server" ID="cmdVincularTodosProdutos" class="btn btn-md btn-danger vincularTodosProdutos" TabIndex="10000" ClientIDMode="Static" data-toggle="tooltip" title="Vincular Todos os Produtos Não Cadastrados"><i class="fa fa-link" style="cursor: pointer;"></i></asp:LinkButton>
                                                                    </div>
                                                                </div>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <div class="link">
                                                                    <asp:LinkButton ID="cmdCodigo" ClientIDMode="Static" runat="server" href='<%# string.Format("/Manutencao/Produtos_Detalhe.aspx?id={0}", Eval("idItem")) %>' Text='<%# Eval("sCodigo") %>' Style="cursor: pointer;"></asp:LinkButton>
                                                                </div>
                                                                <div runat="server" id="div_modaisCodigo" class="modaisCodigo">
                                                                    &nbsp;
                                                                        <div>
                                                                            <asp:LinkButton runat="server" ID="cmdUnificar" ClientIDMode="Static" class="unificarProduto" data-toggle="tooltip" title="Unificar Duplicados"><i class="fa fa-edit" style="cursor: pointer;"></i></asp:LinkButton>
                                                                            <asp:LinkButton runat="server" ID="cmdVincularProdutos" ClientIDMode="Static" class="vincularProduto" data-toggle="tooltip" title="Vincular Produto"><i class="fa fa-link" style="cursor: pointer;"></i></asp:LinkButton>
                                                                        </div>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Descrição do Produto">
                                                            <ItemTemplate>
                                                                <div>
                                                                    <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                        OnClientClick='<%# "openModal(\"" + Eval("idItem") + "\"); return false;" %>'
                                                                        OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"produto\");" %>'
                                                                        OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"produto\");" %>'
                                                                        CssClass="produto-detalhe-link"
                                                                        ClientIDMode="Static"
                                                                        data-idproduto='<%# Eval("idItem") %>'
                                                                        data-tabela="produto" />

                                                                    <div id='<%# Eval("idItem") + "_produto" %>' class="product-card" style="display: none;">
                                                                        <!--conteudo via script -->
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="24%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:BoundField DataField="TipoProduto" HeaderText="Tipo de Produto">
                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="Prazo">
                                                            <ItemTemplate>
                                                                <asp:TextBox runat="server" ID="nPrazo" Text='<%# Bind("dtInclusao") %>' class="form-control prazo"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:BoundField DataField="SUnidade" HeaderText="Ud.">
                                                            <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="Qtde">
                                                            <ItemTemplate>
                                                                <uc1:TextBox_Padrao runat="server" ID="nQuantidade_Produtos" IDMode="Static" Text='<%# Bind("NQuantidade", "{0:N2}") %>' Classe="qtd" MascaraDinamica="$money($input, ',')" />
                                                            </ItemTemplate>
                                                            <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <div id="Atualiza_Valores_Produtos" class="row">
                                                                    <div class="col-lg-12" style="display: flex;">
                                                                        <label>Valor com IPI</label>
                                                                        <div class="col-lg-4" style="display: flex; justify-content: right; padding: 0;">
                                                                            <div class="col-lg-9" style="padding: 0;">
                                                                                <asp:LinkButton runat="server" ID="cmdAtualiza_Valores_Produtos" ClientIDMode="Static" OnClick="cmdAtualiza_Valores_Produtos_Click" class="form-control btn-success" data-toggle="tooltip" title="Atualizar Valores"><i class="fa fa-money" style="position: relative; left: -20%;"></i></asp:LinkButton>
                                                                            </div>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <div style="display: flex;">
                                                                    <div style="width: 80%">
                                                                        <asp:Label runat="server" ID="nPreco" Text='<%# Bind("Preco", "{0:N2}") %>' class="valor"></asp:Label>
                                                                    </div>
                                                                    <div style="width: 20%;">
                                                                        <asp:LinkButton runat="server" ID="lnkAtualiza_Valor" TabIndex="10000" ClientIDMode="Static" CommandArgument='<%# Bind("idRegistro") %>' CommandName="Produtos" data-toggle="tooltip" title="Atualizar Valores"><i class="fa fa-money"></i></asp:LinkButton>
                                                                    </div>
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Desconto">
                                                            <ItemTemplate>
                                                                <div style="display: flex; flex-wrap: nowrap; align-items: center; gap: 5px;">
                                                                    <asp:TextBox runat="server" ID="nDesc_Produtos" ClientIDMode="Static" Text='<%# Bind("nFator", "{0:N4}") %>' class="form-control dto desconto" aria-describedby="spanSimbolo" Style="z-index: 1;" MaxLength="10"></asp:TextBox>
                                                                    %
                                                                </div>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Valor com IPI e Desconto">
                                                            <ItemTemplate>
                                                                <asp:TextBox runat="server" ID="nUnitario" ClientIDMode="Static" Text='<%# Bind("nUnitario", "{0:N2}") %>' class="form-control unitario"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Total com IPI">
                                                            <ItemTemplate>
                                                                <asp:Label runat="server" ID="nTotal" Text='<%# Bind("NTotal", "{0:N2}") %>' class="total"></asp:Label>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField>
                                                            <HeaderTemplate>
                                                                <asp:CheckBox runat="server" ID="cbExcluirTodos_Produto" class="form-control selecionaTodosProdutos" Style="background: none; border: none;" ClientIDMode="Static"></asp:CheckBox>
                                                            </HeaderTemplate>
                                                            <ItemTemplate>
                                                                <asp:CheckBox runat="server" ID="cbExcluir_Produto" class="form-control" ClientIDMode="Static"></asp:CheckBox>

                                                                <%# NovaLinha(Eval("idRegistro"), "_1_Produto") %>

                                                                <asp:GridView ID="gvProdutos_Composicao" class="table table-striped table-bordered table-hover tablePai"
                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvProdutos_Composicao_RowDataBound">
                                                                    <Columns>

                                                                        <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                                            <HeaderStyle CssClass="id" />
                                                                            <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id idItem" />
                                                                        </asp:BoundField>

                                                                        <asp:TemplateField HeaderText="Item">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="nOrdem" Text='<%# Bind("nOrdem") %>' class="form-control ordem"></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                            DataTextField="sCodigo" HeaderText="Código"
                                                                            DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                            DataTextField="sDscProduto" HeaderText="Descrição"
                                                                            DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="descricao link" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:BoundField DataField="TipoProduto" HeaderText="Tipo">
                                                                            <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:BoundField>

                                                                        <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:BoundField>

                                                                        <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade">
                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:BoundField>

                                                                        <asp:TemplateField HeaderText="Valor Unitário">
                                                                            <ItemTemplate>
                                                                                <asp:Label runat="server" ID="nPreco" Text='<%# Bind("Preco", "{0:N2}") %>' class="valor"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Total">
                                                                            <ItemTemplate>
                                                                                <asp:Label runat="server" ID="nTotal" Text='<%# Bind("NTotal", "{0:N2}") %>' class="total"></asp:Label>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                    </Columns>
                                                                </asp:GridView>

                                                            </ItemTemplate>
                                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                    </Columns>
                                                </asp:GridView>

                                            </div>
                                        </div>

                                    </div>

                                    <div class="modal fade" id="modalVincularTodosProdutos" data-backdrop="static">
                                        <div class="modal-dialog" style="width: 50%;">
                                            <div class="modal-content" style="overflow: visible !important;">
                                                <div class="modal-header">
                                                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                                                    <h4 class="modal-title">Vincular Todos os Produtos</h4>
                                                </div>
                                                <div class="modal-body">

                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_VincularTodosProdutos" />

                                                    <div class="row">
                                                        <div class="col-lg-12 form-group">
                                                            <label>Produtos, não cadastrados e presentes no Orçamento, para serem Vinculados</label>
                                                            <asp:ListBox runat="server" ID="lstProdutos_NaoCadastrados" SelectionMode="Multiple" CssClass="Caixa_Selecao"></asp:ListBox>
                                                        </div>
                                                        <div class="col-lg-12 form-group">
                                                            <label>Produto para Vincular</label>
                                                            <asp:DropDownList runat="server" ID="ddlVincularTodosProdutos" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="modal-footer">
                                                        <asp:Button ID="cmdVincularTodosProdutos" class="btn btn-success" runat="server" Text="Vincular" OnClick="cmdVincularTodosProdutos_Click" />
                                                        <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="modal fade" id="modalVincularProdutos" data-backdrop="static">
                                        <div class="modal-dialog" style="width: 50%;">
                                            <div class="modal-content" style="overflow: visible !important;">
                                                <div class="modal-header">
                                                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                                                    <h4 class="modal-title">Vincular Produto</h4>
                                                </div>
                                                <div class="modal-body">

                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_VincularProdutos" />

                                                    <div class="row">
                                                        <div class="col-lg-12 form-group">
                                                            <label>Produto não Cadastrado</label>
                                                            <asp:TextBox runat="server" ID="txtVincularProduto" class="form-control" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                        <div class="col-lg-12 form-group">
                                                            <label>Produto a ser Vinculado</label>
                                                            <asp:DropDownList runat="server" ID="ddlVincularProduto" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                    <div class="modal-footer">
                                                        <asp:Button ID="cmdVincularProduto" class="btn btn-success" runat="server" Text="Vincular" OnClick="cmdVincularProduto_Click" />
                                                        <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                    </div>

                                    <div id="dialog-ExcluirTodosProdutos" class="modal" style="padding: 10px;" title="Excluir">
                                        <b>Deseja excluir todos os Produtos selecionados?</b>
                                    </div>

                                    <div id="dialog-UnificarProdutosDuplicados" class="modal" style="padding: 10px;" title="Unificar Produto">
                                        <b></b>
                                    </div>

                                    <div id="dialog-UnificarTodosProdutosDuplicados" class="modal" style="padding: 10px;" title="Unificar Todos">
                                        <b></b>
                                    </div>

                                    <div runat="server" id="div_Salvar_Sempre_4" class="col-lg-12">
                                        <fieldset class="form-stacked actions">
                                            <asp:Button ID="cmdSalvar_Sempre_4" class="btn btn-lg btn-success" runat="server" Text="Salvar" />
                                            <input type="submit" name="cancel" runat="server" class="btn btn-lg btn-warning" value="Cancelar" id="field_Cancel_4" title="Voltar" onclick="history.go(-1)">
                                        </fieldset>
                                    </div>

                                </div>
                            </div>
                        </div>

                    </asp:Panel>

                    <asp:Panel runat="server" ID="pn5" class="painel desaparece2">

                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_View" />

                        <div class="panel panel-default" runat="server" id="div_pn5">
                            <div class="panel-heading">
                                <div style="padding: 0; width: 100%; display: flex;">
                                    <div style="padding: 0; width: 90%">
                                        <h3 class="panel-title"><b>Visualização Geral e Confirmação</b></h3>
                                    </div>
                                    <div style="display: flex; justify-content: right; padding: 0; width: 10%">
                                        <asp:LinkButton runat="server" ID="cmdAtualiza" OnClick="cmdAtualiza_Click" class="form-control btn-info" Style="width: 22.5%" data-toggle="tooltip" title="Atualizar Informações do Orçamento"><i class="fa fa-refresh"></i></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <div class="panel-body">

                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaDentro_View" />
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina_ProdutosRegras_Fixa" />

                                <div id="abas">
                                    <ul id="ul_tab_View" class="nav nav-tabs" role="tablist">

                                        <li role="presentation" class="tabpanel active" runat="server" id="aba_Orcamento">
                                            <a href="#Orcamento" id="aba-Orcamento" role="tab" data-toggle="tab" aria-controls="Orcamento" aria-expanded="false"><b>Orçamento</b></a>
                                        </li>
                                        <li role="presentation" id="aba_CheckList_View" runat="server">
                                            <a href="#CheckList_View" role="tab" id="aba-CheckList_View" data-toggle="tab" aria-controls="CheckList_View"><b>Escopos</b></a>
                                        </li>
                                        <li role="presentation" id="aba_Servicos_Recursos_View" runat="server">
                                            <a href="#Servicos_Recursos_View" role="tab" id="aba-Servicos_Recursos_View" data-toggle="tab" aria-controls="Servicos_Recursos_View"><b>Serviços</b></a>
                                        </li>
                                        <li role="presentation" id="aba_Produtos" runat="server">
                                            <a href="#Produtos" role="tab" id="aba-Produtos" data-toggle="tab" aria-controls="Produtos"><b>Produtos</b></a>
                                        </li>
                                        <li role="presentation" id="aba_Documentos" runat="server">
                                            <a href="#Documentos" role="tab" id="aba-Documentos" data-toggle="tab" aria-controls="Documentos"><b>Documentos</b></a>
                                        </li>
                                        <li role="presentation" id="aba_Comparativos" runat="server">
                                            <a href="#Comparativos" role="tab" id="aba-Comparativos" data-toggle="tab" aria-controls="Comparativos"><b>Comparativos</b></a>
                                        </li>
                                        <li role="presentation" id="aba_Historico" runat="server">
                                            <a href="#Historico" role="tab" id="aba-Historico" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                                        </li>

                                    </ul>
                                </div>

                                <br />

                                <div id="tab_View" class="tab-content">

                                    <div role="tabpanel" class="tab-pane fade in active" id="Orcamento" aria-labelledby="Orcamento-tab">

                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Orcamento_View_Fixa" />

                                        <div class="row">

                                            <div runat="server" id="div_Orcamento_id">

                                                <div class="col-xs-2">
                                                    <div class="form-group">
                                                        <label>ID</label>

                                                        <div class="form-group input-group">

                                                            <asp:LinkButton ID="cmdRetornar" class="input-group-addon colorLink" runat="server" OnClick="cmdRetornar_click"><b><</b></asp:LinkButton>

                                                            <asp:TextBox runat="server" ID="txtID_Orcamento" class="form-control" ReadOnly="true"></asp:TextBox>

                                                            <asp:LinkButton ID="cmdAvancar" class="input-group-addon colorLink" runat="server" OnClick="cmdAvancar_click"><b>></b></asp:LinkButton>

                                                        </div>

                                                    </div>
                                                </div>

                                                <div class="col-xs-2">
                                                    <div class="form-group">
                                                        <label>Número</label>
                                                        <asp:TextBox runat="server" ID="txtNumero_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2 form-group">
                                                    <label>N° Controle TT</label>
                                                    <asp:TextBox runat="server" ID="txtControle_TT_id" class="form-control" ReadOnly="true"></asp:TextBox>
                                                </div>

                                                <div class="col-lg-5">
                                                    <div class="form-group">
                                                        <label>Referência</label>
                                                        <asp:TextBox runat="server" ID="txtreferencia_id" class="form-control" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-lg-2 form-group">
                                                    <label>Status</label>
                                                    <asp:DropDownList runat="server" ID="ddlStatus" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlStatus_SelectedIndexChanged"></asp:DropDownList>
                                                </div>

                                            </div>

                                            <div runat="server" id="div_Orcamento_View" class="col-lg-12">

                                                <div class="form-group">

                                                    <div class="panel panel-default" runat="server" id="div_panel_InfoInicial_View">
                                                        <div class="panel-heading">
                                                            <h3 class="panel-title"><b>Cadastro Inicial</b></h3>
                                                        </div>
                                                        <div class="panel-body">

                                                            <div class="row">

                                                                <div class="col-lg-12" style="padding: 0;">

                                                                    <div class="col-lg-3">
                                                                        <div class="form-group">
                                                                            <label>CNPJ do Parceiro</label>
                                                                            <asp:TextBox runat="server" ID="txtCNPJ_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-9">
                                                                        <div class="form-group">
                                                                            <label>Razão Social do Parceiro</label>
                                                                            <asp:TextBox runat="server" ID="txtRazaoSocial_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                </div>

                                                                <div class="col-lg-12" style="padding: 0;">

                                                                    <div class="col-lg-8">
                                                                        <div class="form-group">
                                                                            <label>Logradouro Fiscal</label>
                                                                            <asp:TextBox runat="server" ID="txtEndereco_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2">
                                                                        <div class="form-group">
                                                                            <label>UF Fiscal</label>
                                                                            <asp:TextBox runat="server" ID="txtUF_Fiscal_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2">
                                                                        <div class="form-group">
                                                                            <label>Município Fiscal</label>
                                                                            <asp:TextBox runat="server" ID="txtMunicipioFiscal_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                </div>

                                                                <div class="col-lg-12" style="padding: 0;">

                                                                    <div class="col-lg-8">
                                                                        <div class="form-group">
                                                                            <label>Logradouro de Entrega</label>
                                                                            <asp:TextBox runat="server" ID="txtEnderecoEntrega_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2">
                                                                        <div class="form-group">
                                                                            <label>UF Entrega</label>
                                                                            <asp:TextBox runat="server" ID="txtUF_Entrega_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2">
                                                                        <div class="form-group">
                                                                            <label>Município Entrega</label>
                                                                            <asp:TextBox runat="server" ID="txtMunicipio_Entrega_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                </div>

                                                                <div class="col-lg-12" style="padding: 0;">

                                                                    <div class="col-lg-3">
                                                                        <div class="form-group">
                                                                            <label>IE</label>
                                                                            <asp:TextBox runat="server" ID="txtIE_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2">
                                                                        <div class="form-group">
                                                                            <label>Tipo de Venda</label>
                                                                            <asp:TextBox runat="server" ID="txtDestinoVenda_View" class="form-control" ReadOnly="true" Text="Consumo"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div runat="server" id="div_Moeda_View" class="col-lg-5 padd-0">
                                                                        <div runat="server" class="col-lg-6 form-group">
                                                                            <uc1:TextBox_Padrao runat="server" ID="txtMoeda_View" Titulo="Moeda" ReadOnly="true" />
                                                                        </div>

                                                                        <div class="col-lg-6 form-group">
                                                                            <uc1:TextBox_Padrao runat="server" ID="txtCambio_View" Titulo="Taxa de Câmbio" ReadOnly="true" />
                                                                        </div>
                                                                    </div>

                                                                </div>

                                                                <div class="col-lg-12" style="padding: 0;">

                                                                    <div class="col-lg-3 form-group">
                                                                        <label>Contato do Cliente</label>
                                                                        <asp:TextBox runat="server" ID="txtContato_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                    </div>

                                                                    <div class="col-lg-2 form-group">
                                                                        <label>Tipo de Cliente</label>
                                                                        <asp:TextBox runat="server" ID="txtTipoCliente_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                    </div>

                                                                    <div runat="server" id="div_rptSegmentos_View" class="col-lg-7 form-group" style="padding-left: 0;">
                                                                        <label>Segmentos do Cliente</label>
                                                                        <div class="form-control" style="width: 100%; height: 100%; display: flex; flex-wrap: wrap; justify-content: start; border: none; padding: 0; box-shadow: none;">

                                                                            <asp:Repeater runat="server" ID="rptSegmentos_View" OnItemDataBound="rptSegmentos_View_ItemDataBound">
                                                                                <ItemTemplate>

                                                                                    <asp:Label ID="lblSegmento_View" runat="server" TabIndex="1000" class="segmento" ClientIDMode="Static"></asp:Label>

                                                                                </ItemTemplate>
                                                                            </asp:Repeater>

                                                                        </div>
                                                                    </div>

                                                                </div>

                                                                <div class="col-lg-12" style="padding: 0;">

                                                                    <div class="col-lg-3">
                                                                        <div class="form-group">
                                                                            <label>Tabela de Preço</label>
                                                                            <asp:TextBox runat="server" ID="txtTabela_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div runat="server" id="div_Tabela_Obs_View" class="col-lg-9">
                                                                        <div class="form-group">
                                                                            <label>Motivo da Alteração da Tabela de Preço</label>
                                                                            <asp:TextBox runat="server" ID="txtTabela_Obs_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                </div>

                                                                <div class="col-lg-12" style="padding: 0;">

                                                                    <div class="col-lg-3">
                                                                        <div class="form-group">
                                                                            <label>Tipo de Orçamento</label>
                                                                            <asp:TextBox runat="server" ID="txtTipoOrcamento_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-2">
                                                                        <div class="form-group">
                                                                            <label>Fluxo</label>
                                                                            <asp:TextBox runat="server" ID="txtFluxo_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div runat="server" id="div_rptTiposServicos_View" class="col-lg-3 form-group" style="padding-left: 0;">
                                                                        <label>Tipos de Serviços</label>
                                                                        <div class="form-control" style="width: 100%; height: 100%; display: flex; flex-wrap: wrap; justify-content: start; border: none; padding: 0; box-shadow: none;">

                                                                            <asp:Repeater runat="server" ID="rptTiposServicos_View" OnItemDataBound="rptTiposServicos_View_ItemDataBound">
                                                                                <ItemTemplate>

                                                                                    <asp:Label ID="lblTipo_View" runat="server" TabIndex="1000" class="tag" ClientIDMode="Static"></asp:Label>

                                                                                </ItemTemplate>
                                                                            </asp:Repeater>

                                                                        </div>
                                                                    </div>

                                                                    <div runat="server" id="div_rptEscopos_View" class="col-lg-4 form-group" style="padding-left: 0;">
                                                                        <label>Escopos</label>
                                                                        <div class="form-control" style="width: 100%; height: 100%; display: flex; flex-wrap: wrap; justify-content: start; border: none; padding: 0; box-shadow: none;">

                                                                            <asp:Repeater runat="server" ID="rptEscopos_View" OnItemDataBound="rptEscopos_View_ItemDataBound">
                                                                                <ItemTemplate>

                                                                                    <asp:Label ID="lblEscopo_View" runat="server" TabIndex="1000" class="tag" ClientIDMode="Static"></asp:Label>

                                                                                </ItemTemplate>
                                                                            </asp:Repeater>

                                                                        </div>
                                                                    </div>

                                                                </div>

                                                                <div class="col-lg-12" style="padding: 0;">

                                                                    <div class="col-lg-3">
                                                                        <div class="form-group">
                                                                            <label>Empresa</label>
                                                                            <asp:TextBox runat="server" ID="txtEmpresa_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-1">
                                                                        <div class="form-group">
                                                                            <label>UF</label>
                                                                            <asp:TextBox runat="server" ID="txtUF_Origem_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                </div>

                                                                <div class="col-lg-12" style="padding: 0;">

                                                                    <div class="col-lg-1">
                                                                        <div class="form-group">
                                                                            <label>Confidencial</label>
                                                                            <asp:TextBox runat="server" ID="txtConfidencial_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-1" runat="server" id="div_Revisao_View">
                                                                        <div class="form-group">
                                                                            <label>Revisão</label>
                                                                            <asp:TextBox runat="server" ID="txtRevisao_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                </div>

                                                            </div>

                                                            <div class="row">
                                                                <div class="col-lg-2">
                                                                    <div class="form-group">
                                                                        <label>Total Produtos + ST</label>
                                                                        <asp:TextBox runat="server" ID="txtTotalProdutos_View" class="form-control totalGeral" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-2">
                                                                    <div class="form-group">
                                                                        <label>Total Serviços</label>
                                                                        <asp:TextBox runat="server" ID="txtTotalServicos_View" class="form-control totalGeral" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-2">
                                                                    <div class="form-group">
                                                                        <label>Total Projetos</label>
                                                                        <asp:TextBox runat="server" ID="txtTotalProjeto_View" class="form-control totalGeral" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                                <div class="col-lg-2">
                                                                    <div class="form-group">
                                                                        <label>Total Orçamento</label>
                                                                        <asp:TextBox runat="server" ID="txtTotal_View" class="form-control totalGeral" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>

                                                    <div class="panel panel-default" runat="server" id="div_panelFinalizacao_View">
                                                        <div class="panel-heading">
                                                            <h3 class="panel-title"><b>Informações do Orçamento</b></h3>
                                                        </div>
                                                        <div class="panel-body">

                                                            <div class="form-group">

                                                                <div class="row">
                                                                    <div class="col-lg-2 form-group">
                                                                        <label>N° Controle TT</label>
                                                                        <asp:TextBox runat="server" ID="txtControle_TT_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                    </div>
                                                                    <div class="col-lg-4">
                                                                        <div class="form-group">
                                                                            <label>Referência</label>
                                                                            <asp:TextBox runat="server" ID="txtReferencia_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="row">
                                                                    <div class="col-lg-3">
                                                                        <div class="form-group">
                                                                            <label>Condição de Pagamento</label>
                                                                            <asp:TextBox runat="server" ID="txtPagamento_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div runat="server" id="div_NovaCondPgto_Motivo_View" class="col-lg-5 form-group">
                                                                        <label>Motivo da Alteração da Cond. de Pgto.</label>
                                                                        <asp:TextBox runat="server" ID="txtNovaCondPgto_Motivo_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                    </div>

                                                                    <div class="col-lg-2">
                                                                        <div class="form-group">
                                                                            <label>Vendedor</label>
                                                                            <asp:TextBox runat="server" ID="txtVendedor_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>
                                                                </div>

                                                                <div class="row">
                                                                    <div class="col-lg-3">
                                                                        <div class="form-group">
                                                                            <label>Forma de Envio</label>
                                                                            <asp:TextBox runat="server" ID="txtFormaEnvio_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-3">
                                                                        <div class="form-group">
                                                                            <label>Empresa de Transporte</label>
                                                                            <asp:TextBox runat="server" ID="txtTransporte_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                        </div>
                                                                    </div>

                                                                    <div class="col-lg-6" style="padding: 0;">

                                                                        <div class="col-lg-3">
                                                                            <div class="form-group">
                                                                                <label>Estimativa de Entrega</label>
                                                                                <asp:TextBox runat="server" ID="txtEstimativa_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                            </div>
                                                                        </div>

                                                                        <div class="col-lg-3">
                                                                            <div class="form-group">
                                                                                <label>Dias Prev.</label>
                                                                                <asp:TextBox runat="server" ID="txtDiasPrevisao_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                            </div>
                                                                        </div>

                                                                        <div class="col-lg-3">
                                                                            <div class="form-group">
                                                                                <label>Validade em Dias</label>
                                                                                <asp:TextBox runat="server" ID="txtValidade_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                            </div>
                                                                        </div>

                                                                        <div class="col-lg-3">
                                                                            <div class="form-group">
                                                                                <label>Frete</label>
                                                                                <asp:TextBox runat="server" ID="txtFrete_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                            </div>
                                                                        </div>

                                                                    </div>

                                                                </div>

                                                                <div class="row">

                                                                    <div class="col-lg-6 form-group">
                                                                        <label>Instalador</label>
                                                                        <asp:TextBox runat="server" ID="txtInstalador_View" class="form-control" ReadOnly="true"></asp:TextBox>
                                                                    </div>

                                                                    <div runat="server" id="div_Custo_View" class="col-lg-4 padd-0">
                                                                        <div class="col-lg-6 form-group">
                                                                            <uc1:TextBox_Padrao runat="server" ID="txtCusto_Aduaneiro_View" Titulo="Custo Aduaneiro" ReadOnly="true" />
                                                                        </div>

                                                                        <div class="col-lg-6 form-group">
                                                                            <uc1:TextBox_Padrao runat="server" ID="txtCusto_Despachante_View" Titulo="Custo Despachante" ReadOnly="true" />
                                                                        </div>
                                                                    </div>

                                                                </div>

                                                            </div>

                                                            <div class="row">
                                                                <div class="col-lg-12">
                                                                    <div class="form-group">
                                                                        <label>Observações</label>
                                                                        <asp:TextBox runat="server" ID="txtObs_View" class="form-control" TextMode="MultiLine" ReadOnly="true" Height="200px"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>

                                                        </div>
                                                    </div>

                                                </div>

                                            </div>

                                        </div>

                                        <span runat="server" id="spanSalvar_Obs"><b>Obs: Não é necessário que todos os campos e Etapas estejam preenchidas, em caso de dúvidas, por favor, Consulte o Manual.</b></span>

                                        <fieldset class="form-stacked actions">

                                            <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" />
                                            <asp:Button ID="cmdEditar" class="btn btn-lg btn-success" runat="server" Text="Editar" />
                                            <input type="submit" name="cancel" runat="server" class="btn btn-lg btn-warning" value="Cancelar" id="field_cancel" title="Voltar" onclick="history.go(-1)">

                                            <div class="btn-group dropup">
                                                <button id="cmdVincular_group" runat="server" type="button" class="btn btn-lg btn-primary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Vincular CRM  <span class="caret"></span></button>
                                                <ul class="dropdown-menu">
                                                    <li>
                                                        <asp:LinkButton ID="cmdVincularNovo" class="btn btn-lg btn-success" runat="server" Text="Novo CRM" OnClick="cmdVincularNovo_Click"></asp:LinkButton></li>
                                                    <li>
                                                        <asp:LinkButton ID="cmdVincularExistente" class="btn btn-lg btn-primary crm" runat="server" Text="CRM Existente"></asp:LinkButton></li>
                                                </ul>
                                            </div>

                                            <div class="btn-group dropup">
                                                <button id="cmdVincular_Pedido_group" runat="server" type="button" class="btn btn-lg btn-danger dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Vincular Pedido  <span class="caret"></span></button>
                                                <ul class="dropdown-menu">
                                                    <li>
                                                        <asp:LinkButton ID="cmdVincular_Pedido" class="btn btn-lg btn-warning pedido novo" runat="server">Novo Pedido</asp:LinkButton></li>
                                                    <li>
                                                        <asp:LinkButton ID="cmdVincular_Pedido_Existente" class="btn btn-lg btn-danger pedido" runat="server">Pedido Existente</asp:LinkButton></li>
                                                </ul>
                                            </div>

                                            <asp:HyperLink ID="cmdCotacao" class="btn btn-lg btn-primary" Style="float: right;" runat="server" Target="_blank">Cotação <i class="fa fa-arrow-right"></i></asp:HyperLink>
                                            <asp:HyperLink ID="cmdCRM" class="btn btn-lg btn-primary" Style="float: right;" runat="server" Target="_blank">CRM <i class="fa fa-arrow-right"></i></asp:HyperLink>
                                            <asp:HyperLink ID="cmdPedido" class="btn btn-lg btn-primary" Style="float: right;" runat="server" Target="_blank">Pedido <i class="fa fa-arrow-right"></i></asp:HyperLink>

                                        </fieldset>

                                    </div>

                                    <div role="tabpanel" class="tab-pane fade" id="CheckList_View" aria-labelledby="CheckList_View-tab">

                                        <div runat="server" id="div_CheckList_View" class="col-lg-12 row">

                                            <div runat="server" id="div_rptCheckList_View" class="row">
                                                <div class="col-lg-12">
                                                    <div>

                                                        <asp:Repeater runat="server" ID="rptCheckList_View" OnItemDataBound="rptCheckList_View_ItemDataBound">
                                                            <ItemTemplate>

                                                                <asp:GridView ID="gvCheckList_View" class="table table-striped table-bordered table-hover checklist"
                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idEscopo, idCategoria, sDscCategoria" OnRowDataBound="gvCheckList_View_RowDataBound" OnRowCreated="gvCheckList_View_RowCreated">
                                                                    <Columns>

                                                                        <asp:BoundField DataField="idEscopo" HeaderText="Escopo">
                                                                            <HeaderStyle CssClass="id escopo" />
                                                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id escopo" />
                                                                        </asp:BoundField>

                                                                        <asp:BoundField DataField="idCategoria" HeaderText="Categoria">
                                                                            <HeaderStyle CssClass="id categoria" />
                                                                            <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id categoria" />
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

                                        <span runat="server" id="spanSalvar_Obs_2"><b>Obs: Não é necessário que todos os campos e Etapas estejam preenchidas, em caso de dúvidas, por favor, Consulte o Manual.</b></span>

                                        <fieldset class="form-stacked actions">

                                            <asp:Button ID="cmdSalvar_View_CheckList" class="btn btn-lg btn-success" runat="server" Text="Salvar" />
                                            <asp:Button ID="cmdEditar_View_CheckList" class="btn btn-lg btn-success" runat="server" Text="Editar" />
                                            <input type="submit" name="cancel" runat="server" class="btn btn-lg btn-warning" value="Cancelar" id="voltar_1" title="Voltar" onclick="history.go(-1)">
                                        </fieldset>

                                    </div>

                                    <div role="tabpanel" class="tab-pane fade" id="Servicos_Recursos_View" aria-labelledby="Servicos_Recursos_View-tab">

                                        <div runat="server" id="div_Servicos_Recursos_View" class="col-lg-12 row">

                                            <div class="row">
                                                <div class="col-lg-2 form-group">
                                                    <label>Valor Total dos Serviços</label>
                                                    <asp:TextBox runat="server" ID="txtTotalServico_Recurso_View" class="form-control totalGeral" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div runat="server" id="div_gvServicos_Recursos_View" class="row">
                                                <div class="col-lg-12">

                                                    <asp:GridView ID="gvServicos_Recursos_View" class="table table-striped table-bordered table-hover "
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvServicos_Recursos_View_RowDataBound">
                                                        <Columns>

                                                            <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                                <HeaderStyle CssClass="id" />
                                                                <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Item">
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lblnOrdem_View" Text='<%# Eval("nOrdem") %>'></asp:Label>
                                                                    <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_1_Servico_View" + Eval("idRegistro") %>'></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                DataTextField="sCodigo" HeaderText="Código"
                                                                DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                            </asp:HyperLinkField>

                                                            <asp:TemplateField HeaderText="Descrição">
                                                                <ItemTemplate>
                                                                    <div>
                                                                        <asp:LinkButton ID="lnkCard" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                            OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"servico_View\");" %>'
                                                                            OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"servico_View\");" %>'
                                                                            CssClass="produto-detalhe-link"
                                                                            ClientIDMode="Static"
                                                                            data-idproduto='<%# Eval("idItem") %>'
                                                                            data-tabela="produto" />

                                                                        <div id='<%# Eval("idItem") + "_servico_View" %>' class="product-card" style="display: none;">
                                                                            <!--conteudo via script -->
                                                                        </div>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="TipoProduto" HeaderText="Tipo">
                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade">
                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="dtInclusao" HeaderText="Prazo">
                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="Preco" HeaderText="Valor Unitário">
                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="valor" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="NMargem" HeaderText="Margem">
                                                                <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="valor" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="NFator" HeaderText="Desconto">
                                                                <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="dto" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Ajuste">
                                                                <ItemTemplate>
                                                                    <div runat="server" id="divServico_Comparativo" class="" style="display: inline; padding: 1px; width: 100%; justify-content: center; vertical-align: middle;">
                                                                        <i runat="server" id="iconeServico_Comparativo" class="fa fa-plus"></i>
                                                                        &nbsp;
                                                                            <asp:Label runat="server" ID="nAjusteServicos_Comparativos" ClientIDMode="Static" Text='<%# Bind("NAjuste", "{0:N2}") %>' class="valor"></asp:Label>
                                                                        &nbsp;%
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Total">
                                                                <ItemTemplate>

                                                                    <asp:Label runat="server" ID="nTotal" Text='<%# Bind("NTotal", "{0:N2}") %>' class="total"></asp:Label>

                                                                    <%# NovaLinha(Eval("idRegistro"), "_1_Servico_View") %>

                                                                    <asp:GridView ID="gvComposicao_Servicos_View_1" class="table table-striped table-bordered table-hover " DataKeyNames="idItem"
                                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvComposicao_Servicos_View_1_RowDataBound">
                                                                        <Columns>

                                                                            <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                                                <HeaderStyle CssClass="id" />
                                                                                <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id" />
                                                                            </asp:BoundField>

                                                                            <asp:TemplateField HeaderText="ID">
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblnOrdem_View" Text='<%# Eval("idItem") %>'></asp:Label>
                                                                                    <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_2_Servico_View" + Eval("idRegistro") %>'></asp:LinkButton>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>

                                                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                DataTextField="sCodigo" HeaderText="Código"
                                                                                DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                                            </asp:HyperLinkField>

                                                                            <asp:TemplateField HeaderText="Descrição">
                                                                                <ItemTemplate>
                                                                                    <div>
                                                                                        <asp:LinkButton ID="lnkCard" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                                            OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"subServico_View\");" %>'
                                                                                            OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"subServico_View\");" %>'
                                                                                            CssClass="produto-detalhe-link"
                                                                                            ClientIDMode="Static"
                                                                                            data-idproduto='<%# Eval("idItem") %>'
                                                                                            data-tabela="produto" />

                                                                                        <div id='<%# Eval("idItem") + "_subServico_View" %>' class="product-card" style="display: none;">
                                                                                            <!--conteudo via script -->
                                                                                        </div>
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>

                                                                            <asp:BoundField DataField="TipoProduto" HeaderText="Tipo">
                                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade">
                                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="Preco" HeaderText="Valor Unitário">
                                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="valor" />
                                                                            </asp:BoundField>

                                                                            <asp:TemplateField HeaderText="Total de Custo">
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblTotalCusto" Text='<%# Math.Round(decimal.Parse(Eval("Preco").ToString()) * decimal.Parse(Eval("NQuantidade").ToString()), 2) %>'></asp:Label>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>

                                                                            <asp:BoundField DataField="NMargem" HeaderText="Margem">
                                                                                <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="valor" />
                                                                            </asp:BoundField>

                                                                            <asp:TemplateField HeaderText="Total de Venda">
                                                                                <ItemTemplate>

                                                                                    <asp:Label runat="server" ID="nTotal" Text='<%# Bind("NTotal", "{0:N2}") %>' class="total"></asp:Label>

                                                                                    <%# NovaLinha(Eval("idRegistro"), "_2_Servico_View") %>

                                                                                    <asp:GridView ID="gvComposicao_Servicos_View_2" class="table table-striped table-bordered table-hover " DataKeyNames="idItem"
                                                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvComposicao_Servicos_View_2_RowDataBound">
                                                                                        <Columns>

                                                                                            <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                                                                <HeaderStyle CssClass="id" />
                                                                                                <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id" />
                                                                                            </asp:BoundField>

                                                                                            <asp:TemplateField HeaderText="ID">
                                                                                                <ItemTemplate>
                                                                                                    <asp:Label runat="server" ID="lblnOrdem_View" Text='<%# Eval("idItem") %>'></asp:Label>
                                                                                                    <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_3_Servico_View" + Eval("idRegistro") %>'></asp:LinkButton>
                                                                                                </ItemTemplate>
                                                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                            </asp:TemplateField>

                                                                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                                DataTextField="sCodigo" HeaderText="Código"
                                                                                                DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                                                            </asp:HyperLinkField>

                                                                                            <asp:TemplateField HeaderText="Descrição">
                                                                                                <ItemTemplate>
                                                                                                    <div>
                                                                                                        <asp:LinkButton ID="lnkCard" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                                                            OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"recurso_1_View\");" %>'
                                                                                                            OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"recurso_1_View\");" %>'
                                                                                                            CssClass="produto-detalhe-link"
                                                                                                            ClientIDMode="Static"
                                                                                                            data-idproduto='<%# Eval("idItem") %>'
                                                                                                            data-tabela="produto" />

                                                                                                        <div id='<%# Eval("idItem") + "_recurso_1_View" %>' class="product-card" style="display: none;">
                                                                                                            <!--conteudo via script -->
                                                                                                        </div>
                                                                                                    </div>
                                                                                                </ItemTemplate>
                                                                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                            </asp:TemplateField>

                                                                                            <asp:BoundField DataField="TipoProduto" HeaderText="Tipo">
                                                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                            </asp:BoundField>

                                                                                            <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                            </asp:BoundField>

                                                                                            <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade">
                                                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                            </asp:BoundField>

                                                                                            <asp:BoundField DataField="Preco" HeaderText="Valor Unitário">
                                                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="valor" />
                                                                                            </asp:BoundField>

                                                                                            <asp:TemplateField HeaderText="Total de Custo">
                                                                                                <ItemTemplate>
                                                                                                    <asp:Label runat="server" ID="lblTotalCusto" Text='<%# Math.Round(decimal.Parse(Eval("Preco").ToString()) * decimal.Parse(Eval("NQuantidade").ToString()), 2) %>'></asp:Label>
                                                                                                </ItemTemplate>
                                                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                            </asp:TemplateField>

                                                                                            <asp:BoundField DataField="NMargem" HeaderText="Margem">
                                                                                                <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="valor" />
                                                                                            </asp:BoundField>

                                                                                            <asp:TemplateField HeaderText="Total de Venda">
                                                                                                <ItemTemplate>

                                                                                                    <asp:Label runat="server" ID="nTotal" Text='<%# Bind("NTotal", "{0:N2}") %>' class="total"></asp:Label>

                                                                                                    <%# NovaLinha(Eval("idRegistro"), "_3_Servico_View") %>

                                                                                                    <asp:GridView ID="gvComposicao_Servicos_View_3" class="table table-striped table-bordered table-hover " DataKeyNames="idItem"
                                                                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvComposicao_Servicos_View_3_RowDataBound">
                                                                                                        <Columns>

                                                                                                            <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                                                                                <HeaderStyle CssClass="id" />
                                                                                                                <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id" />
                                                                                                            </asp:BoundField>

                                                                                                            <asp:TemplateField HeaderText="ID">
                                                                                                                <ItemTemplate>
                                                                                                                    <asp:Label runat="server" ID="lblnOrdem_View" Text='<%# Eval("idItem") %>'></asp:Label>
                                                                                                                </ItemTemplate>
                                                                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                            </asp:TemplateField>

                                                                                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                                                DataTextField="sCodigo" HeaderText="Código"
                                                                                                                DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                                                                            </asp:HyperLinkField>

                                                                                                            <asp:TemplateField HeaderText="Descrição">
                                                                                                                <ItemTemplate>
                                                                                                                    <div>
                                                                                                                        <asp:LinkButton ID="lnkCard" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                                                                            OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"recurso_2_View\");" %>'
                                                                                                                            OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"recurso_2_View\");" %>'
                                                                                                                            CssClass="produto-detalhe-link"
                                                                                                                            ClientIDMode="Static"
                                                                                                                            data-idproduto='<%# Eval("idItem") %>'
                                                                                                                            data-tabela="produto" />

                                                                                                                        <div id='<%# Eval("idItem") + "_recurso_2_View" %>' class="product-card" style="display: none;">
                                                                                                                            <!--conteudo via script -->
                                                                                                                        </div>
                                                                                                                    </div>
                                                                                                                </ItemTemplate>
                                                                                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                                                            </asp:TemplateField>

                                                                                                            <asp:BoundField DataField="TipoProduto" HeaderText="Tipo">
                                                                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                            </asp:BoundField>

                                                                                                            <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                            </asp:BoundField>

                                                                                                            <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade">
                                                                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                            </asp:BoundField>

                                                                                                            <asp:BoundField DataField="Preco" HeaderText="Valor Unitário">
                                                                                                                <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="valor" />
                                                                                                            </asp:BoundField>

                                                                                                            <asp:TemplateField HeaderText="Total de Custo">
                                                                                                                <ItemTemplate>
                                                                                                                    <asp:Label runat="server" ID="lblTotalCusto" Text='<%# Math.Round(decimal.Parse(Eval("Preco").ToString()) * decimal.Parse(Eval("NQuantidade").ToString()), 2) %>'></asp:Label>
                                                                                                                </ItemTemplate>
                                                                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                            </asp:TemplateField>

                                                                                                            <asp:BoundField DataField="NMargem" HeaderText="Margem">
                                                                                                                <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="valor" />
                                                                                                            </asp:BoundField>

                                                                                                            <asp:BoundField DataField="NTotal" HeaderText="Total de Venda">
                                                                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="total" />
                                                                                                            </asp:BoundField>

                                                                                                        </Columns>
                                                                                                    </asp:GridView>

                                                                                                </ItemTemplate>
                                                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                            </asp:TemplateField>

                                                                                        </Columns>
                                                                                    </asp:GridView>

                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>

                                                                        </Columns>
                                                                    </asp:GridView>

                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                        </Columns>
                                                    </asp:GridView>

                                                </div>
                                            </div>

                                        </div>

                                        <span runat="server" id="spanSalvar_Obs_3"><b>Obs: Não é necessário que todos os campos e Etapas estejam preenchidas, em caso de dúvidas, por favor, Consulte o Manual.</b></span>

                                        <fieldset class="form-stacked actions">

                                            <asp:Button ID="cmdSalvar_View_Servicos" class="btn btn-lg btn-success" runat="server" Text="Salvar" />
                                            <asp:Button ID="cmdEditar_View_Servicos" class="btn btn-lg btn-success" runat="server" Text="Editar" />
                                            <input type="submit" name="cancel" runat="server" class="btn btn-lg btn-warning" value="Cancelar" id="voltar_2" title="Voltar" onclick="history.go(-1)">
                                        </fieldset>

                                    </div>

                                    <div role="tabpanel" class="tab-pane fade" id="Produtos" aria-labelledby="Produtos-tab">

                                        <div runat="server" id="div_Produtos_View">

                                            <div class="row">
                                                <div class="col-lg-2 form-group">
                                                    <label>Total Produtos + ST</label>
                                                    <asp:TextBox runat="server" ID="txtTotalProduto_View" class="form-control totalGeral" ReadOnly="true"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div runat="server" id="div_tbTotalProdutos" class="row">
                                                <div class="col-lg-12">

                                                    <asp:Table runat="server" ID="tbTotalProdutos" class="table table-striped table-bordered table-hover" Style="font-family: Tahoma; font-size: 15px;">

                                                        <asp:TableHeaderRow Style="font-weight: bold;">
                                                            <asp:TableHeaderCell>Valor de ST</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Valor de DIFAL</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Total Líquido</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Valor de COFINS</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Valor de PIS</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Valor de ICMS</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Total sem IPI</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Valor de IPI</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Total com IPI</asp:TableHeaderCell>
                                                        </asp:TableHeaderRow>
                                                        <asp:TableRow>
                                                            <asp:TableCell runat="server" ID="sTotal_ST"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="sTotal_DIFAL"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="sTotalLiquido"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="sTotal_COFINS"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="sTotal_PIS"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="sTotal_ICMS"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="sTotal_sem_IPI"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="sTotal_IPI"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="sTotal_com_IPI"></asp:TableCell>
                                                        </asp:TableRow>

                                                    </asp:Table>

                                                </div>
                                            </div>

                                            <div runat="server" id="div_gvProdutos_View" class="row">
                                                <div class="col-lg-12">

                                                    <asp:GridView ID="gv_Produtos_View" class="table table-striped table-bordered table-hover "
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gv_Produtos_View_RowDataBound">
                                                        <Columns>

                                                            <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                                <HeaderStyle CssClass="id" />
                                                                <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id idItem" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Item">
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="lblnOrdem" Text='<%# Eval("nOrdem") %>' ClientIDMode="Static"></asp:Label>
                                                                    <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_1_Sistema_View" + Eval("idRegistro") %>'></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                DataTextField="sCodigo" HeaderText="Código do Produto"
                                                                DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                            </asp:HyperLinkField>

                                                            <asp:TemplateField HeaderText="Descrição do Produto">
                                                                <ItemTemplate>
                                                                    <div>
                                                                        <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                            OnClientClick='<%# "openModal(\"" + Eval("idItem") + "\"); return false;" %>'
                                                                            OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"produto_View\");" %>'
                                                                            OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"produto_View\");" %>'
                                                                            CssClass="produto-detalhe-link"
                                                                            ClientIDMode="Static"
                                                                            data-idproduto='<%# Eval("idItem") %>'
                                                                            data-tabela="produto" />

                                                                        <div id='<%# Eval("idItem") + "_produto_View" %>' class="product-card" style="display: none;">
                                                                            <!--conteudo via script -->
                                                                        </div>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="sNCM" HeaderText="NCM">
                                                                <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="TipoProduto" HeaderText="Tipo de Produto">
                                                                <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="SUnidade" HeaderText="Unidade Medida">
                                                                <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="NQuantidade" HeaderText="Uds">
                                                                <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sOrigem" HeaderText="Origem">
                                                                <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="sIndustrializado" HeaderText="Ind.">
                                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:BoundField DataField="NFator" HeaderText="Desconto" DataFormatString="{0:N4} %">
                                                                <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Unitário">
                                                                <ItemTemplate>
                                                                    <asp:Label Text='<%# Math.Round(decimal.Parse(Eval("nUnitario").ToString()) - decimal.Parse(Eval("nVlr_IPI").ToString()), 2).ToString("N2") %>' runat="server" ID="lblValorComIPI" ClientIDMode="Static" />
                                                                    <b class='sSimboloMoeda'></b>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="NIPI" HeaderText="IPI" DataFormatString="{0:N2} %">
                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Unitário IPI">
                                                                <ItemTemplate>
                                                                    <asp:Label Text='<%# Math.Round(decimal.Parse(Eval("nUnitario").ToString()), 2).ToString("N2") %>' runat="server" ID="lblValorSemIPI" ClientIDMode="Static" />
                                                                    <b class='sSimboloMoeda'></b>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Total">
                                                                <ItemTemplate>
                                                                    <asp:Label Text='<%# Math.Round((decimal.Parse(Eval("nUnitario").ToString()) - decimal.Parse(Eval("nVlr_IPI").ToString())) * decimal.Parse(Eval("NQuantidade").ToString()), 2).ToString("N2") %>' runat="server" ID="lblTotalSemIPI" ClientIDMode="Static" />
                                                                    <b class='sSimboloMoeda'></b>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Total com IPI">
                                                                <ItemTemplate>
                                                                    <asp:Label runat="server" ID="nTotal" Text='<%# Eval("NTotal", "{0:N2}") %>'></asp:Label>
                                                                    <b class='sSimboloMoeda'></b>

                                                                    <%# NovaLinha(Eval("idRegistro"), "_1_Sistema_View") %>

                                                                    <asp:GridView ID="gv_Produtos_View_1" class="table table-striped table-bordered table-hover "
                                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gv_Produtos_View_1_RowDataBound">
                                                                        <Columns>

                                                                            <asp:BoundField DataField="idItem" HeaderText="ID">
                                                                                <HeaderStyle CssClass="id" />
                                                                                <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id idItem" />
                                                                            </asp:BoundField>

                                                                            <asp:TemplateField HeaderText="Item">
                                                                                <ItemTemplate>
                                                                                    <asp:Label runat="server" ID="lblnOrdem" Text='<%# Eval("nOrdem") %>' ClientIDMode="Static"></asp:Label>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>

                                                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                DataTextField="sCodigo" HeaderText="Código do Produto"
                                                                                DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                                            </asp:HyperLinkField>

                                                                            <asp:TemplateField HeaderText="Descrição do Produto">
                                                                                <ItemTemplate>
                                                                                    <div>
                                                                                        <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                                            OnClientClick='<%# "openModal(\"" + Eval("idItem") + "\"); return false;" %>'
                                                                                            OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"produto_View_1\");" %>'
                                                                                            OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"produto_View_1\");" %>'
                                                                                            CssClass="produto-detalhe-link"
                                                                                            ClientIDMode="Static"
                                                                                            data-idproduto='<%# Eval("idItem") %>'
                                                                                            data-tabela="produto" />

                                                                                        <div id='<%# Eval("idItem") + "_produto_View_1" %>' class="product-card" style="display: none;">
                                                                                            <!--conteudo via script -->
                                                                                        </div>
                                                                                    </div>
                                                                                </ItemTemplate>
                                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                            </asp:TemplateField>

                                                                            <asp:BoundField DataField="sNCM" HeaderText="NCM">
                                                                                <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="TipoProduto" HeaderText="Tipo de Produto">
                                                                                <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="SUnidade" HeaderText="Unidade Medida">
                                                                                <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                            <asp:BoundField DataField="NQuantidade" HeaderText="Uds">
                                                                                <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                            </asp:BoundField>

                                                                        </Columns>
                                                                    </asp:GridView>

                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                        </Columns>
                                                    </asp:GridView>

                                                </div>
                                            </div>

                                        </div>

                                        <span runat="server" id="spanSalvar_Obs_4"><b>Obs: Não é necessário que todos os campos e Etapas estejam preenchidas, em caso de dúvidas, por favor, Consulte o Manual.</b></span>

                                        <fieldset class="form-stacked actions">
                                            <asp:Button ID="cmdSalvar_View_Produtos" class="btn btn-lg btn-success" runat="server" Text="Salvar" />
                                            <asp:Button ID="cmdEditar_View_Produtos" class="btn btn-lg btn-success" runat="server" Text="Editar" />
                                            <input type="submit" name="cancel" runat="server" class="btn btn-lg btn-warning" value="Cancelar" id="voltar_3" title="Voltar" onclick="history.go(-1)">
                                        </fieldset>

                                    </div>

                                    <div role="tabpanel" class="tab-pane fade" id="Documentos" aria-labelledby="Documentos-tab">

                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Documentos_View_Fixa" />

                                        <div class="col-lg-12 row" runat="server" id="div_Documentos_View">

                                            <div class="row">
                                                <div class="col-lg-12 form-group">

                                                    <asp:CheckBox ID="cbMinimo_PDF" Text="Total Líquido" runat="server" CssClass="btn btn-primary" />
                                                    <asp:CheckBox ID="cbEscopos_PDF" Text="Escopos" runat="server" CssClass="btn btn-primary" />
                                                    <asp:CheckBox ID="cbCliente_PDF" Text="Dados de Produtos do Cliente" runat="server" CssClass="btn btn-primary" />
                                                    <asp:CheckBox ID="cbComposicaoSistema_PDF" Text="Exibe Composição de Sistemas" runat="server" CssClass="btn btn-primary" />
                                                    <asp:CheckBox ID="cbLPU" Text="Formato LPU" runat="server" CssClass="btn btn-primary" />
                                                    <asp:CheckBox ID="cb_Frete" Text="Exibe Frete" runat="server" CssClass="btn btn-primary" />

                                                </div>
                                            </div>

                                            <div class="row">
                                                <div class="col-lg-12 form-group">

                                                    <fieldset class="form-stacked actions">

                                                        <asp:Button ID="cmdGeraPDF" class="btn btn-lg btn-danger" runat="server" Text="PDF" OnClick="cmdGeraPDF_Click" />
                                                        <asp:Button ID="cmdGeraExcel" class="btn btn-lg btn-success" runat="server" Text="Excel" OnClick="cmdGeraExcel_Click" />

                                                    </fieldset>

                                                </div>
                                            </div>

                                        </div>

                                    </div>

                                    <div role="tabpanel" class="tab-pane fade" id="Comparativos" aria-labelledby="Comparativos-tab">

                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Comparativos_View_Fixa" />

                                        <div runat="server" id="div_tab_Comparativos">

                                            <div class="row" runat="server" id="div_Comparativo_Totais">

                                                <div class="col-lg-12" style="padding: 0;">

                                                    <div class="col-lg-3">
                                                        <div class="form-group">
                                                            <label>Empresa</label>
                                                            <asp:TextBox runat="server" ID="txtComparativo_Empresa" class="form-control" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-1">
                                                        <div class="form-group">
                                                            <label>UF</label>
                                                            <asp:TextBox runat="server" ID="txtUF_Origem_Comparativo" class="form-control" ReadOnly="true"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>

                                                <div class="col-lg-12" style="padding: 0;">

                                                    <div class="col-lg-2 form-group">
                                                        <label>Total Líquido - Produtos</label>
                                                        <div class="input-group">
                                                            <span id="spanTotalLiquidoProdutos" class="input-group-addon"><b class='sSimboloMoeda'></b></span>
                                                            <asp:TextBox runat="server" ID="txtComparativo_LiquidoProdutos" class="form-control" ReadOnly="true" aria-describedby="spanTotalLiquidoProdutos" Style="z-index: 1;"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2 form-group">
                                                        <label>Total Bruto - Produtos</label>
                                                        <div class="input-group">
                                                            <span id="spanTotalBrutoProdutos" class="input-group-addon"><b class='sSimboloMoeda'></b></span>
                                                            <asp:TextBox runat="server" ID="txtComparativo_BrutoProdutos" class="form-control" ReadOnly="true" aria-describedby="spanTotalBrutoProdutos" Style="z-index: 1;"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2 form-group">
                                                        <label>Total Líquido - Serviços</label>
                                                        <div class="input-group">
                                                            <span id="spanTotalLiquidoServicos" class="input-group-addon"><b class='sSimboloMoeda'></b></span>
                                                            <asp:TextBox runat="server" ID="txtComparativo_LiquidoServicos" class="form-control" ReadOnly="true" aria-describedby="spanTotalLiquidoServicos" Style="z-index: 1;"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2 form-group">
                                                        <label>Total Bruto - Serviços</label>
                                                        <div class="input-group">
                                                            <span id="spanTotalBrutoServicos" class="input-group-addon"><b class='sSimboloMoeda'></b></span>
                                                            <asp:TextBox runat="server" ID="txtComparativo_BrutoServicos" class="form-control" ReadOnly="true" aria-describedby="spanTotalBrutoServicos" Style="z-index: 1;"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2 form-group">
                                                        <label>Total Líquido - Orçamento</label>
                                                        <div class="input-group">
                                                            <span id="spanTotalLiquidoOrcamento" class="input-group-addon"><b class='sSimboloMoeda'></b></span>
                                                            <asp:TextBox runat="server" ID="txtComparativo_LiquidoOrcamento" class="form-control" ReadOnly="true" aria-describedby="spanTotalLiquidoOrcamento" Style="z-index: 1;"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-2 form-group">
                                                        <label>Total Bruto - Orçamento</label>
                                                        <div class="input-group">
                                                            <span id="spanTotalBrutoOrcamento" class="input-group-addon"><b class='sSimboloMoeda'></b></span>
                                                            <asp:TextBox runat="server" ID="txtComparativo_BrutoOrcamento" class="form-control" ReadOnly="true" aria-describedby="spanTotalBrutoOrcamento" Style="z-index: 1;"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                </div>

                                            </div>

                                            <div runat="server" id="div_ImpostosProdutos_Comparativos" class="row">
                                                <div class="col-lg-12">

                                                    <asp:Table runat="server" ID="tbImposotosProdutos_Comparativos" class="table table-striped table-bordered table-hover " Style="font-family: Tahoma; font-size: 15px;">

                                                        <asp:TableHeaderRow Style="font-weight: bold;">
                                                            <asp:TableHeaderCell>Valor de ST</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Valor de DIFAL</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Total Líquido</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Valor de COFINS</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Valor de PIS</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Valor de ICMS</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Total sem IPI</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Valor de IPI</asp:TableHeaderCell>
                                                            <asp:TableHeaderCell>Total com IPI</asp:TableHeaderCell>
                                                        </asp:TableHeaderRow>
                                                        <asp:TableRow>
                                                            <asp:TableCell runat="server" ID="tbcST_ImpostosProdutos_Comparativos"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="tbcDIFAL_ImpostosProdutos_Comparativos"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="tbcLiquido_ImpostosProdutos_Comparativos"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="tbcCOFINS_ImpostosProdutos_Comparativos"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="tbcPIS_ImpostosProdutos_Comparativos"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="tbcICMS_ImpostosProdutos_Comparativos"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="tbcSemIPI_ImpostosProdutos_Comparativos"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="tbcVlr_IPI_ImpostosProdutos_Comparativos"></asp:TableCell>
                                                            <asp:TableCell runat="server" ID="tbcIPI_ImpostosProdutos_Comparativos"></asp:TableCell>
                                                        </asp:TableRow>

                                                    </asp:Table>

                                                </div>
                                            </div>

                                            <div class="panel panel-default" runat="server" id="div_Comparativos">
                                                <div class="panel-heading">
                                                    <div style="padding: 0; width: 100%; display: flex;">
                                                        <div style="padding: 0; width: 90%">
                                                            <h3 class="panel-title"><b>Comparativos</b></h3>
                                                        </div>
                                                        <div style="display: flex; justify-content: right; padding: 0; width: 10%">
                                                            <asp:LinkButton runat="server" ID="cmdAtualiza_Comparativos" OnClick="cmdAtualiza_Click" class="form-control btn-info" Style="width: 22.5%" data-toggle="tooltip" title="Atualizar Informações dos Comparativos"><i class="fa fa-refresh"></i></asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div class="panel-body">

                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Comparativos_Fixa" />
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Comparativos" />

                                                    <div class="row">

                                                        <div class="col-lg-3">
                                                            <div class="form-group">
                                                                <label>Empresa</label>
                                                                <asp:DropDownList runat="server" ID="ddlEmpresa_Comparativo_Ajustado" class="form-control" AutoPostBack="true" OnSelectedIndexChanged="ddlEmpresa_Comparativo_Ajustado_SelectedIndexChanged"></asp:DropDownList>
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-1">
                                                            <div class="form-group">
                                                                <label>UF</label>
                                                                <asp:TextBox runat="server" ID="txtUF_Origem_Comparativo_Ajustado" class="form-control" ReadOnly="true"></asp:TextBox>
                                                            </div>
                                                        </div>

                                                        <div runat="server" id="div_Espaco__Empreitada_Drawback" class="col-lg-3">&nbsp;</div>

                                                        <div class="col-lg-3" runat="server" id="div_cmdEmpreitada">
                                                            <div class="form-group">
                                                                <label>&nbsp;</label>
                                                                <asp:Button runat="server" ID="cmdEmpreitada" OnClick="cmdEmpreitada_Click" CssClass="form-control btn-success" Text="Converter Orçamento em Empreitada" />
                                                            </div>
                                                        </div>

                                                        <div class="col-lg-2">
                                                            <div runat="server" id="div_cmdDrawback" class="col-lg-12 form-group btn-group dropdown" style="padding: 0;">
                                                                <label>&nbsp;</label>
                                                                <button id="cmdAplicarDrawback" runat="server" type="button" class="form-control btn-primary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">Redução de Impostos  <span class="caret"></span></button>
                                                                <ul class="dropdown-menu dropdown-menu-right">

                                                                    <li>
                                                                        <asp:Button runat="server" ID="cmdDrawback_Parcial" OnClick="cmdDrawback_Click" CssClass="form-control btn-danger" Text="Drawback Parcial" /></li>

                                                                    <li>
                                                                        <asp:Button runat="server" ID="cmdDrawback_Integral" OnClick="cmdDrawback_Click" CssClass="form-control btn-success" Text="Drawback Integral" /></li>

                                                                    <li>
                                                                        <asp:Button runat="server" ID="cmdDrawback_ZonaFranca" OnClick="cmdDrawback_Click" CssClass="form-control btn-warning" Text="Zona Franca" /></li>

                                                                </ul>
                                                            </div>
                                                        </div>

                                                    </div>

                                                    <div runat="server" id="div_Comparativos_Ajustado">

                                                        <div class="row" runat="server" id="div_Comparativos_Totais_Ajustado">

                                                            <div class="col-lg-12">
                                                                <div class="form-group">
                                                                    <span runat="server" id="spanComparativos_Totais_Ajustado"><b>Obs: Os campos abaixo representam os Valores Totais, já com as alterações dos Comparativos aplicadas!</b></span>
                                                                </div>
                                                            </div>

                                                            <div class="col-lg-12" style="padding: 0;">

                                                                <div class="col-lg-2 form-group">
                                                                    <uc1:TextBox_Padrao runat="server" ID="txtComparativos_LiquidoProdutos_Ajustado" Classe="ajusteporcentagem" Titulo="Total Líquido - Produtos" MascaraDinamica="$money($input, ',')" Grupo="true" Prefixo="true" Grupo_Simbolo="<b class='sSimboloMoeda'></b>" AutoPostBack="true" OnTextChanged="txtCalculaProdutos_Comparativos_TextChanged" />
                                                                </div>

                                                                <div class="col-lg-2 form-group">
                                                                    <uc1:TextBox_Padrao runat="server" ID="txtComparativos_BrutoProdutos_Ajustado" Titulo="Total Bruto - Produtos" ReadOnly="true" Grupo="true" Prefixo="true" Grupo_Simbolo="<b class='sSimboloMoeda'></b>" />
                                                                </div>

                                                                <div class="col-lg-2 form-group">
                                                                    <uc1:TextBox_Padrao runat="server" ID="txtComparativos_LiquidoServicos_Ajustado" Titulo="Total Líquido - Serviços" ReadOnly="true" Grupo="true" Prefixo="true" Grupo_Simbolo="<b class='sSimboloMoeda'></b>" />
                                                                </div>

                                                                <div class="col-lg-2 form-group">
                                                                    <uc1:TextBox_Padrao runat="server" ID="txtComparativos_BrutoServicos_Ajustado" Titulo="Total Bruto - Serviços" ReadOnly="true" Grupo="true" Prefixo="true" Grupo_Simbolo="<b class='sSimboloMoeda'></b>" />
                                                                </div>

                                                                <div class="col-lg-2 form-group">
                                                                    <uc1:TextBox_Padrao runat="server" ID="txtComparativos_LiquidoOrcamento_Ajustado" Titulo="Total Líquido - Orçamento" ReadOnly="true" Grupo="true" Prefixo="true" Grupo_Simbolo="<b class='sSimboloMoeda'></b>" />
                                                                </div>

                                                                <div class="col-lg-2 form-group">
                                                                    <uc1:TextBox_Padrao runat="server" ID="txtComparativos_BrutoOrcamento_Ajustado" Titulo="Total Bruto - Orçamento" ReadOnly="true" Grupo="true" Prefixo="true" Grupo_Simbolo="<b class='sSimboloMoeda'></b>" />
                                                                </div>

                                                            </div>

                                                        </div>

                                                        <div class="row" runat="server" id="div_Comparativos_Totais_Empreitada">
                                                            <div class="col-lg-12" style="padding: 0;">

                                                                <div class="col-lg-2 form-group">
                                                                    <uc1:TextBox_Padrao runat="server" ID="txtTotalProdutos_Empreitada" Titulo="Total Líquido - Produtos" MascaraDinamica="$money($input, ',')" Classe="ajusteporcentagem" Grupo_Simbolo="<i class='fa fa-percent'></i>" AutoPostBack="true" OnTextChanged="txtCalculaProdutos_Comparativos_TextChanged" />
                                                                </div>

                                                                <div class="col-lg-2 form-group">
                                                                    <uc1:TextBox_Padrao runat="server" ID="txtTotalServicos_Empreitada" Titulo="Total Líquido - Serviços" ReadOnly="true" Grupo_Simbolo="<i class='fa fa-percent'></i>" />
                                                                </div>

                                                                <div class="col-lg-2 form-group">
                                                                    <uc1:TextBox_Padrao runat="server" ID="txtTotalOrcamento_Empreitada" Titulo="Total Líquido - Orçamento" ReadOnly="true" Prefixo="true" Grupo_Simbolo="<b class='sSimboloMoeda'></b>" />
                                                                </div>

                                                            </div>
                                                        </div>

                                                        <div runat="server" id="div_ImpostosProdutos_Comparativos_Ajustado" class="row">
                                                            <div class="col-lg-12">

                                                                <label>Impostos de Produtos</label>
                                                                <asp:Table runat="server" ID="tbImpostosProdutos_Comparativos_Ajustado" class="table table-striped table-bordered table-hover " Style="font-family: Tahoma; font-size: 15px;">

                                                                    <asp:TableHeaderRow Style="font-weight: bold;">
                                                                        <asp:TableHeaderCell>Valor de ST</asp:TableHeaderCell>
                                                                        <asp:TableHeaderCell>Valor de DIFAL</asp:TableHeaderCell>
                                                                        <asp:TableHeaderCell>Total Líquido</asp:TableHeaderCell>
                                                                        <asp:TableHeaderCell>Valor de COFINS</asp:TableHeaderCell>
                                                                        <asp:TableHeaderCell>Valor de PIS</asp:TableHeaderCell>
                                                                        <asp:TableHeaderCell>Valor de ICMS</asp:TableHeaderCell>
                                                                        <asp:TableHeaderCell>Total sem IPI</asp:TableHeaderCell>
                                                                        <asp:TableHeaderCell>Valor de IPI</asp:TableHeaderCell>
                                                                        <asp:TableHeaderCell>Total com IPI</asp:TableHeaderCell>
                                                                    </asp:TableHeaderRow>
                                                                    <asp:TableRow>
                                                                        <asp:TableCell runat="server" ID="tbcST_ImpostosProdutos_Comparativos_Ajustado"></asp:TableCell>
                                                                        <asp:TableCell runat="server" ID="tbcDIFAL_ImpostosProdutos_Comparativos_Ajustado"></asp:TableCell>
                                                                        <asp:TableCell runat="server" ID="tbcLiquido_ImpostosProdutos_Comparativos_Ajustado"></asp:TableCell>
                                                                        <asp:TableCell runat="server" ID="tbcCOFINS_ImpostosProdutos_Comparativos_Ajustado"></asp:TableCell>
                                                                        <asp:TableCell runat="server" ID="tbcPIS_ImpostosProdutos_Comparativos_Ajustado"></asp:TableCell>
                                                                        <asp:TableCell runat="server" ID="tbcICMS_ImpostosProdutos_Comparativos_Ajustado"></asp:TableCell>
                                                                        <asp:TableCell runat="server" ID="tbcSemIPI_ImpostosProdutos_Comparativos_Ajustado"></asp:TableCell>
                                                                        <asp:TableCell runat="server" ID="tbcVlr_IPI_ImpostosProdutos_Comparativos_Ajustado"></asp:TableCell>
                                                                        <asp:TableCell runat="server" ID="tbcIPI_ImpostosProdutos_Comparativos_Ajustado"></asp:TableCell>
                                                                    </asp:TableRow>

                                                                </asp:Table>

                                                            </div>
                                                        </div>

                                                        <div id="abas_Comparativo">
                                                            <ul id="ul_tab_Comparativo" class="nav nav-tabs" role="tablist">

                                                                <li role="presentation" class="tabpanel active" runat="server" id="aba_Comparativo_Produtos">
                                                                    <a href="#Comparativo_Produtos" id="aba-Comparativo_Produtos" role="tab" data-toggle="tab" aria-controls="Produtos" aria-expanded="false"><b>Produtos</b></a>
                                                                </li>
                                                                <li role="presentation" id="aba_Comparativo_Servicos" runat="server">
                                                                    <a href="#Comparativo_Servicos" role="tab" id="aba-Comparativo_Servicos" data-toggle="tab" aria-controls="Servicos"><b>Serviços</b></a>
                                                                </li>
                                                            </ul>
                                                        </div>

                                                        <br />

                                                        <div id="tab_Comparativo" class="tab-content">

                                                            <div role="tabpanel" class="tab-pane fade in active" id="Comparativo_Produtos" aria-labelledby="Comparativo_Produtos-tab">
                                                                <div class="col-lg-12" style="padding: 0;">

                                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Comparativo_Produtos" />

                                                                    <div class="row" runat="server" id="div_ComparativosProdutos">

                                                                        <div class="col-lg-4" style="padding: 0;">
                                                                            <div runat="server" id="div_Desconto_Global_Produtos_Comparativos" class="col-lg-6">
                                                                                <div class="form-group">
                                                                                    <label>Desconto Global (%) <small runat="server" id="smDesconto_Produtos_Comparativos">- Máximo: 5%</small></label>
                                                                                    <asp:TextBox runat="server" ID="txtDesconto_Global_Produtos_Comparativos" Text="0.00" class="form-control desconto" AutoPostBack="true" OnTextChanged="txtDescontoProduto_Comparativos_TextChanged"></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                            <div runat="server" id="div_Prazo_Global_Produtos_Comparativos" class="col-lg-6">
                                                                                <div class="form-group">
                                                                                    <label>Prazo de Entrega em Dias</label>
                                                                                    <asp:TextBox runat="server" ID="txtPrazo_Global_Produtos_Comparativos" Text="15" class="form-control"></asp:TextBox>
                                                                                </div>
                                                                            </div>
                                                                        </div>

                                                                        <div runat="server" id="div_Converter_Comparativo" class="col-lg-8 form-group" style="padding: 0;">

                                                                            <div class="col-lg-9">
                                                                                <label>Sistemas</label>
                                                                                <asp:DropDownList runat="server" ID="ddlSistemas_Comparativo" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                                            </div>

                                                                            <div class="col-lg-3">
                                                                                <label>&nbsp;</label>
                                                                                <asp:Button runat="server" ID="cmdConverter_Sistema_Comparativos" Text="Converter Produtos em Sistema" class="btn-success form-control" OnClick="cmdConverter_Sistema_Comparativos_Click" />
                                                                            </div>

                                                                        </div>

                                                                    </div>

                                                                    <div runat="server" id="div_gvComparativoProdutos" class="col-lg-12" style="padding: 0;">
                                                                        <div class="col-lg-12 form-group" style="padding: 0;">
                                                                            <div>

                                                                                <asp:GridView ID="gvComparativoProdutos" class="table table-striped table-bordered table-hover "
                                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvComparativoProdutos_RowDataBound">
                                                                                    <Columns>

                                                                                        <asp:TemplateField>
                                                                                            <HeaderTemplate>
                                                                                                <asp:CheckBox runat="server" ID="cbComparativo_Todos" class="btn btn-small" ClientIDMode="Static" />
                                                                                            </HeaderTemplate>
                                                                                            <ItemTemplate>
                                                                                                <asp:CheckBox runat="server" ID="cbComparativo" class="btn btn-small" />
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="comparativo" />
                                                                                        </asp:TemplateField>

                                                                                        <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                                                            <HeaderStyle CssClass="id" />
                                                                                            <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id idItem" />
                                                                                        </asp:BoundField>

                                                                                        <asp:TemplateField HeaderText="Item +">
                                                                                            <ItemTemplate>
                                                                                                <asp:TextBox runat="server" ID="txtnOrdem" class="form-control" Text='<%# Eval("nOrdem") %>' ClientIDMode="Static"></asp:TextBox>
                                                                                                <div style="display: flex; padding: 5px;">
                                                                                                    <div style="display: flex; margin-right: 30%;">
                                                                                                        <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-toggle="tooltip" title="Exibir Composição" data-div-id='<%# "tr_1_Sistema_Comparativo" + Eval("idRegistro") %>'></asp:LinkButton>
                                                                                                    </div>
                                                                                                    <div style="display: flex; margin-left: 30%;">
                                                                                                        <asp:LinkButton ID="cmdDuplicarSistema" runat="server" ClientIDMode="Static" CssClass="duplicarSistemaComparativos" data-toggle="tooltip" title="Duplicar Sistema" CommandName="Duplicar" CommandArgument='<%# Eval("idRegistro") %>' OnClick="cmdDuplicarSistema_Click"><i class="fa fa-files-o" style="cursor: pointer;"></i></asp:LinkButton>
                                                                                                    </div>
                                                                                                </div>
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:TemplateField>

                                                                                        <asp:BoundField DataField="nOrdem" HeaderText="Item">
                                                                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:BoundField>

                                                                                        <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                            DataTextField="sCodigo" HeaderText="Código do Produto"
                                                                                            DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                                                        </asp:HyperLinkField>

                                                                                        <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                            DataTextField="sDscProduto" HeaderText="Descrição do Produto"
                                                                                            DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                            <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                                                        </asp:HyperLinkField>

                                                                                        <asp:BoundField DataField="TipoProduto" HeaderText="Tipo de Produto">
                                                                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:BoundField>

                                                                                        <asp:BoundField DataField="dtInclusao" HeaderText="Prazo">
                                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:BoundField>

                                                                                        <asp:TemplateField HeaderText="Prazo">
                                                                                            <ItemTemplate>
                                                                                                <asp:TextBox runat="server" ID="prazo" Text='<%# Bind("dtInclusao") %>' class="form-control prazo prazoComparativo"></asp:TextBox>
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:TemplateField>

                                                                                        <asp:BoundField DataField="SUnidade" HeaderText="Unidade Medida">
                                                                                            <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:BoundField>

                                                                                        <asp:BoundField DataField="NQuantidade" HeaderText="Uds">
                                                                                            <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="qtd" />
                                                                                        </asp:BoundField>

                                                                                        <asp:TemplateField HeaderText="Unitário">
                                                                                            <ItemTemplate>
                                                                                                <asp:Label Text='<%# Math.Round(decimal.Parse(Eval("NIPI").ToString()) > decimal.Zero ?
                                                                                                                                    decimal.Parse(Eval("Preco").ToString()) / (decimal.Parse(Eval("NIPI").ToString()) / 100 + 1)
                                                                                                                                : decimal.Parse(Eval("Preco").ToString()), 2).ToString("N2") %>'
                                                                                                    runat="server" ID="lblValorSemIPI" ClientIDMode="Static" CssClass="valorIPI" />
                                                                                                <b class='sSimboloMoeda'></b>
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:TemplateField>

                                                                                        <asp:BoundField DataField="NIPI" HeaderText="IPI" DataFormatString="{0:N2} %">
                                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:BoundField>

                                                                                        <asp:TemplateField HeaderText="Desconto">
                                                                                            <ItemTemplate>
                                                                                                <div class="input-group">
                                                                                                    <asp:TextBox runat="server" ID="nDto" Text='<%# Bind("nFator", "{0:N4}") %>' ClientIDMode="Static" class="form-control desconto descontoComparativo" aria-describedby="spanSimbolo" Style="z-index: 0;" AutoPostBack="true" OnTextChanged="txtCalculaProdutos_Comparativos_TextChanged"></asp:TextBox>
                                                                                                    <span id="spanSimbolo" class="input-group-addon">%</span>
                                                                                                </div>
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="9%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:TemplateField>

                                                                                        <asp:TemplateField HeaderText="Unitário com IPI e Desconto">
                                                                                            <ItemTemplate>
                                                                                                <uc1:TextBox_Padrao runat="server" ID="lblValorComIPI" Text='<%# Bind("nUnitario", "{0:N2}") %>' IDMode="Static" MascaraDinamica="$money($input, ',')" AutoPostBack="true" OnTextChanged="txtCalculaProdutos_Comparativos_TextChanged" />
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:TemplateField>

                                                                                        <asp:TemplateField HeaderText="Total com IPI">
                                                                                            <ItemTemplate>
                                                                                                <asp:Label runat="server" ID="nTotal" Text='<%# Eval("NTotal", "{0:N2}") %>' CssClass="totalComparativo"></asp:Label>
                                                                                                <b class='sSimboloMoeda'></b>

                                                                                                <%# NovaLinha(Eval("idRegistro"), "_1_Sistema_Comparativo") %>

                                                                                                <asp:GridView ID="gvProdutos_Composicao_Comparativos" class="table table-striped table-bordered table-hover tablePai"
                                                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvProdutos_Composicao_Comparativos_RowDataBound">
                                                                                                    <Columns>

                                                                                                        <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                                                                            <HeaderStyle CssClass="id" />
                                                                                                            <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id idItem" />
                                                                                                        </asp:BoundField>

                                                                                                        <asp:TemplateField HeaderText="Item">
                                                                                                            <ItemTemplate>
                                                                                                                <asp:TextBox runat="server" ID="txtnOrdem" class="form-control" Text='<%# Eval("nOrdem") %>' ClientIDMode="Static"></asp:TextBox>
                                                                                                            </ItemTemplate>
                                                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                        </asp:TemplateField>

                                                                                                        <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                                            DataTextField="sCodigo" HeaderText="Código"
                                                                                                            DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                                                                        </asp:HyperLinkField>

                                                                                                        <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                                            DataTextField="sDscProduto" HeaderText="Descrição"
                                                                                                            DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                                            <ItemStyle Width="55%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="descricao link" />
                                                                                                        </asp:HyperLinkField>

                                                                                                        <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                        </asp:BoundField>

                                                                                                        <asp:TemplateField HeaderText="Quantidade">
                                                                                                            <ItemTemplate>
                                                                                                                <uc1:TextBox_Padrao runat="server" ID="txtnQuantidade" IDMode="Static" Text='<%# Bind("NQuantidade", "{0:N2}") %>' Classe="qtd" MascaraDinamica="$money($input, ',')" />
                                                                                                            </ItemTemplate>
                                                                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                                        </asp:TemplateField>

                                                                                                    </Columns>
                                                                                                </asp:GridView>

                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:TemplateField>

                                                                                    </Columns>
                                                                                </asp:GridView>

                                                                            </div>
                                                                        </div>
                                                                    </div>

                                                                </div>
                                                            </div>

                                                            <div role="tabpanel" class="tab-pane fade" id="Comparativo_Servicos" aria-labelledby="Comparativo_Servicos-tab">
                                                                <div class="col-lg-12" style="padding: 0;" runat="server" id="div_Comparativo_Servicos">

                                                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaServicos_Comparativos" />

                                                                    <div runat="server" id="divIncluirServico_Empreitada" class="row">

                                                                        <div class="col-lg-3 form-group">
                                                                            <label>Código do Serviço</label>
                                                                            <asp:TextBox runat="server" ID="txtIncluirServico_Empreitada_Codigo" class="form-control"></asp:TextBox>
                                                                        </div>
                                                                        <div class="col-lg-7 form-group">
                                                                            <label>Descrição do Serviço</label>
                                                                            <asp:TextBox runat="server" ID="txtIncluirServico_Empreitada_Descricao" class="form-control"></asp:TextBox>
                                                                        </div>
                                                                        <div class="col-lg-2 form-group">
                                                                            <label>&nbsp;</label>
                                                                            <asp:Button runat="server" ID="cmdIncluirServico_Empreitada" class="form-control btn-info" Text="Incluir Serviço" OnClick="cmdIncluirServico_Empreitada_Click"></asp:Button>
                                                                        </div>

                                                                    </div>

                                                                    <div class="row">

                                                                        <div runat="server" id="div_PrazoGlobal_Servicos_Empreitada" class="col-lg-2">
                                                                            <div class="form-group">
                                                                                <label>Prazo de Entrega em Dias</label>
                                                                                <asp:TextBox runat="server" ID="txtPrazo_Global_Servicos_Empreitada" Text="15" class="form-control"></asp:TextBox>
                                                                            </div>
                                                                        </div>

                                                                    </div>

                                                                    <div class="col-lg-12" style="padding: 0;">
                                                                        <div class="col-lg-12 form-group" style="padding: 0;">
                                                                            <div>

                                                                                <asp:GridView ID="gvComparativoServicos" class="table table-striped table-bordered table-hover "
                                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="gvComparativoServicos_RowDataBound" OnRowDeleting="gvComparativoServicos_RowDeleting">
                                                                                    <Columns>

                                                                                        <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                                                            <HeaderStyle CssClass="id" />
                                                                                            <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id" />
                                                                                        </asp:BoundField>

                                                                                        <asp:TemplateField HeaderText="Item">
                                                                                            <ItemTemplate>
                                                                                                <asp:TextBox runat="server" ID="txtnOrdem" class="form-control" Text='<%# Eval("nOrdem") %>' ClientIDMode="Static"></asp:TextBox>
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:TemplateField>

                                                                                        <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                            DataTextField="sCodigo" HeaderText="Código"
                                                                                            DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                                                        </asp:HyperLinkField>

                                                                                        <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                                                            DataTextField="sDscProduto" HeaderText="Descrição"
                                                                                            DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                                            <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                                                        </asp:HyperLinkField>

                                                                                        <asp:BoundField DataField="TipoProduto" HeaderText="Tipo">
                                                                                            <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:BoundField>

                                                                                        <asp:BoundField DataField="SUnidade" HeaderText="Unidade">
                                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:BoundField>

                                                                                        <asp:TemplateField HeaderText="Prazo">
                                                                                            <ItemTemplate>
                                                                                                <asp:TextBox runat="server" ID="prazo" ClientIDMode="Static" Text='<%# Bind("dtInclusao") %>' class="form-control prazo prazoComparativo"></asp:TextBox>
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:TemplateField>

                                                                                        <asp:BoundField DataField="NQuantidade" HeaderText="Quantidade">
                                                                                            <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="qtd qtdServico" />
                                                                                        </asp:BoundField>

                                                                                        <asp:BoundField DataField="Preco" HeaderText="Valor Unitário">
                                                                                            <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="valor valorServico" />
                                                                                        </asp:BoundField>

                                                                                        <asp:BoundField DataField="NMargem" HeaderText="Margem">
                                                                                            <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="valor margemServico" />
                                                                                        </asp:BoundField>

                                                                                        <asp:BoundField DataField="NFator" HeaderText="Desconto">
                                                                                            <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="valor dtoServico" />
                                                                                        </asp:BoundField>

                                                                                        <asp:TemplateField HeaderText="Ajuste">
                                                                                            <ItemTemplate>
                                                                                                <div runat="server" id="divServico_Comparativo" class="" style="display: inline; padding: 1px; width: 100%; justify-content: center; vertical-align: middle;">
                                                                                                    <i runat="server" id="iconeServico_Comparativo" class="fa fa-plus"></i>
                                                                                                    &nbsp;
                                                                                                    <asp:Label runat="server" ID="nAjusteServicos_Comparativos" ClientIDMode="Static" Text='<%# Bind("NAjuste", "{0:N2}") %>' class="valor"></asp:Label>
                                                                                                    &nbsp;%
                                                                                                </div>
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:TemplateField>

                                                                                        <asp:TemplateField HeaderText="Total">
                                                                                            <ItemTemplate>
                                                                                                <asp:TextBox runat="server" ID="nTotalServicos_Comparativos" ClientIDMode="Static" Text='<%# Bind("NTotal", "{0:N2}") %>' class="form-control" AutoPostBack="true" OnTextChanged="nTotalServicos_Comparativos_TextChanged"></asp:TextBox>
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:TemplateField>

                                                                                        <asp:TemplateField HeaderText="Total">
                                                                                            <ItemTemplate>
                                                                                                <asp:Label runat="server" ID="lbl_TotalServicos_Comparativos" Text='<%# Bind("NTotal", "{0:N2}") %>'></asp:Label>
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                                        </asp:TemplateField>

                                                                                        <asp:TemplateField HeaderText="Excluir">
                                                                                            <ItemTemplate>
                                                                                                <asp:LinkButton runat="server" ID="cmdExcluir_Servico_Empreitada" CommandName="Delete" TabIndex="10000" CssClass="btn btn-small" ClientIDMode="Static" data-toggle="tooltip" title="Excluir"><i class="fa fa-eraser"></i></asp:LinkButton>
                                                                                            </ItemTemplate>
                                                                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="id" />
                                                                                            <HeaderStyle CssClass="id" />
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

                                                </div>
                                            </div>

                                            <span runat="server" id="spanObs_Comparativos"><b>Obs: Os ajustes realizados na aba de Comparativos, serão aplicados em uma nova Revisão deste mesmo Orçamento!</b></span>

                                            <fieldset class="form-stacked actions">

                                                <asp:Button ID="cmdAplicar_Comparativos" class="btn btn-lg btn-success" runat="server" Text="Aplicar Comparativos" />
                                                <asp:Button ID="voltar_5" class="btn btn-lg btn-warning" runat="server" Text="Cancelar" OnClick="voltar_Click" />

                                            </fieldset>

                                        </div>

                                    </div>

                                    <div role="tabpanel" class="tab-pane fade" id="Historico" aria-labelledby="Historico-tab">

                                        <div runat="server" id="div_gvHistorico" class="col-lg-12 row">

                                            <asp:GridView ID="gvHistorico" class="table table-striped table-bordered table-hover "
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px">
                                                <Columns>

                                                    <asp:BoundField DataField="sTipoAcao" HeaderText="Tipo de Ação">
                                                        <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sAcao" HeaderText="Ação">
                                                        <ItemStyle Width="50%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="dtLog" HeaderText="Data">
                                                        <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sDscUsuario" HeaderText="Por">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                </Columns>
                                            </asp:GridView>

                                        </div>

                                        <fieldset class="form-stacked actions">
                                            <asp:Button ID="cmdSalvar_View_Historico" class="btn btn-lg btn-success" runat="server" Text="Salvar" />
                                            <asp:Button ID="cmdEditar_View_Historico" class="btn btn-lg btn-success" runat="server" Text="Editar" />
                                            <input type="submit" name="cancel" runat="server" class="btn btn-lg btn-warning" value="Cancelar" id="voltar_6" title="Voltar" onclick="history.go(-1)">
                                        </fieldset>

                                    </div>

                                </div>

                                <div class="modal fade" id="modalVincularCRM" data-backdrop="static">
                                    <div class="modal-dialog" style="width: 40%;">
                                        <div class="modal-content" style="overflow: visible !important;">
                                            <div class="modal-header">
                                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                                                <h4 class="modal-title">Vincular CRM</h4>
                                            </div>
                                            <div class="modal-body">

                                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaModalVincula" />

                                                <div class="row">
                                                    <div class="col-lg-12 form-group">
                                                        <label>CRM a ser Vinculado</label>
                                                        <asp:DropDownList runat="server" ID="ddlVincularCRM" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="row modal-footer">
                                                    <asp:Button ID="cmdVincular" class="btn btn-success" runat="server" Text="Salvar" OnClick="cmdVincular_Click" />
                                                    <asp:Button runat="server" ID="cmdFecharModal_VinculaCRM" class="btn btn-danger" Text="Cancelar" OnClick="cmdFecharModal_Click" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="modal fade" id="modalVincularPedido" data-backdrop="static">
                                    <div class="modal-dialog" style="width: 40%;">
                                        <div class="modal-content" style="overflow: visible !important;">
                                            <div class="modal-header">
                                                <button type="button" class="close" data-dismiss="modal">&times;</button>
                                                <h4 runat="server" id="modalVincularPedido_titulo" class="modal-title">Vincular à um Pedido</h4>
                                            </div>
                                            <div class="modal-body">

                                                <uc1:MensagemPagina runat="server" ID="MensagemPaginaModalVincula_Pedido" />

                                                <div runat="server" id="div_ddlVinculaPedido" class="row">
                                                    <div class="col-lg-12 form-group">
                                                        <label>Pedido a ser Vinculado</label>
                                                        <asp:DropDownList runat="server" ID="ddlVincularPedido" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div runat="server" id="div_txtPrevisaoEntrega" class="row">
                                                    <div class="col-lg-4 form-group">
                                                        <label>Estimativa de Entrega</label>
                                                        <asp:TextBox runat="server" ID="txtPrevisaoEntrega" class="form-control" type="date"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="row modal-footer">
                                                    <asp:Button ID="cmdConfirmar_Vinculo_Pedido" class="btn btn-success" runat="server" Text="Vincular" OnClick="cmdConfirmar_Vinculo_Pedido_Click" />
                                                    <asp:Button ID="cmdVincular_Novo_Pedido" class="btn btn-success" runat="server" Text="Novo Pedido" OnClick="cmdVincular_Pedido_Click" />
                                                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div id="dialog-Salvar" class="modal" style="padding: 10px;" title="Salvar">
                                    <p>
                                        <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
                                    </p>
                                </div>

                                <div id="dialog-Editar" class="modal" style="padding: 10px;" title="Editar">
                                    <p>
                                        <asp:Label ID="lblTituloEditar" runat="server" Text=""></asp:Label>
                                    </p>
                                </div>

                                <div id="dialog-Aplicar_Comparativos" class="modal" style="padding: 10px;" title="Aplicar">
                                    <p>
                                        <label runat="server" id="lbl_modalAplicarComparativos"></label>
                                    </p>
                                </div>

                            </div>
                        </div>

                    </asp:Panel>

                    <div class="modal fade" id="produtoDetalheModal" tabindex="-1" role="dialog" aria-labelledby="detailsModalLabel" aria-hidden="true">
                        <div class="modal-dialog" role="document">
                            <div class="modal-content" style="display: table;">
                                <div class="modal-header" id="modalInfo">
                                </div>
                                <div class="modal-body" id="modalBody">
                                    <!-- conteudo via javascript-->
                                </div>
                            </div>
                        </div>
                    </div>

                    <div runat="server" id="divRegiaoAvancar" class="regiao r_avancar">
                        <a runat="server" id="cmdAvancarEtapa" class="botoes b_avancar"><i style="position: absolute; left: 42.5%; top: 36%;" class="fa fa-chevron-right"></i></a>
                    </div>

                </div>

            </asp:Panel>

            <div runat="server" id="div_Voltar" class="col-lg-12">
                <fieldset class="form-stacked actions">
                    <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field_cancel_Voltar" title="Voltar" onclick="history.go(-1)">
                </fieldset>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="modal fade" id="modalUpload_ImportarExcel" data-backdrop="static">
        <div class="modal-dialog" style="width: 65%;">
            <div class="modal-content" style="overflow: visible !important;">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Importar Produtos</h4>
                </div>
                <div class="modal-body">
                    <div class="row">
                        <div class="col-lg-12">
                            <div class="col-lg-12">

                                <div class="row">
                                    <div class="col-lg-12">
                                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Geral_Modal_ImportarProdutos" />
                                    </div>
                                </div>

                                <div class="row">

                                    <div id="abas_Modal_ImportarProdutos">
                                        <ul id="abas_ImportarProdutos" class="nav nav-tabs" role="tablist">

                                            <li role="presentation" class="tabpanel active" id="Li1" runat="server">
                                                <a href="#Excel" role="tab" id="aba-Excel" data-toggle="tab" aria-controls="Excel"><b>Arquivo Excel</b></a>
                                            </li>
                                            <li role="presentation" runat="server" id="Li2">
                                                <a href="#Pedidos" id="aba-Pedidos" role="tab" data-toggle="tab" aria-controls="Pedidos" aria-expanded="false"><b>Pedidos</b></a>
                                            </li>
                                            <li role="presentation" runat="server" id="Li3">
                                                <a href="#LM" id="aba-LM" role="tab" data-toggle="tab" aria-controls="LM" aria-expanded="false"><b>LM de Pedidos</b></a>
                                            </li>
                                            <li role="presentation" runat="server" id="Li4">
                                                <a href="#Orcamentos" id="aba-Orcamentos" role="tab" data-toggle="tab" aria-controls="Orcamentos" aria-expanded="false"><b>Orçamentos</b></a>
                                            </li>
                                            <li role="presentation" runat="server" id="Li5">
                                                <a href="#TabelaPreco" id="aba-TabelaPreco" role="tab" data-toggle="tab" aria-controls="TabelaPreco" aria-expanded="false"><b>Tabela de Preço</b></a>
                                            </li>

                                        </ul>
                                    </div>

                                </div>

                                <br />

                                <div id="tab_ImportarProdutos" class="tab-content">

                                    <div role="tabpanel" class="tab-pane fade in active" id="Excel" aria-labelledby="Excel-tab">

                                        <div class="row">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_ImportarProdutos" />

                                            <div style="margin: 0 -15px;">
                                                <uc1:ExcelImportar runat="server" ID="ExcelImportar" />
                                            </div>
                                        </div>

                                        <div class="row">
                                            <div class="modal-footer" style="padding: 15px 0;">
                                                <asp:Button ID="cmdExcel_ImportarProdutos" class="btn btn-success" runat="server" Text="Importar Produtos" OnClick="cmdExcel_ImportarProdutos_Click" />
                                                <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                                            </div>
                                        </div>

                                        <div class="row">
                                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_ImportarProdutos_Aviso" />
                                        </div>

                                    </div>

                                    <div role="tabpanel" class="tab-pane fade" id="Pedidos" aria-labelledby="Pedidos-tab">

                                        <asp:UpdatePanel runat="server" ID="UpdModal_Importar_Pedidos" UpdateMode="Conditional">
                                            <ContentTemplate>

                                                <div class="row">
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_ImportarProdutos_Pedidos" />
                                                </div>

                                                <div class="row">
                                                    <div class="form-group">
                                                        <label>Pedido</label>
                                                        <asp:DropDownList runat="server" ID="ddlPedidos_ImportarProdutos" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="modal-footer" style="padding: 15px 0;">
                                                        <asp:Button ID="cmdPedidos_ImportarProdutos" class="btn btn-success" runat="server" Text="Importar Produtos" OnClick="cmdPedidos_ImportarProdutos_Click" />
                                                        <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                                                    </div>
                                                </div>

                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="cmdPedidos_ImportarProdutos" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>

                                    </div>

                                    <div role="tabpanel" class="tab-pane fade" id="LM" aria-labelledby="LM-tab">

                                        <asp:UpdatePanel runat="server" ID="UpdModal_Importar_LM" UpdateMode="Conditional">
                                            <ContentTemplate>

                                                <div class="row">
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_ImportarProdutos_LM" />
                                                </div>

                                                <div class="row">
                                                    <div class="form-group">
                                                        <label>Pedido</label>
                                                        <asp:DropDownList runat="server" ID="ddlSelecionaPedido_LM_ImportarProdutos" class="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlSelecionaPedido_LM_ImportarProdutos_SelectedIndexChanged"></asp:DropDownList>
                                                    </div>

                                                    <div runat="server" id="divLM_ImportarProdutos" class="form-group">
                                                        <label>Lista de Materiais</label>
                                                        <asp:DropDownList runat="server" ID="ddlLM_ImportarProdutos" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="modal-footer" style="padding: 15px 0;">
                                                        <asp:Button ID="cmdLM_ImportarProdutos" class="btn btn-success" runat="server" Text="Importar Produtos" OnClick="cmdLM_ImportarProdutos_Click" />
                                                        <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                                                    </div>
                                                </div>

                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="ddlSelecionaPedido_LM_ImportarProdutos" EventName="SelectedIndexChanged" />
                                                <asp:AsyncPostBackTrigger ControlID="cmdLM_ImportarProdutos" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>

                                    </div>

                                    <div role="tabpanel" class="tab-pane fade" id="Orcamentos" aria-labelledby="Orcamentos-tab">

                                        <asp:UpdatePanel runat="server" ID="UpdModal_Importar_Orcamento" UpdateMode="Conditional">
                                            <ContentTemplate>

                                                <div class="row">
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Modal_ImportarProdutos_Orcamento" />
                                                </div>

                                                <div class="row">
                                                    <div class="form-group">
                                                        <label>Orçamento</label>
                                                        <asp:DropDownList runat="server" ID="ddlOrcamento_ImportarProdutos" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="modal-footer" style="padding: 15px 0;">
                                                        <asp:Button ID="cmdOrcamento_ImportarProdutos" class="btn btn-success" runat="server" Text="Importar Produtos" OnClick="cmdOrcamento_ImportarProdutos_Click" />
                                                        <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                                                    </div>
                                                </div>

                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="cmdOrcamento_ImportarProdutos" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>

                                    </div>

                                    <div role="tabpanel" class="tab-pane fade" id="TabelaPreco" aria-labelledby="TabelaPreco-tab">

                                        <asp:UpdatePanel runat="server" ID="UpdModal_Importar_TabelaPreco" UpdateMode="Conditional">
                                            <ContentTemplate>

                                                <div class="row">
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Fixa_Modal_ImportarProdutos_TabelaPreco" />

                                                    <div class="form-group">
                                                        <label>Tabela de Preço</label>
                                                        <asp:TextBox runat="server" ID="txtTabelaPreco_ImportarProdutos" class="form-control" ReadOnly="true"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="row">
                                                    <div class="modal-footer" style="padding: 15px 0;">
                                                        <asp:Button ID="cmdTabelaPreco_ImportarProdutos" class="btn btn-success" runat="server" Text="Importar Produtos" OnClick="cmdTabelaPreco_ImportarProdutos_Click" />
                                                        <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                                                    </div>
                                                </div>

                                            </ContentTemplate>
                                            <Triggers>
                                                <asp:AsyncPostBackTrigger ControlID="cmdTabelaPreco_ImportarProdutos" EventName="Click" />
                                            </Triggers>
                                        </asp:UpdatePanel>

                                    </div>

                                </div>

                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
