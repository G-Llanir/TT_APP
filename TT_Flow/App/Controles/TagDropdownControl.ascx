<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TagDropdownControl.ascx.cs" Inherits="TT_Flow.App.Controles.TagDropdownControl" %>

<asp:HiddenField ID="hfSelectedTags" runat="server" />

<div class="tag-container" id="<%= this.ClientID %>_tagContainer">
    <div class="tag-box" id="<%= this.ClientID %>_tagBox">
        <input id="<%= this.ClientID %>_tagInput" class="tag-input" placeholder="Selecione..." />
        <div id="<%= this.ClientID %>_suggestions" class="suggestions" role="listbox"></div>
    </div>
    <div id="<%= this.ClientID %>_valorStatus" class="valor-status"></div>
</div>

<%-- SCRIPT MODIFICADO PARA SER AUTOSSUFICIENTE --%>
<script type="text/javascript">
    class TagDropdownManager {
        constructor(clientId, tagsDisponiveis, tagsSelecionadasIniciais, modo, placeholderTexto, placeholderValor, valorLimite) {
            this.clientId = clientId;
            this.tagsDisponiveis = tagsDisponiveis;
            this.modo = modo || 'PaiUnico';
            this.isReadOnly = this.modo === 'Leitura';
            this.valorLimite = parseFloat(valorLimite) || 0;

            this.placeholderTexto = placeholderTexto || 'Selecione...';
            this.placeholderValor = typeof placeholderValor !== 'undefined' ? placeholderValor : '';

            this.tagsSelecionadas = tagsSelecionadasIniciais.map(t => ({ ...t, valor: t.valor || 0, instanceId: Date.now() + Math.random() }));

            this.elements = {
                hiddenField: document.getElementById(clientId + '_hfSelectedTags'),
                tagBox: document.getElementById(clientId + '_tagBox'),
                tagInput: document.getElementById(clientId + '_tagInput'),
                suggestions: document.getElementById(clientId + '_suggestions'),
                valorStatus: document.getElementById(clientId + '_valorStatus')
            };

            // A chamada this.init() foi movida para a função de inicialização global para melhor controle.
        }

        init() {
            if (!this.isReadOnly) {
                this.addEventListeners();
            } else {
                if (this.elements.tagInput) this.elements.tagInput.style.display = 'none';
                if (this.elements.tagBox) this.elements.tagBox.classList.add('read-only');
            }
            this.renderTags();
            this.atualizarEstadoUI();
        }

        addEventListeners() {
            this.elements.tagInput.addEventListener('input', () => this.showSuggestions());
            this.elements.tagInput.addEventListener('focus', () => this.showSuggestions());
            this.elements.tagBox.addEventListener('click', (e) => {
                if (e.target === this.elements.tagBox) this.elements.tagInput.focus();
            });
            document.addEventListener('click', (e) => {
                if (this.elements.tagBox && !this.elements.tagBox.contains(e.target)) {
                    if (this.elements.suggestions) this.elements.suggestions.style.display = 'none';
                }
            });
        }

        calcularSomaAtual() {
            return this.tagsSelecionadas.reduce((soma, tag) => soma + (parseFloat(tag.valor) || 0), 0);
        }

        atualizarEstadoUI() {
            if (this.modo !== 'PaiComValor' || !this.elements.valorStatus) {
                if (this.elements.valorStatus) this.elements.valorStatus.style.display = 'none';
                return;
            }
            this.elements.valorStatus.style.display = 'block';

            const somaAtual = this.calcularSomaAtual();
            const limiteAlcancado = somaAtual >= this.valorLimite;

            const somaFormatada = somaAtual.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });
            const limiteFormatado = this.valorLimite.toLocaleString('pt-BR', { minimumFractionDigits: 2, maximumFractionDigits: 2 });

            this.elements.valorStatus.textContent = `Total: R$ ${somaFormatada} de R$ ${limiteFormatado}`;

            if (limiteAlcancado) {
                this.elements.tagInput.disabled = true;
                this.elements.tagInput.placeholder = 'Limite atingido';
                this.elements.valorStatus.classList.add('limite-atingido');
            } else {
                this.elements.tagInput.disabled = false;
                this.elements.tagInput.placeholder = this.placeholderTexto;
                this.elements.valorStatus.classList.remove('limite-atingido');
            }
        }

        updateHiddenField() {
            if (this.isReadOnly) return;
            const dataToServer = this.tagsSelecionadas.map(t => {
                const { instanceId, ...rest } = t;
                return rest;
            });
            this.elements.hiddenField.value = JSON.stringify(dataToServer);
        }

        showSuggestions() {
            if (this.modo === 'PaiComValor' && this.calcularSomaAtual() >= this.valorLimite) {
                this.elements.suggestions.style.display = 'none';
                return;
            }

            const query = this.elements.tagInput.value.trim().toLowerCase();
            this.elements.suggestions.innerHTML = '';
            let filtradas = this.tagsDisponiveis;

            if (this.modo === 'PaiUnico') {
                filtradas = filtradas.filter(t => !this.tagsSelecionadas.some(s => s.idPai === t.id));
            }

            if (this.modo === 'PaiRepetido' || this.modo === 'PaiComValor') {
                filtradas = filtradas.filter(tagData => {
                    if (!tagData.filhos || tagData.filhos.length === 0) return true;
                    const filhosUsados = this.tagsSelecionadas.filter(s => s.idPai === tagData.id).map(s => s.idFilho);
                    return tagData.filhos.length > filhosUsados.length;
                });
            }

            filtradas = filtradas.filter(t => (query === '' || t.nome.toLowerCase().includes(query)));

            if (filtradas.length === 0) {
                this.elements.suggestions.style.display = 'none';
                return;
            }

            filtradas.forEach(tagData => {
                const div = document.createElement('div');
                div.textContent = tagData.nome;
                div.tabIndex = 0;
                div.addEventListener('click', () => this.addTag(tagData));
                div.addEventListener('keydown', (e) => { if (e.key === 'Enter') this.addTag(tagData); });
                this.elements.suggestions.appendChild(div);
            });
            this.elements.suggestions.style.display = 'block';
        }

        addTag(tagData) {
            this.tagsSelecionadas.push({
                instanceId: Date.now() + Math.random(),
                idPai: tagData.id,
                nomePai: tagData.nome,
                idFilho: '',
                nomeFilho: '',
                valor: 0
            });
            this.renderTags();
            this.updateHiddenField();
            this.atualizarEstadoUI();
            this.elements.tagInput.value = '';
            this.elements.suggestions.style.display = 'none';
            this.elements.tagInput.focus();
        }

        removeTag(tagToRemove) {
            this.tagsSelecionadas = this.tagsSelecionadas.filter(t => t.instanceId !== tagToRemove.instanceId);
            this.renderTags();
            this.updateHiddenField();
            this.atualizarEstadoUI();
            this.elements.tagInput.focus();
        }

        renderTags() {
            this.elements.tagBox.querySelectorAll('.tag').forEach(el => el.remove());
            this.tagsSelecionadas.forEach(tag => {
                const tagElement = this.createTagElement(tag);
                this.elements.tagBox.insertBefore(tagElement, this.elements.tagInput);
            });
        }

        createTagElement(tag) {
            const el = document.createElement('div');
            el.className = 'tag';

            const nameSpan = document.createElement('span');
            nameSpan.className = 'name';
            nameSpan.textContent = tag.nomePai;
            el.appendChild(nameSpan);

            if (this.isReadOnly) {
                const childInfoSpan = document.createElement('span');
                childInfoSpan.className = 'child-name-readonly';
                let readonlyText = tag.nomeFilho || 'N/D';
                if (this.modo === 'PaiComValor') {
                    readonlyText += ` - R$ ${(tag.valor || 0).toLocaleString('pt-BR', { minimumFractionDigits: 2 })}`;
                }
                childInfoSpan.textContent = readonlyText;
                el.appendChild(childInfoSpan);
            }
            else if (this.modo === 'PaiComValor') {
                const select = this.createChildSelect(tag);
                el.appendChild(select);

                const valorInput = document.createElement('input');
                valorInput.type = 'number';
                valorInput.className = 'valor-input';
                valorInput.step = '0.01';
                valorInput.min = '0';
                valorInput.placeholder = '0,00';
                valorInput.value = tag.valor > 0 ? tag.valor.toString().replace('.', ',') : '';

                valorInput.addEventListener('input', (e) => {
                    let novoValor = parseFloat(e.target.value.replace(',', '.')) || 0;
                    const valorAntigo = parseFloat(tag.valor) || 0;
                    const somaSemEsteItem = this.calcularSomaAtual() - valorAntigo;

                    if ((somaSemEsteItem + novoValor) > this.valorLimite) {
                        novoValor = this.valorLimite - somaSemEsteItem;
                        e.target.value = novoValor.toFixed(2).replace('.', ',');
                    }
                    tag.valor = novoValor < 0 ? 0 : novoValor;
                    this.updateHiddenField();
                    this.atualizarEstadoUI();
                });
                el.appendChild(valorInput);
            }
            else {
                const select = this.createChildSelect(tag);
                el.appendChild(select);
            }

            if (!this.isReadOnly) {
                const removeBtn = this.createRemoveButton(tag);
                el.appendChild(removeBtn);
            }
            return el;
        }

        createRemoveButton(tag) {
            const removeBtn = document.createElement('button');
            removeBtn.type = 'button';
            removeBtn.className = 'remove';
            removeBtn.textContent = '×';
            removeBtn.title = 'Remover tag';
            removeBtn.addEventListener('click', () => this.removeTag(tag));
            return removeBtn;
        }

        createChildSelect(tag) {
            const select = document.createElement('select');
            const tagOriginal = this.tagsDisponiveis.find(t => t.id === tag.idPai);

            const placeholder = document.createElement('option');
            placeholder.value = this.placeholderValor;
            placeholder.textContent = this.placeholderTexto;
            select.appendChild(placeholder);

            if (tagOriginal && tagOriginal.filhos) {
                let availableChildren = tagOriginal.filhos;

                if (this.modo === 'PaiRepetido' || this.modo === 'PaiComValor') {
                    const usedChildIds = this.tagsSelecionadas
                        .filter(s => s.idPai === tag.idPai && s.instanceId !== tag.instanceId)
                        .map(s => s.idFilho);
                    availableChildren = tagOriginal.filhos.filter(f => !usedChildIds.includes(f.id));
                }

                availableChildren.forEach(f => {
                    const opt = document.createElement('option');
                    opt.value = f.id;
                    opt.textContent = f.nome;
                    select.appendChild(opt);
                });
            }

            select.value = tag.idFilho || this.placeholderValor;

            select.addEventListener('change', (e) => {
                const selectedOption = e.target.options[e.target.selectedIndex];
                tag.idFilho = e.target.value;
                tag.nomeFilho = e.target.value !== this.placeholderValor ? selectedOption.text : '';
                this.renderTags();
                this.updateHiddenField();
            });
            return select;
        }
    }

    // Gerenciador global de instâncias para evitar duplicação em postbacks
    window.tagDropdownInstances = window.tagDropdownInstances || {};

    // Função global que será chamada pelo ScriptManager para criar ou recarregar o controle
    function TDC_init(clientId, tagsDisponiveis, tagsSelecionadasIniciais, modo, placeholderTexto, placeholderValor, valorLimite) {
        // Limpa a instância anterior se ela existir, para reconstruir com o estado atualizado do servidor
        if (window.tagDropdownInstances[clientId]) {
            delete window.tagDropdownInstances[clientId];
        }

        const instance = new TagDropdownManager(clientId, tagsDisponiveis, tagsSelecionadasIniciais, modo, placeholderTexto, placeholderValor, valorLimite);
        instance.init(); // Inicializa a lógica da classe
        window.tagDropdownInstances[clientId] = instance; // Armazena a nova instância
    }

    // Função global para a página pai (ContasPagar) poder atualizar o valor limite
    function TDC_setValorTotalPai(clientId, novoValor) {
        const instance = window.tagDropdownInstances[clientId];
        if (instance) {
            instance.valorLimite = parseFloat(novoValor) || 0;
            instance.atualizarEstadoUI();
        }
    }
</script>

<style>
    .tag-container { position: relative; }
    .tag-box { position: relative; display:flex; flex-wrap:wrap; align-items:center; border:1px solid #ccc; padding:8px; border-radius:8px; background:#fff; cursor:text; min-height:40px; }
    .tag-box:focus-within { border-color: #007bff; box-shadow: 0 0 0 2px rgba(0, 123, 255, 0.25); }
    .tag { display:flex; align-items:center; background:#007bff; color:#fff; border-radius:20px; padding:6px 12px; margin:4px; gap:8px; font-size: 14px; }
    .tag span.name { font-weight:600; }
    .tag select, .tag .valor-input { border:none; background:#fff; color:#000; border-radius:6px; padding:4px 8px; cursor:pointer; font-size:13px; max-width: 120px; }
    .tag select { -webkit-appearance: none; appearance: none; padding-right: 20px; background-image: url("data:image/svg+xml,%3csvg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 16 16'%3e%3cpath fill='none' stroke='%23333' stroke-linecap='round' stroke-linejoin='round' stroke-width='2' d='M2 5l6 6 6-6'/%3e%3c/svg%3e"); background-repeat: no-repeat; background-position: right 4px center; background-size: 12px; }
    .tag .remove { background:rgba(255,255,255,0.2); border:none; color:white; font-size:16px; font-weight: bold; cursor:pointer; border-radius:50%; width:22px; height:22px; display:flex; align-items:center; justify-content:center; line-height: 1; }
    .tag .remove:hover { background: rgba(0,0,0,0.2); }
    .tag-input { border:none; outline:none; flex:1; min-width:140px; font-size:14px; padding:5px; background: transparent;}
    .suggestions { position:absolute; top: 100%; left:0; right:0; background:#fff; border:1px solid #ccc; border-radius:6px; max-height:180px; overflow:auto; display:none; z-index:50; margin-top: 4px; box-shadow: 0 4px 8px rgba(0,0,0,0.1); }
    .suggestions div { padding:10px; cursor:pointer; }
    .suggestions div:hover, .suggestions div:focus { background:#007bff; color:#fff; outline: none; }
    .tag-box.read-only { background-color: #f0f0f0; cursor: default; border-color: #ddd; }
    .tag-box.read-only:focus-within { box-shadow: none; }
    .tag.read-only { background-color: #6c757d; }
    .child-name-readonly { background: #fff; color: #333; padding: 4px 8px; border-radius: 6px; font-size: 13px; }
    .valor-status { font-size: 12px; color: #555; padding: 4px 8px; margin-top: 4px; text-align: right; display: none; }
    .valor-status.limite-atingido { color: #dc3545; font-weight: bold; }
</style>