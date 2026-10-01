<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" EnableViewState="false" AutoEventWireup="true" CodeBehind="Tabelas_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.Tabelas_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/ExcelImportar.ascx" TagPrefix="uc1" TagName="ExcelImportar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script type="text/javascript" src='<%= ResolveUrl("~/App/JS/Tabelas_ControleValores.min.js?v=1") %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/App/JS/Tabelas.min.js?v=1") %>'></script>
    <script type="text/javascript" src='<%= ResolveUrl("~/App/JS/xlsx.full.min.js?v=1") %>'></script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="UpdDetalhe" runat="server">
        <ContentTemplate>

            <uc1:Manual runat="server" ID="manual" />

            <style>
                .invisivel {
                    display: none !important;
                }

                .CheckBoxMini {
                    width: 40px;
                }

                .CheckBoxMicro {
                    width: 30px;
                    height: 20px;
                }

                /*-------------------------------------------------------------*/
                /* Estilos para a Aba de Vínculos */

                .arvore {
                    display: flex;
                    flex-wrap: wrap;
                    overflow: scroll;
                    overflow-x: scroll;
                    overflow-y: scroll;
                    width: 100%;
                    max-height: 100vh;
                    justify-content: center;
                }

                .tabelaLink {
                    position: relative;
                    color: black;
                    margin: 30px 15px 30px 15px;
                }

                    .tabelaLink:hover {
                        text-decoration: none;
                        color: black;
                        transition: all 0.5s;
                    }


                .no_tabela {
                    position: relative;
                    min-width: 250px;
                    min-height: 50px;
                    max-width: 350px;
                    max-height: 350px;
                    border: 3px solid red;
                    border-radius: 10px;
                    padding: 15px;
                    margin: 0;
                    text-align: center;
                    box-shadow: rgba(0, 0, 0, 0.35) 5px 5px;
                    font-size: 16px;
                    overflow-wrap: anywhere;
                    text-overflow: ellipsis;
                }

                .tabelaPai {
                    border: 2px solid black;
                }

                    .tabelaPai:hover {
                        border: 2px solid lightgreen;
                    }

                .tabelaFilha {
                    border: 2px solid black;
                }

                    .tabelaFilha:hover {
                        border: 2px solid lightgreen;
                    }

                .arvore ul {
                    display: flex;
                    padding: 0;
                    min-width: 100px;
                    position: relative;
                    transition: all 0.5s;
                    -webkit-transition: all 0.5s;
                    -moz-transition: all 0.5s;
                    justify-content: center;
                }

                    .arvore ul li ul li:only-child a {
                        display: flex;
                        justify-content: center;
                    }

                    .arvore ul ul {
                        display: flex;
                        min-width: 100px;
                        margin: 0 20px 20px 20px;
                        padding-top: 30px;
                        padding-left: 0;
                        position: relative;
                        transition: all 0.5s;
                        -webkit-transition: all 0.5s;
                        -moz-transition: all 0.5s;
                        justify-content: center;
                    }

                .arvore .tabelasPais {
                    min-width: 100%;
                    display: flex;
                    justify-content: center;
                    margin: 0;
                    padding: 0 0 7px 0;
                }

                    .arvore .tabelasPais li {
                        display: flex;
                        justify-content: center;
                        flex-wrap: wrap;
                        list-style-type: none;
                        position: relative;
                        transition: all 0.5s;
                        -webkit-transition: all 0.5s;
                        -moz-transition: all 0.5s;
                    }

                        .arvore .tabelasPais li::before, .arvore li::after {
                            content: '';
                            position: absolute;
                            top: 90%;
                            right: 50%;
                            border-bottom: 1px solid black;
                            width: 50%;
                            height: 20px;
                        }

                        .arvore .tabelasPais li::after {
                            right: auto;
                            left: 50%;
                            border-left: 1px solid black;
                        }

                        .arvore .tabelasPais li:only-child::after, .arvore li:only-child::before {
                            display: none;
                        }

                        .arvore .tabelasPais li:only-child {
                            padding-top: 0;
                        }

                        .arvore .tabelasPais li:first-child::before, .arvore li:last-child::after {
                            border: 0 none;
                        }

                        .arvore .tabelasPais li:last-child::before {
                            border-right: 1px solid black;
                            border-radius: 0 0 5px 0;
                            -webkit-border-radius: 0 0 5px 0;
                            -moz-border-radius: 0 0 5px 0;
                        }

                        .arvore .tabelasPais li:first-child::after {
                            border-radius: 0 0 0 5px;
                            -webkit-border-radius: 0 0 0 5px;
                            -moz-border-radius: 0 0 0 5px;
                        }

                .arvore li li {
                    display: flex;
                    justify-content: center;
                    flex-wrap: wrap;
                    list-style-type: none;
                    position: relative;
                    transition: all 0.5s;
                    -webkit-transition: all 0.5s;
                    -moz-transition: all 0.5s;
                }

                    .arvore li li::before, .arvore li li::after {
                        content: '';
                        position: absolute;
                        top: 0;
                        right: 50%;
                        border-top: 1px solid black;
                        border-bottom: none;
                        width: 50%;
                        height: 20px;
                    }

                    .arvore li li::after {
                        right: auto;
                        left: 50%;
                        border-left: 1px solid black;
                    }

                    .arvore li li:only-child::after, .arvore li li:only-child::before {
                        display: none;
                    }

                    .arvore li li:only-child {
                        display: block;
                        padding-top: 0;
                    }

                    .arvore li li:first-child::before, .arvore li li:last-child::after {
                        border: 0 none;
                    }

                    .arvore li li:last-child::before {
                        border-right: 1px solid black;
                        border-radius: 0 5px 0 0;
                        -webkit-border-radius: 0 5px 0 0;
                        -moz-border-radius: 0 5px 0 0;
                    }

                    .arvore li li:first-child::after {
                        border-radius: 5px 0 0 0;
                        -webkit-border-radius: 5px 0 0 0;
                        -moz-border-radius: 5px 0 0 0;
                    }

                .arvore ul ul::before,
                .arvore ul ul::after {
                    content: '';
                    position: absolute;
                    top: 0;
                    left: 50%;
                    border-left: 1px solid black;
                    width: 0;
                    height: 20px;
                }

                .arvore ul ul::after {
                    content: '';
                    position: absolute;
                    border-style: solid;
                    border-width: 10px 10px 0 0;
                    border-color: black transparent;
                    top: 0;
                    left: calc(50% - 1px);
                    transform: rotate(225deg);
                }


                .setaVisivel::before,
                .setaVisivel::after {
                    display: block;
                }

                .setaInvisivel::before,
                .setaInvisivel::after {
                    display: none;
                }

                @media (min-width: 100%) {
                    .arvore {
                        --justify-content: start !important;
                    }
                }

                /*-------------------------------------------------------------*/
                /* Estilos para as funções de Paginação */

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

                /*-------------------------------------------------------------*/
                /* Estilos para o cadeado de Edição */

                .lock-label, .lock-label-controle {
                    position: relative;
                    height: 45px;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    background-color: transparent;
                    border-radius: 15px;
                    cursor: pointer;
                    transition: all 0.3s;
                }

                    .lock-label:hover::after, .lock-label-controle:hover::after {
                        content: "Clique para habilitar a edição";
                        position: absolute;
                        bottom: 100%;
                        left: 10%;
                        transform: translateX(-50%);
                        background-color: #333;
                        color: #fff;
                        padding: 5px 8px;
                        border-radius: 4px;
                        white-space: nowrap;
                        font-size: 12px;
                        pointer-events: none;
                        opacity: 0;
                        transition: opacity 0.2s ease-in-out;
                        z-index: 1;
                    }

                    .lock-label:hover::after, .lock-label-controle:hover::after {
                        content: "Clique para bloquear a edição";
                        opacity: 1;
                        z-index: 10;
                    }

                .lock-wrapper {
                    width: fit-content;
                    height: fit-content;
                    display: flex;
                    flex-direction: column;
                    align-items: center;
                    justify-content: center;
                }

                .shackle {
                    background-color: transparent;
                    height: 9px;
                    width: 14px;
                    border-top-right-radius: 10px;
                    border-top-left-radius: 10px;
                    border-top: 3px solid black;
                    border-left: 3px solid black;
                    border-right: 3px solid black;
                    transform: rotateY(150deg) translateX(3px);
                    transform-origin: right;
                    transition: all 0.3s;
                }

                .lock-body {
                    width: 15px;
                }

                .shackle-closed {
                    transform: rotateY(0deg);
                }

                .shackle-open {
                    transform: rotateY(150deg) translateX(3px);
                    transform-origin: right;
                }

                .lock-label-closed {
                    pointer-events: none;
                }

                .lock-label:active, .lock-label-controle:active {
                    transform: scale(0.9);
                }

                /*-------------------------------------------------------------*/
                /* Estilo para a Aba de Controle de Valores */

                .tipo-primary {
                    background-color: rgb(150, 200, 255);
                }

                .table-hover > tbody > tr:hover > .tipo-primary,
                .table-hover > tbody > tr > td.tipo-primary:hover,
                .table-hover > tbody > tr > th.tipo-primary:hover {
                    background-color: rgb(140, 190, 245);
                }

                /*-------------------------------------------------------------*/
                /* Estilos para manipulações do .DataTables */

                .dataTables_scrollBody {
                    overflow: visible !important;
                }

                .dataTables_scrollHead {
                    overflow: visible !important;
                }

                /*-------------------------------------------------------------*/
                /* Estilos para o filtro de Consulta com .Chosen */

                .chosen-container-multi {
                    border-radius: 5px;
                }

                    .chosen-container-multi ul {
                        border-radius: 5px;
                        padding: 2.5px 5px;
                    }

                        .chosen-container-multi ul li.search-choice {
                            display: flex !important;
                            align-items: center !important;
                            padding: 7.5px 30px 7.5px 7.5px !important;
                            color: white !important;
                            background: green !important;
                            border-radius: 5px !important;
                        }

                            .chosen-container-multi ul li.search-choice a.search-choice-close {
                                background: none !important;
                                color: white !important;
                                top: 6px !important;
                                right: 5px !important;
                                font-size: 20px !important;
                                font-weight: bold !important;
                                font-family: Verdana !important;
                            }

                                .chosen-container-multi ul li.search-choice a.search-choice-close:hover {
                                    color: lightgray !important;
                                    font-size: 20px !important;
                                    font-weight: bold !important;
                                    font-family: Verdana !important;
                                    text-decoration: none !important;
                                }

                /*-------------------------------------------------------------*/
                /* Estilos para a Aba de Histórico */

                .infoObs {
                    text-decoration: none;
                    color: blue;
                }

                .infoAlterada {
                    text-decoration: underline;
                    color: red;
                }

                .infoNova {
                    text-decoration: underline;
                    color: green;
                }
            </style>

            <div runat="server" id="div_ControleValores_colunasDinamicas" class="invisivel"></div>
            <div runat="server" id="div_ControleValores_Header_1" class="invisivel"></div>
            <div runat="server" id="div_ControleValores_Header_2" class="invisivel"></div>

            <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div id="hdd">
                <asp:HiddenField ID="hddidTabela" runat="server" Value="0" />
                <asp:HiddenField ID="hddTipoTabela" runat="server" Value="0" />
                <asp:HiddenField ID="hddTerritorio" runat="server" />
                <asp:HiddenField ID="hddComposicao_nMoedaOrigem" runat="server" />
                <asp:HiddenField ID="hddComposicao_nMoedaDestino" runat="server" />
                <asp:HiddenField ID="hddComposicao_idItem" runat="server" />
                <asp:HiddenField ID="hddComposicao_idItemComposicao" runat="server" />
                <asp:HiddenField ID="hddComposicao_sDscTipoProduto" runat="server" />
                <asp:HiddenField ID="hddComposicao_idGrupo" runat="server" />
                <asp:HiddenField ID="hddComposicao_idFamilia" runat="server" />
                <asp:HiddenField ID="hddComposicao_sDscGrupo" runat="server" />
                <asp:HiddenField ID="hddComposicao_sDscFamilia" runat="server" />
                <asp:HiddenField ID="hddComposicao_sUnidade" runat="server" />
                <asp:HiddenField ID="hddComposicao_nII" runat="server" />
                <asp:HiddenField ID="hddComposicao_nIPI" runat="server" />
                <asp:HiddenField ID="hddComposicao_nPIS" runat="server" />
                <asp:HiddenField ID="hddComposicao_nCOFINS" runat="server" />
                <asp:HiddenField ID="hddComposicao_nICMS" runat="server" />
                <asp:HiddenField ID="hddComposicao_sIndustrializado" runat="server" />
                <asp:HiddenField ID="hddIdsTabelasImportadas" runat="server" Value="|" />
                <asp:HiddenField ID="hddExibicao" runat="server" Value="0" />
                <asp:HiddenField ID="hddCalculaNCM" runat="server" Value="S" />
                <asp:HiddenField ID="hddsTabelasVinculadas" runat="server" />
                <asp:HiddenField ID="hddsItens_TabelasVinculadas" runat="server" />
                <asp:HiddenField ID="hddsCadeado" runat="server" />
                <asp:HiddenField ID="hddsPermissaoCadeado" runat="server" />
                <asp:HiddenField ID="hddFiltro_Vinculos" runat="server" />
                <asp:HiddenField ID="hddFiltro_ID" runat="server" Value="0" />
            </div>

            <div id="abas">
                <ul id="tab_Tabelas" class="nav nav-tabs" role="tablist">
                    <li role="presentation" class="tabpanel active" id="aba_Principal">
                        <a href="#principal" id="aba-principal" role="tab" data-toggle="tab" aria-controls="principal" aria-expanded="false"><b>Tabela de Preço</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Vinculos">
                        <a href="#vinculos" role="tab" id="aba_vinculos-tab" data-toggle="tab" aria-controls="vinculos"><b>Vínculos</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Controle">
                        <a href="#controle" role="tab" id="aba_controle-tab" data-toggle="tab" aria-controls="controle"><b>Controle de Valores</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Arquivos">
                        <a href="#arquivos" role="tab" id="aba_arquivos-tab" data-toggle="tab" aria-controls="arquivos"><b>Arquivos</b></a>
                    </li>
                    <li role="presentation" id="aba_Historico" runat="server">
                        <a href="#historico" role="tab" id="aba_historico-tab" data-toggle="tab" aria-controls="historico"><b>Histórico</b></a>
                    </li>
                </ul>
            </div>

            <br />

            <div id="tab" class="tab-content">

                <div role="tabpanel" class="tab-pane fade in active" id="principal" aria-labelledby="principal">

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <!-- Panel Tabela -->
                    <div runat="server" id="pnTabela" class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Tabela</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">

                                <div class="col-lg-5">
                                    <label>ID</label>
                                    <asp:TextBox runat="server" ID="txtidTabela" class="form-control CaixaTextoMini" ReadOnly="true"></asp:TextBox>
                                </div>

                                <div class="col-lg-2">
                                    <h3 runat="server" id="hStatus_LPU"></h3>
                                </div>

                                <div class="col-lg-4">&nbsp;</div>

                                <div runat="server" id="div_bloquearEdicao" class="col-lg-1 form-group" style="display: flex; justify-content: right;">
                                    <div class="form-group">
                                        <label id="lbl_Lock" runat="server" class="lock-label">
                                            <span class="lock-wrapper">
                                                <span id="spanCadeado" runat="server" class="shackle shackle-open"></span>
                                                <svg
                                                    class="lock-body"
                                                    width="15"
                                                    height="15"
                                                    viewBox="0 0 28 28"
                                                    fill="none"
                                                    xmlns="http://www.w3.org/2000/svg">
                                                    <path
                                                        fill-rule="evenodd"
                                                        clip-rule="evenodd"
                                                        d="M0 5C0 2.23858 2.23858 0 5 0H23C25.7614 0 28 2.23858 28 5V23C28 25.7614 25.7614 28 23 28H5C2.23858 28 0 25.7614 0 23V5ZM16 13.2361C16.6137 12.6868 17 11.8885 17 11C17 9.34315 15.6569 8 14 8C12.3431 8 11 9.34315 11 11C11 11.8885 11.3863 12.6868 12 13.2361V18C12 19.1046 12.8954 20 14 20C15.1046 20 16 19.1046 16 18V13.2361Z"
                                                        fill="black">
                                                    </path>
                                                </svg>
                                            </span>
                                        </label>
                                    </div>
                                </div>

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label runat="server">Título da Tabela</label>
                                        <asp:TextBox runat="server" ID="txtTituloTabela" class="form-control"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-12 row">

                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <label runat="server">Observação</label>
                                            <asp:TextBox runat="server" ID="txtObservacao" MaxLength="100" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div runat="server" id="div_dtVigencia" class="col-lg-4">
                                        <div class="col-lg-6" style="padding-left: 0;">
                                            <div class="form-group">
                                                <label runat="server">Data de Vigência - Início</label>
                                                <asp:TextBox runat="server" ID="txtdtInicial" type="date" class="form-control"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-6" style="padding-right: 0;">
                                            <div class="form-group">
                                                <label runat="server">Data de Vigência - Fim</label>
                                                <asp:TextBox runat="server" ID="txtdtFinal" type="date" class="form-control"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                <div class="col-lg-12 row">
                                    <div class="form-group">

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Tipo de Tabela</label>
                                                <asp:DropDownList ID="ddlTipoTabela" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div runat="server" id="divImportar">
                                            <div class="col-lg-8">
                                                <div class="form-group">
                                                    <label runat="server" id="lblVincularTabelas">Vincular Tabelas</label>
                                                    <asp:ListBox runat="server" ID="lstTabelas" SelectionMode="Multiple" class="form-control Caixa_Selecao"></asp:ListBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>&nbsp;</label>
                                                    <asp:Button ID="cmdImportar" class="btn btn-info form-control vincularTabelas" runat="server" Text="Vincular" />
                                                </div>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_ImportarVinculadas">
                                            <div class="col-lg-8">
                                                <div class="form-group">
                                                    <label>Tabelas Vinculadas</label>
                                                    <asp:ListBox runat="server" ID="lstTabelas_Vinculadas" SelectionMode="Multiple" class="form-control Caixa_Selecao"></asp:ListBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>&nbsp;</label>
                                                    <asp:Button ID="cmdImportarTabelasVinculadas" class="btn btn-info form-control importarItens_Vinculadas" runat="server" Text="Importar" />
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>&nbsp;</label>
                                                    <asp:Button ID="cmdDesvincularTabelas_modal" class="btn btn-danger form-control" runat="server" Text="Desvincular Tabelas" />
                                                </div>
                                            </div>
                                        </div>

                                        <div runat="server" id="divVincularFornecedor">
                                            <div class="col-lg-8">
                                                <div class="form-group">
                                                    <label>Fornecedores</label>
                                                    <asp:ListBox runat="server" ID="lstFornecedores" class="form-control Caixa_Selecao" SelectionMode="Multiple"></asp:ListBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>&nbsp;</label>
                                                    <asp:Button ID="cmdImportarProdutosFornecedor" class="btn btn-info form-control importarItens_Fornecedores" runat="server" Text="Importar Produtos dos Fornecedores" />
                                                </div>
                                            </div>
                                        </div>

                                        <div runat="server" id="divVincularParceiros">
                                            <div class="col-lg-8">
                                                <div class="form-group">
                                                    <label>Parceiros</label>
                                                    <asp:ListBox runat="server" ID="lstParceiros" class="form-control Caixa_Selecao" SelectionMode="Multiple"></asp:ListBox>
                                                </div>
                                            </div>
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>&nbsp;</label>
                                                    <asp:Button ID="cmdImportarProdutosParceiros" class="btn btn-info form-control importarItens_Parceiros" runat="server" Text="Importar Produtos dos Parceiros" />
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>

                                <div runat="server" id="divTaxas" class="col-lg-12 row">

                                    <div class="col-lg-2 form-group" id="div_taxaGlobalFator" runat="server">
                                        <label runat="server" id="lblFator">Fator Global</label>
                                        <asp:TextBox runat="server" ID="txtFator" class="form-control fatorGlobal"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2 form-group" id="div_DescontoGlobal" runat="server">
                                        <label runat="server">Desconto (%)</label>
                                        <asp:TextBox runat="server" ID="txtDesconto" class="form-control descontoGlobal"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2 form-group" id="div_taxaGlobalEnvio" runat="server">
                                        <label runat="server">Taxa de Envio (%)</label>
                                        <asp:TextBox runat="server" ID="txtEnvio" class="form-control txEnvioGlobal"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2 form-group" id="div_taxaGlobalLocal" runat="server">
                                        <label runat="server">Taxa Local (%)</label>
                                        <asp:TextBox runat="server" ID="txtLocal" class="form-control txLocalGlobal"></asp:TextBox>
                                    </div>

                                    <div class="col-lg-2 form-group" id="div_taxaGlobalMargem" runat="server">
                                        <label runat="server">Margem Global (%)</label>
                                        <asp:TextBox runat="server" ID="txtMargem" class="form-control margemGlobal"></asp:TextBox>
                                    </div>

                                </div>

                                <div class="col-lg-12 row">

                                    <div class="col-lg-4" id="div_MoedaOrigem" runat="server">
                                        <div class="form-group" runat="server">
                                            <label runat="server" id="lblMoedaOrigem">Moeda de Origem</label>
                                            <asp:DropDownList runat="server" ID="ddlidMoedaOrigem" class="form-control Caixa_Selecao"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-4" id="div_MoedaDestino" runat="server">
                                        <div class="form-group" runat="server">
                                            <label runat="server" id="lblMoedaDestino">Moeda de Destino</label>
                                            <asp:DropDownList runat="server" ID="ddlidMoedaDestino" class="form-control Caixa_Selecao"></asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-4" id="div_Cambio" runat="server">
                                        <div class="col-lg-6 row">
                                            <label>Taxa de câmbio</label>
                                            <div class="input-group">
                                                <asp:TextBox runat="server" ID="txtCambio" class="form-control" ToolTip="Taxa de Câmbio automático" ReadOnly="true" aria-describedby="lblSimboloMoeda_Cambio" Style="z-index: 0;"></asp:TextBox>
                                                <span runat="server" id="lblSimboloMoeda_Cambio" class="input-group-addon"></span>
                                            </div>
                                        </div>
                                    </div>

                                    <div id="div_sAtivo" class="col-lg-12">
                                        <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                                    </div>

                                    <div runat="server" id="div_sValidada" class="col-lg-12">
                                        <uc1:ComboAtivo runat="server" ID="ComboValidada" />
                                    </div>

                                </div>

                            </div>
                        </div>
                    </div>

                    <!-- Panel Itens -->
                    <div class="panel panel-default" runat="server" id="div_PanelItens">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b runat="server" id="bPanelItens_Title">Produtos</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">

                                <div class="col-lg-12" style="padding: 0;">

                                    <div runat="server" id="div_cmdFuncoes" class="col-lg-12 form-group d-flex space-b">
                                        <div id="esquerda">
                                            <asp:Button runat="server" ID="btnFiltro" Text="Filtrar Itens" class="btn btn-info" />
                                            <asp:Button runat="server" ID="cmdControleGrupo" Text="Grupos" class="btn btn-info" />
                                            <asp:Button runat="server" ID="cmdControleFamilia" Text="Famílias" class="btn btn-info" />
                                            <asp:Button runat="server" ID="cmdImportarItens_div" Text="Importar Itens" class="btn btn-info" />
                                            <asp:Button runat="server" ID="cmdIncluirItem_div" Text="Incluir Item" class="btn btn-info" />
                                        </div>
                                        <div id="direita">
                                            <asp:Button runat="server" ID="cmdVisualizacao_Excel_LPU" Text="Salvar Visualização para Excel" class="btn btn-info" />
                                            <asp:Button runat="server" ID="cmdExportacaoExcel_LPU" Text="Exportar Excel" class="btn btn-success" />
                                        </div>
                                    </div>

                                    <div runat="server" id="div_Filtro" class="col-lg-12 invisivel">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Filtro de Itens</b></h3>
                                            </div>
                                            <div class="panel-body">

                                                <div class="row">

                                                    <div class="col-lg-4">
                                                        <div class="form-group">
                                                            <label>Filtro</label>
                                                            <asp:TextBox runat="server" ID="txtFiltroDescricao" class="form-control" placeholder="Filtrar por Código ou Descrição" MaxLength="100"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Tipo de Produto</label>
                                                            <asp:DropDownList runat="server" ID="ddlFiltroTipo" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Unidade</label>
                                                            <asp:DropDownList runat="server" ID="ddlFiltroUnidade" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div runat="server" id="divFiltroGrupo" class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Grupo</label>
                                                            <asp:DropDownList runat="server" ID="ddlFiltroGrupo" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div runat="server" id="divFiltroFamilia" class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Família</label>
                                                            <asp:DropDownList runat="server" ID="ddlFiltroFamília" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                        </div>
                                                    </div>

                                                </div>

                                                <div class="row">

                                                    <div runat="server" id="divFiltroOrigem" class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Origem</label>
                                                            <asp:DropDownList runat="server" ID="ddlFiltroOrigem" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-1" runat="server" id="divFiltroAlterados">
                                                        <div class="form-group">
                                                            <label>Alterados</label>
                                                            <asp:CheckBox runat="server" ID="cbFiltroAlterados" class="form-control CheckBoxMini"></asp:CheckBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-1" runat="server" id="divFiltroIncluidos">
                                                        <div class="form-group">
                                                            <label>Incluídos</label>
                                                            <asp:CheckBox runat="server" ID="cbFiltroIncluidos" class="form-control CheckBoxMini"></asp:CheckBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-1" runat="server" id="divFiltroExcluidos">
                                                        <div class="form-group">
                                                            <label>Excluídos</label>
                                                            <asp:CheckBox runat="server" ID="cbFiltroExcluido" class="form-control CheckBoxMini"></asp:CheckBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-1" runat="server" id="divFiltroLiberados">
                                                        <div class="form-group">
                                                            <label>Liberados</label>
                                                            <asp:CheckBox runat="server" ID="cbFiltroLiberado" class="form-control CheckBoxMini"></asp:CheckBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-1" runat="server" id="divFiltroNaoLiberados">
                                                        <div class="form-group">
                                                            <div style="width: 150%;">
                                                                <label>Não Liberados</label>
                                                            </div>
                                                            <asp:CheckBox runat="server" ID="cbFiltroNaoLiberado" class="form-control CheckBoxMini"></asp:CheckBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-lg-1" runat="server" id="divFiltroImportados">
                                                        <div class="form-group">
                                                            <label>Importados</label>
                                                            <asp:CheckBox runat="server" ID="cbFiltroImportado" class="form-control CheckBoxMini"></asp:CheckBox>
                                                        </div>
                                                    </div>

                                                </div>

                                            </div>
                                        </div>
                                    </div>

                                    <div runat="server" id="divControle_Grupo" class="col-lg-4 invisivel">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title" style="width: 50%"><b>Alterar Fator por Grupo</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <asp:GridView ID="gvControle_Grupo" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="15px">
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>

                                    <div runat="server" id="divControle_Familia" class="col-lg-4 invisivel">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Alterar Fator por Família</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <asp:GridView ID="gvControle_Familia" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="15px">
                                                </asp:GridView>
                                            </div>
                                        </div>
                                    </div>

                                    <div runat="server" id="div_ImportarItens" class="col-lg-4 invisivel">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <div class="d-flex space-b">
                                                    <h3 class="panel-title"><b>Importar Itens</b></h3>
                                                    <asp:Button ID="cmdImportarItens_Excel" class="btn btn-success w-50 form-control importarExcel" runat="server" Text="Importar via Excel" />
                                                </div>
                                            </div>
                                            <div class="panel-body">

                                                <div class="panel panel-default">
                                                    <div runat="server" id="div_ImportarItem_Grupo" class="panel-body">
                                                        <div class="form-group">
                                                            <label>Grupos de Produtos</label>
                                                            <asp:ListBox runat="server" ID="lstGrupos" SelectionMode="Multiple" class="form-control Caixa_Selecao"></asp:ListBox>
                                                        </div>
                                                        <asp:Button ID="cmdImportarItens_Grupo" class="btn btn-info w-50 form-control" runat="server" Text="Importar por Grupo" />
                                                    </div>
                                                </div>

                                                <div class="panel panel-default m-0">
                                                    <div runat="server" id="div_ImportarItem_Familia" class="panel-body">
                                                        <div class="form-group">
                                                            <label>Famílias de Produtos</label>
                                                            <asp:ListBox runat="server" ID="lstFamilias" SelectionMode="Multiple" class="form-control Caixa_Selecao"></asp:ListBox>
                                                        </div>
                                                        <asp:Button ID="cmdImportarItens_Familia" class="btn btn-info w-50 form-control" runat="server" Text="Importar por Família" />
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>

                                    <div runat="server" id="divIncluirItem" class="col-lg-12 invisivel">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Incluir Item</b></h3>
                                            </div>
                                            <div class="panel-body">

                                                <div class="col-lg-12 row" style="margin-right: 0; padding-right: 0;">

                                                    <uc1:MensagemPagina runat="server" ID="MensagemIncluirItem" />

                                                    <div class="col-lg-2" style="padding: 0;">
                                                        <div class="form-group">
                                                            <label runat="server" id="lblCodigo">Código do Produto</label>
                                                            <asp:TextBox ID="txtComposicao_sCodigoProduto" class="form-control" runat="server" ValidationGroup="Item" MaxLength="50"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-7">
                                                        <div class="form-group">
                                                            <label runat="server" id="lblDescricao">Descrição do Produto</label>
                                                            <asp:TextBox ID="txtComposicao_sDscProduto" class="form-control" runat="server" ValidationGroup="Item" MaxLength="200"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div runat="server" id="div_IncluirPreco" class="col-lg-2">
                                                        <div class="form-group">
                                                            <label>Preço</label>
                                                            <asp:TextBox ID="txtPreco" class="form-control" runat="server" MaxLength="10" ValidationGroup="Item"></asp:TextBox>
                                                        </div>
                                                    </div>

                                                    <div class="col-lg-1" style="padding: 0;">
                                                        <div class="form-group">
                                                            <label>&nbsp;</label>
                                                            <asp:Button ID="cmdIncluir_Item" class="form-control btn btn-info" runat="server" Text="Incluir Item" />
                                                        </div>
                                                    </div>
                                                </div>

                                            </div>
                                        </div>
                                    </div>

                                    <div runat="server" id="div_ExportacaoLPU" class="col-lg-4 invisivel float-r">
                                        <div class="panel panel-default">
                                            <div class="panel-heading">
                                                <h3 class="panel-title"><b>Salvar Visualização para Excel</b></h3>
                                            </div>
                                            <div class="panel-body">
                                                <div class="row">

                                                    <div class="col-lg-12 form-group">
                                                        <uc1:MensagemPagina runat="server" ID="Mensagem_ExportacaoLPU" />
                                                    </div>

                                                    <div class="col-lg-12 form-group">
                                                        <label>Título da Visualização</label>
                                                        <asp:TextBox runat="server" ID="txtExportacaoLPU" class="form-control" MaxLength="200"></asp:TextBox>
                                                    </div>

                                                </div>
                                            </div>
                                            <div class="panel-footer">
                                                <asp:LinkButton runat="server" ID="cmdSalva_ExportacaoLPU" Text="Salvar" CssClass="btn btn-success"></asp:LinkButton>
                                            </div>
                                        </div>
                                    </div>

                                </div>

                                <div id="div_Legenda" runat="server" class="col-lg-2 form-group">
                                    <div class="panel panel-default" style="margin: 0;">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Legenda / Filtro Rápido</b></h3>
                                        </div>
                                        <div class="panel-body">
                                            <div class="col-lg-12 id" runat="server" id="divLegendaLiberado">
                                                <label runat="server" id="lblLegenda2">Liberado</label>
                                                <div class="col-lg-12">
                                                    <asp:Button runat="server" ID="btnLegendaSuccess" class="btn btn-success CheckBoxMicro"></asp:Button>
                                                </div>
                                            </div>
                                            <div class="col-lg-12 id" runat="server" id="divLegendaNaoLiberado">
                                                <label runat="server" id="lblLegenda1">Não Liberado</label>
                                                <div class="col-lg-12">
                                                    <asp:Button runat="server" ID="btnLegendaDanger" class="btn btn-danger CheckBoxMicro"></asp:Button>
                                                </div>
                                            </div>
                                            <div class="col-lg-12 id" runat="server" id="divLegendaAlterado">
                                                <label runat="server" id="lblLegenda3">Alterados</label>
                                                <div class="col-lg-12">
                                                    <asp:Button runat="server" ID="btnLegendaWarning" class="btn btn-warning CheckBoxMicro"></asp:Button>
                                                </div>
                                            </div>
                                            <div class="col-lg-12 id" runat="server" id="divLegendaIncluido">
                                                <label runat="server" id="lblLegenda4">Incluídos</label>
                                                <div class="col-lg-12">
                                                    <asp:Button runat="server" ID="btnLegendaInfo" class="btn btn-info CheckBoxMicro"></asp:Button>
                                                </div>
                                            </div>
                                            <div class="col-lg-12" runat="server" id="div_LegendaLimpar">
                                                <label>Limpar Filtros</label>
                                                <div class="col-lg-12">
                                                    <asp:Button runat="server" ID="btnLegendaDefault" class="btn btn-light CheckBoxMicro" Style="border: solid 1px black;"></asp:Button>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemItens" />
                                </div>

                                <div class="col-lg-12" style="padding: 0;">

                                    <div runat="server" id="div_cmdFunc_Secundarias" class="col-lg-11">
                                        <label>&nbsp;</label>
                                        <div runat="server" id="divAlternarColunas" style="display: flex;">
                                            <asp:Button runat="server" ID="cmdAlternarExibicao" class="btn btn-md btn-success" Text="Alternar Exibição" />
                                            &nbsp;
                                            <asp:Button runat="server" ID="cmdCalculaNCM" class="btn btn-md btn-warning" Text="Calcular sem Impostos (NCM)" />
                                            &nbsp;
                                            <asp:Button runat="server" ID="cmdValidarItens_Vinculados" class="btn btn-md btn-primary invisivel" Text="Validar Produtos" />
                                            &nbsp;                                            
                                            <asp:LinkButton runat="server" ID="lnkAtualiza" OnClick="lnkAtualiza_Click" class="btn btn-info" Style="text-align: center;" data-toggle="tooltip" title="Atualizar Informações dos Itens"><i class="fa fa-refresh" style="text-align: center;"></i></asp:LinkButton>
                                        </div>
                                    </div>

                                    <div class="col-lg-1" id="divLiberaTodos" runat="server" style="display: flex; flex-wrap: wrap;">
                                        <label style="position: relative; left: 20%" runat="server" id="lblLiberaTodos">Liberar Todos</label>
                                        <div class="col-lg-12" style="position: relative; left: 65%; padding: 0;">
                                            <asp:CheckBox runat="server" ID="cbLiberaTodos" />
                                        </div>
                                    </div>

                                </div>

                                <div class="col-lg-12" runat="server" id="divLegendaOrigem" style="margin-top: 10px;">
                                    <b>Obs:</b> O campo de Origem do Item representa o ID da Tabela de Preços de onde ele foi importado, caso este seja 0 (zero), significa que o Item não foi Importado de outra Tabela, mas sim incluído diretamente nesta Tabela.
                                </div>

                                <div runat="server" id="div_filtros_Consulta" class="col-lg-12" style="padding: 0; margin-top: 15px;">
                                    <div class="col-lg-10" style="display: flex; flex-wrap: wrap;">
                                        <div style="display: flex; justify-content: space-between; width: 99.5%;">
                                            <label>Consultar Itens - Filtrados por Grupo</label>
                                            <i class="iconLimpa_FiltroGrupos fa fa-times" style="cursor: pointer;" data-toggle="tooltip" title="Limpar Grupos"></i>
                                        </div>
                                        <asp:ListBox runat="server" ID="lstFiltros_Consulta" class="form-control Caixa_Selecao" SelectionMode="Multiple"></asp:ListBox>
                                    </div>

                                    <div class="col-lg-2">
                                        <label>&nbsp;</label>
                                        <asp:Button runat="server" ID="cmdConsulta_Itens" class="btn-info form-control" Text="Consultar Itens"></asp:Button>
                                    </div>

                                    <div class="col-lg-10">&nbsp;</div>

                                    <div class="col-lg-2 form-group">
                                        <label>&nbsp;</label>
                                        <asp:Button runat="server" ID="cmdConsulta_Itens_Todos" data-toggle="tooltip" class="btn-primary form-control todos" Text="Consultar Todos os Itens"></asp:Button>
                                    </div>
                                </div>

                                <div class="col-lg-12" id="div_GridItensTabela" runat="server">
                                    <asp:GridView ID="dtgItens" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="15px">
                                    </asp:GridView>
                                </div>

                            </div>
                        </div>
                    </div>

                </div>

                <div class="tab-pane fade" id="vinculos" aria-labelledby="vinculos">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <div style="padding: 0; width: 100%; display: flex;">
                                <div style="padding: 0; width: 58%">
                                    <h3 class="panel-title"><b>Vínculos</b></h3>
                                </div>
                                <div style="display: flex; justify-content: right; padding: 0; width: 70%; margin-right: 2%;">
                                    <asp:ListBox runat="server" ID="lstFiltroTipos_Vinculos" class="form-control Caixa_Selecao" SelectionMode="Multiple"></asp:ListBox>
                                </div>
                                <div style="display: flex; justify-content: right; padding: 0; width: 10%">
                                    <asp:Button runat="server" ID="cmdAplicarFiltros_Vinculos" class="form-control btn-info" Text="Filtrar" OnClick="cmdAplicarFiltros_Vinculos_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel-body">
                        <div class="row">
                            <div class="col-lg-12" style="padding: 0;">
                                <div class="form-group">
                                    <div class="arvore">

                                        <ul runat="server" id="ulPais" class="tabelasPais">
                                            <asp:Repeater runat="server" ID="rptTabelasPai">
                                                <ItemTemplate>
                                                    <li>
                                                        <asp:HyperLink runat="server" ID="hplTabelaPai" ClientIDMode="Static" CssClass="tabelaLink" NavigateUrl='<%# string.Format("Tabelas_Detalhe.aspx?id={0}", Eval("idTabela")) %>'>
                                                            <div class="no_tabela tabelaPai">
                                                                <%# Eval("sDscTabela") %>
                                                            </div>
                                                        </asp:HyperLink>
                                                    </li>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </ul>

                                        <ul style="min-width: 100%;">
                                            <li style="min-width: 100%;">

                                                <ul runat="server" id="ulPrincipal" style="justify-content: center;">
                                                    <asp:Repeater runat="server" ID="rptTabelaPrincipal">
                                                        <ItemTemplate>
                                                            <li>
                                                                <div id="hplTabelaPrincipal" class="no_tabela">
                                                                    <%# Eval("sDscTabela") %>
                                                                </div>
                                                            </li>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ul>

                                                <ul runat="server" id="ulFilhos">
                                                    <asp:Repeater runat="server" ID="rptTabelasFilhas" OnItemDataBound="rptTabelasFilhas_ItemDataBound">
                                                        <ItemTemplate>
                                                            <li>

                                                                <asp:HyperLink runat="server" ID="hplTabelaFilha" ClientIDMode="Static" CssClass="tabelaLink" NavigateUrl='<%# string.Format("Tabelas_Detalhe.aspx?id={0}", Eval("idTabela")) %>'>
                                                                    <div class="no_tabela tabelaFilha">
                                                                        <%# Eval("sDscTabela") %>
                                                                    </div>
                                                                </asp:HyperLink>

                                                                <ul runat="server" id="ulFilhosFilhos">
                                                                    <asp:Repeater runat="server" ID="rptTabelasFilhasFilhas" OnItemDataBound="rptTabelasFilhasFilhas_ItemDataBound">
                                                                        <ItemTemplate>
                                                                            <li>
                                                                                <asp:HyperLink runat="server" ID="hplTabelaFilhaFilha" ClientIDMode="Static" CssClass="tabelaLink" NavigateUrl='<%# string.Format("Tabelas_Detalhe.aspx?id={0}", Eval("idTabela")) %>'>
                                                                                <div class="no_tabela tabelaFilha">
                                                                                    <%# Eval("sDscTabela") %>
                                                                                </div>
                                                                                </asp:HyperLink>
                                                                                <ul runat="server" id="ulFilhosNetos">
                                                                                    <asp:Repeater runat="server" ID="rptTabelasFilhasNetas">
                                                                                        <ItemTemplate>
                                                                                            <li>
                                                                                                <asp:HyperLink runat="server" ID="hplTabelaFilhaNeta" ClientIDMode="Static" CssClass="tabelaLink" NavigateUrl='<%# string.Format("Tabelas_Detalhe.aspx?id={0}", Eval("idTabela")) %>'>
                                                                                                <div class="no_tabela tabelaFilha">
                                                                                                    <%# Eval("sDscTabela") %>
                                                                                                </div>
                                                                                                </asp:HyperLink>
                                                                                            </li>
                                                                                        </ItemTemplate>
                                                                                    </asp:Repeater>
                                                                                </ul>

                                                                            </li>
                                                                        </ItemTemplate>
                                                                    </asp:Repeater>
                                                                </ul>

                                                            </li>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </ul>

                                            </li>
                                        </ul>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div role="tabpanel" class="tab-pane fade" id="controle" aria-labelledby="controle">
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Controle de Valores</b></h3>
                        </div>
                        <div class="panel-body">
                            <uc1:MensagemPagina runat="server" ID="MensagemControleValores" />
                            <div class="form-stacked row">

                                <div class="col-lg-12 form-group">
                                    <asp:Button runat="server" ID="cmdControle_Filtro" Text="Filtrar Itens" class="btn btn-md btn-info" />
                                </div>

                                <div runat="server" id="divControle_Filtro" class="col-lg-12 invisivel">
                                    <div class="panel panel-default">
                                        <div class="panel-heading">
                                            <h3 class="panel-title"><b>Filtro de Itens</b></h3>
                                        </div>
                                        <div class="panel-body">

                                            <div class="row">

                                                <div class="col-lg-4">
                                                    <div class="form-group">
                                                        <label>Filtro</label>
                                                        <asp:TextBox runat="server" ID="txtControle_Filtro" class="form-control" placeholder="Filtrar por Código ou Descrição" MaxLength="100"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Tipo de Produto</label>
                                                        <asp:DropDownList runat="server" ID="ddlControle_Filtro_Tipo" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Unidade</label>
                                                        <asp:DropDownList runat="server" ID="ddlControle_Filtro_Unidade" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div runat="server" id="div3" class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Grupo</label>
                                                        <asp:DropDownList runat="server" ID="ddlControle_Filtro_Grupo" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div runat="server" id="div4" class="col-lg-2">
                                                    <div class="form-group">
                                                        <label>Família</label>
                                                        <asp:DropDownList runat="server" ID="ddlControle_Filtro_Familia" class="form-control Caixa_Selecao"></asp:DropDownList>
                                                    </div>
                                                </div>

                                            </div>

                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">

                                        <asp:Table runat="server" ID="tbLegenda_ControleValores" class="table table-striped table-bordered table-hover table-condensed" Width="100%">
                                            <asp:TableHeaderRow>
                                                <asp:TableHeaderCell HorizontalAlign="Center" VerticalAlign="Middle" ColumnSpan="6">Legenda - Cores por Tipo de Tabela</asp:TableHeaderCell>
                                            </asp:TableHeaderRow>
                                            <asp:TableRow>
                                                <asp:TableCell runat="server" ID="cell_Legenda5" CssClass="tipo-primary">Custo Fornecedor</asp:TableCell>
                                                <asp:TableCell runat="server" ID="cell_Legenda4" CssClass="tipo-primary">Industrialização TT</asp:TableCell>
                                                <asp:TableCell runat="server" ID="cell_Legenda2" CssClass="info">Custo TT</asp:TableCell>
                                                <asp:TableCell runat="server" ID="cell_Legenda3" CssClass="warning">Vendas PVP</asp:TableCell>
                                                <asp:TableCell runat="server" ID="cell_Legenda1" CssClass="success">Vendas Customizadas</asp:TableCell>
                                                <asp:TableCell runat="server" ID="cell_Legenda11" CssClass="success">LPU</asp:TableCell>
                                                <asp:TableCell runat="server" ID="cell_Legenda10" CssClass="danger">Custo Empreitada</asp:TableCell>
                                            </asp:TableRow>
                                        </asp:Table>

                                    </div>
                                </div>

                                <div runat="server" id="div_gvControleValores" class="col-lg-12">

                                    <div class="scroll_top form-group" style="overflow-x: scroll !important;">
                                        <div class="scroll_sync" style="height: 1px;"></div>
                                    </div>

                                    <div class="scroll_bottom" style="overflow-x: auto; width: 100%;">
                                        <asp:GridView ID="gvControleValores" class="table table-bordered table-hover"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="3" AutoGenerateColumns="False" GridLines="None" ShowFooter="False">
                                        </asp:GridView>
                                    </div>

                                </div>

                            </div>
                        </div>
                    </div>
                </div>

                <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="arquivos">
                    <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px">
                        <embed type="text/html" runat="server" id="eArquivos" width="800" height="500" />
                    </div>
                </div>

                <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico">
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Histórico de Itens</b></h3>
                        </div>
                        <div class="panel-body">
                            <uc1:MensagemPagina runat="server" ID="MensagemHistorico" />
                            <div class="row">
                                <div class="col-lg-12" style="padding: 0;">
                                    <div class="col-lg-3 form-group">
                                        <asp:LinkButton runat="server" ID="cmdConsulta_Historico" class="btn btn-info form-control" Text="Consultar Histórico"></asp:LinkButton>
                                    </div>
                                </div>
                            </div>
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">
                                    <asp:GridView ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False">
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" />
                <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field_cancel" title="Voltar" onclick="history.go(-1)">
            </fieldset>

            <div id="dialog-Salvar" class="modal" style="padding: 10px;" title="Salvar">
                <p>
                    <asp:Label ID="lblTituloSalvar" runat="server" Text=""></asp:Label>
                    <br />
                    <br />
                    <asp:Label runat="server" ID="lblLembrete"></asp:Label>
                </p>
            </div>

            <div class="modal fade" id="modalValidarItens_TabelasVinculadas" data-backdrop="static">
                <div class="modal-dialog" style="width: 70%;">
                    <div class="modal-content" style="overflow: visible;">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Validar Produtos</h4>
                        </div>
                        <div class="modal-body">

                            <div class="row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="Mensagem_ModalValidarItens" />
                                </div>

                                <div class="col-lg-12 form-group">
                                    <asp:ListBox runat="server" ID="lstValidarItens_TabelasVinculadas" SelectionMode="Multiple"></asp:ListBox>
                                </div>
                            </div>

                            <div class="modal-footer">
                                <asp:Button ID="cmdValidarItens_TabelasVinculadas" class="btn btn-success" runat="server" Text="Validar Itens" />
                                <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="modalDesvincularTabelas" data-backdrop="static">
                <div class="modal-dialog" style="width: 65%;">
                    <div class="modal-content" style="overflow: visible;">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Desvincular Tabelas</h4>
                        </div>
                        <div class="modal-body">

                            <div class="row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="Mensagem_ModalDesvincularTabelas" />
                                </div>

                                <div class="col-lg-12 form-group">
                                    <label>Tabelas</label>
                                    <asp:ListBox runat="server" ID="lstDesvincularTabelas" SelectionMode="Multiple" class="Caixa_Selecao"></asp:ListBox>
                                </div>
                            </div>

                            <div class="modal-footer">
                                <asp:Button ID="cmdDesvincularTabelas" class="btn btn-success desvincularTabelas" runat="server" Text="Desvincular" />
                                <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="modalExportar_LPU">
                <div class="modal-dialog" style="width: 65%;">
                    <div class="modal-content" style="overflow: visible;">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Exportar LPU</h4>
                        </div>
                        <div class="modal-body">

                            <div class="row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="Mensagem_ModalExportar_LPU" />
                                </div>

                                <div id="div_ddlVisualizacao" class="col-lg-12 form-group"></div>

                                <div class="col-lg-12">
                                    <asp:CheckBox runat="server" ID="cbProdutosCliente" Text="Produtos de Clientes" CssClass="btn btn-primary float-r" />
                                </div>
                            </div>

                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="cmdExcluirVisualizacao" class="btn btn-danger float-l" runat="server" Text="Excluir" />
                            <asp:Button ID="cmdExportarExcel" class="btn btn-success" runat="server" Text="Exportar LPU" />
                            <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

    <div id="importarExcel" style="display: none;">
        <asp:FileUpload runat="server" ID="ImportarArquivo" accept=".xls, .xlsx" />
    </div>

    <div class="modal fade" id="modalUpload_ImportarExcel" data-backdrop="static">
        <div class="modal-dialog" style="width: 40%;">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                    <h4 class="modal-title">Importar Excel</h4>
                </div>
                <div class="modal-body">

                    <div class="row">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="Mensagem_Modal_ImportarProdutos" />
                        </div>

                        <uc1:ExcelImportar runat="server" ID="ExcelImportar" />
                    </div>

                    <div class="modal-footer">
                        <asp:Button ID="cmdImportarItens_Excel_Modal" class="btn btn-success" runat="server" Text="Importar" />
                        <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                    </div>

                </div>
            </div>
        </div>
    </div>

</asp:Content>