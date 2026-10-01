<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Escopo.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.Escopo" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>


<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual"/>

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <script type="text/javascript">
        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }
    </script>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Escopo"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
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
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                            <Columns>

                                                <asp:HyperLinkField DataNavigateUrlFields="idEscopo"
                                                    DataTextField="idEscopo" HeaderText="ID"
                                                    DataNavigateUrlFormatString="Escopo_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="20%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idEscopo"
                                                    DataTextField="sDscEscopo" HeaderText="Descrição"
                                                    DataNavigateUrlFormatString="Escopo_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="60%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:BoundField DataField="nCategorias" HeaderText="N° Categorias">
                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
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
