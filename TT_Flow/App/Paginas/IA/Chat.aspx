
<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="TT_Flow.App.Paginas.IA.Chat" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="<%= ResolveUrl("~/App/JS/ia-markdown.js") %>" type="text/javascript"></script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
        .ia-chat-shell {
            display: grid;
            grid-template-columns: minmax(240px, 280px) minmax(0, 1fr);
            gap: 16px;
            width: 100%;
            min-width: 0;
            min-height: 520px;
            height: calc(100vh - 245px);
        }

        .ia-chat-sidebar,
        .ia-chat-main {
            min-width: 0;
            min-height: 0;
            border: 1px solid #d9e2ec;
            background: #fff;
            border-radius: 4px;
        }

        .ia-chat-sidebar {
            display: flex;
            flex-direction: column;
            overflow: hidden;
        }

        .ia-chat-sidebar-header,
        .ia-chat-header {
            padding: 12px 14px;
            border-bottom: 1px solid #d9e2ec;
            background: #f7f9fb;
            font-weight: 700;
            color: #243b53;
        }

        .ia-chat-header {
            display: flex;
            align-items: center;
            gap: 10px;
            min-height: 58px;
        }

        .ia-chat-sidebar-header {
            display: flex;
            align-items: center;
            justify-content: space-between;
            gap: 8px;
        }

        .ia-chat-sidebar-title {
            display: inline-flex;
            align-items: center;
            gap: 6px;
            min-width: 0;
        }

        .ia-chat-sidebar-header .btn {
            flex: 0 0 auto;
            font-weight: 700;
            padding: 3px 8px;
        }

        .ia-chat-conversas {
            flex: 1 1 auto;
            min-height: 0;
            max-height: none;
            overflow-y: auto;
        }

        .ia-chat-conversa {
            display: block;
            width: 100%;
            padding: 10px 12px;
            border: 0;
            border-bottom: 1px solid #eef2f7;
            background: #fff;
            text-align: left;
            color: #334e68;
        }

        .ia-chat-conversa:hover,
        .ia-chat-conversa.is-active {
            background: #eef6ee;
            color: #005000;
        }

        .ia-chat-conversa small {
            display: block;
            color: #7b8794;
            margin-top: 3px;
        }

        .ia-chat-main {
            display: flex;
            min-width: 0;
            flex-direction: column;
            overflow: hidden;
        }

        .ia-chat-messages {
            flex: 1 1 auto;
            min-height: 0;
            max-height: none;
            overflow-y: auto;
            padding: 18px;
            background: #fbfcfd;
        }

        .ia-chat-empty {
            margin: 30px auto;
            max-width: 560px;
            color: #52606d;
            text-align: center;
            line-height: 1.6;
        }

        .ia-chat-empty-animation {
            display: block;
            width: min(320px, 100%);
            height: auto;
            margin: 0 auto 8px;
        }

        .ia-chat-empty-copy {
            min-height: 24px;
        }

        .ia-chat-message {
            max-width: 82%;
            margin-bottom: 12px;
            padding: 11px 13px;
            border-radius: 4px;
            line-height: 1.45;
            word-break: break-word;
        }

        .ia-chat-message.user {
            margin-left: auto;
            background: #eef6ee;
            color: #17324d;
            white-space: pre-wrap;
        }

        .ia-chat-message.assistant {
            margin-right: auto;
            background: #ffffff;
            border: 1px solid #d9e2ec;
            color: #263238;
        }

        .ia-chat-message.error {
            margin-right: auto;
            background: #fff1f0;
            border: 1px solid #f2b8b5;
            color: #8a1f17;
        }

        .ia-chat-message p {
            margin: 0 0 8px;
        }

        .ia-chat-message p:last-child,
        .ia-chat-message ol:last-child,
        .ia-chat-message ul:last-child,
        .ia-chat-message .ia-chat-table-wrap:last-child {
            margin-bottom: 0;
        }

        .ia-chat-message ol,
        .ia-chat-message ul {
            margin: 6px 0 8px 22px;
            padding-left: 16px;
        }

        .ia-chat-message li {
            margin: 3px 0;
        }

        .ia-chat-table-wrap {
            width: 100%;
            margin: 8px 0 10px;
            overflow-x: auto;
        }

        .ia-chat-message table {
            width: 100%;
            min-width: 520px;
            border-collapse: collapse;
            background: #fff;
            font-size: 12px;
            line-height: 1.35;
        }

        .ia-chat-message th,
        .ia-chat-message td {
            padding: 7px 8px;
            border: 1px solid #d9e2ec;
            text-align: left;
            vertical-align: top;
        }

        .ia-chat-message th {
            background: #f1f5f9;
            color: #243b53;
            font-weight: 700;
        }

        .ia-chat-message tr:nth-child(even) td {
            background: #fbfcfd;
        }

        .ia-chat-empty-response {
            color: #7b8794;
            font-style: italic;
        }

        .ia-chat-message code {
            padding: 1px 4px;
            border-radius: 3px;
            background: #eef2f7;
            color: #17324d;
            font-family: Consolas, Monaco, monospace;
            font-size: 12px;
        }

        /* Sem color: link herda o azul do bootstrap. Link azul e convencao da web, nao identidade. */
        .ia-chat-message a {
            font-weight: 700;
            text-decoration: underline;
        }

        .ia-chat-sources {
            margin-top: 10px;
            padding-top: 8px;
            border-top: 1px solid #e6edf5;
        }

        .ia-chat-sources-title {
            margin-bottom: 6px;
            color: #52606d;
            font-size: 12px;
            font-weight: 700;
        }

        .ia-chat-source {
            display: block;
            margin: 4px 0;
            padding: 6px 8px;
            border: 1px solid #d9e2ec;
            border-radius: 4px;
            background: #fbfcfd;
            color: #243b53;
            text-decoration: none;
        }

        .ia-chat-source:hover {
            background: #eef6ee;
            text-decoration: none;
        }

        .ia-chat-source strong {
            display: block;
            color: #005000;
        }

        .ia-chat-source small {
            display: block;
            color: #7b8794;
            font-weight: 400;
        }

        .ia-chat-source-head {
            display: flex;
            align-items: flex-start;
            justify-content: space-between;
            gap: 8px;
        }

        .ia-chat-source-kind {
            display: inline-block;
            margin-right: 5px;
            padding: 1px 5px;
            border-radius: 3px;
            background: #e6f4ea;
            color: #1b6b43;
            font-size: 10px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .ia-chat-source-file {
            border-left: 3px solid #5cb85c;
        }

        .ia-chat-source-meta {
            margin-top: 4px;
        }

        .ia-chat-source-toggle {
            margin-top: 6px;
            padding: 2px 7px;
            border: 1px solid #c9d6e2;
            border-radius: 4px;
            background: #fff;
            color: #005000;
            font-size: 11px;
        }

        .ia-chat-source-toggle:hover {
            background: #eef6ee;
        }

        .ia-chat-source-preview {
            display: none;
            margin: 7px 0 0;
            max-height: 220px;
            overflow: auto;
            white-space: pre-wrap;
            word-break: break-word;
            border: 1px solid #d9e2ec;
            border-radius: 4px;
            background: #f7f9fb;
            color: #263238;
            font-family: Consolas, "Courier New", monospace;
            font-size: 11px;
            line-height: 1.45;
            padding: 8px;
        }

        .ia-chat-source.is-open .ia-chat-source-preview {
            display: block;
        }

        .ia-chat-feedback {
            display: flex;
            align-items: center;
            flex-wrap: wrap;
            gap: 6px;
            margin-top: 10px;
            color: #7b8794;
            font-size: 12px;
        }

        .ia-chat-feedback button {
            width: 28px;
            height: 28px;
            padding: 0;
            border: 1px solid #d9e2ec;
            border-radius: 4px;
            background: #fff;
            color: #52606d;
            line-height: 26px;
            text-align: center;
        }

        .ia-chat-feedback button:hover {
            background: #eef6ee;
            color: #005000;
        }

        /* Selecionado se distingue do hover pela borda forte: o fundo dos dois e o mesmo tint. */
        .ia-chat-feedback button.is-selected {
            border-color: #5cb85c;
            background: #eef6ee;
            color: #005000;
        }

        .ia-chat-feedback button.is-saving {
            opacity: .65;
            pointer-events: none;
        }

        .ia-chat-feedback-comment {
            display: none;
            width: 100%;
            margin-top: 6px;
        }

        .ia-chat-feedback.is-comment-open .ia-chat-feedback-comment {
            display: block;
        }

        .ia-chat-feedback-comment textarea {
            min-height: 62px;
            resize: vertical;
            font-size: 12px;
        }

        .ia-chat-feedback-actions {
            margin-top: 6px;
            text-align: right;
        }

        .ia-chat-feedback .ia-chat-feedback-save,
        .ia-chat-feedback .ia-chat-feedback-cancel {
            width: auto;
            min-width: 32px;
            padding: 0 8px;
        }

        .ia-chat-composer {
            flex: 0 0 auto;
            padding: 14px;
            border-top: 1px solid #d9e2ec;
            background: #fff;
        }

        .ia-chat-composer textarea {
            resize: vertical;
            min-height: 78px;
            max-height: 170px;
        }

        .ia-chat-attachments {
            display: flex;
            flex-wrap: wrap;
            gap: 6px;
            margin-top: 8px;
        }

        .ia-chat-attachment {
            display: inline-flex;
            align-items: center;
            max-width: 100%;
            gap: 6px;
            padding: 5px 7px;
            border: 1px solid #d9e2ec;
            border-radius: 4px;
            background: #f7f9fb;
            color: #334e68;
            font-size: 12px;
        }

        .ia-chat-attachment strong {
            overflow: hidden;
            max-width: 220px;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        .ia-chat-attachment small {
            color: #7b8794;
        }

        .ia-chat-attachment.is-ready {
            border-color: #a7d7c5;
            background: #f1fbf7;
        }

        .ia-chat-attachment.is-error {
            border-color: #f2b8b5;
            background: #fff1f0;
            color: #8a1f17;
        }

        .ia-chat-attachment-remove {
            width: 20px;
            height: 20px;
            padding: 0;
            border: 0;
            background: transparent;
            color: #52606d;
            line-height: 18px;
        }

        .ia-chat-message-attachments {
            margin-top: 8px;
            padding-top: 7px;
            border-top: 1px solid rgba(23, 50, 77, .12);
        }

        .ia-chat-actions {
            display: flex;
            align-items: center;
            justify-content: space-between;
            flex-wrap: wrap;
            gap: 12px;
            margin-top: 10px;
        }

        .ia-chat-actions > div {
            display: flex;
            align-items: center;
            justify-content: flex-end;
            flex-wrap: wrap;
            gap: 6px;
        }

        .ia-chat-status {
            color: #7b8794;
            font-size: 12px;
        }

        .ia-chat-contexto {
            display: flex;
            align-items: center;
            gap: 8px;
            margin-bottom: 8px;
            padding: 7px 10px;
            border: 1px solid #a7d7c5;
            border-radius: 4px;
            background: #eef6ee;
            color: #005000;
            font-size: 12px;
        }

        .ia-chat-contexto strong {
            flex: 1 1 auto;
            min-width: 0;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        .ia-chat-contexto button {
            width: 20px;
            height: 20px;
            padding: 0;
            border: 0;
            background: transparent;
            color: #52606d;
        }

        .ia-chat-contexto button:hover {
            color: #8a1f17;
        }

        .ia-chat-templates {
            margin-bottom: 8px;
            border: 1px solid #d9e2ec;
            border-radius: 4px;
            background: #f7f9fb;
        }

        .ia-chat-templates-header {
            padding: 7px 10px;
            border-bottom: 1px solid #e6edf5;
            color: #243b53;
            font-size: 12px;
            font-weight: 700;
        }

        .ia-chat-templates-lista {
            max-height: 180px;
            overflow-y: auto;
        }

        .ia-chat-template-item {
            display: flex;
            align-items: stretch;
            border-bottom: 1px solid #eef2f7;
        }

        .ia-chat-template-usar {
            flex: 1 1 auto;
            min-width: 0;
            overflow: hidden;
            padding: 7px 10px;
            border: 0;
            background: transparent;
            color: #005000;
            font-size: 12px;
            text-align: left;
            text-overflow: ellipsis;
            white-space: nowrap;
        }

        .ia-chat-template-usar:hover {
            background: #eef6ee;
        }

        .ia-chat-template-usar small {
            color: #7b8794;
        }

        .ia-chat-template-del {
            width: 28px;
            padding: 0;
            border: 0;
            background: transparent;
            color: #9fb3c8;
        }

        .ia-chat-template-del:hover {
            color: #8a1f17;
        }

        .ia-chat-templates-vazio {
            padding: 10px;
            color: #7b8794;
            font-size: 12px;
        }

        .ia-chat-templates-novo {
            display: flex;
            gap: 6px;
            padding: 8px 10px;
            border-top: 1px solid #e6edf5;
        }

        .ia-chat-templates-novo input {
            flex: 1 1 auto;
        }

        .ia-chat-acao {
            border-left: 3px solid #e8a13c;
            background: #fffaf0;
        }

        .ia-chat-acao.is-erro {
            border-left-color: #c0392b;
        }

        .ia-chat-acao-titulo {
            margin-bottom: 6px;
            color: #8a5a12;
            font-size: 12px;
            font-weight: 700;
            text-transform: uppercase;
        }

        .ia-chat-acao-descricao {
            margin: 6px 0 0;
            color: #5f4b25;
            line-height: 1.45;
        }

        .ia-chat-acao-resumo {
            margin-top: 8px;
            padding: 8px 10px;
            border: 1px solid #efd8ad;
            background: #fff;
        }

        .ia-chat-acao-resumo-titulo {
            margin-bottom: 4px;
            color: #6f4b12;
            font-size: 12px;
            font-weight: 700;
        }

        .ia-chat-acao-resumo dl {
            display: grid;
            grid-template-columns: max-content 1fr;
            gap: 2px 12px;
            margin: 0;
        }

        .ia-chat-acao-resumo dt {
            color: #6c6252;
            font-weight: 400;
        }

        .ia-chat-acao-resumo dd {
            margin: 0;
            font-weight: 600;
            word-break: break-word;
        }

        .ia-chat-acao-botoes {
            display: flex;
            gap: 8px;
            margin-top: 10px;
            flex-wrap: wrap;
        }

        .ia-chat-acao-campos {
            margin-top: 8px;
        }

        .ia-chat-acao-grupo {
            margin: 8px 0 5px;
            padding-top: 6px;
            border-top: 1px solid #efd8ad;
            color: #6f4b12;
            font-weight: 700;
            font-size: 12px;
        }

        .ia-chat-acao-campo {
            margin-bottom: 7px;
        }

        .ia-chat-acao-campo label {
            display: block;
            margin-bottom: 2px;
            font-size: 12px;
            font-family: Consolas, Monaco, monospace;
            color: #5b431b;
        }

        .ia-chat-acao-campo .help-block {
            margin: 2px 0 0;
            font-size: 11px;
        }

        .ia-chat-escolhas {
            display: grid;
            gap: 6px;
        }

        .ia-chat-escolha {
            display: flex;
            align-items: flex-start;
            gap: 8px;
            margin: 0;
            padding: 8px;
            border: 1px solid #dfc58f;
            background: #fff;
            cursor: pointer;
        }

        .ia-chat-escolha input {
            margin-top: 3px;
        }

        .ia-chat-escolha span,
        .ia-chat-escolha small {
            display: block;
        }

        .ia-chat-escolha small {
            margin-top: 2px;
            color: #6c6252;
            font-weight: 400;
        }

        .ia-chat-typing {
            display: flex;
            align-items: center;
            gap: 5px;
            max-width: 82%;
            margin-right: auto;
            margin-bottom: 12px;
            padding: 13px 15px;
            border: 1px solid #d9e2ec;
            border-radius: 4px;
            background: #fff;
        }

        .ia-chat-typing span {
            width: 7px;
            height: 7px;
            border-radius: 50%;
            background: #90a4b8;
            animation: iaTypingPulse 1.2s infinite ease-in-out;
        }

        .ia-chat-typing span:nth-child(2) {
            animation-delay: .2s;
        }

        .ia-chat-typing span:nth-child(3) {
            animation-delay: .4s;
        }

        /* Passo do workflow em andamento e tempo decorrido, ao lado dos pontinhos */
        .ia-chat-typing em {
            min-width: 0;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
            color: #52606d;
            font-size: 12px;
            font-style: normal;
        }

        .ia-chat-typing em.ia-chat-typing-tempo {
            flex: 0 0 auto;
            color: #7b8794;
        }

        .ia-chat-typing em:first-of-type {
            margin-left: 4px;
        }

        @keyframes iaTypingPulse {
            0%, 80%, 100% { opacity: .25; transform: translateY(0); }
            40% { opacity: 1; transform: translateY(-3px); }
        }

        .ia-chat-msg-tools {
            display: flex;
            gap: 6px;
            margin-top: 8px;
        }

        .ia-chat-msg-tool {
            padding: 2px 8px;
            border: 1px solid #d9e2ec;
            border-radius: 4px;
            background: #fff;
            color: #52606d;
            font-size: 11px;
        }

        .ia-chat-msg-tool:hover {
            background: #eef6ee;
            color: #005000;
        }

        .ia-chat-sidebar-busca {
            padding: 8px 10px;
            border-bottom: 1px solid #eef2f7;
            background: #fff;
        }

        .ia-chat-carregar-mais {
            display: block;
            width: 100%;
            padding: 9px;
            border: 0;
            border-top: 1px solid #eef2f7;
            background: #f7f9fb;
            color: #005000;
            font-size: 12px;
            font-weight: 700;
        }

        .ia-chat-carregar-mais:hover {
            background: #eef6ee;
        }

        .ia-chat-conversa-item {
            display: flex;
            align-items: stretch;
            border-bottom: 1px solid #eef2f7;
        }

        .ia-chat-conversa-item .ia-chat-conversa {
            flex: 1 1 auto;
            min-width: 0;
            border-bottom: 0;
        }

        .ia-chat-conversa-acoes {
            display: flex;
            flex-direction: column;
            justify-content: center;
            gap: 2px;
            padding: 0 6px;
        }

        .ia-chat-conversa-acao {
            width: 22px;
            height: 22px;
            padding: 0;
            border: 0;
            background: transparent;
            color: #9fb3c8;
            font-size: 12px;
            line-height: 22px;
        }

        .ia-chat-conversa-acao:hover {
            color: #005000;
        }

        .ia-chat-conversa-acao.is-favorita {
            color: #e8a13c;
        }

        .ia-chat-prompts {
            display: flex;
            flex-wrap: wrap;
            justify-content: center;
            gap: 8px;
            margin-top: 18px;
        }

        .ia-chat-prompt-chip {
            padding: 6px 12px;
            border: 1px solid #c9d6e2;
            border-radius: 16px;
            background: #fff;
            color: #005000;
            font-size: 12px;
        }

        .ia-chat-prompt-chip:hover {
            background: #eef6ee;
            border-color: #5cb85c;
        }

        .ia-chat-message blockquote {
            margin: 8px 0;
            padding: 6px 10px;
            border-left: 3px solid #c9d6e2;
            background: #f7f9fb;
            color: #52606d;
        }

        .ia-chat-message pre.ia-chat-code {
            margin: 8px 0;
            padding: 10px;
            border: 1px solid #d9e2ec;
            border-radius: 4px;
            background: #f4f6f8;
            overflow-x: auto;
        }

        .ia-chat-message pre.ia-chat-code code {
            display: block;
            padding: 0;
            background: transparent;
            white-space: pre;
        }

        .ia-chat-message del {
            color: #7b8794;
        }

        @media (max-height: 720px) and (min-width: 901px) {
            .ia-chat-shell {
                min-height: 430px;
                height: calc(100vh - 210px);
            }

            .ia-chat-messages {
                padding: 14px;
            }

            .ia-chat-empty {
                margin: 46px auto;
            }

            .ia-chat-composer {
                padding: 10px;
            }

            .ia-chat-composer textarea {
                min-height: 58px;
            }
        }

        @media (max-width: 900px) {
            .ia-chat-shell {
                grid-template-columns: 1fr;
                height: auto;
                min-height: 0;
                gap: 12px;
            }

            .ia-chat-sidebar {
                min-height: 0;
                max-height: 260px;
            }

            .ia-chat-conversas {
                max-height: 190px;
            }

            .ia-chat-main {
                min-height: 520px;
            }

            .ia-chat-messages {
                min-height: 300px;
            }
        }

        @media (max-width: 560px) {
            .ia-chat-sidebar-header,
            .ia-chat-actions,
            .ia-chat-templates-novo,
            .ia-chat-contexto,
            .ia-chat-source-head {
                align-items: stretch;
                flex-direction: column;
            }

            .ia-chat-sidebar-header .btn,
            .ia-chat-actions > div,
            .ia-chat-actions > div .btn,
            .ia-chat-templates-novo .btn,
            .ia-chat-source-toggle {
                width: 100%;
            }

            .ia-chat-actions > div {
                justify-content: stretch;
            }

            .ia-chat-message {
                max-width: 100%;
            }

            .ia-chat-messages {
                padding: 12px;
            }

            .ia-chat-main {
                min-height: 480px;
            }

            .ia-chat-composer {
                padding: 10px;
            }
        }
    </style>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Tina"></asp:Label><small> Assistente IA</small>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Tina" />
        </div>

        <div class="col-lg-12">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>

        <div class="col-lg-12">
            <div class="ia-chat-shell">
                <aside class="ia-chat-sidebar">
                    <div class="ia-chat-sidebar-header">
                        <span class="ia-chat-sidebar-title"><i class="fa fa-comments-o"></i> Conversas</span>
                        <button id="btnNovaConversaIA" type="button" class="btn btn-success btn-xs" title="Novo chat">
                            <i class="fa fa-plus"></i> Novo chat
                        </button>
                    </div>
                    <div class="ia-chat-sidebar-busca">
                        <input id="txtBuscaConversaIA" type="text" class="form-control input-sm" maxlength="100" placeholder="Buscar conversas" autocomplete="off" />
                    </div>
                    <div id="iaChatConversas" class="ia-chat-conversas"></div>
                    <button id="btnCarregarMaisIA" type="button" class="ia-chat-carregar-mais" style="display:none;">Carregar mais</button>
                </aside>

                <section class="ia-chat-main">
                    <div class="ia-chat-header">
                        <i class="fa fa-shield"></i> Tina - Assistente do T-Flow
                    </div>
                    <div id="iaChatMessages" class="ia-chat-messages">
                        <div class="ia-chat-empty">
                            <img class="ia-chat-empty-animation" src="<%= ResolveUrl("~/App/img/tina-v1.gif") %>" alt="Tina surgindo em uma chama" />
                            <div class="ia-chat-empty-copy">Faça uma pergunta ao assistente.</div>
                        </div>
                    </div>
                    <div class="ia-chat-composer">
                        <div id="iaChatContexto" class="ia-chat-contexto" style="display:none;"></div>
                        <div id="iaChatTemplates" class="ia-chat-templates" style="display:none;">
                            <div class="ia-chat-templates-header"><i class="fa fa-bookmark-o"></i> Templates de prompt</div>
                            <div id="iaChatTemplatesLista" class="ia-chat-templates-lista"></div>
                            <div class="ia-chat-templates-novo">
                                <input id="txtTituloTemplateIA" type="text" class="form-control input-sm" maxlength="200" placeholder="Titulo para salvar o texto atual como template" autocomplete="off" />
                                <button id="btnSalvarTemplateIA" type="button" class="btn btn-default btn-sm">Salvar texto atual</button>
                            </div>
                        </div>
                        <textarea id="txtMensagemIA" class="form-control" maxlength="8000" placeholder="Digite sua pergunta"></textarea>
                        <input id="fileIAArquivos" type="file" multiple accept=".pdf,.docx,.txt,.md,.csv,.xlsx" style="display:none;" />
                        <div id="iaChatArquivos" class="ia-chat-attachments"></div>
                        <div class="ia-chat-actions">
                            <span id="iaChatStatus" class="ia-chat-status">Pronto</span>
                            <div>
                                <% if (PodeUsarConhecimento) { %>
                                <%-- A base ja e consultada sozinha quando a pergunta pede. Este botao e o MODO ESTRITO:
                                     forca a busca e proibe a IA de responder por fora do material da base. --%>
                                <button id="btnUsarConhecimentoIA" type="button" class="btn btn-default" title="Modo estrito: responder somente com o que estiver na base de conhecimento (FAQ + documentos). Desligado, a IA ja consulta a base sozinha quando a pergunta pede." aria-label="Responder somente pela base de conhecimento" aria-pressed="false">
                                    <i class="fa fa-book"></i>
                                </button>
                                <% } %>
                                <button id="btnTemplatesIA" type="button" class="btn btn-default" title="Templates de prompt" aria-label="Templates de prompt">
                                    <i class="fa fa-bookmark-o"></i>
                                </button>
                                <button id="btnAnexarIA" type="button" class="btn btn-default" title="Anexar arquivo" aria-label="Anexar arquivo">
                                    <i class="fa fa-paperclip"></i>
                                </button>
                                <button id="btnEnviarIA" type="button" class="btn btn-primary">
                                    <i class="fa fa-paper-plane"></i> Enviar
                                </button>
                            </div>
                        </div>
                    </div>
                </section>
            </div>
        </div>
    </div>

    <script>
        (function () {
            var idConversaAtual = obterParametroInteiro('id');
            var enviando = false;
            var arquivosPendentes = [];
            var maxArquivosMensagem = 3;
            var paginaConversas = 1;
            var termoBuscaConversas = '';
            var buscaConversasTimeout = null;
            var ultimaMensagemUsuario = '';
            var usarConhecimentoAtivo = false;
            var progressoTexto = '';
            var progressoInicio = 0;
            var progressoTimer = null;
            var promptsSugeridos = [
                'O que voce pode fazer?',
                'Qual o status do pedido 12345?',
                'Procurar parceiro pelo CNPJ',
                'Resuma os avisos importantes desta semana'
            ];
            var promptsPorContexto = {
                'PEDIDO': ['Qual o status deste pedido?', 'Resuma este pedido', 'Quais arquivos este pedido tem?', 'Redigir e-mail de atualizacao sobre este pedido'],
                'PARCEIRO': ['Resuma este parceiro', 'Quais pedidos este parceiro tem?', 'Redigir mensagem para este parceiro'],
                'PRODUTO': ['Detalhe este produto', 'Em quais pedidos este produto aparece?']
            };
            var contextoTela = obterContextoTela();

            $(document).ready(function () {
                listarConversas();
                renderizarContextoTela();
                if (idConversaAtual > 0) {
                    carregarConversa(idConversaAtual);
                } else {
                    mostrarEstadoVazio();
                }

                $('#btnEnviarIA').on('click', enviarMensagem);
                $('#btnNovaConversaIA').on('click', novaConversa);
                // Botao so existe se o usuario tem a permissao 695 (renderizado no servidor).
                // Ligado = modo estrito. Desligado NAO desliga a base: ela segue disponivel e a IA
                // consulta sozinha quando a pergunta pede (IA.Conhecimento.SempreDisponivel).
                $('#btnUsarConhecimentoIA').on('click', function () {
                    usarConhecimentoAtivo = !usarConhecimentoAtivo;
                    $(this)
                        .toggleClass('btn-success', usarConhecimentoAtivo)
                        .toggleClass('btn-default', !usarConhecimentoAtivo)
                        .attr('aria-pressed', usarConhecimentoAtivo ? 'true' : 'false');
                    atualizarStatus(usarConhecimentoAtivo ? 'Modo estrito: so a base de conhecimento' : 'Modo normal: a IA consulta a base quando precisar');
                });
                $('#btnAnexarIA').on('click', function () { $('#fileIAArquivos').val('').trigger('click'); });
                $('#fileIAArquivos').on('change', anexarArquivosSelecionados);
                $('#btnCarregarMaisIA').on('click', function () {
                    paginaConversas++;
                    carregarConversasPagina(true);
                });
                $('#btnTemplatesIA').on('click', function () {
                    var $painel = $('#iaChatTemplates');
                    if ($painel.is(':visible')) {
                        $painel.hide();
                        return;
                    }
                    $painel.show();
                    carregarTemplates();
                });
                $('#btnSalvarTemplateIA').on('click', salvarTemplateAtual);
                $('#txtBuscaConversaIA').on('input', function () {
                    var valor = $.trim($(this).val());
                    window.clearTimeout(buscaConversasTimeout);
                    buscaConversasTimeout = window.setTimeout(function () {
                        termoBuscaConversas = valor;
                        listarConversas();
                    }, 300);
                });
                $('#txtMensagemIA').on('keydown', function (e) {
                    if (e.keyCode === 13 && !e.shiftKey) {
                        e.preventDefault();
                        enviarMensagem();
                    }
                });
            });

            function novaConversa() {
                idConversaAtual = 0;
                $('#iaChatConversas .ia-chat-conversa').removeClass('is-active');
                mostrarEstadoVazio('Nova conversa iniciada.');
                $('#txtMensagemIA').val('').focus();
                arquivosPendentes = [];
                renderizarArquivosPendentes();
                atualizarStatus('Pronto');
            }

            function mostrarEstadoVazio(mensagem) {
                var $vazio = $('<div class="ia-chat-empty"></div>');
                $vazio.append($('<img class="ia-chat-empty-animation" alt="Tina surgindo em uma chama" />').attr('src', '<%= ResolveUrl("~/App/img/tina-v1.gif") %>'));
                $vazio.append($('<div class="ia-chat-empty-copy"></div>').text(mensagem || 'Faça uma pergunta ao assistente.'));
                $vazio.append(renderizarPromptsSugeridos());
                $('#iaChatMessages').empty().append($vazio);
            }

            function renderizarPromptsSugeridos() {
                var $wrap = $('<div class="ia-chat-prompts"></div>');
                $.each(obterPromptsSugeridos(), function (_, prompt) {
                    var $chip = $('<button type="button" class="ia-chat-prompt-chip"></button>').text(prompt);
                    $chip.on('click', function () {
                        $('#txtMensagemIA').val(prompt).focus();
                    });
                    $wrap.append($chip);
                });
                return $wrap;
            }

            function obterPromptsSugeridos() {
                if (contextoTela && promptsPorContexto[contextoTela.tela]) {
                    return promptsPorContexto[contextoTela.tela];
                }

                return promptsSugeridos;
            }

            function obterContextoTela() {
                var match = new RegExp('[?&]contexto=([^&]+)').exec(window.location.search);
                if (!match) return null;

                try {
                    var obj = JSON.parse(decodeURIComponent(match[1].replace(/\+/g, '%20')));
                    if (!obj || !obj.tela) return null;

                    var id = parseInt(obj.id || 0, 10);
                    return {
                        tela: String(obj.tela || '').toUpperCase().substring(0, 50),
                        id: isNaN(id) ? 0 : id,
                        titulo: String(obj.titulo || '').substring(0, 200),
                        resumo: String(obj.resumo || '').substring(0, 600)
                    };
                } catch (e) {
                    return null;
                }
            }

            function renderizarContextoTela() {
                var $ctx = $('#iaChatContexto');
                $ctx.empty();

                if (!contextoTela) {
                    $ctx.hide();
                    return;
                }

                $ctx.append($('<i class="fa fa-crosshairs"></i>'));
                $ctx.append($('<strong></strong>').text('Contexto: ' + (contextoTela.titulo || (contextoTela.tela + ' #' + contextoTela.id))));

                var $fechar = $('<button type="button" title="Remover contexto desta conversa" aria-label="Remover contexto"><i class="fa fa-times"></i></button>');
                $fechar.on('click', function () {
                    contextoTela = null;
                    renderizarContextoTela();
                    if (!idConversaAtual) {
                        mostrarEstadoVazio();
                    }
                    atualizarStatus('Contexto removido');
                });

                $ctx.append($fechar).show();
            }

            function enviarMensagem() {
                if (enviando) return;

                var mensagem = $.trim($('#txtMensagemIA').val());
                if (!mensagem) {
                    atualizarStatus('Informe uma mensagem');
                    return;
                }

                if (possuiArquivoNaoPronto()) {
                    atualizarStatus('Aguarde os arquivos ficarem prontos');
                    return;
                }

                var arquivosMensagem = arquivosPendentes.slice(0);
                var idsArquivos = $.map(arquivosMensagem, function (arquivo) { return arquivo.IdArquivoIA || 0; });
                var urlEnvio = idsArquivos.length ? '/API/IA_Chat.aspx/EnviarMensagemComArquivos' : '/API/IA_Chat.aspx/EnviarMensagem';
                var contextoJson = contextoTela
                    ? JSON.stringify({ tela: contextoTela.tela, id: contextoTela.id, titulo: contextoTela.titulo, resumo: contextoTela.resumo })
                    : '';
                var bUsarConhecimento = usarConhecimentoAtivo ? 'S' : 'N';
                var payload = idsArquivos.length
                    ? { idConversaIA: String(idConversaAtual || 0), sMensagem: mensagem, sArquivosIAJson: JSON.stringify(idsArquivos), sContextoJson: contextoJson, bUsarConhecimento: bUsarConhecimento }
                    : { idConversaIA: String(idConversaAtual || 0), sMensagem: mensagem, sContextoJson: contextoJson, bUsarConhecimento: bUsarConhecimento };

                ultimaMensagemUsuario = mensagem;
                enviando = true;
                $('#txtMensagemIA').val('');
                arquivosPendentes = [];
                renderizarArquivosPendentes();
                limparVazio();
                adicionarMensagem('user', mensagem, [], 0, '', '', arquivosMensagem);
                mostrarDigitando();
                iniciarProgresso();
                atualizarStatus('Processando...');
                $('#btnEnviarIA').prop('disabled', true);

                if (suportaStreaming()) {
                    enviarMensagemStream(urlEnvio, payload);
                    return;
                }

                enviarMensagemAjax(urlEnvio, payload);
            }

            function suportaStreaming() {
                return !!(window.fetch && window.ReadableStream && window.TextDecoder);
            }

            function finalizarEnvio() {
                pararProgresso();
                removerDigitando();
                enviando = false;
                $('#btnEnviarIA').prop('disabled', false);
                atualizarStatus('Pronto');
                $('#txtMensagemIA').focus();
            }

            function tratarRetornoFinal(retorno) {
                if (!retorno) {
                    adicionarMensagem('error', 'Resposta inválida do servidor.');
                    return;
                }

                if (retorno.IdConversaIA) {
                    idConversaAtual = retorno.IdConversaIA;
                }

                if (retorno.Sucesso) {
                    if (retorno.Mensagens && retorno.Mensagens.length) {
                        renderizarMensagens(retorno.Mensagens);
                    } else {
                        adicionarMensagem('assistant', retorno.Resposta || retorno.Mensagem || '', retorno.Fontes || []);
                    }
                    if (retorno.AcaoPendente && (retorno.AcaoPendente.IdAprovacaoIA || retorno.AcaoPendente.IdExecucaoIA)) {
                        renderizarAcaoPendente(retorno.AcaoPendente);
                    }
                    listarConversas();
                } else {
                    adicionarMensagem('error', retorno.Mensagem || 'Não foi possível processar a mensagem.');
                }
            }

            function renderizarAcaoPendente(acao) {
                var $msg = $('<div class="ia-chat-message assistant ia-chat-acao"></div>');

                var tipoAcao = String(acao.Tipo || '').toUpperCase();
                var ehAprovacaoWorkflow = tipoAcao === 'WORKFLOW_APROVACAO';
                var ehEsperaWorkflow = tipoAcao === 'WORKFLOW_ESPERA';
                var ehEntradasWorkflow = tipoAcao === 'WORKFLOW_ENTRADAS';
                var ehEscolhaWorkflow = tipoAcao === 'WORKFLOW_ESCOLHA';
                var ehSelecaoMultiplaWorkflow = tipoAcao === 'WORKFLOW_SELECAO_MULTIPLA';
                var ehWorkflow = tipoAcao === 'WORKFLOW' || ehAprovacaoWorkflow || ehEsperaWorkflow || ehEntradasWorkflow || ehEscolhaWorkflow || ehSelecaoMultiplaWorkflow || !!acao.IdExecucaoIA;
                var tituloFerramenta = ehWorkflow ? (acao.Ferramenta || acao.Resumo) : (acao.Ferramenta || acao.Resumo);
                var descricaoAcao = $.trim(String(acao.Descricao || acao.Resumo || ''));
                $msg.append($('<div class="ia-chat-acao-titulo"><i class="fa fa-exclamation-circle"></i> ' + ((ehEscolhaWorkflow || ehSelecaoMultiplaWorkflow) ? 'Escolha necessária' : (ehEntradasWorkflow ? 'Workflow aguardando dados' : (ehEsperaWorkflow ? 'Workflow aguardando' : 'Confirmação necessária'))) + '</div>'));
                var $linhaFerramenta = $('<p></p>').text((ehEscolhaWorkflow || ehSelecaoMultiplaWorkflow) ? 'Etapa: ' : (ehEntradasWorkflow ? 'Nó: ' : (ehAprovacaoWorkflow ? 'Aprovação: ' : (ehEsperaWorkflow ? 'Espera: ' : 'Ferramenta: '))));
                $linhaFerramenta.append($('<strong></strong>').text(tituloFerramenta || 'ferramenta'));
                $msg.append($linhaFerramenta);
                if (descricaoAcao && descricaoAcao !== (tituloFerramenta || '')) {
                    $msg.append($('<p class="ia-chat-acao-descricao"></p>').text(descricaoAcao));
                }

                var $botoes = $('<div class="ia-chat-acao-botoes"></div>');
                // btn-success: este botao GRAVA (executa a acao WRITE aprovada). btn-primary e de busca.
                var $confirmar = $('<button type="button" class="btn btn-success btn-sm"><i class="fa fa-check"></i> ' + ((ehEscolhaWorkflow || ehSelecaoMultiplaWorkflow) ? 'Selecionar e continuar' : (ehEntradasWorkflow ? 'Continuar workflow' : (ehAprovacaoWorkflow ? 'Aprovar' : (ehEsperaWorkflow ? 'Continuar' : 'Confirmar')))) + '</button>');
                var $rejeitar = $('<button type="button" class="btn btn-warning btn-sm"><i class="fa fa-times"></i> Rejeitar</button>');
                var $cancelar = $('<button type="button" class="btn btn-default btn-sm"><i class="fa fa-times"></i> Cancelar</button>');
                var dadosEntradas = {};

                function jsonSeguro(texto) {
                    try { return JSON.parse(texto || '{}') || {}; }
                    catch (e) { return {}; }
                }

                function converterEntrada(valor, tipo) {
                    tipo = String(tipo || 'string');
                    if (tipo === 'integer') return parseInt(valor || '0', 10) || 0;
                    if (tipo === 'number') return parseFloat(String(valor || '0').replace(',', '.')) || 0;
                    return valor || '';
                }

                function renderizarCamposEntradas() {
                    dadosEntradas = jsonSeguro(acao.DadosJson || '{}');
                    var campos = dadosEntradas.campos || [];
                    if (!ehEntradasWorkflow || !campos.length) return null;

                    var $wrap = $('<div class="ia-chat-acao-campos"></div>');
                    if (dadosEntradas.mensagem) {
                        $wrap.append($('<p class="text-muted"></p>').text(dadosEntradas.mensagem));
                    }

                    var grupoAtual = '';
                    campos.forEach(function (campo, idx) {
                        var grupo = String(campo.grupo || 'Geral');
                        if (grupo !== grupoAtual) {
                            grupoAtual = grupo;
                            $wrap.append($('<div class="ia-chat-acao-grupo"></div>').text(grupoAtual));
                        }

                        var $campo = $('<div class="ia-chat-acao-campo"></div>');
                        var $label = $('<label></label>').text(campo.nome || '');
                        if (campo.obrigatorio) $label.append(' *');
                        var opcoesCampo = campo.opcoes || [];
                        var $input;
                        if (opcoesCampo.length) {
                            $input = $('<select class="form-control input-sm ia-chat-entrada-runtime"></select>')
                                .attr('data-i', idx)
                                .append($('<option></option>').val('').text('Selecione...'));
                            $.each(opcoesCampo, function (_, opcao) {
                                var valorOpcao = opcao && typeof opcao === 'object' ? opcao.valor : opcao;
                                var rotuloOpcao = opcao && typeof opcao === 'object' ? opcao.rotulo : opcao;
                                $input.append($('<option></option>').val(valorOpcao || '').text(rotuloOpcao || valorOpcao || ''));
                            });
                        } else {
                            $input = $('<input type="text" class="form-control input-sm ia-chat-entrada-runtime" />')
                                .attr('data-i', idx)
                                .attr('placeholder', campo.param || campo.nome || '');
                        }
                        $campo.append($label).append($input);
                        if (campo.descricao) $campo.append($('<span class="help-block"></span>').text(campo.descricao));
                        $wrap.append($campo);
                    });

                    return $wrap;
                }

                var $camposEntradas = renderizarCamposEntradas();
                if ($camposEntradas) $msg.append($camposEntradas);

                function renderizarOpcoesEscolha() {
                    dadosEntradas = jsonSeguro(acao.DadosJson || '{}');
                    var opcoes = dadosEntradas.opcoes || [];
                    if ((!ehEscolhaWorkflow && !ehSelecaoMultiplaWorkflow) || !opcoes.length) return null;
                    var $wrap = $('<div class="ia-chat-acao-campos ia-chat-escolhas"></div>');
                    if (dadosEntradas.mensagemUsuario) {
                        $wrap.append($('<p class="text-muted"></p>').text(dadosEntradas.mensagemUsuario));
                    }
                    $.each(opcoes, function (indice, opcao) {
                        var id = 'iaEscolhaWorkflow_' + String(acao.IdExecucaoIA || 0) + '_' + indice;
                        var $linha = $('<label class="ia-chat-escolha"></label>').attr('for', id);
                        var $radio = $('<input />')
                            .attr('type', ehSelecaoMultiplaWorkflow ? 'checkbox' : 'radio')
                            .attr('name', ehSelecaoMultiplaWorkflow ? 'iaSelecaoMultiplaWorkflow' : 'iaEscolhaWorkflow')
                            .attr('id', id)
                            .val('__indice__:' + indice);
                        var $texto = $('<span></span>');
                        $texto.append($('<strong></strong>').text(opcao.rotulo || opcao.valor || 'Opção'));
                        if (opcao.descricao) $texto.append($('<small></small>').text(opcao.descricao));
                        $linha.append($radio).append($texto);
                        $wrap.append($linha);
                    });
                    return $wrap;
                }

                var $opcoesEscolha = renderizarOpcoesEscolha();
                if ($opcoesEscolha) $msg.append($opcoesEscolha);

                // Confirmação de escrita: mostra o que será gravado, em nomes (não em ids), quando o servidor enviou o resumo.
                function renderizarResumoConfirmacao() {
                    if (ehEscolhaWorkflow || ehSelecaoMultiplaWorkflow || ehEntradasWorkflow || ehAprovacaoWorkflow || ehEsperaWorkflow) return null;
                    var linhas = jsonSeguro(acao.DadosJson || '{}').resumoConfirmacao || [];
                    if (!linhas.length) return null;

                    var $resumo = $('<div class="ia-chat-acao-resumo"></div>');
                    $resumo.append($('<div class="ia-chat-acao-resumo-titulo"></div>').text('O que será gravado'));
                    var $lista = $('<dl></dl>');
                    $.each(linhas, function (_, linha) {
                        $lista.append($('<dt></dt>').text(linha.rotulo || ''));
                        $lista.append($('<dd></dd>').text(linha.valor || ''));
                    });
                    return $resumo.append($lista);
                }

                var $resumoConfirmacao = renderizarResumoConfirmacao();
                if ($resumoConfirmacao) $msg.append($resumoConfirmacao);

                function entradasRuntimeJson() {
                    var campos = dadosEntradas.campos || [];
                    var novas = {};
                    var $invalido = null;
                    $msg.find('.ia-chat-entrada-runtime').each(function () {
                        var $input = $(this);
                        var idx = parseInt($input.attr('data-i') || '0', 10);
                        var campo = campos[idx] || {};
                        var nome = String(campo.nome || '');
                        var valor = $.trim($input.val());
                        if (!nome) return;
                        if (campo.obrigatorio && !valor && !$invalido) {
                            $invalido = $input;
                            return;
                        }
                        novas[nome] = converterEntrada(valor, campo.tipo || 'string');
                    });
                    if ($invalido) {
                        $invalido.focus();
                        atualizarStatus('Preencha as entradas obrigatórias para continuar.');
                        return null;
                    }
                    return JSON.stringify(novas);
                }

                function processar(url, entradasJson, valorEscolha) {
                    $confirmar.prop('disabled', true);
                    $rejeitar.prop('disabled', true);
                    $cancelar.prop('disabled', true);
                    atualizarStatus('Processando ação...');

                    var payload = ehWorkflow
                        ? { idExecucaoIA: String(acao.IdExecucaoIA || 0), idConversaIA: String(idConversaAtual || 0) }
                        : { idAprovacaoIA: String(acao.IdAprovacaoIA || 0) };
                    if (entradasJson != null) payload.entradasJson = entradasJson;
                    if (valorEscolha != null) payload.valor = valorEscolha;

                    $.ajax({
                        url: url,
                        data: JSON.stringify(payload),
                        type: 'POST',
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        success: function (response) {
                            var retorno = response && response.d ? response.d : null;
                            var ok = retorno && retorno.Sucesso;

                            $botoes.remove();

                            // READ e WRITE confirmados devolvem resposta formatada e mensagens persistidas;
                            // reaproveita o render normal, removendo o card antes de recarregar a conversa.
                            if (ok && retorno.AcaoPendente && (retorno.AcaoPendente.IdAprovacaoIA || retorno.AcaoPendente.IdExecucaoIA)) {
                                $msg.remove();
                                renderizarAcaoPendente(retorno.AcaoPendente);
                            } else if (ok && (retorno.Resposta || (retorno.Mensagens && retorno.Mensagens.length))) {
                                $msg.remove();
                                tratarRetornoFinal(retorno);
                            } else {
                                $msg.append($('<p></p>').append($('<strong></strong>').text((retorno && retorno.Mensagem) || 'Ação processada.')));
                                if (!ok) {
                                    $msg.addClass('is-erro');
                                }
                            }

                            atualizarStatus(ok ? 'Ação processada' : 'Falha na ação');
                            rolarFim();
                        },
                        error: function (xhr) {
                            $confirmar.prop('disabled', false);
                            $rejeitar.prop('disabled', false);
                            $cancelar.prop('disabled', false);
                            atualizarStatus(extrairErroAjax(xhr));
                        }
                    });
                }

                $confirmar.on('click', function () {
                    if (ehEscolhaWorkflow || ehSelecaoMultiplaWorkflow) {
                        var valorEscolha;
                        if (ehSelecaoMultiplaWorkflow) {
                            var valoresEscolhidos = [];
                            $msg.find('input[name="iaSelecaoMultiplaWorkflow"]:checked').each(function () { valoresEscolhidos.push($(this).val()); });
                            valorEscolha = JSON.stringify(valoresEscolhidos);
                        } else {
                            valorEscolha = $msg.find('input[name="iaEscolhaWorkflow"]:checked').val();
                        }
                        if (!ehSelecaoMultiplaWorkflow && !valorEscolha) {
                            atualizarStatus('Selecione uma opção para continuar.');
                            return;
                        }
                        processar('/API/IA_Chat.aspx/SelecionarOpcaoWorkflow', null, valorEscolha);
                        return;
                    }
                    if (ehEntradasWorkflow) {
                        var entradasJson = entradasRuntimeJson();
                        if (entradasJson == null) return;
                        processar('/API/IA_Chat.aspx/InformarEntradasWorkflow', entradasJson);
                        return;
                    }
                    processar(ehWorkflow ? '/API/IA_Chat.aspx/ConfirmarWorkflow' : '/API/IA_Chat.aspx/ConfirmarAcao');
                });
                $rejeitar.on('click', function () { processar('/API/IA_Chat.aspx/RejeitarWorkflow'); });
                $cancelar.on('click', function () { processar(ehWorkflow ? '/API/IA_Chat.aspx/CancelarWorkflow' : '/API/IA_Chat.aspx/CancelarAcao'); });

                $botoes.append($confirmar);
                if (ehAprovacaoWorkflow) {
                    $botoes.append($rejeitar);
                }
                $botoes.append($cancelar);
                $msg.append($botoes);
                $('#iaChatMessages').append($msg);
                rolarFim();
            }

            function enviarMensagemAjax(urlEnvio, payload) {
                $.ajax({
                    url: urlEnvio,
                    data: JSON.stringify(payload),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        removerDigitando();
                        tratarRetornoFinal(response && response.d ? response.d : null);
                    },
                    error: function (xhr) {
                        removerDigitando();
                        adicionarMensagem('error', extrairErroAjax(xhr));
                    },
                    complete: function () {
                        finalizarEnvio();
                    }
                });
            }

            function enviarMensagemStream(urlEnvio, payload) {
                var recebeuEvento = false;
                var streamEncerrado = false;
                var $bolhaStream = null;
                var textoStream = '';

                function garantirBolha() {
                    if (!$bolhaStream) {
                        removerDigitando();
                        $bolhaStream = $('<div class="ia-chat-message assistant"></div>');
                        $('#iaChatMessages').append($bolhaStream);
                    }
                    return $bolhaStream;
                }

                function limparBolha() {
                    if ($bolhaStream) {
                        $bolhaStream.remove();
                        $bolhaStream = null;
                    }
                    textoStream = '';
                }

                function tratarEvento(evento) {
                    recebeuEvento = true;
                    var tipo = evento ? evento.t : '';
                    var valor = evento ? evento.v : null;

                    if (tipo === 'status') {
                        var textoStatus = String(valor || 'Processando...');
                        atualizarStatus(textoStatus);
                        atualizarProgresso(textoStatus);
                    } else if (tipo === 'ferramenta') {
                        limparBolha();
                        mostrarDigitando();
                        var textoFerramenta = 'Consultando ' + String(valor || 'ferramenta') + '...';
                        atualizarStatus(textoFerramenta);
                        atualizarProgresso(textoFerramenta);
                    } else if (tipo === 'delta') {
                        textoStream += String(valor || '');
                        garantirBolha().text(textoStream);
                        rolarFim();
                    } else if (tipo === 'erro') {
                        streamEncerrado = true;
                        limparBolha();
                        removerDigitando();
                        adicionarMensagem('error', String(valor || 'Erro no processamento.'));
                    } else if (tipo === 'fim') {
                        streamEncerrado = true;
                        limparBolha();
                        removerDigitando();
                        tratarRetornoFinal(valor || null);
                    }
                }

                fetch('/API/IA_Stream.aspx', {
                    method: 'POST',
                    headers: { 'Content-Type': 'application/json; charset=utf-8' },
                    credentials: 'same-origin',
                    body: JSON.stringify(payload)
                }).then(function (resp) {
                    if (!resp.ok || !resp.body) {
                        throw new Error('HTTP ' + resp.status);
                    }

                    var reader = resp.body.getReader();
                    var decoder = new TextDecoder('utf-8');
                    var buffer = '';

                    function processarTexto(texto) {
                        buffer += texto;
                        var linhas = buffer.split('\n');
                        buffer = linhas.pop();

                        for (var i = 0; i < linhas.length; i++) {
                            var linha = $.trim(linhas[i]);
                            if (!linha) continue;

                            var evento = null;
                            try { evento = JSON.parse(linha); } catch (e) { evento = null; }
                            if (evento) tratarEvento(evento);
                        }
                    }

                    function bombear() {
                        return reader.read().then(function (parte) {
                            if (parte.done) {
                                processarTexto(decoder.decode());
                                return null;
                            }

                            processarTexto(decoder.decode(parte.value, { stream: true }));
                            return bombear();
                        });
                    }

                    return bombear();
                }).then(function () {
                    if (!streamEncerrado) {
                        limparBolha();
                        adicionarMensagem('error', 'A resposta foi interrompida antes do fim. Tente novamente.');
                    }
                    finalizarEnvio();
                }).catch(function () {
                    if (streamEncerrado) {
                        finalizarEnvio();
                        return;
                    }

                    if (recebeuEvento) {
                        limparBolha();
                        adicionarMensagem('error', 'Conexão perdida durante a resposta. Tente novamente.');
                        finalizarEnvio();
                        return;
                    }

                    // Endpoint de stream indisponível: cai para o fluxo AJAX tradicional
                    enviarMensagemAjax(urlEnvio, payload);
                });
            }

            function carregarTemplates() {
                $.ajax({
                    url: '/API/IA_Chat.aspx/ListarTemplates',
                    data: '{}',
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        var retorno = response && response.d ? response.d : null;
                        renderizarTemplates(retorno && retorno.Templates ? retorno.Templates : []);
                    },
                    error: function (xhr) {
                        atualizarStatus(extrairErroAjax(xhr));
                    }
                });
            }

            function renderizarTemplates(templates) {
                var $lista = $('#iaChatTemplatesLista');
                $lista.empty();

                if (!templates.length) {
                    $lista.html('<div class="ia-chat-templates-vazio">Nenhum template salvo. Digite um prompt, informe um titulo abaixo e salve.</div>');
                    return;
                }

                $.each(templates, function (_, template) {
                    var $item = $('<div class="ia-chat-template-item"></div>');
                    var $usar = $('<button type="button" class="ia-chat-template-usar"></button>')
                        .attr('title', (template.Conteudo || '').substring(0, 300));
                    $usar.append(document.createTextNode(template.Titulo || 'Template'));
                    if (template.ContextoTela) {
                        $usar.append($('<small></small>').text(' [' + template.ContextoTela + ']'));
                    }
                    $usar.on('click', function () {
                        $('#txtMensagemIA').val(template.Conteudo || '').focus();
                        $('#iaChatTemplates').hide();
                    });

                    var $excluir = $('<button type="button" class="ia-chat-template-del" title="Excluir template" aria-label="Excluir template"><i class="fa fa-times"></i></button>');
                    $excluir.on('click', function () {
                        if (!window.confirm('Excluir o template "' + (template.Titulo || '') + '"?')) return;
                        $.ajax({
                            url: '/API/IA_Chat.aspx/DeletarTemplate',
                            data: JSON.stringify({ idTemplatePromptIA: String(template.IdTemplatePromptIA) }),
                            type: 'POST',
                            dataType: 'json',
                            contentType: 'application/json; charset=utf-8',
                            success: function (response) {
                                var retorno = response && response.d ? response.d : null;
                                renderizarTemplates(retorno && retorno.Templates ? retorno.Templates : []);
                                atualizarStatus('Template removido');
                            },
                            error: function (xhr) {
                                atualizarStatus(extrairErroAjax(xhr));
                            }
                        });
                    });

                    $item.append($usar).append($excluir);
                    $lista.append($item);
                });
            }

            function salvarTemplateAtual() {
                var titulo = $.trim($('#txtTituloTemplateIA').val());
                var conteudo = $.trim($('#txtMensagemIA').val());

                if (!titulo) {
                    atualizarStatus('Informe um titulo para o template');
                    return;
                }

                if (!conteudo) {
                    atualizarStatus('Digite o texto do prompt antes de salvar');
                    return;
                }

                $.ajax({
                    url: '/API/IA_Chat.aspx/SalvarTemplate',
                    data: JSON.stringify({ sTitulo: titulo, sConteudo: conteudo, sContextoTela: contextoTela ? contextoTela.tela : '' }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        var retorno = response && response.d ? response.d : null;
                        if (retorno && retorno.Sucesso) {
                            $('#txtTituloTemplateIA').val('');
                            renderizarTemplates(retorno.Templates || []);
                            atualizarStatus('Template salvo');
                        } else {
                            atualizarStatus((retorno && retorno.Mensagem) || 'Nao foi possivel salvar o template');
                        }
                    },
                    error: function (xhr) {
                        atualizarStatus(extrairErroAjax(xhr));
                    }
                });
            }

            function mostrarDigitando() {
                removerDigitando();
                // O passo em andamento fica na região "status" (lida por leitor de tela); o cronômetro, que muda todo
                // segundo, fica fora dela (aria-hidden) para não ser anunciado a cada tick.
                $('#iaChatMessages').append('<div id="iaChatDigitando" class="ia-chat-typing" role="status" aria-label="Assistente processando"><span></span><span></span><span></span><em id="iaChatDigitandoTexto"></em><em id="iaChatDigitandoTempo" class="ia-chat-typing-tempo" aria-hidden="true"></em></div>');
                renderizarProgresso();
                rolarFim();
            }

            function removerDigitando() {
                $('#iaChatDigitando').remove();
            }

            // Andamento enquanto o assistente trabalha: o passo atual (eventos "status"/"ferramenta" do stream, inclusive
            // cada passo de um workflow) e, passados 5 s, o tempo decorrido.
            function iniciarProgresso() {
                pararProgresso();
                progressoTexto = '';
                progressoInicio = new Date().getTime();
                progressoTimer = window.setInterval(renderizarProgresso, 1000);
            }

            function pararProgresso() {
                if (progressoTimer) {
                    window.clearInterval(progressoTimer);
                    progressoTimer = null;
                }
                progressoTexto = '';
                progressoInicio = 0;
            }

            function atualizarProgresso(texto) {
                progressoTexto = texto || '';
                renderizarProgresso();
            }

            function renderizarProgresso() {
                var $texto = $('#iaChatDigitandoTexto');
                if (!$texto.length) return;

                var segundos = progressoInicio ? Math.floor((new Date().getTime() - progressoInicio) / 1000) : 0;
                $texto.text(progressoTexto);
                $('#iaChatDigitandoTempo').text(segundos >= 5 ? segundos + ' s' : '');
            }

            function listarConversas() {
                paginaConversas = 1;
                carregarConversasPagina(false);
            }

            function carregarConversasPagina(anexar) {
                $.ajax({
                    url: '/API/IA_Chat.aspx/ListarConversasPaginado',
                    data: JSON.stringify({ nPagina: String(paginaConversas), sTermo: termoBuscaConversas || '' }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        var retorno = response && response.d ? response.d : null;
                        var conversas = retorno && retorno.Conversas ? retorno.Conversas : [];
                        renderizarConversas(conversas, anexar);
                        $('#btnCarregarMaisIA').toggle(conversas.length >= 30);
                    }
                });
            }

            function carregarConversa(idConversa) {
                idConversaAtual = idConversa;
                atualizarStatus('Carregando...');

                $.ajax({
                    url: '/API/IA_Chat.aspx/CarregarConversa',
                    data: JSON.stringify({ idConversaIA: String(idConversa) }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        var retorno = response && response.d ? response.d : null;
                        var mensagens = retorno && retorno.Mensagens ? retorno.Mensagens : [];
                        renderizarMensagens(mensagens);
                        if (retorno && retorno.AcaoPendente && retorno.AcaoPendente.IdExecucaoIA) {
                            renderizarAcaoPendente(retorno.AcaoPendente);
                        }
                    },
                    complete: function () {
                        atualizarStatus('Pronto');
                    }
                });
            }

            function renderizarConversas(conversas, anexar) {
                var $lista = $('#iaChatConversas');

                if (!anexar) {
                    $lista.empty();
                }

                if (!conversas.length && !anexar) {
                    $lista.html('<div class="ia-chat-empty" style="margin:24px 12px;">' + (termoBuscaConversas ? 'Nenhuma conversa encontrada.' : 'Nenhuma conversa registrada.') + '</div>');
                    return;
                }

                $.each(conversas, function (_, conversa) {
                    var $item = $('<div class="ia-chat-conversa-item"></div>');
                    var $btn = $('<button type="button" class="ia-chat-conversa"></button>');
                    $btn.toggleClass('is-active', conversa.IdConversaIA === idConversaAtual);
                    $btn.append($('<span></span>').text(conversa.Titulo || 'Conversa IA'));
                    $btn.append($('<small></small>').text(conversa.DtUltimaMensagem || ''));
                    $btn.on('click', function () { carregarConversa(conversa.IdConversaIA); });

                    var $acoes = $('<div class="ia-chat-conversa-acoes"></div>');
                    var tituloFavorita = conversa.Favorita ? 'Remover dos favoritos' : 'Fixar como favorita';
                    var $favorita = $('<button type="button" class="ia-chat-conversa-acao"><i class="fa"></i></button>')
                        .attr('title', tituloFavorita)
                        .attr('aria-label', tituloFavorita)
                        .toggleClass('is-favorita', !!conversa.Favorita);
                    $favorita.find('i').addClass(conversa.Favorita ? 'fa-star' : 'fa-star-o');
                    $favorita.on('click', function (e) {
                        e.stopPropagation();
                        favoritarConversa(conversa);
                    });

                    var $remover = $('<button type="button" class="ia-chat-conversa-acao" title="Remover conversa" aria-label="Remover conversa"><i class="fa fa-trash-o"></i></button>');
                    $remover.on('click', function (e) {
                        e.stopPropagation();
                        deletarConversa(conversa);
                    });

                    $acoes.append($favorita).append($remover);
                    $item.append($btn).append($acoes);
                    $lista.append($item);
                });
            }

            function favoritarConversa(conversa) {
                $.ajax({
                    url: '/API/IA_Chat.aspx/FavoritarConversa',
                    data: JSON.stringify({ idConversaIA: String(conversa.IdConversaIA), sFavorita: conversa.Favorita ? 'N' : 'S' }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        var retorno = response && response.d ? response.d : null;
                        if (retorno && retorno.Sucesso) {
                            atualizarStatus(conversa.Favorita ? 'Conversa removida dos favoritos' : 'Conversa fixada como favorita');
                            listarConversas();
                        } else {
                            atualizarStatus((retorno && retorno.Mensagem) || 'Nao foi possivel atualizar favorito');
                        }
                    },
                    error: function (xhr) {
                        atualizarStatus(extrairErroAjax(xhr));
                    }
                });
            }

            function deletarConversa(conversa) {
                if (!window.confirm('Remover a conversa "' + (conversa.Titulo || 'Conversa IA') + '"? Ela deixara de aparecer na lista.')) {
                    return;
                }

                $.ajax({
                    url: '/API/IA_Chat.aspx/DeletarConversa',
                    data: JSON.stringify({ idConversaIA: String(conversa.IdConversaIA) }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        var retorno = response && response.d ? response.d : null;
                        if (retorno && retorno.Sucesso) {
                            atualizarStatus('Conversa removida');
                            if (conversa.IdConversaIA === idConversaAtual) {
                                novaConversa();
                            }
                            listarConversas();
                        } else {
                            atualizarStatus((retorno && retorno.Mensagem) || 'Nao foi possivel remover a conversa');
                        }
                    },
                    error: function (xhr) {
                        atualizarStatus(extrairErroAjax(xhr));
                    }
                });
            }

            function renderizarMensagens(mensagens) {
                $('#iaChatMessages').empty();

                if (!mensagens.length) {
                    mostrarEstadoVazio('Nenhuma mensagem nesta conversa.');
                    return;
                }

                $.each(mensagens, function (_, mensagem) {
                    var papel = String(mensagem.Papel || '').toLowerCase();
                    if (papel === 'user') {
                        ultimaMensagemUsuario = mensagem.Conteudo || '';
                    }
                    adicionarMensagem(
                        papel === 'user' ? 'user' : 'assistant',
                        mensagem.Conteudo || '',
                        mensagem.Fontes || [],
                        mensagem.IdMensagemIA || 0,
                        mensagem.Avaliacao || '',
                        mensagem.AvaliacaoComentario || '',
                        mensagem.Arquivos || []
                    );
                });
            }

            function adicionarMensagem(tipo, texto, fontes, idMensagemIA, avaliacao, comentarioAvaliacao, arquivos) {
                limparVazio();

                var classe = tipo === 'user' ? 'user' : (tipo === 'error' ? 'error' : 'assistant');
                var $msg = $('<div></div>').addClass('ia-chat-message ' + classe);

                if (classe === 'user') {
                    $msg.text(texto || '');
                } else {
                    $msg.html(renderizarMarkdownSeguro(texto || ''));
                }

                if (classe === 'user' && arquivos && arquivos.length) {
                    $msg.append(renderizarArquivosMensagem(arquivos));
                }

                if (classe === 'assistant' && fontes && fontes.length) {
                    $msg.append(renderizarFontes(fontes));
                }

                if (classe === 'assistant' && idMensagemIA) {
                    $msg.append(renderizarAvaliacao(idMensagemIA, avaliacao || '', comentarioAvaliacao || ''));
                }

                if (classe === 'assistant' && $.trim(texto || '')) {
                    $('#iaChatMessages .ia-chat-msg-refazer').remove();
                    $msg.append(renderizarFerramentasMensagem(texto || ''));
                }

                $('#iaChatMessages').append($msg);
                rolarFim();
            }

            function renderizarFerramentasMensagem(textoOriginal) {
                var $wrap = $('<div class="ia-chat-msg-tools"></div>');

                var $copiar = $('<button type="button" class="ia-chat-msg-tool" title="Copiar resposta" aria-label="Copiar resposta"><i class="fa fa-clipboard"></i> Copiar</button>');
                $copiar.on('click', function () {
                    copiarTextoResposta(textoOriginal, $copiar);
                });

                var $refazer = $('<button type="button" class="ia-chat-msg-tool ia-chat-msg-refazer" title="Refazer resposta" aria-label="Refazer resposta"><i class="fa fa-refresh"></i> Refazer</button>');
                $refazer.on('click', refazerUltimaResposta);

                $wrap.append($copiar).append($refazer);
                return $wrap;
            }

            function copiarTextoResposta(texto, $btn) {
                function feedback(ok) {
                    $btn.html(ok ? '<i class="fa fa-check"></i> Copiado' : '<i class="fa fa-times"></i> Falhou');
                    window.setTimeout(function () {
                        $btn.html('<i class="fa fa-clipboard"></i> Copiar');
                    }, 2000);
                }

                if (navigator.clipboard && navigator.clipboard.writeText) {
                    navigator.clipboard.writeText(texto).then(
                        function () { feedback(true); },
                        function () { feedback(copiarFallback(texto)); }
                    );
                    return;
                }

                feedback(copiarFallback(texto));
            }

            function copiarFallback(texto) {
                var area = document.createElement('textarea');
                area.value = texto;
                area.style.position = 'fixed';
                area.style.opacity = '0';
                document.body.appendChild(area);
                area.select();

                var ok = false;
                try {
                    ok = document.execCommand('copy');
                } catch (e) {
                    ok = false;
                }

                document.body.removeChild(area);
                return ok;
            }

            function refazerUltimaResposta() {
                if (enviando) return;

                if (!ultimaMensagemUsuario) {
                    atualizarStatus('Nenhuma pergunta para refazer');
                    return;
                }

                $('#txtMensagemIA').val(ultimaMensagemUsuario);
                enviarMensagem();
            }

            function anexarArquivosSelecionados(e) {
                var files = e.target.files || [];

                $.each(files, function (_, file) {
                    if (arquivosPendentes.length >= maxArquivosMensagem) {
                        atualizarStatus('Limite de ' + maxArquivosMensagem + ' arquivos por mensagem');
                        return false;
                    }

                    var item = {
                        Uid: String(new Date().getTime()) + String(Math.random()).replace('.', ''),
                        IdArquivoIA: 0,
                        NomeOriginal: file.name,
                        TamanhoFormatado: formatarBytesCliente(file.size),
                        Status: 'ENVIANDO',
                        Erro: ''
                    };

                    arquivosPendentes.push(item);
                    renderizarArquivosPendentes();
                    uploadArquivoIA(file, item);
                });
            }

            function uploadArquivoIA(file, item) {
                var formData = new FormData();
                formData.append('arquivo', file);
                formData.append('idConversaIA', String(idConversaAtual || 0));
                atualizarStatus('Enviando arquivo...');

                $.ajax({
                    url: '/API/IA_ArquivoUpload.aspx',
                    data: formData,
                    type: 'POST',
                    dataType: 'json',
                    processData: false,
                    contentType: false,
                    success: function (retorno) {
                        var arquivo = retorno && retorno.Arquivo ? retorno.Arquivo : null;

                        if (arquivo) {
                            atualizarItemArquivo(item.Uid, arquivo);
                            if (arquivo.Status === 'CONVERTENDO' || arquivo.Status === 'ENVIADO' || arquivo.Status === 'PROCESSANDO') {
                                consultarArquivoDepois(item.Uid, arquivo.IdArquivoIA, 1);
                            }
                        } else {
                            atualizarItemArquivo(item.Uid, { Status: 'ERRO_CONVERSAO', Erro: (retorno && retorno.Mensagem) || 'Nao foi possivel anexar arquivo.' });
                        }

                        atualizarStatus((retorno && retorno.Mensagem) || 'Arquivo enviado');
                    },
                    error: function (xhr) {
                        atualizarItemArquivo(item.Uid, { Status: 'ERRO_CONVERSAO', Erro: extrairErroAjax(xhr) });
                        atualizarStatus('Erro ao enviar arquivo');
                    }
                });
            }

            function consultarArquivoDepois(uid, idArquivoIA, tentativa) {
                if (!arquivoAindaPendente(uid) || tentativa > 60) return;

                window.setTimeout(function () {
                    if (!arquivoAindaPendente(uid)) return;

                    $.ajax({
                        url: '/API/IA_Chat.aspx/ConsultarArquivo',
                        data: JSON.stringify({ idArquivoIA: String(idArquivoIA), idConversaIA: String(idConversaAtual || 0) }),
                        type: 'POST',
                        dataType: 'json',
                        contentType: 'application/json; charset=utf-8',
                        success: function (response) {
                            var retorno = response && response.d ? response.d : null;
                            var arquivo = retorno && retorno.Arquivo ? retorno.Arquivo : null;
                            if (arquivo) {
                                atualizarItemArquivo(uid, arquivo);
                                if (arquivo.Status === 'CONVERTENDO' || arquivo.Status === 'ENVIADO' || arquivo.Status === 'PROCESSANDO') {
                                    consultarArquivoDepois(uid, idArquivoIA, tentativa + 1);
                                }
                            }
                        }
                    });
                }, 2000);
            }

            function atualizarItemArquivo(uid, arquivo) {
                $.each(arquivosPendentes, function (_, item) {
                    if (item.Uid === uid) {
                        item.IdArquivoIA = arquivo.IdArquivoIA || item.IdArquivoIA || 0;
                        item.NomeOriginal = arquivo.NomeOriginal || item.NomeOriginal || '';
                        item.TamanhoFormatado = arquivo.TamanhoFormatado || item.TamanhoFormatado || '';
                        item.Status = arquivo.Status || item.Status || '';
                        item.Erro = arquivo.Erro || '';
                        item.HashSHA256 = arquivo.HashSHA256 || item.HashSHA256 || '';
                    }
                });

                renderizarArquivosPendentes();
            }

            function arquivoAindaPendente(uid) {
                var existe = false;
                $.each(arquivosPendentes, function (_, item) {
                    if (item.Uid === uid) {
                        existe = true;
                    }
                });
                return existe;
            }

            function possuiArquivoNaoPronto() {
                var bloqueado = false;
                $.each(arquivosPendentes, function (_, arquivo) {
                    if (arquivo.Status !== 'PRONTO') {
                        bloqueado = true;
                    }
                });
                return bloqueado;
            }

            function renderizarArquivosPendentes() {
                var $wrap = $('#iaChatArquivos');
                $wrap.empty();

                $.each(arquivosPendentes, function (_, arquivo) {
                    var $item = renderizarChipArquivo(arquivo, true);
                    $wrap.append($item);
                });
            }

            function renderizarArquivosMensagem(arquivos) {
                var $wrap = $('<div class="ia-chat-message-attachments"></div>');
                $.each(arquivos, function (_, arquivo) {
                    $wrap.append(renderizarChipArquivo(arquivo, false));
                });
                return $wrap;
            }

            function renderizarChipArquivo(arquivo, removivel) {
                var status = textoStatusArquivo(arquivo.Status || '');
                var $item = $('<span class="ia-chat-attachment"></span>').addClass(classeStatusArquivo(arquivo.Status || ''));
                $item.append($('<i class="fa fa-file-text-o"></i>'));
                $item.append($('<strong></strong>').text(arquivo.NomeOriginal || 'arquivo'));
                $item.append($('<small></small>').text(status + (arquivo.TamanhoFormatado ? ' | ' + arquivo.TamanhoFormatado : '')));

                if (arquivo.Erro) {
                    $item.attr('title', arquivo.Erro);
                }

                if (removivel) {
                    var $remover = $('<button type="button" class="ia-chat-attachment-remove" title="Remover arquivo" aria-label="Remover arquivo"><i class="fa fa-times"></i></button>');
                    $remover.on('click', function () {
                        arquivosPendentes = $.grep(arquivosPendentes, function (item) { return item.Uid !== arquivo.Uid; });
                        renderizarArquivosPendentes();
                        atualizarStatus('Arquivo removido');
                    });
                    $item.append($remover);
                }

                return $item;
            }

            function textoStatusArquivo(status) {
                status = String(status || '').toUpperCase();
                if (status === 'PRONTO') return 'Pronto';
                if (status === 'CONVERTENDO') return 'Convertendo';
                if (status === 'PROCESSANDO') return 'Processando';
                if (status === 'ERRO_CONVERSAO') return 'Erro';
                if (status === 'ENVIANDO') return 'Enviando';
                if (status === 'ENVIADO') return 'Enviado';
                return status || 'Arquivo';
            }

            function classeStatusArquivo(status) {
                status = String(status || '').toUpperCase();
                if (status === 'PRONTO') return 'is-ready';
                if (status === 'ERRO_CONVERSAO') return 'is-error';
                return '';
            }

            function formatarBytesCliente(bytes) {
                bytes = parseInt(bytes || 0, 10);
                if (!bytes) return '0 KB';
                if (bytes < 1024 * 1024) return Math.max(1, Math.round(bytes / 1024)) + ' KB';
                return (bytes / (1024 * 1024)).toFixed(2).replace(/\.00$/, '') + ' MB';
            }

            function renderizarAvaliacao(idMensagemIA, avaliacao, comentarioAvaliacao) {
                var $wrap = $('<div class="ia-chat-feedback"></div>')
                    .attr('data-id-mensagem', idMensagemIA)
                    .attr('data-avaliacao-atual', avaliacao || '');
                var $label = $('<span></span>').text('Esta resposta ajudou?');
                var $util = $('<button type="button" title="Resposta util" aria-label="Resposta util"><i class="fa fa-thumbs-o-up"></i></button>').attr('data-avaliacao', 'UTIL');
                var $naoUtil = $('<button type="button" title="Resposta nao util" aria-label="Resposta nao util"><i class="fa fa-thumbs-o-down"></i></button>').attr('data-avaliacao', 'NAO_UTIL');
                var $comentario = $('<div class="ia-chat-feedback-comment"></div>');
                var $textarea = $('<textarea class="form-control" maxlength="1000" placeholder="Comentario opcional"></textarea>').val(comentarioAvaliacao || '');
                var $acoes = $('<div class="ia-chat-feedback-actions"></div>');
                var $salvar = $('<button type="button" class="ia-chat-feedback-save" title="Salvar comentario" aria-label="Salvar comentario"><i class="fa fa-check"></i></button>');
                var $cancelar = $('<button type="button" class="ia-chat-feedback-cancel" title="Cancelar" aria-label="Cancelar"><i class="fa fa-times"></i></button>');

                $acoes.append($salvar).append($cancelar);
                $comentario.append($textarea).append($acoes);
                $wrap.append($label).append($util).append($naoUtil).append($comentario);

                atualizarBotoesAvaliacao($wrap, avaliacao);
                if (avaliacao === 'NAO_UTIL' || comentarioAvaliacao) {
                    $wrap.addClass('is-comment-open');
                }

                $wrap.on('click', 'button[data-avaliacao]', function () {
                    var novaAvaliacao = $(this).attr('data-avaliacao');
                    if (novaAvaliacao === 'NAO_UTIL') {
                        $wrap.addClass('is-comment-open');
                        $textarea.focus();
                        if ($wrap.attr('data-avaliacao-atual') !== 'NAO_UTIL') {
                            atualizarStatus('Informe um comentario opcional e salve');
                        }
                        return;
                    }

                    $textarea.val('');
                    avaliarResposta(idMensagemIA, novaAvaliacao, '', $wrap);
                });

                $salvar.on('click', function () {
                    avaliarResposta(idMensagemIA, 'NAO_UTIL', $textarea.val(), $wrap);
                });

                $cancelar.on('click', function () {
                    if ($wrap.attr('data-avaliacao-atual') === 'NAO_UTIL') {
                        $textarea.val(comentarioAvaliacao || '');
                    } else {
                        $wrap.removeClass('is-comment-open');
                        $textarea.val('');
                    }
                });

                return $wrap;
            }

            function avaliarResposta(idMensagemIA, avaliacao, comentario, $wrap) {
                if (!idMensagemIA || !avaliacao) return;

                $wrap.find('button').addClass('is-saving');
                atualizarStatus('Salvando avaliacao...');

                $.ajax({
                    url: '/API/IA_Chat.aspx/AvaliarResposta',
                    data: JSON.stringify({ idMensagemIA: String(idMensagemIA), sAvaliacao: avaliacao, sComentario: comentario || '' }),
                    type: 'POST',
                    dataType: 'json',
                    contentType: 'application/json; charset=utf-8',
                    success: function (response) {
                        var retorno = response && response.d ? response.d : null;

                        if (retorno && retorno.Sucesso) {
                            atualizarBotoesAvaliacao($wrap, avaliacao);
                            $wrap.attr('data-avaliacao-atual', avaliacao);
                            comentarioAvaliacao = comentario || '';
                            if (avaliacao === 'UTIL') {
                                $wrap.removeClass('is-comment-open');
                            }
                            atualizarStatus('Avaliacao salva');
                        } else {
                            atualizarStatus((retorno && retorno.Mensagem) || 'Nao foi possivel salvar avaliacao');
                        }
                    },
                    error: function (xhr) {
                        atualizarStatus(extrairErroAjax(xhr));
                    },
                    complete: function () {
                        $wrap.find('button').removeClass('is-saving');
                    }
                });
            }

            function atualizarBotoesAvaliacao($wrap, avaliacao) {
                $wrap.find('button').removeClass('is-selected');
                if (avaliacao) {
                    $wrap.find('button[data-avaliacao="' + avaliacao + '"]').addClass('is-selected');
                }
            }

            function renderizarFontes(fontes) {
                var $wrap = $('<div class="ia-chat-sources"></div>');
                $wrap.append($('<div class="ia-chat-sources-title"></div>').text('Fontes usadas (' + fontes.length + ')'));

                $.each(fontes, function (_, fonte) {
                    if (fonteEhArquivo(fonte)) {
                        $wrap.append(renderizarFonteArquivo(fonte));
                    } else {
                        $wrap.append(renderizarFontePadrao(fonte));
                    }
                });

                return $wrap;
            }

            function renderizarFontePadrao(fonte) {
                var titulo = fonte.Titulo || fonte.Tipo || fonte.Ferramenta || 'Fonte';
                var resumo = fonte.Resumo || fonte.Ferramenta || '';
                var tipo = fonte.Tipo || '';
                var href = normalizarHref(fonte.Url || '');
                var $item;

                if (hrefPermitido(href)) {
                    $item = $('<a class="ia-chat-source" target="_blank" rel="noopener noreferrer"></a>').attr('href', href);
                } else {
                    $item = $('<span class="ia-chat-source"></span>');
                }

                $item.append($('<strong></strong>').text(tipo ? tipo + ': ' + titulo : titulo));
                if (resumo) {
                    $item.append($('<small></small>').text(resumo));
                }

                return $item;
            }

            function renderizarFonteArquivo(fonte) {
                var titulo = fonte.Titulo || 'Arquivo';
                var idArquivo = inteiroFonte(fonte.IdArquivoIA);
                var idTrecho = inteiroFonte(fonte.IdArquivoTrechoIA);
                var inicioChar = inteiroFonte(fonte.InicioChar);
                var fimChar = inteiroFonte(fonte.FimChar);
                var hash = fonte.HashSHA256 || '';
                var trecho = fonte.Trecho || '';
                var $item = $('<div class="ia-chat-source ia-chat-source-file"></div>');
                var $head = $('<div class="ia-chat-source-head"></div>');
                var $titulo = $('<strong></strong>');
                var metadados = [];

                $titulo.append($('<span class="ia-chat-source-kind"></span>').text('Arquivo'));
                $titulo.append(document.createTextNode(titulo));
                $head.append($titulo);
                $item.append($head);

                if (idArquivo) metadados.push('Arquivo #' + idArquivo);
                if (idTrecho) metadados.push('Trecho #' + idTrecho);
                if (inicioChar || fimChar) metadados.push('Chars ' + inicioChar + '-' + fimChar);
                if (hash) metadados.push('Hash ' + resumirHash(hash));

                if (metadados.length) {
                    $item.append($('<small class="ia-chat-source-meta"></small>').text(metadados.join(' | ')));
                }

                if (fonte.Resumo) {
                    $item.append($('<small></small>').text(fonte.Resumo));
                }

                if (trecho) {
                    var $toggle = $('<button type="button" class="ia-chat-source-toggle"><i class="fa fa-eye"></i> Ver trecho</button>');
                    var $preview = $('<pre class="ia-chat-source-preview"></pre>').text(trecho);

                    $toggle.on('click', function () {
                        var aberto = $item.toggleClass('is-open').hasClass('is-open');
                        $toggle.html(aberto ? '<i class="fa fa-eye-slash"></i> Ocultar trecho' : '<i class="fa fa-eye"></i> Ver trecho');
                    });

                    $item.append($toggle).append($preview);
                }

                return $item;
            }

            function fonteEhArquivo(fonte) {
                return String((fonte && fonte.Tipo) || '').toLowerCase() === 'arquivo'
                    || inteiroFonte(fonte && fonte.IdArquivoIA) > 0
                    || !!(fonte && fonte.Trecho);
            }

            function inteiroFonte(valor) {
                var numero = parseInt(valor || 0, 10);
                return isNaN(numero) ? 0 : numero;
            }

            function resumirHash(hash) {
                hash = String(hash || '');
                return hash.length > 16 ? hash.substring(0, 12) + '...' : hash;
            }

            // Renderizador movido para App/JS/ia-markdown.js para o modal de markdown de
            // Arquivos.aspx usar o mesmo - inclusive a allowlist de href, que barra javascript:.
            // Aliases finos: os pontos de chamada abaixo continuam identicos.
            function renderizarMarkdownSeguro(texto) { return IA_Markdown.renderizar(texto); }
            function normalizarHref(href) { return IA_Markdown.normalizarHref(href); }
            function hrefPermitido(href) { return IA_Markdown.hrefPermitido(href); }

            function limparVazio() {
                $('#iaChatMessages .ia-chat-empty').remove();
            }

            function atualizarStatus(texto) {
                $('#iaChatStatus').text(texto || '');
            }

            function rolarFim() {
                var el = $('#iaChatMessages').get(0);
                if (el) el.scrollTop = el.scrollHeight;
            }

            function extrairErroAjax(xhr) {
                try {
                    if (xhr && xhr.responseJSON && xhr.responseJSON.Message) {
                        return xhr.responseJSON.Message;
                    }

                    if (xhr && xhr.responseText) {
                        return xhr.responseText.substring(0, 500);
                    }
                } catch (e) { }

                return 'Erro ao comunicar com o servidor.';
            }

            function obterParametroInteiro(nome) {
                var match = new RegExp('[?&]' + nome + '=([^&]+)').exec(window.location.search);
                var valor = match ? parseInt(decodeURIComponent(match[1]), 10) : 0;
                return isNaN(valor) ? 0 : valor;
            }
        })();
    </script>
</asp:Content>
