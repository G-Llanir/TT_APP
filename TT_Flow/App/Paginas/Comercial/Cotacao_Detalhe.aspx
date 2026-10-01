<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" CodeBehind="Cotacao_Detalhe.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.Cotacao_Detalhe" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/ExcelImportar.ascx" TagPrefix="uc1" TagName="ExcelImportar" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=15.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="RV" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <asp:UpdatePanel ID="UpdpDetalhe" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <style>
                .invisivel {
                    display: none;
                }

                .card {
                    box-shadow: 0px 2px 8px 2px rgba(0,0,0,0.3);
                    transition: 0.3s;
                    width: 100%;
                    border-radius: 5px;
                }

                .card-body {
                    padding: 10px;
                    display: flex;
                    align-items: flex-start;
                }

                .flex-grow-1 {
                    flex-grow: 1;
                    margin-left: 15px;
                }

                .img-thumbnail {
                    border: none;
                    width: 100px;
                    height: auto;
                }

                .me-3 {
                    margin-right: 1rem;
                    padding: 2px;
                    border-bottom: 1px dashed black;
                }

                .modal-dialog {
                    display: flex;
                    justify-content: center;
                }

                .modal-title {
                    text-shadow: 1px 2px 3px rgba(0, 0, 0, 0.2);
                    font-size: 20px;
                    font-weight: bold;
                    margin: 0;
                }

                .infoObs {
                    text-decoration: none;
                    color: blue;
                }

                .infoAlterada {
                    text-decoration: underline;
                    color: red;
                }

                .infoNova {
                    text-decoration: underline;
                    color: green;
                }
            </style>

            <div id="hdd">
                <asp:HiddenField ID="hddidCotacao" runat="server" Value="0" />
                <asp:HiddenField ID="hddParceiros" runat="server" Value="[]" />
                <asp:HiddenField ID="hddidParceiro" runat="server" Value="0" />
                <asp:HiddenField ID="hddEditar" runat="server" Value="false" />
                <asp:HiddenField ID="hddItens" runat="server" Value="[]" />
                <asp:HiddenField ID="hddIncluirItem" runat="server" Value="0" />
                <asp:HiddenField ID="hddnNumeroPedido" runat="server" Value="0" />
                <asp:HiddenField ID="hddidOrcamento_Vinculado" runat="server" Value="0" />
                <asp:HiddenField ID="hdddtPedido" runat="server" />
                <asp:HiddenField ID="hddsCliente" runat="server" Value="false" />
            </div>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb" NivelPagina="3" TitulodaPagina="Detalhe" />
                </div>
            </div>

            <div id="abas">
                <ul id="tab_Cotacao" class="nav nav-tabs" role="tablist">
                    <li role="presentation" class="tabpanel active">
                        <a href="#cotacao" id="aba_Cotacao" role="tabpanel" data-toggle="tab" aria-controls="home" aria-expanded="false" runat="server"><b>Cotação</b></a>
                    </li>
                    <li role="presentation" runat="server" id="aba_Historico">
                        <a href="#historico" role="tab" id="historico-tab" data-toggle="tab" aria-controls="Historico"><b>Histórico</b></a>
                    </li>
                </ul>
            </div>

            <div id="tab" class="tab-content">

                <div role="tabpanel" class="tab-pane fade in active" id="cotacao" aria-labelledby="cotacao-tab">
                    <br />
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Cotação</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group row">

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                                </div>

                                <div id="div_Cotacao" class="row">
                                    <div class="col-lg-12">

                                        <div runat="server" id="div_ID" class="col-lg-12 form-group" style="padding: 0;">
                                            <div class="col-lg-1 form-group">
                                                <label>ID</label>
                                                <asp:TextBox runat="server" ID="txtidCotacao" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_Parceiro" class="col-lg-12" style="padding: 0;">
                                            <asp:Panel runat="server" ID="pnParceiro" DefaultButton="cmdSelecionarParceiro">

                                                <div class="col-lg-2 form-group">
                                                    <label>CPF / CNPJ</label>
                                                    <asp:TextBox runat="server" ID="txtCPF_Parceiro" CssClass="form-control"></asp:TextBox>
                                                </div>

                                                <div class="col-lg-8 form-group">
                                                    <label>Nome do Parceiro</label>
                                                    <asp:TextBox runat="server" ID="txtNome_Parceiro" CssClass="form-control"></asp:TextBox>
                                                </div>

                                                <div runat="server" id="div_cmdSelecionarParceiro" class="col-lg-2 form-group">
                                                    <label>&nbsp;</label>
                                                    <asp:Button runat="server" ID="cmdSelecionarParceiro" CssClass="form-control btn-info" Text="Selecionar Parceiro" OnClick="cmdSelecionarParceiro_Click" />
                                                </div>

                                            </asp:Panel>
                                        </div>

                                        <div runat="server" id="div_Endereco" class="col-lg-12" style="padding: 0;">
                                            <div class="col-lg-2 form-group">
                                                <label>IE</label>
                                                <asp:TextBox runat="server" ID="txtIE" CssClass="form-control" placeholder="Inscrição Estadual" ReadOnly="true"></asp:TextBox>
                                            </div>

                                            <div class="col-lg-5 form-group">
                                                <label>Endereço de Entrega</label>
                                                <asp:DropDownList runat="server" ID="ddlEnderecoEntrega" class="form-control Caixa_Selecao"></asp:DropDownList>
                                            </div>

                                            <div class="col-lg-3 form-group">
                                                <label>Tabela de Preços</label>
                                                <asp:DropDownList runat="server" ID="ddlTabela" CssClass="form-control Caixa_Selecao" AutoPostBack="true" OnSelectedIndexChanged="ddlTabela_SelectedIndexChanged"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_Referencia" class="col-lg-12" style="padding: 0;">
                                            <div class="col-lg-2 form-group">
                                                <label>N° da Cotação</label>
                                                <asp:TextBox runat="server" ID="txtnCotacao" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                            </div>

                                            <div class="col-lg-4 form-group">
                                                <label>Referência</label>
                                                <asp:TextBox runat="server" ID="txtReferencia" class="form-control" MaxLength="60"></asp:TextBox>
                                            </div>

                                            <div id="div_ddlCondicaoPagamento" class="col-lg-4">
                                                <div class="form-group">
                                                    <label>Condição de Pagamento</label>
                                                    <asp:DropDownList ID="ddlCondicaoPagamento" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_Obs" class="col-lg-12 form-group">
                                            <label>Observação</label>
                                            <asp:TextBox runat="server" ID="txtObs" CssClass="form-control" TextMode="MultiLine" Rows="5"></asp:TextBox>
                                        </div>

                                        <div runat="server" id="div_Total" class="col-lg-2 form-group">
                                            <label>Total da Cotação</label>
                                            <asp:TextBox runat="server" ID="txtTotal" CssClass="form-control" ReadOnly="true"></asp:TextBox>
                                        </div>

                                    </div>
                                </div>

                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Incluir_Itens" />
                                </div>

                                <div id="div_Itens" runat="server" class="col-lg-12">
                                    <div class="panel panel-default" style="margin: 0;">
                                        <div class="panel-heading" style="display: flex; justify-content: space-between;">
                                            <h3 class="panel-title"><b>Itens</b></h3>
                                            <asp:LinkButton ID="cmdAtualizar" class="btn btn-sm btn-info" runat="server" OnClick="cmdAtualizar_Click" data-toggle="tooltip" title="Atualizar Itens" TabIndex="100"><i class="fa fa-refresh"></i></asp:LinkButton>
                                        </div>
                                        <div class="panel-body">
                                            <div class="form-group row">

                                                <div class="col-lg-12">
                                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_Itens" />
                                                </div>

                                                <div runat="server" id="divIncluir_Itens" class="col-lg-12" style="padding: 0;">
                                                    <asp:Panel runat="server" ID="pnIncluir_Itens" DefaultButton="cmdIncluir_Item">

                                                        <div class="col-lg-2 form-group">
                                                            <label>Código</label>
                                                            <asp:TextBox ID="txtCodigo_Item" class="form-control" runat="server"></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-4">
                                                            <label>Descrição</label>
                                                            <asp:TextBox ID="txtDescricao_Item" class="form-control" runat="server"></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-2">
                                                            <label>Quantidade</label>
                                                            <asp:TextBox ID="txtQuantidade_Item" class="form-control valor" runat="server" MaxLength="3" Text="1"></asp:TextBox>
                                                        </div>

                                                        <div class="col-lg-2">
                                                            <label>&nbsp;</label>
                                                            <asp:Button ID="cmdIncluir_Item" class="form-control btn-info" runat="server" Text="Incluir Item" OnClick="cmdIncluir_Item_Click" />
                                                        </div>

                                                        <div class="col-lg-2">
                                                            <label>&nbsp;</label>
                                                            <asp:Button ID="cmdImportarItens" class="form-control btn-primary" runat="server" Text="Importar Itens" OnClientClick="$('#modalImportarItens_Excel').modal('show');" />
                                                        </div>

                                                    </asp:Panel>
                                                </div>

                                                <div runat="server" id="div_Excluir" class="col-lg-12 form-group" style="padding: 0;">

                                                    <div class="col-lg-10">&nbsp;</div>

                                                    <div class="col-lg-2">
                                                        <asp:Button ID="cmdExcluir_Item" class="form-control btn-danger" runat="server" Text="Excluir Itens" OnClick="cmdExcluir_Item_Click" />
                                                    </div>

                                                </div>

                                                <div runat="server" id="div_dtgItens" class="col-lg-12">
                                                    <asp:GridView ID="dtgItens" class="table table-striped table-bordered table-hover table-condensed" Style="margin: 0;"
                                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="14px" OnRowDataBound="dtgItens_RowDataBound">
                                                        <Columns>

                                                            <asp:BoundField DataField="idContador" HeaderText="idContador">
                                                                <HeaderStyle CssClass="invisivel" />
                                                                <ItemStyle Width="0%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="invisivel" />
                                                            </asp:BoundField>

                                                            <asp:HyperLinkField DataNavigateUrlFields="idProduto"
                                                                DataTextField="sCodigoProduto" HeaderText="Código" Target="_blank"
                                                                DataNavigateUrlFormatString="/App/Paginas/Manutencao/Produtos_Detalhe.aspx?id={0}">
                                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:HyperLinkField>

                                                            <asp:TemplateField HeaderText="Descrição">
                                                                <ItemTemplate>
                                                                    <div>
                                                                        <asp:LinkButton ID="lnkProdutoDetalhe" runat="server" Text='<%# Eval("sDscProduto") %>'
                                                                            OnClientClick='<%# "openModal(\"" + Eval("idProduto") + "\"); return false;" %>'
                                                                            OnMouseOver='<%# "mostraCard(this, \"" + Eval("idProduto") + "\", \"produto\");" %>'
                                                                            OnMouseOut='<%# "escondeCard( \"" + Eval("idProduto") + "\", \"produto\");" %>'
                                                                            CssClass="epi-detalhe-link"
                                                                            ClientIDMode="Static"
                                                                            data-idproduto='<%# Eval("idProduto") %>'
                                                                            data-tabela="epi" />

                                                                        <div id='<%# Eval("idProduto") + "_produto" %>' class="product-card" style="display: none;">
                                                                            <!--conteudo via script -->
                                                                        </div>
                                                                    </div>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="35%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:BoundField DataField="sUnidade" HeaderText="Unidade">
                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:BoundField>

                                                            <asp:TemplateField HeaderText="Quantidade">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtQtd" class="form-control qtd valor" runat="server" Text='<%# Convert.ToDouble(Eval("nQuantidade")).ToString("N2") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Valor com IPI">
                                                                <ItemTemplate>
                                                                    <asp:TextBox ID="txtValor" class="form-control preco valor" runat="server" Text='<%# Convert.ToDouble(Eval("nValorUnitario")).ToString("N2") %>'></asp:TextBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField HeaderText="Total">
                                                                <ItemTemplate>
                                                                    <asp:Label ID="txtTotal" class="total" runat="server" Text='<%# Convert.ToDouble(Eval("nValorTotal")).ToString("N2") %>'></asp:Label>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="13%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                            <asp:TemplateField>
                                                                <HeaderTemplate>
                                                                    <asp:CheckBox ID="cbExcluir_Todos" runat="server" CssClass="excluirTodos"></asp:CheckBox>
                                                                </HeaderTemplate>
                                                                <HeaderStyle CssClass="text-center" />
                                                                <ItemTemplate>
                                                                    <asp:CheckBox ID="cbExcluir" runat="server" CssClass="form-control excluir"></asp:CheckBox>
                                                                </ItemTemplate>
                                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                            </asp:TemplateField>

                                                        </Columns>
                                                    </asp:GridView>
                                                </div>

                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>

                <div role="tabpanel" class="tab-pane fade" id="historico" aria-labelledby="historico-tab">
                    <br />
                    <div class="panel panel-default" runat="server" id="div_Historico">
                        <div class="panel-heading">
                            <h3 class="panel-title"><b>Histórico</b></h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-stacked row">
                                <div class="col-lg-12 table-responsive">
                                    <asp:GridView ID="gv_Historico" class="table table-striped table-bordered table-hover table-condensed table-responsive"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" OnRowDataBound="gv_Historico_RowDataBound">
                                        <Columns>

                                            <asp:BoundField DataField="sTipoAcao" HeaderText="Tipo">
                                                <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sAcao" HeaderText="Detalhe da Alteração">
                                                <ItemStyle Width="65%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtLog" HeaderText="Em">
                                                <ItemStyle Width="13%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <uc1:PainelAtualizacao runat="server" ID="PainelAtualizacao" />

            <fieldset class="form-stacked actions">
                <asp:HyperLink ID="cmdOrcamento" class="btn btn-lg btn-primary" runat="server" Target="_blank"><i class="fa fa-chevron-left"></i> Orçamento</asp:HyperLink>
                <asp:Button ID="cmdSalvar" class="btn btn-lg btn-success" runat="server" Text="Salvar" />
                <asp:Button ID="cmdEditar" class="btn btn-lg btn-success" runat="server" Text="Editar" />
                <asp:Button ID="cmdVoltar" class="btn btn-lg btn-warning" runat="server" Text="Cancelar" OnClientClick="history.go(-1)" />
                <asp:Button ID="cmdGerarPDF" class="btn btn-lg btn-danger" runat="server" Text="PDF" OnClick="cmdGerarPDF_Click" />
                <asp:HyperLink ID="cmdVincular" class="btn btn-lg btn-primary" runat="server" Text="Novo Orçamento" Target="_blank"></asp:HyperLink>
            </fieldset>

            <div id="dialog-Salvar" class="modal" title="Salvar">
                <p>
                    <label>Deseja Salvar a Cotação?</label>
                </p>
            </div>

            <div id="dialog-Editar" class="modal" title="Editar">
                <p>
                    <label>Deseja Editar a Cotação?</label>
                </p>
            </div>

            <div class="modal fade" id="produtoDetalheModal">
                <div class="modal-dialog" style="width: 35%;">
                    <div class="modal-content" style="display: table; width: 100%;">
                        <div class="modal-header" id="modalInfo">
                        </div>
                        <div class="modal-body" id="modalBody">
                            <!-- conteudo via javascript-->
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="cmdImportarItens_Excel" />
        </Triggers>
    </asp:UpdatePanel>

    <div class="modal fade" id="modalImportarItens_Excel">
        <div class="modal-dialog" style="width: 40%;">
            <div class="modal-content">
                <div class="modal-header">
                    <h3 class="modal-title">Importar Itens via Excel</h3>
                    <button type="button" class="close" data-dismiss="modal">&times;</button>
                </div>
                <div class="modal-body">

                    <div class="row">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="Mensagem_Modal_ImportarItens_Excel" />
                        </div>

                        <uc1:ExcelImportar runat="server" ID="ExcelImportar" />
                    </div>

                </div>
                <div class="modal-footer">
                    <asp:Button ID="cmdImportarItens_Excel" class="btn btn-success" runat="server" Text="Importar" OnClick="cmdImportarItens_Excel_Click" />
                    <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                </div>
            </div>
        </div>
    </div>

    <div id="importarExcel" style="display: none;">
        <asp:FileUpload runat="server" ID="ImportarItens_Excel" accept=".xls, .xlsx" />
    </div>

</asp:Content>