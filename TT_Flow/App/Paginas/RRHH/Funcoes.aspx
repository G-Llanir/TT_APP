<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Funcoes.aspx.cs" Inherits="TT_Hub.App.Paginas.RRHH.Funcoes" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

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
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label>
                        <small>
                            <asp:Label ID="lblSubTituloPagina" runat="server" Text=" Manutenção"></asp:Label></small>
                    </h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12" runat="server" id="DIV_Filtro">

                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i>Filtro para pesquisa</h3>
                        </div>
                        <div class="panel-body">

                            <%-- PRIMEIRA LINHA DE FILTROS --%>
                            <div class="row">
                                <div class="col-lg-4 form-group">
                                    <asp:DropDownList ID="ddlidEmpresa" runat="server" AutoPostBack="true" OnSelectedIndexChanged="ddlidEmpresa_SelectedIndexChanged" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>
                                <div class="col-lg-4 form-group">
                                    <asp:DropDownList ID="ddlidDepartamento" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                </div>
                                <div class="col-lg-4 form-group">
                                    <asp:DropDownList ID="ddlidGHE" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                            </div>

                            <%-- SEGUNDA LINHA DE FILTROS --%>
                            <div class="row">
                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlidTipoContrato" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlsCBO" class="form-control Caixa_Selecao" runat="server"></asp:DropDownList>
                                </div>
                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlTipoFuncao" class="form-control Caixa_Selecao" runat="server">
                                        <asp:ListItem Value="0">Todos os Tipos</asp:ListItem>
                                        <asp:ListItem Value="1">Administrativo</asp:ListItem>
                                        <asp:ListItem Value="2">Operacional</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <%-- TERCEIRA LINHA DE FILTROS --%>
                            <div class="row">
                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlDuplaFuncao" class="form-control Caixa_Selecao" runat="server">
                                        <asp:ListItem Value="T">Dupla Função (Ambos)</asp:ListItem>
                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlPericulosidade" class="form-control Caixa_Selecao" runat="server">
                                        <asp:ListItem Value="T">Periculosidade (Ambos)</asp:ListItem>
                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-3 form-group">
                                    <asp:DropDownList ID="ddlNoturnidade" class="form-control Caixa_Selecao" runat="server">
                                        <asp:ListItem Value="T">Noturnidade (Ambos)</asp:ListItem>
                                        <asp:ListItem Value="S">Sim</asp:ListItem>
                                        <asp:ListItem Value="N">Não</asp:ListItem>
                                    </asp:DropDownList>
                                </div>

                                <div class="col-lg-4 form-group">
                                    <asp:TextBox ID="txtPesquisa" class="form-control" placeholder="Pesquisar por nome da função" runat="server"></asp:TextBox>
                                </div>
                                <div class="col-lg-2 form-group">
                                    <asp:Button ID="cmdPesquisar" class="btn btn-primary" runat="server" Text="Buscar" OnClick="cmdPesquisar_Click" OnClientClick="MudarBotao()" />
                                    <asp:Button ID="cmdNovo" class="btn btn-success" runat="server" Text="Novo" OnClick="cmdNovo_Click" />
                                </div>
                            </div>

                        </div>

                    </div>
                </div>
            </div>

            <asp:Panel ID="pnResultado" class="" runat="server">
                <div id="resultado">
                    <div class="panel panel-primary">
                        <div class="panel-body">
                            <div class="table-responsive">
                                <asp:GridView ID="dtgvConsulta" class="table table-striped table-bordered table-hover "
                                    runat="server" Width="100%" CellSpacing="1" CellPadding="1" AutoGenerateColumns="False" GridLines="None"
                                    ShowFooter="False" Font-Names="Tahoma" Font-Overline="False" Font-Size="Small">
                                    <Columns>
                                        <asp:HyperLinkField DataNavigateUrlFields="idFuncao"
                                            DataTextField="idFuncao" HeaderText="ID"
                                            DataNavigateUrlFormatString="Funcoes_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="5%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idFuncao"
                                            DataTextField="sDscFuncao" HeaderText="Função"
                                            DataNavigateUrlFormatString="Funcoes_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="25%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="sDscEmpresa" HeaderText="Empresa">
                                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <ItemStyle Width="8%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscGHE_Completo" HeaderText="GHE/Setor">
                                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <ItemStyle Width="15%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscDepartamento" HeaderText="Departamento">
                                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <ItemStyle Width="12%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sCBO" HeaderText="CBO">
                                            <HeaderStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                            <ItemStyle Width="7%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="nRemuneracaoBase" HeaderText="Salário">
                                            <HeaderStyle HorizontalAlign="Right" VerticalAlign="Middle" />
                                            <ItemStyle Width="8%" HorizontalAlign="Right" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <%-- NOVAS COLUNAS ADICIONADAS --%>
                                        <asp:BoundField DataField="sDscTipoFuncao" HeaderText="Tipo">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            <ItemStyle Width="6%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="sDscDuplaFuncao" HeaderText="Dupla F.">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>
                                        <asp:BoundField DataField="sDscPericulosidade" HeaderText="Peric.">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idFuncao,sDscFuncao"
                                            DataTextField="nQtdFuncionarios" HeaderText="Qtd Colab."
                                            DataNavigateUrlFormatString="Colaboradores.aspx?idFuncao={0}">
                                            <HeaderStyle Width="5%" />
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:BoundField DataField="sAtivo_Completa" HeaderText="Ativo">
                                            <HeaderStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
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
                    </div>
                </div>
            </asp:Panel>

            <asp:Panel ID="pnMensagem" CssClass="row" runat="server">
                <uc1:MensagemPagina runat="server" ID="MensagemPagina" />
            </asp:Panel>

            </div>
        </div>
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
