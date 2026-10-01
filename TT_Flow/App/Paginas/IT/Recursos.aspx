<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Recursos.aspx.cs" Inherits="TT_Flow.App.Paginas.IT.Recursos" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <div id="DIV_ESPACO" style="height: 1000px" runat="server" visible="false"></div>

    <div class="form-stacked row">

        <style>
            .linkFilhas a {
                cursor: pointer;
            }

            .tabela {
                border: solid 2px gray;
                border-radius: 7.5px;
            }

                .tabela thead tr {
                    border-bottom: solid 3px darkgray;
                }
        </style>

        <div class="col-lg-12">
            <h1>
                <asp:Label ID="lblTituloPagina" runat="server" Text="Permissões"></asp:Label><small> Consulta</small>
            </h1>
            <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="Permissões" />
        </div>

        <div class="col-lg-12">
            <div class="panel panel-primary">

                <div class="panel-heading">
                    <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para Pesquisa</h3>
                </div>
                <div class="panel-body" style="padding: 15px 0 15px 0;">
                    <div class="col-lg-12">

                        <div class="row">

                            <div class="col-lg-4 form-group">
                                <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                            </div>

                            <div class="col-lg-4 form-group">
                                <asp:DropDownList ID="ddlidRecursosPai" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                            </div>

                            <div class="col-lg-2 form-group">
                                <asp:DropDownList ID="ddlidSistema" runat="server" class="form-control Caixa_Selecao">
                                    <asp:ListItem Value="0" Text="Todos os Sistemas"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="HUB"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="T-Flow"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="Cloud-TTG"></asp:ListItem>
                                    <asp:ListItem Value="4" Text="Webmail"></asp:ListItem>
                                    <asp:ListItem Value="5" Text="Usuário"></asp:ListItem>
                                    <asp:ListItem Value="6" Text="Logoff"></asp:ListItem>
                                    <asp:ListItem Value="7" Text="Help Desk"></asp:ListItem>
                                    <asp:ListItem Value="8" Text="Área Colaborador"></asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <div class="col-lg-2 form-group">
                                <asp:DropDownList ID="ddlidStatus" runat="server" class="form-control Caixa_Selecao">
                                    <asp:ListItem Value="A" Text="Todos os Status"></asp:ListItem>
                                    <asp:ListItem Value="S" Text="Ativo"></asp:ListItem>
                                    <asp:ListItem Value="N" Text="Inativo"></asp:ListItem>
                                </asp:DropDownList>
                            </div>

                        </div>

                        <div class="row">

                            <div class="col-lg-3 form-group">
                                <asp:DropDownList ID="ddlFiltro" runat="server" class="form-control Caixa_Selecao">
                                    <asp:ListItem Value="0" Text="Todas as Permissões"></asp:ListItem>
                                    <asp:ListItem Value="1" Text="Possui URL"></asp:ListItem>
                                    <asp:ListItem Value="2" Text="Permissão Pai"></asp:ListItem>
                                    <asp:ListItem Value="3" Text="É Menu"></asp:ListItem>
                                    <asp:ListItem Value="4" Text="Página Inicial"></asp:ListItem>
                                    <asp:ListItem Value="5" Text="Possui Ícone"></asp:ListItem>
                                </asp:DropDownList>
                            </div>

                            <div class="col-lg-2 form-group">
                                <asp:DropDownList ID="ddlAndar" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                            </div>

                            <div class="col-lg-2 form-group">
                                <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" />
                                <asp:HyperLink ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" Target="_blank" NavigateUrl="~/App/Paginas/IT/Recursos_Detalhe.aspx?id=0"></asp:HyperLink>
                            </div>

                        </div>

                    </div>
                </div>
            </div>
        </div>

        <div class="col-lg-12">
            <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
        </div>

        <div class="col-lg-12">
            <asp:Panel ID="pnResultado" class="" runat="server">
                <div class="panel panel-primary">
                    <div class="panel-body">
                        <div class="table-responsive">
                            <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover"
                                runat="server" Width="100%" CellSpacing="0" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowDataBound="dtgvConsulta_RowDataBound">
                                <Columns>

                                    <asp:HyperLinkField DataNavigateUrlFields="idRecurso"
                                        DataTextField="idRecurso" HeaderText="ID"
                                        DataNavigateUrlFormatString="Recursos_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idRecurso"
                                        DataTextField="sDscRecurso" HeaderText="Descrição"
                                        DataNavigateUrlFormatString="Recursos_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="sDsc" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="sURL"
                                        DataTextField="sURL" HeaderText="URL"
                                        DataNavigateUrlFormatString="{0}">
                                        <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idSistema"
                                        DataTextField="sNomeSistema" HeaderText="Sistema"
                                        DataNavigateUrlFormatString="Recursos_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataNavigateUrlFields="idRecursoPai"
                                        DataTextField="sRecursoPai" HeaderText="Permissão Pai"
                                        DataNavigateUrlFormatString="Recursos_Detalhe.aspx?id={0}">
                                        <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                    </asp:HyperLinkField>

                                    <asp:HyperLinkField DataTextField="nFilhas" HeaderText="N° Filhas">
                                        <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="linkFilhas" />
                                    </asp:HyperLinkField>

                                    <asp:BoundField DataField="nOrdem" HeaderText="Ordem">
                                        <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="nOrdem_Menu" HeaderText="Andar no Menu">
                                        <ItemStyle Width="7%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sMenu" HeaderText="É Menu?">
                                        <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                    <asp:BoundField DataField="sIcone" HeaderText="Ícone">
                                        <ItemStyle Width="3%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                    </asp:BoundField>

                                </Columns>
                            </asp:GridView>
                        </div>
                    </div>
                </div>
            </asp:Panel>
        </div>

        <div class="modal fade" id="modalRecursos">
            <div class="modal-dialog" style="width: 65%;">
                <div class="modal-content">
                    <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal">&times;</button>
                        <h3 class="modal-title">Permissões Filhas</h3>
                    </div>
                    <div class="modal-body">

                        <label class="pai"></label>

                        <div style="padding-bottom: 10px;">
                            <div class="tabela form-group"></div>
                        </div>

                    </div>
                </div>
            </div>
        </div>

    </div>

</asp:Content>