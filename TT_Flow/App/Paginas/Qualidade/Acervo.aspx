<%@ Page Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Acervo.aspx.cs" Inherits="TT_Flow.App.Paginas.Qualidade.Acervo" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>




<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
    <style>
        .tag-list {
            width: 100%;
            flex-wrap: wrap;
            display: flex;
            overflow: hidden;
        }

        .tag-item {
            background: lightgray;
            padding: 2px 4px;
            font-size: 13px;
            text-transform: uppercase;
            margin: 2px;
            display: inline-flex;
            flex-wrap: nowrap;
            border-radius: 5px;
        }

        .download {
            align-content: center;
            font-size: large;
        }

        .scrollable-grid {
            overflow-x: auto;
        }

        .empresa {
            display: none;
        }
    </style>

</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Acervo"></asp:Label>
                <small>Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />

            <uc1:MensagemPagina runat="server" ID="MensagemPaginaConsulta" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Filtro para pesquisa </h3>

                </div>
                <div class="panel-body ">
                    <div class="form-group">
                        <div class="col-lg-12">
                            <div class="row">
                                <div class="col-lg-1">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtdtInicial" class="form-control CaixaTextoData" placeholder="Data Inicial" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-1">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtdtFinal" class="form-control CaixaTextoData" placeholder="Data Final" runat="server" MaxLength="10" data-mask="00/00/0000"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlidEmpresa" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlidParceiro" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlidFluxo" class="form-control  Caixa_Selecao" runat="server"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtsDescricao" class="form-control " placeholder="Referência" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-1">
                                    <div class="form-group">
                                        <asp:Button ID="btnPesquisar" class="btn btn-primary" runat="server" Text="Pesquisar" OnClick="btnPesquisar_Click" />
                                    </div>
                                </div>
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
                                        <asp:GridView ID="dtgvAcervo" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvAcervo_RowDataBound" OnRowCommand="dtgvAcervo_RowCommand">
                                            <Columns>

                                                <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa">
                                                    <HeaderStyle Width="3%" CssClass="empresa" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" CssClass="empresa" />
                                                </asp:BoundField>


                                                <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                    DataTextField="nNumeroPedido" HeaderText="Pedido"
                                                    DataNavigateUrlFormatString="/App/Paginas/Pedidos_Detalhe.aspx?id={0}&sTp=2"
                                                    Target="_blank">
                                                    <HeaderStyle Width="3%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>
                                                
                                                <asp:BoundField DataField="dtConclusao" HeaderText="Data Finalização">
                                                    <HeaderStyle Width="5%"/>
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle"/>
                                                </asp:BoundField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                                    DataTextField="sReferencia" HeaderText="Referência"
                                                    DataNavigateUrlFormatString="/App/Paginas/Pedidos_Detalhe.aspx?id={0}&sTp=2"
                                                    Target="_blank">
                                                    <HeaderStyle Width="15%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:BoundField DataField="sRazaoSocial" HeaderText="Razão Social">
                                                    <HeaderStyle Width="25%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:BoundField>

                                                <asp:BoundField DataField="nValor" HeaderText="Valor" DataFormatString="R$ {0:N2}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
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

    <script>
        $(function () {
            var $dtInicial = $('[id*=txtdtInicial]');
            var $dtFinal = $('[id*=txtdtFinal]');

            $dtInicial.datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            }).mask('99/99/9999');

            $dtFinal.datepicker({
                autoclose: true,
                format: 'dd/mm/yyyy',
                language: 'pt-BR'
            }).mask('99/99/9999');

            $dtInicial.data('previous', $dtInicial.val());
            $dtFinal.data('previous', $dtFinal.val());
        });
    </script>
</asp:Content>
