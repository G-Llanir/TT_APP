<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" EnableEventValidation="false" AutoEventWireup="true" CodeBehind="Produtos_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Produtos_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/FiltroPesquisa.ascx" TagPrefix="uc1" TagName="FiltroPesquisa" %>
<%@ Register Src="~/App/Controles/Pesquisa_Parceiros.ascx" TagPrefix="uc1" TagName="Pesquisa_Parceiros" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/SwitchAtivo.ascx" TagPrefix="uc1" TagName="SwitchAtivo" %>
<%@ Register Src="~/App/Controles/TextBox_Padrao.ascx" TagPrefix="uc1" TagName="TextBox_Padrao" %>
<%@ Register Src="~/App/Controles/DropDownList_Padrao.ascx" TagPrefix="uc1" TagName="DropDownList_Padrao" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .hideColuna {
            display: none;
        }

        input#cphCorpo_cbsListarOrcamento {
            margin: 0px;
        }

        .center {
            display: flex;
            justify-content: center;
        }

        #lock {
            display: none;
        }

        .lock-label {
            position: relative;
            width: 45px;
            height: 45px;
            display: flex;
            align-items: center;
            justify-content: center;
            background-color: transparent;
            border-radius: 15px;
            cursor: pointer;
            transition: all 0.3s;
        }

            .lock-label:hover::after {
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

            .lock-label:hover::after {
                opacity: 1;
            }

        #lock:checked + .lock-label:hover::after {
            content: "Clique para bloquear a edição";
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

        .lock-label:active {
            transform: scale(0.9);
        }

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

        .modal-largo {
            width: 80% !important;
            max-width: none !important;
        }

        .naoAltera {
            background-color: #eee;
            cursor: not-allowed;
        }

        .toastPersonalizado {
            border: 1px solid black;
            border-radius: 5px;
        }

        .barraProgresso_toastPersonalizado {
            background-color: black;
        }

        .painelTabelas {
            display: flex;
            justify-content: space-between;
            align-items: center;
            cursor: pointer;
        }

        .explicacaoTabelas {
            cursor: pointer;
        }

        .swal2-title {
            font-size: 1.5em !important;
        }

        .swal2-icon {
            width: 3.5em !important;
            height: 3.5em !important;
            min-width: 3.5em !important;
            min-height: 3.5em !important;
        }

        td {
            word-break: break-word;
        }

        .collapsed-row {
            display: none;
            padding: 1px;
            margin: 1px;
        }
    </style>

    <asp:UpdatePanel runat="server" ID="UpdManual">
        <ContentTemplate>

            <uc1:Manual runat="server" ID="manual" />

            <asp:HiddenField runat="server" ID="hddsManual" />

        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" class="tituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>
        <div class="col-lg-12">
            <uc1:MensagemPagina runat="server" ID="MensagemPaginaGlobal" />
        </div>
    </div>

    <br />

    <div id="abas">
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <ul id="tab_Produto" class="nav nav-tabs" role="tablist">
                    <li role="presentation" class="tabpanel active" runat="server" id="aba_Produto">
                        <a href="#produto" id="aba-produto" role="tab" data-toggle="tab" aria-controls="produto" aria-expanded="false"><b runat="server" id="bAabaProduto">Produto</b></a>
                    </li>
                    <li role="presentation" id="aba_Idiomas" runat="server">
                        <a href="#descricao" role="tab" id="descricao-tab" data-toggle="tab" aria-controls="Descricao"><b>Idiomas</b></a>
                    </li>
                    <li role="presentation" id="aba_caracteristicasProduto" runat="server">
                        <a href="#caracteristicasProduto" role="tab" id="aba_caracteristicasProduto-tab" data-toggle="tab" aria-controls="caracteristicasProduto"><b>Caracteristicas do Produto</b></a>
                    </li>
                    <li role="presentation" id="aba_Arquivos" runat="server">
                        <a href="#arquivos" role="tab" id="arquivos-tab" data-toggle="tab" aria-controls="Arquivos"><b>Arquivos</b></a>
                    </li>
                    <li role="presentation" id="aba_Composicao" runat="server">
                        <a href="#composicao" role="tab" id="composicao-tab" data-toggle="tab" aria-controls="Composicao"><b>Composição</b></a>
                    </li>
                    <li role="presentation" id="aba_Instalacao" runat="server" visible="false"><%--Instalação/Obra: só para item do tipo Produto--%>
                        <a href="#instalacao" role="tab" id="instalacao-tab" data-toggle="tab" aria-controls="Instalacao"><b>Instalação/Obra</b></a>
                    </li> 
                    <li role="presentation" id="aba_Fabricacao" runat="server">
                        <a href="#fabricacao" role="tab" id="fabricacao-tab" data-toggle="tab" aria-controls="Fabricacao"><b>Fabricação</b></a>
                    </li>
                    <li role="presentation" id="aba_Movimentacao" runat="server">
                        <a href="#movimentacao" role="tab" id="movimentacao-tab" data-toggle="tab" aria-controls="Movimentacao"><b>Movimentação</b></a>
                    </li>
                    <li role="presentation" id="aba_Lotes" runat="server"><%--Thiago Rodrigues *08/01/2025--%>
                        <a href="#lotes" role="tab" id="lotes-tab" data-toggle="tab" aria-controls="lotes"><b>Lotes</b></a>
                    </li>
                    <li role="presentation" id="aba_Sugestao" runat="server">
                        <a href="#sugestao" role="tab" id="sugestao-tab" data-toggle="tab" aria-controls="Sugestao"><b>Sugestão</b></a>
                    </li>
                    <li role="presentation" id="aba_Fornecedores" runat="server">
                        <a href="#fornecedores" role="tab" id="fornecedores-tab" data-toggle="tab" aria-controls="Fornecedores"><b>Fornecedores</b></a>
                    </li>
                    <li role="presentation" id="aba_Clientes" runat="server">
                        <a href="#clientes" role="tab" id="clientes-tab" data-toggle="tab" aria-controls="Clientes"><b>Parceiros</b></a>
                    </li>
                    <li role="presentation" id="aba_Documentacao" runat="server"><%--Agnes Partal *19/08/2024--%>
                        <a href="#documentacao" role="tab" id="documentacao-tab" data-toggle="tab" aria-controls="documentacao"><b>Documentação</b></a>
                    </li>
                    <li role="presentation" id="aba_EpiConsumiveis" runat="server" visible="false"><%--Thiago Rodrigues *05/08/2025--%>
                        <a href="#EpiConsumiveis" role="tab" id="EpiConsumiveis-tab" data-toggle="tab" aria-controls="epiConsumiveis"><b>EPI/Consumíveis</b></a>
                    </li>
                    <li role="presentation" id="aba_Tabelas" runat="server">
                        <a href="#tabelas" role="tab" id="tabelas-tab" data-toggle="tab" aria-controls="tabelas"><b>Tabelas de Preços</b></a>
                    </li>
                    <li role="presentation" id="aba_Historico" runat="server"><%--Agnes Partal * 05/11/2024--%>
                        <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="historico"><b>Histórico</b></a>
                    </li>

                </ul>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>

    <br />

    <div id="tab" class="tab-content">

        <div role="tabpanel" class="tab-pane fade in active" id="produto" aria-labelledby="produto-tab">
            <asp:UpdatePanel ID="upDetalhe1" runat="server">
                <ContentTemplate>

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                    <asp:Button runat="server" ID="cmdGAMBIARRA" OnClick="cmdGAMBIARRA_Click" CssClass="drax" Style="display: none;" />

                    <div class="panel panel-default" runat="server" id="div_Cabecalho">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Detalhe</b></h3>
                        </div>
                        <div class="panel-body">
                            <br />
                            <div class="form-stacked">

                                <div class="row">
                                    <div class="col-lg-12">
                                        <div runat="server" id="div_Id_Codigo_Desc_Img1" class="col-lg-6">
                                            <div class="row">
                                                <div runat="server" id="div_ID" class="col-lg-4">
                                                    <div runat="server" id="div_ID1" class="form-group">
                                                        <label>ID</label>

                                                        <div class="form-group input-group">

                                                            <asp:LinkButton ID="cmRetornar" class="btn btn-sm btn-info input-group-addon colorLink" runat="server" OnClick="cmdRetornar_click"><b><</b></asp:LinkButton>

                                                            <asp:TextBox ID="txtidProduto" class="form-control" runat="server" disabled="0"></asp:TextBox>

                                                            <asp:LinkButton ID="cmAvancar" class="btn btn-sm btn-info input-group-addon colorLink" runat="server" OnClick="cmdAvancar_click"><b>></b></asp:LinkButton>

                                                        </div>
                                                    </div>
                                                </div>
                                                <div runat="server" id="div_Codigo" class="col-lg-8">
                                                    <div runat="server" id="div_Codigo1" class="form-group">
                                                        <label runat="server" id="lblCodigo">Código do Produto </label>
                                                        <asp:TextBox ID="txtsCodigo" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                    <div class="col-lg-2" id="div_Unidade" runat="server">
                                                        <div class="form-group">
                                                            <label>Unidade</label>
                                                            <asp:DropDownList ID="ddlsUnidade" class="form-control" runat="server"></asp:DropDownList>
                                                        </div>
                                                    </div>
                                                </div>
                                                <div runat="server" id="div_Desc" class="col-lg-12">
                                                    <div runat="server" id="div_Desc1" class="form-group">
                                                        <label runat="server" id="lblDescricao">Descrição em português </label>
                                                        <asp:TextBox ID="txtsDscProduto" class="form-control" runat="server" MaxLength="250"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div runat="server" id="div_ImagemProduto" class="col-lg-4">
                                            <div class="form-group">
                                                <asp:Image ID="imgProdutoPrincipal" runat="server" class="text-center" Height="150" Width="150" Style="display: inline-block; vertical-align: middle; margin-right: 10px;" />
                                            </div>
                                        </div>
                                        <div runat="server" id="div_bloquearEdicao" class="col-lg-2" style="display: flex; justify-content: right;">
                                            <div class="form-group">
                                                <input type="checkbox" id="lock" <%= hddsCadeado.Value == "N" ? "checked='checked'" : "" %> />
                                                <label for="lock" class="<%= hddsPermissaoCadeado.Value == "0" ? "lock-label lock-label-closed" : "lock-label" %>">
                                                    <span class="lock-wrapper">
                                                        <span class="<%= hddsCadeado.Value == "N" ? "shackle shackle-open" : "shackle shackle-closed" %>"></span>
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
                                    </div>

                                </div>

                                <div runat="server" id="div_Tipo" class="col-lg-6">
                                    <div class="form-group">
                                        <label runat="server" id="lblTipo">Tipo do Produto </label>
                                        <asp:DropDownList ID="ddlTipoProduto" runat="server" class="form-control" attrname="TipoProduto" AutoPostBack="True" OnSelectedIndexChanged="ddlTipoProduto_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div runat="server" id="div_Grupos" class="col-lg-3">
                                    <div class="form-group">
                                        <label>Grupo </label>
                                        <asp:DropDownList ID="ddlGrupo" class="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div runat="server" id="div2" class="col-lg-3">
                                    <div runat="server" id="div3" class="form-group">
                                        <label runat="server" id="Label2">Fabricante</label>
                                        <asp:TextBox ID="txtsFabricante" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div runat="server" id="div_localArmazenamento" class="col-lg-6">
                                    <div class="form-group">
                                        <label>Local de Armazenagem Padrão</label>
                                        <asp:DropDownList ID="ddlArmazenagem" class="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div runat="server" id="div_Familia" class="col-lg-3">
                                    <div class="form-group">
                                        <label>Familia </label>
                                        <asp:DropDownList ID="ddlFamilia" class="form-control" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3" runat="server" id="DIV_Sistemas">
                                    <div class="form-group">
                                        <b>
                                            <label>Sistemas</label></b>
                                        <asp:ListBox ID="ddlsSistemas" runat="server" class="form-control yes_no select Caixa_Selecao" SelectionMode="Multiple"></asp:ListBox>
                                    </div>
                                </div>

                                <asp:UpdatePanel ID="updPanel_CEST" runat="server">
                                    <ContentTemplate>

                                        <div class="col-lg-6">
                                            <div class="row">
                                                <div class="col-lg-4">
                                                    <div class="form-group">
                                                        <label>Código CEST</label>
                                                        <asp:TextBox ID="txtCodigoCEST" class="form-control" runat="server" MaxLength="10" OnTextChanged="txtsCodigoCEST_TextChanged" AutoPostBack="True"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-lg-8">
                                                    <div class="form-group">
                                                        <label>Selecione CEST</label>
                                                        <asp:DropDownList ID="ddlCEST" class="form-control" OnSelectedIndexChanged="ddlCEST_SelectedIndexChanged" runat="server" AutoPostBack="true"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-lg-6">

                                            <div class="row">
                                                <div class="col-lg-4">
                                                    <div class="form-group">
                                                        <label>Código NCM</label>
                                                        <asp:TextBox ID="txtCodigoNCM" runat="server" class="form-control" MaxLength="10" OnTextChanged="txtsCodigoNCM_TextChanged" AutoPostBack="True"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-lg-8">
                                                    <div class="form-group">
                                                        <label>Selecione NCM</label>
                                                        <asp:DropDownList ID="ddlNCM" class="form-control" OnSelectedIndexChanged="ddlNCM_SelectedIndexChanged" runat="server" AutoPostBack="true" DataTextField="sDscNCM" DataValueField="sCodigoNCM"></asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>

                                        </div>
                                    </ContentTemplate>
                                </asp:UpdatePanel>

                                <div runat="server" id="div_CategoriaVendas" class="col-lg-6">
                                    <div class="form-group">
                                        <label>Categoria de Vendas </label>
                                        <asp:DropDownList ID="ddlidCategoriaVendas" runat="server" class="form-control"></asp:DropDownList>
                                    </div>
                                </div>

                                <div runat="server" id="div_paisOrigem" class="col-lg-6">
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <div class="form-group">
                                                <label>País de Origem</label>
                                                <asp:DropDownList ID="ddlPaisOrigem" class="form-control" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <uc1:ComboAtivo runat="server" ID="ComboAtivo" />
                                        </div>

                                    </div>
                                </div>

                                <div runat="server" id="div_ServicoMunicipal_NBS_IndOp" class="col-lg-12 padd-0">

                                    <div class="col-lg-3 form-group">
                                        <uc1:DropDownList_Padrao runat="server" ID="ddlidServicoMunicipal" Titulo="Serviço Municipal" Obrigatorio="true" Validacao="true" AutoPostBack="true" OnSelectedIndexChanged="ddlidServicoMunicipal_SelectedIndexChanged" Link="/App/Paginas/Adm/Fiscal/ServicoMunicipal.aspx?id=" />
                                    </div>

                                    <div runat="server" id="div_codigoServicoPai" class="col-lg-4 form-group" visible="false">
                                        <label>Serviço Federal Pai</label>
                                        <asp:DropDownList ID="ddlsCodigoServicoPai" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>

                                    <div runat="server" id="div_codigoServicoFilho" class="col-lg-5 form-group" visible="false">
                                        <label>Serviço Federal Filho</label>
                                        <asp:DropDownList ID="ddlsCodigoServicoFilho" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>

                                    <div class="col-lg-6 form-group">
                                        <uc1:DropDownList_Padrao runat="server" ID="ddlNBS" Titulo="Código NBS" Obrigatorio="true" Validacao="true" Link="/App/Paginas/Adm/Manutencao/NBS.aspx?id=" />
                                    </div>

                                    <div class="col-lg-6 form-group">
                                        <uc1:DropDownList_Padrao runat="server" ID="ddlIndOp" Titulo="Indicador de Operação" Obrigatorio="true" Validacao="true" Link="/App/Paginas/Adm/Manutencao/IndicadorOperacao.aspx?id=" />
                                    </div>

                                </div>

                                <div runat="server" id="div_ExibeComercial_LM" class="col-lg-6">
                                    <div class="row">
                                        <div runat="server" id="div_ExibeComercial" class="col-lg-6">
                                            <div class="form-group">
                                                <label>Exibe em Comercial </label>
                                                <asp:DropDownList ID="ddlExibeComercial" runat="server" class="form-control">
                                                    <asp:ListItem Text="Sim" Value="S" />
                                                    <asp:ListItem Text="Não" Value="N" />
                                                    <asp:ListItem Text="Sim - Liberado Cliente" Value="C" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                        <div runat="server" id="div_LMO" class="col-lg-6" visible="false">
                                            <div class="form-group">
                                                <label>Exibe em LME/LMO </label>
                                                <asp:DropDownList ID="ddlExibeLM" runat="server" class="form-control">
                                                    <asp:ListItem Text="Sim" Value="S" />
                                                    <asp:ListItem Text="Não" Value="N" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div runat="server" id="divEAN" class="col-lg-6" visible="false">
                                    <div class="row">
                                        <div class="form-group col-lg-4">
                                            <label>Código EAN</label>
                                            <asp:TextBox ID="txtsCodigoEAN" runat="server" class="form-control" MaxLength="40"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12" runat="server" id="DIV_CategoriaPagar">
                                    <div class="col-lg-12 row">
                                        <div class="form-group">
                                            <label>Categoria Contas Pagar</label>
                                            <asp:DropDownList ID="ddlCategoriapagar" runat="server" class="form-control CaixaTextoGrande">
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-lg-2 row">
                                        <div class="form-group">
                                            <label>Valor Máximo (R$)</label>
                                            <asp:TextBox ID="txtvlrMaximo" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12" runat="server" id="DIV_Gastos">
                                    <uc1:SwitchAtivo runat="server" ID="ExibirGastos" />
                                </div>

                                <div runat="server" id="divComboAtivo2" class="col-lg-12">
                                    <uc1:ComboAtivo runat="server" ID="comboAtivoServico" />
                                </div>

                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server" id="divEtiquetas" visible="false">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Etiquetas</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaEtiquetas" />
                                </div>

                                <div class="col-lg-2" id="div_Impressora" runat="server">
                                    <div class="form-group">
                                        <label id="lblImpressora" runat="server">Impressora</label>
                                        <asp:DropDownList ID="ddlImpressora" runat="server" AutoPostBack="False" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-sm-2">
                                    <div runat="server" id="div8" class="form-group">
                                        <label runat="server" id="Label4">Nº Etiquetas</label>
                                        <asp:TextBox ID="txtnQuantidade" step="1" TextMode="Number" class="form-control" runat="server" MaxLength="25" placeholder="Quantidade"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" id="div9" runat="server" style="padding-top: 25px;">
                                    <asp:Button ID="cmdGerarEtiquetas" class="btn btn-sm btn-success" runat="server" Text="Gerar Etiquetas" OnClick="cmdGerarEtiquetas_Click" />
                                </div>
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddlTipoProduto" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="ddlUnidade" EventName="SelectedIndexChanged" />
                </Triggers>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="arquivos" aria-labelledby="arquivos-tab">
            <div class=" embed-responsive embed-responsive-16by9" style="min-height: 800px" runat="server" id="upArquivos">
                <embed type="text/html" runat="server" id="frmArquivos" width="900" height="500" />
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="descricao" aria-labelledby="descricao-tab">
            <asp:UpdatePanel ID="UpdatePanel4" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b runat="server" id="panelTitle_Descricao">Descrição Produto</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemProdutos_Descricao" />
                                </div>
                                <div>

                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <label>Tipo da Descrição </label>
                                            <asp:DropDownList ID="ddlDescricao_IdTipo" class="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <label>País</label>
                                            <asp:DropDownList ID="ddlidPais" class="form-control" runat="server"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>Descrição</label>
                                            <asp:TextBox ID="txtsDscDescricao" class="form-control " runat="server" MaxLength="500" TextMode="MultiLine" Height="120px"></asp:TextBox>
                                        </div>
                                    </div>


                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <br />
                                            <asp:Button ID="cmdProdutos_Descricao_Incluir" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdProdutos_Descricao_Incluir_Click" />
                                        </div>
                                    </div>
                                    <br />
                                </div>

                                <div class="col-lg-12">
                                    <asp:GridView ID="dtgProdutos_Descricao" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDeleting="dtgProdutos_Descricao_RowDeleting" OnRowCommand="dtgProdutos_Descricao_RowCommand">
                                        <Columns>

                                            <asp:BoundField DataField="sDscTipo" HeaderText="Tipo">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscPais" HeaderText="País">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscDescricao" HeaderText="Descrição">
                                                <ItemStyle Width="74%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Editar">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkDescricao_Editar" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Editar" CommandArgument='<%# Container.DataItemIndex.ToString() %>'><i class="fa fa-pencil"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Excluir">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkDescricao_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa fa-eraser"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
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

        <div role="tabpanel" class="tab-pane fade" id="caracteristicasProduto" aria-labelledby="caracteristicasProduto-tab">
            <asp:UpdatePanel runat="server">
                <ContentTemplate>

                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Configurações de Armazenagem</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:Image ID="Image3" ImageUrl="/App/img/award-solid.svg" runat="server" class="text-center" Height="120" Width="120" Style="display: inline-block; vertical-align: middle; margin-right: 10px;" />
                                    </div>
                                </div>

                                <div runat="server" class="col-lg-3">
                                    <div class="form-group">
                                        <label>Local OPI</label>
                                        <asp:DropDownList ID="ddlLocalOPI" class="form-control  yes_no select Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>


                                <div runat="server" class="col-lg-2">
                                    <div class="form-group">
                                        <label>Tipo de Etiqueta</label>
                                        <asp:DropDownList ID="ddlTipoEtiqueta" class="form-control yes_no select Caixa_Selecao" runat="server" ToolTip="Unitária(Vai impirimir etiqueta um pra um), Agrupada(vai imprimir uma etiqueta para muitos)">
                                            <asp:ListItem Text="Unitária" Value="U" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="Agrupada" Value="A"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <%--Thiago Rodrigues 19/11/2024--%>
                                <div runat="server" id="div_Garantia" class="col-lg-1">
                                    <div class="form-group">
                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivoGarantia" ClientIDMode="Static" onclientstatechanged="toggleGarantiaDiv" />
                                    </div>
                                </div>

                                <div runat="server" id="div_meses" class="col-lg-2" style="display: none;">
                                    <div class="form-group">
                                        <label>Garantia (Meses)</label>
                                        <asp:TextBox ID="nTempoGarantia" class="form-control" runat="server" onkeypress="return validarInteiro(event);" oninput="formatarValor(this);" step="0" Style="width: 150px;"></asp:TextBox>
                                    </div>
                                </div>
                                <div runat="server" id="div_seriealizavel" class="col-lg-2">
                                    <div class="form-group">
                                        <uc1:SwitchAtivo runat="server" ID="SwitchAtivoSerivalizavel" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>


            <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Produto</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:Image ID="Img2" ImageUrl="/App/img/wms_dimensoes.svg" runat="server" class="text-center" Height="120" Width="120" Style="display: inline-block; vertical-align: middle; margin-right: 10px;" />
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Altura (mm)</label>
                                        <asp:TextBox ID="txtnAltura" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Largura (mm)</label>
                                        <asp:TextBox ID="txtnLargura" class="form-control" runat="server"></asp:TextBox>
                                    </div>

                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Comprimento (mm)</label>
                                        <asp:TextBox ID="txtnComprimento" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Vol. (m3)</label>
                                        <asp:TextBox ID="txtnVolume" class="form-control" runat="server" ReadOnly="true"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Unidade</label>
                                        <asp:DropDownList ID="ddlUnidade" class="form-control" attrname="Ibama" runat="server" OnSelectedIndexChanged="ddlUnidadeProduto_SelectedIndexChanged">
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Peso neto (kg)</label>
                                        <asp:TextBox ID="txtnPesoNeto" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Peso bruto (kg)</label>
                                        <asp:TextBox ID="txtnPesoBruto" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Unidade de Entrega</label>
                                        <asp:DropDownList ID="ddlUnidadeEntrega" class="form-control" attrname="Ibama" runat="server">
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Ibama</label>
                                        <asp:DropDownList ID="ddlIbama" class="form-control" attrname="IBAMA" runat="server">
                                            <asp:ListItem Text="Possui IBAMA?" Value="0"></asp:ListItem>
                                            <asp:ListItem Text="SIM" Value="SIM"></asp:ListItem>
                                            <asp:ListItem Text="NÃO" Value="NÃO"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>

                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Unidade mínima compra</label>
                                        <asp:TextBox ID="txtsUnMinimaCompra" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Saldo</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="form-group">
                                    <div class="col-lg-2">
                                        <asp:Image ID="imgSaldo" ImageUrl="../../img/wms_saldos.png" runat="server" class="text-center" Height="120" Width="120" Style="display: inline-block; vertical-align: middle; margin-right: 10px;" />
                                    </div>
                                </div>
                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Saldo Atual</label>
                                        <asp:TextBox ID="txtnEstoqueAtual" class="form-control" ReadOnly="true" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Reservado</label>
                                        <asp:TextBox ID="txtnEstoqueReservado" class="form-control" ReadOnly="true" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Estoque Minimo</label>
                                        <asp:TextBox ID="txtnEstoqueMinimo" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>



                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Transporte</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="form-group">
                                    <div class="col-lg-2">
                                        <asp:Image ID="imgTransporte" ImageUrl="~/App/img/wms_transporte.png" runat="server" class="text-center" Height="120" Width="120" Style="display: inline-block; vertical-align: middle; margin-right: 10px;" />
                                    </div>
                                </div>
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>Cubagem (m3)</label>
                                        <asp:TextBox ID="txtnCubagem" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <label>UMZ Ratio </label>
                                        <asp:TextBox ID="txtnUMZ_Ratio" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="movimentacao" aria-labelledby="movimentacao-tab">
            <asp:UpdatePanel ID="updpHistorico" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Movimentação</b></h3>
                        </div>
                        <div class="panel-body">

                            <div class="row">

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Tipo de Movimentação</label>
                                        <asp:DropDownList ID="ddliTipoAlteracao" runat="server" class="form-control" OnSelectedIndexChanged="ddlPeriodo_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-3" id="div_Motivo" runat="server" visible="false">
                                    <div class="form-group">
                                        <label>Motivo</label>
                                        <asp:DropDownList ID="ddlMotivo" runat="server" class="form-control" OnSelectedIndexChanged="ddlPeriodo_SelectedIndexChanged" AutoPostBack="true"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <br />
                                        <asp:Button ID="Movimentacao_cmdConsultar" class="btn btn-info" runat="server" ValidationGroup="Movimentacao" Text="Consultar" OnClick="Movimentacao_cmdConsultar_Click" />
                                    </div>
                                </div>

                                <br />
                                <br />

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaMovimentacao" />
                                </div>

                                <div class="col-lg-12" runat="server" id="div_Movimentacao">
                                    <div class="form-group">
                                        <asp:GridView ID="dtgMovimentacaoItens" class="table table-striped table-bordered table-hover table-condensed"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgMovimentacaoItens_RowDataBound">
                                            <Columns>

                                                <asp:HyperLinkField DataNavigateUrlFields="idMovimentacao"
                                                    DataTextField="idMovimentacao" HeaderText="Movimentação"
                                                    DataNavigateUrlFormatString="../WMS/Movimentacao_Detalhe.aspx?id={0}" Target="_blank">
                                                    <HeaderStyle Width="8%" />
                                                    <ItemStyle HorizontalAlign="Left" Width="8%" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:BoundField DataField="dtMovimentacao" HeaderText="Data">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscTipoMovimentacao" HeaderText="Tipo">
                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscMotivo" HeaderText="Motivo">
                                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nQuantidadeMovimentacao" HeaderText="Quantidade" HeaderStyle-HorizontalAlign="Right" DataFormatString="{0:N4}">
                                                    <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:HyperLinkField DataNavigateUrlFields="sLink"
                                                    DataTextField="sObservacao" HeaderText="Origem/Observação"
                                                    DataNavigateUrlFormatString="{0}" Target="_blank">
                                                    <HeaderStyle Width="20%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>



                                                <asp:BoundField DataField="sDscStatus" HeaderText="Status">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nSaldo" HeaderText="Saldo" HeaderStyle-HorizontalAlign="Right" DataFormatString="{0:N4}">
                                                    <ItemStyle Width="8%" Font-Bold="true" HorizontalAlign="Right" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </ContentTemplate>
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="ddliTipoAlteracao" EventName="" />
                </Triggers>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="composicao" aria-labelledby="composicao-tab">
            <asp:UpdatePanel ID="updpComposicao" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Composição</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">

                                <div class="col-lg-12">

                                    <div class="row">
                                        <div runat="server" id="div_FiltroPesquisa" class="col-lg-10">
                                            <uc1:FiltroPesquisa runat="server" ID="FiltroPesquisa" />
                                        </div>

                                        <div runat="server" id="div_ExibeComrcial_Composicao" class="col-sm-1">
                                            <div class="form-group">
                                                <label runat="server" id="lblExibeComrcial_Composicao">Exibe em Comercial/LM?</label>
                                                <asp:DropDownList ID="ddlExibePedido" runat="server" class="form-control">
                                                    <asp:ListItem Text="Sim" Value="S"></asp:ListItem>
                                                    <asp:ListItem Text="Não" Value="N"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_cmdComposicao_IncluirItem" class="col-sm-1">
                                            <div class="form-group">
                                                <label runat="server" id="lblcmdComposicao_IncluirItem">&nbsp;</label>
                                                <br />
                                                <div runat="server" id="divbr2_Composicao_IncluirItem">
                                                    <br />
                                                </div>
                                                <asp:Button ID="cmdComposicao_IncluirItem" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdComposicao_IncluirItem_Click" />
                                                <asp:Button ID="cmdComposicao_Alterar" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Alterar" OnClick="cmdComposicao_IncluirItem_Click" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="form-group">
                                        <div class="row" runat="server" id="div_SelecaoItens">
                                            <asp:HiddenField ID="hddComposicao_idProduto" runat="server" />
                                            <asp:HiddenField ID="hddComposicao_sDscTipoProduto" runat="server" />


                                            <div class="col-lg-3">
                                                <br />

                                                <asp:Label ID="lblMensagem_Item" runat="server" Text="" Visible="false" CssClass="label-danger"></asp:Label>
                                            </div>
                                        </div>
                                        <uc1:MensagemPagina runat="server" ID="Composicao_MensagemPagina" />


                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="dtgItens" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDeleting="dtgItens_RowDeleting">
                                                    <Columns>

                                                        <asp:BoundField DataField="idItemComposicao" HeaderText="ID">
                                                            <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="Ordem">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="nOrdem" class="form-control" Text='<%# Bind("nOrdem") %>' runat="server"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idItemComposicao"
                                                            DataTextField="sCodigo" HeaderText="Código"
                                                            DataNavigateUrlFormatString="Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="10%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                        </asp:HyperLinkField>

                                                        <asp:HyperLinkField DataNavigateUrlFields="idItemComposicao"
                                                            DataTextField="sDscProduto" HeaderText="Descrição"
                                                            DataNavigateUrlFormatString="Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                            <HeaderStyle Width="46%" />
                                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                        </asp:HyperLinkField>

                                                        <asp:BoundField DataField="TipoProduto" HeaderText="Tipo">
                                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sUnidade" HeaderText="Unidade" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:TemplateField HeaderText="Quantidade">
                                                            <ItemTemplate>
                                                                <asp:TextBox ID="txtnQuantidade" class="form-control" Text='<%# Bind("nQuantidade") %>' runat="server"></asp:TextBox>
                                                            </ItemTemplate>
                                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="Exibir em Comercial">
                                                            <ItemTemplate>

                                                                <asp:DropDownList ID="ddlExibePedidoGV" runat="server" class="form-control" SelectedValue='<%# Bind("sExibePedido") %>'>
                                                                    <asp:ListItem Text="Sim" Value="Sim"></asp:ListItem>
                                                                    <asp:ListItem Text="Não" Value="Não"></asp:ListItem>
                                                                </asp:DropDownList>

                                                            </ItemTemplate>
                                                            <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:TemplateField>

                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
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
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
   <div role="tabpanel" class="tab-pane fade" id="instalacao" aria-labelledby="instalacao-tab">
            <asp:UpdatePanel ID="updpInstalacao" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Instalação/Obra</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="Instalacao_MensagemPagina" />
                                    <div class="row">
                                        <div runat="server" id="div_Instalacao_Filtro" class="col-lg-11">
                                            <uc1:FiltroPesquisa runat="server" ID="fpInstalacao" />
                                        </div>
                                        <div runat="server" id="div_Instalacao_Incluir" class="col-sm-1">
                                            <div class="form-group">
                                                <label>&nbsp;</label>
                                                <br />
                                                <asp:Button ID="cmdInstalacao_IncluirItem" class="btn btn-info" runat="server" ValidationGroup="Instalacao" Text="Incluir" OnClick="cmdInstalacao_IncluirItem_Click" />
                                            </div>
                                        </div>
                                    </div>



                                    <div class="row">
                                        <div class="col-lg-12">
                                            <asp:GridView ID="dtgInstalacao" class="table table-striped table-bordered table-hover table-condensed"
                                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDeleting="dtgInstalacao_RowDeleting">
                                                <Columns>

                                                    <asp:TemplateField HeaderText="Ordem">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtInstalacao_nOrdem" class="form-control" Text='<%# Bind("Ordem") %>' runat="server"></asp:TextBox>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:TemplateField>

                                                    <asp:HyperLinkField DataNavigateUrlFields="IdItemInstalacao"
                                                        DataTextField="Codigo" HeaderText="Código"
                                                        DataNavigateUrlFormatString="Produtos_Detalhe.aspx?id={0}&amp;stp=1">
                                                        <HeaderStyle Width="12%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                    </asp:HyperLinkField>

                                                    <asp:BoundField DataField="Descricao" HeaderText="Descrição">
                                                        <ItemStyle Width="47%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="Tipo" HeaderText="Tipo">
                                                        <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="Unidade" HeaderText="Unidade">
                                                        <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:TemplateField HeaderText="HH">
                                                        <ItemTemplate>
                                                            <asp:TextBox ID="txtInstalacao_nHH" class="form-control" Text='<%# Bind("HH", "{0:N2}") %>' runat="server"></asp:TextBox>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:TemplateField>

                                                    <asp:TemplateField HeaderText="">
                                                        <ItemTemplate>
                                                            <asp:LinkButton ID="lnkInstalacao_Excluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                        </ItemTemplate>
                                                        <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:TemplateField>

                                                </Columns>
                                            </asp:GridView>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-3 col-lg-offset-9">
                                            <div class="form-group">
                                                <label>Total HH</label>
                                                <asp:TextBox ID="txtInstalacao_TotalHH" runat="server" class="form-control" ReadOnly="true" Style="text-align: end; background-color: lightgreen;"></asp:TextBox>
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
        <div role="tabpanel" class="tab-pane fade" id="fabricacao" aria-labelledby="fabricacao-tab">
            <asp:UpdatePanel ID="UpdatePanel6" runat="server">
                <ContentTemplate>

                    <uc1:MensagemPagina runat="server" ID="Fabricacao_MensagemPagina" />

                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>&nbsp;Resumo</b></h3>
                        </div>
                        <div class="panel-body" id="panelFabricacaoTotais">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="row">

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Total Matéria Prima</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon">R$</span>
                                                    <asp:TextBox ID="txtnTotalProduto" runat="server" class="form-control" ReadOnly="true" Style="text-align: end;"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Total Insumos e Ferramentas</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon">R$</span>
                                                    <asp:TextBox ID="txtnTotalInsumoFerramenta" runat="server" class="form-control" ReadOnly="true" Style="text-align: end;"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Total Recursos</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon">R$</span>
                                                    <asp:TextBox ID="txtnTotalRecursos" runat="server" class="form-control" ReadOnly="true" Style="text-align: end;"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Total Fabricação</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon">R$</span>
                                                    <asp:TextBox ID="txtnTotalFabricacao" runat="server" class="form-control" ReadOnly="true" Style="background-color: lightgreen; text-align: end;"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>&nbsp;Matéria Prima</b></h3>
                        </div>
                        <div class="panel-body" id="panelFabricacaoProduto">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="row">
                                        <div runat="server" id="div4" class="col-lg-12" style="padding: 0px;">
                                            <div runat="server" id="div12" class="col-lg-11">
                                                <uc1:FiltroPesquisa runat="server" ID="fpFabricacaoProduto" />
                                            </div>
                                            <div class="col-lg-1">
                                                <asp:Button ID="btnIncluirProduto" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdFabricacaoProduto_Click" Style="margin-top: 45px;" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <asp:GridView ID="dtgv_fabricacao_produto_tipo" class="table table-striped table-bordered table-hover " DataKeyNames="idTipo" data-grid-type="Produto"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgv_fabricacao_produto_tipo_RowDataBound">
                                            <Columns>

                                                <asp:TemplateField>
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnToggle" ClientIDMode="Static" runat="server" CssClass="composicaoLinha" data-div-id='<%# "tr_1_Fabricacao" + string.Format("{0}", Eval("idTipo")) %>'></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" Font-Bold="true" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Total">
                                                    <ItemTemplate>
                                                        <asp:Label ID="nTotalTipo" runat="server" Text='<%# Eval("nTotalTipo", "R$ {0:N4}") %>' Font-Bold="true"></asp:Label>
                                                        <%# NovaLinha(string.Format("{0}", Eval("idTipo")), "_1_Fabricacao") %>

                                                        <asp:GridView ID="dtgv_fabricacao_produto" class="table table-striped table-bordered table-hover table-condensed" data-grid-type="Produto"
                                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idFabricacaoProduto, idTabela"
                                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDeleting="dtgv_fabricacao_produto_RowDeleting">
                                                            <Columns>

                                                                <asp:BoundField DataField="idFabricacaoProduto" HeaderText="ID">
                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Ordem">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="nOrdem" class="form-control" Text='<%# Bind("nOrdem") %>' runat="server"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:HyperLinkField DataNavigateUrlFields="idFabricacaoProduto"
                                                                    DataTextField="sCodigo" HeaderText="Código do Produto"
                                                                    DataNavigateUrlFormatString="Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                    <HeaderStyle Width="10%" />
                                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                                </asp:HyperLinkField>

                                                                <asp:TemplateField HeaderText="Descrição do Produto">
                                                                    <ItemTemplate>
                                                                        <div>
                                                                            <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDescricao") %>'
                                                                                OnClientClick='<%# "openModal(\"" + Eval("idFabricacaoProduto") + "\"); return false;" %>'
                                                                                OnMouseOver='<%# "mostraCard(this, \"" + Eval("idFabricacaoProduto") + "\", \"produto\");" %>'
                                                                                OnMouseOut='<%# "escondeCard( \"" + Eval("idFabricacaoProduto") + "\", \"produto\");" %>'
                                                                                CssClass="produto-detalhe-link"
                                                                                ClientIDMode="Static"
                                                                                data-idproduto='<%# Eval("idFabricacaoProduto") %>'
                                                                                data-tabela="produto" />

                                                                            <div id='<%# Eval("idFabricacaoProduto") + "_produto" %>' class="product-card" style="display: none;">
                                                                                <!--conteudo via script -->
                                                                            </div>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="39%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:BoundField DataField="sUnidade" HeaderText="Unidade" HeaderStyle-HorizontalAlign="Center">
                                                                    <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:BoundField>

                                                                <asp:TemplateField HeaderText="Quantidade">
                                                                    <ItemTemplate>
                                                                        <asp:TextBox ID="txtnQtd" class="form-control" Text='<%# Bind("nQtd") %>' runat="server"></asp:TextBox>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Valor Unitário">
                                                                    <ItemTemplate>
                                                                        <div class="input-group">
                                                                            <span class="input-group-addon">R$</span>
                                                                            <asp:TextBox ID="txtnValor" class="form-control" Text='<%# Bind("nValorUnitario", "{0:N4}") %>' runat="server" Style="z-index: 0;"></asp:TextBox>
                                                                        </div>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="Total Produto">
                                                                    <ItemTemplate>
                                                                        <asp:Label ID="lblTotal" class="lblTotal" Text='<%# Eval("nTotal", "R$ {0:N4}") %>' runat="server"></asp:Label>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="10%" HorizontalAlign="Right" />
                                                                </asp:TemplateField>

                                                                <asp:TemplateField HeaderText="">
                                                                    <ItemTemplate>
                                                                        <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete" CommandArgument='<%# Eval("idFabricacaoProduto") %>'>
                                                                                <i class="fa-eraser fa"></i>
                                                                        </asp:LinkButton>
                                                                    </ItemTemplate>
                                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                </asp:TemplateField>

                                                            </Columns>
                                                        </asp:GridView>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>&nbsp;Insumos e Ferramentas</b></h3>
                        </div>
                        <div class="panel-body" id="panelFabricacaoInsumoFerramenta">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="row">
                                        <div runat="server" id="div13" class="col-lg-12" style="padding: 0px;">
                                            <div runat="server" id="div14" class="col-lg-11">
                                                <uc1:FiltroPesquisa runat="server" ID="fpFabricacaoInsumoFerramenta" />
                                            </div>
                                            <div class="col-lg-1">
                                                <asp:Button ID="btnIncluirInsumoFerramenta" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirInsumoFerramenta_Click" Style="margin-top: 45px;" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <asp:GridView ID="gv_fabricacao_insumo_ferramenta" class="table table-striped table-bordered table-hover table-condensed" data-grid-type="InsumoFerramenta"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idFabricacaoInsumoFerramenta"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDeleting="gv_fabricacao_insumo_ferramenta_RowDeleting">
                                            <Columns>

                                                <asp:BoundField DataField="idFabricacaoInsumoFerramenta" HeaderText="ID">
                                                    <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Ordem">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="nOrdem" class="form-control" Text='<%# Bind("nOrdem") %>' runat="server"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idFabricacaoInsumoFerramenta"
                                                    DataTextField="sCodigo" HeaderText="Código do Produto"
                                                    DataNavigateUrlFormatString="Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                                </asp:HyperLinkField>

                                                <asp:TemplateField HeaderText="Descrição do Produto">
                                                    <ItemTemplate>
                                                        <div>
                                                            <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDescricao") %>'
                                                                OnClientClick='<%# "openModal(\"" + Eval("idFabricacaoInsumoFerramenta") + "\"); return false;" %>'
                                                                OnMouseOver='<%# "mostraCard(this, \"" + Eval("idFabricacaoInsumoFerramenta") + "\", \"produto\");" %>'
                                                                OnMouseOut='<%# "escondeCard( \"" + Eval("idFabricacaoInsumoFerramenta") + "\", \"produto\");" %>'
                                                                CssClass="produto-detalhe-link"
                                                                ClientIDMode="Static"
                                                                data-idproduto='<%# Eval("idFabricacaoInsumoFerramenta") %>'
                                                                data-tabela="produto" />

                                                            <div id='<%# Eval("idFabricacaoInsumoFerramenta") + "_produto" %>' class="product-card" style="display: none;">
                                                                <!--conteudo via script -->
                                                            </div>
                                                        </div>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="39%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sUnidade" HeaderText="Unidade" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Quantidade">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtnQtd" class="form-control" Text='<%# Bind("nQtd") %>' runat="server"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Valor Unitário">
                                                    <ItemTemplate>
                                                        <div class="input-group">
                                                            <span class="input-group-addon">R$</span>
                                                            <asp:TextBox ID="txtnValor" class="form-control" Text='<%# Bind("nValorUnitario", "{0:N4}") %>' runat="server" Style="z-index: 0;"></asp:TextBox>
                                                        </div>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Total Produto">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotal" class="lblTotal" Text='<%# Eval("nTotal", "R$ {0:N4}") %>' runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" HorizontalAlign="Right" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete" CommandArgument='<%# Eval("idFabricacaoInsumoFerramenta") %>'>
                                                        <i class="fa-eraser fa"></i>
                                                        </asp:LinkButton>
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

                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>&nbsp;Recursos</b></h3>
                        </div>
                        <div class="panel-body" id="panelFabricacaoRecurso">
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="row">
                                        <div runat="server" id="div6" class="col-lg-9">
                                            <uc1:FiltroPesquisa runat="server" ID="fpFabricacaoRecurso"  sRegistraUnidade="S" sNomeFiltro="fpFabricacaoRecurso" STipoFabricado="S" />
                                        </div>
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Unidade</label>
                                                <asp:DropDownList ID="ddlsUnidadeRecurso" runat="server" class="form-control"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <asp:Button ID="btnIncluirRecurso" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdFabricacaoRecurso_Click" Style="margin-top: 24px;" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="form-group">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <asp:GridView ID="dtg_fabricacao_recurso" class="table table-striped table-bordered table-hover table-condensed" data-grid-type="Recursos"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idFabricacaoRecurso"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDeleting="dtg_fabricacao_recurso_RowDeleting">
                                            <Columns>

                                                <asp:BoundField DataField="idFabricacaoRecurso" HeaderText="ID">
                                                    <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Ordem">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="nOrdem" class="form-control" Text='<%# Bind("nOrdem") %>' runat="server"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="sCodigo" HeaderText="Código Recurso" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDescricao" HeaderText="Descrição Recurso" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemStyle Width="39%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sUnidade" HeaderText="Unidade" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Quantidade">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtnQtd" class="form-control" Text='<%# Bind("nQtd") %>' runat="server"></asp:TextBox>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Valor Unitário">
                                                    <ItemTemplate>
                                                        <div class="input-group">
                                                            <span class="input-group-addon">R$</span>
                                                            <asp:TextBox ID="txtnValor" class="form-control" Text='<%# Bind("nValorUnitario", "{0:N4}") %>' runat="server" Style="z-index: 0;"></asp:TextBox>
                                                        </div>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Total Produto">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblTotal" class="lblTotal" Text='<%# Eval("nTotal", "R$ {0:N4}") %>' runat="server"></asp:Label>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="10%" HorizontalAlign="Right" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete" CommandArgument='<%# Eval("idFabricacaoRecurso") %>'><i class="fa-eraser fa"></i></asp:LinkButton>
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

                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>&nbsp;Procedimento</b></h3>
                        </div>
                        <div class="panel-body" id="panelFabricacaoProcesso">
                            <div class="row">
                                <div class="col-lg-12" style="padding: 0px;">
                                    <div class="form-group">

                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <label>Ordem</label>
                                                <asp:TextBox ID="txtnOrdem" class="form-control" runat="server" TextMode="Number"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-10">
                                            <div class="form-group">
                                                <label>Descrição Procedimento</label>
                                                <asp:TextBox ID="txtsDescricao" class="form-control" runat="server" TextMode="MultiLine" Style="height: 35px"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <asp:Button ID="btnIncluirProcesso" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdFabricacaoProcesso_Click" Style="margin-top: 24px;" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:GridView ID="dtg_fabricacao_processo" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idLinha"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="dtg_fabricacao_processo_RowDataBound" OnRowDeleting="dtg_fabricacao_processo_RowDeleting">
                                        <Columns>

                                            <asp:BoundField DataField="idFabricacaoProcesso" HeaderText="ID" Visible="false">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="idLinha" HeaderText="ID" Visible="false">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Ordem">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="nOrdem" class="form-control" Text='<%# Bind("nOrdem") %>' runat="server"></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Recurso">
                                                <ItemTemplate>
                                                    <asp:DropDownList ID="ddlFabricacaoRecurso" runat="server" class="form-control"></asp:DropDownList>
                                                </ItemTemplate>
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Processo">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtsDescricao" class="form-control" Text='<%# Bind("sDescricao") %>' runat="server"></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle Width="81%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete" CommandArgument='<%# Eval("idFabricacaoProcesso") %>'><i class="fa-eraser fa"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
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

        <div role="tabpanel" class="tab-pane fade" id="sugestao" aria-labelledby="sugestao-tab">
            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Sugestão de Produtos</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-12">
                                    <uc1:FiltroPesquisa runat="server" ID="FiltroPesquisa1" />
                                    <div class="form-group">
                                        <div class="row" runat="server" id="div1">
                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <asp:Button ID="cmdIncluirSugestao" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirSugestao_Click" />
                                                </div>
                                            </div>
                                            <div class="col-lg-3">
                                                <br />
                                                <asp:Label ID="Label1" runat="server" Text="" Visible="false" CssClass="label-danger"></asp:Label>
                                            </div>
                                        </div>
                                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaSugestao" />


                                        <div class="row">
                                            <div class="col-lg-12">
                                                <asp:GridView ID="dtgSugestao" class="table table-striped table-bordered table-hover table-condensed"
                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgSugestao_RowDataBound" OnRowDeleting="dtgSugestao_RowDeleting">
                                                    <Columns>

                                                        <asp:BoundField DataField="sCodigo" HeaderText="Código">
                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>

                                                        <asp:BoundField DataField="sDscProduto" HeaderText="Descrição">
                                                            <ItemStyle Width="40%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <%--                                                        <asp:BoundField DataField="TipoProduto" HeaderText="Tipo do Produto">
                                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                        </asp:BoundField>--%>
                                                        <asp:BoundField DataField="sUnidade" HeaderText="Unidade" HeaderStyle-HorizontalAlign="Center">
                                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" HeaderStyle-HorizontalAlign="Right" DataFormatString="{0:N4}">
                                                            <ItemStyle Width="10%" HorizontalAlign="right" VerticalAlign="Middle" />
                                                        </asp:BoundField>
                                                        <asp:TemplateField HeaderText="">
                                                            <ItemTemplate>
                                                                <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
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
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="fornecedores" aria-labelledby="fornecedores-tab">
            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Fornecedores</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Fornecedor" />
                                </div>

                                <uc1:Pesquisa_Parceiros runat="server" ID="Pesquisa_Parceiros" />
                                <div class="form-group">

                                    <div class="col-lg-12" style="padding: 0;">
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Tipo</label>
                                                <asp:DropDownList ID="FN_ddlidTipo" class="form-control" runat="server">
                                                    <asp:ListItem Selected="True" Value="0" Text="Selecione o tipo"></asp:ListItem>
                                                    <asp:ListItem Value="1" Text="Principal"></asp:ListItem>
                                                    <asp:ListItem Value="2" Text="Secundário"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-lg-3">
                                        <div class="form-group">
                                            <label>Código do Produto no Fornecedor</label>
                                            <asp:TextBox ID="FN_txtCodigoFornecedor" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-6">
                                        <div class="form-group">
                                            <label>Descrição do Produto no Fornecedor</label>
                                            <asp:TextBox ID="FN_txtbsDscProdutoFornecedor" runat="server" class="form-control" ToolTip="Insira a descrição que é padrão do fornecedor ou o que aparece na Nota fiscal" MaxLength="400"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Valor unitário</label>
                                            <asp:TextBox ID="FN_ValorUnitario" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-1">
                                        <div class="form-group">
                                            <label>&nbsp;</label>
                                            <asp:Button ID="cmdIncluirFornecedor" class="btn btn-info form-control" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirFornecedor_Click" />
                                        </div>
                                    </div>

                                    <div class="col-lg-12">
                                        <asp:GridView ID="dtgFornecedores" class="table table-striped table-bordered table-hover table-condensed"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgFornecedores_RowDataBound" OnRowDeleting="dtgFornecedores_RowDeleting">
                                            <Columns>

                                                <asp:BoundField DataField="IdParceiro" HeaderText="Parceiro">
                                                    <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="stipoFornecedor" HeaderText="Tipo">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sRazaoSocial" HeaderText="Fornecedor">
                                                    <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sCodigoFornecedor" HeaderText="Código do Produto no Fornecedor">
                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="SDscFornecedorProduto" HeaderText="Descrição do Produto no Fornecedor">
                                                    <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nValorUnitario" HeaderText="Valor Unitário" DataFormatString="{0:N2}">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:TemplateField HeaderText="Excluir">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
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
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="documentacao" aria-labelledby="documentacao-tab">
            <asp:UpdatePanel ID="UpdatePanel5" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b runat="server" id="B1">Documentação</b></h3>
                        </div>

                        <div class="panel-body">
                            <div class="form-group row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Documentacao" />
                                </div>
                                <div>

                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Tipo</label>
                                            <asp:DropDownList ID="ddlDocumentacaoTipo" class="form-control" runat="server">
                                                <asp:ListItem Text="Selecione um Tipo" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Apresentação" Value="Apresentacao"></asp:ListItem>
                                                <asp:ListItem Text="Arquivo BIM" Value="BIM"></asp:ListItem>
                                                <asp:ListItem Text="DataSheet" Value="DataSheet"></asp:ListItem>
                                                <asp:ListItem Text="Imagens do Produto" Value="Imagem"></asp:ListItem>
                                                <asp:ListItem Text="Memorial Descritivo" Value="Memorial"></asp:ListItem>
                                                <asp:ListItem Text="Termo de Referência" Value="Termo"></asp:ListItem>
                                                <asp:ListItem Text="Vídeo" Value="Video"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label>Idioma</label>
                                            <asp:DropDownList ID="ddlDocumentacaoIdioma" class="form-control" runat="server">
                                                <asp:ListItem Text="Selecione um Idioma" Value="0"></asp:ListItem>
                                                <asp:ListItem Text="Espanhol" Value="Espanhol"></asp:ListItem>
                                                <asp:ListItem Text="Inglês" Value="Ingles"></asp:ListItem>
                                                <asp:ListItem Text="Português" Value="Portugues"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-lg-5">
                                        <div class="form-group">
                                            <label>Endereço (Link)</label>
                                            <asp:TextBox ID="txtEnderecoLink" class="form-control " runat="server" MaxLength="500"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-1">
                                        <div class="form-group">
                                            <label>Listar em Orçamento</label>
                                            <asp:CheckBox ID="cbsListarOrcamento" runat="server" class="form-control" Style="width: fit-content; padding: 4px; display: flex; margin-left: 55px;"></asp:CheckBox>
                                        </div>
                                    </div>

                                    <div class="col-lg-2" id="div_btnIncluir" runat="server">
                                        <div class="form-group">
                                            <br />
                                            <asp:Button ID="btnDocumentacaoIncluir" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="btnDocumentacaoIncluir_Click" />
                                        </div>
                                    </div>

                                    <br />
                                </div>
                                <div class="col-lg-12">
                                    <asp:GridView ID="dtgv_Documentacao" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" EnableViewState="true" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" DataKeyNames="idLinha" OnSorting="dtgv_Documentacao_Sorting" OnRowDataBound="dtgv_Documentacao_RowDataBound">
                                        <Columns>

                                            <asp:BoundField DataField="sDscTipo" HeaderText="Tipo">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Idioma">
                                                <ItemTemplate>
                                                    <asp:Label ID="lblIdioma" runat="server" Text='<%# Eval("sDscIdioma") %>' CssClass="idioma-label" />
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sDscEndereco" HeaderText="Endereço (Link)">
                                                <ItemStyle Width="75%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Listar Orçamento">
                                                <ItemTemplate>
                                                    <asp:Literal ID="litListarOrcamento" runat="server"></asp:Literal>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:HiddenField runat="server" ID="hddidLinha" Value='<%# Eval("idLinha") %>' />
                                                    <asp:LinkButton ID="lnkDescricao_Editar" runat="server" CssClass="btn btn-small" ToolTip="Editar" TabIndex="99" OnClick="lnkDescricao_Editar_Click"><i class="fa fa-pencil"></i></asp:LinkButton>
                                                    <asp:LinkButton ID="lnkDescricao_Excluir" runat="server" ToolTip="Excluir" CssClass="btn btn-small" TabIndex="100" OnClick="lnkDescricao_Excluir_Click"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="idLinha" HeaderText="ID">
                                                <HeaderStyle Width="3%" CssClass="hideColuna" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="hideColuna" />
                                            </asp:BoundField>
                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </div>
                        </div>
                        <asp:HiddenField runat="server" ID="hddGato" Value="0" />
                        <asp:HiddenField runat="server" ID="hddidDocumentacao" Value="" />
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="clientes" aria-labelledby="clientes-tab">
            <asp:UpdatePanel ID="UpdatePanel_Clientes" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Parceiros</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Clientes" />
                                </div>

                                <div class="col-lg-12" style="padding: 0;">
                                    <div class="col-lg-8 form-group">
                                        <label>Parceiros</label>
                                        <asp:ListBox runat="server" ID="lstParceiros" SelectionMode="Multiple"></asp:ListBox>
                                        <asp:HiddenField runat="server" ID="hdd_IncluirParceiros" Value="" />
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Código do Produto no Parceiro</label>
                                        <asp:TextBox ID="Clientes_txtCodigoProduto" class="form-control" runat="server" MaxLength="50"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-7">
                                    <div class="form-group">
                                        <label>Descrição do Produto no Parceiro</label>
                                        <asp:TextBox ID="Clientes_txtsDscProduto" runat="server" class="form-control" MaxLength="400"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>&nbsp;</label>
                                        <asp:Button ID="cmdIncluir_DadosCliente" class="btn btn-info form-control" runat="server" Text="Incluir" OnClick="cmdIncluir_DadosCliente_Click" />
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <asp:Button runat="server" ID="cmdExcluir_Selecionados" class="btn btn-danger" Text="Excluir Parceiros" OnClick="cmdExcluir_Selecionados_Click" />
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <asp:GridView ID="gvClientes" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDeleting="gvClientes_RowDeleting">
                                        <Columns>

                                            <asp:BoundField DataField="IdParceiro" HeaderText="Parceiro">
                                                <HeaderStyle CssClass="hideColuna" />
                                                <ItemStyle CssClass="hideColuna" />
                                            </asp:BoundField>

                                            <asp:TemplateField>
                                                <HeaderTemplate>
                                                    <asp:CheckBox runat="server" ID="cbExcluir_Todos" />
                                                </HeaderTemplate>
                                                <ItemTemplate>
                                                    <asp:CheckBox runat="server" ID="cbExcluir" ClientIDMode="Static" />
                                                </ItemTemplate>
                                                <HeaderStyle CssClass="center" />
                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sCNPJ_CPF" HeaderText="CPF / CNPJ">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sRazaoSocial" HeaderText="Parceiro">
                                                <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Código do Produto no Parceiro">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtCodigo" runat="server" ClientIDMode="Static" CssClass="form-control" Text='<%# Bind("sCodigoFornecedor") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Descrição do Produto no Parceiro">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="txtsDsc" runat="server" ClientIDMode="Static" CssClass="form-control" Text='<%# Bind("sDscFornecedorProduto") %>'></asp:TextBox>
                                                </ItemTemplate>
                                                <ItemStyle Width="35%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:TemplateField HeaderText="Excluir">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete"><i class="fa-eraser fa"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
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

        <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico">

            <div class="panel panel-default" runat="server" id="DIV_historico">
                <div class="panel-heading">

                    <h3 class="panel-title"><b>Histórico</b></h3>
                </div>
                <div class="panel-body">
                    <div class="form-stacked row">
                        <div class="col-lg-12 table-responsive">
                            <asp:UpdatePanel runat="server">
                                <ContentTemplate>
                                    <asp:GridView
                                        ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" OnRowDataBound="gv_Historico_RowDataBound">
                                        <Columns>
                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data de Atualização">
                                                <ItemStyle Width="10%" HorizontalAlign="center" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="sAcao" HeaderText="Tipo">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                            <asp:BoundField DataField="sDscAcao" HeaderText="Ação">
                                                <ItemStyle Width="70%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                        </Columns>
                                    </asp:GridView>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="lotes" aria-labelledby="lotes-tab">
            <asp:UpdatePanel ID="UpdatePanelLotes" runat="server">
                <ContentTemplate>
                    <div class="panel panel-default" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Lotes</b></h3>
                        </div>
                        <div class="panel-body">

                            <div class="row">
                                <div class="col-lg-12" style="margin-bottom: 15px;">
                                    <div class="well-lg label-info" runat="server" id="caixaTitulo">
                                        <div class="row">
                                            <div class="col-lg-8">
                                                <h4><span id="MainContent_lblTituloPagina">
                                                    <asp:Label ID="lblInfo" runat="server" Text="Informação"></asp:Label>

                                                </span></h4>
                                            </div>
                                            <%--<div class="col-lg-4 text-right">
                                                <span>
                                                    <asp:Label ID="lblInfo" class="label" runat="server" Text="TITULO_PAGINA"></asp:Label>
                                                </span>
                                            </div>--%>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12" runat="server" id="div5">
                                    <div class="form-group">
                                        <asp:GridView ID="dtgLotes" class="table table-striped table-bordered table-hover table-condensed"
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                            <Columns>
                                                <asp:BoundField DataField="sLote" HeaderText="Lote">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="nSerie" HeaderText="Número de Série">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <%--  <asp:BoundField DataField="idMovimentacao" HeaderText="Movimentação">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>--%>
                                                <asp:HyperLinkField DataNavigateUrlFields="idMovimentacao"
                                                    DataTextField="idMovimentacao" HeaderText="Movimentação"
                                                    DataNavigateUrlFormatString="../WMS/Movimentacao_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>
                                                <asp:BoundField DataField="sDscLocalArmazenamento" HeaderText="Local">
                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sTipoMov" HeaderText="Tipo Movimentação">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="dtAtualizacao" HeaderText="Última Atualização">
                                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>
                                                <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário">
                                                    <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
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

        <div role="tabpanel" class="tab-pane fade" id="EpiConsumiveis" aria-labelledby="EpiConsumiveis-tab">
            <%--  <asp:UpdatePanel runat="server" ID="updEpiConsumivel">
                <ContentTemplate>--%>

            <div class="panel panel-default" runat="server" id="div_Incluir_EPIs">
                <div class="panel-heading">
                    <h3 class="panel-title"><b runat="server" id="hEpi">EPIs</b></h3>
                </div>
                <div class="panel-body">

                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina_Incluir_EPI" />
                    </div>

                    <div class="col-lg-12" runat="server">
                        <div class="form-group row">

                            <div runat="server" id="divIncluir_EPI">
                                <div class="col-lg-12">
                                    <div class="form-stacled row">

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Código do EPI</label>
                                                <asp:TextBox ID="txtCodigo_EPI" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Descrição do EPI</label>
                                                <asp:TextBox ID="txtDesc_EPI" class="form-control" runat="server"></asp:TextBox>
                                            </div>
                                        </div>

                                        <%--                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <label>CA do EPI</label>
                                                <asp:TextBox ID="txtIncluirEPI_CA" class="form-control qtd" TextMode="Number" runat="server"></asp:TextBox>
                                            </div>
                                        </div>--%>

                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <label>Qtd do EPI</label>
                                                <asp:TextBox ID="txtIncluirEPI_Qtd" class="form-control qtd" runat="server" TextMode="Number" MaxLength="3" Text="1"></asp:TextBox>
                                            </div>
                                        </div>

                                        <%--          <div class="col-lg-2" style="padding: 0;">
                                            <label>Periodicidade</label>
                                            <div class="input-group" style="width: 100%;">
                                                <asp:TextBox ID="txtIncluirEPI_nTempo" class="form-control qtd" TextMode="Number" runat="server" MaxLength="3" Text="1" aria-label="..." Style="width: 40%;"></asp:TextBox>

                                                <asp:DropDownList ID="ddlIncluirEPI_sTipoPeriodo" runat="server" class="form-control input-group-addon" aria-label="..." Style="width: 60%; z-index: 1; background-color: white; text-align: start;">
                                                    <asp:ListItem Value="H">Hora</asp:ListItem>
                                                    <asp:ListItem Value="D">Dia</asp:ListItem>
                                                    <asp:ListItem Value="S">Semana</asp:ListItem>
                                                    <asp:ListItem Value="M">Mês</asp:ListItem>
                                                    <asp:ListItem Value="A">Ano</asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>--%>

                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <label>&nbsp;</label>
                                                <asp:Button ID="cmdEPI_Incluir" class="form-control btn-info" runat="server"
                                                    Text="Incluir EPI" OnClick="cmdEPI_Incluir_Click" />
                                            </div>
                                        </div>

                                    </div>
                                </div>
                            </div>

                            <div runat="server" id="div_dtgEPI" class="col-lg-12">
                                <asp:GridView ID="dtgRecursosEPI" runat="server"
                                    AutoGenerateColumns="False"
                                    DataKeyNames="idLinha"
                                    OnRowDataBound="dtgRecursosEPI_RowDataBound"
                                    OnRowDeleting="dtgRecursosEPI_RowDeleting"
                                    class="table table-striped table-bordered table-hover table-condensed"
                                    Width="100%" CellSpacing="1" CellPadding="1" GridLines="None"
                                    Font-Names="Tahoma" Font-Size="Small">
                                    <Columns>

                                        <asp:BoundField DataField="idLinha" HeaderText="idLinha" Visible="false">
                                            <ItemStyle CssClass="id" />
                                            <HeaderStyle CssClass="id" />
                                        </asp:BoundField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                            DataTextField="sCodigoEPI" HeaderText="Código do EPI"
                                            DataNavigateUrlFormatString="/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                        </asp:HyperLinkField>

                                        <asp:TemplateField HeaderText="Descrição do EPI">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscEPI") %>'
                                                    OnClientClick='<%# "openModal(\"" + Eval("idItem") + "\"); return false;" %>'
                                                    OnMouseOver='<%# "mostraCard(this, \"" + Eval("idItem") + "\", \"produto\");" %>'
                                                    OnMouseOut='<%# "escondeCard( \"" + Eval("idItem") + "\", \"produto\");" %>'
                                                    CssClass="epi-detalhe-link" />
                                            </ItemTemplate>
                                            <ItemStyle Width="35%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <%--                                        <asp:TemplateField HeaderText="CA">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtCA" runat="server" Text='<%# Bind("nCA") %>' TextMode="Number" class="form-control qtd"></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>--%>


                                        <asp:TemplateField HeaderText="Qtd.">
                                            <ItemTemplate>
                                                <asp:TextBox ID="txtnQuantidade" runat="server" Text='<%# Bind("nQuantidade") %>' class="form-control qtd" TextMode="Number" MaxLength="3"></asp:TextBox>
                                            </ItemTemplate>
                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                        <%--              <asp:TemplateField HeaderText="Periodicidade">
                                            <ItemTemplate>
                                                <div style="width: 100%; display: flex;">
                                                    <div style="width: 45%;">
                                                        <asp:TextBox ID="txtnQuantidadeTempo" runat="server" Text='<%# Bind("nQuantidadeTempo") %>' class="form-control qtd" TextMode="Number" MaxLength="3"></asp:TextBox>
                                                    </div>
                                                    <div style="width: 55%; margin-left: 5px;">
                                                        <asp:DropDownList ID="ddlsTipoPeriodo" runat="server" class="form-control">
                                                            <asp:ListItem Value="H">Hora</asp:ListItem>
                                                            <asp:ListItem Value="D">Dia</asp:ListItem>
                                                            <asp:ListItem Value="S">Semana</asp:ListItem>
                                                            <asp:ListItem Value="M">Mês</asp:ListItem>
                                                            <asp:ListItem Value="A">Ano</asp:ListItem>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                            <ItemStyle Width="15%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>--%>

                                        <asp:TemplateField HeaderText="Ação">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkEPI_Excluir" runat="server"
                                                    CommandName="Delete"
                                                    CommandArgument='<%# Eval("idLinha") %>'
                                                    CssClass="btn btn-danger btn-sm"
                                                    ToolTip="Excluir">
                                                    <i class="fa fa-eraser"></i>
                                                </asp:LinkButton>
                                                <%--OnClientClick="return confirm('Tem certeza que deseja excluir este EPI?');"--%>
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

            <div class="panel panel-default" runat="server">
                <div class="panel-heading">
                    <h3 class="panel-title"><b runat="server" id="B2">Consumíveis</b></h3>
                </div>
                <div class="panel-body">

                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina1" />
                    </div>

                    <div class="col-lg-12">

                        <div class="row">
                            <div class="col-lg-12">
                                <div class="row">
                                    <div runat="server" id="div7" class="col-lg-12">
                                        <uc1:FiltroPesquisa runat="server" ID="FiltroPesquisa2" snomefiltro="FiltroPesquisa2" sRegistraUnidade="S" />
                                        <asp:Button ID="cmdIncluirConsumivel" class="btn btn-info" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdConsumivelProduto_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="form-group">
                            <div class="row">
                                <div class="col-lg-12">
                                    <asp:GridView ID="GridView1" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None" DataKeyNames="idConsumivel,idLinha"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="dtgv_consumivel_produto_RowDataBound" OnRowDeleting="dtgv_consumivel_produto_RowDeleting">
                                        <Columns>

                                            <%--                                            <asp:BoundField DataField="idRegistro" HeaderText="ID">
                                                <ItemStyle Width="1%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>--%>

                                            <asp:HyperLinkField DataNavigateUrlFields="idConsumivel"
                                                DataTextField="sCodigo" HeaderText="Código do Produto"
                                                DataNavigateUrlFormatString="Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                <HeaderStyle Width="10%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="link" />
                                            </asp:HyperLinkField>

                                            <asp:TemplateField HeaderText="Descrição do Produto">
                                                <ItemTemplate>
                                                    <div>
                                                        <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDescricao") %>'
                                                            OnClientClick='<%# "openModal(\"" + Eval("idConsumivel") + "\"); return false;" %>'
                                                            OnMouseOver='<%# "mostraCard(this, \"" + Eval("idConsumivel") + "\", \"produto\");" %>'
                                                            OnMouseOut='<%# "escondeCard( \"" + Eval("idConsumivel") + "\", \"produto\");" %>'
                                                            CssClass="produto-detalhe-link"
                                                            ClientIDMode="Static"
                                                            data-idproduto='<%# Eval("idConsumivel") %>'
                                                            data-tabela="produto" />

                                                        <div id='<%# Eval("idConsumivel") + "_produto" %>' class="product-card" style="display: none;">
                                                            <!--conteudo via script -->
                                                        </div>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sUnidade" HeaderText="Unidade" HeaderStyle-HorizontalAlign="Center">
                                                <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="nQtd" HeaderText="Quantidade" HeaderStyle-HorizontalAlign="Center">
                                                <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkExcluir" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Delete" CommandArgument='<%# Eval("idLinha") %>'>
                                                        <i class="fa-eraser fa"></i>
                                                    </asp:LinkButton>
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

            <%--                </ContentTemplate>
            </asp:UpdatePanel>--%>
        </div>

        <div role="tabpanel" class="tab-pane fade" id="tabelas" aria-labelledby="tabelas-tab">
            <asp:UpdatePanel ID="UpdatePanel_Tabelas" runat="server" UpdateMode="Always">
                <ContentTemplate>
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Tabelas de Preços </b>
                                <label class="explicacaoTabelas explicacaoAba" data-swal-toast-explicacaoaba="#template-explicaAba"><i class="fa fa-question-circle"></i></label>
                                <label class="explicacaoTabelas avisoAba" data-swal-toast-avisoaba="#template-avisaAba"><i class="fa fa-exclamation-circle"></i></label>
                            </h3>
                        </div>
                        <div class="panel-body">

                            <template id="template-explicaAba">
                                <swal-icon type="question" color="gray"></swal-icon>
                                <swal-title>...</swal-title>
                            </template>

                            <template id="template-avisaAba">
                                <swal-icon type="warning" color="rgb(0, 175, 255)"></swal-icon>
                                <swal-title>Esta Aba não segue o Fluxo de Vínculos entre Tabelas, ou seja, Adicionar uma Tabela não irá adicionar suas Tabelas Filhas automaticamente e o mesmo vale para a alteração dos Valores.</swal-title>
                            </template>

                            <div class="row">

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Tabelas" />
                                </div>

                                <div runat="server" id="div_lstAdicionarTabela" class="col-lg-12" style="padding: 0;">
                                    <div class="col-lg-10 form-group">
                                        <label>Tabelas de Preço</label>
                                        <asp:ListBox runat="server" ID="lstAdicionarTabelas" class="form-control Caixa_Selecao" SelectionMode="Multiple"></asp:ListBox>
                                    </div>
                                    <div class="col-lg-2 form-group">
                                        <label>&nbsp;</label>
                                        <asp:LinkButton runat="server" ID="cmdAdicionarTabelas" class="btn btn-info form-control" Text="Adicionar Tabela" OnClick="cmdAdicionarTabelas_Click"></asp:LinkButton>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="panel panel-default" runat="server" id="pnCustoFornecedor">
                                        <div id="toggle_5" class="panel-heading painelTabelas toggle-panel">
                                            <h3 class="panel-title m-0"><b>Custo Fornecedor</b></h3>
                                            <span class="fa fa-chevron-up"></span>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">

                                                <div class="col-lg-12">
                                                    <div class="panel panel-default" runat="server" id="pnCustoFornecedor_Nacional">
                                                        <div id="toggle_5_1" class="panel-heading painelTabelas toggle-panel">
                                                            <h3 class="panel-title m-0"><b>Nacional</b></h3>
                                                            <span class="fa fa-chevron-up"></span>
                                                        </div>
                                                        <div class="panel-body">
                                                            <div class="col-lg-12">
                                                                <asp:GridView ID="gvTabelas_CustoFornecedor_Nacional" class="table table-striped table-bordered table-hover table-condensed"
                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idTabela" OnRowCommand="gvTabelas_RowCommand">
                                                                    <Columns>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                            DataTextField="idTabela" HeaderText="ID"
                                                                            DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                            DataTextField="sDscTabela" HeaderText="Titulo da Tabela"
                                                                            DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:BoundField>

                                                                        <asp:TemplateField HeaderText="Taxa de Câmbio">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtCambio" class="form-control nCambio naoAltera" ClientIDMode="Static" Text='<%# Eval("nCambio") %>' aria-describedby="cambio"></asp:TextBox>
                                                                                    <span class="input-group-addon" id="cambio"><%# string.Format("{0} / {1}", Eval("sSimbolo_MoedaOrigem"), Eval("sSimbolo_MoedaDestino")) %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Valor">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtPreco" class="form-control nPreco" ClientIDMode="Static" Text='<%# Eval("nPreco") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>' aria-describedby="valor"></asp:TextBox>
                                                                                    <span class="input-group-addon" id="valor"><%# Eval("sSimbolo_MoedaOrigem") %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Impostos (NCM)">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtImpostos" class="form-control nImpostos naoAltera" ClientIDMode="Static" Text='<%# Eval("nTaxaImpostos") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="11%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Total">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtTotal" class="form-control nTotal naoAltera" ClientIDMode="Static" Text='<%# Eval("nTotal") %>' aria-describedby="total"></asp:TextBox>
                                                                                    <span class="input-group-addon" id="total"><%# Eval("sSimbolo_MoedaDestino") %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Excluir">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton runat="server" ID="cmdExcluir" class="excluir" ClientIDMode="Static" CommandName="Excluir" CommandArgument='<%# Eval("idTabela") %>'><i class="fa fa-eraser"></i></asp:LinkButton>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12">
                                                    <div class="panel panel-default" runat="server" id="pnCustoFornecedor_Internacional">
                                                        <div id="toggle_5_2" class="panel-heading painelTabelas toggle-panel">
                                                            <h3 class="panel-title m-0"><b>Internacional</b></h3>
                                                            <span class="fa fa-chevron-up"></span>
                                                        </div>
                                                        <div class="panel-body">
                                                            <div class="col-lg-12">
                                                                <asp:GridView ID="gvTabelas_CustoFornecedor_Internacional" class="table table-striped table-bordered table-hover table-condensed"
                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idTabela" OnRowCommand="gvTabelas_RowCommand">
                                                                    <Columns>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                            DataTextField="idTabela" HeaderText="ID"
                                                                            DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                            DataTextField="sDscTabela" HeaderText="Titulo da Tabela"
                                                                            DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:BoundField>

                                                                        <asp:TemplateField HeaderText="Taxa de Câmbio">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtCambio" class="form-control nCambio naoAltera" ClientIDMode="Static" Text='<%# Eval("nCambio") %>' aria-describedby="cambio"></asp:TextBox>
                                                                                    <span class="input-group-addon" id="cambio"><%# string.Format("{0} / {1}", Eval("sSimbolo_MoedaOrigem"), Eval("sSimbolo_MoedaDestino")) %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="11%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Valor">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtPreco" class="form-control nPreco" ClientIDMode="Static" Text='<%# Eval("nPreco") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>' aria-describedby="valor"></asp:TextBox>
                                                                                    <span class="input-group-addon" id="valor"><%# Eval("sSimbolo_MoedaOrigem") %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="11%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Taxa de Envio">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtEnvio" class="form-control nEnvio" ClientIDMode="Static" Text='<%# Eval("nTaxaEnvio") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Taxa Local">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtLocal" class="form-control nLocal" ClientIDMode="Static" Text='<%# Eval("nTaxaLocal") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Total">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtTotal" class="form-control nTotal naoAltera" ClientIDMode="Static" Text='<%# Eval("nTotal") %>' aria-describedby="total"></asp:TextBox>
                                                                                    <span class="input-group-addon" id="total"><%# Eval("sSimbolo_MoedaDestino") %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="11%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Excluir">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton runat="server" ID="cmdExcluir" class="excluir" ClientIDMode="Static" CommandName="Excluir" CommandArgument='<%# Eval("idTabela") %>'><i class="fa fa-eraser"></i></asp:LinkButton>
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
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="panel panel-default" runat="server" id="pnCustoTT">
                                        <div id="toggle_2" class="panel-heading painelTabelas toggle-panel">
                                            <h3 class="panel-title m-0"><b>Custo TT</b></h3>
                                            <span class="fa fa-chevron-up"></span>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">

                                                <div class="col-lg-12">
                                                    <div class="panel panel-default" runat="server" id="pnCustoTT_Nacional">
                                                        <div id="toggle_2_1" class="panel-heading painelTabelas toggle-panel">
                                                            <h3 class="panel-title m-0"><b>Nacional</b></h3>
                                                            <span class="fa fa-chevron-up"></span>
                                                        </div>
                                                        <div class="panel-body">
                                                            <div class="col-lg-12">
                                                                <asp:GridView ID="gvTabelas_CustoTT_Nacional" class="table table-striped table-bordered table-hover table-condensed"
                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idTabela" OnRowCommand="gvTabelas_RowCommand">
                                                                    <Columns>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                            DataTextField="idTabela" HeaderText="ID"
                                                                            DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                            DataTextField="sDscTabela" HeaderText="Titulo da Tabela"
                                                                            DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:BoundField>

                                                                        <asp:TemplateField HeaderText="Taxa de Câmbio">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtCambio" class="form-control nCambio naoAltera" ClientIDMode="Static" Text='<%# Eval("nCambio") %>' aria-describedby="cambio"></asp:TextBox>
                                                                                    <span class="input-group-addon" id="cambio"><%# string.Format("{0} / {1}", Eval("sSimbolo_MoedaOrigem"), Eval("sSimbolo_MoedaDestino")) %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Valor">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtPreco" class="form-control nPreco" ClientIDMode="Static" Text='<%# Eval("nPreco") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>' aria-describedby="valor"></asp:TextBox>
                                                                                    <span class="input-group-addon" id="valor"><%# Eval("sSimbolo_MoedaOrigem") %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="11%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Margem">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtMargem" class="form-control nMargem" ClientIDMode="Static" Text='<%# Eval("nMargem") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Impostos (NCM)">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtImpostos" class="form-control nImpostos naoAltera" ClientIDMode="Static" Text='<%# Eval("nTaxaImpostos") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Total">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtTotal" class="form-control nTotal naoAltera" ClientIDMode="Static" Text='<%# Eval("nTotal") %>' aria-describedby="total"></asp:TextBox>
                                                                                    <span class="input-group-addon" id="total"><%# Eval("sSimbolo_MoedaDestino") %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="11%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Excluir">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton runat="server" ID="cmdExcluir" class="excluir" ClientIDMode="Static" CommandName="Excluir" CommandArgument='<%# Eval("idTabela") %>'><i class="fa fa-eraser"></i></asp:LinkButton>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12">
                                                    <div class="panel panel-default" runat="server" id="pnCustoTT_Internacional">
                                                        <div id="toggle_2_2" class="panel-heading painelTabelas toggle-panel">
                                                            <h3 class="panel-title m-0"><b>Internacional</b></h3>
                                                            <span class="fa fa-chevron-up"></span>
                                                        </div>
                                                        <div class="panel-body">
                                                            <div class="col-lg-12">
                                                                <asp:GridView ID="gvTabelas_CustoTT_Internacional" class="table table-striped table-bordered table-hover table-condensed"
                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idTabela" OnRowCommand="gvTabelas_RowCommand">
                                                                    <Columns>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                            DataTextField="idTabela" HeaderText="ID"
                                                                            DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                            DataTextField="sDscTabela" HeaderText="Titulo da Tabela"
                                                                            DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:BoundField>

                                                                        <asp:TemplateField HeaderText="Taxa de Câmbio">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtCambio" class="form-control nCambio naoAltera" ClientIDMode="Static" Text='<%# Eval("nCambio") %>'></asp:TextBox>
                                                                                    <span class="input-group-addon" id="cambio"><%# string.Format("{0} / {1}", Eval("sSimbolo_MoedaOrigem"), Eval("sSimbolo_MoedaDestino")) %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Valor">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtPreco" class="form-control nPreco" ClientIDMode="Static" Text='<%# Eval("nPreco") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                                    <span class="input-group-addon" id="valor"><%# Eval("sSimbolo_MoedaOrigem") %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="11%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Margem">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtMargem" class="form-control nMargem" ClientIDMode="Static" Text='<%# Eval("nMargem") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Taxa de Envio">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtEnvio" class="form-control nEnvio" ClientIDMode="Static" Text='<%# Eval("nTaxaEnvio") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Taxa Local">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtLocal" class="form-control nLocal" ClientIDMode="Static" Text='<%# Eval("nTaxaLocal") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Impostos (NCM)">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtImpostos" class="form-control nImpostos naoAltera" ClientIDMode="Static" Text='<%# Eval("nTaxaImpostos") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Total">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtTotal" class="form-control nTotal naoAltera" ClientIDMode="Static" Text='<%# Eval("nTotal") %>'></asp:TextBox>
                                                                                    <span class="input-group-addon" id="total"><%# Eval("sSimbolo_MoedaDestino") %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="11%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Excluir">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton runat="server" ID="cmdExcluir" class="excluir" ClientIDMode="Static" CommandName="Excluir" CommandArgument='<%# Eval("idTabela") %>'><i class="fa fa-eraser"></i></asp:LinkButton>
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
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="panel panel-default" runat="server" id="pnIndustrializacaoTT">
                                        <div id="toggle_4" class="panel-heading painelTabelas toggle-panel">
                                            <h3 class="panel-title m-0"><b>Industrialização TT</b></h3>
                                            <span class="fa fa-chevron-up"></span>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">

                                                <div class="col-lg-12">
                                                    <div class="panel panel-default" runat="server" id="pnIndustrializacaoTT_Nacional">
                                                        <div id="toggle_4_1" class="panel-heading painelTabelas toggle-panel">
                                                            <h3 class="panel-title m-0"><b>Nacional</b></h3>
                                                            <span class="fa fa-chevron-up"></span>
                                                        </div>
                                                        <div class="panel-body">
                                                            <div class="col-lg-12">
                                                                <asp:GridView ID="gvTabelas_IndustrializaçãoTT_Nacional" class="table table-striped table-bordered table-hover table-condensed"
                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idTabela" OnRowCommand="gvTabelas_RowCommand">
                                                                    <Columns>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                            DataTextField="idTabela" HeaderText="ID"
                                                                            DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                            DataTextField="sDscTabela" HeaderText="Titulo da Tabela"
                                                                            DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                            <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:BoundField>

                                                                        <asp:TemplateField HeaderText="Taxa de Câmbio">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtCambio" class="form-control nCambio naoAltera" ClientIDMode="Static" Text='<%# Eval("nCambio") %>'></asp:TextBox>
                                                                                    <span class="input-group-addon" id="cambio"><%# string.Format("{0} / {1}", Eval("sSimbolo_MoedaOrigem"), Eval("sSimbolo_MoedaDestino")) %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="11%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Valor">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtPreco" class="form-control nPreco" ClientIDMode="Static" Text='<%# Eval("nPreco") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                                    <span class="input-group-addon" id="valor"><%# Eval("sSimbolo_MoedaOrigem") %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="11%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Fator">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtFator" class="form-control nFator" ClientIDMode="Static" Text='<%# Eval("nFator") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Impostos (NCM)">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtImpostos" class="form-control nImpostos naoAltera" ClientIDMode="Static" Text='<%# Eval("nTaxaImpostos") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Total">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtTotal" class="form-control nTotal naoAltera" ClientIDMode="Static" Text='<%# Eval("nTotal") %>'></asp:TextBox>
                                                                                    <span class="input-group-addon" id="total"><%# Eval("sSimbolo_MoedaDestino") %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="11%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Excluir">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton runat="server" ID="cmdExcluir" class="excluir" ClientIDMode="Static" CommandName="Excluir" CommandArgument='<%# Eval("idTabela") %>'><i class="fa fa-eraser"></i></asp:LinkButton>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div class="col-lg-12">
                                                    <div class="panel panel-default" runat="server" id="pnIndustrializacaoTT_Internacional">
                                                        <div id="toggle_4_2" class="panel-heading painelTabelas toggle-panel">
                                                            <h3 class="panel-title m-0"><b>Internacional</b></h3>
                                                            <span class="fa fa-chevron-up"></span>
                                                        </div>
                                                        <div class="panel-body">
                                                            <div class="col-lg-12">
                                                                <asp:GridView ID="gvTabelas_IndustrializaçãoTT_Internacional" class="table table-striped table-bordered table-hover table-condensed"
                                                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idTabela" OnRowCommand="gvTabelas_RowCommand">
                                                                    <Columns>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                            DataTextField="idTabela" HeaderText="ID"
                                                                            DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                            DataTextField="sDscTabela" HeaderText="Titulo da Tabela"
                                                                            DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:HyperLinkField>

                                                                        <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:BoundField>

                                                                        <asp:TemplateField HeaderText="Taxa de Câmbio">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtCambio" class="form-control nCambio naoAltera" ClientIDMode="Static" Text='<%# Eval("nCambio") %>'></asp:TextBox>
                                                                                    <span class="input-group-addon" id="cambio"><%# string.Format("{0} / {1}", Eval("sSimbolo_MoedaOrigem"), Eval("sSimbolo_MoedaDestino")) %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Valor">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtPreco" class="form-control nPreco" ClientIDMode="Static" Text='<%# Eval("nPreco") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                                    <span class="input-group-addon" id="valor"><%# Eval("sSimbolo_MoedaOrigem") %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Taxa de Envio">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtEnvio" class="form-control nEnvio" ClientIDMode="Static" Text='<%# Eval("nTaxaEnvio") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Taxa Local">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtLocal" class="form-control nLocal" ClientIDMode="Static" Text='<%# Eval("nTaxaLocal") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Fator">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtFator" class="form-control nFator" ClientIDMode="Static" Text='<%# Eval("nFator") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Impostos (NCM)">
                                                                            <ItemTemplate>
                                                                                <asp:TextBox runat="server" ID="txtImpostos" class="form-control nImpostos naoAltera" ClientIDMode="Static" Text='<%# Eval("nTaxaImpostos") %>'></asp:TextBox>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Total">
                                                                            <ItemTemplate>
                                                                                <div class="input-group">
                                                                                    <asp:TextBox runat="server" ID="txtTotal" class="form-control nTotal naoAltera" ClientIDMode="Static" Text='<%# Eval("nTotal") %>'></asp:TextBox>
                                                                                    <span class="input-group-addon" id="total"><%# Eval("sSimbolo_MoedaDestino") %></span>
                                                                                </div>
                                                                            </ItemTemplate>
                                                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                                        </asp:TemplateField>

                                                                        <asp:TemplateField HeaderText="Excluir">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton runat="server" ID="cmdExcluir" class="excluir" ClientIDMode="Static" CommandName="Excluir" CommandArgument='<%# Eval("idTabela") %>'><i class="fa fa-eraser"></i></asp:LinkButton>
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
                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <div class="panel panel-default" runat="server" id="pnVendasPVP">
                                        <div id="toggle_3" class="panel-heading painelTabelas toggle-panel">
                                            <h3 class="panel-title m-0"><b>Vendas PVP</b></h3>
                                            <span class="fa fa-chevron-up"></span>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">

                                                <div class="col-lg-12">
                                                    <asp:GridView ID="gvTabelas_VendasPVP" class="table table-striped table-bordered table-hover table-condensed"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idTabela" OnRowCommand="gvTabelas_RowCommand">
                                                        <Columns>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                DataTextField="idTabela" HeaderText="ID"
                                                                DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                DataTextField="sDscTabela" HeaderText="Titulo da Tabela"
                                                                DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Taxa de Câmbio">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtCambio" class="form-control nCambio naoAltera" ClientIDMode="Static" Text='<%# Eval("nCambio") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="cambio"><%# string.Format("{0} / {1}", Eval("sSimbolo_MoedaOrigem"), Eval("sSimbolo_MoedaDestino")) %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Valor">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtPreco" class="form-control nPreco" ClientIDMode="Static" Text='<%# Eval("nPreco") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="valor"><%# Eval("sSimbolo_MoedaOrigem") %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Fator">
                                                                <ItemTemplate>
                                                                    <asp:TextBox runat="server" ID="txtFator" class="form-control nFator" ClientIDMode="Static" Text='<%# Eval("nFator") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Total">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtTotal" class="form-control nTotal naoAltera" ClientIDMode="Static" Text='<%# Eval("nTotal") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="total"><%# Eval("sSimbolo_MoedaDestino") %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Excluir">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton runat="server" ID="cmdExcluir" class="excluir" ClientIDMode="Static" CommandName="Excluir" CommandArgument='<%# Eval("idTabela") %>'><i class="fa fa-eraser"></i></asp:LinkButton>
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

                                <div class="col-lg-12">
                                    <div class="panel panel-default" runat="server" id="pnVendasCustom">
                                        <div id="toggle_1" class="panel-heading painelTabelas toggle-panel">
                                            <h3 class="panel-title m-0"><b>Vendas Customizadas</b></h3>
                                            <span class="fa fa-chevron-up"></span>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">

                                                <div class="col-lg-12">
                                                    <asp:GridView ID="gvTabelas_VendasCustomizadas" class="table table-striped table-bordered table-hover table-condensed"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idTabela" OnRowDataBound="gvTabelas_VendasCustomizadas_RowDataBound" OnRowCommand="gvTabelas_RowCommand">
                                                        <Columns>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                DataTextField="idTabela" HeaderText="ID"
                                                                DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                DataTextField="sDscTabela" HeaderText="Titulo da Tabela"
                                                                DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Taxa de Câmbio">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtCambio" class="form-control nCambio naoAltera" ClientIDMode="Static" Text='<%# Eval("nCambio") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="cambio"><%# string.Format("{0} / {1}", Eval("sSimbolo_MoedaOrigem"), Eval("sSimbolo_MoedaDestino")) %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Valor">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtPreco" class="form-control nPreco" ClientIDMode="Static" Text='<%# Eval("nPreco") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="valor"><%# Eval("sSimbolo_MoedaOrigem") %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Desconto">
                                                                <ItemTemplate>
                                                                    <asp:TextBox runat="server" ID="txtDesconto" class="form-control nDesconto" ClientIDMode="Static" Text='<%# Eval("nFator") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Total">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtTotal" class="form-control nTotal naoAltera" ClientIDMode="Static" Text='<%# Eval("nTotal") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="total"><%# Eval("sSimbolo_MoedaDestino") %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Liberado">
                                                                <ItemTemplate>
                                                                    <asp:CheckBox runat="server" ID="cbLiberado" ClientIDMode="Static" class="btn btn-small" Style="cursor: default;"></asp:CheckBox>
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

                                <div class="col-lg-12">
                                    <div class="panel panel-default" runat="server" id="pnCustoEmpreitada">
                                        <div id="toggle_10" class="panel-heading painelTabelas toggle-panel">
                                            <h3 class="panel-title m-0"><b>Custo Empreitada</b></h3>
                                            <span class="fa fa-chevron-up"></span>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">

                                                <div class="col-lg-12">
                                                    <asp:GridView ID="gvTabelas_CustoEmpreitada" class="table table-striped table-bordered table-hover table-condensed"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idTabela" OnRowCommand="gvTabelas_RowCommand">
                                                        <Columns>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                DataTextField="idTabela" HeaderText="ID"
                                                                DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                DataTextField="sDscTabela" HeaderText="Titulo da Tabela"
                                                                DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Taxa de Câmbio">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtCambio" class="form-control nCambio naoAltera" ClientIDMode="Static" Text='<%# Eval("nCambio") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="cambio"><%# string.Format("{0} / {1}", Eval("sSimbolo_MoedaOrigem"), Eval("sSimbolo_MoedaDestino")) %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Valor">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtPreco" class="form-control nPreco" ClientIDMode="Static" Text='<%# Eval("nPreco") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="valor"><%# Eval("sSimbolo_MoedaOrigem") %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Fator">
                                                                <ItemTemplate>
                                                                    <asp:TextBox runat="server" ID="txtFator" class="form-control nFator" ClientIDMode="Static" Text='<%# Eval("nFator") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Total">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtTotal" class="form-control nTotal naoAltera" ClientIDMode="Static" Text='<%# Eval("nTotal") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="total"><%# Eval("sSimbolo_MoedaDestino") %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Excluir">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton runat="server" ID="cmdExcluir" class="excluir" ClientIDMode="Static" CommandName="Excluir" CommandArgument='<%# Eval("idTabela") %>'><i class="fa fa-eraser"></i></asp:LinkButton>
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

                                <div class="col-lg-12">
                                    <div class="panel panel-default" runat="server" id="pnVendasLPU">
                                        <div id="toggle_11" class="panel-heading painelTabelas toggle-panel">
                                            <h3 class="panel-title m-0"><b>Vendas LPU</b></h3>
                                            <span class="fa fa-chevron-up"></span>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">

                                                <div class="col-lg-12">
                                                    <asp:GridView ID="gvTabelas_VendasLPU" class="table table-striped table-bordered table-hover table-condensed"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idTabela" OnRowCommand="gvTabelas_RowCommand">
                                                        <Columns>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                DataTextField="idTabela" HeaderText="ID"
                                                                DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                DataTextField="sDscTabela" HeaderText="Titulo da Tabela"
                                                                DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Taxa de Câmbio">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtCambio" class="form-control nCambio naoAltera" ClientIDMode="Static" Text='<%# Eval("nCambio") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="cambio"><%# string.Format("{0} / {1}", Eval("sSimbolo_MoedaOrigem"), Eval("sSimbolo_MoedaDestino")) %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Valor">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtPreco" class="form-control nPreco" ClientIDMode="Static" Text='<%# Eval("nPreco") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="valor"><%# Eval("sSimbolo_MoedaOrigem") %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Desconto">
                                                                <ItemTemplate>
                                                                    <asp:TextBox runat="server" ID="txtDesconto" class="form-control nDesconto" ClientIDMode="Static" Text='<%# Eval("nFator") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Total">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtTotal" class="form-control nTotal naoAltera" ClientIDMode="Static" Text='<%# Eval("nTotal") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="total"><%# Eval("sSimbolo_MoedaDestino") %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Excluir">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton runat="server" ID="cmdExcluir" class="excluir" ClientIDMode="Static" CommandName="Excluir" CommandArgument='<%# Eval("idTabela") %>'><i class="fa fa-eraser"></i></asp:LinkButton>
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

                                <div class="col-lg-12">
                                    <div class="panel panel-default" runat="server" id="pnCustoRecursos">
                                        <div id="toggle_8" class="panel-heading painelTabelas toggle-panel">
                                            <h3 class="panel-title m-0"><b>Custo Recursos</b></h3>
                                            <span class="fa fa-chevron-up"></span>
                                        </div>
                                        <div class="panel-body">
                                            <div class="row">

                                                <div class="col-lg-12">
                                                    <asp:GridView ID="gvTabelas_CustoRecursos" class="table table-striped table-bordered table-hover table-condensed"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" DataKeyNames="idTabela" OnRowCommand="gvTabelas_RowCommand">
                                                        <Columns>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                DataTextField="idTabela" HeaderText="ID"
                                                                DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                                                DataTextField="sDscTabela" HeaderText="Titulo da Tabela"
                                                                DataNavigateUrlFormatString="../Comercial/Tabelas_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                                                <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Taxa de Câmbio">
                                                                <ItemTemplate>
                                                                    <div class="input-group">
                                                                        <asp:TextBox runat="server" ID="txtCambio" class="form-control nCambio naoAltera" ClientIDMode="Static" Text='<%# Eval("nCambio") %>'></asp:TextBox>
                                                                        <span class="input-group-addon" id="cambio"><%# string.Format("{0} / {1}", Eval("sSimbolo_MoedaOrigem"), Eval("sSimbolo_MoedaDestino")) %></span>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Sudeste">
                                                                <ItemTemplate>
                                                                    <div style="display: flex;">
                                                                        <asp:TextBox runat="server" ID="txtPreco_Zona_SD" class="form-control nPreco_Zona_SD" ClientIDMode="Static" Text='<%# Eval("nPreco_Zona_SD") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                        <asp:LinkButton runat="server" ID="cmdAplicaZonas" Style="align-self: center; font-size: 18px; margin-left: 3px;" class="aplicaValor_Zonas" data-toggle="tooltip" title="Aplicar valor em todas as Zonas"><i class="fa fa-arrow-right"></i></asp:LinkButton>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Nordeste">
                                                                <ItemTemplate>
                                                                    <asp:TextBox runat="server" ID="txtPreco_Zona_ND" class="form-control nPreco_Zona_ND" ClientIDMode="Static" Text='<%# Eval("nPreco_Zona_ND") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Norte">
                                                                <ItemTemplate>
                                                                    <asp:TextBox runat="server" ID="txtPreco_Zona_N" class="form-control nPreco_Zona_N" ClientIDMode="Static" Text='<%# Eval("nPreco_Zona_N") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Centro-Oeste">
                                                                <ItemTemplate>
                                                                    <asp:TextBox runat="server" ID="txtPreco_Zona_CO" class="form-control nPreco_Zona_CO" ClientIDMode="Static" Text='<%# Eval("nPreco_Zona_CO") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Sul">
                                                                <ItemTemplate>
                                                                    <asp:TextBox runat="server" ID="txtPreco_Zona_S" class="form-control nPreco_Zona_S" ClientIDMode="Static" Text='<%# Eval("nPreco_Zona_S") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Margem">
                                                                <ItemTemplate>
                                                                    <asp:TextBox runat="server" ID="txtFator" class="form-control nFator" ClientIDMode="Static" Text='<%# Eval("nFator") %>' ReadOnly='<%# !TT.FrameWork.Funcoes.ValidaPermissao(Permissao.Produtos.EditarAbaTabelas) %>' data-tipo='<%# Eval("idTipoTabela") %>' data-moeda='<%# Eval("idMoedaOrigem") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Excluir">
                                                                <ItemTemplate>
                                                                    <asp:LinkButton runat="server" ID="cmdExcluir" class="excluir" ClientIDMode="Static" CommandName="Excluir" CommandArgument='<%# Eval("idTabela") %>'><i class="fa fa-eraser"></i></asp:LinkButton>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
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
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>

    </div>

    <div id="dialog_Aceitar" class="modal" title="Status Edição">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <asp:Label ID="Label3" runat="server" ClientIDMode="Static" Text=""></asp:Label>
        </p>
    </div>

    <uc1:MensagemPagina runat="server" ID="MensagemItem" />
    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

    <fieldset class="form-stacked actions">
        <asp:Button ID="cmdSalvar" ClientIDMode="Static" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" />
        &nbsp;
        <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Cancelar" id="field-cancel" title="Voltar" onclick="history.go(-1)">&nbsp;
    </fieldset>

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

    <div id="hdd">
        <asp:HiddenField ID="hddidProduto" runat="server" />
        <asp:HiddenField ID="hddidTipoFiltroPesquisa" runat="server" Value="23" />
        <asp:HiddenField ID="hddsCadeado" runat="server" Value="N" />
        <asp:HiddenField ID="hddsPermissaoCadeado" runat="server" Value="0" />
        <asp:HiddenField ID="hddnQtdSeries" runat="server" Value="0" />
        <asp:HiddenField ID="hddsTabelas_Abertas" runat="server" Value="|" />
        <asp:HiddenField ID="hddEPIs" runat="server" Value="[]" />
        <asp:HiddenField ID="hddIncluir_idEPI" runat="server" Value="0" />
        <asp:HiddenField ID="hddsCodigoServicoPai" runat="server" Value="0" />
        <asp:HiddenField ID="hddsCodigoServicoFilho" runat="server" Value="0" />
        <asp:HiddenField ID="hddsTipoFabricacaoProduto" runat="server" Value="" />
        <asp:HiddenField ID="hddnValorUnitario" runat="server" Value="0" />
        <asp:HiddenField ID="hddidTabela" runat="server" Value="" />
    </div>

    <script>
        $(document).ready(function () {
            $('#documentacao-tab').on('click', function () {
                $('#<%= hddGato.ClientID %>').val('1');
                $('#<%= btnDocumentacaoIncluir.ClientID %>').click();
            });


        });

        document.addEventListener('DOMContentLoaded', function () {
            document.body.addEventListener('change', function (event) {
                if (event.target && event.target.id === 'lock') {
                    var isChecked = event.target.checked;

                    var mensagem;
                    if (isChecked) {
                        mensagem = "Deseja habilitar a edição?"
                    } else {
                        mensagem = "Deseja desabilitar a edição?"
                    }

                    alterarEdicao(mensagem, event);
                }
            });

        });

        window.addEventListener('load', function () {

            var permissao = document.getElementById('<%= hddsPermissaoCadeado.ClientID %>').value;
            var lockCheckbox = document.getElementById('lock');
            var hddidProduto = document.getElementById('<%= hddidProduto.ClientID %>').value;

            if (lockCheckbox) {

                var hddsCadeado = document.getElementById('<%= hddsCadeado.ClientID %>').value;
                var isChecked = (hddsCadeado === "N");
                var btnSalvar = document.getElementById('<%= cmdSalvar.ClientID %>');

                if (hddidProduto === '0') {
                    isChecked = true;

                }

                if (isChecked) {
                    lockCheckbox.checked = isChecked;
                    btnSalvar.style.display = 'inline';
                }
                else {
                    btnSalvar.style.display = 'none';
                    lockCheckbox.checked = isChecked;
                    var shackle = document.querySelector('.shackle');
                    shackle.style.transform = 'rotateY(0deg)';

                }
            }

        });

        $v192(function () {
            $v192("#dialog_Aceitar").dialog({
                resizable: false,
                height: "auto",
                width: 400,
                modal: true,
                autoOpen: false
            });
        });

        function alterarEdicao(mensagem, event) {
            document.getElementById('Label3').innerText = mensagem;

            var btnSalvar = document.getElementById('<%= cmdSalvar.ClientID %>');
            var hddidProduto = document.getElementById('<%= hddidProduto.ClientID %>');
            var lockCheckbox = document.getElementById('lock');
            var isChecked = event.target.checked;

            $v192('#dialog_Aceitar').dialog('option', 'buttons', {
                "Sim": function () {
                    var idProduto = hddidProduto.value;

                    $.ajax({
                        url: '/app/Paginas/Manutencao/Produtos_Detalhe.aspx/SalvaEstadoCadeado',
                        data: JSON.stringify({
                            isLocked: isChecked,
                            idProduto: idProduto
                        }),
                        contentType: 'application/json; charset=utf-8',
                        type: 'POST',
                        dataType: 'json',
                        success: function (data) {
                            var shackle = document.querySelector('.shackle');
                            if (isChecked) {
                                btnSalvar.style.display = 'inline';
                                shackle.style.transform = 'rotateY(150deg) translateX(3px)';
                                shackle.style.transformOrigin = 'right';
                            } else {
                                btnSalvar.style.display = 'none';
                                shackle.style.transform = 'rotateY(0deg)';
                            }
                            window.location.href = location.href;
                        },
                        error: function (response) {
                            alert(response.responseText);
                        },
                        failure: function (response) {
                            alert(response.responseText);
                        }
                    });

                    $v192(this).dialog("close");
                },
                "Não": function () {
                    lockCheckbox.checked = !isChecked;

                    var shackle = document.querySelector('.shackle');
                    if (!isChecked) {
                        btnSalvar.style.display = 'inline';
                        shackle.style.transform = 'rotateY(150deg) translateX(3px)';
                        shackle.style.transformOrigin = 'right';
                    } else {
                        btnSalvar.style.display = 'none';
                        shackle.style.transform = 'rotateY(0deg)';
                    }

                    $v192(this).dialog("close");
                }
            });

            $v192('#dialog_Aceitar').dialog('open');


        }

        function toggleGarantiaDiv(isChecked) {
            const divMeses = document.getElementById('<%= div_meses.ClientID %>');
            if (divMeses) {
                divMeses.style.display = isChecked ? 'block' : 'none';
                //console.log(isChecked ? "S - Exibindo div_meses" : "N - Ocultando div_meses");
            } else {
                console.error('div_meses não encontrada');
            }
        }

        function initSwitchAtivoGarantia() {
            const switchAtivo = document.getElementById('SwitchAtivoGarantia_idSwitch');
            if (switchAtivo) {
                switchAtivo.removeEventListener('change', onSwitchAtivoChange);
                switchAtivo.addEventListener('change', onSwitchAtivoChange);

                toggleGarantiaDiv(switchAtivo.checked);
            } else {
                console.error('SwitchAtivoGarantia não encontrado');
            }
        }

        function onSwitchAtivoChange() {
            toggleGarantiaDiv(this.checked);
        }

        function validarInteiro(event) {
            var keyCode = event.keyCode || event.which;
            var tecla = String.fromCharCode(keyCode);

            if (!/^\d$/.test(tecla) && keyCode !== 8) {
                event.preventDefault();
                return false;
            }
            return true;
        }

        function formatarValor(input) {
            var valor = input.value;

            if (valor === "") {
                input.value = "0";
            }
        }

        // Função para ativar a aba "Características do Produto"
        function ativarAbaCaracteristicas() {
            $('#aba_caracteristicasProduto-tab').tab('show');
            document.getElementById('<%= nTempoGarantia.ClientID %>').focus();
        }

        // Executa na carga inicial
        document.addEventListener('DOMContentLoaded', function () {
            initSwitchAtivoGarantia();
        });

        // Executa após postbacks parciais
        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            initSwitchAtivoGarantia();
        });

        var cardTimer = {};

        function mostraCard(element, idProduto, tabela) {

            cardTimer[idProduto + '_' + tabela] = setTimeout(function () {
                console.log(idProduto);
                $.ajax({
                    url: "/API/Pagina_Ajax.aspx/GetProdutoDetalhes",
                    data: JSON.stringify({ idProduto: idProduto }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        var produto = JSON.parse(response.d);
                        var cardProduto = `
                                            <div class="card">
                                                <div class="card-body d-flex">
                                                    <div class="flex-shrink-0" style="min-inline-size: fit-content;">
                                                        ${produto.imagem ? `<img src="${produto.imagem}" alt="Imagem do Produto" class="img-fluid img-thumbnail" style="width: 100px; height: auto;" />` : ''}
                                                    </div>
                                                    <div class="flex-grow-1 d-flex flex-column ms-3" style="min-width: 30%">
                                                        <div class="d-flex">                                          
                                                            <div class="card-text me-3"> ${produto.sCategoriaVendas ? `<strong>Categoria Vendas: </strong>${produto.sCategoriaVendas}` : ''}</div>
                                                            <div class="card-text me-3"> ${produto.sTipo ? `<strong>Tipo: </strong>${produto.sTipo}` : ''}</div>
                                                            <div class="card-text me-3"> ${produto.sFabricante ? `<strong>Fabricante: </strong>${produto.sFabricante}` : ''}</div>
                                                            <div class="card-text me-3"> ${produto.sGrupo ? `<strong>Grupo: </strong>${produto.sGrupo}` : ''}</div>
                                                            <div class="card-text me-3"> ${produto.sFamilia ? `<strong>Família: </strong>${produto.sFamilia}` : ''}</div>
                                                        </div>
                                                    </div>
                                                    <div class="flex-grow-1 d-flex flex-column ms-3" style="max-width: 70%">
                                                        <div class="d-flex">
                                                            <div class="card-text"> ${produto.sCodigoCEST ? `<strong>CEST: </strong>${produto.sCodigoCEST}` : ''}</div>
                                                            <div class="card-text me-3"> ${produto.sCodigoNCM ? `<strong>NCM: </strong>${produto.sCodigoNCM}` : ''}</div>
                                                            <div class="card-text me-3"> ${produto.sPaisOrigem ? `<strong>Origem: </strong>${produto.sPaisOrigem}` : ''}</div>                                                                           
                                                            <div class="card-text me-3"> ${produto.sLocalArmazenamento ? `<strong>Local Armazenamento: </strong>${produto.sLocalArmazenamento}` : ''}</div>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        `;

                        var cardId = idProduto + '_' + tabela;
                        var card = document.getElementById(cardId);
                        card.innerHTML = cardProduto;

                        //Posição card                    
                        var rect = element.getBoundingClientRect();
                        var scrollTop = document.documentElement.scrollTop || document.body.scrollTop;
                        var scrollLeft = document.documentElement.scrollLeft || document.body.scrollLeft;

                        hideAllCards();

                        card.style.top = (rect.top + scrollTop - 10) + 'px';
                        card.style.left = (rect.right + scrollLeft + element.offsetWidth + 10) + 'px';
                        card.style.display = 'block';
                    },
                    error: function (error) {
                        console.error("Erro ao obter os detalhes do produto:", error);
                    }
                });
            }, 300);
        }

        function escondeCard(idProduto, tabela) {
            var cardId = idProduto + '_' + tabela;
            var card = document.getElementById(cardId);

            clearTimeout(cardTimer[idProduto + '_' + tabela]);

            card.style.display = 'none';

        }

        function hideAllCards() {
            var cards = document.querySelectorAll('.product-card');
            cards.forEach(function (card) {
                card.style.display = 'none';
            });
        }

        function openModal(idProduto) {
            $.ajax({
                url: "/API/Pagina_Ajax.aspx/GetProdutoDetalhes",
                data: JSON.stringify({ idProduto: idProduto }),
                type: 'POST',
                dataType: 'json',
                contentType: 'application/json; charset=utf-8',
                success: function (response) {
                    var produto = JSON.parse(response.d);

                    var tituloProduto = `
                                             <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                                 <span aria-hidden="true">&times;</span>
                                             </button>
                                             <h5 class="modal-title" id="detailsModalLabel">${produto.sCodigo} - ${produto.sDsc}</h5>
                                         `;

                    var modalHeader = document.getElementById('modalHeader');
                    modalHeader.innerHTML = tituloProduto;

                    var imagem = '';

                    if (produto.imagem) {
                        imagem += `
                                    <div style="text-align: center; margin-bottom: 20px;">
                                        <img src="${produto.imagem}" alt="Imagem do Produto" class="img-fluid" style="width: 300px; height: auto;"/>
                                    </div>
                                `;
                    }

                    var tabelaProduto = '<table class="table table-bordered">';

                    if (produto.sCategoriaVendas) {
                        tabelaProduto += `
                                            <tr>
                                                <th>Categoria Vendas</th>
                                                <td>${produto.sCategoriaVendas}</td>
                                            </tr>
                                        `;
                    }

                    if (produto.sTipo) {
                        tabelaProduto += `
                                             <tr>
                                                 <th>Tipo</th>
                                                 <td>${produto.sTipo}</td>
                                             </tr>
                                         `;
                    }

                    if (produto.sGrupo) {
                        tabelaProduto += `
                                            <tr>
                                                <th>Grupo</th>
                                                <td>${produto.sGrupo}</td>
                                            </tr>
                                        `;
                    }

                    if (produto.sFabricante) {
                        tabelaProduto += `
                                             <tr>
                                                 <th>Fabricante</th>
                                                 <td>${produto.sFabricante}</td>
                                             </tr>
                                         `;
                    }

                    if (produto.sLocalArmazenamento) {
                        tabelaProduto += `
                                             <tr>
                                                 <th>Local Armazenamento</th>
                                                 <td>${produto.sLocalArmazenamento}</td>
                                             </tr>
                                         `;
                    }

                    if (produto.sFamilia) {
                        tabelaProduto += `
                                             <tr>
                                                 <th>Família</th>
                                                 <td>${produto.sFamilia}</td>
                                             </tr>
                                         `;
                    }

                    if (produto.sCodigoCEST) {
                        tabelaProduto += `
                                             <tr>
                                                 <th>CEST</th>
                                                 <td>${produto.sCodigoCEST}</td>
                                             </tr>
                                         `;
                    }

                    if (produto.sCodigoNCM) {
                        tabelaProduto += `
                                             <tr>
                                                 <th>NCM</th>
                                                 <td>${produto.sCodigoNCM}</td>
                                             </tr>
                                         `;
                    }

                    if (produto.sPaisOrigem) {
                        tabelaProduto += `
                                             <tr>
                                                 <th>Origem</th>
                                                 <td>${produto.sPaisOrigem}</td>
                                             </tr>
                                         `;
                    }

                    tabelaProduto += `</table>`;

                    var modalBody = document.getElementById('modalBody');
                    modalBody.innerHTML = imagem + tabelaProduto;

                    $('#produtoDetalheModal').modal('show');
                },
                error: function (error) {
                    console.error("Erro ao obter os detalhes do produto:", error);
                }
            });
        }
    </script>

</asp:Content>
