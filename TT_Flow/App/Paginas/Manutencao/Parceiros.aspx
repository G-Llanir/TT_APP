<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Parceiros.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Parceiros" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>



<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
        <ContentTemplate>
            <style>
                .usuarios {
                    display: flex;
                    flex-wrap: wrap;
                }

                    .usuarios > .label {
                        margin: 0 2.5px 2.5px 0;
                        padding: .35em;
                        font-size: 85%;
                        overflow: hidden;
                        text-overflow: ellipsis;
                        max-width: 100%;
                    }

                .col-lg-12 > .usuarios > .label {
                    font-size: 100% !important;
                }
            </style>
            <script type="text/javascript">
                function MudarBotao() {
                    document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
                }
            </script>
            <div class="form-stacked row" id="DIV_PESQUISA" runat="server">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small>
                            <asp:Label ID="lblSubTituloPagina" runat="server" Text="SUB_TITULO"></asp:Label></small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">

                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">
                                <div class="col-lg-4">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlTipoParceiro" runat="server" class="form-control" attrname="Tipo de Parceiro"></asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-lg-4">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                                </div>
                            </div>
                        </div>
                    </div>
                </div>


                <div class="col-lg-12">
                    <asp:Panel ID="pnResultado" class="" runat="server">
                        <div id="resultado">
                            <div class="panel panel-primary">
                                <div class="panel-body">
                                    <div class="table-responsive">
                                        <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                            runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                            <Columns>

                                                <asp:HyperLinkField DataNavigateUrlFields="idCliente"
                                                    DataTextField="idCliente" HeaderText="ID"
                                                    DataNavigateUrlFormatString="Parceiros_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="5%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idCliente"
                                                    DataTextField="sRazaoSocial" HeaderText="Nome / Razão Social"
                                                    DataNavigateUrlFormatString="Parceiros_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="28%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idCliente"
                                                    DataTextField="sNomeFantasia" HeaderText="Fantasia"
                                                    DataNavigateUrlFormatString="Parceiros_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="20%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idCliente"
                                                    DataTextField="sCPF_CNPJ" HeaderText="CPF/CNPJ/VAT/FEIN"
                                                    DataNavigateUrlFormatString="Parceiros_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="13%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idCliente"
                                                    DataTextField="sDscTipoSituacaoCliente" HeaderText="Situação"
                                                    DataNavigateUrlFormatString="Parceiros_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="10%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idCliente"
                                                    DataTextField="sTipoPesquisa" HeaderText="Tipo Pesquisa"
                                                    DataNavigateUrlFormatString="Parceiros_Detalhe.aspx?id={0}">
                                                    <HeaderStyle Width="15%" />
                                                    <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:TemplateField HeaderText="Tipo de Parceiro">
                                                    <ItemTemplate>
                                                        <div class="usuarios">
                                                            <asp:Literal Text='<%# Eval("sDscTipoParceiro") %>' runat="server" />
                                                        </div>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:TemplateField>

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </asp:Panel>
                    <asp:Panel ID="pnMensagem" runat="server">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </asp:Panel>
                </div>
            </div>
        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
