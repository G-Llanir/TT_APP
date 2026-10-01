<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Comparativo_ContasPagar.aspx.cs" Inherits="TT_Hub.App.Paginas.Adm.Financeiro.Comparativo_ContasPagar" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <uc1:Manual runat="server" ID="manual" />
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <script type="text/javascript">

        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Gerando Relatório..";
        }

        $(function () {
            // Aplica na carga inicial
            ReaplicarMultiselect();
        });

        // Função para reaplicar o bootstrap-multiselect
        function ReaplicarMultiselect() {
            $('[id*=lstidCategoriaPagar]').multiselect({
                buttonWidth: '100%',
                includeSelectAllOption: true,
                selectAllName: 'Todas as Categorias ',
                maxHeight: 400,
                dropRight: true,
                nSelectedText: ' - Categorias Selecionadas!',
                allSelectedText: 'Todas as Categorias',
                enableFiltering: true
            });
        }

        // Garante que o multiselect seja recriado após o postback do dropdown
        var prm = Sys.WebForms.PageRequestManager.getInstance();
        if (prm != null) {
            prm.add_endRequest(function (sender, e) {
                ReaplicarMultiselect();
            });
        }

    </script>


    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Comparativo Financeiro"></asp:Label><small> Administração</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12" runat="server" id="DIV_Filtro">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="col-lg-4">
                            <div class="row">
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtdtInicio" class="form-control" placeholder="Emissão" runat="server" MaxLength="10" type="date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtdtFinal" class="form-control " runat="server" placeholder="Vencimento" MaxLength="10" type="date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-4">
                                    <asp:DropDownList ID="ddlsTipoPesquisa" runat="server" class="form-control Caixa_Selecao">
                                        <asp:ListItem  Value="0">Vencimento</asp:ListItem>
                                        <asp:ListItem Selected="True" Value="1">Emissão</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-3">
                             <div class="row">
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlidEmpresa" class="form-control  Caixa_Selecao" runat="server" ></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-6">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlTipoVisualizacao" runat="server" class="form-control Caixa_Selecao" 
                                            AutoPostBack="true" OnSelectedIndexChanged="ddlTipoVisualizacao_SelectedIndexChanged">
                                            <asp:ListItem Value="C" Selected="True">Consolidado</asp:ListItem>
                                            <asp:ListItem Value="R">Receitas</asp:ListItem>
                                            <asp:ListItem Value="D">Despesas</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                             </div>
                        </div>

                        <div class="col-lg-3">
                            <div class="form-group">
                                <asp:ListBox ID="lstidCategoriaPagar" runat="server" SelectionMode="Multiple" Width="100%"></asp:ListBox>
                            </div>
                        </div>

                        <div class="col-lg-2" >
                            <div class="form-group"> 
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Gerar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                <asp:Button ID="cmdExportarExcel" class="btn btn-info" runat="server" Text="Exportar Excel"  OnClientClick="abrirExportacao(); return false;" />
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
                        <div class="table-responsive" runat="server" id="DIV_RELATORIO" >

                        </div>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>
    </div>
    <script>
        function abrirExportacao() {
            var url = window.location.pathname + "?export=1";
            window.open(url, "_blank");
        }
    </script>
</asp:Content>