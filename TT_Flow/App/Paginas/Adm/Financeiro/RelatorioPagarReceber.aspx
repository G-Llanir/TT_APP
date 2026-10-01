<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="RelatorioPagarReceber.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.RelatorioPagarReceber" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

    <style>
        .dataTables_scrollFootInner {
            padding-right: 0px !important;
        }
    </style>

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
                <asp:Label ID="lblTituloPagina" runat="server" Text="Relatório"></asp:Label>
                <small>
                    <asp:Label ID="lblSubTituloPagina" runat="server" Text="Consulta"></asp:Label></small>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>

        <div class="col-lg-12" runat="server" id="DIV_Filtro">
            <div class="panel panel-primary">

                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                </div>

                <div class="panel-body ">

                    <div class="form-group">

                        <div class="row">

                            <div class="col-lg-4">
                                <div class="form-group row">
                                    <div class="col-lg-6">
                                        <asp:TextBox ID="txtdtInicio" class="form-control" placeholder="Emissão" runat="server" MaxLength="10" type="date" ToolTip="Inicio"></asp:TextBox>
                                    </div>
                                    <div class="col-lg-6">
                                        <asp:TextBox ID="txtdtFinal" class="form-control" runat="server" placeholder="Vencimento" MaxLength="10" type="date" ToolTip="Final"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidEmpresa" class="form-control  Caixa_Selecao" runat="server" attrname="idContasPagar"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-2" style="margin-top: 5px;">
                                <div class="form-group">
                                    <asp:CheckBoxList ID="cblsPagarReceber" runat="server" RepeatColumns="10" RepeatLayout="Table" RepeatDirection="Vertical" AutoPostBack="true" OnSelectedIndexChanged="cblsPagarReceber_SelectedIndexChanged">
                                        <asp:ListItem Value="P">&nbsp;Contas a Pagar&nbsp;&nbsp;&nbsp;</asp:ListItem>
                                        <asp:ListItem Value="R">&nbsp;Contas a Receber</asp:ListItem>
                                    </asp:CheckBoxList>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:Button ID="Button3" class="btn btn-danger" runat="server" Text="PDF" OnClick="cmdPDF_Click" />
                                    <asp:Button ID="Button4" class="btn btn-success" runat="server" Text="Excel" OnClick="cmdExcel_Click" />
                                </div>
                            </div>

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

                            <div class="table-responsive" runat="server" id="gvPagar">
                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">

                                    <Columns>

                                        <asp:BoundField DataField="sSemana" HeaderText="Semana">
                                            <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sCodigo" HeaderText="Codigo">
                                            <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa">
                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscCategoriaPagar" HeaderText="Referência">
                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscCredor" HeaderText="Solicitante">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDocumento" HeaderText="Conceito">
                                            <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sCNPJ_CPF" HeaderText="CNPJ / CPF">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtpagamento_Info_Pag" HeaderText="Data de Pagamento">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtVencimento" HeaderText="Data de Vencimento">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nTotal" HeaderText="Valor do Pagamento">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscConta" HeaderText="Banco">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscFormapagamento" HeaderText="Forma Pagamento">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sStatus" HeaderText="Status">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                </asp:GridView>
                            </div>

                            <div class="table-responsive" runat="server" id="gvReceber">
                                <asp:GridView ID="dtgvReceber" class="table table-striped table-bordered table-hover"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">

                                    <Columns>

                                        <asp:BoundField DataField="sSemana" HeaderText="Semana">
                                            <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sCodigo" HeaderText="Codigo">
                                            <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa">
                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscCategoriaReceber" HeaderText="Referência">
                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sRazaoSocial" HeaderText="Solicitante">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDocumento" HeaderText="Conceito">
                                            <ItemStyle Width="16%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sCNPJ_CPF" HeaderText="CNPJ / CPF">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtRecebimento_Info_Rec" HeaderText="Data de Recebimento">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="dtVencimento" HeaderText="Data de Vencimento">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nTotal" HeaderText="Valor do Recebimento">
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscConta" HeaderText="Banco">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscFormaRecebimento" HeaderText="Forma Pagamento">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sStatus" HeaderText="Status">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
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
</asp:Content>
