<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="EmissaoNFE.aspx.cs" Inherits="TT_Flow.App.Paginas.Adm.Faturamento.EmissaoNFE" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Emissor NF-e"></asp:Label><small> Consulta</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-filter"></i>&nbsp;Filtro para pesquisa </h3>
                </div>
                <div class="panel-body">

                    <div class="col-lg-4 padd-0">
                        <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar" runat="server"></asp:TextBox>
                    </div>

                    <div class="col-lg-2">
                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                        <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                    </div>

                </div>
            </div>

            <div class="panel panel-primary">
                <div class="panel-body" id="panelExibirOPI">

                    <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    <asp:Panel ID="pnResultado" class="" runat="server">

                        <div class="table-responsive">
                            <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                <Columns>

                                    <asp:BoundField DataField="idPedido" HeaderText="ID">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nNumeroPedido" HeaderText="N° Pedido">
                                        <ItemStyle Width="7%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sReferencia" HeaderText="Referência">
                                        <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                        DataTextField="sCliente" HeaderText="Cliente"
                                        DataNavigateUrlFormatString="EmissaoNFE_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="30%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:BoundField DataField="dtPedido" HeaderText="Data">
                                        <ItemStyle Width="8%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="statusNota" HeaderText="Status">
                                        <ItemStyle Width="15%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sObservacaoXML" HeaderText="Observação">
                                        <ItemStyle Width="25%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sEmissaoNfe" HeaderText="EmissaoNfe">
                                        <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="chNFe" HeaderText="Chave">
                                        <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sCaminho_UniNFe" HeaderText="Caminho">
                                        <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sCPF_CNPJ" HeaderText="CNPJ">
                                        <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                    </asp:BoundField>
                                </Columns>
                            </asp:GridView>
                        </div>
                    </asp:Panel>

                </div>
            </div>
        </div>
    </div>

</asp:Content>