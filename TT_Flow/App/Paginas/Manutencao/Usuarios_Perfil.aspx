<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Usuarios_Perfil.aspx.cs" Inherits="TT_Hub.App.Paginas.Manutencao.Usuarios_Perfil" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="updPerfil" runat="server">
        <ContentTemplate>

            <script type="text/javascript">
                function MudarBotao() {
                    document.getElementById("cphCorpo_cmdPesquisar").value = "Pesquisando..";
                }
            </script>

            <div class="form-stacked row">
                <div class="col-lg-12">
                    <h1>
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label><small> Manutenção</small></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">
                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">

                            <div class="form-group row">
                                <div class="col-lg-4">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-lg-4">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:HyperLink ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" NavigateUrl="/App/Paginas/Manutencao/Usuarios_Perfil_Detalhe.aspx?id=0" Target="_blank"></asp:HyperLink>
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
                                            ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small" OnRowCommand="dtgvConsulta_RowCommand">
                                            <Columns>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPerfil"
                                                    DataTextField="idPerfil" HeaderText="ID"
                                                    DataNavigateUrlFormatString="Usuarios_Perfil_Detalhe.aspx?id={0}">
                                                    <ItemStyle Width="3%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPerfil"
                                                    DataTextField="sDscPerfil" HeaderText="Nome do Perfil"
                                                    DataNavigateUrlFormatString="Usuarios_Perfil_Detalhe.aspx?id={0}">
                                                    <ItemStyle Width="50%" HorizontalAlign="Left" VerticalAlign="Middle" CssClass="sDsc" />
                                                </asp:HyperLinkField>

                                                <asp:HyperLinkField DataNavigateUrlFields="idPerfil"
                                                    DataTextField="sDscTipoPerfil" HeaderText="Tipo do Perfil"
                                                    DataNavigateUrlFormatString="Usuarios_Perfil_Detalhe.aspx?id={0}">
                                                    <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                                </asp:HyperLinkField>

                                                <asp:TemplateField HeaderText="Usuários">
                                                    <ItemTemplate>
                                                        <asp:LinkButton runat="server" ID="cmdUsuarios" CommandName='<%# Eval("idPerfil") %>' CommandArgument='<%# Eval("sDscPerfil") %>' Text='<%# Eval("nQtdUsuarios") %>'></asp:LinkButton>
                                                    </ItemTemplate>
                                                    <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" CssClass="usuarios" />
                                                </asp:TemplateField>

                                                <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
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

                    <asp:Panel ID="pnMensagem" CssClass="row" runat="server">
                        <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
                    </asp:Panel>

                </div>
            </div>

            <div class="modal fade" id="modal">
                <div class="modal-dialog" style="width: 45%;">
                    <div class="modal-content" style="overflow: visible;">
                        <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal">&times;</button>
                            <h3 runat="server" id="modalTitle" class="modal-title"></h3>
                        </div>
                        <div class="modal-body">

                            <div class="panel panel-primary">
                                <div class="panel-body table-responsive">
                                    <asp:GridView ID="gvUsuarios" class="table table-striped table-hover dataTable no-footer"
                                        runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                        ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                        <Columns>

                                            <asp:HyperLinkField DataNavigateUrlFields="idUsuario"
                                                DataTextField="idUsuario" HeaderText="ID" Target="_blank"
                                                DataNavigateUrlFormatString="/App/Paginas/Manutencao/Usuarios_Detalhe.aspx?idu={0}">
                                                <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:HyperLinkField DataNavigateUrlFields="idUsuario"
                                                DataTextField="sDscUsuario" HeaderText="Usuário" Target="_blank"
                                                DataNavigateUrlFormatString="/App/Paginas/Manutencao/Usuarios_Detalhe.aspx?idu={0}">
                                                <ItemStyle Width="50%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:HyperLinkField>

                                            <asp:BoundField DataField="sLogin" HeaderText="Login">
                                                <ItemStyle Width="20%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                            <asp:BoundField DataField="sDscTipo" HeaderText="Tipo">
                                                <ItemStyle Width="25%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                            </asp:BoundField>

                                        </Columns>
                                    </asp:GridView>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>
