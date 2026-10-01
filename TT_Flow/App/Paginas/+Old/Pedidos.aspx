<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Pedidos.aspx.cs" Inherits="TT_Flow.App.Pedidos" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:manual runat="server" ID="manual" />

    <script type="text/javascript">
        $(function () {
            $('[id*=txtdtInicio]').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            });

            $('[id*=txtdtFinal]').datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            });

            $('[id*=txtdtInicio]').mask('99/99/9999');
            $('[id*=txtdtFinal]').mask('99/99/9999');
        });
    </script>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>

        <div class="col-lg-12" runat="server" id="DIV_Filtro">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa</h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <asp:UpdatePanel ID="UpdFiltros" runat="server">
                            <ContentTemplate>

                                <div class="col-lg-2">
                                    <div class="row">
                                        <div class="col-lg-6">
                                            <asp:TextBox ID="txtdtInicio" class="form-control CaixaTextoData" placeholder="Data Inicial" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                        </div>
                                        <div class="col-lg-6">
                                            <asp:TextBox ID="txtdtFinal" class="form-control CaixaTextoData" runat="server" placeholder="Data Final" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-lg-2 form-group" runat="server" id="DIV_Empresa">
                                    <asp:DropDownList ID="ddlidEmpresa" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group" runat="server" id="DIV_Financeiro">
                                    <asp:DropDownList ID="ddlFinanceiro" runat="server" class="Caixa_Selecao form-control">
                                        <asp:ListItem Value="0" Text="Todos os Status Financeiro" />
                                        <asp:ListItem Value="1" Text="Liquidado" />
                                        <asp:ListItem Value="2" Text="Parcial" />
                                        <asp:ListItem Value="3" Text="Aberto" />
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-4 form-group" runat="server" id="DIV_Importador">
                                    <asp:DropDownList ID="ddlImportador" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                                </div>

                                <div class="col-lg-4 form-group" runat="server" id="DIV_Despachante">
                                    <asp:DropDownList ID="ddlImportacao_idDespachante" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                                </div>

                                <div class="col-lg-4 form-group">
                                    <asp:DropDownList ID="ddlCliente" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group" runat="server" id="DIV_PaisOrigem">
                                    <asp:DropDownList ID="ddlImportacao_idPais" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group" runat="server" id="DIV_idModal">
                                    <asp:DropDownList ID="ddlidModal" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                                </div>

                                <div class="col-lg-4 form-group" runat="server" id="DIV_Departamento">
                                    <asp:DropDownList ID="ddlDepartamento" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                                </div>

                                <div class="col-lg-4 form-group" runat="server" id="DIV_Status">
                                    <asp:DropDownList ID="ddlStatus" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                                </div>

                                <div class="col-lg-3 form-group" runat="server" id="DIV_Fluxo">
                                    <asp:ListBox ID="ddlFluxo" runat="server" class="Caixa_Selecao form-control" SelectionMode="Multiple"></asp:ListBox>
                                </div>

                                <div class="col-lg-2 form-group" runat="server" id="div_ddlPedidoCliente">
                                    <asp:DropDownList ID="ddlPedidoCliente" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group" runat="server" id="DIV_Tipos">
                                    <asp:DropDownList ID="ddlTipos" runat="server" class="form-control Caixa_Selecao">
                                        <asp:ListItem Value="">Selecione o Tipo</asp:ListItem>
                                        <asp:ListItem Value="N">Nacional</asp:ListItem>
                                        <asp:ListItem Value="I">Internacional</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>

                            </ContentTemplate>
                        </asp:UpdatePanel>

                        <div class="col-lg-2 form-group" runat="server" id="DIV_Button">
                            <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClientClick="MudarBotao_Pesquisa(this)" OnClick="cmdPesquisar_Click" />
                            <asp:Button ID="cmdNovoPedido" class="btn btn-success" runat="server" Text="Novo Pedido" OnClick="cmdNovoPedido_Click" />
                        </div>

                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-12">

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

            <asp:Panel ID="pnResultado" class="" runat="server">
                <div id="resultado">
                    <div class="panel panel-primary">
                        <div class="panel-body">

                            <div class="table-responsive">

                                <asp:GridView ID="gvPedidos" runat="server" AutoGenerateColumns="false" CellPadding="0" CellSpacing="1"
                                    class="table table-striped table-bordered table-hover table-condensed table-responsive tablesorter" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvPedidos_RowDataBound">
                                    <Columns>

                                        <%-- 0 --%>
                                        <asp:HyperLinkField DataNavigateUrlFields="idPedido, idTipo"
                                            DataTextField="nNumeroPedido" HeaderText="ID"
                                            DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}&sTp={1}">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <%-- 1 --%>
                                        <asp:HyperLinkField DataNavigateUrlFields="idPedido, idTipo"
                                            DataTextField="sPedidoCompras" HeaderText="Pedido de Compras"
                                            DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}&sTp={1}">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <%-- 2 --%>
                                        <asp:HyperLinkField DataNavigateUrlFields="idPedido, idTipo"
                                            DataTextField="sPedidoCliente" HeaderText="Pedido Cliente"
                                            DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}&sTp={1}">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <%-- 3 --%>
                                        <asp:HyperLinkField DataNavigateUrlFields="idPedido, idTipo"
                                            DataTextField="nControleTT" HeaderText="N° Controle TT"
                                            DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}&sTp={1}">
                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <%-- 4 --%>
                                        <asp:HyperLinkField DataNavigateUrlFields="idPedido, idTipo"
                                            DataTextField="sReferencia" HeaderText="Referência"
                                            DataNavigateUrlFormatString="Pedidos_Detalhe.aspx?id={0}&sTp={1}">
                                            <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <%-- 5 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscCliente" HeaderText="Cliente" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="15%" />
                                        </asp:BoundField>

                                        <%-- 6 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDsc_Fornecedor" HeaderText="Exportador" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="18%" />
                                        </asp:BoundField>

                                        <%-- 7 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDsc_Despachante" HeaderText="Despachante" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="18%" />
                                        </asp:BoundField>

                                        <%-- 8 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDsc_Importador" HeaderText="Importador" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="18%" />
                                        </asp:BoundField>

                                        <%-- 9 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtPedido" HeaderText="Data Pedido" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="6%" />
                                        </asp:BoundField>

                                        <%-- 10 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscFluxo" HeaderText="Fluxo" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </asp:BoundField>

                                        <%-- 11 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="sStatus" HeaderText="Status Financeiro" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="12%" />
                                        </asp:BoundField>

                                        <%-- 12 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDsc_Pais" HeaderText="Origem" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="8%" />
                                        </asp:BoundField>

                                        <%-- 13 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscModal" HeaderText="Modal" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="8%" />
                                        </asp:BoundField>

                                        <%-- 14 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscStatus" HeaderText="Status" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </asp:BoundField>

                                        <%-- 15 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscDepartamento" HeaderText="Departamento" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="10%" />
                                        </asp:BoundField>

                                        <%-- 16 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscTipoEnvio" HeaderText="Forma de Envio" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="6%" />
                                        </asp:BoundField>

                                        <%-- 17 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtEstimativaEntrega" HeaderText="Estimativa Entrega" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="6%" />
                                        </asp:BoundField>

                                        <%-- 18 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtPrevisaoEntregaFormatada" HeaderText="Previsão Entrega" HtmlEncode="False" HtmlEncodeFormatString="False">
                                            <ItemStyle HorizontalAlign="Left" Width="6%" />
                                        </asp:BoundField>

                                        <%-- 19 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="vlrTotal" HeaderText="Valor Total" HtmlEncode="False" HtmlEncodeFormatString="False" DataFormatString="{0:N2}">
                                            <ItemStyle HorizontalAlign="Left" Width="7%" />
                                        </asp:BoundField>

                                        <%-- 20 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="PrevistoFaturar" HeaderText="Previsto Faturar" HtmlEncode="False" HtmlEncodeFormatString="False" DataFormatString="{0:N2}">
                                            <ItemStyle HorizontalAlign="Left" Width="7%" />
                                        </asp:BoundField>

                                        <%-- 21 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="vlrFaturado" HeaderText="Valor Faturado" HtmlEncode="False" HtmlEncodeFormatString="False" DataFormatString="{0:N2}">
                                            <ItemStyle HorizontalAlign="Left" Width="7%" />
                                        </asp:BoundField>

                                        <%-- 22 --%>
                                        <asp:BoundField ConvertEmptyStringToNull="False" DataField="saldo" HeaderText="Saldo" HtmlEncode="False" HtmlEncodeFormatString="False" DataFormatString="{0:N2}">
                                            <ItemStyle HorizontalAlign="Left" Width="7%" />
                                        </asp:BoundField>

                                        <%-- 23 --%>
                                        <asp:BoundField DataField="idPedido" HeaderText="ID">
                                            <HeaderStyle CssClass="invisivel" />
                                            <ItemStyle CssClass="invisivel" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>

                                <asp:HiddenField ID="hddidTipo" runat="server" />
                                <asp:HiddenField ID="hddidFluxo" runat="server" />
                                <asp:HiddenField ID="hddidEmpresa" runat="server" />

                            </div>

                        </div>
                    </div>
                </div>
            </asp:Panel>

        </div>
    </div>

</asp:Content>
