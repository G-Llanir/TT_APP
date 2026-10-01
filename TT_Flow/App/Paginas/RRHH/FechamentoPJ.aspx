<%@ Page Title="Fechamento de Horas" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="FechamentoPJ.aspx.cs" Inherits="TT_Flow.App.Paginas.RRHH.FechamentoPJ" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .timeline-day { border-left: 4px solid #ddd; margin-bottom: 15px; background-color: #fff; }
        .timeline-item { border-bottom: 1px solid #eee; padding: 10px; }
        .timeline-item:last-child { border-bottom: none; }
        
        .tipo-SISTEMA { border-left: 3px solid #777; background-color: #f9f9f9; }
        .tipo-AJUSTE_MANUAL { border-left: 3px solid #5bc0de; background-color: #fff; }
        
        .editavel:hover { background-color: #d9edf7; cursor: pointer; }
        
        .icon-locked { color: #777; }
        .icon-unlocked { color: #337ab7; }

        .modal-confirm-body { font-size: 16px; text-align: center; padding: 30px 20px; }

/* Força o Loading do UpdateProgress (se houver) a ficar ACIMA da modal */
/* Ajuste a classe '.modal-loading' ou o ID do seu PainelAtualizacao se souber o nome exato da div de load global */
.update-progress-div, .modal-backdrop-load { 
    z-index: 100000 !important; 
}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1><i class="fa fa-money"></i> Fechamento PJ</h1>
            <uc1:BreadCrumb ID="BreadCrumb" runat="server" NivelPagina="3" TitulodaPagina="Fechamento de Horas" />
        </div>
    </div>

    <asp:UpdatePanel ID="updGeral" runat="server" UpdateMode="Conditional">
        <ContentTemplate>
            
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            
            <asp:HiddenField ID="hddAcaoConfirmacao" runat="server" />
            <asp:HiddenField ID="hddIdConfirmacao" runat="server" />

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-filter"></i> Filtros e Ações</h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Mês</label>
                                        <asp:DropDownList ID="ddlMes" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>Ano</label>
                                        <asp:DropDownList ID="ddlAno" runat="server" CssClass="form-control"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <label>&nbsp;</label>
                                        <asp:LinkButton ID="btnCarregar" runat="server" CssClass="btn btn-primary btn-block" OnClick="btnCarregar_Click">
                                            <i class="fa fa-refresh"></i> Carregar
                                        </asp:LinkButton>
                                    </div>
                                </div>
                                <div class="col-lg-2 pull-right">
                                     <div class="form-group">
                                        <label>&nbsp;</label>
                                        <button class="btn btn-default btn-block" type="button" onclick="$('#modalHistorico').modal('show');">
                                            <i class="fa fa-history"></i> Histórico
                                        </button>
                                     </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div id="divDashboard" runat="server" visible="false">
                
                <div class="row" style="margin-bottom: 20px;">
                    <div class="col-lg-12">
                         <div class="well">
                             <div class="row">
                                 <div class="col-md-3">
                                     <label>Status</label><br />
                                     <asp:Label ID="lblStatus" runat="server" CssClass="label label-default" style="font-size: 14px;">RASCUNHO</asp:Label>
                                 </div>
                                 <div class="col-md-3">
                                     <label>Total Sistema</label><br />
                                     <asp:Label ID="lblTotalSistema" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label>
                                 </div>
                                 <div class="col-md-3">
                                     <label>Total Ajustes</label><br />
                                     <asp:Label ID="lblTotalAjustes" runat="server" Font-Bold="true" Font-Size="Large"></asp:Label>
                                 </div>
                                 <div class="col-md-3 text-right">
                                     <label>Total Geral</label><br />
                                     <h2 style="margin-top: 0; color: #337ab7;">
                                         <asp:Label ID="lblTotalHoras" runat="server" Text="0,00"></asp:Label> h
                                     </h2>
                                 </div>
                             </div>
                             
                             <div class="row" style="margin-top: 10px;">
                                 <div class="col-md-12 text-right">
                                      <div id="divResetar" runat="server">
                                        <asp:LinkButton ID="btnResetar" runat="server" CssClass="btn btn-danger btn-xs"
                                            OnClientClick="return solicitarConfirmacao('RESETAR', '', 'Isso apagará o rascunho atual e buscará os dados novamente. Confirmar?');">
                                            <i class="fa fa-trash-o"></i> Descartar Rascunho e Recarregar
                                        </asp:LinkButton>
                                    </div>
                                     <div id="divAcompanhar" runat="server" visible="false" style="margin-top: 5px;">
                                        <asp:HyperLink ID="lnkAcompanhar" runat="server" Target="_blank" CssClass="btn btn-info btn-xs">
                                            <i class="fa fa-external-link"></i> Ver Solicitação #<asp:Label ID="lblIdSolicitacaoCard" runat="server"></asp:Label>
                                        </asp:HyperLink>
                                    </div>
                                 </div>
                             </div>
                         </div>
                    </div>
                </div>

                <div class="row" id="divBtnAdicionar" runat="server">
                    <div class="col-lg-12" style="margin-bottom: 10px;">
                        <button type="button" class="btn btn-success btn-block" onclick="abrirModalNovo()">
                            <i class="fa fa-plus"></i> Adicionar Ajuste Manual
                        </button>
                    </div>
                </div>

                <div class="row">
                    <div class="col-lg-12">
                        <asp:HiddenField ID="hddIdItemEdicao" runat="server" Value="0" />
                        
                        <asp:Repeater ID="rptDias" runat="server">
                            <ItemTemplate>
                                <div class="panel panel-default">
                                    <div class="panel-heading clearfix">
                                        <div class="pull-left">
                                            <strong><%# Eval("Dia") %> - <%# Eval("DiaSemana") %></strong>
                                        </div>
                                        <div class="pull-right">
                                            <span class="badge"><%# Eval("TotalHorasDia", "{0:N2}") %>h</span>
                                        </div>
                                    </div>
                                    <div class="panel-body" style="padding: 0;">
                                        <asp:Repeater ID="rptDetalhes" runat="server" DataSource='<%# Eval("Itens") %>' OnItemCommand="rptItens_ItemCommand">
                                            <ItemTemplate>
                                                <div class='timeline-item tipo-<%# Eval("TipoRegistro") %> <%# ValidarEdicao(Eval("TipoRegistro")) ? "editavel" : "" %>'>
                                                    <div class="row">
                                                        <div class="col-md-9">
                                                            <asp:LinkButton ID="btnEditarLinha" runat="server"
                                                                CommandName="EditarAjuste"
                                                                CommandArgument='<%# Eval("IdItem") %>'
                                                                Enabled='<%# ValidarEdicao(Eval("TipoRegistro")) %>'
                                                                Style="text-decoration: none; color: inherit; display: block;">
                                                                
                                                                <div class="media">
                                                                    <div class="media-left">
                                                                         <i class='fa <%# Eval("TipoRegistro").ToString() == "SISTEMA" ? "fa-lock icon-locked" : "fa-unlock icon-unlocked" %> fa-2x'></i>
                                                                    </div>
                                                                    <div class="media-body">
                                                                        <h5 class="media-heading" style="font-weight: bold;"><%# Eval("Titulo") %></h5>
                                                                        <small class="text-muted"><%# Eval("Observacao") %></small>
                                                                         <asp:PlaceHolder runat="server" Visible='<%# ValidarEdicao(Eval("TipoRegistro")) %>'>
                                                                            <span class="label label-info"><i class="fa fa-pencil"></i> Editar</span>
                                                                        </asp:PlaceHolder>
                                                                    </div>
                                                                </div>
                                                            </asp:LinkButton>
                                                        </div>
                                                        <div class="col-md-3 text-right">
                                                            <h4 style="margin: 5px 0;"><%# Eval("Horas", "{0:N2}") %>h</h4>
                                                            
                                                            <asp:LinkButton ID="btnExcluir" runat="server"
                                                                Visible='<%# ValidarEdicao(Eval("TipoRegistro")) %>'
                                                                OnClientClick='<%# "return solicitarConfirmacao(\"EXCLUIR\", \"" + Eval("IdItem") + "\", \"Deseja realmente excluir este ajuste?\");" %>'
                                                                CssClass="btn btn-link btn-xs text-danger"
                                                                ToolTip="Excluir">
                                                                <i class="fa fa-times text-danger"></i> Remover
                                                            </asp:LinkButton>
                                                        </div>
                                                    </div>
                                                </div>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </div>
                                </div>
                            </ItemTemplate>
                        </asp:Repeater>
                    </div>
                </div>
                
                <div id="divBarraFinalizar" runat="server" class="row" visible="false" style="margin-top: 20px; margin-bottom: 40px;">
                    <div class="col-lg-12">
                         <div class="alert alert-warning text-center">
                             <p>Ao finalizar, não será possível realizar mais alterações.</p>
                             <br />
                             <asp:Button ID="btnFinalizar" runat="server" Text="Enviar Fechamento e Gerar Solicitação"
                                CssClass="btn btn-success btn-lg"
                                OnClientClick="return solicitarConfirmacao('FINALIZAR', '', 'Tem certeza que deseja fechar o mês? Não será possível alterar depois.');" />
                         </div>
                    </div>
                </div>

                <asp:HiddenField ID="hddIdFechamento" runat="server" Value="0" />
                <asp:HiddenField ID="hddStatus" runat="server" Value="" />

            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="modal fade" id="modalHistorico" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title"><i class="fa fa-history"></i> Meus Fechamentos</h4>
                </div>
                <div class="modal-body" style="padding: 15px;">
                     <asp:UpdatePanel ID="updMenu" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="table-responsive">
                                <table class="table table-hover table-striped">
                                    <thead>
                                        <tr>
                                            <th>Período</th>
                                            <th>Status</th>
                                            <th>Gerado em</th>
                                            <th class="text-center" style="width: 50px;">Ação</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <asp:Repeater ID="rptHistorico" runat="server" OnItemCommand="rptHistorico_ItemCommand">
                                            <ItemTemplate>
                                                <tr class='<%# Eval("idFechamento").ToString() == hddIdFechamento.Value ? "info" : "" %>'>
                                                    <td style="vertical-align: middle; font-weight: bold;"><%# Eval("TituloExibicao") %></td>
                                                    <td style="vertical-align: middle;">
                                                        <span class='label <%# Eval("sStatus").ToString() == "RASCUNHO" ? "label-warning" : (Eval("sStatus").ToString() == "REJEITADO" ? "label-danger" : "label-success") %>'>
                                                            <%# Eval("sStatus") %>
                                                        </span>
                                                    </td>
                                                    <td style="vertical-align: middle;"><%# Eval("dtGeracao", "{0:dd/MM/yyyy}") %></td>
                                                    <td class="text-center">
                                                        <asp:LinkButton ID="lnkItemHistorico" runat="server"
                                                            CommandName="CarregarFechamento"
                                                            CommandArgument='<%# Eval("idFechamento") + "|" + Eval("sStatus") %>'
                                                            CssClass="btn btn-primary btn-xs" ToolTip="Carregar">
                                                            <i class="fa fa-folder-open"></i>
                                                        </asp:LinkButton>
                                                    </td>
                                                </tr>
                                            </ItemTemplate>
                                        </asp:Repeater>
                                    </tbody>
                                </table>
                            </div>
                            <div runat="server" id="divSemHistorico" visible="false" class="text-center text-muted" style="padding: 20px;">
                                <i class="fa fa-folder-open-o fa-3x"></i><br /><br />Nenhum relatório encontrado.
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalAjuste" tabindex="-1" role="dialog" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title"><i class="fa fa-clock-o"></i> Adicionar/Editar Horas</h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="updModalAjuste" runat="server">
                        <ContentTemplate>
                             <div class="form-group">
                                <label>Data do Ajuste</label>
                                <asp:TextBox ID="txtDataAjuste" runat="server" TextMode="Date" CssClass="form-control"></asp:TextBox>
                            </div>
                            <div class="form-group">
                                <label>Quantidade de Horas</label>
                                <div class="input-group">
                                    <asp:TextBox ID="txtHorasAjuste" runat="server" TextMode="Number" step="0.01" CssClass="form-control" placeholder="Ex: 2,5"></asp:TextBox>
                                    <span class="input-group-addon">h</span>
                                </div>
                            </div>
                            <div class="form-group">
                                <label>Motivo / Observação</label>
                                <asp:TextBox ID="txtObsAjuste" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control"></asp:TextBox>
                            </div>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                     <asp:UpdatePanel ID="updBotoesModal" runat="server">
                        <ContentTemplate>
                            <asp:Button ID="btnExcluirModal" runat="server" Text="Excluir"
                                CssClass="btn btn-danger pull-left"
                                OnClientClick="return solicitarConfirmacao('EXCLUIR', $('#cphCorpo_hddIdItemEdicao').val(), 'Tem certeza que deseja excluir este item?');"
                                Visible="false" />

                            <button type="button" class="btn btn-default" data-dismiss="modal">Cancelar</button>
                            
                            <button type="button" class="btn btn-primary" onclick="return solicitarConfirmacao('SALVAR', '', 'Confirma o salvamento deste ajuste?');">Salvar</button>
                         </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>

    <div class="modal fade" id="modalConfirmacao" tabindex="-1" role="dialog" style="z-index: 99999; background: rgba(0,0,0,0.5);" > 
        <div class="modal-dialog modal-sm" role="document" style="margin-top: 15%;">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title"><i class="fa fa-question-circle"></i> Confirmação</h4>
                </div>
                <div class="modal-body modal-confirm-body">
                    <p id="lblMsgConfirmacao">Tem certeza?</p>
                </div>
                <div class="modal-footer" style="text-align: center;">
                    <asp:UpdatePanel ID="updConfirmacao" runat="server">
                        <ContentTemplate>
                            <button type="button" class="btn btn-default" data-dismiss="modal">Não</button>
                           <asp:LinkButton ID="btnConfirmarAcao" runat="server" 
    CssClass="btn btn-success" 
    OnClick="btnConfirmarAcao_Click" 
    OnClientClick="ativarLoad(this);">
    Sim
</asp:LinkButton>
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>
            </div>
        </div>
    </div>

<script type="text/javascript">
    function abrirModalNovo() {
        $('#<%= txtDataAjuste.ClientID %>').val('');
            $('#<%= txtHorasAjuste.ClientID %>').val('');
            $('#<%= txtObsAjuste.ClientID %>').val('');
            $('#<%= hddIdItemEdicao.ClientID %>').val('0'); 
            
            $('#<%= btnExcluirModal.ClientID %>').hide();
            $('#modalAjuste').modal('show');
        }

        function abrirModalAjuste() {
            $('#modalAjuste').modal('show');
        }

        // FUNÇÃO CENTRAL PARA CHAMAR A MODAL
        function solicitarConfirmacao(acao, id, mensagem) {
            // Garante que o botão esteja "limpo" antes de abrir novamente
            resetarBotaoConfirmacao();

            $('#<%= hddAcaoConfirmacao.ClientID %>').val(acao);
            $('#<%= hddIdConfirmacao.ClientID %>').val(id);
            $('#lblMsgConfirmacao').text(mensagem);

            $('#modalConfirmacao').modal('show');
            return false; // Evita PostBack direto
        }

        // --- CORREÇÃO PRINCIPAL AQUI ---
        function fecharTodasModais() {
            // 1. Esconde as modais visualmente
            $('.modal').modal('hide');
            
            // 2. FORÇA a remoção do backdrop (fundo escuro) que fica órfão
            $('.modal-backdrop').remove();
            
            // 3. Remove a classe que trava o scroll da página principal
            $('body').removeClass('modal-open');
            $('body').css('padding-right', ''); // Remove padding extra do bootstrap
            
            // 4. Reseta o botão visualmente (caso o updatepanel falhe em redesenhar)
            resetarBotaoConfirmacao();
        }

        function ativarLoad(btn) {
            btn.innerHTML = '<i class="fa fa-spinner fa-spin"></i> Processando...';
            btn.classList.add('disabled');
            return true;
        }

        function resetarBotaoConfirmacao() {
            var btn = document.getElementById('<%= btnConfirmarAcao.ClientID %>');
        if (btn) {
            btn.innerHTML = 'Sim';
            btn.classList.remove('disabled');
        }
    }
    </script>
</asp:Content>