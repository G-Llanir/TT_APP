<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Workflows.aspx.cs" Inherits="TT_Flow.App.Paginas.IA.Workflows" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .wf-exec-json {
            font-family: Consolas, Monaco, monospace;
            min-height: 120px;
            resize: vertical;
        }

        .wf-mono {
            font-family: Consolas, Monaco, monospace;
        }

        .wf-exec-input-row {
            display: flex;
            gap: 6px;
            align-items: center;
            margin-bottom: 6px;
        }

        .wf-exec-input-row label {
            width: 150px;
            margin: 0;
            font-family: Consolas, Monaco, monospace;
            font-size: 12px;
            font-weight: normal;
        }

        .wf-exec-input-row input {
            flex: 1;
        }

        .wf-exec-input-field {
            flex: 1;
            min-width: 0;
        }

        .wf-exec-input-field .help-block {
            margin: 2px 0 0;
            font-size: 11px;
            color: #718291;
        }

        .wf-exec-group-title {
            margin: 8px 0 5px;
            padding-top: 6px;
            border-top: 1px solid #e3eaf2;
            color: #34495e;
            font-weight: bold;
            font-size: 12px;
        }

        .wf-exec-group-title:first-child {
            margin-top: 0;
            padding-top: 0;
            border-top: 0;
        }

        .wf-exec-conditional-inputs {
            margin-top: 10px;
            border: 1px dashed #b7c8d9;
            border-radius: 6px;
            padding: 8px 10px;
            background: #fbfcfd;
        }

        .wf-exec-conditional-inputs summary {
            cursor: pointer;
            color: #415466;
        }

        .wf-exec-runtime-fields {
            margin-top: 8px;
        }

        .wf-exec-escolhas {
            display: grid;
            gap: 6px;
            margin: 10px 0;
        }

        .wf-exec-escolha {
            display: flex;
            align-items: flex-start;
            gap: 9px;
            margin: 0;
            padding: 9px 10px;
            border: 1px solid #d9c88a;
            border-radius: 5px;
            background: #fffdf5;
            cursor: pointer;
            font-weight: normal;
        }

        .wf-exec-escolha:hover {
            border-color: #b7952e;
            background: #fff9df;
        }

        .wf-exec-escolha input {
            flex: 0 0 auto;
            margin-top: 3px;
        }

        .wf-exec-escolha span,
        .wf-exec-escolha strong,
        .wf-exec-escolha small {
            display: block;
            min-width: 0;
            overflow-wrap: anywhere;
        }

        .wf-exec-escolha small {
            margin-top: 2px;
            color: #66727c;
        }

        .wf-permissao-id {
            display: inline-block;
            min-width: 42px;
            margin-right: 6px;
            padding: 2px 6px;
            border-radius: 3px;
            background: #f3f6f9;
            color: #415466;
            font-family: Consolas, Monaco, monospace;
            font-size: 11px;
        }

        .wf-permissao-desc {
            color: #2f4050;
        }

        .wf-exec-trace-passo {
            border: 1px solid #ddd;
            border-radius: 4px;
            margin-bottom: 8px;
            padding: 8px 10px;
            background: #fff;
        }

        .wf-exec-noid {
            display: inline-block;
            min-width: 56px;
            padding: 2px 6px;
            border-radius: 3px;
            background: #f5f5f5;
            color: #555;
            font-family: Consolas, Monaco, monospace;
            font-size: 11px;
        }

        .wf-exec-io {
            margin-top: 8px;
            padding: 8px;
            background: #f7f7f7;
            border: 1px solid #e5e5e5;
            border-radius: 3px;
            white-space: pre-wrap;
            word-break: break-word;
            font-family: Consolas, Monaco, monospace;
            font-size: 12px;
        }

        .wf-espera-actions {
            display: flex;
            flex-wrap: wrap;
            gap: 6px;
            align-items: center;
        }

        .wf-espera-actions .input-group {
            max-width: 340px;
        }

        #wfEsperaStatus {
            display: none;
            margin-top: 7px;
            margin-bottom: 0;
            padding: 6px 10px;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <asp:UpdatePanel ID="updWorkflows" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Workflows IA"></asp:Label><small> Automação</small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Workflows IA" />
                </div>

                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </div>


                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-random"></i> Workflows registrados</h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <a href="Workflow_Detalhe.aspx?id=0" class="btn btn-sm btn-success"><i class="fa fa-plus"></i> Novo workflow</a>
                                <button type="button" class="btn btn-sm btn-default" onclick="wfAbrirImportarWorkflow()"><i class="fa fa-upload"></i> Importar workflow</button>
                                <button type="button" class="btn btn-sm btn-default" onclick="wfAbrirArquivados()"><i class="fa fa-archive"></i> Arquivados</button>
                                <span class="text-muted padd-l">Monte o fluxo, teste passo a passo e ative quando estiver pronto.</span>
                            </div>
                            <div class="form-group wf-espera-actions">
                                <button type="button" class="btn btn-sm btn-default" id="wfBtnProcessarEsperas" onclick="wfExecProcessarEsperas()">
                                    <i class="fa fa-clock-o"></i> Processar esperas vencidas
                                </button>
                                <div class="input-group input-group-sm">
                                    <input type="text" class="form-control wf-mono" id="wfEventoEspera" placeholder="nome_do_evento" />
                                    <span class="input-group-btn">
                                        <button type="button" class="btn btn-default" id="wfBtnLiberarEvento" onclick="wfExecLiberarEvento()">
                                            <i class="fa fa-bolt"></i> Liberar evento
                                        </button>
                                    </span>
                                </div>
                                <span class="text-muted">Afeta somente execuções pausadas do usuário atual.</span>
                            </div>
                            <div id="wfEsperaStatus" class="alert"></div>
                            <div class="table-responsive">
                                <asp:GridView ID="dtgvWorkflows" CssClass="table table-striped table-bordered table-hover"
                                    runat="server" Width="100%" AutoGenerateColumns="False" GridLines="None"
                                    Font-Names="Tahoma" Font-Size="Small" DataKeyNames="IdWorkflowIA"
                                    EmptyDataText="Nenhum workflow criado ainda."
                                    OnRowDataBound="dtgvWorkflows_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="Nome" HeaderText="Nome interno" />
                                        <asp:BoundField DataField="Descricao" HeaderText="Descrição" />
                                        <asp:TemplateField HeaderText="Permissão">
                                            <ItemTemplate>
                                                <%# PermissaoWorkflowHtml(Eval("IdRecursoNecessario"), Eval("RecursoDescricao")) %>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="Escopo" HeaderText="Tipo" />
                                        <asp:TemplateField HeaderText="Ativo" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <asp:CheckBox ID="chkAtivo" runat="server" AutoPostBack="true"
                                                    OnCheckedChanged="chkAtivo_CheckedChanged"
                                                    Checked='<%# (bool)Eval("Ativo") %>' />
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="">
                                            <ItemTemplate>
                                                <asp:PlaceHolder runat="server" Visible='<%# PodeExecutarWorkflow(Eval("Ativo"), Eval("GrafoJson"), Eval("Escopo")) %>'>
                                                    <button type="button" class="btn btn-info btn-xs" title="Executar workflow"
                                                        data-id='<%# Eval("IdWorkflowIA") %>'
                                                        data-nome='<%# System.Web.HttpUtility.HtmlAttributeEncode(System.Convert.ToString(Eval("Nome"))) %>'
                                                        data-entradas='<%# EntradasWorkflowAttr(Eval("GrafoJson")) %>'
                                                        onclick="wfExecAbrir(this)">
                                                        <i class="fa fa-play"></i> Executar
                                                    </button>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%# WorkflowAtivo(Eval("Ativo")) %>'>
                                                    <asp:HyperLink runat="server" CssClass="btn btn-default btn-xs" ToolTip="Workflow Editor"
                                                        NavigateUrl='<%# "Workflow_Detalhe.aspx?id=" + Eval("IdWorkflowIA") %>'><i class="fa fa-pencil"></i> Workflow Editor</asp:HyperLink>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%# !WorkflowAtivo(Eval("Ativo")) %>'>
                                                    <span class="label label-default">Inativo</span>
                                                </asp:PlaceHolder>
                                                <asp:PlaceHolder runat="server" Visible='<%# PodeArquivarWorkflow(Eval("Ativo")) %>'>
                                                    <button type="button" class="btn btn-default btn-xs" title="Arquivar workflow (some da lista; pode desarquivar depois)"
                                                        data-id='<%# Eval("IdWorkflowIA") %>'
                                                        data-nome='<%# System.Web.HttpUtility.HtmlAttributeEncode(System.Convert.ToString(Eval("Nome"))) %>'
                                                        onclick="wfArquivarWorkflow(this)">
                                                        <i class="fa fa-archive"></i> Arquivar
                                                    </button>
                                                </asp:PlaceHolder>
                                            </ItemTemplate>
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

    <%-- Fora do UpdatePanel para o bootstrap modal não perder estado em postback parcial. --%>
    <div class="modal fade" id="Modal_ExecutarWorkflow">
        <div class="modal-dialog modal-grande">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-title">
                        <label>Executar workflow: <span id="wfExecTitulo"></span></label>
                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span></button>
                    </div>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="wfExecIdWorkflow" value="0" />
                    <input type="hidden" id="wfExecIdExecucao" value="0" />

                    <div class="form-group">
                        <div style="display:flex;align-items:center;gap:8px;margin-bottom:6px;">
                            <label style="margin:0;">Entradas</label>
                            <div class="btn-group btn-group-xs" data-toggle="buttons">
                                <label class="btn btn-default active" onclick="wfExecSetModo('inputs')">
                                    <input type="radio" name="wfExecModo" checked="checked" /> Inputs
                                </label>
                                <label class="btn btn-default" onclick="wfExecSetModo('json')">
                                    <input type="radio" name="wfExecModo" /> JSON
                                </label>
                            </div>
                        </div>
                        <div id="wfExecEntradasCampos"></div>
                        <div id="wfExecJsonBox" style="display:none;">
                            <textarea id="wfExecEntradas" class="form-control wf-exec-json">{}</textarea>
                        </div>
                    </div>

                    <div id="wfExecStatus" class="alert alert-info" style="display:none;"></div>

                    <div id="wfExecAprovacao" class="alert alert-warning" style="display:none;">
                        <p><strong>Pendência do workflow</strong></p>
                        <p><span id="wfExecRotuloAprovacao">Ferramenta:</span> <strong id="wfExecResumoAprovacao"></strong></p>
                        <p class="text-muted" id="wfExecDescricaoAprovacao" style="display:none;margin-top:-6px;"></p>
                        <div id="wfExecEscolhas" class="wf-exec-escolhas" style="display:none;"></div>
                        <button type="button" class="btn btn-warning btn-sm" id="wfExecBtnConfirmar" onclick="wfExecConfirmar()">
                            <i class="fa fa-check"></i> Confirmar passo
                        </button>
                        <button type="button" class="btn btn-default btn-sm" id="wfExecBtnRejeitar" style="display:none;" onclick="wfExecRejeitar()">
                            <i class="fa fa-times"></i> Rejeitar
                        </button>
                        <button type="button" class="btn btn-default btn-sm" id="wfExecBtnCancelar" onclick="wfExecCancelar()">
                            <i class="fa fa-ban"></i> Cancelar execução
                        </button>
                    </div>

                    <div id="wfExecEntradasPendentes" class="alert alert-warning" style="display:none;">
                        <p><strong id="wfExecEntradasTitulo">Entradas pendentes</strong></p>
                        <p class="text-muted" id="wfExecEntradasMensagem">Informe os dados necessários para continuar.</p>
                        <div id="wfExecEntradasPendentesCampos" class="wf-exec-runtime-fields"></div>
                        <button type="button" class="btn btn-primary btn-sm" id="wfExecBtnContinuarEntradas" onclick="wfExecContinuarEntradas()">
                            <i class="fa fa-play"></i> Continuar workflow
                        </button>
                        <button type="button" class="btn btn-default btn-sm" onclick="wfExecCancelar()">
                            <i class="fa fa-ban"></i> Cancelar execução
                        </button>
                    </div>

                    <h4>Trace</h4>
                    <div id="wfExecTrace" class="well well-sm">Nenhuma execução iniciada.</div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>
                    <button type="button" class="btn btn-primary" id="wfExecBtnIniciar" onclick="wfExecIniciar()">
                        <i class="fa fa-play"></i> Executar
                    </button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="Modal_ImportarWorkflow">
        <div class="modal-dialog modal-grande">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-title">
                        <label><i class="fa fa-upload"></i> Importar workflow</label>
                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span></button>
                    </div>
                </div>
                <div class="modal-body">
                    <p class="text-muted">
                        Cole o JSON exportado (botão "Exportar workflow" na edição de um workflow, ou o conteúdo
                        de um arquivo em <code>SQL/IA/Workflows/</code>). Se o nome interno já existir, o
                        workflow é <strong>atualizado</strong> (mesmo id, execuções ligadas a ele continuam
                        válidas); senão, é <strong>criado</strong>.
                    </p>
                    <div class="form-group">
                        <label>Ou escolha o arquivo .json</label>
                        <input type="file" id="wfImportarArquivo" accept=".json,application/json" />
                    </div>
                    <div class="form-group">
                        <textarea id="wfImportarTexto" class="form-control wf-exec-json" rows="14" placeholder='{"versaoExportacao":1,"sNomeInterno":"...", ...}'></textarea>
                    </div>
                    <div id="wfImportarStatus"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Cancelar</button>
                    <button type="button" class="btn btn-primary" id="wfImportarBtn" onclick="wfImportarWorkflowConfirmar()">
                        <i class="fa fa-upload"></i> Importar
                    </button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="Modal_Arquivados">
        <div class="modal-dialog modal-grande">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-title">
                        <label><i class="fa fa-archive"></i> Workflows arquivados</label>
                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span></button>
                    </div>
                </div>
                <div class="modal-body">
                    <p class="text-muted">
                        Arquivar tira o workflow da lista principal sem apagar nada. Desarquivar traz de volta
                        para a lista, ainda inativo (reativar continua sendo o checkbox Ativo, à parte).
                    </p>
                    <div id="wfArquivadosLista"></div>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>
                </div>
            </div>
        </div>
    </div>

    <script>
        (function () {
            var urls = {
                iniciar: '<%= ResolveUrl("~/App/Paginas/IA/Workflows.aspx/IniciarExecucao") %>',
                confirmar: '<%= ResolveUrl("~/App/Paginas/IA/Workflows.aspx/ConfirmarExecucao") %>',
                selecionarOpcao: '<%= ResolveUrl("~/App/Paginas/IA/Workflows.aspx/SelecionarOpcaoExecucao") %>',
                informarEntradas: '<%= ResolveUrl("~/App/Paginas/IA/Workflows.aspx/InformarEntradasExecucao") %>',
                cancelar: '<%= ResolveUrl("~/App/Paginas/IA/Workflows.aspx/CancelarExecucao") %>',
                rejeitar: '<%= ResolveUrl("~/App/Paginas/IA/Workflows.aspx/RejeitarExecucao") %>',
                processarEsperas: '<%= ResolveUrl("~/App/Paginas/IA/Workflows.aspx/ProcessarEsperasAgendadas") %>',
                liberarEvento: '<%= ResolveUrl("~/App/Paginas/IA/Workflows.aspx/LiberarEventoEspera") %>',
                importar: '<%= ResolveUrl("~/App/Paginas/IA/Workflows.aspx/ImportarWorkflow") %>',
                arquivar: '<%= ResolveUrl("~/App/Paginas/IA/Workflows.aspx/ArquivarWorkflow") %>',
                desarquivar: '<%= ResolveUrl("~/App/Paginas/IA/Workflows.aspx/DesarquivarWorkflow") %>',
                listarArquivados: '<%= ResolveUrl("~/App/Paginas/IA/Workflows.aspx/ListarWorkflowsArquivados") %>'
            };
            var wfExecEntradasDefs = [];
            var wfExecEntradasPendentesDefs = [];
            var wfExecModo = 'inputs';
            var wfExecTipoPausa = '';

            function esc(valor) {
                return String(valor == null ? '' : valor)
                    .replace(/&/g, '&amp;')
                    .replace(/</g, '&lt;')
                    .replace(/>/g, '&gt;')
                    .replace(/"/g, '&quot;')
                    .replace(/'/g, '&#39;');
            }

            function bonito(valor) {
                if (!valor) return '';
                try { return JSON.stringify(JSON.parse(valor), null, 2); }
                catch (e) { return String(valor); }
            }

            function traceCampo(t, nome) {
                t = t || {};
                if (nome === 'Entrada') return t.EntradaCompleta || t.entradaCompleta || t.Entrada || t.entrada || '';
                if (nome === 'Saida') return t.SaidaCompleta || t.saidaCompleta || t.Saida || t.saida || '';
                return '';
            }

            function boolTrace(valor) {
                return valor === true || valor === 1 || valor === '1' || String(valor || '').toLowerCase() === 'true' || String(valor || '').toUpperCase() === 'S';
            }

            function erroTratadoTrace(t) {
                return boolTrace(t && (t.ErroTratado || t.erroTratado));
            }

            function statusTraceLabel(t) {
                var st = String((t && (t.Status || t.status)) || '').toUpperCase();
                return st === 'ERRO' && erroTratadoTrace(t) ? 'ERRO TRATADO' : st;
            }

            function classeTraceLabel(t) {
                var st = String((t && (t.Status || t.status)) || '').toUpperCase();
                if (st === 'OK') return 'success';
                if (st === 'PAUSADO') return 'warning';
                if (st === 'ERRO' && erroTratadoTrace(t)) return 'warning';
                return 'danger';
            }

            function valorPadraoEntrada(tipo) {
                return tipo === 'integer' || tipo === 'number' ? '0' : '';
            }

            function normalizarObrigatoriedade(valor) {
                valor = String(valor || 'obrigatorio').toLowerCase();
                if (valor === 'obrigatoria') valor = 'obrigatorio';
                return valor === 'opcional' || valor === 'condicional' ? valor : 'obrigatorio';
            }

            function entradaEhInicial(e) {
                return normalizarObrigatoriedade(e && e.obrigatoriedade) !== 'condicional';
            }

            function entradaEhObrigatoria(e) {
                return normalizarObrigatoriedade(e && e.obrigatoriedade) === 'obrigatorio';
            }

            function entradasObjetoDosCampos() {
                var obj = {};
                $('#wfExecEntradasCampos .wf-exec-input').each(function () {
                    var $input = $(this);
                    var nome = String($input.data('nome') || '');
                    var tipo = String($input.data('tipo') || 'string');
                    var uso = String($input.data('uso') || 'obrigatorio');
                    var valor = $input.val();
                    if ((uso === 'opcional' || uso === 'condicional') && $.trim(valor) === '') return;

                    if (tipo === 'integer') {
                        obj[nome] = parseInt(valor || '0', 10) || 0;
                    } else if (tipo === 'number') {
                        obj[nome] = parseFloat(String(valor || '0').replace(',', '.')) || 0;
                    } else {
                        obj[nome] = valor || '';
                    }
                });
                return obj;
            }

            function aplicarJsonNosCampos(obj) {
                if (!obj || typeof obj !== 'object') return;

                $('#wfExecEntradasCampos .wf-exec-input').each(function () {
                    var $input = $(this);
                    var nome = String($input.data('nome') || '');
                    if (Object.prototype.hasOwnProperty.call(obj, nome)) {
                        $input.val(obj[nome] == null ? '' : obj[nome]);
                    }
                });
            }

            function renderizarEntradasCampos() {
                if (!wfExecEntradasDefs.length) {
                    $('#wfExecEntradasCampos').html('<span class="help-block">Este workflow não declarou entradas.</span>');
                    $('#wfExecEntradas').val('{}');
                    return;
                }

                var iniciais = [];
                var condicionais = [];
                wfExecEntradasDefs.forEach(function (e) {
                    if (entradaEhInicial(e)) iniciais.push(e);
                    else condicionais.push(e);
                });

                function campoHtml(e, condicional) {
                    var nome = String(e.nome || '');
                    var tipo = String(e.tipo || 'string');
                    var uso = normalizarObrigatoriedade(e.obrigatoriedade);
                    var valor = condicional ? '' : valorPadraoEntrada(tipo);
                    return '<div class="wf-exec-input-row">' +
                        '<label title="' + esc(tipo) + '">' + esc(nome) + (entradaEhObrigatoria(e) ? ' <span class="text-danger">*</span>' : '') + '</label>' +
                        '<div class="wf-exec-input-field">' +
                            '<input type="text" class="form-control input-sm wf-exec-input" data-nome="' + esc(nome) + '" data-tipo="' + esc(tipo) + '" data-uso="' + esc(uso) + '" value="' + esc(valor) + '" oninput="wfExecAtualizarJsonDosCampos()" />' +
                            (e.descricao ? '<span class="help-block">' + esc(e.descricao) + '</span>' : '') +
                        '</div>' +
                        '</div>';
                }

                var html = '';
                var grupoAtual = '';
                iniciais.forEach(function (e) {
                    var grupo = String(e.grupo || 'Geral');
                    if (grupo !== grupoAtual) {
                        grupoAtual = grupo;
                        html += '<div class="wf-exec-group-title">' + esc(grupoAtual) + '</div>';
                    }
                    html += campoHtml(e, false);
                });
                if (condicionais.length) {
                    var grupos = {};
                    condicionais.forEach(function (e) {
                        var grupo = String(e.grupo || 'Geral');
                        if (!grupos[grupo]) grupos[grupo] = [];
                        grupos[grupo].push(e);
                    });
                    html += '<details class="wf-exec-conditional-inputs">' +
                        '<summary><strong>Entradas condicionais</strong> <span class="text-muted">(' + condicionais.length + ' campo(s), pedidos apenas se necessário)</span></summary>' +
                        '<p class="text-muted">Preencha antes se quiser. Se ficarem em branco, o workflow vai pedir quando chegar ao nó que precisa desses dados.</p>';
                    Object.keys(grupos).forEach(function (grupo) {
                        html += '<div class="wf-exec-group-title">' + esc(grupo) + '</div>';
                        grupos[grupo].forEach(function (e) { html += campoHtml(e, true); });
                    });
                    html += '</details>';
                }
                $('#wfExecEntradasCampos').html(html);
                $('#wfExecEntradas').val(JSON.stringify(entradasObjetoDosCampos(), null, 2));
            }

            function entradasJsonAtual() {
                if (wfExecModo === 'json') {
                    return $('#wfExecEntradas').val() || '{}';
                }

                var json = JSON.stringify(entradasObjetoDosCampos(), null, 2);
                $('#wfExecEntradas').val(json);
                return json;
            }

            window.wfExecAtualizarJsonDosCampos = function () {
                if (wfExecModo === 'inputs') {
                    $('#wfExecEntradas').val(JSON.stringify(entradasObjetoDosCampos(), null, 2));
                }
            };

            function classeStatus(status) {
                if (status === 'CONCLUIDO') return 'success';
                if (status === 'PAUSADO') return 'warning';
                if (status === 'CANCELADO') return 'info';
                return 'danger';
            }

            function postar(url, payload) {
                return $.ajax({
                    type: 'POST',
                    url: url,
                    data: JSON.stringify(payload),
                    contentType: 'application/json; charset=utf-8',
                    dataType: 'json'
                });
            }

            function renderizarStatus(resposta) {
                var status = resposta.Status || 'ERRO';
                var msg = resposta.Mensagem || 'Execução processada.';
                $('#wfExecStatus')
                    .removeClass('alert-success alert-warning alert-danger alert-info')
                    .addClass('alert-' + classeStatus(status))
                    .html('<strong>' + esc(status) + '</strong> - ' + esc(msg))
                    .show();
            }

            function formatarDuracaoMs(ms) {
                ms = Number(ms) || 0;
                if (ms <= 0) return '';
                if (ms < 1000) return ms + ' ms';
                var s = ms / 1000;
                return (s < 10 ? s.toFixed(1) : String(Math.round(s))).replace('.', ',') + ' s';
            }

            function renderizarTrace(trace) {
                if (!trace || !trace.length) {
                    $('#wfExecTrace').html('<span class="text-muted">Sem passos registrados.</span>');
                    return;
                }

                var html = '';
                trace.forEach(function (t, idx) {
                    var cls = classeTraceLabel(t);
                    var entradaTrace = traceCampo(t, 'Entrada');
                    var saidaTrace = traceCampo(t, 'Saida');
                    var temDet = entradaTrace || saidaTrace;
                    var duracao = formatarDuracaoMs(t.DuracaoMs || t.duracaoMs);

                    html += '<div class="wf-exec-trace-passo">' +
                        '<div style="display:flex;align-items:center;gap:6px;">' +
                            '<span class="wf-exec-noid">' + esc(t.No) + '</span>' +
                            '<strong>' + esc(t.Titulo || t.Tipo) + '</strong>' +
                            '<span style="margin-left:auto;">' +
                                (duracao ? '<small class="text-muted" title="Tempo do passo">' + esc(duracao) + '</small> ' : '') +
                                (temDet ? '<button type="button" class="btn btn-xs btn-default" onclick="wfExecToggleDet(' + idx + ')">Detalhes</button> ' : '') +
                                '<span class="label label-' + cls + '">' + esc(statusTraceLabel(t)) + '</span>' +
                            '</span>' +
                        '</div>';

                    if (temDet) {
                        html += '<div id="wfExecDet' + idx + '" style="display:none;">' +
                            (entradaTrace ? '<div class="wf-exec-io"><b>in:</b>\n' + esc(bonito(entradaTrace)) + '</div>' : '') +
                            (saidaTrace ? '<div class="wf-exec-io"><b>out:</b>\n' + esc(bonito(saidaTrace)) + '</div>' : '') +
                        '</div>';
                    }

                    html += '</div>';
                });

                $('#wfExecTrace').html(html);
            }

            function jsonObjetoSeguro(texto) {
                if (!texto) return {};
                if (typeof texto === 'object') return texto || {};
                try { return JSON.parse(texto || '{}') || {}; } catch (e) { return {}; }
            }

            function dadosPausaResposta(resposta) {
                resposta = resposta || {};
                var dados = dadosPausaBrutos(resposta);
                if (dados && dados.campos && dados.campos.length) return dados;

                var trace = resposta.Trace || [];
                for (var i = trace.length - 1; i >= 0; i--) {
                    if (!trace[i] || String(trace[i].Status || '').toUpperCase() !== 'PAUSADO') continue;
                    dados = jsonObjetoSeguro(trace[i].Entrada || trace[i].entrada);
                    if (dados && dados.campos && dados.campos.length) {
                        if (!dados.no) dados.no = trace[i].No || trace[i].no || '';
                        if (!dados.titulo) dados.titulo = trace[i].Titulo || trace[i].titulo || '';
                        return dados;
                    }
                }
                return {};
            }

            function dadosPausaBrutos(resposta) {
                resposta = resposta || {};
                return jsonObjetoSeguro(resposta.DadosPausaJson || resposta.dadosPausaJson || resposta.DadosPausa || resposta.dadosPausa);
            }

            function ocultarEscolhas() {
                $('#wfExecEscolhas').empty().hide();
            }

            function renderizarEscolhas(resposta, selecaoMultipla) {
                var dados = dadosPausaBrutos(resposta);
                var opcoes = dados && dados.opcoes ? dados.opcoes : [];
                if (!opcoes.length) {
                    $('#wfExecEscolhas').html('<div class="text-danger">Nenhuma opção disponível para seleção.</div>').show();
                    return false;
                }

                var html = '';
                opcoes.forEach(function (opcao, idx) {
                    opcao = opcao || {};
                    var id = 'wfExecOpcao_' + String($('#wfExecIdExecucao').val() || '0') + '_' + idx;
                    var nome = selecaoMultipla ? 'wfExecSelecaoMultipla' : 'wfExecEscolha';
                    html += '<label class="wf-exec-escolha" for="' + esc(id) + '">' +
                        '<input type="' + (selecaoMultipla ? 'checkbox' : 'radio') + '" name="' + nome + '" id="' + esc(id) + '" value="__indice__:' + idx + '" />' +
                        '<span><strong>' + esc(opcao.rotulo || opcao.valor || ('Opção ' + (idx + 1))) + '</strong>' +
                        (opcao.descricao ? '<small>' + esc(opcao.descricao) + '</small>' : '') + '</span>' +
                        '</label>';
                });

                $('#wfExecEscolhas').html(html).show();
                return true;
            }

            function pausaEhCadastroCliente(dados) {
                dados = dados || {};
                if (String(dados.ferramenta || '').toLowerCase() === 'clientes_criar_para_orcamento') return true;
                var campos = dados.campos || [];
                for (var i = 0; i < campos.length; i++) {
                    if (String(campos[i].grupo || '').toLowerCase() === 'cadastro do cliente') return true;
                }
                return false;
            }

            function converterEntradaRuntime(valor, tipo) {
                tipo = String(tipo || 'string');
                if (tipo === 'integer') return parseInt(valor || '0', 10) || 0;
                if (tipo === 'number') return parseFloat(String(valor || '0').replace(',', '.')) || 0;
                return valor || '';
            }

            function renderizarEntradasPendentes(resposta) {
                var dados = dadosPausaResposta(resposta);
                if (!dados || !dados.campos || !dados.campos.length) {
                    $('#wfExecEntradasPendentes').hide();
                    wfExecEntradasPendentesDefs = [];
                    return false;
                }

                wfExecEntradasPendentesDefs = dados.campos || [];
                $('#wfExecEntradasTitulo').text(pausaEhCadastroCliente(dados) ? 'Precisamos dos dados para cadastrar o cliente' : 'Entradas pendentes');
                $('#wfExecEntradasMensagem').text(dados.mensagem || 'Informe os dados necessários para continuar este nó.');

                var grupos = {};
                wfExecEntradasPendentesDefs.forEach(function (campo, idx) {
                    var grupo = String(campo.grupo || 'Geral');
                    if (!grupos[grupo]) grupos[grupo] = [];
                    grupos[grupo].push({ campo: campo, idx: idx });
                });

                var html = '';
                Object.keys(grupos).forEach(function (grupo) {
                    html += '<div class="wf-exec-group-title">' + esc(grupo) + '</div>';
                    grupos[grupo].forEach(function (item) {
                        var campo = item.campo || {};
                        html += '<div class="wf-exec-input-row">' +
                            '<label title="' + esc(campo.tipo || 'string') + '">' + esc(campo.nome || '') + (campo.obrigatorio ? ' <span class="text-danger">*</span>' : '') + '</label>' +
                            '<div class="wf-exec-input-field">' +
                                '<input type="text" class="form-control input-sm wf-exec-runtime-input" data-i="' + item.idx + '" value="" placeholder="' + esc(campo.param || campo.nome || '') + '" />' +
                                '<span class="help-block">Usado em <span class="wf-mono">' + esc(campo.param || 'parâmetro') + '</span>.' + (campo.descricao ? '<br>' + esc(campo.descricao) : '') + '</span>' +
                            '</div>' +
                            '</div>';
                    });
                });

                $('#wfExecEntradasPendentesCampos').html(html);
                $('#wfExecEntradasPendentes').show();
                return true;
            }

            function renderizarResposta(resposta) {
                resposta = resposta || {};
                $('#wfExecIdExecucao').val(resposta.IdExecucao || $('#wfExecIdExecucao').val() || '0');
                renderizarStatus(resposta);
                renderizarTrace(resposta.Trace || []);

                if (resposta.Status === 'PAUSADO') {
                    wfExecTipoPausa = String(resposta.TipoPausa || '').toUpperCase();
                    var ehAprovacao = wfExecTipoPausa === 'APROVACAO';
                    var ehEspera = wfExecTipoPausa === 'ESPERA';
                    var ehEntradas = wfExecTipoPausa === 'ENTRADAS';
                    var ehEscolha = wfExecTipoPausa === 'ESCOLHA';
                    var ehSelecaoMultipla = wfExecTipoPausa === 'SELECAO_MULTIPLA';
                    if (ehEntradas && renderizarEntradasPendentes(resposta)) {
                        $('#wfExecAprovacao').hide();
                        ocultarEscolhas();
                        $('#wfExecBtnIniciar').prop('disabled', true);
                        return;
                    }
                    $('#wfExecEntradasPendentes').hide();
                    var tituloPausa = resposta.Resumo || ferramentaPausada(resposta.Trace) || 'ferramenta';
                    var descricaoPausa = descricaoPausaResposta(resposta, tituloPausa);
                    $('#wfExecResumoAprovacao').text(tituloPausa);
                    $('#wfExecDescricaoAprovacao').text(descricaoPausa).toggle(!!descricaoPausa);
                    $('#wfExecRotuloAprovacao').text(ehAprovacao ? 'Aprovação:' : (ehEspera ? 'Espera:' : ((ehEscolha || ehSelecaoMultipla) ? 'Busca:' : 'Ferramenta:')));
                    $('#wfExecBtnConfirmar').html('<i class="fa fa-check"></i> ' + (ehAprovacao ? 'Aprovar' : (ehEspera ? 'Continuar' : ((ehEscolha || ehSelecaoMultipla) ? 'Continuar com a seleção' : 'Confirmar passo'))));
                    $('#wfExecBtnRejeitar').toggle(ehAprovacao);
                    if (ehEscolha || ehSelecaoMultipla) {
                        var possuiOpcoes = renderizarEscolhas(resposta, ehSelecaoMultipla);
                        $('#wfExecBtnConfirmar').prop('disabled', ehEscolha && !possuiOpcoes);
                    } else {
                        ocultarEscolhas();
                        $('#wfExecBtnConfirmar').prop('disabled', false);
                    }
                    $('#wfExecAprovacao').show();
                    $('#wfExecBtnIniciar').prop('disabled', true);
                } else {
                    wfExecTipoPausa = '';
                    $('#wfExecAprovacao').hide();
                    $('#wfExecDescricaoAprovacao').hide().text('');
                    ocultarEscolhas();
                    $('#wfExecEntradasPendentes').hide();
                    wfExecEntradasPendentesDefs = [];
                    $('#wfExecBtnRejeitar').hide();
                    $('#wfExecBtnIniciar').prop('disabled', false);
                }
            }

            function ferramentaPausada(trace) {
                trace = trace || [];
                for (var i = trace.length - 1; i >= 0; i--) {
                    if (trace[i] && trace[i].Status === 'PAUSADO' && trace[i].Titulo) {
                        return trace[i].Titulo;
                    }
                }
                return '';
            }

            function descricaoPausaResposta(resposta, titulo) {
                resposta = resposta || {};
                var descricao = $.trim(String(resposta.Descricao || resposta.Mensagem || ''));
                titulo = $.trim(String(titulo || ''));
                if (!descricao || descricao === titulo) return '';
                return descricao;
            }

            window.wfExecAbrir = function (botao) {
                var $botao = $(botao);
                try { wfExecEntradasDefs = JSON.parse($botao.attr('data-entradas') || '[]') || []; }
                catch (e) { wfExecEntradasDefs = []; }
                wfExecEntradasPendentesDefs = [];
                wfExecModo = 'inputs';
                wfExecTipoPausa = '';
                $('#wfExecIdWorkflow').val($botao.data('id') || '0');
                $('#wfExecIdExecucao').val('0');
                $('#wfExecTitulo').text($botao.data('nome') || 'workflow');
                $('input[name="wfExecModo"]').first().prop('checked', true).closest('label').addClass('active').siblings().removeClass('active');
                $('#wfExecEntradasCampos').show();
                $('#wfExecJsonBox').hide();
                renderizarEntradasCampos();
                $('#wfExecStatus').hide();
                $('#wfExecAprovacao').hide();
                ocultarEscolhas();
                $('#wfExecEntradasPendentes').hide();
                $('#wfExecBtnRejeitar').hide().prop('disabled', false).html('<i class="fa fa-times"></i> Rejeitar');
                $('#wfExecTrace').html('Nenhuma execução iniciada.');
                $('#wfExecBtnIniciar').prop('disabled', false).html('<i class="fa fa-play"></i> Executar');
                $('#Modal_ExecutarWorkflow').modal('show');
            };

            window.wfExecSetModo = function (modo) {
                wfExecModo = modo === 'json' ? 'json' : 'inputs';
                if (wfExecModo === 'json') {
                    $('#wfExecEntradas').val(JSON.stringify(entradasObjetoDosCampos(), null, 2));
                    $('#wfExecEntradasCampos').hide();
                    $('#wfExecJsonBox').show();
                } else {
                    try { aplicarJsonNosCampos(JSON.parse($('#wfExecEntradas').val() || '{}')); }
                    catch (e) { }
                    $('#wfExecEntradasCampos').show();
                    $('#wfExecJsonBox').hide();
                }
            };

            function renderizarLoteEspera(resposta) {
                resposta = resposta || {};
                var status = resposta.Status || 'OK';
                var cls = status === 'ERRO' ? 'danger' : (resposta.TotalErro > 0 ? 'warning' : 'success');
                var msg = resposta.Mensagem || 'Processamento concluído.';
                var detalhe = ' Encontradas: ' + (resposta.TotalEncontrado || 0) +
                    ' | Processadas: ' + (resposta.TotalProcessado || 0) +
                    ' | Concluídas: ' + (resposta.TotalConcluido || 0) +
                    ' | Pausadas: ' + (resposta.TotalPausado || 0) +
                    ' | Erros: ' + (resposta.TotalErro || 0) + '.';

                $('#wfEsperaStatus')
                    .removeClass('alert-success alert-warning alert-danger alert-info')
                    .addClass('alert-' + cls)
                    .text(msg + detalhe)
                    .show();
            }

            window.wfExecProcessarEsperas = function () {
                var $btn = $('#wfBtnProcessarEsperas');
                $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Processando...');
                postar(urls.processarEsperas, {})
                    .done(function (retorno) {
                        renderizarLoteEspera(retorno && retorno.d ? retorno.d : {});
                    })
                    .fail(function () {
                        renderizarLoteEspera({ Status: 'ERRO', Mensagem: 'Não foi possível processar esperas.', TotalErro: 1 });
                    })
                    .always(function () {
                        $btn.prop('disabled', false).html('<i class="fa fa-clock-o"></i> Processar esperas vencidas');
                    });
            };

            window.wfExecLiberarEvento = function () {
                var evento = String($('#wfEventoEspera').val() || '').trim();
                var $btn = $('#wfBtnLiberarEvento');
                if (!evento) {
                    renderizarLoteEspera({ Status: 'ERRO', Mensagem: 'Informe o nome do evento.', TotalErro: 1 });
                    return;
                }

                $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Liberando...');
                postar(urls.liberarEvento, { evento: evento })
                    .done(function (retorno) {
                        renderizarLoteEspera(retorno && retorno.d ? retorno.d : {});
                    })
                    .fail(function () {
                        renderizarLoteEspera({ Status: 'ERRO', Mensagem: 'Não foi possível liberar o evento.', TotalErro: 1 });
                    })
                    .always(function () {
                        $btn.prop('disabled', false).html('<i class="fa fa-bolt"></i> Liberar evento');
                    });
            };

            $('#wfEventoEspera').on('keydown', function (ev) {
                if (ev.key === 'Enter') {
                    ev.preventDefault();
                    window.wfExecLiberarEvento();
                }
            });

            window.wfExecIniciar = function () {
                var idWorkflowIA = parseInt($('#wfExecIdWorkflow').val() || '0', 10);
                var entradasJson = entradasJsonAtual();
                var $btn = $('#wfExecBtnIniciar');

                try { JSON.parse(entradasJson); }
                catch (e) {
                    renderizarResposta({ Status: 'ERRO', Mensagem: 'JSON de entradas inválido.', Trace: [] });
                    return;
                }

                $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Executando...');
                postar(urls.iniciar, { idWorkflowIA: idWorkflowIA, entradasJson: entradasJson })
                    .done(function (retorno) {
                        renderizarResposta(retorno && retorno.d ? retorno.d : {});
                    })
                    .fail(function () {
                        renderizarResposta({ Status: 'ERRO', Mensagem: 'Não foi possível iniciar a execução.', Trace: [] });
                    })
                    .always(function () {
                        if ($('#wfExecAprovacao').is(':visible')) {
                            $btn.prop('disabled', true).html('<i class="fa fa-play"></i> Executar');
                        } else {
                            $btn.prop('disabled', false).html('<i class="fa fa-play"></i> Executar novamente');
                        }
                    });
            };

            window.wfExecContinuarEntradas = function () {
                var idExecucaoIA = parseInt($('#wfExecIdExecucao').val() || '0', 10);
                var novas = {};
                var invalido = null;

                $('#wfExecEntradasPendentesCampos .wf-exec-runtime-input').each(function () {
                    var $input = $(this);
                    var idx = parseInt($input.data('i') || '0', 10);
                    var campo = wfExecEntradasPendentesDefs[idx] || {};
                    var nome = String(campo.nome || '');
                    var valor = $.trim($input.val());
                    if (!nome) return;
                    if (campo.obrigatorio && !valor && !invalido) {
                        invalido = $input;
                        return;
                    }
                    novas[nome] = converterEntradaRuntime(valor, campo.tipo || 'string');
                });

                if (invalido) {
                    invalido.focus();
                    renderizarStatus({ Status: 'PAUSADO', Mensagem: 'Preencha as entradas obrigatórias para continuar.' });
                    return;
                }

                var $btn = $('#wfExecBtnContinuarEntradas');
                $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Continuando...');
                postar(urls.informarEntradas, { idExecucaoIA: idExecucaoIA, entradasJson: JSON.stringify(novas) })
                    .done(function (retorno) {
                        renderizarResposta(retorno && retorno.d ? retorno.d : {});
                    })
                    .fail(function () {
                        renderizarResposta({ Status: 'ERRO', Mensagem: 'Não foi possível informar as entradas pendentes.', Trace: [] });
                    })
                    .always(function () {
                        $btn.prop('disabled', false).html('<i class="fa fa-play"></i> Continuar workflow');
                    });
            };

            window.wfExecConfirmar = function () {
                var idExecucaoIA = parseInt($('#wfExecIdExecucao').val() || '0', 10);
                var $btn = $('#wfExecBtnConfirmar');
                var ehEscolha = wfExecTipoPausa === 'ESCOLHA';
                var ehSelecaoMultipla = wfExecTipoPausa === 'SELECAO_MULTIPLA';
                var valor = null;

                if (ehEscolha) {
                    var $escolha = $('#wfExecEscolhas input[name="wfExecEscolha"]:checked');
                    if (!$escolha.length) {
                        renderizarStatus({ Status: 'PAUSADO', Mensagem: 'Selecione uma opção para o workflow continuar.' });
                        return;
                    }
                    valor = $escolha.val();
                } else if (ehSelecaoMultipla) {
                    var selecoes = [];
                    $('#wfExecEscolhas input[name="wfExecSelecaoMultipla"]:checked').each(function () { selecoes.push($(this).val()); });
                    valor = JSON.stringify(selecoes);
                }

                $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> ' + ((ehEscolha || ehSelecaoMultipla) ? 'Aplicando escolha...' : 'Confirmando...'));
                postar((ehEscolha || ehSelecaoMultipla) ? urls.selecionarOpcao : urls.confirmar,
                    (ehEscolha || ehSelecaoMultipla) ? { idExecucaoIA: idExecucaoIA, valor: valor } : { idExecucaoIA: idExecucaoIA })
                    .done(function (retorno) {
                        renderizarResposta(retorno && retorno.d ? retorno.d : {});
                    })
                    .fail(function () {
                        renderizarResposta({ Status: 'ERRO', Mensagem: (ehEscolha || ehSelecaoMultipla) ? 'Não foi possível aplicar a seleção.' : 'Não foi possível confirmar o passo.', Trace: [] });
                    })
                    .always(function () {
                        var ehAprovacao = wfExecTipoPausa === 'APROVACAO';
                        var ehEspera = wfExecTipoPausa === 'ESPERA';
                        var aindaEscolha = wfExecTipoPausa === 'ESCOLHA' || wfExecTipoPausa === 'SELECAO_MULTIPLA';
                        var escolhaSemOpcoes = wfExecTipoPausa === 'ESCOLHA' && !$('#wfExecEscolhas input[name="wfExecEscolha"]').length;
                        $btn.prop('disabled', escolhaSemOpcoes).html('<i class="fa fa-check"></i> ' + (ehAprovacao ? 'Aprovar' : (ehEspera ? 'Continuar' : (aindaEscolha ? 'Continuar com a seleção' : 'Confirmar passo'))));
                    });
            };

            window.wfExecRejeitar = function () {
                var idExecucaoIA = parseInt($('#wfExecIdExecucao').val() || '0', 10);
                var $btn = $('#wfExecBtnRejeitar');
                $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Rejeitando...');
                postar(urls.rejeitar, { idExecucaoIA: idExecucaoIA })
                    .done(function (retorno) {
                        renderizarResposta(retorno && retorno.d ? retorno.d : {});
                    })
                    .fail(function () {
                        renderizarResposta({ Status: 'ERRO', Mensagem: 'Não foi possível rejeitar a aprovação.', Trace: [] });
                    })
                    .always(function () {
                        $btn.prop('disabled', false).html('<i class="fa fa-times"></i> Rejeitar');
                    });
            };

            window.wfExecCancelar = function () {
                var idExecucaoIA = parseInt($('#wfExecIdExecucao').val() || '0', 10);
                var $btn = $('#wfExecBtnCancelar');
                $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Cancelando...');
                postar(urls.cancelar, { idExecucaoIA: idExecucaoIA })
                    .done(function (retorno) {
                        renderizarResposta(retorno && retorno.d ? retorno.d : {});
                    })
                    .fail(function () {
                        renderizarResposta({ Status: 'ERRO', Mensagem: 'Não foi possível cancelar a execução.', Trace: [] });
                    })
                    .always(function () {
                        $btn.prop('disabled', false).html('<i class="fa fa-ban"></i> Cancelar execução');
                    });
            };

            window.wfExecToggleDet = function (idx) {
                $('#wfExecDet' + idx).toggle();
            };

            // Importar workflow: cola/escolhe o JSON exportado (Workflow_Detalhe.aspx > Exportar workflow, ou
            // um arquivo de SQL/IA/Workflows/) e aplica via ImportarWorkflow (cria ou atualiza pelo nome
            // interno). Sem isso, essa operação só existia rodando EXEC dbo.sp_IA_Workflow_Importar no SSMS.
            window.wfAbrirImportarWorkflow = function () {
                $('#wfImportarTexto').val('');
                $('#wfImportarArquivo').val('');
                $('#wfImportarStatus').empty();
                $('#Modal_ImportarWorkflow').modal('show');
            };

            $('#wfImportarArquivo').on('change', function (ev) {
                var arquivo = ev.target.files && ev.target.files[0];
                if (!arquivo) return;
                var leitor = new FileReader();
                leitor.onload = function (e) { $('#wfImportarTexto').val(e.target.result); };
                leitor.readAsText(arquivo, 'UTF-8');
            });

            window.wfImportarWorkflowConfirmar = function () {
                var texto = $.trim($('#wfImportarTexto').val());
                if (!texto) {
                    $('#wfImportarStatus').html('<div class="alert alert-warning" style="padding:6px 10px;">Cole o JSON ou escolha o arquivo.</div>');
                    return;
                }
                try { JSON.parse(texto); }
                catch (e) {
                    $('#wfImportarStatus').html('<div class="alert alert-danger" style="padding:6px 10px;">JSON inválido: ' + esc(e.message) + '</div>');
                    return;
                }

                var $btn = $('#wfImportarBtn');
                $btn.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Importando...');
                postar(urls.importar, { sWorkflowJson: texto })
                    .done(function (retorno) {
                        var r = retorno && retorno.d ? retorno.d : {};
                        if (r.Sucesso) {
                            $('#wfImportarStatus').html('<div class="alert alert-success" style="padding:6px 10px;">Workflow "' + esc(r.SNomeInterno) + '" (id ' + esc(r.IdWorkflowIA) + ') importado. Recarregando a lista...</div>');
                            setTimeout(function () { window.location.reload(); }, 900);
                        } else {
                            $('#wfImportarStatus').html('<div class="alert alert-danger" style="padding:6px 10px;">' + esc(r.Mensagem || 'Não foi possível importar o workflow.') + '</div>');
                        }
                    })
                    .fail(function () {
                        $('#wfImportarStatus').html('<div class="alert alert-danger" style="padding:6px 10px;">Não foi possível importar o workflow.</div>');
                    })
                    .always(function () {
                        $btn.prop('disabled', false).html('<i class="fa fa-upload"></i> Importar');
                    });
            };

            // Some da lista principal sem apagar nada (reversível em "Arquivados"). O servidor recusa sozinho
            // se o workflow ainda estiver ativo - a mensagem de recusa vem dele, só exibimos.
            window.wfArquivarWorkflow = function (botao) {
                var $botao = $(botao);
                var idWorkflowIA = parseInt($botao.data('id') || '0', 10);
                var nome = String($botao.data('nome') || 'este workflow');
                if (!idWorkflowIA) return;
                if (!window.confirm('Arquivar "' + nome + '"? Ele sai da lista, mas pode ser desarquivado depois em "Arquivados".')) return;

                $botao.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Arquivando...');
                postar(urls.arquivar, { idWorkflowIA: idWorkflowIA })
                    .done(function (retorno) {
                        var r = retorno && retorno.d ? retorno.d : {};
                        if (r.Sucesso) {
                            window.location.reload();
                        } else {
                            alert(r.Mensagem || 'Não foi possível arquivar o workflow.');
                            $botao.prop('disabled', false).html('<i class="fa fa-archive"></i> Arquivar');
                        }
                    })
                    .fail(function () {
                        alert('Não foi possível arquivar o workflow.');
                        $botao.prop('disabled', false).html('<i class="fa fa-archive"></i> Arquivar');
                    });
            };

            function renderizarArquivados(lista) {
                var $box = $('#wfArquivadosLista');
                if (!lista || !lista.length) {
                    $box.html('<p class="text-muted">Nenhum workflow arquivado.</p>');
                    return;
                }
                var html = '<table class="table table-striped table-bordered table-hover"><thead><tr>' +
                    '<th>Nome interno</th><th>Descrição</th><th></th></tr></thead><tbody>';
                for (var i = 0; i < lista.length; i++) {
                    var wf = lista[i];
                    html += '<tr>' +
                        '<td>' + esc(wf.Nome) + '</td>' +
                        '<td>' + esc(wf.Descricao) + '</td>' +
                        '<td><button type="button" class="btn btn-default btn-xs" data-id="' + esc(wf.IdWorkflowIA) + '" data-nome="' + esc(wf.Nome) + '" onclick="wfDesarquivarWorkflow(this)">' +
                        '<i class="fa fa-undo"></i> Desarquivar</button></td>' +
                        '</tr>';
                }
                html += '</tbody></table>';
                $box.html(html);
            }

            window.wfAbrirArquivados = function () {
                $('#wfArquivadosLista').html('<p class="text-muted"><i class="fa fa-spinner fa-spin"></i> Carregando...</p>');
                $('#Modal_Arquivados').modal('show');
                postar(urls.listarArquivados, {})
                    .done(function (retorno) {
                        var r = retorno && retorno.d ? retorno.d : {};
                        if (r.Sucesso) {
                            renderizarArquivados(r.Workflows || []);
                        } else {
                            $('#wfArquivadosLista').html('<div class="alert alert-danger" style="padding:6px 10px;">' + esc(r.Mensagem || 'Não foi possível carregar os workflows arquivados.') + '</div>');
                        }
                    })
                    .fail(function () {
                        $('#wfArquivadosLista').html('<div class="alert alert-danger" style="padding:6px 10px;">Não foi possível carregar os workflows arquivados.</div>');
                    });
            };

            // Volta pra lista principal (ainda inativo - reativar continua sendo o checkbox Ativo, à parte);
            // recarrega a página pra ele já aparecer lá, em vez de só sumir do modal.
            window.wfDesarquivarWorkflow = function (botao) {
                var $botao = $(botao);
                var idWorkflowIA = parseInt($botao.data('id') || '0', 10);
                if (!idWorkflowIA) return;

                $botao.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Desarquivando...');
                postar(urls.desarquivar, { idWorkflowIA: idWorkflowIA })
                    .done(function (retorno) {
                        var r = retorno && retorno.d ? retorno.d : {};
                        if (r.Sucesso) {
                            window.location.reload();
                        } else {
                            alert(r.Mensagem || 'Não foi possível desarquivar o workflow.');
                            $botao.prop('disabled', false).html('<i class="fa fa-undo"></i> Desarquivar');
                        }
                    })
                    .fail(function () {
                        alert('Não foi possível desarquivar o workflow.');
                        $botao.prop('disabled', false).html('<i class="fa fa-undo"></i> Desarquivar');
                    });
            };
        })();
    </script>
</asp:Content>
