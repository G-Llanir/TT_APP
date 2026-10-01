<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Financeiro_Cliente.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.Financeiro_Cliente" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <uc1:Manual runat="server" ID="manual" />
    <div id="DIV_ESPACO" style="height: 600px" runat="server" visible="false"></div>

    <script type="text/javascript">

        function MudarBotao() {
            document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
        }

        function toggleVisibility(show) {
            var grid = document.getElementById('simulacaoGrid');
            var title = document.getElementById('tituloResumo');
            if (show) {
                grid.style.display = 'block';  // Mostra o grid e o título
                title.style.display = 'block'; // Certifique-se de que o título também seja mostrado
            } else {
                grid.style.display = 'none';   // Oculta o grid e o título
                title.style.display = 'none';  // Certifique-se de que o título também seja ocultado
            }
        }


        $v192 = jQuery.noConflict();
    </script>

    <div class="form-stacked row">

        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Financeiro"></asp:Label>
                <small>Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12" runat="server" id="DIV_Filtro">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                </div>
                <div class="panel-body ">
                    <div class="form-group">

                        <div class="col-lg-12">
                            <div class="row">
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtdtInicio" class="form-control" placeholder="Emissão" runat="server" MaxLength="10" type="date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtdtFinal" class="form-control" runat="server" placeholder="Vencimento" MaxLength="10" type="date"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlsStatus" runat="server" class="form-control yes_no select Caixa_Selecao Caixa_Selecao" attrname="idContasReceber">
                                            <asp:ListItem Value="">Todos os Status</asp:ListItem>
                                            <asp:ListItem Value="Liquidado">Liquidado</asp:ListItem>
                                            <asp:ListItem Selected="True" Value="Em Aberto">Em Aberto</asp:ListItem>
                                            <asp:ListItem Value="Em Atraso">Em Atraso</asp:ListItem>
                                            <asp:ListItem Value="Vencendo Hoje">Vencendo Hoje</asp:ListItem>
                                            <asp:ListItem Value="A vencer em 7 dias">A vencer em 7 dias</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    </div>
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

                            <%----------------------------------------------------%>
                            <button type="button" class="btn" data-toggle="collapse" data-target="#toggleButtons" style="background-color: #009a22; color: white;">
                                Mostrar/Ocultar Colunas
                            </button>
                            <div id="toggleButtons" class="collapse">
                                <br />
                                <asp:PlaceHolder ID="placeholderButtons" runat="server"></asp:PlaceHolder>
                            </div>
                            <br />
                            <br />

                            <%----------------------------------------------------%>
                            <div class="table-responsive">
                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                    DataKeyNames="idContasReceber" runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">

                                    <Columns>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="idContasReceber" HeaderText="ID"
                                            DataNavigateUrlFormatString="Financeiro_Cliente_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="5%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="sDocumento" HeaderText="Pedido"
                                            DataNavigateUrlFormatString="Financeiro_Cliente_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="8%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="sRazaoSocial" HeaderText="Parceiro"
                                            DataNavigateUrlFormatString="Financeiro_Cliente_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="18%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="dtEmissao" HeaderText="Data Emissão"
                                            DataNavigateUrlFormatString="Financeiro_Cliente_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="8%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="dtVencimento" HeaderText="Data Vencimento"
                                            DataNavigateUrlFormatString="Financeiro_Cliente_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="9%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                            DataTextField="sQuantidadeParcela" HeaderText="Parcela"
                                            DataNavigateUrlFormatString="Financeiro_Cliente_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="4%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="nValorOriginal" HeaderText="Valor Líquido"
                                            DataFormatString="{0:C2}" SortExpression="nValorOriginal"
                                            ItemStyle-HorizontalAlign="Left" HeaderStyle-Width="12%" />

                                        <asp:BoundField DataField="nValorPago" HeaderText="Valor Recebido" DataFormatString="{0:C2}">
                                            <HeaderStyle Width="11%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}"
                                            DataTextField="nSaldo" HeaderText="Saldo em Aberto"
                                            DataNavigateUrlFormatString="Financeiro_Cliente_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="12%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="sStatus" HeaderText="Status">
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

    <style>
        .modal-header-content {
            display: flex;
            align-items: center;
        }

        .modal-logo img {
            max-width: 50px;
            margin-right: 10px;
        }

        .modal-title-container {
            flex-grow: 1;
        }

        .modal-title {
            color: #009A22;
            text-shadow: 2px 2px 2px rgba(0, 0, 0, 0.2);
            font-size: 24px;
            font-weight: bold;
            margin: 0;
        }

        body {
            font-family: Arial, sans-serif;
        }

        h2 {
            color: #333;
        }

        .gridViewStyle {
            border-collapse: collapse;
            margin: 20px 0;
            width: 100%;
            box-shadow: 0 2px 15px rgba(0, 0, 0, 0.15);
        }

            .gridViewStyle th, .gridViewStyle td {
                border: 1px solid #ddd;
                padding: 12px 15px;
                text-align: left;
            }

            .gridViewStyle th {
                background-color: #009A22;
                color: #ffffff;
            }

            .gridViewStyle tr:nth-child(even) {
                background-color: #f2f2f2;
            }

            .gridViewStyle tr:hover {
                background-color: #ddd;
            }

            .gridViewStyle a {
                color: #007bff;
                text-decoration: none;
            }

                .gridViewStyle a:hover {
                    text-decoration: underline;
                }

        .scrollable-grid {
            overflow-x: auto;
        }
    </style>

</asp:Content>


