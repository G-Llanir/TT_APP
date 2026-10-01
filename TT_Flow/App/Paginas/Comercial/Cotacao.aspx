<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Cotacao.aspx.cs" Inherits="TT_Flow.App.Paginas.Comercial.Cotacao" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>
<%@ Register Src="~/App/Controles/PainelAtualizacao.ascx" TagPrefix="uc1" TagName="PainelAtualizacao" %>
<%@ Register Src="~/App/Controles/Manual.ascx" TagPrefix="uc1" TagName="Manual" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <uc1:Manual runat="server" ID="manual" />

    <style>
        .invisivel {
            display: none;
        }

        .label {
            font-size: 15px;
            font-weight: bold;
        }

        .card {
            box-shadow: 0px 2px 8px 2px rgba(0,0,0,0.3);
            transition: 0.3s;
            width: 100%;
            border-radius: 5px;
        }

        .card-body {
            padding: 10px;
            display: flex;
            align-items: flex-start;
        }

        .flex-grow-1 {
            flex-grow: 1;
            margin-left: 15px;
        }

        .img-thumbnail {
            border: none;
            width: 100px;
            height: auto;
        }

        .me-3 {
            margin-right: 1rem;
            padding: 2px;
            border-bottom: 1px dashed black;
        }

        .success {
            color: #5cb85c !important;
        }

        .warning {
            color: #f0ad4e !important;
        }

        .danger {
            color: #d9534f !important;
        }

        .default {
            color: #777 !important;
        }
    </style>

    <asp:UpdatePanel ID="UpdatePanel" runat="server">
        <ContentTemplate>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Consulta</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">

                            <div class="row">

                                <div class="col-lg-3 form-group">
                                    <asp:TextBox ID="txtPesquisa" runat="server" class="form-control" placeholder="Pesquisar" MaxLength="200"></asp:TextBox>
                                </div>

                                <div class="col-lg-4 form-group">
                                    <asp:DropDownList ID="ddlParceiros" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlStatus" runat="server" CssClass="form-control Caixa_Selecao">
                                        <asp:ListItem Value="0" Text="Todos os Status"></asp:ListItem>
                                        <asp:ListItem Value="1" Text="Em Aberto"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Convertida em Orçamento"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-2 form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisa(this)" />
                                    <asp:HyperLink ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" Target="_blank" NavigateUrl="/App/Paginas/Comercial/Cotacao_Detalhe.aspx?id=0"></asp:HyperLink>
                                </div>

                            </div>

                        </div>
                    </div>
                </div>
            </div>

            <asp:Panel ID="pnResultado" runat="server">
                <div id="resultado">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">

                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                    <Columns>

                                        <asp:HyperLinkField DataNavigateUrlFields="idCotacao"
                                            DataTextField="nNumeroPedido" HeaderText="N°"
                                            DataNavigateUrlFormatString="Cotacao_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idCotacao"
                                            DataTextField="sReferencia" HeaderText="Referência"
                                            DataNavigateUrlFormatString="Cotacao_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idCotacao"
                                            DataTextField="sDscParceiro" HeaderText="Parceiro"
                                            DataNavigateUrlFormatString="Cotacao_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idCotacao" DataNavigateUrlFormatString="Cotacao_Detalhe.aspx?id={0}"
                                            DataTextField="nTotal" HeaderText="Total" DataTextFormatString="{0:N2}">
                                            <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idCotacao"
                                            DataTextField="sDscStatus" HeaderText="Status"
                                            DataNavigateUrlFormatString="Cotacao_Detalhe.aspx?id={0}">
                                            <ItemStyle Width="13%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idCotacao" DataNavigateUrlFormatString="Cotacao_Detalhe.aspx?id={0}"
                                            DataTextField="dtPedido" HeaderText="Criado em" DataTextFormatString="{0:dd/MM/yyyy}">
                                            <ItemStyle Width="12%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

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

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>