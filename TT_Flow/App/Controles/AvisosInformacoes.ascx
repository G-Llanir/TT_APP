<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AvisosInformacoes.ascx.cs" Inherits="TT_Flow.App.Controles.AvisosInformacoes" %>

<style>
    .avisos-informacoes-timeline {
        position: relative;
        padding-left: 40px;
        list-style: none;
        margin: 10px 0 0;
    }

        .avisos-informacoes-timeline::before {
            content: '';
            position: absolute;
            left: 20px;
            top: 0;
            bottom: 0;
            width: 2px;
            background-color: #ddd;
        }

    .avisos-informacoes-timeline .timeline-item {
        position: relative;
        margin-bottom: 16px;
    }

    .avisos-informacoes-timeline .timeline-dot {
        position: absolute;
        left: -29px;
        top: 5px;
        width: 18px;
        height: 18px;
        border-radius: 50%;
        background-color: #fff;
        border: 4px solid #ffc107;
    }

    .avisos-informacoes-timeline .timeline-item.status-warning .timeline-dot {
        border-color: #ffc107;
    }

    .avisos-informacoes-timeline .timeline-content {
        background-color: #f9f9f9;
        padding: 12px;
        border-radius: 5px;
        border: 1px solid #eee;
    }

    .mensagem-card {
        display: grid;
        grid-template-columns: minmax(0, 1fr) auto;
        grid-template-rows: auto auto;
        gap: 10px 12px;
        grid-template-areas:
            "remetente data"
            "link tags";
    }

    .mensagem-card-remetente {
        grid-area: remetente;
        font-weight: 700;
        font-size: 1.05em;
        line-height: 1.3;
        color: #333;
        word-break: break-word;
    }

        .mensagem-card-remetente i {
            margin-right: 6px;
            color: #337ab7;
        }

    .mensagem-card-data {
        grid-area: data;
        text-align: right;
        font-size: 0.82em;
        line-height: 1.3;
        color: #777;
        white-space: nowrap;
    }

        .mensagem-card-data i {
            margin-right: 4px;
            color: #999;
        }

    .mensagem-card-link {
        grid-area: link;
        align-self: end;
        font-size: 0.9em;
        line-height: 1.35;
        word-break: break-word;
    }

    .mensagem-card-tags {
        grid-area: tags;
        align-self: end;
        text-align: right;
        line-height: 1.4;
    }

    .avisos-informacoes-tag {
        display: inline-block;
        margin: 1px 0 1px 4px;
        padding: 2px 7px;
        border-radius: 10px;
        background: #e8eef7;
        color: #2f5d8c;
        font-size: 11px;
    }

    .avisos-informacoes-scroll {
        max-height: 520px;
        overflow-y: auto;
        overflow-x: hidden;
    }

    .avisos-informacoes-vazio {
        text-align: center;
        color: #777;
        padding: 24px 12px;
    }

    .avisos-informacoes-rodape {
        text-align: center;
        padding-top: 8px;
        border-top: 1px solid #eee;
        margin-top: 8px;
    }
</style>

<asp:Panel ID="pnlAvisos" runat="server" CssClass="avisos-informacoes-painel">
    <asp:UpdatePanel ID="upAvisos" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title">
                        <i class="fa fa-bell"></i>
                        <asp:Label ID="lblTituloPainel" runat="server"></asp:Label>
                    </h3>
                </div>
                <div class="panel-body">
                    <asp:Label ID="lblVazio" runat="server" Visible="false" CssClass="avisos-informacoes-vazio"></asp:Label>
                    <asp:Panel ID="pnLista" runat="server" Visible="false" CssClass="avisos-informacoes-scroll">
                        <asp:Repeater ID="rptAvisos" runat="server" OnItemDataBound="rptAvisos_ItemDataBound">
                            <HeaderTemplate>
                                <ul class="avisos-informacoes-timeline">
                            </HeaderTemplate>
                            <ItemTemplate>
                                <li class="timeline-item status-warning">
                                    <div class="timeline-dot"></div>
                                    <div class="timeline-content mensagem-card">
                                        <div class="mensagem-card-remetente">
                                            <i class="fa fa-user" aria-hidden="true"></i>
                                            <%# Eval("sDscUsuarioRemetente") %>
                                        </div>
                                        <div class="mensagem-card-data">
                                            <i class="fa fa-calendar" aria-hidden="true"></i>
                                            <%# FormatarData(Eval("dtInclusao")) %>
                                        </div>
                                        <div class="mensagem-card-link">
                                            <asp:HyperLink ID="lnkAssunto" runat="server" Target="_blank"></asp:HyperLink>
                                        </div>
                                        <asp:Panel ID="pnTagsAviso" runat="server" CssClass="mensagem-card-tags">
                                            <asp:Repeater ID="rptTagsAviso" runat="server">
                                                <ItemTemplate>
                                                    <span class="avisos-informacoes-tag"><%# Container.DataItem %></span>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </asp:Panel>
                                    </div>
                                </li>
                            </ItemTemplate>
                            <FooterTemplate>
                                </ul>
                            </FooterTemplate>
                        </asp:Repeater>
                    </asp:Panel>
                    <asp:Panel ID="pnVerTodos" runat="server" Visible="false" CssClass="avisos-informacoes-rodape">
                        <asp:HyperLink ID="lnkVerTodos" runat="server" Target="_self"></asp:HyperLink>
                    </asp:Panel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Panel>
