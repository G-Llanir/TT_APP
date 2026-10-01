<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Fabricacao_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.PCP.Fabricacao_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/LeitorQuagga.ascx" TagPrefix="uc1" TagName="LeitorQuagga" %>
<%@ Register Src="~/App/Controles/GerenciadorMembrosModal.ascx" TagPrefix="uc1" TagName="GerenciadorMembrosModal" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /********  tabela externa  *******************************************/
        .kanban-table-outer {
            width: 100%;
            border-collapse: collapse;
            font-family: Arial, sans-serif;
        }

            .kanban-table-outer th,
            .kanban-table-outer td {
                border: 1px solid #ccc;
                padding: 0.75rem;
                text-align: center;
            }

        /* coluna fixa do ID */
        .id-col {
            width: 90px;
            background: #024e0a;
            border-right: 4px solid #009a22;
            vertical-align: middle;
            text-align: center;
            padding: 0.5rem 0.25rem;
        }

        .badge-id {
            background: #009a22;
            color: #fff;
            font-size: 1.2rem;
            font-weight: 700;
            padding: 0.5rem 1rem;
            border-radius: 6px;
            display: inline-block;
            text-align: center;
        }

        .project-title {
            background: #024e0a;
            color: #fff;
            font-size: 1.2rem;
            font-weight: bold;
            text-align: left;
            padding-left: 1rem;
        }

        /********  sub-table (colunas variáveis)  ****************************/
        .kanban-table-inner {
            width: 100%;
            border-collapse: collapse;
        }

            .kanban-table-inner th,
            .kanban-table-inner td {
                border: 1px solid #ccc;
                padding: 0.6rem;
                text-align: center;
            }

        .col-header th {
            background: #009a22;
            color: #fff;
            font-weight: 600;
        }

        .cards-row td {
            background: #fff;
        }

        .cards-row:hover td {
            background: #f2fff4;
        }

        /* card simplificado já existente */
        .kanban-card {
            border-left: none !important;
            padding: 6px;
        }

        .success {
            background-color: #198754 !important;
            color: #fff !important;
        }

        .danger {
            background-color: #dc3545 !important;
            color: #fff !important;
        }

        .warning {
            background-color: #ffc107 !important;
            color: #000 !important;
        }

        .info {
            background-color: #0dcaf0 !important;
            color: #000 !important;
        }

        .primary {
            background-color: #0d6efd !important;
            color: #fff !important;
        }

        .secondary {
            background-color: #6c757d !important;
            color: #fff !important;
        }

        .light {
            background-color: #f8f9fa !important;
            color: #000 !important;
        }

        .dark {
            background-color: #212529 !important;
            color: #fff !important;
        }

        /* ---------------------- MODAL PROJETOS ---------------------- */
        .modal-projetos-header {
            background-color: #024e0a;
            color: #fff;
            padding: 1.5rem 2rem;
            border-top-left-radius: 1rem;
            border-top-right-radius: 1rem;
            align-items: center;
            font-size: 1.5rem;
            font-weight: 700;
        }

        .modal-projetos-body {
            background-color: #f9f9f9;
            padding: 2rem 2rem;
            font-size: 1.2rem;
            line-height: 1.6;
        }

        .modal-projetos-card {
            background-color: #ffffff;
            border-left: 8px solid #009a22;
            border-radius: 1rem;
            padding: 2rem;
            box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
            margin-bottom: 2.5rem;
        }

        .modal-projetos-produto {
            font-size: 1.5rem;
            font-weight: 700;
            margin-bottom: 1rem;
        }

        .modal-projetos-requisicao {
            font-size: 1.1rem;
            margin-bottom: 1.5rem;
            color: #444;
        }

        .modal-projetos-recurso {
            background-color: #e9f8ec;
            border: 1.5px solid #009a22;
            border-left-width: 8px;
            border-radius: 1rem;
            padding: 1.5rem 2rem;
            margin-bottom: 1.5rem;
        }

            .modal-projetos-recurso h6 {
                font-size: 1.3rem;
                font-weight: 700;
                margin-bottom: 0.75rem;
                color: #007d12;
            }

        .modal-projetos-processo {
            font-size: 1rem;
            color: #555;
            margin-bottom: 0.75rem;
            line-height: 1.4;
        }

        /* Cores de apoio */
        .bg-verde-tt {
            background-color: #009a22 !important;
        }

        .text-verde-tt {
            color: #009a22 !important;
        }

        .border-verde-tt {
            border-color: #009a22 !important;
        }

        .bg-verde-escuro {
            background-color: #024e0a !important;
        }

        .text-verde-escuro {
            color: #024e0a !important;
        }

        .border-verde-escuro {
            border-color: #024e0a !important;
        }
    </style>
    <style>
        .btn-visualizar-rounded {
            border-radius: 50px;
            background-color: transparent;
            border: 2px solid #0d6efd;
            color: #0d6efd;
            padding: 0.375rem 1.25rem;
            font-weight: 600;
            transition: all 0.3s ease;
            display: inline-flex;
            align-items: center;
            gap: 0.5rem;
            cursor: pointer;
        }

            .btn-visualizar-rounded i {
                font-size: 1.1rem;
            }

            .btn-visualizar-rounded:hover,
            .btn-visualizar-rounded:focus {
                background-color: #0d6efd;
                color: #fff;
                border-color: #0d6efd;
                text-decoration: none;
            }

        .btn-group-visualizar {
            display: flex;
            flex-direction: column;
            align-items: center;
            gap: 0.5rem;
            margin-top: 0.5rem;
        }

        .status-filhas-container {
            display: flex;
            margin-top: 5px;
            justify-content: center;
            border-radius: 4px;
            overflow: hidden;
            box-shadow: inset 0 1px 3px rgba(0,0,0,0.1);
        }

        .status-item {
            flex-grow: 1;
            min-width: 25px;
            height: 20px;
            display: flex;
            align-items: center;
            justify-content: center;
            font-weight: bold;
            font-size: 0.85em;
            color: #fff;
            border: none;
            padding: 0 4px;
            white-space: nowrap;
        }

        .warning, .info, .light {
            color: #000 !important;
        }

        .projeto-header-row td {
            background: linear-gradient(135deg, #024e0a 0%, #009a22 100%);
            padding: 1rem 1.5rem;
            text-align: left;
            border-radius: 6px 6px 0 0;
            color: #fff;
        }


        .projeto-info-header {
            display: flex;
            align-items: center;
            gap: 1rem;
            flex-wrap: wrap;
        }

        .badge-projeto-id {
            background: rgba(255,255,255,0.2);
            padding: 0.4rem 0.8rem;
            border-radius: 4px;
            font-weight: 700;
        }

        .btn-visualizar-projeto {
            background: #fff;
            color: #024e0a;
            border: none;
            padding: 0.5rem 1rem;
            border-radius: 4px;
            font-weight: 600;
            cursor: pointer;
            transition: all 0.2s;
            display: inline-flex;
            align-items: center;
            gap: 0.5rem;
        }

            .btn-visualizar-projeto:hover {
                background: #f0f0f0;
                transform: translateY(-1px);
            }

        .projeto-produtos-table {
            width: 100%;
            border-collapse: collapse;
            font-family: 'Segoe UI', Arial, sans-serif;
            background: #fff;
            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
            margin-bottom: 0;
        }

        .produtos-header th {
            background: #f8f9fa;
            color: #024e0a;
            font-weight: 700;
            text-transform: uppercase;
            letter-spacing: 0.5px;
            padding: 0.75rem 0.5rem;
            border-bottom: 2px solid #000;
            text-align: center;
            vertical-align: middle;
        }

        .produto-row:not(:last-child) td {
            border-bottom: 2px solid #777 !important;
        }

        .col-imagem {
            width: 100px;
            text-align: center;
        }

        .col-produto {
            width: 200px;
            text-align: left;
        }

        .col-detalhes {
            width: 200px;
            text-align: left;
        }

        .cell-imagem,
        .cell-produto,
        .cell-detalhes,
        .cell-operacao {
            padding: 0.75rem 0.5rem;
            border: 1px solid #e0e0e0;
            vertical-align: middle;
        }

        .cell-detalhes {
            border-right: 1px solid #777 !important;
            min-width: 200px;
        }

        .cell-produto {
            min-width: 200px;
        }

        .produto-info {
            display: flex;
            flex-direction: column;
            gap: 0.35rem;
        }

        .produto-codigo-link {
            font-weight: 700;
            color: #009a22;
            text-decoration: none;
            border-bottom: 1px dashed #009a22;
        }

            .produto-codigo-link:hover {
                color: #024e0a;
                border-bottom-color: #024e0a;
                background: #f0fff0;
                padding: 2px 4px;
                border-radius: 3px;
            }

        .produto-nome {
            color: #666;
        }

        .detalhes-ordem {
            display: flex;
            flex-direction: column;
            gap: 0.25rem;
        }

        .detalhe-item {
            display: flex;
            justify-content: space-between;
        }

        .detalhe-label {
            font-weight: 600;
            color: #555;
        }

        .detalhe-valor {
            color: #333;
            font-weight: 500;
        }

        .cell-operacao {
            background: #fff;
            border-left: 1px solid #ddd;
            text-align: left;
            min-width: 250px;
        }

        .operacao-container {
            display: flex;
            align-items: flex-start;
            gap: 0.5rem;
        }

        .status-indicator {
            width: 14px;
            height: 14px;
            border-radius: 50%;
            flex-shrink: 0;
            margin-top: 2px;
        }

        .operacao-info {
            flex: 1;
        }

        .op-numero-nome {
            color: #024e0a;
            margin-bottom: 0.25rem;
            font-weight: bold;
        }

        .op-ct, .op-producao {
            display: flex;
            gap: 0.25rem;
        }

        .projeto-separator {
            height: 30px;
            background: transparent;
        }

        @media (max-width: 1200px) {
            .projeto-produtos-table {
                font-size: 0.75rem;
            }

            .col-imagem {
                width: 80px;
            }

            .col-produto {
                width: 170px;
            }

            .col-detalhes {
                width: 170px;
            }
        }
    </style>
</asp:Content>

<asp:Content ID="content_frmEnvio_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <div class="form-stacked">
        <div class="col-lg-12 row">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Fabricação"></asp:Label>
            </h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Detalhe" />
        </div>


    </div>
    <div>
        <ul id="tab_Fabricacao" class="nav nav-tabs" role="tablist">
            <li role="presentation" class="active">
                <a href="#Fabricacao" role="tab" data-toggle="tab" aria-controls="Fabricacao" aria-expanded="true"><b>Gestão à Vista</b></a>
            </li>
            <li role="presentation">
                <a href="#StatusFuncionarios" role="tab" data-toggle="tab" aria-controls="StatusFuncionarios" aria-expanded="false"><b>Status dos Funcionários</b></a>
            </li>
            <li role="presentation">
                <a href="#Historico" role="tab" data-toggle="tab" aria-controls="Historico" aria-expanded="false"><b>Histórico</b></a>
            </li>
        </ul>
    </div>
    <br />

    <div id="tab" class="tab-content">
        <div role="tabpanel" class="tab-pane fade in active" id="Fabricacao" aria-labelledby="Envio-tab">

            <div id="divFiltro" runat="server" class="row" visible="false">
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body ">
                            <div class="row">
                                <%--    <div class="col-md-3">
                                    <uc1:ComboAtivo ID="cmbAtivo" runat="server" CssClass="form-control" />
                                </div>
                                <div class="col-md-3">
                                    <asp:TextBox ID="txtCodigoProduto" runat="server" CssClass="form-control" placeholder="Código do Produto"></asp:TextBox>
                                </div>--%>
                                <div class="col-md-3">
                                    <asp:TextBox ID="txtNomeFabricacao" runat="server" CssClass="form-control" placeholder="Pesquisa"></asp:TextBox>
                                </div>
                                <div class="col-md-3">
                                    <asp:Button ID="btnPesquisar" runat="server" CssClass="btn btn-primary btn-block" Text="Pesquisar" OnClick="btnPesquisar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-body table-responsive">
                            <%-- <div class="panel-heading">
                                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Gestor à Vista </h3>
                            </div>--%>
                            <div>
                                <div class="panel panel-info">
                                    <div class="panel-heading bg-tt text-white">
                                        <h4 class="panel-title">Legenda de Status</h4>
                                    </div>
                                    <div class="panel-body">
                                        <p>As cores abaixo indicam o status atual de cada item:</p>
                                        <ul class="list-inline">
                                            <li>
                                                <span class="badge rounded-pill px-3 py-1 info text-dark">Não Iniciada</span>
                                                - Atividade não foi iniciada.
                                            </li>
                                            <li>
                                                <span class="badge rounded-pill px-3 py-1 warning text-dark">Em Andamento</span>
                                                - Atividade está em execução no momento.
                                            </li>
                                            <li>
                                                <span class="badge rounded-pill px-3 py-1 success">Finalizada</span>
                                                - Atividade foi finalizada com sucesso.
                                            </li>
                                        </ul>
                                    </div>
                                </div>
                            </div>

                            <div class="unified-wrapper">
                                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                                <asp:Repeater ID="rptProjetos" runat="server" OnItemDataBound="rptProjetos_ItemDataBound">
                                    <ItemTemplate>

                                        <table class="projeto-produtos-table">
                                            <thead>
                                                <tr class="projeto-header-row">
                                                    <td colspan='<%# GetTotalColunas(Container.DataItem) %>'>
                                                        <div class="projeto-info-header">
                                                            <span class="badge-projeto-id">Projeto: <%# Eval("idProjeto") %></span>
                                                            <asp:LinkButton ID="btnAbrirProjeto" runat="server"
                                                                CommandName="AbrirProjeto"
                                                                CommandArgument='<%# Eval("idProjeto") %>'
                                                                OnCommand="cmdAbrirProjeto_Click"
                                                                CssClass="btn-visualizar-projeto">
                                                                <i class="fa fa-folder-open"></i> Abrir Projeto
                                                            </asp:LinkButton>
                                                        </div>
                                                    </td>
                                                </tr>

                                                <tr class="produtos-header">
                                                    <th class="col-produto">Produtos</th>
                                                    <th class="col-detalhes">Detalhes da Ordem</th>
                                                    <th class="col-operacoes-unificado" colspan='<%# GetNumeroOperacoes(Container.DataItem) %>'>Atividades</th>
                                                </tr>
                                            </thead>
                                            <tbody>
                                                <asp:Repeater ID="rptProdutos" runat="server" OnItemDataBound="rptProdutos_ItemDataBound">
                                                    <ItemTemplate>
                                                        <tr class="produto-row">                                                       

                                                            <td class="cell-produto">
                                                                <div class="produto-info">
                                                                    <asp:HyperLink ID="lnkCodigoProduto" runat="server"
                                                                        CssClass="produto-codigo-link"
                                                                        NavigateUrl='<%# ResolveUrl("~/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id=" + Eval("idProduto")) %>'
                                                                        Target="_blank">
                                                                        <%# Eval("sCodigo") %>
                                                                    </asp:HyperLink>
                                                                    <span class="produto-nome"><%# Eval("sDscProduto") %></span>
                                                                </div>
                                                            </td>

                                                            <td class="cell-detalhes">
                                                                <div class="detalhes-ordem">
                                                                    <div class="detalhe-item">
                                                                        <span class="detalhe-label">Quantidade:</span>
                                                                        <span class="detalhe-valor"><%# Eval("Quantidade") %></span>
                                                                    </div>
                                                                    <div class="detalhe-item">
                                                                        <span class="detalhe-label">Atualização:</span>
                                                                        <span class="detalhe-valor"><%# Eval("dtAtualizacao") %></span>
                                                                    </div>
                                                                </div>
                                                            </td>

                                                            <asp:PlaceHolder ID="phOperacoesProduto" runat="server"></asp:PlaceHolder>
                                                        </tr>
                                                    </ItemTemplate>
                                                </asp:Repeater>
                                            </tbody>
                                        </table>

                                        <div class="projeto-separator"></div>
                                    </ItemTemplate>

                                    <FooterTemplate>
                                        </div>
                                    </FooterTemplate>
                                </asp:Repeater>
                            </div>
                        </div>
                        <%-- <uc1:painelatualizacao runat="server" id="PainelAtualizacao" />--%>
                        <br />
                    </div>
                </div>
            </div>

            <fieldset class="form-stacked actions">
                <asp:Button ID="cmdVoltar" class="btn btn-lg btn-warning" runat="server" Text="Voltar" OnClick="Voltar_Click" Visible="false" />
            </fieldset>
        </div>
        <uc1:GerenciadorMembrosModal runat="server" ID="GerenciadorMembrosModal" />

        <%-- Aba 2: Status dos Funcionários (Layout Pivoteado) --%>
        <div role="tabpanel" class="tab-pane fade" id="StatusFuncionarios" aria-labelledby="StatusFuncionarios-tab">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-user-clock"></i>Status Atual dos Usuários</h3>
                </div>
                <div class="panel-body table-responsive">

                    <asp:Repeater ID="rptStatusSimplificado" runat="server">
                        <HeaderTemplate>
                            <table class="table table-hover table-bordered">
                                <thead class="thead-light">
                                    <tr>
                                        <th style="width: 25%;">Usuário</th>
                                        <th style="width: 40%;">Atividade Atual</th>
                                        <th style="width: 35%;">Última Atividade Concluída</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td style='<%# (bool)Eval("IsIdle") ? "background-color: #f2dede;": "background-color: #dff0d8;" %>'>
                                    <strong style='<%# (bool)Eval("IsIdle") ? "color: #a94442;": "color: #3c763d;" %>'>
                                        <%# Eval("NomeUsuario") %>
                                    </strong>
                                </td>
                                <td style='<%# (bool)Eval("IsIdle") ? "background-color: #f2dede;": "background-color: #dff0d8;" %>'>
                                    <%# string.IsNullOrEmpty(Eval("AtividadeAtual") as string) ? "Sem Atividade" : Eval("AtividadeAtual") %>
                                </td>
                                <td>
                                    <%# string.IsNullOrEmpty(Eval("AtividadeAnterior") as string) ? "<span class='text-muted'>Nenhuma</span>" : Eval("AtividadeAnterior") %>
                                </td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                </table>
                        </FooterTemplate>
                    </asp:Repeater>

                    <%-- Painel para quando não há dados --%>
                    <asp:Panel ID="pnlFuncionariosVazio" runat="server" Visible="false">
                        <div class="alert alert-warning text-center">
                            <strong>Nenhum usuário encontrado em projetos de fabricação.</strong>
                        </div>
                    </asp:Panel>

                </div>
            </div>
        </div>

        <%-- Aba 3: Histórico de Atividades --%>
        <div role="tabpanel" class="tab-pane fade" id="Historico" aria-labelledby="Historico-tab">
            <div class="panel panel-info">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-history"></i>Histórico de Atividades por Recurso</h3>
                </div>
                <div class="panel-body">
                    <%-- O Repeater do histórico vai aqui --%>
                    <asp:Repeater ID="rptHistorico" runat="server">
                        <HeaderTemplate>
                            <table class="table table-striped table-bordered">
                                <thead>
                                    <tr>
                                        <th>Recurso (Atividade Pai)</th>
                                        <th>Processo (Atividade Filha)</th>
                                        <th>Início</th>
                                        <th>Fim</th>
                                        <th>Duração (min)</th>
                                        <th>Executado por</th>
                                    </tr>
                                </thead>
                                <tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Eval("NomeRecurso") %></td>
                                <td><%# Eval("NomeProcesso") %></td>
                                <td><%# Eval("DataInicio", "{0:dd/MM/yyyy HH:mm}") %></td>
                                <td><%# Eval("DataFim", "{0:dd/MM/yyyy HH:mm}") %></td>
                                <td><%# Eval("DuracaoMinutos") %></td>
                                <td><%# Eval("NomeExecutor") %></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate>
                            </tbody>
                        </table>
                   
                        </FooterTemplate>
                    </asp:Repeater>
                    <asp:Panel ID="pnlHistoricoVazio" runat="server" Visible="false">
                        <div class="alert alert-warning text-center">
                            <strong>Nenhum registro encontrado.</strong>
                        </div>
                    </asp:Panel>
                </div>
            </div>
        </div>

    </div>



    <script type="text/javascript">
        $(document).ready(function () {

        });

        Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
            // Resposta ao postback para manipulação dos componentes após atualização do UpdatePanel
        });
    </script>

    <asp:HiddenField ID="hddidFabricacao" runat="server" />
    <asp:HiddenField ID="hddidAtividade" runat="server" />
    <asp:HiddenField ID="hddidProjeto" runat="server" />
    <asp:HiddenField ID="hddidRequisicao" runat="server" />
    <asp:HiddenField ID="hddidProduto" runat="server" />
    <asp:HiddenField ID="hddidFornecedor" runat="server" />
    <asp:HiddenField ID="hddidParceiro" runat="server" />
    <asp:HiddenField ID="hddsEdicao" runat="server" />
    <asp:HiddenField ID="hddidEmpresa" runat="server" />
    <asp:HiddenField ID="hddidHistorico" runat="server" />
    <asp:HiddenField ID="hddsObservacao" runat="server" />
</asp:Content>


