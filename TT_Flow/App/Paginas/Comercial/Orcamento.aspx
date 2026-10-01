<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Orcamento.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.Orcamento" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>

            <uc1:Manual runat="server" ID="manual" />

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Consulta</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>

                <div class="col-lg-12" runat="server" id="div_FiltroPesquisa">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-filter"></i>&nbsp;Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">

                            <div class="row">

                                <div class="col-lg-4 form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar por Referência" runat="server"></asp:TextBox>
                                </div>

                                <div class="col-lg-6 form-group">
                                    <asp:DropDownList ID="ddlidCliente" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlTipoOrcamento" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                            </div>

                            <div class="row">

                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlFiltro_Ano" runat="server" class="form-control"></asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group">
                                    <asp:DropDownList ID="ddlFluxoOrcamento" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlidEmpresa" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlStatus" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group" runat="server" id="div_Vendedores">
                                    <asp:DropDownList ID="ddlVendedores" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                            </div>

                            <div class="row">

                                <div class="col-lg-3">
                                    <asp:DropDownList ID="ddlRevisoes" runat="server" class="form-control Caixa_Selecao">
                                        <asp:ListItem Value="0" Text="Todos os Orçamentos" />
                                        <asp:ListItem Value="1" Text="Apenas Orçamentos" />
                                        <asp:ListItem Value="2" Text="Apenas Revisões" />
                                        <asp:ListItem Value="3" Text="Apenas Comparativos" />
                                        <asp:ListItem Value="4" Text="Apenas Redução de Impostos" />
                                        <asp:ListItem Value="5" Text="Apenas Empreitadas" />
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-3">
                                    <asp:DropDownList ID="ddlNacionalidade" runat="server" class="form-control Caixa_Selecao">
                                        <asp:ListItem Value="T" Text="Todos as Nacionalidades" />
                                        <asp:ListItem Value="N" Text="Apenas Nacionais" />
                                        <asp:ListItem Value="I" Text="Apenas Internacionais" />
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-4">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisa(this)" />
                                    <asp:HyperLink ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" NavigateUrl="/App/Paginas/Comercial/Orcamento_Detalhe.aspx?id=0" Target="_blank" />
                                </div>

                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />

            <div runat="server" id="resultado">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <div class="table-responsive">
                            <asp:GridView ID="gvConsulta" class="table table-striped table-bordered table-hover "
                                runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowCommand="gvConsulta_RowCommand" OnRowDataBound="gvConsulta_RowDataBound">
                                <Columns>

                                    <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                        DataTextField="idPedido" HeaderText="ID"
                                        DataNavigateUrlFormatString="Orcamento_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="0%" CssClass="shhh" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" CssClass="shhh" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                        DataTextField="nNumeroPedido" HeaderText="N°"
                                        DataNavigateUrlFormatString="Orcamento_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="5%" />
                                        <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idPedido"
                                        DataTextField="sReferencia" HeaderText="Referência"
                                        DataNavigateUrlFormatString="Orcamento_Detalhe.aspx?id={0}">
                                        <HeaderStyle Width="22%" />
                                        <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:BoundField DataField="sDscCliente" HeaderText="Cliente">
                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="dtPedido" HeaderText="Data">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscTipoOrcamento" HeaderText="Tipo de Orçamento">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscFluxo" HeaderText="Fluxo">
                                        <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="vlrEnvioInvoice" HeaderText="Serviços" DataFormatString="{0:N2}">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="vlrProdutos" HeaderText="Produtos" DataFormatString="{0:N2}">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nTotalOrcamento" HeaderText="Total" DataFormatString="{0:N2}">
                                        <ItemStyle Width="5%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sConfidencialCompleto" HeaderText="Confidencial">
                                        <ItemStyle Width="4%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:TemplateField HeaderText="Revisão">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkRevisao" runat="server" CssClass="btn btn-small" TabIndex="99" CommandName="Revisao" CommandArgument='<%# Eval("idPedido") %>' data-toggle="tooltip_top" title="Nova Revisão"><i class="fa fa-files-o"></i></asp:LinkButton>
                                            <asp:HiddenField runat="server" ID="hddidPedido_Vinculado" ClientIDMode="Static" Value='<%# Eval("idPedido_Vinculado") %>' />
                                        </ItemTemplate>
                                        <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Duplicar">
                                        <ItemTemplate>
                                            <asp:LinkButton ID="lnkDuplicar" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Duplicar" CommandArgument='<%# Eval("idPedido") %>' data-toggle="tooltip_top" title="Duplicar Orçamento"><i class="fa fa-plus"></i></asp:LinkButton>
                                        </ItemTemplate>
                                        <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:TemplateField>

                                    <asp:BoundField DataField="sReferencia" HeaderText="">
                                        <HeaderStyle CssClass="invisivel" />
                                        <ItemStyle Width="0%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="invisivel" />
                                    </asp:BoundField>

                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
