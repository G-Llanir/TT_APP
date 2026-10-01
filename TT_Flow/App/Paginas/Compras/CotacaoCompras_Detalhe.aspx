<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="CotacaoCompras_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Compras.CotacaoCompras_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Pesquisa_Parceiros.ascx" TagPrefix="uc1" TagName="Pesquisa_Parceiros" %>
<%@ Register Src="~/App/Controles/FiltroPesquisa.ascx" TagPrefix="uc1" TagName="FiltroPesquisa" %>
<%@ Register Src="~/App/Controles/ExcelImportar.ascx" TagPrefix="uc1" TagName="ExcelImportar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="content_frmEnvio_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>
    <style>
        /*CSS Colapso da Tabela*/
        .gvHeader th {
            padding: 7px;
            background-color: #1A4C1A;
            color: #fff;
            border: 1px solid #bbb;
            font-weight: normal;
        }

        .gvChildHeader th {
            padding: 4px;
            background-color: #999966;
            color: #fff;
            border: 1px solid #bbb;
            font-weight: normal;
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

        /*modal*/
        .modal-header-content {
            display: flex;
            align-items: center;
        }

        .modal-logo img {
            max-width: 50px;
            margin-right: 10px;
        }

        .modal-title-container {
            flex-grow: 1;
        }

        .modal-title {
            color: #009A22;
            text-shadow: 2px 2px 2px rgba(0, 0, 0, 0.2);
            font-size: 24px;
            font-weight: bold;
            margin: 0;
        }

        .card {
            border: 1px solid #e0e0e0;
            border-radius: 0.5rem;
            transition: all 0.3s ease;
            overflow: hidden;
            height: 100%; /* Garante que os cards na mesma linha tenham a mesma altura */
        }

        .card:hover {
            transform: translateY(-5px);
            box-shadow: 0 10px 20px rgba(0, 0, 0, 0.1);
        }

        .card-header {
            background-color: #033712;
            color: #ffffff;
            border-bottom: 1px solid #0056b3;
            padding: 1rem;
            display: flex;
            justify-content: space-between;
            align-items: center;
        }

        .card-body {
            padding: 1.5rem;
            font-size: 1.1rem;
        }

        .card-footer {
            background-color: #f8f9fa;
            border-top: 1px solid #e0e0e0;
            padding: 1rem;
            margin-top: auto; /* Empurra o footer para o final do card */
        }

        .card-title {
            font-size: 1.5rem;
            font-weight: bold;
        }

        .card-subtitle {
            font-size: 1.25rem;
            color: #6c757d;
        }

        .card-text {
            font-size: 1.2rem;
            line-height: 1.6;
        }

        .card-text strong {
            font-size: 1.2rem;
        }

        .badge {
            font-size: 0.875rem;
            padding: 0.35rem 0.7rem;
        }

        /* Estilos da Tabela Comparativa */
        .comparativo-container {
            margin-top: 20px;
            margin-bottom: 20px;
        }

        .comparativo-table {
            width: 100%;
            border-collapse: collapse;
        }

        .comparativo-table th, .comparativo-table td {
            border: 1px solid #ddd;
            padding: 8px;
            text-align: center;
        }

        .comparativo-table th {
            background-color: #009a22;
            color: white;
        }

        .comparativo-table tr:nth-child(even) {
            background-color: #f9f9f9;
        }

        .comparativo-table tr:hover {
            background-color: #e0e0e0;
        }

        .comparativo-table .produto-cell {
            font-weight: bold;
            background-color: #033712;
            color: white;
            text-align: left;
            padding-left: 15px;
        }

        .comparativo-header-checkbox {
            margin-right: 8px;
        }

        .comparativo-table th[colspan="2"] {
            text-align: center;
        }

        .comparativo-table td {
            background-color: #ffffff;
        }

        /*Status Span*/
        .status-circle {
            width: 15px; /* Largura da bolinha */
            height: 15px; /* Altura da bolinha */
            border-radius: 50%; /* Forma circular */
            display: inline-block; /* Para que fique no mesmo nível da linha */
            margin-left: 10px; /* Espaço entre o título e a bolinha */
        }

        .status-success {
            background-color: green;
        }

        .status-danger {
            background-color: red;
        }
        /* Menu Modal */
        .mini-menu {
            background-color: #009a22; /* Cor de fundo principal */
            border-radius: 5px;
            padding: 10px;
        }

        .mini-menu ul {
            list-style-type: none;
            padding: 0;
            display: flex; /* Exibe os itens do menu horizontalmente */
        }

        .mini-menu li {
            margin-right: 10px; /* Espaçamento entre os itens */
        }

        .mini-menu li:last-child {
            margin-right: 0; /* Remove o espaçamento do último item */
        }

        .mini-menu li a {
            color: #FFFFFF; /* Texto branco para melhor contraste */
            text-decoration: none;
            padding: 5px 10px;
            display: flex; /* Alinhamento do texto e da seta */
            align-items: center;
            border-radius: 3px; /* Bordas levemente arredondadas */
            transition: background-color 0.3s ease; /* Suaviza a transição ao passar o mouse */
        }

        .mini-menu li a:hover {
            background-color: #8efa5c; /* Cor de hover mais clara */
            color: #009a22; /* Texto verde escuro no hover */
        }

        .arrow {
            margin-left: 5px; /* Espaço entre o texto e a seta */
        }

        .activeMn {
            background-color: #FFFFFF; /* Fundo branco para item ativo */
            color: #009a22; /* Texto verde para item ativo */
        }
        /* Grupo de vinculação personalizado */
        .btn-vincular-group {
            display: flex;
            align-items: center;
        }

        /* Campo de texto exclusivo */
        .btn-vincular-input {
            border-radius: 0;
            width: auto;
            flex-grow: 1;
        }

        /* Botão de vincular exclusivo */
        .btn-vincular-addon {
            display: inline-block;
            padding: 6px 12px;
            cursor: pointer;
            margin-left: 4px; /* Espaço entre o input e o botão */
        }

        /*Validar Campos*/
        .campo-vazio {
            background-color: #f8d7da; /* Fundo vermelho claro */
        }

        .bg-tt {
            background-color: #009a22 !important;
        }

        .btn-group .btn.active {
            background-color: #006417;
            color: white;
            border-color: #004a11;
        }
        
    </style>
    <div class="form-stacked">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Compras"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
    </div>

    <div>
        <ul id="tab_Cotacao" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="tabpanel active">
                <a href="#Cotacao" id="aba_Cotacao" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false"><b>Solicitação de Cotação</b></a>
            </li>
        </ul>
    </div>

    <div id="tab" class="tab-content">
        <div role="tabpanel" class="tab-pane fade in active" id="Cotacao" aria-labelledby="Envio-tab">

            <br />
            <div class="panel panel-default" runat="server" id="div_Cotacao">
                <div class="panel-heading bg-tt text-white">
                    <h3 class="panel-title"><b style="color: white">Dados da Cotação</b></h3>
                </div>
                <div class="panel-body">
                    <asp:UpdatePanel ID="updDetalhe" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="col-lg-12">

                                <div class="form-stacked row">
                                    <div class="row">
                                        <div class="col-lg-10">
                                            <div class="form-group">
                                                <label>ID</label>
                                                <asp:TextBox ID="txtidCotacao" Placeholder="Novo" class="form-control CaixaTextoMini" runat="server" disabled="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="form-stacked row">
                                    <div class="row">
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label for="txtDtCotacao">Data da Cotação:</label>
                                                <asp:TextBox ID="txtDtCotacao" runat="server" CssClass="form-control" type="date" disabled="true"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label for="txtDtCotacao">Validade da Cotação:</label>
                                                <asp:TextBox ID="txtdtValidade" runat="server" CssClass="form-control" type="date"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-3" runat="server" id="DIV_DADOS_Comprador">
                                            <div class="form-group">
                                                <label>
                                                    <asp:Label ID="lblComprador" runat="server" Text="Comprador"></asp:Label></label>
                                                <asp:DropDownList ID="ddlComprador" runat="server" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-lg-2" runat="server" id="Div_ddlEmpresa">
                                            <div class="form-group">
                                                <label>Empresa</label>
                                                <asp:DropDownList ID="ddlidEmpresa" runat="server" AutoPostBack="True" class="form-control yes_no select Caixa_Selecao" OnSelectedIndexChanged="ddlEmpresa_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-3" id="DivConceito" runat="server">
                                            <div class="form-group">
                                                <label>Conceito </label>
                                                <asp:DropDownList ID="ddlConceito" runat="server" AutoPostBack="True" class="form-control yes_no select Caixa_Selecao" OnSelectedIndexChanged="ddlConceito_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-lg-4" id="divGrupoPatrimonio" runat="server" visible="false">
                                            <div class="form-group">
                                                <label>Grupo de Patrimônio </label>
                                                <asp:DropDownList ID="ddlGrupoPatrimonio" runat="server" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row">
                                        <div class="col-lg-3" runat="server" id="DIV_EnderecoEntrega">
                                            <div class="form-group">
                                                <label>Endereço de Entrega</label>
                                                <asp:DropDownList ID="ddlsEnderecoEntrega" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                            </div>
                                        </div>
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label for="txtsDscCotacao">Descrição</label>
                                                <asp:TextBox ID="txtsDscCotacao" runat="server" CssClass="form-control" placeholder="Descrição da Cotação"></asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-lg-2" runat="server" visible="false">
                                            <div class="form-group">
                                                <label for="txtnValorCotação">Valor da Cotação:</label>
                                                <asp:TextBox ID="txtnValor" runat="server" CssClass="form-control" placeholder="Valor" Visible="false"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row" id="Div_Observacao" runat="server">
                                        <div class="col-lg-12">
                                            <div class="form-group">
                                                <label>Observação </label>
                                                <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="120px" disabled="true"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>

            <!-- =================================================================================================================================================================== -->
            <!-- PAINEL DE COMPARATIVO - MOVIMENTADO PARA FORA DO PAINEL DE DADOS E REESTILIZADO -->
            <!-- =================================================================================================================================================================== -->
            <div class="panel panel-default" runat="server" id="div_Comparativo">
                <div class="panel-heading bg-tt text-white">
                    <h3 class="panel-title"><b style="color: white">Comparativo de Fornecedores</b></h3>
                </div>
                <div class="panel-body">
                    <asp:UpdatePanel ID="updComparativo" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                             <div class="btn-group" role="group" aria-label="Visualização do Comparativo">
                                <asp:Button ID="cmdMostrarCards" Text="Visão em Cartões" runat="server" CssClass="btn btn-primary active" OnClick="cmdMostrarCards_Click" />
                                <asp:Button ID="cmdComparar" Text="Tabela Comparativa" runat="server" CssClass="btn btn-default" OnClick="cmdAbrirComparativos_Click" />
                            </div>
                            <hr />
                            
                            <div id="panelCardsComparativo" class="form-stacked row" runat="server">
                                <asp:Repeater ID="rptFornecedores" runat="server" OnItemDataBound="rptFornecedores_ItemDataBound">
                                    <ItemTemplate>
                                        <div class="col-md-4 mb-4" style="margin-bottom: 15px;">
                                            <div class="card border-light shadow-sm" style="cursor: pointer;" onclick="document.getElementById('<%# ((LinkButton)Container.FindControl("lnkCard")).ClientID %>').click();">
                                                <div class="card-header bg-primary text-white d-flex justify-content-between align-items-center">
                                                    <h5 class="card-title mb-0"><%# Eval("sFornecedor") %></h5>
                                                    <asp:Panel ID="statusBolinha" runat="server" CssClass="status-circle" />
                                                </div>
                                                <div class="card-body">
                                                    <h6 class="card-subtitle mb-2 text-muted">Detalhes</h6>
                                                    <p class="card-text">
                                                        <strong>CNPJ:</strong> <%# Eval("sCnpj") %><br />
                                                        <strong>Prazo de Entrega:</strong> <%# Eval("nPrazoEntrega") %> dias<br />
                                                        <strong>Valor Fornecedor:</strong> R$ <%# Eval("nValor", "{0:N4}") %><br />
                                                        <asp:Label ID="contadorCamposNaoPreenchidos" runat="server" CssClass="badge badge-warning" Text='<%# Eval("nCamposNaoPreenchidos") + " Dados Pendentes" %>'></asp:Label>
                                                    </p>
                                                </div>
                                                <div class="card-footer bg-light text-muted">
                                                    Atualizado em <%# Eval("dtAtualizacao", "{0:dd/MM/yyyy}") %>
                                                </div>
                                            </div>
                                            <br />
                                            <!-- LinkButton invisível para acionar o evento no code-behind -->
                                            <asp:LinkButton ID="lnkCard" runat="server" OnClick="lnkCard_Click" CommandArgument='<%# Eval("idFornecedor") %>' Style="display: none;"></asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </div>

                            <div id="divComparador" class="form-stacked row" runat="server" visible="false">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaComparativo" />
                                </div>
                                <div class="col-lg-12">
                                    <div class="table-responsive">
                                        <asp:Repeater ID="rptComparativos" runat="server" OnItemDataBound="rptComparativos_ItemDataBound" OnItemCreated="rptComparativos_ItemCreated">
                                            <HeaderTemplate>
                                                <div class="comparativo-container">
                                                    <table class="comparativo-table table table-bordered">
                                                        <thead>
                                                            <tr>
                                                                <th rowspan="2" class="produto-cell">Produto</th>
                                                                <asp:Repeater ID="rptFornecedores" runat="server">
                                                                    <ItemTemplate>
                                                                        <th colspan="2">
                                                                            <asp:CheckBox ID="chkGlobal" runat="server" OnCheckedChanged="chkGlobal_CheckedChanged" AutoPostBack="true" />
                                                                            <%# Container.DataItem %>
                                                                        </th>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tr>
                                                            <tr>
                                                                <asp:Repeater ID="rptHeaders" runat="server">
                                                                    <ItemTemplate>
                                                                        <th><%# Container.DataItem %></th>
                                                                    </ItemTemplate>
                                                                </asp:Repeater>
                                                            </tr>
                                                        </thead>
                                                        <tbody>
                                            </HeaderTemplate>
                                            <ItemTemplate>
                                                <tr>
                                                    <td class="produto-cell"><%# Eval("sDscProduto") %></td>
                                                    <asp:Repeater ID="rptValores" runat="server" OnItemDataBound="rptValores_ItemDataBound">
                                                        <ItemTemplate>
                                                            <td>
                                                                <asp:CheckBox ID="chkItem" runat="server"
                                                                    CssClass='<%# Eval("idParceiro") %>'
                                                                    OnCheckedChanged="chkItem_CheckedChanged" AutoPostBack="true"
                                                                    Visible='<%# !string.IsNullOrEmpty(Eval("Preco").ToString()) %>' />
                                                                <asp:Label ID="lblPreco" runat="server" Text='<%# FormatarPrecoBrasileiro(Eval("Preco")) %>'></asp:Label>
                                                            </td>
                                                            <td>
                                                                <asp:Label ID="lblPrevisao" runat="server" Text='<%# Convert.ToDateTime(Eval("Previsao")).ToString("dd/MM/yyyy") %>'></asp:Label>
                                                            </td>
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </tr>
                                                <asp:HiddenField ID="hiddenIdProduto" runat="server" />
                                                <asp:HiddenField ID="hiddenIdParceiro" runat="server" />
                                            </ItemTemplate>
                                            <FooterTemplate>
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </FooterTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>

                                <div class="col-lg-12" style="margin-top:20px;">
                                    <asp:Button ID="cmdGerarPedido"
                                        Text="Gerar Pedido"
                                        runat="server"
                                        Visible="false"
                                        CssClass="btn btn-success"
                                        OnClick="GerarPedido_Click"
                                        UseSubmitBehavior="false"
                                        OnClientClick="this.disabled=true; this.value='Aguarde, gerando...';" />
                                </div>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>

            <div class="panel panel-default" runat="server" id="Div_ItensCotacao">
                <div class="panel-heading">
                    <div>
                        <ul id="tab_Itens" class="nav nav-tabs" role="tablist">
                            <li role="presentation" class="active">
                                <a href="#Itens" id="aba_Itens" role="tabpanel" data-toggle="tab" aria-controls="Itens" aria-expanded="false"><b>Itens</b></a>
                            </li>
                            <li role="presentation" id="li_Importar" runat="server">
                                <a href="#Importar" id="aba_Importar" role="tabpanel" data-toggle="tab" aria-controls="Importar" aria-expanded="false"><b>Importar Itens</b></a>
                            </li>
                        </ul>
                    </div>
                </div>
                <div class="panel-body">
                    <div class="row">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaItens" />
                        </div>
                    </div>
                    <br />
                    <div class="row">

                        <div id="tab_itens" class="tab-content">
                            <div role="tabpanel" class="tab-pane fade in active" id="Itens" aria-labelledby="Item-tab">
                                <div class="col-lg-12" id="DivAddProdutos" runat="server">
                                    <div class="row">
                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <label>Código</label>
                                                <asp:TextBox ID="txtsCodigoProduto" Placeholder="Código" class="form-control" runat="server" MaxLength="20" ValidationGroup="Item" AutoPostBack="True" OnTextChanged="txtsCodigoProduto_TextChanged"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Descrição Produto</label>
                                                <asp:TextBox ID="txtsDscProduto" class="form-control" runat="server" MaxLength="200" ValidationGroup="Item" Placeholder="Produto"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <label>Unidade</label>
                                                <asp:DropDownList ID="ddlsUnidade" runat="server" class="form-control" attrname="Unidade" ValidationGroup="Item"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <label>Quantidade</label>
                                                <asp:TextBox ID="txtnQuantidade" class="form-control uppercase" runat="server" MaxLength="10" ValidationGroup="Item" Placeholder="qtd"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div class="col-lg-1">
                                            <div class="form-group">
                                                <label>&nbsp;</label>
                                                <asp:Button ID="cmdIncluirItem" class="btn btn-info btn-block mt-2" runat="server" ValidationGroup="Item" Text="Incluir" OnClick="cmdIncluirItem_Click" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-12 mt-4">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgItens" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                            Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" DataKeyNames="IdProduto" OnRowDataBound="dtgItens_RowDataBound">
                                            <Columns>

                                                <asp:TemplateField HeaderText="Vincular Produto">
                                                    <ItemTemplate>
                                                        <div class="btn-vincular-group">
                                                            <asp:TextBox ID="txtCodigo" placeholder="Código Prod" runat="server" CssClass="form-control btn-vincular-input" Style="width: 80px; display: inline-block;" />
                                                            <asp:LinkButton ID="cmdVincular" Text="" CssClass="btn btn-sm btn-warning btn-vincular-addon"
                                                                OnClick="cmdVincularItem_Click" CommandArgument='<%# Eval("SCodigo") %>' runat="server">
                                                            <i class="fa fa-link"></i>
                                                            </asp:LinkButton>
                                                        </div>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="2%" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="SCodigo" HeaderText="Código" ItemStyle-Width="10%" />
                                                <asp:BoundField DataField="SDscProduto" HeaderText="Produto" ItemStyle-Width="20%" />
                                                <asp:BoundField DataField="SUnidade" HeaderText="UN" ItemStyle-Width="2%" />
                                                <asp:TemplateField HeaderText="Quantidade">
                                                    <ItemTemplate>
                                                        <asp:TextBox ID="txtnQuantidade" CssClass="form-control" runat="server" Text='<%# Bind("NQuantidade") %>' min="0" step="1" Style="width: 80px;" />
                                                        <asp:TextBox ID="txtsAtivo" CssClass="form-control" runat="server" Text='<%# Bind("SLink") %>' Visible="false" />
                                                        <asp:TextBox ID="txtidCotacao" CssClass="form-control" runat="server" Text='<%# Bind("IdCotacao") %>' Visible="false" />
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="5%" />
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Ação">
                                                    <ItemTemplate>
                                                        <asp:LinkButton Text="" CssClass="btn btn-sm btn-danger" runat="server" OnClick="cmdExcluirItem_Click"><i class="fa fa-trash-o"></i></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <HeaderStyle Width="2%" />
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>

                                <div class="col-lg-12" id="div_btnFornecedor" runat="server">
                                    <fieldset class="form-stacked actions">
                                        <asp:Button ID="cmdAdcionarFornecedor" Text="Adicionar Fornecedor" runat="server" CssClass="btn btn-lg btn-primary" OnClick="AdcionarFornecedor_Click" />
                                    </fieldset>
                                </div>
                            </div>

                            <div role="tabpanel" class="tab-pane fade" id="Importar" aria-labelledby="Importar-tab">
                                <div id="importarExcel" style="display: none;">
                                    <asp:FileUpload runat="server" ID="ImportarArquivo" accept=".xls, .xlsx" />
                                </div>
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>&nbsp;</label>
                                        <uc1:ExcelImportar runat="server" ID="ExcelImportar" />
                                    </div>
                                    <fieldset class="form-stacked actions">
                                        <asp:Button ID="cmdImportarProdutos_Modal" class="btn btn-success" runat="server" Text="Importar" OnClick="cmdImportarProdutos_Modal_Click" />
                                    </fieldset>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
                <%-- MODAL --%>
                <div class="modal fade" id="modalEnvio" tabindex="-1" role="dialog" aria-labelledby="modalEnvioLabel" aria-hidden="true" data-backdrop="static">
                    <asp:UpdatePanel ID="updModal" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="modal-dialog modal-lg" role="document">
                                <div class="modal-content">
                                    <div class="modal-header">
                                        <button type="button" class="close" data-dismiss="modal" aria-label="Fechar">
                                            <span aria-hidden="true">&times;</span>
                                        </button>
                                        <h4 class="modal-title" id="lblModalEnvio">
                                            <asp:Label ID="lbltituloModal" Text="Fornecedor - Cadastro/Atualização" runat="server" />
                                        </h4>
                                    </div>
                                    <div class="modal-body">
                                        <div class="panel-body">

                                            <div id="Div_Menu" runat="server" class="row">
                                                <div class="mini-menu col-lg-12">
                                                    <ul>
                                                        <li>
                                                            <asp:LinkButton ID="cmdInicio" runat="server" OnClientClick="toggleVisibility('cphCorpo_Div_Forms'); return false;"><b>Fornecedores</b><span class="arrow">&rsaquo;</span></asp:LinkButton>
                                                        </li>
                                                        <li runat="server" id="li_historico">
                                                            <asp:LinkButton ID="cmdHistorico" runat="server" OnClientClick="toggleVisibility('cphCorpo_Div_Historico'); return false;"><b>Histórico</b> <span class="arrow">&rsaquo;</span></asp:LinkButton>
                                                        </li>
                                                    </ul>
                                                </div>
                                            </div>

                                            <div class="col-lg-12 mb-4" style="margin-bottom: 2rem;">
                                                <br />
                                                <div id="Div_Forms" class="row" runat="server" style="display: block;">
                                                    <div class="row">
                                                        <div class="col-lg-12">
                                                            <uc1:MensagemPagina runat="server" ID="MensagemPaginaModal" />
                                                        </div>
                                                        <div class="col-lg-12">
                                                            <div class="form-row">
                                                                <div class="row">
                                                                    <div class="form-group col-lg-3" runat="server" id="div6">
                                                                        <b>
                                                                            <asp:Label ID="lblnPedido" runat="server" Text="Nº Pedido (Cliente)"></asp:Label></b>
                                                                        <asp:TextBox ID="txtsPedidoCompra" class="form-control uppercase" runat="server" MaxLength="20" placeholder="PO/PC"></asp:TextBox>
                                                                    </div>
                                                                </div>

                                                                <div class="row">
                                                                    <div class="form-group col-lg-4" id="DivCentroCustos" runat="server">
                                                                        <label>Centro de Custos </label>
                                                                        <asp:DropDownList ID="ddlCentroCusto" runat="server" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                                    </div>
                                                                </div>

                                                                <div class="row">
                                                                    <uc1:Pesquisa_Parceiros runat="server" ID="Pesquisa_Parceiros" SsTipoParceiro="1" Visible="false" />

                                                                    <div class="form-group col-lg-3" runat="server" id="div4">
                                                                        <label>Tipo de Compra</label>
                                                                        <asp:DropDownList ID="ddlsTipoCompra" runat="server" class="form-control yes_no select Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlEmpresa_SelectedIndexChanged">
                                                                            <asp:ListItem Value="0">Selecione o Tipo</asp:ListItem>
                                                                            <asp:ListItem Value="N">Nacional</asp:ListItem>
                                                                            <asp:ListItem Value="I">Internacional</asp:ListItem>
                                                                        </asp:DropDownList>
                                                                    </div>

                                                                    <div class="col-lg-6" id="DIV_Fornecedor" runat="server">
                                                                        <div class="form-group">
                                                                            <label>Fornecedor</label>
                                                                            <asp:DropDownList ID="ddlFornecedor" runat="server" AutoPostBack="True" class="form-control yes_no select Caixa_Selecao" OnSelectedIndexChanged="ddlFornecedor_SelectedIndexChanged"></asp:DropDownList>
                                                                        </div>
                                                                    </div>
                                                                </div>
                                                                <div class="row">
                                                                    <div class="form-group col-lg-3">
                                                                        <label for="txtsTelefone">Telefone</label>
                                                                        <asp:TextBox ID="txtsTelefone" class="form-control" runat="server" placeholder="Telefone"></asp:TextBox>
                                                                    </div>
                                                                    <div class="form-group col-lg-4">
                                                                        <label for="txtsEmail">Email:</label>
                                                                        <asp:TextBox ID="txtsEmail" class="form-control" runat="server" placeholder="exemplo@exem.com"></asp:TextBox>
                                                                    </div>
                                                                    <div class="form-group col-lg-4" runat="server" visible="false">
                                                                        <label for="txtsRGIE">RG/IE:</label>
                                                                        <asp:TextBox ID="txtsRGIE" class="form-control" runat="server" placeholder="RG ou IE"></asp:TextBox>
                                                                    </div>
                                                                    <div class="form-group col-lg-2">
                                                                        <label for="txtnPrazoEntrega">Prazo</label>
                                                                        <asp:TextBox ID="txtnPrazoEntrega" CssClass="form-control" runat="server" placeholder="Nº Dias" TextMode="Number" Min="0" Step="1" />
                                                                    </div>
                                                                    <div class="form-group col-lg-3" runat="server" id="div1">
                                                                        <label id="lblFormadeEnvio" runat="server">Forma de Envio</label>
                                                                        <asp:DropDownList ID="ddlTipoEnvio" runat="server" AutoPostBack="true" class="form-control yes_no select Caixa_Selecao" OnSelectedIndexChanged="ddlTipoEnvio_SelectedIndexChanged"></asp:DropDownList>
                                                                    </div>
                                                                </div>

                                                                <div class="row">
                                                                    <div class="form-group col-lg-3" runat="server" id="div2">
                                                                        <label>Condição Pagamento</label>
                                                                        <asp:DropDownList ID="ddlCondPagamento" runat="server" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                                    </div>
                                                                    <div class="form-group col-lg-4" runat="server" id="div3">
                                                                        <label>Fluxo</label>
                                                                        <asp:DropDownList ID="ddlFluxo" runat="server" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                                                                    </div>
                                                                    <div class="form-group col-lg-2" runat="server" id="div5">
                                                                        <label>Frete:</label>
                                                                        <asp:TextBox ID="txtnFrete" CssClass="form-control" runat="server" MaxLength="11" placeholder="Frete" />
                                                                    </div>

                                                                    <div class="form-group col-lg-3" runat="server" id="divCampoDataEntrega">
                                                                        <label for="txtdtPrevisaoUso">Previsão Entrega:</label>
                                                                        <asp:TextBox ID="txtdtPrevisaoUso" class="form-control" runat="server" type="date" AutoPostBack="true" OnTextChanged="txtdtPrevisaoUso_TextChanged"></asp:TextBox>
                                                                    </div>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row mt-3">
                                                        <div class="col-lg-12" id="div_chkValores" runat="server" style="margin-bottom: 1rem;">
                                                            <asp:CheckBox ID="chkValores" CssClass="btn btn-sm btn-primary" Text="Possui os Valores?" runat="server" AutoPostBack="true" OnCheckedChanged="ChkValores_CheckedChanged" />
                                                            <br />
                                                        </div>

                                                        <div class="col-lg-12" id="div_dgtItensFornecedores" runat="server">
                                                            <div class="table-responsive">
                                                                <asp:GridView ID="dtgItensFornecedor" runat="server" class="table table-striped table-bordered table-hover"
                                                                    Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                                    ShowFooter="False" DataKeyNames="IdProduto" OnRowDataBound="dtgItensFornecedor_RowDataBound">
                                                                    <Columns>
                                                                        <asp:BoundField DataField="SCodigo" HeaderText="Código" ItemStyle-Width="10%" />
                                                                        <asp:BoundField DataField="SDscProduto" HeaderText="Produto" ItemStyle-Width="20%" />
                                                                        <asp:BoundField DataField="SUnidade" HeaderText="UN" ItemStyle-Width="2%" />
                                                                        <asp:TemplateField HeaderText="Quantidade">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblErroQtd" runat="server" ForeColor="Red" />
                                                                                <asp:TextBox ID="txtnQuantidade" disabled="true" CssClass="form-control" runat="server" Text='<%# Bind("NQuantidade") %>' Style="width: 80px;" />
                                                                                <asp:TextBox ID="txtsCodigo" disabled="true" CssClass="form-control" runat="server" Visible="false" Text='<%# Bind("SCodigo") %>' />
                                                                            </ItemTemplate>
                                                                            <HeaderStyle Width="5%" />
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Unitário">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblErroPreco" runat="server" ForeColor="Red" />
                                                                                <asp:TextBox ID="txtnValorCotado" CssClass="form-control" runat="server" MaxLength="11" Text='<%# Bind("NValorCotado","{0:N4}") %>' Style="width: 80px;" />
                                                                            </ItemTemplate>
                                                                            <HeaderStyle Width="5%" />
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="% IPI">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblErroIPI" runat="server" ForeColor="Red" />
                                                                                <asp:TextBox ID="txtnIPI" CssClass="form-control" runat="server" MaxLength="11" Text='<%# Bind("NIPI") %>' Style="width: 80px;" />
                                                                            </ItemTemplate>
                                                                            <HeaderStyle Width="5%" />
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Previsão Entrega">
                                                                            <ItemTemplate>
                                                                                <asp:Label ID="lblErroDt" runat="server" ForeColor="Red" />
                                                                                <asp:TextBox ID="txtdtPrazo" CssClass="form-control" runat="server"
                                                                                    Text='<%# Eval("dtPrevisao") != null && ((DateTime)Eval("dtPrevisao")) != DateTime.MinValue ? ((DateTime)Eval("dtPrevisao")).ToString("yyyy-MM-dd") : string.Empty %>'
                                                                                    TextMode="Date" />
                                                                            </ItemTemplate>
                                                                            <HeaderStyle Width="10%" />
                                                                        </asp:TemplateField>
                                                                        <asp:TemplateField HeaderText="Ação" Visible="false">
                                                                            <ItemTemplate>
                                                                                <asp:LinkButton Text="" CssClass="btn btn-sm btn-danger" runat="server" OnClick="cmdExcluirItemFornecedor_Click"><i class="fa fa-trash-o"></i></asp:LinkButton>
                                                                            </ItemTemplate>
                                                                            <HeaderStyle Width="2%" />
                                                                        </asp:TemplateField>
                                                                    </Columns>
                                                                </asp:GridView>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>

                                                <div id="Div_Historico" runat="server" class="row mb-2" style="display: none;">
                                                    <br />
                                                    <div class="col-lg-12">
                                                        <uc1:MensagemPagina runat="server" ID="MensagemPaginaHistorico" />
                                                    </div>
                                                    <div class="col-lg-12">
                                                        <div class="row">
                                                            <asp:GridView ID="dtgHistorico" runat="server" class="table table-striped table-bordered table-hover table-condensed"
                                                                Width="100%" GridLines="None"
                                                                HeaderStyle-CssClass="gvHeader" CssClass="gvRow" AlternatingRowStyle-CssClass="gvAltRow"
                                                                AutoGenerateColumns="False"
                                                                ShowFooter="False" DataKeyNames="idHistorico" OnRowDataBound="dtgHistorico_RowDataBound">
                                                                <Columns>
                                                                    <asp:TemplateField>
                                                                        <ItemTemplate>
                                                                            <asp:LinkButton ID="btnToggle" runat="server" CssClass="toggle-icon" data-div-id='<%# "tr" + Eval("idHistorico") %>'>
                                                                            <i style="cursor: pointer;"></i>
                                                                            </asp:LinkButton>
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:TemplateField HeaderText="Ação">
                                                                        <ItemTemplate>
                                                                            <asp:Literal ID="LiteralAcao" runat="server" EnableViewState="false" />
                                                                        </ItemTemplate>
                                                                    </asp:TemplateField>
                                                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Atualização" />
                                                                    <asp:TemplateField HeaderText="Responsável">
                                                                        <ItemTemplate>
                                                                            <label><%# Eval("sUsuario") %></label>
                                                                            <%# NovaLinha(Eval("idHistorico"),"") %>

                                                                            <asp:GridView ID="dtgHistoricoItens" runat="server" Width="100%"
                                                                                GridLines="None" AutoGenerateColumns="false" DataKeyNames="idHistorico"
                                                                                HeaderStyle-CssClass="gvChildHeader" CssClass="gvRow" Style="padding: 0; margin: 0"
                                                                                AlternatingRowStyle-CssClass="gvAltRow" OnRowDataBound="dtgHistoricoItens_RowDataBound">
                                                                                <Columns>
                                                                                    <asp:BoundField DataField="sDscProduto" HeaderText="Produto" />
                                                                                    <asp:BoundField DataField="nQuantidade" HeaderText="Quantidade" />
                                                                                    <asp:BoundField DataField="nValor" HeaderText="Unitário" />
                                                                                    <asp:BoundField DataField="dtPrevisao" HeaderText="Previsão" DataFormatString="{0:dd/MM/yyyy}" HtmlEncode="false" />
                                                                                    <asp:TemplateField HeaderText="Ação">
                                                                                        <ItemTemplate>
                                                                                            <asp:Literal ID="LiteralAcao" runat="server" EnableViewState="false" />
                                                                                        </ItemTemplate>
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
                                            <br />
                                            <div class="col-lg-12 mt-4">
                                                <div class="footer">
                                                    <asp:Button ID="cmdRegistrarFornecedor" Text="Salvar" runat="server" CssClass="btn btn-success" OnClick="SalvarFornecedor_Click" />
                                                    <button type="button" class="btn btn-secondary" data-dismiss="modal">Fechar</button>
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
        </div>
    </div>

    <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
    <br />

    <fieldset class="form-stacked actions">
        <asp:Button ID="cmdCotar" Text="Salvar" runat="server" CssClass="btn btn-lg btn-success" OnClick="Salvar_Click" />
        <asp:Button ID="cmdVoltar" class="btn btn-lg btn-warning" runat="server" Text="Voltar" OnClick="Voltar_Click" />
    </fieldset>

    <script type="text/javascript">
        function aplicarMascaraValorCotado() {
            $('input[id^=cphCorpo_dtgItensFornecedor_txtnValorCotado]').on('input', function () {
                let valor = this.value.replace(/\D/g, ''); // Remove tudo que não for número

                if (valor.length === 0) {
                    valor = '0';
                }

                while (valor.length < 5) {
                    valor = '0' + valor;
                }
                
                let parteInteira = valor.slice(0, valor.length - 4); 
                let parteDecimal = valor.slice(valor.length - 4); 

                parteInteira = parseInt(parteInteira, 10).toString();
                parteInteira = parteInteira.replace(/\B(?=(\d{3})+(?!\d))/g, ".");

                let valorFormatado = parteInteira + ',' + parteDecimal;
                this.value = valorFormatado;
            });
        }
        function aplicarMascaraNIPI() {
            $('input[id^=cphCorpo_dtgItensFornecedor_txtnIPI]').on('input', function () {
                let valor = this.value.replace(/\D/g, '');

                if (valor.length === 0) {
                    valor = '0';
                }

                while (valor.length < 3) {
                    valor = '0' + valor;
                }

                let parteInteira = valor.slice(0, valor.length - 2);
                let parteDecimal = valor.slice(valor.length - 2);

                parteInteira = parseInt(parteInteira, 10).toString();
                parteInteira = parteInteira.replace(/\B(?=(\d{3})+(?!\d))/g, ".");

                let valorFormatado = parteInteira + ',' + parteDecimal;
                this.value = valorFormatado;
            });
        }
        function aplicarMascaraValorFrete() {
            $('input[id^=cphCorpo_txtnFrete]').on('input', function () {
                let valor = this.value.replace(/\D/g, '');

                if (valor.length === 0) {
                    valor = '0';
                }

                while (valor.length < 3) {
                    valor = '0' + valor;
                }

                let parteInteira = valor.slice(0, valor.length - 2);
                let parteDecimal = valor.slice(valor.length - 2);

                parteInteira = parseInt(parteInteira, 10).toString();
                parteInteira = parteInteira.replace(/\B(?=(\d{3})+(?!\d))/g, ".");

                let valorFormatado = parteInteira + ',' + parteDecimal;
                this.value = valorFormatado;
            });
        }
        function aplicarMascaraRGIE() {
            var rgIEField = $('#<%= txtsRGIE.ClientID %>');

            rgIEField.on('input', function () {
                this.value = this.value.replace(/[^0-9]/g, '');
                if (this.value.length > 9) {
                    this.value = this.value.substring(0, 9);
                }
            });
        }

        function toggleVisibility(divId) {
            var divs = ['cphCorpo_Div_Forms', 'cphCorpo_Div_Historico'];

            divs.forEach(function (id) {
                var div = document.getElementById(id);
                if (id === divId) {
                    div.style.display = "block";
                } else {
                    div.style.display = "none";
                }
            });
        }

        function aplicarMascaraTelefone() {
            var telefoneField = $('#<%= txtsTelefone.ClientID %>');

            telefoneField.on('input', function () {
                this.value = this.value.replace(/[^0-9]/g, '');
                if (this.value.length > 11) {
                    this.value = this.value.substring(0, 11);
                }

                if (this.value.length <= 10) {
                    this.value = this.value.replace(/(\d{2})(\d{4})(\d{0,4})/, "($1) $2-$3");
                } else {
                    this.value = this.value.replace(/(\d{2})(\d{5})(\d{0,4})/, "($1) $2-$3");
                }
            });
        }

        function RegistrarColapsoScript() {
            $(document).ready(function () {
                $('.toggle-icon').addClass('fa fa-plus');

                $('.toggle-icon').click(function () {
                    var icon = $(this);
                    var divId = $(this).data('div-id');
                    var current = $('#' + divId).css('display');
                    if (current == 'none') {
                        $('#' + divId).show('slow');
                        icon.removeClass('fa fa-plus').addClass('fa fa-minus');
                    } else {
                        $('#' + divId).hide('slow');
                        icon.removeClass('fa fa-minus').addClass('fa fa-plus');
                    }
                    return false; 
                });
            });
        }

        $(document).ready(function () {
            aplicarMascaraValorCotado();
            aplicarMascaraNIPI();
            aplicarMascaraValorFrete();
            aplicarMascaraRGIE();
            aplicarMascaraTelefone();
            RegistrarColapsoScript();

            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                aplicarMascaraValorCotado();
                aplicarMascaraNIPI();
                aplicarMascaraValorFrete();
                aplicarMascaraRGIE();
                aplicarMascaraTelefone();
                RegistrarColapsoScript();
            });
        });

    </script>
    <asp:HiddenField ID="hddidCotacao" runat="server" />
    <asp:HiddenField ID="hddidRequisicao" runat="server" />
    <asp:HiddenField ID="hddidProduto" runat="server" />
    <asp:HiddenField ID="hddidFornecedor" runat="server" />
    <asp:HiddenField ID="hddidParceiro" runat="server" />
    <asp:HiddenField ID="hddsEdicao" runat="server" />
    <asp:HiddenField ID="hddidEmpresa" runat="server" />
    <asp:HiddenField ID="hddidHistorico" runat="server" />
    <asp:HiddenField ID="hddsObservacao" runat="server" />
</asp:Content>
