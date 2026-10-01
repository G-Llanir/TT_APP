<%@ Page Title="" Language="C#" MasterPageFile="~/App/main.master" AutoEventWireup="true" CodeBehind="Avaliacao.aspx.cs" Inherits="TT_Hub.App.Paginas.RRHH.Avaliacao" %>

<%@ Register Src="~/App/Controles/BreadCrumb.ascx" TagPrefix="uc1" TagName="BreadCrumb" %>
<%@ Register Src="~/App/Controles/MensagemPagina.ascx" TagPrefix="uc1" TagName="MensagemPagina" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="cphCorpo" runat="server">
    <style>
    .tag-list {
        width: 100%;
        flex-wrap: wrap;
        display: flex;
        overflow: hidden;
    }

    .tag-item {
        background: green;
        padding: 2px 4px;
        font-size: 12px;
       
        margin: 3px;
        display: inline-flex;
        flex-wrap: nowrap;
        border-radius: 4px;
    }
    </style>

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
                        <asp:Label ID="lblTituloPagina" runat="server" Text="TITULO_PAGINA"></asp:Label></h1>
                    <uc1:BreadCrumb runat="server" ID="BreadCrumb_Pagina" NivelPagina="2" TitulodaPagina="" />
                </div>
                <div class="col-lg-12">

                    <div class="panel panel-primary">
                        <div class="panel-heading">
                            <h3 class="panel-title"><i class="fa fa-bar-chart-o"></i> Filtro para pesquisa </h3>
                        </div>
                        <div class="panel-body">

                            <div class="row">
                                <div class="col-lg-2">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtsReferencia" class="form-control" runat="server" MaxLength="7" placeholder="MM/AAAA"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:DropDownList ID="ddlsidDepartamento" runat="server" class="form-control Caixa_Selecao"></asp:DropDownList>
                                    </div>
                                </div>

                     

                                <div class="col-lg-4">
                                    <div class="form-group">
                                        <asp:TextBox ID="txtPesquisa" class="form-control " placeholder="Pesquisar" runat="server" MaxLength="200"></asp:TextBox>
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

                                        <asp:HyperLinkField DataNavigateUrlFields="idAvaliacao"
                                            DataTextField="idAvaliacao" HeaderText="ID"
                                            DataNavigateUrlFormatString="Avaliacao_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="3%" />
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idAvaliacao"
                                            DataTextField="sReferencia" HeaderText="Referência"
                                            DataNavigateUrlFormatString="Avaliacao_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="8%" />
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idAvaliacao"
                                            DataTextField="sDscAvaliacao" HeaderText="Descrição"
                                            DataNavigateUrlFormatString="Avaliacao_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="30%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        <asp:HyperLinkField DataNavigateUrlFields="idAvaliacao"
                                            DataTextField="sDscDepartamento" HeaderText="Departamento(s)"
                                            DataNavigateUrlFormatString="Avaliacao_Detalhe.aspx?id={0}">
                                            <HeaderStyle Width="25%" />
                                            <ItemStyle HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>

                                        
                                        <asp:HyperLinkField DataNavigateUrlFields="idAvaliacao"
                                            DataTextField="nQtdRespostas" HeaderText="Qtd Aplicada"
                                            DataNavigateUrlFormatString="Avaliacao_Detalhe.aspx?id={0}&sTipo=Resultados">
                                            <HeaderStyle Width="5%" />
                                            <ItemStyle HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:HyperLinkField>
                                         
                                       <%-- <asp:TemplateField HeaderText="Resultados">
                                            <ItemTemplate>
                                                <asp:LinkButton ID="lnkDuplicar" runat="server" CssClass="btn btn-small" TabIndex="100" CommandName="Resultados" CommandArgument='<%# Eval("idAvaliacao") %>'><i class="fa fa-files-o"></i></asp:LinkButton>
                                            </ItemTemplate>
                                            <ItemStyle Width="5%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:TemplateField>--%>

                                        <asp:BoundField DataField="dtAtualizacao" HeaderText="Atualizado em">
                                            <ItemStyle Width="10%" HorizontalAlign="Center" VerticalAlign="Middle" />
                                        </asp:BoundField>

                                        <asp:BoundField DataField="sDscUsuario" HeaderText="Por">
                                            <ItemStyle Width="10%" HorizontalAlign="Left" VerticalAlign="Middle" />
                                        </asp:BoundField>

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
