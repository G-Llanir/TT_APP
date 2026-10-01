<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Status.aspx.cs" Inherits="TT_Hub.App.Paginas.Manutencao.Status" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>


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
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlsTipo" runat="server" class="form-control yes_no select">
                                            <asp:ListItem Selected="True" Value="">Selecione um Tipo</asp:ListItem>
                                            <asp:ListItem Value="Orcamento">Orçamento</asp:ListItem>
                                            <asp:ListItem Value="CRM">CRM</asp:ListItem>
                                            <asp:ListItem Value="Pedido">Pedido</asp:ListItem>
                                            <asp:ListItem Value="Comex">Comex</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                        <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>


                <div class="col-lg-12">
                    <asp:Panel ID="pnResultado" class="" runat="server">
                        <div id="resultado">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                            <Columns>

                                                <asp:HyperLinkField DataNavigateUrlFields="idStatus"
                                                    DataTextField="idStatus" HeaderText="ID"
                                                    DataNavigateUrlFormatString="Status_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idStatus"
                                                    DataTextField="sDscStatus" HeaderText="Descrição"
                                                    DataNavigateUrlFormatString="Status_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="35%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:BoundField DataField="sTipo" HeaderText="Tipo">
                                                    <HeaderStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle"/>
                                                    <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sExibeCliente" HeaderText="Exibe para Cliente">
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sLiberadoFaturamento" HeaderText="Liberado para Faturamento">
                                                    <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                                    <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                                    <ItemStyle Width="18%" HorizontalAlign="Left" VerticalAlign="Middle" />
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
                    <asp:Panel ID="pnMensagem" CssClass="row" runat="server">
                        <asp:Label ID="lblMensagem" class="warning" runat="server" Text="Label"></asp:Label>
                    </asp:Panel>

                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
