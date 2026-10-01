<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Tabelas.aspx.cs" Inherits="TT_Flow.App.Paginas.Tabelas" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/ExcelImportar.ascx" TagPrefix="uc1" TagName="ExcelImportar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server" UpdateMode="Conditional">
        <ContentTemplate>

            <asp:HiddenField runat="server" ID="hddTabela" Value="0" />

            <uc1:Manual runat="server" ID="manual" />

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Consulta</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">

                            <div class="row">

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="div_TipoTabela">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlidTipoTabela" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlTerritorioTabela" runat="server" class="form-control Caixa_Selecao">
                                            <asp:ListItem Value="0">Todos os Territórios</asp:ListItem>
                                            <asp:ListItem Value="1">Nacional</asp:ListItem>
                                            <asp:ListItem Value="2">Internacional</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlidMoedaOrigem" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlidMoedaDestino" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                            </div>

                            <div class="row">

                                <div class="col-lg-2">
                                    <asp:DropDownList ID="ddlsValidada" runat="server" class="form-control Caixa_Selecao">
                                        <asp:ListItem Value="0">Todas as Validações</asp:ListItem>
                                        <asp:ListItem Value="S">Validada</asp:ListItem>
                                        <asp:ListItem Value="N">Não Validada</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-2">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisa(this)" />
                                    <asp:HyperLink ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" NavigateUrl="/App/Paginas/Comercial/Tabelas_Detalhe.aspx?id=0" Target="_blank"></asp:HyperLink>
                                </div>

                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

            <asp:Panel ID="pnResultado" runat="server">
                <div id="resultado">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">
                                <asp:GridView ID="gvConsulta" class="table table-striped table-bordered table-hover " DataKeyNames="idTipoTabela"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvConsulta_RowDataBound" OnRowCommand="gvConsulta_RowCommand">
                                    <Columns>

                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                            DataTextField="idTabela" HeaderText="ID"
                                            DataNavigateUrlFormatString="Tabelas_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="2%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idTabela"
                                            DataTextField="sDscTabela" HeaderText="Título"
                                            DataNavigateUrlFormatString="Tabelas_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="sObservacao" HeaderText="Observação">
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscParceiro" HeaderText="Parceiro Vinculado">
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscTipoTabela" HeaderText="Tipo de Tabela">
                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sSimboloMoedaOrigem" HeaderText="Moeda Origem">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sSimboloMoedaDestino" HeaderText="Moeda Destino">
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Data de Atualização">
                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Atualizado Por">
                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Atualizado Por">
                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:TemplateField HeaderText="Ações - Itens">
                                            <ItemTemplate>
                                                <div class="btn-group" style="display: flex; flex-wrap: nowrap;">
                                                    <asp:LinkButton runat="server" ID="lnkExibicao" class="btn btn-info" data-toggle="tooltip_top" title="Exportar LPU" CommandName="LPU" CommandArgument='<%# Eval("idTabela") %>'><i class="fa fa-download"></i></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="lnkExportar" class="btn btn-success" data-toggle="tooltip_top" title="Exportar Itens" CommandName="Exportar" CommandArgument='<%# Eval("idTabela") %>'><i class="fa fa-download"></i></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="lnkImportar" class="btn btn-primary" data-toggle="tooltip_top" title="Importar Itens" CommandName="Importar" CommandArgument='<%# string.Format("{0} - {1}", Eval("idTabela"), Eval("sDscTabela")) %>'><i class="fa fa-upload"></i></asp:LinkButton>
                                                </div>
                                            </ItemTemplate>
                                            <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <div id="importarExcel" style="display: none;">
                <asp:FileUpload runat="server" ID="ImportarArquivo" accept=".xls, .xlsx" />
            </div>

            <div class="modal fade" id="modalExportar_LPU">
                <div class="modal-dialog" style="width: 65%;">
                    <div class="modal-content" style="overflow: visible;">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Exportar LPU</h4>
                        </div>
                        <div class="modal-body">

                            <div class="row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="MensagemPagina_ModalExportar_LPU" />
                                </div>

                                <div class="col-lg-12 form-group">
                                    <label>Visualizações para Exportação</label>
                                    <asp:Literal runat="server" ID="ltrVisualizacao"></asp:Literal>
                                    <asp:HiddenField runat="server" ID="hddVisualizacao" Value="0" />
                                </div>

                                <div class="col-lg-12">
                                    <asp:CheckBox runat="server" ID="cbProdutosCliente" Text="Produtos de Clientes" CssClass="btn btn-primary float-r" />
                                </div>
                            </div>

                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="cmdExcluirVisualizacao" class="btn btn-danger" Style="float: left;" runat="server" Text="Excluir" OnClick="cmdExcluirVisualizacao_Click" />
                            <asp:Button ID="cmdExportarExcel" class="btn btn-success" runat="server" Text="Exportar LPU" OnClick="cmdExportarExcel_Click" />
                            <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>
                        </div>
                    </div>
                </div>
            </div>

            <div class="modal fade" id="modalUpload_ImportarExcel">
                <div class="modal-dialog" style="width: 35%;">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h4 class="modal-title">Importar Excel</h4>
                        </div>
                        <div class="modal-body">

                            <div class="row">
                                <div class="col-lg-12">
                                    <uc1:MensagemPagina runat="server" ID="Mensagem_Modal_ImportarProdutos" />
                                </div>
                                <div class="col-lg-12 form-group">
                                    <label>Tabela de Preços</label>
                                    <asp:TextBox runat="server" ID="txtTabela_ImportarProdutos" ReadOnly="true" CssClass="form-control"></asp:TextBox>
                                </div>

                                <uc1:ExcelImportar runat="server" ID="ExcelImportar" />
                            </div>

                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="cmdImportarItens_Excel_Modal" class="btn btn-success" runat="server" Text="Importar" OnClick="cmdImportarItens_Excel_Modal_Click" />
                            <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:PostBackTrigger ControlID="gvConsulta" />
            <asp:PostBackTrigger ControlID="cmdImportarItens_Excel_Modal" />
            <asp:PostBackTrigger ControlID="cmdImportarItens_Excel_Modal" />
        </Triggers>
    </asp:UpdatePanel>

</asp:Content>