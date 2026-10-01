<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Usuarios.aspx.cs" Inherits="TT_Flow.App.Paginas.Manutencao.Usuarios" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server"></asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">

    <asp:UpdatePanel ID="UpdatePanel1" runat="server">
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

                    <div class="panel panel-primary" id="div_Selecao" runat="server">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">
                            <div class="form-group">

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar" runat="server"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-3">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlPerfil" runat="server" class="form-control Caixa_Selecao" OnSelectedIndexChanged="cmdPesquisar_Click" AutoPostBack="True"></asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlSituacao" runat="server" class="form-control Caixa_Selecao" OnSelectedIndexChanged="cmdPesquisar_Click" AutoPostBack="True">
                                            <asp:ListItem Value="T">Todos</asp:ListItem>
                                            <asp:ListItem Selected="True" Value="S">Ativos</asp:ListItem>
                                            <asp:ListItem Value="N">Inativos</asp:ListItem>
                                            <asp:ListItem Value="E">Excluidos</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlsTipo" runat="server" class="form-control Caixa_Selecao" OnSelectedIndexChanged="cmdPesquisar_Click" AutoPostBack="True">
                                            <asp:ListItem Value="T">Todos</asp:ListItem>
                                            <asp:ListItem Selected="True" Value="I">Usuário Interno</asp:ListItem>
                                            <asp:ListItem Value="A">Administradores</asp:ListItem>
                                            <asp:ListItem Value="C">Usuário Externo (HUB)</asp:ListItem>
                                            <asp:ListItem Value="P">Usuário Externo (Parceiros)</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>

                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                        <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                                    </div>
                                </div>

                            </div>
                        </div>
                    </div>
                </div>


                <div class="col-lg-12">

                    <asp:Panel ID="pnResultado" class="" runat="server">
                        <div class="panel panel-primary">
                            <div class="panel-body">
                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                    <Columns>

                                        <asp:HyperLinkField DataNavigateUrlFields="idUsuario"
                                            DataTextField="idUsuario" HeaderText="ID"
                                            DataNavigateUrlFormatString="Usuarios_Detalhe.aspx?idu={0}">
                                            <HeaderStyle Width="3%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idUsuario"
                                            DataTextField="sDscUsuario" HeaderText="Nome"
                                            DataNavigateUrlFormatString="Usuarios_Detalhe.aspx?idu={0}">
                                            <HeaderStyle Width="25%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="sLogin" HeaderText="Login">
                                            <HeaderStyle HorizontalAlign="left" VerticalAlign="Middle" />
                                            <ItemStyle Width="10%" HorizontalAlign="left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDScTipo" HeaderText="Tipo">
                                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <%--<asp:BoundField DataField="sDscPerfil" HeaderText="Perfil de Acesso">
                                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <ItemStyle Width="30%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>--%>

                                        <asp:HyperLinkField DataNavigateUrlFields="idPerfil"
                                            DataTextField="sDscPerfil" HeaderText="Perfil de Acesso"
                                            DataNavigateUrlFormatString="/App/Paginas/Manutencao/Usuarios_Perfil_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="30%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="sSituacao_Completa" HeaderText="Ativo">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>


                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Data Atualização">
                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                    </Columns>
                                    <FooterStyle CssClass="TAB_Fundo_Azul" />
                                    <RowStyle CssClass="texto_padrao_preto" />
                                    <PagerStyle BackColor="Black" ForeColor="Black" HorizontalAlign="Center" />
                                    <HeaderStyle CssClass="TAB_Fundo_Azul" />
                                    <EditRowStyle BackColor="#2461BF" />

                                </asp:GridView>
                            </div>
                        </div>
                    </asp:Panel>

                    <asp:Panel ID="pnMensagem" runat="server">
                        <asp:Label ID="lblMensagem" class="warning" runat="server" Text="Label"></asp:Label>
                    </asp:Panel>

                </div>

            </div>

        </ContentTemplate>
    </asp:UpdatePanel>

</asp:Content>