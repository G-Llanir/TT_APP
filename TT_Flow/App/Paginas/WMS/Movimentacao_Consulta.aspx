<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Movimentacao_Consulta.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Movimentacao_Consulta" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="UpdDetalhe" runat="server">
        <ContentTemplate>
            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                        <small>Consulta</small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtros </h3>
                        </div>
                        <div class="panel-body">
                            <div class="col-lg-1">
                                <div class="form-group">
                                    <asp:TextBox ID="txtDataInicial" class="form-control" runat="server" placeholder="Data Inicial" data-mask="00/00/0000" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-lg-1    ">
                                <div class="form-group">
                                    <asp:TextBox ID="txtDataFinal" class="form-control" runat="server" placeholder="Data Final" data-mask="00/00/0000" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>

                            
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidStatus" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidTipoMovimentacao" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidTipoMovimentacao_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-lg-2">
                                <div class="form-group">
                                    <asp:DropDownList ID="ddlidMotivo" class="form-control" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddl_SelectedIndexChanged"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-lg-4">
                                <div class="form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                    <asp:Button ID="cmdNovaMovimentacao" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovaMovimentacao_Click" />
                                     <asp:Button ID="cmdImportarXml" class="btn btn-primary" runat="server" Text="Importar XML" OnClick="cmdImportarXml_Click" />
                                    <asp:Button ID="cmdExportar" class="btn btn-info" runat="server" Text="Exportar Excel" OnClick="cmdExportar_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
            <div class="form-stacked row">

                <asp:Panel ID="pnMensagem" runat="server">
                    <div class="col-lg-12">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </div>

                </asp:Panel>

                <asp:Panel ID="pnResultado" runat="server">
                    <div class="col-lg-12">
                        <div class="panel panel-primary">
                            <div class="panel-body">
                                <div class="table-responsive">

                                    <asp:UpdatePanel ID="updExport" runat="server">
                                        <ContentTemplate>

                                            <asp:GridView ID="dtgvConsulta" runat="server" class="table table-striped table-bordered table-hover" GridLines="None"
                                                CellSpacing="1" CellPadding="1" Width="100%" ShowFooter="False" Font-Names="Tahoma"
                                                Font-Overline="False" Font-Size="11px" AutoGenerateColumns="false" OnRowDataBound="dtgvConsulta_RowDataBound">
                                                <Columns>

                                                    <asp:HyperLinkField DataNavigateUrlFields="idMovimentacao"
                                                        DataTextField="idMovimentacao" HeaderText="ID"
                                                        DataNavigateUrlFormatString="Movimentacao_Detalhe.aspx?id={0}">
                                                        <HeaderStyle Width="6%" />
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:HyperLinkField>


                                                    <asp:HyperLinkField DataNavigateUrlFields="idMovimentacao"
                                                        DataTextField="dtMovimentacao" HeaderText="Data"
                                                        DataNavigateUrlFormatString="Movimentacao_Detalhe.aspx?id={0}">
                                                        <HeaderStyle Width="10%" />
                                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                                    </asp:HyperLinkField>

                                                    <asp:HyperLinkField DataNavigateUrlFields="idMovimentacao"
                                                        DataTextField="sDscTipoMovimentacao" HeaderText="Tipo Movimentação"
                                                        DataNavigateUrlFormatString="Movimentacao_Detalhe.aspx?id={0}">
                                                        <HeaderStyle Width="12%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:HyperLinkField>

                                                    <asp:HyperLinkField DataNavigateUrlFields="idMovimentacao"
                                                        DataTextField="sDscMotivo" HeaderText="Motivo"
                                                        DataNavigateUrlFormatString="Movimentacao_Detalhe.aspx?id={0}">
                                                        <HeaderStyle Width="8%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:HyperLinkField>

                                                    <asp:HyperLinkField DataNavigateUrlFields="idMovimentacao"
                                                        DataTextField="sDscParceiro" HeaderText="Parceiro"
                                                        DataNavigateUrlFormatString="Movimentacao_Detalhe.aspx?id={0}">
                                                        <HeaderStyle Width="26%" />
                                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:HyperLinkField>

                                                    <asp:BoundField DataField="sNotaFiscal" HeaderText="Nota Fiscal">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sPO" HeaderText="Pedido/OPI">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sDscStatus" HeaderText="Status">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                    <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Usuário">
                                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                    </asp:BoundField>

                                                </Columns>
                                            </asp:GridView>

                                        </ContentTemplate>
                                    </asp:UpdatePanel>

                                </div>
                            </div>
                </asp:Panel>
            </div>

        </ContentTemplate>
        <Triggers>
            <asp:AsyncPostBackTrigger ControlID="ddlidTipoMovimentacao" EventName="SelectedIndexChanged" />
            <asp:AsyncPostBackTrigger ControlID="ddlidMotivo" EventName="SelectedIndexChanged" />
        </Triggers>
    </asp:UpdatePanel>

    <script type="text/javascript">
        function InicializarFiltrosData() {
            var $datas = $('[id*=txtDataInicial], [id*=txtDataFinal]');

            $datas.datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            }).mask('99/99/9999');
        }

        $(document).ready(function () {
            InicializarFiltrosData();
        });

        if (typeof Sys !== 'undefined' && Sys.WebForms && Sys.WebForms.PageRequestManager) {
            Sys.WebForms.PageRequestManager.getInstance().add_endRequest(function () {
                InicializarFiltrosData();
            });
        }
    </script>
</asp:Content>
