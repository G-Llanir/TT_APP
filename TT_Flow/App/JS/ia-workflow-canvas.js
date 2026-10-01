(function () {
    'use strict';

    var WF_OPERADORES = [['=', 'igual a'], ['!=', 'diferente de'], ['>', 'maior que'], ['<', 'menor que'], ['>=', 'maior ou igual'], ['<=', 'menor ou igual'], ['contem', 'contém']];
    var WF_ORIGENS = [['entrada', 'Entrada do fluxo'], ['passo', 'Saída de nó'], ['fixo', 'Valor fixo'], ['token', 'Usuário logado']];
    var WF_TIPOS = [['string', 'Texto'], ['integer', 'Inteiro'], ['number', 'Decimal'], ['array', 'Lista'], ['json', 'JSON']];
    var WF_OBRIGATORIEDADES = [['obrigatorio', 'Obrigatória'], ['opcional', 'Opcional'], ['condicional', 'Condicional']];
    var catalogo = window.WF_CATALOGO || [];
    var ids = window.WF_IDS || {};
    var urls = window.WF_URLS || {};
    var estado = { entradas: [], nos: [] };
    var editor = null;
    var renderizando = false;
    var selectedNoId = '';
    var dfParaNo = {};
    var noParaDf = {};
    var nodeSeq = 2;
    var testeModo = 'inputs';
    var execRealId = 0;
    var execAcaoEmAndamento = false;
    var ultimoTrace = [];
    var ultimoTipoPausa = '';
    var replayTimer = null;
    var replayAtivo = false;
    var popupOcultoPara = '';
    var popupEntradasNoId = '';
    var erroPopupOcultoPara = '';
    var entradasRuntimeExtras = {};
    var popupRuntimeAtivo = null;
    var parametroModalNoId = '';
    var layoutWorkflowPreparado = false;
    var inspectorMinimizado = false;
    var modoMoverCanvas = false;
    var canvasPanAtivo = false;
    var canvasPanOrigem = null;
    var canvasPanPendente = null;
    var canvasPanRaf = null;
    var espacoPressionado = false;
    var jsonViewerCache = {};
    var jsonViewerSeq = 0;
    var jsonViewerAtual = null; // id exibido no momento no modal, para o botao Baixar arquivo

    function el(id) { return document.getElementById(id); }
    function esc(s) { return String(s == null ? '' : s).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;').replace(/"/g, '&quot;').replace(/'/g, '&#39;'); }
    function clone(o) { return JSON.parse(JSON.stringify(o == null ? {} : o)); }
    function str(v) { return v == null ? '' : String(v); }
    function trim(v) { return str(v).replace(/^\s+|\s+$/g, ''); }
    function jsonValor(v) {
        if (v == null) return '';
        if (typeof v === 'object') return v;
        var texto = String(v);
        if (!trim(texto)) return texto;
        try { return JSON.parse(texto); } catch (e) { return texto; }
    }
    function jsonFormatado(v) {
        var valor = jsonValor(v);
        if (valor && typeof valor === 'object') return JSON.stringify(valor, null, 2);
        return valor == null ? '' : String(valor);
    }
    function bonito(s) { return jsonFormatado(s); }
    function jsonColorido(v) {
        var texto = jsonFormatado(v);
        var re = /("(?:\\.|[^"\\])*"(?=\s*:)|"(?:\\.|[^"\\])*"|-?\d+(?:\.\d+)?(?:[eE][+\-]?\d+)?|\btrue\b|\bfalse\b|\bnull\b)/g;
        var html = '';
        var ultimo = 0;
        var m;
        while ((m = re.exec(texto)) !== null) {
            html += esc(texto.slice(ultimo, m.index));
            var token = m[0];
            var resto = texto.slice(m.index + token.length);
            var cls = 'wf-json-number';
            if (token.charAt(0) === '"') cls = /^\s*:/.test(resto) ? 'wf-json-key' : 'wf-json-string';
            else if (token === 'true' || token === 'false') cls = 'wf-json-bool';
            else if (token === 'null') cls = 'wf-json-null';
            html += '<span class="' + cls + '">' + esc(token) + '</span>';
            ultimo = re.lastIndex;
        }
        html += esc(texto.slice(ultimo));
        return html;
    }
    function registrarJsonViewer(titulo, valor, nomeArquivo) {
        var id = 'json_' + (++jsonViewerSeq);
        jsonViewerCache[id] = { titulo: titulo || 'JSON', valor: valor, nomeArquivo: nomeArquivo || '' };
        return id;
    }
    function renderJsonTrace(titulo, valor) {
        if (valor == null || valor === '') return '';
        var id = registrarJsonViewer(titulo, valor);
        return '<div class="wf-json-box">' +
            '<div class="wf-json-head">' +
                '<strong>' + esc(titulo) + '</strong>' +
                '<button type="button" class="btn btn-xs btn-default" onclick="wfAbrirJsonViewer(\'' + esc(id) + '\')"><i class="fa fa-code"></i> Visualizar JSON</button>' +
            '</div>' +
            '<pre class="wf-json-code">' + jsonColorido(valor) + '</pre>' +
            '</div>';
    }
    function traceCampo(t, nome) {
        t = t || {};
        if (nome === 'Entrada') return t.EntradaCompleta || t.entradaCompleta || t.Entrada || t.entrada || '';
        if (nome === 'Saida') return t.SaidaCompleta || t.saidaCompleta || t.Saida || t.saida || '';
        return t[nome] || t[nome.charAt(0).toLowerCase() + nome.slice(1)] || '';
    }
    function erroTratadoTrace(t) {
        return boolCfg(t && (t.ErroTratado || t.erroTratado));
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
    function catalogoPorNome(nome) { for (var i = 0; i < catalogo.length; i++) if (catalogo[i].nome === nome) return catalogo[i]; return null; }
    function paramMeta(cat, nome) { if (!cat || !cat.params) return null; for (var i = 0; i < cat.params.length; i++) if (cat.params[i].nome === nome) return cat.params[i]; return null; }
    function ferramentaLabel(c) { return c ? (c.nome + (c.escopo === 'WRITE' ? ' (escrita)' : '')) : ''; }
    function opcoesFerramenta(valor, textoVazio) {
        var h = '<option value="">' + esc(textoVazio || '- escolha a ferramenta -') + '</option>';
        var selecionadaIncluida = !valor;
        catalogo.forEach(function (c) {
            if (c.nome === valor) selecionadaIncluida = true;
            h += '<option value="' + esc(c.nome) + '"' + (valor === c.nome ? ' selected' : '') + '>' + esc(ferramentaLabel(c)) + '</option>';
        });
        if (valor && !selecionadaIncluida) {
            h += '<option value="' + esc(valor) + '" selected>' + esc(valor) + '</option>';
        }
        return h;
    }
    function renderFerramentaPicker(no, textoVazio) {
        return '<div class="wf-tool-picker">' +
            '<select class="form-control input-sm Caixa_Selecao wf-tool-select" data-noid="' + esc(no.id) + '" data-placeholder="' + esc(textoVazio || '- escolha a ferramenta -') + '" onchange="wfSelFerramentaNo(\'' + esc(no.id) + '\',this.value)">' + opcoesFerramenta(no.ferramenta || '', textoVazio) + '</select>' +
            '</div>';
    }
    function aplicarSelectsFerramenta(root) {
        var jq = (window.$ && window.$.fn && window.$.fn.chosen) ? window.$ :
            ((window.jQuery && window.jQuery.fn && window.jQuery.fn.chosen) ? window.jQuery : null);
        if (!jq) return;
        var $root = root ? jq(root) : jq(document);
        $root.find('select.wf-tool-select').each(function () {
            var $select = jq(this);
            $select.addClass('Caixa_Selecao');
            if ($select.data('chosen')) {
                $select.trigger('chosen:updated');
                return;
            }
            if (typeof window.initializeChosen === 'function' && window.$ && window.$.fn && window.$.fn.chosen) window.initializeChosen();
            else $select.chosen({ width: '100%' });
            $select.trigger('chosen:updated');
        });
    }
    function labelTipoEntrada(tipo) {
        for (var i = 0; i < WF_TIPOS.length; i++) if (WF_TIPOS[i][0] === tipo) return WF_TIPOS[i][1];
        return tipo || 'Texto';
    }
    function labelObrigatoriedade(valor) {
        valor = normalizarObrigatoriedade(valor);
        for (var i = 0; i < WF_OBRIGATORIEDADES.length; i++) if (WF_OBRIGATORIEDADES[i][0] === valor) return WF_OBRIGATORIEDADES[i][1];
        return 'Obrigatória';
    }
    function normalizarObrigatoriedade(valor) {
        valor = trim(valor || 'obrigatorio').toLowerCase();
        if (valor === 'obrigatoria') valor = 'obrigatorio';
        if (valor !== 'obrigatorio' && valor !== 'opcional' && valor !== 'condicional') valor = 'obrigatorio';
        return valor;
    }
    function normalizarOpcoesEntrada(opcoes) {
        var resultado = [];
        (Array.isArray(opcoes) ? opcoes : []).forEach(function (opcao) {
            var valor = trim(opcao && typeof opcao === 'object' ? opcao.valor : opcao);
            if (!valor) return;
            var rotulo = trim(opcao && typeof opcao === 'object' ? opcao.rotulo : opcao) || valor;
            resultado.push({ valor: valor, rotulo: rotulo });
        });
        return resultado;
    }
    function opcoesEntradaTexto(opcoes) {
        return normalizarOpcoesEntrada(opcoes).map(function (opcao) {
            return opcao.valor + (opcao.rotulo !== opcao.valor ? '|' + opcao.rotulo : '');
        }).join('\n');
    }
    function opcoesEntradaDoTexto(texto) {
        return String(texto || '').split(/\r?\n/).map(function (linha) {
            var partes = linha.split('|');
            var valor = trim(partes.shift());
            var rotulo = trim(partes.join('|')) || valor;
            return valor ? { valor: valor, rotulo: rotulo } : null;
        }).filter(function (opcao) { return !!opcao; });
    }
    function optionsEntradaHtml(opcoes, valor, incluirVazio) {
        var html = incluirVazio === false ? '' : '<option value="">Selecione...</option>';
        normalizarOpcoesEntrada(opcoes).forEach(function (opcao) {
            html += '<option value="' + esc(opcao.valor) + '"' + (String(valor == null ? '' : valor) === opcao.valor ? ' selected' : '') + '>' + esc(opcao.rotulo) + '</option>';
        });
        return html;
    }
    function normalizarEntrada(e) {
        e = e || {};
        e.nome = trim(e.nome);
        e.tipo = e.tipo || 'string';
        e.grupo = trim(e.grupo || 'Geral');
        e.obrigatoriedade = normalizarObrigatoriedade(e.obrigatoriedade);
        e.descricao = trim(e.descricao || '');
        e.opcoes = normalizarOpcoesEntrada(e.opcoes);
        return e;
    }
    function entradaEhInicial(e) {
        return normalizarObrigatoriedade(e && e.obrigatoriedade) !== 'condicional';
    }
    function entradaEhObrigatoria(e) {
        return normalizarObrigatoriedade(e && e.obrigatoriedade) === 'obrigatorio';
    }
    function entradaOptionLabel(e) {
        e = normalizarEntrada(clone(e || {}));
        var sufixo = e.grupo && e.grupo !== 'Geral' ? ' - ' + e.grupo : '';
        if (e.obrigatoriedade === 'condicional') sufixo += ' (condicional)';
        else if (e.obrigatoriedade === 'opcional') sufixo += ' (opcional)';
        return e.nome + sufixo;
    }
    function tipoParametro(par) {
        return String((par && par.tipo) || 'string').toLowerCase();
    }
    function parametroEhTexto(par) {
        var tipo = tipoParametro(par);
        return tipo.indexOf('string') >= 0 || tipo === 'texto';
    }
    function parametroEhNumero(par) {
        var tipo = tipoParametro(par);
        return tipo.indexOf('number') >= 0 || tipo.indexOf('integer') >= 0 || tipo.indexOf('decimal') >= 0;
    }
    function enumParametro(par) {
        return par && par.enum && par.enum.length ? par.enum : [];
    }
    function permiteVazioParametro(par) {
        if (!parametroEhTexto(par)) return false;
        if (String(par && par.minLength) === '0') return true;
        return /string vazia|texto vazio|valor vazio|vazia|vazio/i.test(String((par && par.descricao) || ''));
    }
    function valorParametroPreenchido(m, par) {
        if (!m) return false;
        if (boolCfg(m.vazioExplicito) && permiteVazioParametro(par)) return true;
        return trim(m.valor) !== '';
    }
    function resumoOrigemParametro(m) {
        if (!m) return 'não configurado';
        if (boolCfg(m.vazioExplicito)) return 'vazio';
        if (m.origem === 'entrada') return 'entrada.' + (m.valor || '?');
        if (m.origem === 'passo') return m.valor || 'saída de nó';
        if (m.origem === 'token') return 'usuário logado';
        return m.valor === '' ? 'valor fixo vazio' : String(m.valor);
    }
    function tipoInfo(tipo) {
        tipo = trim(tipo).toLowerCase();
        if (tipo === 'inicio') return { titulo: 'Início', icone: 'fa-play', classe: 'wf-node-inicio', entradas: 0, saidas: 1 };
        if (tipo === 'fim') return { titulo: 'Fim', icone: 'fa-flag-checkered', classe: 'wf-node-fim', entradas: 1, saidas: 0 };
        if (tipo === 'condicao') return { titulo: 'Condição', icone: 'fa-code-fork', classe: 'wf-node-condicao', entradas: 1, saidas: 2 };
        if (tipo === 'definir') return { titulo: 'Definir valor', icone: 'fa-pencil-square-o', classe: 'wf-node-definir', entradas: 1, saidas: 1 };
        if (tipo === 'ia') return { titulo: 'IA', icone: 'fa-magic', classe: 'wf-node-ia', entradas: 1, saidas: 2 };
        if (tipo === 'merge') return { titulo: 'Merge', icone: 'fa-compress', classe: 'wf-node-merge', entradas: 2, saidas: 1 };
        if (tipo === 'aprovacao') return { titulo: 'Aprovação', icone: 'fa-check-square-o', classe: 'wf-node-aprovacao', entradas: 1, saidas: 2 };
        if (tipo === 'loop') return { titulo: 'Loop', icone: 'fa-repeat', classe: 'wf-node-loop', entradas: 1, saidas: 2 };
        if (tipo === 'espera') return { titulo: 'Espera', icone: 'fa-clock-o', classe: 'wf-node-espera', entradas: 1, saidas: 1 };
        return { titulo: 'Ferramenta', icone: 'fa-plug', classe: 'wf-node-ferramenta', entradas: 1, saidas: 2 };
    }
    function noPorId(id) {
        for (var i = 0; i < estado.nos.length; i++) if (estado.nos[i].id === id) return estado.nos[i];
        return null;
    }
    function existeNo(id) { return !!noPorId(id); }
    function saidaPrincipal(no) {
        if (!no) return [''];
        if (no.tipo === 'condicao') return [no.seVerdadeiro, no.seFalso];
        if (no.tipo === 'aprovacao') return [no.proximo, no.seRejeitado];
        if (no.tipo === 'loop') return [no.proximo, no.aoConcluir];
        if (no.tipo === 'ferramenta' || no.tipo === 'ia') return [no.proximo, no.emErro];
        return [no.proximo];
    }
    function tituloNo(no) {
        if (!no) return '';
        if (no.tipo === 'inicio') return 'Início';
        if (no.tipo === 'fim') return 'Fim';
        if (no.tipo === 'ferramenta') return no.ferramenta || 'Ferramenta';
        if (no.tipo === 'condicao') return no.campo ? ('se ' + no.campo) : 'Condição';
        if (no.tipo === 'definir') return 'valor';
        if (no.tipo === 'ia') return no.titulo || 'IA';
        if (no.tipo === 'merge') return no.titulo || 'Merge';
        if (no.tipo === 'aprovacao') return no.titulo || 'Aprovação humana';
        if (no.tipo === 'loop') return no.titulo || 'Loop';
        if (no.tipo === 'espera') return no.titulo || 'Espera';
        return no.tipo || 'nó';
    }
    function tracePorNo(id) {
        for (var i = ultimoTrace.length - 1; i >= 0; i--) if (ultimoTrace[i] && ultimoTrace[i].No === id) return ultimoTrace[i];
        return null;
    }
    function ultimoErroTrace(trace) {
        trace = trace || [];
        for (var i = trace.length - 1; i >= 0; i--) {
            if (trace[i] && String(trace[i].Status || '').toUpperCase() === 'ERRO' && trace[i].No) return trace[i];
        }
        return null;
    }
    function parametrosPendentes(no, params) {
        var pendentes = 0;
        (params || []).forEach(function (par) {
            if (!par.obrigatorio) return;
            var m = (no.entradas || []).filter(function (x) { return x.param === par.nome; })[0];
            if (!valorParametroPreenchido(m, par)) pendentes++;
        });
        return pendentes;
    }
    function propRuntime(obj, nome) {
        if (!obj || !nome || typeof obj !== 'object') return null;
        if (Object.prototype.hasOwnProperty.call(obj, nome)) return { existe: true, valor: obj[nome] };
        var alvo = String(nome).toLowerCase();
        for (var k in obj) {
            if (Object.prototype.hasOwnProperty.call(obj, k) && String(k).toLowerCase() === alvo) {
                return { existe: true, valor: obj[k] };
            }
        }
        return null;
    }
    function nomeEntradaBase(nome) {
        nome = trim(nome || '');
        if (nome.toLowerCase().indexOf('entrada.') === 0) nome = nome.slice(8);
        var ponto = nome.indexOf('.');
        var colchete = nome.indexOf('[');
        var corte = -1;
        if (ponto >= 0 && colchete >= 0) corte = Math.min(ponto, colchete);
        else if (ponto >= 0) corte = ponto;
        else if (colchete >= 0) corte = colchete;
        return corte >= 0 ? nome.slice(0, corte) : nome;
    }
    function entradaDefinicao(nome) {
        var base = nomeEntradaBase(nome);
        var alvo = String(nome || '').toLowerCase();
        var alvoBase = String(base || '').toLowerCase();
        for (var i = 0; i < estado.entradas.length; i++) {
            if (!estado.entradas[i]) continue;
            var nomeAtual = String(estado.entradas[i].nome || '').toLowerCase();
            if (nomeAtual === alvo || nomeAtual === alvoBase) return estado.entradas[i];
        }
        return null;
    }
    function converterEntradaRuntime(valor, tipo) {
        tipo = String(tipo || 'string').toLowerCase();
        if (tipo === 'integer') return parseInt(valor || '0', 10) || 0;
        if (tipo === 'number') return parseFloat(String(valor || '0').replace(',', '.')) || 0;
        if (tipo === 'array' || tipo === 'json') {
            if (valor == null || trim(valor) === '') return tipo === 'array' ? [] : {};
            if (typeof valor === 'object') return valor;
            try {
                var parsed = JSON.parse(String(valor));
                if (tipo === 'array') return Array.isArray(parsed) ? parsed : [];
                return parsed;
            } catch (e) {
                return tipo === 'array' ? [] : {};
            }
        }
        return valor == null ? '' : String(valor);
    }
    function entradaValorVazio(valor, tipo) {
        tipo = String(tipo || 'string').toLowerCase();
        if (tipo === 'array') return !converterEntradaRuntime(valor, tipo).length;
        if (tipo === 'json') {
            var obj = converterEntradaRuntime(valor, tipo);
            if (!obj) return true;
            if (Array.isArray(obj)) return !obj.length;
            if (typeof obj === 'object') return !Object.keys(obj).length;
            return trim(obj) === '';
        }
        return trim(valor) === '';
    }
    function normalizarRetries(valor) {
        var n = parseInt(valor || '0', 10);
        if (isNaN(n) || n < 0) n = 0;
        if (n > 3) n = 3;
        return n;
    }
    function normalizarIntervaloRetry(valor) {
        var n = parseInt(valor || '0', 10);
        if (isNaN(n) || n < 0) n = 0;
        if (n > 10) n = 10;
        return n;
    }
    function boolCfg(valor) {
        return valor === true || valor === 1 || valor === '1' || String(valor || '').toLowerCase() === 'true' || String(valor || '').toUpperCase() === 'S';
    }
    function resolverSegmentoRuntime(atual, segmento) {
        var partes = String(segmento || '').match(/[^\[\]]+|\[\d+\]/g) || [];
        if (!partes.length) return { existe: false, valor: null };
        for (var i = 0; i < partes.length; i++) {
            var parte = partes[i];
            if (/^\[\d+\]$/.test(parte)) {
                var idx = parseInt(parte.slice(1, -1), 10);
                if (!Array.isArray(atual) || idx < 0 || idx >= atual.length) return { existe: false, valor: null };
                atual = atual[idx];
                continue;
            }
            var prop = propRuntime(atual, parte);
            if (!prop) return { existe: false, valor: null };
            atual = prop.valor;
        }
        return { existe: true, valor: atual };
    }
    function valorRuntimePorReferencia(obj, nome) {
        if (!nome || !obj) return { existe: false, valor: null };
        var prop = propRuntime(obj, nome);
        if (prop) return prop;

        var ref = trim(nome);
        if (ref.toLowerCase().indexOf('entrada.') === 0) ref = ref.slice(8);
        prop = propRuntime(obj, ref);
        if (prop) return prop;

        var atual = obj;
        var partes = ref.split('.');
        for (var i = 0; i < partes.length; i++) {
            var seg = resolverSegmentoRuntime(atual, partes[i]);
            if (!seg.existe) return { existe: false, valor: null };
            atual = seg.valor;
        }
        return { existe: true, valor: atual };
    }
    function entradaFoiInformadaRuntime(obj, nome) {
        return valorRuntimePorReferencia(obj, nome).existe;
    }
    function temValorRuntime(obj, nome) {
        var resolvido = valorRuntimePorReferencia(obj, nome);
        if (!resolvido.existe) return false;
        var v = resolvido.valor;
        if (v == null) return false;
        if (typeof v === 'string') return trim(v) !== '';
        if (Array.isArray(v)) return v.length > 0;
        if (typeof v === 'object') return Object.keys(v).length > 0;
        return true;
    }
    function limparReplay() {
        if (replayTimer) {
            clearTimeout(replayTimer);
            replayTimer = null;
        }
        replayAtivo = false;
    }

    function grafoPadrao() {
        return {
            versao: 2,
            entradas: [],
            nos: [
                { id: 'n1', tipo: 'inicio', proximo: 'nFim', x: 80, y: 230 },
                { id: 'nFim', tipo: 'fim', x: 390, y: 230 }
            ]
        };
    }

    function normalizarNo(no, idx) {
        no = clone(no);
        no.id = trim(no.id) || novoId();
        no.tipo = trim(no.tipo || 'ferramenta').toLowerCase();
        if (no.tipo !== 'inicio' && no.tipo !== 'fim' && no.tipo !== 'ferramenta' && no.tipo !== 'condicao' && no.tipo !== 'definir' && no.tipo !== 'ia' && no.tipo !== 'merge' && no.tipo !== 'aprovacao' && no.tipo !== 'loop' && no.tipo !== 'espera') {
            no.tipo = 'ferramenta';
        }
        if (typeof no.x === 'undefined' && typeof no.pos_x !== 'undefined') no.x = no.pos_x;
        if (typeof no.y === 'undefined' && typeof no.pos_y !== 'undefined') no.y = no.pos_y;
        if (isNaN(parseFloat(no.x))) no.x = 80 + (idx * 250);
        if (isNaN(parseFloat(no.y))) no.y = 230;
        no.x = parseFloat(no.x);
        no.y = parseFloat(no.y);
        no.minimizado = boolCfg(no.minimizado);
        no.conexoesVisuais = no.conexoesVisuais && typeof no.conexoesVisuais === 'object' ? no.conexoesVisuais : {};

        if (no.tipo === 'ferramenta') {
            no.ferramenta = no.ferramenta || '';
            no.entradas = no.entradas || [];
            no.proximo = no.proximo || '';
            no.emErro = no.emErro || '';
            no.retries = normalizarRetries(no.retries);
            no.intervaloRetrySegundos = normalizarIntervaloRetry(no.intervaloRetrySegundos);
            no.backoffRetry = boolCfg(no.backoffRetry);
        } else if (no.tipo === 'condicao') {
            no.campo = no.campo || '';
            no.operador = no.operador || '=';
            no.valorOrigem = no.valorOrigem || 'fixo';
            no.valor = no.valor || '';
            no.seVerdadeiro = no.seVerdadeiro || '';
            no.seFalso = no.seFalso || '';
        } else if (no.tipo === 'definir') {
            no.origem = no.origem || 'fixo';
            no.valor = no.valor || '';
            no.proximo = no.proximo || '';
        } else if (no.tipo === 'ia') {
            no.titulo = no.titulo || 'IA';
            no.prompt = no.prompt || '';
            no.entradas = no.entradas || [];
            no.modoSaida = 'json_texto';
            no.schemaSaida = no.schemaSaida || '{}';
            no.proximo = no.proximo || '';
            no.emErro = no.emErro || '';
            no.retries = normalizarRetries(no.retries);
            no.intervaloRetrySegundos = normalizarIntervaloRetry(no.intervaloRetrySegundos);
            no.backoffRetry = boolCfg(no.backoffRetry);
        } else if (no.tipo === 'merge') {
            no.titulo = no.titulo || 'Merge';
            no.proximo = no.proximo || '';
        } else if (no.tipo === 'aprovacao') {
            no.titulo = no.titulo || 'Aprovação humana';
            no.mensagem = no.mensagem || '';
            no.proximo = no.proximo || '';
            no.seRejeitado = no.seRejeitado || '';
        } else if (no.tipo === 'loop') {
            no.titulo = no.titulo || 'Loop';
            no.modoLoop = no.modoLoop || no.modo || 'contador';
            if (no.modoLoop !== 'lista') no.modoLoop = 'contador';
            no.listaOrigem = no.listaOrigem || 'entrada';
            no.listaValor = no.listaValor || '';
            no.limite = no.limite || 3;
            no.proximo = no.proximo || '';
            no.aoConcluir = no.aoConcluir || '';
        } else if (no.tipo === 'espera') {
            no.titulo = no.titulo || 'Espera';
            no.modo = no.modo || 'manual';
            no.dataHora = no.dataHora || '';
            no.evento = no.evento || '';
            no.mensagem = no.mensagem || '';
            no.proximo = no.proximo || '';
        } else if (no.tipo === 'inicio') {
            no.proximo = no.proximo || '';
        }
        return no;
    }

    function normalizarPontosConexao(pontos) {
        var saida = [];
        (pontos || []).forEach(function (p) {
            if (!p) return;
            var x = parseFloat(p.pos_x != null ? p.pos_x : p.x);
            var y = parseFloat(p.pos_y != null ? p.pos_y : p.y);
            if (!isNaN(x) && !isNaN(y)) saida.push({ pos_x: x, pos_y: y });
        });
        return saida;
    }

    function conexaoVisual(no, saida, destino) {
        var cfg = no && no.conexoesVisuais ? no.conexoesVisuais[saida] : null;
        if (!cfg || (cfg.destino && cfg.destino !== destino)) return null;
        var pontos = normalizarPontosConexao(cfg.pontos || cfg.points);
        return pontos.length ? { input: cfg.input || 'input_1', pontos: pontos } : null;
    }

    function registrarConexaoVisual(no, saida, destino, entrada, pontos) {
        if (!no) return;
        if (!no.conexoesVisuais || typeof no.conexoesVisuais !== 'object') no.conexoesVisuais = {};
        var pts = normalizarPontosConexao(pontos);
        if (!pts.length) {
            delete no.conexoesVisuais[saida];
            return;
        }
        no.conexoesVisuais[saida] = {
            destino: destino,
            input: entrada || 'input_1',
            pontos: pts
        };
    }

    function pontosConexaoDom(dfOrigem, dfDestino, saida, entrada) {
        var pontos = [];
        if (!editor || !dfOrigem || !dfDestino || !saida || !entrada) return pontos;
        var canvas = el('wfCanvas');
        if (!canvas) return pontos;
        var origemCls = 'node_out_node-' + dfOrigem;
        var destinoCls = 'node_in_node-' + dfDestino;
        var linhas = canvas.querySelectorAll('.connection');
        for (var i = 0; i < linhas.length; i++) {
            var linha = linhas[i];
            if (!linha.classList.contains(origemCls) || !linha.classList.contains(destinoCls) || !linha.classList.contains(saida) || !linha.classList.contains(entrada)) continue;
            var circles = linha.querySelectorAll('.point');
            for (var p = 0; p < circles.length; p++) {
                var x = parseFloat(circles[p].getAttribute('cx'));
                var y = parseFloat(circles[p].getAttribute('cy'));
                if (!isNaN(x) && !isNaN(y)) pontos.push({ pos_x: x, pos_y: y });
            }
            break;
        }
        return pontos;
    }

    function carregarGrafo() {
        var raw = el(ids.hddGrafo) ? el(ids.hddGrafo).value : '';
        var grafo;
        try { grafo = JSON.parse(raw || '{}'); } catch (e) { grafo = {}; }
        if (!grafo.nos || !grafo.nos.length) grafo = grafoPadrao();

        estado.entradas = Array.isArray(grafo.entradas) ? grafo.entradas.map(function (e) { return normalizarEntrada(e); }) : [];
        estado.nos = [];
        var precisaLayout = !grafo.versao;
        for (var i = 0; i < (grafo.nos || []).length; i++) {
            if (typeof grafo.nos[i].x === 'undefined' || typeof grafo.nos[i].y === 'undefined') precisaLayout = true;
            estado.nos.push(normalizarNo(grafo.nos[i], i));
        }
        garantirInicioFim();
        atualizarSequencia();
        if (precisaLayout) autoLayout();
    }

    function garantirInicioFim() {
        var temInicio = false, temFim = false;
        for (var i = 0; i < estado.nos.length; i++) {
            if (estado.nos[i].tipo === 'inicio') temInicio = true;
            if (estado.nos[i].tipo === 'fim') temFim = true;
        }
        if (!temInicio) estado.nos.unshift({ id: 'n1', tipo: 'inicio', proximo: 'nFim', x: 80, y: 230 });
        if (!temFim) estado.nos.push({ id: 'nFim', tipo: 'fim', x: 400, y: 230 });
    }

    function atualizarSequencia() {
        var max = 1;
        for (var i = 0; i < estado.nos.length; i++) {
            var m = /^n(\d+)$/i.exec(estado.nos[i].id || '');
            if (m) max = Math.max(max, parseInt(m[1], 10));
        }
        nodeSeq = Math.max(2, max + 1);
    }

    function novoId() {
        var id;
        do { id = 'n' + (nodeSeq++); } while (existeNo(id) || id === 'nFim');
        return id;
    }

    function criarNo(tipo, x, y) {
        tipo = tipo || 'ferramenta';
        if (tipo === 'inicio') {
            var inicio = primeiroNoTipo('inicio');
            if (inicio) return inicio;
        }
        if (tipo === 'fim') {
            var fim = primeiroNoTipo('fim');
            if (fim) return fim;
        }

        var no = normalizarNo({ id: novoId(), tipo: tipo, x: x || 300, y: y || 230 }, estado.nos.length);
        if (tipo === 'condicao') {
            no.seVerdadeiro = '';
            no.seFalso = '';
        } else if (tipo !== 'fim') {
            no.proximo = '';
        }
        estado.nos.push(no);
        return no;
    }

    function primeiroNoTipo(tipo) {
        for (var i = 0; i < estado.nos.length; i++) if (estado.nos[i].tipo === tipo) return estado.nos[i];
        return null;
    }

    function montarGrafo() {
        syncInputsEditorToState();
        syncEditorToState();
        return { versao: 2, entradas: estado.entradas, nos: estado.nos };
    }

    function filhosLayout(no) {
        var filhos = [];
        if (!no) return filhos;
        if (no.tipo === 'condicao') {
            if (no.seVerdadeiro) filhos.push({ id: no.seVerdadeiro, ramo: 'verdadeiro' });
            if (no.seFalso) filhos.push({ id: no.seFalso, ramo: 'falso' });
        } else if (no.tipo === 'aprovacao') {
            if (no.proximo) filhos.push({ id: no.proximo, ramo: 'aprovado' });
            if (no.seRejeitado) filhos.push({ id: no.seRejeitado, ramo: 'rejeitado' });
        } else if (no.tipo === 'loop') {
            if (no.proximo) filhos.push({ id: no.proximo, ramo: 'repetir' });
            if (no.aoConcluir) filhos.push({ id: no.aoConcluir, ramo: 'concluir' });
        } else if (no.tipo === 'ferramenta' || no.tipo === 'ia') {
            if (no.proximo) filhos.push({ id: no.proximo, ramo: 'principal' });
            if (no.emErro) filhos.push({ id: no.emErro, ramo: 'erro' });
        } else if (no.tipo !== 'fim' && no.proximo) {
            filhos.push({ id: no.proximo, ramo: 'principal' });
        }
        return filhos;
    }

    function registrarLayout(id, profundidade, peso, pilha, profundidades, pesos) {
        if (!id || !existeNo(id) || profundidade > estado.nos.length + 2) return;
        if (pilha[id]) return;

        if (typeof profundidades[id] === 'undefined' || profundidade > profundidades[id]) {
            profundidades[id] = profundidade;
        }
        if (!pesos[id]) pesos[id] = { soma: 0, qtd: 0 };
        pesos[id].soma += peso;
        pesos[id].qtd++;

        var proxPilha = {};
        Object.keys(pilha).forEach(function (k) { proxPilha[k] = true; });
        proxPilha[id] = true;

        var no = noPorId(id);
        if (!no) return;
        if (no.tipo === 'condicao') {
            var abertura = Math.max(1.15, 1.85 - (profundidade * 0.12));
            if (no.seVerdadeiro) registrarLayout(no.seVerdadeiro, profundidade + 1, peso - abertura, proxPilha, profundidades, pesos);
            if (no.seFalso) registrarLayout(no.seFalso, profundidade + 1, peso + abertura, proxPilha, profundidades, pesos);
            return;
        }

        if (no.tipo === 'aprovacao' || no.tipo === 'loop') {
            var aberturaControle = Math.max(1.0, 1.55 - (profundidade * 0.10));
            if (no.proximo) registrarLayout(no.proximo, profundidade + 1, peso - aberturaControle, proxPilha, profundidades, pesos);
            if (no.seRejeitado) registrarLayout(no.seRejeitado, profundidade + 1, peso + aberturaControle, proxPilha, profundidades, pesos);
            if (no.aoConcluir) registrarLayout(no.aoConcluir, profundidade + 1, peso + aberturaControle, proxPilha, profundidades, pesos);
            return;
        }

        var filhos = filhosLayout(no);
        for (var i = 0; i < filhos.length; i++) {
            registrarLayout(filhos[i].id, profundidade + 1, peso, proxPilha, profundidades, pesos);
        }
    }

    function autoLayout() {
        var inicio = primeiroNoTipo('inicio');
        var profundidades = {};
        var pesos = {};
        var ordemOriginal = {};
        for (var i = 0; i < estado.nos.length; i++) ordemOriginal[estado.nos[i].id] = i;

        if (inicio) registrarLayout(inicio.id, 0, 0, {}, profundidades, pesos);

        var proximoPesoSolto = 0;
        Object.keys(pesos).forEach(function (id) {
            var p = pesos[id];
            if (p && p.qtd) proximoPesoSolto = Math.max(proximoPesoSolto, Math.abs(p.soma / p.qtd) + 2.6);
        });

        for (var n = 0; n < estado.nos.length; n++) {
            if (typeof profundidades[estado.nos[n].id] === 'undefined') {
                registrarLayout(estado.nos[n].id, 0, proximoPesoSolto, {}, profundidades, pesos);
                proximoPesoSolto += 2.6;
            }
        }

        var grupos = {};
        var menorPeso = 0;
        var maiorProfundidade = 0;
        for (var j = 0; j < estado.nos.length; j++) {
            var no = estado.nos[j];
            var profundidade = profundidades[no.id] || 0;
            var pesoInfo = pesos[no.id];
            var peso = pesoInfo && pesoInfo.qtd ? (pesoInfo.soma / pesoInfo.qtd) : 0;
            menorPeso = Math.min(menorPeso, peso);
            maiorProfundidade = Math.max(maiorProfundidade, profundidade);
            if (!grupos[profundidade]) grupos[profundidade] = [];
            grupos[profundidade].push({ no: no, peso: peso, ordem: ordemOriginal[no.id] || 0 });
        }

        var margemX = 80;
        var margemY = 90;
        var larguraColuna = 315;
        var alturaLinha = 135;
        var deslocamentoPeso = menorPeso < 0 ? Math.abs(menorPeso) : 0;

        Object.keys(grupos).sort(function (a, b) { return parseInt(a, 10) - parseInt(b, 10); }).forEach(function (prof) {
            var itens = grupos[prof];
            itens.sort(function (a, b) {
                if (a.peso === b.peso) return a.ordem - b.ordem;
                return a.peso - b.peso;
            });

            var ultimoY = -999999;
            for (var i = 0; i < itens.length; i++) {
                var y = margemY + ((itens[i].peso + deslocamentoPeso) * alturaLinha);
                if (y < ultimoY + alturaLinha) y = ultimoY + alturaLinha;
                itens[i].no.x = margemX + (parseInt(prof, 10) * larguraColuna);
                itens[i].no.y = Math.round(y / 10) * 10;
                ultimoY = itens[i].no.y;
            }
        });

        return { nos: estado.nos.length, colunas: maiorProfundidade + 1 };
    }

    function nodeHtml(no) {
        var info = tipoInfo(no.tipo);
        var meta = '';
        var acoes = '';
        var minimizado = boolCfg(no.minimizado);
        var tituloCabecalho = minimizado ? tituloNo(no) : info.titulo;
        if (no.tipo === 'condicao') meta = 'Verdadeiro / falso';
        else if (no.tipo === 'ia') meta = 'Saída: texto + dados / erro';
        else if (no.tipo === 'merge') meta = 'Junção de ramos';
        else if (no.tipo === 'aprovacao') meta = 'Aprovar / rejeitar';
        else if (no.tipo === 'loop') meta = 'Repetir / concluir';
        else if (no.tipo === 'espera') meta = 'Pausar / continuar';
        else if (no.tipo === 'ferramenta') {
            var cat = catalogoPorNome(no.ferramenta);
            var params = cat ? (cat.params || []) : [];
            var pendentes = parametrosPendentes(no, params);
            meta = (cat && cat.escopo === 'WRITE' ? 'Escrita: exige confirmação' : 'Leitura') + ' / erro';
            acoes = '<button type="button" class="wf-node-input-pill' + (pendentes ? ' wf-node-input-pendente' : '') + '" onmousedown="event.stopPropagation()" ontouchstart="event.stopPropagation()" onclick="event.stopPropagation(); wfAbrirParametrosNo(\'' + esc(no.id) + '\')">' +
                '<i class="fa fa-sliders"></i> ' + (params.length ? (pendentes ? pendentes + ' pendente(s)' : params.length + ' parâmetro(s)') : 'Parâmetros') +
                '</button>';
        } else if (no.tipo === 'definir') meta = 'Saída: valor';
        return '' +
            '<div class="wf-canvas-node' + (minimizado ? ' wf-node-collapsed' : '') + '" data-noid="' + esc(no.id) + '">' +
                '<div class="wf-node-head">' +
                    '<i class="fa ' + info.icone + '"></i>' +
                    '<span class="wf-node-title" title="' + esc(info.titulo + ': ' + tituloNo(no)) + '">' + esc(tituloCabecalho) + '</span>' +
                    '<span class="wf-node-id">' + esc(no.id) + '</span>' +
                    '<button type="button" class="wf-node-min-btn" title="' + (minimizado ? 'Expandir nó' : 'Recolher nó') + '" onmousedown="event.stopPropagation()" ontouchstart="event.stopPropagation()" onclick="event.stopPropagation(); wfToggleNoMinimizado(\'' + esc(no.id) + '\')"><i class="fa ' + (minimizado ? 'fa-plus' : 'fa-minus') + '"></i></button>' +
                '</div>' +
                '<div class="wf-node-body">' +
                    '<div class="wf-node-caption" title="' + esc(tituloNo(no)) + '">' + esc(tituloNo(no)) + '</div>' +
                    '<div class="wf-node-meta" title="' + esc(meta) + '">' + esc(meta) + '</div>' +
                    acoes +
                '</div>' +
            '</div>';
    }

    function drawflowData() {
        dfParaNo = {};
        noParaDf = {};
        var data = { drawflow: { Home: { data: {} } } };
        for (var i = 0; i < estado.nos.length; i++) {
            var dfId = String(i + 1);
            var no = estado.nos[i];
            dfParaNo[dfId] = no.id;
            noParaDf[no.id] = dfId;
        }

        for (var j = 0; j < estado.nos.length; j++) {
            var n = estado.nos[j], info = tipoInfo(n.tipo), idDf = noParaDf[n.id], inputs = {}, outputs = {};
            for (var a = 1; a <= info.entradas; a++) inputs['input_' + a] = { connections: [] };
            for (var b = 1; b <= info.saidas; b++) outputs['output_' + b] = { connections: [] };
            data.drawflow.Home.data[idDf] = {
                id: parseInt(idDf, 10),
                name: n.tipo,
                data: { noid: n.id },
                class: info.classe,
                html: nodeHtml(n),
                typenode: false,
                inputs: inputs,
                outputs: outputs,
                pos_x: n.x,
                pos_y: n.y
            };
        }

        var entradasUsadas = {};
        function ligar(origem, saida, destino) {
            if (!destino || !noParaDf[origem] || !noParaDf[destino]) return;
            var out = data.drawflow.Home.data[noParaDf[origem]].outputs[saida];
            var origemNo = noPorId(origem);
            var destinoNo = noPorId(destino);
            var entradaNome = 'input_1';
            if (destinoNo && destinoNo.tipo === 'merge') {
                entradasUsadas[destino] = (entradasUsadas[destino] || 0) + 1;
                entradaNome = 'input_' + Math.min(entradasUsadas[destino], tipoInfo(destinoNo.tipo).entradas);
            }
            var inp = data.drawflow.Home.data[noParaDf[destino]].inputs[entradaNome];
            if (!out || !inp) return;
            var connOut = { node: noParaDf[destino], output: entradaNome };
            var visual = conexaoVisual(origemNo, saida, destino);
            if (visual && visual.pontos.length) {
                connOut.points = visual.pontos;
            }
            out.connections.push(connOut);
            inp.connections.push({ node: noParaDf[origem], input: saida });
        }

        for (var k = 0; k < estado.nos.length; k++) {
            var noAtual = estado.nos[k];
            if (noAtual.tipo === 'condicao') {
                ligar(noAtual.id, 'output_1', noAtual.seVerdadeiro);
                ligar(noAtual.id, 'output_2', noAtual.seFalso);
            } else if (noAtual.tipo === 'aprovacao') {
                ligar(noAtual.id, 'output_1', noAtual.proximo);
                ligar(noAtual.id, 'output_2', noAtual.seRejeitado);
            } else if (noAtual.tipo === 'loop') {
                ligar(noAtual.id, 'output_1', noAtual.proximo);
                ligar(noAtual.id, 'output_2', noAtual.aoConcluir);
            } else if (noAtual.tipo === 'ferramenta' || noAtual.tipo === 'ia') {
                ligar(noAtual.id, 'output_1', noAtual.proximo);
                ligar(noAtual.id, 'output_2', noAtual.emErro);
            } else {
                ligar(noAtual.id, 'output_1', noAtual.proximo);
            }
        }

        return data;
    }

    function iniciarCanvas() {
        if (editor || !el('wfCanvas') || typeof Drawflow === 'undefined') return;
        editor = new Drawflow(el('wfCanvas'));
        editor.reroute = true;
        editor.curvature = 0.48;
        editor.zoom_min = 0.3;
        editor.zoom_max = 1.9;
        editor.zoom_value = 0.08;
        editor.draggable_inputs = false;
        editor.start();
        // Sem isto o navegador usa o padrao (centro do proprio precanvas, que e bem maior que a area visivel
        // - o "centro" dele fica em qualquer canto). translate3d()+scale() em aplicarTranslacaoCanvas/
        // ajustarZoomParaCaber assume escala a partir do canto (0,0); com a origem no centro, o tamanho fica
        // certo (scale nao muda tamanho por causa da origem) mas a posicao sai deslocada - exatamente o "pega
        // o workflow inteiro mas nao centraliza" que dava sem esta linha.
        if (editor.precanvas) editor.precanvas.style.transformOrigin = '0 0';

        editor.on('nodeSelected', function (dfId) {
            selectedNoId = dfParaNo[String(dfId)] || '';
            popupOcultoPara = '';
            popupEntradasNoId = '';
            inspectorMinimizado = false;
            renderProps();
            marcarSelecionado();
        });
        editor.on('nodeUnselected', function () {
            selectedNoId = '';
            renderProps();
        });
        editor.on('nodeMoved', agendaSync);
        editor.on('connectionCreated', agendaSync);
        editor.on('connectionRemoved', agendaSync);
        editor.on('addReroute', agendaSync);
        editor.on('removeReroute', agendaSync);
        editor.on('rerouteMoved', agendaSync);
        editor.on('nodeRemoved', function (dfId) {
            var noid = dfParaNo[String(dfId)] || '';
            if (!noid) return;
            var no = noPorId(noid);
            if (no && (no.tipo === 'inicio' || no.tipo === 'fim')) {
                setTimeout(function () { wfCanvasRender(); }, 0);
                return;
            }
            removerNo(noid, false);
            renderTudo(false);
        });

        prepararDragDrop();
        prepararPanCanvas();
        prepararModalParametros();
        wfCanvasRender();
    }

    function agendaSync() {
        setTimeout(function () {
            syncEditorToState();
            atualizarHidden();
            renderLista();
            renderProps();
            aplicarTraceNoCanvas(ultimoTrace);
            renderPopupErroTrace(ultimoTrace);
        }, 0);
    }

    function prepararDragDrop() {
        var itens = document.querySelectorAll('[data-wf-tipo]');
        for (var i = 0; i < itens.length; i++) {
            itens[i].addEventListener('dragstart', function (ev) {
                ev.dataTransfer.setData('text/plain', this.getAttribute('data-wf-tipo') || 'ferramenta');
            });
        }
        el('wfCanvas').addEventListener('dragover', function (ev) { ev.preventDefault(); });
        el('wfCanvas').addEventListener('drop', function (ev) {
            ev.preventDefault();
            var tipo = ev.dataTransfer.getData('text/plain') || 'ferramenta';
            var rect = el('wfCanvas').getBoundingClientRect();
            var zoom = editor ? editor.zoom || 1 : 1;
            var x = (ev.clientX - rect.left - (editor ? editor.canvas_x : 0)) / zoom;
            var y = (ev.clientY - rect.top - (editor ? editor.canvas_y : 0)) / zoom;
            wfCanvasAddNo(tipo, x, y);
        });
    }

    function atualizarModoMoverCanvas() {
        var canvas = el('wfCanvas');
        var botao = el('wfBtnModoMoverCanvas');
        if (canvas) {
            if (modoMoverCanvas) canvas.classList.add('wf-pan-mode');
            else canvas.classList.remove('wf-pan-mode');
            if (!canvasPanAtivo) canvas.classList.remove('wf-pan-active');
        }
        if (botao) {
            if (modoMoverCanvas) botao.classList.add('active');
            else botao.classList.remove('active');
        }
    }

    function aplicarTranslacaoCanvas(x, y) {
        if (!editor || !editor.precanvas) return;
        editor.canvas_x = x;
        editor.canvas_y = y;
        editor.precanvas.style.transform = 'translate3d(' + x + 'px, ' + y + 'px, 0) scale(' + editor.zoom + ')';
    }

    // Caixa que envolve todos os nos (posicao/tamanho reais no DOM, sem o zoom atual - offsetLeft/offsetTop/
    // offsetWidth/offsetHeight nao sao afetados pelo transform: scale() do precanvas, so pela posicao/tamanho
    // de layout de cada .drawflow-node). Null se o canvas ainda nao tem no nenhum (grafo vazio).
    function limitesDosNos() {
        var nos = document.querySelectorAll('#wfCanvas .drawflow-node');
        if (!nos.length) return null;
        var minX = Infinity, minY = Infinity, maxX = -Infinity, maxY = -Infinity;
        for (var i = 0; i < nos.length; i++) {
            var no = nos[i];
            var esquerda = no.offsetLeft, topo = no.offsetTop;
            var direita = esquerda + no.offsetWidth, baixo = topo + no.offsetHeight;
            if (esquerda < minX) minX = esquerda;
            if (topo < minY) minY = topo;
            if (direita > maxX) maxX = direita;
            if (baixo > maxY) maxY = baixo;
        }
        return { minX: minX, minY: minY, maxX: maxX, maxY: maxY, largura: maxX - minX, altura: maxY - minY };
    }

    // Ajusta zoom e posicao para caber todo o workflow na area visivel, centralizado - usado ao abrir a tela e
    // no botao "Centralizar". Sem isso, abrir um workflow cujos nos ficaram longe da origem (comum apos varias
    // edicoes/reorganizacoes) mostrava uma area em branco do canvas, com zoom e posicao sempre nos mesmos
    // valores fixos em vez de acompanhar onde os nos realmente estao.
    function ajustarZoomParaCaber() {
        if (!editor || !editor.precanvas) return false;
        var container = el('wfCanvas');
        var limites = limitesDosNos();
        if (!container || !limites) return false;

        var margem = 60;
        var larguraVisivel = container.clientWidth - margem * 2;
        var alturaVisivel = container.clientHeight - margem * 2;
        if (larguraVisivel <= 0 || alturaVisivel <= 0) return false;

        var zoomMin = editor.zoom_min || 0.3;
        var zoomMax = editor.zoom_max || 1.6;
        // Nunca amplia sozinho acima de 100% (um workflow com 1-2 nos nao precisa encher a tela); so reduz
        // quando precisar para caber tudo.
        var zoomAlvo = Math.min(1, larguraVisivel / Math.max(limites.largura, 1), alturaVisivel / Math.max(limites.altura, 1));
        zoomAlvo = Math.max(zoomMin, Math.min(zoomMax, zoomAlvo));

        editor.zoom = zoomAlvo;
        var centroX = (limites.minX + limites.maxX) / 2;
        var centroY = (limites.minY + limites.maxY) / 2;
        aplicarTranslacaoCanvas((container.clientWidth / 2) - centroX * zoomAlvo, (container.clientHeight / 2) - centroY * zoomAlvo);
        return true;
    }

    function agendarTranslacaoCanvas(x, y) {
        canvasPanPendente = { x: x, y: y };
        if (canvasPanRaf) return;
        var raf = window.requestAnimationFrame || function (fn) { return setTimeout(fn, 16); };
        canvasPanRaf = raf(function () {
            canvasPanRaf = null;
            if (!canvasPanPendente) return;
            aplicarTranslacaoCanvas(canvasPanPendente.x, canvasPanPendente.y);
            canvasPanPendente = null;
        });
    }

    function alvoInterativoCanvas(alvo) {
        if (!alvo || !alvo.closest) return false;
        return !!alvo.closest('button,input,textarea,select,a,.chosen-container,.input,.output,.main-path,.point');
    }

    function iniciarPanCanvas(ev) {
        if (!editor || !editor.precanvas) return false;
        var ehMouse = ev.type === 'mousedown';
        var deveMover = modoMoverCanvas || (ehMouse && ev.button === 1) || (ehMouse && ev.button === 0 && espacoPressionado);
        if (!deveMover) return false;
        if (ehMouse && ev.button !== 0 && ev.button !== 1) return false;
        if (alvoInterativoCanvas(ev.target)) return false;

        ev.preventDefault();
        ev.stopPropagation();
        if (ev.stopImmediatePropagation) ev.stopImmediatePropagation();

        popupEntradasNoId = '';
        esconderPopupEntradas();
        esconderPopupErro();
        canvasPanAtivo = true;
        canvasPanOrigem = {
            x: ev.clientX,
            y: ev.clientY,
            canvasX: editor.canvas_x || 0,
            canvasY: editor.canvas_y || 0
        };
        if (el('wfCanvas')) el('wfCanvas').classList.add('wf-pan-active');
        document.addEventListener('mousemove', moverPanCanvas, true);
        document.addEventListener('mouseup', finalizarPanCanvas, true);
        return true;
    }

    function moverPanCanvas(ev) {
        if (!canvasPanAtivo || !canvasPanOrigem) return;
        ev.preventDefault();
        var x = canvasPanOrigem.canvasX + (ev.clientX - canvasPanOrigem.x);
        var y = canvasPanOrigem.canvasY + (ev.clientY - canvasPanOrigem.y);
        agendarTranslacaoCanvas(x, y);
    }

    function finalizarPanCanvas(ev) {
        if (ev) {
            ev.preventDefault();
            ev.stopPropagation();
            if (ev.stopImmediatePropagation) ev.stopImmediatePropagation();
        }
        canvasPanAtivo = false;
        canvasPanOrigem = null;
        canvasPanPendente = null;
        canvasPanRaf = null;
        document.removeEventListener('mousemove', moverPanCanvas, true);
        document.removeEventListener('mouseup', finalizarPanCanvas, true);
        atualizarModoMoverCanvas();
    }

    function prepararPanCanvas() {
        var canvas = el('wfCanvas');
        if (!canvas || canvas.getAttribute('data-wf-pan') === '1') return;
        canvas.setAttribute('data-wf-pan', '1');
        canvas.addEventListener('mousedown', iniciarPanCanvas, true);
        canvas.addEventListener('auxclick', function (ev) {
            if (ev.button === 1 && !alvoInterativoCanvas(ev.target)) {
                ev.preventDefault();
                ev.stopPropagation();
            }
        }, true);
        document.addEventListener('keydown', function (ev) {
            var espaco = ev.code === 'Space' || ev.key === ' ' || ev.keyCode === 32;
            if (espaco && !/INPUT|TEXTAREA|SELECT/.test((ev.target || {}).tagName || '')) {
                ev.preventDefault();
                espacoPressionado = true;
                if (el('wfCanvas')) el('wfCanvas').classList.add('wf-pan-mode');
            }
        });
        document.addEventListener('keyup', function (ev) {
            var espaco = ev.code === 'Space' || ev.key === ' ' || ev.keyCode === 32;
            if (espaco) {
                espacoPressionado = false;
                if (!modoMoverCanvas && !canvasPanAtivo) atualizarModoMoverCanvas();
            }
        });
        atualizarModoMoverCanvas();
    }

    function prepararModalParametros() {
        var modal = el('wfParametrosModal');
        if (!modal || modal.getAttribute('data-wf-modal') === '1') return;
        modal.setAttribute('data-wf-modal', '1');
        if (modal.parentNode !== document.body) {
            document.body.appendChild(modal);
        }
        var jq = window.jQuery || window.$;
        if (jq && jq.fn && jq.fn.modal) {
            jq(modal).on('hidden.bs.modal', function () {
                parametroModalNoId = '';
                modal.classList.remove('wf-modal-fallback', 'in');
                renderProps();
                renderLista();
                atualizarHidden();
            });
        }
    }

    function mostrarModalParametrosSeguro() {
        var modal = el('wfParametrosModal');
        if (!modal) return;
        if (modal.parentNode !== document.body) {
            document.body.appendChild(modal);
        }

        var jq = window.jQuery || window.$;
        if (jq && jq.fn && jq.fn.modal) {
            try {
                jq(modal).modal({ backdrop: true, keyboard: true, show: true });
                return;
            } catch (ex) {
                if (window.console && window.console.error) window.console.error('Falha ao abrir modal de parâmetros.', ex);
                try { jq('.modal-backdrop').remove(); } catch (ignore) { }
            }
        }

        modal.style.display = 'block';
        modal.classList.add('in', 'wf-modal-fallback');
        document.body.classList.add('modal-open');
    }

    function fecharModalParametrosFallback() {
        var modal = el('wfParametrosModal');
        if (!modal) return;
        modal.style.display = 'none';
        modal.classList.remove('in', 'wf-modal-fallback');
        document.body.classList.remove('modal-open');
        parametroModalNoId = '';
        renderProps();
        renderLista();
        atualizarHidden();
    }

    function mostrarModalJsonSeguro() {
        var modal = el('wfJsonModal');
        if (!modal) return;
        if (modal.parentNode !== document.body) {
            document.body.appendChild(modal);
        }

        var jq = window.jQuery || window.$;
        if (jq && jq.fn && jq.fn.modal) {
            try {
                jq(modal).modal({ backdrop: true, keyboard: true, show: true });
                return;
            } catch (ex) {
                if (window.console && window.console.error) window.console.error('Falha ao abrir visualizador de JSON.', ex);
                try { jq('.modal-backdrop').remove(); } catch (ignore) { }
            }
        }

        modal.style.display = 'block';
        modal.classList.add('in', 'wf-modal-fallback');
        document.body.classList.add('modal-open');
    }

    function fecharModalJsonFallback() {
        var modal = el('wfJsonModal');
        if (!modal) return;
        modal.style.display = 'none';
        modal.classList.remove('in', 'wf-modal-fallback');
        document.body.classList.remove('modal-open');
    }

    function alvoEdicaoGeral(alvo) {
        if (!alvo) return false;
        var tag = String(alvo.tagName || '').toUpperCase();
        if (tag === 'INPUT' || tag === 'TEXTAREA' || tag === 'SELECT') return true;
        if (alvo.isContentEditable) return true;
        return !!(alvo.closest && alvo.closest('.chosen-container,.modal'));
    }

    function contarConexoesGrafo() {
        var total = 0;
        estado.nos.forEach(function (no) {
            saidaPrincipal(no).forEach(function (destino) {
                if (destino) total++;
            });
        });
        return total;
    }

    function plural(valor, singular, pluralTexto) {
        return valor + ' ' + (valor === 1 ? singular : pluralTexto);
    }

    function atualizarResumoLayout() {
        var resumo = el('wfHeaderResumo');
        var stats = el('wfCanvasStats');
        var nos = estado.nos.length;
        var conexoes = contarConexoesGrafo();
        var texto = plural(nos, 'nó', 'nós') + ' | ' + plural(conexoes, 'conexão', 'conexões');
        if (resumo) resumo.textContent = texto;
        if (stats) stats.textContent = texto;

        var ddlAtivo = document.querySelector('select[id$="_ddlAtivo"],select[id="ddlAtivo"]');
        var badge = el('wfHeaderAtivoBadge');
        if (badge) {
            var ativo = ddlAtivo ? ddlAtivo.value === 'S' : false;
            badge.textContent = ativo ? 'Ativo' : 'Inativo';
            badge.classList.remove('wf-status-active', 'wf-status-inactive');
            badge.classList.add(ativo ? 'wf-status-active' : 'wf-status-inactive');
        }
    }

    function atualizarInspectorLayout() {
        var painel = el('wfInspectorPanel');
        var shell = el('wfEditorShell');
        if (!painel) return;
        var aberto = !!(selectedNoId && noPorId(selectedNoId) && !inspectorMinimizado);
        painel.classList.toggle('wf-inspector-closed', !aberto);
        if (shell) shell.classList.toggle('wf-inspector-collapsed', !aberto);
    }

    function mostrarModalConfigSeguro() {
        var modal = el('wfConfigModal');
        if (!modal) return;
        var jq = window.jQuery || window.$;
        if (jq && jq.fn && jq.fn.modal) {
            try {
                jq(modal).modal({ backdrop: true, keyboard: true, show: true });
                return;
            } catch (ex) {
                if (window.console && window.console.error) window.console.error('Falha ao abrir configurações do workflow.', ex);
                try { jq('.modal-backdrop').remove(); } catch (ignore) { }
            }
        }

        modal.style.display = 'block';
        modal.classList.add('in', 'wf-modal-fallback');
        document.body.classList.add('modal-open');
    }

    function fecharModalConfigFallback() {
        var modal = el('wfConfigModal');
        if (!modal) return;
        modal.style.display = 'none';
        modal.classList.remove('in', 'wf-modal-fallback');
        document.body.classList.remove('modal-open');
        atualizarResumoLayout();
    }

    function mostrarModalAtalhosSeguro() {
        var modal = el('wfShortcutsModal');
        if (!modal) return;
        var jq = window.jQuery || window.$;
        if (jq && jq.fn && jq.fn.modal) {
            try {
                jq(modal).modal({ backdrop: true, keyboard: true, show: true });
                return;
            } catch (ex) {
                if (window.console && window.console.error) window.console.error('Falha ao abrir atalhos do workflow.', ex);
                try { jq('.modal-backdrop').remove(); } catch (ignore) { }
            }
        }

        modal.style.display = 'block';
        modal.classList.add('in', 'wf-modal-fallback');
        document.body.classList.add('modal-open');
    }

    function fecharModalAtalhosFallback() {
        var modal = el('wfShortcutsModal');
        if (!modal) return;
        modal.style.display = 'none';
        modal.classList.remove('in', 'wf-modal-fallback');
        document.body.classList.remove('modal-open');
    }

    function setBibliotecaRecolhida(recolhida) {
        var shell = el('wfEditorShell');
        var btn = document.querySelector('#wfNodeLibrary .wf-panel-head .btn');
        if (!shell) return;
        if (recolhida) shell.classList.add('wf-library-collapsed');
        else shell.classList.remove('wf-library-collapsed');
        if (btn) {
            btn.title = recolhida ? 'Expandir biblioteca' : 'Recolher biblioteca';
            btn.innerHTML = '<i class="fa ' + (recolhida ? 'fa-angle-right' : 'fa-angle-left') + '"></i>';
        }
    }

    function abrirCanvasTab(callback) {
        if (window.jQuery && window.jQuery.fn && window.jQuery.fn.tab) {
            window.jQuery('a[href="#abaCanvas"]').tab('show');
            setTimeout(function () {
                if (callback) callback();
            }, 30);
            return;
        }
        if (callback) callback();
    }

    function prepararLayoutWorkflow() {
        if (layoutWorkflowPreparado) return;
        layoutWorkflowPreparado = true;

        var ddlAtivo = document.querySelector('select[id$="_ddlAtivo"],select[id="ddlAtivo"]');
        if (ddlAtivo) ddlAtivo.addEventListener('change', atualizarResumoLayout);

        document.addEventListener('keydown', function (ev) {
            var key = ev.key || '';
            var keyLower = key.toLowerCase();
            if ((ev.ctrlKey || ev.metaKey) && keyLower === 's') {
                ev.preventDefault();
                if (window.wfHeaderSalvar) window.wfHeaderSalvar();
                return;
            }
            if ((ev.ctrlKey || ev.metaKey) && ev.shiftKey && key === 'Enter') {
                ev.preventDefault();
                if (window.wfExecutarReal) window.wfExecutarReal();
                return;
            }
            if ((ev.ctrlKey || ev.metaKey) && key === 'Enter') {
                ev.preventDefault();
                if (window.wfTestar) window.wfTestar();
                return;
            }
            if ((key === 'Delete' || key === 'Backspace') && !alvoEdicaoGeral(ev.target) && selectedNoId) {
                ev.preventDefault();
                window.wfCanvasRemoverSelecionado();
                return;
            }
            if (key === 'Escape') {
                if (window.wfFecharJsonViewer) window.wfFecharJsonViewer();
                if (window.wfFecharParametrosModal) window.wfFecharParametrosModal();
                if (window.wfCloseConfigModal) window.wfCloseConfigModal();
                if (window.wfCloseShortcutsModal) window.wfCloseShortcutsModal();
                if (!popupRuntimeAtivo) {
                    popupEntradasNoId = '';
                    esconderPopupEntradas();
                }
                esconderPopupErro();
                return;
            }
            if (key === '/' && !alvoEdicaoGeral(ev.target)) {
                ev.preventDefault();
                setBibliotecaRecolhida(false);
                var busca = el('wfNodeSearch');
                if (busca) busca.focus();
            }
        });

        atualizarResumoLayout();
        atualizarInspectorLayout();
    }

    function wfCanvasRender() {
        if (!editor) return;
        renderizando = true;
        editor.import(drawflowData(), false);
        renderizando = false;
        marcarSelecionado();
        aplicarTraceNoCanvas(ultimoTrace);
        renderPopupEntradas(noPorId(selectedNoId));
        renderPopupErroTrace(ultimoTrace);
        atualizarResumoLayout();
    }

    function syncEditorToState() {
        if (!editor || renderizando) return;
        var exp = editor.export();
        var data = exp && exp.drawflow && exp.drawflow.Home ? exp.drawflow.Home.data : {};
        var porNo = {};
        Object.keys(data).forEach(function (dfId) {
            var noid = data[dfId].data ? data[dfId].data.noid : dfParaNo[dfId];
            var no = noPorId(noid);
            if (no) {
                no.x = data[dfId].pos_x;
                no.y = data[dfId].pos_y;
                porNo[dfId] = no;
            }
        });

        for (var i = 0; i < estado.nos.length; i++) {
            estado.nos[i].conexoesVisuais = {};
            if (estado.nos[i].tipo === 'condicao') {
                estado.nos[i].seVerdadeiro = '';
                estado.nos[i].seFalso = '';
            } else if (estado.nos[i].tipo === 'aprovacao') {
                estado.nos[i].proximo = '';
                estado.nos[i].seRejeitado = '';
            } else if (estado.nos[i].tipo === 'loop') {
                estado.nos[i].proximo = '';
                estado.nos[i].aoConcluir = '';
            } else if (estado.nos[i].tipo === 'ferramenta' || estado.nos[i].tipo === 'ia') {
                estado.nos[i].proximo = '';
                estado.nos[i].emErro = '';
            } else if (estado.nos[i].tipo !== 'fim') {
                estado.nos[i].proximo = '';
            }
        }

        Object.keys(data).forEach(function (dfId) {
            var origem = porNo[dfId];
            if (!origem || !data[dfId].outputs) return;
            Object.keys(data[dfId].outputs).forEach(function (saida) {
                var conns = data[dfId].outputs[saida].connections || [];
                if (!conns.length) return;
                var conn = conns[0];
                var destino = dfParaNo[String(conn.node)] || (data[conn.node] && data[conn.node].data ? data[conn.node].data.noid : '');
                if (!destino) return;
                var entrada = conn.output || 'input_1';
                var pontos = pontosConexaoDom(dfId, conn.node, saida, entrada);
                registrarConexaoVisual(origem, saida, destino, entrada, pontos.length ? pontos : conn.points);
                if (origem.tipo === 'condicao') {
                    if (saida === 'output_1') origem.seVerdadeiro = destino;
                    if (saida === 'output_2') origem.seFalso = destino;
                } else if (origem.tipo === 'aprovacao') {
                    if (saida === 'output_1') origem.proximo = destino;
                    if (saida === 'output_2') origem.seRejeitado = destino;
                } else if (origem.tipo === 'loop') {
                    if (saida === 'output_1') origem.proximo = destino;
                    if (saida === 'output_2') origem.aoConcluir = destino;
                } else if (origem.tipo === 'ferramenta' || origem.tipo === 'ia') {
                    if (saida === 'output_1') origem.proximo = destino;
                    if (saida === 'output_2') origem.emErro = destino;
                } else if (origem.tipo !== 'fim') {
                    origem.proximo = destino;
                }
            });
        });
    }

    function conexoesMultiplasEditor() {
        var probs = [];
        if (!editor) return probs;
        var data = editor.export().drawflow.Home.data || {};
        Object.keys(data).forEach(function (dfId) {
            var noid = data[dfId].data ? data[dfId].data.noid : dfParaNo[dfId];
            var no = noPorId(noid);
            if (!no || !data[dfId].outputs) return;
            Object.keys(data[dfId].outputs).forEach(function (saida) {
                var conns = data[dfId].outputs[saida].connections || [];
                if (conns.length > 1) probs.push(noid + ': a saída ' + saida + ' tem mais de um conector.');
            });
        });
        return probs;
    }

    function renderTudo(canvas) {
        renderInputs();
        renderLista();
        renderProps();
        if (canvas !== false) wfCanvasRender();
        atualizarHidden();
        aplicarSelectsFerramenta(document);
        atualizarResumoLayout();
    }

    function atualizarHidden() {
        syncInputsEditorToState();
        if (el(ids.hddGrafo)) el(ids.hddGrafo).value = JSON.stringify({ versao: 2, entradas: estado.entradas, nos: estado.nos });
    }

    function marcarSelecionado() {
        if (!editor) return;
        var nodes = document.querySelectorAll('#wfCanvas .drawflow-node');
        for (var i = 0; i < nodes.length; i++) nodes[i].classList.remove('selected');
        if (!selectedNoId) return;
        var dfId = noParaDf[selectedNoId];
        if (!dfId) return;
        var node = document.getElementById('node-' + dfId);
        if (node) node.classList.add('selected');
    }

    function aplicarTraceNoCanvas(trace) {
        if (replayAtivo) return;
        if (!editor) return;
        var nodes = document.querySelectorAll('#wfCanvas .drawflow-node');
        for (var i = 0; i < nodes.length; i++) {
            nodes[i].classList.remove('wf-status-ok', 'wf-status-erro', 'wf-status-erro-tratado', 'wf-status-pausado', 'wf-status-executando');
        }
        (trace || []).forEach(function (t) {
            var dfId = noParaDf[t.No];
            var node = dfId ? document.getElementById('node-' + dfId) : null;
            if (!node) return;
            var st = String(t.Status || '').toUpperCase();
            if (st === 'OK') node.classList.add('wf-status-ok');
            else if (st === 'PAUSADO') node.classList.add('wf-status-pausado');
            else if (st === 'ERRO') node.classList.add(erroTratadoTrace(t) ? 'wf-status-erro-tratado' : 'wf-status-erro');
        });
    }

    function removerStatusNosCanvas() {
        var nodes = document.querySelectorAll('#wfCanvas .drawflow-node');
        for (var i = 0; i < nodes.length; i++) {
            nodes[i].classList.remove('wf-status-ok', 'wf-status-erro', 'wf-status-erro-tratado', 'wf-status-pausado', 'wf-status-executando', 'selected');
        }
    }

    function marcarNoCanvas(noid, status) {
        if (!editor || !noid) return;
        var dfId = noParaDf[noid];
        var node = dfId ? document.getElementById('node-' + dfId) : null;
        if (!node) return;
        var nodes = document.querySelectorAll('#wfCanvas .drawflow-node');
        for (var i = 0; i < nodes.length; i++) nodes[i].classList.remove('selected');
        node.classList.remove('wf-status-ok', 'wf-status-erro', 'wf-status-erro-tratado', 'wf-status-pausado', 'wf-status-executando');
        status = String(status || '').toUpperCase();
        if (status === 'OK') node.classList.add('wf-status-ok');
        else if (status === 'PAUSADO') node.classList.add('wf-status-pausado');
        else if (status === 'ERRO') node.classList.add('wf-status-erro');
        else node.classList.add('wf-status-executando');
        node.classList.add('selected');

        selectedNoId = noid;
        renderProps();
    }

    function canvasStatusHtml(classe, conteudo) {
        var box = el('wfCanvasExecStatus');
        if (!box) return;
        box.innerHTML = '<div class="alert alert-' + classe + '">' + conteudo + '</div>';
    }

    function iniciarExecucaoNoCanvas(texto) {
        limparReplay();
        ultimoTrace = [];
        erroPopupOcultoPara = '';
        popupRuntimeAtivo = null;
        popupOcultoPara = selectedNoId || popupOcultoPara;
        popupEntradasNoId = '';
        esconderPopupEntradas();
        esconderPopupErro();
        removerStatusNosCanvas();
        var inicio = primeiroNoTipo('inicio');
        if (inicio) marcarNoCanvas(inicio.id, 'EXECUTANDO');
        canvasStatusHtml('info', '<i class="fa fa-spinner fa-spin"></i> ' + esc(texto || 'Executando workflow...'));
        renderLista();
    }

    function canvasAcoesPendentes(titulo, tipoPausa, descricao) {
        var ehAprovacao = String(tipoPausa || '').toUpperCase() === 'APROVACAO';
        var ehEspera = String(tipoPausa || '').toUpperCase() === 'ESPERA';
        var ehEntradas = String(tipoPausa || '').toUpperCase() === 'ENTRADAS';
        var ehEscolha = String(tipoPausa || '').toUpperCase() === 'ESCOLHA';
        var ehSelecaoMultipla = String(tipoPausa || '').toUpperCase() === 'SELECAO_MULTIPLA';
        var fallback = ehEntradas ? 'dados do usuário' : (ehAprovacao ? 'aprovação' : (ehEspera ? 'espera' : ((ehEscolha || ehSelecaoMultipla) ? 'escolha' : 'ferramenta')));
        var descricaoTexto = trim(descricao || '');
        var tituloTexto = titulo || fallback;
        var descricaoHtml = descricaoTexto && descricaoTexto !== tituloTexto
            ? '<div class="wf-canvas-exec-desc">' + esc(descricaoTexto) + '</div>'
            : '';
        return '<div class="wf-canvas-exec-actions">' +
            '<div class="wf-canvas-exec-buttons">' +
                (ehEntradas ? '' : ((ehEscolha || ehSelecaoMultipla)
                    ? '<button type="button" class="btn btn-warning btn-xs" onclick="wfFocarEscolhaReal()"><i class="fa fa-list-ul"></i> Escolher opção</button>'
                    : '<button type="button" class="btn btn-warning btn-xs" onclick="wfConfirmarReal()"><i class="fa fa-check"></i> ' + (ehAprovacao ? 'Aprovar' : (ehEspera ? 'Continuar' : 'Confirmar passo')) + '</button>')) +
                (ehAprovacao ? '<button type="button" class="btn btn-default btn-xs" onclick="wfRejeitarReal()"><i class="fa fa-times"></i> Rejeitar</button>' : '') +
                '<button type="button" class="btn btn-default btn-xs" onclick="wfCancelarReal()"><i class="fa fa-ban"></i> Cancelar execução</button>' +
            '</div>' +
            '<div class="wf-canvas-exec-text"><span class="text-muted">' + (ehEntradas ? 'Aguardando entradas' : (ehAprovacao ? 'Aguardando aprovação' : (ehEspera ? 'Aguardando espera' : ((ehEscolha || ehSelecaoMultipla) ? 'Aguardando escolha' : 'Aguardando confirmação')))) + ': <strong>' + esc(tituloTexto) + '</strong></span>' + descricaoHtml + '</div>' +
            '</div>';
    }

    function finalizarStatusCanvas(status, mensagem, trace) {
        status = status || 'ERRO';
        var classe = classeExec(status);
        var pausada = status === 'PAUSADO' ? (ferramentaPausada(trace) || 'ferramenta') : '';
        var texto = '<strong>' + esc(status) + '</strong> - ' + esc(mensagem || 'Execução processada.');
        if (pausada) texto += canvasAcoesPendentes(pausada, ultimoTipoPausa, mensagem);
        canvasStatusHtml(classe, texto);
    }

    function reproduzirTraceNoCanvas(trace, status, mensagem, inicioReplay) {
        limparReplay();
        trace = trace || [];
        inicioReplay = parseInt(inicioReplay || '0', 10) || 0;
        if (inicioReplay < 0 || inicioReplay > trace.length) inicioReplay = 0;
        removerStatusNosCanvas();
        if (inicioReplay > 0) {
            aplicarTraceNoCanvas(trace.slice(0, inicioReplay));
        }

        var traceReplay = trace.slice(inicioReplay);
        if (!trace.length) {
            finalizarStatusCanvas(status, mensagem, trace);
            renderLista();
            popupOcultoPara = selectedNoId || popupOcultoPara;
            popupEntradasNoId = '';
            if (!popupRuntimeAtivo) esconderPopupEntradas();
            renderProps();
            renderPopupErroTrace(trace);
            return;
        }
        if (!traceReplay.length) {
            aplicarTraceNoCanvas(trace);
            finalizarStatusCanvas(status, mensagem, trace);
            renderLista();
            popupOcultoPara = selectedNoId || popupOcultoPara;
            popupEntradasNoId = '';
            if (!popupRuntimeAtivo) esconderPopupEntradas();
            renderProps();
            renderPopupErroTrace(trace);
            return;
        }

        replayAtivo = true;
        var idx = 0;
        function passo() {
            if (idx >= traceReplay.length) {
                replayAtivo = false;
                aplicarTraceNoCanvas(trace);
                finalizarStatusCanvas(status, mensagem, trace);
                renderLista();
                popupOcultoPara = selectedNoId || popupOcultoPara;
                popupEntradasNoId = '';
                if (!popupRuntimeAtivo) esconderPopupEntradas();
                renderProps();
                renderPopupErroTrace(trace);
                return;
            }

            var t = traceReplay[idx] || {};
            marcarNoCanvas(t.No, 'EXECUTANDO');
            canvasStatusHtml('info', '<i class="fa fa-spinner fa-spin"></i> Passo ' + (inicioReplay + idx + 1) + ' de ' + trace.length + ': <strong>' + esc(t.Titulo || t.No || 'nó') + '</strong>');

            replayTimer = setTimeout(function () {
                marcarNoCanvas(t.No, t.Status || 'OK');
                idx++;
                replayTimer = setTimeout(passo, 320);
            }, 520);
        }
        passo();
    }

    function renderInputs() {
        var h = estado.entradas.length
            ? '<div class="wf-input-sort-hint"><i class="fa fa-arrows-v"></i> Arraste pela alça ou use as setas para reposicionar as entradas.</div><div class="wf-input-list">'
            : '';
        estado.entradas.forEach(function (e, i) {
            e = normalizarEntrada(e);
            var tipos = WF_TIPOS.map(function (t) { return '<option value="' + esc(t[0]) + '"' + (e.tipo === t[0] ? ' selected' : '') + '>' + esc(t[1]) + '</option>'; }).join('');
            var usos = WF_OBRIGATORIEDADES.map(function (o) { return '<option value="' + esc(o[0]) + '"' + (e.obrigatoriedade === o[0] ? ' selected' : '') + '>' + esc(o[1]) + '</option>'; }).join('');
            h += '<div class="wf-input-row" data-wf-input-idx="' + i + '" ondragover="wfInputDragOver(event)" ondragleave="wfInputDragLeave(event)" ondrop="wfInputDrop(event,' + i + ')">' +
                '<div class="wf-input-order">' +
                    '<button type="button" class="btn btn-default btn-xs wf-input-drag" title="Arraste para reposicionar" draggable="true" ondragstart="wfInputDragStart(event,' + i + ')" ondragend="wfInputDragEnd(event)"><i class="fa fa-bars"></i></button>' +
                    '<button type="button" class="btn btn-default btn-xs" title="Mover para cima" onclick="wfMoverInput(' + i + ',-1)"' + (i === 0 ? ' disabled' : '') + '><i class="fa fa-chevron-up"></i></button>' +
                    '<button type="button" class="btn btn-default btn-xs" title="Mover para baixo" onclick="wfMoverInput(' + i + ',1)"' + (i === estado.entradas.length - 1 ? ' disabled' : '') + '><i class="fa fa-chevron-down"></i></button>' +
                '</div>' +
                '<div class="wf-input-card-body">' +
                    '<div class="wf-input-grid">' +
                        '<div class="wf-input-field"><label>Nome</label><input class="form-control input-sm wf-mono" data-wf-input-field="nome" placeholder="nome" value="' + esc(e.nome) + '" onchange="wfInputChange(' + i + ',\'nome\',this.value)"></div>' +
                        '<div class="wf-input-field"><label>Tipo</label><select class="form-control input-sm" data-wf-input-field="tipo" onchange="wfInputChange(' + i + ',\'tipo\',this.value)">' + tipos + '</select></div>' +
                        '<div class="wf-input-field"><label>Grupo</label><input class="form-control input-sm" data-wf-input-field="grupo" placeholder="Grupo" value="' + esc(e.grupo || 'Geral') + '" onchange="wfInputChange(' + i + ',\'grupo\',this.value)"></div>' +
                        '<div class="wf-input-field"><label>Uso</label><select class="form-control input-sm" data-wf-input-field="obrigatoriedade" onchange="wfInputChange(' + i + ',\'obrigatoriedade\',this.value)">' + usos + '</select></div>' +
                    '</div>' +
                    '<div class="wf-input-field wf-input-desc-wrap"><label>Descrição exibida ao usuário</label><textarea class="form-control input-sm wf-input-desc" data-wf-input-field="descricao" rows="2" placeholder="Explique quando e como esta entrada deve ser preenchida." onchange="wfInputChange(' + i + ',\'descricao\',this.value)">' + esc(e.descricao || '') + '</textarea></div>' +
                    '<div class="wf-input-field wf-input-desc-wrap"><label>Opções de escolha</label><textarea class="form-control input-sm wf-mono" data-wf-input-field="opcoesTexto" rows="2" placeholder="VALOR|Rótulo (uma opção por linha)" onchange="wfInputChange(' + i + ',\'opcoesTexto\',this.value)">' + esc(opcoesEntradaTexto(e.opcoes)) + '</textarea><span class="help-block">Deixe vazio para exibir um campo de texto livre.</span></div>' +
                '</div>' +
                '<div class="wf-input-remove"><button type="button" class="btn btn-default btn-sm" title="Remover entrada" onclick="wfRemoverInput(' + i + ')"><i class="fa fa-times"></i></button></div>' +
                '</div>';
        });
        if (estado.entradas.length) h += '</div>';
        if (el('wfInputs')) el('wfInputs').innerHTML = h;
        renderTesteInputs();
    }

    function syncInputsEditorToState() {
        var box = el('wfInputs');
        if (!box) return;
        var rows = box.querySelectorAll('.wf-input-row');
        if (!rows.length) return;
        var entradas = [];
        for (var i = 0; i < rows.length; i++) {
            var idx = parseInt(rows[i].getAttribute('data-wf-input-idx') || String(i), 10);
            var entrada = clone(estado.entradas[idx] || {});
            var campos = rows[i].querySelectorAll('[data-wf-input-field]');
            for (var c = 0; c < campos.length; c++) {
                var campo = campos[c].getAttribute('data-wf-input-field');
                if (campo === 'opcoesTexto') entrada.opcoes = opcoesEntradaDoTexto(campos[c].value);
                else entrada[campo] = campos[c].value;
            }
            entradas.push(normalizarEntrada(entrada));
        }
        estado.entradas = entradas;
    }

    function valorPadraoEntrada(tipo) {
        if (tipo === 'integer' || tipo === 'number') return '0';
        if (tipo === 'array') return '[]';
        if (tipo === 'json') return '{}';
        return '';
    }
    function entradaEhJson(tipo) {
        tipo = String(tipo || '').toLowerCase();
        return tipo === 'array' || tipo === 'json';
    }
    function arrayLista(valor) {
        if (Array.isArray(valor)) return valor;
        if (typeof valor === 'string' && trim(valor)) {
            try {
                var parsed = JSON.parse(valor);
                if (Array.isArray(parsed)) return parsed;
            } catch (e) { }
        }
        return [];
    }
    function listaValorParaTexto(item) {
        if (item == null) return '';
        if (typeof item === 'object') return JSON.stringify(item);
        return String(item);
    }
    function listaTextoParaValor(texto) {
        texto = trim(texto);
        if (!texto) return '';
        if (texto.charAt(0) === '{' || texto.charAt(0) === '[') {
            try { return JSON.parse(texto); } catch (e) { }
        }
        if (/^(true|false|null)$/i.test(texto)) {
            try { return JSON.parse(texto.toLowerCase()); } catch (e2) { }
        }
        return texto;
    }
    function listaLinhaHtml(item) {
        return '<div class="wf-lista-row">' +
            '<input type="text" class="form-control input-sm wf-lista-valor" placeholder="Valor do item" value="' + esc(listaValorParaTexto(item)) + '" oninput="wfListaEntradaChange(this)">' +
            '<button type="button" class="btn btn-default btn-sm" onclick="wfListaEntradaRemove(this)"><i class="fa fa-times"></i></button>' +
            '</div>';
    }
    function renderListaEditor(nome, valor, uso, opcoes) {
        opcoes = opcoes || {};
        var itens = arrayLista(valor);
        if (!itens.length) itens = [''];
        var json = JSON.stringify(arrayLista(valor));
        var linhas = itens.map(function (item) { return listaLinhaHtml(item); }).join('');
        var hiddenClass = opcoes.hiddenClass || 'wf-test-input';
        var dataIndice = opcoes.indice == null ? '' : ' data-i="' + esc(opcoes.indice) + '"';
        var dataRuntime = opcoes.runtime ? ' data-runtime="S"' : '';
        return '<div class="wf-lista-editor" data-nome="' + esc(nome) + '" data-uso="' + esc(uso || 'obrigatorio') + '"' + dataRuntime + '>' +
            '<div class="wf-lista-rows">' + linhas + '</div>' +
            '<div class="wf-lista-actions">' +
                '<button type="button" class="btn btn-default btn-xs" onclick="wfListaEntradaAdd(this)"><i class="fa fa-plus"></i> Item</button>' +
                '<span class="text-muted">Cada linha vira um item da lista.</span>' +
            '</div>' +
            '<textarea class="' + esc(hiddenClass) + ' wf-lista-json" data-nome="' + esc(nome) + '" data-tipo="array" data-uso="' + esc(uso || 'obrigatorio') + '"' + dataIndice + ' style="display:none;">' + esc(json) + '</textarea>' +
            '</div>';
    }
    function renderListaEntrada(e, valor, condicional) {
        return '<div class="wf-test-input-row wf-lista-input-row' + (condicional ? ' wf-test-input-row-conditional' : '') + '">' +
            '<label title="' + esc(labelTipoEntrada(e.tipo || 'array')) + '">' + esc(e.nome) + (entradaEhObrigatoria(e) ? ' <span class="wf-req">*</span>' : '') + '</label>' +
            '<div class="wf-test-input-field">' +
                renderListaEditor(e.nome, valor, e.obrigatoriedade) +
                (e.descricao ? '<span class="help-block">' + esc(e.descricao) + '</span>' : '') +
            '</div>' +
            '</div>';
    }
    function listaEditorDoElemento(elemento) {
        var editorLista = elemento;
        while (editorLista && !editorLista.classList.contains('wf-lista-editor')) editorLista = editorLista.parentNode;
        return editorLista;
    }
    function atualizarEditoresLista(nome, valor, ignorarEditor) {
        var editores = document.querySelectorAll('.wf-lista-editor');
        for (var i = 0; i < editores.length; i++) {
            if (editores[i] === ignorarEditor) continue;
            if (editores[i].getAttribute('data-nome') === nome) window.wfListaRenderizar(editores[i], arrayLista(valor), false);
        }
    }
    function arrayProdutos(valor) {
        if (Array.isArray(valor)) return valor;
        if (typeof valor === 'string' && trim(valor)) {
            try {
                var parsed = JSON.parse(valor);
                if (Array.isArray(parsed)) return parsed;
            } catch (e) { }
        }
        return [];
    }
    function produtoLinhaHtml(item) {
        item = item || {};
        return '<div class="wf-produto-row">' +
            '<input class="form-control input-sm wf-produto-campo wf-produto-nome" placeholder="Produto" value="' + esc(item.produto || item.descricao || '') + '" oninput="wfProdutoEntradaChange(this)">' +
            '<input class="form-control input-sm wf-produto-campo wf-produto-tabela" placeholder="Tabela opcional" value="' + esc(item.tabelaPreco || item.tabela || item.tabelaPrecoNome || '') + '" oninput="wfProdutoEntradaChange(this)">' +
            '<input type="number" step="any" min="0" class="form-control input-sm wf-produto-campo wf-produto-quantidade" placeholder="Qtd." value="' + esc(item.quantidade == null ? '' : item.quantidade) + '" oninput="wfProdutoEntradaChange(this)">' +
            '<select class="form-control input-sm wf-produto-campo wf-produto-tipo" onchange="wfProdutoEntradaChange(this)">' +
                '<option value="produto"' + ((item.tipoItem || 'produto') === 'produto' ? ' selected' : '') + '>Produto</option>' +
                '<option value="servico_recurso"' + (item.tipoItem === 'servico_recurso' ? ' selected' : '') + '>Serviço/recurso</option>' +
            '</select>' +
            '<input type="number" step="any" min="0" max="100" class="form-control input-sm wf-produto-campo wf-produto-desconto" placeholder="% desc." value="' + esc(item.descontoPercentual == null ? '0' : item.descontoPercentual) + '" oninput="wfProdutoEntradaChange(this)">' +
            '<button type="button" class="btn btn-default btn-sm" onclick="wfProdutoEntradaRemove(this)"><i class="fa fa-times"></i></button>' +
            '</div>';
    }
    function renderProdutosEntrada(e, valor, condicional) {
        var produtos = arrayProdutos(valor);
        if (!produtos.length) produtos = [{ produto: '', tabelaPreco: '', quantidade: '', tipoItem: 'produto', descontoPercentual: 0 }];
        var json = JSON.stringify(produtos);
        var rows = produtos.map(function (p) { return produtoLinhaHtml(p); }).join('');
        return '<div class="wf-test-input-row wf-produtos-input-row' + (condicional ? ' wf-test-input-row-conditional' : '') + '">' +
            '<label title="' + esc(labelTipoEntrada(e.tipo || 'array')) + '">' + esc(e.nome) + (entradaEhObrigatoria(e) ? ' <span class="wf-req">*</span>' : '') + '</label>' +
            '<div class="wf-test-input-field">' +
                '<div class="wf-produtos-editor" data-nome="' + esc(e.nome) + '" data-uso="' + esc(e.obrigatoriedade) + '">' +
                    '<div class="wf-produtos-head"><span>Produto</span><span>Tabela</span><span>Qtd.</span><span>Tipo</span><span>Desc.</span><span></span></div>' +
                    '<div class="wf-produtos-rows">' + rows + '</div>' +
                    '<button type="button" class="btn btn-default btn-xs" onclick="wfProdutoEntradaAdd(this)"><i class="fa fa-plus"></i> Produto</button>' +
                    '<textarea class="wf-test-input wf-produtos-json" data-nome="' + esc(e.nome) + '" data-tipo="array" data-uso="' + esc(e.obrigatoriedade) + '" style="display:none;">' + esc(json) + '</textarea>' +
                '</div>' +
                (e.descricao ? '<span class="help-block">' + esc(e.descricao) + '</span>' : '') +
            '</div>' +
            '</div>';
    }
    function testeTextareas() {
        var arr = [];
        if (el('wfTestInputs')) arr.push(el('wfTestInputs'));
        if (el('wfCanvasTestInputs')) arr.push(el('wfCanvasTestInputs'));
        return arr;
    }
    function testeJsonAtual() {
        var areas = testeTextareas();
        for (var i = 0; i < areas.length; i++) {
            if (trim(areas[i].value || '') && trim(areas[i].value || '') !== '{}') return areas[i].value || '{}';
        }
        return areas.length ? (areas[0].value || '{}') : '{}';
    }
    function sincronizarTesteJson(valor) {
        var areas = testeTextareas();
        for (var i = 0; i < areas.length; i++) areas[i].value = valor;
    }
    function atualizarBotoesModoTeste() {
        var botoes = document.querySelectorAll('[data-wf-modo]');
        for (var i = 0; i < botoes.length; i++) {
            var ativo = botoes[i].getAttribute('data-wf-modo') === testeModo;
            if (ativo) botoes[i].classList.add('active');
            else botoes[i].classList.remove('active');
        }
    }
    function testeObjetoAtual() {
        if (testeModo === 'json') {
            try {
                var parsed = JSON.parse(testeJsonAtual()) || {};
                var merged = clone(entradasRuntimeExtras);
                Object.keys(parsed).forEach(function (k) { merged[k] = parsed[k]; });
                return merged;
            } catch (e) { return clone(entradasRuntimeExtras); }
        }
        return testeCamposObjeto();
    }
    function renderTesteInputs() {
        var boxes = [];
        if (el('wfTestInputsCampos')) boxes.push(el('wfTestInputsCampos'));
        if (el('wfCanvasTestInputsCampos')) boxes.push(el('wfCanvasTestInputsCampos'));
        if (!boxes.length) return;
        var valores = testeObjetoAtual();
        var iniciais = estado.entradas.filter(function (e) { return e.nome && entradaEhInicial(e); });
        var condicionais = estado.entradas.filter(function (e) { return e.nome && !entradaEhInicial(e); });
        if (!estado.entradas.length) {
            boxes.forEach(function (box) { box.innerHTML = '<span class="help-block">Este workflow não declarou entradas.</span>'; });
            sincronizarTesteJson('{}');
            return;
        }
        function campoEntradaHtml(e, valor, condicional) {
            var tipoEntrada = String(e.tipo || '').toLowerCase();
            if (tipoEntrada === 'array' && String(e.nome || '').toLowerCase() === 'produtos') {
                return renderProdutosEntrada(e, valor, condicional);
            }
            if (tipoEntrada === 'array') {
                return renderListaEntrada(e, valor, condicional);
            }
            var controle = e.opcoes && e.opcoes.length
                ? '<select class="form-control input-sm wf-test-input" data-nome="' + esc(e.nome) + '" data-tipo="' + esc(e.tipo || 'string') + '" data-uso="' + esc(e.obrigatoriedade) + '" onchange="wfTesteCampoChange(this)">' + optionsEntradaHtml(e.opcoes, valor, true) + '</select>'
                : (entradaEhJson(e.tipo || 'string')
                    ? '<textarea class="form-control input-sm wf-test-input wf-mono" rows="4" data-nome="' + esc(e.nome) + '" data-tipo="' + esc(e.tipo || 'string') + '" data-uso="' + esc(e.obrigatoriedade) + '" oninput="wfTesteCampoChange(this)">' + esc(typeof valor === 'string' ? valor : JSON.stringify(valor == null ? converterEntradaRuntime('', e.tipo) : valor, null, 2)) + '</textarea>'
                    : '<input type="text" class="form-control input-sm wf-test-input" data-nome="' + esc(e.nome) + '" data-tipo="' + esc(e.tipo || 'string') + '" data-uso="' + esc(e.obrigatoriedade) + '" value="' + esc(valor) + '" oninput="wfTesteCampoChange(this)">');
            return '<div class="wf-test-input-row' + (condicional ? ' wf-test-input-row-conditional' : '') + '">' +
                '<label title="' + esc(labelTipoEntrada(e.tipo || 'string')) + '">' + esc(e.nome) + (entradaEhObrigatoria(e) ? ' <span class="wf-req">*</span>' : '') + '</label>' +
                '<div class="wf-test-input-field">' +
                    controle +
                    (e.descricao ? '<span class="help-block">' + esc(e.descricao) + '</span>' : '') +
                '</div>' +
                '</div>';
        }

        var h = '';
        var grupoAtual = '';
        iniciais.forEach(function (e) {
            e = normalizarEntrada(e);
            var grupo = e.grupo || 'Geral';
            if (grupo !== grupoAtual) {
                grupoAtual = grupo;
                h += '<div class="wf-test-group-title">' + esc(grupoAtual) + '</div>';
            }
            var valor = Object.prototype.hasOwnProperty.call(valores, e.nome) ? valores[e.nome] : valorPadraoEntrada(e.tipo);
            h += campoEntradaHtml(e, valor, false);
        });
        if (condicionais.length) {
            var gruposCond = {};
            condicionais.forEach(function (e) {
                e = normalizarEntrada(e);
                var g = e.grupo || 'Geral';
                if (!gruposCond[g]) gruposCond[g] = [];
                gruposCond[g].push(e);
            });
            h += '<details class="wf-conditional-inputs">' +
                '<summary><strong>Entradas condicionais</strong> <span class="text-muted">(' + condicionais.length + ' campo(s), pedidos apenas se necessário)</span></summary>' +
                '<p class="text-muted">Você pode preencher antes da execução. Se deixar em branco, o workflow vai pedir somente quando chegar ao nó que precisa desses dados.</p>';
            Object.keys(gruposCond).forEach(function (grupo) {
                h += '<div class="wf-conditional-group">' +
                    '<div class="wf-test-group-title">' + esc(grupo) + '</div>';
                gruposCond[grupo].forEach(function (e) {
                    var valor = Object.prototype.hasOwnProperty.call(valores, e.nome) ? valores[e.nome] : '';
                    h += campoEntradaHtml(e, valor, true);
                });
                h += '</div>';
            });
            h += '</details>';
        }
        boxes.forEach(function (box) { box.innerHTML = h || '<span class="help-block">Nenhuma entrada inicial. Entradas condicionais serão pedidas durante a execução.</span>'; });
        if (testeModo === 'inputs') sincronizarTesteJson(testeEntradasJson());
        atualizarBotoesModoTeste();
    }
    function testeCamposObjeto() {
        var o = clone(entradasRuntimeExtras);
        var campos = document.querySelectorAll('#wfCanvasTestInputsCampos .wf-test-input, #wfTestInputsCampos .wf-test-input');
        var lidos = {};
        for (var i = 0; i < campos.length; i++) {
            var inp = campos[i], nome = inp.getAttribute('data-nome'), tipo = inp.getAttribute('data-tipo') || 'string', valor = inp.value;
            if (lidos[nome]) continue;
            lidos[nome] = true;
            var uso = inp.getAttribute('data-uso');
            if ((uso === 'opcional' || uso === 'condicional') && entradaValorVazio(valor, tipo)) {
                delete o[nome];
                continue;
            }
            if (tipo === 'integer') o[nome] = parseInt(valor || '0', 10) || 0;
            else if (tipo === 'number') o[nome] = parseFloat(String(valor || '0').replace(',', '.')) || 0;
            else if (entradaEhJson(tipo)) o[nome] = converterEntradaRuntime(valor, tipo);
            else o[nome] = valor || '';
        }
        return o;
    }
    function testeEntradasJson() {
        var obj;
        if (testeModo === 'json') {
            try { obj = JSON.parse(testeJsonAtual()) || {}; } catch (e) { return testeJsonAtual(); }
            var merged = clone(entradasRuntimeExtras);
            Object.keys(obj).forEach(function (k) { merged[k] = obj[k]; });
            obj = merged;
        } else {
            obj = testeCamposObjeto();
        }
        var json = JSON.stringify(obj, null, 2);
        sincronizarTesteJson(json);
        return json;
    }
    function idWorkflowAtual() {
        var hidden = el(ids.hddId);
        var id = parseInt(hidden ? hidden.value || '0' : '0', 10) || 0;
        if (id > 0) return id;

        var m = /(?:\?|&)id=(\d+)/i.exec(window.location.search || '');
        return m ? (parseInt(m[1], 10) || 0) : 0;
    }
    function aplicarTesteJsonNosCampos(obj) {
        if (!obj || typeof obj !== 'object') return;
        var campos = document.querySelectorAll('#wfCanvasTestInputsCampos .wf-test-input, #wfTestInputsCampos .wf-test-input');
        for (var i = 0; i < campos.length; i++) {
            var nome = campos[i].getAttribute('data-nome');
            var tipo = campos[i].getAttribute('data-tipo') || 'string';
            if (Object.prototype.hasOwnProperty.call(obj, nome)) {
                campos[i].value = entradaEhJson(tipo) ? JSON.stringify(obj[nome] == null ? converterEntradaRuntime('', tipo) : obj[nome], null, 2) : (obj[nome] == null ? '' : obj[nome]);
            }
        }
        var editores = document.querySelectorAll('.wf-produtos-editor');
        for (var j = 0; j < editores.length; j++) {
            var nomeEditor = editores[j].getAttribute('data-nome');
            if (Object.prototype.hasOwnProperty.call(obj, nomeEditor)) {
                window.wfProdutosRenderizar(editores[j], arrayProdutos(obj[nomeEditor]), false);
            }
        }
        var listas = document.querySelectorAll('.wf-lista-editor');
        for (var k = 0; k < listas.length; k++) {
            var nomeLista = listas[k].getAttribute('data-nome');
            if (Object.prototype.hasOwnProperty.call(obj, nomeLista)) {
                window.wfListaRenderizar(listas[k], arrayLista(obj[nomeLista]), false);
            }
        }
    }

    function nosReferenciaveis(noAtual) {
        return estado.nos.filter(function (n) { return n.id !== noAtual && n.tipo !== 'inicio' && n.tipo !== 'fim' && n.tipo !== 'merge'; });
    }
    function refNode(v) { v = str(v); var i = v.indexOf('.'); return i >= 0 ? v.slice(0, i) : v; }
    function refCampo(v) { v = str(v); var i = v.indexOf('.'); return i >= 0 ? v.slice(i + 1) : ''; }
    function ctrlValor(origem, valor, noAtual, fn) {
        if (origem === 'entrada') {
            var opts = '<option value="">Selecione a entrada</option>' + estado.entradas.map(function (e) { return '<option value="' + esc(e.nome) + '"' + (valor === e.nome ? ' selected' : '') + '>' + esc(entradaOptionLabel(e)) + '</option>'; }).join('');
            return '<div class="wf-ref">' +
                '<select class="form-control input-sm" onchange="' + fn('valor') + '">' + opts + '</select>' +
                '<input class="form-control input-sm wf-mono" placeholder="entrada ou caminho: produtos[0].produto" value="' + esc(valor || '') + '" onchange="' + fn('valor') + '">' +
                '</div>';
        }
        if (origem === 'token') {
            return '<select class="form-control input-sm" onchange="' + fn('valor') + '"><option value="usuariologado"' + (valor === 'usuariologado' ? ' selected' : '') + '>Usuário logado (ID)</option></select>';
        }
        if (origem === 'passo') {
            var node = refNode(valor), campo = refCampo(valor);
            var optsN = '<option value="">Selecione o nó</option>' + nosReferenciaveis(noAtual).map(function (n) { return '<option value="' + esc(n.id) + '"' + (node === n.id ? ' selected' : '') + '>' + esc(n.id + ' - ' + tituloNo(n)) + '</option>'; }).join('');
            return '<div class="wf-ref">' +
                '<select class="form-control input-sm" onchange="' + fn('refnode') + '">' + optsN + '</select>' +
                '<input class="form-control input-sm wf-mono" placeholder="campo, dados.campo ou lista[0].id" value="' + esc(campo) + '" onchange="' + fn('refcampo') + '">' +
                '</div>';
        }
        return '<input class="form-control input-sm" placeholder="valor fixo" value="' + esc(valor) + '" onchange="' + fn('valor') + '">';
    }

    function htmlListaLoopValor(no) {
        var origem = no.listaOrigem || 'entrada';
        var valor = no.listaValor || '';
        if (origem === 'passo') {
            var node = refNode(valor), campo = refCampo(valor);
            var optsN = '<option value="">Selecione o nó</option>' + nosReferenciaveis(no.id).map(function (n) {
                return '<option value="' + esc(n.id) + '"' + (node === n.id ? ' selected' : '') + '>' + esc(n.id + ' - ' + tituloNo(n)) + '</option>';
            }).join('');
            return '<div class="wf-ref wf-loop-list-ref">' +
                '<select class="form-control input-sm" onchange="wfLoopListaChange(\'' + esc(no.id) + '\',\'refnode\',this.value)">' + optsN + '</select>' +
                '<input class="form-control input-sm wf-mono" placeholder="registros, produtos, dados.produtos ou lista[0].itens" value="' + esc(campo) + '" oninput="wfLoopListaChange(\'' + esc(no.id) + '\',\'refcampo\',this.value)">' +
                '</div>';
        }

        var opts = '<option value="">Selecione a entrada</option>' + estado.entradas.map(function (e) {
            var tipo = labelTipoEntrada(e.tipo || 'string');
            return '<option value="' + esc(e.nome) + '"' + (valor === e.nome ? ' selected' : '') + '>' + esc(entradaOptionLabel(e) + ' [' + tipo + ']') + '</option>';
        }).join('');
        if (valor && opts.indexOf('value="' + esc(valor) + '"') < 0) opts += '<option value="' + esc(valor) + '" selected>' + esc(valor) + '</option>';
        return '<div class="wf-ref wf-loop-list-ref">' +
            '<select class="form-control input-sm" onchange="wfLoopListaChange(\'' + esc(no.id) + '\',\'valor\',this.value)">' + opts + '</select>' +
            '<input class="form-control input-sm wf-mono" placeholder="produtos ou entrada.produtos" value="' + esc(valor) + '" oninput="wfLoopListaChange(\'' + esc(no.id) + '\',\'valor\',this.value)">' +
            '</div>';
    }

    function parametroLinhaId(noid, idx) {
        return 'wfParamLinha_' + String(noid || '').replace(/[^a-zA-Z0-9_-]/g, '_') + '_' + idx;
    }

    function obterMapeamentoParametro(no, par) {
        if (!no) return null;
        no.entradas = no.entradas || [];
        for (var i = 0; i < no.entradas.length; i++) {
            if (no.entradas[i].param === par.nome) {
                no.entradas[i].vazioExplicito = boolCfg(no.entradas[i].vazioExplicito);
                return no.entradas[i];
            }
        }
        var m = { param: par.nome, origem: 'entrada', valor: '', vazioExplicito: false };
        no.entradas.push(m);
        return m;
    }

    function opcoesOrigemParametro(m) {
        return WF_ORIGENS.map(function (o) {
            return '<option value="' + o[0] + '"' + (m.origem === o[0] ? ' selected' : '') + '>' + o[1] + '</option>';
        }).join('');
    }

    function htmlValorParametroModal(no, par, m, idx) {
        var fnBase = 'wfParametroModalChange(\'' + esc(no.id) + '\',' + idx + ',';
        if (boolCfg(m.vazioExplicito) && permiteVazioParametro(par)) {
            return '<input class="form-control input-sm" value="" placeholder="Vazio intencional" disabled>';
        }

        if (m.origem === 'entrada') {
            var opts = '<option value="">Selecione a entrada</option>' + estado.entradas.map(function (e) {
                return '<option value="' + esc(e.nome) + '"' + (m.valor === e.nome ? ' selected' : '') + '>' + esc(entradaOptionLabel(e)) + '</option>';
            }).join('');
            if (m.valor && opts.indexOf('value="' + esc(m.valor) + '"') < 0) opts += '<option value="' + esc(m.valor) + '" selected>' + esc(m.valor) + '</option>';
            return '<div class="wf-ref">' +
                '<select class="form-control input-sm" onchange="' + fnBase + '\'valor\',this.value)">' + opts + '</select>' +
                '<input class="form-control input-sm wf-mono" placeholder="entrada ou caminho: produtos[0].produto" value="' + esc(m.valor || '') + '" oninput="' + fnBase + '\'valor\',this.value)">' +
                '</div>';
        }

        if (m.origem === 'token') {
            return '<select class="form-control input-sm" onchange="' + fnBase + '\'valor\',this.value)"><option value="usuariologado"' + (m.valor === 'usuariologado' ? ' selected' : '') + '>Usuário logado (ID)</option></select>';
        }

        if (m.origem === 'passo') {
            var node = refNode(m.valor), campo = refCampo(m.valor);
            var optsN = '<option value="">Selecione o nó</option>' + nosReferenciaveis(no.id).map(function (n) {
                return '<option value="' + esc(n.id) + '"' + (node === n.id ? ' selected' : '') + '>' + esc(n.id + ' - ' + tituloNo(n)) + '</option>';
            }).join('');
            return '<div class="wf-ref">' +
                '<select class="form-control input-sm" onchange="' + fnBase + '\'refnode\',this.value)">' + optsN + '</select>' +
                '<input class="form-control input-sm wf-mono" placeholder="campo, dados.campo ou lista[0].id" value="' + esc(campo) + '" oninput="' + fnBase + '\'refcampo\',this.value)">' +
                '</div>';
        }

        var enums = enumParametro(par);
        if (enums.length) {
            return '<select class="form-control input-sm" onchange="' + fnBase + '\'valor\',this.value)">' + enums.map(function (v) {
                var label = v === '' ? '(vazio)' : v;
                return '<option value="' + esc(v) + '"' + (String(m.valor) === String(v) ? ' selected' : '') + '>' + esc(label) + '</option>';
            }).join('') + '</select>';
        }

        if (parametroEhNumero(par)) {
            var attrs = '';
            if (par.minimo != null && par.minimo !== '') attrs += ' min="' + esc(par.minimo) + '"';
            if (par.maximo != null && par.maximo !== '') attrs += ' max="' + esc(par.maximo) + '"';
            attrs += tipoParametro(par).indexOf('integer') >= 0 ? ' step="1"' : ' step="any"';
            return '<input type="number" class="form-control input-sm" value="' + esc(m.valor) + '"' + attrs + ' oninput="' + fnBase + '\'valor\',this.value)">';
        }

        var max = parseInt(par.maxLength || '0', 10) || 0;
        if (max > 120 || /observa|mensagem|descricao|descrição|sid/i.test(par.nome || '')) {
            return '<textarea class="form-control input-sm" rows="3"' + (max ? ' maxlength="' + esc(max) + '"' : '') + ' oninput="' + fnBase + '\'valor\',this.value)">' + esc(m.valor) + '</textarea>';
        }
        return '<input class="form-control input-sm" value="' + esc(m.valor) + '"' + (max ? ' maxlength="' + esc(max) + '"' : '') + ' oninput="' + fnBase + '\'valor\',this.value)">';
    }

    function renderParametroModalLinha(no, cat, par, idx) {
        var m = obterMapeamentoParametro(no, par);
        var pendente = par.obrigatorio && !valorParametroPreenchido(m, par);
        var vazio = boolCfg(m.vazioExplicito) && permiteVazioParametro(par);
        var cls = 'wf-param-card' + (pendente ? ' wf-param-missing' : '') + (vazio ? ' wf-param-empty' : '');
        var tipo = esc(par.tipo || 'string') + (par.minLength != null ? ' | min ' + esc(par.minLength) : '') + (par.maxLength != null ? ' | max ' + esc(par.maxLength) : '');
        var vazioToggle = permiteVazioParametro(par)
            ? '<label><input type="checkbox" ' + (vazio ? ' checked' : '') + ' onchange="wfParametroModalVazio(\'' + esc(no.id) + '\',' + idx + ',this.checked)"> Usar vazio</label>'
            : '';

        return '<div id="' + parametroLinhaId(no.id, idx) + '" class="' + cls + '">' +
            '<div class="wf-param-card-head">' +
                '<div class="wf-param-card-main">' +
                    '<strong>' + esc(par.nome) + '</strong>' + (par.obrigatorio ? ' <span class="wf-req">*</span>' : '') +
                    (par.descricao ? '<span class="help-block">' + esc(par.descricao) + '</span>' : '') +
                '</div>' +
                '<div class="wf-param-card-type">' + tipo + '</div>' +
            '</div>' +
            '<div class="wf-param-editor-row">' +
                '<select class="form-control input-sm" onchange="wfParametroModalChange(\'' + esc(no.id) + '\',' + idx + ',\'origem\',this.value)">' + opcoesOrigemParametro(m) + '</select>' +
                '<div class="wf-param-value">' + htmlValorParametroModal(no, par, m, idx) + '</div>' +
                '<div class="wf-param-empty-toggle">' + vazioToggle + '</div>' +
            '</div>' +
        '</div>';
    }

    function atualizarStatusParametrosModal(no, cat) {
        var status = el('wfParametrosModalStatus');
        if (!status || !no || !cat) return;
        var params = cat.params || [];
        var pendentes = parametrosPendentes(no, params);
        var vazios = (no.entradas || []).filter(function (m) { return boolCfg(m.vazioExplicito); }).length;
        status.innerHTML = (pendentes ? '<span class="label label-warning">' + pendentes + ' pendente(s)</span> ' : '<span class="label label-success">Completo</span> ') +
            '<span>' + params.length + ' parâmetro(s)' + (vazios ? ' | ' + vazios + ' vazio(s) intencional(is)' : '') + '</span>';
    }

    function atualizarLinhaParametroModal(no, cat, idx) {
        if (!no || !cat || !(cat.params || [])[idx]) return;
        var linha = el(parametroLinhaId(no.id, idx));
        if (linha) linha.outerHTML = renderParametroModalLinha(no, cat, cat.params[idx], idx);
        atualizarStatusParametrosModal(no, cat);
        atualizarHidden();
    }

    function atualizarClasseLinhaParametro(no, cat, idx) {
        if (!no || !cat || !(cat.params || [])[idx]) return;
        var par = cat.params[idx];
        var m = obterMapeamentoParametro(no, par);
        var linha = el(parametroLinhaId(no.id, idx));
        if (!linha) return;
        linha.classList.toggle('wf-param-missing', par.obrigatorio && !valorParametroPreenchido(m, par));
        linha.classList.toggle('wf-param-empty', boolCfg(m.vazioExplicito) && permiteVazioParametro(par));
    }

    function deveAplicarZeroSeguro(par) {
        var texto = ((par && par.nome) || '') + ' ' + ((par && par.descricao) || '');
        return parametroEhNumero(par) && /use\s*0|use zero|informar\s*0|preencha\s*0|padr[aã]o\s*0/i.test(texto);
    }

    function deveAplicarStringVaziaSegura(par) {
        return permiteVazioParametro(par) && /string vazia|texto vazio|valor vazio|vazia|vazio/i.test(String((par && par.descricao) || ''));
    }

    function valorEnumSeguro(par, desejado) {
        var enums = enumParametro(par);
        if (!enums.length) return desejado;
        for (var i = 0; i < enums.length; i++) {
            if (String(enums[i]).toUpperCase() === String(desejado).toUpperCase()) return String(enums[i]);
        }
        return '';
    }

    function deveAplicarNaoSeguro(par) {
        var nome = String((par && par.nome) || '').toLowerCase();
        var texto = nome + ' ' + String((par && par.descricao) || '').toLowerCase();
        return nome === 'confidencial' || nome === 'sconfidencial' || /normal\/n[aã]o|sim\/n[aã]o|use n|informe n|padr[aã]o n/.test(texto);
    }

    function renderParametrosModal(no) {
        var cat = no ? catalogoPorNome(no.ferramenta) : null;
        var titulo = el('wfParametrosModalTitulo');
        var subtitulo = el('wfParametrosModalSubtitulo');
        var aviso = el('wfParametrosModalAviso');
        var body = el('wfParametrosModalBody');
        var btnPadroes = el('wfBtnAplicarPadroesParametros');
        if (!titulo || !body) return;

        titulo.innerHTML = esc(no ? tituloNo(no) : 'Parâmetros do nó');
        if (subtitulo) subtitulo.innerHTML = no ? esc(no.id) : '';
        if (btnPadroes) btnPadroes.style.display = cat && /^orcamento_/i.test(cat.nome || '') && cat.escopo === 'WRITE' ? 'inline-block' : 'none';

        if (!no || !cat) {
            if (aviso) aviso.innerHTML = '<div class="alert alert-info">Escolha uma ferramenta antes de configurar parâmetros.</div>';
            body.innerHTML = '';
            return;
        }

        var params = cat.params || [];
        normalizarMapeamentos(no, params);
        if (aviso) {
            aviso.innerHTML = cat.escopo === 'WRITE'
                ? '<div class="alert alert-warning" style="padding:7px 10px;">Passo de escrita: a execução real pausa para confirmação antes de gravar.</div>'
                : '';
        }
        body.innerHTML = params.length
            ? params.map(function (par, idx) { return renderParametroModalLinha(no, cat, par, idx); }).join('')
            : '<p class="text-muted">Esta ferramenta não possui parâmetros.</p>';
        atualizarStatusParametrosModal(no, cat);
    }

    function abrirModalParametros(noid) {
        var no = noPorId(noid);
        if (!no) return;
        parametroModalNoId = noid;
        selectedNoId = noid;
        popupEntradasNoId = '';
        esconderPopupEntradas();
        esconderPopupErro();
        renderParametrosModal(no);
        marcarSelecionado();
        mostrarModalParametrosSeguro();
    }

    function renderProps() {
        var box = el('wfProps');
        if (!box) return;
        var no = noPorId(selectedNoId);
        if (!no) {
            box.innerHTML = '<p class="text-muted">Selecione um nó no Canvas.</p>';
            esconderPopupEntradas();
            atualizarInspectorLayout();
            return;
        }
        var info = tipoInfo(no.tipo);
        var h = '<div class="form-group"><label>ID</label><input class="form-control input-sm wf-mono" value="' + esc(no.id) + '" readonly></div>' +
            '<div class="form-group"><label>Tipo</label><p><i class="fa ' + info.icone + '"></i> ' + esc(info.titulo) + '</p></div>' +
            '<button type="button" class="btn btn-default btn-xs" style="margin-bottom:10px;" onclick="wfToggleNoMinimizado(\'' + esc(no.id) + '\')">' +
                '<i class="fa ' + (boolCfg(no.minimizado) ? 'fa-expand' : 'fa-compress') + '"></i> ' + (boolCfg(no.minimizado) ? 'Expandir no Canvas' : 'Recolher no Canvas') +
            '</button>';

        if (no.tipo === 'ferramenta') h += renderPropsFerramenta(no);
        else if (no.tipo === 'condicao') h += renderPropsCondicao(no);
        else if (no.tipo === 'definir') h += renderPropsDefinir(no);
        else if (no.tipo === 'ia') h += renderPropsIA(no);
        else if (no.tipo === 'merge') h += renderPropsMerge(no);
        else if (no.tipo === 'aprovacao') h += renderPropsAprovacao(no);
        else if (no.tipo === 'loop') h += renderPropsLoop(no);
        else if (no.tipo === 'espera') h += renderPropsEspera(no);
        else if (no.tipo === 'inicio') h += '<p class="text-muted">Conecte a saída do início ao primeiro nó.</p>';
        else if (no.tipo === 'fim') h += '<p class="text-muted">O fim encerra a execução do workflow.</p>';

        var tr = tracePorNo(no.id);
        if (tr) {
            h += '<hr><h5>Dados da última execução</h5>' +
                renderJsonTrace('Entrada', traceCampo(tr, 'Entrada')) +
                renderJsonTrace('Saída', traceCampo(tr, 'Saida'));
        }
        box.innerHTML = h;
        aplicarSelectsFerramenta(box);
        renderPopupEntradas(no);
        atualizarInspectorLayout();
    }

    function renderResumoMapeamentos(no, cat) {
        if (!cat) return '<p class="text-muted">Escolha uma ferramenta para configurar os parâmetros.</p>';
        var params = cat.params || [];
        normalizarMapeamentos(no, params);
        var pendentes = parametrosPendentes(no, params);
        var configurados = 0, vazios = 0, previews = [];
        params.forEach(function (par) {
            var m = (no.entradas || []).filter(function (x) { return x.param === par.nome; })[0];
            if (valorParametroPreenchido(m, par)) configurados++;
            if (m && boolCfg(m.vazioExplicito)) vazios++;
            if (previews.length < 5) previews.push('<div class="wf-param-summary-line"><span class="wf-noid">' + esc(par.nome) + '</span><span class="wf-param-preview" title="' + esc(resumoOrigemParametro(m)) + '">' + esc(resumoOrigemParametro(m)) + '</span></div>');
        });
        var h = '<div class="wf-param-summary">' +
            '<div class="wf-param-summary-line">' +
                '<span class="label label-' + (pendentes ? 'warning' : 'success') + '">' + (pendentes ? pendentes + ' pendente(s)' : 'completo') + '</span>' +
                '<span class="text-muted">' + configurados + ' de ' + params.length + ' parâmetro(s) configurado(s)' + (vazios ? ' | ' + vazios + ' vazio(s)' : '') + '</span>' +
            '</div>' +
            previews.join('') +
            '</div>';
        h += '<button type="button" class="btn btn-primary btn-sm" onclick="wfAbrirParametrosNo(\'' + esc(no.id) + '\')"><i class="fa fa-sliders"></i> Editar parâmetros</button>';
        return h;
    }

    function renderPropsFerramenta(no) {
        var cat = catalogoPorNome(no.ferramenta);
        if (cat) normalizarMapeamentos(no, cat.params || []);
        var h = '<div class="form-group"><label>Ferramenta</label>' + renderFerramentaPicker(no, '- escolha a ferramenta -') + '</div>';
        if (cat && cat.descricao) h += '<div class="wf-tool-desc">' + esc(cat.descricao) + '</div>';
        if (cat && cat.escopo === 'WRITE') h += '<div class="alert alert-warning" style="padding:5px 8px;font-size:12px;">Passo de escrita: a execução real pausa aqui para confirmação.</div>';
        h += renderResumoMapeamentos(no, cat);
        h += renderRetryControl(no);
        h += '<p class="text-muted">A saída principal segue o fluxo normal; a segunda saída é usada apenas quando a ferramenta falhar.</p>';
        return h;
    }

    function renderRetryControl(no) {
        var retries = normalizarRetries(no.retries);
        var intervalo = normalizarIntervaloRetry(no.intervaloRetrySegundos);
        var backoff = boolCfg(no.backoffRetry);
        return '<div class="form-group"><label>Tentativas extras em falha</label>' +
            '<input type="number" min="0" max="3" class="form-control input-sm" value="' + esc(retries) + '" onchange="wfPropChange(\'retries\',this.value)">' +
            '<span class="help-block">Quantidade de novas tentativas automáticas antes de seguir para a saída de erro.</span>' +
            '</div>' +
            '<div class="form-group"><label>Intervalo entre tentativas (segundos)</label>' +
            '<input type="number" min="0" max="10" class="form-control input-sm" value="' + esc(intervalo) + '" onchange="wfPropChange(\'intervaloRetrySegundos\',this.value)">' +
            '<span class="help-block">Use 0 para tentar novamente imediatamente. O limite é 10 segundos.</span>' +
            '</div>' +
            '<div class="checkbox"><label>' +
            '<input type="checkbox" ' + (backoff ? 'checked' : '') + ' onchange="wfPropChange(\'backoffRetry\',this.checked)"> Backoff progressivo' +
            '</label>' +
            '</div>';
    }

    function esconderPopupEntradas() {
        var pop = el('wfInputPopup');
        if (pop) {
            pop.style.display = 'none';
            pop.className = 'wf-input-popup';
        }
    }

    function esconderPopupErro() {
        var pop = el('wfErroPopup');
        if (pop) pop.style.display = 'none';
    }

    function posicionarPopupEntradas(no) {
        var pop = el('wfInputPopup');
        if (!pop || !editor || !no) return;
        var dfId = noParaDf[no.id];
        var node = dfId ? document.getElementById('node-' + dfId) : null;
        var wrap = pop.parentNode;
        if (!node || !wrap) return;

        var nodeRect = node.getBoundingClientRect();
        var wrapRect = wrap.getBoundingClientRect();
        var largura = pop.offsetWidth || 450;
        var left = nodeRect.left - wrapRect.left + (nodeRect.width / 2) - (largura / 2);
        left = Math.max(12, Math.min(left, wrapRect.width - largura - 12));

        pop.classList.remove('wf-input-popup-above');
        var top = nodeRect.bottom - wrapRect.top + 12;
        if (top + pop.offsetHeight > wrapRect.height - 12) {
            top = nodeRect.top - wrapRect.top - pop.offsetHeight - 12;
            pop.classList.add('wf-input-popup-above');
        }
        if (top < 12) top = 12;

        pop.style.left = Math.round(left) + 'px';
        pop.style.top = Math.round(top) + 'px';
    }

    function posicionarPopupErro(no) {
        var pop = el('wfErroPopup');
        if (!pop || !editor || !no) return;
        var dfId = noParaDf[no.id];
        var node = dfId ? document.getElementById('node-' + dfId) : null;
        var wrap = pop.parentNode;
        if (!node || !wrap) return;

        var nodeRect = node.getBoundingClientRect();
        var wrapRect = wrap.getBoundingClientRect();
        var largura = pop.offsetWidth || 390;
        var left = nodeRect.left - wrapRect.left + (nodeRect.width / 2) - (largura / 2);
        left = Math.max(12, Math.min(left, wrapRect.width - largura - 12));

        pop.classList.remove('wf-error-popup-above');
        var top = nodeRect.bottom - wrapRect.top + 12;
        if (top + pop.offsetHeight > wrapRect.height - 12) {
            top = nodeRect.top - wrapRect.top - pop.offsetHeight - 12;
            pop.classList.add('wf-error-popup-above');
        }
        if (top < 12) top = 12;

        pop.style.left = Math.round(left) + 'px';
        pop.style.top = Math.round(top) + 'px';
    }

    function renderPopupErroTrace(trace) {
        var pop = el('wfErroPopup');
        if (!pop) return;
        if (replayAtivo || popupRuntimeAtivo) {
            esconderPopupErro();
            return;
        }

        var erro = ultimoErroTrace(trace);
        if (!erro || !erro.No || erroPopupOcultoPara === erro.No) {
            esconderPopupErro();
            return;
        }

        var no = noPorId(erro.No);
        if (!no) {
            esconderPopupErro();
            return;
        }

        var saida = traceCampo(erro, 'Saida') || erro.Mensagem || erro.mensagem || 'Erro no nó.';
        var entrada = traceCampo(erro, 'Entrada');
        var tratado = erroTratadoTrace(erro);
        pop.innerHTML = '<div class="wf-error-popup-head">' +
            '<strong><i class="fa fa-exclamation-triangle"></i> ' + (tratado ? 'Erro tratado no nó' : 'Erro no nó') + '</strong>' +
            '<button type="button" class="btn btn-default btn-xs" onclick="wfFecharErroNo()"><i class="fa fa-times"></i></button>' +
            '</div>' +
            '<div class="wf-error-popup-body">' +
                '<p><span class="wf-noid">' + esc(erro.No) + '</span> <strong>' + esc(erro.Titulo || tituloNo(no)) + '</strong></p>' +
                (tratado ? '<p class="text-muted">Este erro foi encaminhado pela saída de erro configurada.</p>' : '') +
                renderJsonTrace('Saída do nó', saida) +
                (entrada ? '<details style="margin-top:7px;"><summary>Entrada do nó</summary>' + renderJsonTrace('Entrada do nó', entrada) + '</details>' : '') +
            '</div>';
        pop.style.display = 'block';
        posicionarPopupErro(no);
    }

    function renderPopupEntradas(no, forcar) {
        var pop = el('wfInputPopup');
        if (!pop) return;
        if (popupRuntimeAtivo) return;
        if (replayAtivo) {
            esconderPopupEntradas();
            return;
        }
        if (!no || no.tipo !== 'ferramenta' || (!forcar && popupOcultoPara === no.id)) {
            esconderPopupEntradas();
            return;
        }
        if (!forcar && popupEntradasNoId !== no.id) {
            esconderPopupEntradas();
            return;
        }

        var cat = catalogoPorNome(no.ferramenta);
        if (cat) normalizarMapeamentos(no, cat.params || []);
        var params = cat ? (cat.params || []) : [];
        var pendentes = parametrosPendentes(no, params);

        var h = '<div class="wf-input-popup-head">' +
            '<strong><i class="fa fa-keyboard-o"></i> Entradas da ferramenta</strong>' +
            (pendentes ? '<span class="label label-warning">' + pendentes + ' pendente(s)</span>' : '') +
            '<button type="button" class="btn btn-default btn-xs" onclick="wfFecharEntradasNo()"><i class="fa fa-times"></i></button>' +
            '</div>' +
            '<div class="wf-input-popup-body">' +
            '<div class="form-group"><label>Ferramenta</label>' + renderFerramentaPicker(no, 'Escolha a ferramenta') + '</div>';

        if (!cat) {
            h += '<p class="text-muted">Escolha uma ferramenta para preencher as entradas necessárias.</p>';
        } else {
            if (cat.descricao) h += '<div class="wf-tool-desc">' + esc(cat.descricao) + '</div>';
            if (cat.escopo === 'WRITE') h += '<div class="alert alert-warning" style="padding:5px 8px;font-size:12px;">Esta ferramenta executa escrita e pode pausar para confirmação.</div>';
            h += renderMapeamentos(no, params, 'wfMapChangeNo');
        }

        pop.className = 'wf-input-popup';
        pop.innerHTML = h + '</div>';
        pop.style.display = 'block';
        aplicarSelectsFerramenta(pop);
        posicionarPopupEntradas(no);
    }

    function pausaEhCadastroCliente(pendente) {
        if (!pendente) return false;
        var no = pendente.no || {};
        if (String(no.ferramenta || '').toLowerCase() === 'clientes_criar_para_orcamento') return true;
        var campos = pendente.campos || [];
        for (var i = 0; i < campos.length; i++) {
            if (String(campos[i].grupo || '').toLowerCase() === 'cadastro do cliente') return true;
        }
        return false;
    }

    function tituloPausaEntradas(pendente) {
        if (pausaEhCadastroCliente(pendente)) return 'Precisamos dos dados para cadastrar o cliente';
        return 'Informe as entradas do fluxo';
    }

    function textoPausaEntradas(pendente) {
        if (pendente && pendente.mensagem) return pendente.mensagem;
        if (pausaEhCadastroCliente(pendente)) {
            return 'O cliente não foi localizado e este ramo precisa dessas informações para criar o cadastro antes de continuar o orçamento.';
        }
        return 'Este nó precisa de dados do usuário para continuar.';
    }

    function camposRuntimeFaltantesNo(no, entradasObj) {
        if (!no || no.tipo !== 'ferramenta') return [];
        var cat = catalogoPorNome(no.ferramenta);
        if (!cat) return [];

        var faltantes = [], vistos = {};
        (no.entradas || []).forEach(function (m) {
            if (!m || m.origem !== 'entrada') return;
            var nomeEntrada = trim(m.valor || m.param || '');
            if (!nomeEntrada) return;
            var meta = paramMeta(cat, m.param) || {};
            var def = entradaDefinicao(nomeEntrada) || {};
            var entradaCondicional = normalizarObrigatoriedade(def.obrigatoriedade || 'obrigatorio') === 'condicional';
            var campoBloqueante = !!meta.obrigatorio && !permiteVazioParametro(meta) && !boolCfg(m.vazioExplicito);
            if (!meta.obrigatorio && !entradaCondicional) return;
            if (boolCfg(m.vazioExplicito) && permiteVazioParametro(meta)) return;

            var jaInformado = campoBloqueante
                ? temValorRuntime(entradasObj, nomeEntrada)
                : entradaFoiInformadaRuntime(entradasObj, nomeEntrada);
            if (vistos[nomeEntrada] || jaInformado) return;
            vistos[nomeEntrada] = true;

            faltantes.push({
                nome: nomeEntrada,
                param: m.param || '',
                tipo: def.tipo || meta.tipo || 'string',
                obrigatorio: campoBloqueante,
                grupo: def.grupo || 'Geral',
                obrigatoriedade: def.obrigatoriedade || (campoBloqueante ? 'obrigatorio' : 'opcional'),
                descricao: def.descricao || meta.descricao || '',
                opcoes: normalizarOpcoesEntrada(def.opcoes || meta.enum || [])
            });
        });
        return faltantes;
    }

    function ordemNosExecucaoAproximada() {
        var inicio = primeiroNoTipo('inicio'), fila = [], vistos = {}, ordem = [];
        if (inicio) fila.push(inicio.id);
        while (fila.length) {
            var id = fila.shift();
            if (!id || vistos[id]) continue;
            vistos[id] = true;
            var no = noPorId(id);
            if (!no) continue;
            ordem.push(no);
            if (no.tipo === 'condicao') {
                if (no.seVerdadeiro) fila.push(no.seVerdadeiro);
                if (no.seFalso) fila.push(no.seFalso);
            } else if (no.tipo === 'aprovacao') {
                if (no.proximo) fila.push(no.proximo);
                if (no.seRejeitado) fila.push(no.seRejeitado);
            } else if (no.tipo === 'loop') {
                if (no.proximo) fila.push(no.proximo);
                if (no.aoConcluir) fila.push(no.aoConcluir);
            } else if (no.tipo === 'ferramenta' || no.tipo === 'ia') {
                if (no.proximo) fila.push(no.proximo);
                if (no.emErro) fila.push(no.emErro);
            } else if (no.proximo) {
                fila.push(no.proximo);
            }
        }
        estado.nos.forEach(function (no) {
            if (!vistos[no.id]) ordem.push(no);
        });
        return ordem;
    }

    function primeiraEntradaRuntimePendente(entradasObj) {
        var ordem = ordemNosExecucaoAproximada();
        for (var i = 0; i < ordem.length; i++) {
            var campos = camposRuntimeFaltantesNo(ordem[i], entradasObj);
            if (campos.length) return { no: ordem[i], campos: campos };
        }
        return null;
    }

    function renderPopupRuntimeEntrada(pendente, acao) {
        var pop = el('wfInputPopup');
        if (!pop || !pendente || !pendente.no) return;
        popupRuntimeAtivo = { acao: acao, noid: pendente.no.id, campos: pendente.campos, idExecucao: pendente.idExecucao || execRealId || 0, novasEntradas: {} };
        selectedNoId = pendente.no.id;
        popupOcultoPara = '';
        marcarNoCanvas(pendente.no.id, 'EXECUTANDO');

        var valores = testeObjetoAtual();
        var grupos = {};
        (pendente.campos || []).forEach(function (campo, i) {
            var grupo = campo.grupo || 'Geral';
            if (!grupos[grupo]) grupos[grupo] = [];
            grupos[grupo].push({ campo: campo, indice: i });
        });

        var h = '<div class="wf-input-popup-head">' +
            '<strong><i class="fa fa-keyboard-o"></i> ' + esc(tituloPausaEntradas(pendente)) + '</strong>' +
            '<button type="button" class="btn btn-default btn-xs" onclick="wfCancelarEntradaRuntime()"><i class="fa fa-times"></i></button>' +
            '</div>' +
            '<div class="wf-input-popup-body">' +
            '<p class="text-muted">' + esc(textoPausaEntradas(pendente)) + '</p>' +
            '<p><span class="wf-noid">' + esc(pendente.no.id) + '</span> <strong>' + esc(tituloNo(pendente.no)) + '</strong></p>';

        Object.keys(grupos).forEach(function (grupoNome) {
            h += '<div class="wf-runtime-group">' +
                '<div class="wf-test-group-title">' + esc(grupoNome) + '</div>';
            grupos[grupoNome].forEach(function (item) {
                var campo = item.campo;
                var valorResolvido = valorRuntimePorReferencia(valores, campo.nome);
                var valor = valorResolvido.existe ? valorResolvido.valor : '';
                var usoCampo = campo.obrigatorio ? 'obrigatorio' : 'opcional';
                var controle = campo.opcoes && campo.opcoes.length
                    ? '<select class="form-control input-sm wf-runtime-input" data-i="' + item.indice + '">' + optionsEntradaHtml(campo.opcoes, valor, true) + '</select>'
                    : ((String(campo.tipo || '').toLowerCase() === 'array')
                        ? renderListaEditor(campo.nome, valor, usoCampo, { hiddenClass: 'wf-runtime-input', indice: item.indice, runtime: true })
                        : '<input type="text" class="form-control input-sm wf-runtime-input" data-i="' + item.indice + '" value="' + esc(valor) + '" placeholder="' + esc(campo.param || campo.nome) + '">');
                h += '<div class="form-group">' +
                    '<label>' + esc(campo.nome) + (campo.obrigatorio ? ' <span class="wf-req">*</span>' : '') + '</label>' +
                    controle +
                    '<span class="help-block" style="margin-bottom:0;">' +
                        'Usado em <span class="wf-mono">' + esc(campo.param || 'parâmetro') + '</span> como ' + esc(labelTipoEntrada(campo.tipo)) + '.' +
                        (campo.descricao ? '<br>' + esc(campo.descricao) : '') +
                    '</span>' +
                    '</div>';
            });
            h += '</div>';
        });

        h += '<button type="button" class="btn btn-primary btn-sm" onclick="wfContinuarComEntradasRuntime()"><i class="fa fa-play"></i> ' + (acao === 'teste' ? 'Continuar teste' : 'Continuar workflow') + '</button>' +
            (acao === 'teste' ? '<p class="help-block" style="margin:6px 0 0;">No teste, o fluxo será processado novamente usando esses novos valores.</p>' : '') +
            '</div>';
        pop.className = 'wf-input-popup wf-runtime-input-popup';
        pop.innerHTML = h;
        pop.style.display = 'block';
        posicionarPopupEntradas(pendente.no);
        canvasStatusHtml('warning', esc(tituloPausaEntradas(pendente)) + ' no nó <strong>' + esc(tituloNo(pendente.no)) + '</strong>.');
    }

    function aplicarEntradasRuntimePopup() {
        if (!popupRuntimeAtivo) return false;
        var inputs = document.querySelectorAll('#wfInputPopup .wf-runtime-input');
        var novas = {};
        for (var i = 0; i < inputs.length; i++) {
            var idx = parseInt(inputs[i].getAttribute('data-i') || '0', 10);
            var campo = popupRuntimeAtivo.campos[idx];
            if (!campo) continue;
            var valor = inputs[i].value;
            if (campo.obrigatorio && entradaValorVazio(valor, campo.tipo)) {
                var foco = inputs[i];
                if (String(campo.tipo || '').toLowerCase() === 'array') {
                    var editorLista = listaEditorDoElemento(inputs[i]);
                    var inputLista = editorLista ? editorLista.querySelector('.wf-lista-valor') : null;
                    if (inputLista) foco = inputLista;
                }
                foco.focus();
                canvasStatusHtml('warning', 'Preencha a entrada obrigatória <strong>' + esc(campo.nome) + '</strong>.');
                return false;
            }
            novas[campo.nome] = converterEntradaRuntime(valor, campo.tipo);
            entradasRuntimeExtras[campo.nome] = novas[campo.nome];
        }
        popupRuntimeAtivo.novasEntradas = novas;
        renderTesteInputs();
        testeEntradasJson();
        return true;
    }

    function checarEntradasRuntimeAntesDeExecutar(acao) {
        var entradasObj = testeObjetoAtual();
        var pendente = primeiraEntradaRuntimePendente(entradasObj);
        if (!pendente) return true;
        renderPopupRuntimeEntrada(pendente, acao);
        return false;
    }

    function normalizarMapeamentos(no, params) {
        var atuais = no.entradas || [];
        no.entradas = (params || []).map(function (par) {
            for (var i = 0; i < atuais.length; i++) {
                if (atuais[i].param === par.nome) {
                    atuais[i].vazioExplicito = boolCfg(atuais[i].vazioExplicito);
                    return atuais[i];
                }
            }
            return { param: par.nome, origem: 'entrada', valor: '', vazioExplicito: false };
        });
    }

    function renderMapeamentos(no, params, fnNome) {
        if (!params || !params.length) return '<p class="text-muted">Este nó não possui parâmetros.</p>';
        var h = '<div class="wf-map-scroll"><table class="table table-condensed"><thead><tr><th style="font-size:11px;">Parâmetro</th><th style="font-size:11px;width:34%;">Origem</th><th style="font-size:11px;">Valor ou referência</th></tr></thead><tbody>';
        params.forEach(function (par, k) {
            var m = (no.entradas || []).filter(function (x) { return x.param === par.nome; })[0] || { param: par.nome, origem: 'entrada', valor: '' };
            var origens = WF_ORIGENS.map(function (o) { return '<option value="' + o[0] + '"' + (m.origem === o[0] ? ' selected' : '') + '>' + o[1] + '</option>'; }).join('');
            var fn = function (field) { return fnNome + '(\'' + esc(no.id) + '\',' + k + ',\'' + field + '\',this.value)'; };
            h += '<tr class="wf-map-row">' +
                '<td class="wf-param" title="' + esc(par.descricao || '') + '">' + esc(par.nome) + (par.obrigatorio ? ' <span class="wf-req">*</span>' : '') + '<br><span style="color:#aaa;font-size:10px;">' + esc(par.tipo || 'string') + '</span></td>' +
                '<td><select class="form-control input-sm" onchange="' + fnNome + '(\'' + esc(no.id) + '\',' + k + ',\'origem\',this.value)">' + origens + '</select></td>' +
                '<td>' + ctrlValor(m.origem, m.valor, no.id, fn) + '</td>' +
                '</tr>';
        });
        return h + '</tbody></table></div>';
    }

    function renderPropsCondicao(no) {
        var cnode = refNode(no.campo), ccampo = refCampo(no.campo);
        var optsN = '<option value="">Selecione o nó</option>' + nosReferenciaveis(no.id).map(function (n) { return '<option value="' + esc(n.id) + '"' + (cnode === n.id ? ' selected' : '') + '>' + esc(n.id + ' - ' + tituloNo(n)) + '</option>'; }).join('');
        var ops = WF_OPERADORES.map(function (o) { return '<option value="' + o[0] + '"' + (no.operador === o[0] ? ' selected' : '') + '>' + o[1] + '</option>'; }).join('');
        var origens = WF_ORIGENS.map(function (o) { return '<option value="' + o[0] + '"' + (no.valorOrigem === o[0] ? ' selected' : '') + '>' + o[1] + '</option>'; }).join('');
        var fnValor = function (field) { return 'wfCondChangeNo(\'' + esc(no.id) + '\',\'valor_' + field + '\',this.value)'; };
        return '<div class="form-group"><label>Campo avaliado</label><div class="wf-ref">' +
            '<select class="form-control input-sm" onchange="wfCondChangeNo(\'' + esc(no.id) + '\',\'campo_refnode\',this.value)">' + optsN + '</select>' +
            '<input class="form-control input-sm wf-mono" placeholder="campo, dados.campo ou lista[0].id" value="' + esc(ccampo) + '" onchange="wfCondChangeNo(\'' + esc(no.id) + '\',\'campo_refcampo\',this.value)">' +
            '</div></div>' +
            '<div class="form-group"><label>Comparação</label><select class="form-control input-sm" onchange="wfCondChangeNo(\'' + esc(no.id) + '\',\'operador\',this.value)">' + ops + '</select></div>' +
            '<div class="form-group"><label>Valor comparado</label><select class="form-control input-sm" onchange="wfCondChangeNo(\'' + esc(no.id) + '\',\'valorOrigem\',this.value)">' + origens + '</select></div>' +
            '<div class="form-group">' + (no.valorOrigem === 'fixo' ? '<input class="form-control input-sm" placeholder="valor" value="' + esc(no.valor) + '" onchange="wfCondChangeNo(\'' + esc(no.id) + '\',\'valor\',this.value)">' : ctrlValor(no.valorOrigem, no.valor, no.id, fnValor)) + '</div>' +
            '<p class="text-muted">Use a porta verde para Verdadeiro e a porta âmbar para Falso.</p>';
    }

    function renderPropsDefinir(no) {
        var origens = WF_ORIGENS.map(function (o) { return '<option value="' + o[0] + '"' + (no.origem === o[0] ? ' selected' : '') + '>' + o[1] + '</option>'; }).join('');
        var fn = function (field) { return 'wfDefinirChangeNo(\'' + esc(no.id) + '\',\'' + field + '\',this.value)'; };
        return '<div class="form-group"><label>Origem</label><select class="form-control input-sm" onchange="wfDefinirChangeNo(\'' + esc(no.id) + '\',\'origem\',this.value)">' + origens + '</select></div>' +
            '<div class="form-group"><label>Valor</label>' + ctrlValor(no.origem, no.valor, no.id, fn) + '</div>' +
            '<p class="text-muted">Referencie este nó como <span class="wf-mono">' + esc(no.id) + '.valor</span>.</p>';
    }

    function renderPropsIA(no) {
        var h = '<div class="form-group"><label>Título</label><input class="form-control input-sm" value="' + esc(no.titulo || '') + '" onchange="wfPropChange(\'titulo\',this.value)"></div>' +
            '<div class="form-group"><label>Prompt</label><textarea class="form-control input-sm" rows="5" onchange="wfPropChange(\'prompt\',this.value)">' + esc(no.prompt || '') + '</textarea></div>' +
            '<label>Dados enviados para a IA</label>' + renderIAEntradas(no) +
            '<button type="button" class="btn btn-default btn-xs" onclick="wfIAAddEntrada(\'' + esc(no.id) + '\')"><i class="fa fa-plus"></i> Adicionar dado</button>' +
            '<div class="form-group" style="margin-top:10px;"><label>Esquema JSON de saída esperado</label><textarea class="form-control input-sm wf-mono" rows="4" onchange="wfPropChange(\'schemaSaida\',this.value)">' + esc(no.schemaSaida || '{}') + '</textarea></div>' +
            renderRetryControl(no) +
            '<p class="text-muted">A saída normalizada será <span class="wf-mono">' + esc(no.id) + '.texto</span> e <span class="wf-mono">' + esc(no.id) + '.dados.campo</span>. A segunda saída trata falhas do nó.</p>';
        return h;
    }

    function renderPropsMerge(no) {
        return '<div class="form-group"><label>Título</label><input class="form-control input-sm" value="' + esc(no.titulo || 'Merge') + '" onchange="wfPropChange(\'titulo\',this.value)"></div>' +
            '<p class="text-muted">Use este nó para juntar dois ramos visuais em um único caminho. Ele não transforma dados; apenas segue para o próximo nó conectado.</p>';
    }

    function renderPropsAprovacao(no) {
        return '<div class="form-group"><label>Título</label><input class="form-control input-sm" value="' + esc(no.titulo || 'Aprovação humana') + '" onchange="wfPropChange(\'titulo\',this.value)"></div>' +
            '<div class="form-group"><label>Mensagem para o usuário</label><textarea class="form-control input-sm" rows="4" onchange="wfPropChange(\'mensagem\',this.value)">' + esc(no.mensagem || '') + '</textarea></div>' +
            '<p class="text-muted">A execução pausa neste nó. A primeira saída segue quando o usuário aprova; a segunda segue quando ele rejeita.</p>';
    }

    function renderPropsLoop(no) {
        var modo = no.modoLoop || 'contador';
        var modos = '<option value="contador"' + (modo !== 'lista' ? ' selected' : '') + '>Contador</option><option value="lista"' + (modo === 'lista' ? ' selected' : '') + '>Lista</option>';
        var origem = no.listaOrigem || 'entrada';
        var origens = [['entrada', 'Entrada do fluxo'], ['passo', 'Saída de nó']].map(function (o) { return '<option value="' + o[0] + '"' + (origem === o[0] ? ' selected' : '') + '>' + o[1] + '</option>'; }).join('');
        var h = '<div class="form-group"><label>Título</label><input class="form-control input-sm" value="' + esc(no.titulo || 'Loop') + '" onchange="wfPropChange(\'titulo\',this.value)"></div>' +
            '<div class="form-group"><label>Modo</label><select class="form-control input-sm" onchange="wfPropChange(\'modoLoop\',this.value)">' + modos + '</select></div>';
        if (modo === 'lista') {
            h += '<div class="form-group"><label>Origem da lista</label><select class="form-control input-sm" onchange="wfLoopListaChange(\'' + esc(no.id) + '\',\'origem\',this.value)">' + origens + '</select></div>' +
                '<div class="form-group"><label>Referência da lista</label>' + htmlListaLoopValor(no) + '</div>' +
                '<p class="text-muted">A primeira saída roda uma vez por item. Referências: <span class="wf-mono">' + esc(no.id) + '.item.produto</span>, <span class="wf-mono">' + esc(no.id) + '.item.quantidade</span>, <span class="wf-mono">' + esc(no.id) + '.indice</span>, <span class="wf-mono">' + esc(no.id) + '.indice0</span>, <span class="wf-mono">' + esc(no.id) + '.total</span>.</p>';
        } else {
            h += '<div class="form-group"><label>Limite de repetições</label><input type="number" min="1" max="50" class="form-control input-sm" value="' + esc(no.limite || 3) + '" onchange="wfPropChange(\'limite\',this.value)"></div>' +
                '<p class="text-muted">A primeira saída repete o caminho enquanto o contador estiver abaixo do limite. A segunda saída segue quando o loop termina. Referências disponíveis: <span class="wf-mono">' + esc(no.id) + '.iteracao</span>, <span class="wf-mono">' + esc(no.id) + '.limite</span> e <span class="wf-mono">' + esc(no.id) + '.continuar</span>.</p>';
        }
        return h;
    }

    function renderPropsEspera(no) {
        var modo = no.modo || 'manual';
        var modos = [
            ['manual', 'Continuação manual'],
            ['data_hora', 'Data/hora'],
            ['evento', 'Evento externo']
        ].map(function (m) { return '<option value="' + m[0] + '"' + (modo === m[0] ? ' selected' : '') + '>' + m[1] + '</option>'; }).join('');

        var h = '<div class="form-group"><label>Título</label><input class="form-control input-sm" value="' + esc(no.titulo || 'Espera') + '" onchange="wfPropChange(\'titulo\',this.value)"></div>' +
            '<div class="form-group"><label>Modo</label><select class="form-control input-sm" onchange="wfPropChange(\'modo\',this.value)">' + modos + '</select></div>';

        if (modo === 'data_hora') {
            h += '<div class="form-group"><label>Data/hora para continuar</label><input type="datetime-local" class="form-control input-sm" value="' + esc(no.dataHora || '') + '" onchange="wfPropChange(\'dataHora\',this.value)"></div>';
        } else if (modo === 'evento') {
            h += '<div class="form-group"><label>Nome do evento</label><input class="form-control input-sm wf-mono" placeholder="pedido_aprovado" value="' + esc(no.evento || '') + '" onchange="wfPropChange(\'evento\',this.value)"></div>';
        }

        h += '<div class="form-group"><label>Mensagem</label><textarea class="form-control input-sm" rows="3" onchange="wfPropChange(\'mensagem\',this.value)">' + esc(no.mensagem || '') + '</textarea></div>' +
            '<p class="text-muted">A execução pausa neste nó. Referencie a saída como <span class="wf-mono">' + esc(no.id) + '.status</span>, <span class="wf-mono">' + esc(no.id) + '.modo</span> ou <span class="wf-mono">' + esc(no.id) + '.liberadoEm</span> após continuar.</p>';
        return h;
    }

    function renderIAEntradas(no) {
        var entradas = no.entradas || [];
        if (!entradas.length) return '<p class="text-muted">Nenhum dado configurado.</p>';
        var h = '';
        entradas.forEach(function (m, i) {
            var origens = WF_ORIGENS.map(function (o) { return '<option value="' + o[0] + '"' + (m.origem === o[0] ? ' selected' : '') + '>' + o[1] + '</option>'; }).join('');
            var fn = function (field) { return 'wfIAEntradaChange(\'' + esc(no.id) + '\',' + i + ',\'' + field + '\',this.value)'; };
            h += '<div class="wf-ia-input-row">' +
                '<input class="form-control input-sm wf-mono" placeholder="nome" value="' + esc(m.param || '') + '" onchange="wfIAEntradaChange(\'' + esc(no.id) + '\',' + i + ',\'param\',this.value)">' +
                '<select class="form-control input-sm" onchange="wfIAEntradaChange(\'' + esc(no.id) + '\',' + i + ',\'origem\',this.value)">' + origens + '</select>' +
                ctrlValor(m.origem, m.valor, no.id, fn) +
                '<button type="button" class="btn btn-default btn-sm" onclick="wfIARemoverEntrada(\'' + esc(no.id) + '\',' + i + ')"><i class="fa fa-times"></i></button>' +
                '</div>';
        });
        return h;
    }

    function renderLista() {
        var box = el('wfListaNos');
        if (!box) return;
        var h = '';
        estado.nos.forEach(function (no) {
            if (no.tipo === 'inicio' || no.tipo === 'fim') return;
            var saidas = saidaPrincipal(no).filter(function (v) { return !!v; }).join(', ') || 'sem conexão';
            var tipoLabel = tipoInfo(no.tipo).titulo;
            var saidaLabel = (no.tipo === 'condicao' || no.tipo === 'aprovacao' || no.tipo === 'loop' || no.tipo === 'ferramenta' || no.tipo === 'ia') ? 'Saídas' : 'Próximo';
            var tr = tracePorNo(no.id);
            var label = tr ? '<span class="label label-' + classeTraceLabel(tr) + '">' + esc(statusTraceLabel(tr)) + '</span>' : '';
            h += '<div class="wf-list-no wf-t-' + esc(no.tipo) + '">' +
                '<div style="display:flex;align-items:center;gap:6px;">' +
                '<span class="wf-noid">' + esc(no.id) + '</span><strong>' + esc(tituloNo(no)) + '</strong> ' + label +
                '<span style="margin-left:auto;">' +
                '<button type="button" class="btn btn-default btn-xs" onclick="wfSelecionarNo(\'' + esc(no.id) + '\')"><i class="fa fa-pencil"></i> Editar</button> ' +
                '<button type="button" class="btn btn-danger btn-xs" onclick="wfRemoverNo(\'' + esc(no.id) + '\')"><i class="fa fa-trash"></i></button>' +
                '</span></div>' +
                '<div class="text-muted" style="margin-top:4px;">Tipo: ' + esc(tipoLabel) + ' | ' + esc(saidaLabel) + ': ' + esc(saidas) + '</div>' +
                (no.tipo === 'ferramenta' ? '<div class="wf-list-tool-picker">' + renderFerramentaPicker(no, '- escolha a ferramenta -') + '</div>' : '') +
                '</div>';
        });
        box.innerHTML = h || '<p class="text-muted">Nenhum nó intermediário. Adicione pelo Canvas ou pelos botões abaixo.</p>';
        aplicarSelectsFerramenta(box);
    }

    function removerNo(noid, render) {
        var no = noPorId(noid);
        if (!no || no.tipo === 'inicio' || no.tipo === 'fim') return;
        estado.nos = estado.nos.filter(function (n) { return n.id !== noid; });
        estado.nos.forEach(function (n) {
            if (n.proximo === noid) n.proximo = '';
            if (n.emErro === noid) n.emErro = '';
            if (n.seVerdadeiro === noid) n.seVerdadeiro = '';
            if (n.seFalso === noid) n.seFalso = '';
            if (n.seRejeitado === noid) n.seRejeitado = '';
            if (n.aoConcluir === noid) n.aoConcluir = '';
            Object.keys(n.conexoesVisuais || {}).forEach(function (saida) {
                if (n.conexoesVisuais[saida] && n.conexoesVisuais[saida].destino === noid) delete n.conexoesVisuais[saida];
            });
            (n.entradas || []).forEach(function (m) { if (m.origem === 'passo' && refNode(m.valor) === noid) m.valor = ''; });
        });
        if (selectedNoId === noid) selectedNoId = '';
        if (render !== false) renderTudo();
    }

    function validarGrafo() {
        syncEditorToState();
        var probs = conexoesMultiplasEditor();
        var inicio = 0, fim = 0, idsNo = {};
        estado.nos.forEach(function (no) {
            if (idsNo[no.id]) probs.push('ID duplicado: ' + no.id + '.');
            idsNo[no.id] = true;
            if (no.tipo === 'inicio') inicio++;
            if (no.tipo === 'fim') fim++;
        });
        if (inicio !== 1) probs.push('O workflow deve ter exatamente um nó de início.');
        if (fim !== 1) probs.push('O workflow deve ter exatamente um nó de fim.');

        estado.nos.forEach(function (no) {
            function destinoValido(campo, label) {
                if (!no[campo]) probs.push(no.id + ': conecte a saída ' + label + '.');
                else if (!idsNo[no[campo]]) probs.push(no.id + ': conector aponta para nó inexistente (' + no[campo] + ').');
            }
            function destinoOpcional(campo, label) {
                if (no[campo] && !idsNo[no[campo]]) probs.push(no.id + ': saída ' + label + ' aponta para nó inexistente (' + no[campo] + ').');
            }
            if (no.tipo === 'inicio') destinoValido('proximo', 'principal');
            else if (no.tipo === 'fim') {
                if (no.proximo || no.seVerdadeiro || no.seFalso || no.emErro || no.seRejeitado || no.aoConcluir) probs.push(no.id + ': nó fim não pode ter saída.');
            } else if (no.tipo === 'condicao') {
                if (!no.campo) probs.push(no.id + ': defina o campo da condição.');
                destinoValido('seVerdadeiro', 'verdadeiro');
                destinoValido('seFalso', 'falso');
            } else if (no.tipo === 'aprovacao') {
                destinoValido('proximo', 'aprovado');
                destinoValido('seRejeitado', 'rejeitado');
                if (!trim(no.titulo)) probs.push(no.id + ': informe o título da aprovação.');
            } else if (no.tipo === 'loop') {
                destinoValido('proximo', 'repetir');
                destinoValido('aoConcluir', 'concluir');
                if ((no.modoLoop || 'contador') === 'lista') {
                    if (!trim(no.listaValor)) probs.push(no.id + ': informe a referência da lista.');
                    if (no.listaOrigem !== 'entrada' && no.listaOrigem !== 'passo') probs.push(no.id + ': escolha uma origem de lista válida.');
                } else {
                    var limite = parseInt(no.limite || '0', 10);
                    if (!limite || limite < 1 || limite > 50) probs.push(no.id + ': o limite do loop deve ficar entre 1 e 50.');
                }
            } else if (no.tipo === 'espera') {
                destinoValido('proximo', 'principal');
                if (!trim(no.titulo)) probs.push(no.id + ': informe o título da espera.');
                if (no.modo !== 'manual' && no.modo !== 'data_hora' && no.modo !== 'evento') probs.push(no.id + ': escolha um modo de espera válido.');
                if (no.modo === 'data_hora' && !trim(no.dataHora)) probs.push(no.id + ': informe a data/hora da espera.');
                if (no.modo === 'evento' && !trim(no.evento)) probs.push(no.id + ': informe o nome do evento aguardado.');
            } else {
                destinoValido('proximo', 'principal');
                destinoOpcional('emErro', 'erro');
            }

            if (no.tipo === 'ferramenta') {
                var retriesFerramenta = normalizarRetries(no.retries);
                if (String(no.retries) !== String(retriesFerramenta)) probs.push(no.id + ': retry deve ficar entre 0 e 3.');
                var intervaloFerramenta = normalizarIntervaloRetry(no.intervaloRetrySegundos);
                if (String(no.intervaloRetrySegundos || 0) !== String(intervaloFerramenta)) probs.push(no.id + ': intervalo de retry deve ficar entre 0 e 10 segundos.');
                if (!no.ferramenta) probs.push(no.id + ': escolha uma ferramenta.');
                var cat = catalogoPorNome(no.ferramenta);
                if (!cat && no.ferramenta) probs.push(no.id + ': ferramenta inexistente ou indisponível.');
                (cat ? cat.params : []).forEach(function (par) {
                    if (!par.obrigatorio) return;
                    var m = (no.entradas || []).filter(function (x) { return x.param === par.nome; })[0];
                    if (!valorParametroPreenchido(m, par)) probs.push(no.id + ': parâmetro obrigatório "' + par.nome + '" sem valor.');
                });
            } else if (no.tipo === 'ia') {
                var retriesIA = normalizarRetries(no.retries);
                if (String(no.retries) !== String(retriesIA)) probs.push(no.id + ': retry deve ficar entre 0 e 3.');
                var intervaloIA = normalizarIntervaloRetry(no.intervaloRetrySegundos);
                if (String(no.intervaloRetrySegundos || 0) !== String(intervaloIA)) probs.push(no.id + ': intervalo de retry deve ficar entre 0 e 10 segundos.');
                if (!trim(no.prompt)) probs.push(no.id + ': configure o prompt do nó IA.');
                if (trim(no.schemaSaida || '{}')) {
                    try { JSON.parse(no.schemaSaida || '{}'); } catch (e) { probs.push(no.id + ': schema de saída do nó IA não é JSON válido.'); }
                }
                (no.entradas || []).forEach(function (m, idx) {
                    if (!trim(m.param)) probs.push(no.id + ': dado de entrada IA #' + (idx + 1) + ' sem nome.');
                });
            }
        });
        return probs;
    }

    function mostrarValidacao(probs, avisos) {
        var box = el('wfCanvasAvisos');
        if (!box) return;
        avisos = avisos || [];
        var html = '';
        if (!probs.length) {
            html += '<div class="alert alert-success" style="padding:6px 8px;">Fluxo válido.</div>';
        } else {
            html += '<div class="alert alert-warning" style="padding:6px 8px;">' + esc(probs[0]) + (probs.length > 1 ? '<br><small>Mais ' + (probs.length - 1) + ' ajuste(s).</small>' : '') + '</div>';
        }
        if (avisos.length) {
            var itens = avisos.slice(0, 12).map(function (a) { return '<li>' + esc(a) + '</li>'; }).join('');
            if (avisos.length > 12) itens += '<li>... e mais ' + (avisos.length - 12) + '.</li>';
            html += '<div class="alert alert-info" style="padding:6px 8px;"><strong>Referências e pontos de atenção (' + avisos.length + ')</strong>' +
                '<ul style="margin:4px 0 0 16px;padding:0;">' + itens + '</ul></div>';
        }
        box.innerHTML = html;
        if ((probs.length || avisos.length) && window.wfBottomTab) window.wfBottomTab('problemas');
    }

    function wfPostar(url, payload, ok, falha) {
        var xhr = new XMLHttpRequest();
        xhr.open('POST', url, true);
        xhr.setRequestHeader('Content-Type', 'application/json; charset=utf-8');
        xhr.onreadystatechange = function () {
            if (xhr.readyState !== 4) return;
            var r = {};
            try { r = (JSON.parse(xhr.responseText) || {}).d || {}; } catch (e) { }
            if (xhr.status >= 200 && xhr.status < 300) ok(r);
            else falha(r);
        };
        xhr.send(JSON.stringify(payload));
    }

    function wfToggleDet(idx) { var d = el('wfdet' + idx); if (d) d.style.display = d.style.display === 'none' ? 'block' : 'none'; }
    function duracaoPasso(t) { return Number(t && (t.DuracaoMs || t.duracaoMs)) || 0; }
    function formatarDuracaoMs(ms) {
        ms = Number(ms) || 0;
        if (ms <= 0) return '';
        if (ms < 1000) return ms + ' ms';
        var s = ms / 1000;
        return (s < 10 ? s.toFixed(1) : String(Math.round(s))).replace('.', ',') + ' s';
    }
    // Linha acima do trace: soma do tempo dos passos e o mais lento (só quando passa de 1 s; traces antigos não têm duração).
    function resumoTempoTrace(trace) {
        var total = 0, maisLento = null;
        (trace || []).forEach(function (t) {
            var ms = duracaoPasso(t);
            total += ms;
            if (ms > 0 && (!maisLento || ms > duracaoPasso(maisLento))) maisLento = t;
        });
        if (total < 1000) return '';
        return '<div class="text-muted" style="font-size:12px;margin-bottom:4px;">Tempo nos passos: <strong>' + esc(formatarDuracaoMs(total)) + '</strong>' +
            (maisLento ? ' &middot; mais lento: <span class="wf-noid">' + esc(maisLento.No) + '</span> ' + esc(maisLento.Titulo) + ' (' + esc(formatarDuracaoMs(duracaoPasso(maisLento))) + ')' : '') +
            '</div>';
    }
    function wfTraceHtml(trace) {
        var h = resumoTempoTrace(trace);
        (trace || []).forEach(function (t, idx) {
            var cls = classeTraceLabel(t);
            var entradaTrace = traceCampo(t, 'Entrada');
            var saidaTrace = traceCampo(t, 'Saida');
            var temDet = entradaTrace || saidaTrace;
            var duracao = formatarDuracaoMs(duracaoPasso(t));
            h += '<div class="wf-trace-passo">' +
                '<div style="display:flex;align-items:center;gap:6px;">' +
                '<span class="wf-noid">' + esc(t.No) + '</span><strong class="wf-mono">' + esc(t.Titulo) + '</strong>' +
                '<span style="margin-left:auto;">' +
                (duracao ? '<small class="text-muted" title="Tempo do passo">' + esc(duracao) + '</small> ' : '') +
                (temDet ? '<button type="button" class="btn btn-xs btn-default" onclick="wfToggleDet(' + idx + ')">Detalhes</button> ' : '') +
                '<span class="label label-' + cls + '">' + esc(statusTraceLabel(t)) + '</span></span>' +
                '</div>' +
                (temDet ? '<div id="wfdet' + idx + '" style="display:none;">' +
                    renderJsonTrace('Entrada', entradaTrace) +
                    renderJsonTrace('Saída', saidaTrace) +
                    '</div>' : '') +
                '</div>';
        });
        return h || '<p class="text-muted">Sem passos registrados.</p>';
    }
    function ferramentaPausada(trace) {
        trace = trace || [];
        for (var i = trace.length - 1; i >= 0; i--) if (trace[i] && trace[i].Status === 'PAUSADO' && trace[i].Titulo) return trace[i].Titulo;
        return '';
    }
    function classeExec(status) {
        if (status === 'CONCLUIDO') return 'success';
        if (status === 'PAUSADO') return 'warning';
        if (status === 'CANCELADO') return 'info';
        return 'danger';
    }
    function idExecucaoResposta(r) {
        r = r || {};
        var valor = r.IdExecucao || r.IdExecucaoIA || r.idExecucao || r.idExecucaoIA || r.idExecucaoIa || 0;
        var id = parseInt(valor, 10);
        return isNaN(id) ? 0 : id;
    }
    function descricaoPausaResposta(r, titulo) {
        r = r || {};
        var descricao = trim(r.Descricao || r.Mensagem || '');
        titulo = trim(titulo || '');
        if (!descricao || descricao === titulo) return '';
        return descricao;
    }
    function execucaoRealValida() {
        var id = parseInt(execRealId || '0', 10) || 0;
        if (id > 0) return id;
        var btn = el('wfBtnConfirmarReal');
        id = parseInt(btn ? (btn.getAttribute('data-id-execucao') || '0') : '0', 10) || 0;
        if (id > 0) {
            execRealId = id;
            return id;
        }
        return 0;
    }
    function avisoExecucaoSemId(acao) {
        var msg = 'Não foi possível identificar a execução pausada para ' + acao + '. Execute o workflow novamente e tente confirmar o passo pendente.';
        canvasStatusHtml('danger', esc(msg));
        if (el('wfTrace')) {
            el('wfTrace').innerHTML = '<div class="alert alert-danger"><strong>ERRO</strong> - ' + esc(msg) + '</div>' + wfTraceHtml(ultimoTrace);
        }
    }
    function jsonObjetoSeguro(texto) {
        if (!texto) return {};
        if (typeof texto === 'object') return texto || {};
        try { return JSON.parse(texto || '{}') || {}; } catch (e) { return {}; }
    }
    function saidasRespostaObjeto(r) {
        r = r || {};
        return jsonObjetoSeguro(r.SaidasJson || r.saidasJson || r.Saidas || r.saidas);
    }
    function hidratarTraceComSaidas(trace, r) {
        trace = trace || [];
        var saidas = saidasRespostaObjeto(r);
        if (!saidas || typeof saidas !== 'object') return trace;
        var ocorrencias = {};
        for (var c = 0; c < trace.length; c++) {
            var idContagem = (trace[c] || {}).No || (trace[c] || {}).no || '';
            if (idContagem) ocorrencias[idContagem] = (ocorrencias[idContagem] || 0) + 1;
        }
        for (var i = 0; i < trace.length; i++) {
            var t = trace[i] || {};
            var idNo = t.No || t.no || '';
            if (!idNo || typeof saidas[idNo] === 'undefined') continue;
            if ((ocorrencias[idNo] || 0) > 1) continue;
            var saidaAtual = t.SaidaCompleta || t.saidaCompleta || t.Saida || t.saida || '';
            var pareceResumida = typeof saidaAtual === 'string' && /\s\.\.\.$/.test(saidaAtual);
            if (!t.SaidaCompleta || pareceResumida) {
                t.SaidaCompleta = saidas[idNo];
            }
        }
        return trace;
    }
    function dadosPausaResposta(r) {
        r = r || {};
        var dados = dadosPausaBrutos(r);
        if (dados && dados.campos && dados.campos.length) return dados;

        var trace = r.Trace || [];
        for (var i = trace.length - 1; i >= 0; i--) {
            if (!trace[i] || String(trace[i].Status || '').toUpperCase() !== 'PAUSADO') continue;
            dados = jsonObjetoSeguro(trace[i].EntradaCompleta || trace[i].entradaCompleta || trace[i].Entrada || trace[i].entrada);
            if (dados && dados.campos && dados.campos.length) {
                if (!dados.no) dados.no = trace[i].No || trace[i].no || '';
                if (!dados.titulo) dados.titulo = trace[i].Titulo || trace[i].titulo || '';
                return dados;
            }
        }
        return {};
    }
    function dadosPausaBrutos(r) {
        r = r || {};
        return jsonObjetoSeguro(r.DadosPausaJson || r.dadosPausaJson || r.DadosPausa || r.dadosPausa);
    }
    function renderPausaEscolha(r, selecaoMultipla) {
        var container = el('wfExecRealEscolhas');
        if (!container) return false;
        var dados = dadosPausaBrutos(r);
        var opcoes = dados && dados.opcoes ? dados.opcoes : [];
        if (!opcoes.length) {
            container.innerHTML = '<div class="text-danger">Nenhuma opção disponível para seleção.</div>';
            container.style.display = 'block';
            return false;
        }
        var html = '';
        for (var i = 0; i < opcoes.length; i++) {
            var opcao = opcoes[i] || {};
            var id = 'wfExecEscolha_' + String(execRealId || '0') + '_' + i;
            html += '<label class="wf-exec-escolha" for="' + esc(id) + '">' +
                '<input type="' + (selecaoMultipla ? 'checkbox' : 'radio') + '" name="' + (selecaoMultipla ? 'wfExecSelecaoMultipla' : 'wfExecEscolha') + '" id="' + esc(id) + '" value="__indice__:' + i + '">' +
                '<span><strong>' + esc(opcao.rotulo || opcao.valor || ('Opção ' + (i + 1))) + '</strong>' +
                (opcao.descricao ? '<small>' + esc(opcao.descricao) + '</small>' : '') + '</span></label>';
        }
        container.innerHTML = html;
        container.style.display = 'grid';
        return true;
    }
    function ocultarPausaEscolha() {
        var container = el('wfExecRealEscolhas');
        if (!container) return;
        container.innerHTML = '';
        container.style.display = 'none';
    }
    function mostrarPausaEntradas(r, acao) {
        var dados = dadosPausaResposta(r);
        if (!dados || !dados.campos || !dados.campos.length) return false;

        var no = noPorId(dados.no || '');
        if (!no) no = { id: dados.no || '', tipo: 'ferramenta', ferramenta: dados.ferramenta || dados.titulo || 'Entradas' };
        renderPopupRuntimeEntrada({ no: no, campos: dados.campos, idExecucao: idExecucaoResposta(r), mensagem: dados.mensagem || '', titulo: dados.titulo || '' }, acao);
        return true;
    }
    function renderExecResposta(r, opcoes) {
        r = r || {};
        opcoes = opcoes || {};
        ultimoTrace = hidratarTraceComSaidas(r.Trace || [], r);
        ultimoTipoPausa = r.TipoPausa || '';
        var status = r.Status || 'ERRO';
        el('wfTrace').innerHTML = '<div class="alert alert-' + classeExec(status) + '" style="padding:6px 10px;">' +
            '<strong>' + esc(status) + '</strong> - ' + esc(r.Mensagem || 'Execução processada.') + '</div>' + wfTraceHtml(ultimoTrace);
        if (window.wfBottomTab) window.wfBottomTab(status === 'PAUSADO' ? 'execucao' : 'trace');
        var idResposta = idExecucaoResposta(r);
        if (idResposta > 0) execRealId = idResposta;
        reproduzirTraceNoCanvas(ultimoTrace, status, r.Mensagem || 'Execução processada.', opcoes.inicioReplay || 0);

        if (status === 'PAUSADO') {
            var ehAprovacao = String(ultimoTipoPausa || '').toUpperCase() === 'APROVACAO';
            var ehEspera = String(ultimoTipoPausa || '').toUpperCase() === 'ESPERA';
            var ehEntradas = String(ultimoTipoPausa || '').toUpperCase() === 'ENTRADAS';
            var ehEscolha = String(ultimoTipoPausa || '').toUpperCase() === 'ESCOLHA';
            var ehSelecaoMultipla = String(ultimoTipoPausa || '').toUpperCase() === 'SELECAO_MULTIPLA';
            var tituloPausa = r.Resumo || ferramentaPausada(ultimoTrace) || 'ferramenta';
            el('wfExecRealFerramenta').textContent = tituloPausa;
            if (el('wfExecRealTitulo')) el('wfExecRealTitulo').textContent = ehEntradas ? 'Entradas pendentes' : (ehEspera ? 'Espera pendente' : ((ehEscolha || ehSelecaoMultipla) ? 'Escolha pendente' : 'Confirmação pendente'));
            if (el('wfExecRealRotulo')) el('wfExecRealRotulo').textContent = ehEntradas ? 'Nó:' : (ehAprovacao ? 'Aprovação:' : (ehEspera ? 'Espera:' : ((ehEscolha || ehSelecaoMultipla) ? 'Busca:' : 'Ferramenta:')));
            if (el('wfExecRealDescricao')) {
                var descricaoPausa = descricaoPausaResposta(r, tituloPausa);
                el('wfExecRealDescricao').textContent = descricaoPausa;
                el('wfExecRealDescricao').style.display = descricaoPausa ? 'block' : 'none';
            }
            if (el('wfBtnConfirmarReal')) {
                el('wfBtnConfirmarReal').innerHTML = '<i class="fa fa-check"></i> ' + (ehAprovacao ? 'Aprovar' : (ehEspera ? 'Continuar' : ((ehEscolha || ehSelecaoMultipla) ? 'Continuar com a seleção' : 'Confirmar passo')));
                el('wfBtnConfirmarReal').setAttribute('data-id-execucao', String(execRealId || ''));
                el('wfBtnConfirmarReal').style.display = ehEntradas ? 'none' : 'inline-block';
            }
            if (el('wfBtnRejeitarReal')) el('wfBtnRejeitarReal').style.display = ehAprovacao ? 'inline-block' : 'none';
            el('wfExecRealAprovacao').style.display = 'block';
            el('wfBtnExecutarReal').disabled = true;
            if (ehEntradas) mostrarPausaEntradas(r, 'entradas-real');
            if (ehEscolha || ehSelecaoMultipla) renderPausaEscolha(r, ehSelecaoMultipla); else ocultarPausaEscolha();
        } else {
            el('wfExecRealAprovacao').style.display = 'none';
            ocultarPausaEscolha();
            if (el('wfExecRealDescricao')) el('wfExecRealDescricao').style.display = 'none';
            if (el('wfBtnConfirmarReal')) el('wfBtnConfirmarReal').style.display = 'inline-block';
            if (el('wfBtnRejeitarReal')) el('wfBtnRejeitarReal').style.display = 'none';
            el('wfBtnExecutarReal').disabled = false;
        }
    }

    window.wfOpenConfigModal = function () {
        mostrarModalConfigSeguro();
    };
    window.wfCloseConfigModal = function () {
        var modal = el('wfConfigModal');
        var jq = window.jQuery || window.$;
        if (jq && jq.fn && jq.fn.modal && modal && !modal.classList.contains('wf-modal-fallback')) {
            jq(modal).modal('hide');
            atualizarResumoLayout();
            return;
        }
        fecharModalConfigFallback();
    };
    window.wfOpenShortcutsModal = function () {
        mostrarModalAtalhosSeguro();
    };
    window.wfCloseShortcutsModal = function () {
        var modal = el('wfShortcutsModal');
        var jq = window.jQuery || window.$;
        if (jq && jq.fn && jq.fn.modal && modal && !modal.classList.contains('wf-modal-fallback')) {
            jq(modal).modal('hide');
            return;
        }
        fecharModalAtalhosFallback();
    };
    window.wfHeaderSalvar = function () {
        if (!window.wfSalvar()) return false;
        var btn = document.querySelector('input[id$="_cmdSalvar"],input[id="cmdSalvar"],button[id$="_cmdSalvar"],button[id="cmdSalvar"]');
        if (btn) {
            btn.click();
            return false;
        }
        alert('Botão de salvar não encontrado na página.');
        return false;
    };
    window.wfToggleNodeLibrary = function () {
        var shell = el('wfEditorShell');
        setBibliotecaRecolhida(!(shell && shell.classList.contains('wf-library-collapsed')));
    };
    window.wfFiltrarNos = function (valor) {
        valor = trim(valor).toLowerCase();
        var totalVisiveis = 0;
        var categorias = document.querySelectorAll('[data-wf-category-block]');
        for (var c = 0; c < categorias.length; c++) {
            var visiveisCategoria = 0;
            var itens = categorias[c].querySelectorAll('.wf-palette-item');
            for (var i = 0; i < itens.length; i++) {
                var texto = (itens[i].getAttribute('data-wf-label') || itens[i].textContent || '').toLowerCase();
                var visivel = !valor || texto.indexOf(valor) >= 0;
                itens[i].style.display = visivel ? '' : 'none';
                if (visivel) {
                    visiveisCategoria++;
                    totalVisiveis++;
                }
            }
            categorias[c].style.display = visiveisCategoria ? '' : 'none';
        }
        if (el('wfPaletteEmpty')) el('wfPaletteEmpty').style.display = totalVisiveis ? 'none' : 'block';
    };
    window.wfToggleInspector = function () {
        if (!selectedNoId || !noPorId(selectedNoId)) {
            canvasStatusHtml('info', 'Selecione um nó para abrir o inspetor.');
            inspectorMinimizado = true;
            atualizarInspectorLayout();
            return;
        }
        inspectorMinimizado = !inspectorMinimizado;
        atualizarInspectorLayout();
    };
    window.wfCloseInspector = function () {
        inspectorMinimizado = true;
        atualizarInspectorLayout();
    };
    window.wfBottomTab = function (nome) {
        nome = nome || 'entradas';
        abrirCanvasTab();
        var painel = el('wfBottomPanel');
        if (painel) painel.classList.remove('wf-bottom-collapsed');
        var botoes = document.querySelectorAll('[data-wf-bottom-tab]');
        var panes = document.querySelectorAll('[data-wf-bottom-pane]');
        var achou = false;
        for (var i = 0; i < botoes.length; i++) {
            if (botoes[i].getAttribute('data-wf-bottom-tab') === nome) achou = true;
        }
        if (!achou) nome = 'entradas';
        for (var b = 0; b < botoes.length; b++) {
            botoes[b].classList.toggle('active', botoes[b].getAttribute('data-wf-bottom-tab') === nome);
        }
        for (var p = 0; p < panes.length; p++) {
            panes[p].classList.toggle('active', panes[p].getAttribute('data-wf-bottom-pane') === nome);
        }
    };
    window.wfToggleBottomPanel = function () {
        var painel = el('wfBottomPanel');
        if (!painel) return;
        painel.classList.toggle('wf-bottom-collapsed');
        var btn = painel.querySelector('.wf-bottom-head > .btn');
        if (btn) btn.innerHTML = '<i class="fa ' + (painel.classList.contains('wf-bottom-collapsed') ? 'fa-angle-up' : 'fa-angle-down') + '"></i>';
    };
    window.wfOpenListaAvancada = function () {
        if (window.jQuery && window.jQuery.fn && window.jQuery.fn.tab) window.jQuery('a[href="#abaLista"]').tab('show');
    };
    window.wfAbrirJsonGrafo = function () {
        atualizarHidden();
        var id = registrarJsonViewer('Grafo do workflow', montarGrafo());
        window.wfAbrirJsonViewer(id);
    };

    window.wfSalvar = function () {
        var probs = validarGrafo();
        mostrarValidacao(probs);
        if (probs.length) {
            alert('Corrija antes de salvar:\n\n' + probs.join('\n'));
            return false;
        }
        atualizarHidden();
        return true;
    };
    window.wfValidarAgora = function () {
        var probs = validarGrafo();
        mostrarValidacao(probs);
        if (probs.length) alert('Corrija antes de salvar:\n\n' + probs.join('\n'));

        // Referências entre nós (servidor): nó inexistente, leitura de nó que não roda antes, caminho de saída antigo,
        // entrada não declarada, valor fixo suspeito. Só o que for erro de referência bloqueia o salvamento.
        atualizarHidden();
        var campoGrafo = window.WF_IDS && window.WF_IDS.hddGrafo ? el(window.WF_IDS.hddGrafo) : null;
        var url = (window.WF_URLS && window.WF_URLS.validar) || '/App/Paginas/IA/Workflow_Detalhe.aspx/ValidarGrafo';
        wfPostar(url, { grafoJson: campoGrafo ? campoGrafo.value : '' }, function (r) {
            var erros = r.Erros || [];
            mostrarValidacao(probs.concat(erros), r.Avisos || []);
            if (!probs.length && erros.length) alert('Corrija antes de salvar:\n\n' + erros.join('\n'));
        }, function () { });
    };
    window.wfCanvasAddNo = function (tipo, x, y) {
        syncEditorToState();
        var no = criarNo(tipo, x, y);
        selectedNoId = no.id;
        inspectorMinimizado = false;
        renderTudo();
    };
    window.wfCanvasOrganizar = function () {
        syncEditorToState();
        var info = autoLayout();
        renderTudo();
        if (el('wfCanvasAvisos')) {
            el('wfCanvasAvisos').innerHTML = '<div class="alert alert-info" style="padding:6px 8px;">Canvas organizado em ' + info.colunas + ' coluna(s), com ' + info.nos + ' nó(s).</div>';
        }
    };
    window.wfCanvasCentralizar = function () {
        if (!editor) return;
        if (ajustarZoomParaCaber()) return;
        // Grafo vazio (sem no nenhum ainda): nao ha o que enquadrar, so volta ao ponto de partida.
        editor.zoom_reset();
        aplicarTranslacaoCanvas(0, 0);
    };
    window.wfCanvasZoomIn = function () { if (editor) editor.zoom_in(); };
    window.wfCanvasZoomOut = function () { if (editor) editor.zoom_out(); };
    window.wfCanvasZoomReset = function () { if (editor) editor.zoom_reset(); };
    window.wfCanvasModoMover = function () {
        modoMoverCanvas = !modoMoverCanvas;
        atualizarModoMoverCanvas();
    };
    window.wfCanvasRemoverSelecionado = function () { if (selectedNoId) removerNo(selectedNoId); };
    window.wfAbrirJsonViewer = function (id) {
        var item = jsonViewerCache[id];
        if (!item) return;
        jsonViewerAtual = id;
        var titulo = el('wfJsonModalTitulo');
        var conteudo = el('wfJsonModalConteudo');
        var botaoBaixar = el('wfJsonModalBaixar');
        if (titulo) titulo.innerHTML = '<i class="fa fa-code"></i> ' + esc(item.titulo || 'JSON');
        if (conteudo) conteudo.innerHTML = jsonColorido(item.valor);
        if (botaoBaixar) botaoBaixar.style.display = item.nomeArquivo ? '' : 'none';
        mostrarModalJsonSeguro();
    };
    // Baixa o JSON exibido no momento no modal (so aparece quando registrarJsonViewer recebeu um nome de
    // arquivo - hoje so a exportacao de workflow usa isso).
    window.wfBaixarJsonViewer = function () {
        var item = jsonViewerCache[jsonViewerAtual];
        if (!item || !item.nomeArquivo) return;
        var texto = typeof item.valor === 'string' ? item.valor : JSON.stringify(item.valor, null, 2);
        var blob = new Blob([texto], { type: 'application/json;charset=utf-8' });
        var url = URL.createObjectURL(blob);
        var a = document.createElement('a');
        a.href = url;
        a.download = item.nomeArquivo;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        setTimeout(function () { URL.revokeObjectURL(url); }, 1000);
    };
    // Exporta o workflow salvo (grafo + metadados) pronto para dbo.sp_IA_Workflow_Importar - mesmo pacote de
    // SQL/IA/Workflows/README.md, sem precisar abrir o SSMS. So funciona com o workflow ja salvo (usa o
    // idWorkflowIA do hidden, nao o que estiver digitado e ainda nao salvo em Nome interno).
    window.wfExportarWorkflow = function () {
        var campo = window.WF_IDS && window.WF_IDS.hddId ? el(window.WF_IDS.hddId) : null;
        var idWorkflowIA = parseInt((campo ? campo.value : '0') || '0', 10);
        if (!idWorkflowIA) { alert('Salve o workflow antes de exportar.'); return; }

        wfPostar(WF_URLS.exportar, { idWorkflowIA: idWorkflowIA }, function (r) {
            if (!r || !r.Sucesso) { alert((r && r.Mensagem) || 'Não foi possível exportar o workflow.'); return; }
            var nomeArquivo = 'workflow.json';
            try { nomeArquivo = (JSON.parse(r.WorkflowJson).sNomeInterno || 'workflow') + '.json'; } catch (e) { }
            var id = registrarJsonViewer('Exportar workflow', r.WorkflowJson, nomeArquivo);
            window.wfAbrirJsonViewer(id);
        }, function () { alert('Não foi possível exportar o workflow.'); });
    };
    window.wfFecharJsonViewer = function () {
        var modal = el('wfJsonModal');
        var jq = window.jQuery || window.$;
        if (jq && jq.fn && jq.fn.modal && modal && !modal.classList.contains('wf-modal-fallback')) {
            jq(modal).modal('hide');
            return;
        }
        fecharModalJsonFallback();
    };
    window.wfAbrirParametrosNo = function (noid) {
        var no = noPorId(noid);
        if (!no) return;
        abrirModalParametros(noid);
    };
    window.wfAbrirEntradasNo = function (noid) {
        abrirModalParametros(noid);
    };
    window.wfFecharParametrosModal = function () {
        var jq = window.jQuery || window.$;
        if (jq && jq.fn && jq.fn.modal && el('wfParametrosModal') && !el('wfParametrosModal').classList.contains('wf-modal-fallback')) {
            jq('#wfParametrosModal').modal('hide');
            return;
        }
        fecharModalParametrosFallback();
    };
    window.wfParametroModalChange = function (noid, idx, field, val) {
        var no = noPorId(noid);
        if (!no || no.tipo !== 'ferramenta') return;
        var cat = catalogoPorNome(no.ferramenta);
        var params = cat ? (cat.params || []) : [];
        var par = params[idx];
        if (!cat || !par) return;

        normalizarMapeamentos(no, params);
        selectedNoId = noid;
        parametroModalNoId = noid;
        var m = obterMapeamentoParametro(no, par);

        if (field === 'origem') {
            m.origem = val;
            m.valor = val === 'token' ? 'usuariologado' : '';
            m.vazioExplicito = false;
            atualizarLinhaParametroModal(no, cat, idx);
            return;
        }

        if (field === 'refnode') m.valor = val + (refCampo(m.valor) ? '.' + refCampo(m.valor) : '');
        else if (field === 'refcampo') m.valor = (refNode(m.valor) || '') + (val ? '.' + val : '');
        else m.valor = val;

        if (trim(m.valor)) m.vazioExplicito = false;
        atualizarClasseLinhaParametro(no, cat, idx);
        atualizarStatusParametrosModal(no, cat);
        atualizarHidden();
    };
    window.wfParametroModalVazio = function (noid, idx, checked) {
        var no = noPorId(noid);
        if (!no || no.tipo !== 'ferramenta') return;
        var cat = catalogoPorNome(no.ferramenta);
        var params = cat ? (cat.params || []) : [];
        var par = params[idx];
        if (!cat || !par || !permiteVazioParametro(par)) return;

        normalizarMapeamentos(no, params);
        selectedNoId = noid;
        parametroModalNoId = noid;
        var m = obterMapeamentoParametro(no, par);
        m.vazioExplicito = !!checked;
        if (checked) {
            m.origem = 'fixo';
            m.valor = '';
        }
        atualizarLinhaParametroModal(no, cat, idx);
    };
    window.wfAplicarPadroesParametros = function () {
        var no = noPorId(parametroModalNoId || selectedNoId);
        if (!no || no.tipo !== 'ferramenta') return;
        var cat = catalogoPorNome(no.ferramenta);
        if (!cat) return;

        var params = cat.params || [];
        normalizarMapeamentos(no, params);
        params.forEach(function (par) {
            var m = obterMapeamentoParametro(no, par);
            if (valorParametroPreenchido(m, par)) return;

            if (deveAplicarStringVaziaSegura(par)) {
                m.origem = 'fixo';
                m.valor = '';
                m.vazioExplicito = true;
                return;
            }

            if (deveAplicarZeroSeguro(par)) {
                m.origem = 'fixo';
                m.valor = '0';
                m.vazioExplicito = false;
                return;
            }

            if (deveAplicarNaoSeguro(par)) {
                var valorN = valorEnumSeguro(par, 'N');
                if (valorN) {
                    m.origem = 'fixo';
                    m.valor = valorN;
                    m.vazioExplicito = false;
                }
            }
        });

        renderParametrosModal(no);
        atualizarHidden();
    };
    window.wfFecharEntradasNo = function () {
        if (popupRuntimeAtivo) {
            popupRuntimeAtivo = null;
            esconderPopupEntradas();
            canvasStatusHtml('info', 'Execução aguardando novas entradas.');
            return;
        }
        popupOcultoPara = selectedNoId || '';
        popupEntradasNoId = '';
        esconderPopupEntradas();
    };
    window.wfFecharErroNo = function () {
        var erroAtual = ultimoErroTrace(ultimoTrace);
        if (erroAtual && erroAtual.No) erroPopupOcultoPara = erroAtual.No;
        esconderPopupErro();
    };
    window.wfCancelarEntradaRuntime = function () {
        popupRuntimeAtivo = null;
        esconderPopupEntradas();
        canvasStatusHtml('info', 'Execução aguardando as entradas do usuário.');
    };
    window.wfContinuarComEntradasRuntime = function () {
        if (!popupRuntimeAtivo || !aplicarEntradasRuntimePopup()) return;
        var acao = popupRuntimeAtivo.acao;
        esconderPopupEntradas();
        if (acao === 'entradas-real') {
            window.wfInformarEntradasReal();
            return;
        }
        popupRuntimeAtivo = null;
        if (acao === 'real') window.wfExecutarReal();
        else window.wfTestar();
    };
    window.wfSelecionarNo = function (noid) {
        abrirCanvasTab(function () {
            selectedNoId = noid;
            inspectorMinimizado = false;
            popupOcultoPara = '';
            popupEntradasNoId = '';
            wfCanvasRender();
            selectedNoId = noid;
            renderProps();
            marcarSelecionado();
        });
    };
    window.wfRemoverNo = function (noid) { removerNo(noid); };
    window.wfAddPasso = function (tipo) {
        syncEditorToState();
        var no = criarNo(tipo, 300 + estado.nos.length * 35, 180 + estado.nos.length * 25);
        inspectorMinimizado = false;
        var fim = primeiroNoTipo('fim');
        var ligado = false;
        for (var i = 0; i < estado.nos.length; i++) {
            if (estado.nos[i].id !== no.id && estado.nos[i].tipo !== 'fim' && estado.nos[i].proximo === (fim ? fim.id : 'nFim')) {
                estado.nos[i].proximo = no.id;
                ligado = true;
                break;
            }
        }
        if (!ligado) {
            var inicio = primeiroNoTipo('inicio');
            if (inicio && (!inicio.proximo || inicio.proximo === (fim ? fim.id : 'nFim'))) inicio.proximo = no.id;
        }
        if (no.tipo === 'condicao') {
            no.seVerdadeiro = fim ? fim.id : 'nFim';
            no.seFalso = fim ? fim.id : 'nFim';
        } else if (no.tipo === 'aprovacao') {
            no.proximo = fim ? fim.id : 'nFim';
            no.seRejeitado = fim ? fim.id : 'nFim';
        } else if (no.tipo === 'loop') {
            no.proximo = fim ? fim.id : 'nFim';
            no.aoConcluir = fim ? fim.id : 'nFim';
        } else {
            no.proximo = fim ? fim.id : 'nFim';
        }
        selectedNoId = no.id;
        renderTudo();
    };

    window.wfAddInput = function () { syncInputsEditorToState(); estado.entradas.push(normalizarEntrada({ nome: '', tipo: 'string', grupo: 'Geral', obrigatoriedade: 'obrigatorio' })); renderTudo(false); };
    window.wfRemoverInput = function (i) { syncInputsEditorToState(); estado.entradas.splice(i, 1); renderTudo(false); };
    window.wfMoverInput = function (i, direcao) {
        syncInputsEditorToState();
        var destino = i + direcao;
        if (i < 0 || destino < 0 || i >= estado.entradas.length || destino >= estado.entradas.length) return;
        var atual = estado.entradas.splice(i, 1)[0];
        estado.entradas.splice(destino, 0, atual);
        renderTudo(false);
    };
    window.wfInputDragStart = function (ev, i) {
        if (ev && ev.dataTransfer) {
            ev.dataTransfer.effectAllowed = 'move';
            ev.dataTransfer.setData('text/plain', String(i));
        }
        var row = ev && ev.target ? ev.target : null;
        while (row && !row.classList.contains('wf-input-row')) row = row.parentNode;
        if (row) row.classList.add('wf-input-dragging');
    };
    window.wfInputDragEnd = function () {
        var rows = document.querySelectorAll('.wf-input-row');
        for (var i = 0; i < rows.length; i++) rows[i].classList.remove('wf-input-dragging', 'wf-input-drop-target');
    };
    window.wfInputDragOver = function (ev) {
        if (ev) {
            ev.preventDefault();
            if (ev.dataTransfer) ev.dataTransfer.dropEffect = 'move';
        }
        var row = ev && ev.currentTarget ? ev.currentTarget : null;
        if (row) row.classList.add('wf-input-drop-target');
    };
    window.wfInputDragLeave = function (ev) {
        var row = ev && ev.currentTarget ? ev.currentTarget : null;
        if (row) row.classList.remove('wf-input-drop-target');
    };
    window.wfInputDrop = function (ev, destino) {
        if (ev) ev.preventDefault();
        syncInputsEditorToState();
        var origem = ev && ev.dataTransfer ? parseInt(ev.dataTransfer.getData('text/plain') || '-1', 10) : -1;
        window.wfInputDragEnd();
        if (origem < 0 || destino < 0 || origem === destino || origem >= estado.entradas.length || destino >= estado.entradas.length) return;
        var item = estado.entradas.splice(origem, 1)[0];
        estado.entradas.splice(destino, 0, item);
        renderTudo(false);
    };
    window.wfInputChange = function (i, campo, valor) {
        if (estado.entradas[i]) {
            if (campo === 'opcoesTexto') estado.entradas[i].opcoes = opcoesEntradaDoTexto(valor);
            else estado.entradas[i][campo] = valor;
            estado.entradas[i] = normalizarEntrada(estado.entradas[i]);
        }
        renderTudo(false);
    };
    window.wfAplicarModeloEntradasOrcamento = function () {
        var modelo = [
            { nome: 'cliente', tipo: 'string', grupo: 'Pedido', obrigatoriedade: 'obrigatorio', descricao: 'Nome, razão social ou documento do cliente.' },
            { nome: 'produtos', tipo: 'array', grupo: 'Pedido', obrigatoriedade: 'obrigatorio', descricao: 'Lista de produtos com produto, tabelaPreco opcional, quantidade, tipoItem e descontoPercentual.' },
            { nome: 'tipoOrcamento', tipo: 'string', grupo: 'Pedido', obrigatoriedade: 'obrigatorio', descricao: 'Tipo de orçamento informado pelo usuário.' },
            { nome: 'referencia', tipo: 'string', grupo: 'Pedido', obrigatoriedade: 'obrigatorio', descricao: 'Referência do orçamento/CRM.' },
            { nome: 'dataEstimativaEntrega', tipo: 'string', grupo: 'Pedido', obrigatoriedade: 'obrigatorio', descricao: 'Data prevista de entrega.' },
            { nome: 'observacao', tipo: 'string', grupo: 'Pedido', obrigatoriedade: 'opcional', descricao: 'Observação do usuário para gravar no orçamento.' },
            { nome: 'modoUsuarioOrcamento', tipo: 'string', grupo: 'Responsável pelo orçamento', obrigatoriedade: 'condicional', descricao: 'Escolha se o responsável será o usuário logado ou outro usuário do sistema.', opcoes: [{ valor: 'USUARIO_LOGADO', rotulo: 'Usar meu usuário' }, { valor: 'OUTRO_USUARIO', rotulo: 'Escolher outro usuário' }] },
            { nome: 'condicaoPagamento', tipo: 'string', grupo: 'Padrões do orçamento', obrigatoriedade: 'opcional', descricao: 'Condição de pagamento desejada.' },
            { nome: 'empresa', tipo: 'string', grupo: 'Padrões do orçamento', obrigatoriedade: 'opcional', descricao: 'Empresa vinculada ao orçamento.' },
            { nome: 'tabelaPreco', tipo: 'string', grupo: 'Padrões do orçamento', obrigatoriedade: 'opcional', descricao: 'Tabela de preço desejada.' },
            { nome: 'tipoEnvio', tipo: 'string', grupo: 'Padrões do orçamento', obrigatoriedade: 'opcional', descricao: 'Forma de envio desejada.' },
            { nome: 'documento', tipo: 'string', grupo: 'Cadastro do cliente', obrigatoriedade: 'condicional', descricao: 'CPF/CNPJ ou documento equivalente.' },
            { nome: 'permitirSemDocumento', tipo: 'string', grupo: 'Cadastro do cliente', obrigatoriedade: 'condicional', descricao: 'Use S para permitir cadastro sem documento.' },
            { nome: 'contatoNome', tipo: 'string', grupo: 'Cadastro do cliente', obrigatoriedade: 'condicional', descricao: 'Nome do contato principal.' },
            { nome: 'contatoTelefone', tipo: 'string', grupo: 'Cadastro do cliente', obrigatoriedade: 'condicional', descricao: 'Telefone do contato.' },
            { nome: 'contatoEmail', tipo: 'string', grupo: 'Cadastro do cliente', obrigatoriedade: 'condicional', descricao: 'E-mail do contato.' },
            { nome: 'cep', tipo: 'string', grupo: 'Cadastro do cliente', obrigatoriedade: 'condicional', descricao: 'CEP do endereço fiscal.' },
            { nome: 'logradouro', tipo: 'string', grupo: 'Cadastro do cliente', obrigatoriedade: 'condicional', descricao: 'Logradouro do endereço fiscal.' },
            { nome: 'numero', tipo: 'string', grupo: 'Cadastro do cliente', obrigatoriedade: 'condicional', descricao: 'Número do endereço fiscal.' },
            { nome: 'bairro', tipo: 'string', grupo: 'Cadastro do cliente', obrigatoriedade: 'condicional', descricao: 'Bairro do endereço fiscal.' },
            { nome: 'cidade', tipo: 'string', grupo: 'Cadastro do cliente', obrigatoriedade: 'condicional', descricao: 'Cidade do endereço fiscal.' },
            { nome: 'uf', tipo: 'string', grupo: 'Cadastro do cliente', obrigatoriedade: 'condicional', descricao: 'UF do endereço fiscal.' },
            { nome: 'pais', tipo: 'string', grupo: 'Cadastro do cliente', obrigatoriedade: 'condicional', descricao: 'País do endereço fiscal.' }
        ];
        var antigos = { produto: true, quantidade: true, valorunitario: true, vendedor: true };
        estado.entradas = estado.entradas.filter(function (e) { return !(e && e.nome && antigos[String(e.nome).toLowerCase()]); });
        var porNome = {};
        estado.entradas.forEach(function (e, idx) {
            if (e && e.nome) porNome[e.nome.toLowerCase()] = idx;
        });
        modelo.forEach(function (e) {
            var chave = e.nome.toLowerCase();
            if (Object.prototype.hasOwnProperty.call(porNome, chave)) {
                var atual = normalizarEntrada(estado.entradas[porNome[chave]]);
                atual.tipo = e.tipo;
                atual.grupo = e.grupo;
                atual.obrigatoriedade = e.obrigatoriedade;
                atual.opcoes = normalizarOpcoesEntrada(e.opcoes);
                if (!atual.descricao) atual.descricao = e.descricao;
                estado.entradas[porNome[chave]] = normalizarEntrada(atual);
            } else {
                estado.entradas.push(normalizarEntrada(e));
            }
        });
        renderTudo(false);
    };
    window.wfToggleNoMinimizado = function (noid) {
        syncEditorToState();
        var no = noPorId(noid);
        if (!no) return;
        no.minimizado = !boolCfg(no.minimizado);
        selectedNoId = noid;
        atualizarHidden();
        wfCanvasRender();
        renderProps();
    };
    window.wfCanvasRecolherTodos = function () {
        syncEditorToState();
        estado.nos.forEach(function (no) {
            if (no.tipo !== 'inicio' && no.tipo !== 'fim') no.minimizado = true;
        });
        atualizarHidden();
        wfCanvasRender();
        renderProps();
    };
    window.wfCanvasExpandirTodos = function () {
        syncEditorToState();
        estado.nos.forEach(function (no) { no.minimizado = false; });
        atualizarHidden();
        wfCanvasRender();
        renderProps();
    };

    window.wfPropChange = function (campo, valor) {
        var no = noPorId(selectedNoId);
        if (!no) return;
        if (campo === 'retries') no[campo] = normalizarRetries(valor);
        else if (campo === 'intervaloRetrySegundos') no[campo] = normalizarIntervaloRetry(valor);
        else if (campo === 'backoffRetry') no[campo] = !!valor;
        else no[campo] = valor;
        renderTudo();
    };
    window.wfLoopListaChange = function (noid, campo, valor) {
        var no = noPorId(noid);
        if (!no) return;
        selectedNoId = noid;

        if (campo === 'origem') {
            no.listaOrigem = valor === 'passo' ? 'passo' : 'entrada';
            no.listaValor = '';
            renderTudo(false);
            return;
        }

        if (campo === 'refnode') {
            var campoAtual = refCampo(no.listaValor);
            no.listaValor = valor ? valor + (campoAtual ? '.' + campoAtual : '') : '';
        } else if (campo === 'refcampo') {
            var nodeAtual = refNode(no.listaValor);
            no.listaValor = nodeAtual ? nodeAtual + (valor ? '.' + valor : '') : '';
        } else {
            no.listaValor = valor || '';
        }

        atualizarHidden();
    };
    window.wfSelFerramentaNo = function (noid, nome) {
        var no = noPorId(noid);
        if (!no) return;
        no.ferramenta = nome;
        selectedNoId = noid;
        popupOcultoPara = '';
        var cat = catalogoPorNome(nome), novas = [];
        if (cat) {
            (cat.params || []).forEach(function (par) {
                var ex = (no.entradas || []).filter(function (x) { return x.param === par.nome; })[0];
                if (ex) {
                    ex.vazioExplicito = boolCfg(ex.vazioExplicito);
                    novas.push(ex);
                } else {
                    novas.push({ param: par.nome, origem: 'entrada', valor: '', vazioExplicito: false });
                }
            });
        }
        no.entradas = novas;
        renderTudo();
    };
    window.wfMapChangeNo = function (noid, mi, field, val) {
        var no = noPorId(noid);
        if (!no || !no.entradas || !no.entradas[mi]) return;
        selectedNoId = noid;
        popupOcultoPara = '';
        var m = no.entradas[mi];
        if (field === 'origem') {
            m.origem = val;
            m.valor = '';
            m.vazioExplicito = false;
            atualizarHidden();
            renderProps();
            wfCanvasRender();
            return;
        }
        if (field === 'refnode') m.valor = val + (refCampo(m.valor) ? '.' + refCampo(m.valor) : '');
        else if (field === 'refcampo') m.valor = (refNode(m.valor) || '') + (val ? '.' + val : '');
        else m.valor = val;
        if (trim(m.valor)) m.vazioExplicito = false;
        atualizarHidden();
        renderProps();
        wfCanvasRender();
    };
    window.wfCondChangeNo = function (noid, field, val) {
        var no = noPorId(noid);
        if (!no) return;
        if (field === 'campo_refnode') no.campo = val + (refCampo(no.campo) ? '.' + refCampo(no.campo) : '');
        else if (field === 'campo_refcampo') no.campo = (refNode(no.campo) || '') + (val ? '.' + val : '');
        else if (field === 'valorOrigem') { no.valorOrigem = val; no.valor = ''; renderProps(); return; }
        else if (field === 'valor_refnode') no.valor = val + (refCampo(no.valor) ? '.' + refCampo(no.valor) : '');
        else if (field === 'valor_refcampo') no.valor = (refNode(no.valor) || '') + (val ? '.' + val : '');
        else no[field] = val;
    };
    window.wfDefinirChangeNo = function (noid, field, val) {
        var no = noPorId(noid);
        if (!no) return;
        if (field === 'origem') { no.origem = val; no.valor = ''; renderProps(); return; }
        if (field === 'refnode') no.valor = val + (refCampo(no.valor) ? '.' + refCampo(no.valor) : '');
        else if (field === 'refcampo') no.valor = (refNode(no.valor) || '') + (val ? '.' + val : '');
        else no.valor = val;
    };
    window.wfIAAddEntrada = function (noid) {
        var no = noPorId(noid);
        if (!no) return;
        no.entradas = no.entradas || [];
        no.entradas.push({ param: 'dado' + (no.entradas.length + 1), origem: 'entrada', valor: '' });
        renderProps();
    };
    window.wfIARemoverEntrada = function (noid, idx) {
        var no = noPorId(noid);
        if (!no || !no.entradas) return;
        no.entradas.splice(idx, 1);
        renderProps();
    };
    window.wfIAEntradaChange = function (noid, idx, field, val) {
        var no = noPorId(noid);
        if (!no || !no.entradas || !no.entradas[idx]) return;
        var m = no.entradas[idx];
        if (field === 'origem') { m.origem = val; m.valor = ''; renderProps(); return; }
        if (field === 'refnode') m.valor = val + (refCampo(m.valor) ? '.' + refCampo(m.valor) : '');
        else if (field === 'refcampo') m.valor = (refNode(m.valor) || '') + (val ? '.' + val : '');
        else m[field] = val;
    };

    window.wfPreencherTeste = function () {
        var o = {};
        estado.entradas.forEach(function (e) {
            e = normalizarEntrada(e);
            if (e.nome && entradaEhObrigatoria(e)) o[e.nome] = valorPadraoEntrada(e.tipo);
        });
        entradasRuntimeExtras = clone(o);
        sincronizarTesteJson(JSON.stringify(o, null, 2));
        renderTesteInputs();
    };
    window.wfSetTesteModo = function (modo) {
        testeModo = modo === 'json' ? 'json' : 'inputs';
        if (testeModo === 'json') {
            sincronizarTesteJson(JSON.stringify(testeCamposObjeto(), null, 2));
            if (el('wfTestInputsCampos')) el('wfTestInputsCampos').style.display = 'none';
            if (el('wfCanvasTestInputsCampos')) el('wfCanvasTestInputsCampos').style.display = 'none';
            if (el('wfTestJsonBox')) el('wfTestJsonBox').style.display = 'block';
            if (el('wfCanvasTestJsonBox')) el('wfCanvasTestJsonBox').style.display = 'block';
        } else {
            try {
                entradasRuntimeExtras = JSON.parse(testeJsonAtual()) || {};
                aplicarTesteJsonNosCampos(entradasRuntimeExtras);
            } catch (e) { }
            if (el('wfTestInputsCampos')) el('wfTestInputsCampos').style.display = 'block';
            if (el('wfCanvasTestInputsCampos')) el('wfCanvasTestInputsCampos').style.display = 'block';
            if (el('wfTestJsonBox')) el('wfTestJsonBox').style.display = 'none';
            if (el('wfCanvasTestJsonBox')) el('wfCanvasTestJsonBox').style.display = 'none';
        }
        atualizarBotoesModoTeste();
    };
    window.wfProdutosRenderizar = function (editor, produtos, sincronizar) {
        if (!editor) return;
        produtos = arrayProdutos(produtos);
        if (!produtos.length) produtos = [{ produto: '', tabelaPreco: '', quantidade: '', tipoItem: 'produto', descontoPercentual: 0 }];
        var rows = editor.querySelector('.wf-produtos-rows');
        if (rows) rows.innerHTML = produtos.map(function (p) { return produtoLinhaHtml(p); }).join('');
        var hidden = editor.querySelector('.wf-produtos-json');
        if (hidden) hidden.value = JSON.stringify(produtos);
        if (sincronizar !== false) window.wfProdutosSync(editor);
    };
    window.wfProdutosSync = function (editor) {
        if (!editor) return;
        var produtos = [];
        var linhas = editor.querySelectorAll('.wf-produto-row');
        for (var i = 0; i < linhas.length; i++) {
            var produto = trim((linhas[i].querySelector('.wf-produto-nome') || {}).value || '');
            var tabelaPreco = trim((linhas[i].querySelector('.wf-produto-tabela') || {}).value || '');
            var quantidadeTxt = (linhas[i].querySelector('.wf-produto-quantidade') || {}).value || '';
            var tipoItem = (linhas[i].querySelector('.wf-produto-tipo') || {}).value || 'produto';
            var descontoTxt = (linhas[i].querySelector('.wf-produto-desconto') || {}).value || '0';
            if (!produto && !tabelaPreco && !trim(quantidadeTxt)) continue;
            produtos.push({
                produto: produto,
                tabelaPreco: tabelaPreco,
                quantidade: parseFloat(String(quantidadeTxt || '0').replace(',', '.')) || 0,
                tipoItem: tipoItem,
                descontoPercentual: parseFloat(String(descontoTxt || '0').replace(',', '.')) || 0
            });
        }
        var hidden = editor.querySelector('.wf-produtos-json');
        if (hidden) {
            hidden.value = JSON.stringify(produtos);
            window.wfTesteCampoChange(hidden);
        }
    };
    window.wfProdutoEntradaAdd = function (btn) {
        var editor = btn ? btn.parentNode : null;
        if (!editor) return;
        var rows = editor.querySelector('.wf-produtos-rows');
        if (rows) rows.insertAdjacentHTML('beforeend', produtoLinhaHtml({ produto: '', tabelaPreco: '', quantidade: '', tipoItem: 'produto', descontoPercentual: 0 }));
        window.wfProdutosSync(editor);
    };
    window.wfProdutoEntradaRemove = function (btn) {
        var row = btn;
        while (row && !row.classList.contains('wf-produto-row')) row = row.parentNode;
        var editor = row;
        while (editor && !editor.classList.contains('wf-produtos-editor')) editor = editor.parentNode;
        if (row && row.parentNode) row.parentNode.removeChild(row);
        if (editor && !editor.querySelector('.wf-produto-row')) {
            var rows = editor.querySelector('.wf-produtos-rows');
            if (rows) rows.insertAdjacentHTML('beforeend', produtoLinhaHtml({ produto: '', tabelaPreco: '', quantidade: '', tipoItem: 'produto', descontoPercentual: 0 }));
        }
        window.wfProdutosSync(editor);
    };
    window.wfProdutoEntradaChange = function (input) {
        var editor = input;
        while (editor && !editor.classList.contains('wf-produtos-editor')) editor = editor.parentNode;
        window.wfProdutosSync(editor);
    };
    window.wfListaRenderizar = function (editor, itens, sincronizar) {
        if (!editor) return;
        itens = arrayLista(itens);
        if (!itens.length) itens = [''];
        var rows = editor.querySelector('.wf-lista-rows');
        if (rows) rows.innerHTML = itens.map(function (item) { return listaLinhaHtml(item); }).join('');
        var hidden = editor.querySelector('.wf-lista-json');
        if (hidden) hidden.value = JSON.stringify(arrayLista(itens).filter(function (item) { return trim(listaValorParaTexto(item)) !== ''; }));
        if (sincronizar !== false) window.wfListaSync(editor);
    };
    window.wfListaSync = function (editor) {
        if (!editor) return;
        var valores = [];
        var linhas = editor.querySelectorAll('.wf-lista-row');
        for (var i = 0; i < linhas.length; i++) {
            var input = linhas[i].querySelector('.wf-lista-valor');
            var texto = input ? trim(input.value) : '';
            if (!texto) continue;
            valores.push(listaTextoParaValor(texto));
        }
        var hidden = editor.querySelector('.wf-lista-json');
        if (hidden) {
            hidden.value = JSON.stringify(valores);
            if (hidden.classList.contains('wf-test-input')) window.wfTesteCampoChange(hidden);
        }
    };
    window.wfListaEntradaAdd = function (btn) {
        var editor = listaEditorDoElemento(btn);
        if (!editor) return;
        var rows = editor.querySelector('.wf-lista-rows');
        if (rows) rows.insertAdjacentHTML('beforeend', listaLinhaHtml(''));
        window.wfListaSync(editor);
        var inputs = editor.querySelectorAll('.wf-lista-valor');
        if (inputs.length) inputs[inputs.length - 1].focus();
    };
    window.wfListaEntradaRemove = function (btn) {
        var row = btn;
        while (row && !row.classList.contains('wf-lista-row')) row = row.parentNode;
        var editor = listaEditorDoElemento(row);
        if (row && row.parentNode) row.parentNode.removeChild(row);
        if (editor && !editor.querySelector('.wf-lista-row')) {
            var rows = editor.querySelector('.wf-lista-rows');
            if (rows) rows.insertAdjacentHTML('beforeend', listaLinhaHtml(''));
        }
        window.wfListaSync(editor);
    };
    window.wfListaEntradaChange = function (input) {
        window.wfListaSync(listaEditorDoElemento(input));
    };
    window.wfTesteCampoChange = function (input) {
        var nome = input.getAttribute('data-nome');
        var tipo = input.getAttribute('data-tipo') || 'string';
        var uso = input.getAttribute('data-uso') || 'obrigatorio';
        if (!nome) return;
        if ((uso === 'opcional' || uso === 'condicional') && entradaValorVazio(input.value, tipo)) {
            delete entradasRuntimeExtras[nome];
        } else {
            var valor = converterEntradaRuntime(input.value, tipo);
            entradasRuntimeExtras[nome] = valor;
        }
        var campos = document.querySelectorAll('.wf-test-input');
        for (var i = 0; i < campos.length; i++) {
            if (campos[i] !== input && campos[i].getAttribute('data-nome') === nome) campos[i].value = input.value;
        }
        if (tipo === 'array') atualizarEditoresLista(nome, converterEntradaRuntime(input.value, tipo), listaEditorDoElemento(input));
        if (testeModo === 'inputs') sincronizarTesteJson(JSON.stringify(testeCamposObjeto(), null, 2));
    };
    window.wfTesteJsonChange = function (textarea) {
        var areas = testeTextareas();
        for (var i = 0; i < areas.length; i++) {
            if (areas[i] !== textarea) areas[i].value = textarea.value;
        }
        if (testeModo === 'json') {
            try {
                entradasRuntimeExtras = JSON.parse(textarea.value || '{}') || {};
                aplicarTesteJsonNosCampos(entradasRuntimeExtras);
            } catch (e) { }
        }
    };
    window.wfTestar = function () {
        var out = el('wfTrace');
        if (window.wfBottomTab) window.wfBottomTab('trace');
        var probs = validarGrafo();
        mostrarValidacao(probs);
        if (probs.length) {
            out.innerHTML = '<div class="alert alert-warning">Corrija o fluxo antes de testar.</div>';
            canvasStatusHtml('warning', esc(probs[0]));
            return;
        }
        out.innerHTML = '<p class="text-muted"><i class="fa fa-spinner fa-spin"></i> Executando...</p>';
        ultimoTipoPausa = '';
        el('wfExecRealAprovacao').style.display = 'none';
        iniciarExecucaoNoCanvas('Testando workflow com as entradas atuais...');
        var entradasJson = testeEntradasJson();
        try { JSON.parse(entradasJson); } catch (e) {
            out.innerHTML = '<div class="alert alert-danger">JSON de entradas inválido.</div>';
            canvasStatusHtml('danger', 'JSON de entradas inválido.');
            if (window.wfBottomTab) window.wfBottomTab('entradas');
            return;
        }
        wfPostar(urls.testar, { grafoJson: JSON.stringify(montarGrafo()), entradasJson: entradasJson }, function (r) {
            if (!r || typeof r.Sucesso === 'undefined') {
                out.innerHTML = '<div class="alert alert-danger">Falha ao executar o teste.</div>';
                reproduzirTraceNoCanvas([], 'ERRO', 'Falha ao executar o teste.');
                return;
            }
            ultimoTrace = hidratarTraceComSaidas(r.Trace || [], r);
            ultimoTipoPausa = r.TipoPausa || '';
            var statusTeste = r.Status || (r.Sucesso ? 'CONCLUIDO' : 'ERRO');
            out.innerHTML = '<div class="alert alert-' + classeExec(statusTeste) + '" style="padding:6px 10px;"><strong>' + esc(statusTeste) + '</strong> - ' + esc(r.Mensagem) + '</div>' + wfTraceHtml(ultimoTrace);
            reproduzirTraceNoCanvas(ultimoTrace, statusTeste, r.Mensagem);
            if (statusTeste === 'PAUSADO' && String(ultimoTipoPausa || '').toUpperCase() === 'ENTRADAS') {
                if (window.wfBottomTab) window.wfBottomTab('execucao');
                mostrarPausaEntradas(r, 'teste');
            }
        }, function () {
            out.innerHTML = '<div class="alert alert-danger">Falha ao executar o teste.</div>';
            reproduzirTraceNoCanvas([], 'ERRO', 'Falha ao executar o teste.');
        });
    };
    window.wfExecutarReal = function () {
        var out = el('wfTrace');
        var idWorkflowIA = idWorkflowAtual();
        var entradasJson = testeEntradasJson();
        var btn = el('wfBtnExecutarReal');
        if (window.wfBottomTab) window.wfBottomTab('execucao');
        try { JSON.parse(entradasJson); } catch (e) {
            out.innerHTML = '<div class="alert alert-danger">JSON de entradas inválido.</div>';
            canvasStatusHtml('danger', 'JSON de entradas inválido.');
            if (window.wfBottomTab) window.wfBottomTab('entradas');
            return;
        }
        execRealId = 0;
        execAcaoEmAndamento = false;
        el('wfExecRealAprovacao').style.display = 'none';
        out.innerHTML = '<p class="text-muted"><i class="fa fa-spinner fa-spin"></i> Executando...</p>';
        iniciarExecucaoNoCanvas('Executando workflow salvo com as entradas atuais...');
        btn.disabled = true;
        btn.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Executando...';
        wfPostar(urls.iniciar, { idWorkflowIA: idWorkflowIA, entradasJson: entradasJson }, function (r) {
            renderExecResposta(r);
            btn.innerHTML = '<i class="fa fa-play"></i> Executar workflow';
        }, function () {
            renderExecResposta({ Status: 'ERRO', Mensagem: 'Não foi possível iniciar a execução.', Trace: [] });
            btn.innerHTML = '<i class="fa fa-play"></i> Executar workflow';
        });
    };
    window.wfInformarEntradasReal = function () {
        if (execAcaoEmAndamento) return;
        var ativo = popupRuntimeAtivo;
        var idExecucao = ativo && ativo.idExecucao ? parseInt(ativo.idExecucao, 10) || 0 : execucaoRealValida();
        if (!idExecucao) {
            avisoExecucaoSemId('informar entradas');
            popupRuntimeAtivo = null;
            return;
        }

        var novas = ativo && ativo.novasEntradas ? ativo.novasEntradas : {};
        var inicioReplay = (ultimoTrace || []).length;
        execAcaoEmAndamento = true;
        canvasStatusHtml('info', '<i class="fa fa-spinner fa-spin"></i> Enviando entradas e retomando o workflow...');
        wfPostar(urls.informarEntradas, { idExecucaoIA: idExecucao, entradasJson: JSON.stringify(novas) }, function (r) {
            popupRuntimeAtivo = null;
            renderExecResposta(r, { inicioReplay: inicioReplay });
            execAcaoEmAndamento = false;
        }, function () {
            popupRuntimeAtivo = null;
            renderExecResposta({ Status: 'ERRO', Mensagem: 'Não foi possível informar as entradas pendentes.', Trace: [] });
            execAcaoEmAndamento = false;
        });
    };
    window.wfConfirmarReal = function () {
        if (execAcaoEmAndamento) return;
        var btn = el('wfBtnConfirmarReal');
        var idExecucao = execucaoRealValida();
        if (!idExecucao) {
            avisoExecucaoSemId('confirmar');
            return;
        }
        var inicioReplay = (ultimoTrace || []).length;
        var ehEscolha = String(ultimoTipoPausa || '').toUpperCase() === 'ESCOLHA';
        var ehSelecaoMultipla = String(ultimoTipoPausa || '').toUpperCase() === 'SELECAO_MULTIPLA';
        var escolha = null;
        if (ehEscolha) {
            escolha = document.querySelector('input[name="wfExecEscolha"]:checked');
            if (!escolha) {
                canvasStatusHtml('warning', 'Selecione uma opção para o workflow continuar.');
                if (el('wfExecRealEscolhas')) el('wfExecRealEscolhas').scrollIntoView({ behavior: 'smooth', block: 'nearest' });
                return;
            }
        }
        if (ehSelecaoMultipla) {
            escolha = [];
            var selecoes = document.querySelectorAll('input[name="wfExecSelecaoMultipla"]:checked');
            for (var i = 0; i < selecoes.length; i++) escolha.push(selecoes[i].value);
        }
        execAcaoEmAndamento = true;
        btn.disabled = true;
        btn.innerHTML = '<i class="fa fa-spinner fa-spin"></i> ' + ((ehEscolha || ehSelecaoMultipla) ? 'Aplicando escolha...' : 'Confirmando...');
        canvasStatusHtml('info', '<i class="fa fa-spinner fa-spin"></i> ' + ((ehEscolha || ehSelecaoMultipla) ? 'Aplicando a seleção...' : 'Confirmando passo pendente...'));
        var valorEscolha = ehSelecaoMultipla ? JSON.stringify(escolha) : (ehEscolha ? escolha.value : null);
        wfPostar((ehEscolha || ehSelecaoMultipla) ? urls.selecionarOpcao : urls.confirmar, (ehEscolha || ehSelecaoMultipla) ? { idExecucaoIA: idExecucao, valor: valorEscolha } : { idExecucaoIA: idExecucao }, function (r) {
            renderExecResposta(r, { inicioReplay: inicioReplay });
            execAcaoEmAndamento = false;
            btn.disabled = false;
            btn.innerHTML = '<i class="fa fa-check"></i> ' + (String(ultimoTipoPausa || '').toUpperCase() === 'APROVACAO' ? 'Aprovar' : (String(ultimoTipoPausa || '').toUpperCase() === 'ESPERA' ? 'Continuar' : ((String(ultimoTipoPausa || '').toUpperCase() === 'ESCOLHA' || String(ultimoTipoPausa || '').toUpperCase() === 'SELECAO_MULTIPLA') ? 'Continuar com a seleção' : 'Confirmar passo')));
        }, function () {
            renderExecResposta({ Status: 'ERRO', Mensagem: (ehEscolha || ehSelecaoMultipla) ? 'Não foi possível aplicar a seleção.' : 'Não foi possível confirmar o passo.', Trace: [] });
            execAcaoEmAndamento = false;
            btn.disabled = false;
            btn.innerHTML = '<i class="fa fa-check"></i> ' + (String(ultimoTipoPausa || '').toUpperCase() === 'APROVACAO' ? 'Aprovar' : (String(ultimoTipoPausa || '').toUpperCase() === 'ESPERA' ? 'Continuar' : ((String(ultimoTipoPausa || '').toUpperCase() === 'ESCOLHA' || String(ultimoTipoPausa || '').toUpperCase() === 'SELECAO_MULTIPLA') ? 'Continuar com a seleção' : 'Confirmar passo')));
        });
    };
    window.wfFocarEscolhaReal = function () {
        if (window.wfBottomTab) window.wfBottomTab('execucao');
        var escolhas = el('wfExecRealEscolhas');
        if (escolhas) escolhas.scrollIntoView({ behavior: 'smooth', block: 'nearest' });
    };
    window.wfRejeitarReal = function () {
        if (execAcaoEmAndamento) return;
        var btn = el('wfBtnRejeitarReal');
        if (!btn) return;
        var idExecucao = execucaoRealValida();
        if (!idExecucao) {
            avisoExecucaoSemId('rejeitar');
            return;
        }
        var inicioReplay = (ultimoTrace || []).length;
        execAcaoEmAndamento = true;
        btn.disabled = true;
        btn.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Rejeitando...';
        canvasStatusHtml('info', '<i class="fa fa-spinner fa-spin"></i> Rejeitando aprovação pendente...');
        wfPostar(urls.rejeitar, { idExecucaoIA: idExecucao }, function (r) {
            renderExecResposta(r, { inicioReplay: inicioReplay });
            execAcaoEmAndamento = false;
            btn.disabled = false;
            btn.innerHTML = '<i class="fa fa-times"></i> Rejeitar';
        }, function () {
            renderExecResposta({ Status: 'ERRO', Mensagem: 'Não foi possível rejeitar a aprovação.', Trace: [] });
            execAcaoEmAndamento = false;
            btn.disabled = false;
            btn.innerHTML = '<i class="fa fa-times"></i> Rejeitar';
        });
    };
    window.wfCancelarReal = function () {
        if (execAcaoEmAndamento) return;
        var btn = el('wfBtnCancelarReal');
        var idExecucao = execucaoRealValida();
        if (!idExecucao) {
            avisoExecucaoSemId('cancelar');
            return;
        }
        var inicioReplay = (ultimoTrace || []).length;
        execAcaoEmAndamento = true;
        btn.disabled = true;
        btn.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Cancelando...';
        canvasStatusHtml('info', '<i class="fa fa-spinner fa-spin"></i> Cancelando execução...');
        wfPostar(urls.cancelar, { idExecucaoIA: idExecucao }, function (r) {
            renderExecResposta(r, { inicioReplay: inicioReplay });
            execAcaoEmAndamento = false;
            btn.disabled = false;
            btn.innerHTML = '<i class="fa fa-ban"></i> Cancelar execução';
        }, function () {
            renderExecResposta({ Status: 'ERRO', Mensagem: 'Não foi possível cancelar a execução.', Trace: [] });
            execAcaoEmAndamento = false;
            btn.disabled = false;
            btn.innerHTML = '<i class="fa fa-ban"></i> Cancelar execução';
        });
    };
    window.wfToggleDet = wfToggleDet;

    function init() {
        carregarGrafo();
        prepararLayoutWorkflow();
        renderInputs();
        renderLista();
        iniciarCanvas();
        renderProps();
        atualizarHidden();
        aplicarSelectsFerramenta(document);
        // So no primeiro carregamento: depois disso, mexer no zoom sozinho atrapalharia quem esta editando.
        // O layout do container as vezes so se acerta depois deste ponto (fonte/CSS), por isso o setTimeout.
        setTimeout(ajustarZoomParaCaber, 80);
        if (window.jQuery) {
            window.jQuery('a[data-toggle="tab"]').on('shown.bs.tab', function () {
                setTimeout(function () {
                    aplicarSelectsFerramenta(document);
                    if (editor && window.jQuery('a[href="#abaCanvas"]').parent().hasClass('active')) {
                        editor.updateConnectionNodes('node-1');
                        wfCanvasRender();
                        atualizarResumoLayout();
                    }
                }, 50);
            });
        }
    }

    if (document.readyState !== 'loading') init();
    else document.addEventListener('DOMContentLoaded', init);
})();
