<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Conhecimento.aspx.cs" Inherits="TT_Flow.App.Paginas.IA.Conhecimento" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%= ResolveUrl("~/App/JS/ia-markdown.js") %>" type="text/javascript"></script>
    <style>
        .ia-conh-modal-texto { min-height: 260px; max-height: 60vh; overflow: auto; word-break: break-word; }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Base de Conhecimento"></asp:Label><small> IA</small>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="IA - Conhecimento" />
        </div>

        <div class="col-lg-12">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>

        <div class="col-lg-12">
            <div class="alert alert-info">
                <i class="fa fa-info-circle"></i>
                Suba documentos (PDF, DOCX, TXT, MD, CSV, XLSX) para a IA usar como base em qualquer conversa.
                As <strong>perguntas frequentes</strong> vêm do módulo <a href="<%= ResolveUrl("~/App/Paginas/FAQ/FAQ_Consulta.aspx") %>">FAQ</a> (apenas as aprovadas) — a IA as consulta automaticamente, não precisa recadastrar aqui.
                <strong>Não inclua dados sensíveis</strong> — todo o conteúdo fica disponível para a IA.
            </div>
        </div>

        <div class="col-lg-12 form-stacked actions">
            <div class="d-flex fw-w" style="align-items: flex-end; gap: 10px;">
                <div class="form-group" style="margin: 0;">
                    <label>Filtrar por categoria</label>
                    <asp:TextBox ID="txtFiltroCategoria" CssClass="form-control" runat="server" MaxLength="100" />
                </div>
                <asp:Button ID="cmdAtualizar" CssClass="btn btn-primary" runat="server" Text="Atualizar" OnClick="cmdAtualizar_Click" />
            </div>
        </div>

        <%-- ===== Pasta de entrada =====
             Mostra o caminho real vindo da configuracao e as regras, para quem gerencia a base nao
             precisar procurar em outra tela (nem em documentacao) so para saber onde copiar.
             Quem LE a pasta e o job tmrConhecimentoPasta do TT_Windows (desde 16/09/2026); por isso o
             painel mostra o estado do job — desligado e a causa mais provavel de "copiei e nao entrou". --%>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-folder-open-o"></i> Pasta de entrada
                        <i class="fa fa-question-circle" data-toggle="tooltip" title="Um jeito de alimentar a base sem passar por esta tela: copie o arquivo para a pasta do servidor e ele entra sozinho."></i>
                    </h3>
                </div>
                <div class="panel-body">
                    <% if (PastaConfigurada) { %>
                    <p style="margin-bottom: 10px;">
                        Copie o arquivo para a pasta abaixo e ele entra na base sozinho — não precisa enviar por esta tela.
                    </p>
                    <div class="d-flex fw-w" style="align-items: center; gap: 10px; margin-bottom: 12px;">
                        <code id="conhPastaCaminho" style="font-size: 14px; padding: 6px 10px;"><%= Server.HtmlEncode(PastaEntrada) %></code>
                        <button type="button" class="btn btn-default btn-sm" title="Copiar o caminho" onclick="Conh.copiarCaminho()">
                            <i class="fa fa-clipboard"></i> Copiar caminho
                        </button>
                        <span id="conhPastaStatus" class="help-block" style="margin: 0;"></span>
                    </div>
                    <% if (JobSituacao == "LIGADO") { %>
                    <p style="margin-bottom: 10px;">
                        <i class="fa fa-clock-o text-success"></i>
                        O serviço <strong>TT_Windows</strong> lê esta pasta a cada <strong><%= JobIntervaloMinutos %> minuto<%= JobIntervaloMinutos == 1 ? "" : "s" %></strong>.
                        Depois de copiar, o arquivo aparece na lista abaixo dentro desse tempo — use <strong>Atualizar</strong> para ver.
                    </p>
                    <% } else if (JobSituacao == "DESLIGADO") { %>
                    <p style="margin-bottom: 10px;">
                        <i class="fa fa-exclamation-triangle text-warning"></i>
                        <strong>A leitura automática está desligada.</strong> O job <code>tmrConhecimentoPasta</code> do serviço
                        TT_Windows está cadastrado mas inativo: arquivos copiados para a pasta <strong>não vão entrar</strong>
                        até alguém ligá-lo nos parâmetros de timers do TT_Windows.
                    </p>
                    <% } else { %>
                    <p style="margin-bottom: 10px;">
                        <i class="fa fa-exclamation-triangle text-warning"></i>
                        <strong>A leitura automática não está cadastrada.</strong> Falta o job <code>tmrConhecimentoPasta</code>
                        nos parâmetros de timers do serviço TT_Windows: arquivos copiados para a pasta <strong>não vão entrar</strong>
                        até ele ser cadastrado.
                    </p>
                    <% } %>
                    <ul style="margin-bottom: 0; padding-left: 18px;">
                        <li><strong>Subpasta vira a categoria.</strong> Um arquivo em <code>\Manuais\</code> entra com a categoria <em>Manuais</em>; solto na raiz, entra sem categoria. Pode criar as subpastas que quiser — não precisa cadastrar nada antes. Só o primeiro nível é lido.</li>
                        <li><strong>Depois de lido, o arquivo sai da pasta:</strong> vai para <code>_Processados</code> se entrou, ou <code>_Erros</code> se foi recusado. Então o que estiver na pasta é sempre o que ainda falta entrar.</li>
                        <li><strong>Arquivo repetido não duplica.</strong> A comparação é pelo conteúdo, não pelo nome: a mesma apostila com dois nomes diferentes entra uma vez só.</li>
                        <li><strong>Formatos:</strong> PDF, DOCX, TXT, MD, CSV e XLSX. Qualquer outro vai para <code>_Erros</code>, e o motivo fica registrado na <a href="<%= ResolveUrl("~/App/Paginas/IA/Auditoria.aspx") %>">Auditoria</a>.</li>
                        <li><strong>Arquivo ainda sendo copiado é esperado:</strong> se a cópia de um arquivo grande não terminou quando o serviço passar, ele fica para a leitura seguinte, sem erro.</li>
                    </ul>
                    <% } else { %>
                    <p style="margin-bottom: 0;">
                        <i class="fa fa-exclamation-triangle text-warning"></i>
                        Nenhuma pasta de entrada configurada. Com ela, dá para alimentar a base copiando arquivos
                        direto para uma pasta do servidor, sem passar por esta tela — inclusive vários de uma vez.
                        Defina o caminho em <a href="<%= ResolveUrl("~/App/Paginas/IA/Configuracao.aspx") %>">IA - Configuração</a>,
                        no painel <strong>Base de conhecimento</strong>.
                    </p>
                    <% } %>
                </div>
            </div>
        </div>

        <%-- ===== Documentos ===== --%>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-file-text-o"></i> Documentos</h3>
                </div>
                <div class="panel-body">
                    <div class="row d-flex fw-w">
                        <div class="col-lg-4 col-md-6 col-sm-12">
                            <div class="form-group">
                                <label>Arquivo</label>
                                <input id="conhFile" type="file" class="form-control" accept=".pdf,.docx,.txt,.md,.csv,.xlsx" />
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-6 col-sm-12">
                            <div class="form-group">
                                <label>Título (opcional)</label>
                                <input id="conhTitulo" type="text" class="form-control" maxlength="200" placeholder="Ex.: Política de vendas" />
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-6 col-sm-12">
                            <div class="form-group">
                                <label>Categoria (opcional)</label>
                                <input id="conhCategoria" type="text" class="form-control" maxlength="100" placeholder="Ex.: vendas" />
                            </div>
                        </div>
                        <div class="col-lg-2 col-md-6 col-sm-12">
                            <div class="form-group">
                                <label>&nbsp;</label>
                                <div>
                                    <button type="button" class="btn btn-success" onclick="Conh.enviarDoc()"><i class="fa fa-upload"></i> Enviar</button>
                                </div>
                            </div>
                        </div>
                    </div>
                    <span id="conhUploadStatus" class="help-block"></span>

                    <div class="table-responsive">
                        <asp:GridView ID="dtgvDocs" runat="server" Width="100%" AutoGenerateColumns="False"
                            CellPadding="0" CellSpacing="1" GridLines="None" ShowFooter="False" ShowHeaderWhenEmpty="True"
                            EmptyDataText="Nenhum documento na base de conhecimento."
                            CssClass="table table-striped table-bordered table-hover table-condensed table-responsive tablesorter"
                            Font-Names="Tahoma" Font-Size="Small">
                            <Columns>
                                <asp:BoundField DataField="dtUpload" HeaderText="Upload" />
                                <asp:BoundField DataField="idArquivoIA" HeaderText="ID" />
                                <asp:BoundField DataField="sTitulo" HeaderText="Título" />
                                <asp:BoundField DataField="sNomeOriginal" HeaderText="Arquivo" />
                                <asp:BoundField DataField="sCategoria" HeaderText="Categoria" />
                                <asp:BoundField DataField="sExtensao" HeaderText="Ext" />
                                <asp:BoundField DataField="sStatus" HeaderText="Status" />
                                <asp:BoundField DataField="nTotalTrechos" HeaderText="Trechos" />
                                <asp:BoundField DataField="sAtivo" HeaderText="Ativo" />
                                <asp:TemplateField HeaderText="Ações" ItemStyle-HorizontalAlign="Center">
                                    <ItemTemplate>
                                        <button type="button" class="btn btn-default btn-xs" title="Ver conteúdo convertido" onclick='Conh.verDoc(<%# Eval("idArquivoIA") %>)'><i class="fa fa-eye"></i></button>
                                        <button type="button" class="btn btn-warning btn-xs" title="Recriar os trechos de busca a partir do conteúdo já convertido" onclick='Conh.retrecharDoc(<%# Eval("idArquivoIA") %>)'><i class="fa fa-refresh"></i></button>
                                        <button type="button" class="btn btn-danger btn-xs" title="Remover da base" onclick='Conh.removerDoc(<%# Eval("idArquivoIA") %>)'><i class="fa fa-trash"></i></button>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                        </asp:GridView>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <%-- Modal de conteúdo (fora de UpdatePanel; nao ha UpdatePanel nesta tela) --%>
    <div class="modal fade" id="Modal_DocMarkdown">
        <div class="modal-dialog modal-grande">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-title">
                        <label id="lblDocTitulo">Conteúdo</label>
                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span></button>
                    </div>
                </div>
                <div class="modal-body">
                    <div id="divDocHtml" class="ia-conh-modal-texto"></div>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        var Conh = (function () {
            var urlUpload = '<%= ResolveUrl("~/API/IA_ArquivoUpload.aspx") %>';
            var urlMd = '<%= ResolveUrl("~/App/Paginas/IA/Conhecimento.aspx/ConsultarMarkdown") %>';
            var urlRemover = '<%= ResolveUrl("~/App/Paginas/IA/Conhecimento.aspx/RemoverDocumento") %>';
            var urlRetrechar = '<%= ResolveUrl("~/App/Paginas/IA/Conhecimento.aspx/ReprocessarTrechos") %>';
            var alvoAtualizar = '<%= cmdAtualizar.UniqueID %>';

            function recarregar() { __doPostBack(alvoAtualizar, ''); }
            function val(id) { return document.getElementById(id).value; }
            function status(txt) { document.getElementById('conhUploadStatus').textContent = txt || ''; }

            function enviarDoc() {
                var f = document.getElementById('conhFile');
                if (!f.files || !f.files.length) { status('Escolha um arquivo.'); return; }
                var fd = new FormData();
                fd.append('file', f.files[0]);
                fd.append('sTipo', 'B');
                fd.append('sCategoria', val('conhCategoria'));
                fd.append('sTituloConhecimento', val('conhTitulo'));
                status('Enviando e convertendo...');

                fetch(urlUpload, { method: 'POST', body: fd, credentials: 'same-origin' })
                    .then(function (r) { return r.json(); })
                    .then(function (resp) {
                        status(resp && resp.Mensagem ? resp.Mensagem : (resp && resp.Sucesso ? 'Enviado.' : 'Falha no envio.'));
                        if (resp && resp.Sucesso) { f.value = ''; document.getElementById('conhTitulo').value = ''; recarregar(); }
                    })
                    .catch(function () { status('Não foi possível enviar o arquivo.'); });
            }

            function postJson(url, dados, ok) {
                $.ajax({
                    type: 'POST', url: url, data: JSON.stringify(dados),
                    contentType: 'application/json; charset=utf-8', dataType: 'json'
                }).done(function (r) { ok(r && r.d ? r.d : {}); })
                  .fail(function () { ok({ Sucesso: false, Mensagem: 'Falha na comunicação.' }); });
            }

            // A grade é montada no carregamento da página e envelhece: outra pessoa pode remover um
            // documento, e a varredura da pasta mexe na base sem ninguém tocar nesta tela. Quando o
            // servidor avisa que o item clicado não existe mais, a tela se atualiza sozinha em vez de
            // deixar a pessoa clicando numa linha fantasma.
            function tratarListaVelha(r) {
                if (!r || !r.ListaDesatualizada) { return false; }
                // alert de proposito, e nao a linha de status: recarregar() dispara postback e apagaria
                // a mensagem antes de alguem ler. O alert segura ate o OK, e so entao a grade atualiza.
                window.alert(r.Mensagem || 'Este documento não está mais na base. Atualizando a lista...');
                recarregar();
                return true;
            }

            function verDoc(id) {
                document.getElementById('lblDocTitulo').textContent = 'Conteúdo';
                document.getElementById('divDocHtml').innerHTML = '<span class="help-block">Carregando...</span>';
                $('#Modal_DocMarkdown').modal('show');
                postJson(urlMd, { idArquivoIA: String(id) }, function (r) {
                    if (r.ListaDesatualizada) {
                        $('#Modal_DocMarkdown').modal('hide');
                        tratarListaVelha(r);
                        return;
                    }
                    document.getElementById('lblDocTitulo').textContent = r.Titulo || 'Conteúdo';
                    if (r.Sucesso) {
                        document.getElementById('divDocHtml').innerHTML = IA_Markdown.renderizar(r.Conteudo, {
                            wrapTabela: 'table-responsive', classeTabela: 'table table-bordered table-condensed', classeCodigo: '',
                            htmlVazio: '<span class="help-block">Sem conteúdo convertido.</span>'
                        });
                    } else {
                        document.getElementById('divDocHtml').innerHTML = '<span class="help-block">' + (r.Mensagem || 'Não foi possível carregar.') + '</span>';
                    }
                });
            }

            function removerDoc(id) {
                if (!window.confirm('Remover este documento da base de conhecimento?')) return;
                postJson(urlRemover, { idArquivoIA: String(id) }, function (r) {
                    if (tratarListaVelha(r)) return;
                    if (r.Sucesso) recarregar(); else window.alert(r.Mensagem || 'Não foi possível remover.');
                });
            }

            function retrecharDoc(id) {
                if (!window.confirm('Recriar os trechos de busca deste documento a partir do conteúdo já convertido?')) return;
                status('Recriando trechos...');
                postJson(urlRetrechar, { idArquivoIA: String(id) }, function (r) {
                    if (tratarListaVelha(r)) return;
                    status(r.Mensagem || '');
                    if (r.Sucesso) recarregar(); else window.alert(r.Mensagem || 'Não foi possível reprocessar os trechos.');
                });
            }

            // O caminho e do servidor, nao da maquina de quem usa: copiar poupa digitar errado ao colar
            // no Explorer. execCommand e o fallback porque o clipboard assincrono exige HTTPS, e a rede
            // interna roda em http.
            function copiarCaminho() {
                var el = document.getElementById('conhPastaCaminho');
                var lbl = document.getElementById('conhPastaStatus');
                if (!el) { return; }
                var texto = el.textContent;

                var ok = function () { lbl.textContent = 'Caminho copiado.'; };
                var falhou = function () { lbl.textContent = 'Não foi possível copiar — selecione o caminho e use Ctrl+C.'; };

                if (navigator.clipboard && window.isSecureContext) {
                    navigator.clipboard.writeText(texto).then(ok, falhou);
                    return;
                }

                try {
                    var area = document.createElement('textarea');
                    area.value = texto;
                    area.style.position = 'fixed';
                    area.style.opacity = '0';
                    document.body.appendChild(area);
                    area.select();
                    var copiou = document.execCommand('copy');
                    document.body.removeChild(area);
                    copiou ? ok() : falhou();
                } catch (e) {
                    falhou();
                }
            }

            return {
                enviarDoc: enviarDoc, verDoc: verDoc, removerDoc: removerDoc, retrecharDoc: retrecharDoc,
                copiarCaminho: copiarCaminho
            };
        })();
    </script>
</asp:Content>
