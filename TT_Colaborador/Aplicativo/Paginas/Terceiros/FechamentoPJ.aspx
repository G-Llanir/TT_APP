<%@ Page Title="Fechamento de Horas" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="FechamentoPJ.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Financeiro.FechamentoPJ" %>

<%@ Register Src="~/Aplicativo/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        /* --- ESTILOS GERAIS --- */
        .resumo-card {
            background: linear-gradient(135deg, #0d6efd 0%, #0a58ca 100%);
            color: white;
            border-radius: 15px;
            box-shadow: 0 4px 15px rgba(13, 110, 253, 0.3);
            margin-bottom: 20px;
            border: none;
        }

        /* Container que centraliza no PC */
        .timeline-container {
            max-width: 900px;
            margin: 0 auto;
        }

        .item-card {
            border: 1px solid #f0f0f0;
            border-radius: 12px;
            background: #fff;
            box-shadow: 0 2px 5px rgba(0,0,0,0.05);
            margin-bottom: 10px;
            transition: transform 0.2s;
            border-left: 5px solid transparent;
            position: relative;
            overflow: hidden;
        }

            .item-card:hover {
                transform: translateY(-2px);
                box-shadow: 0 4px 12px rgba(0,0,0,0.1);
            }

            .item-card.tipo-SISTEMA {
                border-left-color: #6c757d;
            }

            .item-card.tipo-AJUSTE_MANUAL {
                border-left-color: #0dcaf0;
            }

        /* Flexbox Helpers */
        .data-box {
            min-width: 65px;
            text-align: center;
            padding-right: 15px;
            border-right: 1px solid #eee;
            margin-right: 15px;
            flex-shrink: 0;
        }

        .conteudo-box {
            flex-grow: 1;
            min-width: 0;
        }

        .badge-hora {
            font-size: 1rem;
            font-weight: 700;
            padding: 6px 10px;
            border-radius: 6px;
            white-space: nowrap;
        }

        /* Botões Flutuantes e Fixos */
        .fab-container {
            position: fixed;
            bottom: 30px;
            right: 30px;
            z-index: 999;
        }

        .btn-fab {
            width: 60px;
            height: 60px;
            border-radius: 50%;
            background-color: #198754;
            color: white;
            box-shadow: 0 4px 10px rgba(0,0,0,0.3);
            border: none;
            font-size: 24px;
            display: flex;
            align-items: center;
            justify-content: center;
            transition: transform 0.2s;
        }

            .btn-fab:hover {
                transform: scale(1.1);
                background-color: #157347;
            }

        .barra-inferior {
            position: fixed;
            bottom: 0;
            left: 0;
            right: 0;
            background: rgba(255,255,255,0.95);
            border-top: 1px solid #dee2e6;
            padding: 15px 0;
            z-index: 990;
            backdrop-filter: blur(5px);
        }

        .status-pill {
            padding: 5px 15px;
            border-radius: 50px;
            font-size: 0.85rem;
            font-weight: bold;
            text-transform: uppercase;
        }

        .modal-header-custom {
            background-color: #f8f9fa;
            border-bottom: 1px solid #eee;
        }

        /* Remove estilo de link do botão-card */
        .btn-card-invisible {
            text-decoration: none !important;
            color: inherit !important;
            display: flex;
            align-items: center;
            width: 100%;
            border: none;
            background: none;
            padding: 0;
            text-align: left;
        }

            .btn-card-invisible:hover {
                color: inherit !important;
                opacity: 0.8; /* Leve efeito visual ao passar o mouse */
            }
        /* Cursor */
        .cursor-pointer {
            cursor: pointer;
        }

        .cursor-default {
            cursor: default;
        }

        /* NOVO: Efeito de Hover para as linhas da lista */
        .row-item-container {
            transition: all 0.2s ease-in-out;
            border-left: 4px solid transparent; /* Indicador visual lateral */
        }

        /* Estilo para itens do SISTEMA (Cinza/Padrão) */
        .row-item-container.tipo-SISTEMA {
            border-left-color: #dee2e6;
            background-color: #fdfdfd;
        }

        /* Estilo para itens de AJUSTE MANUAL (Azul/Destaque) */
        .row-item-container.tipo-AJUSTE_MANUAL {
            border-left-color: #0dcaf0; /* Ciano do Bootstrap */
            background-color: #fff;
        }

        /* Efeito HOVER apenas se tiver a classe 'editavel' */
        .row-item-container.editavel:hover {
            transform: translateX(5px); /* Move levemente para a direita */
            background-color: #f0f8ff; /* Fundo azul bem claro */
            box-shadow: -2px 2px 8px rgba(0,0,0,0.05);
            cursor: pointer;
        }

        /* Remove a aparência padrão de botão do LinkButton para ele agir como container */
        .btn-block-link {
            text-decoration: none !important;
            color: inherit !important;
            display: block;
            width: 100%;
        }

        /* Novo CSS para o ícone do cadeado */
.icon-status {
    width: 42px;
    height: 42px;
    border-radius: 50%;
    display: flex;
    align-items: center;
    justify-content: center;
    font-size: 1.2rem;
    transition: all 0.3s ease;
}

/* Cores para Sistema (Bloqueado) */
.icon-status.locked {
    background-color: #e9ecef; /* Cinza claro */
    color: #6c757d;            /* Cinza escuro */
    border: 1px solid #dee2e6;
}

/* Cores para Ajuste (Aberto/Editável) */
.icon-status.unlocked {
    background-color: #e7f1ff; /* Azul bem clarinho */
    color: #0d6efd;            /* Azul Bootstrap */
    border: 1px solid #b6d4fe;
}

/* Efeito extra ao passar o mouse na linha editável */
.row-item-container.editavel:hover .icon-status.unlocked {
    background-color: #0d6efd;
    color: white;
    transform: scale(1.1);
}
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <div class="card-body">
        <div id="div_solicitacoes" runat="server">
            <div class="card mb-4 shadow-sm">
                <div class="card-header card-tt d-flex justify-content-between align-items-center">

                    <h5 class="card-title text-white m-0">
                        <i class="fa fa-file-invoice-dollar me-2"></i>Fechamento PJ
    </h5>

                    <button class="btn btn-sm btn-outline-light" type="button" data-bs-toggle="offcanvas" data-bs-target="#offcanvasHistorico">
                        <i class="fa fa-history me-1"></i>Histórico
   
                    </button>

                </div>

                <div class="card-body bg-light">
                    <asp:UpdatePanel ID="updGeral" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>

                            <div class="row g-2 mb-4 align-items-end justify-content-center">
                                <div class="col-6 col-md-3">
                                    <label class="small text-muted fw-bold">Mês</label>
                                    <asp:DropDownList ID="ddlMes" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>
                                <div class="col-6 col-md-3">
                                    <label class="small text-muted fw-bold">Ano</label>
                                    <asp:DropDownList ID="ddlAno" runat="server" CssClass="form-select"></asp:DropDownList>
                                </div>
                                <div class="col-12 col-md-3 d-grid">
                                    <asp:LinkButton ID="btnCarregar" runat="server" CssClass="btn btn-primary" OnClick="btnCarregar_Click">
                                        <i class="fa fa-sync-alt me-2"></i>Carregar
                                    </asp:LinkButton>
                                </div>
                            </div>

                            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

                            <div id="divDashboard" runat="server" visible="false" class="timeline-container">

                                <div class="d-flex justify-content-end mb-2" id="divResetar" runat="server">
                                    <asp:LinkButton ID="btnResetar" runat="server" CssClass="text-danger small text-decoration-none fw-bold"
                                        OnClick="btnResetar_Click" OnClientClick="return abrirConfirmacao('Isso apagará o rascunho atual e buscará os dados novamente. Confirmar?', this.name);">
                                        <i class="fa fa-trash me-1"></i>Descartar e Recarregar Rascunho
                                    </asp:LinkButton>
                                </div>

                                <div class="card resumo-card p-4">
                                    <div class="row align-items-center">
                                        <div class="col-6">
                                            <small class="opacity-75 text-uppercase" style="letter-spacing: 1px">Status</small><br />
                                            <asp:Label ID="lblStatus" runat="server" CssClass="status-pill bg-white text-primary">RASCUNHO</asp:Label>
                                        </div>
                                        <div class="col-6 text-end">
                                            <small class="opacity-75 text-uppercase" style="letter-spacing: 1px">Total Horas</small><br />
                                            <h1 class="m-0 fw-bold display-6">
                                                <asp:Label ID="lblTotalHoras" runat="server" Text="0,00"></asp:Label>
                                            </h1>
                                        </div>
                                    </div>
                                    <div class="mt-3 pt-3 border-top border-white border-opacity-25 d-flex justify-content-between small">
                                        <span><i class="fa fa-laptop me-1"></i>Sistema:
                                            <asp:Label ID="lblTotalSistema" runat="server" Font-Bold="true"></asp:Label></span>
                                        <span><i class="fa fa-pen me-1"></i>Ajustes:
                                            <asp:Label ID="lblTotalAjustes" runat="server" Font-Bold="true"></asp:Label></span>
                                    </div>

                                    <div id="divAcompanhar" runat="server" visible="false" class="mt-3 text-center">
                                        <asp:HyperLink ID="lnkAcompanhar" runat="server" Target="_blank" CssClass="btn btn-sm btn-light fw-bold w-100 text-primary shadow-sm">
                                            <i class="fa fa-external-link-alt me-2"></i>Acompanhar Solicitação #<asp:Label ID="lblIdSolicitacaoCard" runat="server"></asp:Label>
                                        </asp:HyperLink>
                                    </div>

                                </div>

                                <h6 class="text-muted mb-3 mt-4 ps-1 text-uppercase small fw-bold">Timeline de Atividades</h6>
                                <asp:HiddenField ID="hddIdItemEdicao" runat="server" Value="0" />


                               <asp:Repeater ID="rptDias" runat="server">
            <ItemTemplate>
                <div class="card mb-3 border-0 shadow-sm">
                    
                    <div class="card-header bg-white d-flex justify-content-between align-items-center py-3 border-bottom">
                        <div class="d-flex align-items-center">
                            <div class="bg-light rounded text-center py-1 px-2 me-3 border">
                                <div class="fw-bold fs-5 lh-1 text-dark"><%# Eval("Dia") %></div>
                                <div class="small fw-bold text-muted" style="font-size: 0.65rem"><%# Eval("DiaSemana") %></div>
                            </div>
                            <div>
                                <div class="small text-muted text-uppercase fw-bold" style="font-size: 0.7rem;">Total do Dia</div>
                                <h6 class="m-0 fw-bold text-primary"><%# Eval("TotalHorasDia", "{0:N2}") %>h</h6>
                            </div>
                        </div>
                        <div>
                            <span class="badge bg-light text-dark border">
                                <%# ((System.Collections.Generic.List<ItemFechamento>)Eval("Itens")).Count %> apon.
                            </span>
                        </div>
                    </div>

                    <div class="card-body p-0">
                        <ul class="list-group list-group-flush">
                            
                          <asp:Repeater ID="rptDetalhes" runat="server" DataSource='<%# Eval("Itens") %>' OnItemCommand="rptItens_ItemCommand">
    <ItemTemplate>
        
        <li class='list-group-item p-0 border-0 border-bottom row-item-container tipo-<%# Eval("TipoRegistro") %> <%# (Eval("TipoRegistro").ToString() == "AJUSTE_MANUAL" && PodeEditar) ? "editavel" : "" %>'>
            
            <div class="d-flex justify-content-between align-items-stretch">
                
                <div class="flex-grow-1 py-3 ps-3 pe-2">
                    <asp:LinkButton ID="btnEditarLinha" runat="server"
                        CommandName="EditarAjuste"
                        CommandArgument='<%# Eval("IdItem") %>'
                        Enabled='<%# Eval("TipoRegistro").ToString() == "AJUSTE_MANUAL" && PodeEditar %>'
                        CssClass="btn-block-link">

                        <div class="d-flex align-items-center"> <div class="me-3">
                                <div class='icon-status shadow-sm <%# Eval("TipoRegistro").ToString() == "SISTEMA" ? "locked" : "unlocked" %>' 
                                     title='<%# Eval("TipoRegistro").ToString() == "SISTEMA" ? "Registro de Sistema (Bloqueado)" : "Ajuste Manual (Editável)" %>'>
                                    
                                    <i class='fa <%# Eval("TipoRegistro").ToString() == "SISTEMA" ? "fa-lock" : "fa-lock-open" %>'></i>
                                </div>
                            </div>

                            <div>
                                <div class="fw-bold text-dark" style="font-size: 1.05rem;">
                                    <%# Eval("Titulo") %>
                                </div>
                                <div class="text-muted small fst-italic text-wrap mt-1">
                                    <%# Eval("Observacao") %>
                                    
                                    <asp:PlaceHolder runat="server" Visible='<%# Eval("TipoRegistro").ToString() == "AJUSTE_MANUAL" && PodeEditar %>'>
                                        <span class="badge bg-light text-primary border ms-2">Editar <i class="fa fa-pen ms-1"></i></span>
                                    </asp:PlaceHolder>
                                </div>
                            </div>
                        </div>
                    </asp:LinkButton>
                </div>

                <div class="d-flex flex-column justify-content-center align-items-end py-3 pe-3 border-start bg-white" style="min-width: 110px;">
                    
                    <div class="fw-bold fs-5 text-dark mb-1"><%# Eval("Horas", "{0:N2}") %>h</div>

                    <asp:LinkButton ID="btnExcluir" runat="server"
                        CommandName="ExcluirAjuste"
                        CommandArgument='<%# Eval("IdItem") %>'
                        Visible='<%# Eval("TipoRegistro").ToString() == "AJUSTE_MANUAL" && PodeEditar %>'
                        OnClientClick="return abrirConfirmacao('Deseja realmente excluir este ajuste?', this.name);"
                        CssClass="text-danger btn btn-sm btn-link p-0 text-decoration-none z-index-2"
                        ToolTip="Excluir">
                        <small><i class="fa fa-trash-alt me-1"></i>Remover</small>
                    </asp:LinkButton>

                </div>
            </div>
        </li>

    </ItemTemplate>
</asp:Repeater>

                        </ul>
                    </div>
                </div>
            </ItemTemplate>
        </asp:Repeater>



                                <div style="height: 100px;"></div>
                            </div>

                            <div class="fab-container" runat="server" id="divFab" visible="false">
                                <%--        <button type="button" class="btn-fab" onclick="abrirModalAjuste()" title="Adicionar Ajuste Manual"><i class="fa fa-plus"></i></button>--%>
                                <button type="button" class="btn-fab" onclick="abrirModalNovo()" title="Adicionar Ajuste Manual">
                                    <i class="fa fa-plus"></i>
                                </button>
                            </div>

                            <div id="divBarraFinalizar" runat="server" class="barra-inferior shadow-lg text-center" visible="false">
                                <div class="container" style="max-width: 600px;">
                                    <asp:Button ID="btnFinalizar" runat="server" Text="Enviar Fechamento"
                                        CssClass="btn btn-success w-100 btn-lg fw-bold shadow-sm"
                                        OnClick="btnFinalizar_Click"
                                        OnClientClick="return abrirConfirmacao('Tem certeza que deseja fechar o mês? Não será possível alterar depois.', this.name);" />
                                </div>
                            </div>

                            <asp:HiddenField ID="hddIdFechamento" runat="server" Value="0" />
                            <asp:HiddenField ID="hddStatus" runat="server" Value="" />
                        </ContentTemplate>
                    </asp:UpdatePanel>
                </div>

                <div class="offcanvas offcanvas-start" tabindex="-1" id="offcanvasHistorico">
                    <div class="offcanvas-header bg-light border-bottom">
                        <h5 class="offcanvas-title fw-bold text-primary"><i class="fa fa-history me-2"></i>Meus Fechamentos</h5>
                        <button type="button" class="btn-close text-reset" data-bs-dismiss="offcanvas"></button>
                    </div>
                    <div class="offcanvas-body p-0">
                        <div class="list-group list-group-flush">
                            <asp:UpdatePanel ID="updMenu" runat="server" UpdateMode="Conditional">
                                <ContentTemplate>
                                    <asp:Repeater ID="rptHistorico" runat="server" OnItemCommand="rptHistorico_ItemCommand">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkItemHistorico" runat="server"
                                                CommandName="CarregarFechamento"
                                                CommandArgument='<%# Eval("idFechamento") + "|" + Eval("sStatus") %>'
                                                CssClass='<%# "list-group-item list-group-item-action py-3 " + (Eval("idFechamento").ToString() == hddIdFechamento.Value ? "active" : "") %>'>
                                    
                                    <div class="d-flex w-100 justify-content-between align-items-center">
                                        <h6 class="mb-1 fw-bold"><%# Eval("TituloExibicao") %></h6>
                                        <small class='badge rounded-pill <%# Eval("sStatus").ToString() == "RASCUNHO" ? "bg-warning text-dark" : "bg-success" %>'>
                                            <%# Eval("sStatus") %>
                                        </small>
                                    </div>
                                    <small class="opacity-75">Gerado em: <%# Eval("dtGeracao", "{0:dd/MM/yyyy}") %></small>
                                    
                                </asp:LinkButton>
                                        </ItemTemplate>
                                    </asp:Repeater>

                                    <div runat="server" id="divSemHistorico" visible="false" class="text-center p-4 text-muted">
                                        <i class="fa fa-folder-open fa-2x mb-2"></i>
                                        <br />
                                        Nenhum relatório encontrado.
                       
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>

    <div id="dialog-Confirmacao" title="Confirmação" style="display: none;">
        <p>
            <span class="ui-icon ui-icon-alert" style="float: left; margin: 12px 12px 20px 0;"></span>
            <span id="lblMensagemConfirmacao">Tem certeza que deseja prosseguir?</span>
        </p>
    </div>

    <div class="modal fade" id="modalAjuste" tabindex="-1" aria-hidden="true">
        <div class="modal-dialog modal-dialog-centered">
            <div class="modal-content border-0 shadow">
                <div class="modal-header modal-header-custom">
                    <h5 class="modal-title fw-bold"><i class="fa fa-clock me-2 text-primary"></i>Adicionar Horas</h5>
                    <button type="button" class="btn-close" data-bs-dismiss="modal"></button>
                </div>
                <div class="modal-body p-4">
                    <div class="mb-3">
                        <label class="form-label fw-bold">Data do Ajuste</label>
                        <asp:TextBox ID="txtDataAjuste" runat="server" TextMode="Date" CssClass="form-control form-control-lg"></asp:TextBox>
                    </div>
                    <div class="mb-3">
                        <label class="form-label fw-bold">Quantidade de Horas</label>
                        <div class="input-group">
                            <asp:TextBox ID="txtHorasAjuste" runat="server" TextMode="Number" step="0.01" CssClass="form-control form-control-lg" placeholder="Ex: 2,5"></asp:TextBox>
                            <span class="input-group-text">h</span>
                        </div>
                    </div>
                    <div class="mb-3">
                        <label class="form-label fw-bold">Motivo / Observação</label>
                        <asp:TextBox ID="txtObsAjuste" runat="server" TextMode="MultiLine" Rows="3" CssClass="form-control" placeholder="Descreva o motivo do ajuste..."></asp:TextBox>
                    </div>
                </div>
                <div class="modal-footer bg-light d-flex justify-content-between">
                    <div>
                        <asp:Button ID="btnExcluirModal" runat="server" Text="Excluir"
                            CssClass="btn btn-outline-danger border-0"
                            OnClick="btnExcluirModal_Click"
                            OnClientClick="return confirm('Tem certeza?');"
                            Visible="false" />
                    </div>

                    <div>
                        <button type="button" class="btn btn-outline-secondary me-1" data-bs-dismiss="modal">Cancelar</button>
                        <asp:Button ID="btnSalvarAjuste" runat="server" Text="Salvar" CssClass="btn btn-primary px-4" OnClick="btnSalvarAjuste_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script>

        function abrirModalNovo() {
            document.getElementById('<%= txtDataAjuste.ClientID %>').value = '';
            document.getElementById('<%= txtHorasAjuste.ClientID %>').value = '';
            document.getElementById('<%= txtObsAjuste.ClientID %>').value = '';
            document.getElementById('<%= hddIdItemEdicao.ClientID %>').value = '0'; // ID 0 = Insert

            var myModal = new bootstrap.Modal(document.getElementById('modalAjuste'));
            myModal.show();
        }

        // Função para EDIÇÃO (Chamada pelo C# após preencher os campos)
        function abrirModalAjuste() {
            var myModal = new bootstrap.Modal(document.getElementById('modalAjuste'));
            myModal.show();
        }

        function fecharModalAjuste() {
            var el = document.getElementById('modalAjuste');
            var modal = bootstrap.Modal.getInstance(el);
            if (modal) modal.hide();
            $('.modal-backdrop').remove();
        }

        //function abrirModalAjuste() { var myModal = new bootstrap.Modal(document.getElementById('modalAjuste')); myModal.show(); }
        //function fecharModalAjuste() { var el = document.getElementById('modalAjuste'); var modal = bootstrap.Modal.getInstance(el); if (modal) modal.hide(); $('.modal-backdrop').remove(); }
    </script>
</asp:Content>
