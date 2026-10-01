<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="NCM.aspx.cs" Inherits="TT_Flow.App.Paginas.WMS.Manutencao.NCM" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <style>
        .label {
            font-size: 100%;
            padding: .35em;
        }
    </style>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Manutenção</small></h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
        </div>
    </div>

    <div class="form-stacked row">
        <div class="col-lg-12">
            <div class="panel panel-primary">
                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-filter"></i>&nbsp;Filtro para pesquisa</h3>
                </div>
                <div class="panel-body">
                    <div class="col-lg-4 padd-0">
                        <asp:TextBox ID="txtPesquisa" class="form-control" runat="server" placeholder="Pesquisar"></asp:TextBox>
                    </div>

                    <div class="col-lg-3">
                        <asp:DropDownList ID="ddlCEST" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                    </div>

                    <div class="col-lg-5">
                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                        <asp:Button ID="cmdNovoCadastro" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovoCadastro_Click" />
                        <asp:Button ID="cmdExportar" class="btn btn-info" runat="server" Text="Exportar Excel" OnClick="cmdExportar_Click" />
                    </div>
                </div>
            </div>
        </div>
    </div>

    <asp:UpdatePanel runat="server" ID="UpdResultado">
        <ContentTemplate>
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

            <div class="form-stacked row">
                <asp:Panel ID="pnResultado" runat="server">
                    <div class="col-lg-12">
                        <div class="panel panel-primary">
                            <div class="panel-body">
                                <div class="table-responsive">

                                    <asp:GridView ID="dtgvConsulta" runat="server" class="table table-striped table-bordered table-hover" GridLines="None"
                                        CellSpacing="1" CellPadding="1" Width="100%" ShowFooter="False" Font-Names="Tahoma"
                                        Font-Overline="False" Font-Size="13px" AutoGenerateColumns="false" OnRowDataBound="dtgvConsulta_RowDataBound">
                                        <Columns>

                                            <asp:HyperLinkField DataNavigateUrlFields="idNCM" DataTextField="idNCM"
                                                DataNavigateUrlFormatString="NCM_Detalhe.aspx?id={0}" HeaderText="ID">
                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idNCM" DataTextField="sCodigoNCM"
                                                DataNavigateUrlFormatString="NCM_Detalhe.aspx?id={0}" HeaderText="Código">
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idNCM" DataTextField="sDscNCM"
                                                DataNavigateUrlFormatString="NCM_Detalhe.aspx?id={0}" HeaderText="Descrição">
                                                <ItemStyle Width="35%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:TemplateField HeaderText="CEST">
                                                <ItemTemplate>
                                                    <asp:Literal runat="server" Text='<%# Eval("sCEST") %>'></asp:Literal>
                                                </ItemTemplate>
                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sRedBC" HeaderText="Redução?">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Em">
                                                <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                                <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>

                                </div>
                            </div>
                        </div>
                    </div>
                </asp:Panel>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
