<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MensagensDropdown.ascx.cs" Inherits="TT_Flow.App.Controles.MensagensDropdown" %>

<asp:PlaceHolder ID="phDropdown" runat="server">
    <li class="dropdown messages-dropdown">
        <a id="lnkAbrirDropdown" runat="server" href="#" class="dropdown-toggle messages-dropdown-toggle"
            data-toggle="dropdown" role="button" aria-haspopup="true" aria-expanded="false">
            <span class="messages-dropdown-trigger-icon" aria-hidden="true">
                <i class="fa fa-envelope-o"></i>
            </span>
            <span class="messages-dropdown-trigger-text">Mensagens</span>
            <asp:Label ID="lblBadge" runat="server" CssClass="messages-dropdown-badge"></asp:Label>
            <i class="fa fa-angle-down messages-dropdown-caret" aria-hidden="true"></i>
        </a>

        <ul class="dropdown-menu messages-dropdown-menu" role="menu">
            <li class="messages-dropdown-header">
                <div>
                    <span class="messages-dropdown-title">Mensagens</span>
                    <asp:Label ID="lblResumo" runat="server" CssClass="messages-dropdown-summary"></asp:Label>
                </div>
                <span class="messages-dropdown-header-icon" aria-hidden="true"><i class="fa fa-comments-o"></i></span>
            </li>

            <asp:PlaceHolder ID="phFiltros" runat="server">
                <li class="messages-dropdown-filters" role="group" aria-label="Filtrar mensagens não lidas">
                    <button type="button" class="messages-dropdown-filter-button is-active" data-message-filter="todos" aria-pressed="true">
                        Todos
                    </button>
                    <button type="button" class="messages-dropdown-filter-button" data-message-filter="mensagem" aria-pressed="false">
                        <i class="fa fa-envelope-o" aria-hidden="true"></i> Mensagens
                    </button>
                    <button type="button" class="messages-dropdown-filter-button" data-message-filter="aviso" aria-pressed="false">
                        <i class="fa fa-bullhorn" aria-hidden="true"></i> Avisos
                    </button>
                </li>
            </asp:PlaceHolder>

            <li class="messages-dropdown-content">
                <ul class="messages-dropdown-list">
                    <asp:PlaceHolder ID="phLista" runat="server">
                        <asp:Repeater ID="rptMensagens" runat="server">
                            <ItemTemplate>
                                <li class='<%# ObterClasseLinha(Container.ItemIndex) %>' data-message-type='<%# ObterTipoFiltro(Eval("TipoMensagem")) %>'>
                                    <div class='<%# ObterClasseItem(Eval("TipoMensagem")) %>'>
                                        <span class="messages-dropdown-item-icon" aria-hidden="true">
                                            <i class='<%# ObterClasseIcone(Eval("TipoMensagem")) %>'></i>
                                        </span>
                                        <div class="messages-dropdown-item-content">
                                            <div class="messages-dropdown-item-meta">
                                                <span class="messages-dropdown-item-sender"><%# Codificar(Eval("sDscUsuarioRemetente")) %></span>
                                                <span class="messages-dropdown-item-date"><%# FormatarDataResumida(Eval("dtMensagem")) %></span>
                                            </div>
                                            <div class="messages-dropdown-item-subject"><%# CodificarAssunto(Eval("sAssunto")) %></div>
                                            <div class="messages-dropdown-item-actions">
                                                <span class="messages-dropdown-item-type"><%# ObterRotuloTipo(Eval("TipoMensagem")) %></span>
                                                <a class="messages-dropdown-view" href='<%# MontarUrlDetalhe(Eval("idMensagem")) %>'>
                                                    <i class="fa fa-eye" aria-hidden="true"></i> Visualizar
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </li>
                            </ItemTemplate>
                        </asp:Repeater>

                        <li class="messages-dropdown-filter-empty messages-dropdown-state" aria-live="polite" hidden="hidden">
                            <i class="fa fa-check-circle-o" aria-hidden="true"></i>
                            <strong>Nenhum item neste filtro</strong>
                            <span>Selecione outro filtro para consultar as demais não lidas.</span>
                        </li>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phVazio" runat="server">
                        <li class="messages-dropdown-state">
                            <i class="fa fa-check-circle-o" aria-hidden="true"></i>
                            <strong>Tudo em dia</strong>
                            <span>Você não possui mensagens não lidas.</span>
                        </li>
                    </asp:PlaceHolder>

                    <asp:PlaceHolder ID="phFalha" runat="server">
                        <li class="messages-dropdown-state messages-dropdown-state-warning">
                            <i class="fa fa-exclamation-circle" aria-hidden="true"></i>
                            <strong>Lista indisponível</strong>
                            <span>Não foi possível carregar as mensagens recentes.</span>
                        </li>
                    </asp:PlaceHolder>
                </ul>
            </li>

            <li class="messages-dropdown-footer">
                <asp:HyperLink ID="lnkVerTodas" runat="server" CssClass="messages-dropdown-footer-link">
                    <i class="fa fa-inbox" aria-hidden="true"></i> Ver todas
                </asp:HyperLink>
                <asp:HyperLink ID="lnkNovaMensagem" runat="server" CssClass="messages-dropdown-footer-link messages-dropdown-footer-primary">
                    <i class="fa fa-plus" aria-hidden="true"></i> Nova mensagem
                </asp:HyperLink>
            </li>
        </ul>
    </li>
</asp:PlaceHolder>

<script>
    (function () {
        "use strict";

        var limiteItens = 5;

        function encontrarBotaoFiltro(elemento, raiz) {
            while (elemento && elemento !== raiz) {
                if (elemento.classList && elemento.classList.contains("messages-dropdown-filter-button")) {
                    return elemento;
                }

                elemento = elemento.parentNode;
            }

            return null;
        }

        function aplicarFiltro(raiz, filtro) {
            var itens = raiz.querySelectorAll(".messages-dropdown-item[data-message-type]");
            var botoes = raiz.querySelectorAll(".messages-dropdown-filter-button");
            var vazio = raiz.querySelector(".messages-dropdown-filter-empty");
            var conteudo = raiz.querySelector(".messages-dropdown-content");
            var exibidos = 0;
            var i;

            for (i = 0; i < itens.length; i += 1) {
                var corresponde = filtro === "todos" || itens[i].getAttribute("data-message-type") === filtro;
                var exibir = corresponde && exibidos < limiteItens;

                itens[i].classList.toggle("is-visible", exibir);
                itens[i].hidden = !exibir;
                itens[i].setAttribute("aria-hidden", exibir ? "false" : "true");

                if (exibir) {
                    exibidos += 1;
                }
            }

            for (i = 0; i < botoes.length; i += 1) {
                var ativo = botoes[i].getAttribute("data-message-filter") === filtro;
                botoes[i].classList.toggle("is-active", ativo);
                botoes[i].setAttribute("aria-pressed", ativo ? "true" : "false");
            }

            if (vazio) {
                vazio.hidden = exibidos !== 0;
                vazio.classList.toggle("is-active", exibidos === 0);
            }

            if (conteudo) {
                conteudo.scrollTop = 0;
            }
        }

        function iniciarDropdown(raiz) {
            if (raiz.getAttribute("data-message-filter-ready") === "true") {
                return;
            }

            raiz.setAttribute("data-message-filter-ready", "true");
            raiz.addEventListener("click", function (evento) {
                var botao = encontrarBotaoFiltro(evento.target, raiz);
                if (!botao) {
                    return;
                }

                evento.preventDefault();
                evento.stopPropagation();
                aplicarFiltro(raiz, botao.getAttribute("data-message-filter") || "todos");
            });

            aplicarFiltro(raiz, "todos");
        }

        function iniciarTodos() {
            var dropdowns = document.querySelectorAll(".messages-dropdown");
            var i;

            for (i = 0; i < dropdowns.length; i += 1) {
                iniciarDropdown(dropdowns[i]);
            }
        }

        if (document.readyState === "loading") {
            document.addEventListener("DOMContentLoaded", iniciarTodos);
        } else {
            iniciarTodos();
        }

        if (window.Sys && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(iniciarTodos);
        }
    }());
</script>
