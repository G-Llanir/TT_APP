<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="ChangeLog.aspx.cs" Inherits="TT_Flow.App.Paginas.ChangeLog" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 1000px" runat="server" visible="false"></div>

    <style type="text/css">
        .invisivel {
            display: none;
        }

        .green, .red, .orange, .yellow, .darkYellow, .blue, .darkBlue, .lightGreen, .gray {
            border-radius: 10px;
            width: 20px;
            height: 20px;
            align-content: center;
        }

        .green {
            background-color: green;
        }

        .red {
            background-color: red;
        }

        .orange {
            background-color: orange;
        }

        .yellow {
            background-color: yellow;
        }

        .darkYellow {
            background-color: #ffc910;
        }

        .blue {
            background-color: skyblue;
        }

        .darkBlue {
            background-color: blue;
        }

        .lightGreen {
            background-color: lightgreen;
        }

        .gray {
            background-color: gray;
        }

        .tabela {
            border: solid 2px gray;
            border-radius: 7.5px;
        }

            .tabela thead tr {
                border-bottom: solid 3px darkgray;
            }
    </style>

    <asp:UpdatePanel ID="updtPanelGeral" runat="server">
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
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para Pesquisa</h3>
                        </div>
                        <div class="panel-body">
                            <div class="row">

                                <div class="col-lg-3">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>

                                <div class="col-lg-2">
                                    <asp:DropDownList ID="ddlStatus" class="form-control Caixa_Selecao" runat="server">
                                        <asp:ListItem Value="0">Todos os Status</asp:ListItem>
                                        <asp:ListItem Selected="True" Value="-99">Todos os Não Finalizados</asp:ListItem>
                                        <asp:ListItem Value="1">Pendente de Teste</asp:ListItem>
                                        <asp:ListItem Value="6">Em Teste</asp:ListItem>
                                        <asp:ListItem Value="9">Teste Pausado</asp:ListItem>
                                        <asp:ListItem Value="2">Teste Aprovado</asp:ListItem>
                                        <asp:ListItem Value="3">Teste Rejeitado</asp:ListItem>
                                        <asp:ListItem Value="7">Em Produção</asp:ListItem>
                                        <asp:ListItem Value="8">Implantação / Treinamento</asp:ListItem>
                                        <asp:ListItem Value="4">Aprovado</asp:ListItem>
                                        <asp:ListItem Value="5">Rejeitado</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-2">
                                    <asp:DropDownList ID="ddlUsuario" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>

                                <div class="col-lg-2">
                                    <asp:DropDownList ID="ddlFiltro" class="form-control Caixa_Selecao" runat="server">
                                        <asp:ListItem Value="0">Todos</asp:ListItem>
                                        <asp:ListItem Value="1">Possui Vídeos</asp:ListItem>
                                        <asp:ListItem Value="2">Possui Documentação</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-3">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao_Pesquisa(this)" />
                                    <asp:HyperLink ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" Target="_blank" NavigateUrl="~/App/Paginas/IT/ChangeLog_Detalhe.aspx?id=0"></asp:HyperLink>
                                    <asp:Button ID="cmdManuais" class="btn btn-info cmdManuais" runat="server" Text="Manuais" />
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
                                <div class="table-responsive">
                                    <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                        <Columns>

                                            <asp:TemplateField>
                                                <ItemTemplate>
                                                    <asp:Panel runat="server" ID="CirclePanel" CssClass='<%# Eval("sCor_Status") %>'></asp:Panel>
                                                </ItemTemplate>
                                                <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:TemplateField>

                                            <asp:BoundField DataField="sDscStatus" HeaderText="Status">
                                                <HeaderStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                DataTextField="idItem" HeaderText="ID"
                                                DataNavigateUrlFormatString="ChangeLog_Detalhe.aspx?id={0}">
                                                <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                DataTextField="sVersao" HeaderText="Versão"
                                                DataNavigateUrlFormatString="ChangeLog_Detalhe.aspx?id={0}">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                DataTextField="sTipoAlteracao" HeaderText="Tipo da Alteração"
                                                DataNavigateUrlFormatString="ChangeLog_Detalhe.aspx?id={0}">
                                                <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idItem"
                                                DataTextField="sDscTituloAlteracao" HeaderText="Título"
                                                DataNavigateUrlFormatString="ChangeLog_Detalhe.aspx?id={0}">
                                                <HeaderStyle Width="38%" />
                                                <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:BoundField DataField="sVideo" HeaderText="Possui Vídeos?">
                                                <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDocumentoCompleto" HeaderText="Possui Documentação?">
                                                <ItemStyle Width="8%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscUsuarioAtualizacao" HeaderText="Por">
                                                <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
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

            <div class="modal fade" id="modalManuais">
                <div class="modal-dialog" style="width: 55%;">
                    <div class="modal-content">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h3 class="modal-title">Manuais</h3>
                        </div>
                        <div class="modal-body">

                            <div style="padding-bottom: 10px;">
                                <div class="tabela form-group">
                                    <asp:Literal runat="server" ID="ltrTabela_Manuais"></asp:Literal>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
