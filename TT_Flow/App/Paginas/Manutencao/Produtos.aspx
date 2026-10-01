<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Produtos.aspx.cs" Inherits="TT_Hub.App.Paginas.Manutencao.Produtos" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/ProdutosImportador.ascx" TagPrefix="uc1" TagName="ProdutosImportador" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <uc1:Manual runat="server" ID="manual" />

            <asp:HiddenField runat="server" ID="hddsManual" />

            <script type="text/javascript">
                function MudarBotao() {
                    document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
                }
            </script>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Produtos / Serviços</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>

                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">
                            
                            <div class="col-lg-4">
                                 <div class="form-group">
                                     <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                 </div>
                             </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlTipoProduto" runat="server" class="form-control Caixa_Selecao" attrname="Tipo"></asp:DropDownList>
                                </div>
                            </div>
                            <div runat="server" id="div_Grupo" class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlGrupo" runat="server" class="form-control Caixa_Selecao" attrname="Tipo"></asp:DropDownList>
                                </div>
                            </div>
                            <div runat="server" id="div_Familia" class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlFamilia" runat="server" class="form-control Caixa_Selecao" attrname="Tipo"></asp:DropDownList>
                                </div>
                            </div>
                            <div runat="server" id="div_LocalArmazenamento" class="col-lg-3">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlLocalArmazenamento" runat="server" class="form-control Caixa_Selecao" attrname="Tipo"></asp:DropDownList>
                                </div>
                            </div>

                            <div runat="server" id="div_Pais" class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlPaisOrigem" runat="server" class="form-control Caixa_Selecao" attrname="Tipo"></asp:DropDownList>
                                </div>
                            </div>
                            <div runat="server" id="div_CategoriaVenda" class="col-lg-3">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlCategoriaVenda" runat="server" class="form-control Caixa_Selecao" attrname="Tipo"></asp:DropDownList>
                                </div>
                            </div>

                            <div runat="server" id="div_CodigoNCM" class="col-lg-2">
                                <div class="form-group">
                                    <asp:TextBox ID="txtsCodigoNCM" class="form-control" placeholder="Pesquisar NCM" runat="server"></asp:TextBox>
                                </div>
                            </div>
                           
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsExibeComercial" runat="server" class="form-control Caixa_Selecao">
                                        <asp:ListItem Text="Todos os Status em Comercial " Value="T" />
                                        <asp:ListItem Text="Apenas Liberados em Comercial" Value="S" />
                                        <asp:ListItem Text="Apenas Bloqueados em Comercial" Value="N" />
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div runat="server" id="div_ExibeLM" class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsExibeLM" runat="server" class="form-control Caixa_Selecao">
                                        <asp:ListItem Text="Todos os Status em LME " Value="T" />
                                        <asp:ListItem Text="Apenas Liberados em LME" Value="S" />
                                        <asp:ListItem Text="Apenas Bloqueados em LME" Value="N" />
                                    </asp:DropDownList>
                                </div>
                            </div>



                            <div runat="server" id="div_ExibeComposicao" class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsExibeComposicao" runat="server" class="form-control Caixa_Selecao">
                                        <asp:ListItem Text="Todos os Status de Composição" Value="T" />
                                        <asp:ListItem Text="Apenas Liberados a Exibição de Composição" Value="S" />
                                        <asp:ListItem Text="Apenas Bloqueados a Exibição de Composição" Value="N" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                             <div class="col-lg-2">
                                 <div class="form-group">
                                     <asp:DropDownList ID="ddlSituacaoCadastral" runat="server" class="form-control Caixa_Selecao">
                                         <asp:ListItem Text="Todas as Classificações Cadastrais" Value="" Selected="True" />
                                         <asp:ListItem Text="Produtos Classificados" Value="S" />
                                         <asp:ListItem Text="Produtos não classificado" Value="N" />
                                     </asp:DropDownList>
                                 </div>
                             </div>

                            <div runat="server" id="div_sSituacao" class="col-lg-1">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsSituacao" runat="server" class="form-control Caixa_Selecao">
                                        <asp:ListItem Text="Ativos" Value="S" />
                                        <asp:ListItem Text="Inativos" Value="N" />
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div runat="server" id="div_sSubTipo" class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlsSubTipo" runat="server" class="form-control Caixa_Selecao">
                                        <asp:ListItem Text="Todos" Value="0" />
                                        <asp:ListItem Text="Apenas Serviços" Value="1" />
                                        <asp:ListItem Text="Apenas Sub-Serviços" Value="2" />
                                    </asp:DropDownList>
                                </div>
                            </div>
           
                            <div class="col-lg-6">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                                    <asp:Button ID="cmdExportar" class="btn btn-info" runat="server" Text="Exportar Excel" OnClick="cmdExportar_Click" />
                                    <button id="cmdImportar" runat="server" type="button" class="btn btn-primary" data-toggle="modal" data-target="#modalImportarEAN" visible="false">
                                        Importar EAN Excel
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>

            </div>

            <div class="col-lg-12">
                <asp:Panel ID="pnResultado" CssClass="row" runat="server" Style="display: none">
                    <div id="resultado">
                        <div class="panel panel-primary">
                            <div class="panel-body">
                                <div class="table-responsive">
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="13px"
                                        OnRowCommand="dtgvConsulta_RowCommand" OnRowDataBound="dtgvConsulta_RowDataBound">
                                        <Columns>

                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                DataTextField="idItem" HeaderText="ID"
                                                DataNavigateUrlFormatString="Produtos_Detalhe.aspx?id={0}">
                                                <HeaderStyle Width="2%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                DataTextField="sCodigo" HeaderText="Código"
                                                DataNavigateUrlFormatString="Produtos_Detalhe.aspx?id={0}">
                                                <HeaderStyle Width="15%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                DataTextField="sDscProduto" HeaderText="Descrição"
                                                DataNavigateUrlFormatString="Produtos_Detalhe.aspx?id={0}">
                                                <HeaderStyle Width="45%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:BoundField DataField="sCodigoNCM" HeaderText="Código NCM">
                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscTipoProduto" HeaderText="Tipo Produto">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sExibeComercial_Completo" HeaderText="Exibe Comercial">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sSituacao_Completa" HeaderText="Situação">
                                                <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Situação Cadastral">
                                                <ItemTemplate>
                                                    <%# Eval("sCadeado").ToString() == "S" ? "Bloqueado" : Eval("sSituacaoCadastral_Completa").ToString() %>
                                                </ItemTemplate>
                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                                <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Duplicar">
                                                <ItemTemplate>
                                                    <asp:LinkButton ID="lnkDuplicar" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Duplicar" CommandArgument='<%# Eval("idItem") %>'><i class="fa fa-files-o"></i></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="idCadeado" HeaderText="Cadeado" Visible="false">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>
                                        </Columns>

                                        <PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
                                        <EditRowStyle BackColor="#2461BF" />
                                    </asp:GridView>
                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnMensagem" class="row" runat="server">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

    <div class="modal fade" id="modalImportarEAN" tabindex="-1" role="dialog" aria-labelledby="modalImportarEANLabel" aria-hidden="true" data-backdrop="static">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <h4 class="modal-title" id="modalImportarEANLabel">Importar EAN de Produtos via Excel</h4>
                </div>
                <div class="modal-body">
                    <asp:UpdatePanel ID="updImportador" runat="server" UpdateMode="Conditional">
                        <ContentTemplate>
                            <div class="form-group">
                                <uc1:ProdutosImportador runat="server" ID="ProdutosImportador" ModalId="modalImportarEAN" />
                            </div>
                        </ContentTemplate>
                        <Triggers>
                        <%-- ✨ CORREÇÃO AQUI: Remova o EventName ✨ --%>
                        <asp:PostBackTrigger ControlID="ProdutosImportador$btnIniciarImportacao" />
                    </Triggers>
                    </asp:UpdatePanel>
                </div>
                <div class="modal-footer">
                    <asp:LinkButton  id="cmdFecharModal" runat="server" type="button" CssClass="btn btn-secondary" OnClick="cmdFecharModal_Click">Fechar</asp:LinkButton>
                </div>
            </div>
        </div>
    </div>
</asp:Content>
