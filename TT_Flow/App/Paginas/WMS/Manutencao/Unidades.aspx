<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Unidades.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Manutencao.Unidades" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
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
                <asp:Label ID="lblTituloPagina" runat="server" Text="Unidades"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Nova Unidade" OnClick="cmdNovo_Click" />
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

                                                <asp:HyperLinkField DataNavigateUrlFields="idUnidade"
                                                    DataTextField="idUnidade" HeaderText="ID"
                                                    DataNavigateUrlFormatString="Unidades_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="2%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idUnidade"
                                                    DataTextField="sUnidade" HeaderText="Unidade"
                                                    DataNavigateUrlFormatString="Unidades_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="3%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idUnidade"
                                                    DataTextField="sDscUnidade" HeaderText="Descrição"
                                                    DataNavigateUrlFormatString="Unidades_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="4%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idUnidade"
                                                    DataTextField="sDscServico" HeaderText="Serviço"
                                                    DataNavigateUrlFormatString="Unidades_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="2%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idUnidade"
                                                    DataTextField="Ativo" HeaderText="Ativo"
                                                    DataNavigateUrlFormatString="Unidades_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="2%" />
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

            <div class="modal fade" id="myModal">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h4 class="modal-title">Duplicar para o Próximo Ano?</h4>
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                        </div>
                        <div class="modal-body">

                            <div>
                                <label>Repetir em: </label>
                                <asp:TextBox ID="txtsAno" class="form-control" type="text" runat="server"></asp:TextBox>
                            </div>

                            <div class="modal-footer">
                                <asp:Button ID="cmdRepetirAno" class="btn btn-primary" runat="server" Text="Repetir" OnClick="cmdRepetir_click" />
                                <button type="button" class="btn btn-danger" data-dismiss="modal">Fechar</button>
                            </div>
                        </div>
                    </div>
                </div>

            </div>
        </div>
    </div>
</asp:Content>
