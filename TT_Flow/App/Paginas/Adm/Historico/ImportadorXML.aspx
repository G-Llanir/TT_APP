<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ImportadorXML.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Historico.ImportadorXML" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <script src="https://cdn.jsdelivr.net/npm/chart.js"></script>

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

        $(function () {
            $('[id*=lstidFluxo]').multiselect({
                buttonWidth: '195px',
                includeSelectAllOption: true,
                maxHeight: 300,
                dropRight: true,
                nSelectedText: ' - Anos Selecionados!',
                allSelectedText: 'Todos os Anos',
                enableFiltering: false
            });
        });

    </script>

    <style>
        .grafico {
            border: none !important;
        }
    </style>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Importação de XML"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="col-lg-2">
                            <div class="form-group row">
                                <div class="col-lg-6">
                                    <asp:TextBox ID="txtdtInicio" class="form-control CaixaTextoData" placeholder="Data Inicial" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                </div>
                                <div class="col-lg-6">
                                    <asp:TextBox ID="txtdtFinal" class="form-control CaixaTextoData " runat="server" placeholder="Data Final" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlEmpresa" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlCliente" runat="server" class="Caixa_Selecao form-control"></asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-2" runat="server" id="DIV_Financeiro">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlTipo" runat="server" class="Caixa_Selecao form-control">
                                    <asp:ListItem Value="0" Text="Todos os Tipos" />
                                    <asp:ListItem Value="1" Text="Compras" />
                                    <asp:ListItem Value="2" Text="Devolução" />
                                    <asp:ListItem Value="3" Text="Vendas" />
                                    <asp:ListItem Value="4" Text="Outros" />
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <div class="form-group">
                                <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar" runat="server"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Importar Novo XML" OnClick="cmdNovo_Click" />
                            </div>
                        </div>

                    </div>
                </div>
            </div>

            <div class="panel panel-primary">
                <div class="panel-body" id="panelExibirOPI">
                    <div class="form-group">
                        <div class="col-lg-12">
                            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                            <asp:Panel ID="pnResultado" class="" runat="server">

                                <div class="table-responsive">
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                        <Columns>

                                            <asp:BoundField DataField="idImportador" HeaderText="ID">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sEmpresa" HeaderText="Empresa">
                                                <ItemStyle Width="18%" HorizontalAlign="left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sCliente" HeaderText="Cliente">
                                                <ItemStyle Width="18%" HorizontalAlign="left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dhEmi" HeaderText="Data Emissão">
                                                <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idImportador"
                                                DataTextField="sNomeArquivo" HeaderText="Nome do Arquivo"
                                                DataNavigateUrlFormatString="ImportadorXML_Detalhe.aspx?id={0}">
                                                <HeaderStyle Width="46%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:BoundField DataField="nNFe" HeaderText="Número NF-e">
                                                <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Atualização">
                                                <ItemStyle Width="7%" HorizontalAlign="left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Inclusão">
                                                <ItemStyle Width="9%" HorizontalAlign="left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:TemplateField HeaderText="Download">
                                                <ItemTemplate>
                                                    <asp:LinkButton runat="server" ID="btnPDF" class="btn btn-sm btn-danger" Text="PDF" OnClick="btnPDF_Click" CommandArgument='<%# Eval("idArquivoPDF") %>'></asp:LinkButton>
                                                    <asp:LinkButton runat="server" ID="btnXML" class="btn btn-sm btn-primary" Text="XML" OnClick="cmdXMLDownload_Click" CommandArgument='<%# Eval("idArquivo") %>'></asp:LinkButton>
                                                </ItemTemplate>
                                                <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </asp:Panel>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
