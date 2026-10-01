<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="GHE.aspx.cs" Inherits="TT_Hub.App.Paginas.RRHH.GHE" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
        .tag-list {
            width: 100%;
            flex-wrap: wrap;
            display: flex;
            overflow: hidden;
        }

        .tag-item {
            background: green;
            padding: 2px 4px;
            font-size: 12px;
            margin: 3px;
            display: inline-flex;
            flex-wrap: nowrap;
            border-radius: 4px;
        }
    </style>

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
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                        <small>
                            <asp:Label ID="lblSubTituloPagina" runat="server" Text="Grupo Homogêneo de Exposição - Consulta "></asp:Label></small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">

                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">

                            <div class="row">

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlidEmpresa" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlsidSetor" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                                <%-- NOVO FILTRO DE PLANO ADICIONADO --%>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlidPlano" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtsPesquisa" class="form-control " placeholder="Pesquisar" runat="server" MaxLength="200"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                        <asp:Button ID="cmdRelatorioColaborador" class="btn btn-warning" runat="server" Text="Relatório" OnClick="cmdRelatorioColaborador_Click" OnClientClick="MudarBotao()" />
                                        <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                                    </div>
                                </div>

                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnResultado" class="" runat="server">
                <div id="resultado">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">

                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                    <Columns>

                                        <asp:HyperLinkField DataNavigateUrlFields="idGHE"
                                            DataTextField="idGHE" HeaderText="ID"
                                            DataNavigateUrlFormatString="GHE_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="3%" />
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idGHE"
                                            DataTextField="sCodigoGHE" HeaderText="Código"
                                            DataNavigateUrlFormatString="GHE_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="10%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idGHE"
                                            DataTextField="sDscGHE" HeaderText="Descrição"
                                            DataNavigateUrlFormatString="GHE_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="25%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <%-- NOVA COLUNA DE PLANO ADICIONADA --%>
                                        <asp:BoundField DataField="sDscPlano" HeaderText="Plano">
                                            <HeaderStyle Width="15%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idGHE"
                                            DataTextField="sDscEmpresa" HeaderText="Empresa"
                                            DataNavigateUrlFormatString="GHE_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="8%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idGHE"
                                            DataTextField="sDscSetor" HeaderText="Setores"
                                            DataNavigateUrlFormatString="GHE_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="25%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idGHE"
                                            DataTextField="sFuncoes" HeaderText="Funções Vinculadas"
                                            DataNavigateUrlFormatString="Funcoes.aspx?idGHE={0}&sTipo=GHE">
                                            <HeaderStyle Width="25%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idGHE"
                                            DataTextField="nQtdFuncionarios" HeaderText="Qtd Funcionários"
                                            DataNavigateUrlFormatString="Colaboradores.aspx?idGHE={0}&sTipo=GHE">
                                            <HeaderStyle Width="5%" />
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>


                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Atualizado em">
                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <%--                                        <asp:BoundField DataField="sDscUsuario" HeaderText="Por">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>--%>
                                    </Columns>
                                </asp:GridView>

                            </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnMensagem" runat="server">
                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            </asp:Panel>

            <div class="modal fade" id="modalRelatorio" tabindex="-1" role="dialog" aria-labelledby="modalRelatorioLabel">
                <div class="modal-dialog modal-lg" role="document">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title" id="modalRelatorioLabel">Relatório de Colaboradores por GHE</h4>
                        </div>
                        <div class="modal-body">

                            <asp:Panel ID="pbRelatorio" runat="server" Visible="false">

                                <%--  <asp:Button ID="cmdExportarModal" class="btn btn-success" runat="server"
    Text="Exportar para Excel" 
    OnClick="cmdExportarModal_Click" OnClientClick="return beginExport();"/>--%>
                                <asp:HyperLink ID="lnkExportar" runat="server"
                                    CssClass="btn btn-success"
                                    Target="_blank"
                                    NavigateUrl="~/App/Paginas/RRHH/GHE.aspx?action=export">
    <i class="fa fa-file-excel-o"></i> Exportar para Excel
                                </asp:HyperLink>
                                <hr />

                                <div style="max-height: 500px; overflow-y: auto;">
                                    <asp:GridView ID="gvRelatorio" class="table table-striped table-bordered table-hover"
                                        runat="server" Width="100%" CellPadding="1" AutoGenerateColumns="true" GridLines="None"
                                        Font-Size="Small">
                                    </asp:GridView>
                                </div>
                            </asp:Panel>

                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Fechar</button>
                        </div>
                    </div>
                </div>
            </div>

            <%-- <asp:Panel ID="pbRelatorio" runat="server">

                                        <asp:Button ID="cmdExportarExcel" class="btn btn-success" runat="server" Text="Exportar Excel" OnClick="cmdExportarExcel_Click" />

                <asp:GridView ID="gvRelatorio" class="table table-striped table-bordered table-hover "
                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="true" GridLines="None"
                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                </asp:GridView>
            </asp:Panel>--%>
        </ContentTemplate>
        <%--  <Triggers>
            <asp:PostBackTrigger ControlID="cmdExportarModal" />
        </Triggers>--%>
    </asp:UpdatePanel>



</asp:Content>
