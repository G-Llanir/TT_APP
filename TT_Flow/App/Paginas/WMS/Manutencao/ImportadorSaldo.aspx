<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ImportadorSaldo.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Manutencao.ImportadorArquivo" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <script type="text/javascript">
                function MudarBotao() {
                    document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
                }
            </script>


            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Manutenção</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">
                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control CaixaTextoGrande" runat="server" placeholder="Pesquisar"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-4">
                                <asp:DropDownList ID="ddlTipoProduto" runat="server" class="form-control CaixaTextoMedio" attrname="Tipo"></asp:DropDownList>
                            </div>
                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()"/>
                                    <asp:Button ID="cmdNovoCadastro" class="btn btn-success" runat="server" Text="Importar Dados" OnClick="cmdNovoCadastro_Click"/>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="col-lg-12">
                <asp:Panel ID="pnResultado" CssClass="row" runat="server" Style="display: none">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                            <asp:GridView ID="dtgvConsulta" runat="server" class="table table-striped table-bordered table-hover" GridLines="None"
                                CellSpacing="1" CellPadding="1" Width="100%" ShowFooter="False" Font-Names="Tahoma"
                                Font-Overline="False" Font-Size="Small" AutoGenerateColumns="false">
                                <Columns>
                                    <asp:BoundField DataField="sCodigo" HeaderText="Codigo">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscProduto" HeaderText="Descrição">
                                        <ItemStyle Width="54%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nEstoqueAtual" HeaderText="Estoque atual">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                        <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                        <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </asp:Panel>
                <asp:Panel ID="pnMensagem" CssClass="row" runat="server">
                    <asp:Label ID="lblMensagem" class="warning" runat="server" Text=""></asp:Label>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
