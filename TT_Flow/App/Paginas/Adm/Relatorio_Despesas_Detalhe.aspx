<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Relatorio_Despesas_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Relatorio_Despesas_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.min.js"></script>

    <style>
        .chosen-container .chosen-drop {
            z-index: 99999 !important;
        }

        .modal-backdrop {
            z-index: 1040 !important;
        }

        .modal {
            z-index: 1050 !important;
        }

        .actions-footer {
            margin-top: 20px;
            padding-top: 15px;
            border-top: 1px solid #ddd;
            text-align: left;
            background-color: #f9f9f9;
            padding: 15px;
            border-radius: 4px;
        }

            .actions-footer .btn {
                margin-right: 10px;
            }

        .table-bordered {
            border: 1px solid #ddd !important;
        }

            .table-bordered > thead > tr > th, .table-bordered > tbody > tr > th, .table-bordered > tfoot > tr > th,
            .table-bordered > thead > tr > td, .table-bordered > tbody > tr > td, .table-bordered > tfoot > tr > td {
                border: 1px solid #ddd !important;
                vertical-align: middle;
            }

        .empty-grid-msg {
            text-align: center;
            padding: 30px;
            color: #999;
            background-color: #fff;
            border: 1px dashed #ccc;
            font-size: 16px;
        }

        /* --- ESTILO DAS ABAS --- */
        .nav-tabs {
            border-bottom: 2px solid #ddd;
            margin-bottom: 15px;
        }

            .nav-tabs > li > a {
                font-weight: bold;
                color: #555;
                font-size: 14px;
            }

            .nav-tabs > li.active > a, .nav-tabs > li.active > a:hover, .nav-tabs > li.active > a:focus {
                color: #333;
                cursor: default;
                background-color: #fff;
                border: 2px solid #ddd;
                border-bottom-color: transparent;
            }

        /* Estilo inspirado na imagem fornecida */
        .tabela-relatorio {
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            font-size: 12px;
            border: 1px solid #ccc;
            width: 100%;
            background-color: white;
        }

            .tabela-relatorio th {
                background-color: #f0f3f5; /* Cinza claro do header */
                color: #333;
                font-weight: bold;
                text-align: center;
                border: 1px solid #d1d1d1;
                padding: 8px;
                white-space: nowrap;
            }

            .tabela-relatorio td {
                border: 1px solid #e7e7e7;
                padding: 6px;
                vertical-align: middle;
            }

        /* Linha de Título do Colaborador (Azul Claro) */
        .row-colaborador {
            background-color: #d9edf7 !important;
            color: #31708f;
            font-weight: bold;
            font-size: 13px;
            text-align: left;
            border-top: 2px solid #bce8f1;
        }

        /* Linha de Total do Colaborador (Rodapé do grupo) */
        .row-total {
            background-color: #f9f9f9;
            font-weight: bold;
            border-top: 1px solid #ccc;
            color: #555;
        }

        /* Células de dia */
        .cell-day {
            text-align: right;
            width: 70px;
        }

        /* Colunas fixas */
        .cell-info {
            text-align: left;
            min-width: 150px;
        }
    </style>

    <script>
        // --- GERENCIADOR DE REQUISIÇÃO ---
        var prm = Sys.WebForms.PageRequestManager.getInstance();

        prm.add_endRequest(function () {
            aplicarMascaras();

            // Restaura a aba
            var tabAtual = $('#<%= hfTabSelecionada.ClientID %>').val();
            if (tabAtual) {
                $('.nav-tabs a[href="' + tabAtual + '"]').tab('show');
            }

            var tabAtual2 = $('#<%= hfTabSelecionada2.ClientID %>').val();
            if (tabAtual2) {
                $('.nav-tabs a[href="' + tabAtual2 + '"]').tab('show');
            }

            // SE O MODAL SUMIU MAS A TELA TÁ TRAVADA:
            // Verifica se não tem nenhum modal com a classe 'in' (aberto)
            if ($('.modal.in').length === 0) {
                limparBackdropForce();
            }
        });

        function pageLoad() {
            aplicarMascaras();

            // Setup aba inicial
            var tabAtual = $('#<%= hfTabSelecionada.ClientID %>').val();
            if (tabAtual) {
                $('.nav-tabs a[href="' + tabAtual + '"]').tab('show');
            }

            var tabAtual2 = $('#<%= hfTabSelecionada2.ClientID %>').val();
            if (tabAtual2) {
                $('.nav-tabs a[href="' + tabAtual2 + '"]').tab('show');
            }

            $('a[data-toggle="tab"]').on('shown.bs.tab', function (e) {
                $('#<%= hfTabSelecionada.ClientID %>').val($(e.target).attr("href"));
                $('#<%= hfTabSelecionada2.ClientID %>').val($(e.target).attr("href"));

            });

            // Re-bind Dropdown
            $('#<%= ddlsTipoCompra.ClientID %>').off('change').on('change', function () {
                var idTipo = $(this).val();
                if (idTipo != "0") {
                    $.ajax({
                        type: "POST",
                        url: "Relatorio_Despesas_Detalhe.aspx/BuscarLimitePorTipo",
                        data: JSON.stringify({ idTipo: idTipo }),
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (response) {
                            $('#<%= txtnValorLimite.ClientID %>').val(response.d).trigger('input');
                        }
                    });
                }
            });

            configurarRegrasDinamicas();
        }

        function aplicarMascaras() {
            $('#<%= txtnValorLimite.ClientID %>').mask('000.000.000,00', { reverse: true });
            $('#<%= txtQtdDiasRegra.ClientID %>').mask('000');
            $('#<%= txtValorLancamento.ClientID %>').mask('000.000.000,00', { reverse: true });
        }

        // --- FUNÇÕES DE MODAL ---
        function abrirModalRegra() { $('#modalRegra').modal('show'); }
        function abrirModalLancamento() { $('#modalLancamento').modal('show'); }

        function fecharModalRegra() {
            $('#modalRegra').modal('hide');
            limparBackdropForce();
        }

        function fecharModalLancamento() {
            $('#modalLancamento').modal('hide');
            limparBackdropForce();
        }

        // LIMPEZA "NUCLEAR" DO BACKDROP
        function limparBackdropForce() {
            // Remove classes e elementos imediatamente
            $('body').removeClass('modal-open').css('padding-right', '');
            $('.modal-backdrop').remove();

            // Repete após 300ms para garantir (caso o UpdatePanel recrie o backdrop)
            setTimeout(function () {
                $('body').removeClass('modal-open').css('padding-right', '');
                $('.modal-backdrop').remove();
            }, 300);
        }

        function abrirModalArquivos() {
            // Tenta abrir usando jQuery (padrão antigo do Bootstrap 3/4) ou Vanilla (Bootstrap 5)
            try {
                $('#modalArquivos').modal('show');
            } catch (e) {
                var myModal = new bootstrap.Modal(document.getElementById('modalArquivos'));
                myModal.show();
            }
        }

        function fecharModalArquivos() {
            $('#modalArquivos').modal('hide');
            limparBackdropForce();
        }

        function ForcarEsconderLoading() {
            // A Master Page tem um timer de 450ms para mostrar o loading.
            // Nós esperamos 1000ms (1 segundo) para garantir que ele apareceu,
            // e então forçamos ele a sumir, já que a página não vai recarregar (é download).
            setTimeout(function () {
                $('[id*=div_Loading]').hide();
                $('[id*=div_img]').hide();
            }, 1000);
        }

        // --- LÓGICA DE REGRAS (CLIENT SIDE) ---
        function configurarRegrasDinamicas() {
            // Mapeia os elementos (usando ClientID do ASP.NET)
            var $ddlFreq = $('#<%= ddlFrequencia.ClientID %>');
            var $txtDias = $('#<%= txtQtdDiasRegra.ClientID %>');
            var $txtValor = $('#<%= txtnValorLimite.ClientID %>');
            var $lblTotal = $('#lblTotalCalculado');

            // Função interna para Calcular
            function calcularTotal() {
                // Limpa msg
                $lblTotal.text('');

                // Só calcula se for Diário
                if ($ddlFreq.val() === 'Diario') {
                    var dias = parseInt($txtDias.val()) || 0;

                    // Converte "1.000,00" para float JS (1000.00)
                    var valorStr = $txtValor.val().replace(/\./g, '').replace(',', '.');
                    var valor = parseFloat(valorStr) || 0;

                    if (dias > 0 && valor > 0) {
                        var total = dias * valor;
                        // Formata para Real BRL
                        var totalFormatado = total.toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });

                        $lblTotal.html('<br/><i class="fa fa-calculator"></i> Total: <b>' + totalFormatado + '</b>');
                    }
                }
            }

            // Função interna para Bloquear "Avulso"
            function verificarAvulso() {
                var tipo = $ddlFreq.val();

                if (tipo === 'Avulso') {
                    // Define 1 e bloqueia
                    $txtDias.val('1').prop('readonly', true).css('background-color', '#eee');
                    $lblTotal.text(''); // Limpa cálculo pois avulso o total é o próprio valor
                } else {
                    // Libera
                    $txtDias.prop('readonly', false).css('background-color', '#fff');
                    calcularTotal(); // Recalcula se tiver dados
                }
            }

            // --- BIND DOS EVENTOS ---
            // Usa .off() antes de .on() para evitar duplicidade de eventos no UpdatePanel
            $ddlFreq.off('change').on('change', verificarAvulso);
            $txtDias.off('input keyup').on('input keyup', calcularTotal);
            $txtValor.off('input keyup').on('input keyup', calcularTotal);

            // Roda uma vez ao carregar para garantir o estado inicial
            verificarAvulso();
        }
    </script>
</asp:Content>

<asp:Content ID="content_frmCliente_Detalhe" ContentPlaceHolderID="cphCorpo" runat="server">

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Relatório de Despesas"></asp:Label>

                <small class="text-muted">Previsões e Lançamentos</small>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Novo" />
        </div>
    </div>

    <asp:UpdatePanel ID="updGeral" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <asp:HiddenField ID="hfTabSelecionada" runat="server" />
            <asp:HiddenField ID="hfTabSelecionada2" runat="server" />

            <ul class="nav nav-tabs" role="tablist">
                <li role="presentation" class="active">
                    <a href="#tabDefinicoes" aria-controls="tabDefinicoes" role="tab" data-toggle="tab">
                        <i class="fa fa-list-alt"></i>&nbsp Definições
                    </a>
                </li>
                <li role="presentation" id="tabDespesa" runat="server">
                    <a href="#tabRelatorio" aria-controls="tabRelatorio" role="tab" data-toggle="tab">
                        <i class="fa fa-dashboard"></i>&nbsp Relatório de Despesas
                    </a>
                </li>
                <li role="presentation">
                    <a href="#tabHistorico" aria-controls="tabHistorico" role="tab" data-toggle="tab">
                        <i class="fa fa-list-alt"></i>&nbsp Histórico
                     </a>
                </li>
            </ul>

            <div class="tab-content">
                <div role="tabpanel" class="tab-pane active" id="tabDefinicoes">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>1. Definições do Relatório</b></h3>
                        </div>
                        <div class="panel-body">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina_Entregas" />

                            <div class="row">
                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <label>Data Início Vigência</label>
                                        <asp:TextBox ID="txtDtInicio" runat="server" TextMode="Date" class="form-control"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-4">
                                    &nbsp
                                </div>
                                <div class="col-lg-4">
                                    <label>&nbsp</label>
                                    <div class="form-group">
                                        <asp:Literal ID="litStatusBadge" runat="server"></asp:Literal>
                                    </div>

                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Motivo Geral</label>
                                        <asp:TextBox ID="txtsDscMotivo" class="form-control" runat="server" MaxLength="200" placeholder="Ex: Viagem Comercial Sul"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Colaboradores (Participantes)</label>
                                        <asp:Panel ID="pnlColaborador" runat="server">
                                            <asp:ListBox ID="ddlColaborador" runat="server" SelectionMode="Multiple" class="form-control select Caixa_Selecao"></asp:ListBox>
                                        </asp:Panel>
                                        <%--<asp:ListBox ID="ddlColaborador" runat="server" SelectionMode="Multiple" class="form-control select Caixa_Selecao"></asp:ListBox>--%>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Centro de Custo</label>
                                        <%-- <asp:DropDownList ID="ddlCentroCusto" runat="server" class="form-control select Caixa_Selecao"></asp:DropDownList>--%>
                                        <asp:Panel ID="pnlCentroCusto" runat="server">
                                            <asp:DropDownList ID="ddlCentroCusto" runat="server" class="form-control select Caixa_Selecao"></asp:DropDownList>
                                        </asp:Panel>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Observação</label>
                                        <asp:TextBox ID="txtsObservacao" class="form-control" runat="server" TextMode="MultiLine" Height="60px" placeholder="Instruções adicionais..."></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                    <ul class="nav nav-tabs" role="tablist">
                        <li role="presentation" class="active">
                            <a href="#tabRegras" aria-controls="tabRegras" role="tab" data-toggle="tab">
                                <i class="fa fa-list-alt"></i>&nbsp Previsão de Despesas
                            </a>
                        </li>
                        <li role="presentation" runat="server" id="tabLancamento">
                            <a href="#tabLancamentos" aria-controls="tabLancamentos" role="tab" data-toggle="tab">
                                <i class="fa fa-money"></i>&nbsp Lançamentos de Despesas
                            </a>
                        </li>
                    </ul>

                    <div class="tab-content">

                        <div role="tabpanel" class="tab-pane active" id="tabRegras">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>2.Previsão de Despesas</b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="row" style="margin-bottom: 15px;">
                                        <div class="col-lg-12">
                                            <asp:LinkButton ID="btnAbrirModal" runat="server" CssClass="btn btn-info" OnClick="btnAbrirModal_Click">
                                 <i class="fa fa-plus"></i> Adicionar
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvRegras" runat="server" AutoGenerateColumns="False"
                                            CssClass="table table-striped table-bordered table-hover"
                                            OnRowDeleting="gvRegras_RowDeleting" DataKeyNames="idTipo" OnRowDataBound="gvRegras_RowDataBound">
                                            <Columns>
                                                <asp:BoundField DataField="sDescTipo" HeaderText="Tipo de Despesa" />
                                                <asp:BoundField DataField="sFrequencia" HeaderText="Frequência" />
                                                <asp:BoundField DataField="nDias" HeaderText="Dias" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="nValor" HeaderText="Limite (R$)" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="sTotal" HeaderText="Total Previsto (R$)" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" ItemStyle-Font-Bold="true" />
                                                <asp:TemplateField HeaderText="Ação">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="btnExcluirRegra" runat="server" CommandName="Delete" CssClass="btn btn-danger btn-xs" ToolTip="Remover Regra">
                                             <i class="fa fa-trash-o"></i>
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="50px" HorizontalAlign="Left" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <div class="empty-grid-msg"><i class="fa fa-exclamation-circle"></i>Nenhuma regra definida.</div>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <div role="tabpanel" class="tab-pane" id="tabLancamentos">
                            <div class="panel panel-default">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><b>3. Lançamentos Realizados</b></h3>
                                </div>
                                <div class="panel-body">
                                    <div class="row" style="margin-bottom: 15px;">
                                        <div class="col-lg-12">
                                            <asp:LinkButton ID="btnNovoLancamento" runat="server" CssClass="btn btn-success" OnClick="btnNovoLancamento_Click">
                                 <i class="fa fa-money"></i> Novo Lançamento
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                    <div class="table-responsive">
                                        <asp:GridView ID="gvLancamentos" runat="server" AutoGenerateColumns="False"
                                            CssClass="table table-striped table-bordered table-hover"
                                            OnRowCommand="gvLancamentos_RowCommand" DataKeyNames="idItens, idTipoGastos" OnRowDataBound="gvLancamentos_RowDataBound">
                                            <Columns>
                                                <asp:BoundField DataField="sNomeColaborador" HeaderText="Colaborador" />
                                                <asp:BoundField DataField="sDscCartoes" HeaderText="Cartão" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="dtDespesa" HeaderText="Data" DataFormatString="{0:dd/MM/yyyy}" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />
                                                <asp:BoundField DataField="sDescTipo" HeaderText="Tipo" />
                                                <asp:BoundField DataField="sLocal" HeaderText="Local / Descrição" />
                                                <asp:BoundField DataField="nValor" HeaderText="Valor (R$)" HeaderStyle-HorizontalAlign="Left" ItemStyle-HorizontalAlign="Left" />

                                                <asp:TemplateField HeaderText="Comprovante" ItemStyle-HorizontalAlign="Center" HeaderStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <%# ObterIconeArquivo(Eval("idItens")) %>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="80px" />
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Ação">
                                                    <ItemTemplate>
                                                        <div style="white-space: nowrap;">

                                                            <asp:LinkButton ID="btnArquivos" runat="server"
                                                                CommandName="Arquivos"
                                                                CommandArgument='<%# Eval("idItens") %>'
                                                                CssClass="btn btn-info btn-xs"
                                                                ToolTip="Gerenciar Anexos"
                                                                Style="margin-right: 5px;">
                            <i class="fa fa-paperclip"></i>
                                                            </asp:LinkButton>

                                                            <asp:LinkButton ID="btnExcluirLanc" runat="server"
                                                                CommandName="ExcluirLanc"
                                                                CommandArgument='<%# Eval("idItens") %>'
                                                                CssClass="btn btn-danger btn-xs"
                                                                ToolTip="Remover Lançamento">
                            <i class="fa fa-trash-o"></i>
                                                            </asp:LinkButton>

                                                        </div>

                                                    </ItemTemplate>
                                                    <ItemStyle Width="50px" HorizontalAlign="Center" />
                                                </asp:TemplateField>
                                            </Columns>
                                            <EmptyDataTemplate>
                                                <div class="empty-grid-msg"><i class="fa fa-info-circle"></i>Nenhum lançamento realizado ainda.</div>
                                            </EmptyDataTemplate>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>

                <div role="tabpanel" class="tab-pane" id="tabRelatorio">

                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>3. Relatório Consolidado (Saldo Diário)</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="row" style="margin-bottom: 20px;">
                                <div class="col-lg-12">
                                    <span class="label label-success" style="margin-right: 5px;">Verde: Saldo Positivo</span>
                                    <span class="label label-warning" style="margin-right: 5px;">Amarelo: Saldo Positivo (com estouros diários)</span>
                                    <span class="label label-danger">Vermelho: Saldo Negativo</span>
                                </div>
                            </div>

                            <div class="table-responsive">
                                <asp:Literal ID="litRelatorioDinamic" runat="server"></asp:Literal>
                            </div>
                        </div>
                    </div>

                </div>

                <div role="tabpanel" class="tab-pane" id="tabHistorico">

                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>4. Histórico de Alterações</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive">
                                <asp:GridView ID="gvHistorico" runat="server" AutoGenerateColumns="False"
                                    CssClass="table table-striped table-bordered table-hover">
                                    <Columns>
                                        <asp:BoundField DataField="dtStatusFormatada" HeaderText="Data/Hora" ItemStyle-Width="150px" />
                                        <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário" ItemStyle-Width="200px" />
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <span class='label label-<%# Eval("sCor") %>'><%# Eval("sStatus") %></span>
                                            </ItemTemplate>
                                            <ItemStyle Width="100px" HorizontalAlign="Center" />
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="sDscMotivo" HeaderText="Ação / Detalhes" HtmlEncode="false" />
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div class="empty-grid-msg"><i class="fa fa-info-circle"></i>Nenhum histórico registrado.</div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>

                </div>

            </div>



            <div class="row">
                <div class="col-lg-12">
                    <div class="actions-footer">
                        <asp:Button ID="btnSalvarBase" runat="server" CssClass="btn btn-lg btn-success" Text="Salvar" OnClick="btnSalvarBase_Click" />

                        <div class="btn-group dropup" id="divAcoes" runat="server" visible="false">
                            <button type="button" class="btn btn-lg btn-primary dropdown-toggle" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                <i class="fa fa-cogs"></i>Ações <span class="caret"></span>
                            </button>
                            <ul class="dropdown-menu">
                                <li>
                                    <asp:LinkButton ID="btnAcaoFinalizar" runat="server" OnClick="btnAcao_Click" CommandArgument="2">
                            <i class="fa fa-flag-checkered text-primary"></i> Finalizar
                                    </asp:LinkButton>
                                </li>
                                <li role="separator" class="divider"></li>
                                <li>
                                    <asp:LinkButton ID="btnAcaoAprovar" runat="server" OnClick="btnAcao_Click" CommandArgument="3">
                            <i class="fa fa-check text-success"></i> Aprovar
                                    </asp:LinkButton>
                                </li>
                                <li>
                                    <asp:LinkButton ID="btnAcaoRejeitar" runat="server" OnClick="btnAcao_Click" CommandArgument="4">
                            <i class="fa fa-times text-danger"></i> Rejeitar
                                    </asp:LinkButton>
                                </li>
                            </ul>
                        </div>

                        <asp:Button ID="btnVoltar" class="btn btn-lg btn-warning" runat="server" Text="Voltar" OnClick="btnVoltar_Click" />
                    </div>
                </div>
            </div>

            <div class="modal fade" id="modalRegra" tabindex="-1" role="dialog" aria-labelledby="modalRegraLabel" aria-hidden="true" data-backdrop="static">
                <div class="modal-dialog" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title" id="modalRegraLabel">Nova Previsão de Despesa</h4>
                        </div>
                        <div class="modal-body">
                            <uc1:MensagemPagina runat="server" ID="MensagemModal" />
                            <div class="row">
                                <div class="col-lg-12">
                                    <div class="form-group">
                                        <label>Tipo de Despesa</label>
                                        <asp:DropDownList ID="ddlsTipoCompra" runat="server" class="form-control select Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="row">
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Frequência</label>
                                        <asp:DropDownList ID="ddlFrequencia" runat="server" class="form-control select Caixa_Selecao">
                                            <asp:ListItem Value="Diario">Diário (x Dias)</asp:ListItem>
                                            <asp:ListItem Value="Avulso">Avulso (Valor Único)</asp:ListItem>
                                            <%--<asp:ListItem Value="Mensal">Mensal</asp:ListItem>--%>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <label>Qtd. Dias</label>
                                        <asp:TextBox ID="txtQtdDiasRegra" runat="server" class="form-control" placeholder="Ex: 5"></asp:TextBox>
                                        <small class="text-muted">Se avulso, deixe 1.</small>
                                    </div>
                                </div>
                            </div>
                            <div class="row">

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label style="color: green; font-weight: bold;">Valor Limite (R$)</label>
                                        <span id="lblTotalCalculado" style="font-size: 11px; color: #555; margin-left: 5px;"></span>

                                        <asp:TextBox ID="txtnValorLimite" class="form-control" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                            </div>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="btnFecharModal" runat="server" Text="Cancelar" CssClass="btn btn-default" OnClick="btnFecharModal_Click" />
                            <asp:Button ID="btnAdicionarRegra" runat="server" Text="Adicionar Previsão" CssClass="btn btn-primary" OnClick="btnAdicionarRegra_Click" />
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="modalLancamento" tabindex="-1" role="dialog" aria-labelledby="modalLancLabel" aria-hidden="true" data-backdrop="static">
                <div class="modal-dialog modal-lg" role="document">
                    <div class="modal-content">
                        <div class="modal-header" style="background-color: #dff0d8;">
                            <h4 class="modal-title" id="modalLancLabel">
                                <asp:Label ID="lblTituloModal" runat="server" Text="Lançar Despesa"></asp:Label>
                            </h4>
                        </div>
                        <div class="modal-body">
                            <uc1:MensagemPagina runat="server" ID="MensagemModalLanc" />

                            <asp:Panel ID="pnlDadosDespesa" runat="server">
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>Colaborador</label>
                                            <asp:DropDownList ID="ddlUsuarioLancamento" runat="server" autopostback="true" class="form-control select Caixa_Selecao" OnSelectedIndexChanged="ddlUsuarioLancamento_SelectedIndexChanged"></asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Data</label>
                                            <asp:TextBox ID="txtDataLancamento" runat="server" TextMode="Date" class="form-control"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label>Tipo</label>
                                            <asp:DropDownList ID="ddlTipoLancamento" runat="server" class="form-control select Caixa_Selecao"></asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-lg-4">
                                        <div class="form-group">
                                            <label class="form-label">Cartão</label>
                                            <asp:DropDownList ID="ddlidCartao" runat="server"  CssClass="form-control select Caixa_Selecao">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-12">
                                        <div class="form-group">
                                            <label>Local / Descrição</label>
                                            <asp:TextBox ID="txtLocalLancamento" runat="server" class="form-control" MaxLength="100"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-lg-2">
                                        <div class="form-group">
                                            <label style="color: green; font-weight: bold;">Valor Gasto (R$)</label>
                                            <asp:TextBox ID="txtValorLancamento" class="form-control" runat="server"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </asp:Panel>

                            <hr />

                            <h4><i class="fa fa-paperclip"></i>Anexos</h4>
                            <uc1:MensagemPagina runat="server" ID="MensagemModalArquivos" />

                            <div class="well well-sm" style="background: #f9f9f9; padding: 10px; margin-bottom: 15px;">
                                <div class="row">
                                    <div class="col-md-5">
                                        <label>Arquivo</label>
                                        <asp:FileUpload ID="fuArquivo" runat="server" CssClass="form-control" />
                                    </div>
                                    <div class="col-md-5">
                                        <label>Descrição</label>
                                        <asp:TextBox ID="txtDescArquivo" runat="server" CssClass="form-control" placeholder="Ex: Nota Fiscal"></asp:TextBox>
                                    </div>
                                    <div class="col-md-2" style="padding-top: 25px;">
                                        <asp:LinkButton ID="btnAdicionarArquivoLista" runat="server" CssClass="btn btn-primary btn-sm btn-block" OnClick="btnAdicionarArquivoLista_Click">
                                <i class="fa fa-plus"></i> Adicionar
                                        </asp:LinkButton>
                                    </div>
                                </div>
                            </div>

                            <div class="table-responsive">
                                <asp:GridView ID="gvArquivos" runat="server" AutoGenerateColumns="False" CssClass="table table-striped table-bordered table-condensed"
                                    OnRowCommand="gvArquivos_RowCommand"
                                    OnRowDataBound="gvArquivos_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="Nome" HeaderText="Arquivo" />
                                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                                        <asp:TemplateField HeaderText="Ação" ItemStyle-Width="50px" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>

                                                <div style="white-space: nowrap;">

                                                    <%-- BOTÃO DOWNLOAD COM O TRUQUE JS --%>
                                                    <asp:LinkButton ID="btnDownload" runat="server"
                                                        CommandName="Download"
                                                        CommandArgument='<%# Eval("IdTemporario") %>'
                                                        OnClientClick="ForcarEsconderLoading();"
                                                        CssClass="btn btn-xs btn-primary"
                                                        ToolTip="Baixar / Visualizar"
                                                        Style="margin-right: 5px;">
                        <i class="fa fa-download"></i>
                                                    </asp:LinkButton>

                                                    <asp:LinkButton ID="btnRemoverTemp" runat="server"
                                                        CommandName="RemoverTemp"
                                                        CommandArgument='<%# Eval("IdTemporario") %>'
                                                        CssClass="btn btn-xs btn-danger"
                                                        ToolTip="Remover">
                        <i class="fa fa-trash-o"></i>
                                                    </asp:LinkButton>

                                                </div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                    </Columns>
                                    <EmptyDataTemplate>
                                        <div class="text-center text-muted p-2">Nenhum arquivo anexado.</div>
                                    </EmptyDataTemplate>
                                </asp:GridView>
                            </div>
                        </div>

                        <div class="modal-footer">
                            <asp:Button ID="btnCancelarLanc" runat="server" Text="Cancelar" CssClass="btn btn-default" OnClick="btnCancelarLanc_Click" />

                            <asp:Button ID="btnSalvarLancamento" runat="server" Text="Efetivar Lançamento" CssClass="btn btn-success" OnClick="btnSalvarLancamento_Click" />

                            <asp:Button ID="btnSalvarArquivosBanco" runat="server" Text="Salvar Anexos" CssClass="btn btn-info" OnClick="btnSalvarArquivosBanco_Click" Visible="false" />
                        </div>
                    </div>
                </div>
            </div>


        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="btnSalvarLancamento" />
            <asp:PostBackTrigger ControlID="btnAdicionarArquivoLista" />
            <asp:PostBackTrigger ControlID="btnSalvarArquivosBanco" />
            <%--<asp:AsyncPostBackTrigger ControlID="ddlUsuarioLancamento" EventName="SelectedIndexChanged" />--%>
        </Triggers>
    </asp:UpdatePanel>
</asp:Content>
