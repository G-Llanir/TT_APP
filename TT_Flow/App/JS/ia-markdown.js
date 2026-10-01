// =============================================================================================
// Renderizador de markdown seguro, compartilhado pelas telas de IA.
//
// Extraido do Chat.aspx quando o modal de Arquivos.aspx passou a formatar markdown: duplicar
// ~250 linhas seria ruim por si, mas o motivo forte e a seguranca - normalizarHref/hrefPermitido
// so aceitam http(s) e /App//API/, barrando javascript:. Essa regra tem que viver num lugar so.
//
// Depende de jQuery ($.each / $.map / $.trim). Usa $, nao jQuery: no main.master os dois globais
// apontam para instancias diferentes (ver _Contexto/TT_Flow/padroes/frontend.md).
// =============================================================================================

window.IA_Markdown = (function () {
    'use strict';

    // Padroes = o que o Chat sempre usou, para a saida dele continuar identica apos a extracao.
    // Outras telas passam as suas classes via o parametro 'opcoes'.
    var PADRAO = {
        wrapTabela: 'ia-chat-table-wrap',
        classeTabela: '',
        classeCodigo: 'ia-chat-code',
        htmlVazio: '<p class="ia-chat-empty-response">Resposta sem conteudo textual. Reenvie a pergunta para gerar novamente.</p>'
    };

    function opcao(opcoes, nome) {
        return (opcoes && opcoes[nome] !== undefined) ? opcoes[nome] : PADRAO[nome];
    }

    function atributoClasse(valor) {
        return valor ? ' class="' + valor + '"' : '';
    }

    function renderizar(texto, opcoes) {
        texto = String(texto || '').replace(/\u00a0/g, ' ').trim();
        if (!texto) {
            return opcao(opcoes, 'htmlVazio');
        }

        var linhas = texto.replace(/\r\n/g, '\n').replace(/\r/g, '\n').split('\n');
        var html = [];
        var paragrafo = [];
        var listaAberta = '';

        function fecharParagrafo() {
            if (!paragrafo.length) return;
            html.push('<p>' + paragrafo.join('<br />') + '</p>');
            paragrafo = [];
        }

        function fecharLista() {
            if (!listaAberta) return;
            html.push('</' + listaAberta + '>');
            listaAberta = '';
        }

        function abrirLista(tipo) {
            if (listaAberta === tipo) return;
            fecharLista();
            html.push('<' + tipo + '>');
            listaAberta = tipo;
        }

        function linhaTabela(linha) {
            return /^\s*\|.+\|\s*$/.test(String(linha || ''));
        }

        function linhaSeparadoraTabela(linha) {
            return /^\s*\|?\s*:?-{3,}:?\s*(\|\s*:?-{3,}:?\s*)+\|?\s*$/.test(String(linha || ''));
        }

        function celulasTabela(linha) {
            linha = String(linha || '').trim();
            if (linha.charAt(0) === '|') linha = linha.substring(1);
            if (linha.charAt(linha.length - 1) === '|') linha = linha.substring(0, linha.length - 1);
            return $.map(linha.split('|'), function (celula) {
                return $.trim(celula);
            });
        }

        function renderizarTabela(inicio) {
            var cabecalho = celulasTabela(linhas[inicio]);
            var rows = [];
            var indice = inicio + 2;

            while (indice < linhas.length && linhaTabela(linhas[indice]) && !linhaSeparadoraTabela(linhas[indice])) {
                rows.push(celulasTabela(linhas[indice]));
                indice++;
            }

            if (!cabecalho.length || !rows.length) {
                return 0;
            }

            var tabela = ['<div' + atributoClasse(opcao(opcoes, 'wrapTabela')) + '><table' + atributoClasse(opcao(opcoes, 'classeTabela')) + '><thead><tr>'];
            $.each(cabecalho, function (_, celula) {
                tabela.push('<th>' + renderizarInlineSeguro(celula) + '</th>');
            });
            tabela.push('</tr></thead><tbody>');

            $.each(rows, function (_, row) {
                tabela.push('<tr>');
                for (var c = 0; c < cabecalho.length; c++) {
                    tabela.push('<td>' + renderizarInlineSeguro(row[c] || '') + '</td>');
                }
                tabela.push('</tr>');
            });

            tabela.push('</tbody></table></div>');
            html.push(tabela.join(''));
            return indice - inicio;
        }

        for (var i = 0; i < linhas.length; i++) {
            var linhaOriginal = linhas[i];
            var linha = String(linhaOriginal || '').replace(/\s+$/g, '');
            var conteudo = $.trim(linha);

            if (!conteudo) {
                fecharParagrafo();
                fecharLista();
                continue;
            }

            if (/^```/.test(conteudo)) {
                fecharParagrafo();
                fecharLista();
                var codigo = [];
                i++;
                while (i < linhas.length && !/^\s*```/.test(linhas[i])) {
                    codigo.push(linhas[i]);
                    i++;
                }
                html.push('<pre' + atributoClasse(opcao(opcoes, 'classeCodigo')) + '><code>' + escaparHtml(codigo.join('\n')) + '</code></pre>');
                continue;
            }

            if (/^\s*>/.test(linha)) {
                fecharParagrafo();
                fecharLista();
                var citacao = [];
                while (i < linhas.length && /^\s*>/.test(linhas[i])) {
                    citacao.push(renderizarInlineSeguro(String(linhas[i]).replace(/^\s*>\s?/, '')));
                    i++;
                }
                i--;
                html.push('<blockquote>' + citacao.join('<br />') + '</blockquote>');
                continue;
            }

            if (linhaTabela(linha) && i + 1 < linhas.length && linhaSeparadoraTabela(linhas[i + 1])) {
                fecharParagrafo();
                fecharLista();
                var linhasConsumidas = renderizarTabela(i);
                if (linhasConsumidas > 0) {
                    i += linhasConsumidas - 1;
                    continue;
                }
            }

            var titulo = linha.match(/^\s{0,3}#{1,4}\s+(.+)$/);
            if (titulo) {
                fecharParagrafo();
                fecharLista();
                html.push('<p><strong>' + renderizarInlineSeguro(titulo[1]) + '</strong></p>');
                continue;
            }

            var itemOrdenado = linha.match(/^\s*\d+\.\s+(.+)$/);
            if (itemOrdenado) {
                fecharParagrafo();
                abrirLista('ol');
                html.push('<li>' + renderizarInlineSeguro(itemOrdenado[1]) + '</li>');
                continue;
            }

            var itemNaoOrdenado = linha.match(/^\s*[-*]\s+(.+)$/);
            if (itemNaoOrdenado) {
                fecharParagrafo();
                abrirLista('ul');
                html.push('<li>' + renderizarInlineSeguro(itemNaoOrdenado[1]) + '</li>');
                continue;
            }

            fecharLista();
            paragrafo.push(renderizarInlineSeguro(linha));
        }

        fecharParagrafo();
        fecharLista();

        return html.join('') || '&nbsp;';
    }

    function renderizarInlineSeguro(texto) {
        var html = escaparHtml(texto || '');
        var blocos = [];

        function guardar(htmlBloco) {
            var token = '@@IAMDBLOCK' + blocos.length + '@@';
            blocos.push({ token: token, html: htmlBloco });
            return token;
        }

        html = html.replace(/\[([^\]\n]+)\]\(([^)\s]+)\)/g, function (_, label, href) {
            return guardar(criarLinkSeguro(href, label));
        });

        html = html.replace(/`([^`\n]+)`/g, function (_, codigo) {
            var href = normalizarHref(codigo);
            if (hrefPermitido(href)) {
                return guardar(criarLinkSeguro(href, '<code>' + codigo + '</code>'));
            }

            return guardar('<code>' + codigo + '</code>');
        });

        html = html.replace(/(^|[\s(])((?:https?:\/\/|~?\/?App\/|~?\/?API\/)[^\s<>"']*)/gi, function (_, prefixo, hrefTexto) {
            var separado = separarPontuacaoUrl(hrefTexto);
            var href = normalizarHref(separado.href);

            if (!hrefPermitido(href)) {
                return prefixo + hrefTexto;
            }

            return prefixo + guardar(criarLinkSeguro(href, separado.href)) + separado.sufixo;
        });

        html = html.replace(/~~([^~\n]+)~~/g, '<del>$1</del>');
        html = html.replace(/\*\*([^*\n]+)\*\*/g, '<strong>$1</strong>');
        html = html.replace(/__([^_\n]+)__/g, '<strong>$1</strong>');
        html = html.replace(/\*([^*\n]+)\*/g, '<em>$1</em>');
        html = html.replace(/_([^_\n]+)_/g, '<em>$1</em>');

        $.each(blocos, function (_, bloco) {
            html = html.split(bloco.token).join(bloco.html);
        });

        return html;
    }

    function criarLinkSeguro(hrefTexto, labelHtml) {
        var href = normalizarHref(hrefTexto);
        if (!hrefPermitido(href)) {
            return labelHtml;
        }

        return '<a href="' + escaparAtributo(href) + '" target="_blank" rel="noopener noreferrer">' + labelHtml + '</a>';
    }

    function normalizarHref(href) {
        href = desescaparEntidades(String(href || '')).trim().replace(/^["'`]+|["'`]+$/g, '');

        if (/^~\/App\//i.test(href)) href = href.substring(1);
        if (/^~\/API\//i.test(href)) href = href.substring(1);
        if (/^App\//i.test(href)) href = '/' + href;
        if (/^API\//i.test(href)) href = '/' + href;

        return href;
    }

    // Allowlist: so http(s) e caminhos internos. E o que impede javascript: e data: virarem link.
    function hrefPermitido(href) {
        href = String(href || '').trim();
        return /^https?:\/\//i.test(href) || /^\/App\//i.test(href) || /^\/API\//i.test(href);
    }

    function separarPontuacaoUrl(href) {
        var sufixo = '';
        href = String(href || '');

        while (/[.,;:!?)]$/.test(href)) {
            sufixo = href.charAt(href.length - 1) + sufixo;
            href = href.substring(0, href.length - 1);
        }

        return { href: href, sufixo: sufixo };
    }

    function escaparHtml(texto) {
        return String(texto || '')
            .replace(/&/g, '&amp;')
            .replace(/</g, '&lt;')
            .replace(/>/g, '&gt;')
            .replace(/"/g, '&quot;')
            .replace(/'/g, '&#39;');
    }

    function escaparAtributo(texto) {
        return escaparHtml(texto).replace(/`/g, '&#96;');
    }

    function desescaparEntidades(texto) {
        return String(texto || '')
            .replace(/&amp;/g, '&')
            .replace(/&quot;/g, '"')
            .replace(/&#39;/g, "'")
            .replace(/&lt;/g, '<')
            .replace(/&gt;/g, '>');
    }

    return {
        renderizar: renderizar,
        renderizarInline: renderizarInlineSeguro,
        normalizarHref: normalizarHref,
        hrefPermitido: hrefPermitido,
        escaparHtml: escaparHtml,
        escaparAtributo: escaparAtributo
    };
})();
