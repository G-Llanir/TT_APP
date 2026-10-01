<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Arquivos.aspx.cs" Inherits="TT_Flow.App.Paginas.IA.Arquivos" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%= ResolveUrl("~/App/JS/ia-markdown.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
        /* Regras proprias: formatacao de conteudo dentro da celula e o visualizador do modal.
           A casa nao tem equivalente. Fundo, borda e monoespaco do modal vem do <pre> do bootstrap. */
        .ia-arquivos-texto {
            max-width: 360px;
            white-space: pre-wrap;
            word-break: break-word;
            font-size: 11px;
        }

        .ia-arquivos-hash {
            max-width: 180px;
            word-break: break-all;
            font-family: Consolas, "Courier New", monospace;
            font-size: 11px;
        }

        .ia-arquivos-nowrap {
            white-space: nowrap;
        }

        .ia-arquivos-modal-texto {
            min-height: 260px;
            max-height: 60vh;
            overflow: auto;
            white-space: pre-wrap;
            word-break: break-word;
        }

        .ia-arquivos-modal-html {
            min-height: 260px;
            max-height: 60vh;
            overflow: auto;
            word-break: break-word;
        }
    </style>

    <asp:UpdatePanel ID="updArquivosIA" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Arquivos IA"></asp:Label><small> Operação</small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Arquivos IA" />
                </div>

                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-filter"></i> Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-2 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Data inicial</label>
                                        <asp:TextBox ID="txtdtInicial" CssClass="form-control CaixaTextoData" runat="server" MaxLength="10" />
                                    </div>
                                </div>
                                <div class="col-lg-2 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Data final</label>
                                        <asp:TextBox ID="txtdtFinal" CssClass="form-control CaixaTextoData" runat="server" MaxLength="10" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Usuário</label>
                                        <asp:DropDownList ID="ddlUsuario" CssClass="form-control Caixa_Selecao" runat="server" />
                                    </div>
                                </div>
                                <div class="col-lg-2 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Extensão</label>
                                        <asp:DropDownList ID="ddlExtensao" CssClass="form-control Caixa_Selecao" runat="server" />
                                    </div>
                                </div>
                                <div class="col-lg-3 col-md-4 col-sm-6">
                                    <div class="form-group">
                                        <label>Status</label>
                                        <asp:DropDownList ID="ddlStatusArquivo" CssClass="form-control" runat="server">
                                            <asp:ListItem Text="Todos" Value="" />
                                            <asp:ListItem Text="Enviado" Value="ENVIADO" />
                                            <asp:ListItem Text="Convertendo" Value="CONVERTENDO" />
                                            <asp:ListItem Text="Processando" Value="PROCESSANDO" />
                                            <asp:ListItem Text="Pronto" Value="PRONTO" />
                                            <asp:ListItem Text="Erro" Value="ERRO_CONVERSAO" />
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="row">
                                <div class="col-lg-4 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>Buscar (nome, hash, erro)</label>
                                        <asp:TextBox ID="txtTermo" CssClass="form-control" runat="server" MaxLength="200" />
                                    </div>
                                </div>
                                <div class="col-lg-8 col-md-6 col-sm-12">
                                    <div class="form-group">
                                        <label>&nbsp;</label>
                                        <div>
                                            <asp:Button ID="cmdPesquisar" CssClass="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisa(this)" />
                                            <asp:Button ID="cmdLimpar" CssClass="btn btn-default" runat="server" Text="Limpar" OnClick="cmdLimpar_Click" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-file-text-o"></i> Arquivos processados</h3>
                        </div>
                        <div class="panel-body">
                            <div class="table-responsive">
                                <asp:GridView ID="dtgvArquivos" runat="server" Width="100%" AutoGenerateColumns="False"
                                    CellPadding="0" CellSpacing="1" GridLines="None" ShowFooter="False"
                                    CssClass="table table-striped table-bordered table-hover table-condensed table-responsive tablesorter"
                                    Font-Names="Tahoma" Font-Size="Small" OnRowDataBound="dtgvArquivos_RowDataBound">
                                    <Columns>
                                        <asp:BoundField DataField="dtUpload" HeaderText="Upload" />
                                        <asp:BoundField DataField="dtProcessamento" HeaderText="Processamento" />
                                        <asp:BoundField DataField="idArquivoIA" HeaderText="Arquivo" />
                                        <asp:TemplateField HeaderText="Conversa">
                                            <ItemTemplate><%# LinkConversa(Eval("idConversaIA")) %></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:TemplateField HeaderText="Mensagem">
                                            <ItemTemplate><%# TracoSeZero(Eval("idMensagemIA")) %></ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário" />
                                        <asp:BoundField DataField="sNomeOriginal" HeaderText="Nome" />
                                        <asp:BoundField DataField="sExtensao" HeaderText="Extensão" />
                                        <asp:BoundField DataField="sTamanho" HeaderText="Tamanho" />
                                        <asp:TemplateField HeaderText="Status">
                                            <ItemTemplate>
                                                <span class='<%# StatusBadgeCss(Eval("sStatus")) %>'><%# StatusTexto(Eval("sStatus")) %></span>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="nTotalCaracteres" HeaderText="Chars" />
                                        <asp:BoundField DataField="nTotalTrechos" HeaderText="Trechos" />
                                        <asp:BoundField DataField="sPossuiTemporario" HeaderText="Temp" />
                                        <asp:BoundField DataField="sHashSHA256" HeaderText="Hash" />
                                        <asp:TemplateField HeaderText="Resumo">
                                            <ItemTemplate>
                                                <button type="button" class="btn btn-default btn-xs ia-arquivos-ver-conteudo" title="Resumo gerado pela IA"><i class="fa fa-eye"></i> Resumo</button>
                                                <button type="button" class="btn btn-default btn-xs ia-arquivos-ver-markdown" title="Texto convertido do arquivo" data-idarquivo="<%# Eval("idArquivoIA") %>"><i class="fa fa-file-text-o"></i> Markdown</button>
                                                <div class="ia-arquivos-titulo-modal invisivel"><%# TituloModal(Eval("sNomeOriginal")) %></div>
                                                <div class="ia-arquivos-conteudo-modal invisivel"><%# ConteudoModal(Eval("sResumo")) %></div>
                                            </ItemTemplate>
                                        </asp:TemplateField>
                                        <asp:BoundField DataField="sErro" HeaderText="Erro" />
                                        <asp:TemplateField HeaderText="Ações" ItemStyle-HorizontalAlign="Center">
                                            <ItemTemplate>
                                                <button type="button" class="btn btn-warning btn-xs ia-arquivos-reprocessar" title="Reenfileira o arquivo para conversão. Só habilita se o original ainda estiver guardado."
                                                    data-idarquivo="<%# Eval("idArquivoIA") %>"
                                                    data-nome="<%# AtributoHtml(Eval("sNomeOriginal")) %>"
                                                    <%# AtributoReprocessar(Eval("sStatus"), Eval("sPossuiTemporario")) %>><i class="fa fa-refresh"></i> Reprocessar</button>
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

    <%-- Fora do UpdatePanel: dentro, o postback parcial recria a marcacao e o bootstrap perde o modal. --%>
    <div class="modal fade" id="modalArquivoIAConteudo">
        <div class="modal-dialog modal-grande">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-title">
                        <label id="modalArquivoIAConteudoTitulo">Resumo do arquivo</label>
                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span></button>
                    </div>
                </div>
                <div class="modal-body">
                    <%-- <pre> para texto puro (resumo, mensagens); <div> para o markdown renderizado. --%>
                    <pre id="modalArquivoIAConteudoTexto" class="ia-arquivos-modal-texto"></pre>
                    <div id="modalArquivoIAConteudoHtml" class="ia-arquivos-modal-html invisivel"></div>
                </div>
            </div>
        </div>
    </div>

    <script>
        (function () {
            // texto puro (resumo, mensagens de erro) vai no <pre>; markdown convertido vai renderizado.
            function abrirModalArquivoIA(titulo, conteudo, comoMarkdown) {
                var $texto = $('#modalArquivoIAConteudoTexto');
                var $html = $('#modalArquivoIAConteudoHtml');

                $('#modalArquivoIAConteudoTitulo').text(titulo || 'Conteúdo do arquivo');

                if (comoMarkdown) {
                    // Mesmo renderizador do chat: escapa o HTML do arquivo e so aceita link http(s)/interno.
                    $html.html(IA_Markdown.renderizar(conteudo, {
                        wrapTabela: 'table-responsive',
                        classeTabela: 'table table-bordered table-condensed',
                        classeCodigo: '',
                        htmlVazio: '<span class="help-block">Sem conteúdo disponível.</span>'
                    })).removeClass('invisivel');
                    $texto.addClass('invisivel');
                } else {
                    $texto.text(conteudo || 'Sem conteúdo disponível.').removeClass('invisivel');
                    $html.addClass('invisivel');
                }

                $('#modalArquivoIAConteudo').modal('show');
            }

            function registrarModalArquivosIA() {
                $(document).off('click.iaArquivosConteudo', '.ia-arquivos-ver-conteudo');
                $(document).on('click.iaArquivosConteudo', '.ia-arquivos-ver-conteudo', function () {
                    var celula = $(this).closest('td');
                    var titulo = $.trim(celula.find('.ia-arquivos-titulo-modal').text()) || 'Resumo do arquivo';

                    // O resumo tambem e gerado pela IA, entao vem em markdown como as respostas do chat.
                    // Vazio fica por conta do htmlVazio do renderizador.
                    abrirModalArquivoIA(titulo, $.trim(celula.find('.ia-arquivos-conteudo-modal').text()), true);
                });

                $(document).off('click.iaArquivosMarkdown', '.ia-arquivos-ver-markdown');
                $(document).on('click.iaArquivosMarkdown', '.ia-arquivos-ver-markdown', function () {
                    var idArquivoIA = String($(this).data('idarquivo') || '');
                    if (!idArquivoIA) {
                        abrirModalArquivoIA('Markdown do arquivo', 'Arquivo IA invalido.');
                        return;
                    }

                    abrirModalArquivoIA('Markdown do arquivo', 'Carregando markdown convertido...', false);

                    $.ajax({
                        type: 'POST',
                        url: '<%= ResolveUrl("~/App/Paginas/IA/Arquivos.aspx/ConsultarMarkdownArquivo") %>',
                        data: JSON.stringify({ idArquivoIA: idArquivoIA }),
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json'
                    }).done(function (retorno) {
                        var resposta = retorno && retorno.d ? retorno.d : {};

                        // So o markdown de verdade e renderizado; erro do servidor continua texto puro.
                        if (resposta.Sucesso) {
                            abrirModalArquivoIA(resposta.Titulo || 'Markdown do arquivo', resposta.Conteudo, true);
                        } else {
                            abrirModalArquivoIA(resposta.Titulo || 'Markdown do arquivo', resposta.Mensagem || 'Não foi possível carregar o markdown.', false);
                        }
                    }).fail(function () {
                        abrirModalArquivoIA('Markdown do arquivo', 'Não foi possível carregar o markdown convertido.', false);
                    });
                });

                $(document).off('click.iaArquivosReprocessar', '.ia-arquivos-reprocessar');
                $(document).on('click.iaArquivosReprocessar', '.ia-arquivos-reprocessar', function () {
                    var botao = $(this);
                    var idArquivoIA = String(botao.data('idarquivo') || '');
                    var nome = String(botao.data('nome') || 'arquivo');

                    if (!idArquivoIA || botao.is(':disabled')) {
                        return;
                    }

                    if (!window.confirm('Reprocessar o arquivo "' + nome + '"?')) {
                        return;
                    }

                    botao.prop('disabled', true).html('<i class="fa fa-spinner fa-spin"></i> Reprocessando...');

                    $.ajax({
                        type: 'POST',
                        url: '<%= ResolveUrl("~/App/Paginas/IA/Arquivos.aspx/ReprocessarArquivo") %>',
                        data: JSON.stringify({ idArquivoIA: idArquivoIA }),
                        contentType: 'application/json; charset=utf-8',
                        dataType: 'json'
                    }).done(function (retorno) {
                        var resposta = retorno && retorno.d ? retorno.d : {};
                        abrirModalArquivoIA('Reprocessamento de arquivo', resposta.Mensagem || 'Acao concluida.');

                        if (resposta.Sucesso) {
                            window.setTimeout(function () {
                                __doPostBack('<%= cmdPesquisar.UniqueID %>', '');
                            }, 700);
                        } else {
                            botao.prop('disabled', false).html('<i class="fa fa-refresh"></i> Reprocessar');
                        }
                    }).fail(function () {
                        abrirModalArquivoIA('Reprocessamento de arquivo', 'Não foi possível reenfileirar o arquivo.');
                        botao.prop('disabled', false).html('<i class="fa fa-refresh"></i>');
                    });
                });
            }

            registrarModalArquivosIA();
        })();
    </script>
</asp:Content>
