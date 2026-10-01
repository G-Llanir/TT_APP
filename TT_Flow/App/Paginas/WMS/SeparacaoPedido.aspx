<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="SeparacaoPedido.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.SeparacaoPedido" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/FiltroPesquisa.ascx" TagPrefix="uc1" TagName="FiltroPesquisa" %>

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
                <br />
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" runat="server" placeholder="Pesquisar"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-lg-3" runat="server" id="DivTipoCompra">
                                <div class="form-group">
                                    <%--<label>Status Separação</label>--%>
                                    <asp:DropDownList ID="ddlsSeparacao" runat="server" class="form-control yes_no select Caixa_Selecao">
                                        <asp:ListItem Value="">Selecione o Status da Separação</asp:ListItem>
                                        <asp:ListItem Value="N">Pendente</asp:ListItem>
                                        <asp:ListItem Value="S">Separado</asp:ListItem>
                                        <asp:ListItem Value="E">Em Andamento</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
                <div class="col-lg-12">
                    <asp:Panel ID="pnResultado" runat="server">
                        <div class="panel panel-primary">
                            <div class="panel-body ">
                                <asp:GridView ID="dtgvConsulta" runat="server" class="table table-condensed table-striped table-bordered table-hover" GridLines="None"
                                    CellSpacing="1" CellPadding="1" Width="100%" ShowFooter="False" Font-Names="Tahoma"
                                    Font-Overline="False" Font-Size="Small" AutoGenerateColumns="false" OnRowDataBound="dtgvConsulta_RowDataBound">
                                    <Columns>

                                        <asp:BoundField DataField="idOPI" HeaderText="ID">
                                            <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtOPI" HeaderText="Data OPI">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                            DataTextField="sDscOPI" HeaderText="Descrição"
                                            DataNavigateUrlFormatString="SeparacaoPedido_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="30%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="nPedido" HeaderText="N° Pedido">
                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sReferencia" HeaderText="Referência">
                                            <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sSeparacao" HeaderText="Status Separação">
                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sStatus" HeaderText="Status Envios">
                                            <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnMensagem" runat="server">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </asp:Panel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
