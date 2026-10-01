<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ContasPagar.aspx.cs" Inherits="TT_Hub.App.Paginas.Adm.Financeiro.ContasPagar" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>
<%@ Register Src="~/App/Controles/DropDownList_Padrao.ascx" TagPrefix="uc1" TagName="DropDownList_Padrao" %>
<%@ Register Src="~/App/Controles/TextBox_Padrao.ascx" TagPrefix="uc1" TagName="TextBox_Padrao" %>

<asp:Content runat="server" ID="Content1" ContentPlaceHolderID="head"></asp:Content>
<asp:Content runat="server" ID="Content2" ContentPlaceHolderID="cphCorpo">

    <uc1:Manual runat="server" ID="manual" />

    <div id="hdd">
        <asp:HiddenField runat="server" ID="hddsFiltro" Value="N" />
    </div>

    <div class="col-lg-12">
        <h1>
            <asp:Label ID="lblTituloPagina" runat="server" Text="Contas a Pagar"></asp:Label>
            <small>
                <asp:Label ID="lblSubTituloPagina" runat="server" Text="Consulta"></asp:Label></small>
        </h1>
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
                                <asp:ListItem Value="0" Text="Vencimento" Selected="True"></asp:ListItem>
                                <asp:ListItem Value="1" Text="Emissão"></asp:ListItem>
                                <asp:ListItem Value="2" Text="Liquidação"></asp:ListItem>
                            </uc1:DropDownList_Padrao>
                        </div>
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidCategoriaPagar" />
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidCredor" />
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidEmpresa" />
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidCentroDeCusto" />
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidFormaPagamento" />
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidAprovado" />
                    </div>

                    <div class="col-lg-2 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlsStatus">
                            <asp:ListItem Selected="True" Value="">Todos os Status</asp:ListItem>
                            <asp:ListItem Value="Liquidado">Liquidado</asp:ListItem>
                            <asp:ListItem Value="Em Aberto">Em Aberto</asp:ListItem>
                            <asp:ListItem Value="Em Atraso">Em Atraso</asp:ListItem>
                            <asp:ListItem Value="Vencendo Hoje">Vencendo Hoje</asp:ListItem>
                            <asp:ListItem Value="A vencer em 7 dias">A vencer em 7 dias</asp:ListItem>
                        </uc1:DropDownList_Padrao>
                    </div>

                    <div class="col-lg-2 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlsAdiantado">
                            <asp:ListItem Selected="True" Value="T">Todos os Títulos</asp:ListItem>
                            <asp:ListItem Value="S">Adiantamentos</asp:ListItem>
                            <asp:ListItem Value="N">Não Adiantados</asp:ListItem>
                        </uc1:DropDownList_Padrao>
                    </div>

                    <div class="col-lg-4 form-group">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlidConta_Info_Pag" />
                    </div>

                    <div class="col-lg-4">
                        <uc1:DropDownList_Padrao runat="server" ID="ddlsConciliado">
                            <asp:ListItem Selected="True" Value="T">Conciliados e Não Conciliados</asp:ListItem>
                            <asp:ListItem Value="S">Conciliados</asp:ListItem>
                            <asp:ListItem Value="N">Não Conciliado</asp:ListItem>
                            <asp:ListItem Value="P">Parcialmente Conciliado</asp:ListItem>
                        </uc1:DropDownList_Padrao>
                    </div>

                    <div class="col-lg-4">
                        <uc1:TextBox_Padrao runat="server" ID="txtPesquisa" Placeholder="Pesquisar" />
                    </div>

                    <div class="col-lg-4">
                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisar()" />
                        <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Nova Conta a Pagar" OnClick="cmdNovo_Click" />
                        <asp:Button ID="cmdExportar" class="btn btn-info" runat="server" Text="Exportar em Excel" OnClick="cmdExportar_Click" />
                    </div>

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
                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                            ShowFooter="true" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                            <Columns>

                                <%--
                                    ========================
                                    0 --------------- ID
                                    1 --------------- Credor
                                    2 --------------- Data Emissão
                                    3 --------------- Data Vencimento
                                    4 --------------- Dias Até Vencimento
                                    5 --------------- Referência
                                    6 --------------- Parcela
                                    7 --------------- NF/Referência
                                    8 --------------- Multa
                                    9 --------------- Juros
                                    10 --------------- Desconto
                                    11 --------------- Valor Bruto
                                    12 --------------- Valor Líquido
                                    13 --------------- Valor Pago
                                    14 -------------- Saldo em Aberto
                                    15 -------------- Categoria
                                    16 -------------- Status
                                    17 -------------- Empresa
                                    18 -------------- Data Liquidação
                                    19 -------------- Data Conciliação
                                    ========================
                                --%>

                                <asp:HyperLinkField HeaderText="ID" DataNavigateUrlFields="idContasPagar" DataTextField="idContasPagar" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="4%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Credor" DataNavigateUrlFields="idContasPagar" DataTextField="sDscCredor" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Data Emissão" DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:u}" DataTextField="dtEmissao_ordem" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Data Vencimento" DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:u}" DataTextField="dtVencimento_ordem" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Dias Até Venc." DataNavigateUrlFields="idContasPagar" DataTextField="sDiasParaVencer" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Referência" DataNavigateUrlFields="idContasPagar" DataTextField="sDocumento" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="9%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Parcela" DataNavigateUrlFields="idContasPagar" DataTextField="sQuantidadeParcela" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="6%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="NF/Referência" DataNavigateUrlFields="idContasPagar" DataTextField="sCodigo" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Multa" DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:C2}" DataTextField="nMulta" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Juros" DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:C2}" DataTextField="nJuros" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Desconto" DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:C2}" DataTextField="nDesconto" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Valor Bruto" DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:C2}" DataTextField="nValorBruto" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Valor Líquido" DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:C2}" DataTextField="nValorOriginal" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Valor Pago" DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:C2}" DataTextField="nTotal" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Saldo em Aberto" DataNavigateUrlFields="idContasPagar" DataTextFormatString="{0:C2}" DataTextField="nSaldo" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Categoria" DataNavigateUrlFields="idContasPagar" DataTextField="sDscCategoriaPagar" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Status" DataNavigateUrlFields="idContasPagar" DataTextField="sStatus" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:HyperLinkField HeaderText="Empresa" DataNavigateUrlFields="idContasPagar" DataTextField="sDscEmpresa" DataNavigateUrlFormatString="ContasPagar_Detalhe.aspx?id={0}">
                                    <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:HyperLinkField>

                                <asp:BoundField HeaderText="Data Liquidação" DataField="dtLiquidado">
                                    <ItemStyle Width="1%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:BoundField>

                                <asp:BoundField HeaderText="Data Conciliação" DataField="dtConciliado">
                                    <ItemStyle Width="1%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                </asp:BoundField>

                            </Columns>
                        </asp:GridView>
                    </div>
                </div>

            </div>
        </div>

    </asp:Panel>

</asp:Content>
