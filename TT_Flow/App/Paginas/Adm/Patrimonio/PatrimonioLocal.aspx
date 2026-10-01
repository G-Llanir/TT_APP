<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="PatrimonioLocal.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Patrimonio.PatrimonioLocal" %>

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
                <asp:Label ID="lblTituloPagina" runat="server" Text="Local de Patrimônio"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:TextBox ID="txtPesquisa" class="form-control CaixaTextoGgigante" placeholder="Pesquisar" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-6">
                            <div class="form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo Local" OnClick="cmdNovo_Click" />
                                <asp:Button ID="cmdExportar" class="btn btn-info" runat="server" Text="Exportar Excel" OnClick="cmdExportar_Click" />

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

                                                <asp:HyperLinkField DataNavigateUrlFields="idLocal"
                                                    DataTextField="idLocal" HeaderText="ID"
                                                    DataNavigateUrlFormatString="PatrimonioLocal_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="3%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idLocal"
                                                    DataTextField="sDscLocal" HeaderText="Descrição do Local"
                                                    DataNavigateUrlFormatString="PatrimonioLocal_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="50%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idLocal"
                                                    DataTextField="sDscDepartamento" HeaderText="Departamento Padrão"
                                                    DataNavigateUrlFormatString="PatrimonioLocal_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="20%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idLocal"
                                                    DataTextField="sSituacao_Completa" HeaderText="Ativo"
                                                    DataNavigateUrlFormatString="PatrimonioLocal_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="5%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idLocal"
                                                    DataTextField="dtAtualizacao" HeaderText="Data Atualização"
                                                    DataNavigateUrlFormatString="PatrimonioLocal_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="12%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idLocal"
                                                    DataTextField="sDscUsuarioAtualizacao" HeaderText="Por"
                                                    DataNavigateUrlFormatString="PatrimonioLocal_Detalhe.aspx?id={0}">
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
