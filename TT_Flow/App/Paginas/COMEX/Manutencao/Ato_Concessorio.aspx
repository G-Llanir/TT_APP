<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Ato_Concessorio.aspx.cs" Inherits="TT_Flow.App.Paginas.COMEX.Manutencao.Ato_Concessorio" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
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
                <asp:Label ID="lblTituloPagina" runat="server" Text="Ato Concessório"></asp:Label><small> Consulta</small>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>

        <div class="col-lg-12" runat="server" id="DIV_Filtro">

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <div class="col-lg-12">
                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-2">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClientClick="MudarBotao()" OnClick="cmdPesquisar_Click" />
                                <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>


        <div class="col-lg-12">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>

        <div class="col-lg-12">
            <asp:Panel ID="pnResultado" runat="server">
                <div class="panel panel-primary">
                    <div class="panel-body">

                        <div class="table-responsive">

                            <asp:GridView ID="gvAtoConcessorio" runat="server" AutoGenerateColumns="false" CellPadding="0" CellSpacing="1"
                                class="table table-bordered table-hover table-condensed table-responsive tablesorter" GridLines="None"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">

                                <Columns>

                                    <asp:HyperLinkField DataNavigateUrlFields="idAtoConcessorio"
                                        DataTextField="idAtoConcessorio" HeaderText="Ato Concessório"
                                        DataNavigateUrlFormatString="Ato_Concessorio_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="7%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idAtoConcessorio"
                                        DataTextField="sCodigoAtoConcessorio" HeaderText="Código"
                                        DataNavigateUrlFormatString="Ato_Concessorio_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="10%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idAtoConcessorio"
                                        DataTextField="sDscAtoConcessorio" HeaderText="Descrição"
                                        DataNavigateUrlFormatString="Ato_Concessorio_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="60%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtEmissao" HeaderText="Data Emissao" HtmlEncode="False" HtmlEncodeFormatString="False" DataFormatString="{0:dd/MM/yyyy}">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="7%" />
                                    </asp:BoundField>

                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtdeferimento" HeaderText="Data Deferimento" HtmlEncode="False" HtmlEncodeFormatString="False" DataFormatString="{0:dd/MM/yyyy}">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="7%" />
                                    </asp:BoundField>

                                    <asp:BoundField ConvertEmptyStringToNull="False" DataField="dtVencimento" HeaderText="Data Vencimento" HtmlEncode="False" HtmlEncodeFormatString="False" DataFormatString="{0:dd/MM/yyyy}">
                                        <HeaderStyle HorizontalAlign="Left" />
                                        <ItemStyle HorizontalAlign="Left" Width="7%" />
                                    </asp:BoundField>

                                </Columns>

                            </asp:GridView>
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
