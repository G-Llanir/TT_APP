<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="CategoriaContasPagar.aspx.cs" Inherits="TT_Hub.App.Paginas.Adm.Manutencao.CategoriaContasPagar" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <script type="text/javascript">

        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }


        $(function () {


        });

    </script>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Categoria Contas Pagar"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">
                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar" runat="server" style="margin-top: 25px;"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <label>Exibir no Gráfico Caixa</label>
                                <asp:DropDownList ID="ddlsDataExibirCaixa" class="form-control " runat="server">
                                    <asp:ListItem Text="Todas as Categorias" Value="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Não Exibir" Value="N"></asp:ListItem>
                                    <asp:ListItem Text="Por Vencimento" Value="V"></asp:ListItem>
                                    <asp:ListItem Text="Por Emissão" Value="E"></asp:ListItem>
                                    <asp:ListItem Text="Por Apuração" Value="A"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <label>Exibir no Gráfico Contábil</label>
                                <asp:DropDownList ID="ddlsDataExibirContabil" class="form-control " runat="server">
                                    <asp:ListItem Text="Todas as Categorias" Value="" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Não Exibir" Value="N"></asp:ListItem>
                                    <asp:ListItem Text="Por Vencimento" Value="V"></asp:ListItem>
                                    <asp:ListItem Text="Por Emissão" Value="E"></asp:ListItem>
                                    <asp:ListItem Text="Por Apuração" Value="A"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>


                        <div class="col-lg-2" style="margin-top: 25px;">
                            <div class="form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Nova Categoria" OnClick="cmdNovo_Click" />

                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="row">
                <div class="col-lg-12">
                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    <asp:Panel ID="pnResultado" class="" runat="server">

                        <div id="resultado">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">


                                            <Columns>

                                                <asp:HyperLinkField DataNavigateUrlFields="idCategoriaPagar"
                                                    DataTextField="idCategoriaPagar" HeaderText="ID"
                                                    DataNavigateUrlFormatString="CategoriaContasPagar_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="1%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idCategoriaPagar"
                                                    DataTextField="sDscCategoriaPagarPai" HeaderText="Controle de categorias"
                                                    DataNavigateUrlFormatString="CategoriaContasPagar_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="19%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idCategoriaPagar"
                                                    DataTextField="sDscCategoriaPagar" HeaderText="Descrição Categoria"
                                                    DataNavigateUrlFormatString="CategoriaContasPagar_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="20%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idCategoriaPagar"
                                                    DataTextField="sDscCategoriaTipo" HeaderText="Tipo"
                                                    DataNavigateUrlFormatString="CategoriaContasPagar_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="12%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idCategoriaPagar"
                                                    DataTextField="sDscCodContabil" HeaderText="Código Contábil"
                                                    DataNavigateUrlFormatString="CategoriaContasPagar_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="48%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>


                                                <asp:TemplateField HeaderText="É Despesa">
                                                    <HeaderStyle Width="5%" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litDespesa" runat="server"></asp:Literal>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Empréstimo">
                                                    <HeaderStyle Width="5%" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litEmprestimo" runat="server"></asp:Literal>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="É Custo Fixo">
                                                    <HeaderStyle Width="5%" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litDespesaFixa" runat="server"></asp:Literal>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Produtos Financeiros">
                                                    <HeaderStyle Width="5%" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litProdutoFinanceiro" runat="server"></asp:Literal>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                                <asp:TemplateField HeaderText="Exibir no Dashboard">
                                                    <HeaderStyle Width="5%" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litExibirDash" runat="server"></asp:Literal>
                                                    </ItemTemplate>
                                                </asp:TemplateField>

                                               <%-- <asp:TemplateField HeaderText="Relatório de Despesas">
                                                    <HeaderStyle Width="5%" />
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemTemplate>
                                                        <asp:Literal ID="litExibirDespesas" runat="server"></asp:Literal>
                                                    </ItemTemplate>
                                                </asp:TemplateField>--%>

                                                <asp:BoundField DataField="sDataExibirCaixa" HeaderText="Exibir no Gráfico Caixa">
                                                    <ItemStyle Width="7%" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDataExibirContabil" HeaderText="Exibir no Gráfico Contábil">
                                                    <ItemStyle Width="7%" />
                                                </asp:BoundField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>

                </div>
            </div>

        </div>

    </div>

</asp:Content>
