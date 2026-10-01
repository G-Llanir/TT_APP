<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="GerenciadorMembrosModal.ascx.cs" Inherits="TT_Flow.App.Controles.GerenciadorMembrosModal" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<style>
    /* ---------------------- MODAL PROJETOS ---------------------- */
    .modal-projetos-header {
        background-color: #024e0a;
        color: #fff;
        padding: 1.5rem 2rem; 
        border-top-left-radius: 1rem;
        border-top-right-radius: 1rem;
        align-items: center;
        font-size: 1.5rem; 
        font-weight: 700;
    }

    .modal-projetos-body {
        background-color: #f9f9f9;
        padding: 2rem 2rem;
        font-size: 1.2rem; 
        line-height: 1.6; 
    }

    .modal-projetos-card {
        background-color: #ffffff;
        border-left: 8px solid #009a22; 
        border-radius: 1rem;
        padding: 2rem; 
        box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1); 
        margin-bottom: 2.5rem;
    }

    .modal-projetos-produto {
        font-size: 1.5rem;
        font-weight: 700;
        margin-bottom: 1rem;
    }

    .modal-projetos-requisicao {
        font-size: 1.1rem;
        margin-bottom: 1.5rem;
        color: #444; 
    }

    .modal-projetos-recurso {
        background-color: #e9f8ec; 
        border: 1.5px solid #009a22;
        border-left-width: 8px;
        border-radius: 1rem;
        padding: 1.5rem 2rem;
        margin-bottom: 1.5rem;
    }

        .modal-projetos-recurso h6 {
            font-size: 1.3rem;
            font-weight: 700;
            margin-bottom: 0.75rem;
            color: #007d12; 
        }

    .modal-projetos-processo {
        font-size: 1rem;
        color: #555;
        margin-bottom: 0.75rem;
        line-height: 1.4;
    }

    /* Cores de apoio */
    .bg-verde-tt {
        background-color: #009a22 !important;
    }

    .text-verde-tt {
        color: #009a22 !important;
    }

    .border-verde-tt {
        border-color: #009a22 !important;
    }

    .bg-verde-escuro {
        background-color: #024e0a !important;
    }

    .text-verde-escuro {
        color: #024e0a !important;
    }

    .border-verde-escuro {
        border-color: #024e0a !important;
    }

    /* Estilo para o novo container de botões */
    .modal-actions-container {
        padding: 1.5rem 2rem;
        background-color: #f0f0f0; 
        border-top: 1px solid #e0e0e0;
        border-bottom-left-radius: 1rem;
        border-bottom-right-radius: 1rem;
        display: flex;
        justify-content: flex-end;
        gap: 10px; 
    }

        /* ---------------------- MODAL PROJETOS ---------------------- */
    .chosen-container.chosen-disabled {
        background-color: #e9ecef;
        opacity: 0.7; 
        cursor: not-allowed; 
    }

    .usuariosProjeto[data-atividade-finalizada="true"] + .chosen-container .chosen-choices {
        background-color: #e9ecef !important; 
        cursor: not-allowed;
    }

    .usuariosProjeto[data-atividade-finalizada="true"] + .chosen-container .chosen-choices .search-choice {
        background-image: none !important;
        background-color: #d1d6db !important;
        border-color: #adb5bd !important;
        color: #495057 !important; 
        cursor: not-allowed;
    }

    .usuariosProjeto[data-atividade-finalizada="true"] + .chosen-container .chosen-choices .search-choice-close {
        display: none !important;
    }

    .usuariosProjeto[data-atividade-finalizada="true"] + .chosen-container .chosen-choices .search-field input {
        background-color: #e9ecef !important;
        cursor: not-allowed;
    }

</style>
<div class="form-group">

    <div class="modal fade" id="modalAtividades" tabindex="-1" role="dialog" aria-labelledby="modalAtividadesLabel" aria-hidden="true">
        <div class="modal-dialog modal-xl modal-dialog-scrollable" role="document">
            <div class="modal-content rounded-4 border-0">

                <div class="modal-projetos-header modal-header bg-verde-escuro text-white rounded-top px-4 d-flex align-items-center">
<%--                    <h5 class="modal-title fw-bold mb-0 me-auto" id="modalAtividadesLabel">👥 Gerenciar Membros por Atividade</h5>--%>
                    <button type="button" class="close text-white" data-dismiss="modal" aria-label="Fechar">
                        <span aria-hidden="true" style="font-size: 1.8rem;">&times;</span>
                    </button>
                    <asp:Button ID="cmdExportar" runat="server" CssClass="btn btn-danger btn-sm me-2" Text="📄 Exportar PDF" visible="false" OnClick="cmdExportar_Click"/>
                </div>

                <div class="modal-projetos-body modal-body bg-white px-4 py-4">
                    <uc1:MensagemPagina runat="server" ID="MensagemPaginaControle" />
                    <asp:Repeater ID="rptProjetos" runat="server" OnItemDataBound="rptProjetos_ItemDataBound">
                        <ItemTemplate>
                            <div class="modal-projetos-card card shadow-sm border-0 rounded-4 mb-4 p-4">
                                <h5 class="modal-projetos-produto text-verde-escuro mb-3 fw-semibold">🏗️ Projeto: <%# Eval("idProjeto") %> - <%# Eval("sDscTituloProjeto") %>
                                </h5>                      
                                <p class="modal-projetos-requisicao text-muted mb-3">
                                    📄 Requisição: <strong><%# Eval("sNumeroRequisicao") %></strong>
                                </p>

                                <div class="ms-4 border-start ps-3 border-verde-tt border-2">

                                    <asp:Repeater ID="rptAtividadesPai" runat="server" OnItemDataBound="rptAtividadesPai_ItemDataBound">
                                        <ItemTemplate>
                                            <div class="modal-projetos-recurso bg-light border rounded-3 p-3 mb-3">
                                                <h6 class="text-verde-tt fw-bold mb-2">👨‍👧‍👦  <%# Eval("sDscTituloAtividadePai") %>
                                                </h6>
                                                <div class="modal-projetos-processo ms-3 border-start ps-3 border-verde-escuro border-2 text-secondary small mb-1">
                                                    <asp:Repeater ID="rptAtividadesFilhas" runat="server" OnItemDataBound="rptAtividadesFilhas_ItemDataBound">
                                                        <ItemTemplate>
                                                            <h6 class="text-verde-tt fw-bold mb-2">🛠️  <%# Eval("sDscTituloAtividade") %>
                                                            </h6>
                                                              <p class="modal-projetos-processo text-muted small mb-1">
                                                        📊 Status: <strong><%# Eval("sStatusAtividade") %></strong>
                                                    </p>
                                                            <label class="form-label text-muted small mb-1">👥 Membros Disponíveis:</label>
                                                            <asp:ListBox ID="ddlMembrosAtividade" runat="server"
                                                                SelectionMode="Multiple"
                                                                CssClass="form-control Caixa_Selecao usuariosProjeto"></asp:ListBox>
                                                            <asp:HiddenField ID="hdnIdAtividade" runat="server" Value='<%# Eval("idAtividade") %>' />
                                                             <asp:HiddenField ID="HddStatus" runat="server" Value='<%# Eval("sStatusAtividade") %>' />
                                                        </ItemTemplate>
                                                    </asp:Repeater>
                                                </div>
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </div>
                            </div>
                        </ItemTemplate>
                    </asp:Repeater>

                    <div class="modal-actions-container">
                        <asp:Button ID="btnSalvarMembros" runat="server" CssClass="btn btn-success fw-semibold px-4" Text="💾 Salvar Alterações" OnClick="cmdSalvarMembros_Click"  CausesValidation="false" />
                        <asp:Button Text="Fechar" ID="cmdFechar" runat="server" type="button" class="btn btn-danger fw-semibold px-4" OnClick="FecharModal_Click" />
                    </div>                  
                </div>


            </div>
        </div>
    </div>
    <asp:HiddenField ID="hddidProjeto" runat="server" />
</div>
