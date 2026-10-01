// ============================================================================================================================= //
// Master

function initializeChosen() { $(".Caixa_Selecao").chosen({ width: "100%" }); }

// ----- Tooltips ----- //

$('[data-toggle="tooltip"], [data-toggle="tooltip_top"]').tooltip({ placement: 'top', html: true, boundary: 'window' });
$('[data-toggle="tooltip_right"]').tooltip({ placement: 'right', html: true, boundary: 'window' });
$('[data-toggle="tooltip_bottom"]').tooltip({ placement: 'bottom', html: true, boundary: 'window' });
$('[data-toggle="tooltip_left"]').tooltip({ placement: 'left', html: true, boundary: 'window' });

// ----- Funções MudarBotao ----- //

function MudarBotao_Pesquisa(botao) { $(botao).val('Pesquisando...'); }
function MudarBotao_Pesquisar(botao) { $(botao).val('Pesquisando...'); }
function MudarBotao_Salvar(botao) { $(botao).val('Salvando...'); }

// ----- Menu ----- //

function toggleMenu() {
    var menu = document.querySelector('.navbar-static-side');
    var sidebarCollapse = document.querySelector('.sidebar-collapse');

    menu.classList.toggle('open');

    sidebarCollapse.classList.remove('collapse');
    $('.menu-icon').toggleClass('X');
}

document.querySelector('.navbar-static-side').onclick = function (event) { event.stopPropagation(); };

// ----- Cards de Produtos ----- //

var cardTimer = {};
function mostraCard(element, idProduto, tabela) {
    cardTimer[idProduto + '_' + tabela] = setTimeout(function () {
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
                                     ${produto.imagem ? `<img src = "${produto.imagem}" alt="Imagem do Produto" class="img-fluid img-thumbnail" style="width: 100px; height: auto;" />` : ''}
                                 </div>
                                 <div class="flex-grow-1 d-flex flex-column ms-3">
                                     <div class="d-flex">
                                         ${produto.sCategoriaVendas ? `<div class="card-text me-3"> <strong>Categoria Vendas: </strong>${produto.sCategoriaVendas}</div> ` : ''}
                                         ${produto.sFabricante ? `<div class= "card-text me-3"> <strong > Fabricante: </strong >${produto.sFabricante}</div>` : ''}
                                         ${produto.sTipo ? `<div class= "card-text me-3"> <strong > Tipo: </strong >${produto.sTipo}</div>` : ''}
                                         ${produto.sGrupo ? `<div class= "card-text me-3"> <strong > Grupo: </strong >${produto.sGrupo}</div>` : ''}
                                         ${produto.sFamilia ? `<div class= "card-text me-3"> <strong > Família: </strong >${produto.sFamilia}</div>` : ''}
                                         ${produto.sPaisOrigem ? `<div class= "card-text me-3"> <strong > Origem: </strong >${produto.sPaisOrigem}</div>` : ''}
                                         ${produto.sLocalArmazenamento ? `<div class= "card-text me-3"> <strong > Local Armazenamento: </strong >${produto.sLocalArmazenamento}</div>` : ''}
                                     </div>
                                 </div>
                             </div>
                         </div>
                     `;
                var cardId = idProduto + '_' + tabela;
                var card = document.getElementById(cardId);
                card.innerHTML = cardProduto;
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
                     <button type = "button" class= "close" data - dismiss = "modal" aria - label = "Close">
                         <span aria - hidden = "true" > &times;</span>
                     </button>
                     <h5 class= "modal-title" id = "detailsModalLabel" > ${produto.sCodigo} - ${produto.sDsc}</h5>
                 `;
            var modalInfo = document.getElementById('modalInfo');
            modalInfo.innerHTML = tituloProduto;
            var imagem = '';
            if (produto.imagem) {
                imagem += `
                         <div style = "text-align: center; margin-bottom: 20px;">
                             <img src = "${produto.imagem}" alt = "Imagem do Produto" class= "img-fluid" style = "width: 300px; height: auto;" />
                         </div>
                     `;
            }
            var tabelaProduto = '<table class="table table-bordered">';
            if (produto.sCategoriaVendas) {
                tabelaProduto += `
                         <tr>
                             <th> Categoria Vendas </th>
                             <td>${produto.sCategoriaVendas}</td>
                         </tr>
                     `;
            }
            if (produto.sTipo) {
                tabelaProduto += `
                         <tr>
                             <th> Tipo </th>
                             <td>${produto.sTipo}</td>
                         </tr>
                     `;
            }
            if (produto.sGrupo) {
                tabelaProduto += `
                         <tr>
                             <th> Grupo </th>
                             <td>${produto.sGrupo}</td>
                         </tr>
                     `;
            }
            if (produto.sFabricante) {
                tabelaProduto += `
                         <tr>
                             <th> Fabricante </th>
                             <td>${produto.sFabricante}</td>
                         </tr>
                     `;
            }
            if (produto.sLocalArmazenamento) {
                tabelaProduto += `
                         <tr>
                             <th> Local Armazenamento </th>
                             <td>${produto.sLocalArmazenamento}</td>
                         </tr>
                     `;
            }
            if (produto.sFamilia) {
                tabelaProduto += `
                         <tr>
                             <th> Família </th>
                             <td>${produto.sFamilia}</td>
                         </tr>
                     `;
            }
            if (produto.sCodigoCEST) {
                tabelaProduto += `
                         <tr>
                             <th> CEST </th>
                             <td>${produto.sCodigoCEST}</td>
                         </tr>
                     `;
            }
            if (produto.sCodigoNCM) {
                tabelaProduto += `
                         <tr>
                             <th> NCM </th>
                             <td>${produto.sCodigoNCM}</td>
                         </tr>
                     `;
            }
            if (produto.sPaisOrigem) {
                tabelaProduto += `
                         <tr>
                             <th> Origem </th>
                             <td>${produto.sPaisOrigem}</td>
                         </tr>
                     `;
            }
            tabelaProduto += `</table >`;
            var modalBody = document.getElementById('modalBody');
            modalBody.innerHTML = imagem + tabelaProduto;
            $('#produtoDetalheModal').modal('show');
        },
        error: function (error) {
            console.error("Erro ao obter os detalhes do produto:", error);
        }
    });
}

function openProductDetail(idItem) {
    var url = '/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id=' + idItem;
    window.open(url, '_blank');
    return false;
}

// ----- Função para exibir/esconder colunas ----- //

function toggleColumnVisibility(checkbox) {
    var headerText = checkbox.getAttribute('data-column-header');
    var gridViewId = checkbox.getAttribute('data-gridview-id');
    var gridView = document.getElementById(gridViewId);

    if (gridView) {
        var wrapper = gridView.closest('.dataTables_wrapper');
        var temScroll = wrapper && wrapper.querySelector('.dataTables_scrollHead');

        if (temScroll) {
            toggleColumnDataTableComScroll(wrapper, headerText, checkbox.checked);
        } else {
            toggleColumnTable(gridView, headerText, checkbox.checked);
        }
    }
}

function toggleColumnDataTableComScroll(wrapper, headerText, isChecked) {
    var scrollHead = wrapper.querySelector('.dataTables_scrollHead');
    var scrollBody = wrapper.querySelector('.dataTables_scrollBody');

    var columnIndex = -1;
    if (scrollHead) {
        var headers = scrollHead.querySelectorAll('th');
        headers.forEach((th, index) => {
            if (th.textContent.replace(/\s/g, '-') === headerText) {
                columnIndex = index;
            }
        });
    }

    if (columnIndex !== -1) {
        if (scrollHead) {
            var headerCells = scrollHead.querySelectorAll('th');
            if (headerCells[columnIndex]) {
                headerCells[columnIndex].style.display = isChecked ? '' : 'none';
            }
        }

        if (scrollBody) {
            var rows = scrollBody.querySelectorAll('tr');
            rows.forEach(row => {
                var cell = row.cells[columnIndex];
                if (cell) {
                    cell.style.display = isChecked ? '' : 'none';
                }
            });
        }
    }
}

function toggleColumnTable(gridView, headerText, isChecked) {
    var headers = gridView.querySelectorAll('th');
    var columnIndex = -1;

    headers.forEach((th, index) => {
        if (th.textContent.replace(/\s/g, '-') === headerText) {
            columnIndex = index;
        }
    });

    if (columnIndex !== -1) {
        if (headers[columnIndex]) {
            headers[columnIndex].style.display = isChecked ? '' : 'none';
        }

        var rows = gridView.querySelectorAll('tr');
        rows.forEach(row => {
            var cell = row.cells[columnIndex];
            if (cell) {
                cell.style.display = isChecked ? '' : 'none';
            }
        });
    }
}

// ----- Controle TextBox_Padrao ----- //

function ValidarPadrao_TextBox(txt) {
    const tipo = txt.attr('type');
    const valor = txt.val();
    if (tipo == 'email') {
        const regexEmail = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return regexEmail.test(valor);
    } else if (tipo == 'url') {
        const regexURL = /^(https?:\/\/)?([\w\-]+\.)+[a-z]{2,}(\/.*)?$/i;
        return regexURL.test(valor);
    } else if (tipo == 'tel') {
        const regexTel = /^\(?\d{2}\)?\s?\d{4,5}-?\d{4}$/;
        return regexTel.test(valor);
    } else if (txt.attr('data-validarPadrao_Erro') && txt.attr('data-validarPadrao_Erro').length > 0) {
        return (valor && valor.trim().length > 0) ? true : false;
    }
    return true;
}

// ============================================================================================================================= //

$(document).ready(function () {

    // ----- Menu ----- //
    var originalDoPostBack = window.__doPostBack;
    var originalWebForm_DoPostBackWithOptions = window.WebForm_DoPostBackWithOptions;
    window.__doPostBack = function (eventTarget, eventArgument) {
        if (typeof tinymce !== 'undefined') tinymce.triggerSave();
        originalDoPostBack(eventTarget, eventArgument);
    };
    window.WebForm_DoPostBackWithOptions = function (options) {
        if (typeof tinymce !== 'undefined') tinymce.triggerSave();
        return originalWebForm_DoPostBackWithOptions(options);
    };
    $('form').on('submit', function () { if (typeof tinymce !== 'undefined') tinymce.triggerSave(); });

    // ----- Preserva o scroll da página e conteineres internos marcados ----- //
    if (typeof Sys !== "undefined" && Sys.WebForms && Sys.WebForms.PageRequestManager) {
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        var pageScroll = { top: 0, left: 0 };
        var scrollState = {};
        var loadingTimer;

        // Antes de UpdatePanel ser atualizado
        prm.add_beginRequest(function () {
            if (typeof tinymce !== 'undefined') tinymce.triggerSave();
            loadingTimer = setTimeout(function () {
                $('#div_Loading').show();
                $('#div_Loading > .modal').show();
            }, 450);

            // Scroll geral da página
            pageScroll.top = $(window).scrollTop();
            pageScroll.left = $(window).scrollLeft();

            // Scroll dos containers marcados
            scrollState = {};

            $('.preserva-scroll').each(function () {
                const id = this.id;

                if (!id)
                    return;

                scrollState[id] = {
                    top: this.scrollTop,
                    left: this.scrollLeft
                };
            });
        });

        // Depois de UpdatePanel ser atualizado
        prm.add_endRequest(function () {
            initializeChosen();
            clearTimeout(loadingTimer);
            $('#div_Loading').hide();
            $('#div_Loading > .modal').hide();

            // Restaura scroll da página
            $(window).scrollTop(pageScroll.top);
            $(window).scrollLeft(pageScroll.left);

            // Restaura scroll dos containers
            $.each(scrollState, function (id, position) {
                const element = document.getElementById(id);

                if (!element)
                    return;

                element.scrollTop = position.top;
                element.scrollLeft = position.left;
            });
        });
    }

    $(window).on('beforeunload', function () {
        loadingTimer = setTimeout(function () {
            $('#div_Loading').show();
            $('#div_Loading > .modal').show();
        }, 450);
    });

    initializeChosen();

    // ----- Função para Formatar textos estáticos ----- //
    $('[tt-format]').each(function () {
        const $this = $(this);
        $this.text(FormatarTexto($this.text(), $this.attr('tt-format')));
    });

    // ----- Função para exibir/esconder panels ----- //
    $('[data-collapse]').click(function () {
        const $this = $(this);
        $($this.data('target')).slideToggle($this.data('collapse'));
        $this.find('i.iconCollapse').toggleClass($this.find('i.iconCollapse').data('toggleIcon') ?? 'fa-chevron-down fa-chevron-up');
    });

    // ----- CheckList ----- //
    $('table.form-check-list').each(function () {
        const x = $(this).hasClass('form-check-x');
        const primary = $(this).hasClass('form-check-primary');
        const warning = $(this).hasClass('form-check-warning');
        const danger = $(this).hasClass('form-check-danger');
        $(this).find('td').each(function () {
            $(this).addClass('form-check');
            if (x) $(this).addClass('form-check-x');
            if (primary) $(this).addClass('form-check-primary');
            if (warning) $(this).addClass('form-check-warning');
            if (danger) $(this).addClass('form-check-danger');
        });
    });

    $('.form-check-todos input').change(function () {
        const isChecked = $(this).prop('checked');
        const grupo = $(this).closest('.form-check-todos').data('check-todos');
        if (grupo && grupo.length)
            $(`.form-check-unico[data-check-todos="${grupo}"] input`).prop('checked', isChecked);
        else
            $('.form-check-unico input').prop('checked', isChecked);
    });

    // ----- Controle TextBox_Padrao ----- //
    $('input[data-inteiroPadrao]').each(function () {
        $(this).mask($(this).attr('data-inteiroPadrao'), { reverse: true });
    });

    $('input[data-decimalPadrao]').each(function () {
        $(this).mask($(this).attr('data-decimalPadrao'), { reverse: true });
    });

    $('.validarPadrao_TextBox').on('blur', function () {
        const valida = ValidarPadrao_TextBox($(this));
        if (!valida) {
            $(this).closest('div').addClass('validarPadrao_Erro');
            $(this).closest('div').attr('data-validarPadrao_Erro', $(this).attr('data-validarPadrao_Erro'));
        } else {
            $(this).closest('div').removeClass('validarPadrao_Erro');
            $(this).closest('div').removeAttr('data-validarPadrao_Erro');
        }
    });

    // ----- Controle DropDownList_Padrao ----- //
    $('.link_DropDownList_Padrao').each(function () {
        const div = $(this).closest('div');
        const link = $(this).siblings('a');
        if (link && link.attr('data-link')) {
            link.attr('href', link.attr('data-link') + $(this).val() + link.attr('data-sufixo'));
            div.addClass('input-group');
            link.removeClass('invisivel');
            if (!$(this).val() || $(this).val() <= 0) {
                div.removeClass('input-group');
                link.addClass('invisivel');
            }
        }
    });

    $('.link_DropDownList_Padrao').change(function () {
        const div = $(this).closest('div');
        const link = $(this).siblings('a');
        link.attr('href', link.attr('data-link') + $(this).val() + link.attr('data-sufixo'));
        div.addClass('input-group');
        link.removeClass('invisivel');
        if (!$(this).val() || $(this).val() <= 0) {
            div.removeClass('input-group');
            link.addClass('invisivel');
        }
    });

    $('.validarPadrao_DropDownList').on('change blur', function () {
        const $this = $(this);
        const div = $this.closest('div');
        if (!$this.find('option:selected').length || $this.val() == '' || $this.val() == 0) {
            div.addClass('validarPadrao_Erro');
            div.attr('data-validarPadrao_Erro', $this.attr('data-validarPadrao_Erro'));
        } else {
            div.removeClass('validarPadrao_Erro');
            div.removeAttr('data-validarPadrao_Erro');
        }
    });

    $('select[readonly]').each(function () {
        $(this).siblings('.chosen-container').addClass('chosen-disabled');
    });

    // ----- TinyMCE ----- //
    $(document).off('click', 'li[role="presentation"] > a[role="tab"]').on('click', 'li[role="presentation"] > a[role="tab"]', function (e) {
        setTimeout(function () {
            $(window).trigger('resize');
        }, 250);
    });

    $(document).off('click', '.modalConfig').on('click', '.modalConfig', function (e) {
        $('#modalConfig').modal('show');
    });

    $(document).off('click', '.togglePanel').on('click', '.togglePanel', function (e) {
        $(this).find('i.fa').last().toggleClass('fa-chevron-up fa-chevron-down');
        $(this).siblings('.panel-body').toggle('fast');
    });

    window.Aplicar_TinyMCE = Aplicar_TinyMCE;
    Aplicar_TinyMCE();

    // ----- NOVA Função para exibir/esconder colunas ----- //

    $('.datatable-col-toggle').on('click', function () {
        $(this).toggleClass('label-danger label-success');
        const tabela = $('#' + $(this).data('tabela')).DataTable();
        tabela.column(parseInt($(this).data('col'))).visible($(this).hasClass('label-success'));
    });
});

// ============================================================================================================================= //
// Funções auxiliares

function Aplicar_TinyMCE() {
    $('textarea.htmlEditor').each(function () {
        const readonly = $(this).hasClass('naoEdita');
        const altura = $(this).hasClass('pequeno') ? '20em' : $(this).hasClass('medio') ? '30em' : $(this).hasClass('grande') ? '40em' : $(this).hasClass('gigante') ? '60em' : '400px';
        if (tinymce.get($(this).attr('id'))) tinymce.get($(this).attr('id')).remove();
        tinymce.init({
            target: this,
            license_key: 'gpl',
            readonly: readonly,
            elementpath: false,
            branding: false,
            browser_spellcheck: true,
            plugins: ['advlist', 'anchor', 'autolink', 'charmap', 'directionality', 'emoticons', 'fullscreen', 'help', 'image', 'insertdatetime', 'link', 'lists', 'media', 'nonbreaking', 'pagebreak', 'preview', 'save', 'searchreplace', 'table', 'visualblocks', 'visualchars', 'wordcount'],
            toolbar: 'undo redo | styles | removeformat | bold italic underline strikethrough | alignleft aligncenter alignright alignjustify | table | bullist numlist outdent indent | forecolor backcolor | midia | extras | help',
            toolbar_groups: {
                midia: {
                    icon: 'link',
                    tooltip: 'Mídias e Símbolos',
                    items: 'link image media emoticons charmap'
                },
                extras: {
                    icon: 'plus',
                    tooltip: 'Ferramentas adicionais',
                    items: 'ltr rtl | fullscreen preview print'
                }
            },
            statusbar: true,
            menubar: 'file edit view insert format tools table help',
            content_style: 'body { font-family: Helvetica, Arial, sans-serif; font-size: 14px; }',
            language_url: '/App/JS/tinymce_v8_3_2/langs/pt_BR.js',
            language: 'pt_BR',
            height: altura,
            setup: function (editor) {
                editor.on('init', function () {
                    const statusbar = editor.getContainer().querySelector('.tox-statusbar');
                    if (statusbar) {
                        statusbar.querySelector('.tox-statusbar__right-container').style.textDecoration = 'underline';

                        const help = statusbar.querySelector('.tox-statusbar__help-text');
                        help.textContent = 'Ajuda (Alt+0)';
                        help.style.cursor = 'pointer';
                        help.style.textDecoration = 'underline';
                        help.onclick = () => editor.execCommand('mceHelp');
                    }
                });
            }
        });
    });
}

// ============================================================================================================================= //
// Funções adicionais

(function ($) {
    $.fn.toggleAttr = function (attr, value) {
        return this.each(function () {
            const $el = $(this);
            if ($el.is('[' + attr + ']')) {
                $el.removeAttr(attr);
            } else {
                $el.attr(attr, value || '');
            }
        });
    };
})($);

/* Formata texto utilizando uma máscara.
 *   9 / 0 = apenas dígitos
 *   a / A = apenas caracteres alpha
 *   *     = qualquer caractere
 * 
 *   demais = caracteres literais da máscara
 */
function FormatarTexto(value, format) {
    if (!value || !format)
        return value;

    value = String(value);
    format = String(format);

    let result = '';
    let valueIndex = 0;

    for (let i = 0; i < format.length; i++) {
        const maskChar = format[i];

        // Próximo caractere válido para o placeholder
        const getNext = (validator) => {
            while (valueIndex < value.length) {
                const char = value[valueIndex++];
                if (validator(char))
                    return char;
            }

            return null;
        };

        let char = null;
        switch (maskChar) {
            case '9':
            case '0':
                char = getNext(c => /\d/.test(c)); // Apenas dígitos
                break;

            case 'a':
            case 'A':
                char = getNext(c => /[a-zA-Z]/.test(c)); // Apenas caracteres alpha
                break;

            case '*':
                char = getNext(() => true); // Qualquer caractere
                break;

            default:
                result += maskChar; // Caractere literal da máscara
                continue;
        }

        // Não há mais caracteres
        if (char === null)
            break;

        result += char;
    }

    return result;
}

function FormatarValor(valor, casasDecimais) {
    if (isNaN(valor)) return valor;

    let [inteira, decimal] = valor.toFixed(casasDecimais).split(".");
    inteira = inteira.replace(/\B(?=(\d{3})+(?!\d))/g, ".");

    return decimal && casasDecimais ? `${inteira},${decimal}` : inteira;
}

// ============================================================================================================================= //