<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Auditoria.aspx.cs" Inherits="TT_Flow.App.Paginas.IA.Auditoria" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
        /* Texto longo/JSON precisa quebrar e ser monoespacado. */
        .ia-auditoria-json {
            max-width: 420px;
            white-space: pre-wrap;
            word-break: break-word;
            font-family: Consolas, "Courier New", monospace;
            font-size: 11px;
        }

        .ia-auditoria-json-modal {
            max-height: 65vh;
            overflow: auto;
            white-space: pre-wrap;
            word-break: break-word;
            font-size: 12px;
        }

        /* Cores de linha por severidade (Eventos) e status (Chamadas). Reforca o contextual do
           bootstrap para vencer o zebra do table-striped. */
        .table > tbody > tr.danger > td { background-color: #f2dede; }
        .table > tbody > tr.warning > td { background-color: #fcf8e3; }
        .table > tbody > tr.success > td { background-color: #dff0d8; }
        .table > tbody > tr.info > td { background-color: #d9edf7; }
    </style>

    <asp:UpdatePanel ID="updAuditoriaIA" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="Auditoria IA"></asp:Label><small> Governança</small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Auditoria IA" />
                </div>

                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </div>

                <div class="col-lg-12">
                    <ul class="nav nav-tabs" id="abasAuditoria">
                        <li class="active"><a href="#abaEventos" data-toggle="tab"><i class="fa fa-shield"></i> Eventos</a></li>
                        <li><a href="#abaChamadas" data-toggle="tab"><i class="fa fa-plug"></i> Chamadas de ferramenta</a></li>
                        <li><a href="#abaAvaliacoes" data-toggle="tab"><i class="fa fa-thumbs-o-up"></i> Avaliações de respostas</a></li>
                    </ul>
                </div>

                <div class="col-lg-12">
                    <div class="tab-content" style="padding-top:15px;">

                        <%-- ================= ABA EVENTOS ================= --%>
                        <div class="tab-pane active" id="abaEventos">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><i class="fa fa-filter"></i> Filtro dos eventos</h3>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-2 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Data inicial</label>
                                                <asp:TextBox ID="txtEvtDtIni" CssClass="form-control CaixaTextoData" runat="server" MaxLength="10" />
                                            </div>
                                        </div>
                                        <div class="col-lg-2 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Data final</label>
                                                <asp:TextBox ID="txtEvtDtFim" CssClass="form-control CaixaTextoData" runat="server" MaxLength="10" />
                                            </div>
                                        </div>
                                        <div class="col-lg-2 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Usuário</label>
                                                <asp:DropDownList ID="ddlEvtUsuario" CssClass="form-control Caixa_Selecao" runat="server" />
                                            </div>
                                        </div>
                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Evento</label>
                                                <asp:DropDownList ID="ddlEvtEvento" CssClass="form-control Caixa_Selecao" runat="server" />
                                            </div>
                                        </div>
                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Severidade</label>
                                                <asp:DropDownList ID="ddlEvtSeveridade" CssClass="form-control" runat="server">
                                                    <asp:ListItem Text="Todas" Value="" />
                                                    <asp:ListItem Text="Informativo" Value="INFO" />
                                                    <asp:ListItem Text="Alerta" Value="ALERTA" />
                                                    <asp:ListItem Text="Aviso (warn)" Value="WARN" />
                                                    <asp:ListItem Text="Erro" Value="ERRO" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-6 col-md-8 col-sm-12">
                                            <div class="form-group">
                                                <label>Buscar (em toda a tabela)</label>
                                                <asp:TextBox ID="txtEvtBusca" CssClass="form-control" runat="server" MaxLength="200" placeholder="texto, IP, origem, payload, usuário..." />
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-md-4 col-sm-12">
                                            <div class="form-group">
                                                <label>&nbsp;</label>
                                                <div>
                                                    <asp:Button ID="cmdEvtBuscar" CssClass="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdEvtBuscar_Click" />
                                                    <asp:Button ID="cmdEvtLimpar" CssClass="btn btn-default" runat="server" Text="Limpar" OnClick="cmdEvtLimpar_Click" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvAuditoria" runat="server" Width="100%" AutoGenerateColumns="False"
                                            CellPadding="0" CellSpacing="1" GridLines="None" ShowFooter="False"
                                            EmptyDataText="Nenhum evento localizado para os filtros informados."
                                            CssClass="table table-striped table-bordered table-hover table-condensed table-responsive tablesorter"
                                            Font-Names="Tahoma" Font-Size="Small" OnRowDataBound="dtgvAuditoria_RowDataBound">
                                            <Columns>
                                                <asp:BoundField DataField="dtEvento" HeaderText="Data" />
                                                <asp:HyperLinkField DataNavigateUrlFields="idConversaIA" DataTextField="idConversaIA" HeaderText="Conversa" DataNavigateUrlFormatString="Chat.aspx?id={0}" />
                                                <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário" />
                                                <asp:BoundField DataField="sEvento" HeaderText="Evento" />
                                                <asp:BoundField DataField="sSeveridade" HeaderText="Severidade" />
                                                <asp:BoundField DataField="sIP" HeaderText="IP" />
                                                <asp:BoundField DataField="sURLOrigem" HeaderText="Origem" />
                                                <asp:TemplateField HeaderText="Payload" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("sPayloadJson"))) %>'>
                                                            <button type="button" class="btn btn-default btn-xs" title="Ver payload" data-toggle="modal" data-target="#Modal_Json" onclick="IA_verJson(this, 'Payload')"><i class="fa fa-eye"></i></button>
                                                            <span class="invisivel ia-json-fonte"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("sPayloadJson") %>' /></span>
                                                        </asp:PlaceHolder>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%-- ================= ABA CHAMADAS ================= --%>
                        <div class="tab-pane" id="abaChamadas">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><i class="fa fa-filter"></i> Filtro das chamadas de ferramenta</h3>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-2 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Data inicial</label>
                                                <asp:TextBox ID="txtChmDtIni" CssClass="form-control CaixaTextoData" runat="server" MaxLength="10" />
                                            </div>
                                        </div>
                                        <div class="col-lg-2 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Data final</label>
                                                <asp:TextBox ID="txtChmDtFim" CssClass="form-control CaixaTextoData" runat="server" MaxLength="10" />
                                            </div>
                                        </div>
                                        <div class="col-lg-2 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Usuário</label>
                                                <asp:DropDownList ID="ddlChmUsuario" CssClass="form-control Caixa_Selecao" runat="server" />
                                            </div>
                                        </div>
                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Ferramenta</label>
                                                <asp:DropDownList ID="ddlChmFerramenta" CssClass="form-control Caixa_Selecao" runat="server" />
                                            </div>
                                        </div>
                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Status</label>
                                                <asp:DropDownList ID="ddlChmStatus" CssClass="form-control" runat="server">
                                                    <asp:ListItem Text="Todos" Value="" />
                                                    <asp:ListItem Text="Sucesso" Value="SUCESSO" />
                                                    <asp:ListItem Text="Erro" Value="ERRO" />
                                                    <asp:ListItem Text="Negado" Value="NEGADO" />
                                                    <asp:ListItem Text="Pendente de confirmação" Value="PENDENTE_CONFIRMACAO" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-6 col-md-8 col-sm-12">
                                            <div class="form-group">
                                                <label>Buscar (em toda a tabela)</label>
                                                <asp:TextBox ID="txtChmBusca" CssClass="form-control" runat="server" MaxLength="200" placeholder="ferramenta, status, erro, argumentos, resultado, usuário..." />
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-md-4 col-sm-12">
                                            <div class="form-group">
                                                <label>&nbsp;</label>
                                                <div>
                                                    <asp:Button ID="cmdChmBuscar" CssClass="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdChmBuscar_Click" />
                                                    <asp:Button ID="cmdChmLimpar" CssClass="btn btn-default" runat="server" Text="Limpar" OnClick="cmdChmLimpar_Click" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvChamadas" runat="server" Width="100%" AutoGenerateColumns="False"
                                            CellPadding="0" CellSpacing="1" GridLines="None" ShowFooter="False"
                                            EmptyDataText="Nenhuma chamada de ferramenta localizada para os filtros informados."
                                            CssClass="table table-striped table-bordered table-hover table-condensed table-responsive tablesorter"
                                            Font-Names="Tahoma" Font-Size="Small" OnRowDataBound="dtgvChamadas_RowDataBound">
                                            <Columns>
                                                <asp:BoundField DataField="dtSolicitacao" HeaderText="Data" />
                                                <asp:HyperLinkField DataNavigateUrlFields="idConversaIA" DataTextField="idConversaIA" HeaderText="Conversa" DataNavigateUrlFormatString="Chat.aspx?id={0}" />
                                                <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário" />
                                                <asp:BoundField DataField="sNomeFerramenta" HeaderText="Ferramenta" />
                                                <asp:BoundField DataField="sStatus" HeaderText="Status" />
                                                <asp:BoundField DataField="nDuracaoMs" HeaderText="ms" />
                                                <asp:TemplateField HeaderText="Argumentos" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("sArgumentosJson"))) %>'>
                                                            <button type="button" class="btn btn-default btn-xs" title="Ver argumentos" data-toggle="modal" data-target="#Modal_Json" onclick="IA_verJson(this, 'Argumentos')"><i class="fa fa-eye"></i></button>
                                                            <span class="invisivel ia-json-fonte"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("sArgumentosJson") %>' /></span>
                                                        </asp:PlaceHolder>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Resultado" ItemStyle-HorizontalAlign="Center">
                                                    <ItemTemplate>
                                                        <asp:PlaceHolder runat="server" Visible='<%# !string.IsNullOrWhiteSpace(Convert.ToString(Eval("sResultadoJson"))) %>'>
                                                            <button type="button" class="btn btn-default btn-xs" title="Ver resultado" data-toggle="modal" data-target="#Modal_Json" onclick="IA_verJson(this, 'Resultado')"><i class="fa fa-eye"></i></button>
                                                            <span class="invisivel ia-json-fonte"><asp:Literal runat="server" Mode="Encode" Text='<%# Eval("sResultadoJson") %>' /></span>
                                                        </asp:PlaceHolder>
                                                    </ItemTemplate>
                                                </asp:TemplateField>
                                                <asp:BoundField DataField="sErro" HeaderText="Erro" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%-- ================= ABA AVALIACOES ================= --%>
                        <div class="tab-pane" id="abaAvaliacoes">
                            <div class="panel panel-primary">
                                <div class="panel-heading">
                                    <h3 class="panel-title"><i class="fa fa-filter"></i> Filtro das avaliações</h3>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-lg-2 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Data inicial</label>
                                                <asp:TextBox ID="txtAvlDtIni" CssClass="form-control CaixaTextoData" runat="server" MaxLength="10" />
                                            </div>
                                        </div>
                                        <div class="col-lg-2 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Data final</label>
                                                <asp:TextBox ID="txtAvlDtFim" CssClass="form-control CaixaTextoData" runat="server" MaxLength="10" />
                                            </div>
                                        </div>
                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Usuário</label>
                                                <asp:DropDownList ID="ddlAvlUsuario" CssClass="form-control Caixa_Selecao" runat="server" />
                                            </div>
                                        </div>
                                        <div class="col-lg-3 col-md-3 col-sm-6">
                                            <div class="form-group">
                                                <label>Avaliação</label>
                                                <asp:DropDownList ID="ddlAvlAvaliacao" CssClass="form-control" runat="server">
                                                    <asp:ListItem Text="Todas" Value="" />
                                                    <asp:ListItem Text="Útil" Value="UTIL" />
                                                    <asp:ListItem Text="Não útil" Value="NAO_UTIL" />
                                                </asp:DropDownList>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-lg-6 col-md-8 col-sm-12">
                                            <div class="form-group">
                                                <label>Buscar (em toda a tabela)</label>
                                                <asp:TextBox ID="txtAvlBusca" CssClass="form-control" runat="server" MaxLength="200" placeholder="comentário, título, resposta, usuário..." />
                                            </div>
                                        </div>
                                        <div class="col-lg-6 col-md-4 col-sm-12">
                                            <div class="form-group">
                                                <label>&nbsp;</label>
                                                <div>
                                                    <asp:Button ID="cmdAvlBuscar" CssClass="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdAvlBuscar_Click" />
                                                    <asp:Button ID="cmdAvlLimpar" CssClass="btn btn-default" runat="server" Text="Limpar" OnClick="cmdAvlLimpar_Click" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvAvaliacoes" runat="server" Width="100%" AutoGenerateColumns="False"
                                            CellPadding="0" CellSpacing="1" GridLines="None" ShowFooter="False"
                                            EmptyDataText="Nenhuma avaliação localizada para os filtros informados."
                                            CssClass="table table-striped table-bordered table-hover table-condensed table-responsive tablesorter"
                                            Font-Names="Tahoma" Font-Size="Small" OnRowDataBound="dtgvAvaliacoes_RowDataBound">
                                            <Columns>
                                                <asp:BoundField DataField="dtAvaliacao" HeaderText="Data" />
                                                <asp:HyperLinkField DataNavigateUrlFields="idConversaIA" DataTextField="idConversaIA" HeaderText="Conversa" DataNavigateUrlFormatString="Chat.aspx?id={0}" />
                                                <asp:BoundField DataField="sDscUsuario" HeaderText="Usuário" />
                                                <asp:BoundField DataField="sAvaliacaoDsc" HeaderText="Avaliação" />
                                                <asp:BoundField DataField="sTituloConversa" HeaderText="Título da conversa" />
                                                <asp:BoundField DataField="sConteudoResumo" HeaderText="Resposta" />
                                                <asp:BoundField DataField="sComentario" HeaderText="Comentário" />
                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <%-- Fora do UpdatePanel de proposito: dentro, o postback parcial recria a marcacao e o modal
         perde o estado do bootstrap. So markup estatico aqui; o conteudo vem por JS. --%>
    <div class="modal fade" id="Modal_Json">
        <div class="modal-dialog modal-grande">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-title">
                        <label id="lblJsonTitulo">Detalhe</label>
                        <button type="button" class="close" data-dismiss="modal"><span aria-hidden="true">&times;</span></button>
                    </div>
                </div>
                <div class="modal-body">
                    <pre id="preJsonCorpo" class="ia-auditoria-json-modal"></pre>
                </div>
            </div>
        </div>
    </div>

    <script type="text/javascript">
        // Enche o modal de JSON (quem ABRE e o data-api do bootstrap). Ver comentario historico abaixo.
        function IA_verJson(botao, titulo) {
            var fonte = botao.parentNode.querySelector('.ia-json-fonte');
            var texto = fonte ? (fonte.textContent || '').trim() : '';

            if (!texto) {
                texto = '(vazio)';
            } else {
                try { texto = JSON.stringify(JSON.parse(texto), null, 2); } catch (e) { }
            }

            document.getElementById('lblJsonTitulo').textContent = titulo;
            document.getElementById('preJsonCorpo').textContent = texto;
        }

        // Mantem a aba ativa entre os postbacks parciais do UpdatePanel (Buscar/Limpar de cada aba).
        (function () {
            var CHAVE = 'audIA_abaAtiva';
            function ligarAbas() {
                if (!window.jQuery) { return; }
                var $ = window.jQuery;
                var $abas = $('#abasAuditoria a[data-toggle="tab"]');
                if (!$abas.length) { return; }

                $abas.off('shown.bs.tab.audia').on('shown.bs.tab.audia', function (e) {
                    try { sessionStorage.setItem(CHAVE, $(e.target).attr('href')); } catch (x) { }
                });

                var alvo = null;
                try { alvo = sessionStorage.getItem(CHAVE); } catch (x) { }
                if (alvo && $('#abasAuditoria a[href="' + alvo + '"]').length) {
                    $('#abasAuditoria a[href="' + alvo + '"]').tab('show');
                }
            }

            if (window.jQuery) { window.jQuery(document).ready(ligarAbas); }
            if (window.Sys && window.Sys.WebForms) {
                window.Sys.WebForms.PageRequestManager.getInstance().add_endRequest(ligarAbas);
            }
        })();
    </script>
</asp:Content>
