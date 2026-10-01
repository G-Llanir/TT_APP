<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Ferramentas_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.IA.Ferramentas_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
        /* Unica regra propria: a casa nao tem estilo monoespacado, e nome de procedure/coluna e JSON pedem. */
        .ff-mono { font-family: Consolas, "Courier New", monospace; font-size: 12px; }
    </style>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Ferramenta da IA"></asp:Label><small> Administração</small>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="3" TitulodaPagina="Ferramenta" />
        </div>

        <div class="col-lg-12">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>

        <div class="col-lg-12">
            <asp:Panel ID="pnAvisoInterna" runat="server" CssClass="alert alert-info" Visible="false">
                <i class="fa fa-info-circle"></i>
                Esta é uma ferramenta <strong>interna</strong> (execução no código). Você edita os metadados e ativa/desativa,
                mas ela não pode ser excluída e o nome interno não muda.
            </asp:Panel>
            <asp:Panel ID="pnAvisoNova" runat="server" CssClass="alert alert-info" Visible="false">
                <i class="fa fa-info-circle"></i>
                Ferramenta nova. Escolha o tipo de execução <strong>Genérica</strong> para configurá-la 100% aqui (sem programar).
            </asp:Panel>
        </div>

        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-plug"></i> Dados da ferramenta</h3>
                </div>
                <div class="panel-body">
                    <div class="row d-flex fw-w">
                        <div class="col-lg-4 col-md-6 col-sm-12">
                            <div class="form-group">
                                <label>Nome interno</label>
                                <asp:TextBox ID="txtNome" CssClass="form-control ff-mono" runat="server" MaxLength="150" />
                                <span class="help-block">Identificador único (ex.: os_consultar). Não muda depois de criada.</span>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-6 col-sm-12">
                            <div class="form-group">
                                <label>Módulo</label>
                                <asp:TextBox ID="txtModulo" CssClass="form-control" runat="server" MaxLength="80" />
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-6 col-sm-12">
                            <div class="form-group">
                                <label>Permissão necessária (recurso)
                                    <i class="fa fa-question-circle" data-toggle="tooltip" title="Recurso que o usuário precisa ter para a IA poder usar esta ferramenta."></i>
                                </label>
                                <asp:DropDownList ID="ddlIdRecurso" CssClass="form-control" runat="server" />
                            </div>
                        </div>
                    </div>

                    <div class="row">
                        <div class="col-lg-12">
                            <div class="form-group">
                                <label>Descrição (orienta o modelo)</label>
                                <asp:TextBox ID="txtDescricao" CssClass="form-control" runat="server" TextMode="MultiLine" Rows="3" MaxLength="4000" />
                                <span class="help-block">Explica ao modelo o que a ferramenta faz e quando usá-la.</span>
                            </div>
                        </div>
                    </div>

                    <div class="row d-flex fw-w">
                        <div class="col-lg-3 col-md-4 col-sm-6">
                            <div class="form-group">
                                <label>Escopo
                                    <i class="fa fa-question-circle" data-toggle="tooltip" title="READ apenas consulta. WRITE altera dados e exige confirmação do usuário no chat."></i>
                                </label>
                                <asp:DropDownList ID="ddlEscopo" CssClass="form-control" runat="server">
                                    <asp:ListItem Text="Leitura / consulta (READ)" Value="READ" />
                                    <asp:ListItem Text="Alteração (WRITE)" Value="WRITE" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-6">
                            <div class="form-group">
                                <label>Requer confirmação</label>
                                <asp:DropDownList ID="ddlRequerConfirmacao" CssClass="form-control" runat="server">
                                    <asp:ListItem Text="Não" Value="N" />
                                    <asp:ListItem Text="Sim" Value="S" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-6">
                            <div class="form-group">
                                <label>Máx. registros</label>
                                <asp:TextBox ID="txtMaxRegistros" CssClass="form-control" runat="server" MaxLength="4" />
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-6">
                            <div class="form-group">
                                <label>Ativa</label>
                                <asp:DropDownList ID="ddlAtivo" CssClass="form-control" runat="server">
                                    <asp:ListItem Text="Sim" Value="S" />
                                    <asp:ListItem Text="Não" Value="N" />
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-lg-3 col-md-4 col-sm-6">
                            <div class="form-group">
                                <label>Tipo de execução
                                    <i class="fa fa-question-circle" data-toggle="tooltip" title="Genérica = você monta a execução aqui na tela. Interna = a execução vem do código."></i>
                                </label>
                                <asp:DropDownList ID="ddlTipoExecucao" CssClass="form-control" runat="server" ClientIDMode="Static" onchange="FB.toggleTipo()">
                                    <asp:ListItem Text="Genérica (configurada aqui)" Value="G" />
                                    <asp:ListItem Text="Interna (código)" Value="I" />
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <%-- ===== Execução INTERNA: schema em JSON (só p/ ferramentas de código) ===== --%>
        <div id="pnSchemaInterna" class="col-lg-12" style="display: none;">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-code"></i> Schema de parâmetros (JSON)</h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <asp:TextBox ID="txtSchema" CssClass="form-control ff-mono" runat="server" TextMode="MultiLine" Rows="10" />
                        <span class="help-block">JSON Schema dos parâmetros. Usado apenas por ferramentas internas (o schema real vem do código).</span>
                    </div>
                </div>
            </div>
        </div>

        <%-- ===== Execução GENÉRICA: construtor visual ===== --%>
        <div id="pnGenerica" class="col-lg-12" style="display: none;">
            <div class="alert alert-warning">
                <i class="fa fa-exclamation-triangle"></i>
                A ferramenta genérica <strong>executa a procedure que você apontar</strong>. Você é responsável por:
                apontar só para procedures adequadas, <strong>não listar colunas sensíveis</strong> (salário, dados bancários) e,
                em WRITE, garantir que a procedure só altera o que deve. Tudo é auditado.
            </div>

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-database"></i> Execução</h3>
                </div>
                <div class="panel-body">
                    <div class="row d-flex fw-w">
                        <div class="col-lg-8 col-md-8 col-sm-12">
                            <div class="form-group">
                                <label>Procedure</label>
                                <asp:TextBox ID="txtProcedureGenerica" CssClass="form-control ff-mono" runat="server" MaxLength="150" />
                                <span class="help-block">Nome da stored procedure a chamar (ex.: sp_Manipula_tbl_Flow_Clientes).</span>
                            </div>
                        </div>
                        <div class="col-lg-4 col-md-4 col-sm-12">
                            <div class="form-group">
                                <label>Índice da tabela de retorno
                                    <i class="fa fa-question-circle" data-toggle="tooltip" title="Se a procedure devolve mais de um result set, qual deles usar. 0 = o primeiro."></i>
                                </label>
                                <asp:TextBox ID="txtIndiceTabela" CssClass="form-control" runat="server" MaxLength="2" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-list-ol"></i> 1. Argumentos que a IA informa</h3>
                </div>
                <div class="panel-body">
                    <span class="help-block">
                        O que a IA precisa perguntar/deduzir para chamar a ferramenta (ex.: um termo de busca).
                        <strong>No nome use só letras e números</strong> (ex.: <code>dataInicio</code>) — espaço, acento ou <code>_</code> fazem o provider recusar.
                    </span>
                    <table class="table table-condensed">
                        <thead>
                            <tr>
                                <th style="width: 22%">Nome</th>
                                <th style="width: 15%">Tipo</th>
                                <th>Descrição p/ a IA</th>
                                <th style="width: 10%" class="text-center">Obrigatório</th>
                                <th style="width: 5%"></th>
                            </tr>
                        </thead>
                        <tbody id="bldArgsBody"></tbody>
                    </table>
                    <button type="button" class="btn btn-default btn-xs" onclick="FB.addArg()"><i class="fa fa-plus"></i> Adicionar argumento</button>
                </div>
            </div>

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-exchange"></i> 2. Parâmetros da procedure</h3>
                </div>
                <div class="panel-body">
                    <span class="help-block">Como cada parâmetro da SP é preenchido: por um argumento da IA, por um valor fixo, ou pelo usuário logado.</span>
                    <table class="table table-condensed">
                        <thead>
                            <tr>
                                <th style="width: 28%">Parâmetro (@)</th>
                                <th style="width: 24%">Origem</th>
                                <th>Valor</th>
                                <th style="width: 5%"></th>
                            </tr>
                        </thead>
                        <tbody id="bldMapaBody"></tbody>
                    </table>
                    <button type="button" class="btn btn-default btn-xs" onclick="FB.addMapa()"><i class="fa fa-plus"></i> Adicionar parâmetro</button>
                </div>
            </div>

            <div id="pnColunas" class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-columns"></i> 3. Colunas de retorno (para consulta)</h3>
                </div>
                <div class="panel-body">
                    <span class="help-block"><strong>Só o que estiver aqui vai para a IA.</strong> Não liste colunas sensíveis.</span>
                    <table class="table table-condensed">
                        <thead>
                            <tr>
                                <th>Coluna</th>
                                <th style="width: 5%"></th>
                            </tr>
                        </thead>
                        <tbody id="bldColsBody"></tbody>
                    </table>
                    <button type="button" class="btn btn-default btn-xs" onclick="FB.addCol()"><i class="fa fa-plus"></i> Adicionar coluna</button>
                </div>
            </div>

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-flask"></i> 4. Testar</h3>
                </div>
                <div class="panel-body">
                    <button type="button" class="btn btn-default btn-xs" onclick="FB.montarTeste()"><i class="fa fa-refresh"></i> Montar campos de teste</button>
                    <div id="bldTesteBody" class="form-group" style="margin-top: 8px;"></div>
                    <asp:Button ID="cmdTestar" CssClass="btn btn-info btn-sm" runat="server" Text="Testar" OnClick="cmdTestar_Click" OnClientClick="FB.serializar(); FB.serializarTeste();" />
                    <asp:Panel ID="pnResultadoTeste" runat="server" Visible="false" CssClass="form-group" Style="margin-top: 10px;">
                        <asp:Literal ID="litResultadoTeste" runat="server" />
                    </asp:Panel>
                </div>
            </div>
        </div>

        <fieldset class="col-lg-12 form-stacked actions">
            <asp:Button ID="cmdSalvar" CssClass="btn btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" OnClientClick="FB.serializar();" />
            <asp:Button ID="cmdExcluir" CssClass="btn btn-lg btn-danger" runat="server" Text="Excluir" OnClick="cmdExcluir_Click" Visible="false" OnClientClick="return confirm('Excluir esta ferramenta?');" />
            <asp:HyperLink ID="lnkVoltar" runat="server" CssClass="btn btn-lg btn-warning" NavigateUrl="~/App/Paginas/IA/Configuracao.aspx">Voltar</asp:HyperLink>
        </fieldset>

        <asp:HiddenField ID="hddId" runat="server" Value="0" />
        <asp:HiddenField ID="hddArgumentos" runat="server" ClientIDMode="Static" Value="[]" />
        <asp:HiddenField ID="hddMapa" runat="server" ClientIDMode="Static" Value="[]" />
        <asp:HiddenField ID="hddColunas" runat="server" ClientIDMode="Static" Value="[]" />
        <asp:HiddenField ID="hddTesteArgs" runat="server" ClientIDMode="Static" Value="{}" />
    </div>

    <script type="text/javascript">
        var FB = (function () {
            function el(id) { return document.getElementById(id); }
            function esc(s) { return (s == null ? '' : String(s)).replace(/&/g, '&amp;').replace(/"/g, '&quot;').replace(/</g, '&lt;'); }
            function closestTr(node) { while (node && node.tagName !== 'TR') { node = node.parentNode; } return node; }

            function addArg(a) {
                a = a || {};
                var tr = document.createElement('tr');
                tr.innerHTML =
                    '<td><input class="form-control input-sm fb-arg-nome" value="' + esc(a.nome) + '" placeholder="ex: termo"></td>' +
                    '<td><select class="form-control input-sm fb-arg-tipo"><option value="texto">texto</option><option value="numero">número</option></select></td>' +
                    '<td><input class="form-control input-sm fb-arg-desc" value="' + esc(a.descricao) + '" placeholder="descrição para a IA"></td>' +
                    '<td class="text-center"><input type="checkbox" class="fb-arg-obr"></td>' +
                    '<td><button type="button" class="btn btn-default btn-xs" onclick="FB.rm(this)">&times;</button></td>';
                el('bldArgsBody').appendChild(tr);
                if (a.tipo === 'numero') tr.querySelector('.fb-arg-tipo').value = 'numero';
                if (a.obrigatorio) tr.querySelector('.fb-arg-obr').checked = true;
            }

            function addMapa(m) {
                m = m || {};
                var tr = document.createElement('tr');
                tr.innerHTML =
                    '<td><input class="form-control input-sm ff-mono fb-map-param" value="' + esc(m.param) + '" placeholder="@sPesquisa"></td>' +
                    '<td><select class="form-control input-sm fb-map-origem" onchange="FB.hint(this)"><option value="arg">argumento da IA</option><option value="fixo">valor fixo</option><option value="token">usuário logado</option></select></td>' +
                    '<td><select class="form-control input-sm fb-map-arg" onfocus="FB.encherArgs(this)"></select>' +
                    '<input class="form-control input-sm fb-map-valor" value="' + esc(m.valor) + '" placeholder="valor fixo"></td>' +
                    '<td><button type="button" class="btn btn-default btn-xs" onclick="FB.rm(this)">&times;</button></td>';
                el('bldMapaBody').appendChild(tr);
                var sel = tr.querySelector('.fb-map-origem');
                if (m.origem) sel.value = m.origem;
                encherArgs(tr.querySelector('.fb-map-arg'), m.valor || '');
                hint(sel);
            }

            function addCol(c) {
                var tr = document.createElement('tr');
                tr.innerHTML =
                    '<td><input class="form-control input-sm ff-mono fb-col-nome" value="' + esc(c) + '" placeholder="ex: idPedido"></td>' +
                    '<td><button type="button" class="btn btn-default btn-xs" onclick="FB.rm(this)">&times;</button></td>';
                el('bldColsBody').appendChild(tr);
            }

            // Nomes dos argumentos declarados na secao 1
            function argNomes() {
                var out = [];
                rows('bldArgsBody').forEach(function (tr) {
                    var n = tr.querySelector('.fb-arg-nome').value.trim();
                    if (n && out.indexOf(n) < 0) out.push(n);
                });
                return out;
            }

            // Repovoa o combo. Um valor que nao existe mais fica visivel e marcado, em vez de sumir calado.
            function encherArgs(sel, manter) {
                var atual = (manter != null) ? manter : sel.value;
                var nomes = argNomes();
                sel.innerHTML = '';

                if (!nomes.length && !atual) {
                    sel.innerHTML = '<option value="">(defina um argumento na seção 1)</option>';
                    return;
                }

                var lista = nomes.slice();
                if (atual && lista.indexOf(atual) < 0) lista.unshift(atual);

                for (var i = 0; i < lista.length; i++) {
                    var o = document.createElement('option');
                    o.value = lista[i];
                    o.textContent = lista[i] + (nomes.indexOf(lista[i]) < 0 ? '  ← não existe mais!' : '');
                    sel.appendChild(o);
                }
                sel.value = atual || nomes[0];
            }

            function hint(sel) {
                var tr = closestTr(sel);
                var selArg = tr.querySelector('.fb-map-arg');
                var val = tr.querySelector('.fb-map-valor');

                if (sel.value === 'arg') {
                    encherArgs(selArg);
                    selArg.style.display = '';
                    val.style.display = 'none';
                } else if (sel.value === 'fixo') {
                    selArg.style.display = 'none';
                    val.style.display = '';
                    val.readOnly = false;
                    val.placeholder = 'valor fixo';
                    if (val.value === 'usuarioLogado') val.value = '';
                } else {
                    selArg.style.display = 'none';
                    val.style.display = '';
                    val.value = 'usuarioLogado';
                    val.readOnly = true;
                }
            }

            function rm(btn) { var tr = closestTr(btn); tr.parentNode.removeChild(tr); }

            function rows(id) { return el(id) ? Array.prototype.slice.call(el(id).querySelectorAll('tr')) : []; }

            function serializar() {
                var args = [];
                rows('bldArgsBody').forEach(function (tr) {
                    var nome = tr.querySelector('.fb-arg-nome').value.trim();
                    if (!nome) return;
                    args.push({ nome: nome, tipo: tr.querySelector('.fb-arg-tipo').value, descricao: tr.querySelector('.fb-arg-desc').value.trim(), obrigatorio: tr.querySelector('.fb-arg-obr').checked });
                });
                var mapa = [];
                rows('bldMapaBody').forEach(function (tr) {
                    var param = tr.querySelector('.fb-map-param').value.trim();
                    if (!param) return;
                    var origem = tr.querySelector('.fb-map-origem').value;
                    var valor = (origem === 'arg')
                        ? tr.querySelector('.fb-map-arg').value
                        : tr.querySelector('.fb-map-valor').value.trim();
                    mapa.push({ param: param, origem: origem, valor: valor });
                });
                var cols = [];
                rows('bldColsBody').forEach(function (tr) {
                    var c = tr.querySelector('.fb-col-nome').value.trim();
                    if (c) cols.push(c);
                });
                el('hddArgumentos').value = JSON.stringify(args);
                el('hddMapa').value = JSON.stringify(mapa);
                el('hddColunas').value = JSON.stringify(cols);
            }

            function montarTeste() {
                var cont = el('bldTesteBody');
                cont.innerHTML = '';
                var vals = {};
                try { vals = JSON.parse(el('hddTesteArgs').value || '{}') || {}; } catch (e) { vals = {}; }
                var vistos = {};
                rows('bldArgsBody').forEach(function (tr) {
                    var nome = tr.querySelector('.fb-arg-nome').value.trim();
                    if (!nome || vistos[nome]) return;
                    vistos[nome] = true;
                    var v = (vals[nome] != null) ? vals[nome] : '';
                    var div = document.createElement('div');
                    div.className = 'form-group';
                    div.innerHTML = '<label>' + esc(nome) + '</label>' +
                        '<input class="form-control input-sm fb-teste-val" data-arg="' + esc(nome) + '" value="' + esc(v) + '" placeholder="valor de exemplo">';
                    cont.appendChild(div);
                });
                if (!cont.innerHTML) cont.innerHTML = '<span class="help-block">Nenhum argumento definido — a ferramenta não pede parâmetros da IA.</span>';
            }

            function serializarTeste() {
                var o = {};
                var ins = el('bldTesteBody').querySelectorAll('.fb-teste-val');
                for (var i = 0; i < ins.length; i++) { o[ins[i].getAttribute('data-arg')] = ins[i].value; }
                el('hddTesteArgs').value = JSON.stringify(o);
            }

            function toggleTipo() {
                var g = el('ddlTipoExecucao').value === 'G';
                el('pnGenerica').style.display = g ? '' : 'none';
                el('pnSchemaInterna').style.display = g ? 'none' : '';
            }

            function parse(id, def) { try { return JSON.parse(el(id).value || def); } catch (e) { return JSON.parse(def); } }

            function render() {
                (parse('hddArgumentos', '[]') || []).forEach(addArg);
                (parse('hddMapa', '[]') || []).forEach(addMapa);
                (parse('hddColunas', '[]') || []).forEach(addCol);
                toggleTipo();
                montarTeste();
            }

            return { addArg: addArg, addMapa: addMapa, addCol: addCol, rm: rm, hint: hint, encherArgs: encherArgs, serializar: serializar, serializarTeste: serializarTeste, montarTeste: montarTeste, toggleTipo: toggleTipo, render: render };
        })();

        // Usa $ e nao jQuery: no main.master os dois globais apontam para instancias diferentes.
        if (window.$) { $(function () { FB.render(); }); } else { document.addEventListener('DOMContentLoaded', function () { FB.render(); }); }
    </script>
</asp:Content>
