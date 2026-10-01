<%@ Page Title="Meus Lançamentos" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true" CodeBehind="Relatorio_Despesas_Detalhe.aspx.cs" Inherits="TT_Colaborador.Aplicativo.Paginas.Despesas.Relatorio_Despesas_Detalhe" %>

<%@ Register Src="~/Aplicativo/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.mask/1.14.16/jquery.mask.min.js"></script>

    <style>
        .card-header-mobile {
            background-color: #007bff;
            color: white;
            padding: 15px;
            border-radius: 5px 5px 0 0;
        }

        .btn-mobile-action {
            width: 100%;
            padding: 12px;
            font-size: 16px;
            font-weight: bold;
            margin-top: 10px;
        }

        .lancamento-card {
            border-left: 5px solid #28a745;
            background: #fff;
            box-shadow: 0 2px 5px rgba(0,0,0,0.1);
            margin-bottom: 10px;
            padding: 15px;
            border-radius: 4px;
        }

        .lancamento-valor {
            font-size: 18px;
            font-weight: bold;
            color: #333;
        }

        .lancamento-data {
            font-size: 12px;
            color: #666;
        }

        .form-label {
            font-weight: 600;
            color: #555;
        }

        .custom-file-upload {
            border: 1px dashed #ccc;
            display: inline-block;
            padding: 10px;
            cursor: pointer;
            width: 100%;
            text-align: center;
            background: #f9f9f9;
            color: #555;
            border-radius: 4px;
        }

        /* CORREÇÃO DO CLIQUE NO BOTÃO (Para funcionar clicando no ícone) */
    .btn i {
        pointer-events: none;
    }
    
    /* Ajuste visual do upload */
    .custom-file-upload {
        width: auto; 
        flex: 1;
        margin-bottom: 0;
    }
    </style>

    <script>
        $(document).ready(function () {
            applyMasks();
            // Re-aplica máscara após UpdatePanel
            var prm = Sys.WebForms.PageRequestManager.getInstance();
            prm.add_endRequest(function () {
                applyMasks();
            });
        });

        function applyMasks() {
            $('[id*=txtValor]').mask('#.##0,00', { reverse: true });
        }

        // Script simples visual para mudar texto do input file
        function updateFileName(input) {
            if (input.files && input.files.length > 0)
                $('#lblFileName').text(input.files[0].name);
            else
                $('#lblFileName').text('Anexar Foto/Comprovante...');
        }

        function abrirModalArquivos() {
            var myModalEl = document.getElementById('modalArquivos');
            // Cria a instância do Bootstrap 5 e abre
            var modal = new bootstrap.Modal(myModalEl);
            modal.show();
        }

        function ForcarEsconderLoading() {
            // Espera o Load da Master aparecer e força ele a sumir logo em seguida
            setTimeout(function () {
                $('[id*=div_Loading]').hide();
                $('[id*=div_img]').hide();
            }, 1500); // 1.5s de margem de segurança
        }
    </script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:UpdateProgress ID="UpdateProgress1" runat="server">
        <ProgressTemplate>
            <div style="position: fixed; top: 0; left: 0; width: 100%; height: 100%; background: rgba(255,255,255,0.8); z-index: 9999; display: flex; align-items: center; justify-content: center;">
                <div class="spinner-border text-primary" role="status"></div>
            </div>
        </ProgressTemplate>
    </asp:UpdateProgress>

    <div class="container-fluid p-2">

        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

        <div class="card mb-3">
            <div class="card-header-mobile">
                <div class="d-flex justify-content-between">
                    <h4 class="mb-0">Meus Gastos</h4>
                    <asp:LinkButton ID="btnVoltar" runat="server" CssClass="btn btn-sm btn-light text-primary" PostBackUrl="Relatorio_Despesas.aspx">
                        <i class="fa fa-arrow-left"></i> Voltar
                    </asp:LinkButton>
                </div>
            </div>
            <div class="card-body bg-light">

                <div class="row" style="font-size: 13px;">

                    <div class="col-12 d-flex justify-content-between">
                        <div>
                            <small class="text-muted">Motivo / Obra</small><br />
                            <strong style="font-size: 16px;">
                                <asp:Label ID="lblMotivo" runat="server" Text="..."></asp:Label></strong>
                        </div>
                        <div>
                            <asp:Label ID="lblStatus" runat="server" Text="status" CssClass="badge bg-secondary"></asp:Label>
                        </div>
                    </div>
                    <div class="col-6 mt-2">
                        <small class="text-muted">Centro de Custo</small><br />
                        <asp:Label ID="lblCentroCusto" runat="server" Text="..."></asp:Label>
                    </div>
                    <div class="col-6 mt-2">
                        <small class="text-muted">Vigência</small><br />
                        <asp:Label ID="lblPeriodo" runat="server" Text="..."></asp:Label>
                    </div>
                </div>
            </div>
        </div>

        <div class="card mb-3 shadow-sm">
            <div class="card-body p-3">
                <h5 class="card-title text-primary"><i class="fa fa-plus-circle"></i>Novo Lançamento</h5>
                <hr />

                <div class="row">
                    <div class="col-6 mb-2">
                        <label class="form-label">Data</label>
                        <asp:TextBox ID="txtData" runat="server" CssClass="form-control" TextMode="Date"></asp:TextBox>
                    </div>
                    <div class="col-6 mb-2">
                        <label class="form-label">Valor (R$)</label>
                        <asp:TextBox ID="txtValor" runat="server" CssClass="form-control" placeholder="0,00"></asp:TextBox>
                    </div>
                    <div class="col-12 mb-2">
                        <label class="form-label">Tipo de Despesa</label>
                        <asp:DropDownList ID="ddlTipo" runat="server" CssClass="form-control form-select">
                            <asp:ListItem Value="0">Selecione...</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                    <div class="col-12 mb-2">
                        <label class="form-label">Descrição</label>
                        <asp:TextBox ID="txtLocal" runat="server" CssClass="form-control" placeholder="Local / Descrição" MaxLength="100"></asp:TextBox>
                    </div>
                    <div class="col-12 mb-3">
                        <label class="form-label">Comprovantes <small class="text-danger">* Mínimo 1</small></label>

                        <div class="input-group">
                            <label class="custom-file-upload form-control m-0">
                                <asp:FileUpload ID="fileUpload" runat="server" Style="display: none;" onchange="updateFileName(this)" accept="image/*,.pdf" />
                                <i class="fa fa-camera"></i><span id="lblFileName">Selecionar...</span>
                            </label>
                            <asp:LinkButton ID="btnAdicionarArquivo" runat="server" CssClass="btn btn-success" OnClick="btnAdicionarArquivo_Click">
            <i class="fa fa-plus"></i>
        </asp:LinkButton>
                        </div>

                        <asp:UpdatePanel ID="updArquivosNovos" runat="server" UpdateMode="Conditional">
                            <ContentTemplate>
                                <asp:Repeater ID="rptArquivosNovos" runat="server" OnItemCommand="rptArquivosNovos_ItemCommand">
                                    <ItemTemplate>
                                        <div class="file-item">
                                            <span><i class="fa fa-file-text-o"></i><%# Eval("Nome") %></span>
                                            <asp:LinkButton ID="btnRmFile" runat="server" CommandName="Remover" CommandArgument='<%# Eval("IdTemporario") %>' CssClass="text-danger">
                            <i class="fa fa-times"></i>
                        </asp:LinkButton>
                                        </div>
                                    </ItemTemplate>
                                </asp:Repeater>
                                <asp:Label ID="lblQtdArquivos" runat="server" CssClass="small text-muted"></asp:Label>
                            </ContentTemplate>
                            <Triggers>
                                <asp:PostBackTrigger ControlID="btnAdicionarArquivo" />
                                <%-- Necessário para o Upload --%>
                            </Triggers>
                        </asp:UpdatePanel>
                    </div>
                    <div class="col-12">
                        <asp:Button ID="btnSalvar" runat="server" Text="LANÇAR DESPESA" CssClass="btn btn-success btn-mobile-action" OnClick="btnSalvar_Click" />
                    </div>
                </div>
            </div>
        </div>

        <asp:UpdatePanel ID="updLista" runat="server">
            <ContentTemplate>
                <div class="card">
                    <div class="card-header bg-white d-flex justify-content-between p-2">
                        <h6 class="mb-0 text-muted">Meus Lançamentos</h6>
                        <div class="text-success font-weight-bold">
                            <asp:Label ID="lblTotalMeu" runat="server" Text="R$ 0,00"></asp:Label>
                        </div>
                    </div>
                    <div class="card-body p-2">

                        <asp:Repeater ID="rptLancamentos" runat="server" OnItemCommand="rptLancamentos_ItemCommand">
                            <ItemTemplate>
                                <div class="lancamento-card">
                                    <div class="row">
                                        <div class="col-8">
                                            <div class="lancamento-data">
                                                <i class="fa fa-calendar"></i><%# Eval("dtDespesa", "{0:dd/MM/yyyy}") %> - <%# Eval("sDescTipo") %>
                                            </div>
                                            <div style="font-weight: 600; font-size: 14px;"><%# Eval("sLocal") %></div>
                                        </div>
                                        <div class="col-4 text-right" style="text-align: right;">
                                            <div class="lancamento-valor text-success">
                                                <%# Eval("nValor", "{0:C2}") %>
                                            </div>


                                        </div>
                                        <div class="mt-2">
                                            <asp:LinkButton ID="btnVerArquivos" runat="server" CommandName="VerArquivos" CommandArgument='<%# Eval("idItens") %>' CssClass="btn btn-xs btn-info text-white mr-1">
                                                <i class="fa fa-paperclip"></i>
                                            </asp:LinkButton>

                                            <asp:LinkButton ID="btnExcluir" runat="server" CommandName="Excluir" CommandArgument='<%# Eval("idItens") %>' CssClass="btn btn-xs btn-outline-danger " OnClientClick="return confirm('Excluir este item?');">
                                                <i class="fa fa-trash"></i> Excluir
                                            </asp:LinkButton>
                                        </div>
                                    </div>
                                </div>
                            </ItemTemplate>
                            <FooterTemplate>
                                <%# rptLancamentos.Items.Count == 0 ? "<div class='text-center text-muted p-3'>Nenhum lançamento encontrado.</div>" : "" %>
                            </FooterTemplate>
                        </asp:Repeater>

                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>

    </div>

    <div class="modal fade" id="modalArquivos" tabindex="-1" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered">
        <div class="modal-content">
            <div class="modal-header bg-primary text-white">
                <h5 class="modal-title">Comprovantes</h5>
                <button type="button" class="btn-close btn-close-white" data-bs-dismiss="modal" aria-label="Close"></button>
            </div>
            <div class="modal-body p-0">
                <asp:UpdatePanel ID="updModalArquivos" runat="server" UpdateMode="Conditional">
                    <ContentTemplate>
                        <asp:GridView ID="gvArquivosHistorico" runat="server" AutoGenerateColumns="False" 
                            CssClass="table table-striped m-0" 
                            GridLines="None" 
                            OnRowCommand="gvArquivosHistorico_RowCommand" 
                            OnRowDataBound="gvArquivosHistorico_RowDataBound">
                            <Columns>
                                <asp:BoundField DataField="Nome" HeaderText="Arquivo" />
                                <asp:TemplateField ItemStyle-HorizontalAlign="Right">
                                    <ItemTemplate>
                                       <asp:LinkButton ID="btnDownload" runat="server" 
            CommandName="Download" 
            CommandArgument='<%# Eval("IdTemporario") %>' 
            OnClientClick="ForcarEsconderLoading();"
            CssClass="btn btn-sm btn-primary">
            <i class="fa fa-download"></i> Baixar
        </asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <EmptyDataTemplate>
                                <div class="p-3 text-center text-muted">Nenhum arquivo encontrado.</div>
                            </EmptyDataTemplate>
                        </asp:GridView>
                    </ContentTemplate>
                </asp:UpdatePanel>
            </div>
            <div class="modal-footer">
                <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Fechar</button>
            </div>
        </div>
    </div>
</div>
</asp:Content>
