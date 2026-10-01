<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Tarefas_STSO.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.Tarefas_STSO" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <script type="text/javascript">

        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }

    </script>

    <uc1:Manual runat="server" ID="manual" />

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="STSO"></asp:Label><small> Consulta</small>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>

        <div class="col-lg-12" runat="server" id="DIV_Filtro">

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <div class="col-lg-12">
                            <div class="row">
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2" runat="server" id="DIV_Status">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlStatus" runat="server" AutoPostBack="false" class="Caixa_Selecao form-control">
                                            <asp:ListItem Value="0" Text="Todos os Status" />
                                            <asp:ListItem Value="1" Text="Pendente" />
                                            <asp:ListItem Value="2" Text="Tarefa Iniciada" />
                                            <asp:ListItem Value="3" Text="Criação" />
                                            <asp:ListItem Value="4" Text="Envio" />
                                            <asp:ListItem Value="5" Text="Tarefa Finalizada" />
                                            <asp:ListItem Value="6" Text="Download" />
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClientClick="MudarBotao()" OnClick="cmdPesquisar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <div class="col-lg-12">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>

        <div class="col-lg-12" runat="server" id="Grid_Pedidos">
            <asp:Panel ID="pnSTSO" runat="server">
                <div class="panel panel-primary">
                    <div class="panel-body">

                        <button type="button" class="btn" data-toggle="collapse" data-target="#toggleButtons" style="background-color: #009a22; color: white;">
                            Mostrar/Ocultar Colunas
                        </button>
                        <div id="toggleButtons" class="collapse">
                            <br />
                            <asp:PlaceHolder ID="placeholderButtons" runat="server"></asp:PlaceHolder>
                        </div>
                        <br />
                        <br />

                        <div class="table-responsive">

                            <asp:GridView ID="gvPedidos" runat="server" AutoGenerateColumns="false" CellPadding="0" CellSpacing="1"
                                class="table table-striped table-bordered table-hover table-condensed table-responsive tablesorter" GridLines="None"
                                ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvPedidos_RowDataBound">

                                <Columns>

                                    <asp:HyperLinkField DataNavigateUrlFields="idPedido, idTipo"
                                        DataTextField="nNumeroPedido" HeaderText="Pedido"
                                        DataNavigateUrlFormatString="/App/Paginas/Pedidos_Detalhe.aspx?id={0}&sTp={1}">
                                        <HeaderStyle Width="4%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idPedido, idTipo"
                                        DataTextField="sReferencia" HeaderText="Referência"
                                        DataNavigateUrlFormatString="/App/Paginas/Pedidos_Detalhe.aspx?id={0}&sTp={1}">
                                        <HeaderStyle Width="10%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idPedido, idTipo"
                                        DataTextField="sDscCliente" HeaderText="Cliente"
                                        DataNavigateUrlFormatString="/App/Paginas/Pedidos_Detalhe.aspx?id={0}&sTp={1}">
                                        <HeaderStyle Width="18%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtPedido" HeaderText="Data" HtmlEncode="False" HtmlEncodeFormatString="False">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="6%" />
                                    </asp:BoundField>

                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscFluxo" HeaderText="Fluxo" HtmlEncode="False" HtmlEncodeFormatString="False">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="7%" />
                                    </asp:BoundField>

                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="sDscTarefa" HeaderText="Tarefa" HtmlEncode="False" HtmlEncodeFormatString="False">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="7%" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtHistorico" HeaderText="Tarefa Iniciada">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="8%" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtCriacao" HeaderText="Data Criação">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="8%" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtEnvio" HeaderText="Data Envio">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="8%" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtConclusao" HeaderText="Tarefa Conclusão">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="9%" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtDownload" HeaderText="Data Download">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="8%" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sStatus" HeaderText="Status">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="7%" />
                                    </asp:BoundField>

                                </Columns>

                            </asp:GridView>
                            <asp:HiddenField ID="hddidTipo" runat="server" />
                            <asp:HiddenField ID="hddMoeda" runat="server" />
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>

    </div>

    <script>
        $(document).ready(function () {
            $('[id*=gvAtoConcessorio]').DataTable({
                "pageLength": 100,
                order: [[0, 'desc']],
                language: {
                    url: 'https://cdn.datatables.net/plug-ins/1.11.5/i18n/pt-BR.json',
                },
                "LengthChange": false
            });
        });
    </script>

</asp:Content>
