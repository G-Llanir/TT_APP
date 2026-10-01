<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ContasReceber.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Financeiro.ContasReceber" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/DropDownList_Padrao.ascx" TagPrefix="uc1" TagName="DropDownList_Padrao" %>
<%@ Register Src="~/App/Controles/TextBox_Padrao.ascx" TagPrefix="uc1" TagName="TextBox_Padrao" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <style>
        .modal-header-content {
            display: flex;
            align-items: center;
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

    <div class="col-lg-12">
        <h1>
            <asp:Label ID="lblTituloPagina" runat="server" Text="Contas a Receber"></asp:Label><small> <asp:Label ID="lblSubTituloPagina" runat="server" Text="Consulta"></asp:Label></small></h1>
        <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
    </div>

    <asp:Panel runat="server" ID="pnFiltro" CssClass="col-lg-12">
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h3 class="panel-title"><i class="fa fa-filter"></i>&nbsp;Filtro para pesquisa</h3>
            </div>
            <div class="panel-body">

                <div class="row">

                    <div id="datas" class="col-lg-4 padd-0">
                        <div class="col-lg-4 padd-r-0">
                            <uc1:TextBox_Padrao runat="server" ID="txtdtInicio" Modo="Data" />
                        </div>
                        <div class="col-lg-4 padd-r-0">
                            <uc1:TextBox_Padrao runat="server" ID="txtdtFinal" Modo="Data" />
                        </div>
                        <div class="col-lg-4">
                            <uc1:DropDownList_Padrao runat="server" ID="ddlidDataPesquisa">
                                <asp:ListItem Selected="True" Value="0">Vencimento</asp:ListItem>
                                <asp:ListItem Value="1">Emissão</asp:ListItem>
                                <asp:ListItem Value="2">Liquidação</asp:ListItem>
                            </uc1:DropDownList_Padrao>
                        </div>
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidContabil" />
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidCategoriaReceber" />
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidFormaRecebimento" />
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidCentroDeCusto" />
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidEmpresa" />
                    </div>

                    <div class="col-lg-2 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlsStatus">
                            <asp:ListItem Selected="True" Value="">Todos os Status</asp:ListItem>
                            <asp:ListItem Value="Liquidado">Liquidado</asp:ListItem>
                            <asp:ListItem Value="Em Aberto">Em Aberto</asp:ListItem>
                            <asp:ListItem Value="Em Atraso">Em Atraso</asp:ListItem>
                            <asp:ListItem Value="Vencendo Hoje">Vencendo Hoje</asp:ListItem>
                            <asp:ListItem Value="A vencer em 7 dias">A vencer em 7 dias</asp:ListItem>
                            <asp:ListItem Value="Protestado">Protestado</asp:ListItem>
                            <asp:ListItem Value="Cobrança Judicial">Cobrança Judicial</asp:ListItem>
                            <asp:ListItem Value="Inadimplente">Inadimplente</asp:ListItem>
                        </uc1:DropDownList_Padrao>
                    </div>

                    <div class="col-lg-2 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlsTituloAdiantado">
                            <asp:ListItem Selected="True" Value="T">Todos os Títulos</asp:ListItem>
                            <asp:ListItem Value="S">Adiantados</asp:ListItem>
                            <asp:ListItem Value="N">Não Adiantados</asp:ListItem>
                        </uc1:DropDownList_Padrao>
                    </div>

                    <div class="col-lg-2 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidConta" />
                    </div>

                    <div class="col-lg-2 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlsConciliado">
                            <asp:ListItem Selected="True" Value="T">Conciliados e Não Conciliados</asp:ListItem>
                            <asp:ListItem Value="S">Conciliados</asp:ListItem>
                            <asp:ListItem Value="N">Não Conciliado</asp:ListItem>
                            <asp:ListItem Value="P">Parcialmente Conciliado</asp:ListItem>
                        </uc1:DropDownList_Padrao>
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlsCLiente" />
                    </div>

                </div>

                <div class="row">
                    <div class="col-lg-6">
                        <uc1:TextBox_Padrao runat="server" ID="txtPesquisa" Placeholder="Pesquisar" />
                    </div>

                    <div class="col-lg-6">
                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisa()" />
                        <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                        <asp:Button ID="cmdAdiantar" class="btn btn-info" runat="server" Text="Adiantar Títulos" OnClick="cmdAdiantar_Click" />
                        <asp:Button ID="cmdExportar" class="btn btn-info" runat="server" Text="Exportar em Excel" OnClick="cmdExportar_Click" />
                    </div>
                </div>

            </div>
        </div>
    </asp:Panel>

    <asp:Panel runat="server" ID="pnAdiantamento" CssClass="col-lg-12">
        <div class="panel panel-primary">
            <div class="panel-heading">
                <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>&nbsp;Selecione Títulos para Adiantar</h3>
            </div>
            <div class="panel-body">
                <div class="col-lg-6 form-group">
                    <asp:Button runat="server" ID="cmdVoltarAdiantamento" class="btn btn-success" Text="Voltar a Conta a Receber" OnClick="cmdVoltarAdiantamento_Click" />
                    <asp:Button runat="server" ID="cmdAdiantamento" class="btn btn-info" Text="Adiantar selecionados" OnClientClick="prepararEabrirModalAdiantamento(); return false;" Style="display: none;" />
                </div>
            </div>
        </div>
    </asp:Panel>

    <div class="col-lg-12">
        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
    </div>

    <asp:Panel runat="server" ID="pnResultado" CssClass="col-lg-12">

        <div class="panel panel-primary">
            <div class="panel-body">

                <asp:Literal runat="server" ID="ltOcultarColunas" />

                <div class="col-lg-12 padd-0">
                    <div class="table-responsive">
                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                            DataKeyNames="idContasReceber" runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                            ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound" UseAccessibleHeader="true">
                            <Columns>

                                <%--
                                    ========================
                                    0 --------------- CheckBox
                                    1 --------------- ID
                                    2 --------------- Cliente
                                    3 --------------- Data Emissão
                                    4 --------------- Data Vencimento
                                    5 --------------- Dias Até Vencimento
                                    6 --------------- Empresa
                                    7 --------------- Pedido
                                    8 --------------- Parcela
                                    9 --------------- Nota Fiscal
                                    10 -------------- Multa
                                    11 -------------- Juros
                                    12 -------------- Tarifas
                                    13 -------------- Desconto
                                    14 -------------- Valor Bruto
                                    15 -------------- Valor Liquido
                                    16 -------------- Valor Recebido
                                    17 -------------- Saldo em Aberto
                                    18 -------------- Categoria
                                    19 -------------- Status
                                    20 -------------- Forma de Recebimento
                                    21 -------------- Data Liquidação
                                    22 -------------- Data Conciliação
                                    23 -------------- idEmpresa
                                    24 -------------- Titulo Adiantado
                                    ========================
                                --%>

                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:CheckBox ID="chkTitulo_Selecionado" runat="server" />
                                    </ItemTemplate>
                                    <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                </asp:TemplateField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                    DataTextField="idContasReceber" HeaderText="ID"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:BoundField DataField="SRazaoSocial" HeaderText="Cliente" SortExpression="SRazaoSocial" ItemStyle-HorizontalAlign="Left" ItemStyle-Width="25%" />

                                <asp:BoundField DataField="dtEmissao_ordem" HeaderText="Data Emissão" DataFormatString="{0:u}" SortExpression="dtEmissao_ordem" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="6%" />

                                <asp:BoundField DataField="dtVencimento_ordem" HeaderText="Data Vcto" DataFormatString="{0:u}" SortExpression="dtVencimento_ordem" ItemStyle-HorizontalAlign="Center" ItemStyle-Width="6%" />

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                    DataTextField="sDiasParaVencer" HeaderText="Dias Vecto"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                    DataTextField="sDscEmpresa" HeaderText="Empresa"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                    DataTextField="sDocumento" HeaderText="Pedido"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                    DataTextField="sQuantidadeParcela" HeaderText="Parcela"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                    DataTextField="sCodigo" HeaderText="Nota Fiscal"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}"
                                    DataTextField="nMulta" HeaderText="Multa"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}"
                                    DataTextField="nJuros" HeaderText="Juros"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}"
                                    DataTextField="nTarifa" HeaderText="Tarifa"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}"
                                    DataTextField="nDesconto" HeaderText="Desconto"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}"
                                    DataTextField="nValorBruto" HeaderText="Valor Bruto"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:BoundField DataField="nValorOriginal" HeaderText="Valor Líquido" DataFormatString="{0:C2}" SortExpression="nValorOriginal" ItemStyle-HorizontalAlign="Right" ItemStyle-Width="10%" />

                                <asp:BoundField DataField="nTotal" HeaderText="Valor Recebido" DataFormatString="{0:C2}">
                                    <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber" DataTextFormatString="{0:C2}"
                                    DataTextField="nSaldo" HeaderText="Saldo em Aberto"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="10%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                    DataTextField="sDscCategoriaReceber" HeaderText="Categoria"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                    DataTextField="sStatus" HeaderText="Status"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField DataNavigateUrlFields="idContasReceber"
                                    DataTextField="sDscFormaRecebimento" HeaderText="Forma de Recebimento"
                                    DataNavigateUrlFormatString="ContasReceber_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:BoundField DataField="dtLiquidado" HeaderText="Data Liquidação">
                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="dtConciliado" HeaderText="Data Conciliação">
                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="idEmpresa" HeaderText="idEmpresa">
                                    <ItemStyle Width="1%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField DataField="sTituloAdiantado" HeaderText="Titulo Adiantado">
                                    <ItemStyle Width="1%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:BoundField>

                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

            </div>
        </div>

    </asp:Panel>

    <div class="modal fade" id="Modal_Adiantamento" tabindex="-1" role="dialog" aria-labelledby="ModalAdiantamento_Label" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog modal-xl" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="modal-header-content">
                        <div class="modal-logo">
                            <asp:Image ID="Image1" runat="server" ImageUrl="~/App/img/instrucaoTecnica_pdf.jpg" AlternateText="Instrução Técnica" />
                        </div>
                        <div class="modal-title-container">
                            <h4 class="modal-title" id="ModalAdiantamento_Label">Adiantar Títulos</h4>
                        </div>
                    </div>

                </div>
                <div class="modal-body">

                    <div class="col-lg-12 row">
                        <asp:UpdatePanel ID="UpdatePanelMensagem" runat="server">
                            <ContentTemplate>
                                <uc1:MensagemPagina runat="server" ID="Adiantamento_MensagemPagina" />
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                    <div class="form-group row">
                        <div class="col-lg-12">

                            <asp:UpdatePanel ID="UpdatePanel_Limpacampos" runat="server">
                                <ContentTemplate>

                                    <div class="row">
                                        <div class="col-lg-4">
                                            <div class="form-group">
                                                <label>Banco Credor:</label>
                                                <asp:DropDownList ID="ddlidConta_Info_Pag" CssClass="form-control Caixa_Selecao trigger-hide-button" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-5">
                                            <div class="form-group">
                                                <label>Categoria Contas Pagar:</label>
                                                <asp:DropDownList ID="ddlidCategoriaPagar" CssClass="form-control Caixa_Selecao trigger-hide-button" runat="server"></asp:DropDownList>
                                            </div>
                                        </div>

                                        <div class="col-lg-2">
                                            <div class="form-group">
                                                <label>Metódo de Cálculo</label>
                                                <asp:DropDownList ID="ddlsTipoCalculo" CssClass="form-control Caixa_Selecao" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlsTipoCalculo_SelectedIndexChanged">
                                                    <asp:ListItem Text="Selecione um Metódo" Value="0"></asp:ListItem>
                                                    <asp:ListItem Text="Manual" Value="M"></asp:ListItem>
                                                    <asp:ListItem Text="Proporcional" Value="P"></asp:ListItem>
                                                </asp:DropDownList>
                                            </div>
                                        </div>


                                    </div>
                                    <div class="row">
                                        <div class="col-lg-3">
                                            <div class="form-group">
                                                <label>Data do Adiantamento</label>
                                                <asp:TextBox ID="txtdtEmissaoAdiantamento" class="form-control " runat="server" placeholder="Vencimento" MaxLength="10" type="date"></asp:TextBox>
                                            </div>
                                        </div>

                                        <div runat="server" id="div_valoresAdiantamento" visible="false">
                                            <div class="col-lg-2" runat="server" id="div_valorLiberado">
                                                <div class="form-group">
                                                    <label>Valor Liberado</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnValorLiberado" CssClass="form-control trigger-hide-button" Style="z-index: 0;" placeholder="R$ 0.000,00" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Juros ao mês %:</label>
                                                    <asp:TextBox ID="txtnTaxaJuros" CssClass="form-control trigger-hide-button" placeholder="00,00%" MaxLength="20" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="div_totalJuros">
                                                <div class="form-group">
                                                    <label>Total Juros</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnTotalJurosValor" CssClass="form-control trigger-hide-button" Style="z-index: 0;" placeholder="R$ 0.000,00" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>IOF Diário %</label>
                                                    <asp:TextBox ID="txtnIOF" CssClass="form-control trigger-hide-button" placeholder="00,0000000%" MaxLength="20" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>IOF Adicional %</label>
                                                    <asp:TextBox ID="txtnIOFAdicional" CssClass="form-control trigger-hide-button" placeholder="00,0000000%" MaxLength="20" runat="server"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-lg-2" runat="server" id="div_totalIOF">
                                                <div class="form-group">
                                                    <label>Total IOF</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnTotalIOFValor" CssClass="form-control trigger-hide-button" Style="z-index: 0;" placeholder="R$ 0.000,00" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="col-lg-2">
                                                <div class="form-group">
                                                    <label>Tarifas:</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">R$</span>
                                                        <asp:TextBox ID="txtnTarifas" CssClass="form-control trigger-hide-button" Style="z-index: 0;" placeholder="R$ 0.000,00" MaxLength="20" runat="server"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>


                                    <div class="col-lg-12 text-right">
                                        <asp:Button ID="btnSimularAdiantamento" runat="server" CssClass="btn btn-info" OnClick="btnSimularAdiantamento_Click" Text="Simular" />
                                    </div>


                                    <asp:Button ID="btnLimpaCampos" runat="server" Text="Limpar Campos" OnClick="btnLimpaCampos_Click" Style="display: none;" />
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>

                    <div id="simulacaoGrid_Detalhe" runat="server">
                        <div class="row">
                            <div class="col-lg-12">
                                <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                    <ContentTemplate>
                                        <asp:GridView ID="gvSimulacaoAdiantamento_Detalhe" runat="server" AutoGenerateColumns="False" CssClass="gridViewStyle" Font-Size="11px">
                                            <Columns>
                                                <asp:BoundField DataField="idContasReceber_Detalhe" HeaderText="ID" SortExpression="idContasReceber" />
                                                <asp:BoundField DataField="sDscCliente_Detalhe" HeaderText="Cliente" SortExpression="sDscCliente" />
                                                <asp:BoundField DataField="dtVencimento_Detalhe" HeaderText="Vencimento" SortExpression="dtVencimento_Detalhe" DataFormatString="{0:dd-MM-yyyy}" ItemStyle-Width="8%" />
                                                <asp:BoundField DataField="nDias_Detalhe" HeaderText="N° Dias" SortExpression="nDias_Detalhe" ItemStyle-Width="5%" />
                                                <asp:BoundField DataField="nValorTitulo" HeaderText="Valor do Título" SortExpression="valorOriginal" DataFormatString="{0:C2}" />
                                                <asp:BoundField DataField="nJurosTotal_Detalhe" HeaderText="Valor Juros" SortExpression="nJurosTotal" DataFormatString="{0:C2}" />
                                                <asp:BoundField DataField="nSomaIOF_Detalhe" HeaderText="Valor IOF" SortExpression="nSomaIOF_Detalhe" DataFormatString="{0:C2}" />
                                                <asp:BoundField DataField="nDivisaoTarifa" HeaderText="Tarifa" SortExpression="nDivisaoTarifa" DataFormatString="{0:C2}" />
                                                <asp:BoundField DataField="nValorLiberado_Detalhe" HeaderText="Valor Liberado" SortExpression="nValorLiberado_Detalhe" DataFormatString="{0:C2}" />
                                                <asp:BoundField DataField="nTotalDespesas" HeaderText="Total Despesas" SortExpression="nTotalDespesas" DataFormatString="{0:C2}" />
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>

                    <div id="simulacaoGrid" runat="server">
                        <div class="row">
                            <div class="col-lg-12">
                                <asp:UpdatePanel ID="UpdatePanel1" runat="server">
                                    <ContentTemplate>
                                        <h3 id="tituloResumo" runat="server" style="text-align: left;" visible="false">Resumo</h3>
                                        <asp:GridView ID="gvSimulacaoAdiantamento" runat="server" AutoGenerateColumns="False" CssClass="gridViewStyle" Font-Size="11px">
                                            <Columns>
                                                <asp:BoundField DataField="nSomaValorOriginal" HeaderText="Valor do Título" SortExpression="nSomaValorOriginal" DataFormatString="{0:C2}" ItemStyle-Width="15%" />
                                                <asp:BoundField DataField="nJurosTotal" HeaderText="Valor Juros" SortExpression="nJurosTotal" DataFormatString="{0:C2}" ItemStyle-Width="11%" />
                                                <asp:BoundField DataField="nSomaIOF" HeaderText="Valor IOF" SortExpression="nSomaIOF" DataFormatString="{0:C2}" ItemStyle-Width="11%" />
                                                <asp:BoundField DataField="nTarifas" HeaderText="Tarifa" SortExpression="nTarifas" DataFormatString="{0:C2}" ItemStyle-Width="11%" />
                                                <asp:BoundField DataField="nValorLiberado" HeaderText="Total Liberado" SortExpression="nValorLiberado" DataFormatString="{0:C2}" ItemStyle-Width="18%" />
                                                <asp:BoundField DataField="nDespesas" HeaderText="Total Despesas" SortExpression="nDespesas" DataFormatString="{0:C2}" ItemStyle-Width="13%" />
                                            </Columns>
                                        </asp:GridView>
                                    </ContentTemplate>
                                    <Triggers>
                                        <asp:AsyncPostBackTrigger ControlID="btnSimularAdiantamento" EventName="Click" />
                                    </Triggers>
                                </asp:UpdatePanel>
                            </div>
                        </div>
                    </div>

                </div>
                <div class="modal-footer">
                    <div class="col-lg-12 text-right">
                        <asp:UpdatePanel ID="UpdatePanelBotao" runat="server">
                            <ContentTemplate>
                                <asp:Button ID="btnConfirmarAdiantamento" runat="server" CssClass="btn btn-success" OnClick="btnConfirmarAdiantamento_Click" Text="Confirmar Adiantamento" Visible="true" Style="margin-right: 10px;" />
                                <button type="button" class="btn btn-danger" data-dismiss="modal">Cancelar</button>
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                </div>
            </div>
        </div>

    </div>

    <script type="text/javascript">
        $(document).ready(function () {
            $('#<%=cmdAdiantamento.ClientID%>').hide();

            $(document).on('click', '[id*=chkTitulo_Selecionado]', function () {
                if ($('[id*=chkTitulo_Selecionado]:checked').length > 0) $('#<%=cmdAdiantamento.ClientID%>').show();
                else $('#<%=cmdAdiantamento.ClientID%>').hide();
            });
        });

        $(document).ready(function () {
            $('#Modal_Adiantamento').on('keypress', function (e) {
                if (e.which == 13) { // Enter
                    if ($('#<%= btnConfirmarAdiantamento.ClientID %>').is(':visible')) {
                        e.preventDefault();
                        $('#<%= btnConfirmarAdiantamento.ClientID %>').click();
                    } else {
                        e.preventDefault();
                    }
                }
            });

            $(document).on('shown.bs.modal', '#Modal_Adiantamento', function () {
                $(this).focus();
            });
        });

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

</asp:Content>
