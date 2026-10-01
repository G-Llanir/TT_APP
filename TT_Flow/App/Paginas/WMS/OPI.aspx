<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="OPI.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.OPI" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <script type="text/javascript">
        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }
    </script>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="OPI"></asp:Label><small> Ordem de Produção Interna</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">


                        <div class="col-lg-2">
                            <asp:DropDownList ID="ddlStatus" runat="server" class="form-control">
                                <asp:ListItem class="danger" Value="0">Selecione um Status</asp:ListItem>
                                <asp:ListItem Selected="True" Value="1">Pendente</asp:ListItem>
                                <asp:ListItem Value="3">Envio Parcial</asp:ListItem>
                                <asp:ListItem Value="4">Finalizada</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlCliente" runat="server" AutoPostBack="false" class="form-control yes_no select Caixa_Selecao"></asp:DropDownList>
                            </div>

                        </div>
                        <div class="col-lg-2">
                            <div class="form-group">
                                <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
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

                                                <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                                    DataTextField="idOPI" HeaderText="Nº da OPI"
                                                    DataNavigateUrlFormatString="OPI_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="8%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                                    DataTextField="dtOPI" HeaderText="Data OPI"
                                                    DataNavigateUrlFormatString="OPI_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                     <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                    DataTextField="idPedido" HeaderText="Nº Pedido"
                                                    DataNavigateUrlFormatString="../Pedidos_Detalhe.aspx?id={0}&sTp=2">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                                    DataTextField="sReferencia" HeaderText="Referência"
                                                    DataNavigateUrlFormatString="OPI_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="20%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                                    DataTextField="sCliente" HeaderText="Cliente"
                                                    DataNavigateUrlFormatString="OPI_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="25%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                    DataTextField="nPedido" HeaderText="Nº Pedido Cliente"
                                                    DataNavigateUrlFormatString="../Pedidos_Detalhe.aspx?id={0}&sTp=2">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>


                                                <asp:HyperLinkField DataNavigateUrlFields="idOPI"
                                                    DataTextField="sStatus" HeaderText="Status"
                                                    DataNavigateUrlFormatString="OPI_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

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
