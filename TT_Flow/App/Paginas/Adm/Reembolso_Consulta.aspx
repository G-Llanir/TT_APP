<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Reembolso_Consulta.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Reembolso_Consulta" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Reembolso de Despesas"></asp:Label>
                <small class="text-muted"></small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Reembolso" />
        </div>
        <div class="col-lg-12">

            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                </div>
                <div class="panel-body">
                    <div class="form-group">
                        <div class="col-lg-4 row">
                            <div class="form-group">
                                <asp:TextBox ID="txtStatus" class="form-control" runat="server" placeholder="Pesquisar"></asp:TextBox>
                            </div>
                        </div>

                        <div class="col-lg-2">
                            <div class="form-group">
                                <asp:DropDownList ID="ddlsStatus" runat="server" class="form-control yes_no select">
                                    <asp:ListItem Value="0">Selecione o Status</asp:ListItem>
                                    <asp:ListItem Value="1">Aberto</asp:ListItem>
                                    <asp:ListItem Value="2">Finalizado</asp:ListItem>
                                    <asp:ListItem Value="3">Aprovado</asp:ListItem>
                                    <asp:ListItem Value="4">Rejeitado</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>

                        <div class="col-lg-4">
                            <asp:Button ID="cmdPesquisar" class="btn btn-md btn-primary" runat="server" Text="Pesquisar" />
                            <asp:Button ID="cmdNovo" class="btn btn-md btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-12">
            <asp:Panel ID="div_gvRelatorio" class="" runat="server">
                <div id="resultado">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">
                                <asp:GridView ID="gvRelatorio" class="table table-striped table-bordered table-hover"
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    Font-Names="Tahoma" ShowFooter="False" Font-Overline="False" Font-Size="Small" OnRowDataBound="gvRelatorio_RowDataBound">
                                    <Columns>

                                        <asp:HyperLinkField DataNavigateUrlFields="idDespesas"
                                            DataTextField="idDespesas" HeaderText="ID"
                                            DataNavigateUrlFormatString="Reembolso_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="20%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idDespesas"
                                            DataTextField="dtInclusao" HeaderText="Data"
                                            DataNavigateUrlFormatString="Reembolso_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="10%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idDespesas"
                                            DataTextField="sDscUsuario" HeaderText="Colaborador"
                                            DataNavigateUrlFormatString="Reembolso_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="10%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idDespesas"
                                            DataTextField="sDscPedido" HeaderText="Centro de Custo"
                                            DataNavigateUrlFormatString="Reembolso_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="20%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idDespesas"
                                            DataTextField="sDscMotivo" HeaderText="Motivo"
                                            DataNavigateUrlFormatString="Reembolso_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="20%" />
                                            <ItemStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idDespesas"
                                            DataTextField="nValor" HeaderText="Valor Total (R$)"
                                            DataNavigateUrlFormatString="Reembolso_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="10%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idDespesas"
                                            DataTextField="sStatus" HeaderText="Status"
                                            DataNavigateUrlFormatString="Reembolso_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="10%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

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