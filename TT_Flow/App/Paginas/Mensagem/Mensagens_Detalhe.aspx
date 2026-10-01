<%@ Page Title="" Language="C#" ValidateRequest="false" EnableEventValidation="false" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Mensagens_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Mensagem.Mensagens_Detalhe" %>
<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/ComboAtivo.ascx" TagPrefix="uc1" TagName="ComboAtivo" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
        .tt-switch {
            display: inline-flex;
            align-items: center;
            min-height: 34px;
        }

        .tt-switch input[type="checkbox"] {
            position: absolute;
            opacity: 0;
            width: 0;
            height: 0;
        }

        .tt-switch label {
            position: relative;
            display: inline-flex;
            align-items: center;
            margin: 0;
            padding-left: 58px;
            min-height: 30px;
            cursor: pointer;
            font-weight: normal;
        }

        .tt-switch label:before {
            content: "";
            position: absolute;
            left: 0;
            width: 48px;
            height: 26px;
            border-radius: 999px;
            background: #d5dbe3;
            transition: background .2s ease;
        }

        .tt-switch label:after {
            content: "";
            position: absolute;
            left: 3px;
            width: 20px;
            height: 20px;
            border-radius: 50%;
            background: #fff;
            box-shadow: 0 1px 4px rgba(0,0,0,.25);
            transition: transform .2s ease;
        }

        .tt-switch input[type="checkbox"]:checked + label:before {
            background: #337ab7;
        }

        .tt-switch input[type="checkbox"]:checked + label:after {
            transform: translateX(22px);
        }

        .mensagem-picker {
            border: 1px solid #ccc;
            border-radius: 6px;
            background: #fff;
        }

        .mensagem-picker-header {
            padding: 10px;
            border-bottom: 1px solid #e6e7ed;
            background: #f7f8fa;
        }

        .mensagem-picker-lista {
            max-height: 260px;
            overflow-y: auto;
            padding: 8px 10px;
        }

        .mensagem-picker-lista table {
            width: 100%;
            margin: 0;
        }

        .mensagem-picker-lista td {
            display: block;
            padding: 4px 0;
        }

        .mensagem-picker-lista label {
            font-weight: normal;
            margin-left: 6px;
            cursor: pointer;
        }

        .mensagem-picker-empty {
            padding: 12px;
            color: #777;
            font-style: italic;
        }

        .destinatarios-preview {
            margin-top: 10px;
            max-height: 220px;
            overflow-y: auto;
            border: 1px solid #e6e7ed;
            border-radius: 6px;
        }

        .destinatarios-preview table {
            margin-bottom: 0;
        }

        .mensagem-alerta-dashboard {
            margin-top: 8px;
            min-height: 52px;
            visibility: hidden;
        }

        .mensagem-comentario {
            border-bottom: 1px solid #e6e7ed;
            padding: 10px 0;
        }

        .mensagem-comentario:last-child {
            border-bottom: 0;
        }

        .mensagem-tags-valor {
            display: none;
        }

        .mensagem-tags {
            display: flex;
            flex-wrap: wrap;
            align-items: center;
            gap: 8px;
            min-height: 42px;
            padding: 6px 8px;
            border: 1px solid #ccc;
            border-radius: 4px;
            background: #fff;
        }

        .mensagem-tags.tag-readonly {
            background: #eee;
            cursor: not-allowed;
        }

        .mensagem-tag {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            padding: 6px 10px;
            border-radius: 16px;
            background: #d5d5d5;
            color: #111;
            font-size: 13px;
        }

        .mensagem-tag button {
            border: 0;
            background: transparent;
            padding: 0;
            line-height: 1;
            font-weight: bold;
            cursor: pointer;
        }

        .mensagem-tags-input {
            flex: 1;
            min-width: 160px;
            border: 0;
            outline: 0;
            padding: 6px 2px;
        }

        .mensagem-tags-ajuda {
            margin-top: 4px;
            color: #777;
            font-size: 12px;
        }
    </style>

    <script type="text/javascript">
        function Mensagens_AtualizarAvisoDashboard() {
            var chk = document.querySelector('[id$="cbAviso"]');
            var aviso = document.querySelector('[id$="divAvisoDashboard"]');
            if (chk && aviso) aviso.style.visibility = chk.checked ? 'visible' : 'hidden';
        }

        function Mensagens_AtualizarComentarios() {
            var chk = document.querySelector('[id$="cbPermiteComentarios"]');
            var bloco = document.querySelector('[id$="pnlComentarios"]');
            if (chk && bloco) bloco.style.display = chk.checked ? 'block' : 'none';
        }

        function Mensagens_FiltrarLista(input) {
            var picker = input.closest('.mensagem-picker');
            if (!picker) return;

            var termo = input.value.toLowerCase();
            var linhas = picker.querySelectorAll('.mensagem-picker-lista td');
            for (var i = 0; i < linhas.length; i++) {
                var texto = linhas[i].innerText.toLowerCase();
                linhas[i].style.display = texto.indexOf(termo) >= 0 ? '' : 'none';
            }
        }

        function Mensagens_AtualizarPreviewDestinatarios() {
            var preview = document.getElementById('destinatariosPreviewBody');
            if (!preview) return;

            preview.innerHTML = '';
            var selecionados = document.querySelectorAll('[id*="cblUsuarios"] input[type="checkbox"]:checked');
            for (var i = 0; i < selecionados.length; i++) {
                var chk = selecionados[i];
                var label = document.querySelector('label[for="' + chk.id + '"]');
                var texto = label ? label.innerText : chk.value;
                var partes = texto.split(' - ');
                var nome = partes[0] || texto;
                var email = partes.length > 1 ? partes.slice(1).join(' - ') : '';

                var tr = document.createElement('tr');
                tr.innerHTML = '<td>' + nome + '</td><td>' + email + '</td><td class="text-muted">Selecionado</td>';
                preview.appendChild(tr);
            }

            var vazio = document.getElementById('destinatariosPreviewVazio');
            if (vazio) vazio.style.display = selecionados.length === 0 ? 'block' : 'none';
        }

        function Mensagens_NormalizarTag(tag) {
            return (tag || '').replace(/[;,]/g, ' ').replace(/\s+/g, ' ').trim();
        }

        function Mensagens_LimiteTags() {
            return 3;
        }

        function Mensagens_RecuperarTags() {
            var campo = document.querySelector('[id$="txtsTags"]');
            if (!campo || !campo.value) return [];

            var tags = campo.value.split(/[,\s;]+/);
            var resultado = [];
            for (var i = 0; i < tags.length; i++) {
                var tag = Mensagens_NormalizarTag(tags[i]);
                if (tag && resultado.indexOf(tag) < 0) resultado.push(tag);
                if (resultado.length >= Mensagens_LimiteTags()) break;
            }
            return resultado;
        }

        function Mensagens_SalvarTags(tags) {
            var campo = document.querySelector('[id$="txtsTags"]');
            if (campo) campo.value = tags.join(' ');
        }

        function Mensagens_AdicionarTag(tag) {
            tag = Mensagens_NormalizarTag(tag);
            if (!tag) return;

            var tags = Mensagens_RecuperarTags();
            if (tags.length >= Mensagens_LimiteTags() && tags.indexOf(tag) < 0) {
                Mensagens_RenderizarTags();
                return;
            }

            if (tags.indexOf(tag) < 0) {
                tags.push(tag);
                Mensagens_SalvarTags(tags);
            }
            Mensagens_RenderizarTags();
        }

        function Mensagens_RemoverTag(tag) {
            var tags = Mensagens_RecuperarTags().filter(function (item) {
                return item !== tag;
            });
            Mensagens_SalvarTags(tags);
            Mensagens_RenderizarTags();
        }

        function Mensagens_PrepararTags() {
            var input = document.getElementById('mensagemTagsInput');
            if (input && input.value) {
                Mensagens_AdicionarTag(input.value);
                input.value = '';
            }
        }

        function Mensagens_RenderizarTags() {
            var container = document.getElementById('mensagemTagsContainer');
            var chips = document.getElementById('mensagemTagsChips');
            var input = document.getElementById('mensagemTagsInput');
            var campo = document.querySelector('[id$="txtsTags"]');
            if (!container || !chips || !input || !campo) return;

            var somenteLeitura = campo.readOnly || campo.disabled;
            container.className = somenteLeitura ? 'mensagem-tags tag-readonly' : 'mensagem-tags';
            var tags = Mensagens_RecuperarTags();
            input.disabled = somenteLeitura || tags.length >= Mensagens_LimiteTags();
            input.style.display = somenteLeitura ? 'none' : '';
            input.placeholder = tags.length >= Mensagens_LimiteTags() ? 'Limite de 3 tags atingido' : 'Adicionar tag...';
            chips.innerHTML = '';

            for (var i = 0; i < tags.length; i++) {
                var span = document.createElement('span');
                span.className = 'mensagem-tag';
                span.appendChild(document.createTextNode(tags[i]));

                if (!somenteLeitura) {
                    var botao = document.createElement('button');
                    botao.type = 'button';
                    botao.setAttribute('aria-label', 'Remover tag ' + tags[i]);
                    botao.innerHTML = '&times;';
                    botao.onclick = (function (tag) {
                        return function () { Mensagens_RemoverTag(tag); };
                    })(tags[i]);
                    span.appendChild(botao);
                }

                chips.appendChild(span);
            }
        }

        function Mensagens_InicializarTags() {
            var container = document.getElementById('mensagemTagsContainer');
            var input = document.getElementById('mensagemTagsInput');
            if (!container || !input || input.getAttribute('data-tags-inicializado') === 'S') {
                Mensagens_RenderizarTags();
                return;
            }

            input.setAttribute('data-tags-inicializado', 'S');
            container.onclick = function () {
                if (!input.disabled) input.focus();
            };
            input.onkeydown = function (e) {
                if (e.key === 'Enter' || e.key === ',' || e.key === ';') {
                    e.preventDefault();
                    Mensagens_PrepararTags();
                }
                if (e.key === 'Backspace' && !input.value) {
                    var tags = Mensagens_RecuperarTags();
                    tags.pop();
                    Mensagens_SalvarTags(tags);
                    Mensagens_RenderizarTags();
                }
            };
            input.onblur = Mensagens_PrepararTags;
            input.onpaste = function () {
                window.setTimeout(function () {
                    var partes = input.value.split(/[,\s;]+/);
                    if (partes.length > 1) {
                        for (var i = 0; i < partes.length; i++) Mensagens_AdicionarTag(partes[i]);
                        input.value = '';
                    }
                }, 0);
            };

            Mensagens_RenderizarTags();
        }

        function Mensagens_PrepararEnvio() {
            Mensagens_PrepararTags();

            var editor = document.querySelector('[id$="txtsCorpo"]');
            var hidden = document.querySelector('[id$="hddsCorpoHtml"]');
            if (!editor || !hidden) return true;

            var conteudo = editor.value || '';
            if (window.tinymce) {
                var instancia = tinymce.get(editor.id);
                if (instancia) conteudo = instancia.getContent();
            }

            if (!conteudo && hidden.value) return true;

            hidden.value = encodeURIComponent(conteudo);

            if (window.tinymce) {
                var instanciaLimpar = tinymce.get(editor.id);
                if (instanciaLimpar) {
                    instanciaLimpar.setContent('');
                    instanciaLimpar.save();
                }
            }
            editor.value = '';
            editor.disabled = true;

            return true;
        }

        function Mensagens_ReabilitarCorpoHtml() {
            var editor = document.querySelector('[id$="txtsCorpo"]');
            if (editor) editor.disabled = false;
        }

        function Mensagens_RegistrarPreparacaoPostBack() {
            if (window.Mensagens_PreparacaoPostBackRegistrada) return;

            var formulario = document.forms[0];
            if (formulario) {
                formulario.addEventListener('submit', Mensagens_PrepararEnvio, true);
            }

            if (!window.Sys || !Sys.WebForms || !Sys.WebForms.PageRequestManager) {
                window.Mensagens_PreparacaoPostBackRegistrada = true;
                return;
            }

            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_initializeRequest(function () {
                Mensagens_PrepararEnvio();
            });
            prm.add_endRequest(function () {
                Mensagens_ReabilitarCorpoHtml();
            });

            window.Mensagens_PreparacaoPostBackRegistrada = true;
        }

        function pageLoad() {
            Mensagens_ReabilitarCorpoHtml();
            Mensagens_AtualizarAvisoDashboard();
            Mensagens_AtualizarComentarios();
            Mensagens_AtualizarPreviewDestinatarios();
            Mensagens_InicializarTags();
            Mensagens_RegistrarPreparacaoPostBack();
            if (window.Aplicar_TinyMCE) window.Aplicar_TinyMCE();
        }
    </script>
        
        
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>


            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA">

                        </asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div class="form-stacked row">

                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </div>


                <div class="col-lg-6">
                    <div class="form-group">
                        <label>De: </label>
                        <asp:TextBox ID="txtidUsuarioRemetente" class="form-control CaixaTextoMedio" runat="server" disabled="0"></asp:TextBox>
                    </div>
                </div>
                <div class="col-lg-3">
                    <div class="form-group">
                        <label>É um Aviso?</label>
                        <div class="tt-switch">
                            <asp:CheckBox ID="cbAviso" runat="server" Text="Sim" onclick="Mensagens_AtualizarAvisoDashboard();" />
                        </div>
                        <div id="divAvisoDashboard" runat="server" class="alert alert-info mensagem-alerta-dashboard">
                            Esta mensagem será exibida no Dashboard!
                        </div>
                    </div>
                </div>
                <div class="col-lg-3">
                    <div class="form-group">
                        <label>Permite Comentários?</label>
                        <div class="tt-switch">
                            <asp:CheckBox ID="cbPermiteComentarios" runat="server" Text="Sim" onclick="Mensagens_AtualizarComentarios();" />
                        </div>
                    </div>
                </div>

                <div class="col-lg-6" runat="server" id="DIV_Destinatario_GRUPO">
                    <div class="form-group">
                        <label>Departamentos</label>
                        <div class="mensagem-picker">
                            <div class="mensagem-picker-header">
                                <input type="text" class="form-control" placeholder="Buscar departamento..." onkeyup="Mensagens_FiltrarLista(this);" />
                            </div>
                            <div class="mensagem-picker-lista">
                                <asp:CheckBoxList ID="cblDepartamentos" runat="server" AutoPostBack="true" OnSelectedIndexChanged="cblDepartamentos_SelectedIndexChanged" RepeatDirection="Vertical"></asp:CheckBoxList>
                                <asp:Label ID="lblDepartamentosVazio" runat="server" CssClass="mensagem-picker-empty" Text="Nenhum departamento disponível para mensagens." Visible="false"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>

                <div class="col-lg-6">
                    <div class="form-group" runat="server" id="DIV_USUARIOS">
                        <label>Usuários</label>
                        <div class="mensagem-picker">
                            <div class="mensagem-picker-header">
                                <input type="text" class="form-control" placeholder="Buscar usuário..." onkeyup="Mensagens_FiltrarLista(this);" />
                            </div>
                            <div class="mensagem-picker-lista">
                                <asp:CheckBoxList ID="cblUsuarios" runat="server" RepeatDirection="Vertical" onclick="Mensagens_AtualizarPreviewDestinatarios();"></asp:CheckBoxList>
                                <asp:Label ID="lblUsuariosVazio" runat="server" CssClass="mensagem-picker-empty" Text="Selecione um departamento para listar usuários." Visible="false"></asp:Label>
                            </div>
                        </div>
                    </div>
                </div>
             


                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Assunto </label>
                        <asp:TextBox ID="txtsAssunto" class="form-control " runat="server" MaxLength="50"></asp:TextBox>
                    </div>
                </div>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Tags</label>
                        <div id="mensagemTagsContainer" class="mensagem-tags">
                            <span id="mensagemTagsChips"></span>
                            <input type="text" id="mensagemTagsInput" class="mensagem-tags-input" placeholder="Adicionar tag..." autocomplete="off" />
                        </div>
                        <asp:TextBox ID="txtsTags" class="mensagem-tags-valor" runat="server" MaxLength="500"></asp:TextBox>
                        <div class="mensagem-tags-ajuda">Digite uma tag e pressione Enter ou vírgula para adicionar. Limite de 3 tags por mensagem.</div>
                    </div>
                </div>

              <%--  <div class="col-lg-12">
                    <div class="form-group">
                        <label>Corpo </label>
                        <asp:TextBox ID="txtsCorpo" class="form-control" runat="server" MaxLength="4000" TextMode="MultiLine" Height="150px"></asp:TextBox>
                    </div>
                </div>--%>

                <div class="col-lg-12">
                    <div class="form-group">
                        <label>Corpo </label>
                        <textarea runat="server" id="txtsCorpo" class="htmlEditor"></textarea>
                    </div>
                </div>

                <div class="col-lg-12"><%-- ----------------------------esssa--%>

                    <%--------------------------------------------------------------------------------%>

                    <div id="div_EnviarArquivos" class="form-stacked " runat="server">
                        <div class="panel panel-default" runat="server">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Destinatários </b></h3>
                            </div>

                            <div class="panel-body">
                                <div id="destinatariosPreviewVazio" class="alert alert-info">
                                    Selecione usuários para visualizar os destinatários antes do envio.
                                </div>
                                <div class="destinatarios-preview">
                                    <table class="table table-condensed table-striped">
                                        <thead>
                                            <tr>
                                                <th>Nome</th>
                                                <th>E-mail</th>
                                                <th>Status</th>
                                            </tr>
                                        </thead>
                                        <tbody id="destinatariosPreviewBody"></tbody>
                                    </table>
                                </div>
                                <hr />
                                <div class="form-stacked row">

                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgDestinatarios" class="table table-striped table-bordered table-hover table-condensed "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">

                                            <Columns>

                                                <asp:BoundField DataField="sDscUsuario" HeaderText="Nome">
                                                    <ItemStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sEmail" HeaderText="E-mail">
                                                    <ItemStyle Width="30%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="dtLeitura" HeaderText="Data Leitura">
                                                    <ItemStyle Width="20%" HorizontalAlign="left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                            </Columns>

                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div> <%-- ----------------------------esssa--%>

                <div class="col-lg-12">
                    <asp:Panel ID="pnlComentarios" runat="server">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Comentários</b></h3>
                            </div>
                            <div class="panel-body">
                                <asp:Repeater ID="rptComentarios" runat="server">
                                    <ItemTemplate>
                                        <div class="mensagem-comentario">
                                            <strong><%# Eval("sDscUsuario") %></strong>
                                            <small><%# Eval("dtInclusao") %></small>
                                            <p><%# Eval("sComentario") %></p>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <div class="form-group">
                                    <label>Novo comentário</label>
                                    <asp:TextBox ID="txtsComentario" runat="server" CssClass="form-control" TextMode="MultiLine" Height="90px" MaxLength="1000"></asp:TextBox>
                                </div>
                                <asp:Button ID="cmdSalvarComentario" class="btn btn-sm btn-primary" runat="server" Text="Publicar Comentário" OnClick="cmdSalvarComentario_Click" OnClientClick="return Mensagens_PrepararEnvio();" />
                            </div>
                        </div>
                    </asp:Panel>
                </div>

                <div class="col-lg-12">
                    <asp:Panel ID="pnlHistorico" runat="server">
                        <div class="panel panel-default">
                            <div class="panel-heading">
                                <h3 class="panel-title"><b>Histórico</b></h3>
                            </div>
                            <div class="panel-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="dtgHistorico" class="table table-striped table-bordered table-hover table-condensed"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                        <Columns>
                                            <asp:BoundField DataField="dtInclusao" HeaderText="Data" />
                                            <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário" />
                                            <asp:BoundField DataField="sAcao" HeaderText="Ação" />
                                            <asp:BoundField DataField="sAntes" HeaderText="Antes" />
                                            <asp:BoundField DataField="sDepois" HeaderText="Depois" />
                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                </div>

            <div class="col-lg-12">
            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />
             </div>
            
                 <div class="col-lg-12">
        <fieldset class="form-stacked actions">
            <asp:Button ID="cmdEditar" class="btn btn-lg btn-primary" runat="server" Text="Editar" OnClick="cmdEditar_Click" />
            &nbsp;
            <asp:Button ID="cmdSalvar" class="btn  btn-lg btn-success" runat="server" Text="Salvar" OnClick="cmdSalvar_Click" OnClientClick="return Mensagens_PrepararEnvio();" />
            &nbsp;
            <input type="submit" name="cancel" class="btn btn-lg btn-warning" value="Voltar" id="field-cancel" title="Voltar" onclick="history.go(-1)" >&nbsp;
        </fieldset>
                     </div>

        <asp:HiddenField ID="hddidMensagens" runat="server" />
        <asp:HiddenField ID="hddPodeEditar" runat="server" />
        <asp:HiddenField ID="hddEmEdicao" runat="server" />
        <asp:HiddenField ID="hddsCorpoHtml" runat="server" />

        </ContentTemplate>            
    </asp:UpdatePanel>
</asp:Content>

